using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000134 RID: 308
	internal class FileUploadMission : MisisonGoal
	{
		// Token: 0x0600076B RID: 1899 RVA: 0x0007A7D8 File Offset: 0x000789D8
		public FileUploadMission(string path, string filename, string computerWithFileIP, string computerToUploadToIP, string destToUploadToPath, OS _os, bool needsDecrypt = false, string decryptPass = "")
		{
			this.target = filename;
			this.os = _os;
			Computer computer = Programs.getComputer(this.os, computerWithFileIP);
			if (computer == null)
			{
				throw new FormatException("Error parsing File Upload Mission - Source comp " + computerWithFileIP + " not found!");
			}
			this.targetComp = computer;
			this.container = computer.getFolderFromPath(path, false);
			for (int i = 0; i < this.container.files.Count; i++)
			{
				if (this.container.files[i].name.Equals(this.target))
				{
					this.targetData = this.container.files[i].data;
					if (needsDecrypt)
					{
						this.targetData = FileEncrypter.DecryptString(this.targetData, decryptPass)[2];
					}
				}
			}
			Computer computer2 = Programs.getComputer(this.os, computerToUploadToIP);
			if (computer2 == null)
			{
				throw new FormatException("Error parsing File Upload Mission - Dest comp " + computerWithFileIP + " not found!");
			}
			this.uploadTargetComp = computer2;
			this.destinationFolder = computer2.getFolderFromPath(destToUploadToPath, false);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x0007A910 File Offset: 0x00078B10
		public override bool isComplete(List<string> additionalDetails = null)
		{
			for (int i = 0; i < this.destinationFolder.files.Count; i++)
			{
				if (this.targetData == null)
				{
					if (this.destinationFolder.files[i].name.ToLower().Equals(this.target.ToLower()))
					{
						return true;
					}
				}
				else if (this.destinationFolder.files[i].data.Equals(this.targetData))
				{
					return true;
				}
			}
			bool flag = 0 == 0;
			return false;
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x0007A9CC File Offset: 0x00078BCC
		public override string TestCompletable()
		{
			string text = "";
			if (this.container.searchForFile(this.target) == null)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"File to upload (",
					this.container.name,
					"/",
					this.target,
					") does not exist!"
				});
			}
			return text;
		}

		// Token: 0x0400084E RID: 2126
		public Folder container;

		// Token: 0x0400084F RID: 2127
		public string target;

		// Token: 0x04000850 RID: 2128
		public string targetData;

		// Token: 0x04000851 RID: 2129
		public Computer targetComp;

		// Token: 0x04000852 RID: 2130
		public Computer uploadTargetComp;

		// Token: 0x04000853 RID: 2131
		public OS os;

		// Token: 0x04000854 RID: 2132
		public Folder destinationFolder;
	}
}
