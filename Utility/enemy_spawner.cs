using Godot;
using System;
using System.Collections.Generic;

public partial class enemy_spawner : Node2D
{
	[Export] Godot.Collections.Array<spawn_info> spawns = new Godot.Collections.Array<spawn_info>();
	public Node2D player;
	[Export] public int time = 0;

	[Signal] public delegate void changeTimeEventHandler(int time);
	//create a random number
	Random random = new Random();
	int enemy_cap = 500;
	List<PackedScene> enemies_to_spawn = new List<PackedScene>();

	PackedScene weakEnemyKobold = ResourceLoader.Load<PackedScene>("res://Enemy/enemy_kobold_weak.tscn");
	PackedScene strongEnemyKobold = ResourceLoader.Load<PackedScene>("res://Enemy/enemy_kobold_strong.tscn");
	PackedScene weakEnemyGoblin = ResourceLoader.Load<PackedScene>("res://Enemy/enemy_goblin_weak.tscn");
	PackedScene strongEnemyGoblin = ResourceLoader.Load<PackedScene>("res://Enemy/enemy_goblin_strong.tscn");
	PackedScene enemyCyclop = ResourceLoader.Load<PackedScene>("res://Enemy/enemy_cyclop.tscn");

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		Connect(SignalName.changeTime, new Callable(player, "ChangeTime"));
	}

	public void spawnDefault()
	{
		var spawnEnemy1 = (Node2D)weakEnemyKobold.Instantiate();
		var spawnEnemy2 = (Node2D)weakEnemyGoblin.Instantiate();
		var spawnEnemy3 = (Node2D)strongEnemyKobold.Instantiate();
		var spawnEnemy4 = (Node2D)strongEnemyGoblin.Instantiate();
		var spawnEnemy5 = (Node2D)enemyCyclop.Instantiate();
		spawnEnemy1.GlobalPosition = GetRandomPosition();
		spawnEnemy2.GlobalPosition = GetRandomPosition();
		spawnEnemy3.GlobalPosition = GetRandomPosition();
		spawnEnemy4.GlobalPosition = GetRandomPosition();
		spawnEnemy5.GlobalPosition = GetRandomPosition();
		AddChild(spawnEnemy1);
		AddChild(spawnEnemy2);
		if (time >= 350)
		{
			AddChild(spawnEnemy3);
			AddChild(spawnEnemy4);
		}
		if (time >= 400)
		{
			AddChild(spawnEnemy5);
		}

	}
	
	private void _on_timer_timeout()
	{
		time += 1;
		var enemy_spawns = spawns;
		var my_children = GetChildren();
		//loop back to the next spawn_info obj
		foreach (var i in enemy_spawns)
		{
			//if in between, the script continue
			if (time >= i.time_start && time <= i.time_end)
			{
				//check if there is a delay
				if (i.spawn_delay_counter < i.enemy_spawn_delay)
				{
					//increase counter by 1 for next time
					i.spawn_delay_counter++;
				}
				else
				{
					//if not then reset counter to zero and load the enemy resource
					i.spawn_delay_counter = 0;
					var new_enemy = i.enemy as PackedScene;
					var counter = 0;
					//spawn in the number of enemy
					while (counter < i.enemy_num)
					{
						if(my_children.Count <= enemy_cap)
						{
						//create instance of packed scene
						var enemy_spawn = (Node2D)new_enemy.Instantiate();
						//get and set the random position
						enemy_spawn.GlobalPosition = GetRandomPosition();
						//add enemy into the world using the random position we had
						AddChild(enemy_spawn);
						}
						else
						{
							enemies_to_spawn.Add(new_enemy);
						}
						counter++; //increase counter until all the enemy is spawned
					}

				}
			}
			if(time >= 300 )
			{
				if (my_children.Count <= 50)
				{
					spawnDefault();
				}
			}
		}
		if (my_children.Count <= enemy_cap && enemies_to_spawn.Count > 0)
		{
			var spawn_number = Math.Clamp(enemies_to_spawn.Count, 1, 50) - 1;
			var counter = 0;

			while(counter <spawn_number)
			{
				var new_enemy = (Node2D)enemies_to_spawn[0].Instantiate();
				new_enemy.GlobalPosition = GetRandomPosition();
				AddChild(new_enemy);
				enemies_to_spawn.RemoveAt(0);
				counter += 1;
			}
		}
		EmitSignal(SignalName.changeTime, time);
	}

	public Vector2 GetRandomPosition()
	{
		//get the size of our view port(the camera rect), and then multiply in the range of 1,1 and 1,4
		var vpr = GetViewportRect().Size * (float)GD.RandRange(1.1,1.4); 
		var top_left = new Vector2(player.GlobalPosition.X - vpr.X/2, player.GlobalPosition.Y - vpr.Y/2); 
		var top_right = new Vector2(player.GlobalPosition.X + vpr.X/2, player.GlobalPosition.Y - vpr.Y/2); 
		var bottom_left = new Vector2(player.GlobalPosition.X - vpr.X/2, player.GlobalPosition.Y + vpr.Y/2); 
		var bottom_right = new Vector2(player.GlobalPosition.X + vpr.X/2, player.GlobalPosition.Y + vpr.Y/2);
		string[] pos_side = { "up", "down", "left", "right" };
		
		//pick a random number from 0 to string array length
		int randomIndex = random.Next(pos_side.Length);
		//use that random number to call value from array
		string randomElement = pos_side[randomIndex];
		var spawn_pos1 = Vector2.Zero;
		var spawn_pos2 = Vector2.Zero;

		switch (randomElement) //pick a side
		{
			case "up":
				spawn_pos1 = top_left;
				spawn_pos2 = top_right;
				break;
			case "down":
				spawn_pos1 = bottom_left;
				spawn_pos2 = bottom_right;
				break;
			case "left":
				spawn_pos1 = top_left;
				spawn_pos2 = bottom_left;
				break;
			case "right":
				spawn_pos1 = top_right;
				spawn_pos2 = bottom_right;
				break;
		}
		
		//get value between the points
		var x_spawn = (float)GD.RandRange(spawn_pos1.X, spawn_pos2.X);
		var y_spawn = (float)GD.RandRange(spawn_pos1.Y, spawn_pos2.Y);

		return new Vector2(x_spawn, y_spawn);
	}
}


