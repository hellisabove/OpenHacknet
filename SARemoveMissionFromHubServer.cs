using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000035 RID: 53
	public class SARemoveMissionFromHubServer : SerializableAction
	{
		// Token: 0x0600013B RID: 315 RVA: 0x00012820 File Offset: 0x00010A20
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SARemoveMissionFromHubServer Function, removing mission: " + this.MissionFilepath);
			}
			MissionHubServer missionHubServer = computer.getDaemon(typeof(MissionHubServer)) as MissionHubServer;
			if (missionHubServer != null)
			{
				missionHubServer.RemoveMissionFromListings(this.MissionFilepath);
			}
			else
			{
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				if (dlchubServer != null)
				{
					dlchubServer.RemoveMission(this.MissionFilepath);
				}
				else
				{
					MissionListingServer missionListingServer = computer.getDaemon(typeof(MissionListingServer)) as MissionListingServer;
					if (missionListingServer == null)
					{
						throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MissionHubServer, MissionListingServer or DLCHubServer daemon for remove mission function adding mission: " + this.MissionFilepath);
					}
					missionListingServer.removeMission(this.MissionFilepath);
				}
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00012928 File Offset: 0x00010B28
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SARemoveMissionFromHubServer saremoveMissionFromHubServer = new SARemoveMissionFromHubServer();
			if (rdr.MoveToAttribute("MissionFilepath"))
			{
				saremoveMissionFromHubServer.MissionFilepath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saremoveMissionFromHubServer.TargetComp = rdr.ReadContentAsString();
			}
			if (string.IsNullOrWhiteSpace(saremoveMissionFromHubServer.MissionFilepath))
			{
				throw new FormatException("Invalid MissionFilepath");
			}
			if (string.IsNullOrWhiteSpace(saremoveMissionFromHubServer.TargetComp))
			{
				throw new FormatException("Invalid TargetComp");
			}
			return saremoveMissionFromHubServer;
		}

		// Token: 0x0400012E RID: 302
		public string MissionFilepath;

		// Token: 0x0400012F RID: 303
		public string TargetComp;
	}
}
