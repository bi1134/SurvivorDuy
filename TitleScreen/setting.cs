using Godot;
using System;

public partial class setting : Control
{
	string menu = "res://TitleScreen/menu.tscn";
	
	private void _on_return_pressed()
	{
		GetTree().ChangeSceneToFile(menu);
	}
	
}


