using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000031 RID: 49
	public class SARunFunction : SerializableAction
	{
		// Token: 0x0600012F RID: 303 RVA: 0x000120A0 File Offset: 0x000102A0
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				MissionFunctions.runCommand(this.FunctionValue, this.FunctionName);
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

		// Token: 0x06000130 RID: 304 RVA: 0x0001212C File Offset: 0x0001032C
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SARunFunction sarunFunction = new SARunFunction();
			if (rdr.MoveToAttribute("FunctionName"))
			{
				sarunFunction.FunctionName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("FunctionValue"))
			{
				sarunFunction.FunctionValue = rdr.ReadContentAsInt();
			}
			if (rdr.MoveToAttribute("Delay"))
			{
				sarunFunction.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sarunFunction.DelayHost = rdr.ReadContentAsString();
			}
			if (string.IsNullOrWhiteSpace(sarunFunction.FunctionName))
			{
				throw new FormatException("Invalid function name :" + sarunFunction.FunctionName);
			}
			return sarunFunction;
		}

		// Token: 0x0400011A RID: 282
		public string FunctionName;

		// Token: 0x0400011B RID: 283
		public int FunctionValue = 0;

		// Token: 0x0400011C RID: 284
		public float Delay = 0f;

		// Token: 0x0400011D RID: 285
		public string DelayHost;
	}
}
