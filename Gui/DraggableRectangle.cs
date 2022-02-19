using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Gui
{
	// Token: 0x02000119 RID: 281
	public static class DraggableRectangle
	{
		// Token: 0x0600069C RID: 1692 RVA: 0x0006CCCC File Offset: 0x0006AECC
		public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height)
		{
			return DraggableRectangle.doDraggableRectangle(myID, x, y, width, height, -1f, null, null);
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0006CD00 File Offset: 0x0006AF00
		public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height, float selectableBorder, Color? selectedColor, Color? deselectedColor)
		{
			return DraggableRectangle.doDraggableRectangle(myID, x, y, width, height, selectableBorder, selectedColor, deselectedColor, true, true, float.MaxValue, float.MaxValue, float.MinValue, float.MinValue);
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0006CD3C File Offset: 0x0006AF3C
		public static Vector2 doDraggableRectangle(int myID, float x, float y, int width, int height, float selectableBorder, Color? selectedColor, Color? deselectedColor, bool canMoveY, bool canMoveX, float xMax, float yMax, float xMin, float yMin)
		{
			DraggableRectangle.isDragging = false;
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Selected_Color);
			}
			if (deselectedColor == null)
			{
				deselectedColor = new Color?(GuiData.Default_Unselected_Color);
			}
			Vector2 temp = GuiData.temp;
			temp.X = 0f;
			temp.Y = 0f;
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = (int)x;
			tmpRect.Y = (int)y;
			tmpRect.Width = width;
			tmpRect.Height = height;
			Rectangle tmpRect2 = GuiData.tmpRect;
			tmpRect2.X = (int)(x + selectableBorder);
			tmpRect2.Y = (int)(y + selectableBorder);
			tmpRect2.Width = ((selectableBorder == -1f) ? 0 : ((int)((float)width - 2f * selectableBorder)));
			tmpRect2.Height = ((selectableBorder == -1f) ? 0 : ((int)((float)height - 2f * selectableBorder)));
			if (tmpRect.Contains(GuiData.getMousePoint()) && !tmpRect2.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
				if (GuiData.active != myID)
				{
					if (GuiData.mouseWasPressed())
					{
						GuiData.active = myID;
						DraggableRectangle.originalClickPos = GuiData.getMousePos();
						DraggableRectangle.originalClickPos.X = DraggableRectangle.originalClickPos.X - x;
						DraggableRectangle.originalClickPos.Y = DraggableRectangle.originalClickPos.Y - y;
						DraggableRectangle.originalClickOffset = new Vector2(DraggableRectangle.originalClickPos.X - x, DraggableRectangle.originalClickPos.Y - y);
					}
				}
			}
			else if (GuiData.hot == myID)
			{
				GuiData.hot = -1;
			}
			if (GuiData.active == myID)
			{
				if (GuiData.mouseLeftUp())
				{
					GuiData.active = -1;
				}
				else
				{
					if (canMoveX)
					{
						temp.X = (float)GuiData.mouse.X - x - DraggableRectangle.originalClickPos.X;
						temp.X = Math.Min(Math.Max((float)tmpRect.X + temp.X, xMin), xMax) - (float)tmpRect.X;
					}
					if (canMoveY)
					{
						temp.Y = (float)GuiData.mouse.Y - y - DraggableRectangle.originalClickPos.Y;
						temp.Y = Math.Min(Math.Max((float)tmpRect.Y + temp.Y, yMin), yMax) - (float)tmpRect.Y;
					}
					tmpRect.X += (int)temp.X;
					tmpRect.Y += (int)temp.Y;
					DraggableRectangle.isDragging = true;
				}
			}
			if (GuiData.active == myID || GuiData.hot == myID)
			{
				GuiData.blockingInput = true;
			}
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, (GuiData.hot == myID || GuiData.active == myID) ? selectedColor.Value : deselectedColor.Value);
			return temp;
		}

		// Token: 0x0400075A RID: 1882
		public static bool isDragging = false;

		// Token: 0x0400075B RID: 1883
		public static Vector2 originalClickPos;

		// Token: 0x0400075C RID: 1884
		public static Vector2 originalClickOffset;
	}
}
