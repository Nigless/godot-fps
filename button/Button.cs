using Godot;
using System;

public partial class Button : Interact
{
	[Export]
	public AnimationPlayer Animator;

	private bool Pressed = false;

	public override void _Ready()
	{
		Triggered += Press;

		Animator.AnimationFinished += Release;
	}

	private void Press()
	{
		if (Pressed)
			return;

		Animator.Play("pressed");

		Pressed = true;
	}

	private void Release(StringName _)
	{
		if (!Pressed)
			return;

		Animator.PlayBackwards("pressed");

		Pressed = false;
	}


}
