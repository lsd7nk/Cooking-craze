using UnityEngine;

using CookingPrototype.Controllers;

using JetBrains.Annotations;

namespace CookingPrototype.Kitchen {
	[RequireComponent(typeof(OrderPlace))]
	public sealed class OrderServer : MonoBehaviour {
		OrderPlace _orderPlace;

		void Start() {
			_orderPlace = GetComponent<OrderPlace>();
		}

		[UsedImplicitly]
		public void TryServeOrder() {
			var order = OrdersController.Instance.FindOrder(_orderPlace.CurOrder);
			if ( (order == null) || !GameplayController.Instance.TryServeOrder(order) ) {
				return;
			}

			_orderPlace.FreePlace();
		}
	}
}
