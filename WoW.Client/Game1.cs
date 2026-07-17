using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
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
using WoW.Client.Components.GUI;
using WoW.Client.Content;
using WoW.Client.Network;
using WoW.Client.Scenes;
using WoW.Framework;
using WoW.Network;
using WoW.Network.Objects;
using WoW.Network.Packets;
using WoW.Network.Packets.Authentication;
using WoW.Network.Packets.Authenticcation;
using WoW.Network.Packets.Client;
using WoW.Network.Packets.Realm;
using static WoW.Client.Global;

namespace WoW.Client
{
    public class Game1 : Core
    {
        public static string ActiveMapId { get; set; }
        public static RealmserverMetadataObject LastConnectedRealm;

        public static NetworkTestScene NetworkScene;

        public static bool ShouldShowEscapeMenu = false;
        public static bool ShouldShowGMChat = false;
        public static bool ShouldShowWhoMenu = false;
        public static bool ShowShowBackpack = false;

        public Game1() : base(windowTitle: "WoW Pixel Project", width: 1080, height: 640)
        {
            IsMouseVisible = true;
            PauseOnFocusLost = false;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            Global.Network = new NetworkManager(new NetworkEventListener());
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

            // create the player object.
            // this will get added to the scene later.
            Global._Player = new Entity("thePlayer");
            Global._Player.AddComponent<NetFootprintComponent>();

            Scene = new LogonScene();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Global.Network.Update();
        }

        /// <summary>
        /// Cleanly disconnects from the realmserver.
        /// </summary>
        public static void Disconnect()
        {
            var gui = Core.Scene.FindEntity("gui");
            Global.Characters.Clear();
            Global.Realmlist.Clear();
            Global.Network.Disconnect();
            //Global.Network.Disconnect();
            Global.PeerState = GameNetworkState.Offline;
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Global.Config.Save();
        }
    }
}
