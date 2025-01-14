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

        public void CreateLocalPlayer(RealmClient_CreateLocalPlayer thePlayer)
        {
            _theController = new LocalPlayerController();
            _thePlayer = CreateEntity(thePlayer.Name, new Vector2(thePlayer.ZoneX, thePlayer.ZoneY));

            AsepriteFile aseFile = null;
            SpriteRenderer raceRenderer; // todo: replace with animator.
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

            raceRenderer = _thePlayer.AddComponent(new SpriteRenderer(aseFile.Frames[0].ToSprite()));

            if (thePlayer.HairId > 1)
            {
                var hairSprite = Content.LoadAsepriteFile($"Content/Data/Characters/hair_{thePlayer.HairId}_spritesheet.ase");

                _thePlayer.AddComponent(new SpriteRenderer(hairSprite.Frames[0].ToSprite()));
            }

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

            if (theOtherPlayer.HairId > 1)
            {
                var hairSprite = Content.LoadAsepriteFile($"Content/Data/Characters/hair_{theOtherPlayer.HairId}_spritesheet.ase");

                theOtherEntity.AddComponent(new SpriteRenderer(hairSprite.Frames[0].ToSprite()));
            }

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
