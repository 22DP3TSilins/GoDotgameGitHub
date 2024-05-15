// Izmantoju struktūru, jo tās ir ātrākas
public struct GameMode{
	public int Difficulty;
	public int Seed;

	// Pārmaina no string ievaes uz Difficulty un Seed mainīgajiem
	public string SetGameModeStr {set {
		string[] mode = value.Split(':');
		Difficulty = int.Parse(mode[0]);
		Seed = int.Parse(mode[1]);
	}}

	// Konstruktors kas izveidos gamemode nejaušam labirintam
	public GameMode() {
		Difficulty = -1;
		Seed = -1;
	}

	// Konstruktors kas no veseliem skaitļiem izveidos objektu
	public GameMode(int difficulty, int seed) {
		Difficulty = difficulty;
		Seed = seed;
	}
	
	// Konstruktors kas no string ievades izveidos objektu
	public GameMode(string mode) {
		string[] modeSplit = mode.Split(':');
		Difficulty = int.Parse(modeSplit[0]);
		Seed = int.Parse(modeSplit[1]);
	}

	// Parveido par simbolu virkni
	public override string ToString() {
		return $"{Difficulty}:{Seed}";
	}
}
