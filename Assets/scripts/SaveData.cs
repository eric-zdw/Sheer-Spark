using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
	public bool gameStarted;
	public List<string> levelsClearedOnNormal;
	public List<string> levelsClearedOnHard;
	public List<int> scoresNormal;
	public List<int> scoresHard;
	public List<bool> weaponsUnlocked;
	//public List<string> weaponLoadout;

	public SaveData() {
		gameStarted = false;
		levelsClearedOnNormal = new List<string>();
		levelsClearedOnHard = new List<string>();
		scoresNormal = new List<int>();
		scoresHard = new List<int>();

		for (int i = 0; i < 15; i++)
        {
			scoresNormal.Add(0);
			scoresHard.Add(0);
        }

		weaponsUnlocked = new List<bool>();
		for (int i = 0; i < 6; i++)
        {
			weaponsUnlocked.Add(false);
        }

		/*
		weaponLoadout = new List<string>();

		//default weapon loadout
		weaponLoadout.Add("WeaponRed1");
		weaponLoadout.Add("WeaponOrange1");
		weaponLoadout.Add("WeaponYellow1");
		weaponLoadout.Add("WeaponGreen1");
		weaponLoadout.Add("WeaponBlue1");
		weaponLoadout.Add("WeaponPurple1");
		*/
		
	}
}
