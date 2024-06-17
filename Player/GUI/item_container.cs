using Godot;
using System;

public partial class item_container : TextureRect
{
	public string upgrade = null;
	TextureRect itemTexture = new TextureRect();

	public override void _Ready()
	{
		itemTexture = GetNodeOrNull<TextureRect>("itemTexture");
		string iconPath = upgrade_db.UPGRADES[upgrade].Icon;
		if (upgrade != null)
		{
			itemTexture.Texture = ResourceLoader.Load<Texture2D>(iconPath);
		}
		
	}
}
