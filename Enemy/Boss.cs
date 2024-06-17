using Godot;
using Godot.Collections;
using System;
using System.IO;

public partial class Boss : CharacterBody2D
{
	[Export] public float movement_speed = 20.0f;
	[Export] public double hp = 10;
	[Export] public float knockback_recovery = 3.5f;
	[Export] public int experience = 1;
	[Export] public int enemy_damage = 5;
	[Export] private PackedScene winGateScene;


	Timer iceSpearTimer;
	Timer iceSpearAttackTimer;

	int iceSpearAmmo = 0;
	int iceSpearBaseAmmo = 1;
	float iceSpearAttackSpeed = 1f;
	int iceSpearLevel = 1;
	int additionalAttack = 0;

	Vector2 knockback = Vector2.Zero;
	private player player;
	protected Sprite2D sprite;
	private Vector2 direction;
	private AnimationPlayer animation;
	private AnimationPlayer hitflashAnimationPlayer;
	AudioStreamPlayer2D snd_hit;
	protected Node2D loot_base;
	Area2D hitBox;

	boss_health_bar bossHealthBar;
	Panel healthPanel;
	Tween tween;
	Godot.Collections.Dictionary data;
	public bool bossDead = false;

	PackedScene iceSpear = ResourceLoader.Load<PackedScene>("res://Enemy/BossAttack.tscn");
	protected PackedScene deathAnim = ResourceLoader.Load<PackedScene>("res://Enemy/explosion.tscn");
	protected PackedScene expGem = ResourceLoader.Load<PackedScene>("res://Object/experience_gem.tscn");
	protected PackedScene floatingText = ResourceLoader.Load<PackedScene>("res://Enemy/floating_text.tscn");

	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);

	string mapName;

	// Angle for bullet hell pattern
	private float currentAngle = 0.0f;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		loot_base = GetTree().GetFirstNodeInGroup("loot") as Node2D;
		iceSpearTimer = GetNodeOrNull<Timer>("%IceSpearTimer");
		iceSpearAttackTimer = GetNodeOrNull<Timer>("%IceSpearAttackTimer");
		sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		snd_hit = GetNodeOrNull<AudioStreamPlayer2D>("snd_hit");
		animation = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		hitflashAnimationPlayer = GetNodeOrNull<AnimationPlayer>("HitFlashAnimationPlayer");
		hitBox = GetNodeOrNull<Area2D>("HitBox");
		hitBox.Set("damage", enemy_damage);
		animation.Play("walk");
		healthPanel = GetNodeOrNull<Panel>("CanvasLayer/HealthPanel");
		bossHealthBar = GetNodeOrNull<boss_health_bar>("CanvasLayer/HealthPanel/BossHealthBar");
		bossHealthBar._init_health(hp);
		mapName = GetTree().CurrentScene.Name;
		data = new Godot.Collections.Dictionary();

		
		attack();
	}

	private void attack()
	{
		if (iceSpearLevel > 0)
		{
			iceSpearTimer.WaitTime = iceSpearAttackSpeed;
			if (iceSpearTimer.IsStopped())
			{
				iceSpearTimer.Start();
			}
		}
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
		}
		else if (direction.X < -0.1)
		{
			sprite.FlipH = false;
		}
	}

	private void death()
	{
		bossDead = true;
		EmitSignal(SignalName.RemoveFromArray, this);
		var enemy_death = (Node2D)deathAnim.Instantiate();
		enemy_death.Scale = sprite.Scale;
		enemy_death.GlobalPosition = GlobalPosition;
		GetParent().CallDeferred("add_child", enemy_death);

		var new_gem = (experience_gem)expGem.Instantiate();
		new_gem.GlobalPosition = GlobalPosition;
		new_gem.experience = experience;
		loot_base.CallDeferred("add_child", new_gem);

		// Spawn win gate
		var winGate = (Node2D)winGateScene.Instantiate();
		winGate.GlobalPosition = GlobalPosition;
		GetParent().CallDeferred("add_child", winGate);

		bossHealthBar.QueueFree();
		QueueFree();

		if (LevelData.LevelDictionary.ContainsKey(mapName))
		{
			LevelData.LevelDictionary[mapName].beaten = true;
		}

		foreach (var entry in LevelData.LevelDictionary)
		{
			data[entry.Key] = entry.Value.beaten;
		}

		string json = Json.Stringify(data);
		string path = ProjectSettings.GlobalizePath("user://");
		SavetoFile(path, "saveGame1.json", json);

		LevelData.UpdateMapStatus();
	}

	protected void _on_hurt_box_hurt(float damage, Vector2 angle, int knockback_amount)
	{
		hp -= damage;
		hitflashAnimationPlayer.Play("hitflash");
		CreateFloatingText(damage);
		knockback = angle * knockback_amount;
		if (hp <= 0)
		{
			death();
		}
		else
		{
			snd_hit.Play();
		}
		bossHealthBar._Health = hp;
	}
	private void CreateFloatingText(float damage)
	{
		var floatingTxt = (floating_text)floatingText.Instantiate();
		floatingTxt.damageNum = damage;
		floatingTxt.GlobalPosition = GlobalPosition;
		GetParent().AddChild(floatingTxt);
	}


	public void showHealthBar()
	{
		tween = healthPanel.CreateTween();
		healthPanel.Visible = true;
		tween.TweenProperty(healthPanel, "scale", new Vector2(1, 1), 1.5).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	public void hideHealthBar()
	{
		tween = healthPanel.CreateTween();
		tween.TweenProperty(healthPanel, "scale", new Vector2(0.5f, 0.5f), 1.5).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
		tween.Play();
		healthPanel.Visible = false;
	}

	private void _on_health_show_body_entered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			showHealthBar();
		}
	}

	private void _on_health_show_body_exited(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			hideHealthBar();
		}
	}

	private void _on_ice_spear_timer_timeout()
	{
		iceSpearAmmo += iceSpearBaseAmmo + additionalAttack;
		iceSpearAttackTimer.Start();
	}

	private void _on_ice_spear_attack_timer_timeout()
	{
		if (iceSpearAmmo > 0)
		{
			FireBulletHellPattern();
			iceSpearAmmo -= 1;
			if (iceSpearAmmo > 0)
			{
				iceSpearAttackTimer.Start();
			}
			else
			{
				iceSpearAttackTimer.Stop();
			}
		}
	}

	private void FireBulletHellPattern()
	{
		const int numBullets = 8; // number of bullets in the spread
		const float angleIncrement = Mathf.Pi / 16 + 20 ; // angle increment for each bullet

		for (int i = 0; i < numBullets; i++)
		{
			float angle = currentAngle + i * angleIncrement ;
			Vector2 bulletDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			Vector2 targetPosition = GlobalPosition + bulletDirection * 1000.0f; // Target far away

			var iceSpearAttack = (BossAttack)iceSpear.Instantiate();
			iceSpearAttack.Position = GlobalPosition;
			iceSpearAttack.target = targetPosition;
			iceSpearAttack.angle = bulletDirection; // set the direction as a Vector2
			AddChild(iceSpearAttack);
		}

		currentAngle += angleIncrement + 6.5f; //change direction faster
	}

	private void SavetoFile(string path, string fileName, string data)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}

		path = Path.Join(path, fileName);
		try
		{
			File.WriteAllText(path, data);
		}
		catch (System.Exception e) 
		{
			GD.Print(e);
		}

	}

}




