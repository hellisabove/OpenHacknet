using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000FA RID: 250
	internal class ConnectedNodeEffect
	{
		// Token: 0x0600055F RID: 1375 RVA: 0x00054AA4 File Offset: 0x00052CA4
		public ConnectedNodeEffect(OS os)
		{
			this.os = os;
			this.init(false);
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00054AD0 File Offset: 0x00052CD0
		public ConnectedNodeEffect(OS os, bool intense)
		{
			this.os = os;
			this.Intense = intense;
			this.init(intense);
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00054B04 File Offset: 0x00052D04
		private void init(bool intesne = false)
		{
			if (ConnectedNodeEffect.textures == null)
			{
				ConnectedNodeEffect.textures = new List<Texture2D>();
				ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator"));
				ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator2"));
				ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator3"));
				ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator4"));
				ConnectedNodeEffect.textures.Add(this.os.content.Load<Texture2D>("rotator5"));
			}
			this.origin = new Vector2((float)(ConnectedNodeEffect.textures[0].Width / 2), (float)(ConnectedNodeEffect.textures[0].Height / 2));
			int num = (intesne ? 2 : 1) * 7;
			this.tex = new int[num];
			this.distance = new float[num];
			this.offset = new float[num];
			this.timescale = new float[num];
			this.color = new Color(140, 12, 12, 50);
			this.reset();
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00054C5C File Offset: 0x00052E5C
		public void reset()
		{
			float num = this.Intense ? 1.5f : 1f;
			for (int i = 0; i < 7; i++)
			{
				this.tex[i] = Utils.random.Next(ConnectedNodeEffect.textures.Count - 1);
				this.distance[i] = (float)(Utils.random.NextDouble() * (double)(30f * num - 18f / num) + (double)(18f / num));
				this.offset[i] = (float)(Utils.random.NextDouble() * 6.283185307179586);
				this.timescale[i] = (float)Utils.random.NextDouble();
			}
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00054D14 File Offset: 0x00052F14
		public void draw(SpriteBatch sb, Vector2 pos)
		{
			for (int i = 0; i < 7; i++)
			{
				Vector2 vector = this.origin + new Vector2(0f, this.distance[i]);
				float rotation = (float)((double)(this.os.timer * (this.timescale[i] * 0.2f) % 1f) * 6.283185307179586 * (double)((i % 2 == 0) ? 1f : -1f)) + this.offset[i];
				Vector2 vector2 = new Vector2(this.distance[i] / (30f * (this.Intense ? 1.5f : 1f)) * 0.7f + 0.3f, 1f);
				vector2 *= this.ScaleFactor;
				sb.Draw(ConnectedNodeEffect.textures[this.tex[i]], pos, null, this.color, rotation, vector, vector2, SpriteEffects.None, 0.5f);
			}
		}

		// Token: 0x04000617 RID: 1559
		private const int NUMBER_OF_SEGMENTS = 7;

		// Token: 0x04000618 RID: 1560
		private const float MIN_DISTANCE = 18f;

		// Token: 0x04000619 RID: 1561
		private const float MAX_DISTANCE = 30f;

		// Token: 0x0400061A RID: 1562
		private OS os;

		// Token: 0x0400061B RID: 1563
		private static List<Texture2D> textures;

		// Token: 0x0400061C RID: 1564
		public Color color;

		// Token: 0x0400061D RID: 1565
		public bool Intense = false;

		// Token: 0x0400061E RID: 1566
		public float ScaleFactor = 1f;

		// Token: 0x0400061F RID: 1567
		private int[] tex;

		// Token: 0x04000620 RID: 1568
		private float[] distance;

		// Token: 0x04000621 RID: 1569
		private float[] offset;

		// Token: 0x04000622 RID: 1570
		private float[] timescale;

		// Token: 0x04000623 RID: 1571
		private Vector2 origin;
	}
}
