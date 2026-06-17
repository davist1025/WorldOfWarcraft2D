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
        public static string ActiveMapId { get; set; }
        public static RealmserverMetadataObject LastConnectedRealm;

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
            Global.Network = new NetworkController();
            Global.Network.OnProcessorSubscribe += ProcessorSubscription;

            // dictates specifc activity that should occur upon a successful connection attempt to a given server.
            Global.Network.OnPeerConnect += (peer) =>
            {
                switch (Global.OnlineState)
                {
                    case GameNetworkState.Auth_LoggingIn:
                        string[] loginInfo = Core.Scene.FindEntity("gui").GetComponent<ImGuiController>().GetLogin().Split(':');
                        Global.Network.SendToServer(PacketManager.CreateLogonPacket(loginInfo[0], loginInfo[1]));
                        break;
                    case GameNetworkState.Realm:
                        Global.Network.SendToServer(new ClientRealm_TransferLogon() { SessionId = Global.SessionId });
                        break;
                }
            };
            Global.Network.StartClient();

            /*
             * NetState can be changed to display whichever UI is necessary, per ImGUI.
             * We need to set ClientNetwork's connection state at the same time we set this so the packet flow doesn't crash the client or server.
             */

            Global.Config = GameConfiguration.Load();

            base.Initialize();

            IsFixedTimeStep = true;
            ExitOnEscapeKeypress = false;

            Debug.Log($"Loading Tiled maps...");
            var tmxFiles = Directory.GetFiles("Content/Data/").Where(f => f.EndsWith(".tmx")).ToArray();
            Global.Maps = new TmxMap[tmxFiles.Length];
            for (int i = 0; i < tmxFiles.Length; i++)
            {
                var mapFile = tmxFiles[i];
                Global.Maps[i] = Core.Content.LoadTiledMap(mapFile);
                Debug.Log($"Loaded {Global.Maps[i].Properties["id"]}");
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

            Global.InterfaceSprites = new Dictionary<string, Texture2D>()
            {
                { "gear_icon", Core.Content.LoadTexture("Content/Data/UI/gear_img.png") },
                { "hand1_mouse", Core.Content.LoadTexture("Content/Data/UI/hand1_mouse.png") },
                { "merchant_bag_icon", Core.Content.LoadTexture("Content/Data/UI/merchant_loot_bag_img.png") }
            };
            Mouse.SetCursor(MouseCursor.FromTexture2D(Global.InterfaceSprites["hand1_mouse"], 0, 0));

            Scene = new LogonScene();
        }

        /// <summary>
        /// Subscribes all manner of objects to the network processor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessorSubscription()
        {
            Global.Network.Processor.RegisterNestedType<Vector2Serializable>();
            Global.Network.Processor.SubscribeReusable<RealmClient_Disconnect>((newDisconenct) => PacketManager.OnPlayerDisconnect(newDisconenct));

            Global.Network.Processor.SubscribeReusable<AuthClient_LogonCode>((AuthCodeType) => PacketManager.OnLogonResponse(AuthCodeType));

            Global.Network.Processor.SubscribeReusable<AuthClient_Logon>((logon) => PacketManager.OnLogonSuccess(logon));

            Global.Network.Processor.SubscribeNetSerializable<AuthClient_Realm>((realmlist) => PacketManager.OnRealmlist(realmlist));

            Global.Network.Processor.SubscribeReusable<RealmClient_CreateCharacter>((response) => PacketManager.OnCreateCharacter(response));

            Global.Network.Processor.SubscribeNetSerializable<RealmClient_PlayerCharacters>((characters) => PacketManager.OnCharacterList(characters));

            Global.Network.Processor.SubscribeReusable<RealmClient_CreateLocalPlayer>((thePlayer) => PacketManager.OnLocalPlayer(thePlayer));

            Global.Network.Processor.SubscribeReusable<RealmClient_MovementStateValidation>((result) =>
            {
                var localController = Global.Player.GetComponent<LocalPlayerController>();
                localController.LastServerCalculation = result.ServerCalculation.ToVector2XNA();

                var animator = Global.Player.GetComponent<SpriteAnimator>();
                animator.LastNetworkPosition = result.ServerCalculation.ToVector2XNA();

                localController.ProcessInputValidation(result);
            });

            Global.Network.Processor.SubscribeReusable<RealmClient_CreateNetPlayer>((newPlayer) => PacketManager.OnNetworkPlayer(newPlayer));

            // mostly an empty packet. open to suggestions or later implementation :P
            Global.Network.Processor.SubscribeReusable<RealmClient_EnterWorld>((worldParams) => PacketManager.OnEnterWorld(worldParams));

            Global.Network.Processor.SubscribeReusable<RealmClient_MovementStateChange>((serverNetUpdate) => PacketManager.OnPlayerPositionUpdate(serverNetUpdate));

            Global.Network.Processor.SubscribeNetSerializable<RealmClient_CreateNPC>((newNpc) => PacketManager.OnNPC(newNpc));

            Global.Network.Processor.SubscribeReusable<RealmClient_WhoCommand>((whoList) => PacketManager.OnWho(whoList));

            Global.Network.Processor.SubscribeReusable<RealmClient_SetTarget>((target) => PacketManager.OnSetTarget(target));

            Global.Network.Processor.SubscribeReusable<RealmClient_Teleport>((teleport) => PacketManager.OnTeleport(teleport));

            Global.Network.Processor.SubscribeReusable<ChatMessageObject>((newChat) => PacketManager.OnChat(newChat));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Global.Network.Poll();
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
            Global.Network.Disconnect();
            Global.OnlineState = GameNetworkState.Offline;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Global.Config.Save();
        }
    }
}
