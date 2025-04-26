using Godot;
using System;

public partial class StartMenu : Node2D
{
	public Main MainContext()
	{
		return GetNode<Main>("/root/Main");
	}

	public void PlayGame()
	{
		MainContext().NewGame();
	}

	public void Quit()
	{
		MainContext().Quit();
	}
}
