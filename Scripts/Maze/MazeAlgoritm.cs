using Godot;
using System;
using System.Collections.Generic;


public partial class MazeAlgoritm : Node3D
{
	enum Wall: byte {Up = 1, Left = 2, Visited = 4, Explored = 8}
	Node3D meshes = null;
	IDictionary<Coords, Coords> customMazeBehavior = new Dictionary<Coords, Coords>();
	List<Node3D> wallMeshes = new List<Node3D>();
	Finish finish = null;
	player Player = null;
	ScoreBoard scoreBoard = null;
	UI ui = null;
	public override void _Ready()
	{
		meshes = GetNode<Node3D>("../MazeWalls");
		finish = meshes.GetNode<Finish>("Finish");
		Player = GetNode<player>("../Player");
		scoreBoard = GetNode<ScoreBoard>("../Scores");
		ui = GetNode<UI>("../UI/Control");
		genMaze((int)UserData.DefaultSettings["Difficulty"], false);
		// byte[,] walls = genMazeWalls();
		// AssyncLoadMaze(walls);
		// await Task.Run(() => genMaze());
		// solveMaze(walls, new coords(1, 1));
	}

	public void genMaze(int size, bool newMaze, int seed = -1, bool setLocation = true) {

		seed = seed == -1 ? new Random().Next() : seed;
		if (scoreBoard == null) GD.Print("\naaa\naaa\naaa\naaa\naaa");
		if (scoreBoard.user != null) scoreBoard.user.CurrentGameMode = new GameMode(size, seed, ui.genRndMaze.ButtonPressed);
		
		if (setLocation) SetPlayerLocation(size, seed);
		List<List<byte>> walls = genMazeWalls(size, seed);
		AssyncLoadMaze(size, walls, newMaze);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	const float SPACING = 4.0f;
	public void SetPlayerLocation(int size, int seed = -1){
		Random rnd;
		seed = seed == -1 ? new Random().Next() : seed;
		rnd = new Random(seed);
		
		Vector3 newPlayerPos = new(SPACING + 1.5f, -4.24f, SPACING + 0.5f);
		// Vector3 newPlayerPos = new(SPACING + 3.5f, -4.24f, SPACING + 3.5f);

		GD.Print($"Min: {size}");
		if (rnd.Next() < int.MaxValue / 2) {
			newPlayerPos += new Vector3(rnd.Next(0, size - 1), 0.0f, 0.0f) * SPACING;
		} else {
			newPlayerPos += new Vector3(0.0f, 0.0f, rnd.Next(0, size - 1)) * SPACING;
		}
		Player.Position = newPlayerPos;
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
		const float floor = 0.0f;
		for (int i = 1; i < sizex + 2; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				if ((walls[i][j] & (byte)Wall.Up) == 0 && (j != 0)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallz.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(0.0f + i * SPACING, floor, 0.0f + j * SPACING);
					wallMeshes.Add(testMesh);
				}
				if ((walls[i][j] & (byte)Wall.Left) == 0 && (i != sizex + 1)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallx.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(2.0f + i * SPACING, floor, 2.0f + j * SPACING);
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
				testMesh.Position = new Vector3(4.0f + i * SPACING, 0.0f, 2.0f + j * SPACING);
				wallMeshes.Add(testMesh);
			}
		}
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

		finish.Position = new Vector3(2.0f + startx * SPACING, 0.0f, 0.0f + starty * SPACING);

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
		Coords currentDir;
		Coords currentCellAround;
		int randWall;

		int[] cellWalls = new int[4];
		Coords[] directions = new [] { new Coords(0, 0), new Coords(0, 0), new Coords(1, 0), new Coords(0, -1) };
		Coords[] cellsAround = new [] { new Coords(-1, 0), new Coords(0, 1), new Coords(1, 0), new Coords(0, -1) };

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
					break;
				}
			}
			if (j == 0) {
				
				walls[currentx][currenty] |= 8;
				for (int i = 0; i < 4; i++) {
					
					currentDir = directions[i];
					currentCellAround = cellsAround[i];

					currentWall = walls[currentx + currentDir.X][currenty + currentDir.Y];
					currentCell = walls[currentx + currentCellAround.X][currenty + currentCellAround.Y];
					
					
					if(((currentCell & ((byte)Wall.Visited | (byte)Wall.Explored)) == 4) && ((currentWall & (byte)((i % 2) + 1)) > 0)) {
						currentx += currentCellAround.X;
						currenty += currentCellAround.Y;
						break;
					}
				}
				
				if (currentx == startx && currenty == starty) break;
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
