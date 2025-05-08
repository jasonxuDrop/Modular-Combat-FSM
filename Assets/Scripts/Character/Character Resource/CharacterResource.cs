using System;
using UnityEngine;

namespace OnceNull
{
	[System.Serializable]
	public class CharacterResource
	{
		[SerializeField]
		private int _amount;
		
		[SerializeField]
		private int _max;
		
		
		
		public int Amount 
		{
			get => _amount;
			set
			{
				_amount = value;
				OnAmountChanged?.Invoke();
			}
		}

		public int Max => _max;


		public event Action OnAmountChanged;
	}
}