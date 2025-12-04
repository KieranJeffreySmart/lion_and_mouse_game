using centralised_game_api;
using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.StoryContext;

namespace centralised_game_api_tests;

internal static partial class GameRunTestData
{
    internal const string STORY_TEXT_SIMPLE_WIN = "\r\nOnce upon a time there was a little mouse\r\n\r\n\r\nOn day 1, while searching for something to eat the mouse found a sleeping lion and in her haste returned home with only 1 food\r\n\r\nOn day 2, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 3, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 4, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 5, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 6, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 7, after searching for something to eat the mouse returned home with 2 food";

    internal class GameScenarioRun
    {
        public List<ScenarioCommandData> Commands { get; set; } = new List<ScenarioCommandData>();
        public List<ClientCommand> GetCommands(Guid PlayerId)
        {
            return [.. Commands.Select(commandData => 
                {
                    return new ClientCommand 
                    { 
                        CommandType = commandData.Command, 
                        PlayerId = PlayerId 
                    }; 
                })];
        }

        public Func<LionEngine, NewDayEvent, LionBehaviours> LionBehavior { get; set; } = (lionEngine, gameEvent) => LionBehaviours.GoHome;

        public GameData ExpectedGameDataAfterCommands { get; set; } = new GameData();
        public StoryData ExpectedStoryDataAfterCommands { get; set; } = new StoryData();
    }

    internal class ScenarioCommandData
    {
        public GameCommands Command { get; set; } = new GameCommands();
        public GameData ExpectedGameDataAfterCommand { get; set; } = new GameData();
        public StoryData ExpectedStoryDataAfterCommand { get; set; } = new StoryData();
    }

    internal static Dictionary<string, GameScenarioRun> Scenarios = new Dictionary<string, GameScenarioRun>
    {
        {
            "SimpleWinScenario",
            new GameScenarioRun
            {
                Commands =
                [
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt },
                    new ScenarioCommandData { Command = GameCommands.MouseHunt }
                ],
                ExpectedGameDataAfterCommands = new GameData
                {
                    GameState = GameStates.Won.ToString(),
                    FinishingFood = 15,
                    Accolade = "Hoarder"
                },
                ExpectedStoryDataAfterCommands = new StoryData
                {
                    CurrentDay = 8,
                    StoryText = STORY_TEXT_SIMPLE_WIN
                }
            }
        }
    };
}