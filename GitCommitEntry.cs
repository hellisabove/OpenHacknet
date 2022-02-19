using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000019 RID: 25
	public class GitCommitEntry
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x0000D2EC File Offset: 0x0000B4EC
		public override string ToString()
		{
			return "Commit#" + this.EntryNumber.ToString("000") + (this.SourceIP.StartsWith("192.168.1.1") ? "" : "*");
		}

		// Token: 0x040000C4 RID: 196
		public int EntryNumber = 0;

		// Token: 0x040000C5 RID: 197
		public List<string> ChangedFiles = new List<string>();

		// Token: 0x040000C6 RID: 198
		public string Message;

		// Token: 0x040000C7 RID: 199
		public string UserName;

		// Token: 0x040000C8 RID: 200
		public string SourceIP;
	}
}
