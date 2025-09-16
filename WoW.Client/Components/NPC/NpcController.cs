using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoW.Client.Shared.Data;

namespace WoW.Client.Components.NPC
{
    public class NpcController : Component, IUpdatable
    {
        public NpcMetadata Metadata;

        public NpcController(NpcMetadata metadata)
            => Metadata = metadata;

        public override void OnAddedToEntity()
        {
            var model = Metadata.ModelId;
            // todo: load model from Content.

            Debug.Log($"#### New NPC ####");
            Debug.Log($"{Metadata.Name} has a model of: {model}");
            Debug.Log($"{Metadata.WorldId} flags: {Metadata.Flags}");

            // Create renderers, mover, etc.
            var renderer = Entity.AddComponent(new PrototypeSpriteRenderer(16f, 16f));
            renderer.SetColor(Color.MonoGameOrange);
            renderer.RenderLayer = 0;

            var collisionTrigger = Entity.AddComponent(new CircleCollider(64f));
            collisionTrigger.IsTrigger = true;

            /*
             * AsepriteFile aseFile = null;
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
            */

            Entity.AddComponent<NpcFlagRenderer>();
        }

        public void Update()
        {
        }
    }
}
