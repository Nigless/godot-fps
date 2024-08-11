public class RunningState : WalkingState
{


    public RunningState(Ghost stateMachine) : base(stateMachine)
    {
        SPEED = 4.0f;
    }



    public override State UpdateState()
    {
        if (!InputIsRunning || InputIsJumping || !_Context.IsOnFloor())
        {
            return _Context.WalkingState.UpdateState();
        }


        return this;
    }
}