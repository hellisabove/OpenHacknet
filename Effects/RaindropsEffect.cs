using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x02000057 RID: 87
	public class RaindropsEffect
	{
		// Token: 0x060001A7 RID: 423 RVA: 0x00016BE5 File Offset: 0x00014DE5
		public void Init(ContentManager content)
		{
			this.Circle = content.Load<Texture2D>("CircleOutlineLarge");
			this.Gradient = content.Load<Texture2D>("Gradient");
			this.FlashImage = content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00016C1B File Offset: 0x00014E1B
		public void ForceSpawnDrop(Vector3 dropData)
		{
			this.Drops.Add(dropData);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00016C2C File Offset: 0x00014E2C
		public void Update(float dt, float dropsAddedPerSecond)
		{
			if (Utils.randm(1f) < dropsAddedPerSecond * dt)
			{
				this.Drops.Add(new Vector3(Utils.randm(1f), 0f, Utils.rand(this.MaxVerticalLandingVariane)));
			}
			for (int i = 0; i < this.Drops.Count; i++)
			{
				float num = dt * this.FallRate * (1f + this.Drops[i].Z / 2f);
				this.Drops[i] = new Vector3(this.Drops[i].X, this.Drops[i].Y + num, this.Drops[i].Z);
				if (this.Drops[i].Y >= 1f + this.Drops[i].Z)
				{
					float x = this.Drops[i].X;
					float z = this.Drops[i].Z;
					this.Drops.RemoveAt(i);
					this.Circles.Add(new Vector3(x, 0f, z));
					this.FadeoutLines.Add(new Vector3(x, 0f, z));
					i--;
				}
			}
			for (int i = 0; i < this.Circles.Count; i++)
			{
				this.Circles[i] = new Vector3(this.Circles[i].X, this.Circles[i].Y + dt * this.CircleExpandRate, this.Circles[i].Z);
				if (this.Circles[i].Y >= 1f)
				{
					this.Circles.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < this.FadeoutLines.Count; i++)
			{
				this.FadeoutLines[i] = new Vector3(this.FadeoutLines[i].X, this.FadeoutLines[i].Y + dt * this.LineFadeoutRate, this.FadeoutLines[i].Z);
				if (this.FadeoutLines[i].Y >= 1f)
				{
					this.FadeoutLines.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00016EF0 File Offset: 0x000150F0
		public void Render(Rectangle dest, SpriteBatch sb, Color DropColor, float maxCircleRadius, float maxFlashWidth)
		{
			int num = 3;
			int num2 = 4;
			float scale = 0.3f;
			float scale2 = 0.2f;
			Color color = Utils.makeColorAddative(DropColor);
			float num3 = (float)dest.Height * 0.84f;
			for (int i = 0; i < this.FadeoutLines.Count; i++)
			{
				Vector3 vector = this.FadeoutLines[i];
				Rectangle destinationRectangle = new Rectangle((int)((float)dest.X + vector.X * (float)dest.Width - (float)num / 2f), dest.Y, num, (int)(num3 + vector.Z * (float)dest.Height));
				sb.Draw(Utils.white, destinationRectangle, DropColor * scale2 * (1f - vector.Y));
			}
			for (int i = 0; i < this.Drops.Count; i++)
			{
				Vector3 vector2 = this.Drops[i];
				Rectangle destinationRectangle2 = new Rectangle((int)((float)dest.X + vector2.X * (float)dest.Width - (float)num / 2f), dest.Y, num, (int)(vector2.Y * (num3 + vector2.Z * (float)dest.Height)));
				sb.Draw(Utils.white, destinationRectangle2, DropColor * scale);
				Rectangle destinationRectangle3 = new Rectangle(destinationRectangle2.X, destinationRectangle2.Y + destinationRectangle2.Height - num2, destinationRectangle2.Width, num2);
				sb.Draw(Utils.gradient, destinationRectangle3, color);
				sb.Draw(Utils.gradient, destinationRectangle3, Utils.AddativeWhite * 0.5f);
			}
			Vector2 value = new Vector2(4f, 1f);
			for (int i = 0; i < this.Circles.Count; i++)
			{
				Vector3 vector3 = this.Circles[i];
				Vector2 origin = new Vector2((float)(this.Circle.Width / 2), (float)(this.Circle.Height / 2));
				Vector2 scale3 = value * vector3.Y * (maxCircleRadius / (float)this.Circle.Width);
				Vector2 vector4 = new Vector2((float)dest.X + vector3.X * (float)dest.Width, num3 + (float)dest.Y + vector3.Z * (float)dest.Height);
				Rectangle? sourceRectangle = new Rectangle?(Utils.getClipRectForSpritePos(dest, this.Circle, vector4, scale3, origin));
				Vector2 value2 = new Vector2((float)sourceRectangle.Value.X * scale3.X, (float)sourceRectangle.Value.Y * scale3.Y);
				Color color2 = Color.Lerp(Utils.AddativeWhite, Color.Transparent, (float)Math.Min(1.0, (double)vector3.Y * 2.0));
				float num4 = 1f;
				float num5 = 0.6f;
				if (vector3.Y >= num5)
				{
					float num6 = 1f / (1f - num5);
					num4 = 1f - (vector3.Y - num5) * num6;
				}
				num4 *= 0.2f;
				Color color3 = DropColor * num4;
				float num7 = 0.55f;
				if (vector3.Y < num7)
				{
					float num8 = vector3.Y / num7;
					num8 = Utils.QuadraticOutCurve(num8);
					Vector2 vector5 = new Vector2(1f, 3f);
					Vector2 vector6 = new Vector2((float)this.FlashImage.Width * vector5.X, (float)this.FlashImage.Height * vector5.Y);
					vector6 = new Vector2(maxFlashWidth / vector6.X, maxFlashWidth / vector6.Y);
					vector6 *= 0.3f + num8 / 3f * 2f;
					Vector2 centreOrigin = this.FlashImage.GetCentreOrigin();
					Rectangle clipRectForSpritePos = Utils.getClipRectForSpritePos(dest, this.FlashImage, vector4, vector6, centreOrigin);
					Vector2 value3 = new Vector2((float)clipRectForSpritePos.X * vector6.X, (float)clipRectForSpritePos.Y * vector6.Y);
					sb.Draw(this.FlashImage, vector4 + value3, new Rectangle?(clipRectForSpritePos), color * (1f - num8), 0f, centreOrigin, vector6, SpriteEffects.None, 0.5f);
					if (num8 < 0.5f)
					{
						sb.Draw(this.FlashImage, vector4, null, Utils.AddativeWhite * (1f - num8 * 2f), 0f, centreOrigin, vector6 * 0.6f, SpriteEffects.None, 0.5f);
					}
				}
				sb.Draw(this.Circle, vector4 + value2, sourceRectangle, color3, 0f, origin, scale3, SpriteEffects.None, 0.5f);
				sb.Draw(this.Circle, vector4 + value2, sourceRectangle, color2, 0f, origin, scale3, SpriteEffects.None, 0.5f);
			}
		}

		// Token: 0x040001BB RID: 443
		private List<Vector3> Drops = new List<Vector3>();

		// Token: 0x040001BC RID: 444
		private List<Vector3> Circles = new List<Vector3>();

		// Token: 0x040001BD RID: 445
		private List<Vector3> FadeoutLines = new List<Vector3>();

		// Token: 0x040001BE RID: 446
		public float FallRate = 1f;

		// Token: 0x040001BF RID: 447
		public float CircleExpandRate = 0.4f;

		// Token: 0x040001C0 RID: 448
		public float LineFadeoutRate = 2f;

		// Token: 0x040001C1 RID: 449
		public float MaxVerticalLandingVariane = 0.025f;

		// Token: 0x040001C2 RID: 450
		private Texture2D Circle;

		// Token: 0x040001C3 RID: 451
		private Texture2D Gradient;

		// Token: 0x040001C4 RID: 452
		private Texture2D FlashImage;
	}
}
