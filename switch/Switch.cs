using Godot;
using System;

public partial class Switch : Node3D
{
	[Export]
	public Interact Trigger;

	public override void _Ready()
	{
		Trigger.Triggered += Triggered;
	}

	private void Triggered()
	{
		Visible = !Visible;
	}

}
