using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Components.NPC;
using WoW.Client.Scenes;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
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
                case LogonCode.InvalidPassword:
                    Game1.NetState = GameNetworkState.Auth_Invalid;
                    break;
                case LogonCode.AlreadyOnline:
                    Game1.NetState = GameNetworkState.Auth_IsOnline;
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
            switch (response.CreationResult)
            {
                case RealmClient_CreateCharacter.Result.NameBanned:
                case RealmClient_CreateCharacter.Result.NameInUse:
                    Game1.NetState = GameNetworkState.Realm_CharacterNameInvalid;
                    break;
                case RealmClient_CreateCharacter.Result.Success:
                    Game1.Send(new ClientRealm_RequestCharacterList());
                    Game1.NetState = GameNetworkState.Realm;
                    break;
            }
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
            Game1.NetworkScene.CreateLocalPlayer(myPlayer);
        }

        public static void OnNetworkPlayer(RealmClient_CreateNetPlayer otherPlayer)
        {
            Game1.NetworkScene.CreateNetworkPlayer(otherPlayer);
        }

        public static void OnNPC(RealmClient_CreateNPC npc)
        {
            Game1.NetworkScene.CreateNPC(npc.Metadata);
        }

        public static void OnEnterWorld(RealmClient_EnterWorld worldParams)
        {
            // todo: there may be a few of these, organize them in a dictionary or some other object.
            Game1.MovementSpeed = worldParams.MovementSpeed;

            Game1.NetState = GameNetworkState.World;
            Core.StartSceneTransition(new FadeTransition(() => Game1.NetworkScene));

            var gui = Game1.NetworkScene.FindEntity("gui").GetComponent<ImGuiController>();

            var newChatStorage = new ChatMessage()
            {
                Message = worldParams.MOTD,
                Flags = ChatMessageFlag.IsServerMessage
            };
            gui.ChatHistory.Add(newChatStorage);
        }

        public static void OnPlayerPositionUpdate(RealmClient_MovementStateChange netUpdate)
        {
            var netPlayer = Core.Scene.FindEntity(netUpdate.Id);

            if (netPlayer != null)
            {
                if (netUpdate.IsTeleportUpdate)
                    netPlayer.SetPosition(netUpdate.ResultX, netUpdate.ResultY);

                if (netPlayer.HasComponent<NpcController>())
                    netPlayer.SetPosition(netUpdate.ResultX, netUpdate.ResultY);
                else if (netPlayer.HasComponent<NetPlayerController>())
                {
                    var controller = netPlayer.GetComponent<NetPlayerController>();

                    if (netUpdate.IsColliding)
                    {
                        var normal = netUpdate.ColliderNormal.ToVector2XNA();

                        if (normal.Y != 0f)
                            netUpdate.MovementY = 0f;

                        if (normal.X != 0f)
                            netUpdate.MovementX = 0f;
                    }

                    controller.MovementDirectionQueue.Enqueue(new Vector2(netUpdate.MovementX, netUpdate.MovementY));
                }
            }
        }

        public static void OnChat(ChatMessage newChat) 
        {
            var guiController = Game1.Scene.FindEntity("gui").GetComponent<ImGuiController>();

            var newChatStorage = new ChatMessage()
            {
                Message = newChat.Message,
                Flags = newChat.Flags
            };
            
            guiController.ChatHistory.Add(newChatStorage);
        }

        public static void OnWho(RealmClient_WhoCommand whoList)
        {
            var guiController = Game1.Scene.FindEntity("gui").GetComponent<ImGuiController>();
            guiController.OnlineCharacters = whoList.Characters;
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

        /// <summary>
        /// Sent only to players on the map this WorldId exists on.
        /// </summary>
        /// <param name="teleport"></param>
        public static void OnTeleport(RealmClient_Teleport teleport)
        {
            var entity = Core.Scene.FindEntity(teleport.WorldId);

            if (entity != null)
            {
                // Another player is being teleported.
                if (entity.HasComponent<NetPlayerController>())
                {
                    var controller = entity.GetComponent<NetPlayerController>();
                    controller.MapId = teleport.MapId;

                    if (Game1.CurrentMapId.ToLower().Equals(teleport.MapId))
                    {
                        controller.AddToMap();
                        controller.Entity.Position = new Vector2(teleport.X, teleport.Y);
                    }
                    else
                    {
                        // todo: move this code to the netplayercontroller
                        // i.e: "RemoveFromWorld()"
                        entity.RemoveComponent<SpriteRenderer>();
                        entity.RemoveComponent<CircleCollider>();
                        entity.RemoveComponent<Mover>();
                    }

                    Debug.Log($"{controller.Name} has been teleported.");
                }

                if (entity.HasComponent<LocalPlayerController>())
                {
                    Debug.Log("We are being teleported...");

                    // todo: bug may occur here where if we are summoned/teleported to the map we're already in, duplicate NPCs might be created.

                    Game1.CurrentMapId = teleport.MapId;

                    var newLoadTransition = new FadeTransition();
                    newLoadTransition.OnScreenObscured = () =>
                    {
                        // Find the map given by the MapId.
                        TmxMap tmxMapByMapId;
                        tmxMapByMapId = Game1.Maps.Where(map => map.Properties["id"].ToLower().Equals(teleport.MapId)).FirstOrDefault();
                        TiledMapRenderer mapRenderer = null;

                        // Destroy the current map renderer/entity.
                        Core.Scene.FindEntity("map").Destroy();
                        var npcEntities = Core.Scene.FindEntitiesWithTag((int)EntityType.NPC);

                        //for (int i = 0; i < npcEntities.Count; i++)
                        //    npcEntities[i].Destroy();

                        // Create a new map renderer.
                        mapRenderer = Core.Scene.CreateEntity("map").AddComponent(new TiledMapRenderer(tmxMapByMapId, "collision_layer"));
                        mapRenderer.RenderLayer = 10;

                        // Set our local posiiton.
                        entity.SetPosition(new Vector2(teleport.X, teleport.Y));

                        var netPlayersOnMap = Core.Scene
                            .FindComponentsOfType<NetPlayerController>()
                            .Where(player => player.MapId.ToLower().Equals(tmxMapByMapId.Properties["id"]))
                            .ToArray();

                        foreach (var player in netPlayersOnMap)
                            player.AddToMap();
                    };

                    newLoadTransition.OnTransitionCompleted += () =>
                    {
                        var allNpcs = Core.Scene.FindComponentsOfType<NpcController>().Where(npc => !npc.Metadata.MapId.ToLower().Equals(Game1.CurrentMapId.ToLower())).ToArray();

                        foreach (var npc in allNpcs)
                        {
                            Debug.Log($"Destroying NPC: {npc.Metadata.WorldId} from the previous map...");
                            npc.Entity.Destroy();
                        }
                    };
                    Core.StartSceneTransition(newLoadTransition);
                }
            }
        }
        #endregion

        public static void SendTabTargetRequest()
            => Game1.Send(new ClientRealm_TabTarget(), LiteNetLib.DeliveryMethod.ReliableUnordered);
    }
}
