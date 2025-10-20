using UnityEngine;
using System;
using Nk7.UI;
using TMPro;

namespace CookingPrototype.UI
{
    public sealed class StartWindow : MonoBehaviour
    {
        [SerializeField] private View _view;
        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Text _targetOrdersCountLabel;

        public event Action OnHideEvent;

        public void SetTargetOrdersCountText(string value)
        {
            _targetOrdersCountLabel.SetText(value);
        }

        public void Show()
        {
            _view.Show();
            gameObject.SetActive(true);
        }

        private void PlayButtonOnPointerClick()
        {
            OnHideEvent?.Invoke();
            _view.Hide();
        }

        private void OnHideFinish()
        {
            gameObject.SetActive(false);
        }

        private void AddEventListeners()
        {
            _view.OnHideFinishEvent += OnHideFinish;
            _playButton.OnPointerClickEvent += PlayButtonOnPointerClick;
        }

        private void RemoveEventListeners()
        {
            _view.OnHideFinishEvent -= OnHideFinish;
            _playButton.OnPointerClickEvent -= PlayButtonOnPointerClick;
        }

        private void Awake()
        {
            AddEventListeners();
        }

        private void OnDestroy()
        {
            RemoveEventListeners();
        }
    }
}