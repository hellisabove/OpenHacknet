using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200003D RID: 61
	public class SAStartScreenBleedEffect : SerializableAction
	{
		// Token: 0x06000153 RID: 339 RVA: 0x00013754 File Offset: 0x00011954
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				string[] collection = this.ContentLines.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				List<string> list = new List<string>(collection);
				while (list.Count < 3)
				{
					list.Add("");
				}
				os.EffectsUpdater.StartScreenBleed(this.TotalDurationSeconds, this.AlertTitle, list[0], list[1], list[2], this.CompleteAction);
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

		// Token: 0x06000154 RID: 340 RVA: 0x00013840 File Offset: 0x00011A40
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAStartScreenBleedEffect sastartScreenBleedEffect = new SAStartScreenBleedEffect();
			if (rdr.MoveToAttribute("Delay"))
			{
				sastartScreenBleedEffect.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sastartScreenBleedEffect.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("AlertTitle"))
			{
				sastartScreenBleedEffect.AlertTitle = ComputerLoader.filter(rdr.ReadContentAsString());
			}
			if (rdr.MoveToAttribute("CompleteAction"))
			{
				sastartScreenBleedEffect.CompleteAction = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TotalDurationSeconds"))
			{
				sastartScreenBleedEffect.TotalDurationSeconds = (float)rdr.ReadContentAsDouble();
			}
			rdr.MoveToContent();
			sastartScreenBleedEffect.ContentLines = ComputerLoader.filter(rdr.ReadElementContentAsString());
			return sastartScreenBleedEffect;
		}

		// Token: 0x0400014D RID: 333
		[XMLContent]
		public string ContentLines;

		// Token: 0x0400014E RID: 334
		public string AlertTitle;

		// Token: 0x0400014F RID: 335
		public string CompleteAction;

		// Token: 0x04000150 RID: 336
		public float TotalDurationSeconds = 200f;

		// Token: 0x04000151 RID: 337
		public string DelayHost;

		// Token: 0x04000152 RID: 338
		public float Delay;
	}
}
