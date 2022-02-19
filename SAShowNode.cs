using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000048 RID: 72
	public class SAShowNode : SerializableAction
	{
		// Token: 0x06000175 RID: 373 RVA: 0x00014A1C File Offset: 0x00012C1C
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				OS os = (OS)os_obj;
				Computer computer = Programs.getComputer(os, this.Target);
				if (computer != null)
				{
					os.netMap.discoverNode(computer);
				}
				else if (OS.DEBUG_COMMANDS)
				{
					os.write("Error revealing node " + this.Target + " : NODE NOT FOUND");
				}
			}
			else
			{
				Computer computer2 = Programs.getComputer((OS)os_obj, this.DelayHost);
				if (computer2 == null)
				{
					throw new NullReferenceException("Computer " + computer2 + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer2).AddAction(this, delay);
			}
		}

		// Token: 0x06000176 RID: 374 RVA: 0x00014AF4 File Offset: 0x00012CF4
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAShowNode sashowNode = new SAShowNode();
			if (rdr.MoveToAttribute("Delay"))
			{
				sashowNode.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sashowNode.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Target"))
			{
				sashowNode.Target = rdr.ReadContentAsString();
			}
			return sashowNode;
		}

		// Token: 0x04000176 RID: 374
		public string DelayHost;

		// Token: 0x04000177 RID: 375
		public float Delay;

		// Token: 0x04000178 RID: 376
		public string Target;
	}
}
