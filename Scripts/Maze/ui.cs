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
	MazeAlgoritm mazeAlgoritm = new MazeAlgoritm();
	SpinBox mazeSeed = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player1 = GetParent().GetParent().GetNode<player>("Player");
		player1Camera = player1.GetNode<Camera3D>("CameraRig/RotY/Camera");
		mazeSeed = GetNode<SpinBox>("TabContainer/Maze generation/VBoxContainer/Seed/Input");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _fov_changed(double value)
	{
		// Replace with function body.
		GD.Print("fov");
		player1Camera.Fov = (float)value;

	}
	private void _camera_dist_changed(double value)
	{
		// Replace with function body.
		GD.Print("camera");
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
		GD.Print("ofset");
		player1.CameraOfset = (float)value;
	}
	private void _gen_maze()
	{
		// Replace with function body.
		mazeAlgoritm.genMaze((int)mazeSeed.Value);
	}
}
