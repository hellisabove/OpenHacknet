using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000034 RID: 52
	public class SAAddMissionToHubServer : SerializableAction
	{
		// Token: 0x06000138 RID: 312 RVA: 0x000125BC File Offset: 0x000107BC
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for SAAddMissionToHubServer Function, adding mission: " + this.MissionFilepath);
			}
			MissionHubServer missionHubServer = computer.getDaemon(typeof(MissionHubServer)) as MissionHubServer;
			if (missionHubServer != null)
			{
				missionHubServer.AddMissionToListings(Utils.GetFileLoadPrefix() + this.MissionFilepath, -1);
			}
			else
			{
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				if (dlchubServer != null)
				{
					dlchubServer.AddMission(Utils.GetFileLoadPrefix() + this.MissionFilepath, this.AssignmentTag, this.StartsComplete);
				}
				else
				{
					MissionListingServer missionListingServer = computer.getDaemon(typeof(MissionListingServer)) as MissionListingServer;
					if (missionListingServer == null)
					{
						throw new NullReferenceException("Computer " + this.TargetComp + " does not contain a MissionHubServer, MissionListingServer or DLCHubServer daemon for addMission function adding mission: " + this.MissionFilepath);
					}
					List<ActiveMission> branchMissions = os.branchMissions;
					ActiveMission m = (ActiveMission)ComputerLoader.readMission(Utils.GetFileLoadPrefix() + this.MissionFilepath);
					os.branchMissions = branchMissions;
					missionListingServer.addMisison(m, this.AssignmentTag.ToLower() == "top");
				}
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00012724 File Offset: 0x00010924
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAddMissionToHubServer saaddMissionToHubServer = new SAAddMissionToHubServer();
			if (rdr.MoveToAttribute("MissionFilepath"))
			{
				saaddMissionToHubServer.MissionFilepath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saaddMissionToHubServer.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("AssignmentTag"))
			{
				saaddMissionToHubServer.AssignmentTag = rdr.ReadContentAsString();
			}
			if (string.IsNullOrWhiteSpace(saaddMissionToHubServer.AssignmentTag))
			{
				saaddMissionToHubServer.AssignmentTag = null;
			}
			if (rdr.MoveToAttribute("StartsComplete"))
			{
				saaddMissionToHubServer.StartsComplete = (rdr.ReadContentAsString().ToLower() == "true");
			}
			if (string.IsNullOrWhiteSpace(saaddMissionToHubServer.MissionFilepath))
			{
				throw new FormatException("Invalid MissionFilepath");
			}
			if (string.IsNullOrWhiteSpace(saaddMissionToHubServer.TargetComp))
			{
				throw new FormatException("Invalid TargetComp");
			}
			return saaddMissionToHubServer;
		}

		// Token: 0x0400012A RID: 298
		public string MissionFilepath;

		// Token: 0x0400012B RID: 299
		public string TargetComp;

		// Token: 0x0400012C RID: 300
		public string AssignmentTag;

		// Token: 0x0400012D RID: 301
		public bool StartsComplete;
	}
}
