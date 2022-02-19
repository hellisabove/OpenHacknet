using System;

namespace Hacknet
{
	// Token: 0x0200001B RID: 27
	public class OnlineAccount
	{
		// Token: 0x060000C5 RID: 197 RVA: 0x0000D370 File Offset: 0x0000B570
		public override string ToString()
		{
			return this.Username + "#" + this.ID;
		}

		// Token: 0x040000CB RID: 203
		public int ID = 0;

		// Token: 0x040000CC RID: 204
		public string Username;

		// Token: 0x040000CD RID: 205
		public string BanStatus;

		// Token: 0x040000CE RID: 206
		public string Notes;
	}
}
