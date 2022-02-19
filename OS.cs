using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Factions;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Misc;
using Hacknet.Mission;
using Hacknet.Modules.Overlays;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x02000144 RID: 324
	internal class OS : GameScreen
	{
		// Token: 0x060007DD RID: 2013 RVA: 0x00081B00 File Offset: 0x0007FD00
		public OS()
		{
			char[] array = new char[3];
			array[0] = ' ';
			array[1] = '\n';
			this.trimChars = array;
			this.DestroyThreads = false;
			this.canRunContent = true;
			this.stayAliveTimer = OS.TCP_STAYALIVE_TIMER;
			this.opponentLocation = "";
			this.displayCache = "";
			this.getStringCache = "";
			this.commandInvalid = false;
			this.connectedIPLastFrame = "";
			this.homeNodeID = "entropy00";
			this.homeAssetServerID = "entropy01";
			this.DisableTopBarButtons = false;
			this.DisableEmailIcon = false;
			this.LanguageCreatedIn = "en-us";
			this.HasExitedAndEnded = false;
			this.ExitToMenuMessageBox = null;
			this.Flags = new ProgressionFlags();
			this.ActiveHackers = new List<KeyValuePair<string, string>>();
			this.updateErrorCount = 0;
			this.drawErrorCount = 0;
			this.terminalOnlyMode = false;
			this.HasLoadedDLCContent = false;
			this.IsInDLCMode = false;
			this.PreDLCFaction = "entropy";
			this.PreDLCVisibleNodesCache = "";
			this.IsDLCSave = false;
			this.IsDLCConventionDemo = false;
			this.ConditionalActions = new RunnableConditionalActions();
			this.EffectsUpdater = new ActiveEffectsUpdater();
			this.ShowDLCAlertsIcon = false;
			this.TrackersInProgress = new List<OS.TrackerDetail>();
			this.ForceLoadOverrideStream = null;
			this.defaultHighlightColor = new Color(0, 139, 199, 255);
			this.defaultTopBarColor = new Color(130, 65, 27);
			this.warningColor = Color.Red;
			this.highlightColor = new Color(0, 139, 199, 255);
			this.subtleTextColor = new Color(90, 90, 90);
			this.darkBackgroundColor = new Color(8, 8, 8);
			this.indentBackgroundColor = new Color(12, 12, 12);
			this.outlineColor = new Color(68, 68, 68);
			this.lockedColor = new Color(65, 16, 16, 200);
			this.brightLockedColor = new Color(160, 0, 0);
			this.brightUnlockedColor = new Color(0, 160, 0);
			this.unlockedColor = new Color(39, 65, 36);
			this.lightGray = new Color(180, 180, 180);
			this.shellColor = new Color(222, 201, 24);
			this.shellButtonColor = new Color(105, 167, 188);
			this.moduleColorSolid = new Color(50, 59, 90, 255);
			this.moduleColorSolidDefault = new Color(50, 59, 90, 255);
			this.moduleColorStrong = new Color(14, 28, 40, 80);
			this.moduleColorBacking = new Color(5, 6, 7, 10);
			this.topBarColor = new Color(0, 139, 199, 255);
			this.semiTransText = new Color(120, 120, 120, 0);
			this.terminalTextColor = new Color(213, 245, 255);
			this.topBarTextColor = new Color(126, 126, 126, 100);
			this.superLightWhite = new Color(2, 2, 2, 30);
			this.connectedNodeHighlight = new Color(222, 0, 0, 195);
			this.exeModuleTopBar = new Color(130, 65, 27, 80);
			this.exeModuleTitleText = new Color(155, 85, 37, 0);
			this.netmapToolTipColor = new Color(213, 245, 255, 0);
			this.netmapToolTipBackground = new Color(0, 0, 0, 150);
			this.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 0);
			this.topBarIconsColor = Color.White;
			this.BackgroundImageFillColor = Color.Black;
			this.UseAspectPreserveBackgroundScaling = false;
			this.AFX_KeyboardMiddle = new Color(77, 145, 255);
			this.AFX_KeyboardOuter = new Color(105, 138, 255);
			this.AFX_WordLogo = new Color(105, 138, 255);
			this.AFX_Other = new Color(0, 178, 255);
			this.thisComputerNode = new Color(95, 220, 83);
			this.scanlinesColor = new Color(255, 255, 255, 15);
			base..ctor();
			this.multiplayer = false;
			OS.currentInstance = this;
		}

		// Token: 0x060007DE RID: 2014 RVA: 0x00082074 File Offset: 0x00080274
		public OS(TcpClient socket, NetworkStream stream, bool actingServer, ScreenManager sman)
		{
			char[] array = new char[3];
			array[0] = ' ';
			array[1] = '\n';
			this.trimChars = array;
			this.DestroyThreads = false;
			this.canRunContent = true;
			this.stayAliveTimer = OS.TCP_STAYALIVE_TIMER;
			this.opponentLocation = "";
			this.displayCache = "";
			this.getStringCache = "";
			this.commandInvalid = false;
			this.connectedIPLastFrame = "";
			this.homeNodeID = "entropy00";
			this.homeAssetServerID = "entropy01";
			this.DisableTopBarButtons = false;
			this.DisableEmailIcon = false;
			this.LanguageCreatedIn = "en-us";
			this.HasExitedAndEnded = false;
			this.ExitToMenuMessageBox = null;
			this.Flags = new ProgressionFlags();
			this.ActiveHackers = new List<KeyValuePair<string, string>>();
			this.updateErrorCount = 0;
			this.drawErrorCount = 0;
			this.terminalOnlyMode = false;
			this.HasLoadedDLCContent = false;
			this.IsInDLCMode = false;
			this.PreDLCFaction = "entropy";
			this.PreDLCVisibleNodesCache = "";
			this.IsDLCSave = false;
			this.IsDLCConventionDemo = false;
			this.ConditionalActions = new RunnableConditionalActions();
			this.EffectsUpdater = new ActiveEffectsUpdater();
			this.ShowDLCAlertsIcon = false;
			this.TrackersInProgress = new List<OS.TrackerDetail>();
			this.ForceLoadOverrideStream = null;
			this.defaultHighlightColor = new Color(0, 139, 199, 255);
			this.defaultTopBarColor = new Color(130, 65, 27);
			this.warningColor = Color.Red;
			this.highlightColor = new Color(0, 139, 199, 255);
			this.subtleTextColor = new Color(90, 90, 90);
			this.darkBackgroundColor = new Color(8, 8, 8);
			this.indentBackgroundColor = new Color(12, 12, 12);
			this.outlineColor = new Color(68, 68, 68);
			this.lockedColor = new Color(65, 16, 16, 200);
			this.brightLockedColor = new Color(160, 0, 0);
			this.brightUnlockedColor = new Color(0, 160, 0);
			this.unlockedColor = new Color(39, 65, 36);
			this.lightGray = new Color(180, 180, 180);
			this.shellColor = new Color(222, 201, 24);
			this.shellButtonColor = new Color(105, 167, 188);
			this.moduleColorSolid = new Color(50, 59, 90, 255);
			this.moduleColorSolidDefault = new Color(50, 59, 90, 255);
			this.moduleColorStrong = new Color(14, 28, 40, 80);
			this.moduleColorBacking = new Color(5, 6, 7, 10);
			this.topBarColor = new Color(0, 139, 199, 255);
			this.semiTransText = new Color(120, 120, 120, 0);
			this.terminalTextColor = new Color(213, 245, 255);
			this.topBarTextColor = new Color(126, 126, 126, 100);
			this.superLightWhite = new Color(2, 2, 2, 30);
			this.connectedNodeHighlight = new Color(222, 0, 0, 195);
			this.exeModuleTopBar = new Color(130, 65, 27, 80);
			this.exeModuleTitleText = new Color(155, 85, 37, 0);
			this.netmapToolTipColor = new Color(213, 245, 255, 0);
			this.netmapToolTipBackground = new Color(0, 0, 0, 150);
			this.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 0);
			this.topBarIconsColor = Color.White;
			this.BackgroundImageFillColor = Color.Black;
			this.UseAspectPreserveBackgroundScaling = false;
			this.AFX_KeyboardMiddle = new Color(77, 145, 255);
			this.AFX_KeyboardOuter = new Color(105, 138, 255);
			this.AFX_WordLogo = new Color(105, 138, 255);
			this.AFX_Other = new Color(0, 178, 255);
			this.thisComputerNode = new Color(95, 220, 83);
			this.scanlinesColor = new Color(255, 255, 255, 15);
			base..ctor();
			base.ScreenManager = sman;
			this.multiplayer = true;
			this.client = socket;
			this.isServer = actingServer;
			this.netStream = stream;
			this.inBuffer = new byte[4096];
			this.outBuffer = new byte[4096];
			this.encoder = new ASCIIEncoding();
			this.canRunContent = false;
			TextBox.cursorPosition = 0;
			OS.currentInstance = this;
		}

		// Token: 0x060007DF RID: 2015 RVA: 0x00082640 File Offset: 0x00080840
		public override void LoadContent()
		{
			if (this.canRunContent)
			{
				this.delayer = new ActionDelayer();
				ComputerLoader.init(this);
				this.content = base.ScreenManager.Game.Content;
				this.username = ((this.SaveUserAccountName == null) ? (Settings.isConventionDemo ? Settings.ConventionLoginName : Environment.UserName) : this.SaveUserAccountName);
				this.username = FileSanitiser.purifyStringForDisplay(this.username);
				Vector2 compLocation = new Vector2(0.1f, 0.5f);
				if (this.multiplayer && !this.isServer)
				{
					compLocation = new Vector2(0.8f, 0.8f);
				}
				this.ramAvaliable = this.totalRam;
				string text = NetworkMap.generateRandomIP();
				string text2 = (this.multiplayer && this.isServer) ? NetworkMap.generateRandomIP() : text;
				this.thisComputer = new Computer(this.username + " PC", NetworkMap.generateRandomIP(), compLocation, 5, 4, this);
				this.thisComputer.adminIP = this.thisComputer.ip;
				this.thisComputer.idName = "playerComp";
				this.thisComputer.Memory = new MemoryContents();
				Folder folder = this.thisComputer.files.root.searchForFolder("home");
				folder.folders.Add(new Folder("stash"));
				folder.folders.Add(new Folder("misc"));
				this.GibsonIP = NetworkMap.generateRandomIP();
				UserDetail value = this.thisComputer.users[0];
				value.known = true;
				this.thisComputer.users[0] = value;
				this.defaultUser = new UserDetail(this.username, "password", 1);
				this.defaultUser.known = true;
				OSTheme theme = OSTheme.HacknetBlue;
				if (Settings.isConventionDemo && !this.IsDLCConventionDemo && Settings.ShuffleThemeOnDemoStart)
				{
					double num = Utils.random.NextDouble();
					if (num < 0.25)
					{
						theme = OSTheme.HacknetMint;
					}
					else if (num < 0.5)
					{
						theme = OSTheme.HackerGreen;
					}
					else if (num < 0.75)
					{
						theme = OSTheme.HacknetPurple;
					}
				}
				ThemeManager.setThemeOnComputer(this.thisComputer, theme);
				if (this.multiplayer)
				{
					this.thisComputer.addMultiplayerTargetFile();
					this.sendMessage(string.Concat(new object[]
					{
						"newComp #",
						this.thisComputer.ip,
						"#",
						(int)compLocation.X,
						"#",
						(int)compLocation.Y,
						"#",
						5,
						"#",
						this.thisComputer.name
					}));
					this.multiplayerMissionLoaded = false;
				}
				if (!OS.WillLoadSave)
				{
					People.init();
				}
				this.modules = new List<Module>();
				this.exes = new List<ExeModule>();
				this.shells = new List<ShellExe>();
				this.shellIPs = new List<string>();
				Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
				int module_WIDTH = RamModule.MODULE_WIDTH;
				int num2 = 205;
				int num3 = (int)((double)(viewport.Width - module_WIDTH - 6) * 0.44420000000000004);
				int num4 = (int)((double)(viewport.Width - module_WIDTH - 6) * 0.5558);
				int height = viewport.Height - num2 - OS.TOP_BAR_HEIGHT - 6;
				this.terminal = new Terminal(new Rectangle(viewport.Width - 2 - num3, OS.TOP_BAR_HEIGHT, num3, viewport.Height - OS.TOP_BAR_HEIGHT - 2), this);
				this.terminal.name = "TERMINAL";
				this.modules.Add(this.terminal);
				this.netMap = new NetworkMap(new Rectangle(module_WIDTH + 4, viewport.Height - num2 - 2, num4 - 1, num2), this);
				this.netMap.name = "netMap v1.7";
				this.modules.Add(this.netMap);
				this.display = new DisplayModule(new Rectangle(module_WIDTH + 4, OS.TOP_BAR_HEIGHT, num4 - 2, height), this);
				this.display.name = "DISPLAY";
				this.modules.Add(this.display);
				this.ram = new RamModule(new Rectangle(2, OS.TOP_BAR_HEIGHT, module_WIDTH, this.ramAvaliable + RamModule.contentStartOffset), this);
				this.ram.name = "RAM";
				this.modules.Add(this.ram);
				for (int i = 0; i < this.modules.Count; i++)
				{
					this.modules[i].LoadContent();
				}
				if (!Settings.IsInExtensionMode)
				{
					for (int i = 0; i < 2; i++)
					{
						if (this.isServer || !this.multiplayer)
						{
							this.thisComputer.links.Add(i);
						}
						else
						{
							this.thisComputer.links.Add(this.netMap.nodes.Count - 1 - i);
						}
					}
				}
				if (this.allFactions == null)
				{
					this.allFactions = new AllFactions();
					this.allFactions.init();
				}
				if (!Settings.IsInExtensionMode)
				{
					this.currentFaction = this.allFactions.factions[this.allFactions.currentFaction];
				}
				bool flag = false;
				if (!OS.WillLoadSave)
				{
					this.netMap.nodes.Insert(0, this.thisComputer);
					this.netMap.visibleNodes.Add(0);
					if (!Settings.IsInExtensionMode)
					{
						MusicManager.loadAsCurrentSong(this.IsDLCConventionDemo ? "Music\\out_run_the_wolves" : "Music\\Revolve");
					}
					this.LanguageCreatedIn = Settings.ActiveLocale;
					if (Settings.IsInExtensionMode)
					{
						ExtensionLoader.LoadNewExtensionSession(ExtensionLoader.ActiveExtensionInfo, this);
					}
				}
				else
				{
					this.loadSaveFile();
					flag = true;
					Settings.initShowsTutorial = false;
					SaveFixHacks.FixSavesWithTerribleHacks(this);
				}
				if (!this.multiplayer && !flag && !Settings.IsInExtensionMode)
				{
					MailServer.shouldGenerateJunk = false;
					this.netMap.mailServer.addNewUser(this.thisComputer.ip, this.defaultUser);
				}
				this.topBar = new Rectangle(0, 0, viewport.Width, OS.TOP_BAR_HEIGHT - 1);
				this.crashModule = new CrashModule(new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height), this);
				this.crashModule.LoadContent();
				this.introTextModule = new IntroTextModule(new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height), this);
				this.introTextModule.LoadContent();
				this.traceTracker = new TraceTracker(this);
				this.IncConnectionOverlay = new IncomingConnectionOverlay(this);
				this.scanLines = this.content.Load<Texture2D>("ScanLines");
				this.cross = this.content.Load<Texture2D>("Cross");
				this.cog = this.content.Load<Texture2D>("Cog");
				this.saveIcon = this.content.Load<Texture2D>("SaveIcon");
				this.beepSound = this.content.Load<SoundEffect>("SFX/beep");
				if (!Settings.IsInExtensionMode)
				{
					if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "colamaeleon")
					{
						ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/CoelTheme.xml");
						ThemeManager.switchTheme(this, "DLC/Themes/CoelTheme.xml");
					}
					else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "rain_shatter")
					{
						ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/RainTheme.xml");
						ThemeManager.switchTheme(this, "DLC/Themes/RainTheme.xml");
					}
					else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "orann")
					{
						ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/RiptideThemeStandard.xml");
						ThemeManager.switchTheme(this, "DLC/Themes/RiptideThemeStandard.xml");
					}
					else if (DLC1SessionUpgrader.HasDLC1Installed && this.username.ToLower() == "hypernexus")
					{
						ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/MiamiThemeLightBlue.xml");
						ThemeManager.switchTheme(this, "DLC/Themes/MiamiThemeLightBlue.xml");
					}
				}
				if (!this.multiplayer && !flag && !Settings.IsInExtensionMode)
				{
					this.loadMissionNodes();
				}
				if (!this.HasLoadedDLCContent && Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && !Settings.IsInExtensionMode)
				{
					DLC1SessionUpgrader.UpgradeSession(this, flag);
					this.HasLoadedDLCContent = true;
				}
				this.mailicon = new MailIcon(this, new Vector2(0f, 0f));
				this.mailicon.pos.X = (float)(viewport.Width - this.mailicon.getWidth() - 2);
				if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
				{
					this.hubServerAlertsIcon = new HubServerAlertsIcon(this.content, "dhs", new string[]
					{
						"@channel",
						"@Channel",
						"@" + this.defaultUser.name
					});
					this.hubServerAlertsIcon.Init(this);
					if (this.HasLoadedDLCContent)
					{
						this.AircraftInfoOverlay = new AircraftInfoOverlay(this);
					}
				}
				SAChangeAlertIcon.UpdateAlertIcon(this);
				if (!flag)
				{
					MusicManager.playSong();
				}
				if (flag || !Settings.slowOSStartup)
				{
					this.initShowsTutorial = false;
					this.introTextModule.complete = true;
				}
				this.inputEnabled = true;
				this.isLoaded = true;
				this.fullscreen = new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height);
				this.TraceDangerSequence = new TraceDangerSequence(this.content, base.ScreenManager.SpriteBatch, this.fullscreen, this);
				this.endingSequence = new EndingSequenceModule(this.fullscreen, this);
				if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
				{
					this.BootAssitanceModule = new BootCrashAssistanceModule(this.fullscreen, this);
				}
				bool flag2 = Settings.slowOSStartup && !flag;
				if (Settings.IsInExtensionMode)
				{
					if (!ExtensionLoader.ActiveExtensionInfo.HasIntroStartup)
					{
						flag2 = false;
						this.introTextModule.complete = true;
					}
				}
				bool flag3 = Settings.osStartsWithTutorial && (!flag || !this.Flags.HasFlag("TutorialComplete"));
				if (Settings.IsInExtensionMode && !ExtensionLoader.ActiveExtensionInfo.StartsWithTutorial)
				{
					flag3 = false;
					this.initShowsTutorial = false;
				}
				if (flag2)
				{
					this.rebootThisComputer();
					if (Settings.initShowsTutorial)
					{
						this.display.visible = false;
						this.ram.visible = false;
						this.netMap.visible = false;
						this.terminal.visible = true;
					}
				}
				else if (flag3)
				{
					this.display.visible = false;
					this.ram.visible = false;
					this.netMap.visible = false;
					this.terminal.visible = true;
					this.terminal.reset();
					Settings.initShowsTutorial = true;
					this.initShowsTutorial = true;
					if (!OS.TestingPassOnly)
					{
						this.execute("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj Init");
					}
				}
				else if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && HostileHackerBreakinSequence.IsInBlockingHostileFileState(this))
				{
					this.rebootThisComputer();
					this.BootAssitanceModule.ShouldSkipDialogueTypeout = true;
				}
				else
				{
					if (Settings.EnableDLC && HostileHackerBreakinSequence.IsFirstSuccessfulBootAfterBlockingState(this))
					{
						HostileHackerBreakinSequence.ReactToFirstSuccesfulBoot(this);
						this.rebootThisComputer();
					}
					if (!OS.TestingPassOnly)
					{
						this.runCommand("connect " + this.thisComputer.ip);
					}
					Folder folder2 = this.thisComputer.files.root.searchForFolder("sys");
					if (folder2.searchForFile("Notes_Reopener.bat") != null)
					{
						this.runCommand("notes");
					}
				}
				if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && this.HasLoadedDLCContent)
				{
					bool flag4 = false;
					if (!this.Flags.HasFlag("AircraftInfoOverlayDeactivated"))
					{
						if (this.Flags.HasFlag("AircraftInfoOverlayActivated"))
						{
							flag4 = true;
						}
						if (!flag4)
						{
							Computer computer = Programs.getComputer(this, "dair_crash");
							Folder folder3 = computer.files.root.searchForFolder("FlightSystems");
							bool flag5 = false;
							for (int i = 0; i < folder3.files.Count; i++)
							{
								if (folder3.files[i].name == "747FlightOps.dll")
								{
									flag5 = true;
								}
							}
							AircraftDaemon aircraftDaemon = (AircraftDaemon)computer.getDaemon(typeof(AircraftDaemon));
							if (!flag5 && !this.Flags.HasFlag("DLC_PlaneResult"))
							{
								flag4 = true;
							}
						}
					}
					if (flag4)
					{
						Computer computer = Programs.getComputer(this, "dair_crash");
						AircraftDaemon aircraftDaemon = (AircraftDaemon)computer.getDaemon(typeof(AircraftDaemon));
						aircraftDaemon.StartReloadFirmware();
						aircraftDaemon.StartUpdating();
						this.AircraftInfoOverlay.Activate();
						this.AircraftInfoOverlay.IsMonitoringDLCEndingCases = true;
						MissionFunctions.runCommand(0, "playAirlineCrashSongSequence");
					}
				}
			}
			else if (this.multiplayer)
			{
				this.initializeNetwork();
			}
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x000835A1 File Offset: 0x000817A1
		public void loadMultiplayerMission()
		{
			this.currentMission = (ActiveMission)ComputerLoader.readMission("Content/Missions/MultiplayerMission.xml");
		}

		// Token: 0x060007E1 RID: 2017 RVA: 0x000835BC File Offset: 0x000817BC
		public void loadMissionNodes()
		{
			if (!this.multiplayer)
			{
				List<string> list = BootLoadList.getList();
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						Computer.loadFromFile(list[i]);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						Console.WriteLine(ex.StackTrace);
						throw ex;
					}
				}
				if (ComputerLoader.postAllLoadedActions != null)
				{
					ComputerLoader.postAllLoadedActions();
				}
				if (Settings.isDemoMode)
				{
					list = BootLoadList.getDemoList();
					for (int i = 0; i < list.Count; i++)
					{
						Computer.loadFromFile(list[i]);
					}
				}
				if (!this.initShowsTutorial && !Settings.IsInExtensionMode && !this.IsDLCConventionDemo)
				{
					if (Settings.isSpecialTestBuild)
					{
						ComputerLoader.loadMission("Content/Missions/Misc/TesterCSECIntroMission.xml", false);
					}
					else
					{
						ComputerLoader.loadMission("Content/Missions/BitMissionIntro.xml", false);
					}
				}
				else if (!Settings.IsInExtensionMode)
				{
					this.currentMission = (ActiveMission)ComputerLoader.readMission("Content/Missions/BitMissionIntro.xml");
				}
			}
		}

		// Token: 0x060007E2 RID: 2018 RVA: 0x000836EC File Offset: 0x000818EC
		public override void UnloadContent()
		{
		}

		// Token: 0x060007E3 RID: 2019 RVA: 0x000836F0 File Offset: 0x000818F0
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			try
			{
				this.lastGameTime = gameTime;
				base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
				float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
				SFX.Update(num);
				this.timer += num;
				if (this.isLoaded)
				{
					if (this.bootingUp)
					{
						this.canRunContent = false;
						this.thisComputer.bootupTick(num);
						this.crashModule.Update(num);
						if (!this.thisComputer.disabled)
						{
							if (this.FirstTimeStartup && !this.introTextModule.complete)
							{
								this.introTextModule.Update(num);
							}
							if (this.introTextModule.complete)
							{
								this.FirstTimeStartup = false;
								this.bootingUp = false;
								this.canRunContent = true;
								this.crashModule.completeReboot();
							}
						}
					}
				}
				this.delayer.Pump();
				if (this.canRunContent && this.isLoaded)
				{
					try
					{
						if (this.TraceDangerSequence.IsActive)
						{
							this.TraceDangerSequence.Update(num);
						}
						if (this.multiplayer)
						{
							if (!this.multiplayerMissionLoaded && this.opponentComputer != null)
							{
								this.loadMultiplayerMission();
								this.multiplayerMissionLoaded = true;
							}
							this.stayAliveTimer -= num;
							if (this.stayAliveTimer <= 0f)
							{
								this.stayAliveTimer = OS.TCP_STAYALIVE_TIMER;
								this.sendMessage("stayAlive " + (int)OS.currentElapsedTime);
							}
						}
						else if (this.IsInDLCMode || this.ShowDLCAlertsIcon)
						{
							this.hubServerAlertsIcon.Update(num);
						}
						else
						{
							this.mailicon.Update(num);
						}
						for (int i = 0; i < this.modules.Count; i++)
						{
							this.modules[i].Update(num);
						}
						if (this.UpdateSubscriptions != null)
						{
							this.UpdateSubscriptions(num);
						}
						if (this.connectedComp == null)
						{
							if (this.connectedIPLastFrame != null)
							{
								this.handleDisconnection();
							}
						}
						else if (this.connectedIPLastFrame != this.connectedComp.ip)
						{
							this.handleDisconnection();
						}
						for (int i = 0; i < this.TrackersInProgress.Count; i++)
						{
							if (this.connectedComp != null && this.connectedComp == this.TrackersInProgress[i].comp)
							{
								this.TrackersInProgress.RemoveAt(i);
								i--;
							}
							else
							{
								OS.TrackerDetail value = this.TrackersInProgress[i];
								value.timeLeft -= num;
								if (value.timeLeft <= 0f)
								{
									TrackerCompleteSequence.TrackComplete(this, value.comp);
									break;
								}
								this.TrackersInProgress[i] = value;
							}
						}
						this.ramAvaliable = this.totalRam;
						if (this.exes.Count > 0)
						{
							this.exes[0].bounds.Y = this.ram.bounds.Y + RamModule.contentStartOffset;
						}
						for (int i = 0; i < this.exes.Count; i++)
						{
							this.exes[i].bounds.X = this.ram.bounds.X;
							if (i > 0 && i < this.exes.Count)
							{
								this.exes[i].bounds.Y = this.exes[i - 1].bounds.Y + this.exes[i - 1].bounds.Height;
							}
							if (this.exes[i].needsRemoval)
							{
								this.exes.RemoveAt(i);
								i--;
							}
							else
							{
								this.ramAvaliable -= this.exes[i].ramCost;
							}
						}
						for (int i = 0; i < this.exes.Count; i++)
						{
							this.exes[i].Update(num);
						}
						if (this.currentMission != null)
						{
							this.currentMission.Update(num);
						}
						for (int i = 0; i < this.branchMissions.Count; i++)
						{
							this.branchMissions[i].Update(num);
						}
						this.traceTracker.Update(num);
						if (this.gameSavedTextAlpha > 0f)
						{
							this.gameSavedTextAlpha -= num;
						}
						if (this.PorthackCompleteFlashTime > 0f)
						{
							this.PorthackCompleteFlashTime -= num;
						}
						if (this.MissionCompleteFlashTime > 0f)
						{
							this.MissionCompleteFlashTime -= num;
						}
						if (this.warningFlashTimer > 0f)
						{
							this.warningFlashTimer -= num;
							if (this.warningFlashTimer <= 0f)
							{
								this.highlightColor = this.defaultHighlightColor;
							}
							else
							{
								this.highlightColor = Color.Lerp(this.defaultHighlightColor, this.warningColor, this.warningFlashTimer / OS.WARNING_FLASH_TIME);
								this.moduleColorSolid = Color.Lerp(this.moduleColorSolidDefault, this.warningColor, this.warningFlashTimer / OS.WARNING_FLASH_TIME);
							}
						}
						this.EffectsUpdater.Update(num, this);
						float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
						this.IncConnectionOverlay.Update(dt);
						if (this.AircraftInfoOverlay != null && this.AircraftInfoOverlay.IsActive)
						{
							this.AircraftInfoOverlay.Update(dt);
						}
						OS.currentElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
						this.updateErrorCount++;
						if (this.updateErrorCount < 5)
						{
							Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
						}
					}
				}
				else
				{
					this.endingSequence.Update(num);
					if (this.IsInDLCMode)
					{
						this.BootAssitanceModule.Update(num);
					}
				}
				this.ConditionalActions.Update(num, this);
				this.connectedIPLastFrame = ((this.connectedComp != null) ? this.connectedComp.ip : null);
			}
			catch (Exception ex)
			{
				this.updateErrorCount++;
				if (this.updateErrorCount >= 3)
				{
					this.handleUpdateError();
				}
				else
				{
					Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				}
			}
		}

		// Token: 0x060007E4 RID: 2020 RVA: 0x00083EC0 File Offset: 0x000820C0
		private void handleDisconnection()
		{
			Computer computer = Programs.getComputer(this, this.connectedIPLastFrame);
			if (computer != null)
			{
				Administrator admin = computer.admin;
				if (admin != null)
				{
					admin.disconnectionDetected(computer, this);
				}
				if (computer.HasTracker)
				{
					if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(this, computer, null))
					{
						float timeLeft = TrackerCompleteSequence.MinTrackTime + Utils.randm(TrackerCompleteSequence.MaxTrackTime - TrackerCompleteSequence.MinTrackTime);
						this.TrackersInProgress.Add(new OS.TrackerDetail
						{
							comp = computer,
							timeLeft = timeLeft
						});
					}
				}
			}
		}

		// Token: 0x060007E5 RID: 2021 RVA: 0x00083F64 File Offset: 0x00082164
		public override void HandleInput(InputState input)
		{
			GuiData.doInput(input);
			if (Utils.keyPressed(input, Keys.NumLock, new PlayerIndex?(base.ScreenManager.controllingPlayer)))
			{
				PostProcessor.bloomEnabled = !PostProcessor.bloomEnabled;
			}
		}

		// Token: 0x060007E6 RID: 2022 RVA: 0x00083FAA File Offset: 0x000821AA
		public void drawBackground()
		{
			ThemeManager.drawBackgroundImage(GuiData.spriteBatch, this.fullscreen);
		}

		// Token: 0x060007E7 RID: 2023 RVA: 0x00083FC0 File Offset: 0x000821C0
		public void RequestRemovalOfAllPopups()
		{
			if (this.ExitToMenuMessageBox != null)
			{
				this.ExitToMenuMessageBox.ExitScreen();
				this.ExitToMenuMessageBox = null;
			}
		}

		// Token: 0x060007E8 RID: 2024 RVA: 0x00083FF0 File Offset: 0x000821F0
		public void drawScanlines()
		{
			if (PostProcessor.scanlinesEnabled)
			{
				Vector2 position = new Vector2(0f, 0f);
				while (position.X < (float)base.ScreenManager.GraphicsDevice.Viewport.Width)
				{
					while (position.Y < (float)base.ScreenManager.GraphicsDevice.Viewport.Height)
					{
						GuiData.spriteBatch.Draw(this.scanLines, position, this.scanlinesColor);
						position.Y += (float)this.scanLines.Height;
					}
					position.Y = 0f;
					position.X += (float)this.scanLines.Width;
				}
			}
		}

		// Token: 0x060007E9 RID: 2025 RVA: 0x000840D4 File Offset: 0x000822D4
		public void drawModules(GameTime gameTime)
		{
			Vector2 zero = Vector2.Zero;
			GuiData.spriteBatch.Draw(Utils.white, this.topBar, this.topBarColor);
			float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
			try
			{
				if (this.connectedComp != null)
				{
					this.locationString = string.Concat(new string[]
					{
						LocaleTerms.Loc("Location"),
						": ",
						this.connectedComp.name,
						"@",
						this.connectedIP,
						" "
					});
				}
				else
				{
					this.locationString = LocaleTerms.Loc("Location: Not Connected") + " ";
				}
				Vector2 vector = GuiData.UITinyfont.MeasureString(this.locationString);
				zero.X = (float)this.topBar.Width - vector.X - (float)this.mailicon.getWidth();
				zero.Y -= 3f;
				GuiData.spriteBatch.DrawString(GuiData.UITinyfont, this.locationString, zero, this.topBarTextColor);
				if (GuiData.ActiveFontConfig.tinyFontCharHeight * 2f <= (float)this.topBar.Height)
				{
					string text = LocaleTerms.Loc("Home IP:") + " " + this.thisComputer.ip + " ";
					zero.Y += (float)(this.topBar.Height / 2);
					vector = GuiData.UITinyfont.MeasureString(text);
					zero.X = (float)this.topBar.Width - vector.X - (float)this.mailicon.getWidth();
					GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text, zero, this.topBarTextColor);
				}
				zero.Y = 0f;
			}
			catch (Exception)
			{
			}
			zero.X = 110f;
			if (!Settings.isLockedDemoMode && !this.DisableTopBarButtons)
			{
				if (Button.doButton(3827178, 3, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cross))
				{
					this.ExitToMenuMessageBox = new MessageBoxScreen(LocaleTerms.Loc("Quit HackNetOS\nCurrent Session?") + "\n", false, true);
					this.ExitToMenuMessageBox.OverrideAcceptedText = LocaleTerms.Loc("Exit to Menu");
					this.ExitToMenuMessageBox.Accepted += this.quitGame;
					base.ScreenManager.AddScreen(this.ExitToMenuMessageBox);
				}
				bool flag = !this.TraceDangerSequence.IsActive;
				if (flag && Button.doButton(3827179, 26, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cog))
				{
					this.saveGame();
					base.ScreenManager.AddScreen(new OptionsMenu(true), new PlayerIndex?(base.ScreenManager.controllingPlayer));
				}
				bool flag2 = !this.initShowsTutorial && !this.TraceDangerSequence.IsActive;
				flag2 &= (!Settings.IsInExtensionMode || ExtensionLoader.ActiveExtensionInfo.AllowSave);
				if (flag2 && Button.doButton(3827180, 49, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.saveIcon))
				{
					this.saveGame();
				}
				if (!this.initShowsTutorial && Settings.debugCommandsEnabled)
				{
					if (Button.doButton(3827190, 72, 0, 20, this.topBar.Height - 1, "", new Color?(this.topBarIconsColor), this.cog))
					{
						if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 0)
						{
							ThemeManager.switchTheme(this, OSTheme.HacknetBlue);
						}
						else if (ThemeManager.currentTheme == OSTheme.HacknetBlue)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/RiptideClassicTheme.xml");
							this.drawErrorCount = 2;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 2)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/MiamiTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/MiamiTheme.xml");
							this.drawErrorCount = 3;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 3)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/RainTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/RainTheme.xml");
							this.drawErrorCount = 4;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 4)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/RiptideTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/RiptideTheme.xml");
							this.drawErrorCount = 5;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 5)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/CautionTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/CautionTheme.xml");
							this.drawErrorCount = 6;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 6)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/HoraTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/HoraTheme.xml");
							this.drawErrorCount = 7;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 7)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/Floatvoid.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/FloatVoidTheme.xml");
							this.drawErrorCount = 8;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 8)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/MiamiThemeLightBlue.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/MiamiThemeLightBlue.xml");
							this.drawErrorCount = 9;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 9)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/StarfieldClassicTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/StarfieldClassicTheme.xml");
							this.drawErrorCount = 10;
						}
						else if (ThemeManager.currentTheme == OSTheme.Custom && this.drawErrorCount == 10)
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/CoelTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/CoelTheme.xml");
							this.drawErrorCount = 0;
						}
						else if (ThemeManager.currentTheme == OSTheme.HacknetPurple)
						{
							ThemeManager.switchTheme(this, OSTheme.HacknetMint);
						}
						else if (ThemeManager.currentTheme == OSTheme.HacknetMint)
						{
							ThemeManager.switchTheme(this, OSTheme.HackerGreen);
						}
						else
						{
							ThemeManager.setThemeOnComputer(this.thisComputer, "DLC/Themes/CautionTheme.xml");
							ThemeManager.switchTheme(this, "DLC/Themes/CautionTheme.xml");
						}
					}
				}
				else
				{
					zero.X = 80f;
				}
			}
			else
			{
				zero.X = 2f;
			}
			zero.Y = 1f;
			string text2 = string.Concat((int)(1.0 / gameTime.ElapsedGameTime.TotalSeconds + 0.5));
			GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text2, zero, this.topBarTextColor);
			zero.Y = 0f;
			if (!this.multiplayer && !this.DisableTopBarButtons && !this.DisableEmailIcon)
			{
				if (this.IsInDLCMode || this.ShowDLCAlertsIcon)
				{
					int width = this.mailicon.getWidth();
					this.hubServerAlertsIcon.Draw(new Rectangle((int)this.mailicon.pos.X, (int)this.mailicon.pos.Y, width, this.topBar.Height - 2), GuiData.spriteBatch);
				}
				else
				{
					this.mailicon.Draw();
				}
			}
			int num = this.ram.bounds.Height + this.topBar.Height + 16;
			if (num < this.fullscreen.Height && this.ram.visible)
			{
				this.audioVisualizer.Draw(new Rectangle(this.ram.bounds.X, num + 1, this.ram.bounds.Width - 2, this.fullscreen.Height - num - 4), GuiData.spriteBatch);
			}
			for (int i = 0; i < this.modules.Count; i++)
			{
				if (this.modules[i].visible)
				{
					this.modules[i].PreDrawStep();
					this.modules[i].Draw(t);
					this.modules[i].PostDrawStep();
				}
			}
			if (this.ram.visible)
			{
				for (int i = 0; i < this.exes.Count; i++)
				{
					this.exes[i].Draw(t);
				}
			}
			this.IncConnectionOverlay.Draw(this.fullscreen, GuiData.spriteBatch);
			if (this.AircraftInfoOverlay != null && this.AircraftInfoOverlay.IsActive)
			{
				Rectangle dest = new Rectangle(this.fullscreen.X, OS.TOP_BAR_HEIGHT + 1 + Module.PANEL_HEIGHT, this.fullscreen.Width, this.fullscreen.Height - (OS.TOP_BAR_HEIGHT + 2 + Module.PANEL_HEIGHT));
				this.AircraftInfoOverlay.Draw(dest, GuiData.spriteBatch);
			}
			this.traceTracker.Draw(GuiData.spriteBatch);
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			float scaleFactor = 1f - this.gameSavedTextAlpha;
			int num2 = 45;
			Vector2 vector2 = new Vector2(0f, (float)(base.ScreenManager.GraphicsDevice.Viewport.Height - num2));
			vector2 -= scaleFactor * new Vector2(0f, 200f);
			TextItem.doFontLabel(vector2, "SESSION SAVED", GuiData.titlefont, new Color?(this.thisComputerNode * this.gameSavedTextAlpha), float.MaxValue, (float)num2, false);
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x00084C20 File Offset: 0x00082E20
		public void quitGame(object sender, PlayerIndexEventArgs e)
		{
			this.HasExitedAndEnded = true;
			base.ExitScreen();
			MainMenu.resetOS();
			base.ScreenManager.AddScreen(new MainMenu());
			SaveFileManager.Init(false);
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x00084C50 File Offset: 0x00082E50
		public override void Draw(GameTime gameTime)
		{
			try
			{
				float t = (float)gameTime.ElapsedGameTime.TotalSeconds;
				if (this.lastGameTime == null)
				{
					this.lastGameTime = gameTime;
				}
				if (this.canRunContent && this.isLoaded)
				{
					PostProcessor.begin();
					GuiData.startDraw();
					try
					{
						if (!this.TraceDangerSequence.PreventOSRendering)
						{
							this.drawBackground();
							if (this.terminalOnlyMode)
							{
								this.terminal.Draw(t);
							}
							else
							{
								this.drawModules(gameTime);
							}
							SFX.Draw(GuiData.spriteBatch);
						}
						if (this.TraceDangerSequence.IsActive)
						{
							this.TraceDangerSequence.Draw();
						}
					}
					catch (Exception ex)
					{
						this.drawErrorCount++;
						if (this.drawErrorCount < 5)
						{
							Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex) + "\r\n\r\n");
						}
					}
					GuiData.endDraw();
					PostProcessor.end();
					GuiData.startDraw();
					if (this.postFXDrawActions != null)
					{
						this.postFXDrawActions();
						this.postFXDrawActions = null;
					}
					this.drawScanlines();
					GuiData.endDraw();
				}
				else if (this.endingSequence.IsActive)
				{
					PostProcessor.begin();
					base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
					this.endingSequence.Draw(t);
					this.drawScanlines();
					base.ScreenManager.SpriteBatch.End();
					PostProcessor.end();
				}
				else if (this.BootAssitanceModule != null && this.BootAssitanceModule.IsActive)
				{
					PostProcessor.begin();
					base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
					this.BootAssitanceModule.Draw(t);
					this.drawScanlines();
					base.ScreenManager.SpriteBatch.End();
					PostProcessor.end();
				}
				else if (this.bootingUp)
				{
					PostProcessor.begin();
					base.ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone);
					if (this.thisComputer.disabled)
					{
						this.RequestRemovalOfAllPopups();
						if (this.TraceDangerSequence.IsActive)
						{
							this.TraceDangerSequence.CancelTraceDangerSequence();
						}
						this.crashModule.Draw(t);
					}
					else
					{
						this.introTextModule.Draw(t);
					}
					base.ScreenManager.SpriteBatch.End();
					PostProcessor.end();
				}
				else
				{
					GuiData.startDraw();
					TextItem.doSmallLabel(new Vector2(0f, 700f), LocaleTerms.Loc("Loading..."), null);
					GuiData.endDraw();
				}
			}
			catch (Exception ex)
			{
				this.drawErrorCount++;
				if (this.drawErrorCount >= 3)
				{
					this.handleDrawError();
				}
				else
				{
					Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				}
			}
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x00084FE8 File Offset: 0x000831E8
		private void handleUpdateError()
		{
			DebugLog.data.Add("----------------------Handling Update error");
			this.connectedComp = null;
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x00085002 File Offset: 0x00083202
		private void handleDrawError()
		{
			this.connectedComp = null;
			base.ScreenManager.GraphicsDevice.SetRenderTarget(null);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x00085020 File Offset: 0x00083220
		public void endMultiplayerMatch(bool won)
		{
			if (won)
			{
				this.sendMessage("mpOpponentWin " + this.timer.ToString());
			}
			base.ScreenManager.AddScreen(new MultiplayerGameOverScreen(won));
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x00085068 File Offset: 0x00083268
		private void initializeNetwork()
		{
			if (this.isServer)
			{
				string message = "init " + Utils.random.Next();
				this.sendMessage(message);
				Multiplayer.parseInputMessage(message, this);
			}
			else
			{
				this.sendMessage("clientConnect Hallo");
			}
			this.listenerThread = new Thread(new ThreadStart(this.listenThread));
			this.listenerThread.Name = "OS Listener Thread";
			this.listenerThread.Start();
			Console.WriteLine("Created Network Listener Thread");
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x00085100 File Offset: 0x00083300
		public void sendMessage(string message)
		{
			if (this.netStream.CanWrite)
			{
				this.outBuffer = this.encoder.GetBytes(message + "#&#");
				this.netStream.Write(this.outBuffer, 0, this.outBuffer.Length);
				this.netStream.Flush();
				for (int i = 0; i < message.Length; i++)
				{
					this.outBuffer[i] = 0;
				}
			}
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x00085188 File Offset: 0x00083388
		private void listenThread()
		{
			while (!this.DestroyThreads)
			{
				try
				{
					int num = this.netStream.Read(this.inBuffer, 0, this.inBuffer.Length);
					if (num == 0)
					{
						this.clientDisconnected();
					}
					else
					{
						string text = this.encoder.GetString(this.inBuffer).Trim(this.trimChars);
						string[] separator = new string[]
						{
							"#&#"
						};
						string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < array.Length; i++)
						{
							Multiplayer.parseInputMessage(array[i], this);
						}
						for (int i = 0; i < this.inBuffer.Length; i++)
						{
							this.inBuffer[i] = 0;
						}
					}
				}
				catch (IOException ex)
				{
					if (ex.Message.Contains("Unable to read"))
					{
						this.write("Opponent Disconnected - Victory");
						base.ScreenManager.AddScreen(new MultiplayerGameOverScreen(true));
						this.netStream.Close();
						return;
					}
					this.write("NetError: " + DisplayModule.splitForWidth(ex.ToString(), this.terminal.bounds.Width - 20));
				}
				catch (Exception ex2)
				{
					this.write("NetError: " + DisplayModule.splitForWidth(ex2.ToString(), this.terminal.bounds.Width - 20));
					break;
				}
				if (!Game1.threadsExiting)
				{
					continue;
				}
				return;
			}
			Console.WriteLine("Listener Thread Exiting");
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x00085368 File Offset: 0x00083568
		public void clientDisconnected()
		{
			this.write("DISCONNECTION detected");
			this.DestroyThreads = true;
			this.netStream.Close();
			this.client.Close();
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x00085398 File Offset: 0x00083598
		public void timerExpired()
		{
			if (this.traceCompleteOverrideAction != null)
			{
				this.traceCompleteOverrideAction();
			}
			else
			{
				if (this.connectedComp != null && this.connectedComp.admin != null)
				{
					this.connectedComp.admin.traceEjectionDetected(this.connectedComp, this);
				}
				if (this.Flags.HasFlag("CSEC_Member"))
				{
					this.TraceDangerSequence.BeginTraceDangerSequence();
				}
				else
				{
					this.thisComputer.crash(this.thisComputer.ip);
				}
			}
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x00085440 File Offset: 0x00083640
		public void loadSaveFile()
		{
			Stream stream;
			if (this.ForceLoadOverrideStream != null && OS.TestingPassOnly)
			{
				stream = this.ForceLoadOverrideStream;
			}
			else
			{
				stream = SaveFileManager.GetSaveReadStream(this.SaveGameUserName);
			}
			if (stream != null)
			{
				XmlReader xmlReader = XmlReader.Create(stream);
				this.loadTitleSaveData(xmlReader);
				if (Settings.ActiveLocale != this.LanguageCreatedIn)
				{
					LocaleActivator.ActivateLocale(this.LanguageCreatedIn, this.content);
					Settings.ActiveLocale = this.LanguageCreatedIn;
				}
				this.LoadExtraTitleSaveData(xmlReader);
				if (base.IsExiting)
				{
					return;
				}
				this.Flags.Load(xmlReader);
				this.netMap.load(xmlReader);
				this.currentMission = (ActiveMission)ActiveMission.load(xmlReader);
				if (this.currentMission != null && !(this.currentMission.reloadGoalsSourceFile == "Missions/BitMissionIntro.xml"))
				{
					ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(this.currentMission.reloadGoalsSourceFile);
				}
				Console.WriteLine(this.branchMissions.Count);
				this.allFactions = AllFactions.loadFromSave(xmlReader);
				if (!string.IsNullOrWhiteSpace(this.allFactions.currentFaction))
				{
					this.allFactions.setCurrentFaction(this.allFactions.currentFaction, this);
				}
				this.loadOtherSaveData(xmlReader);
				OS.WillLoadSave = false;
				stream.Flush();
				stream.Close();
			}
			this.FirstTimeStartup = false;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x000855C8 File Offset: 0x000837C8
		public void saveGame()
		{
			if (!this.initShowsTutorial)
			{
				if (!Settings.IsInExtensionMode || ExtensionLoader.ActiveExtensionInfo.AllowSave)
				{
					this.execute("save!(SJN!*SNL8vAewew57WewJdwl89(*4;;;&!)@&(ak'^&#@J3KH@!*");
				}
			}
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x00085610 File Offset: 0x00083810
		public void threadedSaveExecute(bool preventSaveText = false)
		{
			if (this.SaveInProgress)
			{
				if (this.SaveInQueue)
				{
					return;
				}
				this.SaveInQueue = true;
			}
			lock (SaveFileManager.CurrentlySaving)
			{
				this.SaveInProgress = true;
				if (this.SaveInQueue)
				{
					this.SaveInQueue = false;
				}
				if (!preventSaveText)
				{
					this.gameSavedTextAlpha = 1f;
				}
				this.writeSaveGame(this.SaveUserAccountName);
				StatsManager.SaveStatProgress();
				Console.WriteLine("Session Saved");
				this.SaveInProgress = false;
			}
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x000856CC File Offset: 0x000838CC
		private void writeSaveGame(string filename)
		{
			string text = "<?xml version =\"1.0\" encoding =\"UTF-8\" ?>\n";
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"<HacknetSave generatedMissionCount=\"",
				MissionGenerator.generationCount,
				"\" Username=\"",
				this.username,
				"\" Language=\"",
				this.LanguageCreatedIn,
				"\" DLCMode=\"",
				this.IsInDLCMode,
				"\" DisableMailIcon=\"",
				this.DisableEmailIcon,
				"\">\n"
			});
			text += this.GetDLCSaveString();
			text += this.Flags.GetSaveString();
			text += this.netMap.getSaveString();
			if (this.currentMission != null)
			{
				text += this.currentMission.getSaveString();
			}
			else
			{
				text += "<mission next=\"NULL_MISSION\" goals=\"none\" activeCheck=\"none\">\n</mission>";
			}
			text += "<branchMissions>\n";
			for (int i = 0; i < this.branchMissions.Count; i++)
			{
				text += this.branchMissions[i].getSaveString();
			}
			text += "</branchMissions>";
			text += this.allFactions.getSaveString();
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"<other music=\"",
				MusicManager.currentSongName,
				"\" homeNode=\"",
				this.homeNodeID,
				"\" homeAssetsNode=\"",
				this.homeAssetServerID,
				"\" />"
			});
			text += "</HacknetSave>";
			SaveFileManager.WriteSaveData(text, filename);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x00085898 File Offset: 0x00083A98
		public void LoadExtraTitleSaveData(XmlReader rdr)
		{
			this.IsDLCSave = false;
			while (!(rdr.Name == "Flags") || !rdr.IsStartElement())
			{
				if (rdr.Name == "DLC")
				{
					this.ReadDLCSaveData(rdr);
				}
				rdr.Read();
			}
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x000858FC File Offset: 0x00083AFC
		private void ReadDLCSaveData(XmlReader rdr)
		{
			this.IsDLCSave = true;
			while (!(rdr.Name == "DLC") || rdr.IsStartElement())
			{
				if (rdr.Name == "DLC")
				{
					if (rdr.MoveToAttribute("Active"))
					{
						this.IsInDLCMode = (rdr.ReadContentAsString().ToLower() == "true");
					}
					if (rdr.MoveToAttribute("LoadedContent"))
					{
						this.HasLoadedDLCContent = (rdr.ReadContentAsString().ToLower() == "true");
					}
				}
				if (rdr.Name == "Flags")
				{
					if (rdr.MoveToAttribute("OriginalFaction"))
					{
						this.PreDLCFaction = rdr.ReadContentAsString();
					}
				}
				if (rdr.Name == "OriginalVisibleNodes")
				{
					this.PreDLCVisibleNodesCache = rdr.ReadElementContentAsString();
				}
				if (rdr.Name == "ConditionalActions")
				{
					this.ConditionalActions = RunnableConditionalActions.Deserialize(rdr);
				}
				rdr.Read();
			}
			if (this.HasLoadedDLCContent)
			{
				if (!DLC1SessionUpgrader.HasDLC1Installed)
				{
					MainMenu.AccumErrors = "LOAD ERROR: Save " + this.SaveGameUserName + " is configured for Labyrinths DLC, but it is not installed on this computer.\n\n\n";
					base.ExitScreen();
					base.IsExiting = true;
				}
			}
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x00085A7C File Offset: 0x00083C7C
		private string GetDLCSaveString()
		{
			string str = string.Concat(new object[]
			{
				"<DLC Active=\"",
				this.IsInDLCMode,
				"\" LoadedContent=\"",
				this.HasLoadedDLCContent,
				"\">\n"
			});
			str = str + "<Flags OriginalFaction=\"" + this.PreDLCFaction + "\"/>\n";
			str = str + "<OriginalVisibleNodes>" + this.PreDLCVisibleNodesCache + "</OriginalVisibleNodes>\n";
			str = str + this.ConditionalActions.GetSaveString() + "\n";
			return str + "</DLC>";
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x00085B24 File Offset: 0x00083D24
		public void loadTitleSaveData(XmlReader reader)
		{
			while (reader.Name != "HacknetSave")
			{
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
			}
			if (reader.MoveToAttribute("generatedMissionCount"))
			{
				int generationCount = reader.ReadContentAsInt();
				MissionGenerator.generationCount = generationCount;
			}
			else
			{
				MissionGenerator.generationCount = 100;
			}
			if (reader.MoveToAttribute("Username"))
			{
				this.username = reader.ReadContentAsString();
				this.defaultUser.name = this.username;
			}
			if (reader.MoveToAttribute("Language"))
			{
				this.LanguageCreatedIn = reader.ReadContentAsString();
			}
			else
			{
				this.LanguageCreatedIn = "en-us";
			}
			if (reader.MoveToAttribute("DLCMode"))
			{
				this.IsInDLCMode = (reader.ReadContentAsString().ToLower() == "true" && Settings.EnableDLC);
			}
			if (reader.MoveToAttribute("DisableMailIcon"))
			{
				this.DisableEmailIcon = (reader.ReadContentAsString().ToLower() == "true" && Settings.EnableDLC);
				return;
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x00085C80 File Offset: 0x00083E80
		public void setMouseVisiblity(bool mouseIsVisible)
		{
			this.delayer.Post(ActionDelayer.NextTick(), delegate
			{
				Game1.getSingleton().IsMouseVisible = mouseIsVisible;
			});
		}

		// Token: 0x060007FD RID: 2045 RVA: 0x00085CBC File Offset: 0x00083EBC
		public void loadBranchMissionsSaveData(XmlReader reader)
		{
			while (reader.Name != "branchMissions")
			{
				if (!reader.EOF)
				{
					reader.Read();
					if (!(reader.Name == "other"))
					{
						continue;
					}
				}
				return;
			}
			if (reader.IsStartElement())
			{
				this.branchMissions.Clear();
				reader.Read();
				for (;;)
				{
					while ((!reader.IsStartElement() || !(reader.Name == "mission")) && (reader.IsStartElement() || !(reader.Name == "branchMissions")))
					{
						reader.Read();
					}
					if (reader.Name == "branchMissions")
					{
						break;
					}
					ActiveMission item = (ActiveMission)ActiveMission.load(reader);
					this.branchMissions.Add(item);
				}
				return;
			}
		}

		// Token: 0x060007FE RID: 2046 RVA: 0x00085DC8 File Offset: 0x00083FC8
		public void loadOtherSaveData(XmlReader reader)
		{
			while (reader.Name != "other")
			{
				if (reader.EOF)
				{
					return;
				}
				reader.Read();
			}
			reader.MoveToAttribute("music");
			string songname = reader.ReadContentAsString();
			MusicManager.playSongImmediatley(songname);
			if (reader.MoveToAttribute("homeNode"))
			{
				string text = reader.ReadContentAsString();
				this.homeNodeID = text;
			}
			if (reader.MoveToAttribute("homeAssetsNode"))
			{
				string text2 = reader.ReadContentAsString();
				this.homeAssetServerID = text2;
				return;
			}
		}

		// Token: 0x060007FF RID: 2047 RVA: 0x00085E62 File Offset: 0x00084062
		public override void inputMethodChanged(bool usingGamePad)
		{
		}

		// Token: 0x06000800 RID: 2048 RVA: 0x00085E68 File Offset: 0x00084068
		public void write(string text)
		{
			if (this.terminal != null && text.Length > 0)
			{
				string text2 = DisplayModule.cleanSplitForWidth(text, this.terminal.bounds.Width - 40);
				if (text2[text2.Length - 1] == '\n')
				{
					text2 = text2.Substring(0, text2.Length - 1);
				}
				this.terminal.writeLine(text2);
			}
		}

		// Token: 0x06000801 RID: 2049 RVA: 0x00085EE4 File Offset: 0x000840E4
		public void writeSingle(string text)
		{
			this.terminal.write(text);
		}

		// Token: 0x06000802 RID: 2050 RVA: 0x00085EF4 File Offset: 0x000840F4
		public void runCommand(string text)
		{
			if (!this.terminal.preventingExecution)
			{
				this.write("\n" + this.terminal.prompt + text);
				this.terminal.lastRunCommand = text;
				this.execute(text);
			}
		}

		// Token: 0x06000803 RID: 2051 RVA: 0x00085F44 File Offset: 0x00084144
		public void execute(string text)
		{
			string[] parameter = text.Split(new char[]
			{
				' '
			});
			Thread thread = new Thread(new ParameterizedThreadStart(this.threadExecute));
			thread.Name = "exe" + thread.Name;
			if (!text.StartsWith("save"))
			{
				thread.IsBackground = true;
			}
			thread.CurrentCulture = Game1.culture;
			thread.CurrentUICulture = Game1.culture;
			Console.WriteLine("Spawning thread for command " + text);
			thread.Start(parameter);
		}

		// Token: 0x06000804 RID: 2052 RVA: 0x00085FD8 File Offset: 0x000841D8
		public void connectedComputerCrashed(Computer c)
		{
			if (this.connectedComp != null)
			{
				this.connectedComp.disconnecting(this.thisComputer.ip, true);
			}
			this.connectedComp = null;
			this.display.command = "crash";
			this.terminal.prompt = "> ";
		}

		// Token: 0x06000805 RID: 2053 RVA: 0x00086034 File Offset: 0x00084234
		public void thisComputerCrashed()
		{
			this.display.command = "";
			this.connectedComp = null;
			this.bootingUp = true;
			this.exes.Clear();
			this.shellIPs.Clear();
			this.crashModule.reset();
			this.setMouseVisiblity(false);
		}

		// Token: 0x06000806 RID: 2054 RVA: 0x0008608C File Offset: 0x0008428C
		public void thisComputerIPReset()
		{
			if (this.traceTracker.active)
			{
				this.traceTracker.active = false;
			}
			if (this.TraceDangerSequence.IsActive)
			{
				this.TraceDangerSequence.CompleteIPResetSucsesfully();
			}
			this.thisComputer.adminIP = this.thisComputer.ip;
		}

		// Token: 0x06000807 RID: 2055 RVA: 0x000860F0 File Offset: 0x000842F0
		public void rebootThisComputer()
		{
			this.setMouseVisiblity(false);
			this.crashModule.reset();
			this.inputEnabled = false;
			this.bootingUp = true;
			this.thisComputer.disabled = true;
			this.canRunContent = false;
			this.thisComputer.silent = true;
			this.thisComputer.crash(this.thisComputer.ip);
			this.thisComputer.silent = false;
			this.thisComputer.bootupTick(CrashModule.BLUESCREEN_TIME + 0.5f);
			this.crashModule.Update(CrashModule.BLUESCREEN_TIME + 0.5f);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x00086190 File Offset: 0x00084390
		public void RefreshTheme()
		{
			this.topBarColor = this.defaultTopBarColor;
			this.highlightColor = this.defaultHighlightColor;
			this.moduleColorSolid = this.moduleColorSolidDefault;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x000861B8 File Offset: 0x000843B8
		private void threadExecute(object threadText)
		{
			try
			{
				string[] arguments = (string[])threadText;
				this.validCommand = ProgramRunner.ExecuteProgram(this, arguments);
			}
			catch (Exception)
			{
				int num = 0;
				num++;
			}
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x000861FC File Offset: 0x000843FC
		public bool hasConnectionPermission(bool admin)
		{
			bool result;
			if (admin)
			{
				result = (this.connectedComp == null || this.connectedComp.adminIP == this.thisComputer.ip || (this.connectedComp.currentUser.type == 0 && this.connectedComp.currentUser.name != null));
			}
			else
			{
				bool flag = !admin;
				if (flag)
				{
					if (this.connectedComp != null)
					{
						if (!this.connectedComp.userLoggedIn)
						{
							flag = false;
						}
					}
				}
				result = (this.connectedComp == null || this.connectedComp.adminIP == this.thisComputer.ip || flag);
			}
			return result;
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x000862E0 File Offset: 0x000844E0
		public void takeAdmin()
		{
			if (this.connectedComp != null)
			{
				this.connectedComp.giveAdmin(this.thisComputer.ip);
				this.runCommand("connect " + this.connectedComp.ip);
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x00086330 File Offset: 0x00084530
		public void takeAdmin(string ip)
		{
			Computer computer = Programs.getComputer(this, ip);
			if (computer != null)
			{
				computer.giveAdmin(this.thisComputer.ip);
				this.runCommand("connect " + computer.ip);
			}
		}

		// Token: 0x0600080D RID: 2061 RVA: 0x00086379 File Offset: 0x00084579
		public void warningFlash()
		{
			this.warningFlashTimer = OS.WARNING_FLASH_TIME;
		}

		// Token: 0x0600080E RID: 2062 RVA: 0x00086388 File Offset: 0x00084588
		public Rectangle getExeBounds()
		{
			int num = this.ram.bounds.Y + RamModule.contentStartOffset;
			for (int i = 0; i < this.exes.Count; i++)
			{
				num += this.exes[i].bounds.Height;
			}
			return new Rectangle(this.ram.bounds.X, num, 252, (int)OS.EXE_MODULE_HEIGHT);
		}

		// Token: 0x0600080F RID: 2063 RVA: 0x00086408 File Offset: 0x00084608
		public void launchExecutable(string exeName, string exeFileData, int targetPort, string[] allParams = null, string originalName = null)
		{
			int num = this.ram.bounds.Y + RamModule.contentStartOffset;
			for (int i = 0; i < this.exes.Count; i++)
			{
				num += this.exes[i].bounds.Height;
			}
			Rectangle location = new Rectangle(this.ram.bounds.X, num, RamModule.MODULE_WIDTH, (int)OS.EXE_MODULE_HEIGHT);
			exeName = exeName.ToLower();
			if (exeName.Equals("porthack"))
			{
				bool flag = false;
				bool flag2 = false;
				if (this.connectedComp != null)
				{
					int num2 = 0;
					for (int i = 0; i < this.connectedComp.portsOpen.Count; i++)
					{
						num2 += (int)this.connectedComp.portsOpen[i];
					}
					if (num2 > this.connectedComp.portsNeededForCrack)
					{
						flag = true;
					}
					if (this.connectedComp.firewall != null && !this.connectedComp.firewall.solved)
					{
						if (flag)
						{
							flag2 = true;
						}
						flag = false;
					}
				}
				if (flag)
				{
					this.addExe(new PortHackExe(location, this));
				}
				else if (flag2)
				{
					this.write(LocaleTerms.Loc("Target Machine Rejecting Syndicated UDP Traffic") + " -\n" + LocaleTerms.Loc("Bypass Firewall to allow unrestricted traffic"));
				}
				else
				{
					this.write(LocaleTerms.Loc("Too Few Open Ports to Run") + " - \n" + LocaleTerms.Loc("Open Additional Ports on Target Machine") + "\n");
				}
			}
			else if (exeName.Equals("forkbomb"))
			{
				if (this.hasConnectionPermission(true))
				{
					if (this.connectedComp == null || this.connectedComp.ip.Equals(this.thisComputer.ip))
					{
						this.addExe(new ForkBombExe(location, this, this.thisComputer.ip));
					}
					else if (this.multiplayer && this.connectedComp.ip.Equals(this.opponentComputer.ip))
					{
						this.sendMessage("eForkBomb " + this.connectedComp.ip);
					}
					else
					{
						this.connectedComp.crash(this.thisComputer.ip);
					}
				}
				else
				{
					this.write(LocaleTerms.Loc("Requires Administrator Access to Run"));
				}
			}
			else if (exeName.Equals("shell"))
			{
				if (this.hasConnectionPermission(true))
				{
					bool flag3 = false;
					string value = (this.connectedComp == null) ? this.thisComputer.ip : this.connectedComp.ip;
					for (int i = 0; i < this.exes.Count; i++)
					{
						if (this.exes[i] is ShellExe)
						{
							ShellExe shellExe = (ShellExe)this.exes[i];
							if (shellExe.targetIP.Equals(value))
							{
								flag3 = true;
							}
						}
					}
					if (!flag3)
					{
						this.addExe(new ShellExe(location, this));
					}
					else
					{
						this.write(LocaleTerms.Loc("This computer is already running a shell."));
					}
				}
				else
				{
					this.write(LocaleTerms.Loc("Requires Administrator Access to Run"));
				}
			}
			else
			{
				string filename = (originalName == null) ? exeName : originalName;
				string exeNameForData = PortExploits.GetExeNameForData(filename, exeFileData);
				if (exeNameForData != null)
				{
					string text = exeNameForData;
					switch (text)
					{
					case "SSHcrack.exe":
						this.addExe(new SSHCrackExe(location, this));
						break;
					case "FTPBounce.exe":
						this.addExe(new FTPBounceExe(location, this));
						break;
					case "SMTPoverflow.exe":
						this.addExe(new SMTPoverflowExe(location, this));
						break;
					case "WebServerWorm.exe":
						this.addExe(new HTTPExploitExe(location, this));
						break;
					case "Tutorial.exe":
						this.addExe(new AdvancedTutorial(location, this));
						break;
					case "Notes.exe":
						this.addExe(new NotesExe(location, this));
						break;
					case "SecurityTracer.exe":
						this.addExe(new SecurityTraceExe(location, this));
						break;
					case "SQL_MemCorrupt.exe":
						this.addExe(new SQLExploitExe(location, this));
						break;
					case "Decypher.exe":
						this.addExe(new DecypherExe(location, this, allParams));
						break;
					case "DECHead.exe":
						this.addExe(new DecypherTrackExe(location, this, allParams));
						break;
					case "Clock.exe":
						this.addExe(new ClockExe(location, this, allParams));
						break;
					case "KBT_PortTest.exe":
						this.addExe(new MedicalPortExe(location, this, allParams));
						break;
					case "TraceKill.exe":
						this.addExe(new TraceKillExe(location, this, allParams));
						break;
					case "eosDeviceScan.exe":
						this.addExe(new EOSDeviceScannerExe(location, this, allParams));
						break;
					case "themechanger.exe":
						this.addExe(new ThemeChangerExe(location, this, allParams));
						break;
					case "HexClock.exe":
						this.addExe(new HexClockExe(location, this, allParams));
						break;
					case "Sequencer.exe":
						this.addExe(new SequencerExe(location, this, allParams));
						break;
					case "hacknet.exe":
						this.write(" ");
						this.write(" ----- Error ----- ");
						this.write(" ");
						this.write("Program \"hacknet.exe\" is already running!");
						this.write(" ");
						this.write(" ----------------- ");
						this.write(" ");
						break;
					case "TorrentStreamInjector.exe":
						this.addExe(new TorrentPortExe(location, this, allParams));
						break;
					case "SSLTrojan.exe":
					{
						SSLPortExe sslportExe = SSLPortExe.GenerateInstanceOrNullFromArguments(allParams, location, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						if (sslportExe != null)
						{
							this.addExe(sslportExe);
						}
						break;
					}
					case "KaguyaTrial.exe":
					{
						bool flag4 = false;
						for (int i = 0; i < this.exes.Count; i++)
						{
							if (this.exes[i] is DLCIntroExe)
							{
								flag4 = true;
							}
						}
						if (!flag4)
						{
							this.addExe(new DLCIntroExe(location, this, allParams));
						}
						else
						{
							this.write(LocaleTerms.Loc("Kaguya Trials Already Running!"));
						}
						break;
					}
					case "FTPSprint.exe":
						this.addExe(new FTPFastExe(location, this, allParams));
						break;
					case "SignalScramble.exe":
					{
						DLCTraceSlower dlctraceSlower = DLCTraceSlower.GenerateInstanceOrNullFromArguments(allParams, location, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						if (dlctraceSlower != null)
						{
							this.addExe(dlctraceSlower);
						}
						break;
					}
					case "MemForensics.exe":
					{
						MemoryForensicsExe memoryForensicsExe = MemoryForensicsExe.GenerateInstanceOrNullFromArguments(location, this, allParams);
						if (memoryForensicsExe != null)
						{
							this.addExe(memoryForensicsExe);
						}
						break;
					}
					case "MemDumpGenerator.exe":
					{
						MemoryDumpDownloader memoryDumpDownloader = MemoryDumpDownloader.GenerateInstanceOrNullFromArguments(allParams, location, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						if (memoryDumpDownloader != null)
						{
							this.addExe(memoryDumpDownloader);
						}
						break;
					}
					case "PacificPortcrusher.exe":
						this.addExe(new PacificPortExe(location, this, allParams));
						break;
					case "NetmapOrganizer.exe":
						this.addExe(new NetmapOrganizerExe(location, this, allParams));
						break;
					case "ComShell.exe":
						ShellOverloaderExe.RunShellOverloaderExe(allParams, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						break;
					case "DNotes.exe":
						NotesDumperExe.RunNotesDumperExe(allParams, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						break;
					case "ClockV2.exe":
						this.addExe(new Clock2Exe(location, this, allParams));
						break;
					case "Tuneswap.exe":
						this.addExe(new TuneswapExe(location, this, allParams));
						break;
					case "RTSPCrack.exe":
						this.addExe(new RTSPPortExe(location, this, allParams));
						break;
					case "ESequencer.exe":
						this.addExe(new ExtensionSequencerExe(location, this, allParams));
						break;
					case "OpShell.exe":
						ShellReopenerExe.RunShellReopenerExe(allParams, this, (this.connectedComp == null) ? this.thisComputer : this.connectedComp);
						break;
					}
				}
				else
				{
					this.write(LocaleTerms.Loc("Program not Found"));
				}
			}
		}

		// Token: 0x06000810 RID: 2064 RVA: 0x00086EE0 File Offset: 0x000850E0
		public void addExe(ExeModule exe)
		{
			Computer computer = (this.connectedComp == null) ? this.thisComputer : this.connectedComp;
			if (exe.needsProxyAccess && computer.proxyActive)
			{
				this.write(LocaleTerms.Loc("Proxy Active -- Cannot Execute"));
			}
			else if (this.ramAvaliable >= exe.ramCost)
			{
				exe.LoadContent();
				this.exes.Add(exe);
			}
			else
			{
				this.ram.FlashMemoryWarning();
				this.write(LocaleTerms.Loc("Insufficient Memory"));
			}
		}

		// Token: 0x06000811 RID: 2065 RVA: 0x00086F7C File Offset: 0x0008517C
		public void failBoot()
		{
			this.graphicsFailBoot();
		}

		// Token: 0x06000812 RID: 2066 RVA: 0x00086F86 File Offset: 0x00085186
		public void graphicsFailBoot()
		{
			ThemeManager.switchTheme(this, OSTheme.TerminalOnlyBlack);
			this.topBar.Y = -100000;
			this.terminalOnlyMode = true;
		}

		// Token: 0x06000813 RID: 2067 RVA: 0x00086FA8 File Offset: 0x000851A8
		public void sucsesfulBoot()
		{
			this.topBar.Y = 0;
			this.terminalOnlyMode = false;
			this.connectedComp = null;
		}

		// Token: 0x040008F1 RID: 2289
		public static bool DEBUG_COMMANDS = Settings.debugCommandsEnabled;

		// Token: 0x040008F2 RID: 2290
		public static float EXE_MODULE_HEIGHT = 250f;

		// Token: 0x040008F3 RID: 2291
		public static float TCP_STAYALIVE_TIMER = 10f;

		// Token: 0x040008F4 RID: 2292
		public static float WARNING_FLASH_TIME = 2f;

		// Token: 0x040008F5 RID: 2293
		public static int TOP_BAR_HEIGHT = 21;

		// Token: 0x040008F6 RID: 2294
		public static OS currentInstance;

		// Token: 0x040008F7 RID: 2295
		public static bool WillLoadSave = false;

		// Token: 0x040008F8 RID: 2296
		public static bool TestingPassOnly = false;

		// Token: 0x040008F9 RID: 2297
		public bool FirstTimeStartup = Settings.slowOSStartup;

		// Token: 0x040008FA RID: 2298
		public bool initShowsTutorial = Settings.initShowsTutorial;

		// Token: 0x040008FB RID: 2299
		public bool inputEnabled = false;

		// Token: 0x040008FC RID: 2300
		public bool isLoaded = false;

		// Token: 0x040008FD RID: 2301
		private Texture2D scanLines;

		// Token: 0x040008FE RID: 2302
		private Texture2D cross;

		// Token: 0x040008FF RID: 2303
		private Texture2D cog;

		// Token: 0x04000900 RID: 2304
		private Texture2D saveIcon;

		// Token: 0x04000901 RID: 2305
		public float PorthackCompleteFlashTime = 0f;

		// Token: 0x04000902 RID: 2306
		public float MissionCompleteFlashTime = 0f;

		// Token: 0x04000903 RID: 2307
		private string locationString = "";

		// Token: 0x04000904 RID: 2308
		public string username = "";

		// Token: 0x04000905 RID: 2309
		public int totalRam = 800 - (OS.TOP_BAR_HEIGHT + 2) - RamModule.contentStartOffset;

		// Token: 0x04000906 RID: 2310
		public int ramAvaliable = 800 - (OS.TOP_BAR_HEIGHT + 2);

		// Token: 0x04000907 RID: 2311
		public int currentPID = 0;

		// Token: 0x04000908 RID: 2312
		public List<ShellExe> shells;

		// Token: 0x04000909 RID: 2313
		public List<string> shellIPs;

		// Token: 0x0400090A RID: 2314
		internal bool bootingUp = false;

		// Token: 0x0400090B RID: 2315
		public CrashModule crashModule;

		// Token: 0x0400090C RID: 2316
		public TraceDangerSequence TraceDangerSequence;

		// Token: 0x0400090D RID: 2317
		private IntroTextModule introTextModule;

		// Token: 0x0400090E RID: 2318
		public GameTime lastGameTime;

		// Token: 0x0400090F RID: 2319
		public UserDetail defaultUser;

		// Token: 0x04000910 RID: 2320
		public Terminal terminal;

		// Token: 0x04000911 RID: 2321
		public NetworkMap netMap;

		// Token: 0x04000912 RID: 2322
		public DisplayModule display;

		// Token: 0x04000913 RID: 2323
		public RamModule ram;

		// Token: 0x04000914 RID: 2324
		private List<Module> modules;

		// Token: 0x04000915 RID: 2325
		public IncomingConnectionOverlay IncConnectionOverlay;

		// Token: 0x04000916 RID: 2326
		public AircraftInfoOverlay AircraftInfoOverlay;

		// Token: 0x04000917 RID: 2327
		public ActiveMission currentMission;

		// Token: 0x04000918 RID: 2328
		public List<ActiveMission> branchMissions = new List<ActiveMission>();

		// Token: 0x04000919 RID: 2329
		public TraceTracker traceTracker;

		// Token: 0x0400091A RID: 2330
		public List<ExeModule> exes;

		// Token: 0x0400091B RID: 2331
		private Rectangle topBar;

		// Token: 0x0400091C RID: 2332
		public EndingSequenceModule endingSequence;

		// Token: 0x0400091D RID: 2333
		public MailIcon mailicon;

		// Token: 0x0400091E RID: 2334
		private AudioVisualizer audioVisualizer = new AudioVisualizer();

		// Token: 0x0400091F RID: 2335
		public string connectedIP = "";

		// Token: 0x04000920 RID: 2336
		public Computer thisComputer = null;

		// Token: 0x04000921 RID: 2337
		public Computer connectedComp = null;

		// Token: 0x04000922 RID: 2338
		public Computer opponentComputer = null;

		// Token: 0x04000923 RID: 2339
		public float warningFlashTimer = 0f;

		// Token: 0x04000924 RID: 2340
		public Faction currentFaction;

		// Token: 0x04000925 RID: 2341
		public AllFactions allFactions;

		// Token: 0x04000926 RID: 2342
		public List<int> navigationPath = new List<int>();

		// Token: 0x04000927 RID: 2343
		public ContentManager content;

		// Token: 0x04000928 RID: 2344
		public float gameSavedTextAlpha = -1f;

		// Token: 0x04000929 RID: 2345
		public string SaveGameUserName = "";

		// Token: 0x0400092A RID: 2346
		public string SaveUserAccountName = null;

		// Token: 0x0400092B RID: 2347
		public string SaveUserPassword = "password";

		// Token: 0x0400092C RID: 2348
		private bool SaveInProgress = false;

		// Token: 0x0400092D RID: 2349
		private bool SaveInQueue = false;

		// Token: 0x0400092E RID: 2350
		public bool multiplayer = false;

		// Token: 0x0400092F RID: 2351
		private TcpClient client;

		// Token: 0x04000930 RID: 2352
		public bool isServer = false;

		// Token: 0x04000931 RID: 2353
		private char[] trimChars;

		// Token: 0x04000932 RID: 2354
		private byte[] inBuffer;

		// Token: 0x04000933 RID: 2355
		private byte[] outBuffer;

		// Token: 0x04000934 RID: 2356
		private ASCIIEncoding encoder;

		// Token: 0x04000935 RID: 2357
		private Thread listenerThread;

		// Token: 0x04000936 RID: 2358
		private bool DestroyThreads;

		// Token: 0x04000937 RID: 2359
		private NetworkStream netStream;

		// Token: 0x04000938 RID: 2360
		public bool canRunContent;

		// Token: 0x04000939 RID: 2361
		public float stayAliveTimer;

		// Token: 0x0400093A RID: 2362
		private bool multiplayerMissionLoaded;

		// Token: 0x0400093B RID: 2363
		public string opponentLocation;

		// Token: 0x0400093C RID: 2364
		public string displayCache;

		// Token: 0x0400093D RID: 2365
		public string getStringCache;

		// Token: 0x0400093E RID: 2366
		public static double currentElapsedTime = 0.0;

		// Token: 0x0400093F RID: 2367
		public static float operationProgress = 0f;

		// Token: 0x04000940 RID: 2368
		public static object displayObjectCache = null;

		// Token: 0x04000941 RID: 2369
		public bool commandInvalid;

		// Token: 0x04000942 RID: 2370
		public Rectangle fullscreen;

		// Token: 0x04000943 RID: 2371
		public bool validCommand;

		// Token: 0x04000944 RID: 2372
		public ActionDelayer delayer;

		// Token: 0x04000945 RID: 2373
		public string connectedIPLastFrame;

		// Token: 0x04000946 RID: 2374
		public string homeNodeID;

		// Token: 0x04000947 RID: 2375
		public string homeAssetServerID;

		// Token: 0x04000948 RID: 2376
		public bool DisableTopBarButtons;

		// Token: 0x04000949 RID: 2377
		public bool DisableEmailIcon;

		// Token: 0x0400094A RID: 2378
		private string LanguageCreatedIn;

		// Token: 0x0400094B RID: 2379
		public bool HasExitedAndEnded;

		// Token: 0x0400094C RID: 2380
		private MessageBoxScreen ExitToMenuMessageBox;

		// Token: 0x0400094D RID: 2381
		public ProgressionFlags Flags;

		// Token: 0x0400094E RID: 2382
		public List<KeyValuePair<string, string>> ActiveHackers;

		// Token: 0x0400094F RID: 2383
		public float timer;

		// Token: 0x04000950 RID: 2384
		public SoundEffect beepSound;

		// Token: 0x04000951 RID: 2385
		private int updateErrorCount;

		// Token: 0x04000952 RID: 2386
		private int drawErrorCount;

		// Token: 0x04000953 RID: 2387
		public bool terminalOnlyMode;

		// Token: 0x04000954 RID: 2388
		public bool HasLoadedDLCContent;

		// Token: 0x04000955 RID: 2389
		public bool IsInDLCMode;

		// Token: 0x04000956 RID: 2390
		public HubServerAlertsIcon hubServerAlertsIcon;

		// Token: 0x04000957 RID: 2391
		public string PreDLCFaction;

		// Token: 0x04000958 RID: 2392
		public string PreDLCVisibleNodesCache;

		// Token: 0x04000959 RID: 2393
		public bool IsDLCSave;

		// Token: 0x0400095A RID: 2394
		public bool IsDLCConventionDemo;

		// Token: 0x0400095B RID: 2395
		public RunnableConditionalActions ConditionalActions;

		// Token: 0x0400095C RID: 2396
		public BootCrashAssistanceModule BootAssitanceModule;

		// Token: 0x0400095D RID: 2397
		internal string GibsonIP;

		// Token: 0x0400095E RID: 2398
		public ActiveEffectsUpdater EffectsUpdater;

		// Token: 0x0400095F RID: 2399
		public bool ShowDLCAlertsIcon;

		// Token: 0x04000960 RID: 2400
		public List<OS.TrackerDetail> TrackersInProgress;

		// Token: 0x04000961 RID: 2401
		internal Stream ForceLoadOverrideStream;

		// Token: 0x04000962 RID: 2402
		public Action postFXDrawActions;

		// Token: 0x04000963 RID: 2403
		public Action<float> UpdateSubscriptions;

		// Token: 0x04000964 RID: 2404
		public Action traceCompleteOverrideAction;

		// Token: 0x04000965 RID: 2405
		public Color defaultHighlightColor;

		// Token: 0x04000966 RID: 2406
		public Color defaultTopBarColor;

		// Token: 0x04000967 RID: 2407
		public Color warningColor;

		// Token: 0x04000968 RID: 2408
		public Color highlightColor;

		// Token: 0x04000969 RID: 2409
		public Color subtleTextColor;

		// Token: 0x0400096A RID: 2410
		public Color darkBackgroundColor;

		// Token: 0x0400096B RID: 2411
		public Color indentBackgroundColor;

		// Token: 0x0400096C RID: 2412
		public Color outlineColor;

		// Token: 0x0400096D RID: 2413
		public Color lockedColor;

		// Token: 0x0400096E RID: 2414
		public Color brightLockedColor;

		// Token: 0x0400096F RID: 2415
		public Color brightUnlockedColor;

		// Token: 0x04000970 RID: 2416
		public Color unlockedColor;

		// Token: 0x04000971 RID: 2417
		public Color lightGray;

		// Token: 0x04000972 RID: 2418
		public Color shellColor;

		// Token: 0x04000973 RID: 2419
		public Color shellButtonColor;

		// Token: 0x04000974 RID: 2420
		public Color moduleColorSolid;

		// Token: 0x04000975 RID: 2421
		public Color moduleColorSolidDefault;

		// Token: 0x04000976 RID: 2422
		public Color moduleColorStrong;

		// Token: 0x04000977 RID: 2423
		public Color moduleColorBacking;

		// Token: 0x04000978 RID: 2424
		public Color topBarColor;

		// Token: 0x04000979 RID: 2425
		public Color semiTransText;

		// Token: 0x0400097A RID: 2426
		public Color terminalTextColor;

		// Token: 0x0400097B RID: 2427
		public Color topBarTextColor;

		// Token: 0x0400097C RID: 2428
		public Color superLightWhite;

		// Token: 0x0400097D RID: 2429
		public Color connectedNodeHighlight;

		// Token: 0x0400097E RID: 2430
		public Color exeModuleTopBar;

		// Token: 0x0400097F RID: 2431
		public Color exeModuleTitleText;

		// Token: 0x04000980 RID: 2432
		public Color netmapToolTipColor;

		// Token: 0x04000981 RID: 2433
		public Color netmapToolTipBackground;

		// Token: 0x04000982 RID: 2434
		public Color displayModuleExtraLayerBackingColor;

		// Token: 0x04000983 RID: 2435
		public Color topBarIconsColor;

		// Token: 0x04000984 RID: 2436
		public Color BackgroundImageFillColor;

		// Token: 0x04000985 RID: 2437
		public bool UseAspectPreserveBackgroundScaling;

		// Token: 0x04000986 RID: 2438
		public Color AFX_KeyboardMiddle;

		// Token: 0x04000987 RID: 2439
		public Color AFX_KeyboardOuter;

		// Token: 0x04000988 RID: 2440
		public Color AFX_WordLogo;

		// Token: 0x04000989 RID: 2441
		public Color AFX_Other;

		// Token: 0x0400098A RID: 2442
		public Color thisComputerNode;

		// Token: 0x0400098B RID: 2443
		public Color scanlinesColor;

		// Token: 0x02000145 RID: 325
		public struct TrackerDetail
		{
			// Token: 0x0400098C RID: 2444
			public Computer comp;

			// Token: 0x0400098D RID: 2445
			public float timeLeft;
		}
	}
}
