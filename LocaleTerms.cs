using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet
{
	// Token: 0x0200007A RID: 122
	public static class LocaleTerms
	{
		// Token: 0x0600025F RID: 607 RVA: 0x00022224 File Offset: 0x00020424
		public static void ReadInTerms(string termsFilepath, bool clearPreviouslyLoadedTerms = true)
		{
			char[] separator = new char[]
			{
				'\t'
			};
			if (clearPreviouslyLoadedTerms)
			{
				LocaleTerms.ActiveTerms.Clear();
			}
			string[] array = File.ReadAllLines(termsFilepath);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i].StartsWith("\t"))
				{
					string[] array2 = array[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length > 1)
					{
						if (!LocaleTerms.ActiveTerms.ContainsKey(array2[0]))
						{
							if (!LocaleTerms.ActiveTerms.ContainsKey(LocaleTerms.RemoveQuotes(array2[0])))
							{
								string input = array2[1].Replace("[%\\n%]", "\n");
								LocaleTerms.ActiveTerms.Add(LocaleTerms.RemoveQuotes(array2[0]), LocaleTerms.RemoveQuotes(input));
							}
						}
					}
				}
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x00022318 File Offset: 0x00020518
		private static string RemoveQuotes(string input)
		{
			if (input.StartsWith("\""))
			{
				input = input.Substring(1);
			}
			if (input.EndsWith("\""))
			{
				input = input.Substring(0, input.Length - 1);
			}
			return input;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x00022369 File Offset: 0x00020569
		public static void ClearForEnUS()
		{
			LocaleTerms.ActiveTerms.Clear();
		}

		// Token: 0x06000262 RID: 610 RVA: 0x00022378 File Offset: 0x00020578
		public static string Loc(string input)
		{
			string result;
			if (Settings.ActiveLocale == "en-us")
			{
				result = input;
			}
			else if (LocaleTerms.ActiveTerms.ContainsKey(input))
			{
				result = LocaleTerms.ActiveTerms[input];
			}
			else
			{
				result = input;
			}
			return result;
		}

		// Token: 0x040002CE RID: 718
		private const string NewlineReplacer = "[%\\n%]";

		// Token: 0x040002CF RID: 719
		public static Dictionary<string, string> ActiveTerms = new Dictionary<string, string>();
	}
}
