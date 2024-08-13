public class RunningState : WalkingState
{


    public RunningState(Ghost stateMachine) : base(stateMachine)
    {
        SPEED = 4.5f;
    }



    public override State UpdateState()
    {
        if (!InputIsRunning || InputIsJumping || !_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<WalkingState>().UpdateState();
        }


        return this;
    }
}