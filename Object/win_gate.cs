using Godot;
using System;

public partial class win_gate : Sprite2D
{
	private Sprite2D doorSprite;
	private player player;


	public override void _Ready()
	{
		doorSprite = GetNodeOrNull<Sprite2D>($".");
		player = GetTree().GetFirstNodeInGroup("player") as player;
	}

	public override void _Process(double delta)
	{
	}

	private void _on_open_detection_body_entered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			doorSprite.Frame = 1;
		}
	}


	private void _on_open_detection_body_exited(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			doorSprite.Frame = 0;
		}
	}

	private void _on_enter_change_scene_body_entered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			player.showWinPanel();
		}
	}
	
	
}








