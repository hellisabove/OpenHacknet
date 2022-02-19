using System;
using System.Xml;
using Hacknet.Daemons.Helpers;

namespace Hacknet
{
	// Token: 0x02000037 RID: 55
	public class SAAddIRCMessage : SerializableAction
	{
		// Token: 0x06000141 RID: 321 RVA: 0x00012B0C File Offset: 0x00010D0C
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddIRCMessage Function, adding message: " + this.Message);
			}
			IRCDaemon ircdaemon = computer.getDaemon(typeof(IRCDaemon)) as IRCDaemon;
			IRCSystem ircsystem;
			if (ircdaemon != null)
			{
				ircsystem = ircdaemon.System;
			}
			else
			{
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				if (dlchubServer == null)
				{
					throw new NullReferenceException("Computer " + this.TargetComp + " does not contain an IRC server daemon for SAAddIRCMessage function adding message: " + this.Message);
				}
				ircsystem = dlchubServer.IRCSystem;
			}
			if (this.Delay <= 0f)
			{
				if (Math.Abs(this.Delay) < 0.001f)
				{
					ircsystem.AddLog(this.Author, this.Message, null);
				}
				else
				{
					DateTime d = DateTime.Now;
					d -= TimeSpan.FromSeconds((double)this.Delay);
					string timestamp = d.Hour.ToString("00") + ":" + d.Minute.ToString("00");
					ircsystem.AddLog(this.Author, this.Message, timestamp);
				}
			}
			else
			{
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction(this, delay);
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00012CB4 File Offset: 0x00010EB4
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAddIRCMessage saaddIRCMessage = new SAAddIRCMessage();
			if (rdr.MoveToAttribute("Author"))
			{
				saaddIRCMessage.Author = ComputerLoader.filter(rdr.ReadContentAsString());
			}
			if (rdr.MoveToAttribute("Delay"))
			{
				saaddIRCMessage.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saaddIRCMessage.TargetComp = rdr.ReadContentAsString();
			}
			rdr.MoveToContent();
			saaddIRCMessage.Message = ComputerLoader.filter(rdr.ReadElementContentAsString());
			if (string.IsNullOrWhiteSpace(saaddIRCMessage.TargetComp))
			{
				throw new FormatException("Invalid Target Comp");
			}
			if (string.IsNullOrWhiteSpace(saaddIRCMessage.Message))
			{
				throw new FormatException("Invalid or Empty Message!");
			}
			return saaddIRCMessage;
		}

		// Token: 0x04000132 RID: 306
		[XMLContent]
		public string Message;

		// Token: 0x04000133 RID: 307
		public string Author;

		// Token: 0x04000134 RID: 308
		public string TargetComp;

		// Token: 0x04000135 RID: 309
		public float Delay = 0f;
	}
}
