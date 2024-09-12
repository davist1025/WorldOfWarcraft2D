using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nez.ECS.Headless
{
	/// <summary>
	/// Processes data about the TiledMap, namely collision.
	/// </summary>
	public class TiledMapProcessor
	{
		public TmxMap TiledMap;
		public TmxLayer CollisionLayer;
		public int PhysicsLayer = 1 << 0; // investigage this :p
		

		// todo: this should not be a component.
		// the server needs access to all map data all the time.

		public TiledMapProcessor(TmxMap tiledMap, string collisionLayerName = null)
		{
			TiledMap = tiledMap;

			if (collisionLayerName != null )
				CollisionLayer = tiledMap.TileLayers[collisionLayerName];
		}
	}
}
