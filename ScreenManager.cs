using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200015A RID: 346
	public class ScreenManager : DrawableGameComponent
	{
		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x00091AF8 File Offset: 0x0008FCF8
		public SpriteBatch SpriteBatch
		{
			get
			{
				return this.spriteBatch;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060008AD RID: 2221 RVA: 0x00091B10 File Offset: 0x0008FD10
		public SpriteFont Font
		{
			get
			{
				return this.font;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x00091B28 File Offset: 0x0008FD28
		// (set) Token: 0x060008AF RID: 2223 RVA: 0x00091B40 File Offset: 0x0008FD40
		public bool TraceEnabled
		{
			get
			{
				return this.traceEnabled;
			}
			set
			{
				this.traceEnabled = value;
			}
		}

		// Token: 0x060008B0 RID: 2224 RVA: 0x00091B4A File Offset: 0x0008FD4A
		public ScreenManager(Game game) : base(game)
		{
		}

		// Token: 0x060008B1 RID: 2225 RVA: 0x00091B89 File Offset: 0x0008FD89
		public override void Initialize()
		{
			base.Initialize();
			this.isInitialized = true;
			this.traceEnabled = false;
		}

		// Token: 0x060008B2 RID: 2226 RVA: 0x00091BA4 File Offset: 0x0008FDA4
		protected override void LoadContent()
		{
			ContentManager content = base.Game.Content;
			this.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			GuiData.spriteBatch = new SpriteBatch(base.GraphicsDevice);
			this.font = content.Load<SpriteFont>("Font12");
			this.blankTexture = new Texture2D(base.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
			this.blankTexture.SetData<Color>(new Color[]
			{
				Color.White
			});
			try
			{
				this.alertSound = content.Load<SoundEffect>("SFX/Bip");
			}
			catch (Exception)
			{
				Settings.soundDisabled = true;
			}
			foreach (GameScreen gameScreen in this.screens)
			{
				gameScreen.LoadContent();
			}
			this.controllingPlayer = PlayerIndex.One;
		}

		// Token: 0x060008B3 RID: 2227 RVA: 0x00091CB0 File Offset: 0x0008FEB0
		protected override void UnloadContent()
		{
			foreach (GameScreen gameScreen in this.screens)
			{
				gameScreen.UnloadContent();
			}
		}

		// Token: 0x060008B4 RID: 2228 RVA: 0x00091D0C File Offset: 0x0008FF0C
		public override void Update(GameTime gameTime)
		{
			GameScreen gameScreen = null;
			try
			{
				bool flag = this.usingGamePad;
				this.usingGamePad = false;
				for (int i = 0; i < this.input.CurrentGamePadStates.Length; i++)
				{
					if (this.input.CurrentGamePadStates[i].IsConnected)
					{
						this.usingGamePad = true;
					}
				}
				this.input.Update();
				this.screensToUpdate.Clear();
				foreach (GameScreen item in this.screens)
				{
					this.screensToUpdate.Add(item);
				}
				if (this.screensToUpdate.Count == 0)
				{
					foreach (GameScreen item in this.screens)
					{
						this.screensToUpdate.Add(item);
					}
				}
				bool flag2 = !base.Game.IsActive;
				bool coveredByOtherScreen = false;
				bool flag3 = false;
				while (this.screensToUpdate.Count > 0)
				{
					gameScreen = this.screensToUpdate[this.screensToUpdate.Count - 1];
					this.screensToUpdate.RemoveAt(this.screensToUpdate.Count - 1);
					if (!flag2)
					{
						if (gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active)
						{
							if (flag != this.usingGamePad)
							{
								gameScreen.inputMethodChanged(this.usingGamePad);
							}
							gameScreen.HandleInput(this.input);
							flag3 = true;
						}
					}
					gameScreen.Update(gameTime, flag2, coveredByOtherScreen);
					if (flag3)
					{
						flag2 = true;
					}
					if (gameScreen.ScreenState == ScreenState.TransitionOn || gameScreen.ScreenState == ScreenState.Active)
					{
						if (!gameScreen.IsPopup)
						{
							coveredByOtherScreen = true;
						}
					}
				}
				if (this.traceEnabled)
				{
					this.TraceScreens();
				}
			}
			catch (Exception ex)
			{
				if (gameScreen != null)
				{
					this.RemoveScreen(gameScreen);
				}
			}
		}

		// Token: 0x060008B5 RID: 2229 RVA: 0x00091FB4 File Offset: 0x000901B4
		private void TraceScreens()
		{
			List<string> list = new List<string>();
			foreach (GameScreen gameScreen in this.screens)
			{
				list.Add(gameScreen.GetType().Name);
			}
		}

		// Token: 0x060008B6 RID: 2230 RVA: 0x00092020 File Offset: 0x00090220
		public override void Draw(GameTime gameTime)
		{
			base.GraphicsDevice.Clear(this.screenFillColor);
			for (int i = 0; i < this.screens.Count; i++)
			{
				if (this.screens[i].ScreenState != ScreenState.Hidden)
				{
					this.screens[i].Draw(gameTime);
				}
			}
		}

		// Token: 0x060008B7 RID: 2231 RVA: 0x0009208C File Offset: 0x0009028C
		private void handleCriticalErrorBoxAccepted(object sender, PlayerIndexEventArgs e)
		{
			try
			{
				this.spriteBatch.End();
			}
			catch
			{
			}
			this.handleCriticalError();
		}

		// Token: 0x060008B8 RID: 2232 RVA: 0x000920C8 File Offset: 0x000902C8
		public void handleCriticalError()
		{
			if (this.screens.Count <= 1)
			{
				this.AddScreen(new MainMenu(), new PlayerIndex?(this.controllingPlayer));
			}
		}

		// Token: 0x060008B9 RID: 2233 RVA: 0x00092101 File Offset: 0x00090301
		public void AddScreen(GameScreen screen)
		{
			this.AddScreen(screen, new PlayerIndex?(this.controllingPlayer));
		}

		// Token: 0x060008BA RID: 2234 RVA: 0x00092118 File Offset: 0x00090318
		public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
		{
			screen.ControllingPlayer = controllingPlayer;
			screen.ScreenManager = this;
			screen.IsExiting = false;
			if (this.isInitialized)
			{
				screen.LoadContent();
			}
			this.screens.Add(screen);
		}

		// Token: 0x060008BB RID: 2235 RVA: 0x00092161 File Offset: 0x00090361
		public void ShowPopup(string message)
		{
			this.AddScreen(new MessageBoxScreen(this.clipStringForMessageBox(message), false), new PlayerIndex?(this.controllingPlayer));
		}

		// Token: 0x060008BC RID: 2236 RVA: 0x00092184 File Offset: 0x00090384
		public void playAlertSound()
		{
			if (!Settings.soundDisabled)
			{
				this.alertSound.Play(0.3f, 0f, 0f);
			}
		}

		// Token: 0x060008BD RID: 2237 RVA: 0x000921B8 File Offset: 0x000903B8
		public void RemoveScreen(GameScreen screen)
		{
			if (this.isInitialized)
			{
				screen.UnloadContent();
			}
			this.screens.Remove(screen);
			this.screensToUpdate.Remove(screen);
			if (this.screens.Count <= 0)
			{
				this.handleCriticalError();
			}
		}

		// Token: 0x060008BE RID: 2238 RVA: 0x00092214 File Offset: 0x00090414
		public GameScreen[] GetScreens()
		{
			return this.screens.ToArray();
		}

		// Token: 0x060008BF RID: 2239 RVA: 0x00092234 File Offset: 0x00090434
		public void FadeBackBufferToBlack(int alpha)
		{
			Viewport viewport = base.GraphicsDevice.Viewport;
			this.spriteBatch.Begin();
			this.spriteBatch.Draw(this.blankTexture, new Rectangle(0, 0, viewport.Width, viewport.Height), new Color(0, 0, 0, (int)((byte)alpha)));
			this.spriteBatch.End();
		}

		// Token: 0x060008C0 RID: 2240 RVA: 0x00092298 File Offset: 0x00090498
		public string clipStringForMessageBox(string s)
		{
			string text = "";
			int num = 0;
			foreach (char c in s)
			{
				if (c != '\n')
				{
					text += c;
				}
				else
				{
					text += ' ';
				}
				num++;
				if (num > 50)
				{
					text += '\n';
					num = 0;
				}
			}
			return text;
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x0009232B File Offset: 0x0009052B
		private void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
		{
		}

		// Token: 0x04000A13 RID: 2579
		private List<GameScreen> screens = new List<GameScreen>();

		// Token: 0x04000A14 RID: 2580
		private List<GameScreen> screensToUpdate = new List<GameScreen>();

		// Token: 0x04000A15 RID: 2581
		private InputState input = new InputState();

		// Token: 0x04000A16 RID: 2582
		private SpriteBatch spriteBatch;

		// Token: 0x04000A17 RID: 2583
		private SpriteFont font;

		// Token: 0x04000A18 RID: 2584
		private SoundEffect alertSound;

		// Token: 0x04000A19 RID: 2585
		public SpriteFont hugeFont;

		// Token: 0x04000A1A RID: 2586
		private Texture2D blankTexture;

		// Token: 0x04000A1B RID: 2587
		private bool isInitialized;

		// Token: 0x04000A1C RID: 2588
		private bool traceEnabled;

		// Token: 0x04000A1D RID: 2589
		public PlayerIndex controllingPlayer;

		// Token: 0x04000A1E RID: 2590
		public Color screenFillColor = Color.Black;

		// Token: 0x04000A1F RID: 2591
		public bool usingGamePad = false;

		// Token: 0x04000A20 RID: 2592
		public AudioEngine audioEngine;

		// Token: 0x04000A21 RID: 2593
		public WaveBank waveBank;

		// Token: 0x04000A22 RID: 2594
		public WaveBank musicBank;

		// Token: 0x04000A23 RID: 2595
		public SoundBank soundBank;
	}
}
