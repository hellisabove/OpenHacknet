using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000050 RID: 80
	public class SCOnDisconnect : SerializableCondition
	{
		// Token: 0x0600018E RID: 398 RVA: 0x00015440 File Offset: 0x00013640
		public override bool Check(object os_obj)
		{
			bool result;
			if (!this.hasHadFrameWhereThisWasFalse)
			{
				this.hasHadFrameWhereThisWasFalse = true;
				result = false;
			}
			else
			{
				bool flag = ((OS)os_obj).connectedComp == null || ((OS)os_obj).connectedComp == ((OS)os_obj).thisComputer;
				if (string.IsNullOrWhiteSpace(this.target) || this.target.ToLower() == "none")
				{
					result = flag;
				}
				else
				{
					OS os = (OS)os_obj;
					Computer computer = Programs.getComputer(os, this.target);
					if (computer == null)
					{
						result = flag;
					}
					else
					{
						result = (os.connectedIPLastFrame == computer.ip && (flag || (os.connectedComp != null && os.connectedComp.ip != computer.ip)));
					}
				}
			}
			return result;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00015540 File Offset: 0x00013740
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCOnDisconnect sconDisconnect = new SCOnDisconnect();
			if (rdr.MoveToAttribute("target"))
			{
				sconDisconnect.target = rdr.ReadContentAsString();
			}
			return sconDisconnect;
		}

		// Token: 0x04000185 RID: 389
		private bool hasHadFrameWhereThisWasFalse = false;

		// Token: 0x04000186 RID: 390
		public string target;
	}
}
