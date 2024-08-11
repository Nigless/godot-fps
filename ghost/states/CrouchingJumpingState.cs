using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingJumpingState : CrouchingFallingState
{

    private const float JUMP_VELOCITY = 3.5f;


    public CrouchingJumpingState(Ghost stateMachine) : base(stateMachine)
    {
    }


    public override void Enter()
    {
        var velocity = _Context.Velocity;
        velocity.Y += JUMP_VELOCITY;
        _Context.Velocity = velocity;
    }

    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || _Context.Velocity.Y <= 0)
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }

        return this;
    }
}