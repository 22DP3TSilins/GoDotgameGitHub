using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

public partial class ScoreBoard : CanvasLayer
{
	
	public UserData user;
	player Player = null;
	IDictionary<string, UserData> users;
	UI ui = null;
	CheckButton uiGenRndMaze = null;
	Login login = null;
	GameMode currentGameMode;
	ScoreForPlayer[] leaderBoard = new ScoreForPlayer[4];
	// Called when the node enters the scene tree for the first time.
	// 5251b6c1d47e864df4c010698dc6a398358d07ed7589ee82140ea307fcfa219a82c57cdabb72de18e86e840d1a3e2f3b30726f2514d47e3d9cd9cecad16ad09c
	public override void _Ready()
	{
		// File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(
		// 	new Dictionary<string, UserData>(){{
		// 		"5251b6c1d47e864df4c010698dc6a398358d07ed7589ee82140ea307fcfa219a82c57cdabb72de18e86e840d1a3e2f3b30726f2514d47e3d9cd9cecad16ad09c", 
		// 		new UserData()}}));
		// File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(users));
		string jsonData = File.ReadAllText("Data/Scores.json");
		users = JsonSerializer.Deserialize<IDictionary<string, UserData>>(jsonData);
		foreach (var userForEach in users) userForEach.Value.Load();
		GetNode<CanvasLayer>("../Login").Show();
		login = GetNode<Login>("../Login/Control");
		ui = GetNode<UI>("../UI/Control");
		Player = GetNode<player>("../Player");
		uiGenRndMaze = ui.GetNode<CheckButton>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		
		// allScores.Sort();
		// allScores = allScores.ToDictionary<string, TimeSpan>(key => key.Key);
		for (int i = 0; i < 4; i++) {
			leaderBoard[i] = GetNode("Control/VBoxContainer").GetChild<ScoreForPlayer>(i+1);
		}
		SetGameMode();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			IDictionary<string, TimeSpan> allScores = new Dictionary<string, TimeSpan>();
			int IofUser = -1;
			int i = 0;
			int i2 = 0;
			// GD.Print("ForEach start:");
			foreach (KeyValuePair<string, UserData> userForEach in users) {
				TimeSpan userScore = userForEach.Value.GetBestTime(currentGameMode);
				if (userForEach.Value == user) IofUser = i;
				if (userScore != TimeSpan.Zero) {
					allScores.Add(userForEach.Value.Username, userScore);
					// GD.Print($"i: {i} {i2} ss:");
					i++;

				} else {
					// GD.Print($"i: {i} {i2} f:");
				}
				i2++;
			}
			// GD.Print("ForEach end:\n");

			var sorted = allScores.OrderBy(key => key.Value).ThenBy(key => key.Key);

			for (i = 0; i < 3; i++) leaderBoard[i].ClearScore();

			i = 0;
			foreach (var playerScore in sorted) {
				if (i > 3) break;
				leaderBoard[i].SetScore(i + 1, playerScore.Key, playerScore.Value);
				i++;
			}
			
			if (user != null && IofUser != -1) {
				leaderBoard[3].SetScore(IofUser + 1, user.Username, user.GetTime);
			}

		// } else {
		// 	user?.Finish(currentGameMode);
		// }
		// if (user != null) {
		// 	TimeSpan pagajusaisLaiks = DateTime.Now - user.Start;
		// 	string text = string.Format("{0:0.000}", pagajusaisLaiks.TotalSeconds);
		// }

	}
	// public void AddUserScore(string usernameHash) {
	// 	users.Add(usernameHash, new UserData());
	// }
	public void SetCurrentUser(string usernameHash, bool newUser) {
		user?.Stop();
		ui.SaveSettings(user);
		if (newUser) users.Add(usernameHash, new UserData(login.GetUsernameFromHash(usernameHash)));
		user = users[usernameHash];
		Player.Started = false;
		// user.RestartTime();
		user.Load();
		ui.LoadSettings(user);
		GD.Print(user == users[usernameHash]);
		SetGameMode();
		Player.Started = false;
		// user.ContinueTime();
	}
	public void SetGameMode() {
		currentGameMode = uiGenRndMaze.ButtonPressed ? new() : new((int)ui.difficultyInput.Value, (int)ui.mazeSeed.Value);
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest) {
			Stop();
			user?.Save();
			ui.SaveSettings(user);
			
			File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(users));
			GetTree().Quit(); // default behavior
		}
	}
	public void StartClock() {
		if (user != null) user.Start = DateTime.Now;
	}
	public void Finish() {
		Player.Started = false;
		user?.Finish(currentGameMode);
	}
	public void Stop() {
		user?.Stop();
	}
	public void Continue() {
		user?.ContinueTime();
	}
	public void RestartTime() {
		user?.RestartTime();
	}
	public void DeleteAccount() {
		if (users.ContainsKey(login.currentUserHash)) users.Remove(login.currentUserHash);
	}
}
