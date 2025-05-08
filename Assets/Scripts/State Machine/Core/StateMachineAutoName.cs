using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OnceNull.FSM
{
	/// <summary>
	/// This is a helper class to automatically name the state machine nodes in the inspector.
	/// This will require a state machine to work. But state machines can work without this class.
	/// </summary>
	
	[DefaultExecutionOrder(-100)]
	public class StateMachineAutoName : MonoBehaviour
	{
		[Required]
		public StateMachine StateMachine;
	
		[FoldoutGroup("Naming Convention")]
		public string StatePrefix = "State ";
		[FoldoutGroup("Naming Convention")]
		public string TransitionPrefix = "Transition ";
		[FoldoutGroup("Naming Convention")]
		public string AnyTransitionPrefix = "Any Transition ";
		[FoldoutGroup("Naming Convention")]
		public string StateSeparator = " -> ";
		
		[PropertySpace]
		[FoldoutGroup("Naming Convention")]
		public bool DoLiveRename = true;
		[FoldoutGroup("Naming Convention")]
		public string LiveStatePrefix = "@CURRENT ";


		private void OnEnable()
		{
			if (StateMachine == null)
			{
				Debug.LogError("StateMachine is not assigned.");
				return;
			}

			if (DoLiveRename)
				StateMachine.OnStateMachineEvent += HandleStateMachineEvent;
		}
		
		private void OnDisable()
		{
			if (DoLiveRename)
				StateMachine.OnStateMachineEvent -= HandleStateMachineEvent;
		}
		
		

		private void HandleStateMachineEvent(StateMachineEvent e)
		{
			if (e.Type == StateMachineEventType.ExitedState)
			{
				if (e.State != null && e.State.name.StartsWith(LiveStatePrefix))
				{
					e.State.name = e.State.name.Substring(LiveStatePrefix.Length);
				}
			}
			else if (e.Type == StateMachineEventType.EnteredState)
			{
				if (e.State != null && !e.State.name.StartsWith(LiveStatePrefix))
				{
					e.State.name = LiveStatePrefix + e.State.name;
				}
			}
		}
		
		
		
		[Button(ButtonSizes.Medium)]
		public void AutoName()
		{
			if (Application.isPlaying) return;
			
#if UNITY_EDITOR
			if (StateMachine == null)
			{
				Debug.LogError("StateMachine is not assigned.");
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(StateMachine.gameObject, "Auto Name States");
#endif

			foreach (var state in StateMachine.States)
			{
				if (state.gameObject.name.StartsWith(StatePrefix)) continue;

#if UNITY_EDITOR
				Undo.RegisterFullObjectHierarchyUndo(state.gameObject, "Auto Name State");
#endif
				state.gameObject.name = StatePrefix + state.gameObject.name;
			}

			foreach (var state in StateMachine.States)
			{
				foreach (var stateTransition in state.StateTransitions)
				{
#if UNITY_EDITOR
					Undo.RegisterFullObjectHierarchyUndo(stateTransition.gameObject, "Auto Name State Transition");
#endif
					stateTransition.gameObject.name = TransitionPrefix
					                                  + state.gameObject.name.Substring(StatePrefix.Length)
					                                  + StateSeparator
					                                  + stateTransition.TargetState.gameObject.name.Substring(StatePrefix.Length);
				}
			}

			foreach (var transition in StateMachine.AnyTransitions)
			{
#if UNITY_EDITOR
				Undo.RegisterFullObjectHierarchyUndo(transition.gameObject, "Auto Name Any Transition");
#endif
				transition.gameObject.name = AnyTransitionPrefix
				                             + StateSeparator
				                             + transition.TargetState.gameObject.name.Substring(StatePrefix.Length);
			}

#if UNITY_EDITOR
			EditorUtility.SetDirty(StateMachine);
#endif
		}

		
		
		[Button]
		public void StripNamePrefixes()
		{
			if (Application.isPlaying) return;
			
#if UNITY_EDITOR
			if (StateMachine == null)
			{
				Debug.LogError("StateMachine is not assigned.");
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(StateMachine.gameObject, "Strip Name Prefixes");
#endif

			foreach (var state in StateMachine.States)
			{
				if (!state.gameObject.name.StartsWith(StatePrefix)) continue;

#if UNITY_EDITOR
				Undo.RegisterFullObjectHierarchyUndo(state.gameObject, "Strip State Name Prefix");
#endif
				state.gameObject.name = state.gameObject.name.Substring(StatePrefix.Length);
			}

			foreach (var state in StateMachine.States)
			{
				foreach (var stateTransition in state.StateTransitions)
				{
					if (!stateTransition.gameObject.name.StartsWith(TransitionPrefix)) continue;

#if UNITY_EDITOR
					Undo.RegisterFullObjectHierarchyUndo(stateTransition.gameObject, "Strip State Transition Name Prefix");
#endif
					stateTransition.gameObject.name = stateTransition.gameObject.name.Substring(TransitionPrefix.Length);
				}
			}

			foreach (var transition in StateMachine.AnyTransitions)
			{
				if (!transition.gameObject.name.StartsWith(AnyTransitionPrefix)) continue;

#if UNITY_EDITOR
				Undo.RegisterFullObjectHierarchyUndo(transition.gameObject, "Strip Any Transition Name Prefix");
#endif
				transition.gameObject.name = transition.gameObject.name.Substring(AnyTransitionPrefix.Length);
			}

#if UNITY_EDITOR
			EditorUtility.SetDirty(StateMachine);
#endif
		}
	}
}