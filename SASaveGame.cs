using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000046 RID: 70
	public class SASaveGame : SerializableAction
	{
		// Token: 0x0600016F RID: 367 RVA: 0x00014844 File Offset: 0x00012A44
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				OS os = (OS)os_obj;
				os.saveGame();
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

		// Token: 0x06000170 RID: 368 RVA: 0x000148CC File Offset: 0x00012ACC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SASaveGame sasaveGame = new SASaveGame();
			if (rdr.MoveToAttribute("Delay"))
			{
				sasaveGame.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sasaveGame.DelayHost = rdr.ReadContentAsString();
			}
			return sasaveGame;
		}

		// Token: 0x04000172 RID: 370
		public string DelayHost;

		// Token: 0x04000173 RID: 371
		public float Delay;
	}
}
