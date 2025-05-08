using Sirenix.OdinInspector;
using Sirenix.Utilities;

namespace OnceNull
{
	[System.Serializable]
	public class PredicateWrapper
	{
		[HideLabel, PropertyOrder(-20)]
		public Predicate Predicate;

		[ShowInInspector, DisplayAsString, HideLabel, PropertyOrder(-10), EnableGUI]
		public string TypeName => Predicate.SafeIsUnityNull() ? 
			"(null)" : 
			Predicate.GetType().ToString();
		
		[HorizontalGroup("Row 2")]
		[ToggleLeft]
		public bool DoInvert;
		
		[HorizontalGroup("Row 2")]
		[ReadOnly]
		[ToggleLeft]
		public bool LastResult;
		
		public PredicateWrapper()
		{
			Predicate = null;
			DoInvert = false;
			LastResult = false;
		}
		
		public PredicateWrapper(Predicate predicate)
		{
			Predicate = predicate;
			DoInvert = false;
			LastResult = false;
		}
	}
}