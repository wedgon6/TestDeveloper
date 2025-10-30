using UnityEngine;
using UnityEngine.UI;
using UniRx;
using CookingPrototype.MessageBrokerEvent;

namespace CookingPrototype.UI
{
	public sealed class WinWindow : GameWindow
	{
		[SerializeField] private Button _okButton;
		[SerializeField] private Button _closeButton;

		//public override void Show(int totalOrdersServed, int ordersTarget)
		//{
		//	base.Show(totalOrdersServed, ordersTarget);

		//	_goalText.text = $"{totalOrdersServed}/{ordersTarget}";
		//	_goalBar.fillAmount = Mathf.Clamp01((float)totalOrdersServed / ordersTarget);
		//	gameObject.SetActive(true);
		//}

		protected override void Init()
		{
			_okButton.onClick.AddListener(OnGameClose);
			_closeButton.onClick.AddListener(OnGameClose);
		}

		private void OnGameClose()
		{
			MessageBroker.Default.Publish(new M_GameClose());
		}
	}
}