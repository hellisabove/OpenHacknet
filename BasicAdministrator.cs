using System;

namespace Hacknet
{
	// Token: 0x02000162 RID: 354
	internal class BasicAdministrator : Administrator
	{
		// Token: 0x060008EF RID: 2287 RVA: 0x0009481C File Offset: 0x00092A1C
		public override void disconnectionDetected(Computer c, OS os)
		{
			base.disconnectionDetected(c, os);
			double time = 20.0 * Utils.random.NextDouble();
			os.delayer.Post(ActionDelayer.Wait(time), delegate
			{
				if (os.connectedComp == null || os.connectedComp.ip != c.ip)
				{
					for (int i = 0; i < c.ports.Count; i++)
					{
						c.closePort(c.ports[i], "LOCAL_ADMIN");
					}
					if (this.ResetsPassword)
					{
						c.setAdminPassword(PortExploits.getRandomPassword());
					}
					c.adminIP = c.ip;
					if (c.firewall != null)
					{
						c.firewall.solved = false;
						c.firewall.resetSolutionProgress();
					}
				}
			});
		}
	}
}
