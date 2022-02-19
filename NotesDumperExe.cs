using System;

namespace Hacknet
{
	// Token: 0x02000198 RID: 408
	internal static class NotesDumperExe
	{
		// Token: 0x06000A49 RID: 2633 RVA: 0x000A4168 File Offset: 0x000A2368
		public static void RunNotesDumperExe(string[] args, object osObj, Computer target)
		{
			OS os = (OS)osObj;
			Folder folder = os.thisComputer.files.root.searchForFolder("home");
			FileEntry fileEntry = folder.searchForFile("Notes.txt");
			if (fileEntry == null)
			{
				os.write(LocaleTerms.Loc("Dump Notes Output:") + "_______________\n");
				os.write(" ");
				os.write(LocaleTerms.Loc("ERROR: No notes found on home system!"));
				os.write("_______________________________");
				os.write(" ");
			}
			else
			{
				string[] array = fileEntry.data.Split(new string[]
				{
					"\n\n----------\n\n"
				}, StringSplitOptions.RemoveEmptyEntries);
				os.write(" ");
				os.write(LocaleTerms.Loc("Notes") + ":________________________");
				os.write(" ");
				for (int i = 0; i < array.Length; i++)
				{
					os.write(array[i]);
					os.write("______________________________");
					os.write(" ");
				}
			}
		}
	}
}
