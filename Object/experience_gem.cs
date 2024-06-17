using Godot;
using System;

public partial class experience_gem : Area2D
{
	[Export] public int experience = 1;

	Texture2D spr_green = ResourceLoader.Load<Texture2D>("res://Textures/Items/Gems/Gem_green.png");
	Texture2D spr_blue = ResourceLoader.Load<Texture2D>("res://Textures/Items/Gems/Gem_blue.png");
	Texture2D spr_red = ResourceLoader.Load<Texture2D>("res://Textures/Items/Gems/Gem_red.png");

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

		if (experience < 5) //lowest color is green
		{
			return;
		}
		else if (experience < 25)
		{
			sprite.Texture = spr_blue;
		}
		else // if >= 25 then change to red
		{
			sprite.Texture = spr_red;
		}

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
		return experience;
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




