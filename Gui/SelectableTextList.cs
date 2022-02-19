using System;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Gui
{
	// Token: 0x0200011E RID: 286
	public static class SelectableTextList
	{
		// Token: 0x060006BA RID: 1722 RVA: 0x0006DF84 File Offset: 0x0006C184
		public static int doList(int myID, int x, int y, int width, int height, string[] text, int lastSelectedIndex, Color? selectedColor)
		{
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Selected_Color);
			}
			int num = -1;
			SelectableTextList.wasActivated = false;
			int num2 = lastSelectedIndex;
			SelectableTextList.selectionWasChanged = false;
			Vector2 mousePos = GuiData.getMousePos();
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
				SelectableTextList.scrollOffset += (int)GuiData.getMouseWheelScroll();
				SelectableTextList.scrollOffset = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int)((float)height / 18f)));
			}
			else if (GuiData.hot == myID)
			{
				GuiData.hot = -1;
			}
			int num3 = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int)((float)height / 18f)));
			if (GuiData.hot == myID)
			{
				for (int i = 0; i < text.Length; i++)
				{
					if (mousePos.Y >= (float)y + (float)i * 18f && mousePos.Y <= (float)y + (float)(i + 1) * 18f && mousePos.Y < (float)(y + height))
					{
						num = i + num3;
						SelectableTextList.wasActivated = true;
					}
				}
			}
			if (num != -1 && num != lastSelectedIndex && GuiData.mouseLeftUp())
			{
				lastSelectedIndex = num;
			}
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, (GuiData.hot == myID) ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Backing_Color);
			tmpRect.X += 2;
			tmpRect.Width -= 4;
			tmpRect.Y += 2;
			tmpRect.Height -= 4;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.Default_Dark_Background_Color);
			Vector2 position = new Vector2((float)tmpRect.X, (float)tmpRect.Y);
			tmpRect.Height = 18;
			for (int i = num3; i < text.Length; i++)
			{
				GuiData.spriteBatch.Draw(Utils.white, tmpRect, (lastSelectedIndex == i) ? selectedColor.Value : ((num == i) ? GuiData.Default_Unselected_Color : GuiData.Default_Dark_Neutral_Color));
				Vector2 scale = GuiData.UITinyfont.MeasureString(text[i]);
				if (scale.X > (float)(width - 4))
				{
					scale.X = (float)(width - 4) / scale.X;
				}
				else
				{
					scale.X = 1f;
				}
				if (scale.Y > 18f)
				{
					scale.Y = 14f / scale.Y;
				}
				else
				{
					scale.Y = 1f;
				}
				GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text[i], position, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
				position.Y += 18f;
				tmpRect.Y += 18;
				if (position.Y > (float)(y + height - 4))
				{
					break;
				}
			}
			if ((float)text.Length * 18f > (float)height)
			{
				float num4 = 2f;
				float num5 = (float)height / ((float)text.Length * 18f);
				num5 *= (float)height;
				height -= 4;
				float num6 = (float)(-(float)height) + ((float)height - num5) * ((float)num3 / (((float)text.Length * 18f - (float)height) / 18f));
				tmpRect.X = (int)(position.X + (float)width - 3f * num4 - 2f);
				tmpRect.Y = (int)(position.Y + num6 + 2f);
				tmpRect.Height = (int)num5;
				tmpRect.Width = (int)num4;
				GuiData.spriteBatch.Draw(Utils.white, tmpRect, SelectableTextList.scrollBarColor);
			}
			if (lastSelectedIndex != num2)
			{
				SelectableTextList.selectionWasChanged = true;
			}
			return lastSelectedIndex;
		}

		// Token: 0x060006BB RID: 1723 RVA: 0x0006E3DC File Offset: 0x0006C5DC
		public static int doFancyList(int myID, int x, int y, int width, int height, string[] text, int lastSelectedIndex, Color? selectedColor, bool HasDraggableScrollbar = false)
		{
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Selected_Color);
			}
			int num = -1;
			SelectableTextList.wasActivated = false;
			int num2 = lastSelectedIndex;
			SelectableTextList.selectionWasChanged = false;
			Vector2 mousePos = GuiData.getMousePos();
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
				SelectableTextList.scrollOffset += (int)GuiData.getMouseWheelScroll();
				SelectableTextList.scrollOffset = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int)((float)height / 18f)));
			}
			else if (GuiData.hot == myID)
			{
				GuiData.hot = -1;
			}
			int num3 = Math.Max(0, Math.Min(SelectableTextList.scrollOffset, text.Length - (int)((float)height / 18f)));
			float num4 = HasDraggableScrollbar ? 4f : 2f;
			if (GuiData.hot == myID)
			{
				if (!HasDraggableScrollbar || mousePos.X < (float)(x + width) - 2f * num4)
				{
					for (int i = 0; i < text.Length; i++)
					{
						if (mousePos.Y >= (float)y + (float)i * 18f && mousePos.Y <= (float)y + (float)(i + 1) * 18f && mousePos.Y < (float)(y + height))
						{
							num = i + num3;
						}
					}
				}
			}
			if (num != -1 && num != lastSelectedIndex && GuiData.mouseLeftUp())
			{
				lastSelectedIndex = num;
				SelectableTextList.wasActivated = true;
			}
			tmpRect.X += 2;
			tmpRect.Width -= 4;
			tmpRect.Y += 2;
			tmpRect.Height -= 4;
			Vector2 input = new Vector2((float)tmpRect.X, (float)tmpRect.Y);
			tmpRect.Height = 18;
			for (int i = num3; i < text.Length; i++)
			{
				GuiData.spriteBatch.Draw(Utils.white, tmpRect, (lastSelectedIndex == i) ? selectedColor.Value : ((num == i) ? (selectedColor.Value * 0.45f) : GuiData.Default_Dark_Neutral_Color));
				Vector2 scale = GuiData.UITinyfont.MeasureString(text[i]);
				if (scale.X > (float)(width - 4))
				{
					scale.X = (float)(width - 4) / scale.X;
				}
				else
				{
					scale.X = 1f;
				}
				if (scale.Y > 18f)
				{
					scale.Y = 18f / scale.Y;
				}
				else
				{
					scale.Y = 1f;
				}
				scale.X = Math.Min(scale.X, scale.Y);
				scale.Y = Math.Min(scale.X, scale.Y);
				bool flag = !LocaleActivator.ActiveLocaleIsCJK() && Settings.ActiveLocale != "en-us";
				if (flag)
				{
					input.Y += 3f;
				}
				GuiData.spriteBatch.DrawString(GuiData.UITinyfont, text[i], Utils.ClipVec2ForTextRendering(input), (lastSelectedIndex == i) ? Color.Black : Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
				if (flag)
				{
					input.Y -= 3f;
				}
				input.Y += 18f;
				tmpRect.Y += 18;
				if (input.Y > (float)(y + height - 4))
				{
					break;
				}
			}
			if ((float)text.Length * 18f > (float)height)
			{
				float num5 = num4;
				float num6 = (float)height / ((float)text.Length * 18f);
				num6 *= (float)height;
				height -= 4;
				float num7 = (float)(-(float)height) + ((float)height - num6) * ((float)num3 / (((float)text.Length * 18f - (float)height) / 18f));
				tmpRect.X = (int)(input.X + (float)width - (HasDraggableScrollbar ? 2f : 3f) * num5 - 2f);
				tmpRect.Y = (int)(input.Y + num7 + 2f);
				tmpRect.Height = (int)num6;
				tmpRect.Width = (int)num5;
				if (!HasDraggableScrollbar)
				{
					GuiData.spriteBatch.Draw(Utils.white, tmpRect, SelectableTextList.scrollBarColor);
				}
				else
				{
					float num8 = ScrollBar.doVerticalScrollBar(myID + 101, tmpRect.X, y, tmpRect.Width, height, (int)((float)text.Length * 18f), (float)num3 * 18f);
					int num9 = (int)(num8 / 18f);
					SelectableTextList.scrollOffset = num9;
				}
			}
			if (lastSelectedIndex != num2)
			{
				SelectableTextList.selectionWasChanged = true;
			}
			return lastSelectedIndex;
		}

		// Token: 0x04000793 RID: 1939
		public const int BORDER_WIDTH = 2;

		// Token: 0x04000794 RID: 1940
		public const float ITEM_HEIGHT = 18f;

		// Token: 0x04000795 RID: 1941
		public static int scrollOffset = 0;

		// Token: 0x04000796 RID: 1942
		public static bool wasActivated = false;

		// Token: 0x04000797 RID: 1943
		public static bool selectionWasChanged = false;

		// Token: 0x04000798 RID: 1944
		private static Color scrollBarColor = new Color(140, 140, 140, 80);
	}
}
