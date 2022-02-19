using System;

namespace Hacknet.Misc
{
	// Token: 0x02000085 RID: 133
	public static class SessionAccelerator
	{
		// Token: 0x060002A7 RID: 679 RVA: 0x00027208 File Offset: 0x00025408
		public static void AccelerateSessionToDLCHA(object osObj)
		{
			OS os = (OS)osObj;
			os.Flags.AddFlag("TutorialComplete");
			os.delayer.RunAllDelayedActions();
			os.allFactions.setCurrentFaction("Bibliotheque", os);
			os.IsInDLCMode = true;
			ThemeManager.setThemeOnComputer(os.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
			ThemeManager.switchTheme(os, "DLC/Themes/RiptideThemeStandard.xml");
			os.netMap.discoverNode(Programs.getComputer(os, "dhs"));
			MissionFunctions.runCommand(0, "setFaction:Bibliotheque");
			Computer computer = Programs.getComputer(os, "dhs");
			DLCHubServer dlchubServer = (DLCHubServer)computer.getDaemon(typeof(DLCHubServer));
			int num = 11;
			for (int i = 0; i < num; i++)
			{
				MissionFunctions.runCommand(1, "addRankSilent");
				if (i + 1 < num)
				{
					os.delayer.RunAllDelayedActions();
					dlchubServer.DelayedActions.InstantlyResolveAllActions(os);
					dlchubServer.ClearAllActiveMissions();
				}
			}
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 6881);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 211);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 31);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 443);
			MissionFunctions.runCommand(9, "changeSongDLC");
		}

		// Token: 0x060002A8 RID: 680 RVA: 0x0002735C File Offset: 0x0002555C
		public static void AccelerateSessionToDLCEND(object osObj)
		{
			OS os = (OS)osObj;
			os.Flags.AddFlag("TutorialComplete");
			os.delayer.RunAllDelayedActions();
			os.allFactions.setCurrentFaction("Bibliotheque", os);
			os.IsInDLCMode = true;
			ThemeManager.setThemeOnComputer(os.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
			ThemeManager.switchTheme(os, "DLC/Themes/RiptideThemeStandard.xml");
			os.netMap.discoverNode(Programs.getComputer(os, "dhs"));
			MissionFunctions.runCommand(0, "setFaction:Bibliotheque");
			Computer computer = Programs.getComputer(os, "dhs");
			DLCHubServer dlchubServer = (DLCHubServer)computer.getDaemon(typeof(DLCHubServer));
			int num = 11;
			for (int i = 0; i < num; i++)
			{
				MissionFunctions.runCommand(1, "addRankSilent");
				if (i + 1 < num)
				{
					os.delayer.RunAllDelayedActions();
					dlchubServer.DelayedActions.InstantlyResolveAllActions(os);
					dlchubServer.ClearAllActiveMissions();
				}
			}
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 6881);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 211);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 31);
			SessionAccelerator.AddProgramToComputer(os.thisComputer, 443);
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x000275AC File Offset: 0x000257AC
		public static void AccelerateSessionToDLCStart(object osObj)
		{
			OS os = (OS)osObj;
			os.Flags.AddFlag("TutorialComplete");
			os.delayer.RunAllDelayedActions();
			ThemeManager.setThemeOnComputer(os.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
			ThemeManager.switchTheme(os, "DLC/Themes/RiptideClassicTheme.xml");
			for (int i = 0; i < 60; i++)
			{
				int index;
				do
				{
					index = Utils.random.Next(os.netMap.nodes.Count);
				}
				while (os.netMap.nodes[index].idName == "mainHub" || os.netMap.nodes[index].idName == "entropy00" || os.netMap.nodes[index].idName == "entropy01");
				os.netMap.discoverNode(os.netMap.nodes[index]);
			}
			os.netMap.lastAddedNode = os.thisComputer;
			os.delayer.Post(ActionDelayer.Wait(0.15), delegate
			{
				Game1.getSingleton().IsMouseVisible = true;
				os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], "SSHCrack.exe"));
				os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], "FTPBounce.exe"));
				os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[13], "eosDeviceScan.exe"));
				MissionFunctions.runCommand(7, "changeSong");
				MusicManager.stop();
			});
			os.delayer.Post(ActionDelayer.Wait(56.0), delegate
			{
				ComputerLoader.loadMission("Content/DLC/Missions/Demo/DLCDemoIntroMission1.xml", false);
			});
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00027784 File Offset: 0x00025984
		internal static void AddProgramToComputer(Computer c, int portnum)
		{
			FileEntry item = new FileEntry(PortExploits.crackExeData[portnum], PortExploits.cracks[portnum]);
			c.files.root.searchForFolder("bin").files.Add(item);
		}
	}
}
