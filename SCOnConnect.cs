using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004B RID: 75
	public class SCOnConnect : SerializableCondition
	{
		// Token: 0x0600017F RID: 383 RVA: 0x00015014 File Offset: 0x00013214
		public override bool Check(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.target);
			bool result;
			if (computer == null)
			{
				result = false;
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(this.requiredFlags))
				{
					string[] array = this.requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						if (!os.Flags.HasFlag(array[i]))
						{
							return false;
						}
					}
				}
				if (!this.needsMissionComplete || (os.currentMission != null && os.currentMission.isComplete(null)))
				{
					if (os.connectedComp != null && os.connectedComp.ip == computer.ip)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00015108 File Offset: 0x00013308
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCOnConnect sconConnect = new SCOnConnect();
			if (rdr.MoveToAttribute("target"))
			{
				sconConnect.target = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("needsMissionComplete"))
			{
				sconConnect.needsMissionComplete = (rdr.ReadContentAsString().ToLower() == "true");
			}
			if (rdr.MoveToAttribute("requiredFlags"))
			{
				sconConnect.requiredFlags = rdr.ReadContentAsString();
			}
			if (sconConnect.target == null)
			{
				throw new FormatException("Target computer not specified in OnConnect condition");
			}
			return sconConnect;
		}

		// Token: 0x0400017E RID: 382
		public string target;

		// Token: 0x0400017F RID: 383
		public bool needsMissionComplete;

		// Token: 0x04000180 RID: 384
		public string requiredFlags;
	}
}
