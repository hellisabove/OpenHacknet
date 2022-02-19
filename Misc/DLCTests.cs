using System;
using System.Collections.Generic;
using System.Linq;
using Hacknet.Factions;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x0200007E RID: 126
	public static class DLCTests
	{
		// Token: 0x06000272 RID: 626 RVA: 0x000235A8 File Offset: 0x000217A8
		public static string TestDLCFunctionality(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "";
			int num = 0;
			bool enableDLC = Settings.EnableDLC;
			Settings.EnableDLC = true;
			text += DLCTests.TestCustomPortMapping(screenMan, out num);
			text += DLCTests.TestCustomPortMappingOnLoadedComputer(screenMan, out num);
			text += DLCTests.TestMemorySerialization(screenMan, out num);
			text += DLCTests.TestMemoryOnLoadedComputer(screenMan, out num);
			text += DLCTests.TestMemoryInjectionOnLoadedComputer(screenMan, out num);
			text += DLCTests.TestDLCProgression(screenMan, out num);
			text += DLCTests.TestDLCPasswordsCorrect(screenMan, out num);
			text += DLCTests.TestDLCMiscFilesCorrect(screenMan, out num);
			Settings.EnableDLC = enableDLC;
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				(text.Length > 10) ? "\r\n" : " ",
				"Complete - ",
				num,
				" errors found"
			});
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000273 RID: 627 RVA: 0x000238E4 File Offset: 0x00021AE4
		public static string TestCustomPortMapping(ScreenManager screenMan, out int errorsAdded)
		{
			int errors = 0;
			string ret = "";
			DLCTests.TestComputersForLoad(screenMan, delegate(Computer old, Computer current)
			{
				object ret;
				if ((old.PortRemapping == null && current.PortRemapping != null) || (old.PortRemapping != null && current.PortRemapping == null))
				{
					errors++;
					ret = ret + "\r\nPort Remapping Failed to load on computer " + current.name;
				}
				if (old.PortRemapping != null && current.PortRemapping != null)
				{
					foreach (KeyValuePair<int, int> keyValuePair in current.PortRemapping)
					{
						if (!old.PortRemapping.ContainsKey(keyValuePair.Key))
						{
							errors++;
							ret = ret;
							ret = string.Concat(new object[]
							{
								ret,
								"\r\n\r\nPort remapping save error on Computer ",
								current.name,
								"\r\n",
								keyValuePair.Key,
								" : ",
								keyValuePair.Value,
								" has no corresponding value"
							});
						}
						else if (old.PortRemapping[keyValuePair.Key] != keyValuePair.Value)
						{
							errors++;
							ret = ret;
							ret = string.Concat(new object[]
							{
								ret,
								"\r\n\r\nPort remapping save error on Computer ",
								current.name,
								"\r\nKey ",
								keyValuePair.Key,
								" : Current: ",
								keyValuePair.Value,
								" Old: ",
								old.PortRemapping[keyValuePair.Key]
							});
						}
					}
				}
			});
			errorsAdded = errors;
			return ret;
		}

		// Token: 0x06000274 RID: 628 RVA: 0x00023A10 File Offset: 0x00021C10
		public static string TestMemorySerialization(ScreenManager screenMan, out int errorsAdded)
		{
			int errors = 0;
			string ret = "";
			string[] wildcardUpgradeServerIDs = new string[]
			{
				"polarSnakeDest",
				"naixGateway",
				"BitWorkServer"
			};
			DLCTests.TestComputersForLoad(screenMan, delegate(Computer old, Computer current)
			{
				if ((old.Memory != null && current.Memory == null) || (old.Memory == null && current.Memory != null))
				{
					if (!wildcardUpgradeServerIDs.Contains(old.idName))
					{
						errors++;
						ret += "\r\n\r\nMemory serialization error: old and current null mismatch";
					}
				}
				if (old.Memory != null && current.Memory != null)
				{
					string text = old.Memory.TestEqualsWithErrorReport(current.Memory);
					if (text.Length > 0)
					{
						errors++;
						ret += text;
					}
				}
			});
			errorsAdded = errors;
			return ret;
		}

		// Token: 0x06000275 RID: 629 RVA: 0x00023A84 File Offset: 0x00021C84
		public static string TestCustomPortMappingOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			Computer computer = (Computer)ComputerLoader.loadComputer("Content/Tests/DLCTests/TestComp1.xml", false, false);
			if (computer.PortRemapping == null)
			{
				num++;
				text += "\r\nLoaded test comp1 Does not correctly load in port remapping.";
			}
			if (computer.PortRemapping.Count != 4)
			{
				num++;
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\r\nLoaded test comp reads in ",
					computer.PortRemapping.Count,
					" remaps, instead of the expected 4"
				});
			}
			if (computer.PortRemapping[22] != 1234 || computer.PortRemapping[21] != 99 || computer.PortRemapping[80] != 3 || computer.PortRemapping[1433] != 1432)
			{
				num++;
				text += "\r\nLoaded test comp reads in incorrect port mapping.";
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000276 RID: 630 RVA: 0x00023B9C File Offset: 0x00021D9C
		public static string TestMemoryOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			Computer computer = (Computer)ComputerLoader.loadComputer("Content/Tests/DLCTests/TestComp1.xml", false, false);
			if (computer.Memory == null)
			{
				num++;
				text += "\r\nLoaded test comp1 Does not correctly load in memory.";
			}
			if (computer.Memory.DataBlocks.Count != 3)
			{
				num++;
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\r\nLoaded test comp reads in ",
					computer.Memory.DataBlocks.Count,
					" data blocks in memory, instead of the expected 3"
				});
			}
			if (computer.Memory.CommandsRun.Count != 1)
			{
				num++;
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\r\nLoaded test comp reads in ",
					computer.Memory.CommandsRun.Count,
					" commands run in memory, instead of the expected 1"
				});
			}
			else if (computer.Memory.CommandsRun[0] != "connect 123.123.123.123")
			{
				num++;
				text = text + "\r\nLoaded test comp reads in " + computer.Memory.CommandsRun[0] + " as command, instead of the expected value";
			}
			string encodedFileString = computer.Memory.GetEncodedFileString();
			MemoryContents memoryFromEncodedFileString = MemoryContents.GetMemoryFromEncodedFileString(encodedFileString);
			string text2 = memoryFromEncodedFileString.TestEqualsWithErrorReport(computer.Memory);
			if (text2.Length > 0)
			{
				num++;
				text = text + "\r\nErrors in Memory file serialization cycle!\r\n" + text2 + "\n\n";
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000277 RID: 631 RVA: 0x00023D58 File Offset: 0x00021F58
		public static string TestMemoryInjectionOnLoadedComputer(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			Computer computer = (Computer)ComputerLoader.loadComputer("Content/Tests/DLCTests/TestCompNoMemory.xml", false, false);
			if (computer.Memory != null)
			{
				num++;
				text += "\r\nLoaded test comp for no memory does infact have memory somehow????\r\n";
			}
			else
			{
				MemoryDumpInjector.InjectMemory("Content/Tests/DLCTests/InjectedMemory.xml", computer);
				if (computer.Memory == null)
				{
					num++;
					text += "\r\nInjecting memory into loaded comp failed!\r\n";
				}
				else if (computer.Memory.DataBlocks.Count != 2)
				{
					num++;
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"\r\nLoaded test comp reads in ",
						computer.Memory.DataBlocks.Count,
						" data blocks in memory, instead of the expected 2"
					});
				}
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x0002425C File Offset: 0x0002245C
		public static string TestDLCProgression(ScreenManager screenMan, out int errorsAdded)
		{
			int errors = 0;
			string ret = "";
			DLCTests.SetupTestingEnvironment(screenMan, delegate(OS os, List<Computer> comps)
			{
				Console.WriteLine("Testing DLC Progression in " + Settings.ActiveLocale);
				SessionAccelerator.AccelerateSessionToDLCEND(os);
				Computer computer = Programs.getComputer(os, "dhsDrop");
				Folder folder = computer.files.root.searchForFolder("home");
				if (folder.searchForFile("p_SQL_399.gz") == null)
				{
					errors++;
					ret += "\r\nExpo Grave Mission files not copied over to drop server! Mission Incompletable.";
				}
				Computer computer2 = Programs.getComputer(os, "ds4_expo");
				if (!computer2.ports.Contains(21))
				{
					errors++;
					ret += "\r\nExpo Grave Website does not have port 21 added! Mission Incompletable.";
				}
				Computer computer3 = Programs.getComputer(os, "ds3_mail");
				Folder folder2 = computer3.files.root.searchForFolder("mail").searchForFolder("accounts");
				Folder folder3 = folder2.searchForFolder("cornch1p");
				if (folder3 == null)
				{
					errors++;
					ret += "\r\nMagma Mailbox server for It Follows (set3) does not have account! in file, replace kburnaby with cornch1p to fix.";
				}
				else
				{
					Folder folder4 = folder3.searchForFolder("inbox");
					if (folder4.files.Count <= 0)
					{
						errors++;
						ret += "\r\nMagma Mailbox server for It Follows (set3) does not have emails! in file, replace kburnaby with cornch1p to fix.";
					}
				}
				CustomFaction customFaction = CustomFaction.ParseFromFile("Content/DLC/DLCFaction.xml");
				if (customFaction.idName != "Bibliotheque")
				{
					errors++;
					ret = ret + "\r\nHub Faction ID Name is wrong: " + customFaction.idName;
				}
				CustomFactionAction customFactionAction = customFaction.CustomActions[6];
				for (int i = 0; i < customFactionAction.TriggerActions.Count; i++)
				{
					SAAddConditionalActions saaddConditionalActions = customFactionAction.TriggerActions[i] as SAAddConditionalActions;
					if (saaddConditionalActions != null)
					{
						if (saaddConditionalActions.Filepath.Contains("PetsAcceptedActions"))
						{
							errors++;
							ret += "\r\nHub Faction (stage 7) contains PetsAcceptedActions.xml loader - it should be only on the mission itself! Remove it!";
						}
					}
				}
				Computer computer4 = Programs.getComputer(os, "dPets_MF");
				if (computer4.name.ToLower().Contains("digipets"))
				{
					errors++;
					ret += "\r\nNeopals server rename from DigiPets is not complete in this version! Fix it! Remember to do foldernames too.\r\n";
				}
				bool flag = false;
				Computer computer5 = Programs.getComputer(os, "dMF_1_Misc");
				for (int i = 0; i < computer5.users.Count; i++)
				{
					if (computer5.users[i].name == "listen" && computer5.users[i].pass == "4TL4S")
					{
						flag = true;
					}
				}
				if (!flag)
				{
					errors++;
					ret += "\r\nAtlas server (MemForensics/Atlas) needs user listen w. pass 4TL4S\r\n";
				}
				ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission("Content/DLC/Missions/Neopals/PetsMission1.xml");
				if (activeMission.email.body.Contains("DigiPoints"))
				{
					errors++;
					ret += "\r\nNeopals missions references DigiPoints, where it should be NeoPoints! Remember to check the goals field!\r\n";
				}
				Computer computer6 = Programs.getComputer(os, "dhsDrop");
				if (computer6.admin != null)
				{
					errors++;
					ret += "\r\nDLC Drop server has an admin - remove it with type=none. This stops the player being auto logged out.\r\n";
				}
			});
			errorsAdded = errors;
			return ret;
		}

		// Token: 0x06000279 RID: 633 RVA: 0x00024324 File Offset: 0x00022524
		public static string TestDLCPasswordsCorrect(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string ret = "";
			DLCTests.SetupTestingEnvironment(screenMan, delegate(OS os, List<Computer> comps)
			{
				ret += DLCTests.TestPassword(os, "ds4_grave", "fma93dK");
				ret += DLCTests.TestPassword(os, "dpae_psy_1", "catsarebestpet");
				ret += DLCTests.TestPassword(os, "dpa_psylance", "1185JACK");
			});
			errorsAdded = num;
			return ret;
		}

		// Token: 0x0600027A RID: 634 RVA: 0x00024368 File Offset: 0x00022568
		public static string TestDLCMiscFilesCorrect(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			string text2 = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial2.txt");
			if (!text2.Contains("216.239.32.181"))
			{
				num++;
				text += "\r\nDLC/Docs/KaguyaTrial2.txt Does not have the correct IP! Needs 216.239.32.181";
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600027B RID: 635 RVA: 0x000243EC File Offset: 0x000225EC
		private static string TestPassword(OS os, string compID, string passToFind)
		{
			Computer computer = Programs.getComputer(os, compID);
			string result;
			if (computer == null)
			{
				result = string.Concat(new string[]
				{
					"\r\nComputer ",
					compID,
					" Not found when searching for password ",
					passToFind,
					"\r\n"
				});
			}
			else
			{
				bool hasPass = false;
				DLCTests.CheckAllFiles(computer.files.root, delegate(FileEntry f)
				{
					if (f.data.Contains(passToFind))
					{
						hasPass = true;
					}
				});
				if (!hasPass)
				{
					result = string.Concat(new string[]
					{
						"\r\nComputer ",
						compID,
						" (",
						computer.name,
						") Does not contain necessary password: ",
						passToFind,
						"\r\n"
					});
				}
				else
				{
					result = "";
				}
			}
			return result;
		}

		// Token: 0x0600027C RID: 636 RVA: 0x000244E4 File Offset: 0x000226E4
		private static void CheckAllFiles(Folder f, Action<FileEntry> act)
		{
			for (int i = 0; i < f.files.Count; i++)
			{
				if (act != null)
				{
					act(f.files[i]);
				}
			}
			for (int i = 0; i < f.folders.Count; i++)
			{
				DLCTests.CheckAllFiles(f.folders[i], act);
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x00024558 File Offset: 0x00022758
		internal static void SetupTestingEnvironment(ScreenManager screenMan, Action<OS, List<Computer>> CompareSessions)
		{
			string text = "__hacknettestaccount";
			string pass = "__testingpassword";
			SaveFileManager.AddUser(text, pass);
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text);
			OS.TestingPassOnly = true;
			OS os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			os.delayer.RunAllDelayedActions();
			os.threadedSaveExecute(false);
			List<Computer> nodes = os.netMap.nodes;
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			os.delayer.RunAllDelayedActions();
			Game1.getSingleton().IsMouseVisible = true;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			CompareSessions(os, nodes);
			screenMan.RemoveScreen(os);
			OS.TestingPassOnly = false;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000246A0 File Offset: 0x000228A0
		internal static void TestComputersForLoad(ScreenManager screenMan, Action<Computer, Computer> CompareComputerAfterLoad)
		{
			DLCTests.SetupTestingEnvironment(screenMan, delegate(OS os, List<Computer> oldComps)
			{
				for (int i = 0; i < oldComps.Count; i++)
				{
					Computer computer = oldComps[i];
					Computer computer2 = Programs.getComputer(os, computer.ip);
					CompareComputerAfterLoad(computer, computer2);
				}
			});
		}
	}
}
