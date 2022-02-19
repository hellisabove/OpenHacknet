using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000006 RID: 6
	internal class CustomConnectDisplayDaemon : CustomConnectDisplayOverride
	{
		// Token: 0x06000023 RID: 35 RVA: 0x0000362C File Offset: 0x0000182C
		public CustomConnectDisplayDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Display Override"), os)
		{
			this.topEffect = new MovingBarsEffect();
			this.botEffect = new MovingBarsEffect
			{
				IsInverted = true
			};
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003680 File Offset: 0x00001880
		public CustomConnectDisplayDaemon(Computer c, string name, OS os) : base(c, name, os)
		{
			this.topEffect = new MovingBarsEffect();
			this.botEffect = new MovingBarsEffect
			{
				IsInverted = true
			};
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000036CC File Offset: 0x000018CC
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.timeInThisState += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			this.topEffect.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			this.botEffect.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			int num = bounds.X + 20;
			int num2 = bounds.Y + 60;
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			if (!(computer.adminIP == this.os.thisComputer.adminIP))
			{
				this.DrawNonAdminDisplay(bounds, sb);
				this.HasBeenAdminBefore = false;
			}
			else
			{
				if (!this.HasBeenAdminBefore)
				{
					this.timeInThisState = 0f;
					this.HasBeenAdminBefore = true;
				}
				this.DrawAdminDisplay(bounds, sb, computer);
			}
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000037F8 File Offset: 0x000019F8
		internal virtual void DrawAdminDisplay(Rectangle bounds, SpriteBatch sb, Computer c)
		{
			int num = bounds.Width;
			float num2 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.timeInThisState / 1f));
			if (this.timeInThisState < 1f)
			{
				num = (int)((float)bounds.Width * num2);
			}
			float num3 = 120f;
			float num4 = 60f;
			float num5 = 30f;
			float num6 = 10f;
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
			num9 = (int)((float)bounds.Y + (float)num9 * (1f - num7));
			int x = bounds.X + bounds.Width / 2 - num / 2;
			this.DrawCautionLinedMessage(new Rectangle(x, num9, num, num8), stripHeight, this.os.highlightColor, (this.timeInThisState > 0.5f + Utils.randm(0.5f)) ? "ACCESS GRANTED" : "", sb, PatternDrawer.warningStripe, 0);
			x = bounds.X + 20;
			num9 = bounds.Y + 80 + 2;
			if (this.timeInThisState >= 2f)
			{
				this.DrawConnectButtons(bounds, sb, c, 20, num9, AlignmentX.Middle);
			}
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000039C4 File Offset: 0x00001BC4
		internal virtual void DrawConnectButtons(Rectangle bounds, SpriteBatch sb, Computer c, int margin, int y, AlignmentX ButtonAlignment = AlignmentX.Middle)
		{
			int x = bounds.X + margin;
			int num = c.daemons.Count + 6;
			int num2 = 40;
			int num3 = bounds.Height - (y - bounds.Y) - 20;
			num3 -= num * 5;
			if ((double)num3 / (double)num < (double)num2)
			{
				num2 = (int)((double)num3 / (double)num);
			}
			int num4 = bounds.Width / 2;
			switch (ButtonAlignment)
			{
			case AlignmentX.Left:
				x = bounds.X + margin;
				goto IL_B6;
			case AlignmentX.Right:
				x = bounds.X + bounds.Width - (num4 + margin);
				goto IL_B6;
			}
			x = bounds.X + bounds.Width / 2 - num4 / 2;
			IL_B6:
			if (c.daemons.Count > 0)
			{
				y += num2 + 5;
			}
			for (int i = 0; i < c.daemons.Count; i++)
			{
				if (!(c.daemons[i] is CustomConnectDisplayOverride))
				{
					if (Button.doButton(29000 + i, x, y, num4, num2, c.daemons[i].name, new Color?(this.os.highlightColor)))
					{
						this.os.display.command = c.daemons[i].name;
						c.daemons[i].navigatedTo();
					}
					y += num2 + 5;
				}
			}
			if (Button.doButton(300000, x, y, num4, num2, LocaleTerms.Loc("Login"), new Color?(this.os.subtleTextColor)))
			{
				this.os.runCommand("login");
				this.os.terminal.clearCurrentLine();
			}
			y += num2 + 5;
			if (Button.doButton(300002, x, y, num4, num2, LocaleTerms.Loc("Probe System"), new Color?(this.os.highlightColor)))
			{
				this.os.runCommand("probe");
			}
			y += num2 + 5;
			if (Button.doButton(300003, x, y, num4, num2, LocaleTerms.Loc("View Filesystem"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("ls");
			}
			y += num2 + 5;
			if (Button.doButton(300006, x, y, num4, num2, LocaleTerms.Loc("View Logs"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("cd log");
			}
			y += num2 + 5;
			if (Button.doButton(300009, x, y, num4, num2, LocaleTerms.Loc("Scan Network"), new Color?(this.os.hasConnectionPermission(true) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("scan");
			}
			y = bounds.Y + bounds.Height - 30;
			if (Button.doButton(300012, x, y, num4, 20, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
			{
				this.os.runCommand("dc");
			}
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003DA8 File Offset: 0x00001FA8
		internal virtual void DrawCautionLinedMessage(Rectangle dest, int stripHeight, Color color, string Message, SpriteBatch sb, Texture2D stripTexture = null, int textOffsetY = 0)
		{
			if (stripTexture == null)
			{
				stripTexture = PatternDrawer.warningStripe;
			}
			Rectangle dest2 = dest;
			dest2.Height = stripHeight;
			PatternDrawer.draw(dest2, 1f, Color.Transparent, color, sb, stripTexture);
			dest2 = new Rectangle(dest2.X, dest.Y + dest.Height - stripHeight, dest2.Width, stripHeight);
			PatternDrawer.draw(dest2, 1f, Color.Transparent, color, sb, stripTexture);
			int height = dest.Height - dest2.Height * 2 + dest.Height / 10;
			Rectangle rectangle = new Rectangle(dest.X, dest.Y + stripHeight + 2 + textOffsetY, dest.Width, height);
			if (Settings.ActiveLocale == "en-us")
			{
				TextItem.doFontLabelToSize(rectangle, Message, GuiData.titlefont, color, true, false);
			}
			else
			{
				TextItem.doCenteredFontLabel(rectangle, Message, GuiData.font, color, false);
			}
			Rectangle rectangle2 = rectangle;
			rectangle2.Y = dest.Y + dest.Height + 2;
			rectangle2.Height = 30;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003ED0 File Offset: 0x000020D0
		internal virtual void DrawNonAdminDisplay(Rectangle dest, SpriteBatch sb)
		{
			Rectangle destinationRectangle = new Rectangle(dest.X + 1, dest.Y, dest.Width, 1);
			sb.Draw(Utils.white, destinationRectangle, Utils.AddativeRed);
			int num = 120;
			Rectangle rectangle = new Rectangle(dest.X, dest.Y + (dest.Height / 2 - num), dest.Width, num);
			sb.Draw(Utils.white, rectangle, Color.Black * 0.8f);
			int num2 = 20;
			this.DrawCautionLinedMessage(Utils.InsetRectangle(rectangle, 4), num2, Color.Red, "ACCESS DENIED", sb, null, 0);
			int height = 90;
			Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y + num2 + 2, rectangle.Width, height);
			Rectangle rectangle3 = rectangle2;
			rectangle3 = Utils.InsetRectangle(rectangle3, 20);
			rectangle3.Y = rectangle.Y + rectangle.Height + 2;
			rectangle3.Height = 30;
			TextItem.doFontLabelToSize(rectangle3, LocaleTerms.Loc("Non-Admin Account Detected - Login to Proceed or Disconnect Now"), GuiData.font, Color.Black, true, false);
			TextItem.doFontLabelToSize(rectangle3, LocaleTerms.Loc("Non-Admin Account Detected - Login to Proceed or Disconnect Now"), GuiData.font, Color.Red * 0.7f, true, false);
			int num3 = 340;
			int x = dest.X + dest.Width / 2 - num3 / 2;
			int num4 = rectangle3.Y + rectangle3.Height + 10;
			int num5 = 40;
			if (Button.doButton(302001, x, num4, num3, num5, LocaleTerms.Loc("Login"), new Color?(Color.Gray)))
			{
				this.os.runCommand("login");
				this.os.terminal.clearCurrentLine();
			}
			num4 += num5 + 6;
			if (Button.doButton(302003, x, num4, num3, 40, LocaleTerms.Loc("Disconnect"), new Color?(Color.Red)))
			{
				this.os.runCommand("dc");
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000040EC File Offset: 0x000022EC
		public override string getSaveString()
		{
			return "<CustomConnectDisplayDaemon />";
		}

		// Token: 0x04000023 RID: 35
		private MovingBarsEffect topEffect;

		// Token: 0x04000024 RID: 36
		private MovingBarsEffect botEffect;

		// Token: 0x04000025 RID: 37
		private bool HasBeenAdminBefore = false;

		// Token: 0x04000026 RID: 38
		internal float timeInThisState = 0f;
	}
}
