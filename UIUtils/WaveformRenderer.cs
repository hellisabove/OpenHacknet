using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x0200017F RID: 383
	internal class WaveformRenderer
	{
		// Token: 0x0600098C RID: 2444 RVA: 0x0009D318 File Offset: 0x0009B518
		public WaveformRenderer(string filename)
		{
			AudioUtils.openWav(filename, out this.left, out this.right);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x0009D340 File Offset: 0x0009B540
		public void RenderWaveform(double time, double totalTime, SpriteBatch sb, Rectangle bounds)
		{
			time %= totalTime;
			this.SecondBlockSize = (int)((double)this.left.Length / totalTime);
			int num = this.SecondBlockSize / 100;
			int num2 = (int)(time * (double)this.SecondBlockSize);
			int num3 = Math.Min(this.left.Length - 1, num2 + num);
			int num4 = num3 - num2;
			double num5 = (double)bounds.Width / (double)num4;
			int num6 = 0;
			for (int i = 0; i < num4; i++)
			{
				double num7 = this.left[num2 + i];
				if (num7 == 0.0)
				{
					num6++;
				}
				double num8 = num7 * (double)bounds.Height;
				Rectangle destinationRectangle = new Rectangle((int)((double)bounds.X + (double)i * num5), bounds.Y + bounds.Height / 2 - (int)(num8 / 2.0), (int)num5, (int)num8);
				sb.Draw(Utils.white, destinationRectangle, Color.White * 0.4f);
			}
		}

		// Token: 0x04000B1B RID: 2843
		private double[] left;

		// Token: 0x04000B1C RID: 2844
		private double[] right;

		// Token: 0x04000B1D RID: 2845
		private int SecondBlockSize = 100;
	}
}
