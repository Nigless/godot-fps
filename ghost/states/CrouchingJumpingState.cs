using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingJumpingState : JumpingState
{
    public CrouchingJumpingState(Ghost stateMachine) : base(stateMachine)
    {
        COLLIDER_HEIGHT = 1.0f;
        JUMP_VELOCITY = 3.0f;
    }

    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || _Context.Velocity.Y <= 0 || !InputIsCrouching)
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }

        return this;
    }
}