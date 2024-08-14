using System;
using ExtensionMethods;
using Godot;
using static Godot.GD;


public class StandingState : State
{
    protected readonly Ghost _Context;
    protected float ColliderHeight = Ghost.COLLIDER_HEIGHT;
    protected float Fov = Ghost.FOV;


    protected Vector2 InputMoving => Godot.Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
    protected bool InputIsRunning => Godot.Input.IsActionPressed("run");
    protected bool InputIsCrouching => Godot.Input.IsActionPressed("crouch");
    protected bool InputIsJumping => Godot.Input.IsActionJustPressed("jump");

    public StandingState(Ghost context)
    {
        _Context = context;
    }

    protected bool CanStandUp
    {
        get
        {

            if (!_Context.CastUp.IsColliding() || !_Context.CastDown.IsColliding())
                return true;

            var distanceUp = _Context.CastUp.GetCollisionPoint(0).Y.Abs();
            var distanceDown = _Context.CastDown.GetCollisionPoint(0).Y.Abs();

            return distanceUp + distanceDown > Ghost.COLLIDER_HEIGHT;
        }
    }

    public override void Enter()
    {
        Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
    }


    public override void Input(InputEvent ev)
    {
        if (ev is InputEventMouseMotion && Godot.Input.MouseMode == Godot.Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;

            _Context.RotateY(-mouseEvent.Relative.X * Ghost.MOUSE_SENSITIVITY);

            var rotation = _Context.Head.Rotation;
            rotation.X = Math.Clamp(rotation.X - mouseEvent.Relative.Y * Ghost.MOUSE_SENSITIVITY, (float)Math.PI / -2, (float)Math.PI / 2);
            _Context.Head.Rotation = rotation;
        }
    }

    protected virtual void UpdateHead(double delta)
    {

        var position = _Context.Head.Position;
        position.Y = position.Y.Lerp(ColliderHeight / 2 - Ghost.COLLIDER_RADIUS, Ghost.COLLIDER_TRANSITION_SPEED * (float)delta);
        _Context.Head.Position = position;
    }

    protected virtual void UpdateCamera(double delta)
    {
        _Context.Camera.Fov = _Context.Camera.Fov.Lerp(Fov, Ghost.CAMERA_TRANSITION_SPEED * (float)delta);
    }

    protected virtual void UpdateCollider(double delta)
    {
        CapsuleShape3D shape = (CapsuleShape3D)_Context.Collider.Shape;
        var position = _Context.Position;
        var height = shape.Height.Lerp(ColliderHeight, Ghost.COLLIDER_TRANSITION_SPEED * (float)delta);

        position.Y += (height - shape.Height) / 2;
        _Context.Position = position;

        shape.Height = height;
        _Context.Collider.Shape = shape;
    }

    protected virtual void UpdateMoving(double delta)
    {
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var velocity = moveVelocity.MoveToward(Vector3.Zero, Ghost.ACCELERATION * (float)delta);

        velocity.Y = _Context.Velocity.Y;

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override void Update(double delta)
    {
        UpdateCollider(delta);
        UpdateHead(delta);
        UpdateCamera(delta);
        UpdateMoving(delta);
    }

    public override State UpdateState()
    {

        if (InputIsCrouching)
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }

        if (InputIsJumping)
        {
            return _Context.StateMachine.Get<JumpingState>();
        }

        if (!_Context.IsOnFloor())
        {
            return _Context.StateMachine.Get<FallingState>();
        }

        if (InputMoving.Length() > 0)
        {
            return _Context.StateMachine.Get<WalkingState>().UpdateState();
        }

        return this;
    }
}