using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200006B RID: 107
	internal class TorrentPortExe : ExeModule
	{
		// Token: 0x06000217 RID: 535 RVA: 0x0001DAA4 File Offset: 0x0001BCA4
		public TorrentPortExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "TorrentStreamInjector";
			this.ramCost = 360;
			this.IdentifierName = "TorrentInjector";
			this.BackgroundRainEffect.Init(this.os.content);
			this.BackgroundRainEffect.MaxVerticalLandingVariane += 0.04f;
			this.BackgroundRainEffect.FallRate = 0.5f;
			this.RainEffect.Init(this.os.content);
			this.RainEffect.ForceSpawnDrop(new Vector3(0.5f, 0f, 0f));
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			computer.hostileActionTaken();
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0001DBC0 File Offset: 0x0001BDC0
		public override void Update(float t)
		{
			base.Update(t);
			float num = 20f;
			if (this.elapsedTime < 1f)
			{
				num *= this.elapsedTime * 0.2f;
			}
			this.RainEffect.Update(t, this.isComplete ? 2f : num);
			this.BackgroundRainEffect.Update(t, num * 3f);
			if (this.elapsedTime < 4.8f && this.elapsedTime + t >= 4.8f)
			{
				this.Completed();
			}
			else if (this.elapsedTime < 16.5f && this.elapsedTime + t >= 16.5f)
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
			}
			this.elapsedTime += t;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0001DCD4 File Offset: 0x0001BED4
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(6881, this.os.thisComputer.ip);
				this.os.write(" - " + LocaleTerms.Loc("Torrent Stream Injection Complete") + " - ");
				this.isComplete = true;
				this.completionFlashTimer = this.completionFlashDuration;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0001DD5C File Offset: 0x0001BF5C
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			float num = 1f;
			if (this.elapsedTime < 4.8f)
			{
				num = this.elapsedTime / 4.8f;
			}
			Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
			rectangle.Width = (int)((float)rectangle.Width * num);
			if (this.isComplete)
			{
				Color color = this.os.highlightColor * 0.2f;
				color = Color.Lerp(color, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration);
				this.BackgroundRainEffect.Render(base.GetContentAreaDest(), this.spriteBatch, color, 20f, 40f);
			}
			Color color2 = this.os.highlightColor;
			if (this.isComplete)
			{
				color2 = Color.Lerp(color2, Utils.AddativeWhite, this.completionFlashTimer / this.completionFlashDuration);
			}
			this.RainEffect.Render(base.GetContentAreaDest(), this.spriteBatch, color2, 50f, 100f);
		}

		// Token: 0x0400024D RID: 589
		private const float RUN_TIME = 4.8f;

		// Token: 0x0400024E RID: 590
		private const float IDLE_TIME = 16.5f;

		// Token: 0x0400024F RID: 591
		private float elapsedTime = 0f;

		// Token: 0x04000250 RID: 592
		private float completionFlashDuration = 3.2f;

		// Token: 0x04000251 RID: 593
		private float completionFlashTimer = 0f;

		// Token: 0x04000252 RID: 594
		private bool isComplete = false;

		// Token: 0x04000253 RID: 595
		private RaindropsEffect RainEffect = new RaindropsEffect();

		// Token: 0x04000254 RID: 596
		private RaindropsEffect BackgroundRainEffect = new RaindropsEffect();
	}
}
