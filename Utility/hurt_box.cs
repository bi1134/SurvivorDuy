using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class hurt_box : Area2D
{
	private CollisionShape2D collision;
	private Timer disableTimer;
	[Signal] public delegate void HurtEventHandler(float damage, Vector2 angle, int knockback);
	[Export] public HitBoxType hitBoxType = HitBoxType.Cooldown;

	List<Object> hitOnceArray = new List<Object>();

	public enum HitBoxType
	{
		Cooldown,
		HitOnce,
		DisableHitBox
	}

	public override void _Ready()
	{
		collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		disableTimer = GetNodeOrNull<Timer>("DisableTimer");
	}
	private void _on_area_entered(Area2D area)
	{
		if (area.IsInGroup("attack"))
		{
			var areaDamage = area.Get("damage");
			if (!areaDamage.Equals(null))
			{
				switch (hitBoxType)
				{
					case HitBoxType.Cooldown:
						collision.CallDeferred("set", "disabled", true);//disable hurtbox collision
						disableTimer.Start(); //start timer (0.5)
						break;
					case HitBoxType.HitOnce: // HitOnce
						if (!hitOnceArray.Contains(area))
						{
							hitOnceArray.Add(area);
							if (area.HasSignal("RemoveFromArray"))
							{
								if(!area.IsConnected("RemoveFromArray", new Callable(this, "RemoveFromList")))
								{
									area.Connect("RemoveFromArray", new Callable(this, "RemoveFromList"));
								}
							}
						}
						else
						{
							return;
						}
						break;
					case HitBoxType.DisableHitBox: // DisableHitbox
						//check if the area have method
						if (area.HasMethod("tempDisable"))
						{
							area.Call("tempDisable");
						}
						break;
					default:
						break;
				}

				var angle = Vector2.Zero;
				var knockback = 1;
				if(!area.Get("angle").Equals(null))
				{
					angle = (Vector2)area.Get("angle");
				}
				if(!area.Get("knockback_amount").Equals(null))
				{
					knockback = (int)area.Get("knockback_amount");
				}
				EmitSignal(SignalName.Hurt, areaDamage, angle, knockback);
				if (area.HasMethod("EnemyHits"))
				{
					area.Call("EnemyHits", 1);
				}
			}
		}
	}

	private void RemoveFromList(object obj)
	{
		if(hitOnceArray.Contains(obj))
		{
			hitOnceArray.Remove(obj);
		}
	}
	private void _on_disable_timer_timeout()
	{
		collision.CallDeferred("set", "disabled", false);
	}

}


