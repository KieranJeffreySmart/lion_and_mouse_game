namespace Lion_and_mouse.src.Events
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


    public class GameEventMediator : IEventPub, IEventSub
    {
        private readonly Dictionary<Type, List<Action<IGameEvent>>> subs = new Dictionary<Type, List<Action<IGameEvent>>>();
        public void Publish<T>(T gameEvent) where T : class, IGameEvent
        {
            var type = typeof(T);
            if (subs.TryGetValue(type, out List<Action<IGameEvent>>? handlers))
            {
                foreach (var handle in handlers) handle(gameEvent);
            }
        }

        public void Subscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (this.subs.TryGetValue(type, out List<Action<IGameEvent>>? handlers))
            {
                handlers.Add(handler.Handle);
            }
            else
            {
                this.subs[type] = new List<Action<IGameEvent>> { handler.Handle };
            }
        }
    }

    public interface IEventPub
    {
        void Publish<T>(T gameEvent) where T : class, IGameEvent;
    }

    public interface IEventSub
    {
        void Subscribe<T>(IGameEventHandler<T> handler) where T : IGameEvent;
    }

    public interface IGameEventHandler<T> where T : IGameEvent
    {
        void Handle(IGameEvent gameEvent);
    }
}