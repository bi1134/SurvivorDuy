using Godot;
using System;

public partial class floating_text : Node2D
{
	Label label;
	public float damageNum;
	AnimationPlayer animPlayer;

	public override void _Ready()
	{
		label = GetNodeOrNull<Label>("Label");
		animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		label.Text = damageNum.ToString("0.0");
		animPlayer.Play("popup");
	}

	private void _on_animation_player_animation_finished(StringName anim_name)
	{
		QueueFree();
	}
	
}






