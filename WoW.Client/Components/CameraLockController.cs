using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Components
{
    public class CameraLockController : Component, IUpdatable
    {
        private Vector2 _min;
        private Vector2 _max;

        public CameraLockController(Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
        }

        public override void OnAddedToEntity()
        {
            //_min = new Vector2(GameplayTestScene.Map.TileWidth, GameplayTestScene.Map.TileWidth);
            //_max = new Vector2(GameplayTestScene.Map.TileWidth * (GameplayTestScene.Map.Width - 1),
            //    GameplayTestScene.Map.TileWidth * (GameplayTestScene.Map.Height - 1));

            Entity.UpdateOrder = int.MaxValue;
        }

        public void Update()
        {
            var camBounds = Entity.Scene.Camera.Bounds;

            if (camBounds.Top < _min.Y)
                Entity.Scene.Camera.Position += new Vector2(0, _min.Y - camBounds.Top);

            if (camBounds.Left < _min.X)
                Entity.Scene.Camera.Position += new Vector2(_min.X - camBounds.Left, 0);

            if (camBounds.Bottom > _max.Y)
                Entity.Scene.Camera.Position += new Vector2(0, _max.Y - camBounds.Bottom);

            if (camBounds.Right > _max.X)
                Entity.Scene.Camera.Position += new Vector2(_max.X - camBounds.Right, 0);
        }
    }
}
