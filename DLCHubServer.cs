using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hacknet.Daemons.Helpers;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200000B RID: 11
	internal class DLCHubServer : Daemon, IMonitorableDaemon
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00006274 File Offset: 0x00004474
		public DLCHubServer(Computer c, string serviceName, string group, OS _os) : base(c, serviceName, _os)
		{
			this.groupName = group;
			this.MissionAvaliableIcon = this.os.content.Load<Texture2D>("DLC/Icons/focus_icon");
			this.MissionTakenIcon = this.os.content.Load<Texture2D>("DLC/Icons/cross_icon");
			this.MissionPlayersIcon = this.os.content.Load<Texture2D>("DLC/Icons/focus_icon");
			this.LoadingSpinner = this.os.content.Load<Texture2D>("Sprites/Spinner");
			this.ScrollableTextPanel = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
			this.HexBackground = new HexGridBackground(this.os.content);
			this.ButtonPressSound = this.os.content.Load<SoundEffect>("SFX/Bip");
			this.WooshBuildup = this.os.content.Load<SoundEffect>("DLC/SFX/Kilmer_Woosh");
			this.USIntro = this.os.content.Load<SoundEffect>("DLC/SFX/UserspaceIntro");
			this.InitDefaults();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00006435 File Offset: 0x00004635
		public override void initFiles()
		{
			this.initFilesystem();
		}

		// Token: 0x0600004A RID: 74 RVA: 0x0000643F File Offset: 0x0000463F
		private void InitDefaults()
		{
			this.Agents.Add("Channel");
			this.HighlightedWords.Add("Channel", new Color(240, 234, 81));
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00006478 File Offset: 0x00004678
		public void initFilesystem()
		{
			this.rootFolder = new Folder("HomeBase");
			this.missionFolder = new Folder("contracts");
			this.rootFolder.folders.Add(this.missionFolder);
			this.archivesFolder = new Folder("archive");
			this.rootFolder.folders.Add(this.archivesFolder);
			this.actionsFolder = new Folder("runtime");
			this.rootFolder.folders.Add(this.actionsFolder);
			this.DelayedActions = new DelayableActionSystem(this.actionsFolder, this.os);
			FileEntry item = new FileEntry(Computer.generateBinaryString(512), "runtime.dll");
			this.rootFolder.files.Add(item);
			this.ReGenerateConfigFile();
			this.IRCSystem = new IRCSystem(this.rootFolder);
			this.IRCSystem.AttachmentPressedSound = this.ButtonPressSound;
			this.InitTests();
			this.comp.files.root.folders.Add(this.rootFolder);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x0000659A File Offset: 0x0000479A
		private void InitTests()
		{
		}

		// Token: 0x0600004D RID: 77 RVA: 0x000065A0 File Offset: 0x000047A0
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.ReadActiveMissions();
			if (!this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated"))
			{
				this.State = DLCHubServer.DHSState.Welcome;
			}
			else
			{
				this.State = DLCHubServer.DHSState.Home;
			}
			this.ShouldShowMissionIncompleteMessage = false;
			if (this.comp.idName == "dhs")
			{
				if (this.ActiveMissions.Count == 0 && !this.os.Flags.HasFlag("dlc_complete"))
				{
					MissionFunctions.runCommand(1, "addRankSilent");
				}
				Computer computer = Programs.getComputer(this.os, "dhsDrop");
				if (computer != null)
				{
					Folder folder = computer.files.root.searchForFolder("bin");
					bool flag = false;
					for (int i = 0; i < folder.files.Count; i++)
					{
						if (folder.files[i].data == PortExploits.crackExeData[13])
						{
							flag = true;
						}
					}
					if (!flag)
					{
						folder.files.Add(new FileEntry(PortExploits.crackExeData[13], PortExploits.cracks[13]));
					}
				}
			}
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00006704 File Offset: 0x00004904
		public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			IRCSystem ircsystem = this.IRCSystem;
			ircsystem.LogAdded = (Action<string, string>)Delegate.Combine(ircsystem.LogAdded, act);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00006723 File Offset: 0x00004923
		public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			IRCSystem ircsystem = this.IRCSystem;
			ircsystem.LogAdded = (Action<string, string>)Delegate.Remove(ircsystem.LogAdded, act);
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00006744 File Offset: 0x00004944
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000675C File Offset: 0x0000495C
		private void ReGenerateConfigFile()
		{
			FileEntry fileEntry = this.rootFolder.searchForFile("dhs_config.sys");
			if (fileEntry != null)
			{
				this.rootFolder.files.Remove(fileEntry);
			}
			FileEntry item = this.generateConfigFile();
			this.rootFolder.files.Add(item);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000067B4 File Offset: 0x000049B4
		private FileEntry generateConfigFile()
		{
			string text = "##DHS_CONFIG\n";
			text = text + "ThemeColor = " + Utils.convertColorToParseableString(this.themeColor) + "\n";
			text = text + "ServiceName = " + this.name + "\n";
			text = text + "GroupName = " + this.groupName + "\n";
			text = text + "Agents = " + Utils.SerializeListToCSV(this.Agents) + "\n";
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"FactionReferral = ",
				this.AddsFactionPointForMissionCompleteion,
				"\n"
			});
			obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"AutoClearMissions = ",
				this.AutoClearMissionsOnSingleComplete,
				"\n"
			});
			text = text + "HighlightedWords = " + this.GetSerializedWordHighlightList() + "\n";
			obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"AllowContractAbbandon = ",
				this.AllowContractAbbandon,
				"\n"
			});
			text += "\n";
			return new FileEntry(text, "dhs_config.sys");
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000068F8 File Offset: 0x00004AF8
		private void loadFromConfigFileData(string config)
		{
			string[] array = config.Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].StartsWith("//"))
				{
					string text = array[i];
					if (text.StartsWith("ThemeColor"))
					{
						this.themeColor = Utils.convertStringToColor(this.getDataFromConfigLine(text, "= "));
					}
					else if (text.StartsWith("ServiceName"))
					{
						this.name = this.getDataFromConfigLine(text, "= ");
					}
					else if (text.StartsWith("GroupName"))
					{
						this.groupName = this.getDataFromConfigLine(text, "= ");
					}
					else if (text.StartsWith("Agents"))
					{
						this.Agents = new List<string>(this.getDataFromConfigLine(text, "= ").Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries));
					}
					else if (text.StartsWith("FactionReferral"))
					{
						this.AddsFactionPointForMissionCompleteion = (this.getDataFromConfigLine(text, "= ").ToLower() == "true");
					}
					else if (text.StartsWith("AutoClearMissions"))
					{
						this.AutoClearMissionsOnSingleComplete = (this.getDataFromConfigLine(text, "= ").ToLower() == "true");
					}
					else if (text.StartsWith("HighlightedWords"))
					{
						this.DeserializeWordHighlightList(this.getDataFromConfigLine(text, "= "));
					}
					else if (text.StartsWith("AllowContractAbbandon"))
					{
						this.AllowContractAbbandon = (this.getDataFromConfigLine(text, "= ").ToLower() == "true");
					}
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006AD8 File Offset: 0x00004CD8
		private string getDataFromConfigLine(string line, string sentinel = "= ")
		{
			return line.Substring(line.IndexOf(sentinel) + 2);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00006AFC File Offset: 0x00004CFC
		private string GetSerializedWordHighlightList()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, Color> keyValuePair in this.HighlightedWords)
			{
				stringBuilder.Append(keyValuePair.Key + "=" + Utils.convertColorToParseableString(keyValuePair.Value));
				stringBuilder.Append("/");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00006B94 File Offset: 0x00004D94
		private void DeserializeWordHighlightList(string input)
		{
			this.HighlightedWords = new Dictionary<string, Color>();
			string[] array = input.Split(new char[]
			{
				'/'
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries);
				this.HighlightedWords.Add(array2[0], Utils.convertStringToColor(array2[1]));
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00006C08 File Offset: 0x00004E08
		public override void loadInit()
		{
			base.loadInit();
			this.rootFolder = this.comp.files.root.searchForFolder("HomeBase");
			this.missionFolder = this.rootFolder.searchForFolder("contracts");
			this.archivesFolder = this.rootFolder.searchForFolder("archive");
			this.actionsFolder = this.rootFolder.searchForFolder("runtime");
			this.DelayedActions = new DelayableActionSystem(this.actionsFolder, this.os);
			FileEntry fileEntry = this.rootFolder.searchForFile("dhs_config.sys");
			if (fileEntry != null)
			{
				this.loadFromConfigFileData(fileEntry.data);
			}
			this.IRCSystem = new IRCSystem(this.rootFolder);
			this.IRCSystem.AttachmentPressedSound = this.ButtonPressSound;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006CE0 File Offset: 0x00004EE0
		public override string getSaveString()
		{
			return "<DHSDaemon />";
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00006CF8 File Offset: 0x00004EF8
		public void AddAgent(string AgentName, string agentPassword, Color color)
		{
			AgentName = AgentName.Replace(" ", "_").Replace("/", "").Replace("=", "_");
			if (AgentName.Contains(' ') || AgentName.Contains('/') || AgentName.Contains('=') || string.IsNullOrWhiteSpace(AgentName))
			{
				throw new InvalidOperationException("Invalid Agent Name \"" + AgentName + "\"!");
			}
			if (this.Agents.Contains(AgentName))
			{
				this.HighlightedWords[AgentName] = color;
			}
			else
			{
				this.Agents.Add(AgentName);
				this.HighlightedWords.Add(AgentName, color);
			}
			if (this.rootFolder != null)
			{
				this.ReGenerateConfigFile();
			}
			bool flag = false;
			for (int i = 0; i < this.comp.users.Count; i++)
			{
				flag |= (this.comp.users[i].name == AgentName);
			}
			if (!flag)
			{
				this.comp.users.Add(new UserDetail(AgentName, agentPassword, 3));
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006E38 File Offset: 0x00005038
		public void AddMission(string missionPath, string AgentClaimName = null, bool startsComplete = false)
		{
			List<ActiveMission> branchMissions = this.os.branchMissions;
			ActiveMission mission = (ActiveMission)ComputerLoader.readMission(missionPath);
			this.os.branchMissions = branchMissions;
			this.AddMission(mission, AgentClaimName, startsComplete);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00006E74 File Offset: 0x00005074
		public void AddMission(ActiveMission mission, string AgentClaimName = null, bool startsComplete = false)
		{
			this.ActiveMissions.Add(new DLCHubServer.ClaimableMission
			{
				Mission = mission,
				IsComplete = startsComplete,
				AgentClaim = AgentClaimName
			});
			this.ReSerializeActiveMissions();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006EB4 File Offset: 0x000050B4
		public void RemoveMission(string missionPath)
		{
			for (int i = 0; i < this.ActiveMissions.Count; i++)
			{
				if (this.ActiveMissions[i].Mission.reloadGoalsSourceFile == LocalizedFileLoader.GetLocalizedFilepath(missionPath))
				{
					this.ActiveMissions.RemoveAt(i);
					i--;
				}
			}
			this.ReSerializeActiveMissions();
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00006F20 File Offset: 0x00005120
		private void ReadActiveMissions()
		{
			this.ActiveMissions.Clear();
			for (int i = 0; i < this.missionFolder.files.Count; i++)
			{
				if (this.missionFolder.files[i].name.EndsWith(".ctc"))
				{
					int num = 0;
					string agentClaim = null;
					try
					{
						ActiveMission mission = (ActiveMission)MissionSerializer.restoreMissionFromFile(this.missionFolder.files[i].data, out num, out agentClaim);
						DLCHubServer.ClaimableMission claimableMission = new DLCHubServer.ClaimableMission
						{
							AgentClaim = agentClaim,
							Mission = mission,
							IsComplete = (num >= 1)
						};
						this.ActiveMissions.Add(claimableMission);
						if (this.SelectedMission == null && claimableMission.AgentClaim == this.os.defaultUser.name)
						{
							this.SelectedMission = claimableMission;
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00007044 File Offset: 0x00005244
		private void ReSerializeActiveMissions()
		{
			this.missionFolder.files.Clear();
			for (int i = 0; i < this.ActiveMissions.Count; i++)
			{
				string dataEntry = MissionSerializer.generateMissionFile(this.ActiveMissions[i].Mission, this.ActiveMissions[i].IsComplete ? 1 : 0, this.groupName, this.ActiveMissions[i].AgentClaim);
				this.missionFolder.files.Add(new FileEntry(dataEntry, this.GetFilenameForMission(this.ActiveMissions[i].Mission)));
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000070F8 File Offset: 0x000052F8
		private string GetFilenameForMission(ActiveMission mission)
		{
			if (mission.client == null || mission.client == "UNKNOWN")
			{
				mission.client = string.Concat(Utils.getRandomByte());
			}
			return Utils.GetNonRepeatingFilename("Contact_" + mission.client, ".ctc", this.missionFolder);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00007164 File Offset: 0x00005364
		private bool PlayerHasClaimedMission()
		{
			for (int i = 0; i < this.ActiveMissions.Count; i++)
			{
				if (this.ActiveMissions[i].AgentClaim == this.os.defaultUser.name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000071C4 File Offset: 0x000053C4
		private void PlayerAcceptMission(DLCHubServer.ClaimableMission mission)
		{
			mission.AgentClaim = this.os.defaultUser.name;
			this.os.currentMission = mission.Mission;
			mission.Mission.ActivateSuppressedStartFunctionIfPresent();
			ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(mission.Mission.reloadGoalsSourceFile);
			this.IRCSystem.AddLog("Channel", string.Format(LocaleTerms.Loc("CONTRACT CLAIMED: @{0} claimed contract \"{1}\""), this.os.defaultUser.name, mission.Mission.postingTitle), null);
			this.ReSerializeActiveMissions();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000725E File Offset: 0x0000545E
		private void PlayerAbandonedMission(DLCHubServer.ClaimableMission mission)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00007264 File Offset: 0x00005464
		private bool PlayerAttemptCompleteMission(DLCHubServer.ClaimableMission mission, bool ForceComplete = false)
		{
			ForceComplete = (ForceComplete && Settings.forceCompleteEnabled);
			List<DLCHubServer.ClaimableMission> list = new List<DLCHubServer.ClaimableMission>();
			for (int i = 0; i < this.ActiveMissions.Count; i++)
			{
				list.Add(this.ActiveMissions[i]);
			}
			this.ActiveMissions.Clear();
			this.missionFolder.files.Clear();
			bool result;
			if (ForceComplete || this.os.currentMission.isComplete(this.MissionTextResponses))
			{
				ActiveMission currentMission = this.os.currentMission;
				this.os.currentMission = null;
				if (currentMission.endFunctionName != null)
				{
					MissionFunctions.runCommand(currentMission.endFunctionValue, currentMission.endFunctionName);
				}
				if (this.AddsFactionPointForMissionCompleteion)
				{
					MissionFunctions.runCommand(1, "addRankSilent");
				}
				this.IRCSystem.AddLog("Channel", string.Format(LocaleTerms.Loc("CONTRACT COMPLETE: @{0} completed contract \"{1}\""), this.os.defaultUser.name, mission.Mission.postingTitle), null);
				if (this.MissionTextResponses.Count > 0)
				{
					this.IRCSystem.AddLog("Channel", LocaleTerms.Loc("Additional details provided:"), null);
					for (int i = 0; i < this.MissionTextResponses.Count; i++)
					{
						this.IRCSystem.AddLog("Channel", "[" + this.MissionTextResponses[i] + "]", null);
					}
				}
				this.MissionTextResponses.Clear();
				mission.IsComplete = true;
				if (this.AutoClearMissionsOnSingleComplete)
				{
					this.CompleteAndArchiveMissionSet(list);
				}
				this.os.saveGame();
				result = true;
			}
			else
			{
				this.ActiveMissions = list;
				this.ReSerializeActiveMissions();
				this.MissionTextResponses.Clear();
				result = false;
			}
			return result;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000746C File Offset: 0x0000566C
		private void CompleteAndArchiveMissionSet(List<DLCHubServer.ClaimableMission> missionsToArchive)
		{
			for (int i = 0; i < missionsToArchive.Count; i++)
			{
				DLCHubServer.ClaimableMission claimableMission = missionsToArchive[i];
				claimableMission.IsComplete = true;
				string dataEntry = MissionSerializer.generateMissionFile(claimableMission.Mission, claimableMission.IsComplete ? 1 : 0, this.groupName, claimableMission.AgentClaim);
				this.archivesFolder.files.Add(new FileEntry(dataEntry, this.GetFilenameForMission(claimableMission.Mission)));
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000074E8 File Offset: 0x000056E8
		public void ClearAllActiveMissions()
		{
			this.CompleteAndArchiveMissionSet(this.ActiveMissions);
			this.ActiveMissions.Clear();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00007544 File Offset: 0x00005744
		private void Update()
		{
			this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), delegate
			{
				if (this.os.display.command != this.name)
				{
					this.IRCSystem.LeftView();
				}
			});
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00007598 File Offset: 0x00005798
		public bool ShouldDisplayNotifications()
		{
			return this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated") || this.WelcomeFadeoutTimerLeft <= 0f;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x000075E0 File Offset: 0x000057E0
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.UIButtonOffset = 0;
			this.Update();
			Rectangle rectangle = Utils.InsetRectangle(bounds, 2);
			Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y + 10, rectangle.Width, rectangle.Height - 10);
			this.HexBackground.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			sb.Draw(Utils.white, rectangle2, Color.Black * 0.2f);
			this.HexBackground.Draw(rectangle2, sb, Color.Black * 0.1f, this.themeColor * 0.02f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
			int num = 30;
			Rectangle bounds2 = new Rectangle(rectangle.X, rectangle.Y + 2 + num, rectangle.Width, rectangle.Height - num - 2);
			Rectangle bounds3 = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, num);
			this.DrawOptionsPanel(bounds3, sb);
			switch (this.State)
			{
			default:
				if (!this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated"))
				{
					this.DoLoadingPlayerInScreen(bounds, sb);
				}
				else
				{
					this.State = DLCHubServer.DHSState.Home;
				}
				break;
			case DLCHubServer.DHSState.Login:
				break;
			case DLCHubServer.DHSState.Home:
				this.DrawHomeScreen(bounds2, sb);
				if (!this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated"))
				{
					this.DoLoadingPlayerInScreen(bounds, sb);
					this.WelcomeFadeoutTimerLeft -= (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				}
				break;
			case DLCHubServer.DHSState.ArchiveList:
				break;
			case DLCHubServer.DHSState.MissionSelectView:
				this.DrawMissionSelectView(bounds2, sb);
				break;
			case DLCHubServer.DHSState.ContractDetailView:
				this.DrawMissionDetailsPanel(bounds2, sb, this.SelectedMission);
				break;
			}
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000077D8 File Offset: 0x000059D8
		private void DoLoadingPlayerInScreen(Rectangle bounds, SpriteBatch sb)
		{
			this.timeSpentInLoading += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			float num = 10f;
			float num2 = 7f;
			float scale = this.WelcomeFadeoutTimerLeft / this.BaseWelcomeFadeoutTime;
			Rectangle rectangle = Utils.InsetRectangle(bounds, 2);
			Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y + 30, rectangle.Width, rectangle.Height - 30);
			sb.Draw(Utils.white, rectangle2, Color.Black * scale);
			this.HexBackground.Draw(rectangle2, sb, Color.Black * 0.1f * scale, this.themeColor * 0.02f * scale, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
			if (!this.HasStartedWoosh && this.timeSpentInLoading >= 5f)
			{
				this.WooshBuildup.Play(0.7f, 0f, 0f);
				MusicManager.loadAsCurrentSong("DLC\\Music\\Userspacelike");
				this.HasStartedWoosh = true;
			}
			if (this.timeSpentInLoading >= num)
			{
				if (this.WelcomeFadeoutTimerLeft <= 0f)
				{
					this.os.Flags.AddFlag("DLC_Player_IRC_Authenticated");
				}
				this.State = DLCHubServer.DHSState.Home;
			}
			if (MusicManager.isPlaying && this.timeSpentInLoading < num2)
			{
				if (MusicManager.currentSongName == "DLC\\Music\\Userspacelike")
				{
					MusicManager.stop();
				}
			}
			Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, this.LoadingSpinner.Width, this.LoadingSpinner.Height);
			bool flag = this.timeSpentInLoading > num2;
			float amount = flag ? Utils.QuadraticOutCurve(1f - Math.Min(this.timeSpentInLoading - num2, 1f)) : 0f;
			sb.Draw(flag ? this.MissionAvaliableIcon : this.LoadingSpinner, destinationRectangle, null, Color.Lerp(Utils.makeColorAddative(this.themeColor) * scale, Utils.AddativeWhite, amount), flag ? 0f : (this.os.timer * 4f), flag ? this.MissionAvaliableIcon.GetCentreOrigin() : this.LoadingSpinner.GetCentreOrigin(), SpriteEffects.None, 0.4f);
			Rectangle dest = new Rectangle(bounds.X + 10, destinationRectangle.Y + this.LoadingSpinner.Height / 2 + 4, bounds.Width - 20, 26);
			if (this.timeSpentInLoading > 1f)
			{
				TextItem.doCenteredFontLabel(dest, "Accessing Whitelist...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * scale, false);
				dest.Y += dest.Height;
			}
			if (this.timeSpentInLoading > 3f)
			{
				TextItem.doCenteredFontLabel(dest, "Authenticating...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * scale, false);
				dest.Y += dest.Height;
			}
			if (this.timeSpentInLoading > 5f)
			{
				TextItem.doCenteredFontLabel(dest, "Registering Connection...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * scale, false);
				dest.Y += dest.Height;
			}
			if (this.timeSpentInLoading > 5.5f)
			{
				TextItem.doCenteredFontLabel(dest, "Unlocking...", GuiData.smallfont, Utils.AddativeWhite * 0.5f * scale, false);
				dest.Y += dest.Height;
			}
			if (this.timeSpentInLoading > num2)
			{
				if (!MusicManager.isPlaying)
				{
					MusicManager.playSongImmediatley("DLC\\Music\\Userspacelike");
					this.USIntro.Play(0.4f, 0f, 0f);
				}
				float num3 = Utils.QuadraticOutCurve(Math.Min(1f, (this.timeSpentInLoading - num2) / 2f));
				string text = " :: " + string.Format(LocaleTerms.Loc("Welcome {0}"), this.os.SaveUserAccountName) + " :: ";
				Vector2 vector = GuiData.smallfont.MeasureString(text);
				Rectangle dest2 = new Rectangle(dest.X, dest.Y, dest.Width / 2 - (int)(vector.X / 2f), dest.Height);
				int width = dest2.Width;
				dest2.Width = (int)((float)dest2.Width * num3);
				dest2.X += width - dest2.Width;
				Rectangle dest3 = new Rectangle(dest.X + width + (int)vector.X, dest.Y, dest2.Width, dest.Height);
				PatternDrawer.draw(dest2, 1f, Color.Transparent, this.themeColor * scale, sb);
				PatternDrawer.draw(dest3, 1f, Color.Transparent, this.themeColor * scale, sb);
				TextItem.doCenteredFontLabel(dest, text, GuiData.smallfont, this.themeColor * scale, false);
				dest.Y += dest.Height;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00007DB0 File Offset: 0x00005FB0
		private void DrawOptionsPanel(Rectangle bounds, SpriteBatch sb)
		{
			int num = Math.Min(200, bounds.Width / 3);
			int num2 = bounds.Height - 8;
			bool flag = this.PlayerHasClaimedMission();
			int num3 = bounds.X + 2;
			int y = bounds.Y + bounds.Height / 2 - num2 / 2;
			bool flag2 = this.os.Flags.HasFlag("DLC_Player_IRC_Authenticated") || this.WelcomeFadeoutTimerLeft <= 0f;
			switch (this.State)
			{
			case DLCHubServer.DHSState.Welcome:
			case DLCHubServer.DHSState.Login:
			case DLCHubServer.DHSState.Home:
				if (Button.doButton(393001, num3, y, num, num2, LocaleTerms.Loc("Live Projects"), new Color?(flag ? Color.Gray : this.themeColor)) && flag2)
				{
					this.State = DLCHubServer.DHSState.MissionSelectView;
				}
				num3 += num + 8;
				if (flag && Button.doButton(393003, num3, y, num, num2, LocaleTerms.Loc("View Active Project"), new Color?(this.themeColor)) && flag2)
				{
					this.State = DLCHubServer.DHSState.ContractDetailView;
				}
				num3 += num + 8;
				break;
			case DLCHubServer.DHSState.ArchiveList:
			case DLCHubServer.DHSState.MissionSelectView:
			case DLCHubServer.DHSState.ContractDetailView:
				if (Button.doButton(393001, num3, y, num, num2, LocaleTerms.Loc("Chat"), new Color?(flag ? Color.Gray : this.themeColor)) && flag2)
				{
					this.State = DLCHubServer.DHSState.Home;
				}
				num3 += num + 8;
				break;
			}
			int num4 = 100;
			if (Button.doButton(393909, bounds.X + bounds.Width - num4, y, num4, num2, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor)) && flag2)
			{
				this.os.display.command = "connect";
			}
			Rectangle destinationRectangle = bounds;
			destinationRectangle.Y += destinationRectangle.Height;
			destinationRectangle.Height = 1;
			sb.Draw(Utils.white, destinationRectangle, this.themeColor);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00007FE4 File Offset: 0x000061E4
		private void DrawHomeScreen(Rectangle bounds, SpriteBatch sb)
		{
			sb.Draw(Utils.gradient, bounds, Color.Black * 0.8f);
			bounds = Utils.InsetRectangle(bounds, 2);
			bounds.Height -= 6;
			this.IRCSystem.Draw(bounds, sb, false, LocaleTerms.Loc("Unknown"), this.HighlightedWords);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00008048 File Offset: 0x00006248
		private void DrawMissionSelectView(Rectangle bounds, SpriteBatch sb)
		{
			int num = 2;
			int num2 = (this.ActiveMissions.Count > 0) ? (bounds.Height / this.ActiveMissions.Count - num) : 0;
			if (this.ActiveMissions.Count > 0)
			{
				Rectangle bounds2 = new Rectangle(bounds.X, bounds.Y, bounds.Width, num2);
				for (int i = 0; i < this.ActiveMissions.Count; i++)
				{
					this.ScrollableTextPanel.SetScrollbarUIIndexOffset(i + 100);
					this.DrawMissionPanel(bounds2, sb, this.ActiveMissions[i]);
					bounds2.Y += num + num2;
				}
			}
			else
			{
				int num3 = 60;
				Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + bounds.Height / 2 - num3 / 2, bounds.Width, num3);
				sb.Draw(Utils.white, rectangle, Color.Black * 0.9f);
				rectangle = Utils.InsetRectangle(rectangle, 12);
				TextItem.doFontLabelToSize(rectangle, LocaleTerms.Loc("No Projects Available"), GuiData.font, Color.White, true, false);
			}
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00008188 File Offset: 0x00006388
		private void DrawMissionPanel(Rectangle bounds, SpriteBatch sb, DLCHubServer.ClaimableMission mission)
		{
			bool flag = false;
			bool flag2 = this.PlayerHasClaimedMission();
			Texture2D texture = this.MissionAvaliableIcon;
			Color color = Color.White;
			Color value = Color.White;
			if (mission.AgentClaim != null)
			{
				if (mission.AgentClaim == this.os.defaultUser.name)
				{
					texture = this.MissionPlayersIcon;
					flag = true;
				}
				else
				{
					texture = this.MissionTakenIcon;
					color = Color.Gray * 0.8f;
					value = Color.Black;
				}
			}
			else
			{
				color = Color.Lerp(Utils.AddativeWhite, Utils.makeColorAddative(this.themeColor), 0.5f);
				value = this.themeColor;
			}
			bool flag3 = flag || (!flag2 && mission.AgentClaim == null);
			RenderedRectangle.doRectangleOutline(bounds.X, bounds.Y, bounds.Width, bounds.Height, 1, new Color?(flag3 ? this.themeColor : Color.Gray));
			Rectangle destinationRectangle = Utils.InsetRectangle(bounds, 1);
			sb.Draw(Utils.gradient, destinationRectangle, value * 0.2f);
			int num = 4;
			int num2 = Math.Min(bounds.Width / 6, bounds.Height - 2 * num);
			Rectangle rectangle = new Rectangle(bounds.X + num, bounds.Y + num, num2, num2);
			rectangle = Utils.InsetRectangle(rectangle, 2);
			Rectangle rectangle2 = new Rectangle(rectangle.X + rectangle.Width + num, bounds.Y + num, bounds.Width - rectangle.Width - num * 6, 40);
			Rectangle destinationRectangle2 = new Rectangle(rectangle2.X, rectangle2.Y, rectangle2.Width + num * 3, rectangle2.Height);
			sb.Draw(Utils.white, destinationRectangle2, Utils.VeryDarkGray * 0.7f);
			TextItem.doFontLabel(new Vector2((float)(rectangle2.X + 2 * num), (float)rectangle2.Y), mission.Mission.postingTitle, GuiData.font, new Color?(flag3 ? this.themeColor : Color.Gray), (float)rectangle2.Width, (float)rectangle2.Height, true);
			int num3 = bounds.Width / 5 + num;
			if (num3 < 150)
			{
				num3 = 150;
			}
			Rectangle dest = new Rectangle(rectangle2.X + 10, rectangle2.Y + rectangle2.Height + num, bounds.Width / 5 * 4 - (10 + num3 - num), bounds.Height - (rectangle2.Height + num * 3));
			if (flag || mission.AgentClaim == null)
			{
				if (!flag && flag2)
				{
					dest.Width = bounds.Width - (num * 3 + num2);
				}
				string text = Utils.SuperSmartTwimForWidth(mission.Mission.postingBody, dest.Width - num, GuiData.tinyfont);
				ScrollBar.AlwaysDrawUnderBar = true;
				this.ScrollableTextPanel.UpdateScroll(mission.UITextScrollDown);
				this.ScrollableTextPanel.Draw(dest, text, sb, (flag || mission.AgentClaim == null) ? Color.White : (Color.LightGray * 0.6f));
				mission.UITextScrollDown = this.ScrollableTextPanel.GetScrollDown();
				ScrollBar.AlwaysDrawUnderBar = false;
			}
			else
			{
				dest.X = bounds.X;
				dest.Width = bounds.Width;
				Rectangle rectangle3 = new Rectangle(dest.X, dest.Y + ((bounds.Height > 200) ? 20 : 0), dest.Width, 30);
				string text2 = "- " + LocaleTerms.Loc("CLAIMED") + " : ";
				string text3 = text2 + mission.AgentClaim + " - ";
				Vector2 vector = GuiData.font.MeasureString(text3);
				Vector2 vector2 = GuiData.font.MeasureString(text2);
				Vector2 vector3 = GuiData.font.MeasureString(mission.AgentClaim);
				float num4 = Math.Min((float)rectangle3.Width / vector.X, (float)rectangle3.Height / vector.Y);
				float num5 = (float)(rectangle3.X + rectangle3.Width / 2) - vector.X * num4 / 2f;
				float y = (float)(rectangle3.Y + rectangle3.Height / 2) - vector.Y * num4 / 2f;
				sb.DrawString(GuiData.font, text2, new Vector2(num5, y), Color.LightGray, 0f, Vector2.Zero, new Vector2(num4), SpriteEffects.None, 0.4f);
				num5 += vector2.X * num4;
				sb.DrawString(GuiData.font, mission.AgentClaim, new Vector2(num5, y), Color.Lerp(this.themeColor, Color.Gray, 0.55f), 0f, Vector2.Zero, new Vector2(num4), SpriteEffects.None, 0.4f);
				num5 += vector3.X * num4;
				sb.DrawString(GuiData.font, " -", new Vector2(num5, y), Color.LightGray, 0f, Vector2.Zero, new Vector2(num4), SpriteEffects.None, 0.4f);
				rectangle3.Y += rectangle3.Height;
				Rectangle destinationRectangle3 = new Rectangle(bounds.X + num + num2, rectangle3.Y + 6, bounds.Width - num * 2 - num2, (int)((double)dest.Height * 0.6));
				sb.Draw(Utils.white, destinationRectangle3, Color.Black * 0.65f);
				Rectangle dest2 = new Rectangle(destinationRectangle3.X + num + 10, destinationRectangle3.Y, destinationRectangle3.Width - num2 - 6, destinationRectangle3.Height);
				string text4 = Utils.SuperSmartTwimForWidth(mission.Mission.postingBody, dest2.Width - num, GuiData.tinyfont);
				bool offsetToTopLeft = false;
				if (text4.StartsWith("-- "))
				{
					text4 = "\n                " + text4;
					offsetToTopLeft = true;
				}
				TextItem.doFontLabelToSize(dest2, text4, GuiData.tinyfont, Color.DarkGray * 0.5f, true, offsetToTopLeft);
				rectangle3.Y += rectangle3.Height + 5;
				rectangle3.Height = 25;
			}
			sb.Draw(texture, rectangle, color);
			int num6 = Math.Min((bounds.Height - rectangle2.Height - num * 6) / 3, 22);
			Vector2 vector4 = Utils.ClipVec2ForTextRendering(new Vector2((float)(dest.X + dest.Width + num), (float)dest.Y));
			if (mission.AgentClaim == null && !flag2)
			{
				if (Button.doButton(391001 + this.UIButtonOffset, (int)vector4.X, (int)vector4.Y, num3, num6, LocaleTerms.Loc("Claim Contract"), new Color?(this.themeColor)))
				{
					this.PlayerAcceptMission(mission);
					this.SelectedMission = mission;
					this.State = DLCHubServer.DHSState.ContractDetailView;
				}
				vector4.Y += (float)(num6 + num);
				this.UIButtonOffset++;
			}
			if (flag)
			{
				if (Button.doButton(391002 + this.UIButtonOffset, (int)vector4.X, (int)vector4.Y, num3, num6, (num3 > 140) ? LocaleTerms.Loc("View Details") : LocaleTerms.Loc("Details"), new Color?(Color.White)))
				{
					this.State = DLCHubServer.DHSState.ContractDetailView;
					this.ScrollableTextPanel.UpdateScroll(0f);
					this.SelectedMission = mission;
					this.AbandonMissionShowConfirmation = false;
					this.ShouldShowMissionIncompleteMessage = false;
				}
				vector4.Y += (float)(num6 + num);
				this.UIButtonOffset++;
				if (Button.doButton(391003 + this.UIButtonOffset, (int)vector4.X, (int)vector4.Y, num3, num6, LocaleTerms.Loc("Complete"), new Color?(this.themeColor)))
				{
					if (!this.PlayerAttemptCompleteMission(mission, false))
					{
						this.State = DLCHubServer.DHSState.ContractDetailView;
						this.ShouldShowMissionIncompleteMessage = true;
					}
				}
				vector4.Y += (float)(num6 + num);
				this.UIButtonOffset++;
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00008A50 File Offset: 0x00006C50
		private void DrawMissionDetailsPanel(Rectangle bounds, SpriteBatch sb, DLCHubServer.ClaimableMission mission)
		{
			bool flag = false;
			bool flag2 = this.PlayerHasClaimedMission();
			Texture2D texture = this.MissionAvaliableIcon;
			if (mission.AgentClaim != null)
			{
				if (mission.AgentClaim == this.os.defaultUser.name)
				{
					texture = this.MissionPlayersIcon;
					flag = true;
				}
				else
				{
					texture = this.MissionTakenIcon;
				}
			}
			bool flag3 = flag || (!flag2 && mission.AgentClaim == null);
			int num = 4;
			int num2 = Math.Min(bounds.Width / 6, bounds.Height - 2 * num);
			Rectangle destinationRectangle = new Rectangle(bounds.X + num, bounds.Y + num, num2, num2);
			Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.X + destinationRectangle.Width + num, bounds.Y + num, bounds.Width - destinationRectangle.Width - num * 3, 40);
			sb.Draw(Utils.white, destinationRectangle2, Utils.VeryDarkGray * 0.8f);
			TextItem.doFontLabel(new Vector2((float)(destinationRectangle2.X + 2 * num), (float)destinationRectangle2.Y), mission.Mission.email.subject, GuiData.font, new Color?(flag3 ? this.themeColor : Color.Gray), (float)destinationRectangle2.Width, (float)destinationRectangle2.Height, true);
			sb.Draw(texture, destinationRectangle, Color.White);
			int num3 = 30;
			Rectangle rectangle = new Rectangle(destinationRectangle.X + destinationRectangle.Width + num, destinationRectangle2.Y + destinationRectangle2.Height + num * 2, bounds.Width - (destinationRectangle.X - bounds.X + destinationRectangle.Width + num * 2), num3);
			this.DrawHeaderLine(rectangle, LocaleTerms.Loc("Briefing"), sb);
			Rectangle dest = rectangle;
			dest.Y += rectangle.Height + num;
			string text = Utils.SuperSmartTwimForWidth(mission.Mission.email.body, dest.Width, GuiData.tinyfont);
			Vector2 vector = GuiData.tinyfont.MeasureString(text);
			int num4 = this.MissionTextResponses.Count * 24 + num3 * 4 + num * 4 + mission.Mission.email.attachments.Count * 30 + 10;
			int val = bounds.Height - (num4 + (dest.Y - bounds.Y)) - num * 2;
			int val2 = 70;
			dest.Height = Math.Max(val2, Math.Min(val, Math.Min((int)vector.Y + 2 * num, bounds.Height / 3 * 2)));
			this.ScrollableTextPanel.Draw(dest, text, sb);
			rectangle.Y = dest.Y + dest.Height + num;
			this.DrawHeaderLine(rectangle, LocaleTerms.Loc("Attachments"), sb);
			Vector2 dpos = new Vector2((float)rectangle.X, (float)(rectangle.Y + rectangle.Height + num));
			if (mission.Mission.email.attachments.Count == 0)
			{
				Rectangle dest2 = rectangle;
				dest2.Y += dest2.Height;
				TextItem.doFontLabelToSize(dest2, "- " + LocaleTerms.Loc("Empty") + " -", GuiData.smallfont, Color.LightGray, true, false);
				dpos.Y += 26f;
			}
			else
			{
				for (int i = 0; i < mission.Mission.email.attachments.Count; i++)
				{
					if (AttachmentRenderer.RenderAttachment(mission.Mission.email.attachments[i], this.os, dpos, this.UIButtonOffset++, this.ButtonPressSound))
					{
						dpos.Y += 22f;
					}
				}
			}
			dpos.Y += (float)num;
			rectangle.Y = (int)dpos.Y;
			if (!mission.IsComplete)
			{
				this.DrawHeaderLine(rectangle, LocaleTerms.Loc("Tools"), sb);
				if (this.ShouldShowMissionIncompleteMessage)
				{
					Rectangle dest3 = rectangle;
					dest3.Height -= 4;
					PatternDrawer.draw(dest3, this.os.timer * 0.0008f, this.os.lockedColor * 0.3f, this.os.brightLockedColor * 0.7f, sb, PatternDrawer.thinStripe);
					TextItem.doRightAlignedBackingLabelScaled(dest3, LocaleTerms.Loc("Mission Incomplete"), GuiData.font, Color.Transparent, Color.White);
				}
				Rectangle rectangle2 = rectangle;
				rectangle2.Y += rectangle.Height + num;
				int num5 = (rectangle2.Width - num * 2) / 2;
				if (Button.doButton(639013 + this.UIButtonOffset++, rectangle2.X, rectangle2.Y, num5, rectangle2.Height, LocaleTerms.Loc("Complete"), new Color?(this.themeColor)))
				{
					if (!this.PlayerAttemptCompleteMission(mission, false))
					{
						this.ShouldShowMissionIncompleteMessage = true;
					}
					else
					{
						this.ShouldShowMissionIncompleteMessage = false;
						this.State = DLCHubServer.DHSState.Home;
					}
				}
				if (Button.doButton(639414 + this.UIButtonOffset++, rectangle2.X + num5 + num, rectangle2.Y, num5, rectangle2.Height, this.AbandonMissionShowConfirmation ? LocaleTerms.Loc("CONFIRM?") : LocaleTerms.Loc("Abandon"), new Color?(this.AllowContractAbbandon ? (this.AbandonMissionShowConfirmation ? this.os.brightLockedColor : this.os.lockedColor) : Color.Gray)) && this.AllowContractAbbandon)
				{
					if (!this.AbandonMissionShowConfirmation)
					{
						this.AbandonMissionShowConfirmation = true;
					}
					else
					{
						this.PlayerAbandonedMission(mission);
					}
				}
				if (Settings.forceCompleteEnabled && Button.doButton(649017 + this.UIButtonOffset++, rectangle2.X + num5 + num, rectangle2.Y + rectangle2.Height + 2, num5, rectangle2.Height, LocaleTerms.Loc("Force Complete"), new Color?(this.themeColor)))
				{
					this.PlayerAttemptCompleteMission(mission, true);
					this.State = DLCHubServer.DHSState.Home;
				}
				Rectangle bounds2 = rectangle2;
				bounds2.Y += rectangle2.Height + num;
				bounds2.Height = bounds.Y + bounds.Height - bounds2.Y - num;
				this.DrawAdditionalDetailsSection(bounds2, sb);
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000918C File Offset: 0x0000738C
		private void DrawAdditionalDetailsSection(Rectangle bounds, SpriteBatch sb)
		{
			int num = 24;
			Vector2 vector = new Vector2((float)bounds.X, (float)bounds.Y);
			TextItem.doFontLabel(vector, LocaleTerms.Loc("Additional Details") + " :", GuiData.smallfont, null, Math.Max(200f, (float)bounds.Width - (vector.X - (float)bounds.Width) * 2f), float.MaxValue, false);
			vector.Y += (float)num;
			for (int i = 0; i < this.MissionTextResponses.Count; i++)
			{
				TextItem.doFontLabel(vector + new Vector2(25f, 0f), this.MissionTextResponses[i], GuiData.tinyfont, null, (float)bounds.Width - (vector.X - (float)bounds.X) * 2f - 20f, float.MaxValue, false);
				float num2 = Math.Min(GuiData.tinyfont.MeasureString(this.MissionTextResponses[i]).X, (float)bounds.Width - (vector.X - (float)bounds.X) * 2f - 20f);
				if (Button.doButton(80000 + i * 100, (int)(vector.X + num2 + 30f), (int)vector.Y, 20, 20, "-", null))
				{
					this.MissionTextResponses.RemoveAt(i);
				}
				vector.Y += (float)num;
			}
			if (this.isAddingTextResponse)
			{
				string text = null;
				bool flag = Programs.parseStringFromGetStringCommand(this.os, out text);
				if (text == null)
				{
					text = "";
				}
				vector.Y += 5f;
				GuiData.spriteBatch.Draw(Utils.white, new Rectangle(bounds.X + 1, (int)vector.Y, bounds.Width - 2 - bounds.Width / 9, 40), this.os.indentBackgroundColor);
				vector.Y += 10f;
				TextItem.doFontLabel(vector + new Vector2(25f, 0f), text, GuiData.tinyfont, null, float.MaxValue, float.MaxValue, false);
				Vector2 vector2 = GuiData.tinyfont.MeasureString(text);
				vector2.Y = 0f;
				if (this.os.timer % 1f <= 0.5f)
				{
					GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int)(vector.X + vector2.X + 2f) + 25, (int)vector.Y, 4, 20), Color.White);
				}
				int num3 = bounds.Width - 1 - bounds.Width / 10;
				if (flag || Button.doButton(8000094, bounds.X + num3 - 4, (int)vector.Y - 10, bounds.Width / 9 - 3, 40, LocaleTerms.Loc("Add"), new Color?(this.os.highlightColor)))
				{
					if (!flag)
					{
						this.os.terminal.executeLine();
					}
					this.isAddingTextResponse = false;
					if (!string.IsNullOrWhiteSpace(text))
					{
						this.MissionTextResponses.Add(text);
					}
					this.inProgressTextResponse = null;
				}
				else
				{
					this.inProgressTextResponse = text;
				}
			}
			else if (Button.doButton(8000098, (int)(vector.X + 4f), (int)vector.Y, 140, 24, LocaleTerms.Loc("Add Detail"), null))
			{
				this.isAddingTextResponse = true;
				this.os.execute("getString Detail");
				this.os.terminal.executionPreventionIsInteruptable = true;
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000095DC File Offset: 0x000077DC
		private void DrawHeaderLine(Rectangle dest, string header, SpriteBatch sb)
		{
			Vector2 vector = GuiData.font.MeasureString(header);
			Vector2 scale = new Vector2(1f, Math.Min((float)dest.Height, vector.Y) / vector.Y);
			if (vector.X > (float)dest.Width)
			{
				scale.X = 1f / (vector.X / (float)dest.Width);
			}
			scale = new Vector2(Math.Min(scale.X, scale.Y), Math.Min(scale.X, scale.Y));
			Vector2 position = new Vector2((float)dest.X, (float)(dest.Y + dest.Height) - vector.Y * scale.Y);
			sb.DrawString(GuiData.font, header, position, this.themeColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.4f);
			dest.Y += dest.Height - 1;
			dest.Height = 1;
			sb.Draw(Utils.white, dest, this.themeColor);
		}

		// Token: 0x0400004F RID: 79
		public const string ROOT_FOLDERNAME = "HomeBase";

		// Token: 0x04000050 RID: 80
		public const string MISSION_FOLDERNAME = "contracts";

		// Token: 0x04000051 RID: 81
		public const string ARCHIVE_FOLDERNAME = "archive";

		// Token: 0x04000052 RID: 82
		public const string ACTIONS_FOLDERNAME = "runtime";

		// Token: 0x04000053 RID: 83
		public const string CONFIG_FILENAME = "dhs_config.sys";

		// Token: 0x04000054 RID: 84
		public List<string> Agents = new List<string>();

		// Token: 0x04000055 RID: 85
		public Color themeColor = Color.MediumVioletRed;

		// Token: 0x04000056 RID: 86
		private string groupName = "Bibliotheca";

		// Token: 0x04000057 RID: 87
		public bool AddsFactionPointForMissionCompleteion = true;

		// Token: 0x04000058 RID: 88
		public bool AutoClearMissionsOnSingleComplete = true;

		// Token: 0x04000059 RID: 89
		public bool AllowContractAbbandon = false;

		// Token: 0x0400005A RID: 90
		public DelayableActionSystem DelayedActions;

		// Token: 0x0400005B RID: 91
		private Folder rootFolder;

		// Token: 0x0400005C RID: 92
		private Folder missionFolder;

		// Token: 0x0400005D RID: 93
		private Folder archivesFolder;

		// Token: 0x0400005E RID: 94
		private Folder actionsFolder;

		// Token: 0x0400005F RID: 95
		private DLCHubServer.DHSState State = DLCHubServer.DHSState.Welcome;

		// Token: 0x04000060 RID: 96
		private DLCHubServer.ClaimableMission SelectedMission = null;

		// Token: 0x04000061 RID: 97
		private bool isAddingTextResponse = false;

		// Token: 0x04000062 RID: 98
		private List<string> MissionTextResponses = new List<string>();

		// Token: 0x04000063 RID: 99
		private string inProgressTextResponse = null;

		// Token: 0x04000064 RID: 100
		private float BaseWelcomeFadeoutTime = 4f;

		// Token: 0x04000065 RID: 101
		private float WelcomeFadeoutTimerLeft = 4f;

		// Token: 0x04000066 RID: 102
		public IRCSystem IRCSystem;

		// Token: 0x04000067 RID: 103
		private Dictionary<string, Color> HighlightedWords = new Dictionary<string, Color>();

		// Token: 0x04000068 RID: 104
		private SoundEffect ButtonPressSound;

		// Token: 0x04000069 RID: 105
		private SoundEffect WooshBuildup;

		// Token: 0x0400006A RID: 106
		private SoundEffect USIntro;

		// Token: 0x0400006B RID: 107
		internal List<DLCHubServer.ClaimableMission> ActiveMissions = new List<DLCHubServer.ClaimableMission>(3);

		// Token: 0x0400006C RID: 108
		private Texture2D MissionAvaliableIcon;

		// Token: 0x0400006D RID: 109
		private Texture2D MissionTakenIcon;

		// Token: 0x0400006E RID: 110
		private Texture2D MissionPlayersIcon;

		// Token: 0x0400006F RID: 111
		private Texture2D LoadingSpinner;

		// Token: 0x04000070 RID: 112
		private int UIButtonOffset = 0;

		// Token: 0x04000071 RID: 113
		private ScrollableTextRegion ScrollableTextPanel;

		// Token: 0x04000072 RID: 114
		private bool ShouldShowMissionIncompleteMessage = false;

		// Token: 0x04000073 RID: 115
		private bool AbandonMissionShowConfirmation = false;

		// Token: 0x04000074 RID: 116
		private HexGridBackground HexBackground;

		// Token: 0x04000075 RID: 117
		private float timeSpentInLoading = 0f;

		// Token: 0x04000076 RID: 118
		private bool HasStartedWoosh = false;

		// Token: 0x0200000C RID: 12
		private enum DHSState
		{
			// Token: 0x04000078 RID: 120
			Welcome,
			// Token: 0x04000079 RID: 121
			Login,
			// Token: 0x0400007A RID: 122
			Home,
			// Token: 0x0400007B RID: 123
			ArchiveList,
			// Token: 0x0400007C RID: 124
			MissionSelectView,
			// Token: 0x0400007D RID: 125
			ContractDetailView
		}

		// Token: 0x0200000D RID: 13
		internal class ClaimableMission
		{
			// Token: 0x0400007E RID: 126
			public string AgentClaim;

			// Token: 0x0400007F RID: 127
			public bool IsComplete;

			// Token: 0x04000080 RID: 128
			public ActiveMission Mission;

			// Token: 0x04000081 RID: 129
			public float UITextScrollDown;
		}
	}
}
