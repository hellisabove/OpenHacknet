using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000109 RID: 265
	public static class DebugLog
	{
		// Token: 0x0600063A RID: 1594 RVA: 0x000661B0 File Offset: 0x000643B0
		public static void add(string s)
		{
			string[] array = s.Split(DebugLog.delimiters);
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].Equals(""))
				{
					DebugLog.data.Add(array[i]);
				}
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x00066200 File Offset: 0x00064400
		public static string GetDump()
		{
			string text = "";
			for (int i = 0; i < DebugLog.data.Count; i++)
			{
				text = text + DebugLog.data[i] + "\r\n";
			}
			return text;
		}

		// Token: 0x040006F5 RID: 1781
		public static char[] delimiters = new char[]
		{
			'\n',
			'\r'
		};

		// Token: 0x040006F6 RID: 1782
		public static List<string> data = new List<string>(64);
	}
}
