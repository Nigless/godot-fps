

using System;
using ExtensionMethods;
using Godot;

public class FallingState : StandingState
{
    private const float SPEED = Ghost.FALLING_SPEED;

    public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public FallingState(Ghost stateMachine) : base(stateMachine)
    {

    }
    protected override void UpdateCollider(double delta)
    {
        CapsuleShape3D shape = (CapsuleShape3D)_Context.Collider.Shape;
        var height = shape.Height.Lerp(ColliderHeight, Ghost.COLLIDER_TRANSITION_SPEED * (float)delta);

        shape.Height = height;
        _Context.Collider.Shape = shape;
    }


    protected override void UpdateMoving(double delta)
    {
        Vector3 velocity;
        var moveVelocity = new Vector3(_Context.Velocity.X, 0.0f, _Context.Velocity.Z);

        var moveDirection = (_Context.Transform.Basis * new Vector3(InputMoving.X, 0.0f, InputMoving.Y)).Normalized();

        var speed = SPEED * (float)delta;

        velocity = moveVelocity + moveDirection * speed;

        if (moveVelocity.Length() > SPEED)
        {
            var coefficient = moveVelocity.AngleTo(moveDirection) / (float)Math.PI;
            velocity = velocity.Normalized()
            * (moveVelocity.Length() - speed * coefficient);
        }
        velocity.Y = _Context.Velocity.Y - Gravity * (float)delta;

        _Context.Velocity = velocity;
        _Context.MoveAndSlide();
    }

    public override State UpdateState()
    {
        if (_Context.IsOnFloor() || InputIsCrouching)
        {
            return _Context.StateMachine.Get<StandingState>().UpdateState();
        }

        return this;
    }

}