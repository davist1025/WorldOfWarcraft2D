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
        public SpriteDirection Direction;

        private Vector2 _currentMoveDirection = Vector2.Zero;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollider;
        private SpriteAnimator _animator;

        public Queue<Vector2> MovementDirectionQueue = new Queue<Vector2>();

        public NetPlayerController(RealmClient_CreateNetPlayer networkPlayer)
        {
            // todo: can 'RemoteCharacter' be used here?
            Name = networkPlayer.Name;
            RaceId = networkPlayer.RaceId;
            HairId = networkPlayer.HairId;
            MapId = networkPlayer.MapId;
            Direction = (SpriteDirection)networkPlayer.Direction;
        }

        public override void OnAddedToEntity()
        {
            _subPixelMovement = new SubpixelVector2();

            _mover = Entity.AddComponent<Mover>();
            _circleCollider = Entity.AddComponent<CircleCollider>();
            _circleCollider.SetRadius(8f);

            Entity.AddComponent(_animator);
        }

        public void Update()
        {
            if (!_animator.IsRunning)
            {
                string startingAnimation = "";
                switch (Direction)
                {
                    case SpriteDirection.North:
                        startingAnimation = "idle_north";
                        break;
                    case SpriteDirection.East:
                        startingAnimation = "idle_east";
                        break;
                    case SpriteDirection.South:
                        startingAnimation = "idle_south";
                        break;
                    case SpriteDirection.West:
                        startingAnimation = "idle_west";
                        break;
                }
                _animator.Play(startingAnimation); // avoids a null-reference exception.
            }

            Vector2 serverInputOut = Vector2.Zero;
            MovementDirectionQueue.TryDequeue(out serverInputOut);

            if (serverInputOut != Vector2.Zero)
            {
                Vector2 movement = new Vector2(serverInputOut.X, serverInputOut.Y);
                var velocity = Game1.MovementSpeed * Time.DeltaTime * movement;
                velocity.Round();

                if (!_animator.IsRunning)
                {
                    string startingAnimation = "";
                    switch (Direction)
                    {
                        case SpriteDirection.North:
                            startingAnimation = "idle_north";
                            break;
                        case SpriteDirection.East:
                            startingAnimation = "idle_east";
                            break;
                        case SpriteDirection.South:
                            startingAnimation = "idle_south";
                            break;
                        case SpriteDirection.West:
                            startingAnimation = "idle_west";
                            break;
                    }
                    _animator.Play(startingAnimation); // avoids a null-reference exception.
                }

                if (movement.X < 0f) Direction = SpriteDirection.West;

                if (movement.X > 0f) Direction = SpriteDirection.East;

                if (movement.Y > 0f) Direction = SpriteDirection.South;

                if (movement.Y < 0f) Direction = SpriteDirection.North;

                switch (Direction)
                {
                    case SpriteDirection.North:
                        if (!_animator.CurrentAnimationName.Equals("run_north"))
                            _animator.Play("run_north");
                        break;
                    case SpriteDirection.East:
                        if (!_animator.CurrentAnimationName.Equals("run_east"))
                            _animator.Play("run_east");
                        break;
                    case SpriteDirection.South:
                        if (!_animator.CurrentAnimationName.Equals("run_south"))
                            _animator.Play("run_south");
                        break;
                    case SpriteDirection.West:
                        if (!_animator.CurrentAnimationName.Equals("run_west"))
                            _animator.Play("run_west");
                        break;
                }

                _mover.CalculateMovementExcluding(ref velocity, new[] { Entity.Scene.FindComponentOfType<LocalPlayerController>().Entity }, out var res);
                _subPixelMovement.Update(ref velocity);
                _mover.ApplyMovement(velocity);
            }

            if (serverInputOut == Vector2.Zero)
            {
                switch (Direction)
                {
                    case SpriteDirection.North:
                        if (!_animator.CurrentAnimationName.Equals("idle_north"))
                            _animator.Play("idle_north");
                        break;
                    case SpriteDirection.East:
                        if (!_animator.CurrentAnimationName.Equals("idle_east"))
                            _animator.Play("idle_east");
                        break;
                    case SpriteDirection.South:
                        if (!_animator.CurrentAnimationName.Equals("idle_south"))
                            _animator.Play("idle_south");
                        break;
                    case SpriteDirection.West:
                        if (!_animator.CurrentAnimationName.Equals("idle_west"))
                            _animator.Play("idle_west");
                        break;
                }
            }

            _animator.Update();
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

            var actorSpriteAtlas = aseFile.ToSpriteAtlas();
            _animator = new SpriteAnimator();
            _animator.AddAnimationsFromAtlas(actorSpriteAtlas);
            _animator.RenderLayer = 5;
            _animator.Speed = 0.5f;

            //renderer = Entity.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            //if (HairId > 1)
            //{
            //    var hairSprite = Core.Scene.Content.LoadAsepriteFile($"Content/Data/Characters/hair_{HairId}_spritesheet.ase");

            //    Entity.AddComponent(new SpriteRenderer(hairSprite.Frames[0].ToSprite()));
            //}
        }
    }
}
