using Godot;
using System;
using System.Diagnostics;

public partial class Spectator : Node3D
{

	private const float BASE_SPEED = 5.0f;
	private const float SPRINT_SPEED = 20.0f;

	private Vector3 moveVelocity = Vector3.Zero;
	private const float MOUSE_SENSITIVITY = 0.001f;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		var moveDirection = Transform.Basis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y).Normalized();

		var pos = Transform;
		pos.Origin += moveDirection * (Input.IsActionPressed("run") ? SPRINT_SPEED : BASE_SPEED) * (float)delta;

		Transform = pos;
	}

	public override void _Input(InputEvent ev)
	{
		if (ev is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;
			RotateY(-mouseEvent.Relative.X * MOUSE_SENSITIVITY);

			var angle = -mouseEvent.Relative.Y * MOUSE_SENSITIVITY;
			var rotation = Rotation;
			rotation.X = (float)Math.Min(Math.Max(rotation.X + angle, -Math.PI / 2), Math.PI / 2);
			Rotation = rotation;
		}

	}
}
