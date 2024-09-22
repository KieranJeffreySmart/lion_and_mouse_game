using System.Data.Common;

namespace lion_and_mouse_game.GameContext
{
    public class Game(GameStates state, Guid playerId)
    {

        private Game(GameStates state, Guid playerId, int finishingFood, Accolades accolade) : this(state, playerId)
        {
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
            return new Game(GameStates.Lost, PlayerId);
        }

        public Game WinGame(int foodStored, Accolades accolade)
        {
            return new Game(GameStates.Won, PlayerId, foodStored, accolade);
        }
    }
}