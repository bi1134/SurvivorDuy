using Godot;
using System;

public partial class base_button : Button
{
	[Signal] public delegate void clickEndEventHandler();

	AudioStreamPlayer2D snd_hover;
	AudioStreamPlayer2D snd_click;

	public override void _Ready()
	{
		snd_hover = GetNodeOrNull<AudioStreamPlayer2D>("snd_hover");
		snd_click = GetNodeOrNull<AudioStreamPlayer2D>("snd_click");
	}


	private void _on_pressed()
	{
		snd_click.Play();
	}
	private void _on_snd_click_finished()
	{
		EmitSignal(SignalName.clickEnd);
	}

}





