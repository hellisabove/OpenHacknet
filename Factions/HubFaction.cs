using System;
using System.Collections.Generic;

namespace Hacknet.Factions
{
	// Token: 0x020000DB RID: 219
	internal class HubFaction : Faction
	{
		// Token: 0x06000462 RID: 1122 RVA: 0x0004617C File Offset: 0x0004437C
		public HubFaction(string _name, int _neededValue) : base(_name, _neededValue)
		{
			this.PlayerLosesValueOnAbandon = false;
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x0004623C File Offset: 0x0004443C
		public override void addValue(int value, object os)
		{
			int playerValue = this.playerValue;
			base.addValue(value, os);
			if (base.valuePassedPoint(playerValue, 1) && !((OS)os).Flags.HasFlag("themeChangerAdded"))
			{
				Computer computer = Programs.getComputer((OS)os, "mainHubAssets");
				Folder folder = computer.files.root.searchForFolder("bin");
				Folder folder2 = new Folder("ThemeChanger");
				folder2.files.Add(new FileEntry(PortExploits.crackExeData[14], "ThemeChanger.exe"));
				string dataEntry = Utils.readEntireFile("Content/LocPost/ThemeChangerReadme.txt");
				folder2.files.Add(new FileEntry(dataEntry, "info.txt"));
				folder.folders.Add(folder2);
				((OS)os).delayer.Post(ActionDelayer.Wait(1.0), delegate
				{
					this.SendAssetAddedNotification(os);
					((OS)os).Flags.AddFlag("themeChangerAdded");
					((OS)os).saveGame();
				});
			}
			if (base.valuePassedPoint(playerValue, 4))
			{
				((OS)os).delayer.Post(ActionDelayer.Wait(2.0), delegate
				{
					Computer computer6 = Programs.getComputer((OS)os, "mainHub");
					MissionHubServer missionHubServer3 = (MissionHubServer)computer6.getDaemon(typeof(MissionHubServer));
					missionHubServer3.AddMissionToListings("Content/Missions/MainHub/BitSet/Missions/BitHubSet01.xml", -1);
					((OS)os).saveGame();
				});
			}
			else if (this.playerValue >= 7 && ((OS)os).Flags.HasFlag("decypher") && ((OS)os).Flags.HasFlag("dechead") && !((OS)os).Flags.HasFlag("csecRankingS2Pass"))
			{
				this.SendNotification(os, "Project Junebug");
				((OS)os).Flags.AddFlag("csecRankingS2Pass");
				((OS)os).saveGame();
			}
			else if (this.playerValue >= 10 && !((OS)os).Flags.HasFlag("bitPathStarted"))
			{
				Computer computer2 = Programs.getComputer((OS)os, "mainHub");
				MissionHubServer missionHubServer = (MissionHubServer)computer2.getDaemon(typeof(MissionHubServer));
				if (missionHubServer != null)
				{
					if (missionHubServer.GetNumberOfAvaliableMissions() > 0)
					{
						return;
					}
				}
				this.ForceStartBitMissions(os);
			}
			if (this.playerValue >= 2 && ((OS)os).Flags.HasFlag("dlc_complete") && DLC1SessionUpgrader.HasDLC1Installed && !((OS)os).Flags.HasFlag("dlc_post_missionadded"))
			{
				bool flag = false;
				string flagStartingWith = ((OS)os).Flags.GetFlagStartingWith("dlc_csec_end_facval");
				if (flagStartingWith != null)
				{
					try
					{
						string value2 = flagStartingWith.Substring(flagStartingWith.IndexOf(":") + 1);
						int num = Convert.ToInt32(value2);
						if (this.playerValue - num > 1)
						{
							flag = true;
						}
					}
					catch (Exception ex)
					{
						Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
					}
				}
				else
				{
					flag = true;
				}
				if (flag && ((OS)os).Flags.HasFlag("DLC_PlaneSaveResponseTriggered"))
				{
					((OS)os).Flags.AddFlag("dlc_post_missionadded");
					Computer computer2 = Programs.getComputer((OS)os, "mainHub");
					MissionHubServer missionHubServer2 = (MissionHubServer)computer2.getDaemon(typeof(MissionHubServer));
					Computer computer3 = Programs.getComputer((OS)os, "dHidden");
					Folder folder3 = computer3.files.root.searchForFolder("home");
					FileEntry item = folder3.searchForFile("PA_0022_Incident.dec");
					Computer computer4 = Programs.getComputer((OS)os, "mainHubAssets");
					computer4.files.root.searchForFolder("home").files.Add(item);
					missionHubServer2.AddMissionToListings("Content/DLC/Missions/Injections/Missions/CSEC_Injection_Mission.xml", -1);
					Computer computer5 = Programs.getComputer((OS)os, "dAttackHome");
					Folder folder4 = computer5.files.root.searchForFolder("home").searchForFolder("uni");
					if (folder4.files.Count == 0)
					{
						MailServer mailServer = ((OS)os).netMap.mailServer.getDaemon(typeof(MailServer)) as MailServer;
						string text = Utils.readEntireFile("Content/DLC/Docs/StrikerLateEmail.txt");
						int num2 = text.IndexOf("\n");
						string subject = text.Substring(0, num2).Trim();
						string body = text.Substring(num2, text.Length - num2).Trim();
						List<string> list = new List<string>();
						if (Settings.ActiveLocale == "en-us")
						{
							list.Add("note#%#Important Extra Information#%#Fuck you");
						}
						mailServer.addMail(MailServer.generateEmail(subject, body, "StrikeR", list), ((OS)os).defaultUser.name);
					}
					((OS)os).saveGame();
				}
			}
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00046848 File Offset: 0x00044A48
		public void ForceStartBitMissions(object os)
		{
			((OS)os).Flags.AddFlag("bitPathStarted");
			((OS)os).delayer.Post(ActionDelayer.Wait(1.6), delegate
			{
				ComputerLoader.loadMission("Content/Missions/BitPath/BitAdv_Intro.xml", false);
			});
			Computer computer = Programs.getComputer((OS)os, "mainHubAssets");
			Folder folder = computer.files.root.searchForFolder("bin");
			Folder folder2 = new Folder("Misc");
			folder2.files.Add(new FileEntry(PortExploits.crackExeData[9], "Decypher.exe"));
			folder2.files.Add(new FileEntry(PortExploits.crackExeData[10], "DECHead.exe"));
			folder2.files.Add(new FileEntry(PortExploits.crackExeData[104], "KBT_PortTest.exe"));
			folder2.files.Add(new FileEntry("Kellis BioTech medical port cycler - target 104-103.", "kbt_readme.txt"));
			folder.folders.Add(folder2);
			this.SendNotification(os, string.Concat(new string[]
			{
				LocaleTerms.Loc("Agent"),
				",\n",
				LocaleTerms.Loc("Additional resources have been added to the CSEC members asset pool, for your free use."),
				" ",
				LocaleTerms.Loc("Find them in the misc folder on the asset server."),
				"\n\n",
				LocaleTerms.Loc("Thankyou"),
				",\n -",
				this.name
			}), this.name + " " + LocaleTerms.Loc("Admins :: Asset Uploads"));
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x000469F8 File Offset: 0x00044BF8
		private void SendNotification(object osIn, string body, string subject)
		{
			OS os = (OS)osIn;
			string sender = this.name + " ReplyBot";
			string mail = MailServer.generateEmail(subject, body, sender);
			MailServer mailServer = (MailServer)os.netMap.mailServer.getDaemon(typeof(MailServer));
			mailServer.addMail(mail, os.defaultUser.name);
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00046A5C File Offset: 0x00044C5C
		private void SendNotification(object osIn, string contractName)
		{
			OS os = (OS)osIn;
			string subject = this.name + " " + LocaleTerms.Loc("Admins :: Flagged for Critical Contract");
			string body = string.Format(Utils.readEntireFile("Content/LocPost/CSEC_JunebugEmail.txt"), this.name);
			string sender = this.name + " ReplyBot";
			string mail = MailServer.generateEmail(subject, body, sender);
			MailServer mailServer = (MailServer)os.netMap.mailServer.getDaemon(typeof(MailServer));
			mailServer.addMail(mail, os.defaultUser.name);
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00046AF4 File Offset: 0x00044CF4
		private void SendAssetAddedNotification(object osIn)
		{
			OS os = (OS)osIn;
			string subject = this.name + " " + LocaleTerms.Loc("Admins :: New asset added");
			string body = string.Format(Utils.readEntireFile("Content/LocPost/CSEC_ThemechangerEmail.txt"), this.name);
			string sender = this.name + " ReplyBot";
			Computer computer = Programs.getComputer(os, "mainHubAssets");
			string text = "link#%#" + computer.name + "#%#" + computer.ip;
			string mail = MailServer.generateEmail(subject, body, sender, new List<string>(new string[]
			{
				text
			}));
			MailServer mailServer = (MailServer)os.netMap.mailServer.getDaemon(typeof(MailServer));
			mailServer.addMail(mail, os.defaultUser.name);
		}
	}
}
