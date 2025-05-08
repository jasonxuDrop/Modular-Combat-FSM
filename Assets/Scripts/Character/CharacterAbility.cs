using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class CharacterAbility : MonoBehaviour
	{
		[ReadOnly]
		[Tooltip("Assigned from Character")]
		public Character Character;
	}
}