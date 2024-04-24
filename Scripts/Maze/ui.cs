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
	public SpinBox mazeSeed = null;
	public CheckButton genRndMaze = null;
	Label difficultyLabel = null;
	public Slider difficultyInput = null;
	ScoreBoard scoreBoard= null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player1 = GetParent().GetParent().GetNode<player>("Player");
		player1Camera = player1.GetNode<Camera3D>("CameraRig/RotY/Camera");
		mazeSeed = GetNode<SpinBox>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Seed/Input");
		mazeAlgoritm = GetParent().GetParent().GetNode<MazeAlgoritm>("MazeGen");
		genRndMaze = GetNode<CheckButton>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		difficultyLabel = GetNode<Label>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Text");
		difficultyInput = GetNode<Slider>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Input");
		scoreBoard = GetNode<ScoreBoard>("../../Scores");
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
			scoreBoard.CurrentGameMode = new GameMode();

		} else {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true, (int)mazeSeed.Value);
			scoreBoard.CurrentGameMode = new GameMode((int)difficultyInput.Value, (int)mazeSeed.Value);
		}
		scoreBoard.RestartTime();
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



