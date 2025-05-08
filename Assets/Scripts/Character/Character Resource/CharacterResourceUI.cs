using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnceNull
{
	public class CharacterResourceUI : MonoBehaviour
	{
		[Required]
		public CharacterResourceManager ResourceManager;
		public string ResourceKey;

		[PropertySpace]
		public Image FillBar;
		
		public TextMeshProUGUI TextAmount;
		[Tooltip("Format when converting int to string. (eg. \"D4\" for 4 digits, \"F2\" for 2 decimal places)")]
		public string TextFormat;
		public bool DoAppendMaxAmount;
		
		private CharacterResource _resource;
		private bool _isSubscribed;


		private void OnEnable()
		{
			TrySubscribe();
		}
		
		private void OnDisable()
		{
			TryUnsubscribe();
		}

		private void Start()
		{
			if (ResourceManager == null)
			{
				Debug.LogError("ResourceManager is not assigned.", this);
				return;
			}
			
			_resource = ResourceManager?.GetResource(ResourceKey);
			TrySubscribe();
			UpdateUI();
		}
		
		

		[Button]
		private void UpdateUI()
		{
			if (_resource == null) return;
			
			float fillAmount = _resource.Max == 0 ? 
				0 : 
				Mathf.Clamp01(_resource.Amount / (float)_resource.Max);

			// update fill amount
			if (FillBar != null)
			{
				FillBar.fillAmount = fillAmount;
			}
			
			// update text amount to minimum 4 digits
			if (TextAmount != null)
			{
				TextAmount.text = $"{_resource.Amount.ToString(TextFormat)}";
				if (DoAppendMaxAmount)
					TextAmount.text += $"/{_resource.Max.ToString(TextFormat)}";
			}
		}

		private void TrySubscribe()
		{
			if (_isSubscribed) return;
			if (_resource == null) return;
			_resource.OnAmountChanged += UpdateUI;
			_isSubscribed = true;
		}
		
		private void TryUnsubscribe()
		{
			if (_resource == null) return;
			if (_isSubscribed == false) return;
			_resource.OnAmountChanged -= UpdateUI;
			_isSubscribed = false;
		}
		
	}
}