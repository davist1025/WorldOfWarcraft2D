using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Content;
using WoW.Client.Scenes;

namespace WoW.Client
{
    public partial class Global
    {
        public enum GameEventType
        {
            LocalPlayerMoved
        }

        public struct GameEventsComparer : IEqualityComparer<GameEventType>
        {
            public bool Equals(GameEventType x, GameEventType y)
            {
                return x == y;
            }


            public int GetHashCode(GameEventType obj)
            {
                return (int)obj;
            }
        }

        public static GameConfiguration Config;

        public static Dictionary<string, Texture2D> InterfaceSprites;
        public static TmxMap[] Maps;

        public static Entity _Player; // todo: [Global._Player] rename to "Player" as cleanup progresses.
    }
}
