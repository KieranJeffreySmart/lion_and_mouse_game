using game_domain_api.GameContext;

namespace game_domain_api.Events
{
    public interface IGameEvent
    {
        public Guid Id { get; }

        public string Name { get; }
    }

    public class NewGameStartedEvent(LionStates lionStartingState) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(NewGameStartedEvent);
        public LionStates LionStartingState { get; } = lionStartingState;
    }

    public class GameWon : IGameEvent
    {
        public GameWon(int foodStored, Accolades accolade)
        {
            FoodStored = foodStored;
            Accolade = accolade;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(GameWon);
        public int FoodStored { get; }
        public Accolades Accolade { get; }
    }

    public class GameLost : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(GameLost);
    }

}