using Microsoft.Xna.Framework;
using Nez;
using Nez.Aseprite;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Shared;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;

namespace WoW.Client.Scenes
{
    public class NetworkTestScene : Scene
    {
        private Entity _thePlayer;
        private LocalPlayerController _theController;

        public override void OnStart()
        {
            base.OnStart();

            //var playerEntity = CreateEntity("player").AddComponent(_theController);
            CreateEntity("gui").AddComponent(new ImGuiController());
            Camera.Entity.AddComponent(new FollowCamera(_thePlayer, Camera));
            Camera.Zoom = 0.5f;
        }

        public override void Initialize()
        {
            // todo: more debug tiled code.
            var map = Content.LoadTiledMap("Content/Data/world1.tmx");

            CreateEntity("testmap").AddComponent(new TiledMapRenderer(map, "collision_layer"));
        }

        //public void CreateEntity(RealmClient_CreateGameObject create)
        //{
        //    var newEntity = CreateEntity(create.Id, new Vector2(create.X, create.Y));
        //    newEntity.Tag = (int)create.EntityType;
        //}

        //public void CreatePlayer(RealmClient_CreateNetPlayer newLogin)
        //{
        //    var existingEntity = FindEntity(newLogin.Id);

        //    if (existingEntity != null)
        //    {
        //        var newController = new NetPlayerController(newLogin.PlayerCharacter);

        //        existingEntity.AddComponent(newController);

        //        Debug.Log($"New player: {newController.Character.Name} has connected!");
        //    }
        //}

        //public void UpdatePlayerPosition(RealmClient_NetPositionInputUpdate posUpdate)
        //{
        //    // should player's receive another's input and formulate their own position??
        //    // as the client, we could also receive this result position and interpolate in-between;
        //    // the client calculates the direction in which the result is (i.e x=1, y=-1) and forms a smooth movement, animation, etc.

        //    var player = FindEntity(posUpdate.Id);
        //    if (player != null)
        //        player.GetComponent<NetPlayerController>().Inputs.Enqueue(new Vector2(posUpdate.X, posUpdate.Y));
        //}

        public void CreateLocalPlayer(RealmClient_CreateLocalPlayer thePlayer)
        {
            _theController = new LocalPlayerController();
            _thePlayer = CreateEntity(thePlayer.Name, new Vector2(thePlayer.ZoneX, thePlayer.ZoneY));

            AsepriteFile aseFile = null;
            SpriteRenderer renderer; // todo: replace with animator.
            RaceType characterRace = (RaceType)thePlayer.RaceId;

            switch (characterRace)
            {
                case RaceType.Human:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/human_spritesheet.ase");
                    break;
                case RaceType.Orc:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/orc_spritesheet.ase");
                    break;
            }

            renderer = _thePlayer.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            _thePlayer.AddComponent(_theController);
        }

        /// <summary>
        /// Creates a networked player for the client to display.
        /// 
        /// Functions similarly to the local player.
        /// </summary>
        /// <param name="theOtherPlayer"></param>
        public void CreateNetworkPlayer(RealmClient_CreateNetPlayer theOtherPlayer)
        {
            var netController = new NetPlayerController();
            var theOtherEntity = CreateEntity(theOtherPlayer.Name, new Vector2(theOtherPlayer.ZoneX, theOtherPlayer.ZoneY));

            AsepriteFile aseFile = null;
            SpriteRenderer renderer; // todo: replace with animator.
            RaceType characterRace = (RaceType)theOtherPlayer.RaceId;

            switch (characterRace)
            {
                case RaceType.Human:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/human_spritesheet.ase");
                    break;
                case RaceType.Orc:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/orc_spritesheet.ase");

                    break;
            }
            renderer = theOtherEntity.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            theOtherEntity.AddComponent(netController);
        }

        public void CreateNPC(RemoteNPC remoteData)
        {
            var npcController = new NetNPCController(remoteData);
            var theNpcEntity = CreateEntity($"{remoteData.Name}{Nez.Random.NextInt(35000)}", new Vector2(remoteData.X, remoteData.Y));
            theNpcEntity.AddComponent(npcController);
        }

        public override void Update()
        {
            base.Update();
        }

        public Entity GetPlayer()
            => _thePlayer;
    }
}
