using Godot;
using System;

public partial class WorldMapLevel : Node2D
{
	[Export] public Node2D up;
	[Export] public Node2D down;
	[Export] public Node2D left;
	[Export] public Node2D right;
}
