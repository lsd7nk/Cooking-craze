using CookingPrototype.PauseHandler;
using UnityEngine;

namespace CookingPrototype.Kitchen {
	public abstract class AbstractFoodPlace : MonoBehaviourPauseHandler {
		public abstract bool IsFree { get; }
		
		public abstract bool TryPlaceFood(Food food);
		public abstract void FreePlace();
	}
}
