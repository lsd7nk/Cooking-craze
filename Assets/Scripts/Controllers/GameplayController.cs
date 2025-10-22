using System;

using UnityEngine;

using CookingPrototype.Kitchen;
using CookingPrototype.UI;

using JetBrains.Annotations;
using CookingPrototype.PauseHandler;

namespace CookingPrototype.Controllers {
	public sealed class GameplayController : MonoBehaviourPauseHandler {
		public static GameplayController Instance { get; private set; }

		public GameObject TapBlock   = null;
		public WinWindow  WinWindow  = null;
		public LoseWindow LoseWindow = null;
		[SerializeField] private StartWindow _startWindow;


		int _ordersTarget = 0;

		public int OrdersTarget {
			get { return _ordersTarget; }
			set {
				_ordersTarget = value;
				TotalOrdersServedChanged?.Invoke();
			}
		}

		public int        TotalOrdersServed { get; private set; } = 0;

		public event Action TotalOrdersServedChanged;

		void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("Another instance of GameplayController already exists");
			}
			Instance = this;

			_startWindow.OnHideEvent += StartGame;
		}

        private void Start()
		{
			StartLevel();
        }

        void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}

			_startWindow.OnHideEvent -= StartGame;
		}

		private void ShowStartWindow()
		{
			_startWindow.SetTargetOrdersCountText(OrdersTarget.ToString());
			_startWindow.Show();
		}

		private void StartLevel()
        {
			PauseController.Instance.SetPause(true);
			ShowStartWindow();
        }
		
		private void StartGame()
        {
			PauseController.Instance.SetPause(false);
        }

		void Init()
		{
			TotalOrdersServed = 0;
			TotalOrdersServedChanged?.Invoke();
		}

		public void CheckGameFinish() {
			if ( CustomersController.Instance.IsComplete ) {
				EndGame(TotalOrdersServed >= OrdersTarget);
			}
		}

		void EndGame(bool win) {
			PauseController.Instance.SetPause(true);
			TapBlock?.SetActive(true);
			if ( win ) {
				WinWindow.Show();
			} else {
				LoseWindow.Show();
			}
		}

		void HideWindows() {
			TapBlock?.SetActive(false);
			WinWindow?.Hide();
			LoseWindow?.Hide();
		}

		[UsedImplicitly]
		public bool TryServeOrder(Order order) {
			if ( !CustomersController.Instance.ServeOrder(order) ) {
				return false;
			}

			TotalOrdersServed++;
			TotalOrdersServedChanged?.Invoke();
			CheckGameFinish();
			return true;
		}

		public void Restart() {
			Init();
			CustomersController.Instance.Init();
			HideWindows();

			foreach (var place in FindObjectsByType<AbstractFoodPlace>(FindObjectsSortMode.None))
			{
				place.FreePlace();
			}

			StartLevel();
		}

		public void CloseGame() {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
