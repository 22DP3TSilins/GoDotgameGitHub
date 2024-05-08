using Godot;
using System;
using System.Collections.Generic;


public class UserData {
	public IDictionary<GameMode, TimeSpan> Scores = new Dictionary<GameMode, TimeSpan>();
	public IDictionary<string, TimeSpan> ScoresStr { get; set; }
	public DateTime Start {get; set;} = DateTime.Now;
	public bool WasJustStopped {get; set;} = false;
	public string Username {get; set;}
	public IDictionary<string, double> Settings {get; set;}
	public TimeSpan TimeFinished {get; set;} = TimeSpan.Zero;
	public float X {get; set;}
	public float Y {get; set;}
	public float Z {get; set;}

	public bool Admin {get; set;}
	// public GameMode currentGameMode = new();
	// public string CurrentGameMode {
	// get {
	// 	return currentGameMode.ToString();
	// } set {
	// 	currentGameMode = new GameMode(value);
	// }}
	public void Finish(GameMode gameMode) {
		if (Admin) return;

		Finished = true;
		TimeFinished = DateTime.Now - Start;
		Stop();
		if (Scores.ContainsKey(gameMode)) {
			Scores[gameMode] = TimeFinished < Scores[gameMode] ? TimeFinished : Scores[gameMode];
		} else {
			Scores[gameMode] = TimeFinished;
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
		if (Finished) return TimeFinished;
		return DateTime.Now - (Start + (WasJustStopped ? DateTime.Now - StopTime : TimeSpan.Zero)); } }
	public TimeSpan GetBestTime(GameMode gameMode) {
		// foreach (var score in Scores) {
		// 	GD.Print($"key {score.Key} score={score.Value}");
		// }
		// GD.Print(Scores.Count);

		return Scores.ContainsKey(gameMode) ? Scores[gameMode] : TimeSpan.Zero;
	}

	public DateTime StopTime {get; set;}

	// public void UpdateScores(TimeSpan time) {
	// 	FastestTimeRandSeed = time < FastestTimeRandSeed ? time : FastestTimeRandSeed;
	// }
	// public void UpdateScores(TimeSpan time, (int difficulty, int seed) {
	// 	Scores[(difficulty, seed)] = time < Scores[(difficulty, seed)] ? time : Scores[(difficulty, seed)];
	// }

	public UserData(string username, bool admin){
		Scores = new Dictionary<GameMode, TimeSpan>();
		Username = username;
		// foreach (var gameModeScore in ScoresStr) {
		// 	string[] mode = gameModeScore.Key.Split(':');
		// 	int difficulty = int.Parse(mode[0]);
		// 	int seed = int.Parse(mode[1]);
		// 	ScoresStr.Add(new GameMode(difficulty,seed).ToString(), gameModeScore.Value);
		// }
		ScoresStr = new Dictionary<string, TimeSpan>();
		Settings = new Dictionary<string, double>{
			{"Difficulty", 0.0},
			{"FOV", 90.0},
			{"cameraMaxDistance", 2.3},
			{"cameraOfset", 0.4},
			{"mazeSeed", 0.0},
			{"randMaze", 1.0}
		};
		Admin = admin;
		// X = Player.Position.X;
		// Y = Player.Position.Y;
		// Z = Player.Position.Z;
	}
	public void RestartTime() {
		Start = DateTime.Now;
		Finished = false;
		WasJustStopped = false;
	}

	public void Save(player Player) {
		ScoresStr.Clear();

		foreach (KeyValuePair<GameMode, TimeSpan> score in Scores) {
			ScoresStr.Add(score.Key.ToString(), score.Value);
		}
		X = Player.Position.X;
		Y = Player.Position.Y;
		Z = Player.Position.Z;
	}
	public void Load(player Player) {
		Scores.Clear();
		foreach (KeyValuePair<string, TimeSpan> score in ScoresStr) {
			Scores.Add(new GameMode(score.Key), score.Value);
		}
		Player.Position = new Vector3(X, Y, Z);
	}
	public void SetPos(player Player) {
		X = Player.Position.X;
		Y = Player.Position.Y;
		Z = Player.Position.Z;
	}
}
