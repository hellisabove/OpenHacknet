using System;
using Steamworks;

namespace Hacknet
{
	// Token: 0x02000091 RID: 145
	public static class StatsManager
	{
		// Token: 0x060002D2 RID: 722 RVA: 0x0002906E File Offset: 0x0002726E
		public static void InitStats()
		{
			StatsManager.UserStatsCallback = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(StatsManager.OnUserStatsReceived));
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x00029088 File Offset: 0x00027288
		private static void OnUserStatsReceived(UserStatsReceived_t pCallback)
		{
			for (int i = 0; i < StatsManager.StatDefinitions.Length; i++)
			{
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x000290B0 File Offset: 0x000272B0
		public static void IncrementStat(string statName, int valueChange)
		{
			if (StatsManager.HasReceivedUserStats)
			{
				SteamUserStats.SetStat(statName, valueChange);
			}
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x000290D4 File Offset: 0x000272D4
		public static void SaveStatProgress()
		{
			if (StatsManager.HasReceivedUserStats)
			{
				SteamUserStats.StoreStats();
			}
		}

		// Token: 0x04000314 RID: 788
		private static bool HasReceivedUserStats = false;

		// Token: 0x04000315 RID: 789
		private static Callback<UserStatsReceived_t> UserStatsCallback;

		// Token: 0x04000316 RID: 790
		private static StatsManager.StatData[] StatDefinitions = new StatsManager.StatData[]
		{
			new StatsManager.StatData
			{
				Name = "commands_run"
			}
		};

		// Token: 0x02000092 RID: 146
		private struct StatData
		{
			// Token: 0x04000317 RID: 791
			public string Name;

			// Token: 0x04000318 RID: 792
			public int IntVal;

			// Token: 0x04000319 RID: 793
			public float FloatVal;
		}
	}
}
