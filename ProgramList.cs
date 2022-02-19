using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000151 RID: 337
	internal static class ProgramList
	{
		// Token: 0x0600084E RID: 2126 RVA: 0x0008BCA8 File Offset: 0x00089EA8
		public static void init()
		{
			ProgramList.programs = new List<string>();
			ProgramList.programs.Add("ls");
			ProgramList.programs.Add("cd");
			ProgramList.programs.Add("probe");
			ProgramList.programs.Add("scan");
			ProgramList.programs.Add("ps");
			ProgramList.programs.Add("kill");
			ProgramList.programs.Add("connect");
			ProgramList.programs.Add("dc");
			ProgramList.programs.Add("disconnect");
			ProgramList.programs.Add("help");
			ProgramList.programs.Add("exe");
			ProgramList.programs.Add("cat");
			ProgramList.programs.Add("scp");
			ProgramList.programs.Add("rm");
			ProgramList.programs.Add("openCDTray");
			ProgramList.programs.Add("closeCDTray");
			ProgramList.programs.Add("login");
			ProgramList.programs.Add("reboot");
			ProgramList.programs.Add("mv");
			ProgramList.programs.Add("upload");
			ProgramList.programs.Add("analyze");
			ProgramList.programs.Add("solve");
			ProgramList.programs.Add("addNote");
		}

		// Token: 0x0600084F RID: 2127 RVA: 0x0008BE30 File Offset: 0x0008A030
		public static List<string> getExeList(OS os)
		{
			List<string> list = new List<string>();
			list.Add("PortHack");
			list.Add("ForkBomb");
			list.Add("Shell");
			list.Add("Tutorial");
			list.Add("Notes");
			Folder folder = os.thisComputer.files.root.folders[2];
			for (int i = 0; i < folder.files.Count; i++)
			{
				list.Add(folder.files[i].name.Replace(".exe", ""));
			}
			return list;
		}

		// Token: 0x040009E9 RID: 2537
		public static List<string> programs;
	}
}
