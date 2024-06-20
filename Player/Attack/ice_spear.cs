using Godot;
using System;

public partial class ice_spear : Area2D
{
	public int level = 1;
	public int hp = 1;
	public int speed = 100;
	public float damage = 5;
	public int knockback_amount = 100;
	public float attack_size = 1.0f;
	public float damageReductionRate = 0.3f; //higher = faster damage got reducted
	private float timeElapsed = 0;


	public Vector2 target = Vector2.Zero;
	public Vector2 angle = Vector2.Zero;
	player player;
	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);


	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		angle = GlobalPosition.DirectionTo(target);
		Rotation = angle.Angle() + (float)Math.PI * 135 / 180;

		switch (level)
		{
			case 1:
				hp = 1;
				speed = 300;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 2:
				hp = 1;
				speed = 300;
				damage = 5 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 3:
				hp = 2;
				speed = 300;
				damage = 8 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
			case 4:
				hp = 2;
				speed = 300;
				damage = 8 * (1 + player.additionalDamage);
				knockback_amount = 100;
				attack_size = 1.0f * (1 + player.spellSize);
				break;
		}

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", new Vector2(1, 1) * attack_size, 1).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += angle.Normalized() * (float)(speed * delta);

		timeElapsed += (float)delta;

		damage = Math.Max(1, damage - timeElapsed * damageReductionRate);
	}

	public void EnemyHits(int charge = 1)
	{
		hp -= charge;
		if (hp <= 0)
		{
			EmitSignal(SignalName.RemoveFromArray, this);
			QueueFree();
		}
	}
	private void _on_timer_timeout()
	{
		EmitSignal(SignalName.RemoveFromArray, this);
		QueueFree();
	}
}


