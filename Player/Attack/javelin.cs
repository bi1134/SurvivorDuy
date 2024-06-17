using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class javelin : Area2D
{
	int level = 1;
	int hp = 9999;
	float speed = 200.0f;
	float damage = 10;
	int knockback_amount = 100;
	int path = 1;
	float attack_size = 1.0f;
	float attack_speed = 4.0f;

	Vector2 target = Vector2.Zero;
	List<Vector2> target_array = new List<Vector2>();

	Vector2 angle = Vector2.Zero;
	Vector2 reset_pos = Vector2.Zero;

	Texture2D spr_javelin_reg = ResourceLoader.Load<Texture2D>("res://Textures/Items/Weapons/Stick.png");
	Texture2D spr_javelin_atk = ResourceLoader.Load<Texture2D>("res://Textures/Items/Weapons/StickAttack.png");

	player player;
	Sprite2D sprite;
	CollisionShape2D collision;
	Timer attackTimer;
	Timer changeDirectionTimer;
	Timer ResetPosTimer;
	AudioStreamPlayer2D snd_attack;

	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		attackTimer = GetNodeOrNull<Timer>("%AttackTimer");
		changeDirectionTimer = GetNodeOrNull<Timer>("%ChangeDirection");
		ResetPosTimer = GetNodeOrNull<Timer>("%ResetPosTimer");
		snd_attack = GetNodeOrNull<AudioStreamPlayer2D>("snd_attack");
		updateJavelin();
		_on_reset_pos_timer_timeout();
	}

	public void updateJavelin()
	{
		level = player.javelinLevel;
		switch (level)
		{
			case 1:
				hp = 9999;
				speed = 300f;
				damage = 10 * (1 + player.additionalDamage);
				knockback_amount = 100;
				path = 1;
				attack_size = 1.0f * (1 + player.spellSize);
				attack_speed = 4.0f * (1 - player.spellCooldown);
				break;
			case 2:
				hp = 9999;
				speed = 300f;
				damage = 10 * (1 + player.additionalDamage);
				knockback_amount = 100;
				path = 2;
				attack_size = 1.0f * (1 + player.spellSize);
				attack_speed = 4.0f * (1 - player.spellCooldown);
				break;

			case 3:
				hp = 9999;
				speed = 300f;
				damage = 10 * (1 + player.additionalDamage);
				knockback_amount = 100;
				path = 3;
				attack_size = 1.0f * (1 + player.spellSize);
				attack_speed = 4.0f * (1 - player.spellCooldown);
				break;

			case 4:
				hp = 9999;
				speed = 300f;
				damage = 15 * (1 + player.additionalDamage);
				knockback_amount = 120;
				path = 3;
				attack_size = 1.0f * (1 + player.spellSize);
				attack_speed = 4.0f * (1 - player.spellCooldown);
				break;
		
		}
		Scale = new Vector2(1.0f, 1.0f) * attack_size;
		attackTimer.WaitTime = attack_speed;

	}

	public override void _PhysicsProcess(double delta)
	{
		//need a target to move towards the enemy
		if (target_array.Count > 0)
		{
			Position += angle.Normalized() * (float)(speed * delta);
		}
		else
		{
			var player_angle = GlobalPosition.DirectionTo(reset_pos);
			var distance_dif = GlobalPosition - player.GlobalPosition;
			var return_speed = 20;
			if (Math.Abs(distance_dif.X) > 500 || Math.Abs(distance_dif.Y) > 500)
			{
				return_speed = 100;
			}
			Position += player_angle * return_speed * (float)delta;
			Rotation = GlobalPosition.DirectionTo(player.GlobalPosition).Angle() + (float)Math.PI * 135 / 180;
		}
	}

	public void addPaths()
	{
		//play sound
		snd_attack.Play();
		//remove from hit once hitbox
		EmitSignal(SignalName.RemoveFromArray, this);
		//clear array for new target
		target_array.Clear();
		var counter = 0;
		//get value for each value in path
		while (counter < path)
		{
			//target gotten from the player script
			var new_path = player.getRandomTarget();
			//add the to the array of the targets
			target_array.Add(new_path);
			//close the loop by incease the value
			counter += 1;
			
		}
		enableAttack(true);
		if (target_array.Count > 0)
		{
			target = target_array[0];
			processPath();
		}
	}

	public void processPath()
	{
		angle = GlobalPosition.DirectionTo(target);
		changeDirectionTimer.Start();
		var tween = CreateTween();
		var new_rotation_degrees = angle.Angle() + Math.PI * 135 / 180;
		tween.TweenProperty(this, "rotation", new_rotation_degrees, 0.25).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	public void enableAttack(bool atk = true)
	{
		if (atk)
		{
			//active coll if argument is true
			collision.CallDeferred("set", "disabled", false);
			sprite.Texture = spr_javelin_atk;
		}
		else
		{
			//deactive of not
			collision.CallDeferred("set", "disabled", true);
			sprite.Texture = spr_javelin_reg;
		}
	}
	private void _on_attack_timer_timeout()
	{
		//call add path
		addPaths();
	}
	private void _on_change_direction_timeout()
	{
		if (target_array.Count > 0)
		{
			target_array.RemoveAt(0);
			if (target_array.Count > 0)
			{
				target = target_array[0];
				processPath();
				snd_attack.Play();
				EmitSignal(SignalName.RemoveFromArray, this);
			}
			else
			{
				changeDirectionTimer.Stop();
				attackTimer.Start();
				enableAttack(false);
			}
		}
		else
		{
			changeDirectionTimer.Stop();
			attackTimer.Start();
			enableAttack(false);
		}
	}
	private void _on_reset_pos_timer_timeout()
	{
		//chose a number between 0 and 3
		var chose_direction = GD.Randi() % 4;
		//set reset_pos to player position
		reset_pos = player.GlobalPosition;
		//change reset_pos by 50 pixels
		switch(chose_direction)
		{
			case 0:
				reset_pos.X += 50;
				break;
			case 1:
				reset_pos.X -= 50;
				break; 
			case 2:
				reset_pos.Y += 50;
				break; 
			case 3:
				reset_pos.Y -= 50;
				break;

		}
	}
}





