using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000135 RID: 309
	internal class GetStringMission : MisisonGoal
	{
		// Token: 0x0600076E RID: 1902 RVA: 0x0007AA4B File Offset: 0x00078C4B
		public GetStringMission(string targetData)
		{
			this.target = targetData;
		}

		// Token: 0x0600076F RID: 1903 RVA: 0x0007AA60 File Offset: 0x00078C60
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
					if (additionalDetails[i].ToLower().Contains(this.target.ToLower()))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x04000855 RID: 2133
		public string target;
	}
}
