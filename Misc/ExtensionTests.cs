using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Hacknet.Extensions;
using Hacknet.Factions;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x02000080 RID: 128
	public static class ExtensionTests
	{
		// Token: 0x06000281 RID: 641 RVA: 0x00024884 File Offset: 0x00022A84
		public static string TestExtensions(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "";
			int num = 0;
			text += ExtensionTests.TestBlankExtension(screenMan, out num);
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"\r\nComplete - ",
				num,
				" errors found"
			});
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000282 RID: 642 RVA: 0x000248E4 File Offset: 0x00022AE4
		public static string TestExtensionCustomStartSong(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string result = "";
			ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo("Content/Tests/TestExtension");
			ExtensionInfo activeExtensionInfo = ExtensionLoader.ActiveExtensionInfo;
			if (activeExtensionInfo.IntroStartupSong != null)
			{
				string text = "Content/Music/" + activeExtensionInfo.IntroStartupSong;
				if (File.Exists(text))
				{
					MusicManager.loadAsCurrentSong(text);
				}
			}
			errorsAdded = num;
			return result;
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00024954 File Offset: 0x00022B54
		public static string TestExtensionForRuntime(ScreenManager screenMan, string path, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = null;
			ExtensionTests.RuntimeLoadAdditionalErrors = "";
			try
			{
				os = (OS)ExtensionTests.SetupOSForTests(path, screenMan);
				string text2 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, os);
				if (!string.IsNullOrWhiteSpace(text2))
				{
					num++;
					text = text + "\n\nSTART MISSION READ ERRORS:\r\n" + text2;
				}
				int num2 = 0;
				text2 = ExtensionTests.TestAllExtensionMissions(os, out num2);
				text += text2;
				num += num2;
				num2 = 0;
				string str = ExtensionTests.TestAllExtensionNodesRuntime(os, out num2);
				text += str;
				num += num2;
			}
			catch (Exception ex)
			{
				text = text + "\nLoad Error:\n" + Utils.GenerateReportFromException(ex).Trim();
				num++;
			}
			if (os != null)
			{
				screenMan.RemoveScreen(os);
			}
			ExtensionTests.CompleteExtensiontesting();
			if (!string.IsNullOrWhiteSpace(ExtensionTests.RuntimeLoadAdditionalErrors))
			{
				num++;
				text = text + "\r\n" + ExtensionTests.RuntimeLoadAdditionalErrors;
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x00024B4C File Offset: 0x00022D4C
		private static string TestAllExtensionMissions(OS os, out int errorsAdded)
		{
			int errors = 0;
			string ret = "";
			Utils.ActOnAllFilesRevursivley(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Missions", delegate(string filename)
			{
				if (filename.EndsWith(".xml"))
				{
					if (OS.TestingPassOnly)
					{
						try
						{
							ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(filename);
							if (activeMission != null)
							{
								ExtensionTests.RuntimeLoadAdditionalErrors += ExtensionTests.TestExtensionMission(activeMission, filename, os);
							}
						}
						catch (Exception ex)
						{
							errors++;
							ret = ret + "Error Loading Mission: " + filename;
							ret = ret + "\r\n\r\n" + Utils.GenerateReportFromExceptionCompact(ex) + "\r\n\r\n";
						}
					}
				}
			});
			errorsAdded = errors;
			return ret;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x00024C58 File Offset: 0x00022E58
		private static string TestAllExtensionNodesRuntime(OS os, out int errorsAdded)
		{
			int errors = 0;
			string ret = "";
			Utils.ActOnAllFilesRevursivley(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Nodes", delegate(string filename)
			{
				if (filename.EndsWith(".xml"))
				{
					try
					{
						Computer computer = Computer.loadFromFile(filename);
						if (computer != null)
						{
							ExtensionLoader.CheckAndAssignCoreServer(computer, os);
						}
					}
					catch (Exception ex)
					{
						string text = "COMPUTER LOAD ERROR:\nError loading computer \"{0}\"\nError: {1} - {2}";
						text = string.Format(text, filename, ex.GetType().Name, ex.Message);
						ret = ret + text + "\r\n";
						errors++;
					}
				}
			});
			errorsAdded = errors;
			return ret;
		}

		// Token: 0x06000286 RID: 646 RVA: 0x00024CC0 File Offset: 0x00022EC0
		private static string TestTagisClosed(string tagname, string allData)
		{
			string text = "";
			bool flag = allData.Contains("</" + tagname + ">") || allData.Contains("</" + tagname + " >");
			bool flag2 = allData.Contains("<" + tagname + ">");
			bool flag3 = flag2 && !flag;
			if (flag3)
			{
				text = text + "Tag " + tagname + " is not properly closed! Check your Slashes.";
				text += "\r\n\r\n";
			}
			return text;
		}

		// Token: 0x06000287 RID: 647 RVA: 0x00024D5C File Offset: 0x00022F5C
		public static string TestExtensionMission(object mission, string filepath, object os)
		{
			string text = "";
			ActiveMission activeMission = (ActiveMission)mission;
			string text2 = File.ReadAllText(filepath);
			text += ExtensionTests.TestTagisClosed("missionEnd", text2);
			text += ExtensionTests.TestTagisClosed("missionStart", text2);
			text += ExtensionTests.TestTagisClosed("nextMission", text2);
			text += ExtensionTests.TestTagisClosed("goals", text2);
			text += ExtensionTests.TestTagisClosed("email", text2);
			text += ExtensionTests.TestTagisClosed("sender", text2);
			text += ExtensionTests.TestTagisClosed("subject", text2);
			text += ExtensionTests.TestTagisClosed("body", text2);
			text += ExtensionTests.TestTagisClosed("attachments", text2);
			if (!text2.Contains("</attachments>"))
			{
				text += "File does not contain attachments tag at the end of the email! It needs to be there!";
				text += "\r\n";
			}
			int count = Regex.Matches(text2, "<goal ").Count;
			if (activeMission.goals.Count != count)
			{
				if (activeMission.goals.Count < count)
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"File defines some goals that are not being correctly parsed in! (",
						activeMission.goals.Count,
						" loaded vs ",
						count,
						" in file)"
					});
					text += "\r\nCheck your syntax and tags! Valid Goals:\r\n";
					for (int i = 0; i < activeMission.goals.Count; i++)
					{
						text = text + "\r\n" + activeMission.goals[i].ToString().Replace("Hacknet.Mission.", "");
					}
					text += "\r\n";
				}
			}
			if (string.IsNullOrWhiteSpace(activeMission.startFunctionName) && text2.Contains("<missionStart"))
			{
				text += "File contains missionStart, but it's not being correctly parsed in. It might be out of order in the file.";
				text += "\r\n";
			}
			if (activeMission.startFunctionName != null)
			{
				try
				{
					if (!activeMission.startFunctionName.Contains("addRank"))
					{
						MissionFunctions.runCommand(activeMission.startFunctionValue, activeMission.startFunctionName);
					}
				}
				catch (Exception ex)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						"Error running start function ",
						activeMission.startFunctionName,
						"\r\n",
						Utils.GenerateReportFromException(ex)
					});
				}
			}
			if (string.IsNullOrWhiteSpace(activeMission.endFunctionName) && text2.Contains("<missionEnd"))
			{
				text += "File contains missionEnd, but it's not being correctly parsed in. It might be out of order in the file.";
				text += "\r\n";
			}
			if (activeMission.endFunctionName != null)
			{
				try
				{
					if (!activeMission.endFunctionName.Contains("addRank"))
					{
						MissionFunctions.runCommand(activeMission.endFunctionValue, activeMission.endFunctionName);
					}
				}
				catch (Exception ex)
				{
					string text3 = text;
					text = string.Concat(new string[]
					{
						text3,
						"Error running end function ",
						activeMission.endFunctionName,
						"\r\n",
						Utils.GenerateReportFromException(ex)
					});
				}
			}
			string text4 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, os);
			if (!string.IsNullOrWhiteSpace(text4))
			{
				text += text4;
			}
			string result;
			if (text.Length > 1)
			{
				result = "Mission Errors for " + filepath.Replace("\\", "/") + ":\r\n" + text;
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x06000288 RID: 648 RVA: 0x00025170 File Offset: 0x00023370
		public static string TestExtensionCustomThemeSerialization(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			CustomTheme customTheme = new CustomTheme();
			Color color = new Color(22, 22, 22, 19);
			customTheme.defaultTopBarColor = color;
			string text2 = "customTheme1.xml";
			Utils.writeToFile(customTheme.GetSaveString(), text2);
			CustomTheme customTheme2 = CustomTheme.Deserialize(text2);
			if (customTheme2.defaultTopBarColor != color)
			{
				num++;
				text += "\nSave/Load broken for themes!";
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000289 RID: 649 RVA: 0x000251F4 File Offset: 0x000233F4
		public static string TestExtensionCustomThemeFile(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			Computer computer = Programs.getComputer(os, "advExamplePC");
			Folder folder = computer.files.root.searchForFolder("sys");
			FileEntry fileEntry = folder.searchForFile("Custom_x-server.sys");
			if (ThemeManager.getThemeForDataString(fileEntry.data) != OSTheme.Custom || ThemeManager.LastLoadedCustomTheme == null)
			{
				num++;
				text += "Custom theme did not read in from file correctly!";
			}
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			computer = Programs.getComputer(os, "advExamplePC");
			folder = computer.files.root.searchForFolder("sys");
			fileEntry = folder.searchForFile("Custom_x-server.sys");
			if (ThemeManager.getThemeForDataString(fileEntry.data) != OSTheme.Custom || ThemeManager.LastLoadedCustomTheme == null)
			{
				num++;
				text += "Custom theme did not read in from file correctly after save/load!";
			}
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0002532C File Offset: 0x0002352C
		public static string TestExtensionCustomFactions(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			text += ExtensionTests.TestExtensionsFactions(os.allFactions, out num);
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			text += ExtensionTests.TestExtensionsFactions(os.allFactions, out num);
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028B RID: 651 RVA: 0x000253BC File Offset: 0x000235BC
		public static string TestExtensionCustomFactionsActions(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			text += ExtensionTests.TestExtensionsFactions(os.allFactions, out num);
			Computer computer = Programs.getComputer(os, "linkNode1");
			Folder folder = computer.files.root.searchForFolder("bin");
			if (folder.searchForFile("FTPBounce.exe") != null)
			{
				num++;
				text += "\nFile somehow already on target system for faction test";
			}
			if (os.currentFaction != null)
			{
				num++;
				text += "\nFaction does not start as null";
			}
			ComputerLoader.loadMission(Utils.GetFileLoadPrefix() + "Missions/FactionTestMission0.xml", false);
			if (os.currentFaction.idName != "autoTestFaction")
			{
				num++;
				text += "\nLoading mission with start function to set player faction does not load faction correctly";
			}
			if (os.currentFaction.playerValue != 0)
			{
				num++;
				text = text + "\nPlayer faction not expected before mission completion. Expected 0, got " + os.currentFaction.playerValue;
			}
			os.currentMission.finish();
			if (os.currentFaction.playerValue != 5)
			{
				num++;
				text = text + "\nPlayer faction not expected after mission completion. Expected 5, got " + os.currentFaction.playerValue;
			}
			FileEntry fileEntry = folder.searchForFile("FTPBounce.exe");
			if (fileEntry == null)
			{
				num++;
				text += "\nFile not added correctly in response to faction progression";
			}
			else if (fileEntry.data != PortExploits.crackExeData[21])
			{
				num++;
				text += "\nFile added through faction system data not correctly filtered.";
			}
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			text += ExtensionTests.TestExtensionsFactions(os.allFactions, out num);
			if (os.currentFaction.idName != "autoTestFaction")
			{
				num++;
				text += "\nFaction not set correctly after load";
			}
			CustomFaction customFaction = os.currentFaction as CustomFaction;
			if (customFaction == null)
			{
				num++;
				text += "\nFaction is not set to the correct type after load";
			}
			if (customFaction.CustomActions.Count != 1)
			{
				num++;
				text += "\nFaction has incorrect number of remaining custom actions after load";
			}
			if (os.currentFaction.playerValue != 5)
			{
				num++;
				text = text + "\nPlayer faction value not set correctly after load. Expected 5, got " + os.currentFaction.playerValue;
			}
			os.currentMission.finish();
			computer = Programs.getComputer(os, "linkNode1");
			folder = computer.files.root.searchForFolder("bin");
			fileEntry = folder.searchForFile("SecondTestFile.txt");
			if (fileEntry == null)
			{
				num++;
				text += "\nSecond File not added correctly in response to faction progression";
			}
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028C RID: 652 RVA: 0x000256F0 File Offset: 0x000238F0
		private static string TestExtensionsFactions(AllFactions allFactions, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			if (allFactions.factions.Count != 2)
			{
				num++;
				text = text + "\nExpected 2 factions, got " + allFactions.factions.Count;
			}
			CustomFaction customFaction = allFactions.factions["tfac"] as CustomFaction;
			if (customFaction == null)
			{
				num++;
				text = text + "\nFaction 1 not listed as custom faction! Reports self as " + customFaction;
			}
			if (customFaction.name != "Test Faction")
			{
				num++;
				text += "\nFaction name incorrect!";
			}
			if (customFaction.playerValue != 0)
			{
				num++;
				text += "\nFaction starting player value incorrect!";
			}
			if (customFaction.CustomActions.Count != 5)
			{
				num++;
				text = text + "\nIncorrect number of custom faction actions! Expected 5, got " + customFaction.CustomActions.Count;
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028D RID: 653 RVA: 0x000257FC File Offset: 0x000239FC
		public static string TestExtensionMissionLoading(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestExtension", screenMan);
			string text2 = TestSuite.TestMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath, os);
			if (!string.IsNullOrWhiteSpace(text2))
			{
				num++;
				text = text + "\n\nMISSION READ ERRORS:\n" + text2;
			}
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x00025880 File Offset: 0x00023A80
		public static string TestExtensionStartNodeVisibility(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
			for (int i = 0; i < os.netMap.nodes.Count; i++)
			{
				string text2 = os.netMap.nodes[i].idName;
				if (os.netMap.visibleNodes.Contains(i))
				{
					if (!(text2 == "playerComp") && !ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes.Contains(text2) && !ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes.Contains(os.netMap.nodes[i].ip))
					{
						num++;
						text = text + "\nNode is discovered but it should not be! Node: " + text2;
					}
				}
			}
			for (int i = 0; i < ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes.Length; i++)
			{
				string text2 = ExtensionLoader.ActiveExtensionInfo.StartingVisibleNodes[i];
				Computer computer = Programs.getComputer(os, text2);
				if (computer != null)
				{
					if (!os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(computer)))
					{
						num++;
						text = text + "\nNode " + text2 + " should be discovered, but it is not! It's on the starting visible list";
					}
				}
			}
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x00025A08 File Offset: 0x00023C08
		public static string TestPopulatedExtension(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
			List<Computer> nodes = os.netMap.nodes;
			text += ExtensionTests.TestPopulatedExtensionRead(os, out num);
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
			text += ExtensionTests.TestPopulatedExtensionRead(os, out num);
			text += TestSuite.getTestingReportForLoadComparison(os, nodes, num, out num);
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000290 RID: 656 RVA: 0x00025AAC File Offset: 0x00023CAC
		private static string TestPopulatedExtensionRead(object os_obj, out int errorsAdded)
		{
			string text = "";
			errorsAdded = 0;
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, "linkNode1");
			Computer computer2 = Programs.getComputer(os, "linkNode2");
			Computer computer3 = Programs.getComputer(os, "linkNode3");
			if (!computer.links.Contains(os.netMap.nodes.IndexOf(computer2)) || !computer2.links.Contains(os.netMap.nodes.IndexOf(computer3)) || !computer3.links.Contains(os.netMap.nodes.IndexOf(computer)))
			{
				errorsAdded++;
				text += "\nLinks Error! Link node 1,2 or 3 does not have specified links\n";
			}
			Computer computer4 = Programs.getComputer(os, "secTestNode");
			if (computer4.adminPass != "sectestpass")
			{
				text += "\nAdmin Password for SecNode does not match!";
				errorsAdded++;
			}
			if (computer4.portsNeededForCrack != 1)
			{
				text = text + "\nSecNode ports needed for crack mismatch. Expected 2, got " + computer4.portsNeededForCrack;
				errorsAdded++;
			}
			if (!Utils.FloatEquals(computer4.traceTime, 300f))
			{
				text = text + "\nSecNode trace time mismatch. Expected 300, got " + computer4.traceTime;
				errorsAdded++;
			}
			if (!Utils.FloatEquals(computer4.proxyOverloadTicks, 2f * Computer.BASE_PROXY_TICKS))
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\nSecNode proxy time mismatch. Expected ",
					2f * Computer.BASE_PROXY_TICKS,
					" Got ",
					computer4.proxyOverloadTicks
				});
				errorsAdded++;
			}
			if (computer4.admin.ResetsPassword || computer4.admin.IsSuper || !(computer4.admin is FastBasicAdministrator))
			{
				text = text + "\nSecNode administrator error! Expected non resetting Fast admin, got " + computer4.admin.ToString();
				errorsAdded++;
			}
			Firewall firewall = new Firewall(6, "Scypio", 1f);
			if (firewall.getSaveString() != computer4.firewall.getSaveString())
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\nFireall difference! Expected \n",
					firewall.getSaveString(),
					"\nbut got\n",
					computer4.firewall.getSaveString()
				});
				errorsAdded++;
			}
			if (!computer4.ports.Contains(21) || !computer4.ports.Contains(22) || !computer4.ports.Contains(80) || !computer4.ports.Contains(25) || !computer4.ports.Contains(1433))
			{
				errorsAdded++;
				text += "\nSecNodem ports assignment error\n";
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < computer4.users.Count; i++)
			{
				if (computer4.users[i].name == "SecTest")
				{
					flag = true;
					if (computer4.users[i].pass != "userpas")
					{
						errorsAdded++;
						text = text + "\nUser account password for user SecTest do not match. Found Pass :" + computer4.users[i].pass + "\n";
					}
				}
				if (computer4.users[i].name == "mailGuy")
				{
					flag2 = true;
					if (computer4.users[i].pass != "mailPass")
					{
						errorsAdded++;
						text = text + "\nUser account password for user MailGuy do not match. Found Pass :" + computer4.users[i].pass + "\n";
					}
				}
			}
			if (!flag2 || !flag)
			{
				errorsAdded++;
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\nAccounts missing from SecTest node - found ",
					computer4.users.Count,
					" users."
				});
			}
			return text;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x00025F4C File Offset: 0x0002414C
		public static string TestBlankExtension(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string result = "";
			OS os;
			try
			{
				os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
			}
			catch (Exception ex)
			{
				errorsAdded = 1;
				return "\nError generating blank extension:\n" + Utils.GenerateReportFromException(ex);
			}
			ExtensionTests.TestOSForBlankSession(os, out result, out num);
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return result;
		}

		// Token: 0x06000292 RID: 658 RVA: 0x00025FC8 File Offset: 0x000241C8
		public static string TestAcademicDatabase(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
			if (os.netMap.academicDatabase == null)
			{
				num++;
				text += "\nNo Academic Database Detected where there should be one!";
			}
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/IntroExtension", screenMan);
			if (os.netMap.academicDatabase == null)
			{
				num++;
				text += "\nNo Academic Database Detected after save/load!";
			}
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = false;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
			if (os.netMap.academicDatabase != null)
			{
				num++;
				text += "\nAcademic database was created in empty session!!";
			}
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x000260C8 File Offset: 0x000242C8
		private static void TestOSForBlankSession(object osobj, out string ret, out int errors)
		{
			OS os = (OS)osobj;
			ret = "";
			errors = 0;
			if (os.netMap.nodes.Count != 2)
			{
				object obj = ret;
				ret = string.Concat(new object[]
				{
					obj,
					"\nError: Expected exactly 2 nodes but found ",
					os.netMap.nodes.Count,
					"\n"
				});
				errors++;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < os.netMap.nodes.Count; i++)
			{
				if (os.netMap.nodes[i].idName == "playerComp")
				{
					flag = true;
				}
				if (os.netMap.nodes[i].idName == "jmail")
				{
					if (os.netMap.nodes[i].getDaemon(typeof(MailServer)) != null)
					{
						flag2 = true;
						if (os.netMap.mailServer != os.netMap.nodes[i])
						{
							ret += "\nMail server is not correctly registered For NetMap.MailServer!\n";
							errors++;
						}
						bool flag3 = false;
						for (int j = 0; j < os.netMap.nodes[i].users.Count; j++)
						{
							if (os.netMap.nodes[i].users[i].name == os.defaultUser.name)
							{
								flag3 = true;
							}
						}
						if (!flag3)
						{
							object obj = ret;
							ret = string.Concat(new object[]
							{
								obj,
								"M\nail server does not contain a User for the player! Has ",
								os.netMap.nodes[i].users.Count,
								" users."
							});
							errors++;
						}
					}
					else
					{
						ret += "\nMail Server ID found but it contains no MailServer Daemon!";
						errors++;
					}
				}
			}
			if (!flag2 || !flag)
			{
				object obj = ret;
				ret = string.Concat(new object[]
				{
					obj,
					"\nBlank server should have a player node and a JMail server with an email daemon. Has Player Server = ",
					flag,
					", Has Mail Server = ",
					flag2,
					"\n"
				});
				errors++;
			}
			if (os.allFactions.factions.Count != 0)
			{
				errors++;
				object obj = ret;
				ret = string.Concat(new object[]
				{
					obj,
					"\nBlank session has factions! : ",
					os.allFactions.factions.Count,
					" found."
				});
			}
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000263F4 File Offset: 0x000245F4
		public static string TestBlankExtensionSaveLoad(ScreenManager screenMan, out int errorsAdded)
		{
			string result = "";
			int num = 0;
			OS os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
			List<Computer> nodes = os.netMap.nodes;
			os.threadedSaveExecute(false);
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = (OS)ExtensionTests.SetupOSForTests("Content/Tests/TestBlankExtension", screenMan);
			ExtensionTests.TestOSForBlankSession(os, out result, out num);
			screenMan.RemoveScreen(os);
			ExtensionTests.CompleteExtensiontesting();
			errorsAdded = num;
			return result;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x00026474 File Offset: 0x00024674
		private static object SetupOSForTests(string ActiveExtensionFoldername, ScreenManager screenMan)
		{
			string text = "__ExtensionTest";
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text);
			SaveFileManager.AddUser(text, "testpass");
			ExtensionTests.OS_wasTestingPass = OS.TestingPassOnly;
			OS.TestingPassOnly = true;
			Settings.IsInExtensionMode = true;
			ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo(ActiveExtensionFoldername);
			OS os = new OS();
			os.SaveUserAccountName = text;
			os.SaveGameUserName = saveFileNameForUsername;
			screenMan.AddScreen(os);
			return os;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000264DD File Offset: 0x000246DD
		private static void CompleteExtensiontesting()
		{
			OS.TestingPassOnly = ExtensionTests.OS_wasTestingPass;
			Settings.IsInExtensionMode = false;
			ExtensionLoader.ActiveExtensionInfo = null;
		}

		// Token: 0x040002D0 RID: 720
		private static bool OS_wasTestingPass = false;

		// Token: 0x040002D1 RID: 721
		internal static string RuntimeLoadAdditionalErrors = "";
	}
}
