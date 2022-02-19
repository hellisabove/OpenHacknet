using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x0200005C RID: 92
	public static class ZoomingDotGridEffect
	{
		// Token: 0x060001B5 RID: 437 RVA: 0x00017AC8 File Offset: 0x00015CC8
		public static void Render(Rectangle dest, SpriteBatch sb, float timer, Color themeColor)
		{
			if (ZoomingDotGridEffect.CircleTex == null)
			{
				ZoomingDotGridEffect.CircleTex = OS.currentInstance.content.Load<Texture2D>("Circle");
			}
			float num = 12f;
			float num2 = 4f;
			float num3 = 4f;
			float num4 = 3.5f * num3;
			float num5 = (float)Math.Sqrt(Math.Pow((double)num4, 2.0) * 2.0);
			float num6 = timer % num;
			float num7 = num4 + (num5 - num4) * (num6 / num);
			float num8 = 0.7853982f;
			float rotation = num6 / num * num8;
			Vector2 origin = new Vector2((float)dest.X, (float)dest.Y);
			float scale = (num6 - (num - num2)) / num2;
			if (num - num6 > num2)
			{
				scale = 0f;
			}
			Vector2 value = num6 / num * new Vector2(-1f * num7 / 2.5f, -1f * num7 / 3f);
			value = Vector2.Zero;
			Vector2 point = new Vector2((float)dest.X - num7 * 0f, (float)dest.Y - num7 * 10f);
			float x = point.X;
			while (point.Y < (float)(dest.Y + dest.Height) + num5 + num3)
			{
				point.X = x;
				while (point.X < (float)(dest.X + dest.Width * 2) + num5 + num3)
				{
					Vector2 vector = ZoomingDotGridEffect.rotatePointAroundOrigin(point, origin, rotation) + value;
					Rectangle targetBounds = new Rectangle((int)(vector.X - num3 / 2f), (int)(vector.Y - num3 / 2f), (int)num3, (int)num3);
					Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds, ZoomingDotGridEffect.CircleTex, themeColor, sb);
					point.X += num7;
				}
				point.Y += num7;
			}
			if (num - num6 <= num2)
			{
				point = new Vector2((float)dest.X - num7 / 2f, (float)dest.Y - num7 * 10.5f);
				x = point.X;
				while (point.Y < (float)(dest.Y + dest.Height) + num5 + num3)
				{
					point.X = x;
					while (point.X < (float)(dest.X + dest.Width * 2) + num5 + num3)
					{
						Vector2 vector = ZoomingDotGridEffect.rotatePointAroundOrigin(point, origin, rotation) + value;
						Rectangle targetBounds = new Rectangle((int)(vector.X - num3 / 2f), (int)(vector.Y - num3 / 2f), (int)num3, (int)num3);
						Utils.RenderSpriteIntoClippedRectDest(dest, targetBounds, ZoomingDotGridEffect.CircleTex, themeColor * scale, sb);
						point.X += num7;
					}
					point.Y += num7;
				}
			}
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00017DF8 File Offset: 0x00015FF8
		private static Vector2 rotatePointAroundOrigin(Vector2 point, Vector2 origin, float rotation)
		{
			Vector2 point2 = point - origin;
			Vector2 value = Utils.RotatePoint(point2, rotation);
			return value + origin;
		}

		// Token: 0x040001CB RID: 459
		private static Texture2D CircleTex;
	}
}
