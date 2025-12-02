using Godot;
using System;

public partial class WinMessage : Node2D
{
	public Main MainContext()
	{
		return GetNode<Main>("/root/Main");
	}
	
	public void Replay()
	{
		MainContext().NewGame();
	}
	
	public void Quit()
	{
		MainContext().Quit();
	}
}
