using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class CharacterAbilityAnimationController : CharacterAbility
	{
		public List<Animator> Animators;

		private Dictionary<string, int> _stateNameHashes;
		
		private void Awake()
		{
			GenerateCache();
		}

		
		
		[Button]
		public void PlayAnimation(string stateName)
		{
			if (_stateNameHashes == null)
			{
				GenerateCache();
			}
			
			if (_stateNameHashes == null)
			{
				Debug.LogWarning("State name hashes not generated. Unable to play animation.");
				return;
			}

			if (_stateNameHashes.TryGetValue(stateName, out var hash))
			{
				foreach (var animator in Animators)
				{
					animator.Play(hash, 0, 0f);
					animator.Update(0f);
				}
			}
			else
			{
				Debug.LogWarning($"State name '{stateName}' not found in cache.");
			}
		}
		
		private void GenerateCache()
		{
			_stateNameHashes = new Dictionary<string, int>();
			foreach (var animator in Animators)
			{
				foreach (var clip in animator.runtimeAnimatorController.animationClips)
				{
					if (!_stateNameHashes.ContainsKey(clip.name))
					{
						_stateNameHashes.Add(clip.name, Animator.StringToHash(clip.name));
					}
				}
			}
		}

		
		
		#region Editor

		public IEnumerable<string> GetAvailableStateNames()
		{
			if (_stateNameHashes == null)
			{
				GenerateCache();
			}

			return _stateNameHashes?.Keys;
		}

		#endregion
	}
}