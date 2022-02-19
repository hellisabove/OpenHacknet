using System;
using System.Collections.Generic;
using Hacknet.Mission;

namespace Hacknet.Misc
{
	// Token: 0x02000084 RID: 132
	public static class SaveFixHacks
	{
		// Token: 0x060002A5 RID: 677 RVA: 0x00026C18 File Offset: 0x00024E18
		public static void FixSavesWithTerribleHacks(object osObj)
		{
			OS os = (OS)osObj;
			Computer computer = Programs.getComputer(os, "mainHubAssets");
			if (computer != null)
			{
				Folder folder = computer.files.root.searchForFolder("bin");
				if (folder != null)
				{
					folder = folder.searchForFolder("Sequencer");
					if (folder != null)
					{
						FileEntry fileEntry = folder.searchForFile("Sequencer.exe");
						if (fileEntry == null)
						{
							folder.files.Add(new FileEntry(PortExploits.crackExeData[17], "Sequencer.exe"));
						}
						else
						{
							fileEntry.data = PortExploits.crackExeData[17];
						}
					}
				}
			}
			Computer computer2 = Programs.getComputer(os, "pacemakerSW_BE");
			if (computer2 != null)
			{
				Console.WriteLine("Searching for pacemaker comp");
				Folder folder = computer2.files.root.searchForFolder("projects");
				if (folder != null)
				{
					Console.WriteLine("Searching for pacemaker projects");
					folder = folder.searchForFolder("KellisBT");
					if (folder != null)
					{
						folder = folder.searchForFolder("Tests");
						if (folder != null)
						{
							Console.WriteLine("Searching for pacemaker file");
							FileEntry fileEntry = folder.searchForFile("PacemakerFirmware_Cycle_Test.dll");
							if (fileEntry == null)
							{
								folder.files.Add(new FileEntry(PortExploits.DangerousPacemakerFirmware, "PacemakerFirmware_Cycle_Test.dll"));
							}
							else
							{
								fileEntry.data = PortExploits.DangerousPacemakerFirmware;
							}
						}
					}
				}
			}
			if (os.HasLoadedDLCContent)
			{
				List<Computer> list = new List<Computer>();
				Computer computer3 = Programs.getComputer(os, "dPets_MF");
				if (computer3.links.Count == 0)
				{
					ComputerLoader.postAllLoadedActions = null;
					List<string> dlclist = BootLoadList.getDLCList();
					for (int i = 0; i < dlclist.Count; i++)
					{
						Computer item = (Computer)ComputerLoader.loadComputer(dlclist[i], true, true);
						list.Add(item);
					}
					ComputerLoader.postAllLoadedActions();
				}
				for (int j = 0; j < list.Count; j++)
				{
					Computer computer4 = Programs.getComputer(os, list[j].idName);
					computer4.links = list[j].links;
				}
				Computer computer5 = Programs.getComputer(os, "dPets_MF");
				Folder folder2 = computer5.files.root.searchForFolder("Database");
				bool flag = false;
				if (folder2.files.Count > 0 && folder2.files[0].data.Contains("DigiPet"))
				{
					for (int j = 0; j < folder2.files.Count; j++)
					{
						folder2.files[j].data = folder2.files[j].data.Replace("DigiPet", "Neopal");
						if (folder2.files[j].data.Contains("Minx"))
						{
							flag = true;
						}
					}
					if (!flag)
					{
					}
					Computer computer6 = Programs.getComputer(os, "dhs");
					DLCHubServer dlchubServer = (DLCHubServer)computer6.getDaemon(typeof(DLCHubServer));
					dlchubServer.navigatedTo();
					if (dlchubServer.ActiveMissions.Count == 0)
					{
						dlchubServer.AddMission(os.currentMission, os.defaultUser.name, false);
					}
				}
				if (os.Flags.HasFlag("KaguyaTrialComplete") && !os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(Programs.getComputer(os, "dhs"))))
				{
					os.Flags.RemoveFlag("KaguyaTrialComplete");
				}
			}
		}

		// Token: 0x060002A6 RID: 678 RVA: 0x0002703C File Offset: 0x0002523C
		public static string GetReportOnHashCodeOfEmptyStringForOtherCLRVersion()
		{
			string data = Utils.readEntireFile("DebugTest.txt");
			string data2 = FileEncrypter.EncryptString("test content", "headercontent", "1.1.1.1.1", "12345asdf", null);
			Console.WriteLine("password 12345asdf hashes to: " + (ushort)"12345asdf".GetHashCode());
			for (ushort num = 0; num < 65535; num += 1)
			{
				string[] array = FileEncrypter.TestingDecryptString(data, num);
				if (array[0].Contains("Firmware"))
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"Match Found: ",
						num,
						" is valid match, current CLR produces: ",
						(ushort)"".GetHashCode()
					}));
				}
				else
				{
					if (num % 1000 == 0)
					{
						Console.Write(".");
					}
					if (num % 50000 == 0)
					{
						Console.WriteLine("");
					}
				}
				string[] array2 = FileEncrypter.TestingDecryptString(data2, num);
				if (array2[2] != null && array2[2].Contains("test") && array2[4] != null && array2[4] == "1" && array2[5] != null && array2[5] == "ENCODED")
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"Alt Match Found: ",
						num,
						" is valid match, current CLR produces: ",
						(ushort)"".GetHashCode()
					}));
				}
			}
			Console.WriteLine("Operation Complete");
			return "";
		}
	}
}
