using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OnceNull.FSM
{
	public class State : MonoBehaviour,
		IReferenceUpdatable
	{
		public event Action OnStateEntered;
		public event Action OnStateExited;
		
		
		
		[FoldoutGroup("References")]
		[HideInInlineEditors, ReadOnly]
		public StateMachine StateMachine;
		
		[ListDrawerSettings(ShowFoldout = false)]
		public List<StateAction> StateActions;
		[ListDrawerSettings(ShowFoldout = false)]
		public List<StateTransition> StateTransitions;

		
		
		public void EnterState()
		{
			StateActions.ForEach(state => state.OnEnter());
			OnStateEntered?.Invoke();
		}
		
		public void ExitState()
		{
			StateActions.ForEach(state => state.OnExit());
			OnStateExited?.Invoke();
		}

		public T GetAction<T>()
		{
			foreach (var action in StateActions)
			{
				if (action is T t)
					return t;
			}

			return default;
		}
		
		
		
		#region Editor Methods
		
		/// <summary>
		/// Automatically update the references of the state node.
		/// Usually called from the state machine's method
		/// </summary>
		[PropertySpace, Button(ButtonSizes.Medium)]
		public void UpdateReferences()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Update State References");
#endif

			if (StateMachine == null)
				StateMachine = GetComponentInParent<StateMachine>();
			
			var actions = GetComponentsInChildren<StateAction>();
			var transitions = GetComponentsInChildren<StateTransition>();
			
			StateActions = new List<StateAction>(actions);
			StateTransitions = new List<StateTransition>(transitions);
			
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
		
		#endregion
	}
}