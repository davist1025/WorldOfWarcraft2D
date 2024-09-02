using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Nez.AI.BehaviorTree
{
	/// <summary>
	/// A data store of properties for the behavior tree to make decisions from.
	/// </summary>
	public class Blackboard : IEnumerable
	{
		private Dictionary<string, object> _data = new Dictionary<string, object>();

		public int Length
		{
			get => _data.Count;
		}

		public object this[string key]
		{
			get => Get(key);
			set => Set(key, value);
		}

		/// <summary>
		/// Determines if the given key for an object is already set.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private bool IsSet(string key)
		{
			return _data.ContainsKey(key);
		}

		private void Set(string key, object value)
		{
			if (IsSet(key))
			{
				if (_data[key].GetType() == value.GetType())
					_data[key] = value;
				else
					Debug.Error($"Unable to set an object of type: {_data[key].GetType()} to a new object of type: {value.GetType()}");
				return;
			}
			_data[key] = value;
		}

		/// <summary>
		/// Attempts to get an object of the given type by key.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public T Get<T>(string key)
		{
			object val = Get(key);

			if (val == null)
				return default(T);

			return (T)val;
		}

		private object Get(string key)
		{
			if (!IsSet(key))
				return null;

			return _data[key];
		}

		public IEnumerator GetEnumerator()
		{
			foreach (var kv in _data)
			{
				yield return kv;
			}
		}

		public void Remove(string key)
			=> _data.Remove(key);
	}
}
