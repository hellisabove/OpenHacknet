using System;
using System.Collections.Generic;
using System.Text;
using Hacknet.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x0200011A RID: 282
	public static class GuiData
	{
		// Token: 0x060006A0 RID: 1696 RVA: 0x0006D068 File Offset: 0x0006B268
		public static void InitFontOptions(ContentManager content)
		{
			string name = GuiData.ActiveFontConfig.name;
			GuiData.FontConfigs.Clear();
			GuiData.FontConfigs.Add(new GuiData.FontCongifOption
			{
				name = "default",
				smallFont = content.Load<SpriteFont>("Font12"),
				tinyFont = content.Load<SpriteFont>("Font10"),
				bigFont = content.Load<SpriteFont>("Font23"),
				tinyFontCharHeight = 10f
			});
			if (string.IsNullOrEmpty(name))
			{
				GuiData.ActiveFontConfig = GuiData.FontConfigs[0];
			}
			GuiData.FontConfigs.Add(new GuiData.FontCongifOption
			{
				name = "medium",
				smallFont = content.Load<SpriteFont>("Font14"),
				tinyFont = content.Load<SpriteFont>("Font12"),
				bigFont = content.Load<SpriteFont>("Font23"),
				tinyFontCharHeight = 14f
			});
			GuiData.FontConfigs.Add(new GuiData.FontCongifOption
			{
				name = "large",
				smallFont = content.Load<SpriteFont>("Font16"),
				tinyFont = content.Load<SpriteFont>("Font14"),
				bigFont = content.Load<SpriteFont>("Font23"),
				tinyFontCharHeight = 16f
			});
			bool flag = false;
			for (int i = 0; i < GuiData.FontConfigs.Count; i++)
			{
				if (GuiData.FontConfigs[i].name == name)
				{
					GuiData.ActivateFontConfig(GuiData.FontConfigs[i]);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				GuiData.ActivateFontConfig(GuiData.FontConfigs[0]);
			}
			if (!GuiData.LocaleFontConfigs.ContainsKey("en-us"))
			{
				GuiData.LocaleFontConfigs.Add("en-us", GuiData.FontConfigs);
			}
		}

		// Token: 0x060006A1 RID: 1697 RVA: 0x0006D26C File Offset: 0x0006B46C
		public static void ActivateFontConfig(string configName)
		{
			List<GuiData.FontCongifOption> list = GuiData.FontConfigs;
			if (GuiData.LocaleFontConfigs.ContainsKey(Settings.ActiveLocale))
			{
				list = GuiData.LocaleFontConfigs[Settings.ActiveLocale];
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].name == configName)
				{
					GuiData.ActivateFontConfig(list[i]);
					break;
				}
			}
		}

		// Token: 0x060006A2 RID: 1698 RVA: 0x0006D2E8 File Offset: 0x0006B4E8
		public static void ActivateFontConfig(GuiData.FontCongifOption config)
		{
			if (config.detailFont != null)
			{
				GuiData.detailfont = config.detailFont;
			}
			GuiData.smallfont = config.smallFont;
			GuiData.tinyfont = config.tinyFont;
			GuiData.font = config.bigFont;
			GuiData.ActiveFontConfig = config;
		}

		// Token: 0x060006A3 RID: 1699 RVA: 0x0006D33C File Offset: 0x0006B53C
		public static void init(GameWindow window)
		{
			if (!GuiData.initialized)
			{
				GuiData.TextInputHook = new TextInputHook(window.Handle);
				GuiData.initialized = true;
			}
		}

		// Token: 0x060006A4 RID: 1700 RVA: 0x0006D36C File Offset: 0x0006B56C
		public static void doInput()
		{
			GuiData.lastMouse = GuiData.mouse;
			GuiData.mouse = Mouse.GetState();
			if (GuiData.lastMouseWheelPos == -1)
			{
				GuiData.lastMouseWheelPos = GuiData.mouse.ScrollWheelValue;
			}
			GuiData.lastMouseScroll = GuiData.lastMouseWheelPos - GuiData.mouse.ScrollWheelValue;
			GuiData.lastMouseWheelPos = GuiData.mouse.ScrollWheelValue;
			GuiData.blockingInput = false;
			GuiData.blockingTextInput = GuiData.willBlockTextInput;
			GuiData.willBlockTextInput = false;
		}

		// Token: 0x060006A5 RID: 1701 RVA: 0x0006D3E6 File Offset: 0x0006B5E6
		public static void doInput(InputState input)
		{
			GuiData.doInput();
			GuiData.lastInput = input;
		}

		// Token: 0x060006A6 RID: 1702 RVA: 0x0006D3F5 File Offset: 0x0006B5F5
		public static void setTimeStep(float t)
		{
			GuiData.lastTimeStep = t;
		}

		// Token: 0x060006A7 RID: 1703 RVA: 0x0006D400 File Offset: 0x0006B600
		public static KeyboardState getKeyboadState()
		{
			return GuiData.lastInput.CurrentKeyboardStates[0];
		}

		// Token: 0x060006A8 RID: 1704 RVA: 0x0006D428 File Offset: 0x0006B628
		public static KeyboardState getLastKeyboadState()
		{
			return GuiData.lastInput.LastKeyboardStates[0];
		}

		// Token: 0x060006A9 RID: 1705 RVA: 0x0006D450 File Offset: 0x0006B650
		public static Vector2 getMousePos()
		{
			GuiData.temp.X = (float)GuiData.mouse.X - GuiData.scrollOffset.X;
			GuiData.temp.Y = (float)GuiData.mouse.Y - GuiData.scrollOffset.Y;
			return GuiData.temp;
		}

		// Token: 0x060006AA RID: 1706 RVA: 0x0006D4A8 File Offset: 0x0006B6A8
		public static Point getMousePoint()
		{
			GuiData.tmpPoint.X = GuiData.mouse.X - (int)GuiData.scrollOffset.X;
			GuiData.tmpPoint.Y = GuiData.mouse.Y - (int)GuiData.scrollOffset.Y;
			return GuiData.tmpPoint;
		}

		// Token: 0x060006AB RID: 1707 RVA: 0x0006D500 File Offset: 0x0006B700
		public static float getMouseWheelScroll()
		{
			return (float)(GuiData.lastMouseScroll / 120);
		}

		// Token: 0x060006AC RID: 1708 RVA: 0x0006D51C File Offset: 0x0006B71C
		public static bool isMouseLeftDown()
		{
			return GuiData.mouse.LeftButton == ButtonState.Pressed;
		}

		// Token: 0x060006AD RID: 1709 RVA: 0x0006D53C File Offset: 0x0006B73C
		public static bool mouseLeftUp()
		{
			return GuiData.lastMouse.LeftButton == ButtonState.Pressed && GuiData.mouse.LeftButton == ButtonState.Released;
		}

		// Token: 0x060006AE RID: 1710 RVA: 0x0006D578 File Offset: 0x0006B778
		public static bool mouseWasPressed()
		{
			return GuiData.lastMouse.LeftButton == ButtonState.Released && GuiData.mouse.LeftButton == ButtonState.Pressed;
		}

		// Token: 0x060006AF RID: 1711 RVA: 0x0006D5B3 File Offset: 0x0006B7B3
		public static void startDraw()
		{
			GuiData.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
		}

		// Token: 0x060006B0 RID: 1712 RVA: 0x0006D5C7 File Offset: 0x0006B7C7
		public static void endDraw()
		{
			GuiData.spriteBatch.End();
		}

		// Token: 0x060006B1 RID: 1713 RVA: 0x0006D5D8 File Offset: 0x0006B7D8
		public static char[] getFilteredKeys()
		{
			string text = GuiData.TextInputHook.Buffer;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] >= ' ' && text[i] <= '~')
				{
					stringBuilder.Append(text[i]);
				}
			}
			GuiData.TextInputHook.clearBuffer();
			text = stringBuilder.ToString();
			char[] array = new char[text.Length];
			for (int i = 0; i < text.Length; i++)
			{
				array[i] = text[i];
			}
			return array;
		}

		// Token: 0x0400075D RID: 1885
		public static Vector2 temp = default(Vector2);

		// Token: 0x0400075E RID: 1886
		private static Point tmpPoint = default(Point);

		// Token: 0x0400075F RID: 1887
		public static Color tmpColor = default(Color);

		// Token: 0x04000760 RID: 1888
		public static Rectangle tmpRect = default(Rectangle);

		// Token: 0x04000761 RID: 1889
		public static MouseState lastMouse;

		// Token: 0x04000762 RID: 1890
		public static MouseState mouse;

		// Token: 0x04000763 RID: 1891
		public static InputState lastInput;

		// Token: 0x04000764 RID: 1892
		public static Color Default_Selected_Color = new Color(0, 166, 235);

		// Token: 0x04000765 RID: 1893
		public static Color Default_Unselected_Color = new Color(255, 128, 0);

		// Token: 0x04000766 RID: 1894
		public static Color Default_Backing_Color = new Color(30, 30, 50, 100);

		// Token: 0x04000767 RID: 1895
		public static Color Default_Light_Backing_Color = new Color(80, 80, 100, 255);

		// Token: 0x04000768 RID: 1896
		public static Color Default_Lit_Backing_Color = new Color(255, 199, 41, 100);

		// Token: 0x04000769 RID: 1897
		public static Color Default_Dark_Neutral_Color = new Color(10, 10, 15, 200);

		// Token: 0x0400076A RID: 1898
		public static Color Default_Dark_Background_Color = new Color(40, 40, 45, 180);

		// Token: 0x0400076B RID: 1899
		public static Color Default_Trans_Grey = new Color(30, 30, 30, 100);

		// Token: 0x0400076C RID: 1900
		public static Color Default_Trans_Grey_Bright = new Color(60, 60, 60, 100);

		// Token: 0x0400076D RID: 1901
		public static Color Default_Trans_Grey_Dark = new Color(20, 20, 20, 200);

		// Token: 0x0400076E RID: 1902
		public static Color Default_Trans_Grey_Strong = new Color(80, 80, 80, 100);

		// Token: 0x0400076F RID: 1903
		public static Color Default_Trans_Grey_Solid = new Color(100, 100, 100, 255);

		// Token: 0x04000770 RID: 1904
		public static int lastMouseWheelPos = -1;

		// Token: 0x04000771 RID: 1905
		private static int lastMouseScroll = 0;

		// Token: 0x04000772 RID: 1906
		public static int hot = -1;

		// Token: 0x04000773 RID: 1907
		public static int active = -1;

		// Token: 0x04000774 RID: 1908
		public static int enganged = -1;

		// Token: 0x04000775 RID: 1909
		public static SpriteBatch spriteBatch;

		// Token: 0x04000776 RID: 1910
		public static SpriteFont font;

		// Token: 0x04000777 RID: 1911
		public static SpriteFont titlefont;

		// Token: 0x04000778 RID: 1912
		public static SpriteFont smallfont;

		// Token: 0x04000779 RID: 1913
		public static SpriteFont tinyfont;

		// Token: 0x0400077A RID: 1914
		public static SpriteFont UITinyfont;

		// Token: 0x0400077B RID: 1915
		public static SpriteFont UISmallfont;

		// Token: 0x0400077C RID: 1916
		public static SpriteFont detailfont;

		// Token: 0x0400077D RID: 1917
		public static bool blockingInput = false;

		// Token: 0x0400077E RID: 1918
		public static bool blockingTextInput = false;

		// Token: 0x0400077F RID: 1919
		public static bool willBlockTextInput = false;

		// Token: 0x04000780 RID: 1920
		public static Vector2 scrollOffset = Vector2.Zero;

		// Token: 0x04000781 RID: 1921
		public static float lastTimeStep = 0.016f;

		// Token: 0x04000782 RID: 1922
		private static bool initialized = false;

		// Token: 0x04000783 RID: 1923
		private static TextInputHook TextInputHook;

		// Token: 0x04000784 RID: 1924
		public static List<GuiData.FontCongifOption> FontConfigs = new List<GuiData.FontCongifOption>();

		// Token: 0x04000785 RID: 1925
		public static Dictionary<string, List<GuiData.FontCongifOption>> LocaleFontConfigs = new Dictionary<string, List<GuiData.FontCongifOption>>();

		// Token: 0x04000786 RID: 1926
		public static GuiData.FontCongifOption ActiveFontConfig = default(GuiData.FontCongifOption);

		// Token: 0x0200011B RID: 283
		public struct FontCongifOption
		{
			// Token: 0x04000787 RID: 1927
			public SpriteFont smallFont;

			// Token: 0x04000788 RID: 1928
			public SpriteFont tinyFont;

			// Token: 0x04000789 RID: 1929
			public SpriteFont bigFont;

			// Token: 0x0400078A RID: 1930
			public SpriteFont detailFont;

			// Token: 0x0400078B RID: 1931
			public string name;

			// Token: 0x0400078C RID: 1932
			public float tinyFontCharHeight;
		}
	}
}
