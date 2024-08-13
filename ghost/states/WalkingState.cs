using Godot;
using Microsoft.VisualBasic;

public class WalkingState : StandingState
{

    protected float SPEED = 2.5f;

    public WalkingState(Ghost stateMachine) : base(stateMachine)
    {
    }

    protected override void UpdateMoving(double delta)
    {
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var moveDirection = (_Context.Transform.Basis * new Vector3(InputMoving.X, 0.0f, InputMoving.Y)).Normalized();

        var velocity = moveVelocity.MoveToward(moveDirection * SPEED, ACCELERATION * (float)delta);
        velocity.Y = _Context.Velocity.Y;

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override State UpdateState()
    {

        if (InputMoving.Length() == 0 || InputIsJumping || !_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }


        if (InputIsRunning)
        {
            return _Context.StateMachine.Get<RunningState>();
        }


        return this;
    }
}