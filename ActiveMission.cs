﻿using System;
using System.Collections.Generic;
using System.Xml;
using Hacknet.Extensions;
using Hacknet.Mission;

namespace Hacknet
{
	// Token: 0x0200013C RID: 316
	internal class ActiveMission
	{
		// Token: 0x06000785 RID: 1925 RVA: 0x0007B818 File Offset: 0x00079A18
		public ActiveMission(List<MisisonGoal> _goals, string next, MailServer.EMailData _email)
		{
			this.goals = _goals;
			this.nextMission = next;
			this.email = _email;
			this.endFunctionValue = -1;
			this.endFunctionName = "";
			this.postingTitle = (this.postingBody = "");
			this.reloadGoalsSourceFile = "Missions/BitMissionIntro.xml";
		}

		// Token: 0x06000786 RID: 1926 RVA: 0x0007B8C4 File Offset: 0x00079AC4
		public void Update(float t)
		{
			if (this.activeCheck)
			{
				if (!this.hasFinished && this.isComplete(null))
				{
					this.finish();
					this.hasFinished = true;
				}
			}
		}

		// Token: 0x06000787 RID: 1927 RVA: 0x0007B90C File Offset: 0x00079B0C
		public string getSaveString()
		{
			string text = string.Concat(new object[]
			{
				"<mission next=\"",
				this.nextMission,
				"\" goals=\"",
				this.reloadGoalsSourceFile,
				"\" reqRank=\"",
				this.requiredRank,
				"\""
			});
			string text2;
			if (this.wasAutoGenerated)
			{
				text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					" genTarget=\"",
					this.genTarget,
					"\" genFile=\"",
					this.genFile,
					"\" genPath=\"",
					this.genPath,
					"\"  genTargetName=\"",
					this.genTargetName,
					"\" genOther=\"",
					this.genOther,
					"\""
				});
			}
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				" activeCheck=\"",
				this.activeCheck,
				"\">\n"
			});
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"<email sender=\"",
				Folder.Filter(this.email.sender),
				"\" subject=\"",
				Folder.Filter(this.email.subject),
				"\">",
				Folder.Filter(this.email.body),
				"</email>"
			});
			obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"<endFunc val=\"",
				this.endFunctionValue,
				"\" name=\"",
				this.endFunctionName,
				"\" />"
			});
			text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"<posting title=\"",
				Folder.Filter(this.postingTitle),
				"\">",
				Folder.Filter(this.postingBody),
				"</posting>"
			});
			return text + "</mission>";
		}

		// Token: 0x06000788 RID: 1928 RVA: 0x0007BB54 File Offset: 0x00079D54
		public static object load(XmlReader reader)
		{
			while (reader.Name != "mission")
			{
				reader.Read();
			}
			reader.MoveToAttribute("next");
			string text = reader.ReadContentAsString();
			reader.MoveToAttribute("goals");
			string text2 = reader.ReadContentAsString();
			if (reader.MoveToAttribute("genTarget"))
			{
				string comp = reader.ReadContentAsString();
				reader.MoveToAttribute("genFile");
				string file = reader.ReadContentAsString();
				reader.MoveToAttribute("genPath");
				string path = reader.ReadContentAsString();
				reader.MoveToAttribute("genTargetName");
				string text3 = reader.ReadContentAsString();
				reader.MoveToAttribute("genOther");
				string other = reader.ReadContentAsString();
				MissionGenerationParser.Comp = comp;
				MissionGenerationParser.File = file;
				MissionGenerationParser.Path = path;
				MissionGenerationParser.Target = text3;
				MissionGenerationParser.Other = other;
			}
			reader.MoveToAttribute("activeChack");
			int num = 0;
			if (reader.MoveToAttribute("reqRank"))
			{
				num = reader.ReadContentAsInt();
			}
			string text4 = reader.ReadContentAsString();
			bool flag = text4.ToLower().Equals("true");
			object result;
			if (text == "NULL_MISSION")
			{
				result = null;
			}
			else
			{
				if (!Settings.IsInExtensionMode && !text2.StartsWith("Content"))
				{
					text2 = "Content/" + text2;
				}
				List<MisisonGoal> list = new List<MisisonGoal>();
				ActiveMission activeMission = new ActiveMission(new List<MisisonGoal>(), "NONE", new MailServer.EMailData("Unknown", "Unknown", "Unknown", new List<string>()));
				try
				{
					activeMission = (ActiveMission)ComputerLoader.readMission(text2);
					list = activeMission.goals;
					flag = (flag || activeMission.activeCheck);
				}
				catch (Exception ex)
				{
					Utils.SendRealWorldEmail("Mission Load Error", "hacknetbugs+Hacknet@gmail.com", "Hacknet " + MainMenu.OSVersion + "\r\n" + Utils.GenerateReportFromException(ex));
				}
				string sendr = "ERRORBOT";
				string subj = "ERROR";
				string text5 = "ERROR :: MAIL LOAD FAILED";
				while (reader.Name != "email" && reader.Name != "endFunc")
				{
					reader.Read();
				}
				if (reader.Name.Equals("email"))
				{
					if (reader.MoveToAttribute("sender"))
					{
						sendr = Folder.deFilter(reader.ReadContentAsString());
					}
					if (reader.MoveToAttribute("subject"))
					{
						subj = Folder.deFilter(reader.ReadContentAsString());
					}
					reader.MoveToContent();
					text5 = reader.ReadElementContentAsString();
					text5 = Folder.deFilter(text5);
				}
				MailServer.EMailData emailData = new MailServer.EMailData(sendr, text5, subj, activeMission.email.attachments);
				ActiveMission activeMission2 = new ActiveMission(list, text, emailData);
				activeMission2.activeCheck = flag;
				activeMission2.reloadGoalsSourceFile = text2;
				activeMission2.requiredRank = num;
				while (reader.Name != "endFunc")
				{
					reader.Read();
				}
				reader.MoveToAttribute("val");
				int num2 = reader.ReadContentAsInt();
				reader.MoveToAttribute("name");
				string text6 = reader.ReadContentAsString();
				activeMission2.endFunctionName = text6;
				activeMission2.endFunctionValue = num2;
				while (reader.Name != "posting")
				{
					reader.Read();
				}
				reader.MoveToAttribute("title");
				string text7 = Folder.deFilter(reader.ReadContentAsString());
				reader.MoveToContent();
				string s = reader.ReadElementContentAsString();
				s = Folder.deFilter(s);
				activeMission2.postingTitle = text7;
				activeMission2.postingBody = s;
				result = activeMission2;
			}
			return result;
		}

		// Token: 0x06000789 RID: 1929 RVA: 0x0007BF20 File Offset: 0x0007A120
		public void addEndFunction(int val, string name)
		{
			this.endFunctionValue = val;
			this.endFunctionName = name;
		}

		// Token: 0x0600078A RID: 1930 RVA: 0x0007BF31 File Offset: 0x0007A131
		public void addStartFunction(int val, string name)
		{
			this.startFunctionValue = val;
			this.startFunctionName = name;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x0007BF44 File Offset: 0x0007A144
		public void ActivateSuppressedStartFunctionIfPresent()
		{
			if (this.startFunctionName != null)
			{
				MissionFunctions.runCommand(this.startFunctionValue, this.startFunctionName);
			}
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0007BF74 File Offset: 0x0007A174
		public bool isComplete(List<string> additionalDetails = null)
		{
			for (int i = 0; i < this.goals.Count; i++)
			{
				if (!this.goals[i].isComplete(additionalDetails))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x0007BFC0 File Offset: 0x0007A1C0
		public void finish()
		{
			OS.currentInstance.branchMissions.Clear();
			if (!this.nextMission.Equals("NONE"))
			{
				string str = "Content/Missions/";
				if (Settings.IsInExtensionMode)
				{
					str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
				}
				ComputerLoader.loadMission(str + this.nextMission, false);
				OS.currentInstance.currentMission.ActivateSuppressedStartFunctionIfPresent();
			}
			else
			{
				OS.currentInstance.currentMission = null;
			}
			if (this.endFunctionName != null)
			{
				MissionFunctions.runCommand(this.endFunctionValue, this.endFunctionName);
			}
			OS.currentInstance.saveGame();
			if (OS.currentInstance.multiplayer)
			{
				OS.currentInstance.endMultiplayerMatch(true);
			}
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x0007C094 File Offset: 0x0007A294
		public void sendEmail(OS os)
		{
			if (this.willSendEmail)
			{
				MailServer mailServer = (MailServer)os.netMap.mailServer.getDaemon(typeof(MailServer));
				mailServer.addMail(MailServer.generateEmail(this.email.subject, this.email.body, this.email.sender, this.email.attachments), os.defaultUser.name);
			}
		}

		// Token: 0x04000874 RID: 2164
		public List<MisisonGoal> goals;

		// Token: 0x04000875 RID: 2165
		private string[] delims = new string[]
		{
			"#%#"
		};

		// Token: 0x04000876 RID: 2166
		public string nextMission;

		// Token: 0x04000877 RID: 2167
		public bool activeCheck = false;

		// Token: 0x04000878 RID: 2168
		public MailServer.EMailData email;

		// Token: 0x04000879 RID: 2169
		private bool hasFinished = false;

		// Token: 0x0400087A RID: 2170
		public int endFunctionValue;

		// Token: 0x0400087B RID: 2171
		public string endFunctionName;

		// Token: 0x0400087C RID: 2172
		public int startFunctionValue;

		// Token: 0x0400087D RID: 2173
		public string startFunctionName;

		// Token: 0x0400087E RID: 2174
		public string postingTitle;

		// Token: 0x0400087F RID: 2175
		public string postingBody;

		// Token: 0x04000880 RID: 2176
		public string[] postingAcceptFlagRequirements = null;

		// Token: 0x04000881 RID: 2177
		public int requiredRank = 0;

		// Token: 0x04000882 RID: 2178
		public int difficulty = 0;

		// Token: 0x04000883 RID: 2179
		public bool ShouldIgnoreSenderVerification = false;

		// Token: 0x04000884 RID: 2180
		public string reloadGoalsSourceFile;

		// Token: 0x04000885 RID: 2181
		public bool wasAutoGenerated = false;

		// Token: 0x04000886 RID: 2182
		public string genTarget;

		// Token: 0x04000887 RID: 2183
		public string genPath;

		// Token: 0x04000888 RID: 2184
		public string genFile;

		// Token: 0x04000889 RID: 2185
		public string genTargetName;

		// Token: 0x0400088A RID: 2186
		public string genOther;

		// Token: 0x0400088B RID: 2187
		public bool willSendEmail = true;

		// Token: 0x0400088C RID: 2188
		public string client;

		// Token: 0x0400088D RID: 2189
		public string target;

		// Token: 0x0400088E RID: 2190
		public Dictionary<string, string> generationKeys;
	}
}