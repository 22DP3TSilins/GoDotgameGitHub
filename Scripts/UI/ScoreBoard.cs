using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text.RegularExpressions;

public partial class ScoreBoard : CanvasLayer
{
	
	public UserData user;
	player Player = null;
	IDictionary<string, UserData> users;
	UI ui = null;
	CheckButton uiGenRndMaze = null;
	Login login = null;
	GameMode currentGameMode;
	List<ScoreForPlayer> leaderBoard = new();
	ScoreForPlayer scoreForPlayer = null;
	VBoxContainer playerScores = null;
	CanvasLayer map = null;
	LineEdit search = null;
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
		playerScores = GetNode<VBoxContainer>("Control/Panel/ScrollContainer/VBoxContainer");
		map = GetNode<CanvasLayer>("../Map");
		search = GetNode<LineEdit>("Control/Panel/LineEdit");

		// for (int i = 0; i < 4; i++) {
		// 	leaderBoard[i] = GetNode("Control/VBoxContainer").GetChild<ScoreForPlayer>(i+1);
		// }
		scoreForPlayer = GetNode<ScoreForPlayer>("Control/Panel/UserScore");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		
		if (user != null && sorted != null) {
			
			scoreForPlayer.SetScore(IofUser == -1 ? sorted.Count() + 1 : IofUser + 1, user.Username, user.GetTime);
		}
	}
	public void SetUser(string usernameHash, bool newUser, bool admin) {
		user?.Stop();
		ui.SaveSettings();
		user?.Save(Player);
		map.Visible = admin;
		if (newUser) users.Add(usernameHash, new UserData(login.GetUsernameFromHash(usernameHash), admin));
		user = users[usernameHash];

		ui.LoadSettings();
		user.Load();
		Player.Started = false;
		user.SetPos(Player);
		ui._gen_maze(false, user.Finished || newUser);
		user.Finished = false;
		
		// Player.Started = false;
	}
	// public void SetGameMode() {
	// 	currentGameMode = uiGenRndMaze.ButtonPressed ? new() : new((int)ui.difficultyInput.Value, (int)ui.mazeSeed.Value);
	// }

	public override void _Notification(int what)
	{
		// Saglabā lietotāju datus pirms iziešanas no programmas
		if (what == NotificationWMCloseRequest) {
			Stop();
			user?.Save(Player);
			ui.SaveSettings();
			
			File.WriteAllText("Data/Scores.json", JsonSerializer.Serialize(users));
			GetTree().Quit(); // default behavior
		}
	}
	public void StartClock() {
		if (user != null) user.Start = DateTime.Now;
	}
	public void Finish() {
		Player.Started = false;
		user?.Finish();
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
	private int IofUser = -1;
	IDictionary<string, TimeSpan> allScores = new Dictionary<string, TimeSpan>();
	IOrderedEnumerable<KeyValuePair<string, TimeSpan>> sorted = null;
	Regex UserSerchRegex = new("");
	public void ResetScoreBoard() {
		UserSerchRegex = new($"^{search.Text}");
		// Noņem visas vērtības no masīva
		allScores.Clear();
		foreach (KeyValuePair<string, UserData> userForEach in users) {

			// Iegūst labāko laiku pašreizējajā spēles režīmā
			TimeSpan userScore = userForEach.Value.GetBestTime(user?.CurrentGameMode);
			
			// Ja spēlētājam ir rezultāts pievieno to un asinhroni pieprasa tabulas rindu spēlētāja rezultāta parādīšanai
			if (userScore != TimeSpan.Zero) {
				ResourceLoader.LoadThreadedRequest("res://Scines/Levels/ScoreForPlayer.tscn");
				allScores.Add(userForEach.Value.Username, userScore);
			} 
		}
		
		// Noņem iepriekšējās rezultātu rindas no tabulas
		int i = 0;
		ScoreForPlayer a;
		while (true) {
			a = playerScores.GetChildOrNull<ScoreForPlayer>(i);
			if (a == null) break;
			a.QueueFree();
			i++;
		}
		
		// Sakārto rezultātus
		sorted = allScores.OrderBy(key => key.Value).ThenBy(key => key.Key);
		
		// Ievieto katra finišējušā spēlētāja datus
		IofUser = -1;
		PackedScene scoreForPlayerScene;
		ScoreForPlayer scoreForPlayerForEach;
		i = 0;
		if (user != null) {
			foreach (var playerScore in sorted) {
				
				// Pārbauda vai šis lietotājs ir pašreizējais lietotājs
				if (string.Compare(playerScore.Key, user.Username) == 0) IofUser = i;
				i++;

				// Izvēlas lietotājus, kuri ir meklēti
				if (!UserSerchRegex.Match(playerScore.Key).Success) continue;

				// Izveido jaunu objektu ar kāda lietotāja rezultātu
				scoreForPlayerScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/ScoreForPlayer.tscn");
				scoreForPlayerForEach = (ScoreForPlayer)scoreForPlayerScene.Instantiate();
				scoreForPlayerForEach._Ready();
				scoreForPlayerForEach.SetScore(i, playerScore.Key, playerScore.Value);
				playerScores.AddChild(scoreForPlayerForEach);

				
			}
		}
		
	}
	private void PlayerSearched(string new_text)
	{
		ResetScoreBoard();
		// Replace with function body.
	}
}
