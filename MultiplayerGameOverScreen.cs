using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200013E RID: 318
	internal class MultiplayerGameOverScreen : GameScreen
	{
		// Token: 0x06000793 RID: 1939 RVA: 0x0007CC74 File Offset: 0x0007AE74
		public MultiplayerGameOverScreen(bool winner)
		{
			this.isWinner = winner;
			base.TransitionOnTime = TimeSpan.FromSeconds(3.5);
			base.TransitionOffTime = TimeSpan.FromSeconds(0.20000000298023224);
			base.IsPopup = true;
		}

		// Token: 0x06000794 RID: 1940 RVA: 0x0007CCC4 File Offset: 0x0007AEC4
		public override void LoadContent()
		{
			base.LoadContent();
			this.winBacking = new Color(5, 30, 8);
			this.lossBacking = new Color(34, 8, 5);
			this.winPattern = new Color(11, 66, 23);
			this.lossPattern = new Color(86, 11, 11);
			this.screenWidth = base.ScreenManager.GraphicsDevice.Viewport.Width;
			this.screenHeight = base.ScreenManager.GraphicsDevice.Viewport.Height;
			this.contentRect = new Rectangle(0, this.screenHeight / 6, this.screenWidth, this.screenHeight - this.screenHeight / 3);
			this.font = base.ScreenManager.Game.Content.Load<SpriteFont>("Kremlin");
		}

		// Token: 0x06000795 RID: 1941 RVA: 0x0007CD9E File Offset: 0x0007AF9E
		public override void HandleInput(InputState input)
		{
			base.HandleInput(input);
			GuiData.doInput(input);
		}

		// Token: 0x06000796 RID: 1942 RVA: 0x0007CDB0 File Offset: 0x0007AFB0
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			base.ScreenManager.FadeBackBufferToBlack((int)Math.Min(base.TransitionAlpha, 150));
			GuiData.startDraw();
			float scale = 1f - base.TransitionPosition;
			PatternDrawer.draw(this.contentRect, 1f, (this.isWinner ? this.winBacking : this.lossBacking) * scale, (this.isWinner ? this.winPattern : this.lossPattern) * scale, GuiData.spriteBatch);
			string text = this.isWinner ? "VICTORY" : "DEFEAT";
			Vector2 pos = this.font.MeasureString(text);
			pos.X = (float)(this.contentRect.X + this.contentRect.Width / 2) - pos.X / 2f;
			pos.Y = (float)(this.contentRect.Y + this.contentRect.Height / 2) - pos.Y / 2f;
			TextItem.DrawShadow = false;
			TextItem.doFontLabel(pos, text, this.font, new Color?(Color.White * scale), float.MaxValue, float.MaxValue, false);
			if (Button.doButton(1008, this.contentRect.X + 10, this.contentRect.Y + this.contentRect.Height - 60, 230, 55, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
			{
				if (OS.currentInstance != null)
				{
					OS.currentInstance.ExitScreen();
				}
				base.ExitScreen();
				base.ScreenManager.AddScreen(new MainMenu());
			}
			GuiData.endDraw();
		}

		// Token: 0x04000893 RID: 2195
		private Rectangle contentRect;

		// Token: 0x04000894 RID: 2196
		private int screenWidth;

		// Token: 0x04000895 RID: 2197
		private int screenHeight;

		// Token: 0x04000896 RID: 2198
		private bool isWinner;

		// Token: 0x04000897 RID: 2199
		private Color winBacking;

		// Token: 0x04000898 RID: 2200
		private Color winPattern;

		// Token: 0x04000899 RID: 2201
		private Color lossBacking;

		// Token: 0x0400089A RID: 2202
		private Color lossPattern;

		// Token: 0x0400089B RID: 2203
		private SpriteFont font;
	}
}
