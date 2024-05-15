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
	CanvasLayer map = null;
	public override void _Ready()
	{
		string jsonData = File.ReadAllText("Data/Scores.json");
		users = JsonSerializer.Deserialize<IDictionary<string, UserData>>(jsonData);
		
		Player = GetNode<player>("../Player");
		foreach (var userForEach in users) userForEach.Value.Load();
		GetNode<CanvasLayer>("../Login").Show();
		login = GetNode<Login>("../Login/Control");
		ui = GetNode<UI>("../UI/Control");
		uiGenRndMaze = ui.GetNode<CheckButton>("Panel/Control/HBoxContainer/Maze generation/MazeGeneration/VBoxContainer/HBoxContainer/CheckButton");
		map = GetNode<CanvasLayer>("../Map");

		for (int i = 0; i < 4; i++) {
			leaderBoard[i] = GetNode("Control/VBoxContainer").GetChild<ScoreForPlayer>(i+1);
		}
		SetGameMode();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		IDictionary<string, TimeSpan> allScores = new Dictionary<string, TimeSpan>();
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
	}
	public void SetCurrentUser(string usernameHash, bool newUser, bool admin) {
		user?.Stop();
		ui.SaveSettings(user);
		user?.Save(Player);
		map.Visible = admin;
		if (newUser) users.Add(usernameHash, new UserData(login.GetUsernameFromHash(usernameHash), admin));
		user = users[usernameHash];
		user.Load();
		ui.LoadSettings(user);
		Player.Started = false;
		ui._gen_maze(false, user.Finished || newUser);
		user.Finished = false;
		user.GoTo(Player);
		Player.Started = false;
	}
	public void SetGameMode() {
		currentGameMode = uiGenRndMaze.ButtonPressed ? new() : new((int)ui.difficultyInput.Value, (int)ui.mazeSeed.Value);
	}

	public override void _Notification(int what)
	{
		// Saglabā lietotāju datus pirms iziešanas no programmas
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
