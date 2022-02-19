using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200012D RID: 301
	internal class MessageBoxScreen : GameScreen
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x0600072C RID: 1836 RVA: 0x00075BE4 File Offset: 0x00073DE4
		// (remove) Token: 0x0600072D RID: 1837 RVA: 0x00075C20 File Offset: 0x00073E20
		public event EventHandler<PlayerIndexEventArgs> Accepted;

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600072E RID: 1838 RVA: 0x00075C5C File Offset: 0x00073E5C
		// (remove) Token: 0x0600072F RID: 1839 RVA: 0x00075C98 File Offset: 0x00073E98
		public event EventHandler<PlayerIndexEventArgs> Cancelled;

		// Token: 0x06000730 RID: 1840 RVA: 0x00075CD4 File Offset: 0x00073ED4
		public MessageBoxScreen(string message) : this(message, false)
		{
		}

		// Token: 0x06000731 RID: 1841 RVA: 0x00075CE4 File Offset: 0x00073EE4
		public MessageBoxScreen(string message, bool includeUsageText)
		{
			if (includeUsageText)
			{
				this.message = message + "\nA button, Space, Enter = ok\nB button, Esc = cancel";
			}
			else
			{
				this.message = message;
			}
			base.IsPopup = true;
			base.TransitionOnTime = TimeSpan.FromSeconds(0.2);
			base.TransitionOffTime = TimeSpan.FromSeconds(0.05);
		}

		// Token: 0x06000732 RID: 1842 RVA: 0x00075D63 File Offset: 0x00073F63
		public MessageBoxScreen(string message, bool includesUsageText, bool hasEscPrompt) : this(message, false)
		{
			this.hasEscGuide = hasEscPrompt;
		}

		// Token: 0x06000733 RID: 1843 RVA: 0x00075D78 File Offset: 0x00073F78
		public override void LoadContent()
		{
			ContentManager content = base.ScreenManager.Game.Content;
			this.top = TextureBank.load("PopupFrame", content);
			this.mid = TextureBank.load("PopupFrame", content);
			if (base.ScreenManager.usingGamePad)
			{
				this.bottom = TextureBank.load("PopupBase", content);
			}
			else
			{
				this.bottom = TextureBank.load("PopupBase", content);
			}
			float num = GuiData.font.MeasureString(this.message).Y + MessageBoxScreen.HEIGHT_BUFFER;
			this.topLeft = new Vector2((float)base.ScreenManager.GraphicsDevice.Viewport.Width / 2f - (float)this.top.Width / 2f, (float)base.ScreenManager.GraphicsDevice.Viewport.Height / 2f - num / 2f - (float)this.top.Height);
			this.contentBounds = new Rectangle((int)this.topLeft.X, (int)this.topLeft.Y + this.top.Height, this.top.Width, (int)num);
			if (this.hasEscGuide)
			{
				this.guideFont = GuiData.font;
			}
		}

		// Token: 0x06000734 RID: 1844 RVA: 0x00075ED8 File Offset: 0x000740D8
		public override void HandleInput(InputState input)
		{
			PlayerIndex playerIndex;
			if (input.IsMenuSelect(base.ControllingPlayer, out playerIndex))
			{
				if (this.Accepted != null)
				{
					this.Accepted(this, new PlayerIndexEventArgs(playerIndex));
				}
				if (this.AcceptedClicked != null)
				{
					this.AcceptedClicked();
				}
				base.ExitScreen();
			}
			else if (input.IsMenuCancel(base.ControllingPlayer, out playerIndex))
			{
				if (this.Cancelled != null)
				{
					this.Cancelled(this, new PlayerIndexEventArgs(playerIndex));
				}
				if (this.CancelClicked != null)
				{
					this.CancelClicked();
				}
				base.ExitScreen();
			}
			GuiData.doInput(input);
		}

		// Token: 0x06000735 RID: 1845 RVA: 0x00075FA4 File Offset: 0x000741A4
		public override void Draw(GameTime gameTime)
		{
			SpriteBatch spriteBatch = base.ScreenManager.SpriteBatch;
			SpriteFont font = GuiData.font;
			base.ScreenManager.FadeBackBufferToBlack((int)Math.Min(base.TransitionAlpha, 140));
			Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
			Vector2 value = new Vector2((float)viewport.Width, (float)viewport.Height);
			Vector2 value2 = font.MeasureString(this.message);
			Vector2 position = value / 2f - value2 / 2f;
			Color color = new Color(22, 22, 22, 255);
			float num = 1f - base.TransitionPosition;
			if (base.ScreenState != ScreenState.TransitionOff)
			{
				spriteBatch.Begin();
				spriteBatch.Draw(this.top, this.topLeft, color);
				spriteBatch.Draw(this.mid, this.contentBounds, color);
				spriteBatch.Draw(this.top, this.topLeft + new Vector2(0f, (float)(this.top.Height + this.contentBounds.Height)), color);
				spriteBatch.DrawString(font, this.message, position, Color.White);
				int num2 = 150;
				int x = this.contentBounds.X + this.contentBounds.Width - (num2 + 5);
				int y = this.contentBounds.Y + this.contentBounds.Height + this.top.Height - 40;
				string input = (this.OverrideCancelText == null) ? "Resume" : this.OverrideCancelText;
				if (Button.doButton(331, x, y, num2, 27, LocaleTerms.Loc(input), null))
				{
					if (this.Cancelled != null)
					{
						this.Cancelled(this, new PlayerIndexEventArgs(base.ScreenManager.controllingPlayer));
					}
					if (this.CancelClicked != null)
					{
						this.CancelClicked();
					}
					base.ExitScreen();
				}
				x = this.contentBounds.X + 5;
				string input2 = (this.OverrideAcceptedText == null) ? "Quit Hacknet" : this.OverrideAcceptedText;
				if (Button.doButton(332, x, y, num2, 27, LocaleTerms.Loc(input2), null))
				{
					if (this.Accepted != null)
					{
						this.Accepted(this, new PlayerIndexEventArgs(base.ScreenManager.controllingPlayer));
					}
					if (this.AcceptedClicked != null)
					{
						this.AcceptedClicked();
					}
					base.ExitScreen();
				}
				spriteBatch.End();
			}
		}

		// Token: 0x06000736 RID: 1846 RVA: 0x00076277 File Offset: 0x00074477
		public override void inputMethodChanged(bool usingGamePad)
		{
		}

		// Token: 0x04000805 RID: 2053
		public static float HEIGHT_BUFFER = 20f;

		// Token: 0x04000806 RID: 2054
		private string message;

		// Token: 0x04000807 RID: 2055
		private Texture2D top;

		// Token: 0x04000808 RID: 2056
		private Texture2D mid;

		// Token: 0x04000809 RID: 2057
		private Texture2D bottom;

		// Token: 0x0400080A RID: 2058
		private Texture2D inputGuide;

		// Token: 0x0400080B RID: 2059
		private SpriteFont guideFont;

		// Token: 0x0400080C RID: 2060
		private Rectangle contentBounds;

		// Token: 0x0400080D RID: 2061
		private Vector2 topLeft;

		// Token: 0x0400080E RID: 2062
		private bool hasEscGuide = false;

		// Token: 0x0400080F RID: 2063
		public string OverrideAcceptedText = null;

		// Token: 0x04000810 RID: 2064
		public string OverrideCancelText = null;

		// Token: 0x04000813 RID: 2067
		public Action AcceptedClicked;

		// Token: 0x04000814 RID: 2068
		public Action CancelClicked;
	}
}
