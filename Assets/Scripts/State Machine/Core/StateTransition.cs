using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OnceNull.FSM
{
	public class StateTransition : MonoBehaviour,
		IReferenceUpdatable
	{
		
		[FoldoutGroup("References")]
		[HideInInlineEditors, ReadOnly]
		public StateMachine StateMachine;
		
		[ValueDropdown(nameof(GetAvailableStates), AppendNextDrawer = true), Required]
		public State TargetState;
		[Required]
		[InlineButton(nameof(AddComplexPredicate), ShowIf = "@Predicate == null")]
		public Predicate Predicate;
		
		
		#region Editor Methods
		
		/// <summary>
		/// Automatically update the references of the state node.
		/// Usually called from the state machine's method
		/// </summary>
		[PropertySpace, Button(ButtonSizes.Medium)]
		public void UpdateReferences()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Update Transition References");
#endif

			if (StateMachine == null)
				StateMachine = GetComponentInParent<StateMachine>();
			if (Predicate == null)
				Predicate = GetComponent<Predicate>();
			
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
		
		public void AddComplexPredicate()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Create Complex Predicate");
#endif
			if (Predicate == null)
				Predicate = gameObject.AddComponent<PredicateComplex>();
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}

		public IEnumerable<State> GetAvailableStates()
		{
			return StateMachine?.GetChildrenStates();
		}
		
		#endregion
		
	}
}