using Godot;
using System;

public partial class frame_saver : Node
{
	Timer timer;

	public override void _Ready()
	{
		timer = GetNodeOrNull<Timer>("Timer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (timer.IsStopped())
		{
			if (Engine.GetFramesPerSecond() < 55)
			{
				disableStuff(20);
			}
			else if (Engine.GetFramesPerSecond() < 70)
			{
				disableStuff(10);
			}
			else if (Engine.GetFramesPerSecond() < 85)
			{
				disableStuff(50);
			}
		}
	}

	public void disableStuff(int args = 20)
	{
		GetTree().CallGroup("enemy", "frameSave", args);
		timer.Start();
	}
}
