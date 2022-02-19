using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200000F RID: 15
	internal class LogoCustomConnectDisplayDaemon : CustomConnectDisplayDaemon
	{
		// Token: 0x0600007E RID: 126 RVA: 0x00009F74 File Offset: 0x00008174
		public LogoCustomConnectDisplayDaemon(Computer c, OS os, string logoImageName, string titleImageName, bool logoShouldClipoverdraw, string buttonAlignment) : base(c, LocaleTerms.Loc("Logo Display Override"), os)
		{
			this.isListed = false;
			this.logoImageName = logoImageName;
			this.titleImageName = titleImageName;
			this.LogoShouldClipOverdraw = logoShouldClipoverdraw;
			this.buttonAlignmentName = buttonAlignment;
			if (!Enum.TryParse<AlignmentX>(buttonAlignment, true, out this.ButtonsAlignment))
			{
				this.ButtonsAlignment = AlignmentX.Right;
			}
			string text = Utils.GetFileLoadPrefix() + logoImageName;
			if (text.EndsWith(".jpg") || text.EndsWith(".png"))
			{
				using (FileStream fileStream = File.OpenRead(text))
				{
					this.LogoImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, fileStream);
				}
			}
			else
			{
				this.LogoImage = os.content.Load<Texture2D>(logoImageName);
			}
			string text2 = Utils.GetFileLoadPrefix() + titleImageName;
			if (text2.EndsWith(".jpg") || text2.EndsWith(".png"))
			{
				using (FileStream fileStream = File.OpenRead(text2))
				{
					this.TitleImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, fileStream);
				}
			}
			else
			{
				this.TitleImage = os.content.Load<Texture2D>(titleImageName);
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000A0EC File Offset: 0x000082EC
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000A0F8 File Offset: 0x000082F8
		internal override void DrawAdminDisplay(Rectangle bounds, SpriteBatch sb, Computer c)
		{
			this.DrawLogoAndTitle(bounds, sb);
			int num = bounds.Width;
			float num2 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.timeInThisState / 1f));
			if (this.timeInThisState < 1f)
			{
				num = (int)((float)bounds.Width * num2);
			}
			float num3 = 110f;
			float num4 = 40f;
			float num5 = 40f;
			float num6 = 8f;
			float num7 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Math.Min(1f, (this.timeInThisState - 1f) / 1f)));
			if (this.timeInThisState < 1f)
			{
				num7 = 0f;
			}
			int num8 = (int)(num4 + (num3 - num4) * (1f - num7));
			if (this.timeInThisState < 1f)
			{
				num8 = (int)((float)num8 * (0.5f + 0.5f * num2));
			}
			int stripHeight = (int)(num6 + (num5 - num6) * (1f - num7));
			int num9 = bounds.Height / 2 - num8 / 2;
			num9 = (int)((float)bounds.Y + (float)num9 * (1f - num7) - 60f * num7);
			int x = bounds.X + bounds.Width / 2 - num / 2;
			this.DrawCautionLinedMessage(new Rectangle(x, num9, num, num8), stripHeight, this.os.highlightColor, (this.timeInThisState > 0.5f + Utils.randm(0.5f)) ? LocaleTerms.Loc("ACCESS GRANTED") : "", sb, PatternDrawer.warningStripe, -2);
			x = bounds.X + 20;
			num9 = bounds.Y + 80 + 2;
			if (this.timeInThisState >= 2f)
			{
				this.DrawConnectButtons(bounds, sb, c, 20, num9, this.ButtonsAlignment);
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000A2DE File Offset: 0x000084DE
		internal override void DrawNonAdminDisplay(Rectangle dest, SpriteBatch sb)
		{
			this.DrawLogoAndTitle(dest, sb);
			base.DrawNonAdminDisplay(dest, sb);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000A2F4 File Offset: 0x000084F4
		private void DrawLogoAndTitle(Rectangle dest, SpriteBatch sb)
		{
			AlignmentX align = AlignmentX.Left;
			AlignmentX align2 = AlignmentX.Right;
			if (this.ButtonsAlignment == AlignmentX.Right)
			{
				align = AlignmentX.Right;
				align2 = AlignmentX.Left;
			}
			Color color = Color.White;
			bool flag = this.comp.adminIP == this.os.thisComputer.adminIP;
			if (!flag)
			{
				color *= 0.4f;
			}
			int num = this.LogoShouldClipOverdraw ? ((int)((float)dest.Width * 0.62f)) : ((int)((float)dest.Width * 0.45f));
			Rectangle targetBounds = new Rectangle(dest.X + 1 + Utils.GetXForAlignment(align2, dest.Width, this.LogoShouldClipOverdraw ? -130 : 0, num), dest.Y + (this.LogoShouldClipOverdraw ? -50 : 20), num, (int)((float)num * ((float)this.LogoImage.Height / (float)this.LogoImage.Width)));
			Rectangle fullBounds = new Rectangle(dest.X, dest.Y - 20, dest.Width, dest.Height + 10);
			if (!flag)
			{
				fullBounds = dest;
			}
			Utils.RenderSpriteIntoClippedRectDest(fullBounds, targetBounds, this.LogoImage, color, sb);
			int num2 = dest.Width / 2;
			int num3 = (int)((float)num2 * ((float)this.TitleImage.Height / (float)this.TitleImage.Width));
			Rectangle targetBounds2 = new Rectangle(dest.X + Utils.GetXForAlignment(align, dest.Width, 20, num2), dest.Y + 80 - (num3 - 40), num2, num3);
			Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds2, this.TitleImage, color, sb);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000A4A4 File Offset: 0x000086A4
		public override string getSaveString()
		{
			return string.Format("<LogoCustomConnectDisplayDaemon logo=\"{0}\" title=\"{1}\" overdrawLogo=\"{2}\" buttonAlignment=\"{3}\" />", new object[]
			{
				this.logoImageName,
				this.titleImageName,
				this.LogoShouldClipOverdraw ? "true" : "false",
				this.buttonAlignmentName
			});
		}

		// Token: 0x04000088 RID: 136
		private const int ButtonLowerHeight = 80;

		// Token: 0x04000089 RID: 137
		public AlignmentX ButtonsAlignment;

		// Token: 0x0400008A RID: 138
		private string logoImageName;

		// Token: 0x0400008B RID: 139
		private string titleImageName;

		// Token: 0x0400008C RID: 140
		private string buttonAlignmentName;

		// Token: 0x0400008D RID: 141
		private Texture2D LogoImage;

		// Token: 0x0400008E RID: 142
		private Texture2D TitleImage;

		// Token: 0x0400008F RID: 143
		private bool LogoShouldClipOverdraw = true;
	}
}
