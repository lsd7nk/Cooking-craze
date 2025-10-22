using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

using CookingPrototype.Kitchen;
using CookingPrototype.PauseHandler;

namespace CookingPrototype.Controllers {
	public class CustomersController : MonoBehaviourPauseHandler {

		public static CustomersController Instance { get; private set; }

		public int                 CustomersTargetNumber = 15;
		public float               CustomerWaitTime      = 18f;
		public float               CustomerSpawnTime     = 3f;
		[SerializeField] private CustomerPlace[] _customerPlaces;

		[HideInInspector]
		public int TotalCustomersGenerated { get; private set; } = 0;

		public event Action TotalCustomersGeneratedChanged;

		const string CUSTOMER_PREFABS_PATH = "Prefabs/Customer";

		float _timer = 0f;
		Stack<List<Order>> _orderSets;

		bool HasFreePlaces {
			get { return _customerPlaces.Any(x => x.IsFree); }
		}

		public bool IsComplete {
			get {
				return TotalCustomersGenerated >= CustomersTargetNumber && _customerPlaces.All(x => x.IsFree);
			}
		}

		void Awake() {
			if ( Instance != null ) {
				Debug.LogError("Another instance of CustomersController already exists!");
			}
			Instance = this;

			Init();
		}

		void OnDestroy() {
			if ( Instance == this ) {
				Instance = null;
			}
		}

		void Update() {
			if (IsOnPause)
            {
				return;
            }

			if ( !HasFreePlaces ) {
				return;
			}

			_timer += Time.deltaTime;

			if ( (TotalCustomersGenerated >= CustomersTargetNumber) || (!(_timer > CustomerSpawnTime)) ) {
				return;
			}

			SpawnCustomer();
			_timer = 0f;
		}

		void SpawnCustomer() {
			CustomerPlace chosenPlace = null;
			int freePlacesCount = 0;

			for (int i = 0; i < _customerPlaces.Length; ++i)
			{
				var place = _customerPlaces[i];

				if (!place.IsFree)
				{
					continue;
				}

				if (Random.Range(0, ++freePlacesCount) == 0)
				{
					chosenPlace = place;
				}
			}

			if (chosenPlace == null)
			{
				return;
			}

			chosenPlace.PlaceCustomer(GenerateCustomer());
			TotalCustomersGenerated++;
			TotalCustomersGeneratedChanged?.Invoke();
		}

		Customer GenerateCustomer() {
			var customerGo = Instantiate(Resources.Load<GameObject>(CUSTOMER_PREFABS_PATH));
			var customer   = customerGo.GetComponent<Customer>();

			var orders = _orderSets.Pop();
			customer.Init(orders);

			return customer;
		}

		Order GenerateRandomOrder() {
			var oc = OrdersController.Instance;
			return oc.Orders[Random.Range(0, oc.Orders.Count)];
		}

		public void Init() {
			var totalOrders = 0;
			_orderSets = new Stack<List<Order>>();
			for (var i = 0; i < CustomersTargetNumber; i++)
			{
				var orders = new List<Order>();
				var ordersNum = Random.Range(1, 4);
				for (var j = 0; j < ordersNum; j++)
				{
					orders.Add(GenerateRandomOrder());
				}
				_orderSets.Push(orders);
				totalOrders += ordersNum;
			}

			for (int i = 0; i < _customerPlaces.Length; ++i)
			{
				_customerPlaces[i].Free();
			}
			
			_timer = 0f;

			TotalCustomersGenerated = 0;
			TotalCustomersGeneratedChanged?.Invoke();
			 
			GameplayController.Instance.OrdersTarget = totalOrders - 2;
		}

		/// <summary>
		/// Отпускаем указанного посетителя
		/// </summary>
		/// <param name="customer"></param>
		public void FreeCustomer(Customer customer)
		{
			var place = GetPlace(customer);

			if (place == null)
			{
				return;
			}

			place.Free();
			GameplayController.Instance.CheckGameFinish();
		}

		/// <summary>
		///  Пытаемся обслужить посетителя с заданным заказом и наименьшим оставшимся временем ожидания.
		///  Если у посетителя это последний оставшийся заказ из списка, то отпускаем его.
		/// </summary>
		/// <param name="order">Заказ, который пытаемся отдать</param>
		/// <returns>Флаг - результат, удалось ли успешно отдать заказ</returns>
		public bool ServeOrder(Order order)
		{
			var customerPlace = GetCustomerPlaceForServe(order);

			if (customerPlace == null)
			{
				return false;
			}

			bool result = customerPlace.CurCustomer.ServeOrder(order);

			if (!customerPlace.CurCustomer.HasAnyOrder())
			{
				FreeCustomer(customerPlace.CurCustomer);
			}

			return result;
		}
		
		private CustomerPlace GetCustomerPlaceForServe(Order order)
        {
			CustomerPlace chosenPlace = null;
			float shortestWaitingTime = float.MaxValue;

			for (int i = 0; i < _customerPlaces.Length; ++i)
			{
				var place = _customerPlaces[i];

				if (place.IsFree || !place.CurCustomer.HasOrder(order))
				{
					continue;
				}

				float waitTime = place.CurCustomer.WaitTime;

				if (waitTime < shortestWaitingTime)
				{
					shortestWaitingTime = waitTime;
					chosenPlace = place;
				}
			}

			return chosenPlace;
        }
		
		private CustomerPlace GetPlace(Customer customer)
        {
			for (int i = 0; i < _customerPlaces.Length; ++i)
			{
				var customerPlace = _customerPlaces[i];

				if (customerPlace.IsFree || customerPlace.CurCustomer != customer)
				{
					continue;
				}

				return customerPlace;
			}

			return null;
        }
	}
}
