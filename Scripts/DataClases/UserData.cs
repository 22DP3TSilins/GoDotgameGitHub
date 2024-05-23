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
	// public Vector3 Pos {get; set;}
	// public Vector3 Rot {get; set;}
	public float PosX {get; set;}
	public float PosY {get; set;}
	public float PosZ {get; set;}
	public float RotX {get; set;}
	public float RotY {get; set;}
	public float RotZ {get; set;}

	public bool Admin {get; set;}
	public Vector3 vector3 {get; set;} = new Vector3(2,4,5);
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
		ScoresStr.Clear();
		
		int i = 0;
		foreach (KeyValuePair<GameMode, TimeSpan> score in Scores) {
			i++;
			ScoresStr.Add(score.Key.ToString(), score.Value);
		}
		SavePos(Player);
	}

	// Ielādē objektā pabeigšanas laikus
	public void Load() {
		Scores.Clear();
		int i = 0;
		foreach (KeyValuePair<string, TimeSpan> score in ScoresStr) {
			
			// Izdzēš dublikātus
			GameMode gameMode = new GameMode(score.Key);
			if (Scores.ContainsKey(gameMode)) {
				Scores[gameMode] = score.Value > Scores[gameMode] ? score.Value : Scores[gameMode];
			} else {
				Scores.Add(new GameMode(score.Key), score.Value);
			}
			
			i++;
		}
	}
	// Saglabā spēlētāja pozīciju objektā
	public void SavePos(player Player) {
		// Pos = Player.Position;
		// Rot = Player.Rotation;
		PosX = Player.Position.X;
		PosY = Player.Position.Y;
		PosZ = Player.Position.Z;
		RotX = Player.Rotation.X;
		RotY = Player.Rotation.Y;
		RotZ = Player.Rotation.Z;
	}

	// Novieto spēlētāju koordinātu vietā
	public void SetPos(player Player) {
	// 	Player.Position = Pos;
	// 	Player.Rotation = Rot;
		Player.Position = new Vector3(PosX, PosY, PosZ);
		Player.Rotation = new Vector3(RotX, RotY, RotZ);
	}
}
