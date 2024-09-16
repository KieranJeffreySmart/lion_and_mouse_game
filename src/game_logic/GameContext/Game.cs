namespace Lion_and_mouse.src.GameContext
{
    internal class Game(GameStates state)
    {
        public GameStates GameState { get; } = state;

        internal Game LoseGame()
        {
            return new Game(GameStates.Lost);
        }

        internal Game WinGame(int foodStored, Accolades accolades)
        {
            return new Game(GameStates.Won);
        }
    }
}