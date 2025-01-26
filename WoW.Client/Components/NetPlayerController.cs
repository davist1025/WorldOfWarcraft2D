using Microsoft.Xna.Framework;
using Nez;
using Nez.Aseprite;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared;
using WoW.Client.Shared.Realm;

namespace WoW.Client.Components
{
    public class NetPlayerController : Component, IUpdatable
    {
        public string Name;
        public int RaceId;
        public int HairId;
        public string MapId;

        private Vector2 _currentMoveDirection = Vector2.Zero;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollider;

        public Queue<Vector2> MovementDirectionQueue = new Queue<Vector2>();

        public NetPlayerController(RealmClient_CreateNetPlayer networkPlayer)
        {
            Name = networkPlayer.Name;
            RaceId = networkPlayer.RaceId;
            HairId = networkPlayer.HairId;
            MapId = networkPlayer.MapId;
        }

        public override void OnAddedToEntity()
        {
            _subPixelMovement = new SubpixelVector2();

            _mover = Entity.AddComponent<Mover>();
            _circleCollider = Entity.AddComponent<CircleCollider>();
            _circleCollider.SetRadius(8f);
        }

        public void Update()
        {
            if (MovementDirectionQueue.TryDequeue(out Vector2 serverOut))
            {
                Vector2 movement = new Vector2(serverOut.X, serverOut.Y);
                var direction = Game1.MovementSpeed * Time.DeltaTime * movement;

                _mover.CalculateMovementExcluding(ref direction, new[] { Entity.Scene.FindComponentOfType<LocalPlayerController>().Entity }, out var res);
                _subPixelMovement.Update(ref direction);
                _mover.ApplyMovement(direction);
            }
        }

        /// <summary>
        /// Creates renderers, Movers, etc.
        /// </summary>
        public void AddToMap()
        {
            AsepriteFile aseFile = null;
            SpriteRenderer renderer; // todo: replace with animator.
            RaceType characterRace = (RaceType)RaceId;

            switch (characterRace)
            {
                case RaceType.Human:
                    aseFile = Core.Scene.Content.LoadAsepriteFile("Content/Data/Characters/human_spritesheet.ase");
                    break;
                case RaceType.Orc:
                    aseFile = Core.Scene.Content.LoadAsepriteFile("Content/Data/Characters/orc_spritesheet.ase");

                    break;
            }

            renderer = Entity.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            if (HairId > 1)
            {
                var hairSprite = Core.Scene.Content.LoadAsepriteFile($"Content/Data/Characters/hair_{HairId}_spritesheet.ase");

                Entity.AddComponent(new SpriteRenderer(hairSprite.Frames[0].ToSprite()));
            }
        }
    }
}
