using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000139 RID: 313
	internal class FileDeletionMission : MisisonGoal
	{
		// Token: 0x0600077C RID: 1916 RVA: 0x0007B3F4 File Offset: 0x000795F4
		public FileDeletionMission(string path, string filename, string computerIP, OS _os)
		{
			this.target = filename;
			this.os = _os;
			this.targetComp = computerIP;
			Computer computer = Programs.getComputer(this.os, this.targetComp);
			if (computer == null && Settings.IsInExtensionMode)
			{
				throw new NullReferenceException("Computer \"" + computerIP + "\" was not found for File Deletion mission goal!");
			}
			this.targetPath = path;
			if (computer != null)
			{
				this.container = computer.getFolderFromPath(path, false);
				if (this.container == null)
				{
					throw new NullReferenceException(string.Concat(new string[]
					{
						"Folder ",
						path,
						" was not found on computer ",
						computerIP,
						" for file deletion mission goal"
					}));
				}
				for (int i = 0; i < this.container.files.Count; i++)
				{
					if (this.container.files[i].name.Equals(this.target))
					{
						this.targetData = this.container.files[i].data;
					}
				}
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x0007B528 File Offset: 0x00079728
		public override bool isComplete(List<string> additionalDetails = null)
		{
			Computer computer = Programs.getComputer(this.os, this.targetComp);
			bool result;
			if (computer == null)
			{
				result = true;
			}
			else
			{
				this.container = computer.getFolderFromPath(this.targetPath, false);
				if (this.container == null)
				{
					result = true;
				}
				else
				{
					for (int i = 0; i < this.container.files.Count; i++)
					{
						if (this.container.files[i].name.Equals(this.target) && (this.container.files[i].data.Equals(this.targetData) || this.targetData == null))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0007B610 File Offset: 0x00079810
		public override string TestCompletable()
		{
			string text = "";
			if (this.container.searchForFile(this.target) == null)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"File to delete (",
					this.container.name,
					"/",
					this.target,
					") does not exist!"
				});
			}
			return text;
		}

		// Token: 0x0400086C RID: 2156
		public Folder container;

		// Token: 0x0400086D RID: 2157
		public string target;

		// Token: 0x0400086E RID: 2158
		public string targetData;

		// Token: 0x0400086F RID: 2159
		public string targetComp;

		// Token: 0x04000870 RID: 2160
		public string targetPath;

		// Token: 0x04000871 RID: 2161
		public OS os;
	}
}
