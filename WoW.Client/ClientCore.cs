using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Console;
using Nez.ImGuiTools;
using Nez.Sprites;
using Nez.Systems;
using Nez.Tiled;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using WoW.Client.Components.GUI;
using WoW.Client.Components.Player;
using WoW.Client.Content;
using WoW.Client.Network;
using WoW.Client.Scenes;
using WoW.Client.Utils;
using WoW.Framework;
using WoW.Framework.Logging;
using WoW.Framework.Network;
using static WoW.Framework.Utils;

namespace WoW.Client
{
    public class ClientCore : Core
    {
        public static string ActiveMapId { get; set; }

        private GameManager _gameManager;

        public ClientCore() : base(windowTitle: "WoW Pixel Project", width: 1080, height: 640)
        {
            IsMouseVisible = true;
            PauseOnFocusLost = false;
            IsFixedTimeStep = true;
        }

        protected override void Initialize()
        {
            /*
             * NetState can be changed to display whichever UI is necessary, per ImGUI.
             * We need to set ClientNetwork's connection state at the same time we set this so the packet flow doesn't crash the client or server.
             */

            base.Initialize();

            IsFixedTimeStep = true;
            ExitOnEscapeKeypress = false;

            CreateGlobalManagers();
            LoadGameContent();

            _gameManager.Network = new NetworkManager(new NetworkEventListener());
            _gameManager.Network.StartClient();
            //Global.Network.RunLatencySimulation();

            // create the player object here because the same entity can be used in on/offline mode.
            // this will get added to the scene later.
            _gameManager.Player = new Entity("thePlayer");

            Scene = new LogonScene();
        }

        private void LoadGameContent()
        {
            var assetManager = Core.GetGlobalManager<AssetManager>();

            Logger.Print($"Loading game textures...", LogEntryType.Debug);
            assetManager.LoadGameTextures();

            Logger.Print($"Loading Tiled maps...", LogEntryType.Debug);
            assetManager.LoadTiledMaps();

            Mouse.SetCursor(MouseCursor.FromTexture2D(assetManager.GetTexture("default_mouse"), 0, 0));
        }

        /// <summary>
        /// Creates managers used throughout the game code, except NetworkManager.
        /// </summary>
        private void CreateGlobalManagers()
        {
            var guiManager = new ImGuiManager()
            {
                ShowCoreWindow = false,
                ShowDemoWindow = false,
                ShowSceneGraphWindow = true,
                ShowSeperateGameWindow  = false,
                ShowMenuBar = false,
                ShowStyleEditor = false,
            };
            Core.RegisterGlobalManager(guiManager);

            _gameManager = new GameManager();
            Core.RegisterGlobalManager(_gameManager);

            var assetManager = new AssetManager();
            Core.RegisterGlobalManager(assetManager);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _gameManager.Network.Update();

            if (_gameManager.Network.IsConnected())
            {
                if (_gameManager.PopPacket(out var dataWriter))
                {
                    _gameManager.Network.SendToServer(dataWriter, dataWriter.DeliveryMethod);
                }
            }
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
        }

        
    }
}
