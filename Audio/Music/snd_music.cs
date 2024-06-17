using Godot;
using System;

public partial class snd_music : AudioStreamPlayer2D
{
	private void _on_player_player_death()
	{
		Playing = false;
	}
	private void _on_player_paused()
	{
		VolumeDb = -40;
	}
	
}




