using Godot;
using System;

public partial class playerScript2 : RigidBody3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		double leftRight = Input.get_axis("Move_left", "move_right");
		double forwardBacward = Input.get_axis("Move_forward", "move_backward");
	}
}
