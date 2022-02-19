using System;
using Hacknet.Effects;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000062 RID: 98
	internal class FTPFastExe : ExeModule
	{
		// Token: 0x060001E8 RID: 488 RVA: 0x0001AC38 File Offset: 0x00018E38
		public FTPFastExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = true;
			this.name = "FTP_SprintBreak";
			this.ramCost = 190;
			this.IdentifierName = "FTPSprint";
			this.points.Init(this.os.content);
			this.points.AllowDoubleLines = true;
			this.points.Explode(70);
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			computer.hostileActionTaken();
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0001ACD8 File Offset: 0x00018ED8
		public override void Update(float t)
		{
			base.Update(t);
			this.points.attractionToCentreMass = this.elapsedTime;
			this.points.Update(t);
			this.points.Update(t);
			this.points.Update(t);
			if (this.elapsedTime < 7f && this.elapsedTime + t >= 7f)
			{
				this.Completed();
			}
			else if (this.elapsedTime < 7.5f && this.elapsedTime + t >= 7.5f)
			{
				this.isExiting = true;
				this.points.NodeColor = Color.White;
			}
			else
			{
				this.points.LineLengthPercentage = Math.Min(0.5f, this.elapsedTime / 7f * 0.7f);
			}
			this.elapsedTime += t;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0001ADCC File Offset: 0x00018FCC
		public override void Completed()
		{
			base.Completed();
			Computer computer = Programs.getComputer(this.os, this.targetIP);
			if (computer != null)
			{
				computer.openPort(21, this.os.thisComputer.ip);
				this.os.write(">> " + LocaleTerms.Loc("FTP Sprint Crack Successful"));
			}
			this.points.FlashComplete();
			this.points.Explode(30);
			this.points.timeRemainingWithoutAttract = 10f;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0001AE60 File Offset: 0x00019060
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			if (this.elapsedTime < 7f)
			{
				float num = this.elapsedTime / 7f;
			}
			Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height / 2 - 16, this.bounds.Width - 4, Math.Min(30, Math.Max(0, this.bounds.Height - 50)));
			Rectangle contentAreaDest = base.GetContentAreaDest();
			if (contentAreaDest.Height > 2)
			{
				this.points.Render(contentAreaDest, this.spriteBatch);
			}
		}

		// Token: 0x04000215 RID: 533
		private const float RUN_TIME = 7f;

		// Token: 0x04000216 RID: 534
		private const float IDLE_TIME = 7.5f;

		// Token: 0x04000217 RID: 535
		private float elapsedTime = 0f;

		// Token: 0x04000218 RID: 536
		private PointGatherEffect points = new PointGatherEffect();
	}
}
