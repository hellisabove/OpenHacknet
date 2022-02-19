using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000038 RID: 56
	public class SACrashComputer : SerializableAction
	{
		// Token: 0x06000144 RID: 324 RVA: 0x00012D94 File Offset: 0x00010F94
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer != null || OS.DEBUG_COMMANDS)
			{
				if (this.Delay <= 0f)
				{
					computer.crash(this.CrashSource);
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
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00012E40 File Offset: 0x00011040
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SACrashComputer sacrashComputer = new SACrashComputer();
			if (rdr.MoveToAttribute("Delay"))
			{
				sacrashComputer.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				sacrashComputer.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("CrashSource"))
			{
				sacrashComputer.CrashSource = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sacrashComputer.DelayHost = rdr.ReadContentAsString();
			}
			return sacrashComputer;
		}

		// Token: 0x04000136 RID: 310
		public string TargetComp;

		// Token: 0x04000137 RID: 311
		public string CrashSource;

		// Token: 0x04000138 RID: 312
		public string DelayHost;

		// Token: 0x04000139 RID: 313
		public float Delay;
	}
}
