using Godot;
using Microsoft.VisualBasic;

public class CrouchingMovingState : WalkingState
{


    public CrouchingMovingState(Ghost stateMachine) : base(stateMachine)
    {
        Speed = 1.5f;
    }

    public override void Enter()
    {
        base.Enter();
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", true);
        _Context.Animator.Set("parameters/camera_state/moving/scale/blend_amount", 2f);
    }

    public override void Exit()
    {
        base.Exit();
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", false);
    }

    public override State UpdateState()
    {

        if (InputMoving.Length() == 0 || InputIsJumping || !InputIsCrouching)
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }


        return this;
    }
}