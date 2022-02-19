using System;
using System.Collections.Generic;
using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x02000081 RID: 129
	public static class LocalizationTests
	{
		// Token: 0x06000298 RID: 664 RVA: 0x00026508 File Offset: 0x00024708
		public static string TestLocalizations(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "\r\n";
			int num = 0;
			string activeLocale = Settings.ActiveLocale;
			for (int i = 0; i < LocaleActivator.SupportedLanguages.Count; i++)
			{
				LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[i].Code, Game1.getSingleton().Content);
				int num2 = 0;
				string text2 = LocalizationTests.LoadAndTestOS(screenMan, out num2);
				num += num2;
				text2 = text2.Replace(".", "").Trim();
				string text3 = text;
				text = string.Concat(new string[]
				{
					text3,
					" -- ",
					LocaleActivator.SupportedLanguages[i].Name,
					": ",
					string.IsNullOrWhiteSpace(text2) ? "Complete\r\n" : ("\r\n" + text2 + "\r\n\r\n")
				});
				text += text2;
			}
			LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"Complete - ",
				num,
				" load stopping errors found"
			});
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000299 RID: 665 RVA: 0x00026650 File Offset: 0x00024850
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
			screenMan.RemoveScreen(os);
			text += TestSuite.getTestingReportForLoadComparison(os, nodes, num, out num);
			text = text + "\r\n" + TestSuite.TestMissions(os);
			errorsAdded = num;
			return text;
		}
	}
}
