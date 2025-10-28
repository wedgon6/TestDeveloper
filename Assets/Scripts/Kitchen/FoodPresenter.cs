using UnityEngine;

using System;

namespace CookingPrototype.Kitchen
{
	public sealed class FoodPresenter : MonoBehaviour
	{
		[Serializable]
		public class FoodVisualizersSet
		{
			public FoodVisualizer RawVisualizer = null;
			public FoodVisualizer CookedVisualizer = null;
			public FoodVisualizer OvercookedVisualizer = null;

			public void Hide()
			{
				RawVisualizer?.SetEnabled(false);
				CookedVisualizer?.SetEnabled(false);
				OvercookedVisualizer?.SetEnabled(false);
			}

			public void ShowEmpty()
			{
				Hide();
			}

			public void ShowStatus(Food.FoodStatus status)
			{
				Hide();
				switch ( status )
				{
					case Food.FoodStatus.Raw:
						{
							RawVisualizer?.SetEnabled(true);
							return;
						}
					case Food.FoodStatus.Cooked:
						{
							CookedVisualizer?.SetEnabled(true);
							return;
						}
					case Food.FoodStatus.Overcooked:
						{
							OvercookedVisualizer?.SetEnabled(true);
							return;
						}
				}
			}
		}

		public string FoodName = string.Empty;
		public FoodVisualizersSet Set = null;
		public FoodPlace Place = null;

		void Start()
		{
			Set?.Hide();
			if ( Place )
			{
				Place.FoodPlaceUpdated += OnFoodPlaceUpdated;
			}
		}

		void OnDestroy()
		{
			if ( Place )
			{
				Place.FoodPlaceUpdated -= OnFoodPlaceUpdated;
			}
		}

		void OnFoodPlaceUpdated()
		{
			if ( Place.IsFree )
			{
				Set?.ShowEmpty();
			}
			else
			{
				if ( Place.CurFood.Name == FoodName )
				{
					Set?.ShowStatus(Place.CurFood.CurStatus);
				}
				else
				{
					Set?.Hide();
				}
			}
		}
	}
}
