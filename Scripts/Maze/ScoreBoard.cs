using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
public struct GameMode{
	public int Difficulty = -1;
	public int Seed = -1;
	public string SetGameModeStr {set {
		string[] mode = value.Split(':');
		Difficulty = int.Parse(mode[0]);
		Seed = int.Parse(mode[1]);

	}}
	public GameMode(int difficulty, int seed) {
		Difficulty = difficulty;
		Seed = seed;
	}
	public GameMode(string mode) {
		string[] modeSplit = mode.Split(':');
		Difficulty = int.Parse(modeSplit[0]);
		Seed = int.Parse(modeSplit[1]);
	}
	public override string ToString() {
		return $"{Difficulty}:{Seed}";
	}
}
class UserData {
		public IDictionary<string, TimeSpan> ScoresStr {get; set;}
		// public IDictionary<GameMode, TimeSpan> Scores = new Dictionary<GameMode, TimeSpan>();
		public TimeSpan FastestTimeRandSeed {get; set;}
		public DateTime Start {get; set;}
		public bool WasJustStopped {get; set;} = false;
		public string Username {get; set;}
		public void Finish(GameMode gameMode) {
			Finished = true;
			TimeSpan time = DateTime.Now - Start;
			Stop();
			if ((gameMode.Seed == -1) && (gameMode.Difficulty == -1)) {
				FastestTimeRandSeed = time < FastestTimeRandSeed ? time : FastestTimeRandSeed;
			} else {
				if (ScoresStr.ContainsKey(gameMode.ToString())) {
					ScoresStr[gameMode.ToString()] = time < ScoresStr[gameMode.ToString()] ? time : ScoresStr[gameMode.ToString()];
				} else {
					ScoresStr[gameMode.ToString()] = time;
				}
			}
		}
		public bool Finished {get; set;} = false;
		public void Stop(){
			if (!WasJustStopped) StopTime = DateTime.Now;
			WasJustStopped = true;
		}
		public void ContinueTime() {
			if (WasJustStopped && !Finished) Start += DateTime.Now - StopTime;
			WasJustStopped = false;
		}
		public TimeSpan GetTime { get { 
			return DateTime.Now - (Start + (WasJustStopped ? DateTime.Now - StopTime : TimeSpan.Zero)); } }
		public DateTime StopTime {get; set;}

		// public void UpdateScores(TimeSpan time) {
		// 	FastestTimeRandSeed = time < FastestTimeRandSeed ? time : FastestTimeRandSeed;
		// }
		// public void UpdateScores(TimeSpan time, (int difficulty, int seed) {
		// 	Scores[(difficulty, seed)] = time < Scores[(difficulty, seed)] ? time : Scores[(difficulty, seed)];
		// }

		public UserData(string username){
			FastestTimeRandSeed = TimeSpan.MaxValue;
			ScoresStr = new Dictionary<string, TimeSpan>();
			Username = username;
			// foreach (var gameModeScore in ScoresStr) {
			// 	string[] mode = gameModeScore.Key.Split(':');
			// 	int difficulty = int.Parse(mode[0]);
			// 	int seed = int.Parse(mode[1]);
			// 	ScoresStr.Add(new GameMode(difficulty,seed).ToString(), gameModeScore.Value);
			// }
		}
		public void RestartTime() {
			Start = DateTime.Now;
			Finished = false;
			WasJustStopped = false;
		}
	}
public partial class ScoreBoard : CanvasLayer
{
	
	UserData user;
	IDictionary<string, UserData> users;
	ui GetUi = null;
	Login login = null;
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
		GetNode<CanvasLayer>("../Login").Show();
		login = GetNode<Login>("../Login/Control");
		GetUi = GetNode<ui>("../UI/Control");

