using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000D7 RID: 215
	public class ThinBarcode
	{
		// Token: 0x06000450 RID: 1104 RVA: 0x000455DC File Offset: 0x000437DC
		public ThinBarcode(int w, int height)
		{
			this.oldW = w;
			this.height = height;
			this.regenerate();
			this.widthsLast = this.widths;
			this.gapsLast = this.gaps;
		}

		// Token: 0x06000451 RID: 1105 RVA: 0x0004564C File Offset: 0x0004384C
		public void regenerate()
		{
			int num = this.oldW;
			this.widthsLast = this.widths;
			this.gapsLast = this.gaps;
			this.widths.Clear();
			this.gaps.Clear();
			int i = 0;
			while (i < num)
			{
				int num2 = Math.Min(num - i, Utils.random.Next(1, 20));
				if (num2 > 0)
				{
					int num3 = Utils.random.Next(3, 11);
					this.widths.Add(num2);
					this.gaps.Add(num3);
					i += num2 + num3;
				}
			}
		}

		// Token: 0x06000452 RID: 1106 RVA: 0x000456F4 File Offset: 0x000438F4
		public void Draw(SpriteBatch sb, int posX, int posY, Color c)
		{
			Rectangle destinationRectangle = new Rectangle(posX, posY, 0, this.height);
			for (int i = 0; i < this.widths.Count; i++)
			{
				destinationRectangle.Width = this.widths[i];
				sb.Draw(Utils.white, destinationRectangle, c);
				destinationRectangle.X += this.widths[i];
				destinationRectangle.X += this.gaps[i];
			}
		}

		// Token: 0x0400052E RID: 1326
		private List<int> widths = new List<int>();

		// Token: 0x0400052F RID: 1327
		private List<int> gaps = new List<int>();

		// Token: 0x04000530 RID: 1328
		private List<int> widthsLast = new List<int>();

		// Token: 0x04000531 RID: 1329
		private List<int> gapsLast = new List<int>();

		// Token: 0x04000532 RID: 1330
		private int height;

		// Token: 0x04000533 RID: 1331
		private int oldW;
	}
}
