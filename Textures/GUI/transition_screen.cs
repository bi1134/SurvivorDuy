using Godot;
using System;

public partial class transition_screen : CanvasLayer
{
	ColorRect colorRect;
	AnimationPlayer animationPlayer;
	public static transition_screen Instance { get; private set; }
	[Signal] public delegate void OnTransitionFinishedEventHandler();

	public override void _Ready()
	{
		colorRect = GetNodeOrNull<ColorRect>("ColorRect");
		animationPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");
		Instance = this;
	}


	public async void ChangeScene(string target)
	{
		animationPlayer.Play("fadeToBlack");
		await ToSignal(animationPlayer, "animation_finished");

		GetTree().ChangeSceneToFile(target);
		animationPlayer.PlayBackwards("fadeToBlack");

	}
   
}
