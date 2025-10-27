using System.Collections;
using CookingPrototype.MessageBrokerEvent;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen
{
	public class TrasherVisualizer : MonoBehaviour
	{
		[SerializeField] private Sprite _closeTrash;
		[SerializeField] private Sprite _openTrash;
		[SerializeField] private Image _trashSprite;
		[SerializeField] private float _timeOpenVisualize;

		private Coroutine _closeCoroutine;
		private float _timer;
		private CompositeDisposable _disposables = new();

		private void Start()
		{
			if ( _trashSprite != null && _closeTrash != null )
				_trashSprite.sprite = _closeTrash;

			MessageBroker.Default
				.Receive<M_ThrowTrash>()
				.Subscribe(m => ThrowTrash())
				.AddTo(_disposables);
		}

		private void OnDestroy()
		{
			if ( _disposables != null )
				_disposables.Dispose();
		}

		public void ThrowTrash()
		{
			if ( _trashSprite != null && _openTrash != null )
			{
				_trashSprite.sprite = _openTrash;
			}

			_timer = Time.realtimeSinceStartup + _timeOpenVisualize;

			if ( _closeCoroutine == null )
				_closeCoroutine = StartCoroutine(WaitAndClose());
		}

		private IEnumerator WaitAndClose()
		{
			while ( Time.realtimeSinceStartup < _timer )
			{
				yield return null;
			}

			if ( _trashSprite != null && _closeTrash != null )
			{
				_trashSprite.sprite = _closeTrash;
			}

			_closeCoroutine = null;
		}
	}
}