using Godot;
using Microsoft.VisualBasic;

public class CrouchingMovingState : WalkingState
{


    public CrouchingMovingState(Ghost stateMachine) : base(stateMachine)
    {
        SPEED = 1f;
        COLLIDER_HEIGHT = 1.0f;
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