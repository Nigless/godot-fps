using Godot;
using System;
using System.Diagnostics;


public partial class WalkingState : GhostState
{
    private const float ACCELERATION = 15.0f;
    private const float MOVING_SPEED = 3.0f;

    private StateMachine stateMachine;
    private IdleState idleState;
    private JumpingState jumpingState;
    private FallingState fallingState;


    public override void _Ready()
    {
        stateMachine = GetParent() as StateMachine;
        idleState = stateMachine.GetNode("idle") as IdleState;
        jumpingState = stateMachine.GetNode("jumping") as JumpingState;
        fallingState = stateMachine.GetNode("falling") as FallingState;
    }

    public override void PhysicsProcess(double delta)
    {

        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        var moveDirection = ghost.Transform.Basis * new Vector3(inputDirection.X, 0.0f, inputDirection.Y).Normalized();
        var velocity = ghost.Velocity;
        velocity = velocity.MoveToward(moveDirection * MOVING_SPEED, ACCELERATION * (float)delta);
        velocity.Y = ghost.Velocity.Y;

        ghost.Velocity = velocity;

        ghost.MoveAndSlide();

        if (velocity.Length() <= 0)
        {
            stateMachine.SwitchTo(idleState);
        }

        if (Input.IsActionPressed("jump"))
        {
            stateMachine.SwitchTo(jumpingState);
            return;
        }

        if (!ghost.IsOnFloor())
        {
            stateMachine.SwitchTo(fallingState);
            return;
        }
    }
}
