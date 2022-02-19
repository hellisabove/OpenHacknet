using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000147 RID: 327
	public static class PatternDrawer
	{
		// Token: 0x0600081B RID: 2075 RVA: 0x00087CD8 File Offset: 0x00085ED8
		public static void init(ContentManager content)
		{
			PatternDrawer.warningStripe = TextureBank.load("StripePattern", content);
			PatternDrawer.errorTile = TextureBank.load("ErrorTile", content);
			PatternDrawer.binaryTile = TextureBank.load("BinaryTile", content);
			PatternDrawer.thinStripe = TextureBank.load("ThinStripe", content);
			PatternDrawer.star = TextureBank.load("Sprites/Star", content);
			PatternDrawer.wipTile = TextureBank.load("Sprites/WIPTile", content);
		}

		// Token: 0x0600081C RID: 2076 RVA: 0x00087D46 File Offset: 0x00085F46
		public static void update(float t)
		{
			PatternDrawer.time += t;
		}

		// Token: 0x0600081D RID: 2077 RVA: 0x00087D55 File Offset: 0x00085F55
		public static void draw(Rectangle dest, float offset, Color backingColor, Color patternColor, SpriteBatch sb)
		{
			PatternDrawer.draw(dest, offset, backingColor, patternColor, sb, PatternDrawer.warningStripe);
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00087D6C File Offset: 0x00085F6C
		public static void draw(Rectangle dest, float offset, Color backingColor, Color patternColor, SpriteBatch sb, Texture2D tex)
		{
			int i = dest.X;
			int num = dest.Y;
			int num2 = Math.Min(dest.Height, tex.Height);
			int num3 = (int)(PatternDrawer.time * offset % 1f * (float)tex.Width);
			Rectangle empty = Rectangle.Empty;
			sb.Draw(Utils.white, dest, backingColor);
			Rectangle destinationRectangle;
			while (num - dest.Y + num2 <= dest.Height)
			{
				i = dest.X;
				destinationRectangle = new Rectangle(i, num, num3, num2);
				empty = new Rectangle(0, 0, num3, num2);
				empty.X = tex.Width - empty.Width;
				sb.Draw(tex, destinationRectangle, new Rectangle?(empty), patternColor);
				i += num3;
				destinationRectangle.X = i;
				destinationRectangle.Width = tex.Width;
				while (i <= dest.X + dest.Width - tex.Width)
				{
					Rectangle? sourceRectangle = null;
					if (dest.Height < tex.Height)
					{
						sourceRectangle = new Rectangle?(new Rectangle(0, 0, tex.Width, dest.Height));
					}
					sb.Draw(tex, destinationRectangle, sourceRectangle, patternColor);
					i += tex.Width;
					destinationRectangle.X = i;
				}
				destinationRectangle.X = i;
				destinationRectangle.Width = dest.X + dest.Width - i;
				empty.Width = destinationRectangle.Width;
				empty.X = 0;
				sb.Draw(tex, destinationRectangle, new Rectangle?(empty), patternColor);
				num += tex.Height;
				destinationRectangle.Y = num;
			}
			destinationRectangle.Height = dest.Height - (num - dest.Y);
			empty.Height = destinationRectangle.Height;
			i = dest.X;
			destinationRectangle.X = i;
			destinationRectangle.Y = num;
			destinationRectangle.Width = num3;
			empty.Width = num3;
			empty.X = tex.Width - empty.Width;
			sb.Draw(tex, destinationRectangle, new Rectangle?(empty), patternColor);
			i += num3;
			destinationRectangle.X = i;
			destinationRectangle.Width = tex.Width;
			empty.Width = tex.Width;
			empty.X = 0;
			while (i <= dest.X + dest.Width - tex.Width)
			{
				sb.Draw(tex, destinationRectangle, new Rectangle?(empty), patternColor);
				i += tex.Width;
				destinationRectangle.X = i;
			}
			destinationRectangle.X = i;
			destinationRectangle.Width = dest.X + dest.Width - i;
			empty.Width = destinationRectangle.Width;
			empty.X = 0;
			sb.Draw(tex, destinationRectangle, new Rectangle?(empty), patternColor);
			num += tex.Height;
			destinationRectangle.Y = num;
		}

		// Token: 0x0400099E RID: 2462
		public static Texture2D warningStripe;

		// Token: 0x0400099F RID: 2463
		public static Texture2D errorTile;

		// Token: 0x040009A0 RID: 2464
		public static Texture2D binaryTile;

		// Token: 0x040009A1 RID: 2465
		public static Texture2D thinStripe;

		// Token: 0x040009A2 RID: 2466
		public static Texture2D star;

		// Token: 0x040009A3 RID: 2467
		public static Texture2D wipTile;

		// Token: 0x040009A4 RID: 2468
		public static float time = 0f;
	}
}
