using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x02000058 RID: 88
	public class ShiftingGridEffect
	{
		// Token: 0x060001AC RID: 428 RVA: 0x000174A9 File Offset: 0x000156A9
		public ShiftingGridEffect()
		{
		}

		// Token: 0x060001AD RID: 429 RVA: 0x000174C3 File Offset: 0x000156C3
		public ShiftingGridEffect(int patternWidth, int patternHeight)
		{
			this.themeGrid = new ShiftingGridEffect.ShiftingGridSpot[patternWidth, patternHeight];
		}

		// Token: 0x060001AE RID: 430 RVA: 0x000174EC File Offset: 0x000156EC
		public void ResetThemeGrid()
		{
			for (int i = 0; i < this.themeGrid.GetLength(0); i++)
			{
				for (int j = 0; j < this.themeGrid.GetLength(1); j++)
				{
					this.ResetGridPoint(j, i);
				}
			}
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00017540 File Offset: 0x00015740
		public void ResetGridPoint(int x, int y)
		{
			this.themeGrid[y, x] = new ShiftingGridEffect.ShiftingGridSpot
			{
				from = this.themeGrid[y, x].to,
				to = Utils.randm(2f),
				time = 0f,
				totalTime = Utils.randm(3f) + 1.2f
			};
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x000175B8 File Offset: 0x000157B8
		public void Update(float t)
		{
			for (int i = 0; i < this.themeGrid.GetLength(0); i++)
			{
				for (int j = 0; j < this.themeGrid.GetLength(1); j++)
				{
					ShiftingGridEffect.ShiftingGridSpot shiftingGridSpot = this.themeGrid[i, j];
					shiftingGridSpot.time += t;
					if (shiftingGridSpot.time >= shiftingGridSpot.totalTime)
					{
						this.ResetGridPoint(j, i);
					}
					else
					{
						this.themeGrid[i, j] = shiftingGridSpot;
					}
				}
			}
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00017658 File Offset: 0x00015858
		public void RenderGrid(Rectangle bounds, SpriteBatch sb, Color c1, Color c2, Color c3, bool centreEffect = false)
		{
			int num = 12;
			if (centreEffect)
			{
				bounds.X -= bounds.Width % num;
			}
			int x = bounds.X + bounds.Width - num - 1;
			Rectangle destinationRectangle = new Rectangle(x, bounds.Y + 1, num, num);
			int num3;
			int num2 = num3 = 0;
			while (destinationRectangle.Y + 1 < bounds.Y + bounds.Height)
			{
				if (destinationRectangle.Y + num + 1 >= bounds.Y + bounds.Height)
				{
					destinationRectangle.Height = bounds.Y + bounds.Height - (destinationRectangle.Y + 2);
				}
				while (destinationRectangle.X - num > bounds.X + bounds.Width - bounds.Width - 1)
				{
					ShiftingGridEffect.ShiftingGridSpot shiftingGridSpot = this.themeGrid[num3 % this.themeGrid.GetLength(0), num2 % this.themeGrid.GetLength(1)];
					float num4 = shiftingGridSpot.time / shiftingGridSpot.totalTime;
					float num5 = shiftingGridSpot.from + num4 * (shiftingGridSpot.to - shiftingGridSpot.from);
					Color value = c3;
					Color value2 = c2;
					if (num5 >= 1f)
					{
						num5 -= 1f;
						value = c2;
						value2 = c1;
					}
					Color color = Color.Lerp(value, value2, num5);
					sb.Draw(Utils.white, destinationRectangle, color);
					num3++;
					destinationRectangle.X -= num + 1;
				}
				num2++;
				destinationRectangle.X = x;
				destinationRectangle.Y += num + 1;
			}
		}

		// Token: 0x040001C5 RID: 453
		private ShiftingGridEffect.ShiftingGridSpot[,] themeGrid = new ShiftingGridEffect.ShiftingGridSpot[20, 100];

		// Token: 0x02000059 RID: 89
		private struct ShiftingGridSpot
		{
			// Token: 0x040001C6 RID: 454
			public float from;

			// Token: 0x040001C7 RID: 455
			public float to;

			// Token: 0x040001C8 RID: 456
			public float time;

			// Token: 0x040001C9 RID: 457
			public float totalTime;
		}
	}
}
