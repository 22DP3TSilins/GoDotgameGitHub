using Godot;
using System;
using System.Threading;

public partial class AssynhronousLoading : Node3D
{	Node3D meshes = null;
	Label firstPlace = null;
	public override void _Ready() {
		
	}
	// int framesPassed = 0;
	// int objCreated = 0;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// Label score;
	public override void _Process(double delta)
	{
		
		//framesPassed++;
		//if (framesPassed > 100) {
			//framesPassed = 0;
			//objCreated++;
			//PackedScene testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Blender/Meshes/Labyrinth/testMesh.blend");
			//Node3D testMesh = (Node3D)testScene.Instantiate();
			//meshes.AddChild(testMesh);
			//testMesh.Position = new Vector3(3.0f + objCreated * 5.0f, 0.0f, 0.0f);
			//GD.Print("Loading...");
//
			//ResourceLoader.LoadThreadedRequest("res://Blender/Meshes/Labyrinth/testMesh.blend");
		//}
	}
	public void ShowGame(){

	}
}
