using System;
using Godot;
using static Godot.GD;


public class StandingState : State
{
    protected readonly Ghost _Context;
    private const float MOUSE_SENSITIVITY = 0.001f;
    protected const float ACCELERATION = 20.0f;
    protected const float COLLIDER_ACCELERATION = 20.0f;
    protected const float COLLIDER_HEIGHT = 1.7f;


    protected Vector2 InputMoving => Godot.Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
    protected bool InputIsRunning => Godot.Input.IsActionPressed("run");
    protected bool InputIsCrouching => Godot.Input.IsActionPressed("crouch");
    protected bool InputIsJumping => Godot.Input.IsActionJustPressed("jump");

    public StandingState(Ghost context)
    {
        _Context = context;
    }


    public override void Input(InputEvent ev)
    {
        if (ev is InputEventMouseMotion && Godot.Input.MouseMode == Godot.Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;

            _Context.RotateY(-mouseEvent.Relative.X * MOUSE_SENSITIVITY);

            var rotation = _Context.Head.Rotation;
            rotation.X = Math.Clamp(rotation.X - mouseEvent.Relative.Y * MOUSE_SENSITIVITY, (float)Math.PI / -2, (float)Math.PI / 2);
            _Context.Head.Rotation = rotation;
        }
    }


    public override void Update(double delta)
    {
        CapsuleShape3D shape = (CapsuleShape3D)_Context.Collider.Shape;

        shape.Height = Lerp.Transit(shape.Height, COLLIDER_HEIGHT, COLLIDER_ACCELERATION * (float)delta);

        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var velocity = moveVelocity.MoveToward(Vector3.Zero, ACCELERATION * (float)delta);

        velocity.Y = _Context.Velocity.Y;

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override State UpdateState()
    {
        if (InputIsCrouching)
        {
            return _Context.CrouchingState.UpdateState();
        }

        if (InputIsJumping)
        {
            return _Context.JumpingState;
        }

        if (!_Context.IsOnFloor())
        {
            return _Context.FallingState;
        }

        if (InputMoving.Length() > 0)
        {
            return _Context.WalkingState.UpdateState();
        }

        return this;
    }
}