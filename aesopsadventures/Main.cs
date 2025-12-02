using System;
using System.Collections.Generic;
using System.Transactions;
using Godot;
using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

public partial class Main : Node
{
	GameEventMediator broker = new();
	LogicLoop logicLoop = new LogicLoop();
	public StoryData Story => logicLoop.Story;
	public MouseData Mouse => logicLoop.Mouse;
	public LionData Lion => logicLoop.Lion;
	public GameData Game => logicLoop.Game;

	PackedScene currentScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		logicLoop.Init();
		Console.WriteLine("process main");
	}
	

	private void CheckGameResult()
	{
		if (logicLoop.GetGame().GameState == GameStates.Lost.ToString())
		{
			GoTo("game_over_message");
		}

		if (logicLoop.GetGame().GameState == GameStates.Won.ToString())
		{
			GoTo("win_message");
		}
	}

	public void Quit()
	{
		GetTree().Quit();
	}

	public void NewGame()
	{
		logicLoop.NewGame(Guid.NewGuid());
		GoTo("story_page");
	}

	public void StayAtHome()
	{
		logicLoop.MouseStayAtHome();
		CheckGameResult();
	}

	public void Hunt()
	{
		logicLoop.MouseHunt();
		CheckGameResult();
	}

	private void GoTo(string sceneName)
	{
		this.GetTree().ChangeSceneToFile($"res://{sceneName}.tscn");
	}

	internal IReadOnlyDictionary<StoryOptions, StoryOption> GetAllStoryOptions()
	{
		return logicLoop.GetAllStoryOptions();
	}
}
