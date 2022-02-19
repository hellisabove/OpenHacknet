using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Hacknet.Localization
{
	// Token: 0x02000077 RID: 119
	public static class LocaleActivator
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000258 RID: 600 RVA: 0x00021D60 File Offset: 0x0001FF60
		public static List<LocaleActivator.LanguageInfo> SupportedLanguages
		{
			get
			{
				if (LocaleActivator.supportedLanguages == null)
				{
					LocaleActivator.LoadSupportedLocales();
				}
				return LocaleActivator.supportedLanguages;
			}
		}

		// Token: 0x06000259 RID: 601 RVA: 0x00021D8C File Offset: 0x0001FF8C
		private static void LoadSupportedLocales()
		{
			LocaleActivator.supportedLanguages = new List<LocaleActivator.LanguageInfo>();
			string text = Utils.readEntireFile("Content/Locales/SupportedLanguages.txt");
			string[] array = text.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length > 2)
				{
					LocaleActivator.supportedLanguages.Add(new LocaleActivator.LanguageInfo
					{
						Name = array2[0],
						Code = array2[1],
						SteamCode = array2[2]
					});
				}
			}
		}

		// Token: 0x0600025A RID: 602 RVA: 0x00021E28 File Offset: 0x00020028
		public static void ActivateLocale(string localeCode, ContentManager content)
		{
			if (localeCode == "en-us")
			{
				LocaleTerms.ClearForEnUS();
			}
			else
			{
				string text = "Content/Locales/" + localeCode + "/UI_Terms.txt";
				string text2 = "Content/Locales/" + localeCode + "/Hacknet_UI_Terms.txt";
				if (!File.Exists(text))
				{
					if (!File.Exists(text2))
					{
						throw new NotSupportedException("Locale " + localeCode + " does not exist or is not supported");
					}
					text = text2;
				}
				LocaleTerms.ReadInTerms(text, true);
				if (DLC1SessionUpgrader.HasDLC1Installed)
				{
					string text3 = "Content/Locales/" + localeCode + "/DLC/Hacknet_UI_Terms.txt";
					if (File.Exists(text3))
					{
						LocaleTerms.ReadInTerms(text3, false);
					}
				}
			}
			Settings.ActiveLocale = localeCode;
			LocaleFontLoader.LoadFontConfigForLocale(localeCode, content);
			FileEntry.init(content);
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00021EFC File Offset: 0x000200FC
		public static bool ActiveLocaleIsCJK()
		{
			return Settings.ActiveLocale.StartsWith("zh") || Settings.ActiveLocale.StartsWith("ja") || Settings.ActiveLocale.StartsWith("ko");
		}

		// Token: 0x040002CA RID: 714
		private static List<LocaleActivator.LanguageInfo> supportedLanguages = null;

		// Token: 0x02000078 RID: 120
		public struct LanguageInfo
		{
			// Token: 0x040002CB RID: 715
			public string Name;

			// Token: 0x040002CC RID: 716
			public string Code;

			// Token: 0x040002CD RID: 717
			public string SteamCode;
		}
	}
}
