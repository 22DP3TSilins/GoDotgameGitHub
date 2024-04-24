using Godot;
using System;

public partial class Finish : Node3D
{
	ScoreBoard scoreBoard = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Area3D>("Mesh/Area3D").Monitoring = true;
		scoreBoard = GetNode<ScoreBoard>("../../Scores");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	private void _on_area_3d_body_entered(Node3D body)
	{
		// Replace with function body.
		GD.Print("Entered1");
		GD.Print(body.Position.X);
		scoreBoard.Finish();
	}

	private void _on_area_3d_body_shape_entered(Rid body_rid, Node3D body, long body_shape_index, long local_shape_index)
	{
		// Replace with function body.
		GD.Print("Entered2");
		GD.Print(body.Position.X);
	}

}




