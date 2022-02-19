using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x020000D1 RID: 209
	internal class DelayMission : MisisonGoal
	{
		// Token: 0x0600043D RID: 1085 RVA: 0x00044A4C File Offset: 0x00042C4C
		public DelayMission(float time)
		{
			this.time = time;
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00044A6C File Offset: 0x00042C6C
		public override bool isComplete(List<string> additionalDetails = null)
		{
			bool result;
			if (this.firstRequest == null)
			{
				this.firstRequest = new DateTime?(DateTime.Now);
				result = false;
			}
			else
			{
				result = ((DateTime.Now - this.firstRequest.Value).TotalSeconds >= (double)this.time);
			}
			return result;
		}

		// Token: 0x0400051A RID: 1306
		public float time;

		// Token: 0x0400051B RID: 1307
		private DateTime? firstRequest = null;
	}
}
