using Godot;
using System;

public partial class GameOverMessage : Node2D
{
	public Main MainContext()
	{
		return GetNode<Main>("/root/Main");
	}
	
	public void Retry()
	{
		MainContext().NewGame();
	}
	
	public void Quit()
	{
		MainContext().Quit();
	}
}
