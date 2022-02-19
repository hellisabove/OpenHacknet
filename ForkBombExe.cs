using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000114 RID: 276
	internal class ForkBombExe : ExeModule
	{
		// Token: 0x06000689 RID: 1673 RVA: 0x0006BF04 File Offset: 0x0006A104
		public ForkBombExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.ramCost = 10;
			this.runnerIP = "UNKNOWN";
			this.IdentifierName = "ForkBomb";
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x0006BF68 File Offset: 0x0006A168
		public ForkBombExe(Rectangle location, OS operatingSystem, string ipFrom) : base(location, operatingSystem)
		{
			this.ramCost = 10;
			this.runnerIP = ipFrom;
			this.IdentifierName = "ForkBomb";
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x0006BFC8 File Offset: 0x0006A1C8
		public override void LoadContent()
		{
			base.LoadContent();
			if (ForkBombExe.binary.Equals(""))
			{
				ForkBombExe.binary = Computer.generateBinaryString(5064);
			}
			float num = GuiData.detailfont.MeasureString("0").X - 0.15f;
			this.charsWide = (int)((float)this.bounds.Width / num + 0.5f);
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x0006C041 File Offset: 0x0006A241
		public override void Killed()
		{
			TrackerCompleteSequence.NextCompleteForkbombShouldTrace = false;
			base.Killed();
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0006C054 File Offset: 0x0006A254
		public override void Update(float t)
		{
			base.Update(t);
			if (this.frameSwitch)
			{
				this.binaryScroll++;
				if (this.binaryScroll >= ForkBombExe.binary.Length - (this.charsWide + 1))
				{
					this.binaryScroll = 0;
				}
			}
			this.frameSwitch = !this.frameSwitch;
			if (this.targetRamUse != this.ramCost)
			{
				int num = (int)(t * ForkBombExe.RAM_CHANGE_PS);
				if (this.os.ramAvaliable < num)
				{
					this.Completed();
				}
				else
				{
					this.ramCost += num;
					if (this.ramCost > this.targetRamUse)
					{
						this.ramCost = this.targetRamUse;
					}
				}
			}
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0006C12C File Offset: 0x0006A32C
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			float num = 8f;
			int num2 = this.binaryScroll;
			if (num2 >= ForkBombExe.binary.Length - (this.charsWide + 1))
			{
				num2 = 0;
			}
			Vector2 position = new Vector2((float)this.bounds.X, (float)this.bounds.Y);
			while (position.Y < (float)(this.bounds.Y + this.bounds.Height) - num)
			{
				this.spriteBatch.DrawString(GuiData.detailfont, ForkBombExe.binary.Substring(num2, this.charsWide), position, Color.White);
				num2 += this.charsWide;
				if (num2 >= ForkBombExe.binary.Length - (this.charsWide + 1))
				{
					num2 = 0;
				}
				position.Y += num;
			}
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0006C21C File Offset: 0x0006A41C
		public override void Completed()
		{
			base.Completed();
			if (TrackerCompleteSequence.NextCompleteForkbombShouldTrace)
			{
				TrackerCompleteSequence.NextCompleteForkbombShouldTrace = false;
				TrackerCompleteSequence.TriggerETAS(this.os);
				this.os.exes.Remove(this);
			}
			else
			{
				this.os.thisComputer.crash(this.runnerIP);
			}
		}

		// Token: 0x04000746 RID: 1862
		public static float RAM_CHANGE_PS = 150f;

		// Token: 0x04000747 RID: 1863
		private int targetRamUse = 999999999;

		// Token: 0x04000748 RID: 1864
		public string runnerIP = "";

		// Token: 0x04000749 RID: 1865
		public static string binary = "";

		// Token: 0x0400074A RID: 1866
		public int binaryScroll = 0;

		// Token: 0x0400074B RID: 1867
		public int charsWide = 0;

		// Token: 0x0400074C RID: 1868
		public bool frameSwitch = false;
	}
}
