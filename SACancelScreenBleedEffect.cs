using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200003E RID: 62
	public class SACancelScreenBleedEffect : SerializableAction
	{
		// Token: 0x06000156 RID: 342 RVA: 0x00013924 File Offset: 0x00011B24
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				os.EffectsUpdater.CancelScreenBleedEffect();
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

		// Token: 0x06000157 RID: 343 RVA: 0x000139AC File Offset: 0x00011BAC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SACancelScreenBleedEffect sacancelScreenBleedEffect = new SACancelScreenBleedEffect();
			if (rdr.MoveToAttribute("Delay"))
			{
				sacancelScreenBleedEffect.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sacancelScreenBleedEffect.DelayHost = rdr.ReadContentAsString();
			}
			return sacancelScreenBleedEffect;
		}

		// Token: 0x04000153 RID: 339
		public string DelayHost;

		// Token: 0x04000154 RID: 340
		public float Delay;
	}
}
