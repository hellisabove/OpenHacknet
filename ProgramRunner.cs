using System;
using Hacknet.Extensions;
using Hacknet.Localization;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000EA RID: 234
	public static class ProgramRunner
	{
		// Token: 0x060004E3 RID: 1251 RVA: 0x0004B790 File Offset: 0x00049990
		public static bool ExecuteProgram(object os_object, string[] arguments)
		{
			ProgramRunner.<>c__DisplayClass6 CS$<>8__locals1 = new ProgramRunner.<>c__DisplayClass6();
			CS$<>8__locals1.os = (OS)os_object;
			string[] array = arguments;
			bool flag = true;
			if (array[0].ToLower().Equals("connect"))
			{
				Programs.connect(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("disconnect") || array[0].Equals("dc"))
			{
				Programs.disconnect(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("ls") || array[0].Equals("dir"))
			{
				Programs.ls(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("cd"))
			{
				Programs.cd(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("cd.."))
			{
				array = new string[]
				{
					"cd",
					".."
				};
				Programs.cd(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("cat") || array[0].Equals("more") || array[0].Equals("less"))
			{
				Programs.cat(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("exe"))
			{
				Programs.execute(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("probe") || array[0].Equals("nmap"))
			{
				Programs.probe(array, CS$<>8__locals1.os);
			}
			else if (array[0].Equals("scp"))
			{
				Programs.scp(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("scan"))
			{
				Programs.scan(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("rm") || array[0].Equals("del"))
			{
				Programs.rm(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("mv"))
			{
				Programs.mv(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("ps"))
			{
				Programs.ps(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("kill") || array[0].Equals("pkill"))
			{
				Programs.kill(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("reboot"))
			{
				Programs.reboot(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("opencdtray"))
			{
				Programs.opCDTray(array, CS$<>8__locals1.os, true);
				flag = false;
			}
			else if (array[0].ToLower().Equals("closecdtray"))
			{
				Programs.opCDTray(array, CS$<>8__locals1.os, false);
				flag = false;
			}
			else if (array[0].Equals("replace"))
			{
				Programs.replace2(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("analyze"))
			{
				Programs.analyze(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("solve"))
			{
				Programs.solve(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("clear"))
			{
				Programs.clear(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("upload") || array[0].Equals("up"))
			{
				Programs.upload(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("login"))
			{
				Programs.login(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("addnote"))
			{
				Programs.addNote(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals(":(){:|:&};:"))
			{
				ProgramRunner.ExecuteProgram(CS$<>8__locals1.os, new string[]
				{
					"forkbomb"
				});
			}
			else if (array[0].ToLower().Equals("append"))
			{
				flag = false;
				string[] quoteSeperatedArgs = Utils.GetQuoteSeperatedArgs(array);
				Folder currentFolder = Programs.getCurrentFolder(CS$<>8__locals1.os);
				if (quoteSeperatedArgs.Length <= 1)
				{
					CS$<>8__locals1.os.write("Usage: append [FILENAME] [LINE TO APPEND]");
					return flag;
				}
				FileEntry fileEntry = currentFolder.searchForFile(quoteSeperatedArgs[1]);
				int num = 2;
				if (fileEntry == null)
				{
					fileEntry = currentFolder.searchForFile(CS$<>8__locals1.os.display.commandArgs[1]);
					if (fileEntry == null)
					{
						CS$<>8__locals1.os.write("Usage: append [FILENAME] [LINE TO APPEND]");
						return flag;
					}
					CS$<>8__locals1.os.write("No filename provided");
					CS$<>8__locals1.os.write("Assuming active flag file \"" + fileEntry.name + "\" For editing");
					if (array.Length == 1)
					{
						array = new string[]
						{
							"append",
							fileEntry.name
						};
					}
					else
					{
						array[1] = fileEntry.name;
					}
					num = 1;
				}
				if (fileEntry != null)
				{
					string text = "";
					for (int i = num; i < quoteSeperatedArgs.Length; i++)
					{
						text = text + quoteSeperatedArgs[i] + " ";
					}
					FileEntry fileEntry2 = fileEntry;
					fileEntry2.data = fileEntry2.data + "\n" + text;
					flag = true;
					array[0] = "cat";
					array[1] = fileEntry.name;
					for (int i = 2; i < array.Length; i++)
					{
						array[i] = "";
					}
					Programs.cat(array, CS$<>8__locals1.os);
				}
			}
			else if (array[0].Equals("remline"))
			{
				FileEntry fileEntry = Programs.getCurrentFolder(CS$<>8__locals1.os).searchForFile(array[1]);
				if (fileEntry != null)
				{
					int num2 = fileEntry.data.LastIndexOf('\n');
					if (num2 < 0)
					{
						num2 = 0;
					}
					fileEntry.data = fileEntry.data.Substring(0, num2);
					flag = true;
					array[0] = "cat";
					for (int i = 2; i < array.Length; i++)
					{
						array[i] = "";
					}
					Programs.cat(array, CS$<>8__locals1.os);
				}
			}
			else if (array[0].Equals("getString"))
			{
				Programs.getString(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].ToLower().Equals("reloadtheme"))
			{
				Folder folder = CS$<>8__locals1.os.thisComputer.files.root.searchForFolder("sys");
				FileEntry fileEntry3 = folder.searchForFile("x-server.sys");
				if (fileEntry3 != null)
				{
					OSTheme themeForDataString = ThemeManager.getThemeForDataString(fileEntry3.data);
					ThemeManager.switchTheme(CS$<>8__locals1.os, themeForDataString);
				}
				flag = false;
			}
			else if (array[0].Equals("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj"))
			{
				Programs.firstTimeInit(array, CS$<>8__locals1.os, false);
				flag = false;
			}
			else if (array[0].Equals("chat"))
			{
				string text2 = "chat " + CS$<>8__locals1.os.username + " ";
				for (int i = 1; i < array.Length; i++)
				{
					text2 = text2 + array[i] + " ";
				}
				if (CS$<>8__locals1.os.multiplayer)
				{
					CS$<>8__locals1.os.sendMessage(text2);
				}
				flag = false;
			}
			else if ((array[0].Equals("exitdemo") || array[0].Equals("resetdemo")) && Settings.isDemoMode)
			{
				MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
				MainMenu screen = new MainMenu();
				CS$<>8__locals1.os.ScreenManager.AddScreen(screen);
				MainMenu.resetOS();
				CS$<>8__locals1.os.ExitScreen();
				OS.currentInstance = null;
				flag = false;
				if (Settings.MultiLingualDemo)
				{
					LocaleActivator.ActivateLocale("zh-cn", Game1.getSingleton().Content);
				}
			}
			else if (array[0].Equals("fh") && OS.DEBUG_COMMANDS)
			{
				Programs.fastHack(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("ra") && OS.DEBUG_COMMANDS)
			{
				Programs.revealAll(array, CS$<>8__locals1.os);
				flag = false;
			}
			else if (array[0].Equals("deathseq") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.TraceDangerSequence.BeginTraceDangerSequence();
				flag = false;
			}
			else if (array[0].Equals("testcredits") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.endingSequence.IsActive = true;
				flag = false;
			}
			else if (array[0].Equals("addflag") && OS.DEBUG_COMMANDS)
			{
				if (array.Length < 2)
				{
					CS$<>8__locals1.os.write("\nFlag to add required\n");
				}
				CS$<>8__locals1.os.Flags.AddFlag(array[1]);
				flag = false;
			}
			else if (array[0].Equals("addTestEmails") && OS.DEBUG_COMMANDS)
			{
				for (int i = 0; i < 4; i++)
				{
					((MailServer)CS$<>8__locals1.os.netMap.mailServer.getDaemon(typeof(MailServer))).addMail(MailServer.generateEmail(string.Concat(new object[]
					{
						"testEmail ",
						i,
						" ",
						Utils.getRandomByte().ToString()
					}), "test", "test"), CS$<>8__locals1.os.defaultUser.name);
				}
				flag = false;
			}
			else if (array[0].Equals("dscan") && OS.DEBUG_COMMANDS)
			{
				if (array.Length < 2)
				{
					CS$<>8__locals1.os.write("\nNode ID Required\n");
				}
				bool flag2 = false;
				for (int i = 0; i < CS$<>8__locals1.os.netMap.nodes.Count; i++)
				{
					if (CS$<>8__locals1.os.netMap.nodes[i].idName.ToLower().StartsWith(array[1].ToLower()))
					{
						CS$<>8__locals1.os.netMap.discoverNode(CS$<>8__locals1.os.netMap.nodes[i]);
						CS$<>8__locals1.os.netMap.nodes[i].highlightFlashTime = 1f;
						flag2 = true;
						break;
					}
				}
				if (!flag2)
				{
					CS$<>8__locals1.os.write("Node ID Not found");
				}
				flag = false;
			}
			else if (array[0].Equals("revmany") && OS.DEBUG_COMMANDS)
			{
				for (int i = 0; i < 60; i++)
				{
					int index;
					do
					{
						index = Utils.random.Next(CS$<>8__locals1.os.netMap.nodes.Count);
					}
					while (CS$<>8__locals1.os.netMap.nodes[index].idName == "mainHub" || CS$<>8__locals1.os.netMap.nodes[index].idName == "entropy00" || CS$<>8__locals1.os.netMap.nodes[index].idName == "entropy01");
					CS$<>8__locals1.os.netMap.discoverNode(CS$<>8__locals1.os.netMap.nodes[index]);
				}
				CS$<>8__locals1.os.netMap.lastAddedNode = CS$<>8__locals1.os.thisComputer;
				CS$<>8__locals1.os.homeAssetServerID = "dhsDrop";
				CS$<>8__locals1.os.homeNodeID = "dhs";
				CS$<>8__locals1.os.netMap.discoverNode(Programs.getComputer(CS$<>8__locals1.os, "dhs"));
				CS$<>8__locals1.os.netMap.discoverNode(Programs.getComputer(CS$<>8__locals1.os, "dhsDrop"));
				flag = false;
			}
			else if (array[0].ToLower().Equals("reloadext") && OS.DEBUG_COMMANDS)
			{
				if (Settings.IsInExtensionMode)
				{
					ExtensionLoader.ReloadExtensionNodes(CS$<>8__locals1.os);
				}
				flag = false;
			}
			else if ((array[0].Equals("testsave") && OS.DEBUG_COMMANDS) || array[0].Equals("save!(SJN!*SNL8vAewew57WewJdwl89(*4;;;&!)@&(ak'^&#@J3KH@!*"))
			{
				CS$<>8__locals1.os.threadedSaveExecute(false);
				SettingsLoader.writeStatusFile();
				flag = false;
			}
			else if (array[0].Equals("testload") && OS.DEBUG_COMMANDS)
			{
				flag = false;
			}
			else if (array[0].Equals("teststrikerhack") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.delayer.Post(ActionDelayer.Wait(3.0), delegate
				{
					MissionFunctions.runCommand(1, "triggerDLCHackRevenge");
				});
				flag = false;
			}
			else if (array[0].Equals("linkToCSECPostDLC") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.execute("dscan mainhub");
				CS$<>8__locals1.os.allFactions.setCurrentFaction("hub", CS$<>8__locals1.os);
				CS$<>8__locals1.os.currentFaction.playerValue = 2;
				CS$<>8__locals1.os.Flags.AddFlag("dlc_complete");
				CS$<>8__locals1.os.Flags.AddFlag("dlc_csec_end_facval:0");
				MissionFunctions.runCommand(1, "addRank");
				flag = false;
			}
			else if (array[0].Equals("debug") && OS.DEBUG_COMMANDS)
			{
				int num3 = PortExploits.services.Count;
				if (array.Length > 1)
				{
					try
					{
						num3 = Convert.ToInt32(array[1]);
					}
					catch (Exception)
					{
					}
				}
				int i = 0;
				while (i < PortExploits.services.Count && i <= num3)
				{
					CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[i]], PortExploits.cracks[PortExploits.portNums[i]]));
					i++;
				}
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[9], PortExploits.cracks[9]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[10], PortExploits.cracks[10]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[11], PortExploits.cracks[11]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[12], PortExploits.cracks[12]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[13], PortExploits.cracks[13]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[14], PortExploits.cracks[14]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[15], PortExploits.cracks[15]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[16], PortExploits.cracks[16]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[17], PortExploits.cracks[17]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[31], PortExploits.cracks[31]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[33], PortExploits.cracks[33]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[34], PortExploits.cracks[34]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[35], PortExploits.cracks[35]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[36], PortExploits.cracks[36]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[37], PortExploits.cracks[37]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[38], PortExploits.cracks[38]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[39], PortExploits.cracks[39]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[41], PortExploits.cracks[41]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[554], PortExploits.cracks[554]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[40], PortExploits.cracks[40]));
				CS$<>8__locals1.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.DangerousPacemakerFirmware, "KBT_TestFirmware.dll"));
				CS$<>8__locals1.os.Flags.AddFlag("dechead");
				CS$<>8__locals1.os.Flags.AddFlag("decypher");
				CS$<>8__locals1.os.Flags.AddFlag("csecBitSet01Complete");
				CS$<>8__locals1.os.Flags.AddFlag("csecRankingS2Pass");
				CS$<>8__locals1.os.Flags.AddFlag("CSEC_Member");
				CS$<>8__locals1.os.Flags.AddFlag("bitPathStarted");
				flag = false;
				for (i = 0; i < 4; i++)
				{
					Computer computer = new Computer("DebugShell" + i, NetworkMap.generateRandomIP(), CS$<>8__locals1.os.netMap.getRandomPosition(), 0, 2, CS$<>8__locals1.os);
					computer.adminIP = CS$<>8__locals1.os.thisComputer.adminIP;
					CS$<>8__locals1.os.netMap.nodes.Add(computer);
					CS$<>8__locals1.os.netMap.discoverNode(computer);
				}
				CS$<>8__locals1.os.netMap.discoverNode("practiceServer");
				CS$<>8__locals1.os.netMap.discoverNode("entropy00");
			}
			else if (array[0].Equals("flash") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.traceTracker.start(40f);
				CS$<>8__locals1.os.warningFlash();
				flag = false;
				CS$<>8__locals1.os.IncConnectionOverlay.Activate();
			}
			else if (array[0].Equals("cycletheme") && OS.DEBUG_COMMANDS)
			{
				ProgramRunner.<>c__DisplayClass8 CS$<>8__locals2 = new ProgramRunner.<>c__DisplayClass8();
				CS$<>8__locals2.CS$<>8__locals7 = CS$<>8__locals1;
				CS$<>8__locals2.ctheme = delegate(OSTheme theme)
				{
					ThemeManager.switchTheme(CS$<>8__locals1.os, theme);
				};
				CS$<>8__locals2.next = 1;
				CS$<>8__locals2.delay = 1.2;
				CS$<>8__locals2.cthemeAct = delegate()
				{
					CS$<>8__locals2.ctheme((OSTheme)CS$<>8__locals2.next);
					CS$<>8__locals2.next = (CS$<>8__locals2.next + 1) % 7;
				};
				ProgramRunner.<>c__DisplayClass8 CS$<>8__locals3 = CS$<>8__locals2;
				CS$<>8__locals3.cthemeAct = (Action)Delegate.Combine(CS$<>8__locals3.cthemeAct, new Action(delegate()
				{
					CS$<>8__locals2.CS$<>8__locals7.os.delayer.Post(ActionDelayer.Wait(CS$<>8__locals2.delay), CS$<>8__locals2.cthemeAct);
				}));
				CS$<>8__locals2.cthemeAct();
			}
			else if (array[0].Equals("testdlc") && OS.DEBUG_COMMANDS)
			{
				MissionFunctions.runCommand(0, "demoFinalMissionEndDLC");
				flag = false;
			}
			else if (array[0].Equals("testircentries") && OS.DEBUG_COMMANDS)
			{
				Computer computer = Programs.getComputer(CS$<>8__locals1.os, "dhs");
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				for (int i = 0; i < 100; i++)
				{
					dlchubServer.IRCSystem.AddLog("Test", "Test Message\nMultiline\nMessage", null);
				}
				flag = false;
			}
			else if (array[0].Equals("testirc") && OS.DEBUG_COMMANDS)
			{
				Computer computer = Programs.getComputer(CS$<>8__locals1.os, "dhs");
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				dlchubServer.IRCSystem.AddLog("Test", "Test Message", null);
				dlchubServer.IRCSystem.AddLog("Channel", "Test Message\nfrom channel", null);
				flag = false;
			}
			else if (array[0].Equals("flashtest") && OS.DEBUG_COMMANDS)
			{
				if (!PostProcessor.dangerModeEnabled)
				{
					PostProcessor.dangerModeEnabled = true;
					PostProcessor.dangerModePercentComplete = 0.5f;
				}
				else
				{
					PostProcessor.dangerModeEnabled = false;
					PostProcessor.dangerModePercentComplete = 0f;
				}
				flag = false;
			}
			else if (array[0].Equals("dectest") && OS.DEBUG_COMMANDS)
			{
				string text3 = "this is a test message for the encrypter";
				string text4 = FileEncrypter.EncryptString(text3, "header message", "1.2.3.4.5", "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooongpass", null);
				CS$<>8__locals1.os.write(text3);
				CS$<>8__locals1.os.write("  ");
				CS$<>8__locals1.os.write("  ");
				CS$<>8__locals1.os.write(text4);
				CS$<>8__locals1.os.write("  ");
				CS$<>8__locals1.os.write("  ");
				CS$<>8__locals1.os.write(FileEncrypter.MakeReplacementsForDisplay(FileEncrypter.DecryptString(text4, "loooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooongpass")[2]));
				CS$<>8__locals1.os.write("  ");
				CS$<>8__locals1.os.write(FileEncrypter.MakeReplacementsForDisplay((FileEncrypter.DecryptString(text4, "wrongPass")[2] == null) ? "NULL" : "CORRECT"));
				CS$<>8__locals1.os.write("  ");
			}
			else if (array[0].Equals("test") && OS.DEBUG_COMMANDS)
			{
				Computer computer2 = Programs.getComputer(CS$<>8__locals1.os, "dhs");
				DLCHubServer dlchubServer2 = (DLCHubServer)computer2.getDaemon(typeof(DLCHubServer));
				dlchubServer2.AddMission((ActiveMission)ComputerLoader.readMission("Content/DLC/Missions/Attack/AttackMission.xml"), null, false);
			}
			else if (array[0].Equals("testtrace") && OS.DEBUG_COMMANDS)
			{
				MissionFunctions.runCommand(1, "triggerDLCHackRevenge");
			}
			else if (array[0].Equals("testboot") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.BootAssitanceModule.IsActive = true;
				CS$<>8__locals1.os.bootingUp = false;
				CS$<>8__locals1.os.canRunContent = false;
				MusicManager.stop();
			}
			else if (array[0].Equals("testhhbs") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.write(HostileHackerBreakinSequence.IsInBlockingHostileFileState(CS$<>8__locals1.os) ? "BLOCKED" : "SAFE");
			}
			else if (array[0].Equals("printflags") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.write(CS$<>8__locals1.os.Flags.GetSaveString());
			}
			else if (array[0].Equals("loseadmin") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.connectedComp.adminIP = CS$<>8__locals1.os.connectedComp.ip;
				flag = false;
			}
			else if (array[0].Equals("runcmd") && OS.DEBUG_COMMANDS)
			{
				if (array.Length > 1)
				{
					string text5 = array[1];
					int value = 0;
					if (array.Length > 2)
					{
						value = Convert.ToInt32(array[1]);
					}
					MissionFunctions.runCommand(value, text5);
				}
			}
			else if (array[0].ToLower().Equals("runhackscript") && OS.DEBUG_COMMANDS)
			{
				if (array.Length > 1)
				{
					string text5 = array[1];
					try
					{
						HackerScriptExecuter.runScript(text5, CS$<>8__locals1.os, CS$<>8__locals1.os.thisComputer.ip, CS$<>8__locals1.os.thisComputer.ip);
					}
					catch (Exception ex)
					{
						CS$<>8__locals1.os.write("Error launching script " + text5);
						CS$<>8__locals1.os.write(Utils.GenerateReportFromExceptionCompact(ex));
					}
				}
			}
			else if (array[0].Equals("MotIsTheBest") && OS.DEBUG_COMMANDS)
			{
				CS$<>8__locals1.os.runCommand("probe");
				CS$<>8__locals1.os.runCommand("exe WebServerWorm 80");
				CS$<>8__locals1.os.runCommand("exe SSHcrack 22");
				CS$<>8__locals1.os.runCommand("exe SMTPoverflow 25");
				CS$<>8__locals1.os.runCommand("exe FTPBounce 21");
			}
			else if (array[0].Equals("help") || array[0].Equals("Help") || array[0].Equals("?") || array[0].Equals("man"))
			{
				int num4 = 0;
				if (array.Length > 1)
				{
					try
					{
						num4 = Convert.ToInt32(array[1]);
						if (num4 > Helpfile.getNumberOfPages())
						{
							CS$<>8__locals1.os.write("Invalid Page Number - Displaying First Page");
							num4 = 0;
						}
					}
					catch (FormatException)
					{
						CS$<>8__locals1.os.write("Invalid Page Number");
					}
					catch (OverflowException)
					{
						CS$<>8__locals1.os.write("Invalid Page Number");
					}
				}
				Helpfile.writeHelp(CS$<>8__locals1.os, num4);
				flag = false;
			}
			else
			{
				if (array[0] != "")
				{
					int num5 = ProgramRunner.AttemptExeProgramExecution(CS$<>8__locals1.os, array);
					if (num5 == 0)
					{
						CS$<>8__locals1.os.write("Execution failed");
					}
					else if (num5 < 0)
					{
						CS$<>8__locals1.os.write("No Command " + array[0] + " - Check Syntax\n");
					}
				}
				flag = false;
			}
			if (flag)
			{
				if (!CS$<>8__locals1.os.commandInvalid)
				{
					CS$<>8__locals1.os.display.command = array[0];
					CS$<>8__locals1.os.display.commandArgs = array;
					CS$<>8__locals1.os.display.typeChanged();
				}
				else
				{
					CS$<>8__locals1.os.commandInvalid = false;
				}
			}
			return flag;
		}

		// Token: 0x060004E4 RID: 1252 RVA: 0x0004D9F0 File Offset: 0x0004BBF0
		public static bool ExeProgramExists(string name, object binariesFolder)
		{
			Folder folder = (Folder)binariesFolder;
			name = name.Replace(".exe", "").ToLower();
			string text = name;
			if (text != null)
			{
				if (text == "porthack" || text == "forkbomb" || text == "shell" || text == "securitytracer" || text == "tutorial" || text == "notes")
				{
					return true;
				}
			}
			bool result = false;
			for (int i = 0; i < folder.files.Count; i++)
			{
				if (folder.files[i].name.Equals(name) || folder.files[i].name.Replace(".exe", "").Equals(name) || folder.files[i].name.Replace(".exe", "").ToLower().Equals(name))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x0004DB30 File Offset: 0x0004BD30
		private static int GetFileIndexOfExeProgram(string name, object binariesFolder)
		{
			Folder folder = (Folder)binariesFolder;
			name = name.Replace(".exe", "").ToLower();
			string text = name;
			if (text != null)
			{
				if (text == "porthack" || text == "forkbomb" || text == "shell" || text == "tutorial" || text == "notes")
				{
					return int.MaxValue;
				}
			}
			int result = -1;
			for (int i = 0; i < folder.files.Count; i++)
			{
				if (folder.files[i].name.Equals(name) || folder.files[i].name.Replace(".exe", "").Equals(name) || folder.files[i].name.Replace(".exe", "").ToLower().Equals(name))
				{
					result = i;
					break;
				}
			}
			return result;
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x0004DC60 File Offset: 0x0004BE60
		private static int AttemptExeProgramExecution(OS os, string[] p)
		{
			Computer computer = (os.connectedComp != null) ? os.connectedComp : os.thisComputer;
			Folder folder = os.thisComputer.files.root.searchForFolder("bin");
			int fileIndexOfExeProgram = ProgramRunner.GetFileIndexOfExeProgram(p[0], folder);
			bool flag = fileIndexOfExeProgram == int.MaxValue;
			int num = -1;
			int key = -1;
			int result;
			if (fileIndexOfExeProgram >= 0)
			{
				string text = null;
				string text2 = null;
				if (!flag)
				{
					text = folder.files[fileIndexOfExeProgram].data;
					for (int i = 0; i < PortExploits.exeNums.Count; i++)
					{
						int num2 = PortExploits.exeNums[i];
						if (PortExploits.crackExeData[num2].Equals(text) || PortExploits.crackExeDataLocalRNG[num2].Equals(text))
						{
							text2 = PortExploits.cracks[num2].Replace(".exe", "").ToLower();
							key = num2;
							break;
						}
					}
					if (text2 == "ftpsprint")
					{
						key = 21;
						if (text == PortExploits.crackExeData[211] || text == PortExploits.crackExeDataLocalRNG[211])
						{
							num = 211;
						}
					}
				}
				else
				{
					text2 = p[0].Replace(".exe", "").ToLower();
					if (text2 == "notes")
					{
						text = PortExploits.crackExeData[8];
					}
					if (text2 == "tutorial")
					{
						text = PortExploits.crackExeData[1];
					}
				}
				if (text2 != null)
				{
					int num3 = -1;
					int num4 = -1;
					if (!flag && PortExploits.needsPort[key])
					{
						if (p.Length > 1)
						{
							try
							{
								num4 = Convert.ToInt32(p[1]);
								int num5 = num4;
								num4 = computer.GetCodePortNumberFromDisplayPort(num4);
								if (num5 == num4)
								{
									num5 = computer.GetDisplayPortNumberFromCodePort(num4);
									if (num4 != num5)
									{
										num4 = -1;
									}
								}
							}
							catch (FormatException)
							{
								num4 = -1;
							}
						}
						else
						{
							if (text2 == "ssltrojan")
							{
								SSLPortExe.GenerateInstanceOrNullFromArguments(p, Rectangle.Empty, os, computer);
								return 0;
							}
							os.write(LocaleTerms.Loc("No port number Provided"));
							return 0;
						}
					}
					if (!flag && PortExploits.needsPort[key])
					{
						try
						{
							for (int i = 0; i < computer.ports.Count; i++)
							{
								if (computer.ports[i] == num4)
								{
									num3 = computer.ports[i];
									break;
								}
							}
						}
						catch
						{
							num3 = -1;
							os.write(LocaleTerms.Loc("No port number Provided"));
							return 0;
						}
						if (num3 == -1)
						{
							os.write(LocaleTerms.Loc("Target Port is Closed"));
							return 0;
						}
						if (num <= -1)
						{
							num = num3;
						}
						if ((num == 211 && num3 != 21) || (text != PortExploits.crackExeData[num] && text != PortExploits.crackExeDataLocalRNG[num]))
						{
							os.write(LocaleTerms.Loc("Target Port running incompatible service for this executable"));
							return 0;
						}
					}
					os.launchExecutable(text2, text, num3, p, p[0]);
					result = 1;
				}
				else
				{
					result = -1;
				}
			}
			else
			{
				result = -1;
			}
			return result;
		}
	}
}
