using Godot;
using System;

public partial class explosion : Sprite2D
{
	AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//play the animation when it spawn
		animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play("explode");
	}
	private void _on_animation_player_animation_finished(StringName anim_name)
	{
		//delete it self when it done
		QueueFree();
	}
}


