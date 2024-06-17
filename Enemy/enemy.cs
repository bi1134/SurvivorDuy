using Godot;
using System;

public partial class enemy : CharacterBody2D
{
	[Export] public float movement_speed = 20.0f;
	[Export] public double hp = 10;
	[Export] public float knockback_recovery = 3.5f;
	[Export] public int experience = 1;
	[Export] public int reroll = 1;
	[Export] public int enemy_damage = 5;


	//performance
	Vector2 screenSize;

	Vector2 knockback = Vector2.Zero;
	private player player;
	protected Sprite2D sprite;
	protected Sprite2D shadow;
	private Vector2 direction;
	private AnimationPlayer animation;
	private AnimationPlayer hitflashAnimationPlayer;
	public AudioStreamPlayer2D snd_hit;
	protected Node2D loot_base;
	protected Node2D reroll_base;
	Area2D hitBox;
	CollisionShape2D collisionShape;

	protected PackedScene deathAnim = ResourceLoader.Load<PackedScene>("res://Enemy/explosion.tscn");
	protected PackedScene expGem = ResourceLoader.Load<PackedScene>("res://Object/experience_gem.tscn");
	protected PackedScene rerollGem = ResourceLoader.Load<PackedScene>("res://Object/reroll.tscn");
	protected PackedScene floatingText = ResourceLoader.Load<PackedScene>("res://Enemy/floating_text.tscn");
	protected PackedScene expMagnet = ResourceLoader.Load<PackedScene>("res://Object/ExpMagnet.tscn");


	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		loot_base = GetTree().GetFirstNodeInGroup("loot") as Node2D;
		sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		shadow = GetNodeOrNull<Sprite2D>("Shadow");
		snd_hit = GetNodeOrNull<AudioStreamPlayer2D>("snd_hit");
		animation = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		hitflashAnimationPlayer = GetNodeOrNull<AnimationPlayer>("HitFlashAnimationPlayer");
		hitBox = GetNodeOrNull<Area2D>("HitBox");

		screenSize = GetViewportRect().Size;
		collisionShape = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		hitBox.Set("damage", enemy_damage);
		animation.Play("walk");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player != null)
		{
			knockback = knockback.MoveToward(Vector2.Zero, knockback_recovery);
			direction = GlobalPosition.DirectionTo(player.GlobalPosition);
			Velocity = direction * movement_speed;
			Velocity += knockback;
		}
		MoveAndSlide();

		if (direction.X > 0.1)
		{
			sprite.FlipH = true;
			shadow.FlipH = true;
		}
		else if (direction.X < -0.1)
		{
			sprite.FlipH = false;
			shadow.FlipH = false;
		}
	}

	private void death()
	{
		EmitSignal(SignalName.RemoveFromArray, this);
		var enemy_death = (Node2D)deathAnim.Instantiate();
		//set explosion scale to the size of the enemy sprite
		enemy_death.Scale = sprite.Scale;
		//set the global position to the enemy position
		enemy_death.GlobalPosition = GlobalPosition;
		GetParent().CallDeferred("add_child", enemy_death);
		//create exp gems
		var new_gem = (experience_gem)expGem.Instantiate();
		new_gem.GlobalPosition = GlobalPosition;
		new_gem.experience = experience;

		var randomNum = GD.Randi();
		if (randomNum % 80 == 1)
		{
			var new_reroll = (reroll)rerollGem.Instantiate();
			new_reroll.GlobalPosition = GlobalPosition + new Vector2(GD.Randi() % 20, GD.Randi() % 20);
			new_reroll.rerolls = reroll;
			loot_base.CallDeferred("add_child", new_reroll);
		}
		if (randomNum % 100 == 1)
		{
			var new_magnet = (magnet)expMagnet.Instantiate();
			new_magnet.GlobalPosition = GlobalPosition + new Vector2(GD.Randi() % 20, GD.Randi() % 20);
			loot_base.CallDeferred("add_child", new_magnet);
		}
		loot_base.CallDeferred("add_child", new_gem);
		//delete enemy from the game
		QueueFree();
	}
	protected void _on_hurt_box_hurt(float damage, Vector2 angle, int knockback_amount)
	{
		hp -= damage;
		knockback = angle * knockback_amount;
		hitflashAnimationPlayer.Play("hitflash");
		CreateFloatingText(damage);
		//check if the hp is <= 0
		if (hp <= 0)
		{
			//call the death func
			death();
		}
		else
		{
			//if not hp 0 or below, play the hit sound fx
			snd_hit.Play();
		}
	}
	

	public void ApplyDamage(float damage)
	{
		hp -= damage;
		hitflashAnimationPlayer.Play("hitflash");
		CreateFloatingText(damage);
		if (hp <= 0)
		{
			death();
		}
	}

	private void CreateFloatingText(float damage)
	{
		var floatingTxt = (floating_text)floatingText.Instantiate();
		floatingTxt.damageNum = damage;
		floatingTxt.GlobalPosition = GlobalPosition;
		GetParent().AddChild(floatingTxt);
	}

	private void frameSave(int amount = 20)
	{
		var randDisable = GD.Randi() % 100;
		if (randDisable < amount)
		{
			collisionShape.CallDeferred("set", "disable", true);
			animation.Stop();
		}
	}

	private void _on_hide_timer_timeout()
	{
		var locationDif = GlobalPosition - player.GlobalPosition;
		if (Math.Abs(locationDif.X) > (screenSize.X / 2) * 1.4 || Math.Abs(locationDif.Y) > (screenSize.Y / 2) * 1.4)
		{
			Visible = false;
		}
		else
		{
			Visible = true;
		}
	}
}


