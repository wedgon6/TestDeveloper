using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using CookingPrototype.Controllers;

namespace CookingPrototype.Kitchen
{
	public sealed class OrderPlace : AbstractFoodPlace
	{

		public List<FoodPlace> Places = new List<FoodPlace>();
		[HideInInspector]
		public List<string> CurOrder = new List<string>();
		public event Action CurOrderUpdated;

		List<Order> _possibleOrders = new List<Order>();

		void Start()
		{
			_possibleOrders.AddRange(OrdersController.Instance.Orders);
			Debug.Log(_possibleOrders.Count);
		}

		bool CanAddFood(Food food)
		{
			if ( CurOrder.Contains(food.Name) )
			{
				Debug.Log(food.Name);
				return false;
			}

			foreach ( var order in _possibleOrders )
			{
				Debug.Log("Foreach 1");
				Debug.Log(order.Name);

				foreach ( var orderFood in order.Foods.Where(x => x.Name == food.Name) )
				{
					Debug.Log(string.IsNullOrEmpty(orderFood.Needs));
					Debug.Log(CurOrder.Contains(orderFood.Needs));

					if ( string.IsNullOrEmpty(orderFood.Needs) || CurOrder.Contains(orderFood.Needs) )
					{
						Debug.Log(food.Name);
						return true;
					}
				}
			}

			return false;
		}

		void UpdatePossibleOrders()
		{
			var ordersToRemove = new List<Order>();
			foreach ( var order in _possibleOrders )
			{
				if ( order.Foods.Where(x => x.Name == CurOrder[CurOrder.Count - 1]).Count() == 0 )
				{
					ordersToRemove.Add(order);
				}
			}
			_possibleOrders.RemoveAll(x => ordersToRemove.Contains(x));
		}

		public override bool TryPlaceFood(Food food)
		{
			if ( !CanAddFood(food) )
			{
				Debug.Log("CAn't add food");
				return false;
			}

			foreach ( var place in Places )
			{
				if ( !place.TryPlaceFood(food) )
				{
					continue;
				}

				CurOrder.Add(food.Name);
				UpdatePossibleOrders();
				CurOrderUpdated?.Invoke();
				return true;
			}

			return false;
		}

		public override void FreePlace()
		{
			_possibleOrders.Clear();
			_possibleOrders.AddRange(OrdersController.Instance.Orders);

			CurOrder.Clear();

			foreach ( var place in Places )
			{
				place.FreePlace();
			}

			CurOrderUpdated?.Invoke();
		}
	}
}
