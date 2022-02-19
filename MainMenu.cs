using System;
using System.IO;
using System.Threading;
using System.Xml;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Misc;
using Hacknet.PlatformAPI.Storage;
using Hacknet.Screens;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200012B RID: 299
	internal class MainMenu : GameScreen
	{
		// Token: 0x0600070D RID: 1805 RVA: 0x0007395C File Offset: 0x00071B5C
		public MainMenu()
		{
			base.TransitionOffTime = TimeSpan.FromSeconds(0.5);
		}

		// Token: 0x0600070E RID: 1806 RVA: 0x000739D4 File Offset: 0x00071BD4
		public override void LoadContent()
		{
			MainMenu.buttonColor = new Color(124, 137, 149);
			MainMenu.exitButtonColor = new Color(105, 82, 82);
			this.titleColor = new Color(190, 190, 190, 0);
			this.titleFont = base.ScreenManager.Game.Content.Load<SpriteFont>("Kremlin");
			this.canLoad = (SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser.Username != null);
			this.hexBackground = new HexGridBackground(base.ScreenManager.Game.Content);
			this.HookUpCreationEvents();
			MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
			Console.WriteLine("Menu load complete");
		}

		// Token: 0x0600070F RID: 1807 RVA: 0x000742AC File Offset: 0x000724AC
		private void HookUpCreationEvents()
		{
			SavefileLoginScreen savefileLoginScreen = this.loginScreen;
			savefileLoginScreen.RequestGoBack = (Action)Delegate.Combine(savefileLoginScreen.RequestGoBack, new Action(delegate()
			{
				this.State = MainMenu.MainMenuState.Normal;
			}));
			SavefileLoginScreen savefileLoginScreen2 = this.loginScreen;
			savefileLoginScreen2.StartNewGameForUsernameAndPass = (Action<string, string>)Delegate.Combine(savefileLoginScreen2.StartNewGameForUsernameAndPass, new Action<string, string>(delegate(string username, string pass)
			{
				if (SaveFileManager.AddUser(username, pass))
				{
					string filePathForLogin = SaveFileManager.GetFilePathForLogin(username, pass);
					base.ExitScreen();
					MainMenu.resetOS();
					if (!Settings.soundDisabled)
					{
						base.ScreenManager.playAlertSound();
					}
					try
					{
						OS os = new OS();
						os.SaveGameUserName = filePathForLogin;
						os.SaveUserAccountName = username;
						if (this.NextStartedGameShouldBeDLCAccelerated)
						{
							os.IsDLCConventionDemo = true;
							os.Flags.AddFlag("TutorialComplete");
							Settings.EnableDLC = true;
							Settings.initShowsTutorial = false;
							os.initShowsTutorial = false;
						}
						base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
						os.Flags.AddFlag("startVer:" + MainMenu.OSVersion);
						if (this.NextStartedGameShouldBeDLCAccelerated)
						{
							SessionAccelerator.AccelerateSessionToDLCStart(os);
							os.delayer.Post(ActionDelayer.Wait(0.15), delegate
							{
								Game1.getSingleton().IsMouseVisible = true;
							});
							this.NextStartedGameShouldBeDLCAccelerated = false;
						}
					}
					catch (Exception ex)
					{
						this.UpdateUIForSaveCreationFailed(ex);
					}
				}
				else
				{
					this.loginScreen.ResetForNewAccount();
					this.loginScreen.WriteToHistory(" ERROR: Username invalid or already in use.");
				}
			}));
			SavefileLoginScreen savefileLoginScreen3 = this.loginScreen;
			savefileLoginScreen3.LoadGameForUserFileAndUsername = (Action<string, string>)Delegate.Combine(savefileLoginScreen3.LoadGameForUserFileAndUsername, new Action<string, string>(delegate(string userFile, string username)
			{
				base.ExitScreen();
				MainMenu.resetOS();
				if (SaveFileManager.StorageMethods[0].FileExists(userFile))
				{
					OS.WillLoadSave = true;
					OS os = new OS();
					os.SaveGameUserName = userFile;
					os.SaveUserAccountName = username;
					try
					{
						base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					}
					catch (XmlException ex)
					{
						this.UpdateUIForSaveCorruption(userFile, ex);
					}
					catch (FormatException ex2)
					{
						this.UpdateUIForSaveCorruption(userFile, ex2);
					}
					catch (NullReferenceException ex3)
					{
						this.UpdateUIForSaveCorruption(userFile, ex3);
					}
					catch (FileNotFoundException ex4)
					{
						this.UpdateUIForSaveMissing(userFile, ex4);
					}
					catch (ContentLoadException ex5)
					{
						string text = Utils.ReadEntireContentsOfStream(SaveFileManager.StorageMethods[0].GetFileReadStream(userFile));
						if (text.Contains("DigiPets"))
						{
							text = text.Replace("DigiPets", "Neopals");
							text = text.Replace("DigiPoints", "Neopoints");
							for (int i = 0; i < 3; i++)
							{
								try
								{
									Thread.Sleep(200);
									SaveFileManager.StorageMethods[0].WriteFileData(userFile, text);
									break;
								}
								catch (IOException)
								{
								}
								Thread.Sleep(500);
							}
							MainMenu.AccumErrors = "-- Savefile Automatically Upgraded - Try again! --";
						}
						else
						{
							this.UpdateUIForSaveCorruption(userFile, ex5);
						}
					}
				}
				else
				{
					OS.WillLoadSave = false;
					this.UpdateUIForSaveMissing(userFile, new FileNotFoundException());
				}
			}));
			AttractModeMenuScreen attractModeMenuScreen = this.attractModeScreen;
			attractModeMenuScreen.Start = (Action)Delegate.Combine(attractModeMenuScreen.Start, new Action(delegate()
			{
				try
				{
					base.ExitScreen();
					MainMenu.resetOS();
					if (!Settings.soundDisabled)
					{
						base.ScreenManager.playAlertSound();
					}
					base.ScreenManager.AddScreen(new OS(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
				}
				catch (Exception ex)
				{
					Utils.writeToFile("OS Load Error: " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
				}
			}));
			AttractModeMenuScreen attractModeMenuScreen2 = this.attractModeScreen;
			attractModeMenuScreen2.StartDLC = (Action)Delegate.Combine(attractModeMenuScreen2.StartDLC, new Action(delegate()
			{
				try
				{
					base.ExitScreen();
					MainMenu.resetOS();
					Settings.EnableDLC = true;
					Settings.initShowsTutorial = false;
					if (!Settings.soundDisabled)
					{
						base.ScreenManager.playAlertSound();
					}
					OS os = new OS();
					os.IsDLCConventionDemo = true;
					os.Flags.AddFlag("TutorialComplete");
					os.SaveGameUserName = "save_" + Settings.ConventionLoginName + ".xml";
					os.SaveUserAccountName = Settings.ConventionLoginName;
					base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					os.allFactions.setCurrentFaction("Bibliotheque", os);
					ThemeManager.setThemeOnComputer(os.thisComputer, "DLC/Themes/RiptideClassicTheme.xml");
					ThemeManager.switchTheme(os, "DLC/Themes/RiptideClassicTheme.xml");
					for (int i = 0; i < 60; i++)
					{
						int index;
						do
						{
							index = Utils.random.Next(os.netMap.nodes.Count);
						}
						while (os.netMap.nodes[index].idName == "mainHub" || os.netMap.nodes[index].idName == "entropy00" || os.netMap.nodes[index].idName == "entropy01");
						os.netMap.discoverNode(os.netMap.nodes[index]);
					}
					os.delayer.Post(ActionDelayer.Wait(0.15), delegate
					{
						Game1.getSingleton().IsMouseVisible = true;
						os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[22], "SSHCrack.exe"));
						os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[21], "FTPBounce.exe"));
						MissionFunctions.runCommand(7, "changeSong");
						MusicManager.stop();
					});
					os.delayer.Post(ActionDelayer.Wait(38.0), delegate
					{
						ComputerLoader.loadMission("Content/DLC/Missions/Demo/DLCDemointroMission1.xml", false);
					});
				}
				catch (Exception ex)
				{
					Utils.writeToFile("OS Load Error: " + ex.ToString() + "\n\n" + ex.StackTrace, "crashLog.txt");
				}
			}));
			ExtensionsMenuScreen extensionsMenuScreen = this.extensionsScreen;
			extensionsMenuScreen.ExitClicked = (Action)Delegate.Combine(extensionsMenuScreen.ExitClicked, new Action(delegate()
			{
				this.State = MainMenu.MainMenuState.Normal;
			}));
			ExtensionsMenuScreen extensionsMenuScreen2 = this.extensionsScreen;
			extensionsMenuScreen2.CreateNewAccountForExtension_UserAndPass = (Action<string, string>)Delegate.Combine(extensionsMenuScreen2.CreateNewAccountForExtension_UserAndPass, new Action<string, string>(delegate(string user, string pass)
			{
				MainMenu.CreateNewAccountForExtensionAndStart(user, pass, base.ScreenManager, this, this.extensionsScreen);
			}));
			ExtensionsMenuScreen extensionsMenuScreen3 = this.extensionsScreen;
			extensionsMenuScreen3.LoadAccountForExtension_FileAndUsername = (Action<string, string>)Delegate.Combine(extensionsMenuScreen3.LoadAccountForExtension_FileAndUsername, new Action<string, string>(delegate(string userFile, string username)
			{
				base.ExitScreen();
				MainMenu.resetOS();
				Settings.IsInExtensionMode = true;
				if (SaveFileManager.StorageMethods[0].FileExists(userFile))
				{
					OS.WillLoadSave = true;
				}
				else
				{
					OS.WillLoadSave = false;
				}
				OS os = new OS();
				os.SaveGameUserName = userFile;
				os.SaveUserAccountName = username;
				base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
			}));
		}

		// Token: 0x06000710 RID: 1808 RVA: 0x000743F4 File Offset: 0x000725F4
		public static void CreateNewAccountForExtensionAndStart(string username, string pass, ScreenManager sman, GameScreen currentScreen = null, ExtensionsMenuScreen extensionsScreen = null)
		{
			if (SaveFileManager.AddUser(username, pass))
			{
				OS os = new OS();
				string filePathForLogin = SaveFileManager.GetFilePathForLogin(username, pass);
				if (currentScreen != null)
				{
					currentScreen.ExitScreen();
				}
				MainMenu.resetOS();
				Settings.IsInExtensionMode = true;
				if (!Settings.soundDisabled)
				{
					sman.playAlertSound();
				}
				os.SaveGameUserName = filePathForLogin;
				os.SaveUserAccountName = username;
				sman.AddScreen(os, new PlayerIndex?(sman.controllingPlayer));
			}
			else if (extensionsScreen != null)
			{
				extensionsScreen.ShowError("Error Creating UserAccount for username :" + username);
			}
			else
			{
				MainMenu.AccumErrors = MainMenu.AccumErrors + "Error auto-loading Extension " + Game1.AutoLoadExtensionPath;
			}
		}

		// Token: 0x06000711 RID: 1809 RVA: 0x000744D0 File Offset: 0x000726D0
		private void UpdateUIForSaveCorruption(string saveName, Exception ex)
		{
			this.State = MainMenu.MainMenuState.Normal;
			string accumErrors = MainMenu.AccumErrors;
			MainMenu.AccumErrors = string.Concat(new string[]
			{
				accumErrors,
				string.Format(LocaleTerms.Loc("ACCOUNT FILE CORRUPTION: Account {0} appears to be corrupted, and will not load."), saveName),
				" Reported Error:\r\n",
				Utils.GenerateReportFromException(ex),
				"\r\n"
			});
			if (!this.hasSentErrorEmail)
			{
				new Thread(delegate()
				{
					Utils.SendErrorEmail(ex, "Save Corruption ", "");
				})
				{
					IsBackground = true,
					Name = "SaveCorruptErrorReportThread"
				}.Start();
				this.hasSentErrorEmail = true;
			}
		}

		// Token: 0x06000712 RID: 1810 RVA: 0x000745B4 File Offset: 0x000727B4
		private void UpdateUIForSaveMissing(string saveName, Exception ex)
		{
			this.State = MainMenu.MainMenuState.Normal;
			string accumErrors = MainMenu.AccumErrors;
			MainMenu.AccumErrors = string.Concat(new string[]
			{
				accumErrors,
				"ACCOUNT FILE NOT FOUND: Account ",
				saveName,
				" appears to be missing. It may have been moved or deleted. Reported Error:\r\n",
				Utils.GenerateReportFromException(ex),
				"\r\n"
			});
			if (!this.hasSentErrorEmail)
			{
				new Thread(delegate()
				{
					Utils.SendErrorEmail(ex, "Save Missing ", "");
				})
				{
					IsBackground = true,
					Name = "SaveMissingErrorReportThread"
				}.Start();
				this.hasSentErrorEmail = true;
			}
		}

		// Token: 0x06000713 RID: 1811 RVA: 0x00074694 File Offset: 0x00072894
		private void UpdateUIForSaveCreationFailed(Exception ex)
		{
			this.State = MainMenu.MainMenuState.Normal;
			MainMenu.AccumErrors = MainMenu.AccumErrors + "CRITICAL ERROR CREATING ACCOUNT: Reported Error:\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
			if (!this.hasSentErrorEmail)
			{
				new Thread(delegate()
				{
					Utils.SendErrorEmail(ex, "Account Creation Error ", "");
				})
				{
					IsBackground = true,
					Name = "SaveAccCreationErrorReportThread"
				}.Start();
				this.hasSentErrorEmail = true;
			}
		}

		// Token: 0x06000714 RID: 1812 RVA: 0x00074724 File Offset: 0x00072924
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			this.framecount++;
			this.hexBackground.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
		}

		// Token: 0x06000715 RID: 1813 RVA: 0x00074765 File Offset: 0x00072965
		public override void HandleInput(InputState input)
		{
			base.HandleInput(input);
			GuiData.doInput(input);
		}

		// Token: 0x06000716 RID: 1814 RVA: 0x00074778 File Offset: 0x00072978
		public static void resetOS()
		{
			if (!Settings.isSpecialTestBuild)
			{
				TextBox.cursorPosition = 0;
				Settings.initShowsTutorial = Settings.osStartsWithTutorial;
				Settings.IsInExtensionMode = false;
				ScrollablePanel.ClearCache();
				PostProcessor.dangerModeEnabled = false;
				PostProcessor.dangerModePercentComplete = 0f;
				PostProcessor.EndingSequenceFlashOutActive = false;
				PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
			}
		}

		// Token: 0x06000717 RID: 1815 RVA: 0x000747D0 File Offset: 0x000729D0
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			try
			{
				PostProcessor.begin();
				base.ScreenManager.FadeBackBufferToBlack(255);
				GuiData.startDraw();
				Rectangle dest = new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height);
				Rectangle destinationRectangle = new Rectangle(-20, -20, base.ScreenManager.GraphicsDevice.Viewport.Width + 40, base.ScreenManager.GraphicsDevice.Viewport.Height + 40);
				Rectangle dest2 = new Rectangle(dest.X + dest.Width / 4, dest.Height / 4, dest.Width / 2, dest.Height / 4);
				GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black);
				if (Settings.DrawHexBackground)
				{
					this.hexBackground.Draw(dest, GuiData.spriteBatch, Color.Transparent, Settings.lighterColorHexBackground ? new Color(20, 20, 20) : new Color(15, 15, 15, 0), HexGridBackground.ColoringAlgorithm.NegaitiveSinWash, 0f);
				}
				TextItem.DrawShadow = false;
				switch (this.State)
				{
				case MainMenu.MainMenuState.NewUser:
					this.DrawLoginScreen(dest2, true);
					goto IL_245;
				case MainMenu.MainMenuState.Login:
					this.DrawLoginScreen(dest2, false);
					goto IL_245;
				case MainMenu.MainMenuState.Extensions:
				{
					bool canRun = this.DrawBackgroundAndTitle();
					Rectangle dest3 = new Rectangle(180, 150, Math.Min(700, dest.Width / 2), (int)((float)dest.Height * 0.7f));
					this.extensionsScreen.Draw(dest3, GuiData.spriteBatch, base.ScreenManager);
					goto IL_245;
				}
				}
				if (Settings.isLockedDemoMode)
				{
					this.attractModeScreen.Draw(dest, GuiData.spriteBatch);
				}
				else
				{
					bool canRun = this.DrawBackgroundAndTitle();
					if (Settings.isLockedDemoMode)
					{
						this.drawDemoModeButtons(canRun);
					}
					else
					{
						this.drawMainMenuButtons(canRun);
						if (Settings.testingMenuItemsEnabled)
						{
							this.drawTestingMainMenuButtons(canRun);
						}
					}
				}
				IL_245:
				GuiData.endDraw();
				PostProcessor.end();
				base.ScreenManager.FadeBackBufferToBlack((int)(byte.MaxValue - base.TransitionAlpha));
			}
			catch (ObjectDisposedException ex)
			{
				if (this.hasSentErrorEmail)
				{
					throw ex;
				}
				string text = Utils.GenerateReportFromException(ex);
				text = text + "\r\n Font:" + this.titleFont;
				text = text + "\r\n White:" + Utils.white;
				text = text + "\r\n WhiteDisposed:" + Utils.white.IsDisposed;
				text = text + "\r\n SmallFont:" + GuiData.smallfont;
				text = text + "\r\n TinyFont:" + GuiData.tinyfont;
				text = text + "\r\n LineEffectTarget:" + FlickeringTextEffect.GetReportString();
				text = text + "\r\n PostProcessort stuff:" + PostProcessor.GetStatusReportString();
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"\r\nRESOLUTION:\r\n ",
					Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferWidth,
					"x"
				});
				text = text + Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferHeight + "\r\nFullscreen: ";
				text += (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false");
				text = text + "\r\n Adapter: " + Game1.getSingleton().GraphicsDevice.Adapter.Description;
				text = text + "\r\n Device Name: " + Game1.getSingleton().GraphicsDevice.Adapter.DeviceName;
				text = text + "\r\n Status: " + Game1.getSingleton().GraphicsDevice.GraphicsDeviceStatus;
				Utils.SendRealWorldEmail(string.Concat(new string[]
				{
					"Hacknet ",
					MainMenu.OSVersion,
					" Crash ",
					DateTime.Now.ToShortDateString(),
					" ",
					DateTime.Now.ToShortTimeString()
				}), "hacknetbugs+Hacknet@gmail.com", text);
				this.hasSentErrorEmail = true;
				SettingsLoader.writeStatusFile();
			}
		}

		// Token: 0x06000718 RID: 1816 RVA: 0x00074C90 File Offset: 0x00072E90
		private bool DrawBackgroundAndTitle()
		{
			Rectangle dest = new Rectangle(180, 120, 340, 100);
			FlickeringTextEffect.DrawLinedFlickeringText(dest, "HACKNET", 7f, 0.55f, this.titleFont, null, this.titleColor, 2);
			string text = "";
			string text2 = string.Concat(new string[]
			{
				"OS",
				DLC1SessionUpgrader.HasDLC1Installed ? "+Labyrinths " : " ",
				MainMenu.OSVersion,
				" ",
				text
			});
			TextItem.doFontLabel(new Vector2(520f, 178f), text2, GuiData.smallfont, new Color?(this.titleColor * 0.5f), 600f, 26f, false);
			bool result = true;
			if (Settings.IsExpireLocked)
			{
				TimeSpan timeSpan = Settings.ExpireTime - DateTime.Now;
				string text3;
				if (timeSpan.TotalSeconds < 1.0)
				{
					text3 = LocaleTerms.Loc("TEST BUILD EXPIRED - EXECUTION DISABLED");
					result = false;
				}
				else
				{
					text3 = "Test Build : Expires in " + timeSpan.ToString();
				}
				TextItem.doFontLabel(new Vector2(180f, 105f), text3, GuiData.smallfont, new Color?(Color.Red * 0.8f), 600f, 26f, false);
			}
			return result;
		}

		// Token: 0x06000719 RID: 1817 RVA: 0x00074E16 File Offset: 0x00073016
		private void DrawLoginScreen(Rectangle dest, bool needsNewUser = false)
		{
			this.loginScreen.Draw(GuiData.spriteBatch, dest);
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x00074E2C File Offset: 0x0007302C
		private void drawDemoModeButtons(bool canRun)
		{
			if (Button.doButton(1, 180, 200, 450, 50, LocaleTerms.Loc("New Session"), new Color?(MainMenu.buttonColor)))
			{
				if (canRun)
				{
					try
					{
						base.ExitScreen();
						MainMenu.resetOS();
						base.ScreenManager.playAlertSound();
						base.ScreenManager.AddScreen(new OS(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
					}
					catch (Exception ex)
					{
						Utils.writeToFile(string.Concat(new string[]
						{
							LocaleTerms.Loc("OS Load Error"),
							": ",
							ex.ToString(),
							"\n\n",
							ex.StackTrace
						}), "crashLog.txt");
					}
				}
			}
			if (Button.doButton(11, 180, 265, 450, 50, LocaleTerms.Loc("Load Session"), new Color?(this.canLoad ? MainMenu.buttonColor : Color.Black)))
			{
				if (this.canLoad)
				{
					try
					{
						if (canRun)
						{
							base.ExitScreen();
							MainMenu.resetOS();
							OS.WillLoadSave = true;
							OS screen = new OS();
							base.ScreenManager.AddScreen(screen, new PlayerIndex?(base.ScreenManager.controllingPlayer));
						}
					}
					catch (Exception ex)
					{
						Utils.writeToFile(string.Concat(new string[]
						{
							LocaleTerms.Loc("OS Load Error"),
							": ",
							ex.ToString(),
							"\n\n",
							ex.StackTrace
						}), "crashLog.txt");
					}
				}
			}
			if (Button.doButton(15, 180, 330, 450, 28, LocaleTerms.Loc("Exit"), new Color?(MainMenu.exitButtonColor)))
			{
				MusicManager.stop();
				Game1.threadsExiting = true;
				Game1.getSingleton().Exit();
			}
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x000750A4 File Offset: 0x000732A4
		private void drawTestingMainMenuButtons(bool canRun)
		{
			SpriteFont tinyfont = GuiData.tinyfont;
			string text = "FONT:";
			for (int i = 0; i < tinyfont.Characters.Count; i++)
			{
				text += tinyfont.Characters[i];
				if (i % 20 == 0)
				{
					text += "\n";
				}
			}
			bool flag = true;
			if (flag)
			{
				text = "Labyrinths Testers:\nPress \"Start Full DLC Test\" to begin\n\n" + text;
			}
			GuiData.spriteBatch.DrawString(tinyfont, text, new Vector2(867f, 200f), Color.White);
			if (Button.doButton(8801, 634, 200, 225, 23, "New Test Session", new Color?(MainMenu.buttonColor)) && canRun)
			{
				if (canRun)
				{
					base.ExitScreen();
					MainMenu.resetOS();
					if (!Settings.soundDisabled)
					{
						base.ScreenManager.playAlertSound();
					}
					OS os = new OS();
					os.SaveGameUserName = "save_--test.xml";
					os.SaveUserAccountName = "__test";
					base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					os.Flags.AddFlag("TutorialComplete");
					os.delayer.RunAllDelayedActions();
					os.threadedSaveExecute(false);
					base.ScreenManager.RemoveScreen(os);
					OS.WillLoadSave = true;
					MainMenu.resetOS();
					os = new OS();
					os.SaveGameUserName = "save_--test.xml";
					os.SaveUserAccountName = "__test";
					base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					os.delayer.Post(ActionDelayer.Wait(0.1), delegate
					{
						Game1.getSingleton().IsMouseVisible = true;
					});
					os.delayer.Post(ActionDelayer.Wait(0.4), delegate
					{
						os.runCommand("debug");
						ComputerLoader.loadMission("Content/Missions/MainHub/Intro/Intro01.xml", false);
					});
					if (!Settings.EnableDLC)
					{
						ComputerLoader.loadMission("Content/Missions/BitMission0.xml", false);
					}
				}
			}
			if (Button.doButton(8803, 634, 225, 225, 23, "New DLC Test Session", new Color?(Settings.EnableDLC ? Color.Gray : MainMenu.buttonColor)) && canRun)
			{
				if (canRun)
				{
					base.ExitScreen();
					MainMenu.resetOS();
					if (!Settings.soundDisabled)
					{
						base.ScreenManager.playAlertSound();
					}
					OS os2 = new OS();
					os2.SaveGameUserName = "save_--test.xml";
					os2.SaveUserAccountName = "__test";
					base.ScreenManager.AddScreen(os2, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					SessionAccelerator.AccelerateSessionToDLCHA(os2);
					os2.threadedSaveExecute(false);
					base.ScreenManager.RemoveScreen(os2);
					OS.WillLoadSave = true;
					MainMenu.resetOS();
					Settings.initShowsTutorial = false;
					os2 = new OS();
					os2.SaveGameUserName = "save_--test.xml";
					os2.SaveUserAccountName = "__test";
					base.ScreenManager.AddScreen(os2, new PlayerIndex?(base.ScreenManager.controllingPlayer));
					os2.delayer.Post(ActionDelayer.Wait(0.15), delegate
					{
						Game1.getSingleton().IsMouseVisible = true;
					});
				}
			}
			if (Button.doButton(8806, 634, 250, 225, 23, "Run Test Suite", new Color?(MainMenu.buttonColor)))
			{
				this.testSuiteResult = TestSuite.RunTestSuite(base.ScreenManager, false);
			}
			if (Button.doButton(8809, 634, 275, 225, 23, "Run Quick Tests", new Color?(MainMenu.buttonColor)))
			{
				this.testSuiteResult = TestSuite.RunTestSuite(base.ScreenManager, true);
			}
			else
			{
				if (Button.doButton(8812, 634, 300, 225, 23, "Start Full DLC Test", new Color?(MainMenu.buttonColor)) && canRun)
				{
					this.StartFullDLCTest();
				}
				if (this.testSuiteResult != null)
				{
					TextItem.doFontLabel(new Vector2(635f, 325f), Utils.SuperSmartTwimForWidth(this.testSuiteResult, 600, GuiData.tinyfont), GuiData.tinyfont, new Color?((this.testSuiteResult.Length > 950) ? Utils.AddativeRed : Utils.AddativeWhite), float.MaxValue, float.MaxValue, false);
				}
			}
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00075600 File Offset: 0x00073800
		private void StartFullDLCTest()
		{
			base.ExitScreen();
			MainMenu.resetOS();
			if (!Settings.soundDisabled)
			{
				base.ScreenManager.playAlertSound();
			}
			OS os = new OS();
			string text = "MediaPreview";
			SaveFileManager.AddUser(text, "test");
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text);
			os.IsDLCConventionDemo = true;
			os.Flags.AddFlag("TutorialComplete");
			Settings.EnableDLC = true;
			Settings.initShowsTutorial = false;
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text;
			os.initShowsTutorial = false;
			base.ScreenManager.AddScreen(os, new PlayerIndex?(base.ScreenManager.controllingPlayer));
			SessionAccelerator.AccelerateSessionToDLCStart(os);
			os.delayer.Post(ActionDelayer.Wait(0.15), delegate
			{
				Game1.getSingleton().IsMouseVisible = true;
			});
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x000756E8 File Offset: 0x000738E8
		private void drawMainMenuButtons(bool canRun)
		{
			int num = 135;
			if (Button.doButton(1, 180, num += 65, 450, 50, LocaleTerms.Loc("New Session"), new Color?(MainMenu.buttonColor)))
			{
				if (canRun)
				{
					this.NextStartedGameShouldBeDLCAccelerated = false;
					this.State = MainMenu.MainMenuState.NewUser;
					this.loginScreen.ClearTextBox();
					this.loginScreen.ResetForNewAccount();
				}
			}
			bool hasSaves = SaveFileManager.HasSaves;
			string text = LocaleTerms.Loc("No Accounts");
			if (hasSaves)
			{
				if (this.canLoad)
				{
					text = string.Format(LocaleTerms.Loc("Continue with account [{0}]"), SaveFileManager.LastLoggedInUser.Username);
				}
				else
				{
					text = LocaleTerms.Loc("Invalid Last Account : Login Manually");
				}
			}
			if (Button.doButton(1102, 180, num += 65, 450, 28, text, new Color?(this.canLoad ? MainMenu.buttonColor : Color.Black)))
			{
				if (this.canLoad)
				{
					this.loginScreen.ClearTextBox();
					this.loginScreen.LoadGameForUserFileAndUsername(SaveFileManager.LastLoggedInUser.FileUsername, SaveFileManager.LastLoggedInUser.Username);
				}
			}
			if (Button.doButton(11, 180, num += 39, 450, 50, LocaleTerms.Loc("Login"), new Color?(hasSaves ? MainMenu.buttonColor : Color.Black)))
			{
				if (hasSaves)
				{
					try
					{
						this.State = MainMenu.MainMenuState.Login;
						this.loginScreen.ClearTextBox();
						this.loginScreen.ResetForLogin();
					}
					catch (Exception ex)
					{
						Utils.writeToFile(string.Concat(new string[]
						{
							LocaleTerms.Loc("OS Load Error"),
							": ",
							ex.ToString(),
							"\n\n",
							ex.StackTrace
						}), "crashLog.txt");
					}
				}
			}
			if (Button.doButton(3, 180, num += 65, 450, 50, LocaleTerms.Loc("Settings"), new Color?(MainMenu.buttonColor)))
			{
				base.ScreenManager.AddScreen(new OptionsMenu(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
			}
			int num2;
			num = (num2 = num + 65);
			if (Settings.isServerMode)
			{
				if (Button.doButton(4, 180, num2, 450, 50, "Start Relay Server", new Color?(MainMenu.buttonColor)))
				{
					base.ScreenManager.AddScreen(new ServerScreen(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
				}
				num2 += 65;
			}
			if (Settings.AllowExtensionMode)
			{
				if (Button.doButton(5, 180, num2, 450, 50, "Extensions", new Color?(MainMenu.buttonColor)))
				{
					this.State = MainMenu.MainMenuState.Extensions;
					this.extensionsScreen.Reset();
				}
				num2 += 65;
			}
			if (Settings.HasLabyrinthsDemoStartMainMenuButton && DLC1SessionUpgrader.HasDLC1Installed)
			{
				if (Button.doButton(7, 180, num2, 450, 28, "New Labyrinths Accelerated Session", new Color?(Color.Lerp(Utils.AddativeWhite, new Color(68, 162, 194), 1f - Utils.rand(0.3f)))))
				{
					if (canRun)
					{
						this.NextStartedGameShouldBeDLCAccelerated = true;
						this.State = MainMenu.MainMenuState.NewUser;
						this.loginScreen.ClearTextBox();
						this.loginScreen.ResetForNewAccount();
					}
				}
				num2 += 65;
			}
			if (Button.doButton(15, 180, num2, 450, 28, LocaleTerms.Loc("Exit"), new Color?(MainMenu.exitButtonColor)))
			{
				MusicManager.stop();
				Game1.threadsExiting = true;
				Game1.getSingleton().Exit();
			}
			num2 += 30;
			if (!PlatformAPISettings.RemoteStorageRunning)
			{
				TextItem.doFontLabel(new Vector2(180f, (float)num2), LocaleTerms.Loc("WARNING: Error connecting to Steam Cloud"), GuiData.smallfont, new Color?(Color.DarkRed), float.MaxValue, float.MaxValue, false);
				num2 += 20;
			}
			if (!string.IsNullOrWhiteSpace(MainMenu.AccumErrors))
			{
				TextItem.doFontLabel(new Vector2(180f, (float)num2), MainMenu.AccumErrors, GuiData.smallfont, new Color?(Color.DarkRed), float.MaxValue, float.MaxValue, false);
				num2 += 20;
			}
		}

		// Token: 0x040007EA RID: 2026
		private const int CharCountForTestPassedMessage = 950;

		// Token: 0x040007EB RID: 2027
		public static string OSVersion = "v5.069";

		// Token: 0x040007EC RID: 2028
		public static string AccumErrors = "";

		// Token: 0x040007ED RID: 2029
		public static Color buttonColor;

		// Token: 0x040007EE RID: 2030
		public static Color exitButtonColor;

		// Token: 0x040007EF RID: 2031
		private SpriteFont titleFont;

		// Token: 0x040007F0 RID: 2032
		private Color titleColor;

		// Token: 0x040007F1 RID: 2033
		private bool canLoad = false;

		// Token: 0x040007F2 RID: 2034
		private bool hasSentErrorEmail = false;

		// Token: 0x040007F3 RID: 2035
		private int framecount = 0;

		// Token: 0x040007F4 RID: 2036
		private HexGridBackground hexBackground;

		// Token: 0x040007F5 RID: 2037
		private string testSuiteResult = null;

		// Token: 0x040007F6 RID: 2038
		private SavefileLoginScreen loginScreen = new SavefileLoginScreen();

		// Token: 0x040007F7 RID: 2039
		private MainMenu.MainMenuState State = MainMenu.MainMenuState.Normal;

		// Token: 0x040007F8 RID: 2040
		private AttractModeMenuScreen attractModeScreen = new AttractModeMenuScreen();

		// Token: 0x040007F9 RID: 2041
		private ExtensionsMenuScreen extensionsScreen = new ExtensionsMenuScreen();

		// Token: 0x040007FA RID: 2042
		private bool NextStartedGameShouldBeDLCAccelerated = false;

		// Token: 0x0200012C RID: 300
		private enum MainMenuState
		{
			// Token: 0x04000801 RID: 2049
			Normal,
			// Token: 0x04000802 RID: 2050
			NewUser,
			// Token: 0x04000803 RID: 2051
			Login,
			// Token: 0x04000804 RID: 2052
			Extensions
		}
	}
}
