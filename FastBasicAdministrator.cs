using System;

namespace Hacknet
{
	// Token: 0x02000165 RID: 357
	internal class FastBasicAdministrator : Administrator
	{
		// Token: 0x060008FF RID: 2303 RVA: 0x000957E0 File Offset: 0x000939E0
		public override void disconnectionDetected(Computer c, OS os)
		{
			base.disconnectionDetected(c, os);
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
			double time = 20.0 * Utils.random.NextDouble();
			Action action = delegate()
			{
				if (os.connectedComp == null || os.connectedComp.ip != c.ip)
				{
					for (int j = 0; j < c.ports.Count; j++)
					{
						c.closePort(c.ports[j], "LOCAL_ADMIN");
					}
					if (this.ResetsPassword)
					{
						c.setAdminPassword(PortExploits.getRandomPassword());
					}
					c.adminIP = c.ip;
					if (c.firewall != null)
					{
						c.firewall.resetSolutionProgress();
					}
				}
			};
			if (this.IsSuper)
			{
				action();
			}
			else
			{
				os.delayer.Post(ActionDelayer.Wait(time), action);
			}
		}
	}
}
