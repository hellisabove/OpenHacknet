using System;

namespace Hacknet.Factions
{
	// Token: 0x020000DA RID: 218
	internal class EntropyFaction : Faction
	{
		// Token: 0x0600045F RID: 1119 RVA: 0x00045FA9 File Offset: 0x000441A9
		public EntropyFaction(string _name, int _neededValue) : base(_name, _neededValue)
		{
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x00045FB8 File Offset: 0x000441B8
		public override void addValue(int value, object os)
		{
			int playerValue = this.playerValue;
			base.addValue(value, os);
			if (base.valuePassedPoint(playerValue, 3))
			{
				((OS)os).Flags.AddFlag("eosPathStarted");
				ComputerLoader.loadMission("Content/Missions/Entropy/StartingSet/eosMissions/eosIntroDelayer.xml", false);
			}
			if (base.valuePassedPoint(playerValue, 4))
			{
				if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && ((OS)os).HasLoadedDLCContent)
				{
					try
					{
						Computer computer = Programs.getComputer((OS)os, "entropy00");
						if (computer == null)
						{
						}
						MissionListingServer missionListingServer = (MissionListingServer)computer.getDaemon(typeof(MissionListingServer));
						missionListingServer.addMisison((ActiveMission)ComputerLoader.readMission("Content/DLC/Missions/BaseGameConnectors/Missions/EntropyDLCConnectorIntro.xml"), true);
						Console.WriteLine("Injected Labyrinths transition mission to Entropy");
					}
					catch (Exception ex)
					{
						Utils.AppendToErrorFile("Could not add in Labyrinths upgrade mission to entropy!\r\n\r\n" + Utils.GenerateReportFromException(ex));
					}
				}
			}
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x000460FC File Offset: 0x000442FC
		public override void playerPassedValue(object os)
		{
			base.playerPassedValue(os);
			if (Settings.isAlphaDemoMode)
			{
				ComputerLoader.loadMission("Content/Missions/Entropy/EntropyMission3.xml", false);
			}
			else
			{
				((OS)os).delayer.Post(ActionDelayer.Wait(1.7), delegate
				{
					ComputerLoader.loadMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", false);
					((OS)os).saveGame();
				});
			}
		}
	}
}
