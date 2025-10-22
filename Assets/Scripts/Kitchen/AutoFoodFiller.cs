using UnityEngine;

using System.Collections.Generic;
using CookingPrototype.PauseHandler;

namespace CookingPrototype.Kitchen {
	public sealed class AutoFoodFiller : MonoBehaviourPauseHandler {
		public string                  FoodName = null;
		public List<AbstractFoodPlace> Places   = new List<AbstractFoodPlace>();

		void Update() {
			if (IsOnPause)
			{
				return;
			}
			
			foreach ( var place in Places ) {
				place.TryPlaceFood(new Food(FoodName));
			}
		}
	}
}
