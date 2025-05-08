using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull.FSM
{
	public class PredicateHasResource : Predicate
	{
		public Character Character;
		
		[PropertySpace]
		
		[Required]
		public string ResourceKey;
		
		public int RequiredAmount;

		[Tooltip("If true, the resource will be cached on start. Don't use this if the resource can be added or " +
		         "removed during gameplay.")]
		public bool DoCacheResource = true;
		
		[Tooltip("If true, debug messages will be printed to the console.")]
		public bool DoDebug;
		

		
		private CharacterResourceManager _resourceManager;
		private CharacterResource _resource;
		
		
		
		private void Start()
		{
			if (Character == null)
			{
				Character = GetComponentInParent<Character>();
			}
			_resourceManager = Character.GetAbility<CharacterResourceManager>();
			if (_resourceManager == null)
			{
				Debug.LogError($"CharacterResourceManager not found on {Character.name}");
				return;
			}
			
			if (DoCacheResource)
			{
				_resource = _resourceManager.GetResource(ResourceKey);
				if (_resource == null)
				{
					Debug.LogError($"Resource with key {ResourceKey} not found on {Character.name}");
				}
			}
		}

		public override bool Evaluate()
		{
			var resource = DoCacheResource ? _resource : _resourceManager.GetResource(ResourceKey);
			if (resource == null)
			{
				Debug.LogError($"Resource with key {ResourceKey} not found on {Character.name}");
				return false;
			}
			
			if (resource.Amount >= RequiredAmount)
			{
				if (DoDebug) Debug.Log($"Resource {ResourceKey} has enough amount: {resource.Amount}/{RequiredAmount}");
				return true;
			}
			else
			{
				if (DoDebug) Debug.Log($"Resource {ResourceKey} does not have enough amount: {resource.Amount}/{RequiredAmount}");
				return false;
			}
		}
	}
}