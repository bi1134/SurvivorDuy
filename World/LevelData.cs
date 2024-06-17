using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelData : Node
{
	public static Dictionary<string, levelDict> LevelDictionary = new Dictionary<string, levelDict>
	{
		//map 1
		{
			"Map1", new levelDict
			{
				unlocked = true,
				score = 0,
				max_score = 0,
				Prerequisite = new List<string>(),
				beaten = false
			}
		},
		{
			"Map2", new levelDict
			{
				unlocked = false,
				score = 0,
				max_score = 0,
				Prerequisite = new List<string>{"Map1"},
				beaten = false
			}
		},
		{
			"Map3", new levelDict
			{
				unlocked = false,
				score = 0,
				max_score = 0,
				Prerequisite = new List<string>{"Map2"},
				beaten = false
			}
		}
	};

	public static void UpdateMapStatus()
	{
		// Map2 depends on Map1
		if (LevelDictionary.ContainsKey("Map1"))
		{
			bool mapBeaten = LevelDictionary["Map1"].beaten;
			if (mapBeaten && LevelDictionary.ContainsKey("Map2"))
			{
				LevelDictionary["Map2"].unlocked = true;
			}
			else if (!mapBeaten && LevelDictionary.ContainsKey("Map2"))
			{
				LevelDictionary["Map2"].unlocked = false;
			}
		}

		// Map3 depends on Map2
		if (LevelDictionary.ContainsKey("Map2"))
		{
			bool mapBeaten = LevelDictionary["Map2"].beaten;
			if (mapBeaten && LevelDictionary.ContainsKey("Map3"))
			{
				LevelDictionary["Map3"].unlocked = true;
			}
			else if (!mapBeaten && LevelDictionary.ContainsKey("Map3"))
			{
				LevelDictionary["Map3"].unlocked = false;
			}
		}
	}

}

public class levelDict
{
	public bool unlocked { get; set; }
	public int score { get; set; }
	public int max_score { get; set; }
	public List<string> Prerequisite { get; set; }
	public bool beaten { get; set; }
}
