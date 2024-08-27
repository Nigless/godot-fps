using Godot;
using System;
using static Godot.GD;

public partial class Ghost : CharacterBody3D
{
	[Export]
	public Node3D Head;


	[Export]
	public Camera3D Camera;

	[Export]
	public CollisionShape3D Collider;

	[Export]
	public ShapeCast3D CastUp;

	[Export]
	public ShapeCast3D CastDown;

	[Export]
	public AnimationTree Animator;

	public StateMachine StateMachine;


	public const float MOUSE_SENSITIVITY = 0.001f;
	public const float FOV = 90;
	public const float ACCELERATION = 20.0f;
	public const float COLLIDER_TRANSITION_SPEED = 10.0f;
	public const float CAMERA_TRANSITION_SPEED = 5.0f;
	public const float COLLIDER_RADIUS = 0.327f;
	public const float WALKING_SPEED = 2.5f;

	public float ColliderHigh = 1.7f;

	public override void _Ready()
	{

		var initial = new StandingState(this);
		initial.Enter();

		StateMachine = new StateMachine(initial)
			.WithState(new WalkingState(this))
			.WithState(new FallingState(this))
			.WithState(new JumpingState(this))
			.WithState(new RunningState(this))
			.WithState(new CrouchingState(this))
			.WithState(new CrouchingMovingState(this))
			.WithState(new CrouchingFallingState(this))
			.WithState(new CrouchingJumpingState(this));


		ColliderHigh = ((CapsuleShape3D)Collider.Shape).Height;
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
