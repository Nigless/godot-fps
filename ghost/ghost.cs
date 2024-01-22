using Godot;
using System;
using System.Diagnostics;

public partial class ghost : CharacterBody3D
{

	private const float BASE_SPEED = 2.0f;
	private const float FALL_SPEED = BASE_SPEED * 0.01f;
	private const float SPRINT_SPEED = 5.0f;
	private const float ACCELERATION = BASE_SPEED * 0.2f;
	private const float JUMP_VELOCITY = 4.5f;

	private Vector3 moveVelocity = Vector3.Zero;
	private const float MOUSE_SENSITIVITY = 0.001f;

	private Node3D head;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		head = GetNode("head") as Node3D;
	}



	public override void _PhysicsProcess(double delta)
	{

		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		var moveDirection = Transform.Basis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y).Normalized();
		var velocity = Velocity;

		if (IsOnFloor())
		{

			velocity.X *= 0.9f;
			velocity.Z *= 0.9f;

			var speed = (Input.IsActionPressed("run") ? SPRINT_SPEED : BASE_SPEED) * 0.1f;
			velocity += moveDirection * speed;


			if (Input.IsActionJustPressed("jump"))
			{
				velocity.Y = JUMP_VELOCITY;
			}
		}
		else
		{
			velocity += moveDirection * FALL_SPEED;

			velocity.Y -= gravity * (float)delta;
		}

		Velocity = velocity;

		Trace.WriteLine(velocity);

		MoveAndSlide();
	}

	public override void _Input(InputEvent ev)
	{
		if (ev is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;
			RotateY(-mouseEvent.Relative.X * MOUSE_SENSITIVITY);

			var angle = -mouseEvent.Relative.Y * MOUSE_SENSITIVITY;
			var rotation = head.Rotation;
			rotation.X = (float)Math.Min(Math.Max(rotation.X + angle, -Math.PI / 2), Math.PI / 2);
			head.Rotation = rotation;
		}

	}
}
