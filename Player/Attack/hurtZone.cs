using Godot;
using System.Collections.Generic;

public partial class hurtZone : Area2D
{
	[Export] public float damagePerSecond = 5;
	[Export] public float displayDistance = 300f;


	public int level = 1;

	player player;
	private Timer damageTimer;
	private List<enemy> enemiesInZone = new List<enemy>();
	CpuParticles2D cpuParticles2D;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as player;
		cpuParticles2D = GetNodeOrNull<CpuParticles2D>("CPUParticles2D");
		damageTimer = GetNodeOrNull<Timer>("damageTimer");
		damageTimer.Start();

	}

	public override void _Process(double delta)
	{
		GlobalPosition = player.GlobalPosition;
	}

	public void updateToxic()
	{
		level = player.toxicLevel;
		switch (level)
		{
			case 1:
				Scale = new Vector2(1,1);
				damagePerSecond = 2 * (1 + player.additionalDamage);
				damageTimer.WaitTime = 1.0f - player.spellCooldown;
				break;
			case 2:
				Scale = new Vector2(1.5f,1.5f);
				damagePerSecond = 2 * (1 + player.additionalDamage);
				damageTimer.WaitTime = 1.0f - player.spellCooldown;
				break;

			case 3:
				Scale = new Vector2(2f, 2f);
				damagePerSecond = 6 * (1 + player.additionalDamage);
				damageTimer.WaitTime = 1.0f - player.spellCooldown;
				break;

			case 4:
				Scale = new Vector2(2f, 2f);
				damagePerSecond = 10 * (1 + player.additionalDamage * 2);
				cpuParticles2D.Amount = 1000;
				cpuParticles2D.Color = new Color(0.47f, 0.03f, 0.31f);
				damageTimer.WaitTime = 1.0f - player.spellCooldown;
				break;
		}
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is enemy enemy)
		{
			if (!enemiesInZone.Contains(enemy))
			{
				enemiesInZone.Add(enemy);
			}
		}
	}

	private void _on_body_exited(Node2D body)
	{
		if (body is enemy enemy)
		{
			enemiesInZone.Remove(enemy);
		}
	
	}
	private void _on_damage_timer_timeout()
	{
		int maxSoundCount = (int)(enemiesInZone.Count * 0.25); 
		int soundCount = 0;
		foreach (var enemy in enemiesInZone)
		{
			if (enemy != null && enemy.IsInsideTree())
			{
				enemy.ApplyDamage(damagePerSecond);
				float distance = (enemy.GlobalPosition - GlobalPosition).Length();
				if (distance <= displayDistance)
				{
					enemy.snd_hit.VolumeDb = -20;
				}
				else
				{
					enemy.snd_hit.VolumeDb = -11;
				}

				if (soundCount < maxSoundCount)
				{
					enemy.snd_hit.Play();
					soundCount++;
				}
			}
		}
		
	}

   
}


