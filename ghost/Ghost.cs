using ExtensionMethods;
using Godot;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Metadata;
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
	public RayCast3D RayCast;

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
	[Export]
	public float Mass = 1f;
	[Export]
	public float MaxMassHold = 1f;

	private float StandingHeight = 1.7f;
	private Node3D Cursor;
	private RigidBody3D? GrabbedBody = null;
	private CapsuleShape3D Collider;
	private Vector3 Gravity = Vector3.Down * ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private Vector2 InputMoving => Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
	private bool InputJumping => Input.IsActionJustPressed("jump");
	private bool InputRunning => Input.IsActionPressed("run");
	private bool InputGrabbing => Input.IsActionPressed("grab");
	private bool InputCrouching => Input.IsActionPressed("crouch");

	private Vector3 GroundSurface;
	private bool Grabbing;
	private bool Grounded => GroundSurface != Vector3.Zero;
	private float DistanceToGround;
	private float DistanceToCelling;
	private bool CanStand => DistanceToGround + DistanceToCelling > StandingHeight;
	private Interact InteractObj;
	private bool Running => InputRunning && InputMoving.Y <= 0;

	public override void _Ready()
	{
		Collider = (CapsuleShape3D)CollisionShape.Shape;
		StandingHeight = Collider.Height;
		((SphereShape3D)CastDown.Shape).Radius = Collider.Radius;
		((SphereShape3D)CastUp.Shape).Radius = Collider.Radius;

		CastUp.TargetPosition = -Gravity.Normalized() * (StandingHeight - Collider.Radius + SafeMargin);
		CastDown.TargetPosition = Gravity.Normalized() * (StandingHeight - Collider.Radius + SafeMargin);

		Cursor = (Node3D)GD.Load<PackedScene>("res://cursor/cursor.tscn").Instantiate();

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
		UpdateInteracting();
		UpdateGrabbing();
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

		UpdatePulling((float)delta);

		UpdatePosition();
	}

	private void UpdatePosition()
	{
		if (!MoveAndSlide())
			return;

		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);

			var collider = collision.GetCollider();

			var normal = collision.GetNormal();

			if (collider is RigidBody3D)
			{
				var rigidBody = collider as RigidBody3D;

				float rigidBodyCoefficient = rigidBody.LinearVelocity.Dot(normal);
				float characterCoefficient = Velocity.Dot(normal);

				if (rigidBodyCoefficient - characterCoefficient > 0)
				{
					float impulseMagnitude = -1 * (rigidBodyCoefficient - characterCoefficient);
					impulseMagnitude /= (1 / Mass) + (1 / rigidBody.Mass);

					Vector3 impulse = impulseMagnitude * normal;

					Velocity -= impulse / Mass;

					rigidBody.ApplyImpulse(impulse, Transform.Origin - rigidBody.Transform.Origin);
				}
			}

			if (normal.Dot(Velocity.Normalized()) >= 0)
				continue;

			Velocity -= Velocity.Project(normal);
		}
	}

	private void UpdatePulling(float delta)
	{
		if (GrabbedBody == null)
			return;

		var targetPosition = RayCast.GlobalPosition + RayCast.GlobalBasis * (RayCast.TargetPosition * 0.5f);

		if (GrabbedBody.Mass < MaxMassHold)
		{
			var impulse = targetPosition - GrabbedBody.GlobalPosition;
			if (impulse.Length() > 0.5)
			{
				ReleaseBody();
				return;
			}

			GrabbedBody.LinearVelocity = impulse / delta;
			GrabbedBody.AngularVelocity *= 0;
			return;
		}

		var handLength = RayCast.TargetPosition.Length();

		var cursorPosition = Cursor.GlobalPosition;

		if (GlobalPosition.DistanceTo(cursorPosition) > handLength)
		{
			ReleaseBody();
			return;
		}

		var pullingDirection = targetPosition - cursorPosition;

		GrabbedBody.ApplyImpulse(pullingDirection, cursorPosition - GrabbedBody.GlobalPosition);

		if (GroundSurface == Vector3.Zero)
			return;

		var currentHand = cursorPosition - (GlobalPosition + Velocity);

		var oppositeForce = currentHand.Normalized() * Math.Max(0, currentHand.Length() - handLength);

		Velocity += oppositeForce - oppositeForce.Project(GroundSurface);
	}

	private void HoverExit()
	{
		if (InteractObj != null)
			InteractObj.HoverExit();

		InteractObj = null;
	}

	private void UpdateInteracting()
	{

		if (!RayCast.IsColliding())
		{
			HoverExit();
			return;
		}

		var collider = RayCast.GetCollider();

		if (collider is not Interact)
		{
			HoverExit();
			return;
		}

		var interact = (Interact)collider;

		if (InputGrabbing && !Grabbing)
			interact.Trigger();

		if (interact == InteractObj)
			return;

		if (InteractObj != null)
			InteractObj.HoverExit();

		interact.HoverEnter();
		InteractObj = interact;
	}

	private void UpdateGrabbing()
	{

		if (InputGrabbing)
		{
			if (Grabbing) return;

			Grabbing = true;

			if (GrabbedBody != null)
				return;

			if (!RayCast.IsColliding())
				return;

			var collider = RayCast.GetCollider();

			var point = RayCast.GetCollisionPoint();

			if (collider is not RigidBody3D)
				return;

			Grabbing = true;

			GrabbedBody = (RigidBody3D)collider;

			GrabbedBody.CanSleep = false;

			if (GrabbedBody.Mass < MaxMassHold)
				return;

			{
				var transform = Cursor.Transform;
				transform.Origin = (point - GrabbedBody.Transform.Origin) * -GrabbedBody.Transform.Basis.GetRotationQuaternion();
				Cursor.Transform = transform;
			}

			GrabbedBody.AddChild(Cursor);

			return;
		}

		Grabbing = false;

		ReleaseBody();
	}

	private void ReleaseBody()
	{
		if (GrabbedBody == null)
			return;

		if (GrabbedBody.Mass > MaxMassHold)
			GrabbedBody.RemoveChild(Cursor);
		else
			GrabbedBody.LinearVelocity *= 0.5f;

		GrabbedBody.CanSleep = true;
		GrabbedBody = null;
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

		for (int i = 0; i < CastDown.GetCollisionCount(); i++)
		{
			var distanceToCollision = CastDown.GetCollisionPoint(i).DistanceTo(Transform.Origin);

			DistanceToGround = distanceToCollision;

			if (distanceToCollision > Collider.Height / 2 + SafeMargin * 3)
			{
				GroundSurface = Vector3.Zero;
				continue;
			}

			var normal = CastDown.GetCollisionNormal(i);

			if (normal.AngleTo(-Gravity.Normalized()) > MaxSlopeAngle)
			{
				GroundSurface = Vector3.Zero;
				continue;
			}

			var collider = CastDown.GetCollider(i);

			if (collider is RigidBody3D)
			{
				var body = (RigidBody3D)collider;

				if (body == GrabbedBody)
					ReleaseBody();
			}

			GroundSurface = normal;

			return;
		}
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
