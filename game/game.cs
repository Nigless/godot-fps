using Godot;
using System;

public partial class game : Node3D
{
	public override void _Input(InputEvent ev)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
			GetTree().Quit();
	}
}
