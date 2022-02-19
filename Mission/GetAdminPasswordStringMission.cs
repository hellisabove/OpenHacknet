using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x0200019A RID: 410
	internal class GetAdminPasswordStringMission : MisisonGoal
	{
		// Token: 0x06000A4B RID: 2635 RVA: 0x000A44EC File Offset: 0x000A26EC
		public GetAdminPasswordStringMission(string compIP, OS _os)
		{
			this.os = _os;
			this.target = Programs.getComputer(this.os, compIP);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x000A4510 File Offset: 0x000A2710
		public override bool isComplete(List<string> additionalDetails = null)
		{
			bool result;
			if (additionalDetails == null)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < additionalDetails.Count; i++)
				{
					if (additionalDetails[i].Contains(this.target.adminPass))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04000B9A RID: 2970
		public Computer target;

		// Token: 0x04000B9B RID: 2971
		public OS os;
	}
}
