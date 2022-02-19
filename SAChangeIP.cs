using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000044 RID: 68
	public class SAChangeIP : SerializableAction
	{
		// Token: 0x06000169 RID: 361 RVA: 0x00014500 File Offset: 0x00012700
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				Computer computer = Programs.getComputer(os, this.TargetComp);
				if (computer != null)
				{
					if (string.IsNullOrWhiteSpace(this.NewIP) || this.NewIP.StartsWith("#RANDOM"))
					{
						this.NewIP = NetworkMap.generateRandomIP();
					}
					computer.ip = this.NewIP;
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

		// Token: 0x0600016A RID: 362 RVA: 0x000145D8 File Offset: 0x000127D8
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAChangeIP sachangeIP = new SAChangeIP();
			if (rdr.MoveToAttribute("Delay"))
			{
				sachangeIP.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sachangeIP.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				sachangeIP.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("NewIP"))
			{
				sachangeIP.NewIP = rdr.ReadContentAsString();
			}
			return sachangeIP;
		}

		// Token: 0x0400016B RID: 363
		public string TargetComp;

		// Token: 0x0400016C RID: 364
		public string NewIP;

		// Token: 0x0400016D RID: 365
		public string DelayHost;

		// Token: 0x0400016E RID: 366
		public float Delay;
	}
}
