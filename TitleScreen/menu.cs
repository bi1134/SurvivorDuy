using Godot;
using System;

public partial class menu : Control
{
	string levelSelect = "res://World/world_map.tscn";
	string setting = "res://TitleScreen/setting.tscn";
	
	private async void _on_btn_play_click_end()
	{
		GetTree().ChangeSceneToFile(levelSelect);
	}
	private void _on_btn_setting_click_end()
	{
		GetTree().ChangeSceneToFile(setting);
	}
	private void _on_btn_exit_click_end()
	{
		GetTree().Quit();
	}
}
















