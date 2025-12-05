namespace game_domain_api.GameContext
{
    public class Game
    {
        private GameData _data = new();

        public Game(GameData data)
        {
            _data = new GameData
            {
                Id = data.Id,
                GameState = data.GameState,
                FinishingFood = data.FinishingFood,
                Accolade = data.Accolade,
                PlayerId = data.PlayerId
            };
        }

        public Guid Id => _data.Id;
        public GameStates GameState => _data.GameState;
        public Guid PlayerId => _data.PlayerId;
        public int FinishingFood => _data.FinishingFood;
        public Accolades Accolade => _data.Accolade;
        public void LoseGame(int foodStored)
        {
            _data =  new GameData
            {
                Id = _data.Id,
                GameState = GameStates.Lost,
                FinishingFood = foodStored,
                Accolade = Accolades.None,
                PlayerId = _data.PlayerId
            };
        }

        public void WinGame(int foodStored, Accolades accolade)
        {
            _data =  new GameData
            {
                Id = _data.Id,
                GameState = GameStates.Won,
                FinishingFood = foodStored,
                Accolade = accolade,
                PlayerId = _data.PlayerId
            };
        }

        public GameData AsData()
        {
            return new GameData
            {
                Id = _data.Id,
                GameState = _data.GameState,
                FinishingFood = _data.FinishingFood,
                Accolade = _data.Accolade,
                PlayerId = _data.PlayerId
            };
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