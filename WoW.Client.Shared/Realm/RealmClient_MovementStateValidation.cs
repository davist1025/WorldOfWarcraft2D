using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Realm
{
    public class RealmClient_MovementStateValidation
    {
        //public string PlayerName { get; set; }
        public Vector2Serializable ServerCalculation { get; set; }
        public long Sequence { get; set; }
    }

    public struct Vector2Serializable : INetSerializable
    {
        public float X;
        public float Y;

        public Vector2Serializable(float x, float y)
        {
            X = x;
            Y = y;
        }

        public void Deserialize(NetDataReader reader)
        {
            X = reader.GetFloat();
            Y = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(X);
            writer.Put(Y);
        }

        public Vector2 ToVector2XNA()
            => new Vector2(X, Y);
    }
}
