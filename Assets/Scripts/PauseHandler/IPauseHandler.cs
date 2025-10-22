namespace CookingPrototype.PauseHandler
{
    public interface IPauseHandler
    {
        bool IsOnPause { get; }

        void OnPause(bool pause);
    }
}