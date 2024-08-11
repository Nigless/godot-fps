using Godot;

public class CrouchingState : StandingState
{
    protected new const float COLLIDER_HEIGHT = 1.0f;

    public CrouchingState(Ghost stateMachine) : base(stateMachine)
    {
    }

    public override void Update(double delta)
    {
        CapsuleShape3D shape = (CapsuleShape3D)_Context.Collider.Shape;

        shape.Height = Lerp.Transit(shape.Height, COLLIDER_HEIGHT, COLLIDER_ACCELERATION * (float)delta);
    }

    public override State UpdateState()
    {
        if (!InputIsCrouching)
        {
            return _Context.StandingState.UpdateState();
        }

        if (!_Context.IsOnFloor())
        {
            return _Context.CrouchingFallingState;
        }

        if (InputIsJumping)
        {
            return _Context.CrouchingJumpingState;
        }

        if (InputMoving.Length() > 0)
        {
            return _Context.CrouchingMovingState;
        }

        return this;
    }
}