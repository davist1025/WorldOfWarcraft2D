using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Components
{
    internal class NetPlayerController : Component, IUpdatable
    {
        public SerializableCharacter Character;

        private Mover _mover;
        private SubpixelVector2 _subPixel;

        public Queue<Vector2> Inputs;

        public NetPlayerController(SerializableCharacter character)
            => Character = character;

        public override void OnAddedToEntity()
        {
            var renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            renderer.Color = Color.Red;

            _mover = Entity.AddComponent<Mover>();
            Inputs = new Queue<Vector2>();
        }

        public void Update()
        {
            if (Inputs.TryDequeue(out var input))
                // is this a valid way of updating a networked player's position?
                // later, we can calculate the change in direction using our current pos and this pos and add animations, etc.
                Entity.Transform.SetPosition(new Vector2(input.X, input.Y));
        }
    }
}
