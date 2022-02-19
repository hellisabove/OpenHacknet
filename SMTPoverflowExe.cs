using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200015B RID: 347
	internal class SMTPoverflowExe : ExeModule
	{
		// Token: 0x060008C2 RID: 2242 RVA: 0x00092330 File Offset: 0x00090530
		public SMTPoverflowExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.ramCost = 356;
			this.IdentifierName = "SMTP Overflow";
			this.activeBarColor = this.os.unlockedColor;
		}

		// Token: 0x060008C3 RID: 2243 RVA: 0x000923C8 File Offset: 0x000905C8
		public override void LoadContent()
		{
			base.LoadContent();
			this.barSize = SMTPoverflowExe.BAR_HEIGHT + 1.4f;
			int num = (int)((float)this.bounds.Height / this.barSize);
			int num2 = this.bounds.Width / 2;
			num2--;
			this.leftBars = new List<Vector2>(num);
			this.rightBars = new List<Vector2>(num);
			for (int i = 0; i < num; i++)
			{
				this.leftBars.Add(new Vector2((float)Utils.random.Next(0, num2), (float)Utils.random.Next(0, num2)));
				this.rightBars.Add(new Vector2((float)Utils.random.Next(0, num2), (float)Utils.random.Next(0, num2)));
			}
			this.barSize = SMTPoverflowExe.BAR_HEIGHT;
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			computer.hostileActionTaken();
		}

		// Token: 0x060008C4 RID: 2244 RVA: 0x000924BC File Offset: 0x000906BC
		public override void Update(float t)
		{
			base.Update(t);
			this.timeAccum += t * 5f;
			this.progress += t / SMTPoverflowExe.DURATION;
			if (this.progress >= 1f)
			{
				this.progress = 1f;
				if (!this.hasCompleted)
				{
					this.Completed();
					this.hasCompleted = true;
				}
				this.sucsessTimer -= t;
				if (this.sucsessTimer <= 0f)
				{
					this.isExiting = true;
				}
			}
			Vector2 value = Vector2.Zero;
			for (int i = 0; i < this.leftBars.Count; i++)
			{
				if (i > this.completedIndex)
				{
					value = this.leftBars[i];
					value.X += (float)(Math.Sin((double)(this.timeAccum + (float)(i * i))) * (double)t) * SMTPoverflowExe.BAR_MOVEMENT;
					value.Y += (float)(Math.Sin((double)(this.timeAccum + (float)i)) * (double)t) * SMTPoverflowExe.BAR_MOVEMENT;
					this.leftBars[i] = value;
					value = this.rightBars[i];
					value.X += (float)(Math.Sin((double)(this.timeAccum + (float)(i * i))) * (double)t) * SMTPoverflowExe.BAR_MOVEMENT;
					value.Y += (float)(Math.Sin((double)(this.timeAccum + (float)i)) * (double)t) * SMTPoverflowExe.BAR_MOVEMENT;
					this.rightBars[i] = value;
				}
			}
			this.completedIndex = (int)(this.progress * (float)(this.leftBars.Count - 1));
		}

		// Token: 0x060008C5 RID: 2245 RVA: 0x00092688 File Offset: 0x00090888
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			this.spriteBatch.DrawString(GuiData.UISmallfont, "SMTP Mail Server Overflow", new Vector2((float)(this.bounds.X + 5), (float)(this.bounds.Y + 12)), this.os.subtleTextColor);
			Vector2 vector = new Vector2((float)(this.bounds.X + 2), (float)(this.bounds.Y + 57));
			int num = (this.bounds.Width - 4) / 2;
			Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int)this.barSize);
			Rectangle rectangle2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int)this.barSize);
			bool flag = false;
			for (int i = 0; i < this.leftBars.Count; i++)
			{
				flag = (i <= this.completedIndex);
				rectangle.Width = (int)this.leftBars[i].Y;
				this.spriteBatch.Draw(Utils.white, rectangle, (flag ? Color.Lerp(this.os.outlineColor, Utils.VeryDarkGray, Utils.randm(0.2f) * Utils.randm(1f)) : this.os.subtleTextColor) * this.fade);
				rectangle.Y += (int)this.barSize + 1;
				Vector2 vector2 = this.rightBars[i];
				rectangle2.Width = (int)vector2.Y;
				rectangle2.X = this.bounds.X + this.bounds.Width - 4 - (int)vector2.Y;
				this.spriteBatch.Draw(Utils.white, rectangle2, (flag ? Color.Lerp(this.os.outlineColor, Utils.VeryDarkGray, Utils.randm(0.2f) * Utils.randm(1f)) : this.os.subtleTextColor) * this.fade);
				rectangle2.Y += (int)this.barSize + 1;
				if (rectangle2.Y + rectangle2.Height >= this.bounds.Y + this.bounds.Height)
				{
					break;
				}
			}
			rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int)this.barSize);
			rectangle2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + 38, (this.bounds.Width - 4) / 2, (int)this.barSize);
			bool flag2 = flag;
			for (int i = 0; i < this.leftBars.Count; i++)
			{
				flag = (i <= this.completedIndex || flag2);
				rectangle.Width = (int)this.leftBars[i].X;
				this.DrawBar(rectangle, flag, true);
				rectangle.Y += (int)this.barSize + 1;
				Vector2 vector2 = this.rightBars[i];
				rectangle2.Width = (int)vector2.X;
				rectangle2.X = this.bounds.X + this.bounds.Width - 4 - (int)vector2.X;
				this.DrawBar(rectangle2, flag, false);
				rectangle2.Y += (int)this.barSize + 1;
				if (rectangle2.Y + rectangle2.Height >= this.bounds.Y + this.bounds.Height)
				{
					break;
				}
			}
		}

		// Token: 0x060008C6 RID: 2246 RVA: 0x00092AD8 File Offset: 0x00090CD8
		private void DrawBar(Rectangle dest, bool barActive, bool isLeft)
		{
			float scale = Math.Min(1f, (float)dest.Width / ((float)this.bounds.Width / 2f));
			this.spriteBatch.Draw(Utils.white, dest, (this.hasCompleted ? Color.Lerp(this.os.highlightColor, Utils.AddativeWhite, Math.Max(0f, this.sucsessTimer)) : (barActive ? this.activeBarColor : Color.White)) * this.fade);
			if (barActive)
			{
				int height = dest.Height;
				dest.Height = 1;
				this.spriteBatch.Draw(Utils.gradientLeftRight, dest, null, this.activeBarHighlightColor * 0.6f * this.fade * scale, 0f, Vector2.Zero, isLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.3f);
				dest.Y++;
				this.spriteBatch.Draw(Utils.gradientLeftRight, dest, null, this.activeBarHighlightColor * 0.2f * this.fade * scale, 0f, Vector2.Zero, isLeft ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0.3f);
				dest.Y--;
				dest.Height = height;
			}
		}

		// Token: 0x060008C7 RID: 2247 RVA: 0x00092C54 File Offset: 0x00090E54
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(25, this.os.thisComputer.ip);
			}
		}

		// Token: 0x04000A24 RID: 2596
		public static float DURATION = 12f;

		// Token: 0x04000A25 RID: 2597
		public static float BAR_MOVEMENT = 30f;

		// Token: 0x04000A26 RID: 2598
		public static float BAR_HEIGHT = 2f;

		// Token: 0x04000A27 RID: 2599
		public float progress;

		// Token: 0x04000A28 RID: 2600
		public bool hasCompleted;

		// Token: 0x04000A29 RID: 2601
		private float sucsessTimer = 0.5f;

		// Token: 0x04000A2A RID: 2602
		private float timeAccum = 0f;

		// Token: 0x04000A2B RID: 2603
		private float barSize = 0f;

		// Token: 0x04000A2C RID: 2604
		private List<Vector2> leftBars;

		// Token: 0x04000A2D RID: 2605
		private List<Vector2> rightBars;

		// Token: 0x04000A2E RID: 2606
		private int completedIndex = 0;

		// Token: 0x04000A2F RID: 2607
		private Color activeBarColor = new Color(34, 82, 64, 255);

		// Token: 0x04000A30 RID: 2608
		private Color activeBarHighlightColor = new Color(0, 186, 99, 0);
	}
}
