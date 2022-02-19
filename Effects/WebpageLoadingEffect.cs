using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000B6 RID: 182
	public static class WebpageLoadingEffect
	{
		// Token: 0x060003B1 RID: 945 RVA: 0x00038CEC File Offset: 0x00036EEC
		public static void DrawLoadingEffect(Rectangle bounds, SpriteBatch sb, object OS_obj, bool drawLoadingText = true)
		{
			OS os = (OS)OS_obj;
			Rectangle rectangle = new Rectangle(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2, 2, 70);
			float num = (float)Math.Abs(Math.Sin((double)(os.timer * 0.2f)) * 100.0);
			Vector2 origin = new Vector2(2f, 10f);
			float num2 = 6.2831855f / num;
			float num3 = 0f;
			int num4 = 0;
			while (num3 < 6.2831855f)
			{
				Rectangle destinationRectangle = rectangle;
				destinationRectangle.Height = Math.Abs((int)((double)((float)rectangle.Height) * Math.Sin((double)(2f * os.timer + (float)num4 * 0.2f)))) + 10;
				origin.Y = 1.6f;
				sb.Draw(Utils.white, destinationRectangle, null, os.highlightColor, num3, origin, SpriteEffects.FlipVertically, 0.6f);
				num4++;
				num3 += num2;
			}
			if (drawLoadingText)
			{
				TextItem.doFontLabelToSize(new Rectangle(bounds.X, bounds.Y + 20, bounds.Width, 30), LocaleTerms.Loc("Loading..."), GuiData.font, Utils.AddativeWhite, false, false);
			}
		}
	}
}
