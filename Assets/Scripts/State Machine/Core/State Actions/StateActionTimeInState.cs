using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace OnceNull.FSM
{
	public class StateActionTimeInState : StateAction
	{
		public const string TimeStampStartTag = "Start";
		public const string TimeStampEndTag = "End";
		
		public float EndTime;
		
		[TableList]
		public List<TimeStamp> TimeStamps = new();
		
		[Title("Debug")]
		[ReadOnly]
		public bool IsInState;
		
		
		
		private Dictionary<string, TimeStamp> _timeStampsCache;
		private float _startTime;


		
		[PropertyRange(0,nameof(EndTime))]
		[ShowInInspector, ReadOnly] 
		public float TimeInState => IsInState ? Time.time - _startTime : 0f;


		
		
		private void Awake()
		{
			UpdateCache();
		}


		public override void OnEnter()
		{
			base.OnEnter();
			IsInState = true;
			_startTime = Time.time;
		}
		
		public override void OnExit()
		{
			base.OnExit();
			IsInState = false;
		}
		
		public float GetTime(string timeStampTag)
		{
			if (timeStampTag == TimeStampStartTag)
				return 0;
			
			if (timeStampTag == TimeStampEndTag)
				return EndTime;
			
			if (_timeStampsCache != null &&
			    _timeStampsCache.TryGetValue(timeStampTag, out var timeStamp))
				return timeStamp.Time;

			if (Application.isPlaying)
				UpdateCache();
			else
			{
				foreach (TimeStamp ts in TimeStamps)
				{
					if (ts.Tag == timeStampTag)
						return ts.Time;
				}

				Debug.LogError($"Time stamp not found: {timeStampTag}");
			}
			
			return 0f;
		}


		private void UpdateCache()
		{
			_timeStampsCache = new Dictionary<string, TimeStamp>();
			foreach (var timeStamp in TimeStamps)
			{
				if (!_timeStampsCache.TryAdd(timeStamp.Tag, timeStamp))
					Debug.LogError($"Duplicate time stamp tag: {timeStamp.Tag}");
			}
		}

		#region Editor

		public IEnumerable<string> GetTags()
		{
			var tags = TimeStamps.ConvertAll(x => x.Tag);
			
			tags.Insert(0, TimeStampStartTag);
			tags.Insert(1, TimeStampEndTag);
			
			return tags;
		}

		#endregion
	}
	
	[Serializable]
	[InlineProperty]
	public struct TimeStamp
	{
		[HorizontalGroup]
		public string Tag;
		[HorizontalGroup]
		public float Time;
		
		public TimeStamp(string tag, float time)
		{
			Tag = tag;
			Time = time;
		}
		
		public override string ToString()
		{
			return $"{Tag}: {Time}";
		}
	}
	
}