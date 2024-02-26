using Godot;
using System;
using System.Diagnostics;


public partial class FallingState : GhostState
{
	private const float MOVING_SPEED = 0.05f;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	private StateMachine stateMachine;
	private IdleState idleState;
	private WalkingState walkingState;


	public override void _Ready()
	{
		stateMachine = GetParent() as StateMachine;
		idleState = stateMachine.GetNode("idle") as IdleState;
		walkingState = stateMachine.GetNode("walking") as WalkingState;
	}

	public override void PhysicsProcess(double delta)
	{

		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		var moveDirection = ghost.Transform.Basis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y).Normalized();
		var velocity = ghost.Velocity;

		velocity += moveDirection * MOVING_SPEED;

		velocity.Y -= gravity * (float)delta;

		ghost.Velocity = velocity;

		ghost.MoveAndSlide();


		if (ghost.IsOnFloor() && ghost.Velocity.Length() > 0)
		{
			stateMachine.SwitchTo(walkingState);
			return;
		}

		if (ghost.IsOnFloor())
		{
			stateMachine.SwitchTo(idleState);
			return;
		}
	}
}
