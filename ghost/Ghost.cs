using ExtensionMethods;
using Godot;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

public partial class Ghost : CharacterBody3D
{
	[Export]
	public Node3D Head;
	[Export]
	public Camera3D Camera;
	[Export]
	public ShapeCast3D CastUp;
	[Export]
	public ShapeCast3D CastDown;
	[Export]
	public AnimationPlayer Animator;
	[Export]
	public CollisionShape3D CollisionShape;

	[Export]
	public float MouseSensitivity = 0.1f;
	[Export]
	public float RunningSpeed = 0.05f;
	[Export]
	public float WalkingSpeed = 0.02f;
	[Export]
	public float FallingSpeed = 0.015f;
	[Export]
	public float CrouchingSpeed = 0.015f;
	[Export]
	public float StandingJumpHeight = 0.02f;
	[Export]
	public float StandingAcceleration = 10;
	[Export]
	public float FallingAcceleration = 3;
	[Export]
	public float CrouchingJumpHeight = 0.02f;
	[Export]
	public float CrouchingTransition = 1f;
	[Export]
	public float CrouchingHeight = 1f;
	[Export]
	public float MaxSlopeAngle = 1f;

	private float StandingHeight = 1.7f;
	private CapsuleShape3D Collider;
	private Vector3 Gravity = Vector3.Down * ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Vector2 InputMoving => Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
	private bool InputJumping => Input.IsActionJustPressed("jump");
	private bool InputRunning => Input.IsActionPressed("run");
	private bool InputCrouching => Input.IsActionPressed("crouch");

	private Vector3 GroundSurface;
	private bool Grounded => GroundSurface != Vector3.Zero;
	private float DistanceToGround;
	private float DistanceToCelling;
	private bool CanStand => DistanceToGround + DistanceToCelling > StandingHeight;
	private bool Running => InputRunning && InputMoving.Y <= 0;

	public override void _Ready()
	{
		Collider = (CapsuleShape3D)CollisionShape.Shape;
		StandingHeight = Collider.Height;
		((SphereShape3D)CastDown.Shape).Radius = Collider.Radius;
		((SphereShape3D)CastUp.Shape).Radius = Collider.Radius;

		CastUp.TargetPosition = -Gravity.Normalized() * (StandingHeight - Collider.Radius + SafeMargin);
		CastDown.TargetPosition = Gravity.Normalized() * (StandingHeight - Collider.Radius + SafeMargin);


		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent ev)
	{
		if (ev is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;

			RotateY(-mouseEvent.Relative.X * MouseSensitivity);

			var rotation = Head.Rotation;
			rotation.X = Math.Clamp(rotation.X - mouseEvent.Relative.Y * MouseSensitivity, (float)Math.PI / -2, (float)Math.PI / 2);
			Head.Rotation = rotation;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		UpdateGround();
		UpdateCelling();

		UpdateGravity((float)delta);


		if (InputCrouching)
			UpdateCollider(CrouchingHeight, (float)delta);
		else if (CanStand) UpdateCollider(StandingHeight, (float)delta);

		if (Grounded)
		{
			if (InputCrouching || !CanStand)
			{
				UpdateMoving(CrouchingSpeed);

				if (InputJumping)
					UpdateJumping(CrouchingJumpHeight);
			}
			else
			{
				if (Running)
					UpdateMoving(RunningSpeed);
				else
					UpdateMoving(WalkingSpeed);

				if (InputJumping)
					UpdateJumping(StandingJumpHeight);
			}
		}
		else if (InputMoving.Length() > 0)
			UpdateFalling(FallingSpeed);

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		MoveAndSlide();

		if (GetSlideCollisionCount() == 0)
			return;

		var normal = GetSlideCollision(0).GetNormal();

		if (normal.Dot(Velocity.Normalized()) >= 0)
			return;

		Velocity -= Velocity.Project(normal);
	}

	private void UpdateCollider(float height, float delta)
	{
		if (Collider.Height == height)
		{
			return;
		}

		var transition = CrouchingTransition * delta;

		var headPosition = Head.Transform.Origin;

		headPosition.Y = headPosition.Y.Lerp(height / 2 - Collider.Radius, transition);

		var currentHeight = Collider.Height;

		Collider.Height = currentHeight.Lerp(height, transition);

		if (GroundSurface != Vector3.Zero)
		{
			var heightDiff = (Collider.Height - currentHeight) / 2;

			var transform = Transform;
			transform.Origin += -Gravity.Normalized() * heightDiff;
			Transform = transform;
		}

		var headTransform = Head.Transform;

		headTransform.Origin = headPosition;

		Head.Transform = headTransform;
	}


	private void UpdateCelling()
	{
		if (!CastUp.IsColliding())
		{
			DistanceToCelling = float.PositiveInfinity;
			return;
		}

		DistanceToCelling = CastUp.GetCollisionPoint(0).DistanceTo(Transform.Origin);
	}

	private void UpdateGround()
	{
		if (!CastDown.IsColliding())
		{
			GroundSurface = Vector3.Zero;
			DistanceToGround = float.PositiveInfinity;
			return;
		}

		var distanceToCollision = CastDown.GetCollisionPoint(0).DistanceTo(Transform.Origin);

		DistanceToGround = distanceToCollision;

		if (distanceToCollision > Collider.Height / 2 + SafeMargin)
		{
			GroundSurface = Vector3.Zero;
			return;
		}

		var normal = CastDown.GetCollisionNormal(0);

		if (normal.AngleTo(-Gravity.Normalized()) > MaxSlopeAngle)
		{
			GroundSurface = Vector3.Zero;
			return;
		}

		GroundSurface = normal;
	}

	private void UpdateJumping(float jumpHigh)
	{
		Velocity = Velocity - Velocity.Project(GroundSurface) + GroundSurface * jumpHigh;
	}

	private void UpdateGravity(float delta)
	{
		Velocity += Gravity * delta;
	}

	private void UpdateMoving(float speed)
	{
		var direction = new Quaternion(Vector3.Up, GroundSurface)
			 	* Transform.Basis.GetRotationQuaternion()
				* new Vector3(InputMoving.X, 0.0f, InputMoving.Y).Normalized();

		var verticalVelocity = Velocity.Project(GroundSurface);

		var horizontalVelocity = Velocity - verticalVelocity;

		horizontalVelocity = horizontalVelocity.MoveToward(direction * speed, StandingAcceleration);

		Velocity = horizontalVelocity + verticalVelocity;
	}


	void UpdateFalling(float speed)
	{
		var direction = (Transform.Basis * new Vector3(InputMoving.X, 0f, InputMoving.Y)).Normalized();

		var verticalVelocity = Velocity.Project(-Gravity.Normalized());

		var horizontalVelocity = Velocity - verticalVelocity;

		var acceleration = (((direction * speed) - horizontalVelocity).Normalized().Dot(direction) + 1) / 2 * FallingAcceleration;

		horizontalVelocity = horizontalVelocity.MoveToward(direction * speed, acceleration);

		Velocity = horizontalVelocity + verticalVelocity;
	}

}
