using UnityEngine;
using UnityEngine.UI;
using CookingPrototype.MessageBrokerEvent;
using UniRx;

namespace CookingPrototype.UI
{
	public sealed class LoseWindow : GameWindow
	{
		[SerializeField] private Button _replayButton;
		[SerializeField] private Button _exitButton;
		[SerializeField] private Button _closeButton;

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