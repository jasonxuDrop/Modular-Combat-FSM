using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace OnceNull.FSM
{
	public class PredicateTimeInStateRange : Predicate,
		IReferenceUpdatable
	{
		public State State;
		public StateActionTimeInState TimeInState;
		
		/// <summary>
		/// The start time stamp tag, inclusive.
		/// if empty, no lower bound
		/// </summary>
		[ValueDropdown(nameof(GetTags), AppendNextDrawer = true), GUIColor("TagValidityColor")]
		public string StartTimeStampTag;
		
		/// <summary>
		/// The end time stamp tag, inclusive.
		/// if empty, no upper bound
		/// </summary>
		[ValueDropdown(nameof(GetTags), AppendNextDrawer = true), GUIColor("TagValidityColor")]
		public string EndTimeStampTag;

		
		private float _startTime;
		private float _endTime;
		

		[ShowInInspector, ReadOnly, HideLabel, MinMaxSlider(0, "EndTime", true), PropertySpace(10)]
		public Vector2 DebugTimeRange => new Vector2(
			string.IsNullOrEmpty(StartTimeStampTag) ? 0 :TimeInState?.GetTime(StartTimeStampTag) ?? 0,
			string.IsNullOrEmpty(EndTimeStampTag) ? EndTime : TimeInState?.GetTime(EndTimeStampTag) ?? EndTime
		);

		private float EndTime => TimeInState?.EndTime ?? 0;

		private void Start()
		{
			_startTime = TimeInState.GetTime(EndTimeStampTag);
			_endTime = TimeInState.GetTime(StartTimeStampTag);
		}

		public override bool Evaluate()
		{
			if (TimeInState == null)
			{
				Debug.LogError($"PredicateTimeInStateRange: No StateActionTimeInState found in state {State.name}.");
				return false;
			}
			
			if (string.IsNullOrEmpty(StartTimeStampTag) && string.IsNullOrEmpty(EndTimeStampTag))
				return true;

			if (string.IsNullOrEmpty(StartTimeStampTag)) 
				return TimeInState.TimeInState <= _startTime;

			if (string.IsNullOrEmpty(EndTimeStampTag))
				return TimeInState.TimeInState >= _endTime;
			
			return TimeInState.TimeInState >= _endTime && 
			       TimeInState.TimeInState <= _startTime;
		}



		#region Editor

		[Button]
		public void UpdateReferences()
		{
#if UNITY_EDITOR
			Undo.RecordObject(this, "Update PredicateTimeInStateRange References");
#endif
			
			State = GetComponentInParent<State>();
			if (State == null)
			{
				Debug.LogError($"PredicateTimeInStateRange: No State component found in parent.");
				return;
			}

			TimeInState = State.GetAction<StateActionTimeInState>();

#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
#endif
		}
		
		private IEnumerable<string> GetTags()
		{
			return TimeInState?.GetTags();
		}
		
		private readonly Color _validTagColor = Color.white;
		private readonly Color _invalidTagColor = new Color(1f, 0.81f, 0.62f, 1f);
		
		private Color TagValidityColor(string tagName)
		{
			if (TimeInState == null || 
			    string.IsNullOrEmpty(tagName) || 
			    TimeInState.GetTags().Contains(tagName))
				return _validTagColor;

			return _invalidTagColor;
		}

		#endregion

	}
}