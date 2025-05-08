using UnityEngine;

namespace OnceNull.FSM
{
	public struct StateMachineEvent 
	{
		public StateMachineEventType Type;
		public State State;
		
		public StateMachineEvent(StateMachineEventType eventType, State state)
		{
			Type = eventType;
			State = state;
		}
	}
	
	public enum StateMachineEventType
	{
		Enable,
		Disable,
		EnteredState,
		ExitedState,
		ChangeStateBegin,
		ChangeStateComplete,
	}
}