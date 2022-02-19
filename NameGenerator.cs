using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000141 RID: 321
	internal static class NameGenerator
	{
		// Token: 0x060007B4 RID: 1972 RVA: 0x0007E380 File Offset: 0x0007C580
		public static void init()
		{
			NameGenerator.main = new List<string>();
			NameGenerator.main.Add("Holopoint");
			NameGenerator.main.Add("Ascendant");
			NameGenerator.main.Add("Enabled");
			NameGenerator.main.Add("Subversion");
			NameGenerator.main.Add("Introversion");
			NameGenerator.main.Add("Photonic");
			NameGenerator.main.Add("Enlightened");
			NameGenerator.main.Add("Software");
			NameGenerator.main.Add("Facespace");
			NameGenerator.main.Add("Mott");
			NameGenerator.main.Add("Starchip");
			NameGenerator.main.Add("Macrosoft");
			NameGenerator.main.Add("Oppol");
			NameGenerator.main.Add("Octovision");
			NameGenerator.main.Add("tijital");
			NameGenerator.main.Add("Valence");
			NameGenerator.main.Add("20%Cooler");
			NameGenerator.main.Add("Celestia");
			NameGenerator.main.Add("Manic");
			NameGenerator.main.Add("Dengler");
			NameGenerator.main.Add("Beagle");
			NameGenerator.main.Add("Warden");
			NameGenerator.main.Add("Phoenix");
			NameGenerator.main.Add("Banished Stallion");
			NameGenerator.postfix = new List<string>();
			NameGenerator.postfix.Add(" Inc");
			NameGenerator.postfix.Add(" Interactive");
			NameGenerator.postfix.Add(".com");
			NameGenerator.postfix.Add(" Internal");
			NameGenerator.postfix.Add(" Software");
			NameGenerator.postfix.Add(" Technologies");
			NameGenerator.postfix.Add(" Tech");
			NameGenerator.postfix.Add(" Solutions");
			NameGenerator.postfix.Add(" Enterprises");
			NameGenerator.postfix.Add(" Studios");
			NameGenerator.postfix.Add(" Consortium");
			NameGenerator.postfix.Add(" Communications");
		}

		// Token: 0x060007B5 RID: 1973 RVA: 0x0007E5E4 File Offset: 0x0007C7E4
		public static string generateName()
		{
			string str = "";
			str += NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)];
			return str + NameGenerator.postfix[Utils.random.Next(0, NameGenerator.postfix.Count)];
		}

		// Token: 0x060007B6 RID: 1974 RVA: 0x0007E64C File Offset: 0x0007C84C
		public static string[] generateCompanyName()
		{
			return new string[]
			{
				NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)],
				NameGenerator.postfix[Utils.random.Next(0, NameGenerator.postfix.Count)]
			};
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0007E6AC File Offset: 0x0007C8AC
		public static string getRandomMain()
		{
			return NameGenerator.main[Utils.random.Next(0, NameGenerator.main.Count)];
		}

		// Token: 0x040008C6 RID: 2246
		public static List<string> main;

		// Token: 0x040008C7 RID: 2247
		public static List<string> postfix;
	}
}
