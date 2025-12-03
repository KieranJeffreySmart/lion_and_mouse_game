using System.Collections.Concurrent;

namespace lion_and_mouse_game.Events
{


    public class GameEventMediator : IEventMediator
    {
        private readonly ConcurrentDictionary<Type, List<Action<IGameEvent>>> subs = new ConcurrentDictionary<Type, List<Action<IGameEvent>>>();
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
            if (subs.TryGetValue(type, out List<Action<IGameEvent>>? handlers))
            {
                handlers.Add(handler.Handle);
            }
            else
            {
                subs[type] = new List<Action<IGameEvent>> { handler.Handle };
            }
        }
    }

    public interface IEventMediator : IEventPub, IEventSub
    {
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