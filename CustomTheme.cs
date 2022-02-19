using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000071 RID: 113
	public class CustomTheme
	{
		// Token: 0x06000233 RID: 563 RVA: 0x0001FA0C File Offset: 0x0001DC0C
		public static CustomTheme Deserialize(string filepath)
		{
			CustomTheme result;
			using (FileStream fileStream = File.OpenRead(filepath))
			{
				result = (CustomTheme)Utils.DeserializeObject(fileStream, typeof(CustomTheme));
			}
			return result;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0001FA5C File Offset: 0x0001DC5C
		public string GetSaveString()
		{
			return Utils.SerializeObject(this);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0001FA74 File Offset: 0x0001DC74
		public void LoadIntoOS(object os_obj)
		{
			OS os = (OS)os_obj;
			FieldInfo[] fields = base.GetType().GetFields();
			FieldInfo[] fields2 = os.GetType().GetFields();
			for (int i = 0; i < fields2.Length; i++)
			{
				for (int j = 0; j < fields.Length; j++)
				{
					if (fields2[i].Name == fields[j].Name)
					{
						fields2[i].SetValue(os, fields[j].GetValue(this));
					}
				}
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0001FB08 File Offset: 0x0001DD08
		public OSTheme GetThemeForLayout()
		{
			OSTheme result;
			if (this.themeLayoutName == null)
			{
				result = OSTheme.HacknetBlue;
			}
			else
			{
				string text = this.themeLayoutName.ToLower();
				switch (text)
				{
				case "blue":
					return OSTheme.HacknetBlue;
				case "green":
					return OSTheme.HackerGreen;
				case "greencompact":
					return OSTheme.GreenCompact;
				case "white":
				case "csec":
					return OSTheme.HacknetWhite;
				case "mint":
				case "teal":
					return OSTheme.HacknetMint;
				case "colamaeleon":
				case "cola":
					return OSTheme.Colamaeleon;
				case "riptide":
					return OSTheme.Riptide;
				case "riptide2":
					return OSTheme.Riptide2;
				}
				result = OSTheme.HacknetPurple;
			}
			return result;
		}

		// Token: 0x0400027C RID: 636
		public Color defaultHighlightColor = new Color(0, 139, 199, 255);

		// Token: 0x0400027D RID: 637
		public Color defaultTopBarColor = new Color(130, 65, 27);

		// Token: 0x0400027E RID: 638
		public Color warningColor = Color.Red;

		// Token: 0x0400027F RID: 639
		public Color subtleTextColor = new Color(90, 90, 90);

		// Token: 0x04000280 RID: 640
		public Color darkBackgroundColor = new Color(8, 8, 8);

		// Token: 0x04000281 RID: 641
		public Color indentBackgroundColor = new Color(12, 12, 12);

		// Token: 0x04000282 RID: 642
		public Color outlineColor = new Color(68, 68, 68);

		// Token: 0x04000283 RID: 643
		public Color lockedColor = new Color(65, 16, 16, 200);

		// Token: 0x04000284 RID: 644
		public Color brightLockedColor = new Color(160, 0, 0);

		// Token: 0x04000285 RID: 645
		public Color brightUnlockedColor = new Color(0, 160, 0);

		// Token: 0x04000286 RID: 646
		public Color unlockedColor = new Color(39, 65, 36);

		// Token: 0x04000287 RID: 647
		public Color lightGray = new Color(180, 180, 180);

		// Token: 0x04000288 RID: 648
		public Color shellColor = new Color(222, 201, 24);

		// Token: 0x04000289 RID: 649
		public Color shellButtonColor = new Color(105, 167, 188);

		// Token: 0x0400028A RID: 650
		public Color moduleColorSolidDefault = new Color(50, 59, 90, 255);

		// Token: 0x0400028B RID: 651
		public Color moduleColorStrong = new Color(14, 28, 40, 80);

		// Token: 0x0400028C RID: 652
		public Color moduleColorBacking = new Color(5, 6, 7, 10);

		// Token: 0x0400028D RID: 653
		public Color semiTransText = new Color(120, 120, 120, 0);

		// Token: 0x0400028E RID: 654
		public Color terminalTextColor = new Color(213, 245, 255);

		// Token: 0x0400028F RID: 655
		public Color topBarTextColor = new Color(126, 126, 126, 100);

		// Token: 0x04000290 RID: 656
		public Color superLightWhite = new Color(2, 2, 2, 30);

		// Token: 0x04000291 RID: 657
		public Color connectedNodeHighlight = new Color(222, 0, 0, 195);

		// Token: 0x04000292 RID: 658
		public Color exeModuleTopBar = new Color(130, 65, 27, 80);

		// Token: 0x04000293 RID: 659
		public Color exeModuleTitleText = new Color(155, 85, 37, 0);

		// Token: 0x04000294 RID: 660
		public Color netmapToolTipColor = new Color(213, 245, 255, 0);

		// Token: 0x04000295 RID: 661
		public Color netmapToolTipBackground = new Color(0, 0, 0, 70);

		// Token: 0x04000296 RID: 662
		public Color topBarIconsColor = Color.White;

		// Token: 0x04000297 RID: 663
		public Color AFX_KeyboardMiddle = new Color(0, 120, 255);

		// Token: 0x04000298 RID: 664
		public Color AFX_KeyboardOuter = new Color(255, 150, 0);

		// Token: 0x04000299 RID: 665
		public Color AFX_WordLogo = new Color(0, 120, 255);

		// Token: 0x0400029A RID: 666
		public Color AFX_Other = new Color(0, 100, 255);

		// Token: 0x0400029B RID: 667
		public Color thisComputerNode = new Color(95, 220, 83);

		// Token: 0x0400029C RID: 668
		public Color scanlinesColor = new Color(255, 255, 255, 15);

		// Token: 0x0400029D RID: 669
		public string themeLayoutName = null;

		// Token: 0x0400029E RID: 670
		public string backgroundImagePath = null;

		// Token: 0x0400029F RID: 671
		public Color BackgroundImageFillColor = Color.Black;

		// Token: 0x040002A0 RID: 672
		public bool UseAspectPreserveBackgroundScaling = false;
	}
}
