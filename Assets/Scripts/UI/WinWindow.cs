using UnityEngine;
using UnityEngine.UI;

using CookingPrototype.Controllers;

using TMPro;

namespace CookingPrototype.UI {
	public sealed class WinWindow : MonoBehaviour {
		public Image    GoalBar     = null;
		public TMP_Text GoalText    = null;
		public Button   OkButton    = null;
		public Button   CloseButton = null;

		bool _isInit = false;

		void Init() {
			var gc = GameplayController.Instance;

			OkButton   .onClick.AddListener(gc.CloseGame);
			CloseButton.onClick.AddListener(gc.CloseGame);
		}

		public void Show () {
			if ( !_isInit ) {
				Init();
			}

			var gc = GameplayController.Instance;

			GoalText.text      = $"{gc.TotalOrdersServed}/{gc.OrdersTarget}";
			GoalBar.fillAmount = Mathf.Clamp01((float) gc.TotalOrdersServed / gc.OrdersTarget);

			gameObject.SetActive(true);
		}

		public void Hide() {
			gameObject.SetActive(false);
		}
	}
}
