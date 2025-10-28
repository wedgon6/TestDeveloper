using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CookingPrototype.Kitchen
{
	public sealed class FoodPlacer : MonoBehaviour
	{
		public string FoodName = string.Empty;
		public List<AbstractFoodPlace> Places = new List<AbstractFoodPlace>();

		[UsedImplicitly]
		public void TryPlaceFood()
		{
			Debug.Log("FOOD NAME  " + FoodName);
			foreach ( var place in Places )
			{
				if ( place.TryPlaceFood(new Food(FoodName)) )
				{
					return;
				}
			}
		}
	}
}
