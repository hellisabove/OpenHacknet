using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Hacknet.Effects;
using Hacknet.Factions;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000105 RID: 261
	internal class MissionHubServer : AuthenticatingDaemon
	{
		// Token: 0x06000601 RID: 1537 RVA: 0x000626E4 File Offset: 0x000608E4
		public MissionHubServer(Computer c, string serviceName, string group, OS _os) : base(c, serviceName, _os)
		{
			this.groupName = group;
			this.decorationPanel = TextureBank.load("Sprites/HubDecoration", this.os.content);
			this.decorationPanelSide = TextureBank.load("Sprites/HubDecorationSide", this.os.content);
			this.lockIcon = TextureBank.load("Lock", this.os.content);
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0006281C File Offset: 0x00060A1C
		public override void initFiles()
		{
			this.root = new Folder("ContractHub");
			this.missionsFolder = new Folder("Contracts");
			this.listingsFolder = new Folder("Listings");
			this.listingArchivesFolder = new Folder("Archives");
			this.missionsFolder.folders.Add(this.listingsFolder);
			this.missionsFolder.folders.Add(this.listingArchivesFolder);
			this.usersFolder = new Folder("Users");
			this.root.folders.Add(this.missionsFolder);
			this.root.folders.Add(this.usersFolder);
			this.root.files.Add(this.generateConfigFile());
			this.root.files.Add(new FileEntry(Computer.generateBinaryString(1024), "net64.sys"));
			this.populateUserList();
			this.os.delayer.Post(ActionDelayer.NextTick(), delegate
			{
				this.loadInitialContracts();
			});
			this.comp.files.root.folders.Add(this.root);
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0006295C File Offset: 0x00060B5C
		private void populateUserList()
		{
			this.initializeUsers();
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x00062968 File Offset: 0x00060B68
		private void loadInitialContracts()
		{
			int num = 0;
			DirectoryInfo directoryInfo = new DirectoryInfo(this.MissionSourceFolderPath);
			FileInfo[] files = directoryInfo.GetFiles("*.xml");
			for (int i = 0; i < files.Length; i++)
			{
				string text = this.MissionSourceFolderPath + files[i].Name;
				try
				{
					this.addMission((ActiveMission)ComputerLoader.readMission(text), false, false, -1);
				}
				catch (Exception innerException)
				{
					FormatException ex = new FormatException("Error Loading Mission: " + text, innerException);
					throw ex;
				}
				num++;
			}
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00062A08 File Offset: 0x00060C08
		public void AddMissionToListings(string missionFilename, int desiredIndex = -1)
		{
			this.addMission((ActiveMission)ComputerLoader.readMission(missionFilename), true, false, desiredIndex);
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00062A20 File Offset: 0x00060C20
		public void RemoveMissionFromListings(string missionFilename)
		{
			List<ActiveMission> branchMissions = this.os.branchMissions;
			ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(Utils.GetFileLoadPrefix() + missionFilename);
			string text = null;
			foreach (KeyValuePair<string, ActiveMission> keyValuePair in this.listingMissions)
			{
				if (keyValuePair.Value.reloadGoalsSourceFile == activeMission.reloadGoalsSourceFile)
				{
					text = keyValuePair.Key;
					break;
				}
			}
			if (text != null)
			{
				this.listingMissions.Remove(text);
				for (int i = 0; i < this.listingsFolder.files.Count; i++)
				{
					if (this.listingsFolder.files[i].name.Contains("#" + text))
					{
						this.listingsFolder.files.RemoveAt(i);
						i--;
					}
				}
			}
			this.os.branchMissions = branchMissions;
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x00062B60 File Offset: 0x00060D60
		private void addMission(ActiveMission mission, bool insertAtTop = false, bool preventRegistryNumberChange = false, int desiredInsertionIndex = -1)
		{
			if (insertAtTop && desiredInsertionIndex <= -1)
			{
				desiredInsertionIndex = 0;
			}
			this.contractRegistryNumber += (int)(Utils.getRandomByte() + 1);
			this.listingMissions.Add(string.Concat(this.contractRegistryNumber), mission);
			FileEntry item = new FileEntry(MissionSerializer.generateMissionFile(mission, this.contractRegistryNumber, "CSEC", null), "Contract#" + this.contractRegistryNumber);
			if (desiredInsertionIndex < this.listingsFolder.files.Count && (insertAtTop || desiredInsertionIndex >= 0))
			{
				this.listingsFolder.files.Insert(desiredInsertionIndex, item);
			}
			else
			{
				this.listingsFolder.files.Add(item);
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x00062C30 File Offset: 0x00060E30
		private FileEntry generateConfigFile()
		{
			string text = "//Contract Hub Setup File\n";
			text = text + "ThemeColor = " + Utils.convertColorToParseableString(this.themeColor) + "\n";
			text = text + "ServiceName = " + this.name + "\n";
			text = text + "GroupName = " + this.groupName + "\n";
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"ContractListingIndexes = ",
				this.contractRegistryNumber,
				"\n"
			});
			text = text + "LineColor = " + Utils.convertColorToParseableString(this.themeColorLine) + "\n";
			text = text + "BackColor = " + Utils.convertColorToParseableString(this.themeColorBackground) + "\n";
			obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"EnableABN = ",
				this.allowAbandon,
				"\n"
			});
			text += "\n";
			return new FileEntry(text, "settings.sys");
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00062D48 File Offset: 0x00060F48
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
					else if (text.StartsWith("ContractListingIndexes"))
					{
						try
						{
							this.contractRegistryNumber = Convert.ToInt32(this.getDataFromConfigLine(text, "= "));
						}
						catch (FormatException)
						{
							this.contractRegistryNumber = 0;
						}
						catch (OverflowException)
						{
							this.contractRegistryNumber = 0;
						}
					}
					if (text.StartsWith("LineColor"))
					{
						this.themeColorLine = Utils.convertStringToColor(this.getDataFromConfigLine(text, "= "));
					}
					if (text.StartsWith("BackColor"))
					{
						this.themeColorBackground = Utils.convertStringToColor(this.getDataFromConfigLine(text, "= "));
					}
					if (text.StartsWith("EnableABN"))
					{
						this.allowAbandon = (this.getDataFromConfigLine(text, "= ").ToLower() == "true");
					}
				}
			}
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x00062F08 File Offset: 0x00061108
		private string getDataFromConfigLine(string line, string sentinel = "= ")
		{
			return line.Substring(line.IndexOf(sentinel) + 2);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00062F2C File Offset: 0x0006112C
		public override void loadInit()
		{
			base.loadInit();
			this.root = this.comp.files.root.searchForFolder("ContractHub");
			this.missionsFolder = this.root.searchForFolder("Contracts");
			this.listingsFolder = this.missionsFolder.searchForFolder("Listings");
			this.listingArchivesFolder = this.missionsFolder.searchForFolder("Archives");
			this.usersFolder = this.root.searchForFolder("Users");
			FileEntry fileEntry = this.root.searchForFile("settings.sys");
			if (fileEntry != null)
			{
				this.loadFromConfigFileData(fileEntry.data);
			}
			this.loadListingMissionsFromFiles();
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00062FE8 File Offset: 0x000611E8
		private void loadListingMissionsFromFiles()
		{
			for (int i = 0; i < this.listingsFolder.files.Count; i++)
			{
				string data = this.listingsFolder.files[i].data;
				string idstringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[i]);
				try
				{
					this.listingMissions.Add(idstringForContractFile, (ActiveMission)MissionSerializer.restoreMissionFromFile(data, out this.contractRegistryNumber));
				}
				catch (FormatException)
				{
				}
			}
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x00063080 File Offset: 0x00061280
		public override string getSaveString()
		{
			return "<MissionHubServer />";
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x00063098 File Offset: 0x00061298
		private void initializeUsers()
		{
			List<int> list = new List<int>();
			if (People.hubAgents == null)
			{
				People.init();
			}
			int num;
			string text;
			string nameEntry;
			for (int i = 0; i < People.hubAgents.Count; i++)
			{
				do
				{
					num = Utils.random.Next(9999);
				}
				while (list.Contains(num));
				text = "USER: " + num + "\n";
				text = text + "Handle: " + People.hubAgents[i].handle + "\n";
				text = text + "Date Joined : " + (DateTime.Now - TimeSpan.FromDays(Utils.random.NextDouble() * 200.0)).ToString().Replace('/', '-').Replace(' ', '_') + "\n";
				text = text + "Status : " + this.generateUserState(People.hubAgents[i].handle) + "\n";
				text = text + "Rank : " + this.generateUserRank(People.hubAgents[i].handle) + "\n";
				nameEntry = People.hubAgents[i].handle + "#" + num;
				FileEntry item = new FileEntry(text, nameEntry);
				this.usersFolder.files.Add(item);
			}
			do
			{
				num = Utils.random.Next(9999);
			}
			while (list.Contains(num));
			text = "USER: " + num + "\n";
			text += "Handle: Bit\n";
			text = text + "Date Joined : " + (DateTime.Now - TimeSpan.FromDays(411.0)).ToString().Replace('/', '-').Replace(' ', '_') + "\n";
			text = text + "Status : " + this.generateUserState("Bit") + "\n";
			text = text + "Rank : " + this.generateUserRank("Bit") + "\n";
			nameEntry = "Bit#" + num;
			FileEntry item2 = new FileEntry(text, nameEntry);
			this.usersFolder.files.Insert(0, item2);
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x00063328 File Offset: 0x00061528
		public void addUser(UserDetail newUser)
		{
			int num = Utils.random.Next(9999);
			string text = "USER: " + num + "\n";
			text = text + "Handle: " + newUser.name + "\n";
			text = text + "Date Joined : " + DateTime.Now.ToString().Replace('/', '-').Replace(' ', '_') + "\n";
			text += "Status : Active\n";
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"Rank : ",
				0,
				"\n"
			});
			string nameEntry = newUser.name + "#" + num;
			FileEntry item = new FileEntry(text, nameEntry);
			this.usersFolder.files.Add(item);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x00063420 File Offset: 0x00061620
		private string generateUserRank(string username)
		{
			if (username != null)
			{
				if (username == "Bit")
				{
					return "2116";
				}
			}
			return string.Concat((int)(Utils.random.NextDouble() * 2500.0));
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x00063470 File Offset: 0x00061670
		private string generateUserState(string username)
		{
			if (username != null)
			{
				if (username == "Bit")
				{
					return "UNKNOWN";
				}
			}
			string result;
			if (username.GetHashCode() < 1073741823)
			{
				result = "Active";
			}
			else
			{
				result = "Passive";
			}
			return result;
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x000634C4 File Offset: 0x000616C4
		public int GetNumberOfAvaliableMissions()
		{
			return this.listingMissions.Count;
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x000634E4 File Offset: 0x000616E4
		private void CheckForGameStateIssuesAndFix()
		{
			if (!Settings.IsInExtensionMode)
			{
				if (this.os.currentMission == null)
				{
					if (this.os.Flags.HasFlag("bitPathStarted") && !this.os.Flags.HasFlag("Victory"))
					{
						bool flag = false;
						foreach (KeyValuePair<string, ActiveMission> keyValuePair in this.listingMissions)
						{
							if (keyValuePair.Value.reloadGoalsSourceFile.Contains("BitPath/BitAdv_Recovery.xml"))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.AddMissionToListings("Content/Missions/BitPath/BitAdv_Recovery.xml", -1);
						}
					}
					if (DLC1SessionUpgrader.HasDLC1Installed && !this.os.Flags.HasFlag("dlc_complete"))
					{
						bool flag = false;
						foreach (KeyValuePair<string, ActiveMission> keyValuePair in this.listingMissions)
						{
							if (keyValuePair.Value.reloadGoalsSourceFile.Contains("CSEC_DLCConnectorIntro.xml"))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							this.AddMissionToListings("Content/DLC/Missions/BaseGameConnectors/Missions/CSEC_DLCConnectorIntro.xml", -1);
						}
					}
				}
				if (this.GetNumberOfAvaliableMissions() == 0 && this.os.currentMission == null && !this.os.Flags.HasFlag("bitPathStarted"))
				{
					foreach (KeyValuePair<string, Faction> keyValuePair2 in this.os.allFactions.factions)
					{
						if (keyValuePair2.Value.idName == "hub")
						{
							HubFaction hubFaction = keyValuePair2.Value as HubFaction;
							if (hubFaction != null)
							{
								hubFaction.ForceStartBitMissions(this.os);
							}
						}
					}
				}
				int num = 0;
				int num2 = 0;
				string text = null;
				foreach (KeyValuePair<string, ActiveMission> keyValuePair in this.listingMissions)
				{
					bool flag2 = true;
					if (keyValuePair.Value.reloadGoalsSourceFile.Contains("DLC"))
					{
						flag2 = false;
					}
					if (keyValuePair.Value.postingAcceptFlagRequirements != null)
					{
						for (int i = 0; i < keyValuePair.Value.postingAcceptFlagRequirements.Length; i++)
						{
							if (!this.os.Flags.HasFlag(keyValuePair.Value.postingAcceptFlagRequirements[i]))
							{
								flag2 = false;
								if (text == null)
								{
									text = keyValuePair.Key;
								}
							}
						}
					}
					if (flag2)
					{
						num++;
					}
					else
					{
						num2++;
					}
				}
				if (num <= 0 && num2 > 0)
				{
					if (this.os.currentMission == null && !this.os.Flags.HasFlag("bitPathStarted"))
					{
						foreach (KeyValuePair<string, Faction> keyValuePair2 in this.os.allFactions.factions)
						{
							if (keyValuePair2.Value.idName == "hub")
							{
								HubFaction hubFaction = keyValuePair2.Value as HubFaction;
								if (hubFaction != null)
								{
									hubFaction.ForceStartBitMissions(this.os);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x00063944 File Offset: 0x00061B44
		private string getIDStringForContractFile(FileEntry file)
		{
			int num = file.name.IndexOf('#');
			string result;
			if (num > -1)
			{
				num++;
				result = file.name.Substring(file.name.IndexOf('#') + 1);
			}
			else
			{
				result = "";
			}
			return result;
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x00063998 File Offset: 0x00061B98
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.screenTransition = 1f;
			this.state = MissionHubServer.HubState.Welcome;
			this.missionListPageNumber = 0;
			if (this.thinBarcodeTop != null)
			{
				this.thinBarcodeTop.regenerate();
			}
			if (this.thinBarcodeBot != null)
			{
				this.thinBarcodeBot.regenerate();
			}
			this.CheckForGameStateIssuesAndFix();
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x000639FF File Offset: 0x00061BFF
		public override void loginGoBack()
		{
			base.loginGoBack();
			this.state = MissionHubServer.HubState.Welcome;
			this.screenTransition = 1f;
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x00063A1B File Offset: 0x00061C1B
		public override void userLoggedIn()
		{
			base.userLoggedIn();
			this.activeUserName = this.user.name;
			this.state = MissionHubServer.HubState.Menu;
			this.screenTransition = 1f;
			this.activeUserLoginTime = DateTime.Now;
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x00063A54 File Offset: 0x00061C54
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.updateScreenTransition();
			switch (this.state)
			{
			case MissionHubServer.HubState.Welcome:
				this.doBarcodeEffect(bounds, sb);
				this.drawWelcomeScreen(bounds, sb);
				break;
			case MissionHubServer.HubState.Menu:
				this.doMenuScreen(bounds, sb);
				this.doLoggedInScreenDetailing(bounds, sb);
				break;
			case MissionHubServer.HubState.Login:
				this.doBarcodeEffect(bounds, sb);
				base.doLoginDisplay(bounds, sb);
				break;
			case MissionHubServer.HubState.Listing:
				this.doListingScreen(bounds, sb);
				break;
			case MissionHubServer.HubState.ContractPreview:
				this.doLoggedInScreenDetailing(bounds, sb);
				this.doContractPreviewScreen(bounds, sb);
				break;
			case MissionHubServer.HubState.UserList:
				this.doLoggedInScreenDetailing(bounds, sb);
				this.doUserListScreen(bounds, sb);
				break;
			case MissionHubServer.HubState.CancelContract:
				this.doCancelContractScreen(bounds, sb);
				this.doLoggedInScreenDetailing(bounds, sb);
				break;
			}
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x00063B28 File Offset: 0x00061D28
		private void updateScreenTransition()
		{
			this.screenTransition -= (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds / 0.3f;
			this.screenTransition = Math.Max(this.screenTransition, 0f);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x00063B78 File Offset: 0x00061D78
		private int getTransitionOffset(int position)
		{
			return (int)(Math.Pow(Math.Min((double)this.screenTransition + (double)position * 0.1, 1.0), 1.0) * 40.0 * (double)this.screenTransition);
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x00063BD0 File Offset: 0x00061DD0
		private void doMenuScreen(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 + 10, bounds.Width / 2, 40);
			if (Button.doButton(101010, rectangle.X + this.getTransitionOffset(0), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Contract Listing"), new Color?(this.themeColor)))
			{
				this.state = MissionHubServer.HubState.Listing;
				this.screenTransition = 1f;
			}
			rectangle.Y += rectangle.Height + 5;
			if (Button.doButton(101015, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("User List"), new Color?(this.themeColor)))
			{
				this.state = MissionHubServer.HubState.UserList;
				this.screenTransition = 1f;
			}
			rectangle.Y += rectangle.Height + 5;
			if (this.allowAbandon)
			{
				if (Button.doButton(101017, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height / 2, LocaleTerms.Loc("Abort Current Contract"), new Color?((this.os.currentMission == null) ? Color.Black : this.themeColor)))
				{
					if (this.os.currentMission != null)
					{
						this.state = MissionHubServer.HubState.CancelContract;
						this.screenTransition = 1f;
					}
				}
				rectangle.Y += rectangle.Height / 2 + 5;
			}
			if (Button.doButton(102015, rectangle.X + this.getTransitionOffset(3), rectangle.Y, rectangle.Width, rectangle.Height / 2, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
			{
				this.os.display.command = "connect";
			}
			rectangle.Y += rectangle.Height + 5;
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x00063E2C File Offset: 0x0006202C
		private void doCancelContractScreen(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 + 10, bounds.Width / 2, 40);
			TextItem.doFontLabel(new Vector2((float)rectangle.X, (float)rectangle.Y), LocaleTerms.Loc("Are you sure you with to abandon your current contract?") + "\n" + LocaleTerms.Loc("This cannot be reversed."), GuiData.font, new Color?(Color.White), (float)(bounds.Width - 30), (float)rectangle.Height, false);
			rectangle.Y += rectangle.Height + 4;
			if (Button.doButton(142011, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Abandon Contract"), new Color?(Color.Red)))
			{
				if (this.os.currentMission != null)
				{
					this.os.currentMission = null;
					this.os.currentFaction.contractAbbandoned(this.os);
					this.screenTransition = 0f;
					this.state = MissionHubServer.HubState.Menu;
					this.CheckForGameStateIssuesAndFix();
				}
			}
			rectangle.Y += rectangle.Height + 10;
			if (Button.doButton(142015, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
			{
				this.screenTransition = 1f;
				this.state = MissionHubServer.HubState.Menu;
			}
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x00063FE0 File Offset: 0x000621E0
		private void doListingScreen(Rectangle bounds, SpriteBatch sb)
		{
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			Rectangle rectangle = this.doListingScreenBackground(bounds, sb);
			Rectangle rectangle2 = new Rectangle(bounds.X + 10, rectangle.Y, bounds.Width / 2, 30);
			int num = 36;
			int num2 = this.missionListPageNumber * this.missionListDisplayed;
			while (num2 < this.listingsFolder.files.Count && rectangle2.Y + num < rectangle.Y + rectangle.Height)
			{
				string idstringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[num2]);
				if (this.listingMissions.ContainsKey(idstringForContractFile))
				{
					ActiveMission activeMission = this.listingMissions[idstringForContractFile];
					if (activeMission == null)
					{
						Console.WriteLine("Mission: " + idstringForContractFile + " is null! There is an error in one of your mission files preventing draw");
					}
					else
					{
						this.drawMissionEntry(new Rectangle(bounds.X + 1, rectangle2.Y, bounds.Width - 2, num), sb, activeMission, num2);
						rectangle2.Y += num;
					}
				}
				num2++;
			}
			int num3 = 0;
			rectangle2 = new Rectangle(bounds.X + 10, rectangle.Y, bounds.Width / 2, 30);
			while (rectangle2.Y + num < rectangle.Y + rectangle.Height)
			{
				rectangle2.Y += num;
				num3++;
			}
			this.missionListDisplayed = num3;
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x00064194 File Offset: 0x00062394
		private void drawMissionEntry(Rectangle bounds, SpriteBatch sb, ActiveMission mission, int index)
		{
			bool flag = false;
			if (mission.postingAcceptFlagRequirements != null)
			{
				for (int i = 0; i < mission.postingAcceptFlagRequirements.Length; i++)
				{
					if (!this.os.Flags.HasFlag(mission.postingAcceptFlagRequirements[i]))
					{
						flag = true;
					}
				}
			}
			if (this.os.currentFaction != null)
			{
				if (this.os.currentFaction.playerValue < mission.requiredRank)
				{
					flag = true;
				}
			}
			int num = index * 139284 + 984275 + index;
			bool outlineOnly = Button.outlineOnly;
			bool drawingOutline = Button.drawingOutline;
			Button.outlineOnly = true;
			Button.drawingOutline = false;
			if (GuiData.active == num)
			{
				sb.Draw(Utils.white, bounds, Color.Black);
			}
			else if (GuiData.hot == num)
			{
				sb.Draw(Utils.white, bounds, this.themeColor * 0.12f);
			}
			else
			{
				Color color = (index % 2 == 0) ? this.themeColorLine : this.themeColorBackground;
				if (flag)
				{
					color = Color.Lerp(color, Color.Gray, 0.25f);
				}
				sb.Draw(Utils.white, bounds, color);
			}
			bool flag2 = mission.postingTitle.StartsWith("#");
			if (flag2)
			{
				PatternDrawer.draw(bounds, 1f, Color.Transparent, Color.Black * 0.6f, sb);
			}
			if (flag)
			{
				Rectangle destinationRectangle = bounds;
				destinationRectangle.Height -= 6;
				destinationRectangle.Y += 3;
				destinationRectangle.X += bounds.Width - bounds.Height - 6;
				destinationRectangle.Width = destinationRectangle.Height;
				sb.Draw(this.lockIcon, destinationRectangle, Color.White * 0.2f);
			}
			if (!flag && Button.doButton(num, bounds.X, bounds.Y, bounds.Width, bounds.Height, "", new Color?(Color.Transparent)))
			{
				this.selectedElementIndex = index;
				this.state = MissionHubServer.HubState.ContractPreview;
				this.screenTransition = 1f;
			}
			string text = mission.postingTitle.Replace("#", "") ?? "";
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 1 + this.getTransitionOffset(index)), (float)(bounds.Y + 3)), text, GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			string text2 = string.Concat(new object[]
			{
				"Target: ",
				mission.target,
				" -- Client: ",
				mission.client,
				" -- Key: ",
				mission.generationKeys
			});
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 1), (float)(bounds.Y + bounds.Height - 16)), text2, GuiData.detailfont, new Color?(Color.White * 0.3f), (float)bounds.Width, 13f, false);
			bounds.Y += bounds.Height - 1;
			bounds.Height = 1;
			sb.Draw(Utils.white, bounds, this.themeColor * 0.2f);
			Button.outlineOnly = outlineOnly;
			Button.drawingOutline = drawingOutline;
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x00064554 File Offset: 0x00062754
		private Rectangle doListingScreenBackground(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle destinationRectangle = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
			sb.Draw(Utils.white, destinationRectangle, this.themeColorBackground);
			this.doLoggedInScreenDetailing(bounds, sb);
			destinationRectangle.Height = 5;
			destinationRectangle.Width = 0;
			destinationRectangle.Y += 12;
			if (this.thinBarcodeTop == null)
			{
				this.thinBarcodeTop = new ThinBarcode(bounds.Width - 4, 5);
			}
			if (this.thinBarcodeBot == null)
			{
				this.thinBarcodeBot = new ThinBarcode(bounds.Width - 4, 5);
			}
			this.thinBarcodeTop.Draw(sb, destinationRectangle.X, destinationRectangle.Y, this.themeColor);
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 20), (float)(destinationRectangle.Y + 10)), "MISSION LISTING", GuiData.titlefont, new Color?(this.themeColor), (float)(bounds.Width - 40), 35f, false);
			destinationRectangle.Y += 45;
			destinationRectangle.X = bounds.X + 1;
			this.thinBarcodeBot.Draw(sb, destinationRectangle.X, destinationRectangle.Y, this.themeColor);
			destinationRectangle.Y += 10;
			destinationRectangle.X = bounds.X + 1;
			destinationRectangle.Width = this.decorationPanel.Width;
			destinationRectangle.Height = this.decorationPanel.Height;
			sb.Draw(this.decorationPanel, destinationRectangle, this.themeColor);
			destinationRectangle.X += destinationRectangle.Width;
			destinationRectangle.Width = bounds.Width - 2 - destinationRectangle.Width;
			sb.Draw(this.decorationPanelSide, destinationRectangle, this.themeColor);
			Vector2 pos = new Vector2((float)(bounds.X + 6), (float)(destinationRectangle.Y + this.decorationPanel.Height / 2 - 8));
			float num = 0.3f;
			int num2 = (int)((float)this.decorationPanel.Height * num);
			float num3 = ((float)this.decorationPanel.Height - (float)num2) / 2f;
			Rectangle destinationRectangle2 = new Rectangle(bounds.X + 1, (int)((float)destinationRectangle.Y + num3), this.decorationPanel.Width, num2);
			Rectangle value = new Rectangle(0, (int)(((float)this.decorationPanel.Height - (float)num2) / 2f), this.decorationPanel.Width, num2);
			bool outlineOnly = Button.outlineOnly;
			bool drawingOutline = Button.drawingOutline;
			Button.outlineOnly = true;
			Button.drawingOutline = false;
			int num4 = 974748322;
			Color color = (GuiData.active == num4) ? Color.Black : ((GuiData.hot == num4) ? this.themeColorLine : this.themeColorBackground);
			sb.Draw(this.decorationPanel, destinationRectangle2, new Rectangle?(value), color, 0f, Vector2.Zero, SpriteEffects.None, 0.6f);
			if (Button.doButton(num4, bounds.X + 1, destinationRectangle.Y + this.decorationPanel.Height / 6 + 4, this.decorationPanel.Width - 30, (int)((float)this.decorationPanel.Height - (float)(this.decorationPanel.Height / 6) * 3.2f), LocaleTerms.Loc("Back"), new Color?(Color.Transparent)))
			{
				this.screenTransition = 1f;
				this.state = MissionHubServer.HubState.Menu;
			}
			Button.outlineOnly = outlineOnly;
			Button.drawingOutline = drawingOutline;
			pos.X += (float)(this.decorationPanel.Width - 10);
			pos.Y += 4f;
			string text = LocaleTerms.Loc(this.groupName + " secure contract listing panel : Verified Connection : Token last verified") + " " + DateTime.Now.ToString();
			TextItem.doFontLabel(pos, text, GuiData.detailfont, new Color?(this.themeColor), (float)destinationRectangle.Width - 10f, 14f, false);
			pos.Y += 12f;
			if (this.missionListPageNumber > 0)
			{
				if (Button.doButton(188278101, (int)pos.X, (int)pos.Y, 45, 20, "<", null))
				{
					this.missionListPageNumber--;
					this.screenTransition = 1f;
				}
			}
			destinationRectangle.X += 50;
			int num5 = this.listingsFolder.files.Count / this.missionListDisplayed + 1;
			int num6 = this.missionListPageNumber + 1;
			string text2 = num6 + "/" + num5;
			float num7 = 50f - GuiData.smallfont.MeasureString(text2).X / 2f;
			sb.DrawString(GuiData.smallfont, text2, new Vector2((float)destinationRectangle.X + num7, (float)((int)pos.Y + 1)), Color.White);
			destinationRectangle.X += 100;
			if (this.missionListPageNumber < num5 - 1)
			{
				if (Button.doButton(188278102, destinationRectangle.X, (int)pos.Y, 45, 20, ">", null))
				{
					this.missionListPageNumber++;
					this.screenTransition = 1f;
				}
			}
			destinationRectangle.Y += this.decorationPanel.Height + 4;
			destinationRectangle.Width = bounds.Width - 2;
			destinationRectangle.X = bounds.X + 1;
			destinationRectangle.Height = 7;
			sb.Draw(Utils.white, destinationRectangle, this.themeColor);
			destinationRectangle.Y += destinationRectangle.Height;
			Rectangle result = new Rectangle(bounds.X, destinationRectangle.Y, bounds.Width, bounds.Height - bounds.Height / 12 - (destinationRectangle.Y - bounds.Y));
			return result;
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x00064BE0 File Offset: 0x00062DE0
		private void doContractPreviewScreen(Rectangle bounds, SpriteBatch sb)
		{
			string idstringForContractFile = this.getIDStringForContractFile(this.listingsFolder.files[this.selectedElementIndex]);
			if (this.listingMissions.ContainsKey(idstringForContractFile))
			{
				ActiveMission activeMission = this.listingMissions[idstringForContractFile];
				Vector2 value = new Vector2((float)(bounds.X + 20), (float)(bounds.Y + 20));
				TextItem.doFontLabel(value + new Vector2((float)this.getTransitionOffset(0), 0f), "CONTRACT:" + idstringForContractFile, GuiData.titlefont, null, (float)(bounds.Width / 2), 40f, false);
				value.Y += 40f;
				TextItem.doFontLabel(value + new Vector2((float)this.getTransitionOffset(1), 0f), activeMission.postingTitle.Replace("#", ""), GuiData.font, null, (float)(bounds.Width - 30), float.MaxValue, false);
				value.Y += 30f;
				string text = DisplayModule.cleanSplitForWidth(activeMission.postingBody, bounds.Width - 110);
				StringBuilder stringBuilder = new StringBuilder();
				int num = (int)(((float)bounds.Width - 20f) / GuiData.smallfont.MeasureString("-").X / 2f);
				for (int i = 1; i < num - 5; i++)
				{
					stringBuilder.Append("-");
				}
				text = text.Replace("###", stringBuilder.ToString());
				if (LocaleActivator.ActiveLocaleIsCJK())
				{
					text = Utils.SuperSmartTwimForWidth(activeMission.postingBody, bounds.Width - 110, GuiData.smallfont);
					value.Y += 20f;
				}
				TextItem.doFontLabel(value + new Vector2((float)this.getTransitionOffset(2), 0f), text, GuiData.smallfont, null, (float)(bounds.Width - 20), float.MaxValue, false);
				int num2 = Math.Max(135, bounds.Height / 6);
				if (Button.doButton(2171618, bounds.X + 20 + this.getTransitionOffset(3), bounds.Y + bounds.Height - num2, bounds.Width / 5, 30, LocaleTerms.Loc("Back"), null))
				{
					this.state = MissionHubServer.HubState.Listing;
					this.screenTransition = 1f;
				}
				if (this.os.currentMission == null)
				{
					if (Button.doButton(2171615, bounds.X + 20 + this.getTransitionOffset(4), bounds.Y + bounds.Height - num2 - 40, bounds.Width / 5, 30, LocaleTerms.Loc("Accept"), new Color?(this.os.highlightColor)))
					{
						this.acceptMission(activeMission, this.selectedElementIndex, idstringForContractFile);
						this.state = MissionHubServer.HubState.Listing;
						this.screenTransition = 1f;
					}
				}
				else
				{
					TextItem.doFontLabelToSize(new Rectangle(bounds.X + 20 + this.getTransitionOffset(4), bounds.Y + bounds.Height - num2 - 40, bounds.Width / 2, 30), LocaleTerms.Loc("Abort current contract to accept new ones."), GuiData.smallfont, Color.White, false, false);
				}
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x00064F88 File Offset: 0x00063188
		private void doUserListScreen(Rectangle bounds, SpriteBatch sb)
		{
			if (Button.doButton(101801, bounds.X + 2, bounds.Y + 20, bounds.Width / 4, 22, LocaleTerms.Loc("Back"), new Color?(this.themeColor)))
			{
				this.state = MissionHubServer.HubState.Menu;
			}
			Rectangle rectangle = new Rectangle(bounds.X + 30, bounds.Y + 50, bounds.Width / 2, 30);
			int num = 18 + rectangle.Height - 8;
			int num2 = bounds.Height - 90;
			int num3 = num2 / num;
			int num4 = num3 * this.userListPageNumber;
			int num5 = 0;
			int num6 = num3 * this.userListPageNumber;
			while (num6 < this.usersFolder.files.Count && num6 < num4 + num3)
			{
				string[] array = this.usersFolder.files[num6].data.Split(Utils.newlineDelim);
				string text3;
				string arg;
				string text2;
				string text = text2 = (arg = (text3 = ""));
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].StartsWith("USER"))
					{
						text2 = this.getDataFromConfigLine(array[i], ": ");
					}
					if (array[i].StartsWith("Rank"))
					{
						text3 = this.getDataFromConfigLine(array[i], ": ");
					}
					if (array[i].StartsWith("Handle"))
					{
						text = this.getDataFromConfigLine(array[i], ": ");
					}
					if (array[i].StartsWith("Date"))
					{
						arg = this.getDataFromConfigLine(array[i], ": ");
					}
				}
				Rectangle destinationRectangle = new Rectangle(rectangle.X + (bounds.Width - 60) - 10, rectangle.Y + 2, rectangle.Height + 1, bounds.Width - 60);
				GuiData.spriteBatch.Draw(Utils.gradient, destinationRectangle, null, this.themeColorLine, 1.5707964f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.8f);
				TextItem.doSmallLabel(new Vector2((float)rectangle.X, (float)rectangle.Y), string.Concat(new string[]
				{
					"#",
					text2,
					" - \"",
					text,
					"\" Rank:",
					text3
				}), null);
				rectangle.Y += 18;
				TextItem.doFontLabel(new Vector2((float)rectangle.X, (float)rectangle.Y), string.Format(LocaleTerms.Loc("Joined {0}"), arg), GuiData.detailfont, null, float.MaxValue, float.MaxValue, false);
				rectangle.Y += rectangle.Height - 10;
				num5 = num6;
				num6++;
			}
			rectangle.Y += 16;
			if (this.userListPageNumber > 0 && Button.doButton(101005, rectangle.X, rectangle.Y, rectangle.Width / 2 - 20, 15, LocaleTerms.Loc("Previous Page"), null))
			{
				this.userListPageNumber--;
			}
			TextItem.doTinyLabel(new Vector2((float)(rectangle.X + rectangle.Width / 2 - 8), (float)rectangle.Y), string.Concat(this.userListPageNumber), null);
			if (this.usersFolder.files.Count > num5 + 1 && Button.doButton(101010, rectangle.X + rectangle.Width / 2 + 10, rectangle.Y, rectangle.Width / 2 - 10, 15, LocaleTerms.Loc("Next Page"), null))
			{
				this.userListPageNumber++;
			}
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x000653DC File Offset: 0x000635DC
		private void acceptMission(ActiveMission mission, int index, string id)
		{
			this.os.currentMission = mission;
			ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(mission.reloadGoalsSourceFile);
			mission.sendEmail(this.os);
			mission.ActivateSuppressedStartFunctionIfPresent();
			FileEntry fileEntry = this.listingsFolder.files[index];
			this.listingsFolder.files.RemoveAt(index);
			string str = string.Concat(new string[]
			{
				"Contract Archive:\nAccepted : ",
				DateTime.Now.ToString(),
				"\nUser : ",
				this.activeUserName,
				"\nActive Since : ",
				this.activeUserLoginTime.ToString()
			});
			this.listingArchivesFolder.files.Add(new FileEntry(str + "\n\n" + fileEntry.data, "Contract#" + id + "Archive"));
			this.listingMissions.Remove(id);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x000654E0 File Offset: 0x000636E0
		private void drawWelcomeScreen(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 3 - 10, bounds.Width / 2, 30);
			string text = string.Format("{0} Contract Hub", this.groupName);
			text = text.ToUpper();
			TextItem.doFontLabel(new Vector2((float)rectangle.X, (float)rectangle.Y), text, GuiData.titlefont, null, (float)bounds.Width / 0.6f, 50f, false);
			rectangle.Y += 50;
			if (Button.doButton(11005, rectangle.X + this.getTransitionOffset(0), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
			{
				base.startLogin();
				this.state = MissionHubServer.HubState.Login;
			}
			rectangle.Y += rectangle.Height + 5;
			if (Button.doButton(12010, rectangle.X + this.getTransitionOffset(1), rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x00065658 File Offset: 0x00063858
		private void doBarcodeEffect(Rectangle bounds, SpriteBatch sb)
		{
			if (this.barcode == null || this.barcode.maxWidth != bounds.Width - 2 || this.barcode.leftRightBias)
			{
				this.barcode = new BarcodeEffect(bounds.Width - 2, true, false);
			}
			this.barcode.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			this.barcode.Draw(bounds.X + 1, bounds.Y + 5 * (bounds.Height / 6) - 1, bounds.Width - 2, bounds.Height / 6, sb, new Color?(this.themeColor));
			this.barcode.isInverted = false;
			this.barcode.Draw(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height / 6, sb, new Color?(this.themeColor));
			this.barcode.isInverted = true;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x00065774 File Offset: 0x00063974
		private void doLoggedInScreenDetailing(Rectangle bounds, SpriteBatch sb)
		{
			string text = string.Concat(new string[]
			{
				LocaleTerms.Loc("Authenticated User:"),
				" ",
				this.activeUserName,
				" -- Token Active Since: ",
				this.activeUserLoginTime.ToString()
			});
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 1), (float)(bounds.Y + 1)), text, GuiData.detailfont, null, (float)(bounds.Width - 2), float.MaxValue, false);
			this.doBaseBarcodeEffect(bounds, sb);
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x00065814 File Offset: 0x00063A14
		private void doBaseBarcodeEffect(Rectangle bounds, SpriteBatch sb)
		{
			if (this.barcode == null || this.barcode.maxWidth != bounds.Width - 2 || !this.barcode.leftRightBias)
			{
				this.barcode = new BarcodeEffect(bounds.Width - 2, true, true);
			}
			this.barcode.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			int y = bounds.Y + 11 * (bounds.Height / 12) - 1;
			this.barcode.Draw(bounds.X + 1, y, bounds.Width - 2, bounds.Height / 12, sb, new Color?(this.themeColor));
		}

		// Token: 0x040006BA RID: 1722
		public const string ROOT_FOLDERNAME = "ContractHub";

		// Token: 0x040006BB RID: 1723
		public const string CONFIG_FILENAME = "settings.sys";

		// Token: 0x040006BC RID: 1724
		public const string CRITICAL_FILE_FILENAME = "net64.sys";

		// Token: 0x040006BD RID: 1725
		public const double BUTTON_TRANSITION_OFFSET = 40.0;

		// Token: 0x040006BE RID: 1726
		public const float TRANSITION_TIME = 0.3f;

		// Token: 0x040006BF RID: 1727
		public const double TRANSITION_ELEMENT_INCREASE = 0.1;

		// Token: 0x040006C0 RID: 1728
		public Color themeColor = Color.PaleTurquoise;

		// Token: 0x040006C1 RID: 1729
		public Color themeColorBackground = new Color(10, 15, 25, 200);

		// Token: 0x040006C2 RID: 1730
		public Color themeColorLine = new Color(20, 25, 45, 200);

		// Token: 0x040006C3 RID: 1731
		private Folder root;

		// Token: 0x040006C4 RID: 1732
		private Folder missionsFolder;

		// Token: 0x040006C5 RID: 1733
		private Folder usersFolder;

		// Token: 0x040006C6 RID: 1734
		private Folder listingsFolder;

		// Token: 0x040006C7 RID: 1735
		private Folder listingArchivesFolder;

		// Token: 0x040006C8 RID: 1736
		private string groupName = "UNKNOWN";

		// Token: 0x040006C9 RID: 1737
		private int contractRegistryNumber = 256;

		// Token: 0x040006CA RID: 1738
		private Dictionary<string, ActiveMission> listingMissions = new Dictionary<string, ActiveMission>();

		// Token: 0x040006CB RID: 1739
		private Texture2D decorationPanel;

		// Token: 0x040006CC RID: 1740
		private Texture2D decorationPanelSide;

		// Token: 0x040006CD RID: 1741
		private Texture2D lockIcon;

		// Token: 0x040006CE RID: 1742
		public string MissionSourceFolderPath = "Content/Missions/MainHub/FirstSet/";

		// Token: 0x040006CF RID: 1743
		private BarcodeEffect barcode;

		// Token: 0x040006D0 RID: 1744
		private ThinBarcode thinBarcodeTop;

		// Token: 0x040006D1 RID: 1745
		private ThinBarcode thinBarcodeBot;

		// Token: 0x040006D2 RID: 1746
		public bool allowAbandon = true;

		// Token: 0x040006D3 RID: 1747
		private MissionHubServer.HubState state = MissionHubServer.HubState.Welcome;

		// Token: 0x040006D4 RID: 1748
		private string activeUserName = "UNKNOWN USER";

		// Token: 0x040006D5 RID: 1749
		private DateTime activeUserLoginTime = DateTime.Now;

		// Token: 0x040006D6 RID: 1750
		private float screenTransition = 0f;

		// Token: 0x040006D7 RID: 1751
		private int selectedElementIndex = 0;

		// Token: 0x040006D8 RID: 1752
		private int userListPageNumber = 0;

		// Token: 0x040006D9 RID: 1753
		private int missionListPageNumber = 0;

		// Token: 0x040006DA RID: 1754
		private int missionListDisplayed = 1;

		// Token: 0x040006DB RID: 1755
		private float timeSpentInLoading = 0f;

		// Token: 0x02000106 RID: 262
		private enum HubState
		{
			// Token: 0x040006DD RID: 1757
			Welcome,
			// Token: 0x040006DE RID: 1758
			Menu,
			// Token: 0x040006DF RID: 1759
			Login,
			// Token: 0x040006E0 RID: 1760
			Listing,
			// Token: 0x040006E1 RID: 1761
			ContractPreview,
			// Token: 0x040006E2 RID: 1762
			UserList,
			// Token: 0x040006E3 RID: 1763
			CancelContract
		}
	}
}
