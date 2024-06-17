using Godot;
using System;

public partial class tornado : Area2D
{
	public int level = 1;
	public int hp = 9999;
	public float speed = 100.0f;
	public float damage = 5;
	float attack_size = 1.0f;
	public int knockback_amount = 100;

	public Vector2 last_movement = Vector2.Zero;
	Vector2 angle = Vector2.Zero;
	Vector2 angle_less = Vector2.Zero;
	Vector2 angle_more = Vector2.Zero;

	player player;

	AnimationPlayer animation;

	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);

	public override void _Ready()
	{
		animation = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		animation.Play("spin");
		player = GetTree().GetFirstNodeInGroup("player") as player;
		switch(level)
		{
			case 1:
				hp = 9999;
				speed = 100.0f;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 2:
				hp = 9999;
				speed = 100.0f;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 3:
				hp = 9999;
				speed = 100.0f;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 4:
				hp = 9999;
				speed = 100.0f;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 125;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
		}
		var move_to_less = Vector2.Zero;
		var move_to_more = Vector2.Zero;

	   if (last_movement == Vector2.Up || last_movement == Vector2.Down)
		{
			move_to_less = GlobalPosition + new Vector2((float)GD.RandRange(-1, -0.25f), last_movement.Y) * 500;
			move_to_more = GlobalPosition + new Vector2((float)GD.RandRange(0.25f, 1), last_movement.Y)*500;
		}
	   else if (last_movement == Vector2.Right || last_movement == Vector2.Left)
		{
			move_to_less = GlobalPosition + new Vector2(last_movement.X, (float)GD.RandRange(-1, -0.25f)) * 500;
			move_to_more = GlobalPosition + new Vector2(last_movement.X, (float)GD.RandRange(0.25f, 1)) * 500;
		}
	   else if (last_movement == new Vector2(1,1) || last_movement == new Vector2(-1,-1) || last_movement == new Vector2(1,-1) || last_movement == new Vector2(-1,1))
		{
			move_to_less = GlobalPosition + new Vector2(last_movement.X, last_movement.Y * (float)GD.RandRange(0, 0.75f)) * 500;
			move_to_more = GlobalPosition + new Vector2(last_movement.X * (float)GD.RandRange(0, 0.75f), last_movement.Y) * 500;
		}

		angle_less = GlobalPosition.DirectionTo(move_to_less);
		angle_more = GlobalPosition.DirectionTo(move_to_more);

		var initial_tween = CreateTween().SetParallel(true);
		initial_tween.TweenProperty(this, "scale", new Vector2(1, 1) * attack_size, 3).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		var final_speed = speed;
		speed = speed / 5.0f;
		initial_tween.TweenProperty(this, "speed", final_speed, 6).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		initial_tween.Play();


		var tween = CreateTween();
		var set_angle = GD.RandRange(0, 1);
		if (set_angle == 1)
		{
			angle = angle_less;
			for (int i = 0; i < 6; i++)
			{
				if (i % 2 == 0)
				{
					tween.TweenProperty(this, "angle", angle_more, 2);
				}
				else
				{
					tween.TweenProperty(this, "angle", angle_less, 2);
				}
			}
		}
		else
		{
			angle = angle_more;
			for (int i = 0; i < 6; i++)
			{
				if (i % 2 == 0)
				{
					tween.TweenProperty(this, "angle", angle_less, 2);
				}
				else
				{
					tween.TweenProperty(this, "angle", angle_more, 2);
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += angle.Normalized() * (float)(speed * delta);
	}
	private void _on_timer_timeout()
	{
		EmitSignal(SignalName.RemoveFromArray);
		QueueFree();
	}
}


