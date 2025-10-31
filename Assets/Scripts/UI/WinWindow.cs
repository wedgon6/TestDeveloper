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