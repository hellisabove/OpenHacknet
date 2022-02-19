using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000113 RID: 275
	internal class FTPBounceExe : ExeModule
	{
		// Token: 0x06000683 RID: 1667 RVA: 0x0006B760 File Offset: 0x00069960
		public FTPBounceExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.ramCost = 210;
			this.IdentifierName = "FTP Bounce";
		}

		// Token: 0x06000684 RID: 1668 RVA: 0x0006B7DC File Offset: 0x000699DC
		public override void LoadContent()
		{
			base.LoadContent();
			this.binary = Computer.generateBinaryString(1024);
			int num = 12;
			this.binaryChars = (this.bounds.Width - 4) / num;
			this.acceptedBinary1 = new byte[this.binaryChars];
			this.acceptedBinary2 = new byte[this.binaryChars];
			for (int i = 0; i < num; i++)
			{
				this.acceptedBinary1[i] = ((Utils.random.NextDouble() > 0.5) ? 0 : 1);
				this.acceptedBinary2[i] = ((Utils.random.NextDouble() > 0.5) ? 0 : 1);
			}
			this.unlockOrder = new int[this.binaryChars];
			for (int i = 0; i < this.binaryChars; i++)
			{
				this.unlockOrder[i] = i;
			}
			int num2 = 300;
			for (int i = 0; i < num2; i++)
			{
				int num3 = Utils.random.Next(0, this.unlockOrder.Length - 1);
				int num4 = Utils.random.Next(0, this.unlockOrder.Length - 1);
				int num5 = this.unlockOrder[num3];
				this.unlockOrder[num3] = this.unlockOrder[num4];
				this.unlockOrder[num4] = num5;
			}
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			computer.hostileActionTaken();
		}

		// Token: 0x06000685 RID: 1669 RVA: 0x0006B950 File Offset: 0x00069B50
		public override void Update(float t)
		{
			base.Update(t);
			if (!this.complete)
			{
				this.binaryScrollTimer -= t;
				if (this.binaryScrollTimer <= 0f)
				{
					this.binaryIndex++;
					this.binaryScrollTimer = FTPBounceExe.SCROLL_RATE;
				}
			}
			this.timeLeft -= t;
			if (this.timeLeft <= 0f)
			{
				if (!this.complete)
				{
					this.complete = true;
					this.Completed();
				}
			}
			this.progress = Math.Min(Math.Abs(1f - this.timeLeft / FTPBounceExe.DURATION), 1f);
			this.unlockedChars1 = Math.Min((int)((float)this.binaryChars * (this.progress * 2f)), this.binaryChars);
			this.unlockedChars2 = Math.Min((int)((float)this.binaryChars * this.progress), this.binaryChars);
		}

		// Token: 0x06000686 RID: 1670 RVA: 0x0006BA54 File Offset: 0x00069C54
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Vector2 vector = new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 12));
			for (int i = 0; i < 6; i++)
			{
				for (int j = 0; j < this.binaryChars; j++)
				{
					this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat(this.binary[(this.binaryIndex + j + i * 20) % (this.binary.Length - 1)]), vector, Color.White);
					vector.X += 12f;
				}
				vector.Y += 12f;
				vector.X = (float)(this.bounds.X + 6);
				if (vector.Y - (float)this.bounds.Y + 24f > (float)this.bounds.Height)
				{
					return;
				}
			}
			vector.Y += 16f;
			if (vector.Y - (float)this.bounds.Y + 24f > (float)this.bounds.Height)
			{
				return;
			}
			this.spriteBatch.DrawString(GuiData.UISmallfont, "Working ::", vector, this.os.subtleTextColor);
			vector.Y += 20f;
			Rectangle destinationRectangle = new Rectangle((int)vector.X, (int)vector.Y, this.bounds.Width - 12, 80);
			destinationRectangle.Height = (int)Math.Min((float)destinationRectangle.Height, (float)this.bounds.Height - (vector.Y - (float)this.bounds.Y));
			this.spriteBatch.Draw(Utils.white, destinationRectangle, (this.complete ? this.os.unlockedColor : this.os.lockedColor) * this.fade);
			if (vector.Y - (float)this.bounds.Y + 80f > (float)this.bounds.Height)
			{
				return;
			}
			for (int j = 0; j < this.unlockedChars1; j++)
			{
				int num = this.unlockOrder[j];
				int num2 = this.unlockOrder[this.binaryChars - j - 1];
				vector.X = (float)(this.bounds.X + 6 + 12 * num);
				this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat(this.acceptedBinary1[num]), vector, Color.White * this.fade);
				this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat(this.acceptedBinary1[num2]), vector + new Vector2(0f, 32f), Color.White * this.fade);
			}
			vector.Y += 16f;
			for (int j = 0; j < this.unlockedChars2; j++)
			{
				int num = this.unlockOrder[this.binaryChars - j - 1];
				int num2 = this.unlockOrder[j];
				vector.X = (float)(this.bounds.X + 6 + 12 * num);
				this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat(this.acceptedBinary2[num]), vector, Color.White * this.fade);
				this.spriteBatch.DrawString(GuiData.UISmallfont, string.Concat(this.acceptedBinary2[num2]), vector + new Vector2(0f, 32f), Color.White * this.fade);
			}
		}

		// Token: 0x06000687 RID: 1671 RVA: 0x0006BE9C File Offset: 0x0006A09C
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(21, this.os.thisComputer.ip);
			}
			this.isExiting = true;
		}

		// Token: 0x04000738 RID: 1848
		public static float DURATION = 15f;

		// Token: 0x04000739 RID: 1849
		public static float SCROLL_RATE = 0.08f;

		// Token: 0x0400073A RID: 1850
		private string binary;

		// Token: 0x0400073B RID: 1851
		private byte[] acceptedBinary1;

		// Token: 0x0400073C RID: 1852
		private byte[] acceptedBinary2;

		// Token: 0x0400073D RID: 1853
		private int binaryChars = 0;

		// Token: 0x0400073E RID: 1854
		private float progress = 0f;

		// Token: 0x0400073F RID: 1855
		private float timeLeft = FTPBounceExe.DURATION;

		// Token: 0x04000740 RID: 1856
		private float binaryScrollTimer = FTPBounceExe.SCROLL_RATE;

		// Token: 0x04000741 RID: 1857
		private int binaryIndex = 0;

		// Token: 0x04000742 RID: 1858
		private bool complete = false;

		// Token: 0x04000743 RID: 1859
		private int unlockedChars1 = 0;

		// Token: 0x04000744 RID: 1860
		private int unlockedChars2 = 0;

		// Token: 0x04000745 RID: 1861
		private int[] unlockOrder;
	}
}
