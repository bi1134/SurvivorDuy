using Godot;
using System;

public partial class boss_health_bar : ProgressBar
{
	Timer timer;
	ProgressBar damageBar;

	double health = 0;

	public override void _Ready()
	{
		timer = GetNodeOrNull<Timer>("Timer");
		damageBar = GetNodeOrNull<ProgressBar>("DamageBar");
	}
	public double _Health
	{
		get { return health; }
		set { SetHealth(value); }
	}

	private void SetHealth(double new_health)
	{
		var prevHealth = health;
		health = Math.Min(MaxValue, new_health);
		Value = health;

		if (health <= 0)
		{
			QueueFree();
		}
		if (health < prevHealth)
		{
			timer.Start();
		}
		else
		{
			damageBar.Value = health;
		}
	}
	public void _init_health(double _health)
	{
		health = _health;
		MaxValue = health;
		Value = health;
		damageBar.MaxValue = health;
		damageBar.Value = health;
	}
	private void _on_timer_timeout()
	{
		damageBar.Value = health;
	}
}


