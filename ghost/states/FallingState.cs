

using System;
using ExtensionMethods;
using Godot;

public class FallingState : StandingState
{
    private const float SPEED = 1.5f;

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public FallingState(Ghost stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        _Context.Animator.Set("parameters/camera_state/conditions/grounded", false);
    }


    protected override void UpdateMoving(double delta)
    {

        Vector3 velocity;
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var moveDirection = (_Context.Transform.Basis * new Vector3(InputMoving.X, 0.0f, InputMoving.Y)).Normalized();

        var speed = SPEED * (float)delta;

        velocity = moveVelocity + moveDirection * speed;

        if (moveVelocity.Length() > SPEED)
        {
            var coefficient = moveVelocity.AngleTo(moveDirection) / (float)Math.PI;
            velocity = velocity.Normalized()
            * (moveVelocity.Length() - speed * coefficient);
        }
        velocity.Y = (float)(_Context.Velocity.Y - Gravity * delta);

        _Context.Animator.Set("parameters/camera_state/falling/speed/add_amount", velocity.Y.Abs() * 0.01);
        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }

        return this;
    }

}