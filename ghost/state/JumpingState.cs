using Godot;
using System;
using System.Diagnostics;


public partial class JumpingState : GhostState
{
	private const float MOVING_SPEED = 0.05f;
	private const float JUMP_VELOCITY = 4.5f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


	private StateMachine stateMachine;
	private FallingState fallingState;
	private IdleState idleState;


	public override void _Ready()
	{
		stateMachine = GetParent() as StateMachine;
		fallingState = stateMachine.GetNode("falling") as FallingState;
		idleState = stateMachine.GetNode("idle") as IdleState;
	}

	public override void Enter()
	{
		var velocity = ghost.Velocity;
		velocity.Y = JUMP_VELOCITY;
		ghost.Velocity = velocity;
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

		if (ghost.Velocity.Y < 0)
		{
			stateMachine.SwitchTo(fallingState);
			return;
		}

		if (ghost.IsOnFloor())
		{
			stateMachine.SwitchTo(idleState);
			return;
		}
	}
}
