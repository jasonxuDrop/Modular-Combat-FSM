using System.Collections.Generic;
using System.Linq;
using OnceNull.FSM;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OnceNull
{
	public class PredicateComplex : Predicate,
		IReferenceUpdatable
	{
		public enum EvaluationMode { And, Or }
		
		[FoldoutGroup("References")]
		[HideInInlineEditors, ReadOnly, LabelText("State (optional)")]
		[Tooltip("If set, the evaluation result will reset when state is entered. Not required")]
		public State State;

		[ListDrawerSettings(ShowFoldout = false)]
		public List<PredicateWrapper> Predicates;
		
		[PropertySpace]
		public EvaluationMode Mode;
		
		public bool DefaultResult;
		
		[Tooltip("If true once, stay true until enter the state again")]
		public bool DoStayTrue;
		
		[VerticalGroup("Update Frequency")]
		[Tooltip("This is only accurate if the predicate is evaluated every Update")]
		public float UpdateFrequency;
		
		[ReadOnly]
		public bool EvaluationResult;



		private Dictionary<Predicate, PredicateWrapper> _previousPredicates;
		private float _lastUpdateTime;
		private float _nextUpdateTime;


		
		private void Awake()
		{
			if (State != null) 
				State.OnStateEntered += EnteredState;
		}
		
		private void OnDestroy()
		{
			if (State != null) 
				State.OnStateEntered -= EnteredState;
		}

		public override bool Evaluate()
		{
			TryUpdateEvaluation();

			return EvaluationResult;
		}

		private void EnteredState()
		{
			EvaluationResult = false;
			TryUpdateEvaluation(true);
		}

		private void TryUpdateEvaluation(bool doForceUpdate = false)
		{
			if (!doForceUpdate &&
			    Time.time < _nextUpdateTime) 
				return;
			
			if (DoStayTrue && EvaluationResult)
				return;
			
			EvaluationResult = UpdateEvaluationResult();
			
			// update the next update time
			if (UpdateFrequency > 0)
			{
				_lastUpdateTime = Time.time;
				_nextUpdateTime = Time.time + UpdateFrequency;
			}
		}

		private bool UpdateEvaluationResult()
		{
			if (Predicates == null || Predicates.Count == 0)
				return DefaultResult;

			bool result = (Mode == EvaluationMode.And);

			foreach (PredicateWrapper predicate in Predicates)
			{
				var evaluationResult = predicate.Predicate.Evaluate();
				if (predicate.DoInvert) 
					evaluationResult = !evaluationResult;
				predicate.LastResult = evaluationResult;
				
				if (Mode == EvaluationMode.And)
				{
					result &= evaluationResult;
					if (!result) break;
				}
				else if (Mode == EvaluationMode.Or)
				{
					result |= evaluationResult;
					if (result) break;
				}
			}

			return result;
		}



		#region Editor Methods
		
		
		
		[PropertySpace, Button(ButtonSizes.Medium)]
		public void UpdateReferences()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Update Complex Predicate References");
#endif
			
			State = GetComponentInParent<State>();
			_previousPredicates = Predicates
				.Where(p => p.Predicate != null)
				.ToDictionary(
				p => p.Predicate,
				p => p
			);
			Predicates = new List<PredicateWrapper>();
			UpdateReferencesRecursive(transform);
			
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
		
		private void UpdateReferencesRecursive(Transform parent)
		{
			var predicates = parent.GetComponents<Predicate>();

			PredicateComplex predicateComplex = predicates
					.FirstOrDefault(p => p != this && p is PredicateComplex) 
					as PredicateComplex;
			
			// if we find a PredicateComplex, we add it to the list and update its references
			// it will handle its own children
			if (predicateComplex != null)
			{
				Predicates.Add(new PredicateWrapper(predicateComplex));
				return;
			}

			foreach (var predicate in predicates)
			{
				if (predicate == null || 
				    predicate == this || 
				    predicate is PredicateComplex) 
					continue;

				if (!_previousPredicates.TryGetValue(predicate, out var newWrapper))
				{
					newWrapper = new PredicateWrapper(predicate);
				}
				
				Predicates.Add(newWrapper);
			}

			foreach (Transform child in parent)
			{
				UpdateReferencesRecursive(child);
			}
		}
		
		
		
		[VerticalGroup("Update Frequency")]
		[ShowInInspector]
		[ProgressBar("_lastUpdateTime", "_nextUpdateTime")]
		[LabelText(" ")]
		[ReadOnly]
		public float DebugProgress => Time.time;

		
		
		#endregion
	}
}