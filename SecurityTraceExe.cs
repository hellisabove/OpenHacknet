using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200015F RID: 351
	internal class SecurityTraceExe : ExeModule
	{
		// Token: 0x060008DB RID: 2267 RVA: 0x00093F18 File Offset: 0x00092118
		public SecurityTraceExe(Rectangle location, OS _os) : base(location, _os)
		{
			this.name = "SecurityTrace";
			this.IdentifierName = "Security Tracer";
			this.ramCost = 150;
			if (this.os.connectedComp == null)
			{
				this.os.connectedComp = this.os.thisComputer;
			}
			this.os.traceTracker.start(30f);
			this.os.warningFlash();
		}

		// Token: 0x060008DC RID: 2268 RVA: 0x00093FA0 File Offset: 0x000921A0
		public override void Killed()
		{
			base.Killed();
			if (this.os.connectedComp == null || !(this.os.connectedComp.idName == "dGibson"))
			{
				this.os.traceTracker.stop();
			}
		}

		// Token: 0x060008DD RID: 2269 RVA: 0x00093FFC File Offset: 0x000921FC
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle contentAreaDest = base.GetContentAreaDest();
			contentAreaDest.X += 2;
			contentAreaDest.Width -= 4;
			contentAreaDest.Height -= Module.PANEL_HEIGHT + 1;
			contentAreaDest.Y += Module.PANEL_HEIGHT;
			PatternDrawer.draw(contentAreaDest, -1f, Color.Transparent, this.os.darkBackgroundColor, this.spriteBatch);
			PatternDrawer.draw(contentAreaDest, 3f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.errorTile);
		}
	}
}
