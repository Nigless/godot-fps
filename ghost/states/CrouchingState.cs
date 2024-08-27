using System;
using ExtensionMethods;
using Godot;
using static Godot.GD;


public class CrouchingState : StandingState
{


    protected bool CanStandUp
    {
        get
        {
            if (!_Context.CastUp.IsColliding() || !_Context.CastDown.IsColliding())
                return true;

            var distanceUp = _Context.CastUp.GetCollisionPoint(0).Y.Abs();
            var distanceDown = _Context.CastDown.GetCollisionPoint(0).Y.Abs();

            return distanceUp + distanceDown > _Context.ColliderHigh;
        }
    }

    public CrouchingState(Ghost stateMachine) : base(stateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", true);
    }

    public override void Exit()
    {
        base.Exit();
        _Context.Animator.Set("parameters/collider_state/conditions/crouching", false);
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