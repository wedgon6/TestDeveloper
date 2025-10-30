using System;
using UnityEngine;
using CookingPrototype.Kitchen;
using CookingPrototype.UI;
using JetBrains.Annotations;
using CookingPrototype.MessageBrokerEvent;
using UniRx;

namespace CookingPrototype.Controllers
{
	public sealed class GameplayController : MonoBehaviour
	{
		public static GameplayController Instance { get; private set; }

		[SerializeField] private GameObject _tapBlock;
		[SerializeField] private WinWindow _winWindow;
		[SerializeField] private LoseWindow _loseWindow;
		[SerializeField] private GameStartWindow _startWindow;
		public GameObject TapBlock = null;
		public WinWindow WinWindow = null;
		public LoseWindow LoseWindow = null;

		int _ordersTarget = 0;
		private CompositeDisposable _disposables = new();

		public int OrdersTarget
		{
			get { return _ordersTarget; }
			set
			{
				_ordersTarget = value;
				TotalOrdersServedChanged?.Invoke();
			}
		}

		public int TotalOrdersServed { get; private set; } = 0;

		public event Action TotalOrdersServedChanged;

		private void Awake()
		{
			Time.timeScale = 0;

			if ( Instance != null )
			{
				Debug.LogError("Another instance of GameplayController already exists");
			}

			Instance = this;

			MessageBroker.Default
				.Receive<M_GameClose>()
				.Subscribe(m => CloseGame())
				.AddTo(_disposables);

			MessageBroker.Default
				.Receive<M_GameRestart>()
				.Subscribe(m => Restart())
				.AddTo(_disposables);

			MessageBroker.Default
				.Receive<M_StartGame>()
				.Subscribe(m => Init())
				.AddTo(_disposables);

			_startWindow.Show(TotalOrdersServed, OrdersTarget);
		}

		private void OnDestroy()
		{
			if ( Instance == this )
				Instance = null;

			if ( _disposables != null )
				_disposables.Dispose();
		}

		[UsedImplicitly]
		public bool TryServeOrder(Order order)
		{
			if ( !CustomersController.Instance.ServeOrder(order) )
				return false;

			TotalOrdersServed++;
			TotalOrdersServedChanged?.Invoke();
			CheckGameFinish();
			return true;
		}

		public void Restart()
		{
			Init();
			CustomersController.Instance.Init();
			HideWindows();

			foreach ( var place in FindObjectsByType<AbstractFoodPlace>(FindObjectsSortMode.None) )
			{
				place.FreePlace();
			}
		}

		public void CloseGame()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}

		public void CheckGameFinish()
		{
			if ( CustomersController.Instance.IsComplete )
				EndGame(TotalOrdersServed >= OrdersTarget);
		}

		private void Init()
		{
			TotalOrdersServed = 0;
			Time.timeScale = 1f;
			TotalOrdersServedChanged?.Invoke();
		}

		private void EndGame(bool win)
		{
			Time.timeScale = 0f;
			TapBlock?.SetActive(true);

			if ( win )
				WinWindow.Show(TotalOrdersServed, OrdersTarget);
			else
				LoseWindow.Show(TotalOrdersServed, OrdersTarget);
		}

		private void HideWindows()
		{
			TapBlock?.SetActive(false);
			WinWindow?.gameObject.SetActive(false);
			LoseWindow?.gameObject.SetActive(false);
		}
	}
}