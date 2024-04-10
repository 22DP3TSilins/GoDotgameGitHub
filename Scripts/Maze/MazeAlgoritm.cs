using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

public readonly struct coords {
	public readonly int X;
	public readonly int Y;

	public coords (int x, int y){
		X = x;
		Y = y;
	}
}

public partial class MazeAlgoritm : Node3D
{
	// const int sizex = 15;
	// const int sizey = 15;
	// const int startx = 8;
	// const int starty = 8;
	enum Wall: byte {Up = 1, Left = 2, Visited = 4, Explored = 8}
	Node3D meshes = null;
	IDictionary<coords, coords> customMazeBehavior = new Dictionary<coords, coords>();
	List<Node3D> wallMeshes = new List<Node3D>();
	Finish finish = null;
	public override void _Ready()
	{
		
		GD.Print("Maze is cookin...");
		meshes = GetParent().GetNode<Node3D>("MazeWalls");
		finish = meshes.GetNode<Finish>("Finish");
		genMaze(8, false);
		// byte[,] walls = genMazeWalls();
		// AssyncLoadMaze(walls);
		// await Task.Run(() => genMaze());
		GD.Print("Maze is cooked");
		// solveMaze(walls, new coords(1, 1));
	}

	public void genMaze(int size, bool newMaze, int seed = -1) {
		List<List<byte>> walls = genMazeWalls(size, seed);
		AssyncLoadMaze(size, walls, newMaze);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void AssyncLoadMaze(int size, List<List<byte>> walls, bool genNew) {
		int sizex = size;
		int sizey = size;
		int wallx = 0;
		int wallz = 0;
		for (int i = 1; i < sizex + 2; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				if (((walls[i][j] & (byte)Wall.Up) == 0) && (j != 0)) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallz.tscn");
					wallz++;
				}
				if ((walls[i][j] & (byte)Wall.Left) == 0 && (i != sizex + 1)) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallx.tscn");
					wallx++;
				}
			}
		}
		PackedScene testScene;
		if (genNew) {
			foreach(Node3D mesh in wallMeshes) {
				mesh.QueueFree();
			}
			wallMeshes = new List<Node3D>();
		}
		for (int i = 1; i < sizex + 2; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				if ((walls[i][j] & (byte)Wall.Up) == 0 && (j != 0)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallz.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(3.0f + i * 4.0f, 0.0f, 3.0f + j * 4.0f);
					wallMeshes.Add(testMesh);
				}
				if ((walls[i][j] & (byte)Wall.Left) == 0 && (i != sizex + 1)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallx.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(5.0f + i * 4.0f, 0.0f, 5.0f + j * 4.0f);
					wallMeshes.Add(testMesh);
				}
			}
		}
		
		for (int i = 0; i < sizex + 1; i++) {
			for (int j = 0; j < sizey + 1; j++) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/WallCorner.tscn");
			}
		}
		for (int i = 0; i < sizex + 1; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/WallCorner.tscn");
				Node3D testMesh = (Node3D)testScene.Instantiate();
				meshes.AddChild(testMesh);
				testMesh.Position = new Vector3(7.0f + i * 4.0f, 0.0f, 5.0f + j * 4.0f);
				wallMeshes.Add(testMesh);
			}
		}
		GD.Print("Loading... labyrinth");
	}
	public List<List<byte>> genMazeWalls(int size, int seed = -1) {
		Random rnd;
		if (seed == -1) {
			rnd = new Random();
		} else {
			rnd = new Random(seed);
		}
		int startx = (int)Math.Ceiling(size / 2.0);
		int starty =  (int)Math.Ceiling(size / 2.0);

		int sizex = size;
		int sizey = size;
		
		int currentx = startx;
		int currenty = starty;

		finish.Position = new Vector3(5.0f + startx * 4.0f, 0.0f, 3.0f + starty * 4.0f);

		// byte[,] walls = new byte[sizex + 2, sizey + 2]; // 1 Up 2 Left 4 Visited 8 Explored compleatly 16 Up maze wall 32 Left maze wall;
		// List<byte> a = new List<byte>()
		List<List<byte>> walls = new List<List<byte>>();
		for (int i = 0; i < sizex + 2; i++) {
			walls.Add(new List<byte>());
			for (int j = 0; j < sizey + 2; j++) {
				walls[i].Add(0);
			}
		}
		for (int i = 0; i < sizex + 2; i++) {
			walls[i][0] |= (byte)Wall.Visited | (byte)Wall.Explored;
			walls[i][sizey + 1] |= (byte)Wall.Visited | (byte)Wall.Explored;
		}
		for (int i = 0; i < sizey + 2; i++) {
			walls[0][i] |= (byte)Wall.Visited | (byte)Wall.Explored;
			walls[sizex + 1][i] |= (byte)Wall.Visited | (byte)Wall.Explored;
		}

		byte currentCell;
		byte currentWall;
		coords currentDir;
		coords currentCellAround;
		int randWall;

		int[] cellWalls = new int[4];
		coords[] directions = new [] { new coords(0, 0), new coords(0, 0), new coords(1, 0), new coords(0, -1) };
		coords[] cellsAround = new [] { new coords(-1, 0), new coords(0, 1), new coords(1, 0), new coords(0, -1) };

		int randDir;

		while (true) {
			walls[currentx][currenty] |= 4;
			int j = 0;
			for (int i = 0; i < 4; i++) {
				try
				{
					currentDir = directions[i];
					currentCellAround = cellsAround[i];

					currentWall = walls[currentx + currentDir.X][currenty + currentDir.Y];
					currentCell = walls[currentx + currentCellAround.X][currenty + currentCellAround.Y];
					if(((currentCell & (byte)Wall.Visited) | (currentWall & (byte)((i % 2) + 1))) == 0) {
						cellWalls[j] = i;
						j++;
					}
				} catch (IndexOutOfRangeException) {
					GD.Print("out of range1");
					break;
				}
			}
			if (j == 0) {
				bool a = true;
				walls[currentx][currenty] |= 8;
				for (int i = 0; i < 4; i++) {
					
					currentDir = directions[i];
					currentCellAround = cellsAround[i];

					currentWall = walls[currentx + currentDir.X][currenty + currentDir.Y];
					currentCell = walls[currentx + currentCellAround.X][currenty + currentCellAround.Y];
					
					
					if(((currentCell & ((byte)Wall.Visited | (byte)Wall.Explored)) == 4) && ((currentWall & (byte)((i % 2) + 1)) > 0)) {
						currentx += currentCellAround.X;
						currenty += currentCellAround.Y;

						a = false;
						break;
					}
				}
				if (a) {
					for (int i = 0; i < 4; i++) {
						currentDir = directions[i];
						currentCellAround = cellsAround[i];

						currentWall = walls[currentx + currentDir.X][currenty + currentDir.Y];
						currentCell = walls[currentx + currentCellAround.X][currenty + currentCellAround.Y];
						GD.Print($"Wall: {currentWall}\nCell: {currentCell}");
					}
					string row = "";
					string newNum = "";
					for (int i = 0; i < sizex + 3; i++) {
						row = "";
						for (int k = 0; k < sizey + 3; k++) {
							newNum = " " + +walls[i][k];
							row += newNum.Length == 2 ? " " + newNum : newNum;
						}
						GD.Print(row + "\n");
					}
					GD.Print("\n");
					GD.Print("aaaaaa");
					break;
				}
				if (currentx == startx && currenty == starty) {GD.Print("reached start"); break;}
			} else {
				randDir = rnd.Next(0, j);
				randWall = cellWalls[randDir];
				currentDir = directions[randWall];
				currentCellAround = cellsAround[randWall];

				walls[currentx + currentDir.X][currenty + currentDir.Y] |= (byte)((randWall % 2) + 1);

				currentx += currentCellAround.X;
				currenty += currentCellAround.Y;

				
			}
		}
		return walls;
	}
	// public (int X1, int Y1, byte Wall1, int X2, int Y2, byte Wall2) 
	// public List<coords> solveMaze(byte[,] walls, int X, int Y) {
	// 	List<coords> path = new List<coords>();



	// 	return path;
	// }

