using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004E RID: 78
	public class SCOnAdminGained : SerializableCondition
	{
		// Token: 0x06000188 RID: 392 RVA: 0x00015310 File Offset: 0x00013510
		public override bool Check(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.target);
			return computer != null && computer.adminIP == os.thisComputer.ip;
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0001535C File Offset: 0x0001355C
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCOnAdminGained sconAdminGained = new SCOnAdminGained();
			if (rdr.MoveToAttribute("target"))
			{
				sconAdminGained.target = rdr.ReadContentAsString();
			}
			if (sconAdminGained.target == null)
			{
				throw new FormatException("Target computer not specified in OnAdminGained condition");
			}
			return sconAdminGained;
		}

		// Token: 0x04000183 RID: 387
		public string target;
	}
}
