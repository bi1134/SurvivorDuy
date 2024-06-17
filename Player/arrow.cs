using Godot;
using System;

public partial class arrow : Area2D
{
	Vector2 mousePosition;

	public override void _Process(double delta)
	{
		mousePosition = GetGlobalMousePosition();
		LookAt(mousePosition);
	}
}
