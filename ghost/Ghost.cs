using Godot;
using System;
using static Godot.GD;

public partial class Ghost : CharacterBody3D
{

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();


    public StandingState StandingState;
    public WalkingState WalkingState;
    public FallingState FallingState;
    public JumpingState JumpingState;
    public RunningState RunningState;
    public CrouchingState CrouchingState;
    public CrouchingMovingState CrouchingMovingState;
    public CrouchingFallingState CrouchingFallingState;
    public CrouchingJumpingState CrouchingJumpingState;

    public State CurrentState;


    [Export]
    public Node3D Head;

    [Export]
    public CollisionShape3D Collider;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;

        StandingState = new StandingState(this);
        WalkingState = new WalkingState(this);
        FallingState = new FallingState(this);
        JumpingState = new JumpingState(this);
        RunningState = new RunningState(this);
        CrouchingState = new CrouchingState(this);
        CrouchingMovingState = new CrouchingMovingState(this);
        CrouchingFallingState = new CrouchingFallingState(this);
        CrouchingJumpingState = new CrouchingJumpingState(this);

        CurrentState = StandingState;
    }

    public override void _PhysicsProcess(double delta)
    {
        UpdateState();
        CurrentState.Update(delta);
    }


    public override void _Input(InputEvent ev)
    {
        CurrentState.Input(ev);
    }


    public void UpdateState()
    {


        var state = CurrentState.UpdateState();

        if (state == CurrentState)
        {
            return;
        }

        Print(state);


        CurrentState.Exit();
        state.Enter();
        CurrentState = state;
    }

}
