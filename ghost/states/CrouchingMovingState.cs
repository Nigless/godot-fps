using Godot;
using Microsoft.VisualBasic;

public class CrouchingMovingState : WalkingState
{


    public CrouchingMovingState(Ghost stateMachine) : base(stateMachine)
    {
        Speed = Ghost.FALLING_SPEED;
        ColliderHeight = Ghost.CROUCHING_COLLIDER_HEIGHT;
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