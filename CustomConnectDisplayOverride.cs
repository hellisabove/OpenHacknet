using System;

namespace Hacknet
{
	// Token: 0x02000005 RID: 5
	internal abstract class CustomConnectDisplayOverride : Daemon
	{
		// Token: 0x06000022 RID: 34 RVA: 0x00003616 File Offset: 0x00001816
		public CustomConnectDisplayOverride(Computer c, string name, OS os) : base(c, name, os)
		{
			this.isListed = false;
		}
	}
}
