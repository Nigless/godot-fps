// class GhostStateMachine
// {
//     public StandingState StandingState;
//     public WalkingState WalkingState;
//     public FallingState FallingState;
//     public JumpingState JumpingState;
//     public RunningState RunningState;
//     public CrouchingState CrouchingState;
//     public CrouchingMovingState CrouchingMovingState;
//     public CrouchingFallingState CrouchingFallingState;
//     public CrouchingJumpingState CrouchingJumpingState;

//     GhostStateMachine(Ghost context)
//     {
//         StandingState = new StandingState(context, this);
//         WalkingState = new WalkingState(context);
//         FallingState = new FallingState(context);
//         JumpingState = new JumpingState(context);
//         RunningState = new RunningState(context);
//         CrouchingState = new CrouchingState(context);
//         CrouchingMovingState = new CrouchingMovingState(context);
//         CrouchingFallingState = new CrouchingFallingState(context);
//         CrouchingJumpingState = new CrouchingJumpingState(context);

//         CurrentState = StandingState;
//     }

// }