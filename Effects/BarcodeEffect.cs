using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x0200010B RID: 267
	public class BarcodeEffect
	{
		// Token: 0x06000655 RID: 1621 RVA: 0x000698F8 File Offset: 0x00067AF8
		public BarcodeEffect(int width, bool inverted = false, bool leftRight = false)
		{
			this.maxWidth = width;
			this.isInverted = inverted;
			this.leftRightBias = leftRight;
			this.reset();
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0006994C File Offset: 0x00067B4C
		public void reset()
		{
			this.widths = new List<float>();
			this.offsets = new List<float>();
			this.delays = new List<float>();
			this.complete = new List<float>();
			float num = 0f;
			bool flag = false;
			while (num < (float)this.maxWidth)
			{
				if ((double)num + 8.0 >= (double)this.maxWidth)
				{
					this.widths.Add((float)this.maxWidth - num);
					this.delays.Add(0f);
					this.complete.Add(0f);
					num = (float)this.maxWidth;
				}
				else
				{
					float num2 = (float)(Utils.random.NextDouble() * (flag ? 2.0 : 8.0));
					if (flag)
					{
						this.offsets.Add(num2);
					}
					else
					{
						this.widths.Add(num2);
						this.complete.Add(0f);
						if (this.leftRightBias)
						{
							float num3 = num / (float)this.maxWidth;
							this.delays.Add((float)((Utils.random.NextDouble() / 2.0 + (double)(num3 / 2f)) * (3.0 * (double)num3)));
						}
						else
						{
							this.delays.Add((float)(Utils.random.NextDouble() * 3.0));
						}
					}
					num += num2;
				}
				flag = !flag;
			}
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00069AEC File Offset: 0x00067CEC
		public void Update(float t)
		{
			bool flag = true;
			for (int i = 0; i < this.delays.Count; i++)
			{
				if (this.delays[i] > 0f)
				{
					List<float> list;
					int index;
					(list = this.delays)[index = i] = list[index] - t;
					if (this.delays[i] < 0f)
					{
						this.complete[i] = -this.delays[i] / 4f;
						this.delays[i] = 0f;
					}
					flag = false;
				}
				else
				{
					List<float> list;
					int index;
					(list = this.complete)[index = i] = list[index] + t / 4f;
					if (this.complete[i] > 1f)
					{
						this.complete[i] = 1f;
					}
					else
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				this.completeTime += t;
				if (this.completeTime > 0.9f)
				{
					this.reset();
					this.completeTime = 0f;
				}
			}
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00069C3C File Offset: 0x00067E3C
		public void Draw(int x, int y, int maxWidth, int maxHeight, SpriteBatch sb, Color? barColor = null)
		{
			Color color = (barColor != null) ? barColor.Value : Color.White;
			color *= (float)Math.Pow((double)(1f - this.completeTime / 0.9f), 3.0);
			Vector2 position = new Vector2((float)x, (float)y);
			float num = 0f;
			for (int i = 0; i < this.widths.Count; i++)
			{
				float num2 = (float)maxHeight;
				if (this.LeftRightMaxSizeFalloff)
				{
					int num3 = Math.Abs(i - this.widths.Count / 2);
					float num4 = 1f - Utils.QuadraticOutCurve((float)num3 / ((float)this.widths.Count / 2f));
					num2 = (float)maxHeight * num4;
				}
				position.X = (float)x + num;
				position.Y = (float)y;
				float x2 = this.widths[i];
				float num5;
				if (this.delays[i] > 0f)
				{
					num5 = 0f;
				}
				else
				{
					num5 = (float)((double)num2 * Math.Pow((double)this.complete[i], 0.5));
				}
				if (this.isInverted)
				{
					position.Y = (float)(y + maxHeight) - num5;
				}
				sb.Draw(Utils.white, position, null, color, 0f, Vector2.Zero, new Vector2(x2, num5), SpriteEffects.None, 0.5f);
				num += this.widths[i];
				if (this.offsets.Count > i)
				{
					num += this.offsets[i];
				}
			}
		}

		// Token: 0x04000710 RID: 1808
		private const double MAX_WIDTH = 8.0;

		// Token: 0x04000711 RID: 1809
		private const double MAX_OFFSET = 2.0;

		// Token: 0x04000712 RID: 1810
		private const double MAX_DELAY = 3.0;

		// Token: 0x04000713 RID: 1811
		private const float COMPLETE_TIME = 4f;

		// Token: 0x04000714 RID: 1812
		private const float COMPLETE_TIME_DELAY = 0.9f;

		// Token: 0x04000715 RID: 1813
		private List<float> widths;

		// Token: 0x04000716 RID: 1814
		private List<float> offsets;

		// Token: 0x04000717 RID: 1815
		private List<float> delays;

		// Token: 0x04000718 RID: 1816
		private List<float> complete;

		// Token: 0x04000719 RID: 1817
		private float completeTime = 0f;

		// Token: 0x0400071A RID: 1818
		public int maxWidth;

		// Token: 0x0400071B RID: 1819
		public bool isInverted = false;

		// Token: 0x0400071C RID: 1820
		public bool leftRightBias = false;

		// Token: 0x0400071D RID: 1821
		public bool LeftRightMaxSizeFalloff = false;
	}
}
