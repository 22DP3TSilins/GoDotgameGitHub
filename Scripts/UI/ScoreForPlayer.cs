using Godot;
using System;

public partial class ScoreForPlayer : AspectRatioContainer
{
	Label scoreLabel = null;
	Label placeLabel = null;
	Label usernameLabel = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("HBoxContainer/Score/Label");
		placeLabel = GetNode<Label>("HBoxContainer/Place/Label");
		usernameLabel = GetNode<Label>("HBoxContainer/Username/Label");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetScore(int place, string username, TimeSpan score) {
		scoreLabel.Text = string.Format("{0:0.000}", score.TotalSeconds);
		usernameLabel.Text = username;
		placeLabel.Text = string.Format("{0}.", place);
	}
	//public void ClearScore() {
		//scoreLabel.Text = "";
		//usernameLabel.Text = "";
		//placeLabel.Text = "";
	//}
}
