using Godot;
using System;
using System.Diagnostics;


public partial class IdleState : GhostState
{
	private const float JUMP_VELOCITY = 4.5f;

	private StateMachine stateMachine;
	private JumpingState jumpingState;
	private WalkingState walkingState;
	private FallingState fallingState;

	public override void Enter()
	{
		base.Enter();
	}

	public override void _Ready()
	{
		stateMachine = GetParent() as StateMachine;
		jumpingState = stateMachine.GetNode("jumping") as JumpingState;
		walkingState = stateMachine.GetNode("walking") as WalkingState;
		fallingState = stateMachine.GetNode("falling") as FallingState;
	}

	public override void PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("jump"))
		{
			stateMachine.SwitchTo(jumpingState);
			return;
		}

		var inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");

		if (inputDirection.Length() > 0)
		{
			stateMachine.SwitchTo(walkingState);
			return;
		}

		if (!ghost.IsOnFloor())
		{
			stateMachine.SwitchTo(fallingState);
			return;
		}
	}
}
