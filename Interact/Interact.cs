using Godot;
using System;
using System.Diagnostics;

public partial class Interact : Node
{
	public event Action Triggered;
	public event Action HoverEntered;
	public event Action HoverExited;

	private bool Hovered = false;

	public void Trigger()
	{
		Triggered?.Invoke();
	}

	public void HoverEnter()
	{
		if (Hovered)
			return;

		HoverEntered?.Invoke();

		Hovered = true;
	}

	public void HoverExit()
	{
		if (!Hovered)
			return;

		HoverExited?.Invoke();
		Hovered = false;
	}
}
