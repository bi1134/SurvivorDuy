using Godot;
using System;

[GlobalClass]
public partial class spawn_info : Resource
{
	[Export] public int time_start;
	[Export] public int time_end;
	[Export] public Resource enemy;
	[Export] public int enemy_num;
	[Export] public int enemy_spawn_delay;

	public int spawn_delay_counter = 0;
}
