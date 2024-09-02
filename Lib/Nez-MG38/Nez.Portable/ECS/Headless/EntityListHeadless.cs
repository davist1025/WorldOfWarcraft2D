using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nez.ECS.Headless
{
	public class EntityListHeadless
	{
		/// <summary>
		/// list of entities added to the scene
		/// </summary>
		FastList<EntityHeadless> _entities = new FastList<EntityHeadless>();

		/// <summary>
		/// The list of entities that were added this frame. Used to group the entities so we can process them simultaneously
		/// </summary>
		HashSet<EntityHeadless> _entitiesToAdd = new HashSet<EntityHeadless>();

		/// <summary>
		/// The list of entities that were marked for removal this frame. Used to group the entities so we can process them simultaneously
		/// </summary>
		HashSet<EntityHeadless> _entitiesToRemove = new HashSet<EntityHeadless>();

		/// <summary>
		/// flag used to determine if we need to sort our entities this frame
		/// </summary>
		bool _isEntityListUnsorted;

		/// <summary>
		/// tracks entities by tag for easy retrieval
		/// </summary>
		Dictionary<int, FastList<EntityHeadless>> _entityDict = new Dictionary<int, FastList<EntityHeadless>>();

		HashSet<int> _unsortedTags = new HashSet<int>();

		// used in updateLists to double buffer so that the original lists can be modified elsewhere
		HashSet<EntityHeadless> _tempEntityList = new HashSet<EntityHeadless>();

		#region array access

		public int Count => _entities.Length;

		public EntityHeadless this[int index] => _entities.Buffer[index];

		#endregion


		public void MarkEntityListUnsorted()
		{
			_isEntityListUnsorted = true;
		}

		internal void MarkTagUnsorted(int tag)
		{
			_unsortedTags.Add(tag);
		}

		/// <summary>
		/// adds an Entity to the list. All lifecycle methods will be called in the next frame.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void Add(EntityHeadless entity)
		{
			_entitiesToAdd.Add(entity);
		}

		/// <summary>
		/// removes an Entity from the list. All lifecycle methods will be called in the next frame.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void Remove(EntityHeadless entity)
		{
			Debug.WarnIf(_entitiesToRemove.Contains(entity),
				"You are trying to remove an entity ({0}) that you already removed", entity.Name);

			// guard against adding and then removing an Entity in the same frame
			if (_entitiesToAdd.Contains(entity))
			{
				_entitiesToAdd.Remove(entity);
				return;
			}

			if (!_entitiesToRemove.Contains(entity))
				_entitiesToRemove.Add(entity);
		}

		/// <summary>
		/// removes all entities from the entities list
		/// </summary>
		public void RemoveAllEntities()
		{
			// clear lists we don't need anymore
			_unsortedTags.Clear();
			_entitiesToAdd.Clear();
			_isEntityListUnsorted = false;

			// why do we update lists here? Mainly to deal with Entities that were detached before a Scene switch. They will still
			// be in the _entitiesToRemove list which will get handled by updateLists.
			UpdateLists();

			for (var i = 0; i < _entities.Length; i++)
			{
				_entities.Buffer[i]._isDestroyed = true;
				_entities.Buffer[i].OnRemovedFromScene();
			}

			_entities.Clear();
			_entityDict.Clear();
		}

		/// <summary>
		/// checks to see if the Entity is presently managed by this EntityList
		/// </summary>
		/// <param name="entity">Entity.</param>
		public bool Contains(EntityHeadless entity)
		{
			return _entities.Contains(entity) || _entitiesToAdd.Contains(entity);
		}

		FastList<EntityHeadless> GetTagList(int tag)
		{
			FastList<EntityHeadless> list;
			if (!_entityDict.TryGetValue(tag, out list))
			{
				list = new FastList<EntityHeadless>();
				_entityDict[tag] = list;
			}

			return _entityDict[tag];
		}

		internal void AddToTagList(EntityHeadless entity)
		{
			var list = GetTagList(entity.Tag);
			if (!list.Contains(entity))
			{
				list.Add(entity);
				_unsortedTags.Add(entity.Tag);
			}
		}

		internal void RemoveFromTagList(EntityHeadless entity)
		{
			FastList<EntityHeadless> list;
			if (_entityDict.TryGetValue(entity.Tag, out list))
				list.Remove(entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Update()
		{
			for (var i = 0; i < _entities.Length; i++)
			{
				var entity = _entities.Buffer[i];
				if (entity.Enabled && (entity.UpdateInterval == 1 || Time.FrameCount % entity.UpdateInterval == 0))
					entity.Update();
			}
		}

		public void UpdateLists()
		{
			// handle removals
			if (_entitiesToRemove.Count > 0)
			{
				Utils.Swap(ref _entitiesToRemove, ref _tempEntityList);
				foreach (var entity in _tempEntityList)
				{
					// handle the tagList
					RemoveFromTagList(entity);

					// handle the regular entity list
					_entities.Remove(entity);
					entity.OnRemovedFromScene(); // todo: remove/edit?
				}

				_tempEntityList.Clear();
			}

			// handle additions
			if (_entitiesToAdd.Count > 0)
			{
				Utils.Swap(ref _entitiesToAdd, ref _tempEntityList);
				foreach (var entity in _tempEntityList)
				{
					_entities.Add(entity);

					// handle the tagList
					AddToTagList(entity);
				}

				// now that all entities are added to the scene, we loop through again and call onAddedToScene
				foreach (var entity in _tempEntityList)
					entity.OnAddedToScene();

				_tempEntityList.Clear();
				_isEntityListUnsorted = true;
			}

			if (_isEntityListUnsorted)
			{
				_entities.Sort();
				_isEntityListUnsorted = false;
			}

			// sort our tagList if needed
			if (_unsortedTags.Count > 0)
			{
				foreach (var tag in _unsortedTags)
					_entityDict[tag].Sort();
				_unsortedTags.Clear();
			}
		}


		#region Entity search

		/// <summary>
		/// returns the first Entity found with a name of name. If none are found returns null.
		/// </summary>
		/// <returns>The entity.</returns>
		/// <param name="name">Name.</param>
		public EntityHeadless FindEntity(string name)
		{
			for (var i = 0; i < _entities.Length; i++)
			{
				if (_entities.Buffer[i].Name == name)
					return _entities.Buffer[i];
			}

			foreach (var entity in _entitiesToAdd)
			{
				if (entity.Name == name)
					return entity;
			}

			return null;
		}

		/// <summary>
		/// returns a list of all entities with tag. If no entities have the tag an empty list is returned. The returned List can be put back in the pool via ListPool.free.
		/// </summary>
		/// <returns>The with tag.</returns>
		/// <param name="tag">Tag.</param>
		public List<EntityHeadless> EntitiesWithTag(int tag)
		{
			var list = GetTagList(tag);

			var returnList = ListPool<EntityHeadless>.Obtain();
			returnList.Capacity = _entities.Length;
			for (var i = 0; i < list.Length; i++)
			{
				returnList.Add(list[i]);
			}

			return returnList;
		}

		/// <summary>
		/// returns a List of all Entities of type T. The returned List can be put back in the pool via ListPool.free.
		/// </summary>
		/// <returns>The of type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> EntitiesOfType<T>() where T : EntityHeadless
		{
			var list = ListPool<T>.Obtain();
			for (var i = 0; i < _entities.Length; i++)
			{
				if (_entities.Buffer[i] is T)
					list.Add((T)_entities.Buffer[i]);
			}

			foreach (var entity in _entitiesToAdd)
			{
				if (entity is T)
				{
					list.Add((T)entity);
				}
			}

			return list;
		}

		/// <summary>
		/// returns the first Component found in the Scene of type T
		/// </summary>
		/// <returns>The component of type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public T FindComponentOfType<T>() where T : ComponentHeadless
		{
			for (var i = 0; i < _entities.Length; i++)
			{
				if (_entities.Buffer[i].Enabled)
				{
					var comp = _entities.Buffer[i].GetComponent<T>();
					if (comp != null)
						return comp;
				}
			}

			foreach (var entity in _entitiesToAdd)
			{
				if (entity.Enabled)
				{
					var comp = entity.GetComponent<T>();
					if (comp != null)
						return comp;
				}
			}

			return null;
		}

		/// <summary>
		/// returns all Components found in the Scene of type T. The returned List can be put back in the pool via ListPool.free.
		/// </summary>
		/// <returns>The components of type.</returns>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public List<T> FindComponentsOfType<T>() where T : ComponentHeadless
		{
			var comps = ListPool<T>.Obtain();
			for (var i = 0; i < _entities.Length; i++)
			{
				if (_entities.Buffer[i].Enabled)
					_entities.Buffer[i].GetComponents<T>(comps);
			}

			foreach (var entity in _entitiesToAdd)
			{
				if (entity.Enabled)
					entity.GetComponents<T>(comps);
			}

			return comps;
		}

		#endregion
	}
}
