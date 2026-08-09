using LiteNetLib;
using Microsoft.Xna.Framework;
using Nez;
using Nez.ECS.Headless;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Database.Models.Auth;
using WoW.Database.Models.Realm.Character;
using WoW.Framework.Logging;
using WoW.Framework.Shared.Components;
using WoW.Realmserver.Network;
using static WoW.Framework.Utils;

namespace WoW.Realmserver.Components
{
    public enum SessionState
    {
        OnCharacterList,
        OnRealm
    }

    public class SessionComponent : Component, IUpdatable
    {
        /// <summary>
        /// This player's account id. 
        /// 
        /// This is given to the realmserver by the authentication server.
        /// </summary>
        public int AccountId { get; init; }

        /// <summary>
        /// The <see cref="NetPeer.Id"/> of this Session. This gets set when the player successfully transfers to the realmserver.
        /// </summary>
        public int ServerId { get; init; }

        /// <summary>
        /// A randomly generated string to differentiate this session from the rest.
        /// </summary>
        public string NetworkId { get; init; }

        public string SessionId { get; init; }

        public SessionState NetworkState = SessionState.OnCharacterList;

        public List<PlayerCharacter> Characters;
        private int _selectedCharacterIndex = -1;

        private SubpixelVector2 _subPixelMovement;
        private Mover _mover;
        private Vector2 _moveDirection = Vector2.Zero;
        private SpeedComponent _speedComponent;

        private Queue<Tuple<Vector2, long>> _movementUpdates = new Queue<Tuple<Vector2, long>>();

        public SessionComponent(int accountId, int serverId, string networkId, string sessionId) 
        {
            AccountId = accountId;
            ServerId = serverId;
            NetworkId = networkId;
            SessionId = sessionId;
        }

        public void Update()
        {
            if (NetworkState == SessionState.OnRealm)
            {
                if (_mover == null)
                {
                    Logger.Print($"Character '{GetSelectedCharacter().Name}' has not had their game components initialized.", LogEntryType.Fatal);
                    Entity.Destroy();
                }

                if (_movementUpdates.Count > 0)
                {
                     Tuple<Vector2, long> movementUpdate = _movementUpdates.Dequeue();

                    var moveDirection = _speedComponent.Speed * Global.DeltaTime * movementUpdate.Item1;
                    moveDirection.Round();

                    _subPixelMovement.Update(ref moveDirection);
                    _mover.ApplyMovement(moveDirection);

                    GetSelectedCharacter().SetPosition(Entity.Position);

                    NetPacketManager.BuildReconciliation(ServerId, Entity.Position, movementUpdate.Item2);

                    Logger.Print($"'{GetSelectedCharacter().Name}' has moved to: {Entity.Position.X}:{Entity.Position.Y}.", LogEntryType.Debug);
                }
            }
        }

        /// <summary>
        /// Initialize all necessary components for a player to play the game.
        /// </summary>
        public void InitializeGameComponents()
        {
            // todo: [session] create collider component.
            _mover = Entity.AddComponent<Mover>();
            _speedComponent = Entity.GetComponent<SpeedComponent>();

            Entity.SetPosition(new Vector2(Characters[_selectedCharacterIndex].XPosition, Characters[_selectedCharacterIndex].YPosition));
        }

        /// <summary>
        /// Called from <see cref="NetPacketManager.ReadEnterWorld(NetPeer, NetPacketReader, DeliveryMethod)"/>
        /// </summary>
        /// <param name="index"></param>
        public void SetSelectedCharacter(int index)
            => _selectedCharacterIndex = index - 1;

        /// <summary>
        /// Returns the character this player is currently using.
        /// </summary>
        /// <returns></returns>
        public PlayerCharacter GetSelectedCharacter()
            => Characters[_selectedCharacterIndex];

        public void EnqueuePositionChange(Vector2 input, long timeTick)
        {
            Logger.Print($"Queueing movement update for '{GetSelectedCharacter().Name}'.", LogEntryType.Debug);
            _movementUpdates.Enqueue(new(input, timeTick));
        }

        /// <summary>
        /// Returns this individual character's movement speed.
        /// </summary>
        /// <returns></returns>
        public float GetCharacterSpeed() => _speedComponent.Speed;
    }
}
