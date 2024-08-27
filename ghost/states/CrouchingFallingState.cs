using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingFallingState : FallingState
{

    public CrouchingFallingState(Ghost stateMachine) : base(stateMachine)
    {
    }


    public override void Enter()
    {
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", true);
        _Context.Animator.Set("parameters/camera_state/conditions/grounded", false);
    }

    public override void Exit()
    {
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", false);
    }


    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || !InputIsCrouching)
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }

        return this;
    }
}