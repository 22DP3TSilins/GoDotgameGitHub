using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

public partial class Login : Control
{
	// Called when the node enters the scene tree for the first time.
	static internal byte[] peper = new byte[] {4, 7, 234, 78};
	// internal 
	internal protected struct User {
		public byte[] Username; 
		private byte[] EncPassword;
		private byte[] Salt;
		public byte[] userDataEncrypted {
			get {
				byte[] bytes = new byte[132];
				Buffer.BlockCopy(Username, 0, bytes, 0, 64);
				Buffer.BlockCopy(EncPassword, 0, bytes, 64, 64);
				Buffer.BlockCopy(Salt, 0, bytes, 128, 4);
				return bytes;
			}
			set {
				Username =  new byte[64];
				EncPassword = new byte[64];
				Salt = new byte[4];

				Buffer.BlockCopy(value, 0, Username, 0, 64);
				Buffer.BlockCopy(value, 64, EncPassword, 0, 64);
				Buffer.BlockCopy(value, 128, Salt, 0, 4);
			}}
		
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public readonly bool PasswordEqual(byte[] bytesPasswordPepered) {
			byte[] encPassword = new byte[72];

			Buffer.BlockCopy(bytesPasswordPepered, 0, encPassword, 0, 72);
			Buffer.BlockCopy(Salt, 0, encPassword, 64, 4);

			return CryptographicOperations.FixedTimeEquals(EncPassword, hMACSHA512.ComputeHash(encPassword));
		}
		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public User(string username, string userPassword)
		{
			byte[] bytes = new byte[4];
			while (true) {
				try {
					using (var rng = new RNGCryptoServiceProvider()) {
						rng.GetBytes(bytes);
					}
					break;
				} catch (CryptographicException) {};
			}
			
			Salt = bytes;

			byte[] bytes1Str = new byte[64];
			byte[] usernameBytes = Encoding.UTF8.GetBytes(username);
			Buffer.BlockCopy(usernameBytes, 0, bytes1Str, 0, usernameBytes.Length);
			Username = bytes1Str;

			byte[] bytesPassword = Encoding.UTF8.GetBytes(userPassword);
			byte[] hashPassword = hMACSHA512.ComputeHash(bytesPassword);
			byte[] encPassword = new byte[72];

			Buffer.BlockCopy(hashPassword, 0, encPassword, 0, 64);
			Buffer.BlockCopy(Salt, 0, encPassword, 64, 4);
			Buffer.BlockCopy(peper, 0, encPassword, 68, 4);

			EncPassword = hMACSHA512.ComputeHash(encPassword);
			GD.Print(EncPassword.Length);
			byte[] a = new byte[4];
		}
	}
	// public readonly struct A {
	// 	public readonly byte[] bytes1 = new byte[]{0, 2, 7};
	// 	public A(byte[] bytes) {
	// 		bytes1 = bytes;
	// 	}
	// }
	// public record ABCD {public string Name { get;} public int MyInt {get;}
	// public ABCD(string name, int myInt) {
	// 	Name = name;
	// 	MyInt = myInt;
	// }};
	internal protected IDictionary<string, User> users = new Dictionary<string, User>();
	internal protected string usersSerialised;
	LineEdit usernameNode = null;
	LineEdit passwordNode = null;
	Label isValidNode = null;
	ScoreBoard scores = null;
	internal protected User user1;
	static readonly HMACSHA512 hMACSHA512 = new(new byte[]{2, 4, 5, 26});
	public override void _Ready()
	{
		usernameNode = GetNode<LineEdit>("Panel/VBoxContainer/Username/LineEdit");
		passwordNode = GetNode<LineEdit>("Panel/VBoxContainer/Password/LineEdit");
		isValidNode = GetNode<Label>("Panel/VBoxContainer/IsValid/Label");
		scores = GetParent().GetParent().GetNode<ScoreBoard>("Scores");

		
		// byte[] bytesUsername = new byte[64];
		// string username1 = "marcipans2355";
		// byte[] username = Encoding.UTF8.GetBytes(username1);
		// Buffer.BlockCopy(username, 0, bytesUsername, 0, username.Length);
		// users.Add(hMACSHA512.ComputeHash(bytesUsername).HexEncode(), new User(username1, "1234abcd"));

		// byte[] bytesUsername2 = new byte[64];
		// string username2 = "a";
		// byte[] username3 = Encoding.UTF8.GetBytes(username2);
		// Buffer.BlockCopy(username3, 0, bytesUsername2, 0, username2.Length);
		// users.Add(hMACSHA512.ComputeHash(bytesUsername).HexEncode(), new User(username2, "b"));

		// usersSerialised = JsonSerializer.Serialize(users);
		// GD.Print(usersSerialised);
		// File.WriteAllText("Data/Users.json", usersSerialised);

		// usersSerialised = JsonSerializer.Serialize(new ABCD("abc", 8));
		// GD.Print(usersSerialised);
		// File.WriteAllText("Data/Users.json", usersSerialised);
		// GD.Print("AAAAAAreadAAAA");
		string jsonData = File.ReadAllText("Data/Users.json");
		// GD.Print(jsonData);
		// GD.Print("AAAAAAreadAAAA");
		// string jsonData = File.ReadAllText("res://Users");
		// try {
		users = JsonSerializer.Deserialize<IDictionary<string, User>>(jsonData);
		// GD.Print(users.Count);
		// } catch (Exception) {
		// 	GD.Print("whhy");
		// }
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void _log_in()
	{
		// Replace with function body.
		GD.Print("Login Start:");
		byte[] bytesUsername = new byte[64];
		byte[] username = Encoding.UTF8.GetBytes(usernameNode.Text);
		Buffer.BlockCopy(username, 0, bytesUsername, 0, username.Length);
		byte[] usernameHashed = hMACSHA512.ComputeHash(bytesUsername);

		byte[] bytesPassword = Encoding.UTF8.GetBytes(passwordNode.Text);
		byte[] hashPassword = hMACSHA512.ComputeHash(bytesPassword);
		byte[] pepperedPassword = new byte[72];

		Buffer.BlockCopy(hashPassword, 0, pepperedPassword, 0, 64);
		Buffer.BlockCopy(peper, 0, pepperedPassword, 68, 4);

		if (users.ContainsKey(usernameHashed.HexEncode())) {
			if (users[usernameHashed.HexEncode()].PasswordEqual(pepperedPassword)) {
				GetParent<CanvasLayer>().Hide();
				scores.Show();
				scores.SetCurrentUser(usernameHashed.HexEncode(), false);
				GD.Print("Login End success:");

				return;
			}
		}

		isValidNode.Text = "Username and/or password is incorrect!";
		isValidNode.Show();
		GD.Print("Login End fail:");
		
	}

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	private void _sign_in()
	{
		// Replace with function body.
		byte[] bytesUsername = new byte[64];
		byte[] username = Encoding.UTF8.GetBytes(usernameNode.Text);
		Buffer.BlockCopy(username, 0, bytesUsername, 0, username.Length);
		byte[] usernameHashed = hMACSHA512.ComputeHash(bytesUsername);

		if (users.ContainsKey(usernameHashed.HexEncode())) {
			isValidNode.Text = "Username already exists!";
			isValidNode.Show();
			GD.Print("fail");
			
		} else {
			users.Add(usernameHashed.HexEncode(), new User(usernameNode.Text, passwordNode.Text));
			GetParent<CanvasLayer>().Hide();
			scores.Show();
			// scores.AddUserScore(usernameHashed.HexEncode());
			scores.SetCurrentUser(usernameHashed.HexEncode(), true);
			GD.Print("success");
		}
	}
	public override void _Notification(int what)
	{
		if (what == NotificationWMCloseRequest) {
			usersSerialised = JsonSerializer.Serialize(users);
			File.WriteAllText("Data/Users.json", usersSerialised);
			GetTree().Quit(); // default behavior
		}
	}
}



