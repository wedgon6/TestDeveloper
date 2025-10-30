using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CookingPrototype.Kitchen;

namespace CookingPrototype.Controllers
{
	public sealed class OrdersController : MonoBehaviour
	{
		static OrdersController _instance = null;

		public static OrdersController Instance
		{
			get
			{
				if ( !_instance )
					_instance = FindAnyObjectByType<OrdersController>();

				if ( _instance && !_instance._isInit )
					_instance.Init();

				return _instance;
			}

			set { _instance = value; }
		}

		public List<Order> Orders = new List<Order>();

		bool _isInit = false;

		private void Awake()
		{
			if ( (_instance != null) && (_instance != this) )
				Debug.LogError("Another instance of OrdersController already exists!");

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

		private void Init()
		{
			if ( _isInit )
				return;

			var ordersConfig = Resources.Load<TextAsset>("Configs/Orders");
			var ordersXml = new XmlDocument();

			using ( var reader = new StringReader(ordersConfig.ToString()) )
			{
				ordersXml.Load(reader);
			}

			var rootElem = ordersXml.DocumentElement;

			foreach ( XmlNode node in rootElem.SelectNodes("order") )
			{
				var order = ParseOrder(node);
				Orders.Add(order);
			}

			_isInit = true;
		}

		private Order ParseOrder(XmlNode node)
		{
			var foods = new List<Order.OrderFood>();
			foreach ( XmlNode foodNode in node.SelectNodes("food") )
			{
				foods.Add(new Order.OrderFood(foodNode.InnerText, foodNode.SelectSingleNode("@needs")?.InnerText));
			}
			return new Order(node.SelectSingleNode("@name").Value, foods);
		}

		public Order FindOrder(List<string> foods)
		{
			return Orders.Find(x =>
			{
				if ( x.Foods.Count != foods.Count )
					return false;

				foreach ( var food in x.Foods )
				{
					if ( x.Foods.Count(f => f.Name == food.Name) != foods.Count(f => f == food.Name) )
						return false;
				}

				return true;
			});
		}
	}
}