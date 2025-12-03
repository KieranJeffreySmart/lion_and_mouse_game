using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.MouseContext;

namespace centralised_game_api
{
    public class CommandHandler
    {
        private GameEngine gameEngine;
        private MouseEngine mouseEngine;

        public CommandHandler(GameEngine gameEngine, MouseEngine mouseEngine)
        {
            this.gameEngine = gameEngine;
            this.mouseEngine = mouseEngine;
        }

        public void Handle(ClientCommand command)
        {
            if (command is null) return;

            if (command.CommandType == GameCommands.MouseHunt)
            {
                if (gameEngine.IsGameRunning && gameEngine.CurrentPlayerId == command.PlayerId) mouseEngine.Hunt();
            }

            if (command.CommandType == GameCommands.MouseStayAtHome)
            {
                if (gameEngine.IsGameRunning && gameEngine.CurrentPlayerId == command.PlayerId) mouseEngine.StayAtHome();
            }
        }
    }

    public class ClientCommand
    {
        public GameCommands CommandType { get; set; }

        public Guid PlayerId { get; set; }
    }

    public enum GameCommands
    {
        MouseHunt,
        MouseStayAtHome
    }

    public enum PlayerTypes
    {
        mouse,
        observer
    }
}