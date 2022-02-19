using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000B1 RID: 177
	public class NodeBounceEffect
	{
		// Token: 0x06000394 RID: 916 RVA: 0x00036958 File Offset: 0x00034B58
		public NodeBounceEffect()
		{
			this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
			this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
		}

		// Token: 0x06000395 RID: 917 RVA: 0x000369E8 File Offset: 0x00034BE8
		public void Update(float t, Action<Vector2> nodeHitAction = null)
		{
			this.timeToNextBounce -= t;
			if (this.timeToNextBounce <= 0f)
			{
				if (this.delayTillNextBounce <= 0f)
				{
					if (nodeHitAction != null)
					{
						nodeHitAction(this.locations[this.locations.Count - 1]);
					}
					this.locations.Add(new Vector2(Utils.rand(), Utils.rand()));
					this.timeToNextBounce = this.TimeBetweenBounces;
					this.delayTillNextBounce = this.NodeHitDelay;
					while (this.locations.Count > this.maxNodes)
					{
						this.locations.RemoveAt(0);
					}
				}
				else
				{
					this.delayTillNextBounce -= t;
					this.timeToNextBounce = 0f;
				}
			}
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00036AD0 File Offset: 0x00034CD0
		public void Draw(SpriteBatch spriteBatch, Rectangle bounds, Color lineColor, Color nodeColor)
		{
			Vector2 value = this.locations[0];
			Vector2 vector = new Vector2((float)bounds.X + 2f, (float)bounds.Y + 26f);
			Vector2 value2 = new Vector2((float)bounds.Width - 4f, (float)bounds.Height - 30f);
			if (value2.X > 0f && value2.Y > 0f)
			{
				for (int i = 1; i < this.locations.Count; i++)
				{
					Vector2 vector2 = this.locations[i];
					if (i == this.locations.Count - 1)
					{
						vector2 = Vector2.Lerp(value, vector2, 1f - this.timeToNextBounce / this.TimeBetweenBounces);
					}
					Utils.drawLine(spriteBatch, vector + value * value2, vector + vector2 * value2, Vector2.Zero, lineColor * ((float)i / (float)this.locations.Count), 0.4f);
					value = this.locations[i];
				}
				for (int i = 1; i < this.locations.Count; i++)
				{
					spriteBatch.Draw(Utils.white, this.locations[i] * value2 + vector, nodeColor);
				}
			}
		}

		// Token: 0x04000417 RID: 1047
		public float TimeBetweenBounces = 0.07f;

		// Token: 0x04000418 RID: 1048
		public float NodeHitDelay = 0.2f;

		// Token: 0x04000419 RID: 1049
		private List<Vector2> locations = new List<Vector2>();

		// Token: 0x0400041A RID: 1050
		private float timeToNextBounce = 0f;

		// Token: 0x0400041B RID: 1051
		private float delayTillNextBounce = 0f;

		// Token: 0x0400041C RID: 1052
		public int maxNodes = 200;
	}
}
