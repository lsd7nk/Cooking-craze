using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace CookingPrototype.Kitchen
{
    public sealed class TrashFoodPlace : AbstractFoodPlace
    {
        [SerializeField, Range(1f, 3f)] private float _timeForFree;
        [SerializeField] private FoodPresenter[] _foodPresenters;

        private CancellationToken _commonCancellationToken;
        private Food _currentFood;

        public override bool IsFree => _currentFood == null;
        
        public override bool TryPlaceFood(Food food)
        {
            if (!IsFree)
            {
                return false;
            }

            if (food.CurStatus != Food.FoodStatus.Overcooked)
            {
                return false;
            }

            PlaceFood(food);

            return true;
        }

        public override void FreePlace()
        {
            if (_currentFood == null)
            {
                return;
            }

            for (int i = 0; i < _foodPresenters.Length; ++i)
            {
                var presenter = _foodPresenters[i];

                if (!_currentFood.Name.Equals(presenter.FoodName))
                {
                    continue;
                }

                presenter.Set.Hide();
            }

            _currentFood = null;
        }

        private void PlaceFood(Food food)
        {
            _currentFood = food;
            
            for (int i = 0; i < _foodPresenters.Length; ++i)
            {
                var presenter = _foodPresenters[i];

                if (!_currentFood.Name.Equals(presenter.FoodName))
                {
                    continue;
                }

                presenter.Set.ShowStatus(Food.FoodStatus.Overcooked);
            }

            FreePlaceAsync().Forget();
        }

        private async UniTaskVoid FreePlaceAsync()
        {
            await UniTask.WaitForSeconds(_timeForFree, cancellationToken: _commonCancellationToken);

            if (_commonCancellationToken.IsCancellationRequested)
            {
                return;
            }

            FreePlace();
        }

        private void Start()
        {
            _commonCancellationToken = this.GetCancellationTokenOnDestroy();
        }
    }
}