using Godot;

public abstract class State
{
    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }

    public virtual void Input(InputEvent ev)
    {

    }

    public virtual void Update(double delta)
    {

    }


    public virtual State UpdateState()
    {
        return this;
    }
}
