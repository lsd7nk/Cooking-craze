using UnityEngine;

namespace CookingPrototype.PauseHandler
{
    public abstract class MonoBehaviourPauseHandler : MonoBehaviour, IPauseHandler
    {
        public bool IsOnPause { get; private set; }

        public virtual void OnPause(bool pause)
        {
            IsOnPause = pause;
        }
    }
}