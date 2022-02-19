using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x020000D0 RID: 208
	internal class CheckFlagSetMission : MisisonGoal
	{
		// Token: 0x0600043B RID: 1083 RVA: 0x00044A0A File Offset: 0x00042C0A
		public CheckFlagSetMission(string targetFlagName, OS _os)
		{
			this.target = targetFlagName;
			this.os = _os;
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00044A24 File Offset: 0x00042C24
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return this.os.Flags.HasFlag(this.target);
		}

		// Token: 0x04000518 RID: 1304
		public string target;

		// Token: 0x04000519 RID: 1305
		private OS os;
	}
}
