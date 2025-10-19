using UnityEngine;

using System;

namespace CookingPrototype.Kitchen {
	public class FoodPlace : AbstractFoodPlace {
		public bool  Cook         = false;
		public float CookTime     = 0f;
		public float OvercookTime = 0f;

		public event Action FoodPlaceUpdated;

		float _timer      = 0f;

		public Food CurFood   { get; private set; } = null;
		public bool IsCooking { get; private set; } = false;

		public bool IsFree { get { return CurFood == null; } }

		public float TimerNormalized {
			get {
				if ( IsFree || !Cook || !IsCooking ) {
					return 0f;
				}
				if ( CurFood.CurStatus == Food.FoodStatus.Raw ) {
					return _timer / CookTime;
				}
				return _timer / OvercookTime;
			}
		}

		void Update() {
			if ( IsFree || !Cook || !IsCooking ) {
				return;
			}

			_timer += Time.deltaTime;
			switch ( CurFood.CurStatus ) {
				case Food.FoodStatus.Raw: {
					if ( _timer > CookTime ) {
						CurFood.CookStep();
						_timer = 0f;
						if ( OvercookTime <= 0f ) {
							IsCooking = false;
						}
						FoodPlaceUpdated?.Invoke();
					}
					break;
				}
				case Food.FoodStatus.Cooked: {
					if ( _timer > OvercookTime ) {
						CurFood.CookStep();
						_timer = 0f;
						IsCooking = false;
						FoodPlaceUpdated?.Invoke();
					}
					break;
				}
			}
		}

		public override bool TryPlaceFood(Food food) {
			if ( !IsFree ) {
				return false;
			}

			CurFood = food;
			if ( Cook && CurFood.CurStatus != (Food.FoodStatus.Overcooked) ) {
				IsCooking = true;
			}

			FoodPlaceUpdated?.Invoke();
			return true;
		}

		public Food ExtractFood() {
			Food res = CurFood;
			CurFood = null;

			FoodPlaceUpdated?.Invoke();
			return res;
		}


		/// <summary>
		/// Освобождаем место
		/// </summary>
		public override void FreePlace() {
			CurFood   = null;
			_timer    = 0f;
			IsCooking = false;
			FoodPlaceUpdated?.Invoke();
		}
	}
}
