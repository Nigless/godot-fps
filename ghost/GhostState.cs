using Godot;
using System;
using System.Diagnostics;


public partial class GhostState : State
{
    private const float MOUSE_SENSITIVITY = 0.001f;



    [Export] public Node3D head;
    [Export] public CharacterBody3D ghost;

    public override void Enter()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void HandleInput(InputEvent ev)
    {
        if (ev is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            InputEventMouseMotion mouseEvent = ev as InputEventMouseMotion;
            ghost.RotateY(-mouseEvent.Relative.X * MOUSE_SENSITIVITY);

            var angle = -mouseEvent.Relative.Y * MOUSE_SENSITIVITY;
            var rotation = head.Rotation;
            rotation.X = (float)Math.Min(Math.Max(rotation.X + angle, -Math.PI / 2), Math.PI / 2);
            head.Rotation = rotation;
        }
    }
}
