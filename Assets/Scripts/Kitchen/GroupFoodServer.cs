using System.Collections.Generic;

using UnityEngine;

using JetBrains.Annotations;

namespace CookingPrototype.Kitchen {
	public sealed class GroupFoodServer : MonoBehaviour {
		public List<FoodServer> Servers = null;

		[UsedImplicitly]
		public void TryServe() {
			foreach ( var server in Servers ) {
				if ( server.TryServeFood() ) {
					return;
				}
			}
		}
	}
}
