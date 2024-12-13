using Godot;
using System;

public partial class Switcher : Interact
{
	[Export]
	public AnimationPlayer Animator;

	private bool Pressed = false;

	public override void _Ready()
	{
		Triggered += Switch;
	}

	private void Switch()
	{
		if (Pressed)
			Animator.Play("toggled");
		else
			Animator.PlayBackwards("toggled");

		Pressed = !Pressed;
	}

}
