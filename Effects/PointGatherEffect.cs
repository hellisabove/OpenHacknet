using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x02000055 RID: 85
	public class PointGatherEffect
	{
		// Token: 0x0600019F RID: 415 RVA: 0x00015F6C File Offset: 0x0001416C
		public void Init(ContentManager content)
		{
			this.CircleTex = content.Load<Texture2D>("Circle");
			this.ScanlinesBackground = content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
			this.Star = content.Load<Texture2D>("EffectFiles/PointClicker/Star");
			this.LineTexture = Utils.white;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00015FB8 File Offset: 0x000141B8
		public void Explode(int entities)
		{
			for (int i = 0; i < entities; i++)
			{
				float num = (Utils.randm(1f) < 0.3f) ? 2f : 1f;
				this.Points.Add(new PointGatherEffect.StarPoint
				{
					Pos = new Vector2(0.5f, 0.5f),
					Velocity = new Vector2((float)(-1.0 * (double)this.MaxSpeed + Utils.random.NextDouble() * (double)this.MaxSpeed * 2.0), (float)(-1.0 * (double)this.MaxSpeed + Utils.random.NextDouble() * (double)this.MaxSpeed * 2.0)),
					size = num,
					drawnSize = num,
					AttractedToOthers = (Utils.randm(1f) < 0.7f)
				});
			}
			this.timeRemainingWithoutAttract = 1f;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000160C8 File Offset: 0x000142C8
		private PointGatherEffect.StarPoint ProcessAttractionBetweenPoints(PointGatherEffect.StarPoint pA, PointGatherEffect.StarPoint pB, float dt, bool constantForce = false)
		{
			Vector2 value = constantForce ? (pA.Pos + pA.Velocity * 0.4f) : pA.Pos;
			float num = Vector2.Distance(pA.Pos, pB.Pos);
			float num2 = (float)Math.Pow((double)num, 2.0);
			if (constantForce)
			{
				num2 = 0.2f;
			}
			float num3 = this.GravityConstant * (pA.size * pB.size) / num2;
			num3 *= pA.size / pB.size;
			Vector2 value2 = value - pB.Pos;
			value2.Normalize();
			value2 *= num3;
			pB.Velocity += value2 * dt;
			return pB;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x000161A6 File Offset: 0x000143A6
		public void FlashComplete()
		{
			this.starSize = 6f;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x000161B4 File Offset: 0x000143B4
		public void Update(float dt)
		{
			float num = this.NoAttractPhaseFriction;
			this.timeRemainingWithoutAttract -= dt;
			if (this.timeRemainingWithoutAttract <= 0f)
			{
				if (this.Points.Count > 1)
				{
					num = this.Friction;
				}
				for (int i = 0; i < this.Points.Count; i++)
				{
					for (int j = 0; j < this.Points.Count; j++)
					{
						if (i != j)
						{
							PointGatherEffect.StarPoint pA = this.Points[i];
							PointGatherEffect.StarPoint pB = this.Points[j];
							if (pB.AttractedToOthers)
							{
								this.Points[j] = this.ProcessAttractionBetweenPoints(pA, pB, dt, false);
							}
						}
					}
				}
			}
			PointGatherEffect.StarPoint pA2 = new PointGatherEffect.StarPoint
			{
				Pos = new Vector2(0.5f),
				size = this.attractionToCentreMass,
				Velocity = Vector2.Zero
			};
			for (int k = 0; k < this.Points.Count; k++)
			{
				PointGatherEffect.StarPoint starPoint = this.Points[k];
				if (starPoint.drawnSize < starPoint.size)
				{
					starPoint.drawnSize = Math.Min(starPoint.size, starPoint.drawnSize + dt * 15f);
				}
				starPoint.Pos += starPoint.Velocity * dt;
				if (starPoint.Pos.X > 1f || starPoint.Pos.Y > 1f || starPoint.Pos.X < 0f || starPoint.Pos.Y < 0f)
				{
					starPoint.Pos.X = Math.Max(0f, Math.Min(starPoint.Pos.X, 1f));
					starPoint.Pos.Y = Math.Max(0f, Math.Min(starPoint.Pos.Y, 1f));
					starPoint.Velocity *= -0.5f;
				}
				starPoint.Velocity -= starPoint.Velocity * (num * dt);
				this.Points[k] = starPoint;
				if (this.Points.Count > 1)
				{
					this.Points[k] = this.ProcessAttractionBetweenPoints(pA2, starPoint, dt, true);
				}
			}
			this.ResolveMergesThisFrame();
			if (this.starSize > 0f)
			{
				this.starSize -= dt * 3f;
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x000164C8 File Offset: 0x000146C8
		private int ResolveMergesThisFrame()
		{
			if (this.timeRemainingWithoutAttract <= 0f)
			{
				for (int i = 0; i < this.Points.Count; i++)
				{
					for (int j = 0; j < this.Points.Count; j++)
					{
						if (i != j)
						{
							PointGatherEffect.StarPoint starPoint = this.Points[i];
							PointGatherEffect.StarPoint starPoint2 = this.Points[j];
							float num = Vector2.Distance(starPoint.Pos, starPoint2.Pos);
							bool flag = 1 == 0;
							if (num < this.absorbDistance * (1f + 0.2f * (starPoint.size + starPoint2.size)))
							{
								float num2 = starPoint.size / starPoint2.size;
								float drawnSize = (starPoint.drawnSize > starPoint2.drawnSize) ? starPoint.drawnSize : starPoint2.drawnSize;
								PointGatherEffect.StarPoint item = new PointGatherEffect.StarPoint
								{
									Pos = ((starPoint.size > starPoint2.size) ? starPoint.Pos : starPoint2.Pos),
									Velocity = Vector2.Lerp(Vector2.Zero, starPoint.Velocity + starPoint2.Velocity, 0.3f),
									size = starPoint.size + starPoint2.size,
									drawnSize = drawnSize
								};
								this.Points.RemoveAt(i);
								if (j > i)
								{
									this.Points.RemoveAt(j - 1);
								}
								else
								{
									this.Points.RemoveAt(j);
								}
								this.Points.Add(item);
								int num3 = 1;
								return num3 + this.ResolveMergesThisFrame();
							}
						}
					}
				}
			}
			return 0;
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x000166C4 File Offset: 0x000148C4
		public void Render(Rectangle dest, SpriteBatch sb)
		{
			Vector2 value = new Vector2((float)dest.Width, (float)dest.Height);
			Vector2 value2 = new Vector2((float)dest.X, (float)dest.Y);
			for (int i = 0; i < this.Points.Count; i++)
			{
				Vector2 vector = this.Points[i].Pos * value + value2;
				float num = this.Points[i].drawnSize;
				int num2 = this.AllowDoubleLines ? this.Points.Count : (this.Points.Count / 2);
				for (int j = 0; j < num2; j++)
				{
					if (j != i)
					{
						Vector2 vector2 = this.Points[j].Pos * value + value2;
						float drawnSize = this.Points[j].drawnSize;
						if (drawnSize + num > 20f)
						{
							Utils.drawLineAlt(sb, vector, vector2, Vector2.Zero, OS.currentInstance.highlightColor * 0.6f, 0.7f, this.LineLengthPercentage, this.LineTexture);
						}
					}
				}
				num /= (float)this.CircleTex.Width;
				float num3 = Utils.SmoothStep(0f, 20f, this.Points[i].drawnSize);
				float num4 = 0.5f - num3;
				float num5 = num3 * 0.5f;
				num3 *= this.GlowScaleMod;
				Vector2 vector3 = new Vector2(num5 * 1f * num3, num5 * 3f * num3);
				Vector2 vector4 = new Vector2((float)this.ScanlinesBackground.Width / 2f, (float)this.ScanlinesBackground.Height / 2f);
				Vector2 origin = new Vector2((float)this.CircleTex.Width / 2f);
				Rectangle clipRectForSpritePos = Utils.getClipRectForSpritePos(dest, this.ScanlinesBackground, vector - vector4 * vector3, vector3);
				sb.Draw(this.ScanlinesBackground, vector + new Vector2((float)clipRectForSpritePos.X * vector3.X, (float)clipRectForSpritePos.Y * vector3.Y), new Rectangle?(clipRectForSpritePos), Utils.AddativeWhite * 0.05f, 0f, vector4, vector3, SpriteEffects.None, 0.7f);
				float num6 = (float)this.CircleTex.Width * num;
				if (num6 > (float)dest.Height)
				{
					num = (float)dest.Height / (float)this.CircleTex.Width;
				}
				sb.Draw(this.CircleTex, vector, null, this.NodeColor, 0f, origin, new Vector2(num), SpriteEffects.None, 0.7f);
				if (this.starSize > 0f && this.Points[i].drawnSize > 30f)
				{
					Color value3 = Color.Lerp(Utils.makeColorAddative(OS.currentInstance.highlightColor), OS.currentInstance.highlightColor, 0.5f);
					sb.Draw(this.Star, vector, null, value3 * (this.starSize / 4f), OS.currentInstance.timer * 0f, new Vector2((float)(this.Star.Width / 2), (float)(this.Star.Height / 2)), new Vector2(this.starSize * 3.2f, this.starSize * 1.2f), SpriteEffects.None, 0.8f);
					value3 = Utils.makeColorAddative(OS.currentInstance.highlightColor);
					sb.Draw(this.Star, vector, null, value3 * (this.starSize / 4f), OS.currentInstance.timer * 0f, new Vector2((float)(this.Star.Width / 2), (float)(this.Star.Height / 2)), new Vector2(this.starSize * 2.7f, this.starSize * 1f), SpriteEffects.None, 0.8f);
				}
			}
		}

		// Token: 0x040001A5 RID: 421
		public List<PointGatherEffect.StarPoint> Points = new List<PointGatherEffect.StarPoint>();

		// Token: 0x040001A6 RID: 422
		public float timeRemainingWithoutAttract = 0f;

		// Token: 0x040001A7 RID: 423
		public float MaxSpeed = 1.6f;

		// Token: 0x040001A8 RID: 424
		public float Friction = 0.001f;

		// Token: 0x040001A9 RID: 425
		public float NoAttractPhaseFriction = 1.7f;

		// Token: 0x040001AA RID: 426
		public float GravityConstant = 0.001f;

		// Token: 0x040001AB RID: 427
		public float absorbDistance = 0.005f;

		// Token: 0x040001AC RID: 428
		public float attractionToCentreMass = 1f;

		// Token: 0x040001AD RID: 429
		public float GlowScaleMod = 1f;

		// Token: 0x040001AE RID: 430
		public float LineLengthPercentage = 1f;

		// Token: 0x040001AF RID: 431
		public bool AllowDoubleLines = false;

		// Token: 0x040001B0 RID: 432
		private float starSize = -1f;

		// Token: 0x040001B1 RID: 433
		public Texture2D CircleTex;

		// Token: 0x040001B2 RID: 434
		public Texture2D ScanlinesBackground;

		// Token: 0x040001B3 RID: 435
		public Texture2D Star;

		// Token: 0x040001B4 RID: 436
		public Texture2D LineTexture;

		// Token: 0x040001B5 RID: 437
		public Color NodeColor = Utils.AddativeWhite * 0.65f;

		// Token: 0x02000056 RID: 86
		public struct StarPoint
		{
			// Token: 0x040001B6 RID: 438
			public Vector2 Pos;

			// Token: 0x040001B7 RID: 439
			public Vector2 Velocity;

			// Token: 0x040001B8 RID: 440
			public float size;

			// Token: 0x040001B9 RID: 441
			public bool AttractedToOthers;

			// Token: 0x040001BA RID: 442
			public float drawnSize;
		}
	}
}
