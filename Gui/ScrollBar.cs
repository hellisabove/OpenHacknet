using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Gui
{
	// Token: 0x020000CC RID: 204
	public static class ScrollBar
	{
		// Token: 0x06000420 RID: 1056 RVA: 0x00041EF4 File Offset: 0x000400F4
		public static float doVerticalScrollBar(int id, int xPos, int yPos, int drawWidth, int drawHeight, int contentHeight, float scroll)
		{
			if (drawHeight > contentHeight)
			{
				contentHeight = drawHeight;
			}
			Rectangle destinationRectangle = new Rectangle(xPos, yPos, drawWidth, drawHeight);
			if (ScrollBar.AlwaysDrawUnderBar || destinationRectangle.Contains(GuiData.getMousePoint()) || GuiData.active == id)
			{
				GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Gray * 0.1f);
			}
			float result = scroll;
			float num = (float)(contentHeight - drawHeight);
			float num2 = (float)drawHeight / (float)contentHeight * (float)drawHeight;
			float num3 = scroll / num;
			float num4 = num3 * ((float)drawHeight - num2);
			float y = DraggableRectangle.doDraggableRectangle(id, (float)xPos, (float)yPos + num4, drawWidth, (int)num2, (float)drawWidth, new Color?(Color.White), new Color?(Color.Gray), true, false, (float)xPos, (float)(yPos + drawHeight) - num2, (float)xPos, (float)yPos).Y;
			if (Math.Abs(y) > 0.1f)
			{
				result = (num4 + y) / ((float)drawHeight - num2) * num;
			}
			return result;
		}

		// Token: 0x0400050A RID: 1290
		public static bool AlwaysDrawUnderBar = false;
	}
}
