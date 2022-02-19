using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Gui
{
	// Token: 0x02000118 RID: 280
	public static class CheckBox
	{
		// Token: 0x0600069A RID: 1690 RVA: 0x0006CB18 File Offset: 0x0006AD18
		public static bool doCheckBox(int myID, int x, int y, bool isChecked, Color? selectedColor)
		{
			if (selectedColor == null)
			{
				selectedColor = new Color?(GuiData.Default_Selected_Color);
			}
			if (GuiData.hot == myID)
			{
				if (GuiData.active == myID)
				{
					if (GuiData.mouseLeftUp())
					{
						isChecked = !isChecked;
						GuiData.active = -1;
					}
				}
			}
			RenderedRectangle.doRectangleOutline(x, y, 20, 20, 2, new Color?((GuiData.hot == myID) ? GuiData.Default_Lit_Backing_Color : GuiData.Default_Light_Backing_Color));
			RenderedRectangle.doRectangle(x + 4, y + 4, 12, 12, isChecked ? selectedColor : new Color?((GuiData.active == myID) ? GuiData.Default_Unselected_Color : GuiData.Default_Backing_Color));
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = 20;
			tmpRect.Height = 20;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
				if (GuiData.isMouseLeftDown())
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
			return isChecked;
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0006CC7C File Offset: 0x0006AE7C
		public static bool doCheckBox(int myID, int x, int y, bool isChecked, Color? selectedColor, string text)
		{
			if (GuiData.hot == myID)
			{
				TextItem.doSmallLabel(new Vector2((float)(x + 20), (float)(y - 20)), text, null);
			}
			return CheckBox.doCheckBox(myID, x, y, isChecked, selectedColor);
		}

		// Token: 0x04000757 RID: 1879
		public const int WIDTH = 20;

		// Token: 0x04000758 RID: 1880
		public const int HEIGHT = 20;

		// Token: 0x04000759 RID: 1881
		public const int INTERIOR_BORDER = 4;
	}
}
