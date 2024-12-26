using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using System;
using System.Collections;
using System.Collections.Generic;
using WoW.Client.Components;
using WoW.Client.Scenes;
using WoW.Client.Shared;
using WoW.Client.Shared.Auth;
using WoW.Client.Shared.Client;
using WoW.Client.Shared.Data;
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
        Realm_CreateCharacter,
        LoadingWorld,
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
        public static RemoteRealmserver LastRealm; // todo: save to disk.
        public static NetworkTestScene NetworkScene;

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
                    Send(new ClientRealm_TransferLogon() { SessionId = SessionId });
            };

            /*
             * NetState can be changed to display whichever UI is necessary, per ImGUI.
             * We need to set ClientNetwork's connection state at the same time we set this so the packet flow doesn't crash the client or server.
             */


            //ClientListener.PeerDisconnectedEvent += (peer, reason) =>
            //{
            //    NetState = GameNetworkState.Offline;
            //};

            _netProcessor = new NetPacketProcessor();

            //_netProcessor.SubscribeReusable<RealmClient_CreateGameObject>((newCreate) =>
            //{
            //    if (_entityQueue.Find(e => e.Id == newCreate.Id) == null)
            //        _entityQueue.Add(newCreate);
            //});

            //_netProcessor.SubscribeReusable<RealmClient_CreateNetObject>((newCreate) =>
            //{
            //    var entityToCreateWithId = _entityQueue.Find(e => e.Id.Equals(newCreate.Id, StringComparison.OrdinalIgnoreCase));
            //    if (entityToCreateWithId != null)
            //    {
            //        Entity entity = new Entity(newCreate.Id);

            //        // todo: what type of component is needed for a network creature?
            //        //entity.AddComponent(new NetPlayerController(newLogin.PlayerCharacter));
            //        entity.SetPosition(new Vector2(entityToCreateWithId.X, entityToCreateWithId.Y));
            //        EntityQueue.Enqueue(entity);
            //    }
            //});

            //_netProcessor.SubscribeNetSerializable<RealmClient_CreateNetPlayer>((newLogin) =>
            //{
            //    var entityToCreateWithId = _entityQueue.Find(e => e.Id.Equals(newLogin.Id, StringComparison.OrdinalIgnoreCase));
            //    if (entityToCreateWithId != null)
            //    {
            //        Entity entity = new Entity(newLogin.Id);

            //        Debug.WarnIf(newLogin.PlayerCharacter == null, "The new client's PlayerCharacter is null in the packet!"); // this shouldn't be possible but we'll keep the check for now.
            //        Debug.LogIf(newLogin.PlayerCharacter != null, $"{newLogin.Id} is playing character: {newLogin.PlayerCharacter.Name}");

            //        entity.AddComponent(new NetPlayerController(newLogin.PlayerCharacter));
            //        entity.SetPosition(new Vector2(entityToCreateWithId.X, entityToCreateWithId.Y));
            //        EntityQueue.Enqueue(entity);
            //    }
            //});

            //_netProcessor.SubscribeReusable<RealmClient_NetPositionInputUpdate>((positionUpdate) =>
            //{
            //    var netTestScene = Scene as NetworkTestScene;
            //    netTestScene.UpdatePlayerPosition(positionUpdate);
            //});

            //_netProcessor.SubscribeReusable<RealmClient_Chat>((newChat) =>
            //{
            //    var netTestScene = Scene as NetworkTestScene;

            //    var guiEntity = netTestScene.FindEntity("gui");
            //    if (guiEntity == null)
            //    {
            //        Debug.Error("Client GUI entity is null!");
            //        return;
            //    }

            //    var guiController = guiEntity.GetComponent<ImGuiController>();
            //    Entity playerById;
            //    string chatFormat = "";

            //    if (!string.Equals(SessionId, newChat.Id, StringComparison.OrdinalIgnoreCase))
            //    {
            //        playerById = netTestScene.FindEntity(newChat.Id);
            //        if (playerById == null)
            //        {
            //            Debug.Error($"No player exists with the given id: {newChat.Id}.");
            //            return;
            //        }

            //        var netPlayerController = playerById.GetComponent<NetPlayerController>();
            //        chatFormat = $"[{netPlayerController.Character.Name}] {newChat.Message}";
            //    }
            //    else
            //    {
            //        playerById = netTestScene.FindEntity("player");
            //        chatFormat = $"[{playerById.GetComponent<LocalPlayerController>().GetName()}] {newChat.Message}";
            //    }

            //    guiController.Chat.Add(chatFormat);
            //});

            //_netProcessor.SubscribeReusable<RealmClient_Disconnect>((newDisconenct) =>
            //{
            //    var netTestScene = Scene as NetworkTestScene;

            //    if (newDisconenct.Code == DisconnectCode.Timeout)
            //    {
            //        var entity = netTestScene.FindEntity(newDisconenct.Id);
            //        var netController = entity.GetComponent<NetPlayerController>();

            //        Debug.Log($"{entity.Name} has disconnected; deleting entity...");

            //        entity.Destroy();
            //    }
            //});

            _netProcessor.SubscribeReusable<AuthClient_LogonCode>((logonCode) =>
            {
                switch (logonCode.Code)
                {
                    case LogonCode.AlreadyOnline:
                        break;
                }
                // todo: handle logon code.
            });

            _netProcessor.SubscribeReusable<AuthClient_Logon>((logon) =>
            {
                SessionId = logon.SessionId;
                // server sends the realmlist automatically.
            });

            _netProcessor.SubscribeNetSerializable<AuthClient_Realm>((realmlist) =>
            {
                Debug.Log($"Received realms: {realmlist.Realmlist.Count}");

                LogonScene scene = Core.Scene as LogonScene;
                var gui = scene.FindEntity("gui").GetComponent<ImGuiController>();

                gui.Realmlist.AddRange(realmlist.Realmlist);
                NetState = GameNetworkState.Auth_Realmlist;
            });

            _netProcessor.SubscribeReusable<RealmClient_CreateCharacter>((response) =>
            {
                Debug.Log(response.CreationResult);
                // todo: return to character select, ask for character list.
            });

            _netProcessor.SubscribeNetSerializable<RealmClient_PlayerCharacters>((characters) =>
            {
                Debug.Log($"Received {characters.Characters.Count} characters.");

                var gui = (Core.Scene as LogonScene).FindEntity("gui").GetComponent<ImGuiController>();

                gui.Characters.Clear();

                gui.Characters.AddRange(characters.Characters);
                NetState = GameNetworkState.Realm_Characters;
            });

            _netProcessor.SubscribeReusable<RealmClient_CreateLocalPlayer>((thePlayer) =>
            {
                NetworkScene = new NetworkTestScene();
                NetworkScene.CreateLocalPlayer(thePlayer);
            });

            _netProcessor.SubscribeReusable<RealmClient_Debug_ServerPosition>((result) =>
            {
                var controller = Scene.FindEntity("thePlayer").GetComponent<LocalPlayerController>();
                controller.LastServerPosition = new Vector2(result.X, result.Y);
            });

            _netProcessor.SubscribeReusable<RealmClient_CreateNetPlayer>((newPlayer) =>
            {
                NetworkScene.CreateNetworkPlayer(newPlayer);
            });

            _netProcessor.SubscribeReusable<RealmClient_EnterWorld>((s) =>
            {
                Game1.NetState = GameNetworkState.World;
                Core.StartSceneTransition(new FadeTransition(() => NetworkScene));
            });

            _netProcessor.SubscribeReusable<RealmClient_NetPositionInputUpdate>((serverNetUpdate) =>
            {
                var netPlayer = Core.Scene.FindEntity(serverNetUpdate.Id);

                if (netPlayer != null)
                {
                    var controller = netPlayer.GetComponent<NetPlayerController>();
                    controller.MovementDirectionQueue.Enqueue(new Vector2(serverNetUpdate.MovementX, serverNetUpdate.MovementY));
                }
            });

            ClientNetwork = new NetManager(ClientListener);
            ClientNetwork.Start();

            base.Initialize();

            IsFixedTimeStep = true;

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

        /// <summary>
        /// Cleanly disconnects from the realmserver.
        /// </summary>
        public static void Disconnect()
        {
            var gui = Core.Scene.FindEntity("gui");
            var component = gui.GetComponent<ImGuiController>();
            component.Characters.Clear();
            component.Realmlist.Clear();
            ClientNetwork.DisconnectAll();
            NetState = GameNetworkState.Offline;
        }
    }
}
