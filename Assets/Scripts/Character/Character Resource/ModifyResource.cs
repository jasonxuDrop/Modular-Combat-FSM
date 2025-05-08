using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class ModifyResource : MonoBehaviour
	{
		[Required]
		public Character Character;
		
		[Required]
		public string ResourceKey;
		
		[PropertySpace]
		
		public int ModifyAmount;

		[Tooltip("If true, the resource will be cached on start. Don't use this if the resource can be added or " +
		         "removed during gameplay.")]
		public bool DoCacheResource = true;
		
		public bool DoClamp = true;
		
		[Tooltip("If true, debug messages will be printed to the console.")]
		public bool DoDebug;

		
		
		private CharacterResourceManager _resourceManager;
		private CharacterResource _resource;

		private void Start()
		{
			_resourceManager = Character.GetAbility<CharacterResourceManager>();
			if (_resourceManager == null)
			{
				Debug.LogError($"CharacterResourceManager not found on {Character.name}");
				return;
			}
			
			if (DoCacheResource)
			{
				_resource = _resourceManager?.GetResource(ResourceKey);
				if (_resource == null)
				{
					Debug.LogError($"Resource with key {ResourceKey} not found on {Character.name}");
				}
			}
		}

		[Button]
		public void Modify()
		{
			if (_resource == null)
			{
				_resource = _resourceManager?.GetResource(ResourceKey);
				if (_resource == null)
				{
					Debug.LogError($"Resource with key {ResourceKey} not found on {Character.name}");
					return;
				}
			}

			_resource.Amount += ModifyAmount;
			if (DoClamp)
			{
				_resource.Amount = Mathf.Clamp(_resource.Amount, 0, _resource.Max);
			}
			
			if (DoDebug) Debug.Log($"Modified resource: {ResourceKey} by {ModifyAmount} => {_resource.Amount}");
		}
	}
}