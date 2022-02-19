using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200016D RID: 365
	public static class SFX
	{
		// Token: 0x06000920 RID: 2336 RVA: 0x00096DB1 File Offset: 0x00094FB1
		public static void init(ContentManager content)
		{
			SFX.circleTex = content.Load<Texture2D>("CircleOutlineLarge");
			SFX.circleOrigin = new Vector2((float)(SFX.circleTex.Width / 2), (float)(SFX.circleTex.Height / 2));
		}

		// Token: 0x06000921 RID: 2337 RVA: 0x00096DE8 File Offset: 0x00094FE8
		public static void AddRadialLine(Vector2 pos, float incomingAngle, float startDistance, float startSpeed, float acceleration, float fadeDistance, float speedSizeMultiplier, Color color, float width = 1f, bool snapMode = false)
		{
			SFX.RadialLines.Add(new SFX.RadialLineData
			{
				destination = pos,
				angle = incomingAngle,
				color = color,
				distance = startDistance,
				Acceleration = acceleration,
				FadeDistance = fadeDistance,
				SizeForSpeedMultiplier = speedSizeMultiplier,
				SnapMode = snapMode,
				width = width
			});
		}

		// Token: 0x06000922 RID: 2338 RVA: 0x00096E58 File Offset: 0x00095058
		public static void addCircle(Vector2 pos, Color color, float radius)
		{
			SFX.circlePos.Add(pos);
			SFX.circleColor.Add(color);
			SFX.circleExpand.Add(0f);
			SFX.circleRadius.Add(radius / 256f);
		}

		// Token: 0x06000923 RID: 2339 RVA: 0x00096E98 File Offset: 0x00095098
		public static void Update(float t)
		{
			for (int i = 0; i < SFX.circlePos.Count; i++)
			{
				List<float> list;
				int index;
				(list = SFX.circleExpand)[index = i] = list[index] + t;
				if (SFX.circleExpand[i] >= 3f)
				{
					SFX.circlePos.RemoveAt(i);
					SFX.circleColor.RemoveAt(i);
					SFX.circleExpand.RemoveAt(i);
					SFX.circleRadius.RemoveAt(i);
					i--;
				}
			}
			for (int i = 0; i < SFX.RadialLines.Count; i++)
			{
				SFX.RadialLineData value = SFX.RadialLines[i];
				value.distance -= value.MovementPerSecond * t;
				value.MovementPerSecond += value.Acceleration * t;
				if (value.distance <= 0f)
				{
					SFX.RadialLines.RemoveAt(i);
					i--;
				}
				else
				{
					SFX.RadialLines[i] = value;
				}
			}
		}

		// Token: 0x06000924 RID: 2340 RVA: 0x00096FB8 File Offset: 0x000951B8
		public static void Draw(SpriteBatch sb)
		{
			try
			{
				for (int i = 0; i < SFX.circlePos.Count; i++)
				{
					float num = SFX.circleExpand[i] / 3f;
					if (num < 0.2f)
					{
						num /= 0.2f;
					}
					else
					{
						num = 1f - num;
					}
					sb.Draw(SFX.circleTex, SFX.circlePos[i], null, SFX.circleColor[i] * num, 0f, SFX.circleOrigin, Vector2.One * SFX.circleRadius[i] * SFX.circleExpand[i], SpriteEffects.None, 0.2f);
				}
				for (int i = 0; i < SFX.RadialLines.Count; i++)
				{
					SFX.RadialLineData radialLineData = SFX.RadialLines[i];
					float num2 = radialLineData.MovementPerSecond * radialLineData.SizeForSpeedMultiplier;
					Vector2 vector = Utils.PolarToCartesian(radialLineData.angle, radialLineData.distance);
					if (radialLineData.SnapMode)
					{
						num2 = Math.Min(num2, vector.Length());
					}
					else
					{
						num2 = Math.Min(num2, radialLineData.distance);
						if (num2 < 0f)
						{
							num2 = 0f;
						}
					}
					Vector2 value = vector + Utils.PolarToCartesian(radialLineData.angle, num2);
					float scale = 1f;
					if (radialLineData.distance > radialLineData.FadeDistance)
					{
						scale = 1f - (radialLineData.distance - radialLineData.FadeDistance) / radialLineData.FadeDistance;
					}
					Utils.drawLineAlt(sb, radialLineData.destination + vector, radialLineData.destination + value, Vector2.Zero, radialLineData.color * scale, 0.5f, radialLineData.width, Utils.gradientLeftRight);
				}
			}
			catch (IndexOutOfRangeException)
			{
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		// Token: 0x04000AA3 RID: 2723
		private static List<Vector2> circlePos = new List<Vector2>();

		// Token: 0x04000AA4 RID: 2724
		private static List<float> circleRadius = new List<float>();

		// Token: 0x04000AA5 RID: 2725
		private static List<float> circleExpand = new List<float>();

		// Token: 0x04000AA6 RID: 2726
		private static List<Color> circleColor = new List<Color>();

		// Token: 0x04000AA7 RID: 2727
		private static List<SFX.RadialLineData> RadialLines = new List<SFX.RadialLineData>();

		// Token: 0x04000AA8 RID: 2728
		private static Texture2D circleTex;

		// Token: 0x04000AA9 RID: 2729
		private static Vector2 circleOrigin;

		// Token: 0x0200016E RID: 366
		private struct RadialLineData
		{
			// Token: 0x04000AAA RID: 2730
			public Vector2 destination;

			// Token: 0x04000AAB RID: 2731
			public float angle;

			// Token: 0x04000AAC RID: 2732
			public float distance;

			// Token: 0x04000AAD RID: 2733
			public Color color;

			// Token: 0x04000AAE RID: 2734
			public float width;

			// Token: 0x04000AAF RID: 2735
			public float MovementPerSecond;

			// Token: 0x04000AB0 RID: 2736
			public float Acceleration;

			// Token: 0x04000AB1 RID: 2737
			public float FadeDistance;

			// Token: 0x04000AB2 RID: 2738
			public float SizeForSpeedMultiplier;

			// Token: 0x04000AB3 RID: 2739
			public bool SnapMode;
		}
	}
}
