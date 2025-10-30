using CookingPrototype.MessageBrokerEvent;
using CookingPrototype.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GameStartWindow : GameWindow
{
	[SerializeField] private Button _startGame;

	protected override void Init()
	{
		_startGame.onClick.AddListener(OnStartGame);
	}

	private void OnStartGame()
	{
		MessageBroker.Default.Publish(new M_StartGame());
		gameObject.SetActive(false);
	}
}