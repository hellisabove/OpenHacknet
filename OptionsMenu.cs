using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x02000143 RID: 323
	internal class OptionsMenu : GameScreen
	{
		// Token: 0x060007D4 RID: 2004 RVA: 0x00080EE4 File Offset: 0x0007F0E4
		public OptionsMenu()
		{
			this.resolutionChanged = false;
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00080F44 File Offset: 0x0007F144
		public OptionsMenu(bool startedFromGameContext)
		{
			this.resolutionChanged = false;
			this.startedFromGameContext = startedFromGameContext;
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00080FAC File Offset: 0x0007F1AC
		public override void LoadContent()
		{
			base.LoadContent();
			this.resolutions = new string[20];
			List<string> list = new List<string>();
			list.Add("1152x768");
			list.Add("1280x854");
			list.Add("1440x960");
			list.Add("2880x1920");
			list.Add("1152x864");
			list.Add("1280x960");
			list.Add("1400x1050");
			list.Add("1600x1200");
			list.Add("1280x760");
			list.Add("1280x1024");
			list.Add("1280x720");
			list.Add("1365x760");
			list.Add("1366x768");
			list.Add("1408x792");
			list.Add("1600x900");
			list.Add("1920x1080");
			list.Add("2560x1440");
			list.Add("1280x800");
			list.Add("1440x900");
			list.Add("1680x1050");
			list.Add("1920x1200");
			list.Add("1792x1008");
			list.Add("2560x1600");
			list.Add("2560x1080");
			list.Add("3440x1440");
			list.Add("3440x1440");
			list.Add("3840x2160");
			list.Add("4096x2160");
			list.Sort();
			this.resolutions = list.ToArray();
			this.fontConfigs = new string[GuiData.FontConfigs.Count];
			for (int i = 0; i < GuiData.FontConfigs.Count; i++)
			{
				this.fontConfigs[i] = GuiData.FontConfigs[i].name;
				if (GuiData.ActiveFontConfig.name == this.fontConfigs[i])
				{
					this.currentFontIndex = i;
				}
			}
			for (int i = 0; i < this.resolutions.Length; i++)
			{
				if (this.resolutions[i].Equals(this.getCurrentResolution()))
				{
					this.currentResIndex = i;
					break;
				}
			}
			this.windowed = this.getIfWindowed();
			this.localeNames = new string[LocaleActivator.SupportedLanguages.Count];
			for (int i = 0; i < LocaleActivator.SupportedLanguages.Count; i++)
			{
				this.localeNames[i] = LocaleActivator.SupportedLanguages[i].Name;
				if (LocaleActivator.SupportedLanguages[i].Code == Settings.ActiveLocale)
				{
					this.currentLocaleIndex = i;
				}
			}
			this.originallyActiveLocale = Settings.ActiveLocale;
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x00081268 File Offset: 0x0007F468
		public string getCurrentResolution()
		{
			string text = "";
			text += base.ScreenManager.GraphicsDevice.Viewport.Width;
			text += "x";
			return text + base.ScreenManager.GraphicsDevice.Viewport.Height;
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x000812D8 File Offset: 0x0007F4D8
		public bool getIfWindowed()
		{
			return Game1.getSingleton().graphics.IsFullScreen;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x000812FC File Offset: 0x0007F4FC
		public void apply()
		{
			bool flag = false;
			if (this.windowed != this.getIfWindowed())
			{
				Game1.getSingleton().graphics.ToggleFullScreen();
				Settings.windowed = this.getIfWindowed();
				flag = true;
			}
			if (this.resolutionChanged)
			{
				string[] array = this.resolutions[this.currentResIndex].Split(this.xArray);
				int preferredBackBufferWidth = Convert.ToInt32(array[0]);
				int preferredBackBufferHeight = Convert.ToInt32(array[1]);
				Game1.getSingleton().graphics.PreferredBackBufferWidth = preferredBackBufferWidth;
				Game1.getSingleton().graphics.PreferredBackBufferHeight = preferredBackBufferHeight;
				Game1.getSingleton().graphics.PreferMultiSampling = SettingsLoader.ShouldMultisample;
				PostProcessor.GenerateMainTarget(Game1.getSingleton().graphics.GraphicsDevice);
			}
			GuiData.ActivateFontConfig(this.fontConfigs[this.currentFontIndex]);
			if (this.resolutionChanged || flag)
			{
				Game1.getSingleton().graphics.ApplyChanges();
				Game1.getSingleton().setNewGraphics();
			}
			else
			{
				base.ExitScreen();
			}
			SettingsLoader.writeStatusFile();
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x00081420 File Offset: 0x0007F620
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			if (this.needsApply)
			{
				base.ScreenManager.GraphicsDevice.SetRenderTarget(null);
				this.apply();
				this.needsApply = false;
			}
			if (GuiData.mouse.LeftButton == ButtonState.Released)
			{
				this.mouseHasBeenReleasedOnThisScreen = true;
			}
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x00081484 File Offset: 0x0007F684
		public override void HandleInput(InputState input)
		{
			base.HandleInput(input);
			GuiData.doInput(input);
			if (Settings.debugCommandsEnabled && Utils.keyPressed(input, Keys.F8, null))
			{
				base.ExitScreen();
				Game1.getSingleton().Exit();
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x000814D8 File Offset: 0x0007F6D8
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			PostProcessor.begin();
			base.ScreenManager.FadeBackBufferToBlack(255);
			GuiData.startDraw();
			PatternDrawer.draw(new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height), 0.5f, Color.Black, new Color(2, 2, 2), GuiData.spriteBatch);
			if (Button.doButton(999, 10, 10, 220, 30, "<- " + LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
			{
				SettingsLoader.writeStatusFile();
				base.ExitScreen();
			}
			if (Button.doButton(9907, 10, 44, 220, 20, LocaleTerms.Loc("Apply Changes"), new Color?(Color.LightBlue)))
			{
				this.needsApply = true;
			}
			int num = 100;
			TextItem.doLabel(new Vector2(400f, (float)num), LocaleTerms.Loc("Resolutions"), null, 200f);
			int num2 = this.currentResIndex;
			this.currentResIndex = SelectableTextList.doFancyList(10, 400, num + 36, 200, 450, this.resolutions, this.currentResIndex, null, false);
			if (!this.mouseHasBeenReleasedOnThisScreen)
			{
				this.currentResIndex = num2;
			}
			else if (SelectableTextList.wasActivated)
			{
				this.resolutionChanged = true;
			}
			if (!this.startedFromGameContext)
			{
				TextItem.doLabel(new Vector2(620f, (float)num), LocaleTerms.Loc("Language"), null, 200f);
				int num3 = this.currentLocaleIndex;
				this.currentLocaleIndex = SelectableTextList.doFancyList(1013, 620, num + 36, 200, 450, this.localeNames, this.currentLocaleIndex, null, false);
				if (!this.mouseHasBeenReleasedOnThisScreen)
				{
					this.currentLocaleIndex = num3;
				}
				else if (SelectableTextList.wasActivated)
				{
					LocaleActivator.ActivateLocale(LocaleActivator.SupportedLanguages[this.currentLocaleIndex].Code, Game1.getSingleton().Content);
					Settings.ActiveLocale = LocaleActivator.SupportedLanguages[this.currentLocaleIndex].Code;
				}
			}
			int num4 = 64;
			float maxWidth = 280f;
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 36)), LocaleTerms.Loc("Fullscreen"), null, maxWidth);
			this.windowed = CheckBox.doCheckBox(20, 100, num4 += 34, this.windowed, null);
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Bloom"), null, maxWidth);
			PostProcessor.bloomEnabled = CheckBox.doCheckBox(21, 100, num4 += 34, PostProcessor.bloomEnabled, null);
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Scanlines"), null, maxWidth);
			PostProcessor.scanlinesEnabled = CheckBox.doCheckBox(22, 100, num4 += 34, PostProcessor.scanlinesEnabled, null);
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Multisampling"), null, maxWidth);
			bool shouldMultisample = SettingsLoader.ShouldMultisample;
			SettingsLoader.ShouldMultisample = CheckBox.doCheckBox(221, 100, num4 += 34, SettingsLoader.ShouldMultisample, null);
			if (shouldMultisample != SettingsLoader.ShouldMultisample)
			{
				this.resolutionChanged = true;
			}
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Audio Visualiser"), null, maxWidth);
			SettingsLoader.ShouldDrawMusicVis = CheckBox.doCheckBox(223, 100, num4 += 34, SettingsLoader.ShouldDrawMusicVis, null);
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Sound Enabled"), null, maxWidth);
			MusicManager.setIsMuted(!CheckBox.doCheckBox(23, 100, num4 += 34, !MusicManager.isMuted, null));
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Music Volume"), null, maxWidth);
			MusicManager.setVolume(SliderBar.doSliderBar(24, 100, num4 += 34, 210, 30, 1f, 0f, MusicManager.getVolume(), 0.001f));
			TextItem.doLabel(new Vector2(100f, (float)(num4 += 32)), LocaleTerms.Loc("Text Size"), null, maxWidth);
			int num5 = this.currentFontIndex;
			this.currentFontIndex = SelectableTextList.doFancyList(25, 100, num4 += 34, 200, 160, this.fontConfigs, this.currentFontIndex, null, false);
			if (this.currentFontIndex != num5 && this.startedFromGameContext)
			{
				try
				{
					if (OS.currentInstance != null)
					{
						OS.currentInstance.terminal.reset();
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
					Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				}
			}
			if (Button.doButton(990, 10, num4 + 150, 220, 30, LocaleTerms.Loc("Apply Changes"), new Color?(Color.LightBlue)))
			{
				this.needsApply = true;
			}
			GuiData.endDraw();
			PostProcessor.end();
		}

		// Token: 0x040008E4 RID: 2276
		private string[] resolutions;

		// Token: 0x040008E5 RID: 2277
		private int currentResIndex;

		// Token: 0x040008E6 RID: 2278
		private string[] fontConfigs;

		// Token: 0x040008E7 RID: 2279
		private int currentFontIndex = -1;

		// Token: 0x040008E8 RID: 2280
		private bool resolutionChanged;

		// Token: 0x040008E9 RID: 2281
		private bool windowed;

		// Token: 0x040008EA RID: 2282
		private char[] xArray = new char[]
		{
			'x'
		};

		// Token: 0x040008EB RID: 2283
		private bool needsApply = false;

		// Token: 0x040008EC RID: 2284
		private bool mouseHasBeenReleasedOnThisScreen = false;

		// Token: 0x040008ED RID: 2285
		private string originallyActiveLocale = "en-us";

		// Token: 0x040008EE RID: 2286
		private string[] localeNames;

		// Token: 0x040008EF RID: 2287
		private int currentLocaleIndex = 0;

		// Token: 0x040008F0 RID: 2288
		private bool startedFromGameContext = false;
	}
}
