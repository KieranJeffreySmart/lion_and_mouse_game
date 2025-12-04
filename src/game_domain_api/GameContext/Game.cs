namespace game_domain_api.GameContext
{
    public class Game(GameStates state, Guid playerId)
    {
        public Game(Guid id, GameStates state, Guid playerId, int finishingFood, Accolades accolade) : this(state, playerId)
        {
            Id = id;
            FinishingFood = finishingFood;
            Accolade = accolade;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public GameStates GameState { get; } = state;
        public Guid PlayerId { get; } = playerId;
        public int FinishingFood { get; } = -1;
        public Accolades Accolade { get; } = Accolades.None;

        public Game LoseGame()
        {
            return new Game(Id, GameStates.Lost, PlayerId, FinishingFood, Accolade);
        }

        public Game WinGame(int foodStored, Accolades accolade)
        {
            return new Game(Id, GameStates.Won, PlayerId, foodStored, accolade);
        }
    }
    
    public enum Accolades
    {
        Unknown = 0,
        None,
        Survivor,
        Hoarder,
        Desperado
    }

    public enum GameStates
    {
        Unknown = 0,
        Playing,
        Lost,
        Won        
    }

    public enum LionStates
    {
        Unknown = 0,
        AtHome,
        Hunting,
        Sleeping
    }
    
    [Serializable]
    public class GameData
    {
        public Guid Id { get; set; } = Guid.Empty;
        public GameStates GameState { get; set; } = GameStates.Unknown;
        public int FinishingFood { get; set; } = -1;
        public Accolades Accolade { get; set; } = Accolades.Unknown;
        public Guid PlayerId { get; set; } = Guid.Empty;
    }

}