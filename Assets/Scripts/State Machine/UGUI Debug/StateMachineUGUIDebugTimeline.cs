using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace OnceNull.FSM
{
	public class StateMachineUGUIDebugTimeline : MonoBehaviour
	{
		public StateMachine StateMachine;
		
		public RectTransform TimelineRectTransform;
		public Image TimelineFillImage;
		public StateMachineUGUIDebugTimelineTimeStamp TimeStampPrefab;
		
		public float HeightPerSecond;
		public Vector2 TimeStampAnchoredPositionOffset;
		
		private readonly List<StateMachineUGUIDebugTimelineTimeStamp> _timeStamps = new();
		private StateActionTimeInState _timeInState;
		private float _timeInStateDuration;


		private void OnEnable()
		{
			StateMachine.OnStateMachineEvent += StateMachineEventHandler;
		}
		
		private void OnDisable()
		{
			StateMachine.OnStateMachineEvent -= StateMachineEventHandler;
		}
		
		private void Start()
		{
			if (StateMachine.CurrentState != null)
			{
				HandleStateChange(StateMachine.CurrentState);
			}
		}

		private void Update()
		{
			if (StateMachine.CurrentState != null && 
			    _timeInState != null)
			{
				TimelineFillImage.fillAmount = _timeInState.TimeInState / _timeInStateDuration;
			}
		}

		private void StateMachineEventHandler(StateMachineEvent e)
		{
			if (e.Type == StateMachineEventType.EnteredState)
			{
				HandleStateChange(e.State);
			}
		}
		


		[Button]
		private void HandleStateChange(State state)
		{
			if (state == null)
				return;

			_timeInState = state.GetAction<StateActionTimeInState>();
			if (_timeInState == null)
			{
				TimelineRectTransform.gameObject.SetActive(false);
				_timeStamps.ForEach(ts => ts.gameObject.SetActive(false));
				TimelineFillImage.fillAmount = 0;
				return;
			}
			
			TimelineRectTransform.gameObject.SetActive(true);
			_timeInStateDuration = _timeInState.EndTime;
			var timeStamps = _timeInState.TimeStamps;
			
			// adjust the height of the timeline
			var sizeDelta = TimelineRectTransform.sizeDelta;
			sizeDelta.y = HeightPerSecond * _timeInStateDuration;
			TimelineRectTransform.sizeDelta = sizeDelta;

			// ensure there are enough time stamps
			while (_timeStamps.Count < timeStamps.Count)
			{
				var newTimeStamp = Instantiate(TimeStampPrefab, transform);
				newTimeStamp.gameObject.SetActive(true);
				_timeStamps.Add(newTimeStamp);
			}
			
			// place each time stamp at the correct position
			for (int i = 0; i < _timeStamps.Count; i++)
			{
				var timeStampInstance = _timeStamps[i];
				if (i < timeStamps.Count)
				{
					timeStampInstance.gameObject.SetActive(true);
	
					var tsRectTransform = timeStampInstance.transform as RectTransform;
					tsRectTransform.anchoredPosition = 
						TimeStampAnchoredPositionOffset + 
						new Vector2(0, -HeightPerSecond * timeStamps[i].Time);

					timeStampInstance.TextLabel.text = timeStamps[i].Tag;
				}
				else
				{
					timeStampInstance.gameObject.SetActive(false);
				}
			}
		}
		
	}
}