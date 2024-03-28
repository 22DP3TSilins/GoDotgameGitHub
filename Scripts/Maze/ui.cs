using Godot;
using System;

public partial class ui : Control
{
	// player player1 = null;
	// Camera3D player1Camera = null;
	// MazeAlgoritm mazeAlgoritm = new MazeAlgoritm();
	// SpinBox mazeSeed = null;
	player player1 = null;
	Camera3D player1Camera = null;
	MazeAlgoritm mazeAlgoritm = null;
	SpinBox mazeSeed = null;
	CheckButton genRndMaze = null;
	Label difficultyLabel = null;
	Slider difficultyInput = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player1 = GetParent().GetParent().GetNode<player>("Player");
		player1Camera = player1.GetNode<Camera3D>("CameraRig/RotY/Camera");
		mazeSeed = GetNode<SpinBox>("Panel/HBoxContainer/Maze generation/MazeGeneration/Seed/Input");
		mazeAlgoritm = GetParent().GetParent().GetNode<MazeAlgoritm>("MazeGen");
		genRndMaze = GetNode<CheckButton>("Panel/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		difficultyLabel = GetNode<Label>("Panel/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Text");
		difficultyInput = GetNode<Slider>("Panel/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Input");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _fov_changed(double value)
	{
		// Replace with function body.
		player1Camera.Fov = (float)value;

	}
	private void _camera_dist_changed(double value)
	{
		// Replace with function body.
		player1.MaxCameraDistance = (float)value;
	}
	private void _on_input_value_changed(double value)
	{
		// Replace with function body.
		GD.Print("huhhh");
	}
	private void _ofset_changed(double value)
	{
		// Replace with function body.
		player1.CameraOfset = (float)value;
	}
	private void _gen_maze()
	{
		// Replace with function body.
		if (genRndMaze.ButtonPressed) {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true);
		} else {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true, (int)mazeSeed.Value);
		}
	}
	private void _difficulty_changed(double value)
	{
		// Replace with function body.
		if (value == 15.0) {
			difficultyLabel.Text = "Difficulty: expert";
		} else if (value > 11.0) {
			difficultyLabel.Text = "Difficulty: Hard";
		} else if (value > 7.0) {
			difficultyLabel.Text = "Difficulty: Medium";
		} else {
			difficultyLabel.Text = "Difficulty: Easy";
		}
	}
}



