using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingFallingState : FallingState
{

    public CrouchingFallingState(Ghost stateMachine) : base(stateMachine)
    {
        ColliderHeight = Ghost.CROUCHING_COLLIDER_HEIGHT;
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