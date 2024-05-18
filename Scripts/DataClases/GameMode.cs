// Izmantoju struktūru, jo tās ir ātrākas
// Struktūras pārbaudes ņem vērā sēklu tikai, ja spēles režīms nav nejaušs
using Godot;

public struct GameMode{
	public int Difficulty;
	public int Seed;
	public bool Rand;

	// Pārmaina no string ievaes uz Difficulty un Seed mainīgajiem
	public string SetGameModeStr {set {
		string[] mode = value.Split(':');
		Difficulty = int.Parse(mode[0]);
		Seed = int.Parse(mode[1]);
	}}

	// Konstruktors kas izveidos gamemode nejaušam labirintam
	// public GameMode() {
	// 	Difficulty = -1;
	// 	Seed = -1;
	// }

	// Konstruktors, kas izveidos objektu
	public GameMode(int difficulty, int seed, bool rand) {
		Difficulty = difficulty;
		Seed = seed;
		Rand = rand;
	}
	
	// Konstruktors kas no string ievades izveidos objektu
	public GameMode(string mode) {
		string[] modeSplit = mode.Split(':');
		Difficulty = int.Parse(modeSplit[0]);
		Seed = int.Parse(modeSplit[1]);
		Rand = bool.Parse(modeSplit[2]);
	}

	// Operātori
	public static bool operator ==(GameMode gm1, GameMode gm2) => gm1.Equals(gm2);
	public static bool operator !=(GameMode gm1, GameMode gm2) => !gm1.Equals(gm2);

	// Parveido par simbolu virkni
	public override string ToString() {
		return $"{Difficulty}:{Seed}:{Rand}";
	}

	public override bool Equals(object obj) {
		// return GetHashCode() == obj.GetHashCode();

		// Pārbauda vai objektu var pārveidot par "GameMode"
		GD.Print("is oby gm: ", obj is GameMode);
		if (!(obj is GameMode))
			return false;
		
		// Pārveido objektu par "GameMode"
		GameMode gm2 = (GameMode) obj;

		// Salīdzina objektus
		if (Rand && gm2.Rand) {
			// GD.Print("rand: ")
			return Difficulty == gm2.Difficulty;
		}
		return !(Difficulty == gm2.Difficulty && Seed == gm2.Seed && Rand && gm2.Rand);
	}

	// Pārrakstu hash funkciju.
	// Izmantoju pirmsskaitļus, jo no tiem ir vislielākā iespēja iegūt unikālu vērtību, 
	// reizinot vērtības ar izvēlēto pirmskaitli un saskaitot tās visas.
	// unchecked izmantoju, lai skaitļi pārplūstot neradītu kļūdas.
	public override int GetHashCode() {
		int hash = 17;
		unchecked {
			hash = hash * 23 + Difficulty.GetHashCode();
			if (!Rand) hash = hash * 23 + Seed.GetHashCode();
		}
		
		return hash;
	}
}
