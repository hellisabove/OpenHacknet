using System;
using Steamworks;

namespace Hacknet
{
	// Token: 0x0200008D RID: 141
	public static class AchievementsManager
	{
		// Token: 0x060002C0 RID: 704 RVA: 0x00028594 File Offset: 0x00026794
		public static bool Unlock(string name, bool recordAndCheckFlag = false)
		{
			bool result;
			try
			{
				string flag = name + "_Unlocked";
				if (!recordAndCheckFlag || !OS.currentInstance.Flags.HasFlag(flag))
				{
					SteamUserStats.SetAchievement(name);
					bool flag2 = SteamUserStats.StoreStats();
					if (flag2)
					{
						OS.currentInstance.Flags.AddFlag(flag);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}
	}
}
