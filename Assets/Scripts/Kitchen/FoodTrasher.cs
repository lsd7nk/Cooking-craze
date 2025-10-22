using UnityEngine;

using JetBrains.Annotations;
using UnityEngine.EventSystems;

namespace CookingPrototype.Kitchen {
	[RequireComponent(typeof(FoodPlace))]
	public sealed class FoodTrasher : MonoBehaviour {
		private const int CLICKS_COUNT_FOR_TRASH_FOOD = 2;

		[SerializeField] private AbstractFoodPlace[] _destinationPlaces;
		
		FoodPlace _place = null;

		void Start() {
			_place = GetComponent<FoodPlace>();
		}

		/// <summary>
		/// Освобождает место по двойному тапу если еда на этом месте сгоревшая.
		/// </summary>
		[UsedImplicitly]
		public void TryTrashFood(BaseEventData baseEventData)
		{
			if (baseEventData is not PointerEventData pointerEventData)
			{
				return;
			}

			int clicksCount = pointerEventData.clickCount;

			if (clicksCount != CLICKS_COUNT_FOR_TRASH_FOOD)
			{
				return;
			}

			var food = _place.CurFood;

			if (food == null)
			{
				return;
			}

			for (int i = 0; i < _destinationPlaces.Length; ++i)
			{
				var destinationPlace = _destinationPlaces[i];

				if (!destinationPlace.TryPlaceFood(food))
				{
					continue;
				}

				_place.FreePlace();
			}
		}
	}
}
