using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class CharacterResourceManager : CharacterAbility
	{
		public List<StringCharacterResourcePair> DefaultResources;
		
		private readonly Dictionary<string, CharacterResource> _resources = new();
		
		private void Awake()
		{
			foreach (var resource in DefaultResources)
			{
				if (!_resources.TryAdd(resource.Key, resource.Value))
					Debug.LogError("Resource with the same key already exists: " + resource.Key, this);
			}
		}
		
		public CharacterResource GetResource(string key)
		{
			if (_resources.TryGetValue(key, out var resource))
			{
				return resource;
			}

			Debug.LogError($"Resource with key {key} not found.", this);
			return null;
		}
	}

	[System.Serializable]
	public struct StringCharacterResourcePair
	{
		public string Key;
		[HideLabel, InlineProperty, PropertySpace(10)]
		public CharacterResource Value;

		public StringCharacterResourcePair(string key, CharacterResource value)
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Key}: {Value.Amount}/{Value.Max}";
		}
	}
}