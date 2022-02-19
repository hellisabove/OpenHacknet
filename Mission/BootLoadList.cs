using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000131 RID: 305
	public static class BootLoadList
	{
		// Token: 0x06000762 RID: 1890 RVA: 0x0007A404 File Offset: 0x00078604
		public static List<string> getList()
		{
			string text = "";
			if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
			{
				text = text + "\r\n" + Utils.readEntireFile("Content/DLC/DLCBootList.txt");
			}
			text += Utils.readEntireFile("Content/Computers/BootLoadList.txt");
			return BootLoadList.getListFromData(text);
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0007A460 File Offset: 0x00078660
		public static List<string> getDLCList()
		{
			string data = Utils.readEntireFile("Content/DLC/DLCBootList.txt");
			return BootLoadList.getListFromData(data);
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x0007A484 File Offset: 0x00078684
		public static List<string> getDemoList()
		{
			string data = Utils.readEntireFile("Content/Computers/DemoLoadList.txt");
			return BootLoadList.getListFromData(data);
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x0007A4A8 File Offset: 0x000786A8
		public static List<string> getAdventureList()
		{
			string data = Utils.readEntireFile("Content/AdventureNetwork/AdventureLoadList.txt");
			return BootLoadList.getListFromData(data);
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0007A4CC File Offset: 0x000786CC
		private static List<string> getListFromData(string data)
		{
			string[] separator = new string[]
			{
				"\n\r",
				"\r\n"
			};
			string[] array = data.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith("#") && array[i].Length > 1)
				{
					list.Add(array[i]);
				}
			}
			return list;
		}
	}
}
