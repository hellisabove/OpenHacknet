using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200006F RID: 111
	internal class RTSPPortExe : ExeModule
	{
		// Token: 0x0600022D RID: 557 RVA: 0x0001F078 File Offset: 0x0001D278
		public RTSPPortExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "RTSPCrack";
			this.ramCost = 360;
			this.IdentifierName = "RTSPCrack";
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			computer.hostileActionTaken();
			this.spinner = new TrailLoadingSpinnerEffect(operatingSystem);
			this.preciceRamCost = (float)this.ramCost;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0001F138 File Offset: 0x0001D338
		public override void Update(float t)
		{
			base.Update(t);
			if (this.elapsedTime < RTSPPortExe.RUN_TIME && this.elapsedTime + t >= RTSPPortExe.RUN_TIME)
			{
				this.Completed();
			}
			else if (this.elapsedTime < RTSPPortExe.IDLE_TIME && this.elapsedTime + t >= RTSPPortExe.IDLE_TIME)
			{
				this.isExiting = true;
			}
			if (this.isComplete)
			{
				this.completionFlashTimer -= t;
				if (this.completionFlashTimer <= 0f)
				{
					this.completionFlashTimer = 0f;
				}
				if (this.ramCost > this.completeRamUse)
				{
					float num = 1f;
					float num2 = 2f;
					if (this.elapsedTime - RTSPPortExe.RUN_TIME < 2f)
					{
						num = (this.elapsedTime - RTSPPortExe.RUN_TIME) / num2;
					}
					this.preciceRamCost -= num * 0.5f;
					this.ramCost = (int)this.preciceRamCost;
					if (this.ramCost < this.completeRamUse)
					{
						this.ramCost = this.completeRamUse;
					}
				}
			}
			this.elapsedTime += t;
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0001F284 File Offset: 0x0001D484
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(554, this.os.thisComputer.ip);
				this.os.write("RTSPCrack Complete");
				this.isComplete = true;
				this.completionFlashTimer = this.completionFlashDuration;
			}
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0001F2F8 File Offset: 0x0001D4F8
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			float num = 1f;
			if (this.elapsedTime < RTSPPortExe.RUN_TIME)
			{
				num = this.elapsedTime / RTSPPortExe.RUN_TIME;
			}
			Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
			rectangle.Width = (int)((float)rectangle.Width * num);
			Color value = this.os.highlightColor;
			value = Color.Lerp(value, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration);
			this.spinner.Draw(base.GetContentAreaDest(), this.spriteBatch, RTSPPortExe.RUN_TIME, (float)Math.Max(0.0, (double)(RTSPPortExe.RUN_TIME - this.elapsedTime)), Math.Max(0f, this.elapsedTime - RTSPPortExe.RUN_TIME), null);
		}

		// Token: 0x04000272 RID: 626
		private static float RUN_TIME = 6.3f;

		// Token: 0x04000273 RID: 627
		private static float IDLE_TIME = 30.5f;

		// Token: 0x04000274 RID: 628
		private float elapsedTime = 0f;

		// Token: 0x04000275 RID: 629
		private float completionFlashDuration = 3.2f;

		// Token: 0x04000276 RID: 630
		private float completionFlashTimer = 0f;

		// Token: 0x04000277 RID: 631
		private bool isComplete = false;

		// Token: 0x04000278 RID: 632
		private TrailLoadingSpinnerEffect spinner;

		// Token: 0x04000279 RID: 633
		private int completeRamUse = 220;

		// Token: 0x0400027A RID: 634
		private float preciceRamCost = 0f;
	}
}
