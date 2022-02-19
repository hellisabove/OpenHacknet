using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000136 RID: 310
	internal class FileChangeMission : MisisonGoal
	{
		// Token: 0x06000770 RID: 1904 RVA: 0x0007AAC0 File Offset: 0x00078CC0
		public FileChangeMission(string path, string filename, string computerIP, string targetKeyword, OS _os, bool isRemoval = false)
		{
			this.target = filename;
			this.targetKeyword = targetKeyword;
			this.isRemoval = isRemoval;
			this.os = _os;
			Computer computer = Programs.getComputer(this.os, computerIP);
			if (computer == null)
			{
				throw new NullReferenceException("Computer \"" + computerIP + "\" not found for FileChange mission goal");
			}
			this.targetComp = computer;
			this.container = computer.getFolderFromPath(path, false);
			for (int i = 0; i < this.container.files.Count; i++)
			{
				if (this.container.files[i].name.Equals(this.target))
				{
					this.targetData = this.container.files[i].data;
				}
			}
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x0007ABB0 File Offset: 0x00078DB0
		public override bool isComplete(List<string> additionalDetails = null)
		{
			for (int i = 0; i < this.container.files.Count; i++)
			{
				if (this.container.files[i].name.Equals(this.target))
				{
					string text = this.container.files[i].data;
					string text2 = this.targetKeyword;
					if (!this.caseSensitive)
					{
						text = text.ToLower();
						text2 = text2.ToLower();
					}
					bool result;
					if (text.Contains(text2))
					{
						result = !this.isRemoval;
					}
					else
					{
						result = this.isRemoval;
					}
					return result;
				}
			}
			if (this.isRemoval)
			{
				Utils.AppendToWarningsFile(string.Concat(new string[]
				{
					"FileChangeMissionGoal Error: File ",
					this.target,
					" was not found in the container folder \"",
					this.container.name,
					"\" - defaulting to true"
				}));
				return true;
			}
			return false;
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x0007AD00 File Offset: 0x00078F00
		public override string TestCompletable()
		{
			string text = "";
			if (this.container.searchForFile(this.target) == null)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"File to change (",
					this.container.name,
					"/",
					this.target,
					") does not exist!"
				});
			}
			return text;
		}

		// Token: 0x04000856 RID: 2134
		public Folder container;

		// Token: 0x04000857 RID: 2135
		public string target;

		// Token: 0x04000858 RID: 2136
		public string targetData;

		// Token: 0x04000859 RID: 2137
		public string targetKeyword;

		// Token: 0x0400085A RID: 2138
		public Computer targetComp;

		// Token: 0x0400085B RID: 2139
		public OS os;

		// Token: 0x0400085C RID: 2140
		public bool isRemoval = false;

		// Token: 0x0400085D RID: 2141
		public bool caseSensitive = false;
	}
}
