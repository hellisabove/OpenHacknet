using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000C1 RID: 193
	internal class HexClockExe : ExeModule
	{
		// Token: 0x060003EF RID: 1007 RVA: 0x0003CDE0 File Offset: 0x0003AFE0
		public HexClockExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "HexClock";
			this.ramCost = 55;
			this.IdentifierName = "HexClock";
			this.targetIP = this.os.thisComputer.ip;
			if (p.Length > 1 && (p[1].ToLower().StartsWith("-s") || p[1].ToLower().StartsWith("-n")))
			{
				this.stopUIChange = true;
			}
			else
			{
				this.os.write("HexClock Running. Use -s or -n for restricted mode\n ");
			}
			this.theme = ThemeManager.currentTheme;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x0003CE97 File Offset: 0x0003B097
		public override void Killed()
		{
			base.Killed();
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x0003CEA4 File Offset: 0x0003B0A4
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			DateTime now = DateTime.Now;
			string text = "#" + now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");
			Color color = Utils.ColorFromHexString(text);
			if (!this.stopUIChange)
			{
				this.AutoUpdateTheme(color);
			}
			Rectangle contentAreaDest = base.GetContentAreaDest();
			this.spriteBatch.Draw(Utils.white, contentAreaDest, color);
			int num = (int)((float)this.bounds.Width * 0.8f);
			int num2 = 30;
			Rectangle dest = new Rectangle(contentAreaDest.X + (contentAreaDest.Width / 2 - num / 2), contentAreaDest.Y + (contentAreaDest.Height / 2 - num2 / 2), num, num2);
			TextItem.doFontLabelToSize(dest, text, GuiData.font, Utils.AddativeWhite, false, false);
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x0003CFBC File Offset: 0x0003B1BC
		private void AutoUpdateTheme(Color c)
		{
			Color c2 = c;
			double num;
			double num2;
			double num3;
			Utils.RGB2HSL(c, out num, out num2, out num3);
			num2 = Math.Min(0.7, num2);
			num3 = Math.Max(0.2, num3);
			c = Utils.HSL2RGB(num, num2, num3);
			num3 = Math.Max(0.35, num3);
			double num4 = num - 0.5;
			if (num4 < 0.0)
			{
				num4 += 1.0;
			}
			Color defaultHighlightColor = Utils.HSL2RGB(num4, num2, num3);
			defaultHighlightColor = Utils.GetComplimentaryColor(c2);
			this.os.defaultHighlightColor = defaultHighlightColor;
			this.os.defaultTopBarColor = new Color(0, 0, 0, 60);
			this.os.highlightColor = this.os.defaultHighlightColor;
			this.os.highlightColor = this.os.defaultHighlightColor;
			this.os.moduleColorSolid = Color.Lerp(c, Utils.AddativeWhite, 0.5f);
			this.os.moduleColorSolidDefault = this.os.moduleColorSolid;
			this.os.moduleColorStrong = c;
			this.os.moduleColorStrong.A = 80;
			this.os.topBarColor = this.os.defaultTopBarColor;
			this.os.exeModuleTopBar = new Color(32, 22, 40, 80);
			this.os.exeModuleTitleText = new Color(91, 132, 207, 0);
			this.os.netmapToolTipColor = new Color(213, 245, 255, 0);
			this.os.netmapToolTipBackground = new Color(0, 0, 0, 70);
			this.os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 60);
			this.os.thisComputerNode = new Color(95, 220, 83);
			this.os.scanlinesColor = new Color(255, 255, 255, 15);
		}

		// Token: 0x04000495 RID: 1173
		private OSTheme theme;

		// Token: 0x04000496 RID: 1174
		private bool stopUIChange = false;
	}
}
