using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x0200019B RID: 411
	public static class GridEffect
	{
		// Token: 0x06000A4D RID: 2637 RVA: 0x000A456C File Offset: 0x000A276C
		public static void DrawGridBackground(Rectangle dest, SpriteBatch sb, int desiredNumOfBlocks, Color CrossColor)
		{
			int num = dest.Width / (desiredNumOfBlocks + 2);
			int num2 = 9;
			float num3 = 1f;
			int num4 = dest.X + num / 2;
			int num5 = dest.Y + num / 2;
			do
			{
				int num6 = 0;
				do
				{
					Rectangle destinationRectangle = new Rectangle(Math.Max(dest.X, num4 - num2 / 2), num5 - (int)(num3 / 2f + 0.5f), num2, (int)num3);
					sb.Draw(Utils.white, destinationRectangle, CrossColor);
					Rectangle destinationRectangle2 = new Rectangle(num4 - (int)(num3 / 2f), Math.Max(dest.Y, num5 - (int)((float)num2 / 2f + 0.5f)), (int)num3, num2 / 2 - (int)(num3 / 2f));
					sb.Draw(Utils.white, destinationRectangle2, CrossColor);
					Rectangle destinationRectangle3 = new Rectangle(num4 - (int)(num3 / 2f), num5 + (int)(num3 / 2f), (int)num3, num2 / 2 - (int)(num3 / 2f));
					sb.Draw(Utils.white, destinationRectangle3, CrossColor);
					num4 += num;
					num6++;
				}
				while (num4 <= dest.X + dest.Width - num / 2);
				num4 = dest.X + num / 2;
				num5 += num;
			}
			while (num5 <= dest.Y + dest.Height - num / 2);
		}
	}
}
