using Godot;

public partial class State : Node
{

	public virtual void PhysicsProcess(double delta) { }
	public virtual void HandleInput(InputEvent ev) { }
	public virtual void Enter() { }
	public virtual void Exit() { }
}
