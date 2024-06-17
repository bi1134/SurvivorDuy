using Godot;
using System;

public partial class hit_box : Area2D
{
	[Export] public int damage = 1;
	private CollisionShape2D collision;
	private Timer disableTimer;

	public override void _Ready()
	{
		collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		disableTimer = GetNodeOrNull<Timer>("DisableHitboxTimer");
	}

	public void tempDisable()
	{
		collision.CallDeferred("set", "disabled", true); //disable hit box collision
		disableTimer.Start();
	}
	private void _on_disable_hitbox_timer_timeout()
	{
		collision.CallDeferred("set", "disabled", false); //enable hitbox collision
	}
}


