using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;


public class CrouchingFallingState : CrouchingState
{

    public CrouchingFallingState(Ghost stateMachine) : base(stateMachine)
    {
    }

    public override void Update(double delta)
    {
        _Context.StateMachine.Get<FallingState>().Update(delta);
    }

    public override State UpdateState()
    {
        if (_Context.IsOnFloor())
        {
            return _Context.StateMachine.Get<CrouchingState>().UpdateState();
        }

        return this;
    }
}