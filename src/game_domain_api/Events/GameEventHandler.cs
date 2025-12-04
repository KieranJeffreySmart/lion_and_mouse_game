namespace game_domain_api.Events
{
    public class GameEventHandler<T> : IGameEventHandler<T> where T : IGameEvent
    {
        Action<T> handleDelegate;

        public GameEventHandler(Action<T> handler)
        {
            handleDelegate = handler;
        }

        public void Handle(IGameEvent gameEvent)
        {
            if (gameEvent is T concreteGameEvent)
            {
                handleDelegate(concreteGameEvent);
            }
        }
    }
}