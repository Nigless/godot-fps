using System.Diagnostics;
using Godot;

public partial class StateMachine : Node
{
	[Export] public State initialState;
	private State _currentState;

	public override void _Ready()
	{
		_currentState = initialState;
		initialState.Enter();
	}

	public void SwitchTo(State state)
	{

		Trace.WriteLine(state.Name);
		_currentState.Exit();
		state.Enter();
		_currentState = state;
	}

	public override void _PhysicsProcess(double delta)
	{
		_currentState.PhysicsProcess(delta);
	}

	public override void _UnhandledInput(InputEvent ev)
	{
		_currentState.HandleInput(ev);
	}
}
