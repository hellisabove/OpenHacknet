using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x0200013B RID: 315
	internal class GetAdminMission : MisisonGoal
	{
		// Token: 0x06000782 RID: 1922 RVA: 0x0007B788 File Offset: 0x00079988
		public GetAdminMission(string compIP, OS _os)
		{
			this.os = _os;
			this.target = Programs.getComputer(this.os, compIP);
			if (this.target == null)
			{
				throw new NullReferenceException("Computer \"" + compIP + "\" not found for FileDeletion mission goal");
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x0007B7E0 File Offset: 0x000799E0
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return this.target.adminIP.Equals(this.os.thisComputer.ip);
		}

		// Token: 0x06000784 RID: 1924 RVA: 0x0007B812 File Offset: 0x00079A12
		public override void reset()
		{
		}

		// Token: 0x04000872 RID: 2162
		public Computer target;

		// Token: 0x04000873 RID: 2163
		public OS os;
	}
}
