using System;
using System.Linq;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000067 RID: 103
	internal class NetmapOrganizerExe : ExeModule, MainDisplayOverrideEXE
	{
		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000207 RID: 519 RVA: 0x0001C8E0 File Offset: 0x0001AAE0
		// (set) Token: 0x06000208 RID: 520 RVA: 0x0001C8F7 File Offset: 0x0001AAF7
		public bool DisplayOverrideIsActive { get; set; }

		// Token: 0x06000209 RID: 521 RVA: 0x0001C900 File Offset: 0x0001AB00
		public NetmapOrganizerExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = false;
			this.name = "NetmapOrganizer";
			this.ramCost = 300;
			this.IdentifierName = "NetmapOrganizer";
			this.DisplayOverrideIsActive = false;
			for (int i = 1; i < p.Length; i++)
			{
				if (p[i].ToLower().StartsWith("-c"))
				{
					this.AllowChaos = true;
				}
			}
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0001C983 File Offset: 0x0001AB83
		public override void Update(float t)
		{
			base.Update(t);
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0001C98E File Offset: 0x0001AB8E
		public override void Completed()
		{
			base.Completed();
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0001C998 File Offset: 0x0001AB98
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle contentAreaDest = base.GetContentAreaDest();
			ZoomingDotGridEffect.Render(contentAreaDest, this.spriteBatch, this.os.timer, this.os.highlightColor * 0.4f);
			int x = contentAreaDest.X + 10;
			int width = contentAreaDest.Width - 20;
			int num = contentAreaDest.Y + 50;
			if (!this.isExiting)
			{
				if (Button.doButton(10777001 + this.PID, x, num, width, 20, LocaleTerms.Loc("Scatter"), new Color?((this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Scatter) ? Color.White : this.os.highlightColor)))
				{
					this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Scatter;
				}
				num += 25;
				if (Button.doButton(10777003 + this.PID, x, num, width, 20, LocaleTerms.Loc("Grid"), new Color?((this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Grid) ? Color.White : this.os.highlightColor)))
				{
					this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Grid;
				}
				num += 25;
				if (Button.doButton(10777005 + this.PID, x, num, width, 20, LocaleTerms.Loc("Scan Sequence Grid"), new Color?((this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.LockGrid) ? Color.White : this.os.highlightColor)))
				{
					this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.LockGrid;
					this.os.netMap.visibleNodes = this.os.netMap.visibleNodes.Distinct<int>().ToList<int>();
				}
				num += 25;
				if (this.AllowChaos && Button.doButton(10777019 + this.PID, x, num, width, 20, LocaleTerms.Loc("CHAOS"), new Color?((this.os.netMap.SortingAlgorithm == NetmapSortingAlgorithm.Chaos) ? Color.White : this.os.highlightColor)))
				{
					this.os.netMap.SortingAlgorithm = NetmapSortingAlgorithm.Chaos;
				}
				num += 25;
				if (Button.doButton(10777088 + this.PID, x, contentAreaDest.Y + contentAreaDest.Height - 24, width, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0001CC5F File Offset: 0x0001AE5F
		public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
		{
		}

		// Token: 0x0400023B RID: 571
		private bool AllowChaos = false;
	}
}
