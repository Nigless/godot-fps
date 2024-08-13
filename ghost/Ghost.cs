using Godot;
using System;
using static Godot.GD;

public partial class Ghost : CharacterBody3D
{
	[Export]
	public Node3D Head;
	[Export]
	public CollisionShape3D Collider;
	[Export]
	public AnimationPlayer Animation;

	public StateMachine StateMachine;

	public override void _Ready()
	{
		StateMachine = new StateMachine(new StandingState(this))
			.WithState(new WalkingState(this))
			.WithState(new FallingState(this))
			.WithState(new JumpingState(this))
			.WithState(new RunningState(this))
			.WithState(new CrouchingState(this))
			.WithState(new CrouchingMovingState(this))
			.WithState(new CrouchingFallingState(this))
			.WithState(new CrouchingJumpingState(this));
	}

	public override void _PhysicsProcess(double delta)
	{
		StateMachine.Update(delta);
	}

	public override void _Input(InputEvent ev)
	{
		StateMachine.Input(ev);
	}

}
