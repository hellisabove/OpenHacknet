using System;
using System.Collections.Generic;
using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x0200007D RID: 125
	public static class DLCLocalizationTests
	{
		// Token: 0x06000270 RID: 624 RVA: 0x000232C4 File Offset: 0x000214C4
		public static string TestDLCLocalizations(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "\r\n[";
			int num = 0;
			string activeLocale = Settings.ActiveLocale;
			for (int i = 0; i < LocaleActivator.SupportedLanguages.Count; i++)
			{
				LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[i].Code, Game1.getSingleton().Content);
				int num2 = 0;
				string text2 = DLCLocalizationTests.LoadAndTestOS(screenMan, out num2);
				num += num2;
				text2 = text2.Replace(".", "").Trim();
				if (string.IsNullOrWhiteSpace(text2))
				{
					text += "==";
				}
				else
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						"\r\n -- ",
						LocaleActivator.SupportedLanguages[i].Name,
						": ",
						string.IsNullOrWhiteSpace(text2) ? "Complete\r\n\r\n" : ("\r\n" + text2 + "\r\n\r\n")
					});
				}
			}
			LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"]\r\nComplete - ",
				num,
				" load stopping errors found"
			});
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000271 RID: 625 RVA: 0x00023424 File Offset: 0x00021624
		private static string LoadAndTestOS(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			string text2 = "__hacknettestaccount";
			string pass = "__testingpassword";
			SaveFileManager.AddUser(text2, pass);
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text2);
			OS.TestingPassOnly = true;
			OS os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text2;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			SessionAccelerator.AccelerateSessionToDLCEND(os);
			os.PreDLCVisibleNodesCache = "123,456,789";
			os.delayer.RunAllDelayedActions();
			os.threadedSaveExecute(false);
			List<Computer> nodes = os.netMap.nodes;
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text2;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			os.delayer.RunAllDelayedActions();
			Game1.getSingleton().IsMouseVisible = true;
			if (os.PreDLCVisibleNodesCache != "123,456,789")
			{
				num++;
				text += "PreDLC Visible Node Cache not saving correctly";
			}
			screenMan.RemoveScreen(os);
			text += TestSuite.getTestingReportForLoadComparison(os, nodes, num, out num);
			text = text + "\r\n" + TestSuite.TestMissions(os);
			string text3 = DLCTests.TestDLCFunctionality(screenMan, out errorsAdded);
			text += ((text3.Length > 30) ? ("\r\n" + text3) : "");
			errorsAdded = num;
			return text;
		}
	}
}
