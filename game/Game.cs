using Godot;
using System;
using static Godot.DisplayServer;

public partial class Game : Node3D
{
	public override void _Input(InputEvent ev)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
			GetTree().Quit();

		if (Input.IsActionJustPressed("fullscreen"))
			if (DisplayServer.WindowGetMode() == WindowMode.Fullscreen)
				DisplayServer.WindowSetMode(WindowMode.Windowed);
			else
				DisplayServer.WindowSetMode(WindowMode.Fullscreen);
	}
}
