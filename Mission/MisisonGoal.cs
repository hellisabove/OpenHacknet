using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000086 RID: 134
	public class MisisonGoal
	{
		// Token: 0x060002AC RID: 684 RVA: 0x000277D0 File Offset: 0x000259D0
		public virtual bool isComplete(List<string> additionalDetails = null)
		{
			return true;
		}

		// Token: 0x060002AD RID: 685 RVA: 0x000277E3 File Offset: 0x000259E3
		public virtual void reset()
		{
		}

		// Token: 0x060002AE RID: 686 RVA: 0x000277E8 File Offset: 0x000259E8
		public virtual string TestCompletable()
		{
			return "";
		}
	}
}
