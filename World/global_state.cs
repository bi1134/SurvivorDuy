using Godot;
using System;
using System.IO;

public partial class global_state : Node
{
	public static global_state Instance { get; private set; }

	public string CurrentLevel { get; set; }

	public override void _Ready()
	{
		
		if (Instance == null)
		{
			Instance = this;
			// prevent the singleton from being freed on scene changes
			this.SetProcess(false);
			GD.Print("GlobalState initialized.");
		}
		else
		{
			QueueFree();
		}
	}
	
}
