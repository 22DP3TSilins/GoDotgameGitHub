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
	public override void _Ready()
	{
		string jsonData = File.ReadAllText("Data/Scores.json");
		users = JsonSerializer.Deserialize<IDictionary<string, UserData>>(jsonData);
		
		Player = GetNode<player>("../Player");
		foreach (var userForEach in users) userForEach.Value.Load(Player);
		GetNode<CanvasLayer>("../Login").Show();
		login = GetNode<Login>("../Login/Control");
		ui = GetNode<UI>("../UI/Control");
		uiGenRndMaze = ui.GetNode<CheckButton>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		
		for (int i = 0; i < 4; i++) {
			leaderBoard[i] = GetNode("Control/VBoxContainer").GetChild<ScoreForPlayer>(i+1);
		}
		SetGameMode();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			IDictionary<string, TimeSpan> allScores = new Dictionary<string, TimeSpan>();
			int i2 = 0;
			// GD.Print("ForEach start:");
			foreach (KeyValuePair<string, UserData> userForEach in users) {
				TimeSpan userScore = userForEach.Value.GetBestTime(currentGameMode);
				
				if (userScore != TimeSpan.Zero) {
					allScores.Add(userForEach.Value.Username, userScore);
					// GD.Print($"i: {i} {i2} ss:");

				} else {
					// GD.Print($"i: {i} {i2} f:");
				}
			}
			// GD.Print("ForEach end:\n");

			var sorted = allScores.OrderBy(key => key.Value).ThenBy(key => key.Key);
			
			int IofUser = -1;
			int i = 0;
			if (!(user == null)) {
				foreach (var userForEach in sorted) {
					if (string.Compare(userForEach.Key, user.Username) == 0) IofUser = i;
					i++;
				}
			}

			for (i = 0; i < 3; i++) leaderBoard[i].ClearScore();

			i = 0;
			foreach (var playerScore in sorted) {
				if (i > 3) break;
				leaderBoard[i].SetScore(i + 1, playerScore.Key, playerScore.Value);
				i++;
			}
			
			if (user != null) {
				
				leaderBoard[3].SetScore(IofUser == -1 ? sorted.Count() + 1 : IofUser + 1, user.Username, user.GetTime);
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
		ui._gen_maze(false, user.Finished || newUser);
		user.Finished = false;
		user.SetPos(Player);
		// user.RestartTime();
		user.Load(Player);
		ui.LoadSettings(user);
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
			user?.Save(Player);
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
