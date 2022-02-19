using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Localization;
using Hacknet.PlatformAPI;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SDL2;

namespace Hacknet
{
	// Token: 0x02000154 RID: 340
	public class Game1 : Game
	{
		// Token: 0x0600087F RID: 2175 RVA: 0x0009043C File Offset: 0x0008E63C
		public Game1()
		{
			Game1.OriginalCultureInfo = CultureInfo.CurrentCulture;
			Game1.culture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = Game1.culture;
			Thread.CurrentThread.CurrentUICulture = Game1.culture;
			Game1.threadsExiting = false;
			this.graphics = new GraphicsDeviceManager(this);
			base.Content.RootDirectory = "Content";
			this.graphics.DeviceDisposing += this.graphics_DeviceDisposing;
			this.graphics.DeviceResetting += this.graphics_DeviceResetting;
			this.graphics.DeviceReset += this.graphics_DeviceReset;
			PlatformAPISettings.InitPlatformAPI();
			SettingsLoader.checkStatus();
			if (SettingsLoader.didLoad)
			{
				this.CanLoadContent = true;
				this.graphics.PreferredBackBufferWidth = Math.Min(SettingsLoader.resWidth, 4096);
				this.graphics.PreferredBackBufferHeight = Math.Min(SettingsLoader.resHeight, 4096);
				this.graphics.IsFullScreen = SettingsLoader.isFullscreen;
				this.graphics.PreferMultiSampling = SettingsLoader.ShouldMultisample;
				if (Settings.StartOnAltMonitor & !SettingsLoader.isFullscreen)
				{
					EventHandler<PreparingDeviceSettingsEventArgs> value = new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettingsForAltMonitor);
					this.graphics.PreparingDeviceSettings += value;
				}
				Console.WriteLine("Loaded Settings - Language Code: " + Settings.ActiveLocale);
			}
			else
			{
				this.graphics.PreferMultiSampling = true;
				if (Settings.windowed)
				{
					this.graphics.PreferredBackBufferWidth = 1280;
					this.graphics.PreferredBackBufferHeight = 800;
					this.CanLoadContent = true;
				}
				else
				{
					this.graphicsPreparedHandler = new EventHandler<PreparingDeviceSettingsEventArgs>(this.graphics_PreparingDeviceSettings);
					this.graphics.PreparingDeviceSettings += this.graphicsPreparedHandler;
					this.graphics.PreferredBackBufferWidth = 1280;
					this.graphics.PreferredBackBufferHeight = 800;
					this.graphics.IsFullScreen = true;
				}
				this.NeedsSettingsLocaleActivation = true;
				Console.WriteLine("Settings file not found, setting defaults.");
			}
			this.CheckAndFixWindowPosition();
			base.IsMouseVisible = true;
			base.IsFixedTimeStep = false;
			Game1.singleton = this;
			base.Exiting += this.handleExit;
			StatsManager.InitStats();
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x000906C5 File Offset: 0x0008E8C5
		private void GraphicsDevice_DeviceLost(object sender, EventArgs e)
		{
			Console.WriteLine("Device Lost...");
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x000906D4 File Offset: 0x0008E8D4
		private void graphics_DeviceReset(object sender, EventArgs e)
		{
			Program.GraphicsDeviceResetLog = Program.GraphicsDeviceResetLog + "Reset at " + DateTime.Now.ToShortTimeString();
			Console.WriteLine("Graphics Device Reset Started");
			if (Utils.white != null)
			{
				Utils.white.Dispose();
				Utils.white = null;
			}
			this.LoadRegenSafeContent();
			this.HasLoadedContent = true;
			Console.WriteLine("Graphics Device Reset Complete");
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x00090746 File Offset: 0x0008E946
		private void graphics_DeviceResetting(object sender, EventArgs e)
		{
			this.HasLoadedContent = false;
			Console.WriteLine("Graphics Device Resetting");
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x0009075B File Offset: 0x0008E95B
		private void graphics_DeviceDisposing(object sender, EventArgs e)
		{
			this.HasLoadedContent = false;
			Console.WriteLine("Graphics Device Disposing");
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00090770 File Offset: 0x0008E970
		private void graphics_PreparingDeviceSettingsForAltMonitor(object sender, PreparingDeviceSettingsEventArgs e)
		{
			foreach (GraphicsAdapter graphicsAdapter in GraphicsAdapter.Adapters)
			{
				if (!graphicsAdapter.IsDefaultAdapter)
				{
					e.GraphicsDeviceInformation.Adapter = graphicsAdapter;
					break;
				}
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x000907DC File Offset: 0x0008E9DC
		private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			try
			{
				DisplayMode currentDisplayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
				int width = currentDisplayMode.Width;
				int height = currentDisplayMode.Height;
				this.graphics.PreferredBackBufferWidth = Math.Min(width, 4096);
				this.graphics.PreferredBackBufferHeight = Math.Min(height, 4096);
				this.graphics.PreferMultiSampling = true;
				this.resolutionSet = false;
				this.graphics.PreparingDeviceSettings -= this.graphicsPreparedHandler;
				this.CanLoadContent = true;
			}
			catch (Exception ex)
			{
				string data = Utils.GenerateReportFromException(ex);
				string text = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				Utils.writeToFile(data, text + "Graphics_CrashReport_" + Guid.NewGuid().ToString().Replace(" ", "_") + ".txt");
				this.graphics.PreferredBackBufferWidth = 1280;
				this.graphics.PreferredBackBufferHeight = 800;
				this.graphics.PreferMultiSampling = true;
				this.graphics.IsFullScreen = false;
			}
		}

		// Token: 0x06000886 RID: 2182 RVA: 0x00090924 File Offset: 0x0008EB24
		public void setNewGraphics()
		{
			this.graphics.ApplyChanges();
			GameScreen[] screens = this.sman.GetScreens();
			bool flag = false;
			string saveGameUserName = null;
			string saveUserAccountName = null;
			for (int i = 0; i < screens.Length; i++)
			{
				OS os = screens[i] as OS;
				if (os != null)
				{
					os.threadedSaveExecute(false);
					flag = true;
					saveGameUserName = os.SaveGameUserName;
					saveUserAccountName = os.SaveUserAccountName;
					break;
				}
			}
			base.Components.Remove(this.sman);
			this.sman = new ScreenManager(this);
			base.Components.Add(this.sman);
			this.LoadGraphicsContent();
			if (flag)
			{
				OS.WillLoadSave = true;
				OS os = new OS();
				os.SaveGameUserName = saveGameUserName;
				os.SaveUserAccountName = saveUserAccountName;
				bool isInExtensionMode = Settings.IsInExtensionMode;
				MainMenu.resetOS();
				Settings.IsInExtensionMode = isInExtensionMode;
				this.sman.AddScreen(os, new PlayerIndex?(this.sman.controllingPlayer));
			}
			GuiData.spriteBatch = this.sman.SpriteBatch;
			if (this.sman.GetScreens().Length == 0)
			{
				this.LoadInitialScreens();
			}
		}

		// Token: 0x06000887 RID: 2183 RVA: 0x00090A60 File Offset: 0x0008EC60
		private void CheckAndFixWindowPosition()
		{
			int val;
			int val2;
			SDL.SDL_GetWindowPosition(base.Window.Handle, out val, out val2);
			if (!this.graphics.IsFullScreen)
			{
				SDL.SDL_SetWindowPosition(base.Window.Handle, Math.Max(val, 10), Math.Max(val2, 10));
			}
		}

		// Token: 0x06000888 RID: 2184 RVA: 0x00090AB8 File Offset: 0x0008ECB8
		public void setWindowPosition(Vector2 pos)
		{
			if (!SettingsLoader.isFullscreen)
			{
				base.Window.IsBorderlessEXT = true;
				SDL.SDL_SetWindowPosition(base.Window.Handle, (int)pos.X, (int)pos.Y);
			}
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x00090B00 File Offset: 0x0008ED00
		protected override void Initialize()
		{
			this.sman = new ScreenManager(this);
			base.Components.Add(this.sman);
			this.graphics.PreferMultiSampling = true;
			NameGenerator.init();
			PatternDrawer.init(base.Content);
			ProgramList.init();
			Cube3D.Initilize(this.graphics.GraphicsDevice);
			AlienwareFXManager.Init();
			this.graphics.GraphicsDevice.DeviceLost += this.GraphicsDevice_DeviceLost;
			base.Initialize();
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00090B8C File Offset: 0x0008ED8C
		private void handleExit(object sender, EventArgs e)
		{
			Game1.threadsExiting = true;
			MusicManager.stop();
			AlienwareFXManager.ReleaseHandle();
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x00090BA4 File Offset: 0x0008EDA4
		private void LoadRegenSafeContent()
		{
			Utils.white = new Texture2D(this.graphics.GraphicsDevice, 1, 1);
			Color[] data = new Color[]
			{
				Color.White
			};
			Utils.white.SetData<Color>(data);
			ContentManager content = base.Content;
			Utils.gradient = content.Load<Texture2D>("Gradient");
			Utils.gradientLeftRight = content.Load<Texture2D>("GradientHorizontal");
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x00090C18 File Offset: 0x0008EE18
		protected override void LoadContent()
		{
			if (this.CanLoadContent)
			{
				PortExploits.populate();
				this.sman.controllingPlayer = PlayerIndex.One;
				if (Settings.isConventionDemo)
				{
					this.setWindowPosition(new Vector2(200f, 200f));
				}
				this.LoadGraphicsContent();
				this.LoadRegenSafeContent();
				ContentManager content = base.Content;
				GuiData.font = content.Load<SpriteFont>("Font23");
				GuiData.titlefont = content.Load<SpriteFont>("Kremlin");
				GuiData.smallfont = base.Content.Load<SpriteFont>("Font12");
				GuiData.UISmallfont = GuiData.smallfont;
				GuiData.tinyfont = base.Content.Load<SpriteFont>("Font10");
				GuiData.UITinyfont = base.Content.Load<SpriteFont>("Font10");
				GuiData.detailfont = base.Content.Load<SpriteFont>("Font7");
				GuiData.spriteBatch = this.sman.SpriteBatch;
				GuiData.InitFontOptions(base.Content);
				GuiData.init(base.Window);
				DLC1SessionUpgrader.CheckForDLCFiles();
				VehicleInfo.init();
				WorldLocationLoader.init();
				ThemeManager.init(content);
				MissionGenerationParser.init();
				MissionGenerator.init(content);
				UsernameGenerator.init();
				MusicManager.init(content);
				SFX.init(content);
				OldSystemSaveFileManifest.Load();
				try
				{
					SaveFileManager.Init(true);
				}
				catch (UnauthorizedAccessException ex)
				{
					MainMenu.AccumErrors += " ---- WARNING ---\nHacknet cannot access the Save File Folder (Path Below) to read/write save files.\nNO PROGRESS WILL BE SAVED.\n";
					MainMenu.AccumErrors += "Check folder permissions, run Hacknet.exe as Administrator, and try again.\n";
					MainMenu.AccumErrors += "If Errors Persist, search for \"Hacknet Workaround\" for a steam forums thread with more options.\n";
					MainMenu.AccumErrors = MainMenu.AccumErrors + ":: Error Details ::\n" + Utils.GenerateReportFromException(ex);
				}
				if (this.NeedsSettingsLocaleActivation)
				{
					if (!Settings.ForceEnglish)
					{
						string codeForActiveLanguage = PlatformAPISettings.GetCodeForActiveLanguage(LocaleActivator.SupportedLanguages);
						LocaleActivator.ActivateLocale(codeForActiveLanguage, base.Content);
						Settings.ActiveLocale = codeForActiveLanguage;
					}
				}
				else if (SettingsLoader.didLoad && Settings.ActiveLocale != "en-us")
				{
					LocaleActivator.ActivateLocale(Settings.ActiveLocale, base.Content);
				}
				Helpfile.init();
				FileEntry.init(base.Content);
				this.HasLoadedContent = true;
				this.LoadInitialScreens();
				if (WebRenderer.Enabled)
				{
					XNAWebRenderer.XNAWR_Initialize("file:///nope.html", WebRenderer.textureUpdated, 512, 512);
					WebRenderer.setSize(512, 512);
				}
			}
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x00090EA4 File Offset: 0x0008F0A4
		protected void LoadGraphicsContent()
		{
			this.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			if (this.NeedsSettingsLocaleActivation)
			{
				SettingsLoader.ShouldMultisample = base.GraphicsDevice.Adapter.IsProfileSupported(GraphicsProfile.HiDef);
			}
			PostProcessor.init(this.graphics.GraphicsDevice, this.spriteBatch, base.Content);
			WebRenderer.init(this.graphics.GraphicsDevice);
		}

		// Token: 0x0600088E RID: 2190 RVA: 0x00090F18 File Offset: 0x0008F118
		protected void LoadInitialScreens()
		{
			if (Settings.ForceEnglish)
			{
				LocaleActivator.ActivateLocale(Settings.ActiveLocale, base.Content);
			}
			if (Game1.AutoLoadExtensionPath != null)
			{
				ExtensionLoader.ActiveExtensionInfo = ExtensionInfo.ReadExtensionInfo(Game1.AutoLoadExtensionPath);
				SaveFileManager.Init(true);
				SaveFileManager.DeleteUser("test");
				MainMenu.CreateNewAccountForExtensionAndStart("test", "test", this.sman, null, null);
			}
			else if (Settings.MenuStartup)
			{
				this.sman.AddScreen(new MainMenu(), new PlayerIndex?(this.sman.controllingPlayer));
			}
			else
			{
				this.sman.AddScreen(new OS(), new PlayerIndex?(this.sman.controllingPlayer));
			}
		}

		// Token: 0x0600088F RID: 2191 RVA: 0x00090FE4 File Offset: 0x0008F1E4
		protected override void UnloadContent()
		{
			if (WebRenderer.Enabled)
			{
				XNAWebRenderer.XNAWR_Shutdown();
			}
		}

		// Token: 0x06000890 RID: 2192 RVA: 0x00091008 File Offset: 0x0008F208
		protected override void Update(GameTime gameTime)
		{
			if (!this.HasLoadedContent)
			{
				this.LoadContent();
			}
			if (WebRenderer.Enabled)
			{
				XNAWebRenderer.XNAWR_Update();
			}
			if (!this.resolutionSet)
			{
				this.setNewGraphics();
				this.resolutionSet = true;
			}
			PatternDrawer.update((float)gameTime.ElapsedGameTime.TotalSeconds);
			GuiData.setTimeStep((float)gameTime.ElapsedGameTime.TotalSeconds);
			MusicManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			ThemeManager.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
			AlienwareFXManager.UpdateForOS(OS.currentInstance);
			base.Update(gameTime);
		}

		// Token: 0x06000891 RID: 2193 RVA: 0x000910BC File Offset: 0x0008F2BC
		protected override void Draw(GameTime gameTime)
		{
			if (this.HasLoadedContent)
			{
				this.IsDrawing = true;
				base.Draw(gameTime);
				this.IsDrawing = false;
			}
		}

		// Token: 0x06000892 RID: 2194 RVA: 0x000910F0 File Offset: 0x0008F2F0
		public static Game1 getSingleton()
		{
			return Game1.singleton;
		}

		// Token: 0x040009EC RID: 2540
		private static Game1 singleton;

		// Token: 0x040009ED RID: 2541
		public static bool threadsExiting;

		// Token: 0x040009EE RID: 2542
		public static CultureInfo culture;

		// Token: 0x040009EF RID: 2543
		public static CultureInfo OriginalCultureInfo;

		// Token: 0x040009F0 RID: 2544
		public GraphicsDeviceManager graphics;

		// Token: 0x040009F1 RID: 2545
		public GraphicsDeviceInformation graphicsInfo;

		// Token: 0x040009F2 RID: 2546
		private SpriteBatch spriteBatch;

		// Token: 0x040009F3 RID: 2547
		public ScreenManager sman;

		// Token: 0x040009F4 RID: 2548
		private bool resolutionSet = true;

		// Token: 0x040009F5 RID: 2549
		private bool IsDrawing = false;

		// Token: 0x040009F6 RID: 2550
		private bool CanLoadContent = false;

		// Token: 0x040009F7 RID: 2551
		private bool HasLoadedContent = false;

		// Token: 0x040009F8 RID: 2552
		private bool NeedsSettingsLocaleActivation = false;

		// Token: 0x040009F9 RID: 2553
		private EventHandler<PreparingDeviceSettingsEventArgs> graphicsPreparedHandler;

		// Token: 0x040009FA RID: 2554
		public static string AutoLoadExtensionPath = null;
	}
}
