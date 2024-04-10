using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.ComponentModel.DataAnnotations;
class UserData {
		public IDictionary<(int difficulty, int seed), TimeSpan> Scores {get; set;}
		public TimeSpan FastestTimeRandSeed {get; set;}
		public DateTime Start {get; set;}
		bool WasJustStopped {get; set;} = false;
		public void Finish((int difficulty, int seed) GameMode) {
			TimeSpan time = DateTime.Now - Start;
			if (GameMode == (-1, -1)) {
				FastestTimeRandSeed = time < FastestTimeRandSeed ? time : FastestTimeRandSeed;
			} else {
				Scores[GameMode] = time < Scores[GameMode] ? time : Scores[GameMode];
			}
		}
		public void Stop(){
			StopTime = DateTime.Now;
			WasJustStopped = true;
		}
		public void ContinueTime() {
			if (WasJustStopped) {
				Start += DateTime.Now - StopTime;
			} else {
				WasJustStopped = false;
			}
		}
		public DateTime StopTime {get; set;}

		// public void UpdateScores(TimeSpan time) {
		// 	FastestTimeRandSeed = time < FastestTimeRandSeed ? time : FastestTimeRandSeed;
		// }
		// public void UpdateScores(TimeSpan time, (int difficulty, int seed) {
		// 	Scores[(difficulty, seed)] = time < Scores[(difficulty, seed)] ? time : Scores[(difficulty, seed)];
		// }

		public UserData(){
			FastestTimeRandSeed = TimeSpan.MaxValue;
			Scores = new Dictionary<(int difficulty, int seed), TimeSpan>();
		}
	}
public partial class ScoreBoard : CanvasLayer
{
	
	UserData user;
	IDictionary<string, UserData> users;
	ui GetUi = null;
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
		GetParent().GetNode<CanvasLayer>("Login").Show();
		GetUi = (ui)GetNode<Node>("../UI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	// public void AddUserScore(string usernameHash) {
	// 	users.Add(usernameHash, new UserData());
	// }
	public void SetCurrentUser(string usernameHash, bool newUser) {
		if (newUser) users.Add(usernameHash, new UserData());
		user = users[usernameHash];
	}

	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest) {
			File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(users));
			GetTree().Quit(); // default behavior
		}
	}
	public void StartClock() {
		if (user != null) user.Start = DateTime.Now;
	}
	public void Finish() {
		if (GetUi.genRndMaze.ButtonPressed) {
			user?.Finish((-1, -1));
		} else {
			user?.Finish(((int)GetUi.difficultyInput.Value, (int)GetUi.mazeSeed.Value));
		}
	}
	public void Stop() {
		user?.Stop();
	}
	public void Continue() {
		user?.ContinueTime();
	}
}
