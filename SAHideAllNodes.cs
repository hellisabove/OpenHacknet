using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000047 RID: 71
	public class SAHideAllNodes : SerializableAction
	{
		// Token: 0x06000172 RID: 370 RVA: 0x0001492C File Offset: 0x00012B2C
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				OS os = (OS)os_obj;
				os.netMap.visibleNodes.Clear();
			}
			else
			{
				Computer computer = Programs.getComputer((OS)os_obj, this.DelayHost);
				if (computer == null)
				{
					throw new NullReferenceException("Computer " + computer + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction(this, delay);
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x000149BC File Offset: 0x00012BBC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAHideAllNodes sahideAllNodes = new SAHideAllNodes();
			if (rdr.MoveToAttribute("Delay"))
			{
				sahideAllNodes.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sahideAllNodes.DelayHost = rdr.ReadContentAsString();
			}
			return sahideAllNodes;
		}

		// Token: 0x04000174 RID: 372
		public string DelayHost;

		// Token: 0x04000175 RID: 373
		public float Delay;
	}
}
