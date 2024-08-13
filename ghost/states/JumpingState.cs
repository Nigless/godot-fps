using static Godot.GD;


public class JumpingState : FallingState
{
    protected float JUMP_VELOCITY = 4.5f;



    public JumpingState(Ghost stateMachine) : base(stateMachine)
    {

    }

    public override void Enter()
    {
        var velocity = _Context.Velocity;
        velocity.Y += JUMP_VELOCITY;
        _Context.Velocity = velocity;
    }


    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || _Context.Velocity.Y <= 0 || InputIsCrouching)
        {
            return _Context.StateMachine.Get<FallingState>().UpdateState();
        }

        return this;
    }

}