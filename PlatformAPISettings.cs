using System;
using System.Collections.Generic;
using System.Globalization;
using Hacknet.Localization;
using Steamworks;

namespace Hacknet
{
	// Token: 0x0200008F RID: 143
	public static class PlatformAPISettings
	{
		// Token: 0x060002C9 RID: 713 RVA: 0x00028B58 File Offset: 0x00026D58
		public static void InitPlatformAPI()
		{
			if (!Settings.isConventionDemo)
			{
				PlatformAPISettings.Running = SteamAPI.Init();
				if (!PlatformAPISettings.Running)
				{
					PlatformAPISettings.Report = "First Init Failed. ";
					Console.WriteLine("Steam Init Failed!");
					PlatformAPISettings.Running = SteamAPI.InitSafe();
					PlatformAPISettings.Report = PlatformAPISettings.Report + " Second init Running = " + PlatformAPISettings.Running;
					Console.WriteLine(PlatformAPISettings.Report);
				}
				else
				{
					PlatformAPISettings.Report = "Steam API Running :" + PlatformAPISettings.Running;
				}
				if (PlatformAPISettings.Running)
				{
					PlatformAPISettings.RemoteStorageRunning = SteamRemoteStorage.IsCloudEnabledForAccount();
				}
			}
		}

		// Token: 0x060002CA RID: 714 RVA: 0x00028C08 File Offset: 0x00026E08
		public static string GetCodeForActiveLanguage(List<LocaleActivator.LanguageInfo> supportedLanguages)
		{
			Console.WriteLine("Scanning for language code...");
			if (PlatformAPISettings.Running)
			{
				string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
				for (int i = 0; i < supportedLanguages.Count; i++)
				{
					if (supportedLanguages[i].SteamCode == currentGameLanguage)
					{
						Console.WriteLine("Matched Steam Language Code : " + currentGameLanguage);
						return supportedLanguages[i].Code;
					}
				}
			}
			CultureInfo originalCultureInfo = Game1.OriginalCultureInfo;
			for (int i = 0; i < supportedLanguages.Count; i++)
			{
				if (supportedLanguages[i].Code == originalCultureInfo.Name)
				{
					Console.WriteLine("Found exact language match for " + originalCultureInfo.Name);
					return supportedLanguages[i].Code;
				}
			}
			for (int i = 0; i < supportedLanguages.Count; i++)
			{
				string text = supportedLanguages[i].Code.Substring(0, 2);
				if (originalCultureInfo.Name.StartsWith(text))
				{
					Console.WriteLine("Found close enough language match for " + text);
					return supportedLanguages[i].Code;
				}
			}
			Console.WriteLine(string.Concat(new string[]
			{
				"No language match found for ",
				originalCultureInfo.Name,
				" - ",
				originalCultureInfo.DisplayName,
				". Reverting to English..."
			}));
			return "en-us";
		}

		// Token: 0x0400030E RID: 782
		public static string Report = "";

		// Token: 0x0400030F RID: 783
		public static bool Running = false;

		// Token: 0x04000310 RID: 784
		public static bool RemoteStorageRunning = false;
	}
}
