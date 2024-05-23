using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Dynamic;
using System.Text.RegularExpressions;

public partial class Login : Control
{
	// Called when the node enters the scene tree for the first time.
	static internal byte[] peper = new byte[] {4, 7, 234, 78};
	
	
	IDictionary<string, User> users = new Dictionary<string, User>();
	internal protected string usersSerialised;
	LineEdit usernameNode = null;
	LineEdit passwordNode = null;
	Label isValidNode = null;
	ScoreBoard scores = null;
	User user1;
	public string currentUserHash = "";
	static readonly HMACSHA512 hmacsha512 = new(new byte[]{2, 4, 5, 26});
	public override void _Ready()
	{
		usernameNode = GetNode<LineEdit>("Panel/VBoxContainer/Username/LineEdit");
		passwordNode = GetNode<LineEdit>("Panel/VBoxContainer/Password/LineEdit");
		isValidNode = GetNode<Label>("Panel/VBoxContainer/IsValid/Label");
		scores = GetParent().GetParent().GetNode<ScoreBoard>("Scores");
		
		string jsonData = File.ReadAllText("Data/Users.json");
		users = JsonSerializer.Deserialize<IDictionary<string, User>>(jsonData);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void _log_in()
	{
		// Replace with function body.
		byte[] bytesUsername = new byte[20];
		if (usernameNode.Text.Length > 20) {
			isValidNode.Text = "Username and/or password is incorrect!";
			isValidNode.Show();
			return;
		}
		byte[] username = Encoding.UTF8.GetBytes(usernameNode.Text);
		Buffer.BlockCopy(username, 0, bytesUsername, 0, username.Length);
		byte[] usernameHashed = hmacsha512.ComputeHash(bytesUsername);

		byte[] bytesPassword = Encoding.UTF8.GetBytes(passwordNode.Text);
		byte[] hashPassword = hmacsha512.ComputeHash(bytesPassword);
		byte[] pepperedPassword = new byte[72];

		Buffer.BlockCopy(hashPassword, 0, pepperedPassword, 0, 64);
		Buffer.BlockCopy(peper, 0, pepperedPassword, 68, 4);
		GD.Print(users, "users");
		if (users.ContainsKey(usernameHashed.HexEncode())) {
			if (users[usernameHashed.HexEncode()].PasswordEqual(pepperedPassword)) {
				GetParent<CanvasLayer>().Hide();
				scores.Show();
				scores.SetUser(usernameHashed.HexEncode(), false, usernameNode.Text.IndexOf('#') == 0);
				Input.MouseMode = Input.MouseModeEnum.Captured;
				currentUserHash = usernameHashed.HexEncode();

				return;
			}
		}

		isValidNode.Text = "Username and/or password is incorrect!";
		isValidNode.Show();
		
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void _sign_in()
	{
		// Replace with function body.
		string passwordResult = CheckPassword(passwordNode.Text);
		string usernameResult = CheckUsername(usernameNode.Text);

		if (passwordResult != null) {
			isValidNode.Text = passwordResult;
			isValidNode.Show();
			return;
		}

		if (usernameResult != null) {
			isValidNode.Text = usernameResult;
			isValidNode.Show();
			return;
		}
		byte[] bytesUsername = new byte[20];
		byte[] username = Encoding.UTF8.GetBytes(usernameNode.Text);
		Buffer.BlockCopy(username, 0, bytesUsername, 0, username.Length);
		byte[] usernameHashed = hmacsha512.ComputeHash(bytesUsername);

		if (users.ContainsKey(usernameHashed.HexEncode())) {
			isValidNode.Text = "Username already exists!";
			isValidNode.Show();
			return;
		}
		
		users.Add(usernameHashed.HexEncode(), new User(usernameNode.Text, passwordNode.Text));
		GetParent<CanvasLayer>().Hide();
		scores.Show();
		scores.SetUser(usernameHashed.HexEncode(), true, usernameNode.Text.IndexOf('#') == 0);
		Input.MouseMode = Input.MouseModeEnum.Captured;
		currentUserHash = usernameHashed.HexEncode();
	
	}
	public override void _Notification(int what) {
		if (what == NotificationWMCloseRequest) {
			usersSerialised = JsonSerializer.Serialize(users);
			File.WriteAllText("Data/Users.json", usersSerialised);
			GetTree().Quit(); // default behavior
		}
	}

	public string GetUsernameFromHash(string usernameHashed) {
		if (users.ContainsKey(usernameHashed)) {
			
			string usernemeWwithNull = Encoding.UTF8.GetString(users[usernameHashed].Username);
			return usernemeWwithNull[..usernemeWwithNull.Find('\x00')];
		}
		return null;
	}
	IDictionary<Regex, string> tests = new Dictionary<Regex, string>(){
			{new Regex(@"[[\]~`!@#$%^&*(){}=\-+\\*\.,<>;:'" + '"' + "|/?]"), "Password must have special characters!"},
			{new Regex(@"[0-9]"), "Password must have numbers!"},
			{new Regex(@"[A-Za-z_]"), "Password must have letters!"},
			{new Regex(@"^(\w|[$[[\]~`!@#$%^&*(){}=\-+\\*\.,<>;:'" + '"' + "|/?])+$"), "Password must not have unreconised symbols!"}
		};
	string CheckPassword(string password) {
		if (password.Length < 8) return "Password is too short! Minimum is 8 characters!";
		
		foreach (KeyValuePair<Regex, string> test in tests) {
			if (!test.Key.Match(password).Success) return test.Value;
		}
		
		return null;
	}

	static string CheckUsername(string username) {
		Regex onlySpecifiedCharsCheck = new(@"^(\w+|[#]\w+)$");

		if (username.Length > 20) return "Username is too long! Maximum length is 20 characters!";
		if (!onlySpecifiedCharsCheck.Match(username).Success) return "Username must have only letters or numbers or \"_\"!";
		
		return null;
	}
	public void ClearInputs() {
		usernameNode.Text = "";
		passwordNode.Text = "";

		scores.Hide();
		isValidNode.Hide();
	}
	public void DeleteAccount() {
		ClearInputs();

		if (users.ContainsKey(currentUserHash)) users.Remove(currentUserHash);
	}
}
