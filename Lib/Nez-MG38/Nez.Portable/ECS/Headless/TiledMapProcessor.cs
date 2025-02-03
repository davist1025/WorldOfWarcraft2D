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
		public TmxMap Map;
		public TmxLayer CollisionLayer;
		public int PhysicsLayer = 1 << 0; // todo: investigage this :p

		private Collider[] _colliders;
		public List<Entity> Creatures;

		public TiledMapProcessor(TmxMap tiledMap, string collisionLayerName = null)
		{
			Map = tiledMap;
			Creatures = new List<Entity>();

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

		public void AddCreature(Entity entity, bool isNpc = false)
		{
			Collider collider = null;
			if (Creatures.AddIfNotPresent(entity))
			{
				if (!entity.HasComponent<Collider>() && isNpc)
					entity.AddComponent(new CircleCollider(64f));

				if (isNpc)
					collider.IsTrigger = true;

				collider = entity.GetComponent<CircleCollider>();
				if (collider != null)
					collider.CollidesWithLayers = PhysicsLayer;
			}
		}
	}
}
