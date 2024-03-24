using Godot;
using System;

public partial class MazeAlgoritm : Node3D
{
	const int sizex = 15;
	const int sizey = 15;
	const int startx = 8;
	const int starty = 8;
	enum Wall: byte {Up = 1, Left = 2, Visited = 4, Explored = 8}
	Node3D meshes = null;
	public override void _Ready()
	{
		GD.Print("Maze is cookin...");
		meshes = GetParent<Node3D>().GetNode<Node3D>("MazeWalls");
		byte[,] walls = genMazeWalls();
		AssyncLoadMaze(walls);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void AssyncLoadMaze(byte[,] walls) {
		for (int i = 1; i < sizex + 2; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				if (((walls[i, j] & (byte)Wall.Up) == 0) && (j != 0)) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallz.tscn");
				}
				if ((walls[i, j] & (byte)Wall.Left) == 0 && (i != sizex + 1)) {
					ResourceLoader.LoadThreadedRequest("res://Scines/Levels/wallx.tscn");
				}
			}
		}
		PackedScene testScene = null;
		for (int i = 1; i < sizex + 2; i++) {
			for (int j = 0; j < sizey + 1; j++) {
				if ((walls[i, j] & (byte)Wall.Up) == 0 && (j != 0)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallz.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(3.0f + i * 4.0f, 0.0f, 3.0f + j * 4.0f);
				}
				if ((walls[i, j] & (byte)Wall.Left) == 0 && (i != sizex + 1)) {
					testScene = (PackedScene)ResourceLoader.LoadThreadedGet("res://Scines/Levels/wallx.tscn");
					Node3D testMesh = (Node3D)testScene.Instantiate();
					meshes.AddChild(testMesh);
					testMesh.Position = new Vector3(5.0f + i * 4.0f, 0.0f, 5.0f + j * 4.0f);
				}
			}
		}
		GD.Print("Loading... labyrinth");
	}
	
	public byte[,] genMazeWalls() {
		Random rnd = new Random();
		
		int currentx = startx;
		int currenty = starty;

		byte[,] walls = new byte[sizex + 2, sizey + 2]; // 1 Up 2 Left 4 Visited 8 Explored compleatly 16 Up maze wall 32 Left maze wall;

		for (int i = 0; i < sizex + 2; i++) {
			walls[i, 0] |= (byte)Wall.Visited | (byte)Wall.Explored;
			walls[i, sizey + 1] |= (byte)Wall.Visited | (byte)Wall.Explored;
		}
		for (int i = 0; i < sizey + 2; i++) {
			walls[0, i] |= (byte)Wall.Visited | (byte)Wall.Explored;
			walls[sizex + 1, i] |= (byte)Wall.Visited | (byte)Wall.Explored;
		}

		byte currentCell;
		byte currentWall;
		(int X, int Y) currentDir;
		(int X, int Y) currentCellAround;
		int randWall;

		int[] cellWalls = new int[4];
		(int X, int Y) [] directions = new [] { (0, 0), (0, 0), (1, 0), (0, -1) };
		(int X, int Y) [] cellsAround = new [] { (-1, 0), (0, 1), (1, 0), (0, -1) };

		int randDir;

		while (true) {
			walls[currentx, currenty] |= 4;
			int j = 0;
			for (int i = 0; i < 4; i++) {
				try
				{
					currentDir = directions[i];
					currentCellAround = cellsAround[i];

					currentWall = walls[currentx + currentDir.X, currenty + currentDir.Y];
					currentCell = walls[currentx + currentCellAround.X, currenty + currentCellAround.Y];
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
				for (int i = 0; i < 4; i++) {
					
					currentDir = directions[i];
					currentCellAround = cellsAround[i];

					currentWall = walls[currentx + currentDir.X, currenty + currentDir.Y];
					currentCell = walls[currentx + currentCellAround.X, currenty + currentCellAround.Y];
					walls[currentx, currenty] |= 8;
					
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

						currentWall = walls[currentx + currentDir.X, currenty + currentDir.Y];
						currentCell = walls[currentx + currentCellAround.X, currenty + currentCellAround.Y];
						GD.Print($"Wall: {currentWall}\nCell: {currentCell}");
					}
					string row = "";
					string newNum = "";
					for (int i = 0; i < sizex + 3; i++) {
						row = "";
						for (int k = 0; k < sizey + 3; k++) {
							newNum = " " + +walls[i, k];
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

				walls[currentx + currentDir.X, currenty + currentDir.Y] |= (byte)((randWall % 2) + 1);

				currentx += currentCellAround.X;
				currenty += currentCellAround.Y;

				
			}
		}
		return walls;
	}
}
