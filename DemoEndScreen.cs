using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x020000EF RID: 239
	public class DemoEndScreen : GameScreen
	{
		// Token: 0x06000512 RID: 1298 RVA: 0x0004EFD4 File Offset: 0x0004D1D4
		public override void LoadContent()
		{
			base.LoadContent();
			this.Fullscreen = new Rectangle(0, 0, base.ScreenManager.GraphicsDevice.Viewport.Width, base.ScreenManager.GraphicsDevice.Viewport.Height);
			if (this.StopsMusic)
			{
				MediaPlayer.Stop();
			}
			PostProcessor.EndingSequenceFlashOutActive = false;
			PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
			PostProcessor.dangerModeEnabled = false;
			if (this.IsDLCDemoScreen)
			{
				this.pointEffect.Init(base.ScreenManager.Game.Content);
				this.pointEffect.GravityConstant = 2E-05f;
				this.pointEffect.GlowScaleMod = 5f;
				this.pointEffect.CircleTex = base.ScreenManager.Game.Content.Load<Texture2D>("NodeCircle");
				this.pointEffect.NodeColor = Utils.AddativeRed * 0.5f;
				this.pointEffect.Explode(300);
				this.pointEffect.timeRemainingWithoutAttract = 2f;
				this.pointEffect.LineTexture = Utils.gradientLeftRight;
				this.pointEffect.LineLengthPercentage = 1f;
				OS.currentInstance.highlightColor = Utils.AddativeRed;
				this.HexBackground = new HexGridBackground(base.ScreenManager.Game.Content);
			}
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0004F148 File Offset: 0x0004D348
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			if (this.IsDLCDemoScreen)
			{
				float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
				this.pointEffect.Update(dt);
				this.HexBackground.Update(dt);
			}
			if (!otherScreenHasFocus && !coveredByOtherScreen)
			{
				this.timeOnThisScreen += gameTime.ElapsedGameTime.TotalSeconds;
			}
			if (this.timeOnThisScreen > 5.0 && Keyboard.GetState().GetPressedKeys().Length > 0)
			{
				if (Settings.isConventionDemo)
				{
					OS.currentInstance.ScreenManager.AddScreen(new MainMenu());
					OS.currentInstance.ExitScreen();
					OS.currentInstance = null;
				}
				else
				{
					Game1.getSingleton().Exit();
				}
			}
			else if (this.timeOnThisScreen > (double)(this.IsDLCDemoScreen ? 51.5f : 20f))
			{
				if (Settings.isConventionDemo)
				{
					OS.currentInstance.ScreenManager.AddScreen(new MainMenu());
					OS.currentInstance.ExitScreen();
					OS.currentInstance = null;
					base.ExitScreen();
					MainMenu.resetOS();
				}
				else
				{
					Game1.getSingleton().Exit();
				}
			}
			PostProcessor.EndingSequenceFlashOutActive = false;
			PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
			PostProcessor.dangerModeEnabled = false;
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0004F2C4 File Offset: 0x0004D4C4
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			PostProcessor.begin();
			GuiData.startDraw();
			GuiData.spriteBatch.Draw(Utils.white, this.Fullscreen, Color.Black);
			if (this.IsDLCDemoScreen)
			{
				this.HexBackground.Draw(Utils.GetFullscreen(), GuiData.spriteBatch, Utils.AddativeRed * 0.2f, Color.Black, HexGridBackground.ColoringAlgorithm.OutlinedSinWash, 0f);
				this.pointEffect.Render(Utils.GetFullscreen(), GuiData.spriteBatch);
				GuiData.spriteBatch.Draw(Utils.white, this.Fullscreen, Color.Black * 0.5f);
			}
			Rectangle dest = Utils.InsetRectangle(this.Fullscreen, 200);
			dest.Y = dest.Y + dest.Height / 2 - 200;
			dest.Height = 400;
			Rectangle destinationRectangle = new Rectangle(this.Fullscreen.X, dest.Y + 50, this.Fullscreen.Width, dest.Height - 148);
			GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeRed * (0.5f + Utils.randm(0.1f)));
			string text = "HACKNET";
			FlickeringTextEffect.DrawLinedFlickeringText(dest, text, this.IsDLCDemoScreen ? 5f : 18f, this.IsDLCDemoScreen ? 0.8f : 0.7f, GuiData.titlefont, null, Color.White, 6);
			dest.Y += 400;
			dest.Height = 120;
			SpriteFont font = GuiData.titlefont;
			if (Settings.ActiveLocale != "en-us")
			{
				font = GuiData.font;
			}
			string input = this.IsDLCDemoScreen ? "EXPANSION COMING DECEMBER" : "MORE SOON";
			FlickeringTextEffect.DrawFlickeringText(dest, Utils.FlipRandomChars(LocaleTerms.Loc(input), this.IsDLCDemoScreen ? 0.0045 : 0.008), -8f, 0.7f, font, null, Color.Gray);
			FlickeringTextEffect.DrawFlickeringText(dest, Utils.FlipRandomChars(LocaleTerms.Loc(input), 0.03), -8f, 0.7f, font, null, Utils.AddativeWhite * 0.15f);
			GuiData.endDraw();
			PostProcessor.end();
		}

		// Token: 0x040005A7 RID: 1447
		private Rectangle Fullscreen;

		// Token: 0x040005A8 RID: 1448
		public bool StopsMusic = true;

		// Token: 0x040005A9 RID: 1449
		public bool IsDLCDemoScreen = false;

		// Token: 0x040005AA RID: 1450
		private double timeOnThisScreen = 0.0;

		// Token: 0x040005AB RID: 1451
		private PointGatherEffect pointEffect = new PointGatherEffect();

		// Token: 0x040005AC RID: 1452
		private HexGridBackground HexBackground;
	}
}
