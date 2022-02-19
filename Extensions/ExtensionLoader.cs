using System;
using System.IO;
using System.Xml;
using Hacknet.Localization;
using Microsoft.Xna.Framework;

namespace Hacknet.Extensions
{
	// Token: 0x02000073 RID: 115
	public static class ExtensionLoader
	{
		// Token: 0x0600023E RID: 574 RVA: 0x00020914 File Offset: 0x0001EB14
		public static void LoadNewExtensionSession(ExtensionInfo info, object os_obj)
		{
			LocaleActivator.ActivateLocale(info.Language, Game1.getSingleton().Content);
			OS os = (OS)os_obj;
			People.ReInitPeopleForExtension();
			if (Directory.Exists(info.FolderPath + "/Nodes"))
			{
				Utils.ActOnAllFilesRevursivley(info.FolderPath + "/Nodes", delegate(string filename)
				{
					if (filename.EndsWith(".xml"))
					{
						if (OS.TestingPassOnly)
						{
							try
							{
								Computer computer3 = Computer.loadFromFile(filename);
								if (computer3 != null)
								{
									ExtensionLoader.CheckAndAssignCoreServer(computer3, os);
								}
							}
							catch (Exception ex)
							{
								string text2 = "COMPUTER LOAD ERROR:\nError loading computer \"{0}\"";
								Exception ex2 = ex;
								text2 = string.Format(text2, filename);
								while (ex2 != null)
								{
									string str = string.Format("\r\nError: {0} - {1}", ex2.GetType().Name, ex2.Message);
									text2 += str;
									ex2 = ex2.InnerException;
								}
								FormatException ex3 = new FormatException(text2, ex);
								throw ex3;
							}
						}
						else
						{
							Computer computer3 = Computer.loadFromFile(filename);
							if (computer3 != null)
							{
								ExtensionLoader.CheckAndAssignCoreServer(computer3, os);
							}
						}
					}
				});
			}
			if (ComputerLoader.postAllLoadedActions != null)
			{
				ComputerLoader.postAllLoadedActions();
			}
			Computer computer = Programs.getComputer(os, "jmail");
			if (computer == null)
			{
				computer = new Computer("JMail Email Server", NetworkMap.generateRandomIP(), new Vector2(0.8f, 0.2f), 6, 1, os);
				computer.idName = "jmail";
				computer.daemons.Add(new MailServer(computer, "JMail", os));
				MailServer.shouldGenerateJunk = false;
				computer.users.Add(new UserDetail(os.defaultUser.name, "mailpassword", 2));
				computer.initDaemons();
				os.netMap.mailServer = computer;
				os.netMap.nodes.Add(computer);
			}
			for (int i = 0; i < info.StartingVisibleNodes.Length; i++)
			{
				Computer computer2 = Programs.getComputer(os, info.StartingVisibleNodes[i]);
				if (computer2 != null)
				{
					os.netMap.discoverNode(computer2);
				}
			}
			for (int i = 0; i < info.FactionDescriptorPaths.Count; i++)
			{
				string text = info.FolderPath + "/" + info.FactionDescriptorPaths[i];
				using (FileStream fileStream = File.OpenRead(text))
				{
					try
					{
						XmlReader xmlRdr = XmlReader.Create(fileStream);
						Faction faction = Faction.loadFromSave(xmlRdr);
						os.allFactions.factions.Add(faction.idName, faction);
					}
					catch (Exception innerException)
					{
						throw new FormatException("Error loading Faction: " + text, innerException);
					}
				}
			}
			OSTheme theme = OSTheme.Custom;
			bool flag = false;
			foreach (object obj in Enum.GetValues(typeof(OSTheme)))
			{
				string a = obj.ToString().ToLower();
				if (a == info.Theme)
				{
					theme = (OSTheme)obj;
					flag = true;
				}
			}
			if (!flag)
			{
				if (File.Exists(info.FolderPath + "/" + info.Theme))
				{
					ThemeManager.setThemeOnComputer(os.thisComputer, info.Theme);
					ThemeManager.switchTheme(os, info.Theme);
				}
			}
			else
			{
				ThemeManager.setThemeOnComputer(os.thisComputer, theme);
				ThemeManager.switchTheme(os, theme);
			}
			ExtensionLoader.LoadExtensionStartTrackAsCurrentSong(info);
			if (info.StartingActionsPath != null)
			{
				RunnableConditionalActions.LoadIntoOS(info.StartingActionsPath, os);
			}
			if (info.StartingMissionPath != null && !info.StartsWithTutorial && !info.HasIntroStartup)
			{
				ExtensionLoader.SendStartingEmailForActiveExtensionNextFrame(os);
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00020D18 File Offset: 0x0001EF18
		public static void LoadExtensionStartTrackAsCurrentSong(ExtensionInfo info)
		{
			if (info.IntroStartupSong != null)
			{
				string introStartupSong = info.IntroStartupSong;
				string text = introStartupSong;
				if (!introStartupSong.EndsWith(".ogg"))
				{
					text = introStartupSong + ".ogg";
				}
				string text2 = info.FolderPath + "/" + text;
				MusicManager.stop();
				if (File.Exists(text2))
				{
					string songname = text2.StartsWith("Extensions") ? ("../" + text2) : text2;
					MusicManager.loadAsCurrentSong(songname);
				}
				else
				{
					text2 = "Music/" + text;
					if (File.Exists("Content/" + text2))
					{
						MusicManager.loadAsCurrentSong(text2.Replace(".ogg", ""));
					}
					else if (File.Exists("Content/DLC/" + text2))
					{
						MusicManager.loadAsCurrentSong("DLC/" + text2.Replace(".ogg", ""));
					}
				}
				MusicManager.stop();
			}
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00020E9C File Offset: 0x0001F09C
		public static void SendStartingEmailForActiveExtensionNextFrame(object os_obj)
		{
			OS os = (OS)os_obj;
			if (!string.IsNullOrWhiteSpace(ExtensionLoader.ActiveExtensionInfo.StartingMissionPath))
			{
				os.delayer.Post(ActionDelayer.NextTick(), delegate
				{
					ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(ExtensionLoader.ActiveExtensionInfo.FolderPath + "/" + ExtensionLoader.ActiveExtensionInfo.StartingMissionPath);
					os.currentMission = activeMission;
					activeMission.sendEmail(os);
					activeMission.ActivateSuppressedStartFunctionIfPresent();
					os.saveGame();
				});
			}
			if (!os.Flags.HasFlag("ExtensionFirstBootComplete"))
			{
				os.Flags.AddFlag("ExtensionFirstBootComplete");
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00020F30 File Offset: 0x0001F130
		internal static void CheckAndAssignCoreServer(Computer c, OS os)
		{
			if (c.idName.ToLower() == "academic")
			{
				if (c.getDaemon(typeof(AcademicDatabaseDaemon)) != null)
				{
					os.netMap.academicDatabase = c;
				}
			}
			if (c.idName.ToLower() == "jmail")
			{
				if (c.getDaemon(typeof(MailServer)) != null)
				{
					os.netMap.mailServer = c;
				}
			}
			if (c.idName.ToLower() == "ispComp")
			{
				if (c.getDaemon(typeof(ISPDaemon)) == null)
				{
					throw new InvalidOperationException("ispComp Does not have an ISP Daemon on it!");
				}
				for (int i = 0; i < os.netMap.nodes.Count; i++)
				{
					if (os.netMap.nodes[i].idName == "ispComp" && os.netMap.nodes[i] != c)
					{
						os.netMap.nodes[i].idName = "ispOriginalComp";
						os.netMap.nodes[i].ip = NetworkMap.generateRandomIP();
						break;
					}
				}
			}
			if (c.idName.ToLower() == "playercomp")
			{
				os.netMap.nodes.Remove(os.thisComputer);
				os.thisComputer = c;
				c.adminIP = c.ip;
				os.netMap.nodes.Remove(c);
				os.netMap.nodes.Insert(0, c);
				if (!os.netMap.visibleNodes.Contains(0))
				{
					os.netMap.visibleNodes.Add(0);
				}
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x000212C0 File Offset: 0x0001F4C0
		public static void ReloadExtensionNodes(object osobj)
		{
			OS os = (OS)osobj;
			ExtensionInfo activeExtensionInfo = ExtensionLoader.ActiveExtensionInfo;
			if (Directory.Exists(activeExtensionInfo.FolderPath + "/Nodes"))
			{
				Utils.ActOnAllFilesRevursivley(activeExtensionInfo.FolderPath + "/Nodes", delegate(string filename)
				{
					if (filename.EndsWith(".xml"))
					{
						if (OS.TestingPassOnly)
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
								FormatException ex2 = new FormatException(text, ex);
								throw ex2;
							}
						}
						else
						{
							Computer computer = (Computer)ComputerLoader.loadComputer(filename, true, false);
							for (int i = 0; i < os.netMap.nodes.Count; i++)
							{
								Computer computer2 = os.netMap.nodes[i];
								if (computer2.idName == computer.idName)
								{
									computer.location = computer2.location;
									computer.adminIP = computer2.adminIP;
									computer.ip = computer2.ip;
									computer.highlightFlashTime = 1f;
									os.netMap.nodes[i] = computer;
									break;
								}
							}
							if (computer != null)
							{
								ExtensionLoader.CheckAndAssignCoreServer(computer, os);
							}
						}
					}
				});
			}
		}

		// Token: 0x040002BE RID: 702
		public static ExtensionInfo ActiveExtensionInfo = null;
	}
}
