using System;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x020000CE RID: 206
	public class HSLColor
	{
		// Token: 0x06000428 RID: 1064 RVA: 0x000429D1 File Offset: 0x00040BD1
		public HSLColor(float H, float S, float L)
		{
			this.Hue = H;
			this.Saturation = S;
			this.Luminosity = L;
		}

		// Token: 0x06000429 RID: 1065 RVA: 0x000429F4 File Offset: 0x00040BF4
		public static HSLColor FromRGB(Color Clr)
		{
			return HSLColor.FromRGB(Clr.R, Clr.G, Clr.B);
		}

		// Token: 0x0600042A RID: 1066 RVA: 0x00042A20 File Offset: 0x00040C20
		public static HSLColor FromRGB(byte R, byte G, byte B)
		{
			float num = (float)R / 255f;
			float num2 = (float)G / 255f;
			float num3 = (float)B / 255f;
			float num4 = Math.Min(Math.Min(num, num2), num3);
			float num5 = Math.Max(Math.Max(num, num2), num3);
			float num6 = num5 - num4;
			float h = 0f;
			float s = 0f;
			float num7 = (num5 + num4) / 2f;
			if (num6 != 0f)
			{
				if (num7 < 0.5f)
				{
					s = num6 / (num5 + num4);
				}
				else
				{
					s = num6 / (2f - num5 - num4);
				}
				if (num == num5)
				{
					h = (num2 - num3) / num6;
				}
				else if (num2 == num5)
				{
					h = 2f + (num3 - num) / num6;
				}
				else if (num3 == num5)
				{
					h = 4f + (num - num2) / num6;
				}
			}
			return new HSLColor(h, s, num7);
		}

		// Token: 0x0600042B RID: 1067 RVA: 0x00042B38 File Offset: 0x00040D38
		private float Hue_2_RGB(float v1, float v2, float vH)
		{
			if (vH < 0f)
			{
				vH += 1f;
			}
			if (vH > 1f)
			{
				vH -= 1f;
			}
			float result;
			if (6f * vH < 1f)
			{
				result = v1 + (v2 - v1) * 6f * vH;
			}
			else if (2f * vH < 1f)
			{
				result = v2;
			}
			else if (3f * vH < 2f)
			{
				result = v1 + (v2 - v1) * (0f - vH) * 6f;
			}
			else
			{
				result = v1;
			}
			return result;
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x00042BE4 File Offset: 0x00040DE4
		public Color ToRGB()
		{
			byte r;
			byte g;
			byte b;
			if (this.Saturation == 0f)
			{
				r = (byte)Math.Round((double)this.Luminosity * 255.0);
				g = (byte)Math.Round((double)this.Luminosity * 255.0);
				b = (byte)Math.Round((double)this.Luminosity * 255.0);
			}
			else
			{
				double num = (double)this.Hue / 6.0;
				double num2;
				if ((double)this.Luminosity < 0.5)
				{
					num2 = (double)this.Luminosity * (1.0 + (double)this.Saturation);
				}
				else
				{
					num2 = (double)(this.Luminosity + this.Saturation - this.Luminosity * this.Saturation);
				}
				double t = 2.0 * (double)this.Luminosity - num2;
				double num3 = num + 0.3333333333333333;
				double num4 = num;
				double num5 = num - 0.3333333333333333;
				num3 = HSLColor.ColorCalc(num3, t, num2);
				num4 = HSLColor.ColorCalc(num4, t, num2);
				num5 = HSLColor.ColorCalc(num5, t, num2);
				r = (byte)Math.Round(num3 * 255.0);
				g = (byte)Math.Round(num4 * 255.0);
				b = (byte)Math.Round(num5 * 255.0);
			}
			return new Color
			{
				R = r,
				G = g,
				B = b,
				A = byte.MaxValue
			};
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x00042D90 File Offset: 0x00040F90
		private static double ColorCalc(double c, double t1, double t2)
		{
			if (c < 0.0)
			{
				c += 1.0;
			}
			if (c > 1.0)
			{
				c -= 1.0;
			}
			double result;
			if (6.0 * c < 1.0)
			{
				result = t1 + (t2 - t1) * 6.0 * c;
			}
			else if (2.0 * c < 1.0)
			{
				result = t2;
			}
			else if (3.0 * c < 2.0)
			{
				result = t1 + (t2 - t1) * (0.6666666666666666 - c) * 6.0;
			}
			else
			{
				result = t1;
			}
			return result;
		}

		// Token: 0x04000513 RID: 1299
		public float Hue;

		// Token: 0x04000514 RID: 1300
		public float Saturation;

		// Token: 0x04000515 RID: 1301
		public float Luminosity;
	}
}
