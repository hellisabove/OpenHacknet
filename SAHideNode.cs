using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000042 RID: 66
	public class SAHideNode : SerializableAction
	{
		// Token: 0x06000163 RID: 355 RVA: 0x000141FC File Offset: 0x000123FC
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				Computer computer = Programs.getComputer(os, this.TargetComp);
				if (computer != null)
				{
					do
					{
						os.netMap.visibleNodes.Remove(os.netMap.nodes.IndexOf(computer));
					}
					while (os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(computer)));
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

		// Token: 0x06000164 RID: 356 RVA: 0x000142E0 File Offset: 0x000124E0
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAHideNode sahideNode = new SAHideNode();
			if (rdr.MoveToAttribute("Delay"))
			{
				sahideNode.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sahideNode.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				sahideNode.TargetComp = rdr.ReadContentAsString();
			}
			return sahideNode;
		}

		// Token: 0x04000164 RID: 356
		public string TargetComp;

		// Token: 0x04000165 RID: 357
		public string DelayHost;

		// Token: 0x04000166 RID: 358
		public float Delay;
	}
}
