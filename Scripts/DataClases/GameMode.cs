public struct GameMode{
	public int Difficulty;
	public int Seed;
	public string SetGameModeStr {set {
		string[] mode = value.Split(':');
		Difficulty = int.Parse(mode[0]);
		Seed = int.Parse(mode[1]);

	}}
	public GameMode() {
		Difficulty = -1;
		Seed = -1;
	}
	public GameMode(int difficulty, int seed) {
		Difficulty = difficulty;
		Seed = seed;
	}
	public GameMode(string mode) {
		string[] modeSplit = mode.Split(':');
		Difficulty = int.Parse(modeSplit[0]);
		Seed = int.Parse(modeSplit[1]);
	}
	public override string ToString() {
		return $"{Difficulty}:{Seed}";
	}
}
