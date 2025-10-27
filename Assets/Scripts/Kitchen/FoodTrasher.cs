using System.Collections;
using CookingPrototype.MessageBrokerEvent;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace CookingPrototype.Kitchen
{
	[RequireComponent(typeof(FoodPlace))]
	public sealed class FoodTrasher : MonoBehaviour
	{
		[SerializeField] private float _doubleClickTime = 0.3f;

		private FoodPlace _place = null;
		private float _timer = 0f;
		private bool _isWaitingForSecondClick = false;
		private Coroutine _coroutine;

		private void Start()
		{
			_place = GetComponent<FoodPlace>();
			_timer = Time.realtimeSinceStartup;
		}

		/// <summary>
		/// Освобождает место по двойному тапу если еда на этом месте сгоревшая.
		/// </summary>
		[UsedImplicitly]
		public void TryTrashFood()
		{
			if ( Time.realtimeSinceStartup - _timer < _doubleClickTime )
			{
				OnDoubleClick();
				_isWaitingForSecondClick = false;
			}
			else
			{
				_isWaitingForSecondClick = true;

				if (_coroutine != null )
					StopCoroutine(_coroutine);

				_coroutine = StartCoroutine(WaitForDoubleClick());
			}

			_timer = Time.realtimeSinceStartup;
		}

		private void OnDoubleClick()
		{
			_place.FreePlace();
			MessageBroker.Default.Publish(new M_ThrowTrash());
		}

		private IEnumerator WaitForDoubleClick()
		{
			yield return new WaitForSeconds(_doubleClickTime);

			if ( _isWaitingForSecondClick )
				_isWaitingForSecondClick = false;
		}
	}
}