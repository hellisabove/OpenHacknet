using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hacknet
{
	// Token: 0x0200014E RID: 334
	public static class WorldLocationLoader
	{
		// Token: 0x0600083F RID: 2111 RVA: 0x0008A248 File Offset: 0x00088448
		public static void init()
		{
			CultureInfo provider = new CultureInfo("en-au");
			string text = Utils.readEntireFile("Content/PersonData/LocationData.txt");
			string[] array = text.Split(Utils.newlineDelim);
			char[] separator = new char[]
			{
				'#'
			};
			WorldLocationLoader.locations = new List<WorldLocation>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator);
				WorldLocationLoader.locations.Add(new WorldLocation(array2[1], array2[0], (float)Convert.ToDouble(array2[2], provider), (float)Convert.ToDouble(array2[3], provider), (float)Convert.ToDouble(array2[4], provider), (float)Convert.ToDouble(array2[5], provider)));
			}
		}

		// Token: 0x06000840 RID: 2112 RVA: 0x0008A2FC File Offset: 0x000884FC
		public static WorldLocation getRandomLocation()
		{
			return WorldLocationLoader.locations[Utils.random.Next(WorldLocationLoader.locations.Count)];
		}

		// Token: 0x06000841 RID: 2113 RVA: 0x0008A32C File Offset: 0x0008852C
		public static WorldLocation getClosestOrCreate(string name)
		{
			for (int i = 0; i < WorldLocationLoader.locations.Count; i++)
			{
				if (WorldLocationLoader.locations[i].name.ToLower().Equals(name.ToLower()))
				{
					return WorldLocationLoader.locations[i];
				}
			}
			WorldLocation worldLocation = new WorldLocation(name, name, (float)Utils.random.NextDouble(), (float)Utils.random.NextDouble(), (float)Utils.random.NextDouble(), (float)Utils.random.NextDouble());
			WorldLocationLoader.locations.Add(worldLocation);
			return worldLocation;
		}

		// Token: 0x040009C8 RID: 2504
		public static List<WorldLocation> locations;
	}
}
