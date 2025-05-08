using UnityEngine;

namespace OnceNull.FSM
{
	public abstract class StateAction : MonoBehaviour
	{
		public virtual void OnEnter() { }
		public virtual void OnExit() { }
		public virtual void OnUpdate() { }
		public virtual void OnLateUpdate() { }
		public virtual void OnFixedUpdate() { }
	}
}