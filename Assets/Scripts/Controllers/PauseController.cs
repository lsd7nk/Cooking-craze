using CookingPrototype.PauseHandler;
using UnityEngine;

namespace CookingPrototype.Controllers
{
    public sealed class PauseController : MonoBehaviour
    {
        public static PauseController Instance { get; private set; }

        [SerializeField] private MonoBehaviourPauseHandler[] _pauseHandlers;

        public void SetPause(bool pause)
        {
            for (int i = 0; i < _pauseHandlers.Length; ++i)
            {
                _pauseHandlers[i].OnPause(pause);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Populate Pause Handlers")]
        private void PopulatePauseHandlers()
        {
            _pauseHandlers = FindObjectsByType<MonoBehaviourPauseHandler>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
#endif

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Another instance of PauseController already exists");
            }

            Instance = this;
        }
    }
}