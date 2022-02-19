using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000040 RID: 64
	public class SAKillExe : SerializableAction
	{
		// Token: 0x0600015C RID: 348 RVA: 0x00013BBC File Offset: 0x00011DBC
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				if (this.ExeName == "*")
				{
					this.ExeName = "";
				}
				for (int i = 0; i < os.exes.Count; i++)
				{
					if (os.exes[i].name.ToLower().Contains(this.ExeName.ToLower()))
					{
						os.exes[i].isExiting = true;
					}
				}
			}
			else
			{
				Computer computer = Programs.getComputer(os, this.DelayHost);
				if (computer == null)
				{
					throw new NullReferenceException("Computer " + computer + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction(this, delay);
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00013CC4 File Offset: 0x00011EC4
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAKillExe sakillExe = new SAKillExe();
			if (rdr.MoveToAttribute("Delay"))
			{
				sakillExe.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sakillExe.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("ExeName"))
			{
				sakillExe.ExeName = rdr.ReadContentAsString();
			}
			return sakillExe;
		}

		// Token: 0x0400015B RID: 347
		public string ExeName;

		// Token: 0x0400015C RID: 348
		public string DelayHost;

		// Token: 0x0400015D RID: 349
		public float Delay;
	}
}
