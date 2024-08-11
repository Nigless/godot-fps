using System;
using System.Collections.Generic;
using Godot;

public class StateMachine
{
    private State _CurrentState;

    private readonly Dictionary<Type, State> _States = new();

    public StateMachine(State state)
    {
        _CurrentState = state;

        WithState(state);
    }

    public StateMachine WithState(State state)
    {
        _States.Add(state.GetType(), state);

        return this;
    }

    public State Get<T>()
    {
        return _States[typeof(T)];
    }

    public void Update(double delta)
    {
        UpdateState();
        _CurrentState.Update(delta);
    }

    public void Input(InputEvent ev)
    {
        _CurrentState.Input(ev);
    }

    private void UpdateState()
    {
        var state = _CurrentState.UpdateState();

        if (state == _CurrentState)
        {
            return;
        }

        _CurrentState.Exit();
        state.Enter();
        _CurrentState = state;
    }


}