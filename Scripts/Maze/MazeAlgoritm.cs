using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class MazeAlgoritm : Node3D
{
	// Called when the node enters the scene tree for the first time.
	Node3D meshes = null;
	public override void _Ready()
	{
		GD.Print("Maze is cookin...");
		meshes = GetParent<Node3D>().GetNode<Node3D>("Meshes");
		byte[,] walls = genRandWalls();
		AssyncLoadMaze(walls);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AssyncLoadMaze(byte[,] walls) {
		// PackedScene testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallx.tscn");
		// List<PackedScene> wallObjArr = new List<PackedScene>();
		
		// int wallsx = 0;
		// int wallsz = 0;

		for (int i = 0; i < 11; i++) {
			for (int j = 0; j < 11; j++) {
				if ((walls[i, j] & 1) == 0) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallx.tscn");
				}
				if ((walls[i, j] & 2) == 0) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallz.tscn");
				}
			}
		}
		PackedScene testScene = null;
		for (int i = 0; i < 11; i++) {
			for (int j = 0; j < 11; j++) {
				if ((walls[i, j] & 1) == 0) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallx.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(3.0f + i * 4.0f, 0.0f, 3.0f + j * 4.0f);
				}
				if ((walls[i, j] & 2) == 0) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallz.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(5.0f + i * 4.0f, 0.0f, 5.0f + j * 4.0f);
				}
				

			}
		}
		
		// Node3D testMesh = (Node3D)testScene.Instantiate();
		// meshes.AddChild(testMesh);
		// testMesh.Position = new Vector3(3.0f + objCreated * 5.0f, 0.0f, 0.0f);
		GD.Print("Loading... labyrinth");

		ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallx.tscn");
	}

	public byte[,] genRandWalls() {
		const int sizex = 10;
		const int sizey = 10;
		
		const int startx = 0;
		const int starty = 0;

		byte[,] walls = new byte[sizex + 1, sizey + 1];

		Random rand = new Random();

		for (int i = 0; i <= sizex; i++) {
			for (int j = 0; j <= sizey; j++) {
				walls[i, j] = (byte)rand.Next(0, 3);
			}
		}
		return walls;
	}

	public byte[,] genMazeWalls() {
		Random rnd = new Random();

		const int sizex = 10;
		const int sizey = 10;
		
		const int startx = 0;
		const int starty = 0;

		int currentx = startx;
		int currenty = starty;

		int currentx2;
		int currenty2;

		byte[,] walls = new byte[sizex + 1, sizey + 1]; // 1 Up 2 Left 4 Visited 8 Explored compleatly 16 Up maze wall 32 Left maze wall;

		for (int i = 0; i < sizex + 1; i++) {
			walls[i, 0] = 16;
		}
		for (int i = 0; i < sizey + 1; i++) {
			walls[0, i] = 32;
		}
		walls[0, 0] = 16 + 32;

		int[] cellWalls = new int[4];
		int j = 0;
		bool canExit = false;

		// byte currentWall = 0;

		// byte currentWallxp = 0;
		// byte currentWallxm = 0;

		// byte currentWallyp = 0;
		// byte currentWallym = 0;

		for (int l = 0; l < 15; l++)
		{
			string row = "";
			string newNum = "";
			for (int i = 0; i <= sizex; i++) {
				row = "";
				for (int k = 0; k <= sizey; k++) {
					newNum = " " + +walls[i, k];
					row += newNum.Length == 2 ? " " + newNum : newNum;
				}
				GD.Print(row + "\n");
			}
			if (currentx == startx && currenty == starty && canExit) {
				return walls;
			}

			ref byte currentWall = ref walls[currentx, currenty];

			if (currentx < sizex) {
				ref byte currentWallxp = ref walls[currentx + 1, currenty];
				if (((currentWallxp & 1) == 1) && ((currentWallxp & 4) == 0)) {
					cellWalls[j] = 3;
					j++;
				}
			}
			if (currenty < sizey) {
				ref byte currentWallyp = ref walls[currentx, currenty + 1];
				if (((currentWall & 1) == 1) && ((currentWallyp & 4) == 0)) {
					cellWalls[j] = 1;
					j++;
				}
			}
			if (currentx > 1) {
				ref byte currentWallxm = ref walls[currentx - 1, currenty];
				if (((currentWall & 2) == 2) && ((currentWallxm & 4) == 0)) {
					cellWalls[j] = 2;
					j++;
				}
			}
			if (currenty > 1) {
				ref byte currentWallyp = ref walls[currentx, currenty + 1];
				if (((currentWallyp & 2) == 2) && ((currentWallyp & 4) == 0)) {
					cellWalls[j] = 4;
					j++;
				}
			}

			GD.Print("\n");
			j = 0;

			// cellWalls = new int[4] {0b11, 0b11, 0b11, 0b11};
			// for (int i = 1; i < 16; i++) {
			// 	if ((walls[currentx, currenty] & (1 << i)) > 0) {
			// 		cellWalls[i] = 1 << i;
			// 		j++;
			// 	}
			// }
			// if (currentx > 0) {
			// 	if (((currentWall & 1) == 1) && ((currentWallxm & 4) == 0)) {
			// 		cellWalls[j] = 1;
			// 		j++;
			// 	}
			// }
			// if (currenty < sizey) {
			// 	if (((currentWall & 2) == 2) && ((currentWallym & 4) == 0)) {
			// 		cellWalls[j] = 2;
			// 		j++;
			// 	}
			// }
			// if (currentx < sizex) {
			// 	if (((currentWallxp & 1) == 1) && ((currentWallxp & 4) == 0)) {
			// 		cellWalls[j] = 3;
			// 		j++;
			// 	}
			// }
			// if (currenty > 0) {
			// 	if (((currentWallym & 2) == 2) && ((currentWallym & 4) == 0)) {
			// 		cellWalls[j] = 4;
			// 		j++;
			// 	}
			// }
			// if (j > 1) {
			// 	lastUnExploredX = currentx;
			// 	lastUnExploredY = currenty;
			// } else 
			if (j == 0) {
				
				
				walls[currentx, currenty] = (byte)(walls[currentx, currenty] | 8);

				if (currentx > 1) {
					ref byte currentWallxm = ref walls[currentx - 1, currenty];
					if (((currentWall & 1) == 0) && ((currentWallxm & 8) == 0)) {
						currentx--;
						continue;
					}
				}
				if (currenty < sizey - 1) {
					ref byte currentWallyp = ref walls[currentx, currenty + 1];
					if (((currentWall & 2) == 0) && ((currentWallyp & 8) == 0)) {
						currenty++;
						continue;
					}
				}
				if (currentx < sizex - 1) {
					ref byte currentWallxp = ref walls[currentx + 1, currenty];
					if (((currentWallxp & 1) == 0) && ((currentWallxp & 8) == 0)) {
						currentx++;
						continue;
					}
				}
				if (currenty > 1) {
					ref byte currentWallym = ref walls[currentx, currenty - 1];
					if (((currentWallym & 2) == 0) && ((currentWallym & 8) == 0)) {
						currenty--;
						continue;
					}
				}
			} else {
				
				int wall = cellWalls[rnd.Next(0, j)];
			
				currentx2 = currentx;
				currenty2 = currenty;

				switch (wall)
				{
					case 2: currentx2++; wall -= 2; break;
					case 3: currenty2--; wall -= 2; break;
				}

				walls[currentx2, currenty2] |= (byte)wall;
				switch (wall)
				{
					case 0: currentx--; break;
					case 1: currenty++; break;
					case 2: currentx++; break;
					case 3: currenty--; break;
				}
				
			}
			canExit = true;
			
		}
		return walls;

	}
}
