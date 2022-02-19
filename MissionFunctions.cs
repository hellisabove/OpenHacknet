using System;
using System.Collections.Generic;
using Hacknet.Factions;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x02000130 RID: 304
	public static class MissionFunctions
	{
		// Token: 0x06000750 RID: 1872 RVA: 0x00077FB0 File Offset: 0x000761B0
		private static void assertOS()
		{
			MissionFunctions.os = OS.currentInstance;
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x000784D4 File Offset: 0x000766D4
		public static void runCommand(int value, string name)
		{
			MissionFunctions.assertOS();
			if (!(name.ToLower().Trim() == "none"))
			{
				if (name.Equals("addRank"))
				{
					if (!OS.TestingPassOnly || MissionFunctions.os.currentFaction != null)
					{
						MissionFunctions.os.currentFaction.addValue(value, MissionFunctions.os);
						string subject = LocaleTerms.Loc("Contract Successful");
						string body = string.Format(Utils.readEntireFile("Content/LocPost/MissionCompleteEmail.txt"), MissionFunctions.os.currentFaction.getRank(), MissionFunctions.os.currentFaction.getMaxRank(), MissionFunctions.os.currentFaction.name);
						string sender = MissionFunctions.os.currentFaction.name + " ReplyBot";
						string mail = MailServer.generateEmail(subject, body, sender);
						MailServer mailServer = (MailServer)MissionFunctions.os.netMap.mailServer.getDaemon(typeof(MailServer));
						mailServer.addMail(mail, MissionFunctions.os.defaultUser.name);
					}
					else if (OS.DEBUG_COMMANDS && MissionFunctions.os.currentFaction == null)
					{
						MissionFunctions.os.write("----------");
						MissionFunctions.os.write("----------");
						MissionFunctions.os.write("ERROR IN FUNCTION 'addRank'");
						MissionFunctions.os.write("Player is not assigned to a faction, so rank cannot be added!");
						MissionFunctions.os.write("Make sure you have assigned a player a faction with the 'SetFaction' function before using this!");
						MissionFunctions.os.write("----------");
						MissionFunctions.os.write("----------");
					}
				}
				else if (name.Equals("addRankSilent"))
				{
					if (!OS.TestingPassOnly || MissionFunctions.os.currentFaction != null)
					{
						MissionFunctions.os.currentFaction.addValue(value, MissionFunctions.os);
					}
				}
				else if (name.StartsWith("addFlags:"))
				{
					string text = name.Substring("addFlags:".Length);
					string[] array = text.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						MissionFunctions.os.Flags.AddFlag(array[i]);
					}
					CustomFaction customFaction = MissionFunctions.os.currentFaction as CustomFaction;
					if (customFaction != null)
					{
						customFaction.CheckForAllCustomActionsToRun(MissionFunctions.os);
					}
				}
				else if (name.StartsWith("removeFlags:"))
				{
					string text = name.Substring("removeFlags:".Length);
					string[] array = text.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						if (MissionFunctions.os.Flags.HasFlag(array[i]))
						{
							MissionFunctions.os.Flags.RemoveFlag(array[i]);
						}
					}
					CustomFaction customFaction = MissionFunctions.os.currentFaction as CustomFaction;
					if (customFaction != null)
					{
						customFaction.CheckForAllCustomActionsToRun(MissionFunctions.os);
					}
				}
				else if (name.StartsWith("setFaction:"))
				{
					string text2 = name.Substring("setFaction:".Length);
					bool flag = false;
					foreach (KeyValuePair<string, Faction> keyValuePair in MissionFunctions.os.allFactions.factions)
					{
						if (keyValuePair.Value.idName.ToLower() == text2.ToLower())
						{
							MissionFunctions.os.allFactions.setCurrentFaction(text2, MissionFunctions.os);
							flag = true;
							break;
						}
					}
					if (!flag && OS.TestingPassOnly)
					{
						throw new NullReferenceException("Faction " + text2 + "not found for setFaction action!");
					}
				}
				else if (name.StartsWith("loadConditionalActions:"))
				{
					string filepath = name.Substring("loadConditionalActions:".Length);
					RunnableConditionalActions.LoadIntoOS(filepath, MissionFunctions.os);
				}
				else if (name.Equals("triggerThemeHackRevenge"))
				{
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(5.0), delegate
					{
						string subject2 = LocaleTerms.Loc("Are you Kidding me?");
						string body2 = Utils.readEntireFile("Content/LocPost/NaixEmail.txt");
						string sender2 = "naix@jmail.com";
						string mail2 = MailServer.generateEmail(subject2, body2, sender2);
						MailServer mailServer2 = (MailServer)MissionFunctions.os.netMap.mailServer.getDaemon(typeof(MailServer));
						mailServer2.addMail(mail2, MissionFunctions.os.defaultUser.name);
						MissionFunctions.os.delayer.Post(ActionDelayer.Wait(24.0), delegate
						{
							try
							{
								HackerScriptExecuter.runScript("HackerScripts/ThemeHack.txt", MissionFunctions.os, null, null);
							}
							catch (Exception ex2)
							{
								if (!Settings.recoverFromErrorsSilently)
								{
									throw ex2;
								}
								MissionFunctions.os.write("CAUTION: UNSYNDICATED OUTSIDE CONNECTION ATTEMPT");
								MissionFunctions.os.write("RECOVERED FROM CONNECTION SUBTERFUGE SUCCESSFULLY");
								Console.WriteLine("Critical error loading hacker script - aborting");
							}
						});
					});
				}
				else if (name.Equals("changeSong"))
				{
					switch (value)
					{
					default:
						MusicManager.transitionToSong("Music\\Revolve");
						break;
					case 2:
						MusicManager.transitionToSong("Music\\The_Quickening");
						break;
					case 3:
						MusicManager.transitionToSong("Music\\TheAlgorithm");
						break;
					case 4:
						MusicManager.transitionToSong("Music\\Ryan3");
						break;
					case 5:
						MusicManager.transitionToSong("Music\\Bit(Ending)");
						break;
					case 6:
						MusicManager.transitionToSong("Music\\Rico_Puestel-Roja_Drifts_By");
						break;
					case 7:
						MusicManager.transitionToSong("Music\\out_run_the_wolves");
						break;
					case 8:
						MusicManager.transitionToSong("Music\\Irritations");
						break;
					case 9:
						MusicManager.transitionToSong("Music\\Broken_Boy");
						break;
					case 10:
						MusicManager.transitionToSong("Music\\Ryan10");
						break;
					case 11:
						MusicManager.transitionToSong("Music\\tetrameth");
						break;
					}
				}
				else if (name.Equals("entropyEndMissionSetup"))
				{
					MissionFunctions.runCommand(3, "changeSong");
					Computer computer = MissionFunctions.findComp("corp0#IS");
					Computer computer2 = MissionFunctions.findComp("corp0#MF");
					Computer computer3 = MissionFunctions.findComp("corp0#BU");
					FileEntry fileEntry = new FileEntry(Computer.generateBinaryString(5000), "HacknetOS.rar");
					FileEntry fileEntry2 = new FileEntry(Computer.generateBinaryString(4000), "HacknetOS_Data.xnb");
					FileEntry fileEntry3 = new FileEntry(Computer.generateBinaryString(4000), "HacknetOS_Content.xnb");
					Folder folder = computer.files.root.folders[2];
					folder.files.Add(fileEntry);
					folder.files.Add(fileEntry2);
					folder.files.Add(fileEntry3);
					folder = computer2.files.root.folders[2];
					folder.files.Add(fileEntry);
					folder.files.Add(fileEntry2);
					folder.files.Add(fileEntry3);
					fileEntry = new FileEntry(fileEntry.data, fileEntry.name + "_backup");
					fileEntry2 = new FileEntry(fileEntry2.data, fileEntry2.name + "_backup");
					fileEntry3 = new FileEntry(fileEntry3.data, fileEntry3.name + "_backup");
					folder = computer3.files.root.folders[2];
					folder.files.Add(fileEntry);
					folder.files.Add(fileEntry2);
					folder.files.Add(fileEntry3);
					computer.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
					computer3.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
					computer2.traceTime = Computer.BASE_TRACE_TIME * 7.5f;
					computer2.portsNeededForCrack = 3;
					computer.portsNeededForCrack = 2;
					computer3.portsNeededForCrack = 2;
					Computer computer4 = MissionFunctions.findComp("entropy01");
					Folder folder2 = computer4.files.root.folders[2];
					folder2.files.Add(new FileEntry(PortExploits.crackExeData[25], "SMTPoverflow.exe"));
					folder2.files.Add(new FileEntry(PortExploits.crackExeData[80], "WebServerWorm.exe"));
				}
				else if (name.Equals("entropyAddSMTPCrack"))
				{
					Computer computer4 = MissionFunctions.findComp("entropy01");
					Folder folder2 = computer4.files.root.folders[2];
					bool flag2 = false;
					for (int i = 0; i < folder2.files.Count; i++)
					{
						if (folder2.files[i].data == PortExploits.crackExeData[25] || folder2.files[i].name == "SMTPoverflow.exe")
						{
							flag2 = true;
						}
					}
					if (!flag2)
					{
						folder2.files.Add(new FileEntry(PortExploits.crackExeData[25], Utils.GetNonRepeatingFilename("SMTPoverflow", ".exe", folder2)));
					}
					MissionFunctions.os.Flags.AddFlag("ThemeHackTransitionAssetsAdded");
				}
				else if (name.Equals("transitionToBitMissions"))
				{
					if (Settings.isDemoMode)
					{
						MissionFunctions.runCommand(6, "changeSong");
						if (Settings.isPressBuildDemo)
						{
							ComputerLoader.loadMission("Content/Missions/Demo/PressBuild/DemoMission01.xml", false);
						}
						else
						{
							ComputerLoader.loadMission("Content/Missions/Demo/AvconDemo.xml", false);
						}
					}
					else
					{
						ComputerLoader.loadMission("Content/Missions/BitMission0.xml", false);
					}
				}
				else if (name.Equals("entropySendCSECInvite"))
				{
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(6.0), delegate
					{
						ComputerLoader.loadMission("Content/Missions/MainHub/Intro/Intro01.xml", false);
					});
				}
				else if (name.Equals("hubBitSetComplete01"))
				{
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(4.0), delegate
					{
						MissionFunctions.runCommand(1, "addRank");
					});
					MissionFunctions.runCommand(3, "changeSong");
					MissionFunctions.os.Flags.AddFlag("csecBitSet01Complete");
				}
				else if (name.Equals("enTechEnableOfflineBackup"))
				{
					Computer computer5 = Programs.getComputer(MissionFunctions.os, "EnTechOfflineBackup");
					MissionFunctions.os.Flags.AddFlag("VaporSequencerEnabled");
					Computer computer4 = MissionFunctions.findComp("mainHubAssets");
					Folder folder2 = computer4.files.root.searchForFolder("bin");
					Folder folder3 = folder2.searchForFolder("Sequencer");
					if (folder3 == null)
					{
						folder3 = new Folder("Sequencer");
						folder2.folders.Add(folder3);
					}
					if (folder3.searchForFile("Sequencer.exe") == null)
					{
						folder3.files.Add(new FileEntry(PortExploits.crackExeData[17], "Sequencer.exe"));
					}
				}
				else if (name.Equals("rudeNaixResponse"))
				{
					AchievementsManager.Unlock("rude_response", false);
				}
				else if (name.Equals("assignPlayerToHubServerFaction"))
				{
					MissionFunctions.os.allFactions.setCurrentFaction("hub", MissionFunctions.os);
					Computer computer6 = Programs.getComputer(MissionFunctions.os, "mainHub");
					MissionHubServer missionHubServer = (MissionHubServer)computer6.getDaemon(typeof(MissionHubServer));
					UserDetail userDetail = new UserDetail(MissionFunctions.os.defaultUser.name, "reptile", 3);
					computer6.addNewUser(computer6.ip, userDetail);
					missionHubServer.addUser(userDetail);
					MissionFunctions.os.homeNodeID = "mainHub";
					MissionFunctions.os.homeAssetServerID = "mainHubAssets";
					MissionFunctions.runCommand(3, "changeSong");
					MissionFunctions.os.Flags.AddFlag("CSEC_Member");
					AchievementsManager.Unlock("progress_csec", false);
					if (MissionFunctions.os.HasLoadedDLCContent && Settings.EnableDLC)
					{
						if (!MissionFunctions.os.Flags.HasFlag("dlc_complete"))
						{
							missionHubServer.AddMissionToListings("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLCConnectorIntro.xml", 1);
						}
					}
				}
				else if (name.Equals("assignPlayerToEntropyFaction"))
				{
					MissionFunctions.runCommand(6, "changeSong");
					MissionFunctions.os.homeNodeID = "entropy00";
					MissionFunctions.os.homeAssetServerID = "entropy01";
					AchievementsManager.Unlock("progress_entropy", false);
				}
				else if (name.Equals("assignPlayerToLelzSec"))
				{
					MissionFunctions.os.homeNodeID = "lelzSecHub";
					MissionFunctions.os.homeAssetServerID = "lelzSecHub";
					MissionFunctions.os.Flags.AddFlag("LelzSec_Member");
					AchievementsManager.Unlock("progress_lelz", false);
				}
				else if (name.Equals("lelzSecVictory"))
				{
					AchievementsManager.Unlock("secret_path_complete", false);
				}
				else if (name.Equals("demoFinalMissionEnd"))
				{
					MissionFunctions.os.exes.Clear();
					PostProcessor.EndingSequenceFlashOutActive = true;
					PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
					MusicManager.stop();
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
					{
						MissionFunctions.os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_down").Play();
					});
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(3.0), delegate
					{
						PostProcessor.dangerModeEnabled = false;
						PostProcessor.dangerModePercentComplete = 0f;
						MissionFunctions.os.ExitScreen();
						MissionFunctions.os.ScreenManager.AddScreen(new DemoEndScreen());
					});
				}
				else if (name.Equals("demoFinalMissionEndDLC"))
				{
					if (Settings.isDemoMode)
					{
						MissionFunctions.os.exes.Clear();
						PostProcessor.EndingSequenceFlashOutActive = true;
						PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
						MusicManager.stop();
						MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.0), delegate
						{
							MissionFunctions.os.content.Load<SoundEffect>("SFX/BrightFlash").Play();
						});
						MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.4), delegate
						{
							MissionFunctions.os.content.Load<SoundEffect>("SFX/TraceKill").Play();
						});
						MissionFunctions.os.delayer.Post(ActionDelayer.Wait(1.6), delegate
						{
							MusicManager.playSongImmediatley("DLC/Music/DreamHead");
							PostProcessor.dangerModeEnabled = false;
							PostProcessor.dangerModePercentComplete = 0f;
							MissionFunctions.os.ScreenManager.AddScreen(new DemoEndScreen
							{
								StopsMusic = false,
								IsDLCDemoScreen = true
							});
							MissionFunctions.os.ExitScreen();
						});
					}
				}
				else if (name.Equals("demoFinalMissionStart"))
				{
					MissionFunctions.os.Flags.AddFlag("DemoSequencerEnabled");
					MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
				}
				else if (name.Equals("CSECTesterGameWorldSetup"))
				{
					int i = 0;
					while (i < PortExploits.services.Count && i < 4)
					{
						MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[i]], PortExploits.cracks[PortExploits.portNums[i]]));
						i++;
					}
					for (i = 0; i < 4; i++)
					{
						Computer computer5 = new Computer("DebugShell" + i, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, 2, MissionFunctions.os);
						computer5.adminIP = MissionFunctions.os.thisComputer.adminIP;
						MissionFunctions.os.netMap.nodes.Add(computer5);
						MissionFunctions.os.netMap.discoverNode(computer5);
					}
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
					{
						MissionFunctions.os.allFactions.setCurrentFaction("entropy", MissionFunctions.os);
						MissionFunctions.os.currentMission = null;
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy00"));
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy01"));
					});
				}
				else if (name.Equals("EntropyFastFowardSetup"))
				{
					MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], PortExploits.cracks[22]));
					MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], PortExploits.cracks[21]));
					for (int i = 0; i < 3; i++)
					{
						Computer computer5 = new Computer("DebugShell" + i, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, 2, MissionFunctions.os);
						computer5.adminIP = MissionFunctions.os.thisComputer.adminIP;
						MissionFunctions.os.netMap.nodes.Add(computer5);
						MissionFunctions.os.netMap.discoverNode(computer5);
					}
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
					{
						MissionFunctions.os.allFactions.setCurrentFaction("entropy", MissionFunctions.os);
						MissionFunctions.os.currentMission = null;
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy00"));
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "entropy01"));
						Computer computer10 = Programs.getComputer(MissionFunctions.os, "entropy01");
						UserDetail value2 = computer10.users[0];
						value2.known = true;
						computer10.users[0] = value2;
						MissionFunctions.os.allFactions.factions[MissionFunctions.os.allFactions.currentFaction].playerValue = 2;
						MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
						{
							MissionFunctions.os.Flags.AddFlag("eosPathStarted");
							ComputerLoader.loadMission("Content/Missions/Entropy/StartingSet/eosMissions/eosIntroDelayer.xml", false);
						});
					});
				}
				else if (name.Equals("CSECFastFowardSetup"))
				{
					MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], PortExploits.cracks[22]));
					MissionFunctions.os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], PortExploits.cracks[21]));
					for (int i = 0; i < 3; i++)
					{
						Computer computer5 = new Computer("DebugShell" + i, NetworkMap.generateRandomIP(), MissionFunctions.os.netMap.getRandomPosition(), 0, 2, MissionFunctions.os);
						computer5.adminIP = MissionFunctions.os.thisComputer.adminIP;
						MissionFunctions.os.netMap.nodes.Add(computer5);
						MissionFunctions.os.netMap.discoverNode(computer5);
					}
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(0.2), delegate
					{
						MissionFunctions.runCommand(0, "assignPlayerToHubServerFaction");
						MissionFunctions.os.currentMission = null;
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "mainHub"));
						MissionFunctions.os.netMap.discoverNode(Programs.getComputer(MissionFunctions.os, "mainHubAssets"));
						Computer computer10 = Programs.getComputer(MissionFunctions.os, "mainHubAssets");
						UserDetail value2 = computer10.users[0];
						value2.known = true;
						computer10.users[0] = value2;
					});
				}
				else if (name.Equals("csecAddTraceKill"))
				{
					Computer computer4 = MissionFunctions.findComp("mainHubAssets");
					Folder folder2 = computer4.files.root.searchForFolder("bin");
					Folder folder3 = folder2.searchForFolder("TK");
					if (folder3 == null)
					{
						folder3 = new Folder("TK");
						folder2.folders.Add(folder3);
					}
					folder3.files.Add(new FileEntry(FileEncrypter.EncryptString(PortExploits.crackExeData[12], "Vapor Trick Enc.", "NULL", "dx122DX", ".exe"), Utils.GetNonRepeatingFilename("TraceKill", ".dec", folder3)));
					MissionFunctions.os.Flags.AddFlag("bitPathStarted");
					MissionFunctions.runCommand(10, "changeSong");
				}
				else if (name.Equals("junebugComplete"))
				{
					Computer computer5 = Programs.getComputer(MissionFunctions.os, "pacemaker01");
					if (computer5 != null)
					{
						HeartMonitorDaemon heartMonitorDaemon = (HeartMonitorDaemon)computer5.getDaemon(typeof(HeartMonitorDaemon));
						if (heartMonitorDaemon != null)
						{
							heartMonitorDaemon.ForceStopBeepSustainSound();
						}
					}
					MissionFunctions.runCommand(1, "addRank");
				}
				else if (name.Equals("eosIntroMissionSetup"))
				{
					Computer computer4 = MissionFunctions.findComp("entropy01");
					Folder folder2 = computer4.files.root.searchForFolder("bin");
					folder2.files.Add(new FileEntry(PortExploits.crackExeData[13], "eosDeviceScan.exe"));
					MissionFunctions.os.delayer.Post(ActionDelayer.Wait(8.0), delegate
					{
						string body2 = Utils.readEntireFile("Content/Post/eosScannerMail.txt");
						string str = Utils.readEntireFile("Content/LocPost/eOSNote.txt");
						string text5 = "note#%#" + LocaleTerms.Loc("eOS Security Basics") + "#%#" + str;
						string mail2 = MailServer.generateEmail("Fwd: eOS Stuff", body2, "vtfx", new List<string>(new string[]
						{
							text5
						}));
						MailServer mailServer2 = (MailServer)MissionFunctions.os.netMap.mailServer.getDaemon(typeof(MailServer));
						mailServer2.addMail(mail2, MissionFunctions.os.defaultUser.name);
						MissionFunctions.os.saveGame();
					});
					MissionFunctions.runCommand(4, "changeSong");
					MissionFunctions.os.saveGame();
				}
				else if (name.Equals("eosIntroEndFunc"))
				{
					MissionFunctions.runCommand(1, "addRank");
					Computer computer7 = MissionFunctions.findComp("entropy00");
					MissionListingServer missionListingServer = (MissionListingServer)computer7.getDaemon(typeof(MissionListingServer));
					List<ActiveMission> branchMissions = MissionFunctions.os.branchMissions;
					ActiveMission m = (ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/StartingSet/eosMissions/eosAddedMission.xml");
					missionListingServer.addMisison(m, false);
					MissionFunctions.os.branchMissions = branchMissions;
				}
				else if (name.Equals("changeSongDLC"))
				{
					switch (value)
					{
					default:
						MusicManager.transitionToSong("DLC\\Music\\Remi2");
						break;
					case 2:
						MusicManager.transitionToSong("DLC\\Music\\snidelyWhiplash");
						break;
					case 3:
						MusicManager.transitionToSong("DLC\\Music\\Slow_Motion");
						break;
					case 4:
						MusicManager.transitionToSong("DLC\\Music\\World_Chase");
						break;
					case 5:
						MusicManager.transitionToSong("DLC\\Music\\HOME_Resonance");
						break;
					case 6:
						MusicManager.transitionToSong("DLC\\Music\\Remi_Finale");
						break;
					case 7:
						MusicManager.transitionToSong("DLC\\Music\\RemiDrone");
						break;
					case 8:
						MusicManager.transitionToSong("DLC\\Music\\DreamHead");
						break;
					case 9:
						MusicManager.transitionToSong("DLC\\Music\\Userspacelike");
						break;
					case 10:
						MusicManager.transitionToSong("DLC\\Music\\CrashTrack");
						break;
					}
				}
				else if (name.Equals("scanAndStartDLCVenganceHack"))
				{
					Computer computer5 = MissionFunctions.findComp("dAttackTarget");
					if (computer5 != null)
					{
						Folder folder4 = computer5.files.root.searchForFolder("log");
						bool flag3 = false;
						for (int i = 0; i < folder4.files.Count; i++)
						{
							if (folder4.files[i].data.Contains(MissionFunctions.os.thisComputer.ip))
							{
								SARunFunction action = new SARunFunction
								{
									DelayHost = "dhs",
									FunctionName = "triggerDLCHackRevenge",
									FunctionValue = 1
								};
								Computer computer8 = Programs.getComputer(MissionFunctions.os, "dhs");
								DLCHubServer dlchubServer = (DLCHubServer)computer8.getDaemon(typeof(DLCHubServer));
								dlchubServer.DelayedActions.AddAction(action, 16f);
								break;
							}
						}
						if (!flag3)
						{
							MissionFunctions.runCommand(4, "changeSongDLC");
						}
					}
				}
				else if (name.Equals("triggerDLCHackRevenge"))
				{
					try
					{
						HackerScriptExecuter.runScript("DLC/ActionScripts/Hackers/SystemHack.txt", MissionFunctions.os, null, null);
					}
					catch (Exception ex)
					{
						if (!Settings.recoverFromErrorsSilently)
						{
							throw ex;
						}
						MissionFunctions.os.write("CAUTION: UNSYNDICATED OUTSIDE CONNECTION ATTEMPT");
						MissionFunctions.os.write("RECOVERED FROM CONNECTION SUBTERFUGE SUCCESSFULLY");
						Console.WriteLine("Critical error loading hacker script - aborting\r\n" + Utils.GenerateReportFromException(ex));
					}
				}
				else if (name.Equals("activateAircraftStatusOverlay"))
				{
					MissionFunctions.os.AircraftInfoOverlay.Activate();
					MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
				}
				else if (name.Equals("activateAircraftStatusOverlayLabyrinthsMonitoring"))
				{
					MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
				}
				else if (name.Equals("deActivateAircraftStatusOverlay"))
				{
					MissionFunctions.os.AircraftInfoOverlay.IsActive = false;
					MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = false;
					MissionFunctions.os.Flags.AddFlag("AircraftInfoOverlayDeactivated");
				}
				else if (name.Equals("defAttackAircraft"))
				{
					Computer computer9 = Programs.getComputer(MissionFunctions.os, "dair_crash");
					Folder folder4 = computer9.files.root.searchForFolder("FlightSystems");
					for (int i = 0; i < folder4.files.Count; i++)
					{
						if (folder4.files[i].name == "747FlightOps.dll")
						{
							folder4.files.RemoveAt(i);
							break;
						}
					}
					AircraftDaemon aircraftDaemon = (AircraftDaemon)computer9.getDaemon(typeof(AircraftDaemon));
					aircraftDaemon.StartReloadFirmware();
					if (!MissionFunctions.os.AircraftInfoOverlay.IsActive)
					{
						MissionFunctions.os.AircraftInfoOverlay.Activate();
						MissionFunctions.os.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
					}
				}
				else if (name.Equals("playAirlineCrashSongSequence"))
				{
					MusicManager.playSongImmediatley("DLC\\Music\\Remi_Finale");
					MediaPlayer.IsRepeating = false;
				}
				else if (name.Equals("flashUI"))
				{
					MissionFunctions.os.warningFlash();
				}
				else if (name.Equals("addRankSilent"))
				{
					MissionFunctions.os.currentFaction.addValue(value, MissionFunctions.os);
				}
				else if (name.StartsWith("addRankFaction:"))
				{
					string text2 = name.Substring("addRankFaction:".Length);
					foreach (KeyValuePair<string, Faction> keyValuePair in MissionFunctions.os.allFactions.factions)
					{
						if (keyValuePair.Value.idName.ToLower() == text2.ToLower())
						{
							keyValuePair.Value.addValue(value, MissionFunctions.os);
							break;
						}
					}
				}
				else if (name.StartsWith("setHubServer:"))
				{
					string text3 = name.Substring("setHubServer:".Length);
					MissionFunctions.os.homeNodeID = text3;
				}
				else if (name.StartsWith("setAssetServer:"))
				{
					string text3 = name.Substring("setAssetServer:".Length);
					MissionFunctions.os.homeAssetServerID = text3;
				}
				else if (name.StartsWith("playCustomSong:"))
				{
					string text4 = name.Substring("playCustomSong:".Length);
					text4 = Utils.GetFileLoadPrefix() + text4;
					if (text4.EndsWith(".ogg"))
					{
						text4 = text4.Substring(0, text4.Length - ".ogg".Length);
					}
					if (text4.StartsWith("Content"))
					{
						text4 = text4.Substring("Content/".Length);
					}
					else if (text4.StartsWith("Extensions"))
					{
						text4 = "../" + text4;
					}
					MusicManager.transitionToSong(text4);
				}
				else if (name.StartsWith("playCustomSongImmediatley:"))
				{
					string text4 = name.Substring("playCustomSongImmediatley:".Length);
					text4 = Utils.GetFileLoadPrefix() + text4;
					if (text4.EndsWith(".ogg"))
					{
						text4 = text4.Substring(0, text4.Length - ".ogg".Length);
					}
					if (text4.StartsWith("Content"))
					{
						text4 = text4.Substring("Content/".Length);
					}
					else if (text4.StartsWith("Extensions"))
					{
						text4 = "../" + text4;
					}
					MusicManager.playSongImmediatley(text4);
				}
				else
				{
					if (OS.TestingPassOnly)
					{
						if (!string.IsNullOrWhiteSpace(name))
						{
							throw new FormatException("No Command Function " + name);
						}
					}
					if (MissionFunctions.ReportErrorInCommand != null)
					{
						MissionFunctions.ReportErrorInCommand(string.Concat(new object[]
						{
							"No command found for \"",
							name,
							"\" with value \"",
							value,
							"\""
						}));
					}
				}
			}
		}

		// Token: 0x06000752 RID: 1874 RVA: 0x0007A384 File Offset: 0x00078584
		private static void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000753 RID: 1875 RVA: 0x0007A38C File Offset: 0x0007858C
		private static Computer findComp(string target)
		{
			for (int i = 0; i < MissionFunctions.os.netMap.nodes.Count; i++)
			{
				if (MissionFunctions.os.netMap.nodes[i].idName.Equals(target))
				{
					return MissionFunctions.os.netMap.nodes[i];
				}
			}
			return null;
		}

		// Token: 0x04000837 RID: 2103
		private static OS os;

		// Token: 0x04000838 RID: 2104
		public static Action<string> ReportErrorInCommand;
	}
}
