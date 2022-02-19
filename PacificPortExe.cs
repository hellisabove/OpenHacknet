using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000068 RID: 104
	internal class PacificPortExe : ExeModule
	{
		// Token: 0x0600020E RID: 526 RVA: 0x0001CC64 File Offset: 0x0001AE64
		public PacificPortExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.name = "Pacific_Portcrusher";
			this.ramCost = 190;
			this.IdentifierName = "PacificPortcrusher";
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			computer.hostileActionTaken();
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0001CCD0 File Offset: 0x0001AED0
		public override void Update(float t)
		{
			base.Update(t);
			if (this.elapsedTime < 6f && this.elapsedTime + t >= 6f)
			{
				this.Completed();
			}
			else if (this.elapsedTime < 6.2f && this.elapsedTime + t >= 6.2f)
			{
				this.isExiting = true;
			}
			this.elapsedTime += t;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0001CD50 File Offset: 0x0001AF50
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(192, this.os.thisComputer.ip);
				this.os.write("Pacific Portcrusher  >>" + LocaleTerms.Loc("SUCCESS") + "<<");
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0001CDC4 File Offset: 0x0001AFC4
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			if (this.effect == null)
			{
				this.effect = new FlyoutEffect(this.spriteBatch.GraphicsDevice, this.os.content, this.bounds.Width - 2, this.bounds.Height - 2);
			}
			float num = 1f;
			if (this.elapsedTime < 6f)
			{
				num = this.elapsedTime / 6f;
			}
			Rectangle contentAreaDest = base.GetContentAreaDest();
			if (contentAreaDest.Height > 2)
			{
				if (this.isExiting)
				{
					this.framesFlashed++;
				}
				this.effect.Draw((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds, contentAreaDest, this.spriteBatch, 50f * num, 3, 100f * Utils.QuadraticOutCurve(1f - num), this.os.highlightColor, false, this.isExiting && this.framesFlashed % 30 <= 3);
			}
		}

		// Token: 0x0400023D RID: 573
		private const float RUN_TIME = 6f;

		// Token: 0x0400023E RID: 574
		private const float IDLE_TIME = 6.2f;

		// Token: 0x0400023F RID: 575
		private float elapsedTime = 0f;

		// Token: 0x04000240 RID: 576
		private int framesFlashed = 0;

		// Token: 0x04000241 RID: 577
		private FlyoutEffect effect;
	}
}
