using System;
using System.Collections.Generic;
using System.IO;
using Hacknet.Extensions;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200012F RID: 303
	internal class MissionListingServer : AuthenticatingDaemon
	{
		// Token: 0x0600073E RID: 1854 RVA: 0x00076474 File Offset: 0x00074674
		public MissionListingServer(Computer c, string serviceName, string group, OS _os, bool _isPublic = false, bool _isAssigner = false) : base(c, serviceName, _os)
		{
			this.groupName = group;
			this.topBar = this.os.content.Load<Texture2D>("Panel");
			this.corner = this.os.content.Load<Texture2D>("Corner");
			if (group.Equals("Entropy"))
			{
				this.themeColor = new Color(3, 102, 49);
				this.logo = this.os.content.Load<Texture2D>("EntropyLogo");
			}
			else if (group.Equals("NetEdu"))
			{
				this.themeColor = new Color(119, 104, 160);
				this.logo = this.os.content.Load<Texture2D>("Sprites/Academic_Logo");
			}
			else if (group.Equals("Kellis Biotech"))
			{
				this.themeColor = new Color(106, 176, 255);
				this.logo = this.os.content.Load<Texture2D>("Sprites/KellisLogo");
			}
			else
			{
				this.themeColor = new Color(204, 163, 27);
				this.logo = this.os.content.Load<Texture2D>("SlashbotLogo");
			}
			this.logoRect = new Rectangle(0, 0, 64, 64);
			this.isPublic = _isPublic;
			this.missionAssigner = _isAssigner;
			if (this.isPublic)
			{
				this.state = 1;
			}
			else
			{
				this.state = 0;
			}
			this.missions = new List<ActiveMission>();
			this.branchMissions = new List<List<ActiveMission>>();
			if (this.isPublic)
			{
				this.listingTitle = string.Format(LocaleTerms.Loc("{0} News"), group);
			}
			else
			{
				this.listingTitle = LocaleTerms.Loc("Available Contracts");
			}
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x00076690 File Offset: 0x00074890
		public MissionListingServer(Computer c, string serviceName, string iconPath, string articleFolderPath, Color themeColor, OS _os, bool _isPublic = false, bool _isAssigner = false) : base(c, serviceName, _os)
		{
			this.groupName = serviceName;
			this.topBar = this.os.content.Load<Texture2D>("Panel");
			this.corner = this.os.content.Load<Texture2D>("Corner");
			this.themeColor = themeColor;
			string str = "Content/";
			if (Settings.IsInExtensionMode)
			{
				str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
			}
			try
			{
				using (FileStream fileStream = File.OpenRead(str + iconPath))
				{
					this.logo = Texture2D.FromStream(GuiData.spriteBatch.GraphicsDevice, fileStream);
				}
			}
			catch (Exception)
			{
				this.logo = this.os.content.Load<Texture2D>("Sprites/Academic_Logo");
			}
			this.NeedsCustomFolderLoad = true;
			this.CustomFolderLoadPath = str + articleFolderPath;
			this.CustomFolderLoadPath = this.CustomFolderLoadPath.Replace('\\', '/');
			if (!this.CustomFolderLoadPath.EndsWith("/"))
			{
				this.CustomFolderLoadPath += "/";
			}
			this.logoRect = new Rectangle(0, 0, 64, 64);
			this.isPublic = _isPublic;
			this.missionAssigner = _isAssigner;
			if (this.isPublic)
			{
				this.state = 1;
			}
			else
			{
				this.state = 0;
			}
			this.missions = new List<ActiveMission>();
			this.branchMissions = new List<List<ActiveMission>>();
			if (this.isPublic)
			{
				this.listingTitle = serviceName;
			}
			else
			{
				this.listingTitle = LocaleTerms.Loc("Available Contracts");
			}
			this.HasCustomColor = true;
			this.IconReloadPath = iconPath;
			this.ArticleFolderPath = articleFolderPath;
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x000768A8 File Offset: 0x00074AA8
		public override void initFiles()
		{
			base.initFiles();
			this.initFilesystem();
			this.addListingsForGroup();
		}

		// Token: 0x06000741 RID: 1857 RVA: 0x000768C0 File Offset: 0x00074AC0
		public override void loadInit()
		{
			base.loadInit();
			this.root = this.comp.files.root.searchForFolder("MsgBoard");
			this.missionFolder = this.root.searchForFolder("listings");
			this.closedMissionsFolder = this.root.searchForFolder("closed");
			if (this.closedMissionsFolder == null)
			{
				this.closedMissionsFolder = new Folder("closed");
				this.root.folders.Add(this.closedMissionsFolder);
			}
			if (!this.missionAssigner)
			{
				for (int i = 0; i < this.missionFolder.files.Count; i++)
				{
					try
					{
						string postingTitle = this.missionFolder.files[i].name.Replace("_", " ");
						string data = this.missionFolder.files[i].data;
						ActiveMission activeMission = new ActiveMission(null, null, default(MailServer.EMailData));
						activeMission.postingTitle = postingTitle;
						activeMission.postingBody = data;
						this.missions.Add(activeMission);
					}
					catch (Exception)
					{
						int num = 0;
						num++;
					}
				}
			}
			else
			{
				List<ActiveMission> list = this.os.branchMissions;
				for (int i = 0; i < this.missionFolder.files.Count; i++)
				{
					try
					{
						string data2 = this.missionFolder.files[i].data;
						int num2 = 0;
						this.os.branchMissions.Clear();
						ActiveMission activeMission = (ActiveMission)MissionSerializer.restoreMissionFromFile(data2, out num2);
						List<ActiveMission> list2 = new List<ActiveMission>();
						list2.AddRange(this.os.branchMissions.ToArray());
						this.branchMissions.Add(list2);
						this.missions.Add(activeMission);
					}
					catch (Exception)
					{
						int num = 0;
						num++;
					}
				}
				this.os.branchMissions = list;
			}
		}

		// Token: 0x06000742 RID: 1858 RVA: 0x00076B04 File Offset: 0x00074D04
		public override string getSaveString()
		{
			string text = "";
			if (this.HasCustomColor)
			{
				text = string.Format("icon=\"{0}\" color=\"{1}\" articles=\"{2}\" ", this.IconReloadPath, Utils.convertColorToParseableString(this.themeColor), this.ArticleFolderPath);
			}
			return string.Concat(new object[]
			{
				"<MissionListingServer name=\"",
				this.name,
				"\" group=\"",
				this.groupName,
				"\" public=\"",
				this.isPublic,
				"\" assign=\"",
				this.missionAssigner,
				"\" title=\"",
				this.listingTitle,
				"\" ",
				text,
				"/>"
			});
		}

		// Token: 0x06000743 RID: 1859 RVA: 0x00076BD4 File Offset: 0x00074DD4
		public void addMisison(ActiveMission m, bool injectToTop = false)
		{
			string dataEntry = m.postingBody;
			if (this.missionAssigner)
			{
				dataEntry = MissionSerializer.generateMissionFile(m, 0, this.groupName, null);
			}
			FileEntry item = new FileEntry(dataEntry, m.postingTitle);
			if (injectToTop)
			{
				this.missionFolder.files.Insert(0, item);
				this.missions.Insert(0, m);
				this.branchMissions.Insert(0, this.os.branchMissions);
			}
			else
			{
				this.missionFolder.files.Add(item);
				this.missions.Add(m);
				this.branchMissions.Add(this.os.branchMissions);
			}
		}

		// Token: 0x06000744 RID: 1860 RVA: 0x00076C90 File Offset: 0x00074E90
		public void removeMission(string missionPath)
		{
			for (int i = 0; i < this.missions.Count; i++)
			{
				if (this.missions[i].reloadGoalsSourceFile == LocalizedFileLoader.GetLocalizedFilepath(missionPath))
				{
					this.removeMission(i);
					i--;
				}
			}
		}

		// Token: 0x06000745 RID: 1861 RVA: 0x00076CEC File Offset: 0x00074EEC
		public void removeMission(int index)
		{
			bool flag = false;
			for (int i = 0; i < this.missionFolder.files.Count; i++)
			{
				if (this.missionFolder.files[i].name.Equals(this.missions[index].postingTitle.Replace(" ", "_")))
				{
					FileEntry item = this.missionFolder.files[i];
					this.missionFolder.files.RemoveAt(i);
					this.closedMissionsFolder.files.Add(item);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
			}
			this.missions.RemoveAt(index);
		}

		// Token: 0x06000746 RID: 1862 RVA: 0x00076DB4 File Offset: 0x00074FB4
		public void initFilesystem()
		{
			this.rootPath = new List<int>();
			this.missionFolderPath = new List<int>();
			this.root = new Folder("MsgBoard");
			this.rootPath.Add(this.comp.files.root.folders.IndexOf(this.root));
			this.missionFolderPath.Add(this.comp.files.root.folders.IndexOf(this.root));
			this.missionFolder = new Folder("listings");
			this.missionFolderPath.Add(0);
			this.root.folders.Add(this.missionFolder);
			this.closedMissionsFolder = new Folder("closed");
			this.root.folders.Add(this.closedMissionsFolder);
			this.sysFile = new FileEntry(Computer.generateBinaryString(1024), "config.sys");
			this.root.files.Add(this.sysFile);
			string dataEntry = Utils.readEntireFile("Content/LocPost/ListingServerCautionFile.txt");
			FileEntry item = new FileEntry(dataEntry, "Config_CAUTION.txt");
			this.root.files.Add(item);
			this.comp.files.root.folders.Add(this.root);
		}

		// Token: 0x06000747 RID: 1863 RVA: 0x00076F18 File Offset: 0x00075118
		public void addListingsForGroup()
		{
			List<ActiveMission> list = this.os.branchMissions;
			this.os.branchMissions = null;
			bool flag = false;
			if (!this.NeedsCustomFolderLoad && this.groupName.ToLower().Equals("entropy"))
			{
				this.NeedsCustomFolderLoad = true;
				this.CustomFolderLoadPath = "Content/Missions/Entropy/StartingSet/";
				flag = true;
			}
			if (this.NeedsCustomFolderLoad)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(this.CustomFolderLoadPath);
				FileInfo[] files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string filename = this.CustomFolderLoadPath + files[i].Name;
					this.os.branchMissions = new List<ActiveMission>();
					this.addMisison((ActiveMission)ComputerLoader.readMission(filename), false);
				}
				if (flag)
				{
					for (int i = 0; i < 2; i++)
					{
						this.os.branchMissions = new List<ActiveMission>();
						this.addMisison((ActiveMission)MissionGenerator.generate(2), false);
					}
				}
			}
			else if (this.groupName.ToLower().Equals("netedu"))
			{
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education4.0.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education2.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education3.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education1.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education4.1.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education5.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Misc/EducationArticles/Education6.xml"), false);
			}
			else if (this.groupName.ToLower().Equals("slashbot"))
			{
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews1.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews3.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews2.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews4.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/Entropy/NewsArticles/EntropyNews5.xml"), false);
			}
			else if (this.groupName.ToLower().Equals("kellis biotech"))
			{
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware1.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware2.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware3.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware4.xml"), false);
				this.addMisison((ActiveMission)ComputerLoader.readMission("Content/Missions/MainHub/PacemakerSet/HardwareArticles/Hardware5.xml"), false);
			}
			this.os.branchMissions = list;
		}

		// Token: 0x06000748 RID: 1864 RVA: 0x0007724C File Offset: 0x0007544C
		public override void navigatedTo()
		{
			base.navigatedTo();
			if (this.isPublic)
			{
				this.state = 1;
			}
			else
			{
				this.state = 0;
			}
			this.ProgressionSaveFixHacks();
		}

		// Token: 0x06000749 RID: 1865 RVA: 0x00077288 File Offset: 0x00075488
		private void ProgressionSaveFixHacks()
		{
			if (this.groupName.Equals("Entropy"))
			{
				if (this.missions.Count == 0 && this.os.currentFaction.idName == "entropy" && this.os.currentMission == null)
				{
					if (!this.os.Flags.HasFlag("ThemeHackTransitionAssetsAdded"))
					{
						ComputerLoader.loadMission("Content/Missions/Entropy/ThemeHackTransitionMission.xml", false);
						this.os.saveGame();
					}
				}
			}
		}

		// Token: 0x0600074A RID: 1866 RVA: 0x00077326 File Offset: 0x00075526
		public override void loginGoBack()
		{
			base.loginGoBack();
			this.state = 0;
		}

		// Token: 0x0600074B RID: 1867 RVA: 0x00077338 File Offset: 0x00075538
		public override void userLoggedIn()
		{
			base.userLoggedIn();
			if (!this.user.name.Equals(""))
			{
				this.state = 1;
			}
			else
			{
				this.state = 0;
			}
		}

		// Token: 0x0600074C RID: 1868 RVA: 0x00077378 File Offset: 0x00075578
		public bool hasSysfile()
		{
			for (int i = 0; i < this.root.files.Count; i++)
			{
				if (this.root.files[i].name.Equals("config.sys"))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000773D8 File Offset: 0x000755D8
		public bool hasListingFile(string name)
		{
			name = name.Replace(" ", "_");
			for (int i = 0; i < this.missionFolder.files.Count; i++)
			{
				if (this.missionFolder.files[i].name == name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x00077448 File Offset: 0x00075648
		public void drawTopBar(Rectangle bounds, SpriteBatch sb)
		{
			this.panelRect = new Rectangle(bounds.X, bounds.Y, bounds.Width - this.corner.Width, this.topBar.Height);
			sb.Draw(this.topBar, this.panelRect, this.themeColor);
			sb.Draw(this.corner, new Vector2((float)(bounds.X + bounds.Width - this.corner.Width), (float)bounds.Y), this.themeColor);
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x000774E4 File Offset: 0x000756E4
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			PatternDrawer.draw(new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2), 0.28f, Color.Transparent, this.themeColor * 0.1f, sb, PatternDrawer.thinStripe);
			this.drawTopBar(bounds, sb);
			if (!this.hasSysfile())
			{
				if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
				{
					this.os.display.command = "connect";
				}
				PatternDrawer.draw(new Rectangle(bounds.X + 1, bounds.Y + 1 + 64, bounds.Width - 2, bounds.Height - 2 - 64), 1f, Color.Transparent, this.os.lockedColor, sb, PatternDrawer.errorTile);
				int num = bounds.X + 20;
				int num2 = bounds.Y + bounds.Height / 2;
				num2 -= 20;
				TextItem.doLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("CRITICAL ERROR"), null);
				num2 += 40;
				TextItem.doSmallLabel(new Vector2((float)num, (float)num2), "ERROR #4040408 - NULL_SYSFILE\nUnhandled Exception - IOException@L 2217 :R 28\nSystem Files Corrupted and/or Destroyed\nContact the System Administrator", null);
			}
			else
			{
				switch (this.state)
				{
				case 0:
				{
					if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
					{
						this.os.display.command = "connect";
					}
					sb.Draw(this.logo, new Rectangle(bounds.X + 30, bounds.Y + 115, 128, 128), Color.White);
					string text = string.IsNullOrWhiteSpace(this.listingTitle) ? (string.Format(LocaleTerms.Loc("{0} Group"), this.groupName) + "\n" + LocaleTerms.Loc("Message Board")) : this.listingTitle;
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 40 + 128), (float)(bounds.Y + 115)), text, GuiData.font, null, (float)(bounds.Width - 40), 60f, false);
					if (Button.doButton(800004, bounds.X + 30, bounds.Y + bounds.Height / 2, 300, 40, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
					{
						base.startLogin();
						this.state = 3;
					}
					break;
				}
				case 1:
				{
					if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
					{
						this.os.display.command = "connect";
					}
					int num = bounds.X + 10;
					int num2 = bounds.Y + this.topBar.Height + 50;
					this.logoRect.X = num;
					this.logoRect.Y = num2;
					sb.Draw(this.logo, this.logoRect, Color.White);
					num += this.logoRect.Width + 5;
					TextItem.doLabel(new Vector2((float)num, (float)num2), this.listingTitle, null);
					num2 += 40;
					for (int i = 0; i < this.missions.Count; i++)
					{
						if (this.hasListingFile(this.missions[i].postingTitle))
						{
							Rectangle rectangle = new Rectangle(num, num2, (int)((float)bounds.Width * 0.8f), 30);
							rectangle = Utils.InsetRectangle(rectangle, 1);
							rectangle.X += 12;
							rectangle.Width -= 12;
							if (this.missions[i].postingTitle.StartsWith("#"))
							{
								PatternDrawer.draw(rectangle, 1f, Color.Black * 1f, Color.DarkRed * 0.3f, sb, PatternDrawer.warningStripe);
							}
							if (Button.doButton(87654 + i, num, num2, (int)((float)bounds.Width * 0.8f), 30, this.missions[i].postingTitle, null))
							{
								this.state = 2;
								this.targetIndex = i;
							}
							num2 += 35;
						}
					}
					break;
				}
				case 2:
				{
					if (Button.doButton(800003, bounds.X + 10, bounds.Y + this.topBar.Height + 10, 300, 30, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
					{
						this.state = 1;
					}
					int num3 = 60;
					int num4 = 84;
					Rectangle destinationRectangle = new Rectangle(bounds.X + 30, bounds.Y + this.topBar.Height + num3, num4, num4);
					sb.Draw(this.logo, destinationRectangle, this.themeColor);
					num3 += 30;
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 34 + num4), (float)(bounds.Y + this.topBar.Height + num3)), this.missions[this.targetIndex].postingTitle, GuiData.font, null, (float)(bounds.Width - (36 + num4 + 6)), 40f, false);
					num3 += 40;
					Rectangle dest = new Rectangle(destinationRectangle.X + destinationRectangle.Width + 2, bounds.Y + this.topBar.Height + num3 - 8, bounds.Width - (destinationRectangle.X - bounds.X + destinationRectangle.Width + 10), PatternDrawer.warningStripe.Height / 2);
					PatternDrawer.draw(dest, 1f, Color.Transparent, this.themeColor, sb, PatternDrawer.warningStripe);
					num3 += 36;
					string text2 = Utils.SuperSmartTwimForWidth(this.missions[this.targetIndex].postingBody, bounds.Width - 60, GuiData.tinyfont);
					if (this.TextRegion == null)
					{
						this.TextRegion = new ScrollableTextRegion(sb.GraphicsDevice);
					}
					this.TextRegion.Draw(new Rectangle(bounds.X + 30, bounds.Y + this.topBar.Height + num3, bounds.Width - 50, bounds.Height - num3 - this.topBar.Height - 10), text2, sb);
					bool flag = this.os.currentFaction != null && this.os.currentFaction.idName.ToLower() == this.groupName.ToLower();
					if (this.missionAssigner && this.os.currentMission == null && flag && Button.doButton(800005, bounds.X + bounds.Width / 2 - 10, bounds.Y + bounds.Height - 35, bounds.Width / 2, 30, LocaleTerms.Loc("Accept"), new Color?(this.os.highlightColor)))
					{
						this.os.currentMission = this.missions[this.targetIndex];
						ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(this.missions[this.targetIndex].reloadGoalsSourceFile);
						this.missions[this.targetIndex].sendEmail(this.os);
						this.missions[this.targetIndex].ActivateSuppressedStartFunctionIfPresent();
						this.removeMission(this.targetIndex);
						this.state = 1;
					}
					else if (this.missionAssigner && this.os.currentMission != null)
					{
						if (this.os.currentMission.wasAutoGenerated && Button.doButton(8000105, bounds.X + 6, bounds.Y + bounds.Height - 29, 210, 25, LocaleTerms.Loc("Abandon Current Contract"), new Color?(this.os.lockedColor)))
						{
							this.os.currentMission = null;
							this.os.currentFaction.contractAbbandoned(this.os);
						}
						TextItem.doFontLabel(new Vector2((float)(bounds.X + 10), (float)(bounds.Y + bounds.Height - 52)), LocaleTerms.Loc("Mission Unavailable") + " : " + (flag ? LocaleTerms.Loc("Complete Existing Contracts") : (LocaleTerms.Loc("User ID Assigned to Different Faction") + " ")), GuiData.smallfont, null, (float)(bounds.Width - 20), 30f, false);
					}
					else if (this.missionAssigner && !flag)
					{
						TextItem.doFontLabel(new Vector2((float)(bounds.X + 10), (float)(bounds.Y + bounds.Height - 52)), LocaleTerms.Loc("Mission Unavailable") + " : " + LocaleTerms.Loc("User ID Assigned to Different Faction") + " ", GuiData.smallfont, null, (float)(bounds.Width - 20), 30f, false);
					}
					break;
				}
				case 3:
					base.doLoginDisplay(bounds, sb);
					break;
				}
			}
		}

		// Token: 0x04000819 RID: 2073
		private const int NEED_LOGIN = 0;

		// Token: 0x0400081A RID: 2074
		private const int BOARD = 1;

		// Token: 0x0400081B RID: 2075
		private const int MESSAGE = 2;

		// Token: 0x0400081C RID: 2076
		private const int LOGIN = 3;

		// Token: 0x0400081D RID: 2077
		private string groupName;

		// Token: 0x0400081E RID: 2078
		public string listingTitle;

		// Token: 0x0400081F RID: 2079
		private Color themeColor;

		// Token: 0x04000820 RID: 2080
		private Texture2D topBar;

		// Token: 0x04000821 RID: 2081
		private Texture2D corner;

		// Token: 0x04000822 RID: 2082
		private Texture2D logo;

		// Token: 0x04000823 RID: 2083
		private Rectangle panelRect;

		// Token: 0x04000824 RID: 2084
		private Rectangle logoRect;

		// Token: 0x04000825 RID: 2085
		private Folder root;

		// Token: 0x04000826 RID: 2086
		private Folder missionFolder;

		// Token: 0x04000827 RID: 2087
		private Folder closedMissionsFolder;

		// Token: 0x04000828 RID: 2088
		private List<int> rootPath;

		// Token: 0x04000829 RID: 2089
		private List<int> missionFolderPath;

		// Token: 0x0400082A RID: 2090
		private FileEntry sysFile;

		// Token: 0x0400082B RID: 2091
		private int targetIndex;

		// Token: 0x0400082C RID: 2092
		private ScrollableTextRegion TextRegion;

		// Token: 0x0400082D RID: 2093
		public bool missionAssigner = false;

		// Token: 0x0400082E RID: 2094
		public bool isPublic;

		// Token: 0x0400082F RID: 2095
		private int state;

		// Token: 0x04000830 RID: 2096
		private bool NeedsCustomFolderLoad = false;

		// Token: 0x04000831 RID: 2097
		private string CustomFolderLoadPath = null;

		// Token: 0x04000832 RID: 2098
		private string IconReloadPath = null;

		// Token: 0x04000833 RID: 2099
		private string ArticleFolderPath = null;

		// Token: 0x04000834 RID: 2100
		private bool HasCustomColor = false;

		// Token: 0x04000835 RID: 2101
		private List<ActiveMission> missions;

		// Token: 0x04000836 RID: 2102
		private List<List<ActiveMission>> branchMissions = new List<List<ActiveMission>>();
	}
}
