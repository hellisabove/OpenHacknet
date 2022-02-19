using System;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet.Gui
{
	// Token: 0x02000117 RID: 279
	public static class Button
	{
		// Token: 0x06000694 RID: 1684 RVA: 0x0006C2C0 File Offset: 0x0006A4C0
		public static bool doButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor)
		{
			return Button.doButton(myID, x, y, width, height, text, selectedColor, Utils.white);
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0006C2E8 File Offset: 0x0006A4E8
		public static bool doButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
		{
			bool result = false;
			if (GuiData.hot == myID && !GuiData.blockingInput)
			{
				if (GuiData.active == myID)
				{
					if (GuiData.mouseLeftUp() || GuiData.mouse.LeftButton == ButtonState.Released)
					{
						result = true;
						GuiData.active = -1;
					}
				}
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			if (tmpRect.Contains(GuiData.getMousePoint()) && !GuiData.blockingInput)
			{
				GuiData.hot = myID;
				if (GuiData.isMouseLeftDown() && (!Button.DisableIfAnotherIsActive || GuiData.active == -1))
				{
					GuiData.active = myID;
				}
			}
			else
			{
				if (GuiData.hot == myID)
				{
					GuiData.hot = -1;
				}
				if (GuiData.isMouseLeftDown() && GuiData.active == myID)
				{
					if (GuiData.active == myID)
					{
						GuiData.active = -1;
					}
				}
			}
			Button.drawModernButton(myID, x, y, width, height, text, selectedColor, tex);
			return result;
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0006C424 File Offset: 0x0006A624
		private static void drawButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
		{
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			tmpRect.X--;
			tmpRect.Width += 2;
			tmpRect.Y--;
			tmpRect.Height += 2;
			if (Button.outlineOnly)
			{
				RenderedRectangle.doRectangleOutline(tmpRect.X, tmpRect.Y, tmpRect.Width, tmpRect.Height, 1, new Color?((GuiData.hot == myID) ? ((GuiData.active == myID) ? GuiData.Default_Selected_Color : GuiData.Default_Lit_Backing_Color) : GuiData.Default_Backing_Color));
			}
			else if (Button.drawingOutline)
			{
				GuiData.spriteBatch.Draw(tex, tmpRect, (GuiData.hot == myID) ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Backing_Color);
			}
			tmpRect.X++;
			tmpRect.Width -= 2;
			tmpRect.Y++;
			tmpRect.Height -= 2;
			if (!Button.outlineOnly)
			{
				GuiData.spriteBatch.Draw(tex, tmpRect, (GuiData.active == myID) ? GuiData.Default_Unselected_Color : selectedColor.Value);
			}
			Vector2 scale = GuiData.tinyfont.MeasureString(text);
			if (scale.X > (float)(width - 4))
			{
				scale.X = (float)(width - 4) / scale.X;
			}
			else
			{
				scale.X = 1f;
			}
			if (scale.Y > (float)(height - 4))
			{
				scale.Y = (float)(height - 2) / scale.Y;
			}
			else
			{
				scale.Y = 1f;
			}
			scale.X = Math.Min(scale.X, scale.Y);
			scale.Y = scale.X;
			GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, new Vector2((float)(x + 2 + 1), (float)(y + 1 + 1)), Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
			GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, new Vector2((float)(x + 2), (float)(y + 1)), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0006C6A0 File Offset: 0x0006A8A0
		private static void drawModernButton(int myID, int x, int y, int width, int height, string text, Color? selectedColor, Texture2D tex)
		{
			int num = (!Button.ForceNoColorTag && width > 65) ? 13 : 0;
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Trans_Grey_Strong);
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			if (tex.Equals(Utils.white))
			{
				if (!Button.outlineOnly)
				{
					GuiData.spriteBatch.Draw(Utils.white, tmpRect, (GuiData.hot == myID) ? ((GuiData.active == myID) ? GuiData.Default_Trans_Grey_Dark : GuiData.Default_Trans_Grey_Bright) : GuiData.Default_Trans_Grey);
					tmpRect.Width = num;
					GuiData.spriteBatch.Draw(Utils.white, tmpRect, selectedColor.Value);
				}
				RenderedRectangle.doRectangleOutline(x, y, width, height, 1, Button.outlineOnly ? selectedColor : new Color?(GuiData.Default_Trans_Grey_Solid));
			}
			else
			{
				GuiData.spriteBatch.Draw(tex, tmpRect, (GuiData.hot == myID) ? ((GuiData.active == myID) ? GuiData.Default_Unselected_Color : GuiData.Default_Lit_Backing_Color) : selectedColor.Value);
			}
			SpriteFont spriteFont = Button.smallButtonDraw ? GuiData.detailfont : GuiData.tinyfont;
			Vector2 scale = spriteFont.MeasureString(text);
			float num2 = LocaleActivator.ActiveLocaleIsCJK() ? 4f : 0f;
			float y2 = scale.Y;
			if (scale.X > (float)(width - 4))
			{
				scale.X = (float)(width - (4 + num + 5)) / scale.X;
			}
			else
			{
				scale.X = 1f;
			}
			if (scale.Y > (float)height + num2 - 0f)
			{
				scale.Y = ((float)height + num2 - 0f) / scale.Y;
			}
			else
			{
				scale.Y = 1f;
			}
			scale.X = Math.Min(scale.X, scale.Y);
			scale.Y = scale.X;
			if (Utils.FloatEquals(1f, scale.Y))
			{
				scale = Vector2.One;
			}
			num += 4;
			float num3 = y2 * scale.Y;
			float num4 = (float)y + (float)height / 2f - num3 / 2f + 1f - num2 * scale.Y / 2f;
			GuiData.spriteBatch.DrawString(spriteFont, text, new Vector2((float)((int)((double)(x + 2 + num) + 0.5)), (float)((int)((double)num4 + 0.5))), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0006C96C File Offset: 0x0006AB6C
		public static bool doHoldDownButton(int myID, int x, int y, int width, int height, string text, bool hasOutline, Color? outlineColor, Color? selectedColor)
		{
			Button.wasPressedDown = false;
			Button.wasReleased = false;
			if (outlineColor == null)
			{
				outlineColor = new Color?(Color.White);
			}
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Selected_Color);
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			bool result = GuiData.isMouseLeftDown() && GuiData.active == myID;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
				if (GuiData.mouseWasPressed())
				{
					Button.wasPressedDown = true;
				}
				if (GuiData.isMouseLeftDown())
				{
					GuiData.active = myID;
					result = true;
				}
			}
			else
			{
				if (GuiData.hot == myID)
				{
					GuiData.hot = -1;
				}
				if (!GuiData.isMouseLeftDown() && GuiData.active == myID)
				{
					if (GuiData.active == myID)
					{
						GuiData.active = -1;
					}
				}
			}
			if (GuiData.mouseLeftUp())
			{
				Button.wasReleased = true;
			}
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, (GuiData.active == myID) ? selectedColor.Value : ((GuiData.hot == myID) ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Light_Backing_Color));
			if (hasOutline)
			{
				RenderedRectangle.doRectangleOutline(x, y, width, height, 2, outlineColor);
			}
			return result;
		}

		// Token: 0x0400074F RID: 1871
		public const int BORDER_WIDTH = 1;

		// Token: 0x04000750 RID: 1872
		public static bool wasPressedDown = false;

		// Token: 0x04000751 RID: 1873
		public static bool wasReleased = false;

		// Token: 0x04000752 RID: 1874
		public static bool drawingOutline = true;

		// Token: 0x04000753 RID: 1875
		public static bool outlineOnly = false;

		// Token: 0x04000754 RID: 1876
		public static bool smallButtonDraw = false;

		// Token: 0x04000755 RID: 1877
		public static bool DisableIfAnotherIsActive = false;

		// Token: 0x04000756 RID: 1878
		public static bool ForceNoColorTag = false;
	}
}
