using Godot;
using System;

public partial class slash : Area2D
{
	public int level = 1;
	public int hp = 99;
	public int speed = 150;
	public float damage = 4;
	public int knockback_amount = 50;
	public float attack_size = 1.0f;


	public Vector2 target = Vector2.Zero;
	public Vector2 angle = Vector2.Zero;
	player player;
	[Signal] public delegate void RemoveFromArrayEventHandler(Node2D node);


	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		angle = GlobalPosition.DirectionTo(target);
		Rotation = angle.Angle() + (float)Math.PI * 1 / 180;
		damage = 4 * (1 + player.additionalDamage);

		var tween = CreateTween();
		tween.TweenProperty(this, "scale", new Vector2(2, 2) * attack_size, 0.1).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
		tween.Play();
	}

	public override void _PhysicsProcess(double delta)
	{

		GlobalPosition += angle.Normalized() * (float)(speed * delta);
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
