using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using System;
using WoW.Client.Components;
using WoW.Client.Scenes;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Realm;

namespace WoW.Client
{
    public enum GameNetworkState
    {
        Offline,
        Auth_LoggingIn,
        Auth_Realmlist,
        Realm,
        Realm_Characters,
        World
    }

    public class Game1 : Core
    {
        public static GameNetworkState NetState = GameNetworkState.Offline;
        public static NetManager ClientNetwork;
        public static EventBasedNetListener ClientListener;
        private static NetPacketProcessor _netProcessor;

        public static string AccountName;
        public static string SessionId;
        public static Realmserver LastRealm;

        public Game1() : base(windowTitle: "WoW Pixel Project", width: 800, height: 600)
        {
            IsMouseVisible = true;
            PauseOnFocusLost = false;
        }

        protected override void Initialize()
        {
            // todo: network stuff init'd here.
            ClientListener = new EventBasedNetListener();
            ClientListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader);
            ClientListener.PeerConnectedEvent += (peer) =>
            {
                if (NetState == GameNetworkState.Offline)
                    NetState = GameNetworkState.Auth_LoggingIn;

                if (NetState == GameNetworkState.Auth_LoggingIn)
                    Send(new ClientAuth_Logon() { AccountName = AccountName });

                if (NetState == GameNetworkState.Realm)
                {
                    Send(new ClientRealm_TransferLogon() { SessionId = SessionId });
                }
            };

            _netProcessor = new NetPacketProcessor();

            _netProcessor.SubscribeReusable<RealmClient_EntityCreate>((newCreate) =>
            {
                var netTestScene = Scene as NetworkTestScene;
                netTestScene.CreateEntity(newCreate);
            });

            _netProcessor.SubscribeReusable<RealmClient_Connect>((newLogin) =>
            {
                var netTestScene = Scene as NetworkTestScene;
                netTestScene.CreatePlayer(newLogin);
            });

            _netProcessor.SubscribeReusable<RealmClient_NetPositionInputUpdate>((positionUpdate) =>
            {
                var netTestScene = Scene as NetworkTestScene;
                netTestScene.UpdatePlayerPosition(positionUpdate);
            });

            _netProcessor.SubscribeReusable<RealmClient_Chat>((newChat) =>
            {
                var netTestScene = Scene as NetworkTestScene;

                var guiEntity = netTestScene.FindEntity("gui");
                if (guiEntity == null)
                {
                    Debug.Error("Client GUI entity is null!");
                    return;
                }

                Entity playerById;
                if (!string.Equals(SessionId, newChat.Id))
                {
                    playerById = netTestScene.FindEntity(newChat.Id);
                    if (playerById == null)
                    {
                        Debug.Error($"No player exists with the given id: {newChat.Id}.");
                        return;
                    }

                    // todo: fix chat message formatting if its our player's message.
                    var netPlayerController = playerById.GetComponent<NetPlayerController>();
                    newChat.Message = $"[{netPlayerController.Username}] {newChat.Message}";

                    var guiController = guiEntity.GetComponent<ImGuiController>();
                    guiController.Chat.Add(newChat.Message);
                }
                else
                    playerById = netTestScene.FindEntity("player");
            });

            _netProcessor.SubscribeReusable<RealmClient_Disconnect>((newDisconenct) =>
            {
                var netTestScene = Scene as NetworkTestScene;

                if (newDisconenct.Code == DisconnectCode.Timeout)
                {
                    var entity = netTestScene.FindEntity(newDisconenct.Id);
                    var netController = entity.GetComponent<NetPlayerController>();

                    Debug.Log($"{entity.Name} has disconnected; deleting entity...");

                    entity.Destroy();
                }
            });

            _netProcessor.SubscribeReusable<AuthClient_LogonCode>((logonCode) =>
            {
                // todo: handle logon code.
            });

            _netProcessor.SubscribeReusable<AuthClient_Logon>((logon) =>
            {
                SessionId = logon.SessionId;
                // server sends the realmlist automatically.

                // do we need to send a realmlist req. packet here or should we just have the server send it?
            });

            _netProcessor.SubscribeReusable<AuthClient_Realmserver>((realmlist) =>
            {
                Debug.Log($"Received realmlist: {realmlist.Name} - {realmlist.Ip}:{realmlist.Port}");

                LogonScene scene = Core.Scene as LogonScene;
                var gui = scene.FindEntity("gui").GetComponent<ImGuiController>();

                gui.Realmlist.Add(new Realmserver(realmlist.Name, realmlist.Ip, realmlist.Port));
                NetState = GameNetworkState.Auth_Realmlist;
            });

            _netProcessor.SubscribeNetSerializable<RealmClient_CharacterList>((characterList) =>
            {
                Debug.Log($"Received {characterList.Characters.Count} characters.");

                var gui = (Core.Scene as LogonScene).FindEntity("gui").GetComponent<ImGuiController>();
                gui.Characters.AddRange(characterList.Characters);
                NetState = GameNetworkState.Realm_Characters;
            });

            ClientNetwork = new NetManager(ClientListener);
            ClientNetwork.Start();

            base.Initialize();

            var guiManager = new ImGuiManager()
            {
                ShowCoreWindow = false,
                ShowDemoWindow = false,
                ShowSceneGraphWindow = false,
                ShowSeperateGameWindow = false,
                ShowMenuBar = false,
                ShowStyleEditor = false,
            };
            Core.RegisterGlobalManager(guiManager);

            Scene = new LogonScene();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            ClientNetwork.PollEvents();
        }

        public static void Send<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(ClientNetwork, packet, delivery);
    }
}
