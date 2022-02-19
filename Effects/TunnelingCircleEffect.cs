using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x0200005B RID: 91
	public class TunnelingCircleEffect
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x0001792C File Offset: 0x00015B2C
		public static void Draw(SpriteBatch sb, Vector2 circleCentre, float diamater, float nodeDiamater, int subdivisions, float timer, Color nodeColor, Color lineColor, Color ZeroNodeColor, Rectangle clipBounds)
		{
			if (TunnelingCircleEffect.CircleTex == null)
			{
				TunnelingCircleEffect.CircleTex = Game1.getSingleton().Content.Load<Texture2D>("Circle");
			}
			float num = diamater / 2f;
			for (int i = 0; i < subdivisions; i++)
			{
				float num2 = (float)(3.141592653589793 / (double)subdivisions);
				float num3 = (float)i * num2;
				Vector2 left = circleCentre - Utils.PolarToCartesian(num3, num);
				Vector2 right = circleCentre + Utils.PolarToCartesian(num3, num);
				Vector2 vector;
				Vector2 vector2;
				Utils.ClipLineSegmentsForRect(clipBounds, left, right, out vector, out vector2);
				if (Vector2.Distance(vector, vector2) > 0.001f)
				{
					Utils.drawLine(sb, vector, vector2, Vector2.Zero, lineColor, 0.2f);
				}
				float num4 = (float)Math.Sin((double)timer + (double)num3);
				Vector2 vector3 = circleCentre + Utils.PolarToCartesian(num3, num * num4) - new Vector2(nodeDiamater / 2f);
				Vector2 scale = new Vector2(nodeDiamater / (float)TunnelingCircleEffect.CircleTex.Width);
				Vector2 origin = new Vector2(nodeDiamater / 2f * scale.X);
				Rectangle clipRectForSpritePos = Utils.getClipRectForSpritePos(clipBounds, TunnelingCircleEffect.CircleTex, vector3, scale.X);
				if (clipRectForSpritePos.Height > 0 && clipRectForSpritePos.Width > 0)
				{
					sb.Draw(TunnelingCircleEffect.CircleTex, vector3, new Rectangle?(clipRectForSpritePos), (i == 0) ? ZeroNodeColor : nodeColor, 0f, origin, scale, SpriteEffects.None, 0.25f);
				}
			}
		}

		// Token: 0x040001CA RID: 458
		private static Texture2D CircleTex;
	}
}
