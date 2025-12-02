using System;
using System.Collections.Generic;
using Godot;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

public partial class StoryPage : Node2D
{
	private readonly Dictionary<StoryOptions, Button> optionBtns = new();
	public Main MainContext()
	{
		return GetNode<Main>("/root/Main");
	}

	public void StayAtHome()
	{
		MainContext().StayAtHome();
		ClearButtonList();
	}
	
	public void Hunt()
	{
		MainContext().Hunt();
		ClearButtonList();
	}
	
	public void Quit()
	{
		MainContext().Quit();
		ClearButtonList();
	}
	
	public void Restart()
	{
		MainContext().NewGame();
		ClearButtonList();
	}

	public void CreateOptionButtons()
	{
		IReadOnlyDictionary<StoryOptions, StoryOption> options = MainContext().GetAllStoryOptions();
		foreach (var kvp in options)
		{
			var option = kvp.Value;
			var button = new Button();
			button.Text = option.OptionText;
			button.Pressed += () => option.Select();

			if (kvp.Key == StoryOptions.MouseStayAtHome)
			{
				button.Pressed += () => this.StayAtHome();
			}
			
			if (kvp.Key == StoryOptions.MouseHunt)
			{
				button.Pressed += () => this.Hunt(); 
			}

			optionBtns.Add(kvp.Key, button);		
		}
	}

	
	private void ClearButtonList()
	{
		var btnList = GetNode<VFlowContainer>("OptionBtnsList");

		for (int i = 0; i < btnList.GetChildCount(); i++)
		{
			btnList.RemoveChild(btnList.GetChild(i));
		}
	}

	public override void _Ready()
	{
		CreateOptionButtons();
		Console.WriteLine("ready story page");
		
		base._Ready();
	}

	public override void _Process(double delta)
	{
		Console.WriteLine("process story page");
		var mainContext = MainContext();
		
		GetNode<Label>("MouseDataLbl").Text = $"Mouse\r\n\tState: {mainContext.Mouse.State}\r\n\tFood: {mainContext.Mouse.Food}";
		GetNode<Label>("StoryDataLbl").Text = $"Game ID: {mainContext.Game.Id}\r\nDay: {mainContext.Story.CurrentDay}";
		GetNode<Label>("LionDataLbl").Text = $"Lion\r\n\tState: {mainContext.Lion.State}";
		GetNode<RichTextLabel>("StoryTextLbl").Text = mainContext.Story.StoryText;


		foreach (StoryOptions option in mainContext.Story.Options)
		{
			Console.WriteLine("adding option");
			var btnList = GetNode<VFlowContainer>("OptionBtnsList");
			btnList.AddChild(optionBtns[option]);
		}
		
		base._Process(delta);
	}
}
