using System;

namespace Hacknet
{
	// Token: 0x020000F4 RID: 244
	internal class Administrator
	{
		// Token: 0x0600053A RID: 1338 RVA: 0x00051FC0 File Offset: 0x000501C0
		public virtual void disconnectionDetected(Computer c, OS os)
		{
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x00051FC3 File Offset: 0x000501C3
		public virtual void traceEjectionDetected(Computer c, OS os)
		{
		}

		// Token: 0x040005E4 RID: 1508
		public bool ResetsPassword = false;

		// Token: 0x040005E5 RID: 1509
		public bool IsSuper = false;
	}
}
