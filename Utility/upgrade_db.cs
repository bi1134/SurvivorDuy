using Godot;
using System;
using System.Collections.Generic;

public partial class upgrade_db : Node
{
	const string IconPath = "res://Textures/Items/Upgrades/";
	const string WeaponPath = "res://Textures/Items/Weapons/";
	public static Dictionary<string, Upgrade> UPGRADES = new Dictionary<string, Upgrade>
	{
		// Ice Spear upgrades
		{
			"iceSpear1", new Upgrade
			{
				Icon = WeaponPath + "Stone.png",
				DisplayName = "Stone",
				Details = "Throws a stone at an enemy.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "weapon"
			}
		},
		{
			"iceSpear2", new Upgrade
			{
				Icon = WeaponPath + "Stone.png",
				DisplayName = "Stone",
				Details = "Throws an extra stone.",
				Level = "Level 2",
				Prerequisite = new List<string>{"iceSpear1"},
				Type = "weapon"
			}
		},
		{
			"iceSpear3", new Upgrade
			{
				Icon = WeaponPath + "Stone.png",
				DisplayName = "Stone",
				Details = "Stones pass through one more enemy and deal +3 damage.",
				Level = "Level 3",
				Prerequisite = new List<string>{"iceSpear2"},
				Type = "weapon"
			}
		},
		{
			"iceSpear4", new Upgrade
			{
				Icon = WeaponPath + "Stone.png",
				DisplayName = "Stone",
				Details = "Throws two extra stones.",
				Level = "Level 4",
				Prerequisite = new List<string>{"iceSpear3"},
				Type = "weapon"
			}
		},
		// Javelin upgrades
		{
			"javelin1", new Upgrade
			{
				Icon = WeaponPath + "Stick.png",
				DisplayName = "Stick",
				Details = "A magical stick attacks enemies in a straight line.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "weapon"
			}
		},
		{
			"javelin2", new Upgrade
			{
				Icon = WeaponPath + "Stick.png",
				DisplayName = "Stick",
				Details = "The stick attacks one more enemy.",
				Level = "Level 2",
				Prerequisite = new List<string>{"javelin1"},
				Type = "weapon"
			}
		},
		{
			"javelin3", new Upgrade
			{
				Icon = WeaponPath + "Stick.png",
				DisplayName = "Stick",
				Details = "The stick attacks one more additional enemy.",
				Level = "Level 3",
				Prerequisite = new List<string>{"javelin2"},
				Type = "weapon"
			}
		},
		{
			"javelin4", new Upgrade
			{
				Icon = WeaponPath + "Stick.png",
				DisplayName = "Stick",
				Details = "The stick deals +5 damage and has +20% knockback.",
				Level = "Level 4",
				Prerequisite = new List<string>{"javelin3"},
				Type = "weapon"
			}
		},
		// toxic upgrades
		{
			"toxic1", new Upgrade
			{
				Icon = WeaponPath + "toxicAura.png",
				DisplayName = "Toxic aura",
				Details = "Create an aura that dealt 2 damage per second to nearby enemy.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "weapon"
			}
		},
		{
			"toxic2", new Upgrade
			{
				Icon = WeaponPath + "toxicAura.png",
				DisplayName = "Toxic aura",
				Details = "The aura became larger by 50%.",
				Level = "Level 2",
				Prerequisite = new List<string>{"toxic1"},
				Type = "weapon"
			}
		},
		{
			"toxic3", new Upgrade
			{
				Icon = WeaponPath + "toxicAura.png",
				DisplayName = "Toxic aura",
				Details = "The aura became larger by 50% and dealt 4 damage more.",
				Level = "Level 3",
				Prerequisite = new List<string>{"toxic2"},
				Type = "weapon"
			}
		},
		{
			"toxic4", new Upgrade
			{
				Icon = WeaponPath + "toxicAura2.png",
				DisplayName = "Toxic aura",
				Details = "The aura dealt 4 damage more.",
				Level = "Level 4",
				Prerequisite = new List<string>{"toxic3"},
				Type = "weapon"
			}
		},
		// Tornado upgrades
		{
			"tornado1", new Upgrade
			{
				Icon = WeaponPath + "tornado.png",
				DisplayName = "Tornado",
				Details = "Creates a tornado that moves towards enemies.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "weapon"
			}
		},
		{
			"tornado2", new Upgrade
			{
				Icon = WeaponPath + "tornado.png",
				DisplayName = "Tornado",
				Details = "Creates an extra tornado.",
				Level = "Level 2",
				Prerequisite = new List<string>{"tornado1"},
				Type = "weapon"
			}
		},
		{
			"tornado3", new Upgrade
			{
				Icon = WeaponPath + "tornado.png",
				DisplayName = "Tornado",
				Details = "Reduces tornado cooldown by 0.5 seconds.",
				Level = "Level 3",
				Prerequisite = new List<string>{"tornado2"},
				Type = "weapon"
			}
		},
		{
			"tornado4", new Upgrade
			{
				Icon = WeaponPath + "tornado.png",
				DisplayName = "Tornado",
				Details = "Creates an extra tornado and increases knockback by 25%.",
				Level = "Level 4",
				Prerequisite = new List<string>{"tornado3"},
				Type = "weapon"
			}
		},
		// Ring upgrades
		{
			"ring1", new Upgrade
			{
				Icon = IconPath + "urand_mage.png",
				DisplayName = "Ring",
				Details = "Spells spawn one more attack.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"ring2", new Upgrade
			{
				Icon = IconPath + "urand_mage.png",
				DisplayName = "Ring",
				Details = "Spells spawn one more attack.",
				Level = "Level 2",
				Prerequisite = new List<string>{"ring1"},
				Type = "upgrade"
			}
		},
		// Armor upgrades
		{
			"armor1", new Upgrade
			{
				Icon = IconPath + "helmet_1.png",
				DisplayName = "Armor",
				Details = "Reduces damage by 1 point.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"armor2", new Upgrade
			{
				Icon = IconPath + "helmet_1.png",
				DisplayName = "Armor",
				Details = "Reduces damage by 1 more point.",
				Level = "Level 2",
				Prerequisite = new List<string>{"armor1"},
				Type = "upgrade"
			}
		},
		{
			"armor3", new Upgrade
			{
				Icon = IconPath + "helmet_1.png",
				DisplayName = "Armor",
				Details = "Reduces damage by 1 more point.",
				Level = "Level 3",
				Prerequisite = new List<string>{"armor2"},
				Type = "upgrade"
			}
		},
		{
			"armor4", new Upgrade
			{
				Icon = IconPath + "helmet_1.png",
				DisplayName = "Armor",
				Details = "Reduces damage by 1 more point.",
				Level = "Level 4",
				Prerequisite = new List<string>{"armor3"},
				Type = "upgrade"
			}
		},
		// magnet upgrades
		{
			"magnet1", new Upgrade
			{
				Icon = IconPath + "magnet.png",
				DisplayName = "Magnet",
				Details = "Increase the pickup range by 150%.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		// Speed upgrades
		{
			"speed1", new Upgrade
			{
				Icon = IconPath + "boots_4_green.png",
				DisplayName = "Boots",
				Details = "+20 movement speed.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"speed2", new Upgrade
			{
				Icon = IconPath + "boots_4_green.png",
				DisplayName = "Boots",
				Details = "+20 movement speed.",
				Level = "Level 2",
				Prerequisite = new List<string>{"speed1"},
				Type = "upgrade"
			}
		},
		{
			"speed3", new Upgrade
			{
				Icon = IconPath + "boots_4_green.png",
				DisplayName = "Boots",
				Details = "+20 movement speed.",
				Level = "Level 3",
				Prerequisite = new List<string>{"speed2"},
				Type = "upgrade"
			}
		},
		{
			"speed4", new Upgrade
			{
				Icon = IconPath + "boots_4_green.png",
				DisplayName = "Boots",
				Details = "+20 movement speed.",
				Level = "Level 4",
				Prerequisite = new List<string>{"speed3"},
				Type = "upgrade"
			}
		},
		// Tome upgrades
		{
			"tome1", new Upgrade
			{
				Icon = IconPath + "thick_new.png",
				DisplayName = "Tome",
				Details = "Increases spell size by 10%.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"tome2", new Upgrade
			{
				Icon = IconPath + "thick_new.png",
				DisplayName = "Tome",
				Details = "Increases spell size by 10%.",
				Level = "Level 2",
				Prerequisite = new List<string>{"tome1"},
				Type = "upgrade"
			}
		},
		{
			"tome3", new Upgrade
			{
				Icon = IconPath + "thick_new.png",
				DisplayName = "Tome",
				Details = "Increases spell size by 10%.",
				Level = "Level 3",
				Prerequisite = new List<string>{"tome2"},
				Type = "upgrade"
			}
		},
		{
			"tome4", new Upgrade
			{
				Icon = IconPath + "thick_new.png",
				DisplayName = "Tome",
				Details = "Increases spell size by 10%.",
				Level = "Level 4",
				Prerequisite = new List<string>{"tome3"},
				Type = "upgrade"
			}
		},
		// Scroll upgrades
		{
			"scroll1", new Upgrade
			{
				Icon = IconPath + "scroll_old.png",
				DisplayName = "Scroll",
				Details = "Decreases spell cooldown by 5%.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"scroll2", new Upgrade
			{
				Icon = IconPath + "scroll_old.png",
				DisplayName = "Scroll",
				Details = "Decreases spell cooldown by 5%.",
				Level = "Level 2",
				Prerequisite = new List<string>{"scroll1"},
				Type = "upgrade"
			}
		},
		{
			"scroll3", new Upgrade
			{
				Icon = IconPath + "scroll_old.png",
				DisplayName = "Scroll",
				Details = "Decreases spell cooldown by 5%.",
				Level = "Level 3",
				Prerequisite = new List<string>{"scroll2"},
				Type = "upgrade"
			}
		},
		{
			"scroll4", new Upgrade
			{
				Icon = IconPath + "scroll_old.png",
				DisplayName = "Scroll",
				Details = "Decreases spell cooldown by 5%.",
				Level = "Level 4",
				Prerequisite = new List<string>{"scroll3"},
				Type = "upgrade"
			}
		},
		// Hearts upgrades
		{
			"emptyHeart1", new Upgrade
			{
				Icon = IconPath + "EmpyHeart.png",
				DisplayName = "Empty Heart",
				Details = "Increase 5 max health",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"emptyHeart2", new Upgrade
			{
				Icon = IconPath + "EmpyHeart.png",
				DisplayName = "Empty Heart",
				Details = "Increase 5 max health",
				Level = "Level 2",
				Prerequisite = new List<string>{"emptyHeart1"},
				Type = "upgrade"
			}
		},
		{
			 "emptyHeart3", new Upgrade
			{
				Icon = IconPath + "EmpyHeart.png",
				DisplayName = "Empty Heart",
				Details = "Increase 5 max health",
				Level = "Level 3",
				Prerequisite = new List<string>{"emptyHeart2"},
				Type = "upgrade"
			}
		},
		{
			"emptyHeart4", new Upgrade
			{
				Icon = IconPath + "EmpyHeart.png",
				DisplayName = "Empty Heart",
				Details = "Increase 5 max health",
				Level = "Level 4",
				Prerequisite = new List<string>{"emptyHeart3"},
				Type = "upgrade"
			}
		},
		// Losing streak upgrades
		{
			"lossStreak1", new Upgrade
			{
				Icon = IconPath + "LossStreak.png",
				DisplayName = "Loss Streak",
				Details = "Raises inflicted damage by 10%.",
				Level = "Level 1",
				Prerequisite = new List<string>(),
				Type = "upgrade"
			}
		},
		{
			"lossStreak2", new Upgrade
			{
				Icon = IconPath + "LossStreak.png",
				DisplayName = "Loss Streak",
				Details = "Raises inflicted damage by 10%.",
				Level = "Level 2",
				Prerequisite = new List<string>{"lossStreak1"},
				Type = "upgrade"
			}
		},
		{
			 "lossStreak3", new Upgrade
			{
				Icon = IconPath + "LossStreak.png",
				DisplayName = "Loss Streak",
				Details = "Raises inflicted damage by 10%.",
				Level = "Level 3",
				Prerequisite = new List<string>{"lossStreak2"},
				Type = "upgrade"
			}
		},
		{
			"lossStreak4", new Upgrade
			{
				Icon = IconPath + "LossStreak.png",
				DisplayName = "Loss Streak",
				Details = "Raises inflicted damage by 10%.",
				Level = "Level 4",
				Prerequisite = new List<string>{"lossStreak3"},
				Type = "upgrade"
			}
		},
		{
			"lossStreak5", new Upgrade
			{
				Icon = IconPath + "LossStreak.png",
				DisplayName = "Loss Streak",
				Details = "Raises inflicted damage by 10%.",
				Level = "Level 5",
				Prerequisite = new List<string>{"lossStreak4"},
				Type = "upgrade"
			}
		},
		// Food item
		{
			"food", new Upgrade
			{
				Icon = IconPath + "chunk.png",
				DisplayName = "Food",
				Details = "Heals 20 health.",
				Level = "1 Chunk",
				Prerequisite = new List<string>(),
				Type = "item"
			}
		}
	};
}

public class Upgrade
{
	public string Icon { get; set; }
	public string DisplayName { get; set; }
	public string Details { get; set; }
	public string Level { get; set; }
	public List<string> Prerequisite { get; set; }
	public string Type { get; set; }
}
