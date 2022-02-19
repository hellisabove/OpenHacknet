using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Gui
{
	// Token: 0x0200011C RID: 284
	public static class RenderedRectangle
	{
		// Token: 0x060006B3 RID: 1715 RVA: 0x0006D820 File Offset: 0x0006BA20
		public static void doRectangle(int x, int y, int width, int height, Color? color)
		{
			if (color == null)
			{
				color = new Color?(GuiData.Default_Backing_Color);
			}
			if (width < 0)
			{
				x += width;
				width = Math.Abs(width);
			}
			if (height < 0)
			{
				y += height;
				height = Math.Abs(height);
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Width = width;
			tmpRect.Y = y;
			tmpRect.Height = height;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0006D8B8 File Offset: 0x0006BAB8
		public static void doRectangle(int x, int y, int width, int height, Color? color, bool blocking)
		{
			RenderedRectangle.doRectangle(x, y, width, height, color);
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			if (blocking && tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.blockingInput = true;
			}
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0006D91C File Offset: 0x0006BB1C
		public static void doRectangleOutline(int x, int y, int width, int height, int thickness, Color? color)
		{
			if (color == null)
			{
				color = new Color?(GuiData.Default_Backing_Color);
			}
			if (width < 0)
			{
				x += width;
				width = Math.Abs(width);
			}
			if (height < 0)
			{
				y += height;
				height = Math.Abs(height);
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x + thickness;
			tmpRect.Width = width - 2 * thickness;
			tmpRect.Y = y;
			tmpRect.Height = thickness;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
			tmpRect.X = x + thickness;
			tmpRect.Width = width - 2 * thickness;
			tmpRect.Y = y + height - thickness;
			tmpRect.Height = thickness;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
			tmpRect.X = x;
			tmpRect.Width = thickness;
			tmpRect.Y = y;
			tmpRect.Height = height;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
			tmpRect.X = x + width - thickness;
			tmpRect.Width = thickness;
			tmpRect.Y = y;
			tmpRect.Height = height;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, color.Value);
		}
	}
}
