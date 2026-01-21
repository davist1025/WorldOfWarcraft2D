using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.ImGuiTools;
using Nez.Sprites;
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
using WoW.Framework;
using WoW.Network;
using WoW.Network.Objects;
using WoW.Network.Packets;
using WoW.Network.Packets.Authentication;
using WoW.Network.Packets.Authenticcation;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;

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
        public static GameConfiguration Config;
        public static NetworkController Network;
        public static GameNetworkState NetworkState = GameNetworkState.Offline;

        public static Entity Player;
        public static string AccountName { get; set; }
        public static string AccountSessionId { get; set; }
        public static string ActiveMapId { get; set; }
        public static float AssignedMovementSpeed = 1f;
        public static RealmserverMetadataObject LastConnectedRealm;

        public static TmxMap[] Maps;

        public static Dictionary<string, Texture2D> InterfaceTextures;

        public static NetworkTestScene NetworkScene;

        public static bool ShouldShowEscapeMenu = false;
        public static bool ShouldShowGMChat = false;
        public static bool ShouldShowWhoMenu = false;

        public Game1() : base(windowTitle: "WoW Pixel Project", width: 1080, height: 640)
        {
            IsMouseVisible = true;
            PauseOnFocusLost = false;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            Network = new NetworkController();
            Network.SubscribeFunction += SubscribeObjects;

            // dictates specifc activity that should occur upon a successful connection attempt to a given server.
            Network.OnPeerConnected += (sender, peer) =>
            {
                switch (NetworkState)
                {
                    case GameNetworkState.Auth_LoggingIn:
                        string[] loginInfo = Core.Scene.FindEntity("gui").GetComponent<ImGuiController>().GetLogin().Split(':');
                        Network.SendToServer(PacketManager.CreateLogonPacket(loginInfo[0], loginInfo[1]));
                        break;
                    case GameNetworkState.Realm:
                        Network.SendToServer(new ClientRealm_TransferLogon() { SessionId = AccountSessionId });
                        break;
                }
            };
            Network.StartClient();

            /*
             * NetState can be changed to display whichever UI is necessary, per ImGUI.
             * We need to set ClientNetwork's connection state at the same time we set this so the packet flow doesn't crash the client or server.
             */

            Config = GameConfiguration.Load();

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

            // todo: ew, clean this up.
            InterfaceTextures = new Dictionary<string, Texture2D>()
            {
                { "gear_icon", Core.Content.LoadTexture("Content/Data/UI/gear_img.png") },
                { "hand1_mouse", Core.Content.LoadTexture("Content/Data/UI/hand1_mouse.png") },
                { "merchant_bag_icon", Core.Content.LoadTexture("Content/Data/UI/merchant_loot_bag_img.png") }
            };
            Mouse.SetCursor(MouseCursor.FromTexture2D(InterfaceTextures["hand1_mouse"], 0, 0));

            Scene = new LogonScene();
        }

        /// <summary>
        /// Subscribes all manner of objects to the network processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubscribeObjects(object sender, EventArgs e)
        {
            Network.Processor.RegisterNestedType<Vector2Serializable>();
            Network.Processor.SubscribeReusable<RealmClient_Disconnect>((newDisconenct) => PacketManager.OnPlayerDisconnect(newDisconenct));

            Network.Processor.SubscribeReusable<AuthClient_LogonCode>((AuthCodeType) => PacketManager.OnLogonResponse(AuthCodeType));

            Network.Processor.SubscribeReusable<AuthClient_Logon>((logon) => PacketManager.OnLogonSuccess(logon));

            Network.Processor.SubscribeNetSerializable<AuthClient_Realm>((realmlist) => PacketManager.OnRealmlist(realmlist));

            Network.Processor.SubscribeReusable<RealmClient_CreateCharacter>((response) => PacketManager.OnCreateCharacter(response));

            Network.Processor.SubscribeNetSerializable<RealmClient_PlayerCharacters>((characters) => PacketManager.OnCharacterList(characters));

            Network.Processor.SubscribeReusable<RealmClient_CreateLocalPlayer>((thePlayer) => PacketManager.OnLocalPlayer(thePlayer));

            Network.Processor.SubscribeReusable<RealmClient_MovementStateValidation>((result) =>
            {
                var localController = Player.GetComponent<LocalPlayerController>();
                localController.LastServerCalculation = result.ServerCalculation.ToVector2XNA();

                var animator = Player.GetComponent<SpriteAnimator>();
                animator.LastNetworkPosition = result.ServerCalculation.ToVector2XNA();

                localController.ProcessInputValidation(result);
            });

            Network.Processor.SubscribeReusable<RealmClient_CreateNetPlayer>((newPlayer) => PacketManager.OnNetworkPlayer(newPlayer));

            // mostly an empty packet. open to suggestions or later implementation :P
            Network.Processor.SubscribeReusable<RealmClient_EnterWorld>((worldParams) => PacketManager.OnEnterWorld(worldParams));

            Network.Processor.SubscribeReusable<RealmClient_MovementStateChange>((serverNetUpdate) => PacketManager.OnPlayerPositionUpdate(serverNetUpdate));

            Network.Processor.SubscribeNetSerializable<RealmClient_CreateNPC>((newNpc) => PacketManager.OnNPC(newNpc));

            Network.Processor.SubscribeReusable<RealmClient_WhoCommand>((whoList) => PacketManager.OnWho(whoList));

            Network.Processor.SubscribeReusable<RealmClient_SetTarget>((target) => PacketManager.OnSetTarget(target));

            Network.Processor.SubscribeReusable<RealmClient_Teleport>((teleport) => PacketManager.OnTeleport(teleport));

            Network.Processor.SubscribeReusable<ChatMessageObject>((newChat) => PacketManager.OnChat(newChat));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Network.Poll();
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
            Network.Disconnect();
            NetworkState = GameNetworkState.Offline;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Config.Save();
        }
    }
}
