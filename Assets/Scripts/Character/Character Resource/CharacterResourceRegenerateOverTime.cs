using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class CharacterResourceRegenerateOverTime : MonoBehaviour
	{
		[Required]
		public CharacterResourceManager ResourceManager;
		[Required]
		public string ResourceKey;

		[Title("Settings")]
		public float RegenerateTime;
		public float RegenerateAmount;
		[Tooltip("If assigned, will only regenerate if this condition is true")]
		public Predicate Condition;

		[ShowInInspector, ReadOnly]
		private CharacterResource _resource;
		private float _regenerateTimer;

		private void Start()
		{
			_resource = GetComponentInParent<CharacterResourceManager>().GetResource(ResourceKey);
		}

		private void Update()
		{
			RegenerateUpdate();
		}

		private void RegenerateUpdate()
		{
			if (Condition != null && 
			    Condition.Evaluate() == false)
			{
				_regenerateTimer = 0f;
				return;
			}
			
			if (_resource == null) return;

			// check if resource is full
			if (_resource.Amount >= _resource.Max) return;

			// accumulate elapsed time
			_regenerateTimer += Time.deltaTime;

			// check if enough time has passed
			if (_regenerateTimer < RegenerateTime) return;

			_regenerateTimer = 0f;

			// regenerate resource
			_resource.Amount += (int) RegenerateAmount;

			// clamp resource amount
			_resource.Amount = Mathf.Clamp(_resource.Amount, 0, _resource.Max);
		}
	}
}