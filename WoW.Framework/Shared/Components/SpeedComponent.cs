using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Framework.Shared.Components
{
    /// <summary>
    /// Simple container for entity speed.
    /// </summary>
    public class SpeedComponent : Component
    {
        public float Speed { get; set; } = 0.0f;

        public SpeedComponent(float speed)
            => Speed = speed;
    }
}
