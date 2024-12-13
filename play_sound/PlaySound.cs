using Godot;
using System;

public partial class PlaySound : AudioStreamPlayer3D
{
	[Export]
	public Interact Trigger;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		Trigger.Triggered += () => Play();
	}

}
