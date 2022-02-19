using System;
using System.Collections.Generic;
using System.IO;
using Hacknet.Factions;
using Hacknet.Mission;

namespace Hacknet
{
	// Token: 0x02000194 RID: 404
	internal class DLC1SessionUpgrader
	{
		// Token: 0x06000A2F RID: 2607 RVA: 0x000A2574 File Offset: 0x000A0774
		public static void CheckForDLCFiles()
		{
			if (File.Exists("Content/DLC/DLCFaction.xml"))
			{
				DLC1SessionUpgrader.HasDLC1Installed = true;
			}
			else
			{
				DLC1SessionUpgrader.HasDLC1Installed = false;
			}
		}

		// Token: 0x06000A30 RID: 2608 RVA: 0x000A25A4 File Offset: 0x000A07A4
		public static void UpgradeSession(object osobj, bool needsNodeInjection)
		{
			OS os = (OS)osobj;
			MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/GibsonLink.xml"), Programs.getComputer(os, "polarSnakeDest"));
			MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/NaixHome.xml"), Programs.getComputer(os, "naixGateway"));
			MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/BitDropBox.xml"), Programs.getComputer(os, "BitWorkServer"));
			MemoryDumpInjector.InjectMemory(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Missions/Injections/MemoryDumps/ExpandKeysInjection.xml"), Programs.getComputer(os, "portcrack01"));
			if (os.thisComputer.Memory == null)
			{
				os.thisComputer.Memory = new MemoryContents();
			}
			os.allFactions.factions.Add("Bibliotheque", CustomFaction.ParseFromFile("Content/DLC/DLCFaction.xml"));
			if (needsNodeInjection)
			{
				if (People.all == null)
				{
					People.init();
				}
				else if (!People.PeopleWereGeneratedWithDLCAdditions)
				{
					People.LoadInDLCPeople();
				}
				List<string> dlclist = BootLoadList.getDLCList();
				for (int i = 0; i < dlclist.Count; i++)
				{
					Computer.loadFromFile(dlclist[i]);
				}
				ComputerLoader.postAllLoadedActions();
			}
			Computer computer = Programs.getComputer(os, "ispComp");
			if (!computer.ports.Contains(443))
			{
				computer.ports.Add(443);
			}
			if (!computer.ports.Contains(6881))
			{
				computer.ports.Add(6881);
			}
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x000A273C File Offset: 0x000A093C
		public static void EndDLCSection(object osobj)
		{
			OS os = (OS)osobj;
			os.IsInDLCMode = false;
			os.mailicon.isEnabled = true;
			os.Flags.AddFlag("dlc_complete");
			if (os.Flags.HasFlag("dlc_start_csec"))
			{
				os.Flags.AddFlag("dlc_complete_FromCSEC");
				ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLC_EndEmail.xml", false);
				os.allFactions.setCurrentFaction("hub", os);
				os.homeNodeID = "mainHub";
				os.homeAssetServerID = "mainHubAssets";
				os.Flags.AddFlag("dlc_csec_end_facval:" + os.currentFaction.playerValue);
			}
			else if (os.Flags.HasFlag("dlc_start_entropy"))
			{
				os.Flags.AddFlag("dlc_complete_FromEntropy");
				ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/Entropy_DLC_EndEmail.xml", false);
				os.allFactions.setCurrentFaction("entropy", os);
				os.homeNodeID = "entropy00";
				os.homeAssetServerID = "entropy01";
			}
			else
			{
				os.Flags.AddFlag("dlc_complete_FromUnknown");
				ComputerLoader.loadMission("Content/DLC/Missions/BaseGameConnectors/Missions/Entropy_DLC_EndEmail.xml", false);
				os.allFactions.setCurrentFaction("entropy", os);
				os.homeNodeID = "entropy00";
				os.homeAssetServerID = "entropy01";
			}
			DLC1SessionUpgrader.ReDsicoverAllVisibleNodesInOSCache(os);
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x000A29A4 File Offset: 0x000A0BA4
		public static void ReDsicoverAllVisibleNodesInOSCache(object osobj)
		{
			OS os = (OS)osobj;
			float num = 0f;
			float num2 = 8f;
			float num3 = num2 / (float)os.netMap.nodes.Count;
			string[] array = os.PreDLCVisibleNodesCache.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				try
				{
					int nIndex = Convert.ToInt32(array[i]);
					os.delayer.Post(ActionDelayer.Wait((double)num), delegate
					{
						os.netMap.visibleNodes.Add(nIndex);
						os.netMap.nodes[nIndex].highlightFlashTime = 1f;
						SFX.addCircle(os.netMap.nodes[nIndex].getScreenSpacePosition(), Utils.AddativeWhite * 0.4f, 70f);
					});
					os.delayer.Post(ActionDelayer.Wait((double)num2), delegate
					{
						SFX.addCircle(os.netMap.nodes[nIndex].getScreenSpacePosition(), Utils.AddativeWhite * 0.3f, 30f);
					});
				}
				catch (FormatException)
				{
					Console.WriteLine("Error restoring node " + i);
				}
				num += num3;
				os.PreDLCVisibleNodesCache = "";
			}
		}

		// Token: 0x04000B7B RID: 2939
		public static bool HasDLC1Installed = false;
	}
}
