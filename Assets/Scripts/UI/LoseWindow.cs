using UnityEngine;
using UnityEngine.UI;
using CookingPrototype.Controllers;

using TMPro;
using CookingPrototype.MessageBrokerEvent;
using UniRx;

namespace CookingPrototype.UI
{
	public sealed class LoseWindow : GameWindow
	{
		[SerializeField] private Button _replayButton;
		[SerializeField] private Button _exitButton;
		[SerializeField] private Button _closeButton;

		//public override void Show(int totalOrdersServed, int ordersTarget)
		//{
		//	base.Show(totalOrdersServed, ordersTarget);
		//	//	_goalText.text = $"{totalOrdersServed}/{ordersTarget}";
		//	//	_goalBar.fillAmount = Mathf.Clamp01((float)totalOrdersServed / ordersTarget);
		//	//	gameObject.SetActive(true);
		//}

		protected override void Init()
		{
			_replayButton.onClick.AddListener(OnRestart);
			_exitButton.onClick.AddListener(OnGameClose);
			_closeButton.onClick.AddListener(OnGameClose);
		}

		private void OnGameClose()
		{
			MessageBroker.Default.Publish(new M_GameClose());
		}

		private void OnRestart()
		{
			MessageBroker.Default.Publish(new M_GameRestart());
		}

	}
}