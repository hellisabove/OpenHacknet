using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Misc;
using Hacknet.PlatformAPI.Storage;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;

namespace Hacknet.Screens
{
	// Token: 0x020000F0 RID: 240
	public class ExtensionsMenuScreen
	{
		// Token: 0x06000516 RID: 1302 RVA: 0x0004F5DC File Offset: 0x0004D7DC
		public ExtensionsMenuScreen()
		{
			SavefileLoginScreen saveScreen = this.SaveScreen;
			saveScreen.StartNewGameForUsernameAndPass = (Action<string, string>)Delegate.Combine(saveScreen.StartNewGameForUsernameAndPass, new Action<string, string>(delegate(string username, string pass)
			{
				OS.WillLoadSave = false;
				if (this.CreateNewAccountForExtension_UserAndPass != null)
				{
					this.CreateNewAccountForExtension_UserAndPass(username, pass);
				}
			}));
			SavefileLoginScreen saveScreen2 = this.SaveScreen;
			saveScreen2.LoadGameForUserFileAndUsername = (Action<string, string>)Delegate.Combine(saveScreen2.LoadGameForUserFileAndUsername, new Action<string, string>(delegate(string filename, string username)
			{
				OS.WillLoadSave = true;
				if (this.LoadAccountForExtension_FileAndUsername != null)
				{
					this.LoadAccountForExtension_FileAndUsername(filename, username);
				}
			}));
			SavefileLoginScreen saveScreen3 = this.SaveScreen;
			saveScreen3.RequestGoBack = (Action)Delegate.Combine(saveScreen3.RequestGoBack, new Action(delegate()
			{
				this.State = ExtensionsMenuScreen.EMSState.Normal;
			}));
			this.SaveScreen.DrawFromTop = true;
			if (Settings.AllowExtensionPublish)
			{
				this.workshopPublishScreen = new SteamWorkshopPublishScreen(Game1.getSingleton().Content);
				SteamWorkshopPublishScreen steamWorkshopPublishScreen = this.workshopPublishScreen;
				steamWorkshopPublishScreen.GoBack = (Action)Delegate.Combine(steamWorkshopPublishScreen.GoBack, new Action(delegate()
				{
					this.IsInPublishScreen = false;
				}));
			}
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0004F744 File Offset: 0x0004D944
		private void LoadExtensions()
		{
			if (!Directory.Exists("Extensions/"))
			{
				Directory.CreateDirectory("Extensions/");
			}
			string[] directories = Directory.GetDirectories("Extensions/");
			for (int i = 0; i < directories.Length; i++)
			{
				this.AddExtensionSafe(directories[i]);
			}
			this.DefaultModImage = Game1.getSingleton().Content.Load<Texture2D>("Sprites/Hammer");
			if (PlatformAPISettings.Running)
			{
				PublishedFileId_t[] array = new PublishedFileId_t[200];
				uint subscribedItems = SteamUGC.GetSubscribedItems(array, 200U);
				Console.WriteLine(subscribedItems);
				if (array != null)
				{
					int i = 0;
					while ((long)i < (long)((ulong)subscribedItems))
					{
						uint itemState = SteamUGC.GetItemState(array[i]);
						EItemState eitemState = (EItemState)Enum.Parse(typeof(EItemState), string.Concat(itemState));
						if (eitemState.HasFlag(EItemState.k_EItemStateInstalled))
						{
							ulong num = 0UL;
							string text = null;
							uint cchFolderSize = 20000U;
							uint num2 = 0U;
							Console.WriteLine((SteamUGC.GetItemInstallInfo(array[i], out num, out text, cchFolderSize, out num2) ? "Installed" : "Uninstalled") + " Extension at: " + text);
							this.AddExtensionSafe(text);
						}
						i++;
					}
				}
			}
			this.HasLoaded = true;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0004F8BF File Offset: 0x0004DABF
		public void Reset()
		{
			this.State = ExtensionsMenuScreen.EMSState.Normal;
			this.ScrollStartIndex = 0;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0004F8D0 File Offset: 0x0004DAD0
		private void AddExtensionSafe(string folderpath)
		{
			if (ExtensionInfo.ExtensionExists(folderpath))
			{
				try
				{
					this.Extensions.Add(ExtensionInfo.ReadExtensionInfo(folderpath));
				}
				catch (Exception ex)
				{
					Console.WriteLine(Utils.GenerateReportFromException(ex));
					string loadErrors = this.LoadErrors;
					this.LoadErrors = string.Concat(new string[]
					{
						loadErrors,
						LocaleTerms.Loc("Error loading ExtensionInfo for"),
						" ",
						folderpath,
						"\n",
						LocaleTerms.Loc("Error details written to"),
						" ",
						folderpath.Replace("\\", "/"),
						"/error_report.txt\n\n"
					});
					string data = "Error loading ExtensionInfo for extension in folder:\n" + folderpath + "\nError details: \n" + Utils.GenerateReportFromException(ex);
					Utils.writeToFile(data, folderpath + "/error_report.txt");
				}
			}
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0004F9D0 File Offset: 0x0004DBD0
		private void EnsureHasLoaded()
		{
			if (!this.HasStartedLoading)
			{
				new Thread(new ThreadStart(this.LoadExtensions))
				{
					IsBackground = true
				}.Start();
				this.HasStartedLoading = true;
			}
		}

		// Token: 0x0600051B RID: 1307 RVA: 0x0004FA12 File Offset: 0x0004DC12
		public void ShowError(string error)
		{
			this.ReportOverride = error;
			this.State = ExtensionsMenuScreen.EMSState.Normal;
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0004FA24 File Offset: 0x0004DC24
		private void ActivateExtensionPage(ExtensionInfo info)
		{
			LocaleActivator.ActivateLocale(info.Language, Game1.getSingleton().Content);
			this.ExtensionInfoToShow = info;
			this.ReportOverride = null;
			this.SaveScreen.ProjectName = info.Name;
			Settings.IsInExtensionMode = true;
			ExtensionLoader.ActiveExtensionInfo = info;
			SaveFileManager.Init(true);
			this.SaveScreen.ResetForNewAccount();
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0004FA88 File Offset: 0x0004DC88
		private void ExitExtensionsScreen()
		{
			this.ExtensionInfoToShow = null;
			Settings.IsInExtensionMode = false;
			SaveFileManager.Init(true);
			if (this.ExitClicked != null)
			{
				this.ExitClicked();
			}
		}

		// Token: 0x0600051E RID: 1310 RVA: 0x0004FAC4 File Offset: 0x0004DCC4
		public void Draw(Rectangle dest, SpriteBatch sb, ScreenManager screenman)
		{
			this.EnsureHasLoaded();
			Rectangle rectangle = new Rectangle(dest.X, dest.Y, dest.Width, 40);
			Color value = Color.Lerp(Utils.AddativeWhite, Utils.AddativeRed, 0.3f + Utils.randm(0.22f));
			SpriteFont font = GuiData.font;
			Vector2 vector = new Vector2((float)dest.X, (float)dest.Y + 44f);
			string original = "E X T E N S I O N S";
			GuiData.spriteBatch.DrawString(font, Utils.FlipRandomChars(original, 0.017999999225139618), vector, Utils.AddativeWhite * (0.17f + Utils.randm(0.04f)));
			GuiData.spriteBatch.DrawString(font, Utils.FlipRandomChars(original, 0.004000000189989805), vector, value * (0.8f + Utils.randm(0.04f)));
			vector.Y += 35f;
			if (this.ExtensionInfoToShow == null)
			{
				vector = this.DrawExtensionList(vector, dest, sb);
			}
			else
			{
				switch (this.State)
				{
				default:
					vector = this.DrawExtensionInfoDetail(vector, dest, sb, screenman, this.ExtensionInfoToShow);
					break;
				case ExtensionsMenuScreen.EMSState.GetUsername:
				case ExtensionsMenuScreen.EMSState.ShowAccounts:
				{
					Rectangle dest2 = new Rectangle(dest.X, dest.Y + dest.Height / 4, dest.Width, (int)((float)dest.Height * 0.8f));
					vector = this.DrawExtensionCreateNewUserOrLoadScreen(vector, dest2, sb, screenman, this.ExtensionInfoToShow);
					break;
				}
				}
			}
			if (Button.doButton(391481, (int)vector.X, (int)vector.Y, 450, 25, LocaleTerms.Loc("Return to Main Menu"), new Color?(MainMenu.exitButtonColor)))
			{
				this.ExitExtensionsScreen();
			}
		}

		// Token: 0x0600051F RID: 1311 RVA: 0x0004FCA4 File Offset: 0x0004DEA4
		private Vector2 DrawExtensionCreateNewUserOrLoadScreen(Vector2 drawpos, Rectangle dest, SpriteBatch sb, ScreenManager screenMan, ExtensionInfo info)
		{
			this.SaveScreen.Draw(sb, dest);
			return drawpos;
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0004FCC8 File Offset: 0x0004DEC8
		private Vector2 DrawExtensionInfoDetail(Vector2 drawpos, Rectangle dest, SpriteBatch sb, ScreenManager screenMan, ExtensionInfo info)
		{
			sb.DrawString(GuiData.titlefont, info.Name.ToUpper(), drawpos, Utils.AddativeWhite * 0.66f);
			drawpos.Y += 80f;
			int height = sb.GraphicsDevice.Viewport.Height;
			int num = 256;
			if (height < 900)
			{
				num = 120;
			}
			Rectangle dest2 = new Rectangle((int)drawpos.X, (int)drawpos.Y, num, num);
			Texture2D texture = this.DefaultModImage;
			if (info.LogoImage != null)
			{
				texture = info.LogoImage;
			}
			FlickeringTextEffect.DrawFlickeringSprite(sb, dest2, texture, 2f, 0.5f, null, Color.White);
			Vector2 position = drawpos + new Vector2((float)num + 40f, 20f);
			float num2 = (float)dest.Width - (drawpos.X - (float)dest.X);
			string description = info.Description;
			string text = Utils.SuperSmartTwimForWidth(description, (int)num2, GuiData.smallfont);
			sb.DrawString(GuiData.smallfont, text, position, Utils.AddativeWhite * 0.7f);
			drawpos.Y += (float)num + 10f;
			Vector2 result;
			if (this.IsInPublishScreen)
			{
				Rectangle fullscreen = Utils.GetFullscreen();
				Rectangle dest3 = new Rectangle((int)drawpos.X, (int)drawpos.Y, fullscreen.Width - (int)drawpos.X * 2, fullscreen.Height - ((int)drawpos.Y + 50));
				result = this.workshopPublishScreen.Draw(sb, dest3, info);
			}
			else
			{
				if (this.ReportOverride != null)
				{
					string text2 = Utils.SuperSmartTwimForWidth(this.ReportOverride, 800, GuiData.smallfont);
					sb.DrawString(GuiData.smallfont, text2, drawpos + new Vector2(460f, 0f), (this.ReportOverride.Length > 250) ? Utils.AddativeRed : Utils.AddativeWhite);
				}
				int num3 = 40;
				int num4 = 5;
				int num5 = info.AllowSave ? 4 : 2;
				int num6 = height - (int)drawpos.Y - 55;
				num3 = Math.Min(num3, (num6 - num5 * num4) / num5);
				if (Button.doButton(7900010, (int)drawpos.X, (int)drawpos.Y, 450, num3, string.Format(LocaleTerms.Loc("New {0} Account"), info.Name), new Color?(MainMenu.buttonColor)))
				{
					this.State = ExtensionsMenuScreen.EMSState.GetUsername;
					this.SaveScreen.ResetForNewAccount();
				}
				drawpos.Y += (float)(num3 + num4);
				if (info.AllowSave)
				{
					bool flag = !string.IsNullOrWhiteSpace(SaveFileManager.LastLoggedInUser.FileUsername);
					if (Button.doButton(7900019, (int)drawpos.X, (int)drawpos.Y, 450, num3, flag ? ("Continue Account : " + SaveFileManager.LastLoggedInUser.Username) : " - No Accounts - ", new Color?(flag ? MainMenu.buttonColor : Color.Black)))
					{
						OS.WillLoadSave = true;
						if (this.LoadAccountForExtension_FileAndUsername != null)
						{
							this.LoadAccountForExtension_FileAndUsername(SaveFileManager.LastLoggedInUser.FileUsername, SaveFileManager.LastLoggedInUser.Username);
						}
					}
					drawpos.Y += (float)(num3 + num4);
					if (Button.doButton(7900020, (int)drawpos.X, (int)drawpos.Y, 450, num3, LocaleTerms.Loc("Login") + "...", new Color?(flag ? MainMenu.buttonColor : Color.Black)))
					{
						this.State = ExtensionsMenuScreen.EMSState.ShowAccounts;
						this.SaveScreen.ResetForLogin();
					}
					drawpos.Y += (float)(num3 + num4);
				}
				if (Button.doButton(7900030, (int)drawpos.X, (int)drawpos.Y, 450, num3, LocaleTerms.Loc("Run Verification Tests"), new Color?(MainMenu.buttonColor)))
				{
					int num7 = 0;
					string text3 = ExtensionTests.TestExtensionForRuntime(screenMan, info.FolderPath, out num7);
					try
					{
						ExtensionInfo.VerifyExtensionInfo(info);
					}
					catch (Exception ex)
					{
						text3 = text3 + "\nExtension Metadata Error:\n" + Utils.GenerateReportFromException(ex);
						num7++;
					}
					if (num7 == 0)
					{
						this.ReportOverride = string.Concat(new string[]
						{
							LocaleTerms.Loc("Testing..."),
							"\n",
							LocaleTerms.Loc("All tests complete"),
							"\n0 ",
							LocaleTerms.Loc("Errors Found")
						});
					}
					else
					{
						this.ReportOverride = LocaleTerms.Loc("Errors Found") + ". " + string.Format(LocaleTerms.Loc("Writing report to {0}"), (info.FolderPath + "/report.txt\n").Replace("\\", "/")) + text3;
						string text4 = info.FolderPath + "/report.txt";
						if (File.Exists(text4))
						{
							File.Delete(text4);
						}
						Utils.writeToFile(this.ReportOverride, text4);
					}
					MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
					ExtensionLoader.ActiveExtensionInfo = this.ExtensionInfoToShow;
				}
				drawpos.Y += (float)(num3 + num4);
				if (Settings.AllowExtensionPublish && PlatformAPISettings.Running)
				{
					if (Button.doButton(7900031, (int)drawpos.X, (int)drawpos.Y, 450, num3, LocaleTerms.Loc("Steam Workshop Publishing"), new Color?(MainMenu.buttonColor)))
					{
						this.IsInPublishScreen = true;
					}
					drawpos.Y += (float)(num3 + num4);
				}
				if (Button.doButton(7900040, (int)drawpos.X, (int)drawpos.Y, 450, 25, LocaleTerms.Loc("Back to Extension List"), new Color?(MainMenu.exitButtonColor)))
				{
					this.ExtensionInfoToShow = null;
				}
				drawpos.Y += 30f;
				result = drawpos;
			}
			return result;
		}

		// Token: 0x06000521 RID: 1313 RVA: 0x00050370 File Offset: 0x0004E570
		private Vector2 DrawExtensionList(Vector2 drawpos, Rectangle dest, SpriteBatch sb)
		{
			if (this.HasLoaded)
			{
				Rectangle fullscreen = Utils.GetFullscreen();
				for (int i = this.ScrollStartIndex; i <= this.Extensions.Count; i++)
				{
					if (drawpos.Y + 120f + 20f >= (float)fullscreen.Height || (this.ScrollStartIndex > 0 && i == this.Extensions.Count))
					{
						int num = 20;
						int num2 = (50 - num * 2) / 4;
						if (Button.doButton(790001 + i, (int)drawpos.X, (int)drawpos.Y + num2, 450, num, "   ^   ", new Color?((this.ScrollStartIndex > 0) ? MainMenu.buttonColor : Color.Black)) && this.ScrollStartIndex > 0)
						{
							this.ScrollStartIndex--;
						}
						bool flag = i <= this.Extensions.Count - 1;
						if (Button.doButton(790101 + i + 1, (int)drawpos.X, (int)drawpos.Y + num + num2 + num2 + 2, 450, num, "   v   ", new Color?(flag ? MainMenu.buttonColor : Color.Black)) && flag)
						{
							this.ScrollStartIndex++;
						}
						drawpos.Y += 55f;
						break;
					}
					if (i < this.Extensions.Count)
					{
						ExtensionInfo extensionInfo = this.Extensions[i];
						if (Button.doButton(780001 + i, (int)drawpos.X, (int)drawpos.Y, 450, 50, extensionInfo.Name, new Color?(Color.White)))
						{
							this.ActivateExtensionPage(extensionInfo);
						}
						drawpos.Y += 55f;
					}
				}
			}
			else
			{
				TextItem.doFontLabel(drawpos, LocaleTerms.Loc("Loading..."), GuiData.font, new Color?(Color.White), (float)dest.Width, 20f, false);
				drawpos.Y += 55f;
			}
			if (!string.IsNullOrWhiteSpace(this.LoadErrors))
			{
				TextItem.doFontLabel(drawpos + new Vector2(0f, 30f), this.LoadErrors, GuiData.smallfont, new Color?(Color.Red), float.MaxValue, float.MaxValue, false);
			}
			return drawpos;
		}

		// Token: 0x040005AD RID: 1453
		public const string ExtensionBaseFolder = "Extensions/";

		// Token: 0x040005AE RID: 1454
		public List<ExtensionInfo> Extensions = new List<ExtensionInfo>();

		// Token: 0x040005AF RID: 1455
		private bool HasLoaded = false;

		// Token: 0x040005B0 RID: 1456
		private bool HasStartedLoading = false;

		// Token: 0x040005B1 RID: 1457
		private ExtensionInfo ExtensionInfoToShow = null;

		// Token: 0x040005B2 RID: 1458
		private Texture2D DefaultModImage = null;

		// Token: 0x040005B3 RID: 1459
		private SavefileLoginScreen SaveScreen = new SavefileLoginScreen();

		// Token: 0x040005B4 RID: 1460
		private SteamWorkshopPublishScreen workshopPublishScreen = null;

		// Token: 0x040005B5 RID: 1461
		private bool IsInPublishScreen = false;

		// Token: 0x040005B6 RID: 1462
		private string ReportOverride = null;

		// Token: 0x040005B7 RID: 1463
		public Action ExitClicked;

		// Token: 0x040005B8 RID: 1464
		public Action<string, string> CreateNewAccountForExtension_UserAndPass;

		// Token: 0x040005B9 RID: 1465
		public Action<string, string> LoadAccountForExtension_FileAndUsername;

		// Token: 0x040005BA RID: 1466
		private ExtensionsMenuScreen.EMSState State = ExtensionsMenuScreen.EMSState.Normal;

		// Token: 0x040005BB RID: 1467
		public string LoadErrors = "";

		// Token: 0x040005BC RID: 1468
		public int ScrollStartIndex = 0;

		// Token: 0x020000F1 RID: 241
		private enum EMSState
		{
			// Token: 0x040005BE RID: 1470
			Normal,
			// Token: 0x040005BF RID: 1471
			GetUsername,
			// Token: 0x040005C0 RID: 1472
			ShowAccounts
		}
	}
}
