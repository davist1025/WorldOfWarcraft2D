using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using Nez.Tiled;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using WoW.Client.Components;
using WoW.Client.Content;
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
        Auth_Banned,
        Auth_Invalid,
        Auth_IsOnline,
        Auth_Realmlist,
        Realm,
        Realm_Characters,
        Realm_CreateCharacter,
        Realm_CharacterNameInvalid,
        LoadingWorld,
        World
    }

    public class Game1 : Core
    {
        public static GameNetworkState NetState = GameNetworkState.Offline;
        public static NetManager ClientNetwork;
        public static EventBasedNetListener ClientListener;
        private static NetPacketProcessor _netProcessor;

        public static TmxMap[] Maps;
        public static GameConfiguration Configuration;
        public static Dictionary<string, Texture2D> InterfaceTextures;

        public static string AccountName;
        public static string SessionId;
        public static RemoteRealmserver LastRealm; // todo: save to disk.
        public static NetworkTestScene NetworkScene;
        public static string CurrentMapId = "";

        public static bool ShouldShowEscapeMenu = false;
        public static bool ShouldShowGMChat = false;
        public static bool ShouldShowWhoMenu = false;

        public static Entity Player;
        private static ClientAuth_Logon _temporaryLogonPacket;

        public static float MovementSpeed = 1f;

        public Game1() : base(windowTitle: "WoW Pixel Project", width: 800, height: 600)
        {
            IsMouseVisible = true;
            PauseOnFocusLost = false;
        }

        protected override void Initialize()
        {
            ClientListener = new EventBasedNetListener();
            ClientListener.NetworkReceiveEvent += (peer, reader, method) => _netProcessor.ReadAllPackets(reader);
            ClientListener.PeerConnectedEvent += (peer) =>
            {
                if (NetState == GameNetworkState.Offline)
                    NetState = GameNetworkState.Auth_LoggingIn;

                if (NetState == GameNetworkState.Auth_LoggingIn)
                {
                    Send(_temporaryLogonPacket);
                }

                if (NetState == GameNetworkState.Realm)
                    Send(new ClientRealm_TransferLogon() { SessionId = SessionId });
            };

            /*
             * NetState can be changed to display whichever UI is necessary, per ImGUI.
             * We need to set ClientNetwork's connection state at the same time we set this so the packet flow doesn't crash the client or server.
             */

            _netProcessor = new NetPacketProcessor();

            _netProcessor.RegisterNestedType<Vector2Serializable>();
            _netProcessor.SubscribeReusable<RealmClient_Disconnect>((newDisconenct) => PacketManager.OnPlayerDisconnect(newDisconenct));

            _netProcessor.SubscribeReusable<AuthClient_LogonCode>((logonCode) => PacketManager.OnLogonResponse(logonCode));

            _netProcessor.SubscribeReusable<AuthClient_Logon>((logon) => PacketManager.OnLogonSuccess(logon));

            _netProcessor.SubscribeNetSerializable<AuthClient_Realm>((realmlist) => PacketManager.OnRealmlist(realmlist));

            _netProcessor.SubscribeReusable<RealmClient_CreateCharacter>((response) => PacketManager.OnCreateCharacter(response));

            _netProcessor.SubscribeNetSerializable<RealmClient_PlayerCharacters>((characters) => PacketManager.OnCharacterList(characters));

            _netProcessor.SubscribeReusable<RealmClient_CreateLocalPlayer>((thePlayer) => PacketManager.OnLocalPlayer(thePlayer));

            _netProcessor.SubscribeReusable<RealmClient_MovementStateValidation>((result) =>
            {
                var controller = Scene.FindComponentOfType<LocalPlayerController>();
                controller.LastServerCalculation = result.ServerCalculation.ToVector2XNA();
                controller.LastServerCalculation = new Vector2(controller.LastServerCalculation.X - 32f, controller.LastServerCalculation.Y - 16f);

                controller.ProcessInputValidation(result);
                // todo: correction from server.
            });

            _netProcessor.SubscribeReusable<RealmClient_CreateNetPlayer>((newPlayer) => PacketManager.OnNetworkPlayer(newPlayer));

            // mostly an empty packet. open to suggestions or later implementation :P
            _netProcessor.SubscribeReusable<RealmClient_EnterWorld>((worldParams) => PacketManager.OnEnterWorld(worldParams));

            _netProcessor.SubscribeReusable<RealmClient_MovementStateChange>((serverNetUpdate) => PacketManager.OnPlayerPositionUpdate(serverNetUpdate));

            _netProcessor.SubscribeNetSerializable<RealmClient_CreateNPC>((newNpc) => PacketManager.OnNPC(newNpc));

            _netProcessor.SubscribeReusable<RealmClient_WhoCommand>((whoList) => PacketManager.OnWho(whoList));

            _netProcessor.SubscribeReusable<RealmClient_SetTarget>((target) => PacketManager.OnSetTarget(target));

            _netProcessor.SubscribeReusable<RealmClient_Teleport>((teleport) => PacketManager.OnTeleport(teleport));

            _netProcessor.SubscribeReusable<ChatMessage>((newChat) => PacketManager.OnChat(newChat));

            ClientNetwork = new NetManager(ClientListener);
            ClientNetwork.Start();

            Configuration = GameConfiguration.Load();

            base.Initialize();

            IsFixedTimeStep = true;
            ExitOnEscapeKeypress = false;

            Debug.Log($"Loading Tiled maps...");
            var tmxFiles = Directory.GetFiles("Content/Data/").Where(f => f.EndsWith(".tmx")).ToArray();
            Maps = new TmxMap[tmxFiles.Length];
            for (int i = 0; i < tmxFiles.Length; i++)
            {
                var mapFile = tmxFiles[i];
                Maps[i] = Core.Content.LoadTiledMap(mapFile);
                Debug.Log($"Loaded {Maps[i].Properties["id"]}");
            }

            NetworkScene = new NetworkTestScene();
            var guiManager = new ImGuiManager()
            {
                ShowCoreWindow = false,
                ShowDemoWindow = false,
                ShowSceneGraphWindow = true,
                ShowSeperateGameWindow = false,
                ShowMenuBar = false,
                ShowStyleEditor = false,
            };
            Core.RegisterGlobalManager(guiManager);

            InterfaceTextures = new Dictionary<string, Texture2D>()
            {
                { "gear_icon", Core.Content.LoadTexture("Content/Data/UI/gear_img.png") },
                { "hand1_mouse", Core.Content.LoadTexture("Content/Data/UI/hand1_mouse.png") },
                { "merchant_bag_icon", Core.Content.LoadTexture("Content/Data/UI/merchant_loot_bag_img.png") }
            };
            Mouse.SetCursor(MouseCursor.FromTexture2D(InterfaceTextures["hand1_mouse"], 0, 0));

            Scene = new LogonScene();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ClientNetwork.PollEvents();
        }

        public static void ConnectAndLogin(string accountName, string password)
        {
            // todo: grab auth ip/port from config.
            ClientNetwork.Connect("127.0.0.1", 8070, "");

            _temporaryLogonPacket = new ClientAuth_Logon()
            {
                AccountName = accountName,
                Password = Shared.Utils.ToSHA256(password)
            };
        }

        public static void Send<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : class, new()
            => _netProcessor.Send(ClientNetwork, packet, delivery);

        public static void SendSerializable<T>(T packet, DeliveryMethod delivery = DeliveryMethod.ReliableOrdered) where T : INetSerializable
        {
            _netProcessor.SendNetSerializable(ClientNetwork, packet, delivery);
        }

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

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Configuration.Save();
        }
    }
}
