using Godot;
using System;
using System.Diagnostics;

public partial class Spawner : Node3D
{
	[Export]
	public Interact Trigger;
	[Export]
	public PackedScene Object;

	public override void _Ready()
	{
		Trigger.Triggered += Spawn;
	}

	private void Spawn()
	{
		var node = (Node3D)Object.Instantiate();
		node.Position = Position;
		GetParent().AddChild(node);
	}

}
