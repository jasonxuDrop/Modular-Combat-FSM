using UnityEngine;

namespace OnceNull.FSM
{
	public class StateActionChangeState : StateAction
	{
		public StateMachine StateMachine;
		public State TargetState;
		
		public bool DoChangeToOriginalStateOnExit = true;

		private State _originalState;
		
		public override void OnEnter()
		{
			base.OnEnter();
			
			_originalState = StateMachine.CurrentState;
			StateMachine.ChangeState(TargetState);
		}

		public override void OnExit()
		{
			base.OnExit();
			
			if (DoChangeToOriginalStateOnExit)
			{
				StateMachine.ChangeState(_originalState);
			}
		}
	}
}