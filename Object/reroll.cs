using Godot;
using System;

public partial class reroll : Area2D
{
	[Export] public int rerolls = 1;

	
	public Node2D target = null;

	//start with negative to move to opposite direction first
	//(create a bounce then going toward the player)
	float speed = -1; 

	Sprite2D sprite;
	CollisionShape2D collision;
	AudioStreamPlayer2D sound;

	public override void _Ready()
	{
		sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		sound = GetNodeOrNull<AudioStreamPlayer2D>("snd_collected");


	}


	public override void _PhysicsProcess(double delta)
	{
		if (target != null)
		{
			//change a Vector2 closer to another Vector2 (this is not a physics func)
			GlobalPosition = GlobalPosition.MoveToward(target.GlobalPosition, speed);
			//increase speed to move toward the player
			speed += 2 * (float)delta;
		}
	}


	public int Collect()
	{
		sound.Play();
		collision.CallDeferred("set", "disabled", true);
		sprite.Visible = false; //make the gem dissapear (fake delete) so the sound still happened instead of getting cut out
		return rerolls;
	}
	private void _on_snd_collected_finished()
	{
		//actual delete
		QueueFree();
	}
	private void _on_despawn_time_timeout()
	{
		if (sprite.GlobalPosition == Position)
		{
		QueueFree();
		}
	}
}
