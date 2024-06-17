using Godot;
using System;

public partial class levelSelect : Control
{

	string level = "res://World/Map1.tscn";
	string menu = "res://TitleScreen/menu.tscn";

	Button button2;

	public override void _Ready()
	{
		button2 = GetNodeOrNull<Button>("btn_play2");
		check();
	}

	private void _on_btn_play_click_end()
	{
		transition_screen.Instance.ChangeScene(level);
	}
	private void _on_button_click_end()
	{
		transition_screen.Instance.ChangeScene(menu);
	}

	public void check()
	{
		if (LevelData.LevelDictionary["Map2"].unlocked)
		{
			button2.Visible = true;
		}
	}
	
	
}




