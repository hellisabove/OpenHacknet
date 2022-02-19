using System;
using System.Collections.Generic;
using System.IO;
using Hacknet.Extensions;
using Hacknet.Localization;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x020000CF RID: 207
	public class TestSuite
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x00042E70 File Offset: 0x00041070
		public static string RunTestSuite(ScreenManager screenMan, bool IsQuickTestMode = false)
		{
			bool soundDisabled = Settings.soundDisabled;
			Settings.soundDisabled = true;
			string activeLocale = Settings.ActiveLocale;
			LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
			Settings.ActiveLocale = "en-us";
			string result = TestSuite.TestSaveLoadOnFile(screenMan, IsQuickTestMode);
			LocaleActivator.ActivateLocale(activeLocale, Game1.getSingleton().Content);
			Settings.ActiveLocale = activeLocale;
			Settings.soundDisabled = soundDisabled;
			return result;
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x00042EDC File Offset: 0x000410DC
		public static string TestSaveLoadOnFile(ScreenManager screenMan, bool IsQuicktestMode = false)
		{
			string text = "__hacknettestaccount";
			string pass = "__testingpassword";
			SaveFileManager.AddUser(text, pass);
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text);
			OS.TestingPassOnly = true;
			string text2 = "";
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
			string text3 = "Serialization and Integrity Test Report:\r\n";
			Console.WriteLine(text3);
			text2 += text3;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			int num = 0;
			text2 += TestSuite.getTestingReportForLoadComparison(os, nodes, num, out num);
			text2 = text2 + "\r\n" + TestSuite.TestMissions(os);
			int num2 = 0;
			text2 += TestSuite.TestGameProgression(os, out num2);
			num += num2;
			for (int i = 0; i < os.netMap.nodes.Count; i++)
			{
				Folder root = os.netMap.nodes[i].files.root;
				TestSuite.DeleteAllFilesRecursivley(root);
			}
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text;
			os.threadedSaveExecute(false);
			nodes = os.netMap.nodes;
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			screenMan.RemoveScreen(os);
			OS.TestingPassOnly = false;
			SaveFileManager.DeleteUser(text);
			int num3 = 0;
			if (!IsQuicktestMode)
			{
				text2 = text2 + "\r\nLocalization Tests: " + LocalizationTests.TestLocalizations(screenMan, out num3);
				text2 = text2 + "\r\nDLC Localization Tests: " + DLCLocalizationTests.TestDLCLocalizations(screenMan, out num3);
			}
			text2 = text2 + "\r\nDLC Tests: " + DLCTests.TestDLCFunctionality(screenMan, out num3);
			text2 = text2 + "\r\nDLC Extended Tests: " + DLCExtendedTests.TesExtendedFunctionality(screenMan, out num3);
			text2 = text2 + "\r\nEDU Edition Tests: " + EduEditionTests.TestEDUFunctionality(screenMan, out num3);
			text2 = text2 + "\r\nMisc Tests: " + TestSuite.TestMiscAndCLRFeatures(screenMan, out num3);
			string text4 = string.Concat(new object[]
			{
				"\r\nCore Tests: Complete - ",
				num,
				" errors found.\r\nTested ",
				nodes.Count,
				" generated nodes vs ",
				os.netMap.nodes.Count,
				" loaded nodes"
			});
			text2 += text4;
			Console.WriteLine(text4);
			MusicManager.stop();
			try
			{
				string text5 = "testreport.txt";
				File.Delete(text5);
				Utils.writeToFile(text2, text5);
			}
			catch (Exception)
			{
			}
			text2 = Utils.CleanStringToRenderable(text2);
			return text2;
		}

		// Token: 0x06000430 RID: 1072 RVA: 0x0004322C File Offset: 0x0004142C
		public static string TestMiscAndCLRFeatures(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "";
			int num = 0;
			if ((ushort)"".GetHashCode() != 5886)
			{
				num++;
				text += "\nBlank string Hashes to an unexpected value! This will break Header file decryption!\n";
			}
			if ((ushort)"12345asdf".GetHashCode() != 25213)
			{
				num++;
				text += "\nTest string Hashes to an unexpected value! This can break file decryption!\n";
			}
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"Complete - ",
				num,
				" errors found"
			});
			errorsAdded = num;
			return text;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x000432D0 File Offset: 0x000414D0
		private static void DeleteAllFilesRecursivley(Folder f)
		{
			f.files.Clear();
			for (int i = 0; i < f.folders.Count; i++)
			{
				TestSuite.DeleteAllFilesRecursivley(f.folders[i]);
			}
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x00043318 File Offset: 0x00041518
		internal static string getTestingReportForLoadComparison(object osobj, List<Computer> oldComps, int currentErrorCount, out int errorCount)
		{
			OS os = (OS)osobj;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			string text = "";
			errorCount = currentErrorCount;
			bool flag = false;
			for (int i = 0; i < os.netMap.nodes.Count; i++)
			{
				for (int j = 0; j < oldComps.Count; j++)
				{
					if (i != j)
					{
						if (os.netMap.nodes[i].idName == oldComps[j].idName)
						{
							errorCount++;
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"\nDuplicate Node ID found! ",
								i,
								": ",
								os.netMap.nodes[i].name,
								" and ",
								j,
								": ",
								oldComps[j].name
							});
							flag = true;
						}
					}
				}
			}
			string result;
			if (flag)
			{
				text += "\nCritical Error encountered - exiting tests early";
				result = text;
			}
			else
			{
				for (int i = 0; i < oldComps.Count; i++)
				{
					Computer computer = oldComps[i];
					Computer computer2 = Programs.getComputer(os, computer.ip);
					TestSuite.Assert(computer.name, computer2.name);
					string text2 = computer2.files.TestEquals(computer.files);
					if (list.Contains(computer2.idName))
					{
						int num = list.IndexOf(computer2.idName);
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Duplicate ID Found - \"",
							computer2.idName,
							"\" from ",
							list2[num],
							"@",
							num,
							" current: ",
							computer2.name,
							"@",
							i
						});
					}
					list.Add(computer2.idName);
					list2.Add(computer2.name);
					if (!Utils.FloatEquals(computer.startingOverloadTicks, computer2.startingOverloadTicks))
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Proxy timer difference - \"",
							computer2.name,
							"\" from Base:",
							computer.startingOverloadTicks,
							" to Current: ",
							computer2.startingOverloadTicks
						});
					}
					if ((computer.firewall != null || computer2.firewall != null) && !computer.firewall.Equals(computer2.firewall))
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							"Firewall difference - \"",
							computer2.name,
							"\" from Base:",
							computer.firewall.ToString(),
							"\r\n to Current: ",
							computer2.firewall.ToString(),
							"\r\n"
						});
					}
					if (computer.icon != computer2.icon)
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							"Icon difference - \"",
							computer2.name,
							"\" from Base:",
							computer.icon,
							" to Current: ",
							computer2.icon
						});
					}
					if (computer.portsNeededForCrack != computer2.portsNeededForCrack)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Port for crack difference - \"",
							computer2.name,
							"\" from Base:",
							computer.portsNeededForCrack,
							" to Current: ",
							computer2.portsNeededForCrack
						});
					}
					for (int k = 0; k < computer.links.Count; k++)
					{
						if (computer.links[k] != computer2.links[k])
						{
							object obj = text2;
							text2 = string.Concat(new object[]
							{
								obj,
								"Link difference - \"",
								computer2.name,
								"\" @",
								k,
								" ",
								computer.links[k],
								" vs ",
								computer2.links[k]
							});
						}
					}
					if (!Utils.FloatEquals(computer.location.X, computer2.location.X) || !Utils.FloatEquals(computer.location.Y, computer2.location.Y))
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Location difference - \"",
							computer2.name,
							"\" from Base:",
							computer.location,
							" to Current: ",
							computer2.location
						});
					}
					if (!Utils.FloatEquals(computer.traceTime, computer2.traceTime))
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Trace timer difference - \"",
							computer2.name,
							"\" from Base:",
							computer.traceTime,
							" to Current: ",
							computer2.traceTime
						});
					}
					if (computer.adminPass != computer2.adminPass)
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							"Password Difference: expected \"",
							computer.adminPass,
							"\" but got \"",
							computer2.adminPass,
							"\"\r\n"
						});
					}
					if (computer.adminIP != computer2.adminIP)
					{
						string text3 = text2;
						text2 = string.Concat(new string[]
						{
							text3,
							"Admin IP Difference: expected \"",
							computer.adminIP,
							"\" but got \"",
							computer2.adminIP,
							"\"\r\n"
						});
					}
					for (int k = 0; k < computer.users.Count; k++)
					{
						if (!computer.users[k].Equals(computer2.users[k]))
						{
							object obj = text2;
							text2 = string.Concat(new object[]
							{
								obj,
								"User difference - \"",
								computer2.name,
								"\" @",
								k,
								" ",
								computer.users[k],
								" vs ",
								computer2.users[k]
							});
						}
					}
					for (int l = 0; l < computer.daemons.Count; l++)
					{
						if (computer.daemons[l].getSaveString() != computer2.daemons[l].getSaveString())
						{
							string text3 = text2;
							text2 = string.Concat(new string[]
							{
								text3,
								"Daemon Difference: expected \r\n-----\r\n",
								computer.daemons[l].getSaveString(),
								"\r\n----- but got -----\r\n",
								computer2.daemons[l].getSaveString(),
								"\r\n-----\r\n"
							});
						}
					}
					if (computer.hasProxy != computer2.hasProxy)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Proxy Difference: OldProxy:",
							computer.hasProxy,
							" vs NewProxy:",
							computer2.hasProxy
						});
					}
					if (computer.proxyActive != computer2.proxyActive)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Proxy Active Difference: OldProxy:",
							computer.proxyActive,
							" vs NewProxy:",
							computer2.proxyActive
						});
					}
					if (computer.portsNeededForCrack != computer2.portsNeededForCrack)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"Ports for crack Difference: Old:",
							computer.portsNeededForCrack,
							" vs New:",
							computer2.portsNeededForCrack
						});
					}
					if (computer.admin != null && computer2.admin != null)
					{
						if (computer.admin.GetType() != computer2.admin.GetType())
						{
							object obj = text2;
							text2 = string.Concat(new object[]
							{
								obj,
								"SecAdmin Difference: Old:",
								computer.admin,
								" vs New:",
								computer2.admin
							});
						}
					}
					if (computer.admin != null && computer.admin.IsSuper != computer2.admin.IsSuper)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"SecAdmin Super Difference: Old:",
							computer.admin.IsSuper,
							" vs New:",
							computer2.admin.IsSuper
						});
					}
					if (computer.admin != null && computer.admin.ResetsPassword != computer2.admin.ResetsPassword)
					{
						object obj = text2;
						text2 = string.Concat(new object[]
						{
							obj,
							"SecAdmin PassReset Difference: Old:",
							computer.admin.ResetsPassword,
							" vs New:",
							computer2.admin.ResetsPassword
						});
					}
					if (text2 != null)
					{
						string text4 = string.Concat(new string[]
						{
							"\r\nErrors in ",
							computer.idName,
							" - \"",
							computer.name,
							"\"\r\n",
							text2,
							"\r\n"
						});
						text = text + text4 + "\r\n";
						Console.WriteLine(text4);
						errorCount++;
					}
					else
					{
						Console.Write(".");
						text += ".";
					}
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06000433 RID: 1075 RVA: 0x00043FE8 File Offset: 0x000421E8
		public static string TestMissions(object os_obj)
		{
			OS os_obj2 = (OS)os_obj;
			string str = "";
			string retAdditions = "";
			MissionFunctions.ReportErrorInCommand = (Action<string>)Delegate.Combine(MissionFunctions.ReportErrorInCommand, new Action<string>(delegate(string report)
			{
				string retAdditions = retAdditions;
				retAdditions = string.Concat(new string[]
				{
					retAdditions,
					TestSuite.ActiveObjectID,
					" : ",
					report,
					"\r\n"
				});
			}));
			TestSuite.TestedMissionNames.Clear();
			try
			{
				string str2 = TestSuite.TestMission("Content/Missions/BitMissionIntro.xml", os_obj2);
				str += str2;
				str2 = TestSuite.TestMission("Content/Missions/BitMission0.xml", os_obj2);
				str += str2;
				string text = "Content/Missions/Entropy/StartingSet/";
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				FileInfo[] files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string missionName = text + files[i].Name;
					str2 = TestSuite.TestMission(missionName, os_obj2);
					str += str2;
				}
				str2 = TestSuite.TestMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", os_obj2);
				str += str2;
				str2 = TestSuite.TestMission("Content/Missions/MainHub/Intro/Intro01.xml", os_obj2);
				str += str2;
				text = "Content/Missions/MainHub/FirstSet/";
				directoryInfo = new DirectoryInfo(text);
				files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string missionName = text + files[i].Name;
					str2 = TestSuite.TestMission(missionName, os_obj2);
					str += str2;
				}
				str2 = TestSuite.TestMission("Content/Missions/lelzSec/IntroTestMission.xml", os_obj2);
				str += str2;
				str2 = TestSuite.TestMission("Content/Missions/MainHub/BitSet/Missions/BitHubSet01.xml", os_obj2);
				str += str2;
				str2 = TestSuite.TestMission("Content/Missions/BitPath/BitAdv_Intro.xml", os_obj2);
				str += str2;
				str2 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/FirstSet/01HubSet02.xml", "changeSong", os_obj2);
				str += str2;
				str2 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/FirstSet/02HubSet05.xml", "addFlags:decypher", os_obj2);
				str += str2;
				str2 = TestSuite.TestMissionEndFucntion("Content/Missions/MainHub/DecypherSet/DECHeadMission02.xml", "addFlags:dechead", os_obj2);
				str += str2;
				MissionFunctions.ReportErrorInCommand = null;
			}
			finally
			{
				MissionFunctions.ReportErrorInCommand = null;
			}
			return str + retAdditions;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x00044210 File Offset: 0x00042410
		public static string TestMission(string missionName, object os_obj)
		{
			string text = "";
			OS os = (OS)os_obj;
			string result;
			if (TestSuite.TestedMissionNames.Contains(missionName))
			{
				result = text;
			}
			else
			{
				try
				{
					if (!File.Exists(missionName))
					{
						text = text + "Invalid Mission Path! : " + missionName + "\r\n";
					}
					ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(missionName);
					TestSuite.ActiveObjectID = missionName;
					string text2 = "";
					for (int i = 0; i < activeMission.goals.Count; i++)
					{
						string text3 = activeMission.goals[i].TestCompletable();
						if (text3 != null && text3.Length > 0)
						{
							object obj = text2;
							text2 = string.Concat(new object[]
							{
								obj,
								missionName,
								" Goal[",
								i,
								"] ",
								activeMission.goals[i].ToString(),
								" :: ",
								text3,
								"\r\n"
							});
						}
					}
					try
					{
						if (!string.IsNullOrWhiteSpace(activeMission.startFunctionName))
						{
							if (!Utils.CheckStringIsRenderable(activeMission.startFunctionName))
							{
								string text4 = text;
								text = string.Concat(new string[]
								{
									text4,
									"Mission ",
									missionName,
									" has unrenderable start function ",
									Utils.CleanStringToRenderable(activeMission.startFunctionName)
								});
							}
							MissionFunctions.runCommand(activeMission.startFunctionValue, activeMission.startFunctionName);
						}
						if (!string.IsNullOrWhiteSpace(activeMission.endFunctionName))
						{
							if (!Utils.CheckStringIsRenderable(activeMission.endFunctionName))
							{
								string text4 = text;
								text = string.Concat(new string[]
								{
									text4,
									"Mission ",
									missionName,
									" has unrenderable end function ",
									Utils.CleanStringToRenderable(activeMission.endFunctionName)
								});
							}
							MissionFunctions.runCommand(activeMission.endFunctionValue, activeMission.endFunctionName);
						}
						string str = Directory.GetCurrentDirectory() + "/";
						string text5 = Utils.GetFileLoadPrefix();
						if (text5 == "Content/")
						{
							text5 += "Missions/";
						}
						else if (!text5.StartsWith("Extensions"))
						{
							str = "";
						}
						string path = str + LocalizedFileLoader.GetLocalizedFilepath(text5 + activeMission.nextMission);
						if (!(activeMission.nextMission == "NONE") && !File.Exists(path))
						{
							string text4 = text;
							text = string.Concat(new string[]
							{
								text4,
								"\r\nNextMission Tag for mission \"",
								missionName,
								"\" has nonexistent next mission path: ",
								activeMission.nextMission,
								"\r\n"
							});
						}
					}
					catch (Exception ex)
					{
						string text4 = text;
						text = string.Concat(new string[]
						{
							text4,
							"Error running start or end mission function of mission: ",
							missionName,
							"\r\nStart Func: ",
							activeMission.startFunctionName,
							"\r\nEnd Func: ",
							activeMission.endFunctionName
						});
						text = text + "\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
					}
					if (text2.Length > 0)
					{
						text = text + text2 + "--------------\r\n";
					}
					TestSuite.TestedMissionNames.Add(missionName);
					string str2 = "Content/Missions/";
					if (Settings.IsInExtensionMode)
					{
						str2 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
					}
					List<ActiveMission> list = new List<ActiveMission>();
					for (int i = 0; i < os.branchMissions.Count; i++)
					{
						list.Add(os.branchMissions[i]);
					}
					if (activeMission.nextMission != null && activeMission.nextMission.ToLower() != "none")
					{
						text += TestSuite.TestMission(str2 + activeMission.nextMission, os);
					}
					for (int i = 0; i < list.Count; i++)
					{
						string localizedFilepath = LocalizedFileLoader.GetLocalizedFilepath(list[i].reloadGoalsSourceFile);
						if (!TestSuite.TestedMissionNames.Contains(localizedFilepath))
						{
							Console.WriteLine("testing Branch Mission " + localizedFilepath);
							text += TestSuite.TestMission(localizedFilepath, os);
						}
					}
				}
				catch (Exception ex)
				{
					string text4 = text;
					text = string.Concat(new string[]
					{
						text4,
						"Error Loading ",
						missionName,
						"\r\n",
						ex.ToString()
					});
				}
				result = text;
			}
			return result;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x0004473C File Offset: 0x0004293C
		public static string TestMissionEndFucntion(string missionName, string expectedEndFunction, object os_obj)
		{
			string text = "";
			OS os = (OS)os_obj;
			if (!File.Exists(missionName))
			{
				text = text + "Invalid Mission Path! : " + missionName + "\r\n";
			}
			ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(missionName);
			if (activeMission.endFunctionName != expectedEndFunction)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\r\nUnexpected end function in ",
					missionName,
					"\r\nExpected: ",
					expectedEndFunction,
					" -- found: ",
					activeMission.endFunctionName
				});
			}
			return text;
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x000447E8 File Offset: 0x000429E8
		public static string TestGameProgression(object os_obj, out int errorsOut)
		{
			string text = "";
			int num = 0;
			OS os = (OS)os_obj;
			os.delayer.RunAllDelayedActions();
			string filename = "Content/Missions/BitMissionIntro.xml";
			ComputerLoader.loadMission(filename, false);
			Folder folder = os.thisComputer.files.root.searchForFolder("bin");
			folder.files.Clear();
			if (!os.currentMission.isComplete(null))
			{
				num++;
				text += "\r\nCouldn't finish first mission...\r\n";
			}
			os.currentMission.finish();
			Computer computer = Programs.getComputer(os, "portcrack01");
			FileEntry fileEntry = computer.files.root.searchForFolder("bin").files[0];
			folder.files.Add(new FileEntry(fileEntry.data, "ssh.exe"));
			if (!os.currentMission.isComplete(null))
			{
				num++;
				text += "\r\nCouldn't finish sshcrack mission...\r\n";
			}
			os.currentMission.finish();
			if (os.currentMission.isComplete(null))
			{
				num++;
				text += "\r\nMission 3 is completable early!\r\n";
			}
			computer = Programs.getComputer(os, "bitMission00");
			computer.giveAdmin(os.thisComputer.ip);
			if (!os.currentMission.isComplete(null))
			{
				num++;
				text += "\r\nCouldn't finish BitMission0\r\n";
			}
			os.currentMission.finish();
			errorsOut = num;
			return text;
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00044978 File Offset: 0x00042B78
		private static void Assert(string first, string second)
		{
			if (first != second)
			{
				throw new InvalidProgramException();
			}
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x0004499C File Offset: 0x00042B9C
		public static void DoWordCount()
		{
			string[] folders = new string[]
			{
				"Content/DLC/Missions",
				"Content/DLC/Docs",
				"Content/DLC/Misc",
				"Content/DLC/ActionScripts"
			};
			string[] fileOnlyFolders = new string[]
			{
				"Content/DLC"
			};
			WordCounter.PerformWordCount(folders, fileOnlyFolders);
		}

		// Token: 0x04000516 RID: 1302
		private static string ActiveObjectID = "";

		// Token: 0x04000517 RID: 1303
		private static List<string> TestedMissionNames = new List<string>();
	}
}
