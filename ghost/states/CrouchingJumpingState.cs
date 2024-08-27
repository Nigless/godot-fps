using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingJumpingState : JumpingState
{
    public CrouchingJumpingState(Ghost stateMachine) : base(stateMachine)
    {
        JumpVelocity = 3f;
    }

    public override void Enter()
    {
        base.Enter();
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", true);
    }

    public override void Exit()
    {
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", false);
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