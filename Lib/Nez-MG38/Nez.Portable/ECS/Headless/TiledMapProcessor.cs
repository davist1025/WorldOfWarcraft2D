using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nez.ECS.Headless
{
	/// <summary>
	/// Processes data about the TiledMap, namely collision.
	/// </summary>
	public class TiledMapProcessor : Component
	{
		public TmxMap TiledMap;
		public TmxLayer CollisionLayer;
		public int PhysicsLayer = 1 << 0; // investigage this :p

		private Collider[] _colliders;

		// the server needs access to all map data all the time.

		public TiledMapProcessor(TmxMap tiledMap, string collisionLayerName = null)
		{
			TiledMap = tiledMap;

			if (collisionLayerName != null )
				CollisionLayer = tiledMap.TileLayers[collisionLayerName];
		}

		public override void OnAddedToEntity()
		{
			if (CollisionLayer != null)
				AddColliders();
		}

		public void AddColliders()
		{
			var collisionRects = CollisionLayer.GetCollisionRectangles();

			_colliders = new Collider[collisionRects.Count];
			for (var i = 0; i < collisionRects.Count; i++)
			{
				var collider = new BoxCollider(collisionRects[i].X + 0f /* local offset X */,
					collisionRects[i].Y + 0f, collisionRects[i].Width, collisionRects[i].Height);
				collider.PhysicsLayer = PhysicsLayer;
				collider.Entity = Entity;
				_colliders[i] = collider;

				Physics.AddCollider(collider);
			}
		}
	}
}
