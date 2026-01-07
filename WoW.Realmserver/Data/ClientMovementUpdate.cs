using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Realmserver.Data
{
    /// <summary>
    /// Used when queueing a movement change request from a client.
    /// </summary>
    public class ClientMovementUpdate
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float DeltaTime { get; init; }
        public long Sequence { get; init; }

        public ClientMovementUpdate(float x, float y, float deltaTime, long sequence)
        {
            X = x;
            Y = y;
            DeltaTime = deltaTime;
            Sequence = sequence;
        }
    }
}
