namespace OnceNull.FSM
{
	public class PredicateBool : Predicate
	{
		public bool Value;

		public override bool Evaluate()
		{
			return Value;
		}
	}
}