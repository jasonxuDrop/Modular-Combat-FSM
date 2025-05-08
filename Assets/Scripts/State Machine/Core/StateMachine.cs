using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace OnceNull.FSM
{
	public class StateMachine : MonoBehaviour,
		IReferenceUpdatable
	{
		public event Action<StateMachineEvent> OnStateMachineEvent;
		
		public List<State> States;
		public List<StateTransition> AnyTransitions;
		public Transform AnyTransitionRootTf;
		[ToggleLeft]
		public bool DoAllowSameStateTransition = true;
		
		[FormerlySerializedAs("Current")]
		[Title("Info")]
		[ValueDropdown(nameof(GetChildrenStates), AppendNextDrawer = true)]
		[InlineEditor]
		public State CurrentState;

		[Title("Debug")]
		public bool DoDebug;
		
		
		
		private StateMachineEvent _stateMachineEvent;
		
		

		private void OnDisable()
		{
			// Exit current state
			ExitState();
			InvokeEvent(StateMachineEventType.Disable, CurrentState);
		}

		private void OnEnable()
		{
			if (CurrentState != null)
			{
				EnterState();
			}
			// If no current state is set, set the first state in the list as the current state
			else if (CurrentState == null && 
			    States.Count > 0)
			{
				ChangeState(States[0]);
			}
			InvokeEvent(StateMachineEventType.Enable, CurrentState);
		}

		private void Update()
		{
			foreach (var action in CurrentState?.StateActions) action.OnUpdate();

			UpdateCheckTransition();
		}
		
		private void LateUpdate()
		{
			foreach (var action in CurrentState?.StateActions) action.OnLateUpdate();
		}

		private void FixedUpdate()
		{
			foreach (var action in CurrentState?.StateActions) action.OnFixedUpdate();
		}

		/// <summary>
		/// Update the state machine's current state.
		/// </summary>
		/// <param name="toState"></param>
		[Button]
		public void ChangeState(State toState)
		{
			// if already in state, do nothing
			if (!DoAllowSameStateTransition && 
			    CurrentState == toState)
				return;

			// if state is defined in the scope of the state machine, do nothing
			if (toState && !States.Contains(toState))
			{
				Debug.LogError($"State {toState.name} is not a valid state in this state machine.", toState);
				return;
			}
			
			if (DoDebug) Debug.Log($"Changing state: {CurrentState?.name} -> {toState.name}", this);

			InvokeEvent(StateMachineEventType.ChangeStateBegin, CurrentState);
			
			// Exit current state
			ExitState();
			
			// Enter new state
			CurrentState = toState;
			EnterState();
			
			InvokeEvent(StateMachineEventType.ChangeStateComplete, CurrentState);
		}

		public void InvokeEvent(StateMachineEventType eventType, State state = null)
		{
			_stateMachineEvent.Type = eventType;
			_stateMachineEvent.State = state;
			OnStateMachineEvent?.Invoke(_stateMachineEvent);
		}
		
		
		
		private void UpdateCheckTransition()
		{
			var transition = GetTransition();
			if (transition != null)
			{
				ChangeState(transition.TargetState);
			}
		}
		
		private void EnterState()
		{
			if (CurrentState == null) return;
			
			CurrentState.EnterState();
				
			InvokeEvent(StateMachineEventType.EnteredState, CurrentState);
		}

		private void ExitState()
		{
			if (CurrentState == null) return;
			
			CurrentState.ExitState();
				
			InvokeEvent(StateMachineEventType.ExitedState, CurrentState);
		}

		/// <summary>
		/// Evaluate all transitions and return the first one that is valid.
		/// </summary>
		/// <returns>The first valid transition.</returns>
		private StateTransition GetTransition()
		{
			// Evaluate any transitions first
			foreach (var transition in AnyTransitions)
				if (transition.Predicate.Evaluate())
					return transition;
			
			// Evaluate current state transitions
			foreach (var transition in CurrentState.StateTransitions)
				if (transition.Predicate.Evaluate())
					return transition;
			
			return null;
		}


		#region Editor Methods

		/// <summary>
		/// Update the references of the state machine. 
		/// </summary>
		[PropertySpace, Button(ButtonSizes.Medium)]
		public void UpdateReferences()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Update State Machine References");
#endif

			// Update any transitions
			var anyTransitions = AnyTransitionRootTf.GetComponentsInChildren<StateTransition>();
			AnyTransitions = new List<StateTransition>(anyTransitions);
			AnyTransitions.ForEach(transition => transition.UpdateReferences()); // update transition references
			
			// Update all states
			var statesInChildren = GetComponentsInChildren<State>();
			States = new List<State>(statesInChildren);
			States.ForEach(state =>
			{
				// state.UpdateReferences();
				state.StateTransitions.ForEach(transition => transition.UpdateReferences()); // also update transitions
			});
			
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}

		public IEnumerable<State> GetChildrenStates()
		{
			return States;
		}

		#endregion
	}
}