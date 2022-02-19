using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200003A RID: 58
	public class SAAddConditionalActions : SerializableAction
	{
		// Token: 0x0600014A RID: 330 RVA: 0x00013070 File Offset: 0x00011270
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				RunnableConditionalActions.LoadIntoOS(this.Filepath, os_obj);
			}
			else
			{
				OS os = (OS)os_obj;
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

		// Token: 0x0600014B RID: 331 RVA: 0x000130F8 File Offset: 0x000112F8
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAddConditionalActions saaddConditionalActions = new SAAddConditionalActions();
			if (rdr.MoveToAttribute("Filepath"))
			{
				saaddConditionalActions.Filepath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Delay"))
			{
				saaddConditionalActions.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				saaddConditionalActions.DelayHost = rdr.ReadContentAsString();
			}
			if (string.IsNullOrWhiteSpace(saaddConditionalActions.Filepath))
			{
				throw new FormatException("Invalid Filepath");
			}
			return saaddConditionalActions;
		}

		// Token: 0x0400013F RID: 319
		public string Filepath;

		// Token: 0x04000140 RID: 320
		public string DelayHost;

		// Token: 0x04000141 RID: 321
		public float Delay;
	}
}
