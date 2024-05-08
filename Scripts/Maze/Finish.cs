using Godot;
using System;

public partial class Finish : Node3D
{
	ScoreBoard scoreBoard = null;
	AudioStreamPlayer audioStreamPlayer = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Area3D>("Mesh/Area3D").Monitoring = true;
		scoreBoard = GetNode<ScoreBoard>("../../Scores");
		audioStreamPlayer = GetNode<AudioStreamPlayer>("Mesh/AudioStreamPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_area_3d_body_entered(Node3D body)
	{
		// Replace with function body.
		scoreBoard.Finish();
		audioStreamPlayer.Play();
	}
}




