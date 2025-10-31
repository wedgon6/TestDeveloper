using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace CookingPrototype.Kitchen
{
	public sealed class GroupFoodServer : MonoBehaviour
	{
		[SerializeField] private List<FoodServer> _servers;

		[UsedImplicitly]
		public void TryServe()
		{
			foreach ( var server in _servers )
			{
				if ( server.TryServeFood() )
					return;
			}
		}
	}
}