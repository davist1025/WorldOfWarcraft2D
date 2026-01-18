using Microsoft.Xna.Framework;
using Nez;
using Nez.Aseprite;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Network.Packets;
using WoW.Network.Packets.Realm;
using static WoW.Framework.Utils;

namespace WoW.Client.Components
{
    public class NetPlayerController : Component, IUpdatable
    {
        public string Name;
        public int RaceId;
        public int HairId;
        public string MapId;
        public ActorAnimationDirection Direction;

        private Vector2 _currentMoveDirection = Vector2.Zero;
        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private CircleCollider _circleCollider;
        private SpriteAnimator _animator;

        public Queue<Vector2> MovementDirectionQueue = new Queue<Vector2>();

        private bool _isInMap = false;

        public NetPlayerController(RealmClient_CreateNetPlayer networkPlayer)
        {
            // todo: can 'RemoteCharacter' be used here?
            Name = networkPlayer.Name;
            RaceId = networkPlayer.RaceId;
            HairId = networkPlayer.HairId;
            MapId = networkPlayer.MapId;
            Direction = (ActorAnimationDirection)networkPlayer.Direction;
        }

        public override void OnAddedToEntity()
        {
            _subPixelMovement = new SubpixelVector2();

            _mover = Entity.AddComponent<Mover>();
            //_circleCollider = Entity.AddComponent<CircleCollider>();
            //_circleCollider.SetRadius(8f);
        }

        public void Update()
        {
            if (_isInMap)
            {
                if (!_animator.IsRunning)
                {
                    string startingAnimation = "";
                    switch (Direction)
                    {
                        case ActorAnimationDirection.North:
                        case ActorAnimationDirection.East:
                        case ActorAnimationDirection.South:
                        case ActorAnimationDirection.West:
                            startingAnimation = "idle";
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

                    if (movement.X < 0f)
                    {
                        Direction = ActorAnimationDirection.West;
                        _animator.FlipX = true;
                    }

                    if (movement.X > 0f)
                    {
                        Direction = ActorAnimationDirection.East;
                        _animator.FlipX = false;
                    }

                    if (movement.Y > 0f) Direction = ActorAnimationDirection.South;

                    if (movement.Y < 0f) Direction = ActorAnimationDirection.North;

                    switch (Direction)
                    {
                        case ActorAnimationDirection.North:
                        case ActorAnimationDirection.East:
                        case ActorAnimationDirection.South:
                        case ActorAnimationDirection.West:
                            if (!_animator.CurrentAnimationName.Equals("walk"))
                                _animator.Play("walk");
                            break;
                    }

                    //_mover.CalculateMovementExcluding(ref velocity, new[] { Entity.Scene.FindComponentOfType<LocalPlayerController>().Entity }, out var res);
                    _subPixelMovement.Update(ref velocity);
                    _mover.ApplyMovement(velocity);
                }

                if (serverInputOut == Vector2.Zero)
                {
                    switch (Direction)
                    {
                        case ActorAnimationDirection.North:
                        case ActorAnimationDirection.East:
                        case ActorAnimationDirection.South:
                        case ActorAnimationDirection.West:
                            if (!_animator.CurrentAnimationName.Equals("idle"))
                                _animator.Play("idle");
                            break;
                    }
                }

                _animator.Update();
            }
        }

        /// <summary>
        /// Creates renderers, Movers, etc.
        /// </summary>
        public void AddToMap()
        {
            AsepriteFile aseFile = null;
            ActorRaceType characterRace = (ActorRaceType)RaceId;

            switch (characterRace)
            {
                case ActorRaceType.Human:
                    aseFile = Core.Scene.Content.LoadAsepriteFile("Content/Data/Characters/human_spritesheet.ase");
                    break;
                case ActorRaceType.Orc:
                    aseFile = Core.Scene.Content.LoadAsepriteFile("Content/Data/Characters/orc_spritesheet.ase");
                    break;
            }

            var actorSpriteAtlas = aseFile.ToSpriteAtlas();
            _animator = new SpriteAnimator();
            _animator.AddAnimationsFromAtlas(actorSpriteAtlas);
            _animator.RenderLayer = 5;
            _animator.Speed = 0.5f;
            Entity.AddComponent(_animator);
            _isInMap = true;

            //renderer = Entity.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            //if (HairId > 1)
            //{
            //    var hairSprite = Core.Scene.Content.LoadAsepriteFile($"Content/Data/Characters/hair_{HairId}_spritesheet.ase");

            //    Entity.AddComponent(new SpriteRenderer(hairSprite.Frames[0].ToSprite()));
            //}
        }
    }
}
