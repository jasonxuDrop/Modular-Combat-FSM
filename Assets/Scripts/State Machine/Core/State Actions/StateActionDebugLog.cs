using UnityEngine;

namespace OnceNull.FSM
{
	public class StateActionDebugLog : StateAction
	{
		public override void OnEnter()
		{
			Debug.Log("StateActionDebugLog: OnEnter");
		}
	}
}