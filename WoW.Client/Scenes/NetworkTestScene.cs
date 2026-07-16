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
using WoW.Client.Components.GUI;
using WoW.Client.Components.NPC;
using WoW.Network.Objects;
using WoW.Network.Packets;
using WoW.Network.Packets.Realm;
using static WoW.Framework.Utils;

namespace WoW.Client.Scenes
{
    public class NetworkTestScene : Scene
    {
        private LocalPlayerController _theController;

        public override void OnStart()
        {
            base.OnStart();

            Scene.SetDefaultDesignResolution(800, 600, SceneResolutionPolicy.ShowAllPixelPerfect);
            //followCam.FollowLerp = 0.5f;
        }

        public override void Initialize()
        {
            //CreateEntity("gui").AddComponent(new ImGuiController());
            CreateEntity("gui").AddComponent<ImGuiGameManagerComponent>();
        }

        public void CreateLocalPlayer(RealmClient_CreateLocalPlayer thePlayer)
        {
            _theController = new LocalPlayerController(thePlayer.Name, (ActorAnimationDirection)thePlayer.Direction);
            Global.Player = CreateEntity(thePlayer.WorldId);
            Global.Player.Transform.Position = new Vector2(thePlayer.ZoneX, thePlayer.ZoneY);
            Global.Player.Transform.LerpedPosition = new Vector2(thePlayer.ZoneX, thePlayer.ZoneY);
            Global.Player.Tag = (int)ActorType.Local;
            Game1.ActiveMapId = thePlayer.MapId;

            Camera.Entity.AddComponent(new FollowCamera(Global.Player, Camera));
            Camera.Zoom = 0.5f;

            AsepriteFile aseFile = null;
            ActorRaceType characterRace = (ActorRaceType)thePlayer.RaceId;

            switch (characterRace)
            {
                case ActorRaceType.Human:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/human_spritesheet.ase");
                    break;
                case ActorRaceType.Orc:
                    aseFile = Content.LoadAsepriteFile("Content/Data/Characters/orc_spritesheet.ase");
                    break;
            }

            var actorSpriteAtlas = aseFile.ToSpriteAtlas();
            var animator = Global.Player.AddComponent<SpriteAnimator>();
            animator.AddAnimationsFromAtlas(actorSpriteAtlas);
            animator.IsNetworked = true;
            animator.LastNetworkPosition = Global.Player.Transform.LerpedPosition;

            // todo: how should we apply the shadow? unsure how to detach it from the rest of the animation and render it separately.
            animator.RenderLayer = 5;

            Global.Player.AddComponent(_theController);

            TmxMap tmxMapByMapId;
            tmxMapByMapId = Global.Maps.Where(map => map.Properties["id"].ToLower().Equals(thePlayer.MapId)).FirstOrDefault();
            TiledMapRenderer mapRenderer = null;

            if (tmxMapByMapId != null)
            {
                mapRenderer = CreateEntity("map").AddComponent(new TiledMapRenderer(tmxMapByMapId, "collision_layer"));
                //Camera.AddComponent(new CameraLockController(new Vector2(tmxMapByMapId.TileWidth, tmxMapByMapId.TileWidth), 
                //    new Vector2(
                //        tmxMapByMapId.TileWidth * (tmxMapByMapId.Width - 1), 
                //        tmxMapByMapId.TileWidth * (tmxMapByMapId.Height - 1))));
            }

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
            theOtherEntity.Tag = (int)ActorType.Networked;

            Debug.Log($"Player: {theOtherEntity.Name} ({netController.Name}) has joined the world!");

            theOtherEntity.AddComponent(netController);

            // only create a renderer(s) if we're on the same map as them.
            if (theOtherPlayer.MapId.ToLower().Equals(Game1.ActiveMapId))
                netController.AddToMap();
        }

        public void CreateNPC(NpcMetadataObject remoteData)
        {
            var npcController = new NpcController(remoteData);
            var theNpcEntity = CreateEntity($"{remoteData.Uid}", remoteData.Position.ToXnaVector2());
            theNpcEntity.Tag = (int)ActorType.Mob;

            theNpcEntity.AddComponent(npcController);
        }

        public override void Update()
        {
            base.Update();

            // todo: crashes from a null reference?
            //if (Input.IsKeyPressed(Game1.Configuration.KeyboardControlMap[Client.Content.ControlMap.TabTarget]))
            //    Game1.Configuration.ControlHandlers[Client.Content.ControlMap.TabTarget]?.Invoke(null, null);
        }
    }
}
