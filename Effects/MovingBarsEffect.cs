using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000AF RID: 175
	public class MovingBarsEffect
	{
		// Token: 0x06000391 RID: 913 RVA: 0x000366FC File Offset: 0x000348FC
		public void Update(float dt)
		{
			for (int i = 0; i < this.Lines.Count; i++)
			{
				MovingBarsEffect.BarLine value = this.Lines[i];
				value.TimeRemaining -= dt;
				if (value.TimeRemaining <= 0f)
				{
					value.Current = value.Next;
					value.Next = Utils.randm(1f);
					value.TotalTimeThisStep = this.MinLineChangeTime + Utils.randm(this.MaxLineChangeTime - this.MinLineChangeTime);
					value.TimeRemaining = value.TotalTimeThisStep;
				}
				this.Lines[i] = value;
			}
		}

		// Token: 0x06000392 RID: 914 RVA: 0x000367B8 File Offset: 0x000349B8
		public void Draw(SpriteBatch sb, Rectangle bounds, float minHeight, float lineWidth, float lineSeperation, Color drawColor)
		{
			int num = 0;
			float num2 = 0f;
			while (num2 + lineWidth < (float)bounds.Width)
			{
				num++;
				num2 += lineWidth + lineSeperation;
			}
			bool flag = false;
			while (this.Lines.Count - 1 < num)
			{
				this.Lines.Add(new MovingBarsEffect.BarLine
				{
					TimeRemaining = -1f
				});
				flag = true;
			}
			if (flag)
			{
				this.Update(0f);
			}
			num2 = (float)bounds.X;
			for (int i = 0; i < num; i++)
			{
				MovingBarsEffect.BarLine barLine = this.Lines[i];
				float num3 = 1f - barLine.TimeRemaining / barLine.TotalTimeThisStep;
				num3 = Utils.QuadraticOutCurve(num3);
				float num4 = barLine.Current + (barLine.Next - barLine.Current) * num3;
				float num5 = (float)bounds.Height - minHeight;
				int num6 = (int)(minHeight + num5 * num4);
				Rectangle destinationRectangle = new Rectangle((int)num2, bounds.Y + bounds.Height - num6, (int)lineWidth, num6);
				if (this.IsInverted)
				{
					destinationRectangle.Y = bounds.Y;
				}
				sb.Draw(Utils.white, destinationRectangle, drawColor);
				num2 += lineWidth + lineSeperation;
			}
		}

		// Token: 0x0400040F RID: 1039
		private List<MovingBarsEffect.BarLine> Lines = new List<MovingBarsEffect.BarLine>();

		// Token: 0x04000410 RID: 1040
		public float MinLineChangeTime = 0.2f;

		// Token: 0x04000411 RID: 1041
		public float MaxLineChangeTime = 2f;

		// Token: 0x04000412 RID: 1042
		public bool IsInverted = false;

		// Token: 0x020000B0 RID: 176
		private struct BarLine
		{
			// Token: 0x04000413 RID: 1043
			public float Current;

			// Token: 0x04000414 RID: 1044
			public float Next;

			// Token: 0x04000415 RID: 1045
			public float TimeRemaining;

			// Token: 0x04000416 RID: 1046
			public float TotalTimeThisStep;
		}
	}
}
