using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class Character : MonoBehaviour
	{

		[TitleGroup("References")]
		[Header("Abilities")]
		[PropertyOrder(100)]
		public List<GameObject> AbilityParents;

		[TitleGroup("References")]
		[ShowInInspector, ReadOnly]
		[PropertyOrder(101)]
		private Dictionary<Type, CharacterAbility> _abilities;


		private void Awake()
		{
			_abilities = new Dictionary<Type, CharacterAbility>();

			foreach (var parent in AbilityParents)
			{
				var abilities = parent.GetComponentsInChildren<CharacterAbility>(true);
				foreach (var ability in abilities)
				{
					RegisterAbility(ability);
				}
			}
		}

		
		
		/// <summary>
		/// Call this method after awake to get the ability of type T.
		/// Returns null and throws an error if the ability is not found.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetAbility<T>() where T : CharacterAbility
		{
			if (_abilities.TryGetValue(typeof(T), out var foundAbility))
			{
				return (T)foundAbility;
			}

			Debug.LogError($"Ability of type {typeof(T)} not found on character.", this);
			return null;
		}
		
		private void RegisterAbility(CharacterAbility ability)
		{
			if (_abilities.TryAdd(ability.GetType(), ability))
			{
				ability.Character = this;
			}
			else 
				Debug.LogError($"Duplicate ability found: {ability.name}", ability);
		}
	}
}