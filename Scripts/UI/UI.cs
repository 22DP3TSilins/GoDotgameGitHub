using Godot;
using System;
using System.Collections.Generic;

public partial class UI : Control
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
	Slider fov = null;
	Slider cameraMaxDistance = null;
	Slider cameraOfset = null;
	ScoreBoard scoreBoard= null;
	Login login = null;
	YesNo yesNo= null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Iegūst mezglus
		player1 = GetParent().GetParent().GetNode<player>("Player");
		player1Camera = player1.GetNode<Camera3D>("CameraRig/RotY/Camera");
		mazeSeed = GetNode<SpinBox>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Seed/Input");
		mazeAlgoritm = GetParent().GetParent().GetNode<MazeAlgoritm>("MazeGen");
		genRndMaze = GetNode<CheckButton>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		difficultyLabel = GetNode<Label>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Text");
		difficultyInput = GetNode<Slider>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/Difficulty/Input");
		fov = GetNode<Slider>("Panel/Control/HBoxContainer/Camera settings/CameraSettings/FOV/Input");
		cameraMaxDistance = GetNode<Slider>("Panel/Control/HBoxContainer/Camera settings/CameraSettings/Distance/Input");
		cameraOfset = GetNode<Slider>("Panel/Control/HBoxContainer/Camera settings/CameraSettings/Ofset/Input");
		scoreBoard = GetNode<ScoreBoard>("../../Scores");
		yesNo = GetNode<YesNo>("../../YesNo");
		login = GetNode<Login>("../../Login/Control");

		// Iedod vērtības mainīgajiem ar objektiem
		Settings = new Dictionary<string, Slider>(){
			{"Difficulty", difficultyInput},
			{"FOV", fov},
			{"cameraMaxDistance", cameraMaxDistance},
			{"cameraOfset", cameraOfset}
		};
		YesNoFunc = new Dictionary<string, (Action Func, string Msg)>(){
			{"delete_accaunt", (DeleteAccountYN, "Are you shure you want to delete accaunt?")},
			{"log_out", (LogOutYN, "Are you shure you want to log out?")}
		};
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _fov_changed(double value)
	{
		// Nomaina FOV
		player1Camera.Fov = (float)value;

	}
	private void _camera_dist_changed(double value)
	{
		// Nomaina Maximālo kameras attālumu no spēlētāja
		
		player1.MaxCameraDistance = (float)value;
	}
	private void _ofset_changed(double value)
	{
		// Nomaina nobīdi
		player1.CameraOfset = (float)value;
	}
	public void _gen_maze()
	{
		// Ģenerē labirintu
		if (genRndMaze.ButtonPressed) {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true);

		} else {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true, (int)mazeSeed.Value);
		}

		// Saglabā iestatījumus un restartē laiku
		scoreBoard.RestartTime();
		scoreBoard.SetGameMode();
		SaveSettings(scoreBoard.user);
		player1.Started = false;
	}
	public void _gen_maze(bool RestartTime, bool resetLocation)
	{
		// Ģenerē labirintu
		if (genRndMaze.ButtonPressed) {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true, -1, resetLocation);

		} else {
			mazeAlgoritm.genMaze((int)difficultyInput.Value, true, (int)mazeSeed.Value, resetLocation);
		}

		// Saglabā iestatījumus un restartē laiku
		if (RestartTime) scoreBoard.RestartTime();
		scoreBoard.SetGameMode();
		SaveSettings(scoreBoard.user);
		player1.Started = false;
	}
	private void _difficulty_changed(double value)
	{
		// Izvēlas grūtības pakāpi
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

	IDictionary<string, Slider> Settings = new Dictionary<string, Slider>();
	public IDictionary<string, (Action Func, string Msg)> YesNoFunc;
	public void LoadSettings(UserData user) {
		// Ielādē iestatījumus, ja spēlētājs nav null
		if (user == null) return;
		foreach (var keyValuePair in Settings) {
			keyValuePair.Value.Value = user.Settings[keyValuePair.Key];
		}

		mazeSeed.Value = user.Settings["mazeSeed"];
		genRndMaze.ButtonPressed = user.Settings["randMaze"] != 0.0;
	}
	public void SaveSettings(UserData user) {
		// Saglabā iestatījumus, ja spēlētājs nav null
		if (user == null) return;

		foreach (var keyValuePair in Settings) {
			user.Settings[keyValuePair.Key] = keyValuePair.Value.Value;
		}

		user.Settings["mazeSeed"] = mazeSeed.Value;
		user.Settings["randMaze"] = genRndMaze.ButtonPressed ? 1.0 : 0.0;
	}
	private void LogOut()
	{
		yesNo.AskYesNo("log_out");
		GetParent<CanvasLayer>().Visible = false;
	}
	private void LogOutYN()
	{
		GetParent<CanvasLayer>().Visible = false;
		login.GetParent<CanvasLayer>().Visible = true;
		login.ClearInputs();
		// Replace with function body.
	}
	private void DeleteAccount()
	{
		yesNo.AskYesNo("delete_accaunt");
		GetParent<CanvasLayer>().Visible = false;
		// Replace with function body.
	}
	private void DeleteAccountYN()
	{
		scoreBoard.DeleteAccount();
		login.DeleteAccount();
		GetParent<CanvasLayer>().Visible = false;
		login.GetParent<CanvasLayer>().Visible = true;
		// Replace with function body.
	}
}
