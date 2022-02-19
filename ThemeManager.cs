using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hacknet.Effects;
using Hacknet.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000172 RID: 370
	public static class ThemeManager
	{
		// Token: 0x0600093C RID: 2364 RVA: 0x0009845C File Offset: 0x0009665C
		public static void init(ContentManager content)
		{
			ThemeManager.fileData = new Dictionary<OSTheme, string>();
			UTF8Encoding utf8Encoding = new UTF8Encoding();
			ThemeManager.hexGrid = new HexGridBackground(content);
			ThemeManager.hexGrid.HexScale = 0.12f;
			foreach (object obj in Enum.GetValues(typeof(OSTheme)))
			{
				OSTheme ostheme = (OSTheme)obj;
				byte[] bytes = utf8Encoding.GetBytes(ostheme.ToString() + ostheme.GetHashCode().ToString());
				string text = "";
				for (int i = 0; i < bytes.Length; i++)
				{
					text += bytes[i];
				}
				ThemeManager.fileData.Add(ostheme, text);
			}
		}

		// Token: 0x0600093D RID: 2365 RVA: 0x00098564 File Offset: 0x00096764
		public static void Update(float dt)
		{
			if (ThemeManager.hexGrid != null)
			{
				ThemeManager.hexGrid.Update(dt * 0.4f);
				ThemeManager.hexGrid.HexScale = 0.2f;
			}
			if (ThemeManager.framesTillWebUpdate >= 0)
			{
				ThemeManager.framesTillWebUpdate--;
				if (ThemeManager.framesTillWebUpdate == -1)
				{
					if (OS.currentInstance.connectedComp != null && OS.currentInstance.connectedComp.getDaemon(typeof(WebServerDaemon)) != null)
					{
						WebRenderer.setSize(ThemeManager.webWidth, ThemeManager.webHeight);
					}
					else
					{
						ThemeManager.framesTillWebUpdate = 0;
					}
				}
			}
		}

		// Token: 0x0600093E RID: 2366 RVA: 0x00098618 File Offset: 0x00096818
		public static void switchTheme(object osObject, string customThemePath)
		{
			string str = "Content/";
			if (Settings.IsInExtensionMode)
			{
				str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
			}
			if (File.Exists(str + customThemePath))
			{
				ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize(str + customThemePath);
				ThemeManager.LastLoadedCustomThemePath = customThemePath;
			}
			else if (File.Exists("Content/" + customThemePath))
			{
				str = "Content/";
				ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize(str + customThemePath);
			}
			ThemeManager.switchTheme(osObject, OSTheme.Custom);
		}

		// Token: 0x0600093F RID: 2367 RVA: 0x000986B8 File Offset: 0x000968B8
		public static void switchTheme(object osObject, OSTheme theme)
		{
			OS os = (OS)osObject;
			if (theme == OSTheme.Custom)
			{
				string text = ThemeManager.customBackgroundImageLoadPath;
				ThemeManager.switchTheme(os, OSTheme.HacknetBlue);
				ThemeManager.customBackgroundImageLoadPath = text;
				ThemeManager.switchThemeLayout(os, ThemeManager.LastLoadedCustomTheme.GetThemeForLayout());
				ThemeManager.loadCustomThemeBackground(os, ThemeManager.LastLoadedCustomTheme.backgroundImagePath);
				ThemeManager.LastLoadedCustomTheme.LoadIntoOS(os);
			}
			else
			{
				ThemeManager.switchThemeColors(os, theme);
				ThemeManager.loadThemeBackground(os, theme);
				ThemeManager.switchThemeLayout(os, theme);
			}
			ThemeManager.currentTheme = theme;
			os.RefreshTheme();
		}

		// Token: 0x06000940 RID: 2368 RVA: 0x00098748 File Offset: 0x00096948
		private static void switchThemeLayout(OS os, OSTheme theme)
		{
			int width = os.ScreenManager.GraphicsDevice.Viewport.Width;
			int height = os.ScreenManager.GraphicsDevice.Viewport.Height;
			int width2 = os.display.bounds.Width;
			int height2 = os.display.bounds.Height;
			int num;
			int num2;
			int num3;
			int num4;
			switch (theme)
			{
			case OSTheme.TerminalOnlyBlack:
			{
				Rectangle bounds = new Rectangle(-100000, -100000, 16, 16);
				os.terminal.bounds = new Rectangle(0, 0, width, height);
				os.netMap.bounds = bounds;
				os.display.bounds = bounds;
				os.ram.bounds = bounds;
				goto IL_8B8;
			}
			case OSTheme.HackerGreen:
				num = 205;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.44420000000000004);
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.5558);
				num4 = height - num - OS.TOP_BAR_HEIGHT - 6;
				os.terminal.Bounds = new Rectangle(width - num2 - RamModule.MODULE_WIDTH - 4, OS.TOP_BAR_HEIGHT, num2, height - OS.TOP_BAR_HEIGHT - 2);
				os.netMap.Bounds = new Rectangle(2, height - num - 2, num3 - 1, num);
				os.display.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num3 - 2, num4);
				os.ram.Bounds = new Rectangle(width - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			case OSTheme.HacknetWhite:
			{
				num = 205;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.44420000000000004);
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.5558);
				int height3 = height - num - OS.TOP_BAR_HEIGHT - 6;
				os.terminal.Bounds = new Rectangle(width - num2 - 2, OS.TOP_BAR_HEIGHT, num2, height3);
				os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4 + num3 + 4, height - num - 2, os.terminal.bounds.Width - 4, num);
				os.display.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, num3, height - OS.TOP_BAR_HEIGHT - 2);
				os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			}
			case OSTheme.HacknetMint:
			{
				num = 205;
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.5058);
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.4942);
				int height3 = height - num - OS.TOP_BAR_HEIGHT - 6;
				os.terminal.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, num2, height3);
				os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, height - num - 2, num2 - 1, num);
				os.display.Bounds = new Rectangle(width - num3 - 2, OS.TOP_BAR_HEIGHT, num3 - 1, height - OS.TOP_BAR_HEIGHT - 2);
				os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			}
			case OSTheme.Colamaeleon:
			{
				num = (int)((double)height * 0.33);
				double num5 = 0.4;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * (1.0 - num5)) - 6;
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * num5);
				num4 = height - num - OS.TOP_BAR_HEIGHT - 6 + 1;
				os.terminal.Bounds = new Rectangle(width - num2 - RamModule.MODULE_WIDTH - 4, OS.TOP_BAR_HEIGHT, num2, height - OS.TOP_BAR_HEIGHT - 2);
				os.netMap.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num3, num);
				os.display.Bounds = new Rectangle(2, os.netMap.bounds.Y + os.netMap.bounds.Height + 3, num3, num4);
				os.ram.Bounds = new Rectangle(width - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			}
			case OSTheme.GreenCompact:
				num = 205;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.44420000000000004);
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.5558);
				num4 = height - num - OS.TOP_BAR_HEIGHT - 3;
				os.terminal.Bounds = new Rectangle(width - num2 - RamModule.MODULE_WIDTH - 4 - 2, OS.TOP_BAR_HEIGHT, num2 + 3, height - OS.TOP_BAR_HEIGHT - 2);
				os.netMap.Bounds = new Rectangle(1, height - num - 2, num3 - 1, num);
				os.display.Bounds = new Rectangle(1, OS.TOP_BAR_HEIGHT, num3 - 1, num4);
				os.ram.Bounds = new Rectangle(width - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			case OSTheme.Riptide:
			{
				num = (int)((double)height * 0.33);
				double num5 = 0.51;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * (1.0 - num5)) - 2;
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * num5);
				num4 = height - num - OS.TOP_BAR_HEIGHT - 6 + 2;
				os.terminal.Bounds = new Rectangle(width - num2 - RamModule.MODULE_WIDTH - 6, OS.TOP_BAR_HEIGHT, num2 + 2, height - OS.TOP_BAR_HEIGHT - 2);
				os.netMap.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num3, num);
				os.display.Bounds = new Rectangle(2, os.netMap.bounds.Y + os.netMap.bounds.Height + 2, num3, num4);
				os.ram.Bounds = new Rectangle(width - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			}
			case OSTheme.Riptide2:
			{
				num = (int)((double)height * 0.33);
				double num5 = 0.55;
				num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * (1.0 - num5)) - 6;
				num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * num5);
				num4 = height - num - OS.TOP_BAR_HEIGHT - 6 + 1;
				int x = width - num2 - RamModule.MODULE_WIDTH - 4;
				os.display.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, num3 + 4, height - OS.TOP_BAR_HEIGHT - 2);
				os.netMap.Bounds = new Rectangle(x, OS.TOP_BAR_HEIGHT, num2, num);
				os.terminal.Bounds = new Rectangle(x, os.netMap.bounds.Y + os.netMap.bounds.Height, num2, num4 + 3);
				os.ram.Bounds = new Rectangle(width - RamModule.MODULE_WIDTH - 2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
				goto IL_8B8;
			}
			}
			num = 205;
			num2 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.44420000000000004);
			num3 = (int)((double)(width - RamModule.MODULE_WIDTH - 6) * 0.5558) + 1;
			num4 = height - num - OS.TOP_BAR_HEIGHT - 6;
			if (theme == OSTheme.HacknetPurple)
			{
				num4 += 2;
				num3++;
			}
			os.terminal.Bounds = new Rectangle(width - num2 - 2, OS.TOP_BAR_HEIGHT, num2, height - OS.TOP_BAR_HEIGHT - 2);
			os.netMap.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, height - num - 2, num3 - 2, num);
			os.display.Bounds = new Rectangle(RamModule.MODULE_WIDTH + 4, OS.TOP_BAR_HEIGHT, num3 - 2, num4);
			os.ram.Bounds = new Rectangle(2, OS.TOP_BAR_HEIGHT, RamModule.MODULE_WIDTH, os.totalRam + RamModule.contentStartOffset);
			IL_8B8:
			if (ThemeManager.HasNeverSwappedThemeBefore || os.display.bounds.Width != width2 || os.display.bounds.Height != height2)
			{
				ThemeManager.webWidth = os.display.bounds.Width;
				ThemeManager.webHeight = os.display.bounds.Height;
				ThemeManager.framesTillWebUpdate = 600;
				ThemeManager.HasNeverSwappedThemeBefore = false;
			}
		}

		// Token: 0x06000941 RID: 2369 RVA: 0x0009909C File Offset: 0x0009729C
		internal static void loadCustomThemeBackground(OS os, string imagePathName)
		{
			if (imagePathName == ThemeManager.customBackgroundImageLoadPath)
			{
				ThemeManager.backgroundImage = ThemeManager.lastLoadedCustomBackground;
			}
			else
			{
				if (ThemeManager.backgroundNeedsDisposal)
				{
					Texture2D oldBackground = ThemeManager.backgroundImage;
					os.delayer.Post(ActionDelayer.NextTick(), delegate
					{
						oldBackground.Dispose();
					});
				}
				if (string.IsNullOrWhiteSpace(imagePathName))
				{
					ThemeManager.backgroundImage = null;
				}
				else
				{
					string path = Utils.GetFileLoadPrefix() + imagePathName;
					if (!File.Exists(path))
					{
						path = "Content/" + imagePathName;
					}
					if (File.Exists(path))
					{
						try
						{
							using (FileStream fileStream = File.OpenRead(path))
							{
								ThemeManager.backgroundImage = Texture2D.FromStream(GuiData.spriteBatch.GraphicsDevice, fileStream);
								ThemeManager.backgroundNeedsDisposal = true;
							}
							ThemeManager.lastLoadedCustomBackground = ThemeManager.backgroundImage;
							ThemeManager.customBackgroundImageLoadPath = imagePathName;
						}
						catch (Exception)
						{
							ThemeManager.backgroundImage = null;
						}
					}
					else
					{
						ThemeManager.backgroundImage = null;
					}
				}
			}
		}

		// Token: 0x06000942 RID: 2370 RVA: 0x000991D0 File Offset: 0x000973D0
		internal static void loadThemeBackground(OS os, OSTheme theme)
		{
			switch (theme)
			{
			case OSTheme.TerminalOnlyBlack:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/NoThemeWallpaper");
				break;
			default:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/Razzamataz2");
				break;
			case OSTheme.HacknetTeal:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/AbstractWaves");
				break;
			case OSTheme.HacknetYellow:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/DarkenedAway");
				break;
			case OSTheme.HackerGreen:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/GreenAbstract");
				break;
			case OSTheme.HacknetWhite:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/AwayTooLong");
				break;
			case OSTheme.HacknetPurple:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/BlueCirclesBackground");
				break;
			case OSTheme.HacknetMint:
				ThemeManager.backgroundImage = os.content.Load<Texture2D>("Themes/WaterWashGreen2");
				break;
			}
			ThemeManager.backgroundNeedsDisposal = false;
		}

		// Token: 0x06000943 RID: 2371 RVA: 0x000992DC File Offset: 0x000974DC
		internal static void switchThemeColors(OS os, OSTheme theme)
		{
			os.displayModuleExtraLayerBackingColor = Color.Transparent;
			os.UseAspectPreserveBackgroundScaling = false;
			os.BackgroundImageFillColor = Color.Black;
			switch (theme)
			{
			case OSTheme.TerminalOnlyBlack:
			{
				Color color = new Color(0, 0, 0, 0);
				os.defaultHighlightColor = new Color(0, 0, 0, 200);
				os.defaultTopBarColor = new Color(0, 0, 0, 0);
				os.highlightColor = os.defaultHighlightColor;
				os.shellColor = color;
				os.shellButtonColor = color;
				os.moduleColorSolid = new Color(0, 0, 0, 0);
				os.moduleColorStrong = new Color(0, 0, 0, 0);
				os.moduleColorSolidDefault = new Color(0, 0, 0, 0);
				os.moduleColorBacking = color;
				os.topBarColor = os.defaultTopBarColor;
				os.terminalTextColor = new Color(255, 254, 235);
				os.AFX_KeyboardMiddle = new Color(0, 0, 0);
				os.AFX_KeyboardOuter = new Color(0, 0, 0);
				os.AFX_WordLogo = new Color(0, 0, 0);
				os.AFX_Other = new Color(0, 0, 0);
				os.exeModuleTopBar = color;
				os.exeModuleTitleText = color;
				break;
			}
			case OSTheme.HacknetBlue:
				os.defaultHighlightColor = new Color(0, 139, 199, 255);
				os.defaultTopBarColor = new Color(130, 65, 27);
				os.warningColor = Color.Red;
				os.highlightColor = os.defaultHighlightColor;
				os.subtleTextColor = new Color(90, 90, 90);
				os.darkBackgroundColor = new Color(8, 8, 8);
				os.indentBackgroundColor = new Color(12, 12, 12);
				os.outlineColor = new Color(68, 68, 68);
				os.lockedColor = new Color(65, 16, 16, 200);
				os.brightLockedColor = new Color(160, 0, 0);
				os.brightUnlockedColor = new Color(0, 160, 0);
				os.unlockedColor = new Color(39, 65, 36);
				os.lightGray = new Color(180, 180, 180);
				os.shellColor = new Color(222, 201, 24);
				os.shellButtonColor = new Color(105, 167, 188);
				os.moduleColorSolid = new Color(50, 59, 90, 255);
				os.moduleColorSolidDefault = new Color(50, 59, 90, 255);
				os.moduleColorStrong = new Color(14, 28, 40, 80);
				os.moduleColorBacking = new Color(5, 6, 7, 10);
				os.topBarColor = os.defaultTopBarColor;
				os.semiTransText = new Color(120, 120, 120, 0);
				os.terminalTextColor = new Color(213, 245, 255);
				os.topBarTextColor = new Color(126, 126, 126, 100);
				os.superLightWhite = new Color(2, 2, 2, 30);
				os.connectedNodeHighlight = new Color(222, 0, 0, 195);
				os.exeModuleTopBar = new Color(130, 65, 27, 80);
				os.exeModuleTitleText = new Color(155, 85, 37, 0);
				os.netmapToolTipColor = new Color(213, 245, 255, 0);
				os.netmapToolTipBackground = new Color(0, 0, 0, 70);
				os.topBarIconsColor = Color.White;
				os.AFX_KeyboardMiddle = new Color(0, 120, 255);
				os.AFX_KeyboardOuter = new Color(255, 150, 0);
				os.AFX_WordLogo = new Color(0, 120, 255);
				os.AFX_Other = new Color(0, 100, 255);
				os.thisComputerNode = new Color(95, 220, 83);
				os.scanlinesColor = new Color(255, 255, 255, 15);
				break;
			case OSTheme.HacknetTeal:
				os.defaultHighlightColor = new Color(59, 134, 134, 255);
				os.defaultTopBarColor = new Color(11, 72, 107);
				os.warningColor = Color.Red;
				os.highlightColor = os.defaultHighlightColor;
				os.subtleTextColor = new Color(90, 90, 90);
				os.darkBackgroundColor = new Color(8, 8, 8);
				os.indentBackgroundColor = new Color(12, 12, 12);
				os.outlineColor = new Color(68, 68, 68);
				os.lockedColor = new Color(65, 16, 16, 200);
				os.brightLockedColor = new Color(160, 0, 0);
				os.brightUnlockedColor = new Color(0, 160, 0);
				os.unlockedColor = new Color(39, 65, 36);
				os.lightGray = new Color(180, 180, 180);
				os.shellColor = new Color(121, 189, 154);
				os.shellButtonColor = new Color(207, 240, 158);
				os.moduleColorSolid = new Color(59, 134, 134);
				os.moduleColorSolidDefault = new Color(59, 134, 134);
				os.moduleColorStrong = new Color(14, 28, 40, 80);
				os.moduleColorBacking = new Color(5, 7, 6, 200);
				os.topBarColor = os.defaultTopBarColor;
				os.semiTransText = new Color(120, 120, 120, 0);
				os.terminalTextColor = new Color(213, 245, 255);
				os.topBarTextColor = new Color(126, 126, 126, 100);
				os.superLightWhite = new Color(2, 2, 2, 30);
				os.connectedNodeHighlight = new Color(222, 0, 0, 195);
				os.exeModuleTopBar = new Color(12, 33, 33, 80);
				os.exeModuleTitleText = new Color(11, 72, 107, 0);
				os.netmapToolTipColor = new Color(213, 245, 255, 0);
				os.netmapToolTipBackground = new Color(0, 0, 0, 70);
				os.topBarIconsColor = Color.White;
				os.thisComputerNode = new Color(95, 220, 83);
				os.AFX_KeyboardMiddle = new Color(168, 219, 168);
				os.AFX_KeyboardOuter = new Color(121, 189, 154);
				os.AFX_WordLogo = new Color(59, 134, 134);
				os.AFX_Other = new Color(207, 240, 158);
				os.scanlinesColor = new Color(255, 255, 255, 15);
				break;
			case OSTheme.HacknetYellow:
				os.defaultHighlightColor = new Color(186, 98, 9, 255);
				os.defaultTopBarColor = new Color(89, 48, 6);
				os.highlightColor = os.defaultHighlightColor;
				os.shellColor = new Color(60, 107, 85);
				os.shellButtonColor = new Color(207, 240, 158);
				os.moduleColorSolid = new Color(186, 170, 67, 10);
				os.moduleColorStrong = new Color(201, 154, 10, 10);
				os.moduleColorBacking = new Color(5, 7, 6, 200);
				os.topBarColor = os.defaultTopBarColor;
				os.AFX_KeyboardMiddle = new Color(255, 150, 0);
				os.AFX_KeyboardOuter = new Color(255, 204, 0);
				os.AFX_WordLogo = new Color(255, 179, 0);
				os.AFX_Other = new Color(255, 221, 0);
				os.exeModuleTopBar = new Color(12, 33, 33, 80);
				os.exeModuleTitleText = new Color(11, 72, 107, 0);
				break;
			case OSTheme.HackerGreen:
				os.defaultHighlightColor = new Color(135, 222, 109, 200);
				os.defaultTopBarColor = new Color(6, 40, 16);
				os.highlightColor = os.defaultHighlightColor;
				os.shellColor = new Color(135, 222, 109);
				os.shellButtonColor = new Color(207, 240, 158);
				os.moduleColorSolid = new Color(60, 222, 10, 100);
				os.moduleColorSolidDefault = new Color(60, 222, 10, 100);
				os.moduleColorStrong = new Color(10, 80, 20, 50);
				os.moduleColorBacking = new Color(6, 6, 6, 200);
				os.topBarColor = os.defaultTopBarColor;
				os.terminalTextColor = new Color(95, 255, 70);
				os.AFX_KeyboardMiddle = new Color(60, 255, 10);
				os.AFX_KeyboardOuter = new Color(60, 222, 10);
				os.AFX_WordLogo = new Color(0, 255, 20);
				os.AFX_Other = new Color(0, 255, 0);
				os.exeModuleTopBar = new Color(12, 33, 33, 80);
				os.exeModuleTitleText = new Color(11, 107, 20, 0);
				os.topBarIconsColor = Color.White;
				break;
			case OSTheme.HacknetWhite:
				os.defaultHighlightColor = new Color(185, 219, 255, 255);
				os.defaultTopBarColor = new Color(20, 20, 20);
				os.highlightColor = os.defaultHighlightColor;
				os.shellColor = new Color(156, 185, 190);
				os.shellButtonColor = new Color(159, 220, 231);
				os.moduleColorSolid = new Color(220, 222, 220, 100);
				os.moduleColorSolidDefault = new Color(220, 222, 220, 100);
				os.moduleColorStrong = new Color(71, 71, 71, 50);
				os.moduleColorBacking = new Color(6, 6, 6, 205);
				os.topBarColor = os.defaultTopBarColor;
				os.terminalTextColor = new Color(255, 255, 255);
				os.AFX_KeyboardMiddle = new Color(220, 220, 220);
				os.AFX_KeyboardOuter = new Color(180, 180, 180);
				os.AFX_WordLogo = new Color(170, 80, 80);
				os.AFX_Other = new Color(255, 255, 255);
				os.exeModuleTopBar = new Color(20, 20, 20, 80);
				os.exeModuleTitleText = new Color(200, 200, 200, 0);
				os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 140);
				break;
			case OSTheme.HacknetPurple:
				os.defaultHighlightColor = new Color(35, 158, 121);
				os.defaultTopBarColor = new Color(0, 0, 0, 60);
				os.highlightColor = os.defaultHighlightColor;
				os.highlightColor = os.defaultHighlightColor;
				os.moduleColorSolid = new Color(154, 119, 189, 255);
				os.moduleColorSolidDefault = new Color(154, 119, 189, 255);
				os.moduleColorStrong = new Color(27, 14, 40, 80);
				os.moduleColorBacking = new Color(6, 5, 7, 205);
				os.topBarColor = os.defaultTopBarColor;
				os.exeModuleTopBar = new Color(32, 22, 40, 80);
				os.exeModuleTitleText = new Color(91, 132, 207, 0);
				os.netmapToolTipColor = new Color(213, 245, 255, 0);
				os.netmapToolTipBackground = new Color(0, 0, 0, 70);
				os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 60);
				os.AFX_KeyboardMiddle = new Color(190, 0, 255);
				os.AFX_KeyboardOuter = new Color(20, 90, 255);
				os.AFX_WordLogo = new Color(0, 255, 60);
				os.AFX_Other = new Color(0, 201, 141);
				os.thisComputerNode = new Color(95, 220, 83);
				os.scanlinesColor = new Color(255, 255, 255, 15);
				break;
			case OSTheme.HacknetMint:
				os.defaultHighlightColor = new Color(35, 158, 121);
				os.defaultTopBarColor = new Color(0, 0, 0, 40);
				os.topBarColor = os.defaultTopBarColor;
				os.highlightColor = os.defaultHighlightColor;
				os.moduleColorSolid = new Color(150, 150, 150, 255);
				os.moduleColorSolidDefault = new Color(150, 150, 150, 255);
				os.moduleColorStrong = new Color(43, 43, 43, 80);
				os.moduleColorBacking = new Color(6, 6, 6, 145);
				os.displayModuleExtraLayerBackingColor = new Color(0, 0, 0, 70);
				os.thisComputerNode = new Color(95, 220, 83);
				os.topBarIconsColor = new Color(49, 224, 172);
				os.AFX_KeyboardMiddle = new Color(35, 255, 173);
				os.AFX_KeyboardOuter = new Color(180, 255, 255);
				os.AFX_WordLogo = new Color(0, 255, 60);
				os.AFX_Other = new Color(220, 220, 220);
				os.exeModuleTopBar = new Color(20, 20, 20, 80);
				os.exeModuleTitleText = new Color(49, 224, 172, 0);
				os.scanlinesColor = new Color(255, 255, 255, 15);
				break;
			}
		}

		// Token: 0x06000944 RID: 2372 RVA: 0x0009A108 File Offset: 0x00098308
		public static Color GetRepresentativeColorForTheme(OSTheme theme)
		{
			Color result;
			switch (theme)
			{
			default:
				result = Utils.VeryDarkGray;
				break;
			case OSTheme.HacknetBlue:
				result = new Color(0, 139, 199, 255);
				break;
			case OSTheme.HacknetTeal:
				result = new Color(59, 134, 134, 255);
				break;
			case OSTheme.HacknetYellow:
				result = new Color(186, 98, 9, 255);
				break;
			case OSTheme.HackerGreen:
				result = new Color(135, 222, 109, 200);
				break;
			case OSTheme.HacknetWhite:
				result = new Color(185, 219, 255, 255);
				break;
			case OSTheme.HacknetPurple:
				result = new Color(111, 89, 171, 255);
				break;
			case OSTheme.HacknetMint:
				result = new Color(35, 158, 121);
				break;
			}
			return result;
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x0009A1F4 File Offset: 0x000983F4
		public static void drawBackgroundImage(SpriteBatch sb, Rectangle area)
		{
			switch (ThemeManager.currentTheme)
			{
			default:
				sb.Draw(ThemeManager.backgroundImage, area, (ThemeManager.currentTheme == OSTheme.HackerGreen) ? new Color(100, 150, 100) : Color.White);
				break;
			case OSTheme.HacknetYellow:
			{
				sb.Draw(Utils.white, area, new Color(51, 38, 0, 255));
				Rectangle dest = new Rectangle(area.X - 30, area.Y - 20, area.Width + 60, area.Height + 40);
				ThemeManager.hexGrid.Draw(dest, sb, Color.Transparent, new Color(255, 217, 105) * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
				break;
			}
			case OSTheme.Custom:
				if (ThemeManager.backgroundImage != null)
				{
					if (OS.currentInstance.UseAspectPreserveBackgroundScaling)
					{
						sb.Draw(Utils.white, area, OS.currentInstance.BackgroundImageFillColor);
						Utils.DrawSpriteAspectCorrect(area, sb, ThemeManager.backgroundImage, Color.White, false);
					}
					else
					{
						sb.Draw(ThemeManager.backgroundImage, area, Color.White);
					}
				}
				else
				{
					sb.Draw(Utils.white, area, Color.Lerp(Color.Black, ThemeManager.LastLoadedCustomTheme.moduleColorBacking, 0.33f));
					Rectangle dest = new Rectangle(area.X - 30, area.Y - 20, area.Width + 60, area.Height + 40);
					ThemeManager.hexGrid.Draw(dest, sb, Color.Transparent, Color.Lerp(Color.Transparent, ThemeManager.LastLoadedCustomTheme.moduleColorStrong, 0.2f), HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
				}
				break;
			}
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x0009A3DC File Offset: 0x000985DC
		public static string getThemeDataString(OSTheme theme)
		{
			return ThemeManager.fileData[theme];
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x0009A3FC File Offset: 0x000985FC
		public static string getThemeDataStringForCustomTheme(string customThemePath)
		{
			return ThemeManager.getThemeDataString(OSTheme.Custom) + ThemeManager.CustomThemeIDSeperator + FileEncrypter.EncryptString(customThemePath, "", "", "", null);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0009A490 File Offset: 0x00098690
		public static OSTheme getThemeForDataString(string data)
		{
			OSTheme result;
			if (data.Contains(ThemeManager.CustomThemeIDSeperator))
			{
				string[] seperated = data.Split(new string[]
				{
					ThemeManager.CustomThemeIDSeperator
				}, StringSplitOptions.RemoveEmptyEntries);
				OSTheme key = ThemeManager.fileData.FirstOrDefault((KeyValuePair<OSTheme, string> x) => x.Value == seperated[0]).Key;
				try
				{
					string str = FileEncrypter.DecryptString(seperated[1], "")[2];
					string str2 = "Content/";
					if (Settings.IsInExtensionMode)
					{
						str2 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
					}
					ThemeManager.LastLoadedCustomTheme = CustomTheme.Deserialize(str2 + str);
				}
				catch (Exception)
				{
					ThemeManager.LastLoadedCustomTheme = null;
					return OSTheme.TerminalOnlyBlack;
				}
				result = key;
			}
			else
			{
				result = ThemeManager.fileData.FirstOrDefault((KeyValuePair<OSTheme, string> x) => x.Value == data).Key;
			}
			return result;
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x0009A5BC File Offset: 0x000987BC
		public static void setThemeOnComputer(object computerObject, OSTheme theme)
		{
			Computer computer = (Computer)computerObject;
			Folder folder = computer.files.root.searchForFolder("sys");
			if (folder.containsFile("x-server.sys"))
			{
				FileEntry fileEntry = folder.searchForFile("x-server.sys");
				fileEntry.data = ThemeManager.getThemeDataString(theme);
			}
			else
			{
				FileEntry item = new FileEntry(ThemeManager.getThemeDataString(theme), "x-server.sys");
				folder.files.Add(item);
			}
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x0009A638 File Offset: 0x00098838
		public static void setThemeOnComputer(object computerObject, string customThemePath)
		{
			Computer computer = (Computer)computerObject;
			Folder folder = computer.files.root.searchForFolder("sys");
			string themeDataStringForCustomTheme = ThemeManager.getThemeDataStringForCustomTheme(customThemePath);
			if (folder.containsFile("x-server.sys"))
			{
				FileEntry fileEntry = folder.searchForFile("x-server.sys");
				fileEntry.data = themeDataStringForCustomTheme;
			}
			else
			{
				FileEntry item = new FileEntry(themeDataStringForCustomTheme, "x-server.sys");
				folder.files.Add(item);
			}
		}

		// Token: 0x04000AD1 RID: 2769
		private static string CustomThemeIDSeperator = "___";

		// Token: 0x04000AD2 RID: 2770
		private static Texture2D backgroundImage;

		// Token: 0x04000AD3 RID: 2771
		private static Texture2D lastLoadedCustomBackground;

		// Token: 0x04000AD4 RID: 2772
		private static string customBackgroundImageLoadPath = null;

		// Token: 0x04000AD5 RID: 2773
		private static bool backgroundNeedsDisposal = false;

		// Token: 0x04000AD6 RID: 2774
		public static OSTheme currentTheme;

		// Token: 0x04000AD7 RID: 2775
		public static CustomTheme LastLoadedCustomTheme = null;

		// Token: 0x04000AD8 RID: 2776
		public static string LastLoadedCustomThemePath = null;

		// Token: 0x04000AD9 RID: 2777
		private static HexGridBackground hexGrid;

		// Token: 0x04000ADA RID: 2778
		private static Dictionary<OSTheme, string> fileData;

		// Token: 0x04000ADB RID: 2779
		private static int webWidth;

		// Token: 0x04000ADC RID: 2780
		private static int webHeight;

		// Token: 0x04000ADD RID: 2781
		private static int framesTillWebUpdate = -1;

		// Token: 0x04000ADE RID: 2782
		private static bool HasNeverSwappedThemeBefore = true;
	}
}
