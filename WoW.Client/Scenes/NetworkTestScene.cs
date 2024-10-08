﻿using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Shared.Realm;
using WoW.Client.Shared.Serializable;

namespace WoW.Client.Scenes
{
    public class NetworkTestScene : Scene
    {
        private Entity _thePlayer;
        private LocalPlayerController _theController;

        public NetworkTestScene(SerializableCharacter character)
        {
            _theController = new LocalPlayerController(character);
        }

        public override void OnStart()
        {
            base.OnStart();

            var playerEntity = CreateEntity("player").AddComponent(_theController);
            CreateEntity("gui").AddComponent(new ImGuiController());
            Camera.Entity.AddComponent(new FollowCamera(playerEntity.Entity, Camera));
            Camera.Zoom = 0.5f;
        }

        public override void Initialize()
        {
            // todo: more debug tiled code.
            var map = Content.LoadTiledMap("world1.tmx");

            CreateEntity("testmap").AddComponent(new TiledMapRenderer(map, "collision_layer"));
        }

        public void CreateEntity(RealmClient_CreateGameObject create)
        {
            var newEntity = CreateEntity(create.Id, new Vector2(create.X, create.Y));
            newEntity.Tag = (int)create.EntityType;
        }

        public void CreatePlayer(RealmClient_CreateNetPlayer newLogin)
        {
            var existingEntity = FindEntity(newLogin.Id);

            if (existingEntity != null)
            {
                var newController = new NetPlayerController(newLogin.PlayerCharacter);

                existingEntity.AddComponent(newController);

                Debug.Log($"New player: {newController.Character.Name} has connected!");
            }
        }

        public void UpdatePlayerPosition(RealmClient_NetPositionInputUpdate posUpdate)
        {
            // should player's receive another's input and formulate their own position??
            // as the client, we could also receive this result position and interpolate in-between;
            // the client calculates the direction in which the result is (i.e x=1, y=-1) and forms a smooth movement, animation, etc.

            var player = FindEntity(posUpdate.Id);
            if (player != null)
                player.GetComponent<NetPlayerController>().Inputs.Enqueue(new Vector2(posUpdate.X, posUpdate.Y));
        }

        public override void Update()
        {
            base.Update();

            if (Game1.EntityQueue.Count > 0)
            {
                var entity = Game1.EntityQueue.Dequeue();
                AddEntity(entity);
            }
        }

        public Entity GetPlayer()
            => _thePlayer;
    }
}
