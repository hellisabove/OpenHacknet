using System;
using System.Text;

namespace Hacknet
{
	// Token: 0x020000CD RID: 205
	public class EOSAppGenerator
	{
		// Token: 0x06000422 RID: 1058 RVA: 0x00042004 File Offset: 0x00040204
		private static string[] GenerateNames()
		{
			string[] array = new string[2];
			StringBuilder stringBuilder = new StringBuilder("");
			int num = 0;
			if (Utils.flipCoin())
			{
				stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.Name1) + " ");
				num++;
			}
			stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.Name2));
			if (Utils.flipCoin() || num == 0)
			{
				stringBuilder.Append(" " + Utils.RandomFromArray(EOSAppGenerator.Name3) + " ");
				num++;
			}
			array[1] = stringBuilder.ToString().Trim();
			bool flag = Utils.flipCoin();
			if ((num <= 1 && flag) || (flag && Utils.flipCoin()))
			{
				string text = Utils.RandomFromArray(EOSAppGenerator.Postfix);
				if (Utils.random.NextDouble() < 0.13)
				{
					text = Utils.RandomFromArray(EOSAppGenerator.Name3);
				}
				if (Utils.flipCoin())
				{
					text = text.ToUpper();
				}
				stringBuilder.Append(": " + text);
			}
			array[0] = stringBuilder.ToString().Trim();
			return array;
		}

		// Token: 0x06000423 RID: 1059 RVA: 0x00042150 File Offset: 0x00040350
		public static string GenerateName()
		{
			return EOSAppGenerator.GenerateNames()[0];
		}

		// Token: 0x06000424 RID: 1060 RVA: 0x0004216C File Offset: 0x0004036C
		public static string GenerateAppSaveLine()
		{
			StringBuilder stringBuilder = new StringBuilder("");
			if (Utils.random.NextDouble() < 0.7)
			{
				stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.SaveData1));
				stringBuilder.Append(" ");
			}
			stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.SaveData2));
			stringBuilder.Append(" ");
			if (Utils.random.NextDouble() < 0.08)
			{
				stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.SaveDataWildcards) + " ");
			}
			else
			{
				stringBuilder.Append(Utils.RandomFromArray(EOSAppGenerator.SaveData3) + " ");
			}
			stringBuilder.Append(": ");
			int num = 0;
			if (Utils.random.NextDouble() < 0.7)
			{
				num = (int)Utils.getRandomByte();
			}
			if (Utils.random.NextDouble() < 0.2)
			{
				num = Utils.random.Next();
			}
			stringBuilder.Append(string.Concat(num));
			return stringBuilder.ToString();
		}

		// Token: 0x06000425 RID: 1061 RVA: 0x000422A8 File Offset: 0x000404A8
		public static Folder GetAppFolder()
		{
			string[] array = EOSAppGenerator.GenerateNames();
			string text = array[0];
			string text2 = array[1];
			string foldername = text2.ToLower().Replace(" ", "_").Trim();
			string data = text.Replace(" ", "_").Trim();
			Folder folder = new Folder(foldername);
			folder.files.Add(new FileEntry(Computer.generateBinaryString(1024), "app.pkg"));
			StringBuilder stringBuilder = new StringBuilder("----- [" + text + "] Save Data -----\n\n");
			int num = 8 + Utils.random.Next(8);
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append(EOSAppGenerator.GenerateAppSaveLine());
				stringBuilder.Append("\n\n");
			}
			folder.files.Add(new FileEntry(stringBuilder.ToString(), FileSanitiser.purifyStringForDisplay(data) + ".sav"));
			return folder;
		}

		// Token: 0x0400050B RID: 1291
		public static string[] Name1 = new string[]
		{
			"Candy",
			"Super",
			"Angry",
			"Extreme",
			"Crazy",
			"Delicious",
			"Zombie",
			"Spirits",
			"Ghost",
			"Spooky",
			"Skate",
			"Smash",
			"Tiny",
			"Mini",
			"Small",
			"Cute",
			"Baby",
			"Die"
		};

		// Token: 0x0400050C RID: 1292
		public static string[] Name2 = new string[]
		{
			"Ultraviolence",
			"Violence",
			"Crush",
			"Ninja",
			"Samurai",
			"Birds",
			"Zombie",
			"Rope",
			"Crowds",
			"Mecha",
			"Racer",
			"Point",
			"Fighter",
			"Warrior",
			"Gem",
			"Treasure",
			"Gold",
			"Booty",
			"Loot",
			"Fart",
			"Hard"
		};

		// Token: 0x0400050D RID: 1293
		public static string[] Name3 = new string[]
		{
			"Quest",
			"Saga",
			"Trilogy",
			"1",
			"2",
			"II",
			"IV",
			"8",
			"XXX",
			"Adventure",
			"Crusher",
			"Builder",
			"Extreme",
			"Ultra",
			"MK2",
			"Season Pass",
			"DLC",
			"Free"
		};

		// Token: 0x0400050E RID: 1294
		public static string[] Postfix = new string[]
		{
			"Revengance",
			"The Third",
			"Payback Time",
			"Die Harder",
			"No Survivors",
			"No Prisoners",
			"Bird's Revenge",
			"Edgy Reboot",
			"Justice Edition",
			"Get Naked",
			"Pay2Win Edition",
			"Enemy Unknown",
			"The Crushening",
			"Elucidator",
			"Blood on the sand",
			"The Long War"
		};

		// Token: 0x0400050F RID: 1295
		public static string[] SaveData1 = new string[]
		{
			"Sacred",
			"Holy",
			"Shiny",
			"Gold",
			"Precious",
			"Outlawed",
			"Dirty",
			"Mysterious",
			"Paid",
			"Blood",
			"Culturally Significant",
			"Irreplacable",
			"Priceless",
			"Rare",
			"Normal",
			"Beloved",
			"Evil",
			"Really Evil",
			"Chaotic Good",
			"Huge",
			"DLC",
			"Dramatic"
		};

		// Token: 0x04000510 RID: 1296
		public static string[] SaveData2 = new string[]
		{
			"Urns",
			"Ruins",
			"Coins",
			"Objects",
			"Grandparents",
			"Mermaids",
			"Slaves",
			"Artifacts",
			"Artworks",
			"Planets",
			"Sunglasses",
			"Weapons",
			"Ninja Techniques",
			"Family Members",
			"Spaceships",
			"Racing Vehicles",
			"Tamed Animals",
			"Animals",
			"Wild Panthers",
			"Flowers",
			"Candies",
			"Gems",
			"Birds",
			"Anime Girls",
			"Gains",
			"Reps",
			"Catchphrases",
			"One-Liners",
			"Kanye West Tweets",
			"Buisness Cards"
		};

		// Token: 0x04000511 RID: 1297
		public static string[] SaveData3 = new string[]
		{
			"Desecrated",
			"Ruined",
			"Toppled",
			"Punched",
			"Bought",
			"Collected",
			"Found",
			"Upended",
			"Ruined",
			"Soiled",
			"Violated",
			"Rescued",
			"Witnessed",
			"Gathered",
			"Lost",
			"Murdured",
			"Ultraviolenced",
			"Found",
			"Taken",
			"Liberated",
			"Tamed",
			"Touched",
			"Unlocked",
			"Beaten",
			"Conquered",
			"Hacked",
			"Rescued",
			"Kidnapped",
			"Hoarded",
			"Dodged"
		};

		// Token: 0x04000512 RID: 1298
		public static string[] SaveDataWildcards = new string[]
		{
			"Killed in a single punch",
			"Donated to charity",
			"Stolen form orphans",
			"Crushed in fist",
			"Thrown into the sun",
			"Loved tenderly",
			"Shown off in mirror",
			"Posted on the internet",
			"Thrown into hammerspace",
			"Forgotten thanks to alcohol",
			"Kickflipped over",
			"Brought to justice",
			"Flaunted in rap video"
		};
	}
}