// 	public List<coords> solveMaze(byte[,] walls, coords end) {
// 		List<coords> path = new List<coords> { new coords(startx, starty) };
// 		List<coords> pointsOfMulPaths = new List<coords> { new coords(startx, starty) };
// 		byte[,] walls1 = (byte[,])walls.Clone();

// 		int currentx = startx;
// 		int currenty = starty;

// 		byte currentCell;
// 		byte currentWall;
// 		coords currentDir;
// 		coords currentCellAround;

// 		int[] cellWalls = new int[4];
// 		coords[] directions = new [] { new coords(0, 0), new coords(0, 0), new coords(1, 0), new coords(0, -1) };
// 		coords[] cellsAround = new [] { new coords(-1, 0), new coords(0, 1), new coords(1, 0), new coords(0, -1) };

// 		List<List<int>> allPosibleDirections = new List<List<int>>();
// 		List<int> newPaths;


// 		int paths;
// 		int currentPath;
// 		for (int gen = 0; gen < 100; gen++) {
// 		// while (true) {
// 			paths = 0;
// 			newPaths = new List<int>();
// 			for (int i = 0; i < 4; i++) {
// 				currentDir = directions[i];
// 				currentCellAround = cellsAround[i];

// 				currentWall = walls[currentx + currentDir.X, currenty + currentDir.Y];
// 				// currentCell = walls[currentx + currentCellAround.X, currenty + currentCellAround.Y];
// 				if ((currentWall & ((i % 2) + 1)) > 0) {
// 					paths++;
// 					newPaths.Add(i);
// 				}
// 			}
// 			if (paths == 1 && !(currentx == startx && currenty == starty)) {
// 				currentx = pointsOfMulPaths[pointsOfMulPaths.Count - 1].X;
// 				currenty = pointsOfMulPaths[pointsOfMulPaths.Count - 1].Y;
// 			}
// 			if (paths > 1) {
// 				allPosibleDirections.Add(newPaths);
// 				int lastList = allPosibleDirections.Count - 1;
// 				int dir = allPosibleDirections[lastList][allPosibleDirections[lastList].Count - 1];
// 				currentCellAround = directions[dir];
				
// 				currentx += currentCellAround.X;
// 				currenty += currentCellAround.Y;

// 				allPosibleDirections[lastList].Remove(dir);

// 			}
// 			if (currentx == end.X && currenty == end.Y) {
// 				break;
// 			}
// 			if (paths > 2) {
// 				pointsOfMulPaths.Add(new coords(currentx, currenty));
// 			}
			
			
			
// 		}
// 		foreach (coords coords1 in pointsOfMulPaths) {
// 			GD.Print($"X: {coords1.X}; Y: {coords1.Y}");
// 		}
// 		GD.Print();
// 		return pointsOfMulPaths;

// 	}
// }
}
