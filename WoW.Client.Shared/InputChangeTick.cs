using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared
{
    public class InputChangeTick
    {
        public int Tick { get; init; }
        public Vector2 Input { get; init; }
        public Vector2 Result { get; init; }

        public InputChangeTick(int tick, Vector2 input, Vector2 result)
        {
            Tick = tick;
            Input = input;
            Result = result;
        }
    }
}
