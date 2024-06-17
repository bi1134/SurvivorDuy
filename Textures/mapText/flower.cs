using Godot;
using System;

public partial class flower : Sprite2D
{
	AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("bloom");
	}
}
