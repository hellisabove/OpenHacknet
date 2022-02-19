using System;

namespace Hacknet
{
	// Token: 0x02000166 RID: 358
	internal class FastProgressOnlyAdministrator : Administrator
	{
		// Token: 0x06000901 RID: 2305 RVA: 0x0009592C File Offset: 0x00093B2C
		public override void traceEjectionDetected(Computer c, OS os)
		{
			base.traceEjectionDetected(c, os);
			this.disconnectionDetected(c, os);
		}

		// Token: 0x06000902 RID: 2306 RVA: 0x00095944 File Offset: 0x00093B44
		public override void disconnectionDetected(Computer c, OS os)
		{
			base.disconnectionDetected(c, os);
			if (c.adminIP != os.thisComputer.adminIP)
			{
				for (int i = 0; i < c.ports.Count; i++)
				{
					c.closePort(c.ports[i], "LOCAL_ADMIN");
				}
				if (c.firewall != null)
				{
					c.firewall.resetSolutionProgress();
					c.firewall.solved = false;
				}
				if (c.hasProxy)
				{
					c.proxyActive = true;
					c.proxyOverloadTicks = c.startingOverloadTicks;
				}
			}
		}
	}
}
