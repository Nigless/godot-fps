using System;
using ExtensionMethods;
using Godot;
using static Godot.GD;


public class CrouchingState : StandingState
{

    public CrouchingState(Ghost stateMachine) : base(stateMachine)
    {
        ColliderHeight = Ghost.CROUCHING_COLLIDER_HEIGHT;
    }


    public override void Enter()
    {
    }


    public override State UpdateState()
    {

        if (!InputIsCrouching && CanStandUp)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }

        if (!_Context.IsOnFloor())
        {
            return _Context.StateMachine.Get<CrouchingFallingState>();
        }

        if (InputIsJumping)
        {
            return _Context.StateMachine.Get<CrouchingJumpingState>();
        }

        if (InputMoving.Length() > 0)
        {
            return _Context.StateMachine.Get<CrouchingMovingState>();
        }

        return this;
    }
}