		Dictionary<string, TimeSpan> allScores = new(){
			{"a", new TimeSpan(0, 2, 3)},
			{"d2", new TimeSpan(0, 1, 3)},
			{"d1", new TimeSpan(0, 1, 3)}, 
			{"d3", new TimeSpan(0, 1, 3)},
			{"b", new TimeSpan(0, 0, 3)},
			{"c", new TimeSpan(0, 3, 3)}
		};
		// allScores.Sort();
		// allScores = allScores.ToDictionary<string, TimeSpan>(key => key.Key);
		
		for (int i = 0; i < 4; i++) {
			leaderBoard[i] = GetNode("Control/VBoxContainer").GetChild<ScoreForPlayer>(i+1);
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// List<(TimeSpan, string)> allScores = new();
		if (GetUi.genRndMaze.ButtonPressed) {
			// foreach (var userForeach in users) {
			// 	string usernameHash = userForeach.Key;
			// 	UserData userData = userForeach.Value;
			// 	TimeSpan time = userData.FastestTimeRandSeed;
			// 	string username = login.GetUsernameFromHash(usernameHash);

			// 	allScores.Add((time, username));
				

			// }
			// allScores.Sort();
			Dictionary<string, TimeSpan> allScores2 = new(){
				{"a", new TimeSpan(0, 2, 3)},
				{"d2", new TimeSpan(0, 1, 3)},
				{"d1", new TimeSpan(0, 1, 3)}, 
				{"d3", new TimeSpan(0, 1, 3)},
				{"b", new TimeSpan(0, 0, 3)},
				{"c", new TimeSpan(0, 3, 3)}
			};
			IDictionary<string, TimeSpan> allScores = new Dictionary<string, TimeSpan>();
			foreach (KeyValuePair<string, UserData> userForEach in users) {
				// GD.Print("username start");
				// GD.Print(userForEach.Value.Username);
				// GD.Print("username end");
				allScores.Add(userForEach.Value.Username, userForEach.Value.FastestTimeRandSeed);
			}
			// allScores2.Add("a", new TimeSpan());
			// allScores.Sort();
			// allScores = allScores.ToDictionary<string, TimeSpan>(key => key.Key);
			var sorted = allScores.OrderBy(key => key.Value).ThenBy(key => key.Key);
			// sorted = sorted.OrderBy(key => key.Value);
			// allScores = allScores.OrderBy(key => key.Value);
			// GD.Print("abcd");
			// foreach (var score in sorted) {
			// 	GD.Print(score.Value.TotalSeconds);
			// 	GD.Print(score.Key);
			// 	GD.Print();
			// }
			int i = 0;
			foreach (var playerScore in sorted) {
				if (i > 3 || i == allScores2.Count) break;
				leaderBoard[i].SetScore(i + 1, playerScore.Key, playerScore.Value);
				i++;
			}

			if (user != null) {
				leaderBoard[3].SetScore(1, user.Username, user.GetTime);
			}

		} else {
			user?.Finish(new GameMode((int)GetUi.difficultyInput.Value, (int)GetUi.mazeSeed.Value));
		}
		// if (user != null) {
		// 	TimeSpan pagajusaisLaiks = DateTime.Now - user.Start;
		// 	string text = string.Format("{0:0.000}", pagajusaisLaiks.TotalSeconds);
		// }

	}
	// public void AddUserScore(string usernameHash) {
	// 	users.Add(usernameHash, new UserData());
	// }
	public void SetCurrentUser(string usernameHash, bool newUser) {
		// user?.Stop();
		if (newUser) users.Add(usernameHash, new UserData(login.GetUsernameFromHash(usernameHash)));
		user = users[usernameHash];
		// user.ContinueTime();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest) {
			user?.Stop();
			File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(users));
			GetTree().Quit(); // default behavior
		}
	}
	public void StartClock() {
		if (user != null) user.Start = DateTime.Now;
	}
	public void Finish() {
		GD.Print("finishe");
		if (GetUi.genRndMaze.ButtonPressed) {
			user?.Finish(CurrentGameMode);
		} else {
			user?.Finish(CurrentGameMode);
		}
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
	public GameMode CurrentGameMode = new();
}
