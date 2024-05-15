using Godot;
using System;
using System.Collections.Generic;

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
	public float X {get; set;}
	public float Y {get; set;}
	public float Z {get; set;}
	public bool Admin {get; set;}
	// Saglabā laiku
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
	public TimeSpan GetBestTime(GameMode gameMode) {
		return Scores.ContainsKey(gameMode) ? Scores[gameMode] : TimeSpan.Zero;
	}

	public DateTime StopTime {get; set;}

	// Izveido usera datus
	public UserData(string username, bool admin){
		Admin = admin;
		Scores = new Dictionary<GameMode, TimeSpan>();
		Username = username;
		ScoresStr = new Dictionary<string, TimeSpan>();

		// Defaulta iestatījumi
		Settings = new Dictionary<string, double>{
			{"Difficulty", 0.0},
			{"FOV", 90.0},
			{"cameraMaxDistance", 2.3},
			{"cameraOfset", 0.4},
			{"mazeSeed", 0.0},
			{"randMaze", 1.0}
		};
	}
	// Sāk laiku skaitīt no nulles
	public void RestartTime() {
		Start = DateTime.Now;
		Finished = false;
		WasJustStopped = false;
	}
	// Saglabā objektā pabeigšanas laikus
	public void Save(player Player) {
		ScoresStr.Clear();

		foreach (KeyValuePair<GameMode, TimeSpan> score in Scores) {
			ScoresStr.Add(score.Key.ToString(), score.Value);
		}
		SetPos(Player);
	}
	// Ielādē objektā pabeigšanas laikus
	public void Load() {
		Scores.Clear();
		foreach (KeyValuePair<string, TimeSpan> score in ScoresStr) {
			Scores.Add(new GameMode(score.Key), score.Value);
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
