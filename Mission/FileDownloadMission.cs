using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x0200013A RID: 314
	internal class FileDownloadMission : FileDeletionMission
	{
		// Token: 0x0600077F RID: 1919 RVA: 0x0007B68F File Offset: 0x0007988F
		public FileDownloadMission(string path, string filename, string computerIP, OS os) : base(path, filename, computerIP, os)
		{
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x0007B6A0 File Offset: 0x000798A0
		public override bool isComplete(List<string> additionalDetails = null)
		{
			Folder root = this.os.thisComputer.files.root;
			for (int i = 0; i < root.folders.Count; i++)
			{
				if (root.folders[i].containsFileWithData(this.targetData))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000781 RID: 1921 RVA: 0x0007B708 File Offset: 0x00079908
		public override string TestCompletable()
		{
			string text = "";
			if (this.container.searchForFile(this.target) == null)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"File to download (",
					this.container.name,
					"/",
					this.target,
					") does not exist!"
				});
			}
			return text;
		}
	}
}
