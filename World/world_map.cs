using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dictionary = Godot.Collections.Dictionary;

public partial class world_map : Node2D
{
	string menu = "res://TitleScreen/menu.tscn";
	Node2D levelHolder;
	Sprite2D playerSprite;
	Panel playerPanel;
	Node2D[] Levels;
	WorldMapLevel currentLevel;
	float lerp_speed = 0.5f;
	float lerp_progress = 0.0f;
	bool movement = true;
	float lerp_threshold = 0.1f;
	Node2D targetLevel;
	Timer levelTimer;
	AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		levelHolder = GetNode<Node2D>("LevelHolder");
		playerSprite = GetNode<Sprite2D>("playerPanel/playerSprite");
		playerPanel = GetNode<Panel>("playerPanel");
		currentLevel = GetNode<WorldMapLevel>("LevelHolder/Map1");
		Levels = levelHolder.GetChildren().Cast<Node2D>().ToArray();
		levelTimer = GetNodeOrNull<Timer>("%lvl_timer");
		animationPlayer = GetNodeOrNull<AnimationPlayer>("Sprite2D/AnimationPlayer");
		animationPlayer.Play("mapAnim");

		

		
		
		// set the current level based on the global state
		if (!string.IsNullOrEmpty(global_state.Instance.CurrentLevel))
		{
			currentLevel = GetNodeOrNull<WorldMapLevel>($"LevelHolder/{global_state.Instance.CurrentLevel}");
			playerPanel.Position = currentLevel.GlobalPosition;
		}
		else
		{
			currentLevel = GetNode<WorldMapLevel>("LevelHolder/Map1");
			playerPanel.Position = currentLevel.GlobalPosition;
		}
		LoadGameData();

	}
	public override void _PhysicsProcess(double delta)
	{
		targetLevel = null;

		if (Input.IsActionPressed("up"))
		{
			if (currentLevel.up != null)
			{
				targetLevel = currentLevel.up;
			}
		}
		if (Input.IsActionPressed("down"))
		{
			if (currentLevel.down != null)
			{
				targetLevel = currentLevel.down;
			}
		}
		if (Input.IsActionPressed("left"))
		{
			if (currentLevel.left != null)
			{
				targetLevel = currentLevel.left;
			}
		}
		if (Input.IsActionPressed("right"))
		{
			if (currentLevel.right != null)
			{
				targetLevel = currentLevel.right;
			}
		}
		if (Input.IsActionJustPressed("space"))
		{
			// save the level before change to the next scene
			global_state.Instance.CurrentLevel = currentLevel.Name;
			transition_screen.Instance.ChangeScene("res://World/" + currentLevel.Name + ".tscn");
		}

		if (targetLevel != null &&
			LevelData.LevelDictionary.ContainsKey(targetLevel.Name) &&
			LevelData.LevelDictionary[targetLevel.Name].unlocked &&
			movement)
		{
			movement = false;
			var tween = playerPanel.CreateTween();
			tween.TweenProperty(playerPanel,"position", targetLevel.GlobalPosition, lerp_speed).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			tween.Play();
			OnTweenCompleted();
		}
	}
	public void UpdateLevels()
	{
		foreach (var level in Levels)
		{
			if (LevelData.LevelDictionary.ContainsKey(level.Name))
			{
				if (LevelData.LevelDictionary[level.Name].unlocked == true)
				{
					level.GetNodeOrNull<Sprite2D>("Sprite2D").Texture = (Texture2D)ResourceLoader.Load("res://Textures/mapText/unlocked.png");
					if (LevelData.LevelDictionary[level.Name].beaten == true)
					{
					level.GetNodeOrNull<Sprite2D>("Sprite2D").Texture = (Texture2D)ResourceLoader.Load("res://Textures/mapText/beaten.png");
					}
				}
				else
				{
				   level.GetNodeOrNull<Sprite2D>("Sprite2D").Texture = (Texture2D)ResourceLoader.Load("res://Textures/mapText/locked.png");
				}
			}
		}
	}
	private void OnTweenCompleted()
	{
		currentLevel = (WorldMapLevel)targetLevel;
		movement = true;
	}

	private void LoadGameData()
	{
		string path = ProjectSettings.GlobalizePath("user://");
		string loadedData = LoadTextFromFile(path, "saveGame1.json");

		if (!string.IsNullOrEmpty(loadedData))
		{
			Json jsonLoader = new Json();
			Error error = jsonLoader.Parse(loadedData);

			if (error != Error.Ok)
			{
				GD.Print(error);
				return;
			}

			Dictionary loadedDataDict = (Dictionary)jsonLoader.Data;

			foreach (var level in Levels)
			{
				if (LevelData.LevelDictionary.ContainsKey(level.Name))
				{
					if (loadedDataDict.ContainsKey(level.Name))
					{
						LevelData.LevelDictionary[level.Name].beaten = (bool)loadedDataDict[level.Name];
					}
				}
			}
		}
		else
		{
			// Ensure Map1 is unlocked if no save data is found
			if (LevelData.LevelDictionary.ContainsKey("Map1"))
			{
				LevelData.LevelDictionary["Map1"].unlocked = true;
			}
		}

		LevelData.UpdateMapStatus();
		UpdateLevels();
	}
	private void _on_button_click_end()
	{
		GetTree().ChangeSceneToFile(menu);
	}

	private string LoadTextFromFile(string path, string fileName)
	{
		string data = null;

		path = Path.Join(path, fileName);

		if (!File.Exists(path))
		{
			return null;
		}
		try
		{
			data = File.ReadAllText(path);
		}
		catch (Exception e)
		{
			GD.Print(e);
		}

		return data;
	}
}


