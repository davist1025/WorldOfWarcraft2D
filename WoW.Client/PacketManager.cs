using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Scenes;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Realm;

namespace WoW.Client
{
    public static class PacketManager
    {
        #region Auth
        public static void OnLogonResponse(AuthClient_LogonCode code)
        {
            switch (code.Code)
            {
                case LogonCode.NoRecord:
                    Game1.NetState = GameNetworkState.Auth_Invalid;
                    break;
                case LogonCode.AlreadyOnline:
                    Game1.NetState = GameNetworkState.Auth_IsOnline;
                    break;
                case LogonCode.InvalidPassword:
                    Game1.NetState = GameNetworkState.Auth_Invalid;
                    break;
            }
        }

        public static void OnLogonSuccess(AuthClient_Logon session)
        {
            Game1.SessionId = session.SessionId;
        }

        public static void OnRealmlist(AuthClient_Realm realmlist)
        {
            Debug.Log($"Received realms: {realmlist.Realmlist.Count}");

            LogonScene scene = Core.Scene as LogonScene;
            var gui = scene.FindEntity("gui").GetComponent<ImGuiController>();

            gui.Realmlist.AddRange(realmlist.Realmlist);
            Game1.NetState = GameNetworkState.Auth_Realmlist;
        }
        #endregion

        #region Characters
        public static void OnCreateCharacter(RealmClient_CreateCharacter response)
        {
            Game1.Send(new ClientRealm_RequestCharacterList());
            Game1.NetState = GameNetworkState.Realm;
        }

        public static void OnCharacterList(RealmClient_PlayerCharacters characterList)
        {
            Debug.Log($"Received {characterList.Characters.Count} characters.");

            var gui = (Core.Scene as LogonScene).FindEntity("gui").GetComponent<ImGuiController>();

            gui.Characters.Clear();

            gui.Characters.AddRange(characterList.Characters);
            Game1.NetState = GameNetworkState.Realm_Characters;
        }
        #endregion

        #region World
        public static void OnLocalPlayer(RealmClient_CreateLocalPlayer myPlayer)
        {
            Game1.NetworkScene = new NetworkTestScene();
            Game1.NetworkScene.CreateLocalPlayer(myPlayer);
        }

        public static void OnNetworkPlayer(RealmClient_CreateNetPlayer otherPlayer)
        {
            Game1.NetworkScene.CreateNetworkPlayer(otherPlayer);
        }

        public static void OnNPC(RealmClient_CreateNPC npc)
        {
            Game1.NetworkScene.CreateNPC(npc.Data);
        }

        public static void OnEnterWorld(RealmClient_EnterWorld worldParams)
        {
            Game1.MovementSpeed = worldParams.MovementSpeed;

            Game1.NetState = GameNetworkState.World;
            Core.StartSceneTransition(new FadeTransition(() => Game1.NetworkScene));
        }

        public static void OnPlayerPositionUpdate(RealmClient_NetPositionInputUpdate netUpdate)
        {
            var netPlayer = Core.Scene.FindEntity(netUpdate.Id);

            if (netPlayer != null)
            {
                var controller = netPlayer.GetComponent<NetPlayerController>();
                controller.MovementDirectionQueue.Enqueue(new Vector2(netUpdate.MovementX, netUpdate.MovementY));
            }
        }

        public static void OnChat(RealmClient_Chat chat)
        {
            var guiEntity = Game1.NetworkScene.FindEntity("gui");
            if (guiEntity == null)
            {
                Debug.Error("Client GUI entity is null!");
                return;
            }

            // todo: fix chatting. broke after implementing tab targeting because i changed the player entity's name to "thePlayer".
            // this expects a character name.
            // expand this to allow NPCs, other players and the server.
            string chatFormat = "";
            var guiController = guiEntity.GetComponent<ImGuiController>();

            Debug.Log(chat.IsWhisper + " " + chat.FromWorldId);

            if (chat.IsWhisper) // mostly for formatting purposes.
            {
                var fromEntity = Core.Scene.FindEntity(chat.FromWorldId);
                var controller = fromEntity.GetComponent<NetPlayerController>();

                chatFormat = $"{controller.Name} says: {chat.Message}";
                guiController.Chat.Add(chatFormat);
            }

            if (!chat.IsWhisper)
            {
                if (chat.FromWorldId.Equals("server", StringComparison.OrdinalIgnoreCase))
                {
                    chatFormat = $"SERVER: {chat.Message}";
                    guiController.Chat.Add(chatFormat);
                }
                else
                {
                    Entity playerById = Game1.NetworkScene.FindEntity(chat.FromWorldId);

                    if (playerById != null)
                    {
                        NetPlayerController netController = null;
                        LocalPlayerController localController = null;

                        if ((EntityType)playerById.Tag != EntityType.LocalPlayer)
                            netController = playerById.GetComponent<NetPlayerController>();
                        else
                            localController = playerById.GetComponent<LocalPlayerController>();

                        chatFormat = $"{((netController != null) ? netController.Name : localController.Name)} says: {chat.Message}";
                        guiController.Chat.Add(chatFormat);
                    }
                    else
                        guiController.Chat.Add($"{chat.FromWorldId} cannot be found.");
                }
                
            }
        }

        public static void OnPlayerDisconnect(RealmClient_Disconnect disconnect)
        {
            if (disconnect.Code == DisconnectCode.Timeout)
            {
                var entity = Game1.NetworkScene.FindEntity(disconnect.Id);
                var netController = entity.GetComponent<NetPlayerController>();

                Debug.Log($"{entity.Name} has disconnected; deleting entity...");

                entity.Destroy();
            }
        }

        public static void OnSetTarget(RealmClient_SetTarget target)
        {
            var targetEntity = Game1.NetworkScene.FindEntity(target.WorldId);

            if (targetEntity != null)
            {
                var player = Game1.Player.GetComponent<LocalPlayerController>();
                player.TargetWorldId = target.WorldId;
                //switch ((EntityType)targetEntity.Tag)
                //{
                //    case EntityType.NPC:

                //        break;
                //}
            }

            //var npcs = Game1.NetworkScene.FindEntitiesWithTag((int)EntityType.NPC); // this should never be empty.
            //var matchingTarget = npcs.Find(npc => npc.GetComponent<NetNPCController>().Remote.WorldId.Equals(target.WorldId, StringComparison.OrdinalIgnoreCase));
            //var controller = matchingTarget.GetComponent<NetNPCController>();

            //if (matchingTarget != null)
            //{
            //    var player = Game1.Player.GetComponent<LocalPlayerController>();
            //    player.TargetWorldId = target.WorldId;
            //}
        }
        #endregion

        public static void SendTabTargetRequest()
            => Game1.Send(new ClientRealm_TabTarget(), LiteNetLib.DeliveryMethod.ReliableUnordered);
    }
}
