using Godot;
using System;

public partial class audio_slider_setting : Control
{
	[Export] public sound busName;
	private int busIndex = 0;
	private Label AudioNameLabel;
	private Label AudioNumLabel;
	private HSlider HSlider;

	public override void _Ready()
	{
		AudioNameLabel = GetNodeOrNull<Label>("HBoxContainer/Audio_Name_Label");
		AudioNumLabel = GetNodeOrNull<Label>("HBoxContainer/Audio_Num_Label");
		HSlider = GetNodeOrNull<HSlider>("HBoxContainer/HSlider");
		HSlider.Connect("value_changed",new Callable(this, nameof(on_value_changed)));
		get_bus_name_by_index();
		set_name_label_text();
		set_slider_value();
	}


	public void set_name_label_text()
	{
		AudioNameLabel.Text = busName + " Volume:";
	}

	public void set_audio_num_label_text()
	{
		AudioNumLabel.Text = string.Format("{0:F0}%", HSlider.Value * 100) + "";
	}

	public void get_bus_name_by_index()
	{
		busIndex = AudioServer.GetBusIndex(busName.ToString());
	}

	public void set_slider_value()
	{
		HSlider.Value = DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
		set_audio_num_label_text();
	}

	private void on_value_changed(float value)
	{
		AudioServer.SetBusVolumeDb(busIndex, LinearToDb(value));
		set_audio_num_label_text();
	}
	private float LinearToDb(float linear)
	{
		return 20.0f * (float)Math.Log10(linear);
	}
	public static float DbToLinear(float db)
	{
		return (float)Math.Pow(10, db / 20.0f);
	}
}

public enum sound
{ 
	Master,
	Music,
	Sfx
}

