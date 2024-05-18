using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

// Klase, kas saglabā datus par lietotāju (iestatījumus, pozīciju, rezultātus)
// Klasē īpašumus izmantoju mainīgajiem, kurus vajag saglabāt JSON failā, 
// vai papildu funkcionalitāti vērtību iegūšanai un saglabāšanai mainīgajos
// laukus izmantoju aprēķiniem klasē
public class UserData {
	public IDictionary<GameMode, TimeSpan> Scores = new Dictionary<GameMode, TimeSpan>();
	public IDictionary<string, TimeSpan> ScoresStr { get; set; }
	public DateTime Start {get; set;} = DateTime.Now;
	public bool WasJustStopped {get; set;} = false;
	public string Username {get; set;}
	public IDictionary<string, double> Settings {get; set;}
	public TimeSpan TimeFinished {get; set;} = TimeSpan.Zero;
	public GameMode CurrentGameMode {get; set;}
	public float X {get; set;}
	public float Y {get; set;}
	public float Z {get; set;}
	public bool Admin {get; set;}
	// Saglabā laiku
	// public void Finish(GameMode gameMode) {
	// 	if (Admin) return;

	// 	Finished = true;
	// 	TimeFinished = DateTime.Now - Start;
	// 	Stop();
	// 	if (Scores.ContainsKey(gameMode)) {
	// 		Scores[gameMode] = TimeFinished < Scores[gameMode] ? TimeFinished : Scores[gameMode];
	// 	} else {
	// 		Scores[gameMode] = TimeFinished;
	// 	}
	// }
	public void Finish() {
		if (Admin) return;

		Finished = true;
		TimeFinished = DateTime.Now - Start;
		Stop();
		if (Scores.ContainsKey(CurrentGameMode)) {
			Scores[CurrentGameMode] = TimeFinished < Scores[CurrentGameMode] ? TimeFinished : Scores[CurrentGameMode];
		} else {
			Scores[CurrentGameMode] = TimeFinished;
		}
	}
	public bool Finished {get; set;} = false;
	// Uzliek pauzi
	public void Stop(){
		if (!WasJustStopped) StopTime = DateTime.Now;
		WasJustStopped = true;
	}
	// Turpinaskaitīt laiku pēc pauzes
	public void ContinueTime() {
		if (WasJustStopped && !Finished) Start += DateTime.Now - StopTime;
		WasJustStopped = false;
	}
	// Pašreizējais laiks
	public TimeSpan GetTime { 
		get { 
			if (Finished) return TimeFinished;
			return DateTime.Now - (Start + (WasJustStopped ? DateTime.Now - StopTime : TimeSpan.Zero));
		}
	}
	// Atgriež labāko laiku tajā spēles režīmā
	public TimeSpan GetBestTime(GameMode? gameMode) {
		if (gameMode == null) return TimeSpan.Zero;
		return Scores.ContainsKey((GameMode) gameMode) ? Scores[(GameMode) gameMode] : TimeSpan.Zero;
	}

	public DateTime StopTime {get; set;}
	public static Dictionary<string, double> DefaultSettings = new() {
		{"Difficulty", 8.0},
		{"FOV", 90.0},
		{"cameraMaxDistance", 2.3},
		{"cameraOfset", 0.4},
		{"mazeSeed", 0.0},
		{"randMaze", 1.0}
	};

	// Izveido usera datus
	public UserData(string username, bool admin){
		Admin = admin;
		Scores = new Dictionary<GameMode, TimeSpan>();

		// GameMode gameMode1 = new GameMode(5, 34, true);
		// GameMode gameMode2 = new GameMode(5, 78, true);

		// Scores.Add(gameMode1, new TimeSpan(4, 5, 6));
		// Scores.Add(gameMode2, new TimeSpan(0, 5, 6));

		// bool found1 = Scores.ContainsKey(gameMode1);
		// bool found2 = Scores.ContainsKey(gameMode2);

		// GD.Print(found1, found2);
		// GD.Print(gameMode1.GetHashCode());
		// GD.Print(gameMode2.GetHashCode());
		// GD.Print(gameMode1.Equals(gameMode2));

		// Scores = new Dictionary<GameMode, TimeSpan>();

		Username = username;
		ScoresStr = new Dictionary<string, TimeSpan>();

		// Defaulta iestatījumi
		Settings = DefaultSettings;

		// Defoulta gameMode ir izveidots no defoulta grūtības pakāpes
		CurrentGameMode = new((int)Settings["Difficulty"], -1, true);

	}
	// Sāk laiku skaitīt no nulles
	public void RestartTime() {
		Start = DateTime.Now;
		Finished = false;
		WasJustStopped = false;
	}
	// Saglabā objektā pabeigšanas laikus
	public void Save(player Player) {
		GD.Print("Save");
		ScoresStr.Clear();
		
		int i = 0;
		foreach (KeyValuePair<GameMode, TimeSpan> score in Scores) {
			GD.Print($"i: {i}\ngm: {score.Key}\nscore: {score.Value}\nHash: {score.Key.GetHashCode()}\nHashStr: {score.Key.ToString().GetHashCode()}");
			i++;
			ScoresStr.Add(score.Key.ToString(), score.Value);
		}
		SetPos(Player);
	}
	// Ielādē objektā pabeigšanas laikus
	public void Load() {
		GD.Print("Load");
		Scores.Clear();
		int i = 0;
		foreach (KeyValuePair<string, TimeSpan> score in ScoresStr) {
			GD.Print($"i: {i}\ngm: {score.Key}\nscore: {score.Value}\nHash: {score.Key.GetHashCode()}\nHashStr: {score.Key.ToString().GetHashCode()}");
			
			// Izdzēš dublikātus
			GameMode gameMode = new GameMode(score.Key);
			if (Scores.ContainsKey(gameMode)) {
				Scores[gameMode] = score.Value > Scores[gameMode] ? score.Value : Scores[gameMode];
			} else {
				Scores.Add(new GameMode(score.Key), score.Value);
			}
			
			i++;
		}
		// Player.Position = new Vector3(X, Y, Z);
	}
	// Saglabā spēlētāja pozīciju
	public void SetPos(player Player) {
		X = Player.Position.X;
		Y = Player.Position.Y;
		Z = Player.Position.Z;
	}

	public void GoTo(player Player) {
		Player.Position = new Vector3(X, Y, Z);
	}
}
