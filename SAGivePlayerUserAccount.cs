using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000043 RID: 67
	public class SAGivePlayerUserAccount : SerializableAction
	{
		// Token: 0x06000166 RID: 358 RVA: 0x0001435C File Offset: 0x0001255C
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				Computer computer = Programs.getComputer(os, this.TargetComp);
				if (computer != null)
				{
					for (int i = 0; i < computer.users.Count; i++)
					{
						if (computer.users[i].name == this.Username)
						{
							UserDetail value = computer.users[i];
							value.known = true;
							computer.users[i] = value;
						}
					}
				}
			}
			else
			{
				Computer computer2 = Programs.getComputer(os, this.DelayHost);
				if (computer2 == null)
				{
					throw new NullReferenceException("Computer " + computer2 + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer2).AddAction(this, delay);
			}
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00014464 File Offset: 0x00012664
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAGivePlayerUserAccount sagivePlayerUserAccount = new SAGivePlayerUserAccount();
			if (rdr.MoveToAttribute("Delay"))
			{
				sagivePlayerUserAccount.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sagivePlayerUserAccount.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				sagivePlayerUserAccount.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Username"))
			{
				sagivePlayerUserAccount.Username = rdr.ReadContentAsString();
			}
			return sagivePlayerUserAccount;
		}

		// Token: 0x04000167 RID: 359
		public string TargetComp;

		// Token: 0x04000168 RID: 360
		public string Username;

		// Token: 0x04000169 RID: 361
		public string DelayHost;

		// Token: 0x0400016A RID: 362
		public float Delay;
	}
}
