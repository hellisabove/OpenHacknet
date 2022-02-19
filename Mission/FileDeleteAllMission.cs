using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000088 RID: 136
	internal class FileDeleteAllMission : MisisonGoal
	{
		// Token: 0x060002B3 RID: 691 RVA: 0x000279D4 File Offset: 0x00025BD4
		public FileDeleteAllMission(string path, string computerIP, OS _os)
		{
			this.os = _os;
			Computer computer = Programs.getComputer(this.os, computerIP);
			if (computer == null)
			{
				throw new NullReferenceException("Computer \"" + computerIP + "\" not found for FileDeletion mission goal");
			}
			this.targetComp = computer;
			this.container = computer.getFolderFromPath(path, false);
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00027A34 File Offset: 0x00025C34
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return this.container.files.Count == 0;
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00027A5C File Offset: 0x00025C5C
		public override string TestCompletable()
		{
			return "";
		}

		// Token: 0x040002DE RID: 734
		public Folder container;

		// Token: 0x040002DF RID: 735
		public Computer targetComp;

		// Token: 0x040002E0 RID: 736
		public OS os;
	}
}
