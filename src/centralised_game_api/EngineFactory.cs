using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

namespace centralised_game_api
{
    public class EngineFactory
    {
        GameEngine gameEngine;
        StoryEngine storyEngine;
        MouseEngine mouseEngine;
        LionEngine lionEngine;

        public EngineFactory(IEventPub eventPublisher)
        {
            gameEngine = new(eventPublisher);
            storyEngine = new(eventPublisher);
            mouseEngine = new(eventPublisher);
            lionEngine = new(eventPublisher);
        }

        public GameEngine GetGameEngine() => gameEngine;
        public StoryEngine GetStoryEngine() => storyEngine;
        public MouseEngine GetMouseEngine() => mouseEngine;
        public LionEngine GetLionEngine() => lionEngine;
    }
}