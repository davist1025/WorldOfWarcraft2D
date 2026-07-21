using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Framework.Logging;

namespace WoW.Client.Content
{
    /// <summary>
    /// The hub for all game assets.
    /// </summary>
    internal class AssetManager : GlobalManager
    {
        private const string _rootDataDirectory = "Content/Data/";

        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, TmxMap> _maps;

        public AssetManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _maps = new Dictionary<string, TmxMap>();
        }

        public Texture2D AddTexture(string name, Texture2D texture)
        {
            _textures.Add(name, texture);
            return texture;
        }

        public void LoadEngineTextures()
        {
            var mouseIcon = AddTexture("default_mouse", Core.Content.LoadTexture("Content/Data/UI/hand1_mouse.png"));
        }

        public void LoadTiledMaps()
        {
            var tmxFiles = Directory
                .GetFiles(_rootDataDirectory)
                .Where(f => f.EndsWith(".tmx")).ToArray();

            for (int i = 0; i < tmxFiles.Length; i++)
            {
                var mapFile = tmxFiles[i];

                TmxMap map = Core.Content.LoadTiledMap(mapFile);
                _maps.Add(map.Properties["id"], map);
            }

            Logger.Print($"Loaded {_maps.Count} maps.", Framework.Utils.LogEntryType.Debug);
        }

        public Texture2D GetTexture(string name)
            => _textures[name];
    }
}
