using UnityEngine;

using System.Collections.Generic;

namespace CookingPrototype.Kitchen {
	public sealed class AutoFoodFiller : MonoBehaviour {
		public string                  FoodName = null;
		public List<AbstractFoodPlace> Places   = new List<AbstractFoodPlace>();

		void Update() {
			foreach ( var place in Places ) {
				place.TryPlaceFood(new Food(FoodName));
			}
		}
	}
}
