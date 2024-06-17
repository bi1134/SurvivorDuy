using Godot;
using System;
using System.Collections.Generic;

public partial class item_option : TextureRect
{

	Label lblName;
	Label lblDescription;
	Label lblLevel;
	TextureRect itemIcon;

	bool mouse_over = false;
	public string item = null;
	Node2D player;

	[Signal] public delegate void selectedUpgradeEventHandler(string upgrade);

	public override void _Ready()
	{
		lblName = GetNodeOrNull<Label>("lbl_name");
		lblDescription = GetNodeOrNull<Label>("lbl_description");
		lblLevel = GetNodeOrNull<Label>("lbl_level");
		itemIcon = GetNodeOrNull<TextureRect>("ItemIcon");


		player = GetTree().GetFirstNodeInGroup("player") as Node2D;
		if (player != null)
		{
			Connect(SignalName.selectedUpgrade, new Callable(player, "UpgradeCharacter"));
		}
		if (item == null)
		{
			item = "food";
		}
			
		lblName.Text = upgrade_db.UPGRADES[item].DisplayName;
		lblDescription.Text = upgrade_db.UPGRADES[item].Details;
		lblLevel.Text = upgrade_db.UPGRADES[item].Level;
		string iconPath = upgrade_db.UPGRADES[item].Icon;
		itemIcon.Texture = GD.Load<Texture2D>(iconPath);

	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.IsAction("click"))
			{
				if(mouse_over)
				{
					EmitSignal(SignalName.selectedUpgrade, item);
				}
			}
		}
		else if (@event is InputEventScreenTouch touchEvent)
		{
			if (touchEvent.IsPressed())
			{
				if (mouse_over)
				{
					EmitSignal(SignalName.selectedUpgrade, item);
				}
			}
		}
	}

	private void _on_mouse_entered()
	{
		mouse_over = true;
	}


	private void _on_mouse_exited()
	{
		mouse_over = false;
	}


}




