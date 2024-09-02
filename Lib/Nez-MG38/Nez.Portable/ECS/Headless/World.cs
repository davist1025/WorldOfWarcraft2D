using System;
using System.Collections.Generic;
using System.Text;

namespace Nez.ECS.Headless
{
	public class World
	{
		private EntityListHeadless _entities;

		public World()
		{
			_entities = new EntityListHeadless();
		}

		#region Entity Management

		public EntityHeadless CreateEntity(string name)
		{
			var e = new EntityHeadless(name);
			return AddEntity(e);
		}

		public EntityHeadless AddEntity(EntityHeadless entity)
		{
			Insist.IsFalse(_entities.Contains(entity), "You are attempting to add the same entity to the world twice: {0}", entity);
			_entities.Add(entity);

			for (var i = 0; i < entity.Transform.ChildCount; i++)
				AddEntity(entity.Transform.GetChild(i).Entity);

			return entity;
		}

		public EntityHeadless FindEntity(string name) => _entities.FindEntity(name);

		public List<EntityHeadless> FindEntitiesWithTag(int tag) => _entities.EntitiesWithTag(tag);


		#endregion

		public void Update()
		{
			_entities.UpdateLists();
			_entities.Update();
		}
	}
}
