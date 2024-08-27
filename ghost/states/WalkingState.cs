using ExtensionMethods;
using Godot;
using Microsoft.VisualBasic;
using static Godot.GD;

public class WalkingState : StandingState
{

    protected float Speed = Ghost.WALKING_SPEED;

    public WalkingState(Ghost stateMachine) : base(stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _Context.Animator.Set("parameters/camera_state/conditions/moving", true);
        _Context.Animator.Set("parameters/camera_state/moving/scale/blend_amount", 1f);
    }

    public override void Exit()
    {
        base.Exit();
        _Context.Animator.Set("parameters/camera_state/conditions/moving", false);
        _Context.Animator.Set("parameters/camera_state/moving/speed/scale", 0);
    }

    protected override void UpdateMoving(double delta)
    {
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var moveDirection = (_Context.Transform.Basis * new Vector3(InputMoving.X, 0.0f, InputMoving.Y)).Normalized();

        var velocity = moveVelocity.MoveToward(moveDirection * Speed, Ghost.ACCELERATION * (float)delta);

        _Context.Animator.Set("parameters/camera_state/moving/speed/scale", velocity.Length() * 0.4);
        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override State UpdateState()
    {

        if (InputMoving.Length() == 0 || InputIsJumping || !_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }

        if (InputIsRunning)
        {
            return _Context.StateMachine.Get<RunningState>();
        }

        return this;
    }
}