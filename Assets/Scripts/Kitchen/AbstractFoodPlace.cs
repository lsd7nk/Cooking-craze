using UnityEngine;

namespace CookingPrototype.Kitchen {
	public abstract class AbstractFoodPlace : MonoBehaviour {
		public abstract bool TryPlaceFood(Food food);
		public abstract void FreePlace();
	}
}
