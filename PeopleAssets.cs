using System;

namespace Hacknet
{
	// Token: 0x0200014B RID: 331
	internal class PeopleAssets
	{
		// Token: 0x0600082F RID: 2095 RVA: 0x00089B40 File Offset: 0x00087D40
		public static Degree getRandomDegree(WorldLocation origin)
		{
			Degree degree = new Degree();
			degree.name = PeopleAssets.randOf(PeopleAssets.degreeTitles) + PeopleAssets.randOf(PeopleAssets.degreeNames);
			degree.GPA = 3f + 3f * (float)(Utils.random.NextDouble() - 0.5) * 0.5f;
			while (degree.GPA > 4f)
			{
				degree.GPA -= Utils.randm(0.4f);
			}
			degree.uni = "University of " + origin.name;
			if (Utils.flipCoin())
			{
				degree.uni = origin.name + " University";
			}
			return degree;
		}

		// Token: 0x06000830 RID: 2096 RVA: 0x00089C08 File Offset: 0x00087E08
		public static Degree getRandomHackerDegree(WorldLocation origin)
		{
			Degree degree = new Degree();
			degree.name = PeopleAssets.randOf(PeopleAssets.degreeTitles) + PeopleAssets.randOf(PeopleAssets.hackerDegreeNames);
			degree.GPA = 3f + 5f * (float)(Utils.random.NextDouble() - 0.5) * 0.5f;
			degree.uni = "University of " + origin.name;
			if (Utils.flipCoin())
			{
				degree.uni = origin.name + " University";
			}
			return degree;
		}

		// Token: 0x06000831 RID: 2097 RVA: 0x00089CA8 File Offset: 0x00087EA8
		public static string randOf(string[] array)
		{
			int num = (int)(Math.Max(0.0001, Utils.random.NextDouble()) * (double)array.Length - 1.0);
			return array[num];
		}

		// Token: 0x040009B4 RID: 2484
		private static string[] degreeTitles = new string[]
		{
			"Bachelor of ",
			"Masters in ",
			"PHD in "
		};

		// Token: 0x040009B5 RID: 2485
		private static string[] degreeNames = new string[]
		{
			"Computer Science",
			"Business",
			"Electrical Engineering",
			"Finance",
			"Marketing",
			"The Arts",
			"Computer Graphics",
			"Design",
			"Medicine",
			"Pharmacy",
			"Information Technology",
			"Psychology"
		};

		// Token: 0x040009B6 RID: 2486
		private static string[] hackerDegreeNames = new string[]
		{
			"Computer Science",
			"Digital Security",
			"Computer Networking",
			"Information Technology",
			"Computer Graphics"
		};
	}
}
