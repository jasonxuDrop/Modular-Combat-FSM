using UnityEngine;

namespace OnceNull
{
	public abstract class Predicate : MonoBehaviour, IPredicate
	{
		public abstract bool Evaluate();
	}
}