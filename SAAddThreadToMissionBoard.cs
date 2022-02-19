using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000036 RID: 54
	public class SAAddThreadToMissionBoard : SerializableAction
	{
		// Token: 0x0600013E RID: 318 RVA: 0x000129C0 File Offset: 0x00010BC0
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddThreadToMissionBoard Function, adding thread: " + this.ThreadFilepath);
			}
			MessageBoardDaemon messageBoardDaemon = computer.getDaemon(typeof(MessageBoardDaemon)) as MessageBoardDaemon;
			if (messageBoardDaemon == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MessageBoard daemon for SAAddThreadToMissionBoard function adding thread: " + this.ThreadFilepath);
			}
			string threadData = Utils.readEntireFile(Utils.GetFileLoadPrefix() + this.ThreadFilepath);
			messageBoardDaemon.AddThread(threadData);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00012A74 File Offset: 0x00010C74
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAddThreadToMissionBoard saaddThreadToMissionBoard = new SAAddThreadToMissionBoard();
			if (rdr.MoveToAttribute("ThreadFilepath"))
			{
				saaddThreadToMissionBoard.ThreadFilepath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saaddThreadToMissionBoard.TargetComp = rdr.ReadContentAsString();
			}
			if (string.IsNullOrWhiteSpace(saaddThreadToMissionBoard.ThreadFilepath))
			{
				throw new FormatException("Invalid MissionFilepath");
			}
			if (string.IsNullOrWhiteSpace(saaddThreadToMissionBoard.TargetComp))
			{
				throw new FormatException("Invalid TargetComp");
			}
			return saaddThreadToMissionBoard;
		}

		// Token: 0x04000130 RID: 304
		public string ThreadFilepath;

		// Token: 0x04000131 RID: 305
		public string TargetComp;
	}
}
