using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull.FSM
{
	public class StateMachineUGUIDebugManager : MonoBehaviour
	{
		[Required]
		public StateMachine StateMachine;

		public List<UGUIDebugStateData> StateData;

		private Dictionary<State, UGUIDebugStateData> _stateToDataCache;

		private void Awake()
		{
			_stateToDataCache = new();
			foreach (var data in StateData)
			{
				if (data.State != null)
				{
					_stateToDataCache[data.State] = data;
					data.UGUIDebugState.State = data.State;
				}
			}
		}
		
		

		[Button]
		private void UpdateReference()
		{
			if (StateMachine == null)
			{
				Debug.LogError("StateMachine not found.");
				return;
			}

			foreach (var data in StateData)
			{
				data.UGUIDebugState.State = data.State;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(data.UGUIDebugState);
#endif
				data.UGUIDebugState.StateMachine = StateMachine;
			}
		}
	}

	[System.Serializable]
	public class UGUIDebugStateData
	{
		public State State;
		public StateMachineUGUIDebugState UGUIDebugState;
	}
}