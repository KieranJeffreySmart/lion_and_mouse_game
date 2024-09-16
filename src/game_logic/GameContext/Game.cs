namespace Lion_and_mouse.src.GameContext
{
    public class Game(GameStates state)
    {
        public GameStates GameState { get; } = state;

        public Game LoseGame()
        {
            return new Game(GameStates.Lost);
        }

        public Game WinGame(int foodStored, Accolades accolades)
        {
            return new Game(GameStates.Won);
        }
    }
}