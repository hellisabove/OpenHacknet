using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000AC RID: 172
	public static class FlickeringTextEffect
	{
		// Token: 0x06000384 RID: 900 RVA: 0x00035CB8 File Offset: 0x00033EB8
		public static void DrawFlickeringText(Rectangle dest, string text, float maxOffset, float rarity, SpriteFont font, object os_Obj, Color BaseCol)
		{
			OS os_Obj2 = null;
			if (os_Obj != null)
			{
				os_Obj2 = (OS)os_Obj;
			}
			else
			{
				FlickeringTextEffect.internalTimer += 0.016666668f;
			}
			Color color = new Color((int)BaseCol.R, 0, 0, 0);
			Color color2 = new Color(0, (int)BaseCol.G, 0, 0);
			Color color3 = new Color(0, 0, (int)BaseCol.B, 0);
			TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f, rarity, os_Obj2))), text, font, color, false, false);
			TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f, rarity, os_Obj2))), text, font, color2, false, false);
			TextItem.doFontLabelToSize(FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.5f, -939.7f, rarity, os_Obj2))), text, font, color3, false, false);
		}

		// Token: 0x06000385 RID: 901 RVA: 0x00035D98 File Offset: 0x00033F98
		public static void DrawLinedFlickeringText(Rectangle dest, string text, float maxOffset, float rarity, SpriteFont font, object os_Obj, Color BaseCol, int segmentHeight = 2)
		{
			GraphicsDevice graphicsDevice = GuiData.spriteBatch.GraphicsDevice;
			if (FlickeringTextEffect.LinedItemTarget == null)
			{
				FlickeringTextEffect.LinedItemTarget = new RenderTarget2D(graphicsDevice, 1920, 1080, false, SurfaceFormat.Rgba64, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
				FlickeringTextEffect.LinedItemSB = new SpriteBatch(graphicsDevice);
			}
			if (dest.Width > FlickeringTextEffect.LinedItemTarget.Width || dest.Height > FlickeringTextEffect.LinedItemTarget.Height)
			{
				throw new InvalidOperationException("Target area is too large for the supported rendertarget size!");
			}
			OS os_Obj2 = null;
			if (os_Obj != null)
			{
				os_Obj2 = (OS)os_Obj;
			}
			Rectangle dest2 = new Rectangle(0, 0, dest.Width, dest.Height);
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			graphicsDevice.SetRenderTarget(FlickeringTextEffect.LinedItemTarget);
			graphicsDevice.Clear(Color.Transparent);
			SpriteBatch spriteBatch = GuiData.spriteBatch;
			FlickeringTextEffect.LinedItemSB.Begin();
			GuiData.spriteBatch = FlickeringTextEffect.LinedItemSB;
			FlickeringTextEffect.DrawFlickeringText(dest2, text, maxOffset, rarity, font, os_Obj, BaseCol);
			FlickeringTextEffect.LinedItemSB.End();
			GuiData.spriteBatch = spriteBatch;
			graphicsDevice.SetRenderTarget(currentRenderTarget);
			int num = (int)((float)dest.Height / 19f);
			int num2 = (int)((float)dest.Height / (float)(num + 1));
			float num3 = (float)dest.Height / (float)(num + 1);
			for (int i = 0; i < num + 1; i++)
			{
				Rectangle value = new Rectangle(0, (int)((float)i * (float)num2 - (float)segmentHeight), dest.Width, num2 - segmentHeight);
				Rectangle destinationRectangle = new Rectangle(dest.X + dest2.X, dest.Y + value.Y, value.Width, value.Height);
				GuiData.spriteBatch.Draw(FlickeringTextEffect.LinedItemTarget, destinationRectangle, new Rectangle?(value), Color.White);
				Rectangle rectangle = new Rectangle(0, value.Y + value.Height, value.Width, segmentHeight);
				Rectangle destinationRectangle2 = rectangle;
				destinationRectangle2.X += dest.X;
				destinationRectangle2.Y += dest.Y;
				int num4 = (int)(maxOffset * 1.2f * FlickeringTextEffect.GetOffsetForSinTime((float)(1.2999999523162842 + Math.Sin((double)i * 2.5)), 12.3f * (float)i, rarity, os_Obj2));
				destinationRectangle2.X += num4;
				GuiData.spriteBatch.Draw(FlickeringTextEffect.LinedItemTarget, destinationRectangle2, new Rectangle?(rectangle), Color.White);
			}
		}

		// Token: 0x06000386 RID: 902 RVA: 0x00036034 File Offset: 0x00034234
		public static void DrawFlickeringSprite(SpriteBatch sb, Rectangle dest, Texture2D texture, float maxOffset, float rarity, object os_Obj, Color BaseCol)
		{
			OS os_Obj2 = null;
			if (os_Obj != null)
			{
				os_Obj2 = (OS)os_Obj;
			}
			else
			{
				FlickeringTextEffect.internalTimer += 0.016666668f;
			}
			Color color = new Color((int)BaseCol.R, 0, 0, 0);
			Color color2 = new Color(0, (int)BaseCol.G, 0, 0);
			Color color3 = new Color(0, 0, (int)BaseCol.B, 0);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f, rarity, os_Obj2))), color);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f, rarity, os_Obj2))), color2);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.5f, -939.7f, rarity, os_Obj2))), color3);
		}

		// Token: 0x06000387 RID: 903 RVA: 0x0003610C File Offset: 0x0003430C
		public static void DrawFlickeringSpriteAltWeightings(SpriteBatch sb, Rectangle dest, Texture2D texture, float maxOffset, float rarity, object os_Obj, Color BaseCol)
		{
			OS os_Obj2 = null;
			if (os_Obj != null)
			{
				os_Obj2 = (OS)os_Obj;
			}
			else
			{
				FlickeringTextEffect.internalTimer += 0.016666668f;
			}
			Color color = new Color((int)BaseCol.R, 0, 0, 0);
			Color color2 = new Color(0, (int)BaseCol.G, 0, 0);
			Color color3 = new Color(0, 0, (int)BaseCol.B, 0);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(4f, 0f, rarity, os_Obj2))), color);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(2f, 2f, rarity, os_Obj2))), color2);
			sb.Draw(texture, FlickeringTextEffect.RectAddX(dest, (int)(maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.5f, -4f, rarity, os_Obj2))), color3);
		}

		// Token: 0x06000388 RID: 904 RVA: 0x000361E4 File Offset: 0x000343E4
		public static void DrawFlickeringSpriteFull(SpriteBatch sb, Vector2 pos, float rotation, Vector2 scale, Vector2 origin, Texture2D texture, float timerOffset, float maxOffset, float rarity, object os_Obj, Color BaseCol)
		{
			OS os_Obj2 = null;
			if (os_Obj != null)
			{
				os_Obj2 = (OS)os_Obj;
			}
			else
			{
				FlickeringTextEffect.internalTimer += 0.016666668f;
			}
			Color color = new Color((int)BaseCol.R, 0, 0, 0);
			Color color2 = new Color(0, (int)BaseCol.G, 0, 0);
			Color color3 = new Color(0, 0, (int)BaseCol.B, 0);
			sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(1.3f, 12.3f + timerOffset, rarity, os_Obj2)), null, color, rotation, origin, scale, SpriteEffects.None, 0.9f);
			sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.8f, 29f + timerOffset, rarity, os_Obj2)), null, color2, rotation, origin, scale, SpriteEffects.None, 0.9f);
			sb.Draw(texture, FlickeringTextEffect.VecAddX(pos, maxOffset * FlickeringTextEffect.GetOffsetForSinTime(0.5f, -939.7f + timerOffset, rarity, os_Obj2)), null, color3, rotation, origin, scale, SpriteEffects.None, 0.9f);
		}

		// Token: 0x06000389 RID: 905 RVA: 0x00036304 File Offset: 0x00034504
		public static float GetOffsetForSinTime(float frequency, float offset, float rarity, object os_Obj)
		{
			float num = (os_Obj != null) ? ((OS)os_Obj).timer : FlickeringTextEffect.internalTimer;
			float num2 = (float)Math.Sin((double)((num + offset) * frequency)) - rarity;
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			float num3 = 0.5f + Utils.QuadraticOutCurve(Utils.randm(num2));
			num3 = 2f * (num3 - 0.5f);
			return num3 - num2;
		}

		// Token: 0x0600038A RID: 906 RVA: 0x00036378 File Offset: 0x00034578
		private static Vector2 VecAddX(Vector2 vec, float x)
		{
			return vec + new Vector2(x, 0f);
		}

		// Token: 0x0600038B RID: 907 RVA: 0x0003639C File Offset: 0x0003459C
		private static Rectangle RectAddX(Rectangle rect, int x)
		{
			rect.X += x;
			return rect;
		}

		// Token: 0x0600038C RID: 908 RVA: 0x000363C0 File Offset: 0x000345C0
		public static string GetReportString()
		{
			return string.Concat(new object[]
			{
				"Target : ",
				FlickeringTextEffect.LinedItemTarget,
				"\r\nTargetDisposed : ",
				FlickeringTextEffect.LinedItemTarget.IsDisposed
			});
		}

		// Token: 0x04000402 RID: 1026
		private static float internalTimer = 0f;

		// Token: 0x04000403 RID: 1027
		private static RenderTarget2D LinedItemTarget = null;

		// Token: 0x04000404 RID: 1028
		private static SpriteBatch LinedItemSB;
	}
}
