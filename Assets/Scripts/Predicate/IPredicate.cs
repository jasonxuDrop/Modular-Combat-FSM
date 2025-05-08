namespace OnceNull
{
	/// <summary>
	/// Predicate contains a <see cref="Evaluate"/> function that tests a condition
	/// and then return a True or False value. 
	/// </summary>
	public interface IPredicate
	{
		bool Evaluate();
	}
}