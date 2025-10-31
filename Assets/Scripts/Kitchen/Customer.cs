using CookingPrototype.Controllers;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CookingPrototype.Kitchen
{
	public sealed class Customer : MonoBehaviour
	{
		private const string ORDERS_PREFABS_PATH = "Prefabs/Orders/{0}";

		[SerializeField] private Image _customerImage;
		[SerializeField] private List<Sprite> _customerSprites;
		[SerializeField] private Image _timerBar;
		[SerializeField] private List<CustomerOrderPlace> _orderPlace;

		private List<Order> _orders = null;
		private float _timer = 0f;
		private bool _isActive = false;

		public float WaitTime
		{
			get { return CustomersController.Instance.CustomerWaitTime - _timer; }
		}

		/// <summary>
		/// Есть ли необслуженные заказы у указанного посетителя.
		/// </summary>
		public bool IsComplete { get { return _orders.Count == 0; } }

		private void Update()
		{
			if ( !_isActive )
			{
				return;
			}
			_timer += Time.deltaTime;
			_timerBar.fillAmount = WaitTime / CustomersController.Instance.CustomerWaitTime;

			if ( WaitTime <= 0f )
			{
				CustomersController.Instance.FreeCustomer(this);
			}
		}

		public void Init(List<Order> orders)
		{
			_orders = orders;

			if ( _orders.Count > _orderPlace.Count )
			{
				Debug.LogError("There's too many orders for one customer");
				return;
			}

			_orderPlace.ForEach(x => x.Complete());

			var i = 0;
			for ( ; i < _orders.Count; i++ )
			{
				var order = _orders[i];
				var place = _orderPlace[i];
				Instantiate(Resources.Load<GameObject>(string.Format(ORDERS_PREFABS_PATH, order.Name)), place.transform, false);
				place.Init(order);
			}

			SetRandomSprite();

			_isActive = true;
			_timer = 0f;
		}

		[UsedImplicitly]
		public bool ServeOrder(Order order)
		{
			var place = _orderPlace.Find(x => x.CurrentOrder == order);

			if ( !place )
				return false;

			_orders.Remove(order);
			place.Complete();

			if ( _orders.Count == 0 )
				CustomersController.Instance.FreeCustomer(this);
			else
				_timer = Mathf.Max(0f, _timer - 6f);

			return true;
		}

		[ContextMenu("Set random sprite")]
		private void SetRandomSprite()
		{
			_customerImage.sprite = _customerSprites[Random.Range(0, _customerSprites.Count)];
			_customerImage.SetNativeSize();
		}
	}
}