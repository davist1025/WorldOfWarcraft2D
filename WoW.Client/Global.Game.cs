using Microsoft.Xna.Framework.Graphics;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Content;

namespace WoW.Client
{
    public partial class Global
    {
        public static GameConfiguration Config;

        public static Dictionary<string, Texture2D> InterfaceSprites;
        public static TmxMap[] Maps;
    }
}
