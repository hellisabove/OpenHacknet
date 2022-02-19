using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200015C RID: 348
	internal class SSHCrackExe : ExeModule
	{
		// Token: 0x060008C9 RID: 2249 RVA: 0x00092CC0 File Offset: 0x00090EC0
		public SSHCrackExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.IdentifierName = "SecureShellCrack";
			this.ramCost = 242;
		}

		// Token: 0x060008CA RID: 2250 RVA: 0x00092D10 File Offset: 0x00090F10
		public override void LoadContent()
		{
			base.LoadContent();
			this.GridEntryWidth = 31;
			this.GridEntryHeight = 15;
			this.width = (int)(((float)this.bounds.Width - 4f) / (float)this.GridEntryWidth);
			this.height = (int)(((float)this.bounds.Height - 24f) / (float)this.GridEntryHeight);
			this.Grid = new SSHCrackExe.SSHCrackGridEntry[this.width, this.height];
			int num = 0;
			float num2 = 0f;
			float num3 = SSHCrackExe.DURATION - SSHCrackExe.GRID_REVEAL_DELAY - SSHCrackExe.ENDING_FLASH;
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					int num4 = Math.Max(Math.Abs(j - i / 2), Math.Abs(j + i / 2));
					int num5 = i / 2 + num4;
					float num6 = num3 - num2;
					if (num % 2 == 0)
					{
						num6 = Utils.randm(num3);
						num2 = num6;
					}
					this.Grid[j, i] = new SSHCrackExe.SSHCrackGridEntry
					{
						TimeSinceActivated = 0f,
						CurrentValue = Utils.getRandomByte(),
						TimeTillActive = (float)num5 * SSHCrackExe.SHEEN_FLASH_DELAY,
						TimeTillSolved = num6
					};
					num++;
				}
			}
			this.unlockedFlashColor = Color.Lerp(this.os.unlockedColor, this.os.brightUnlockedColor, 0.4f);
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			computer.hostileActionTaken();
			this.os.write("SecureShellCrack Running...");
		}

		// Token: 0x060008CB RID: 2251 RVA: 0x00092ED4 File Offset: 0x000910D4
		public override void Update(float t)
		{
			base.Update(t);
			this.timeLeft -= t;
			if (this.timeLeft <= 0f)
			{
				if (!this.complete)
				{
					this.complete = true;
					this.Completed();
					this.isExiting = true;
				}
			}
			if (this.timeLeft < SSHCrackExe.GRID_REVEAL_DELAY && SSHCrackExe.GRID_REVEAL_DELAY - this.timeLeft <= t)
			{
				this.GridShowingSheen = true;
				for (int i = 0; i < this.height; i++)
				{
					for (int j = 0; j < this.width; j++)
					{
						int num = Math.Max(Math.Abs(j - i / 2), Math.Abs(j + i / 2));
						int num2 = i / 2 + num;
						SSHCrackExe.SSHCrackGridEntry sshcrackGridEntry = this.Grid[j, i];
						sshcrackGridEntry.TimeSinceActivated = -1f * ((float)num2 * SSHCrackExe.SHEEN_FLASH_DELAY);
						this.Grid[j, i] = sshcrackGridEntry;
					}
				}
			}
			float num3 = SSHCrackExe.DURATION - this.timeLeft;
			for (int i = 0; i < this.height; i++)
			{
				for (int j = 0; j < this.width; j++)
				{
					SSHCrackExe.SSHCrackGridEntry sshcrackGridEntry = this.Grid[j, i];
					sshcrackGridEntry.TimeTillActive -= t;
					if (Utils.randm(0.5f) <= t)
					{
						sshcrackGridEntry.CurrentValue = Utils.getRandomByte();
					}
					if (sshcrackGridEntry.TimeTillActive <= 0f)
					{
						sshcrackGridEntry.TimeTillActive = 0f;
						sshcrackGridEntry.TimeSinceActivated += t;
						if (num3 > SSHCrackExe.GRID_REVEAL_DELAY)
						{
							sshcrackGridEntry.TimeTillSolved -= t;
							if (sshcrackGridEntry.TimeTillSolved <= 0f)
							{
								sshcrackGridEntry.CurrentValue = 0;
							}
						}
					}
					this.Grid[j, i] = sshcrackGridEntry;
				}
			}
		}

		// Token: 0x060008CC RID: 2252 RVA: 0x00093108 File Offset: 0x00091308
		public override void Draw(float t)
		{
			base.Draw(t);
			Rectangle empty = Rectangle.Empty;
			this.drawOutline();
			this.drawTarget("");
			TextItem.doFontLabel(new Vector2((float)(this.bounds.X + 2), (float)(this.bounds.Y + 14)), this.complete ? "Operation Complete" : "SSH Crack in operation...", GuiData.UITinyfont, new Color?(Utils.AddativeWhite * 0.8f * this.fade), (float)(this.bounds.Width - 6), float.MaxValue, false);
			int num = this.bounds.Y + 30;
			int num2 = this.bounds.X + 2;
			float num3 = 0.4f;
			Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + 16, this.GridEntryWidth - 2, this.GridEntryHeight - 2);
			for (int i = 0; i < this.width; i++)
			{
				for (int j = 0; j < this.height; j++)
				{
					SSHCrackExe.SSHCrackGridEntry sshcrackGridEntry = this.Grid[i, j];
					rectangle.X = num2 + i * this.GridEntryWidth;
					rectangle.Y = num + j * this.GridEntryHeight;
					if (rectangle.Y + 1 <= this.bounds.Y + this.bounds.Height)
					{
						if (sshcrackGridEntry.TimeTillActive <= 0f)
						{
							Color value = this.os.lockedColor;
							if (sshcrackGridEntry.TimeSinceActivated < num3)
							{
								float amount = sshcrackGridEntry.TimeSinceActivated / num3;
								value = Color.Lerp(this.os.brightLockedColor, this.os.lockedColor, amount);
							}
							if (sshcrackGridEntry.TimeTillSolved <= 0f)
							{
								value = this.os.unlockedColor;
								if (sshcrackGridEntry.TimeTillSolved > -1f * num3)
								{
									float num4 = -1f * sshcrackGridEntry.TimeTillSolved / num3;
									value = Color.Lerp(this.os.unlockedColor, this.unlockedFlashColor, num4);
								}
							}
							if (this.GridShowingSheen)
							{
								float num4 = 0f;
								if (sshcrackGridEntry.TimeSinceActivated >= 0f)
								{
									num4 = sshcrackGridEntry.TimeSinceActivated / (num3 / 2f);
									if (sshcrackGridEntry.TimeSinceActivated > num3 / 2f)
									{
										num4 = 1f - Math.Min(1f, (sshcrackGridEntry.TimeSinceActivated - num3 / 2f) / (num3 / 2f));
									}
								}
								if (num4 < 0.25f)
								{
									num4 = 0f;
								}
								else if (num4 < 0.75f)
								{
									num4 = 0.5f;
								}
								value = Color.Lerp(this.os.unlockedColor, this.unlockedFlashColor, num4);
							}
							Rectangle destinationRectangle = rectangle;
							bool flag = true;
							if (rectangle.Y + rectangle.Height > this.bounds.Y + this.bounds.Height)
							{
								destinationRectangle.Height = rectangle.Height - (rectangle.Y + rectangle.Height - (this.bounds.Y + this.bounds.Height));
								flag = false;
							}
							this.spriteBatch.Draw(Utils.white, destinationRectangle, value * this.fade);
							float num5 = (sshcrackGridEntry.CurrentValue >= 10) ? ((sshcrackGridEntry.CurrentValue >= 100) ? 1f : 5f) : 8f;
							if (flag)
							{
								this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat(sshcrackGridEntry.CurrentValue), new Vector2((float)rectangle.X + num5, (float)rectangle.Y - 1.5f), Color.White * this.fade);
							}
						}
					}
				}
			}
		}

		// Token: 0x060008CD RID: 2253 RVA: 0x00093574 File Offset: 0x00091774
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(22, this.os.thisComputer.ip);
				this.os.write("-- SecureShellCrack Complete --");
			}
		}

		// Token: 0x04000A31 RID: 2609
		public static float DURATION = 8f;

		// Token: 0x04000A32 RID: 2610
		private static float GRID_REVEAL_DELAY = 0.6f;

		// Token: 0x04000A33 RID: 2611
		private static float ENDING_FLASH = 0.7f;

		// Token: 0x04000A34 RID: 2612
		private static float SHEEN_FLASH_DELAY = 0.03f;

		// Token: 0x04000A35 RID: 2613
		private int width;

		// Token: 0x04000A36 RID: 2614
		private int height;

		// Token: 0x04000A37 RID: 2615
		private float timeLeft = SSHCrackExe.DURATION;

		// Token: 0x04000A38 RID: 2616
		private bool complete = false;

		// Token: 0x04000A39 RID: 2617
		private bool GridShowingSheen = false;

		// Token: 0x04000A3A RID: 2618
		private SSHCrackExe.SSHCrackGridEntry[,] Grid;

		// Token: 0x04000A3B RID: 2619
		private int GridEntryWidth;

		// Token: 0x04000A3C RID: 2620
		private int GridEntryHeight;

		// Token: 0x04000A3D RID: 2621
		private Color unlockedFlashColor;

		// Token: 0x0200015D RID: 349
		private struct SSHCrackGridEntry
		{
			// Token: 0x04000A3E RID: 2622
			public float TimeTillActive;

			// Token: 0x04000A3F RID: 2623
			public float TimeTillSolved;

			// Token: 0x04000A40 RID: 2624
			public float TimeSinceActivated;

			// Token: 0x04000A41 RID: 2625
			public byte CurrentValue;
		}
	}
}
