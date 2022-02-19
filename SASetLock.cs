using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000049 RID: 73
	public class SASetLock : SerializableAction
	{
		// Token: 0x06000178 RID: 376 RVA: 0x00014B70 File Offset: 0x00012D70
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				OS os = (OS)os_obj;
				string text = this.Module.ToLower();
				if (text != null)
				{
					if (!(text == "terminal"))
					{
						if (!(text == "netmap"))
						{
							if (!(text == "ram"))
							{
								if (text == "display")
								{
									os.display.inputLocked = this.IsLocked;
									os.display.visible = !this.IsHidden;
								}
							}
							else
							{
								os.ram.inputLocked = this.IsLocked;
								os.ram.visible = !this.IsHidden;
							}
						}
						else
						{
							os.netMap.inputLocked = this.IsLocked;
							os.netMap.visible = !this.IsHidden;
						}
					}
					else
					{
						os.terminal.inputLocked = this.IsLocked;
						os.terminal.visible = !this.IsHidden;
					}
				}
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

		// Token: 0x06000179 RID: 377 RVA: 0x00014CE0 File Offset: 0x00012EE0
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SASetLock sasetLock = new SASetLock();
			if (rdr.MoveToAttribute("Delay"))
			{
				sasetLock.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sasetLock.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Module"))
			{
				sasetLock.Module = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("IsLocked"))
			{
				sasetLock.IsLocked = rdr.ReadContentAsString().ToLower().StartsWith("t");
			}
			if (rdr.MoveToAttribute("IsHidden"))
			{
				sasetLock.IsHidden = rdr.ReadContentAsString().ToLower().StartsWith("t");
			}
			return sasetLock;
		}

		// Token: 0x04000179 RID: 377
		public string DelayHost;

		// Token: 0x0400017A RID: 378
		public float Delay;

		// Token: 0x0400017B RID: 379
		public string Module;

		// Token: 0x0400017C RID: 380
		public bool IsLocked = false;

		// Token: 0x0400017D RID: 381
		public bool IsHidden = false;
	}
}
