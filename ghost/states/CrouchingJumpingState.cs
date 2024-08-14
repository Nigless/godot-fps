using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingJumpingState : JumpingState
{
    public CrouchingJumpingState(Ghost stateMachine) : base(stateMachine)
    {
        ColliderHeight = Ghost.CROUCHING_COLLIDER_HEIGHT;
        JumpVelocity = Ghost.JUMP_VELOCITY;
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