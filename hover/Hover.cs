using Godot;
using System;
using System.Diagnostics;
using static Godot.BaseMaterial3D;

public partial class Hover : MeshInstance3D
{

	[Export]
	public Interact Trigger;

	[Export]
	public StandardMaterial3D Material = (StandardMaterial3D)GD.Load<StandardMaterial3D>("res://hover/hover.tres").Duplicate();

	public StandardMaterial3D CurrentMaterial;

	public override void _Ready()
	{
		Trigger.HoverEntered += HoverEntered;
		Trigger.HoverExited += HoverExited;

		CurrentMaterial = (StandardMaterial3D)Mesh.SurfaceGetMaterial(0);

		var duplicate = (StandardMaterial3D)CurrentMaterial.Duplicate();

		duplicate.Transparency = TransparencyEnum.AlphaDepthPrePass;
		duplicate.CullMode = CullModeEnum.Back;

		Material.GrowAmount = 0.03f / new Vector3(GlobalTransform[0][0], GlobalTransform[1][1], GlobalTransform[2][2]).Length();
		Material.NextPass = duplicate;
	}

	public void HoverEntered()
	{
		MaterialOverride = Material;
	}


	public void HoverExited()
	{
		MaterialOverride = CurrentMaterial;
	}
}
