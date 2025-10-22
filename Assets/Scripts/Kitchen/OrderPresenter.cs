using UnityEngine;

namespace CookingPrototype.Kitchen {
	[RequireComponent(typeof(OrderPlace))]
	public sealed class OrderPresenter : MonoBehaviour {
		public OrderVisualizer Visualizer = null;

		OrderPlace _orderPlace = null;

		void Start() {
			_orderPlace = GetComponent<OrderPlace>();
			_orderPlace.CurOrderUpdated += OnOrderUpdated;
		}

		void OnDestroy() {
			if ( _orderPlace ) {
				_orderPlace.CurOrderUpdated -= OnOrderUpdated;
			}
		}

		void OnOrderUpdated() {
			Visualizer.Init(_orderPlace.CurOrder);
		}
	}
}
