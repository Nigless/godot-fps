public class RunningState : WalkingState
{


    public RunningState(Ghost stateMachine) : base(stateMachine)
    {
        Speed = 4.5f;
        Fov = Ghost.FOV * 1.1f;
    }


    public override State UpdateState()
    {
        if (InputMoving.Length() == 0 || !InputIsRunning || InputIsJumping || !_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<WalkingState>().UpdateState();
        }


        return this;
    }
}