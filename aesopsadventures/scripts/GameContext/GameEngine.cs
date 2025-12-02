using System;
using System.Text.Json.Serialization;
using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.GameContext
{
	public class GameEngine: IGameEngine
	{
		private Game loadedGame = null;

		public bool IsGameRunning => loadedGame?.GameState == GameStates.Playing;

		public Guid CurrentPlayerId => loadedGame?.PlayerId ?? Guid.Empty;
		
		public void New(Guid playerId)
		{
			loadedGame = new Game(GameStates.Playing, playerId);
		}

		public void GameOver()
		{
			loadedGame = loadedGame?.LoseGame();
		}

		public Accolades WinGame(int foodStored)
		{
			var accolade = CalculateAccolade(foodStored);
			loadedGame = loadedGame?.WinGame(foodStored, accolade);

			return accolade;
		}

		private static Accolades CalculateAccolade(int foodStored)
		{
			if (foodStored < 5) return Accolades.Desperado;
			else if (foodStored > 10) return Accolades.Hoarder;

			return Accolades.Survivor;
		}

		public GameData GetGame()
		{
			return loadedGame is null
				? new GameData { }
				: new GameData
				{
					Id = loadedGame.Id.ToString(),
					GameState = loadedGame.GameState.ToString(),
					FinishingFood = loadedGame.FinishingFood,
					Accolade = loadedGame.Accolade.ToString()
				};
		}
		
	}

	public interface IGameEngine
	{

		public bool IsGameRunning { get; }

		public Guid CurrentPlayerId { get; }
		

		public void New(Guid playerId);

		public void GameOver();

		public Accolades WinGame(int foodStored);

		public GameData GetGame();
	}

	public class GameEngineEventDecerator: IGameEngine
	{
		readonly IGameEngine component;
		readonly IEventPub eventBroker;

		public bool IsGameRunning => component.IsGameRunning;

		public Guid CurrentPlayerId => component.CurrentPlayerId;

		public GameEngineEventDecerator(IEventPub eventBroker, IGameEngine component)
		{
			this.eventBroker = eventBroker;
			this.component = component;
		}

		public void New(Guid playerId)
		{
			component.New(playerId);
			eventBroker.Publish(new NewGameStartedEvent(LionStates.Sleeping));
		}

		public void GameOver()
		{
			component.GameOver();
			eventBroker.Publish(new GameLost());
		}

		public Accolades WinGame(int foodStored)
		{
			var accolade = component.WinGame(foodStored);
			eventBroker.Publish(new GameWon(foodStored, accolade));

			return accolade;
		}

		public GameData GetGame()
		{
			return component.GetGame();
		}
	}

	[Serializable]
	public class GameData
	{
		public string Id { get; set; } = string.Empty;
		public string GameState { get; set; } = string.Empty;
		public int FinishingFood { get; set; } = -1;
		public string Accolade { get; set; } = string.Empty;
	}
}
