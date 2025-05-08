using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull.FSM
{
	public class PredicateIsInState : Predicate
	{
		[Required]
		public StateMachine StateMachine;
		public State TargetState;
		
		public override bool Evaluate()
		{
			if (StateMachine == null)
			{
				Debug.LogError($"StateMachine is null on {gameObject.name}");
				return false;
			}

			return StateMachine.CurrentState == TargetState;
		}
	}
}