using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000061 RID: 97
	internal class DLCTraceSlower : ExeModule
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x0001A3E4 File Offset: 0x000185E4
		private DLCTraceSlower(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.name = "SignalScramble";
			this.ramCost = 50;
			this.IdentifierName = "SignalScramble";
			this.dotEffect = new DepthDotGridEffect(this.os.content);
			this.ActiveConnectedCompIP = ((this.os.connectedComp == null) ? this.os.thisComputer.ip : this.os.connectedComp.ip);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0001A470 File Offset: 0x00018670
		public static DLCTraceSlower GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
		{
			OS operatingSystem = (OS)osObj;
			target.hostileActionTaken();
			return new DLCTraceSlower(location, operatingSystem);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0001A498 File Offset: 0x00018698
		public override void Update(float t)
		{
			base.Update(t);
			string b = (this.os.connectedComp == null) ? this.os.thisComputer.ip : this.os.connectedComp.ip;
			if (this.ActiveConnectedCompIP != b)
			{
				this.isExiting = true;
			}
			if (600 != this.ramCost)
			{
				if (600 < this.ramCost)
				{
					this.ramCost -= (int)(t * 200f);
					if (this.ramCost < 600)
					{
						this.ramCost = 600;
					}
				}
				else
				{
					int num = (int)(t * 200f);
					if (this.os.ramAvaliable >= num)
					{
						this.ramCost += num;
						if (this.ramCost > 600)
						{
							this.ramCost = 600;
						}
					}
				}
			}
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			bool flag = !this.isExiting && computer.PlayerHasAdminPermissions();
			float point = ((float)this.ramCost - 50f) / 550f;
			if (flag)
			{
				float val = 1f - Utils.QuadraticOutCurve(point);
				this.os.traceTracker.trackSpeedFactor = Math.Min(this.os.traceTracker.trackSpeedFactor, val);
			}
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0001A64E File Offset: 0x0001884E
		public override void Killed()
		{
			base.Killed();
			this.os.traceTracker.trackSpeedFactor = 1f;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0001A66D File Offset: 0x0001886D
		public override void Completed()
		{
			base.Completed();
			this.os.traceTracker.trackSpeedFactor = 1f;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0001A68C File Offset: 0x0001888C
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle rectangle = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 2);
			Rectangle rectangle2 = rectangle;
			rectangle2.Height = (int)((float)rectangle.Height * 1.125f);
			bool flag = this.os.connectedComp != null && this.ramCost >= 600 && this.os.connectedComp.adminIP == this.os.thisComputer.ip;
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			bool flag2 = computer.PlayerHasAdminPermissions();
			this.dotEffect.DrawGrid(rectangle, Vector2.Zero, this.spriteBatch, 30f, 10, flag2 ? Color.Gray : new Color(200, 15, 15, 0), 60f, 10f, 0f, this.os.timer, (!flag) ? 1f : (1f - (float)this.ramCost / 600f));
			if (this.bounds.Height >= Module.PANEL_HEIGHT + 4)
			{
				string text = null;
				string text2;
				if (this.ramCost < 600)
				{
					if (this.isExiting)
					{
						text2 = LocaleTerms.Loc("Spinning down...");
					}
					else
					{
						text2 = LocaleTerms.Loc("Spinning up...");
						text = ((float)this.ramCost / 600f * 100f).ToString("00") + "." + Utils.randm(99f).ToString("00") + "%";
					}
				}
				else
				{
					text2 = LocaleTerms.Loc("Suppression Active");
					if (!flag2)
					{
						text2 = LocaleTerms.Loc("Administrator Access Required");
					}
				}
				int num = 27;
				if (this.bounds.Height > 40)
				{
					text2 = "[ " + text2 + " ]";
					Rectangle destinationRectangle = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - num - 10 - num, (int)((float)rectangle.Width * 0.92f), num + 2);
					destinationRectangle.Width += 8;
					this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black * 0.9f);
					destinationRectangle.Width -= 8;
					destinationRectangle.Y += 4;
					destinationRectangle.Height -= 8;
					Vector2 vector = GuiData.font.MeasureString(text2);
					float num2 = (text != null) ? 0.7f : 1f;
					float num3 = Math.Min(1f, Math.Min((float)destinationRectangle.Width * num2 / vector.X, (float)destinationRectangle.Height / vector.Y));
					Vector2 value = new Vector2((float)destinationRectangle.Width - vector.X * num3, (float)(destinationRectangle.Height / 2) - vector.Y * num3 / 2f);
					Color color = Color.Lerp(Color.White, this.os.highlightColor, 0.4f + Utils.randm(0.1f));
					this.spriteBatch.DrawString(GuiData.font, text2, new Vector2((float)destinationRectangle.X, (float)destinationRectangle.Y) + value, color, 0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.5f);
					if (text != null)
					{
						Vector2 value2 = new Vector2((float)destinationRectangle.X + 2f, (float)destinationRectangle.Y + value.Y);
						for (int i = 0; i < text.Length; i++)
						{
							this.spriteBatch.DrawString(GuiData.font, string.Concat(text[i]), value2 + ((text[i] == '.') ? new Vector2(3f, 0f) : Vector2.Zero), color, 0f, Vector2.Zero, new Vector2(num3), SpriteEffects.None, 0.5f);
							value2.X += vector.X / (float)text2.Length * num3 + 2f;
						}
					}
				}
				Rectangle destinationRectangle2 = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - num, rectangle.Width, 25);
				this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black);
				if (Button.doButton(19371002 + this.PID, destinationRectangle2.X + 1, destinationRectangle2.Y, destinationRectangle2.Width - 2, destinationRectangle2.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
					this.os.traceTracker.trackSpeedFactor = 1f;
				}
			}
		}

		// Token: 0x04000210 RID: 528
		private const int TargetRamUse = 600;

		// Token: 0x04000211 RID: 529
		private const float RAM_CHANGE_PS = 200f;

		// Token: 0x04000212 RID: 530
		private const int RAM_STARTING = 50;

		// Token: 0x04000213 RID: 531
		private string ActiveConnectedCompIP = null;

		// Token: 0x04000214 RID: 532
		private DepthDotGridEffect dotEffect;
	}
}
