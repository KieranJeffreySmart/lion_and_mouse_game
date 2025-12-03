using lion_and_mouse_game.Events;

namespace centralised_game_api
{
    public class WebSocketGameEventBroadcaster : IEventPub
    {
        private IEventPub innerPublisher;
        private Action<IGameEvent> pushEvent;

        public WebSocketGameEventBroadcaster(IEventPub innerPublisher, Action<IGameEvent> pushEvent)
        {
            this.innerPublisher = innerPublisher;
            this.pushEvent = pushEvent;
        }

        void IEventPub.Publish<T>(T gameEvent)
        {
            innerPublisher.Publish(gameEvent);
            pushEvent(gameEvent);
        }
    }
}