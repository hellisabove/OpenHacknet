using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Effects
{
	// Token: 0x020000B2 RID: 178
	public class PortHackCubeSequence
	{
		// Token: 0x06000397 RID: 919 RVA: 0x00036C5C File Offset: 0x00034E5C
		public void Reset()
		{
			this.spinup = (this.startup = (this.runtime = (this.idle = (this.spindown = (this.rotTime = 0f)))));
			this.HeartFadeSequenceComplete = false;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00036CAC File Offset: 0x00034EAC
		public void DrawSequence(Rectangle dest, float t, float totalTime)
		{
			float num = 0.6f;
			float num2 = 1f;
			float num3 = this.ShouldCentralSpinInfinitley ? float.MaxValue : (totalTime - 1.7f);
			float maxValue = float.MaxValue;
			float maxValue2 = float.MaxValue;
			this.elapsedTime += t;
			float val = this.elapsedTime;
			val = Math.Max(0f, val);
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			if (this.startup < num)
			{
				this.startup += t;
				num4 = 0f;
				num5 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.startup / num));
			}
			else if (this.spinup < num2)
			{
				this.spinup = Math.Min(num2, this.spinup + t);
				num4 = this.spinup / num2;
				this.rotTime += t * num4;
			}
			else if (this.runtime < num3)
			{
				this.runtime = Math.Min(num3, this.runtime + t);
				this.rotTime += t;
			}
			else if (this.spindown < maxValue)
			{
				this.spindown += t;
				float num7 = Math.Min(1f, this.spindown) / 2.3f / 1f;
				num4 = 0.1f + 0.9f * (1f - num7);
				if (num7 >= 0.4f)
				{
					num4 = 0.2f;
				}
				this.rotTime += t * num4;
				num6 = 1f - num7;
				num5 = 0.3f + 0.7f * Utils.QuadraticOutCurve(1f - num7);
			}
			else if (this.idle < maxValue2)
			{
				this.idle += t;
				num4 = 0.1f;
				this.rotTime += t * (num4 * 2f);
				num6 = 0f;
				num5 = 0.3f;
			}
			else
			{
				this.spinup = (this.startup = (this.runtime = (this.idle = (this.spindown = (this.rotTime = 0f)))));
			}
			int num8 = 20;
			for (int i = 0; i < num8; i++)
			{
				float num9 = (float)i / (float)num8;
				float num10 = 1f + 0.05f * num4 * ((float)(num8 - i) / (float)num8 * 10f);
				float num11 = num6;
				float num12 = this.rotTime * num10 - (float)i * 0.1f * num11;
				num12 = Math.Max(0f, num12);
				float num13 = num12 * 1.5f;
				float num14 = num5;
				for (int j = 0; j < i; j++)
				{
					num14 *= num5;
				}
				float num15 = num5 * ((float)(num8 - i) / (float)num8);
				bool flag = num9 <= num4;
				Cube3D.RenderWireframe(new Vector3((float)(dest.X / 2), 0f, 0f), 2.6f + (float)i / 4f * num14, new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f) + num13, MathHelper.ToRadians(0f)), Color.White);
			}
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00037048 File Offset: 0x00035248
		public void DrawHeartSequence(Rectangle dest, float t, float totalTime)
		{
			float num = 3f;
			float num2 = 10f;
			float num3 = 2f;
			float num4 = 9f;
			float maxValue = float.MaxValue;
			float num5 = 2f;
			float num6 = 21f;
			this.elapsedTime += t;
			float val = this.elapsedTime;
			val = Math.Max(0f, val);
			float num7 = 1f;
			float num8 = 1f;
			float num9 = 1f;
			float num10 = 0f;
			if (this.startup < num)
			{
				this.startup += t;
				num8 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.startup / 3f));
			}
			else if (this.spinup < num2)
			{
				this.spinup = Math.Min(10f, this.spinup + t);
				num7 = this.spinup / 10f;
				this.rotTime += t * num7;
			}
			else if (this.runtime < num3)
			{
				this.runtime = Math.Min(2f, this.runtime + t);
				this.rotTime += t;
			}
			else if (this.spindown < num4)
			{
				this.spindown += t;
				float num11 = this.spindown / 9f;
				num7 = 0.1f + 0.9f * (1f - num11);
				this.rotTime += t * num7;
				num9 = 1f - num11;
				num8 = 0.3f + 0.7f * Utils.QuadraticOutCurve(1f - num11);
			}
			else if (this.idle < maxValue)
			{
				this.idle += t;
				num7 = 0.1f;
				this.rotTime += t * num7;
				num9 = 0f;
				num8 = 0.3f;
				if (this.idle > num5)
				{
					num10 = Utils.QuadraticOutCurve(Math.Min(1f, (this.idle - num5) / num6));
					this.HeartFadeSequenceComplete = (num10 >= 1f);
				}
			}
			else
			{
				this.spinup = (this.startup = (this.runtime = (this.idle = (this.spindown = (this.rotTime = 0f)))));
			}
			int num12 = 20;
			for (int i = 0; i < num12; i++)
			{
				float num13 = (float)i / (float)num12;
				float num14 = 1f + 0.05f * num7 * ((float)(num12 - i) / (float)num12 * 10f);
				float num15 = num9;
				float num16 = this.rotTime * num14 - (float)i * 0.1f * num15;
				num16 = Math.Max(0f, num16);
				float num17 = num16 * 1.5f;
				float num18 = num8;
				for (int j = 0; j < i; j++)
				{
					num18 *= num8;
				}
				float num19 = num8 * ((float)(num12 - i) / (float)num12);
				bool flag = num13 <= num7;
				Color color = Color.White;
				if (num10 > 0f)
				{
					float num20 = 1f / (float)num12;
					if ((float)i * num20 < num10)
					{
						float num21 = (float)(i + 1) * num20;
						if (num10 > num21)
						{
							color = Color.Transparent;
						}
						else
						{
							float num22 = 1f - num10 % num20 / num20;
							color = Color.Lerp(Utils.AddativeRed, Color.Red, num22) * num22;
						}
					}
				}
				Cube3D.RenderWireframe(new Vector3((float)(dest.X / 2), 0f, 0f), 2.6f + (float)i / 4f * num18, new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f) + num17, MathHelper.ToRadians(0f)), color);
			}
		}

		// Token: 0x0400041D RID: 1053
		private float rotTime = 0f;

		// Token: 0x0400041E RID: 1054
		private float elapsedTime = 0f;

		// Token: 0x0400041F RID: 1055
		private float startup = 0.3f;

		// Token: 0x04000420 RID: 1056
		private float spinup = 0f;

		// Token: 0x04000421 RID: 1057
		private float runtime = 0f;

		// Token: 0x04000422 RID: 1058
		private float spindown = 0f;

		// Token: 0x04000423 RID: 1059
		private float idle = 0f;

		// Token: 0x04000424 RID: 1060
		public bool HeartFadeSequenceComplete = false;

		// Token: 0x04000425 RID: 1061
		public bool ShouldCentralSpinInfinitley = false;
	}
}
