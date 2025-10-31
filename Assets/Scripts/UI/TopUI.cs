using CookingPrototype.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.UI
{
	public sealed class TopUI : MonoBehaviour
	{
		public Image OrdersBar = null;
		public TMP_Text OrdersCountText = null;
		public TMP_Text CustomersCountText = null;

		private void Start()
		{
			GameplayController.Instance.TotalOrdersServedChanged += OnOrdersChanged;
			CustomersController.Instance.TotalCustomersGeneratedChanged += OnCustomersChanged;
			OnOrdersChanged();
			OnCustomersChanged();
		}

		private void OnDestroy()
		{
			if ( GameplayController.Instance )
			{
				GameplayController.Instance.TotalOrdersServedChanged -= OnOrdersChanged;
			}

			if ( CustomersController.Instance )
			{
				CustomersController.Instance.TotalCustomersGeneratedChanged -= OnCustomersChanged;
			}
		}

		private void OnCustomersChanged()
		{
			var cc = CustomersController.Instance;
			CustomersCountText.text = (cc.CustomersTargetNumber - cc.TotalCustomersGenerated).ToString();
		}

		private void OnOrdersChanged()
		{
			var gc = GameplayController.Instance;
			OrdersCountText.text = $"{gc.TotalOrdersServed}/{gc.OrdersTarget}";
			OrdersBar.fillAmount = (float)gc.TotalOrdersServed / gc.OrdersTarget;
		}
	}
}