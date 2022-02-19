using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Hacknet
{
	// Token: 0x0200007B RID: 123
	public static class LocalizedFileLoader
	{
		// Token: 0x06000264 RID: 612 RVA: 0x000223D4 File Offset: 0x000205D4
		public static string Read(string filepath)
		{
			return LocalizedFileLoader.FilterStringForLocalization(File.ReadAllText(LocalizedFileLoader.GetLocalizedFilepath(filepath)));
		}

		// Token: 0x06000265 RID: 613 RVA: 0x000223F8 File Offset: 0x000205F8
		public static string GetLocalizedFilepath(string filepath)
		{
			filepath = filepath.Replace("\\", "/");
			string str = "Content/Locales/" + Settings.ActiveLocale;
			string text = filepath.Replace("Content/", str + "/");
			string result;
			if (File.Exists(text))
			{
				result = text;
			}
			else
			{
				result = filepath;
			}
			return result;
		}

		// Token: 0x06000266 RID: 614 RVA: 0x00022458 File Offset: 0x00020658
		public static string SafeFilterString(string data)
		{
			string source = "\r\n\t ";
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				if (GuiData.tinyfont.Characters.Contains(data[i]) || source.Contains(data[i]))
				{
					stringBuilder.Append(data[i]);
				}
				else
				{
					stringBuilder.Append("?");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000267 RID: 615 RVA: 0x000224E4 File Offset: 0x000206E4
		public static string FilterStringForLocalization(string data)
		{
			return data.Replace("&quot;", "'").Replace("\u00a0", "").Replace("[PRÉNOM]#[NOM]#[NUM_DOSSIER]#32#Rural#N/A#N/A#N/A#[DERNIERS_MOTS]", "[FIRST_NAME]#[LAST_NAME]#[RECORD_NUM]#32#Rural#N/A#N/A#N/A#[LAST_WORDS]").Replace("[PRÉNOM]", "[FIRST_NAME]");
		}
	}
}
