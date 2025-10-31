using CookingPrototype.Kitchen;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CookingPrototype.Controllers
{
	public class CustomersController : MonoBehaviour
	{
		private const string CUSTOMER_PREFABS_PATH = "Prefabs/Customer";
		public static CustomersController Instance { get; private set; }

		[SerializeField] private int _customersTargetNumber = 15;
		[SerializeField] private float _customerWaitTime = 18f;
		[SerializeField] private float _customerSpawnTime = 3f;
		[SerializeField] private List<CustomerPlace> _customerPlaces = new();

		private float _timer = 0f;
		private Stack<List<Order>> _orderSets;
		
		public int CustomersTargetNumber => _customersTargetNumber;
		public float CustomerWaitTime => _customerWaitTime;
		public float CustomerSpawnTime => _customerSpawnTime;
		public List<CustomerPlace> CustomerPlaces => _customerPlaces;

		[HideInInspector]
		public int TotalCustomersGenerated { get; private set; } = 0;

		public event Action TotalCustomersGeneratedChanged;

		private bool _hasFreePlaces
		{
			get { return CustomerPlaces.Any(x => x.IsFree); }
		}

		public bool IsComplete
		{
			get
			{
				return TotalCustomersGenerated >= CustomersTargetNumber && CustomerPlaces.All(x => x.IsFree);
			}
		}

		private void Awake()
		{
			if ( Instance != null )
				Debug.LogError("Another instance of CustomersController already exists!");

			Instance = this;
		}

		private void OnDestroy()
		{
			if ( Instance == this )
				Instance = null;
		}

		private void Start()
		{
			Init();
		}

		private void Update()
		{
			if ( !_hasFreePlaces )
				return;

			_timer += Time.deltaTime;

			if ( (TotalCustomersGenerated >= CustomersTargetNumber) || (!(_timer > CustomerSpawnTime)) )
				return;

			SpawnCustomer();
			_timer = 0f;
		}

		public void Init()
		{
			var totalOrders = 0;
			_orderSets = new Stack<List<Order>>();

			for ( var i = 0; i < CustomersTargetNumber; i++ )
			{
				var orders = new List<Order>();
				var ordersNum = Random.Range(1, 4);

				for ( var j = 0; j < ordersNum; j++ )
				{
					orders.Add(GenerateRandomOrder());
				}

				_orderSets.Push(orders);
				totalOrders += ordersNum;
			}

			CustomerPlaces.ForEach(x => x.Free());
			_timer = 0f;

			TotalCustomersGenerated = 0;
			TotalCustomersGeneratedChanged?.Invoke();

			GameplayController.Instance.OrdersTarget = totalOrders - 2;
		}

		/// <summary>
		/// Отпускаем указанного посетителя
		/// </summary>
		/// <param name="customer"></param>
		public void FreeCustomer(Customer customer)
		{
			var place = CustomerPlaces.Find(x => x.CurrentCustomer == customer);

			if ( place == null )
				return;

			place.Free();
			GameplayController.Instance.CheckGameFinish();
		}

		/// <summary>
		///  Пытаемся обслужить посетителя с заданным заказом и наименьшим оставшимся временем ожидания.
		///  Если у посетителя это последний оставшийся заказ из списка, то отпускаем его.
		/// </summary>
		/// <param name="order">Заказ, который пытаемся отдать</param>
		/// <returns>Флаг - результат, удалось ли успешно отдать заказ</returns>
		public bool ServeOrder(Order order)
		{
			float maxTime = float.MaxValue;
			Customer customer = null;

			foreach ( var place in _customerPlaces )
			{
				if ( place.CurrentCustomer != null )
				{
					if ( place.CurrentCustomer.WaitTime < maxTime )
					{
						customer = place.CurrentCustomer;
						maxTime = customer.WaitTime;
					}
				}
			}

			return customer.ServeOrder(order);
		}

		private void SpawnCustomer()
		{
			var freePlaces = CustomerPlaces.FindAll(x => x.IsFree);

			if ( freePlaces.Count <= 0 )
				return;

			var place = freePlaces[Random.Range(0, freePlaces.Count)];
			place.PlaceCustomer(GenerateCustomer());
			TotalCustomersGenerated++;
			TotalCustomersGeneratedChanged?.Invoke();
		}

		private Customer GenerateCustomer()
		{
			var customerGo = Instantiate(Resources.Load<GameObject>(CUSTOMER_PREFABS_PATH));
			var customer = customerGo.GetComponent<Customer>();

			var orders = _orderSets.Pop();
			customer.Init(orders);

			return customer;
		}

		private Order GenerateRandomOrder()
		{
			var oc = OrdersController.Instance;
			return oc.Orders[Random.Range(0, oc.Orders.Count)];
		}
	}
}