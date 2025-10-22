using System.Collections.Generic;

using UnityEngine;

using CookingPrototype.Controllers;

namespace CookingPrototype.Kitchen {
	[RequireComponent(typeof(FoodPlace))]
	public sealed class FoodServer : MonoBehaviour {

		FoodPlace _place = null;

		void Start() {
			_place = GetComponent<FoodPlace>();
		}

		public bool TryServeFood() {
			if ( _place.IsFree || (_place.CurFood.CurStatus != Food.FoodStatus.Cooked) ) {
				return false;
			}
			var order = OrdersController.Instance.FindOrder(new List<string>(1) { _place.CurFood.Name });
			if ( (order == null) || !GameplayController.Instance.TryServeOrder(order) ) {
				return false;
			}

			_place.FreePlace();
			return true;
		}
	}
}
