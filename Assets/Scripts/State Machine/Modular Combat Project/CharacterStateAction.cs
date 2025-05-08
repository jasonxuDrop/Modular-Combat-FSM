using System;
using UnityEngine;

namespace OnceNull.FSM
{
	public class CharacterStateAction : StateAction
	{
		public Character Character;

		protected virtual void Start()
		{
			Character = GetComponentInParent<Character>();
			if (Character == null) Debug.LogError("Character not found in parent objects.");
		}
	}
}