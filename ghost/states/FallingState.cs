

using System;
using Godot;

public class FallingState : StandingState
{
    private const float SPEED = 2.0f;



    public FallingState(Ghost stateMachine) : base(stateMachine)
    {

    }


    public override void Update(double delta)
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
        velocity.Y = _Context.Velocity.Y - _Context.Gravity * (float)delta;

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }


    public override State UpdateState()
    {
        if (_Context.IsOnFloor())
        {
            return _Context.StandingState.UpdateState();
        }

        return this;
    }

}