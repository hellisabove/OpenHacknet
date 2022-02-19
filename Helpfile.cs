using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
	// Token: 0x02000122 RID: 290
	internal class Helpfile
	{
		// Token: 0x060006D8 RID: 1752 RVA: 0x00070FA4 File Offset: 0x0006F1A4
		public static void init()
		{
			Helpfile.help = new List<string>();
			string text = "\n    ";
			Helpfile.help.Add("help [PAGE NUMBER]" + text + LocaleTerms.Loc("Displays the specified page of commands.").Replace("\n", text));
			Helpfile.help.Add("scp [filename] [OPTIONAL: destination]" + text + LocaleTerms.Loc("Copies file named [filename] from remote machine to specified local folder (/bin default)").Replace("\n", text));
			Helpfile.help.Add("scan" + text + LocaleTerms.Loc("Scans for links on the connected machine and adds them to the Map").Replace("\n", text));
			Helpfile.help.Add("rm [filename (or use * for all files in folder)]" + text + LocaleTerms.Loc("Deletes specified file(s)").Replace("\n", text));
			Helpfile.help.Add("ps" + text + LocaleTerms.Loc("Lists currently running processes and their PIDs").Replace("\n", text));
			Helpfile.help.Add("kill [PID]" + text + LocaleTerms.Loc("Kills Process number [PID]").Replace("\n", text));
			Helpfile.help.Add("ls" + text + LocaleTerms.Loc("Lists all files in current directory").Replace("\n", text));
			Helpfile.help.Add("cd [foldername]" + text + LocaleTerms.Loc("Moves current working directory to the specified folder").Replace("\n", text));
			Helpfile.help.Add(string.Concat(new string[]
			{
				"mv [FILE] [DESTINATION]",
				text,
				LocaleTerms.Loc("Moves or renames [FILE] to [DESTINATION]").Replace("\n", text),
				text,
				"(i.e: mv hi.txt ../bin/hi.txt)"
			}));
			Helpfile.help.Add("connect [ip]" + text + LocaleTerms.Loc("Connect to an External Computer").Replace("\n", text));
			Helpfile.help.Add("probe" + text + string.Format(LocaleTerms.Loc("Scans the connected machine for{0}active ports and security level"), text).Replace("\n", text));
			Helpfile.help.Add("exe" + text + LocaleTerms.Loc("Lists all available executables in the local /bin/ folder (Includes hidden and embedded executables)").Replace("\n", text));
			Helpfile.help.Add("disconnect" + text + LocaleTerms.Loc("Terminate the current open connection.").Replace("\n", text) + " ALT: \"dc\"");
			Helpfile.help.Add("cat [filename]" + text + LocaleTerms.Loc("Displays contents of file").Replace("\n", text));
			Helpfile.help.Add("openCDTray" + text + LocaleTerms.Loc("Opens the connected Computer's CD Tray").Replace("\n", text));
			Helpfile.help.Add("closeCDTray" + text + LocaleTerms.Loc("Closes the connected Computer's CD Tray").Replace("\n", text));
			Helpfile.help.Add("reboot [OPTIONAL: -i]" + text + LocaleTerms.Loc("Reboots the connected computer. The -i flag reboots instantly").Replace("\n", text));
			Helpfile.help.Add("replace [filename] \"target\" \"replacement\"" + text + LocaleTerms.Loc("Replaces the target text in the file with the replacement").Replace("\n", text));
			Helpfile.help.Add("analyze" + text + LocaleTerms.Loc("Performs an analysis pass on the firewall of the target machine").Replace("\n", text));
			Helpfile.help.Add("solve [FIREWALL SOLUTION]" + text + LocaleTerms.Loc("Attempts to solve the firewall of target machine to allow UDP Traffic").Replace("\n", text));
			Helpfile.help.Add("login" + text + LocaleTerms.Loc("Requests a username and password to log in to the connected system").Replace("\n", text));
			Helpfile.help.Add("upload [LOCAL FILE PATH]" + text + LocaleTerms.Loc("Uploads the indicated file on your local machine to the current connected directory").Replace("\n", text));
			Helpfile.help.Add("clear" + text + LocaleTerms.Loc("Clears the terminal").Replace("\n", text));
			Helpfile.help.Add("addNote [NOTE]" + text + LocaleTerms.Loc("Add Note").Replace("\n", text));
			Helpfile.help.Add("append [FILENAME] [DATA]" + text + LocaleTerms.Loc("Appends a line containing [DATA] to [FILENAME]").Replace("\n", text));
			Helpfile.help.Add("shell" + text + LocaleTerms.Loc("Opens a remote access shell on target machine with Proxy overload\n and IP trap capabilities").Replace("\n", text));
			Helpfile.LoadedLanguage = Settings.ActiveLocale;
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x00071454 File Offset: 0x0006F654
		public static void writeHelp(OS os, int page = 0)
		{
			if (Helpfile.LoadedLanguage != Settings.ActiveLocale)
			{
				Helpfile.init();
			}
			if (page == 0)
			{
				page = 1;
			}
			int num = (page - 1) * Helpfile.ITEMS_PER_PAGE;
			if (num >= Helpfile.help.Count)
			{
				num = 0;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Helpfile.prefix.Replace("[PAGENUM]", string.Concat(page)).Replace("[TOTALPAGES]", string.Concat(Helpfile.getNumberOfPages())) + "\n");
			int num2 = num;
			while (num2 < Helpfile.help.Count && num2 < num + Helpfile.ITEMS_PER_PAGE)
			{
				stringBuilder.Append(((num2 == 0) ? " " : "") + Helpfile.help[num2] + "\n  \n ");
				num2++;
			}
			os.write(stringBuilder + "\n" + Helpfile.postfix);
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x00071560 File Offset: 0x0006F760
		public static int getNumberOfPages()
		{
			return Helpfile.help.Count / Helpfile.ITEMS_PER_PAGE + 1;
		}

		// Token: 0x040007AB RID: 1963
		private static int ITEMS_PER_PAGE = 10;

		// Token: 0x040007AC RID: 1964
		public static List<string> help;

		// Token: 0x040007AD RID: 1965
		public static string prefix = "---------------------------------\n" + LocaleTerms.Loc("Command List - Page [PAGENUM] of [TOTALPAGES]") + ":\n";

		// Token: 0x040007AE RID: 1966
		private static string postfix = "help [PAGE NUMBER]\n " + LocaleTerms.Loc("Displays the specified page of commands.") + "\n---------------------------------\n";

		// Token: 0x040007AF RID: 1967
		private static string LoadedLanguage = "en-us";
	}
}
