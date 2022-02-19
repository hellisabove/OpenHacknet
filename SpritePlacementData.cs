using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000184 RID: 388
	public struct SpritePlacementData
	{
		// Token: 0x0600099F RID: 2463 RVA: 0x0009D5E0 File Offset: 0x0009B7E0
		public SpritePlacementData(Vector2 position, Vector2 scales, float layerDepth)
		{
			this.pos = position;
			this.scale = scales;
			this.depth = layerDepth;
			this.spriteIndex = 0;
		}

		// Token: 0x060009A0 RID: 2464 RVA: 0x0009D5FF File Offset: 0x0009B7FF
		public SpritePlacementData(Vector2 position, Vector2 scales, float layerDepth, int SpriteIndex)
		{
			this.pos = position;
			this.scale = scales;
			this.depth = layerDepth;
			this.spriteIndex = SpriteIndex;
		}

		// Token: 0x04000B2A RID: 2858
		public Vector2 pos;

		// Token: 0x04000B2B RID: 2859
		public float depth;

		// Token: 0x04000B2C RID: 2860
		public Vector2 scale;

		// Token: 0x04000B2D RID: 2861
		public int spriteIndex;
	}
}
