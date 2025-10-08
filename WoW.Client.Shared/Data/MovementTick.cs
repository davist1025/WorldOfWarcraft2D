using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Data
{
    public struct MovementTick
    {
        public int TickId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2 Result { get; set; }
    }
}
