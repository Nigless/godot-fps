using System;
using ExtensionMethods;
using Godot;
using static Godot.GD;


public class StandingState : State
{
    protected readonly Ghost _Context;
    protected float Fov = Ghost.FOV;


    protected Vector2 InputMoving => Godot.Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
    protected bool InputIsRunning => Godot.Input.IsActionPressed("run");
    protected bool InputIsCrouching => Godot.Input.IsActionPressed("crouch");
    protected bool InputIsJumping => Godot.Input.IsActionJustPressed("jump");

    public StandingState(Ghost context)
    {
        _Context = context;
    }

    public override void Enter()
    {
        Godot.Input.MouseMode = Godot.Input.MouseModeEnum.Captured;
        _Context.Animator.Set("parameters/camera_state/conditions/grounded", true);
    }

    public override void Exit()
    {
        _Context.Animator.Set("parameters/camera_state/conditions/grounded", false);
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

    protected virtual void UpdateMoving(double delta)
    {
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var velocity = moveVelocity.MoveToward(Vector3.Zero, Ghost.ACCELERATION * (float)delta);

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override void Update(double delta)
    {
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