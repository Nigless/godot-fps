using Godot;
using System;

public partial class Pointer : TextureRect
{
	[Export]
	public Interact Trigger;

	public override void _Ready()
	{
		Trigger.HoverEntered += () => Visible = true;
		Trigger.HoverExited += () => Visible = false;
	}


}
