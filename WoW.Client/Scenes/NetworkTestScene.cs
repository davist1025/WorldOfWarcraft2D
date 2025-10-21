using Microsoft.Xna.Framework;
using Nez;
using Nez.Aseprite;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Components;
using WoW.Client.Components.NPC;
using WoW.Client.Shared;
using WoW.Client.Shared.Data;
using WoW.Client.Shared.Realm;

namespace WoW.Client.Scenes
{
    public class NetworkTestScene : Scene
    {
        private LocalPlayerController _theController;

        public override void OnStart()
        {
            base.OnStart();

            Scene.SetDefaultDesignResolution(800, 600, SceneResolutionPolicy.ShowAllPixelPerfect);

            var followCam = Camera.Entity.AddComponent(new FollowCamera(Game1.Player, Camera));
            Camera.Zoom = 0.5f;
            //followCam.FollowLerp = 0.5f;
        }

        public override void Initialize()
        {
            CreateEntity("gui").AddComponent(new ImGuiController());
        }

        public void CreateLocalPlayer(RealmClient_CreateLocalPlayer thePlayer)
        {
            _theController = new LocalPlayerController(thePlayer.Name, (SpriteDirection)thePlayer.Direction);
            Game1.Player = CreateEntity(thePlayer.WorldId);
            Game1.Player.Transform.Position = new Vector2(thePlayer.ZoneX, thePlayer.ZoneY);
            Game1.Player.Transform.LerpedPosition = new Vector2(thePlayer.ZoneX, thePlayer.ZoneY);
            Game1.Player.Tag = (int)EntityType.LocalPlayer;
            Game1.CurrentMapId = thePlayer.MapId;

            AsepriteFile aseFile = null;
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

            var actorSpriteAtlas = aseFile.ToSpriteAtlas();
            var animator = Game1.Player.AddComponent<SpriteAnimator>();
            animator.AddAnimationsFromAtlas(actorSpriteAtlas);
            animator.IsNetworked = true;

            // todo: how should we apply the shadow? unsure how to detach it from the rest of the animation and render it separately.
            animator.RenderLayer = 5;

            Game1.Player.AddComponent(_theController);

            TmxMap tmxMapByMapId;
            tmxMapByMapId = Game1.Maps.Where(map => map.Properties["id"].ToLower().Equals(thePlayer.MapId)).FirstOrDefault();
            TiledMapRenderer mapRenderer = null;

            if (tmxMapByMapId != null)
                mapRenderer = CreateEntity("map").AddComponent(new TiledMapRenderer(tmxMapByMapId, "collision_layer"));

            if (mapRenderer != null)
                mapRenderer.RenderLayer = 10;
        }

        /// <summary>
        /// Creates a networked player for the client to display.
        /// 
        /// Functions similarly to the local player.
        /// </summary>
        /// <param name="theOtherPlayer"></param>
        public void CreateNetworkPlayer(RealmClient_CreateNetPlayer theOtherPlayer)
        {
            var netController = new NetPlayerController(theOtherPlayer);
            var theOtherEntity = CreateEntity(theOtherPlayer.WorldId, new Vector2(theOtherPlayer.ZoneX, theOtherPlayer.ZoneY));
            theOtherEntity.Tag = (int)EntityType.NetPlayer;

            Debug.Log($"Player: {theOtherEntity.Name} ({netController.Name}) has joined the world!");

            theOtherEntity.AddComponent(netController);

            // only create a renderer(s) if we're on the same map as them.
            if (theOtherPlayer.MapId.ToLower().Equals(Game1.CurrentMapId))
                netController.AddToMap();
        }

        public void CreateNPC(NpcMetadata remoteData)
        {
            var npcController = new NpcController(remoteData);
            var theNpcEntity = CreateEntity($"{remoteData.WorldId}", new Vector2(remoteData.X, remoteData.Y));
            theNpcEntity.Tag = (int)EntityType.NPC;

            theNpcEntity.AddComponent(npcController);
        }

        public override void Update()
        {
            base.Update();

            // todo: crashes from a null reference?
            if (Input.IsKeyPressed(Game1.Configuration.KeyboardControlMap[Client.Content.ControlMap.TabTarget]))
                Game1.Configuration.ControlHandlers[Client.Content.ControlMap.TabTarget]?.Invoke(null, null);
        }
    }
}
