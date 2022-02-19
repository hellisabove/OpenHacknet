using System;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000AD RID: 173
	public class HexGridBackground
	{
		// Token: 0x0600038E RID: 910 RVA: 0x0003641C File Offset: 0x0003461C
		public HexGridBackground(ContentManager content)
		{
			this.Hex = content.Load<Texture2D>("Sprites/Misc/Hexagon");
			this.Outline = content.Load<Texture2D>("Sprites/Misc/HexOutline");
		}

		// Token: 0x0600038F RID: 911 RVA: 0x0003647D File Offset: 0x0003467D
		public void Update(float dt)
		{
			this.timer += dt;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00036490 File Offset: 0x00034690
		public void Draw(Rectangle dest, SpriteBatch sb, Color first, Color second, HexGridBackground.ColoringAlgorithm algorithm = HexGridBackground.ColoringAlgorithm.CorrectedSinWash, float angle = 0f)
		{
			float num = 2f;
			float num2 = (float)dest.X;
			float num3 = (float)dest.Y;
			float num4 = this.HexScale * (float)this.Hex.Width;
			float num5 = this.HexScale * (float)this.Hex.Height;
			int num6 = 10;
			bool flag = true;
			while (num2 + num4 < (float)(dest.X + dest.Width))
			{
				while (num3 + num5 < (float)(dest.Y + dest.Height))
				{
					this.lcg.reSeed(num6);
					float num7 = this.lcg.NextFloat();
					bool flag2 = false;
					Color value = second;
					if (this.HasRedFlashyOnes && num7 <= 0.001f)
					{
						value = Utils.AddativeRed * 0.5f;
						flag2 = true;
					}
					float amount;
					switch (algorithm)
					{
					case HexGridBackground.ColoringAlgorithm.NegaitiveSinWash:
						amount = (float)Math.Sin((double)(num7 + this.timer * this.lcg.NextFloat()));
						break;
					case HexGridBackground.ColoringAlgorithm.CorrectedSinWash:
					case HexGridBackground.ColoringAlgorithm.OutlinedSinWash:
						goto IL_E8;
					default:
						goto IL_E8;
					}
					IL_124:
					sb.Draw(this.Hex, Utils.RotatePoint(new Vector2(num2, num3), angle), null, Color.Lerp(first, value, amount), angle, Vector2.Zero, new Vector2(this.HexScale), SpriteEffects.None, 0.4f);
					if (algorithm == HexGridBackground.ColoringAlgorithm.OutlinedSinWash)
					{
						if (flag2)
						{
							value = Utils.AddativeRed;
						}
						sb.Draw(this.Outline, Utils.RotatePoint(new Vector2(num2, num3), angle), null, Color.Lerp(first, value, amount), angle, Vector2.Zero, new Vector2(this.HexScale), SpriteEffects.None, 0.4f);
					}
					num6++;
					num3 += num5 + num;
					continue;
					IL_E8:
					amount = Math.Abs((float)Math.Sin((double)(num7 + this.timer * Math.Abs(this.lcg.NextFloat() * (flag2 ? 1f : 0.3f)))));
					goto IL_124;
				}
				num2 += num4 - 60f * this.HexScale + num;
				num3 = (float)dest.Y;
				num6++;
				if (flag)
				{
					num3 -= num5 / 2f;
				}
				flag = !flag;
			}
		}

		// Token: 0x04000405 RID: 1029
		private Texture2D Hex;

		// Token: 0x04000406 RID: 1030
		private Texture2D Outline;

		// Token: 0x04000407 RID: 1031
		private float timer = 0f;

		// Token: 0x04000408 RID: 1032
		private LCG lcg = new LCG(false);

		// Token: 0x04000409 RID: 1033
		public bool HasRedFlashyOnes = false;

		// Token: 0x0400040A RID: 1034
		public float HexScale = 0.1f;

		// Token: 0x020000AE RID: 174
		public enum ColoringAlgorithm
		{
			// Token: 0x0400040C RID: 1036
			NegaitiveSinWash,
			// Token: 0x0400040D RID: 1037
			CorrectedSinWash,
			// Token: 0x0400040E RID: 1038
			OutlinedSinWash
		}
	}
}
