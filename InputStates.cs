using System;

namespace Hacknet
{
	// Token: 0x02000124 RID: 292
	public struct InputStates
	{
		// Token: 0x060006E4 RID: 1764 RVA: 0x00071F74 File Offset: 0x00070174
		public static bool operator ==(InputStates self, InputStates other)
		{
			return self.movement == other.movement && self.jumping == other.jumping && self.useItem == other.useItem && self.wmovement == other.wmovement && self.wjumping == other.wjumping && self.wuseItem == other.wuseItem;
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x00071FF8 File Offset: 0x000701F8
		public static bool operator !=(InputStates self, InputStates other)
		{
			return self.movement != other.movement || self.jumping != other.jumping || self.useItem != other.useItem || self.wmovement != other.wmovement || self.wjumping != other.wjumping || self.wuseItem != other.wuseItem;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0007207C File Offset: 0x0007027C
		public override bool Equals(object obj)
		{
			return obj is InputStates && (InputStates)obj == this;
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x000720C0 File Offset: 0x000702C0
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x040007BB RID: 1979
		public float movement;

		// Token: 0x040007BC RID: 1980
		public bool jumping;

		// Token: 0x040007BD RID: 1981
		public bool useItem;

		// Token: 0x040007BE RID: 1982
		public float wmovement;

		// Token: 0x040007BF RID: 1983
		public bool wjumping;

		// Token: 0x040007C0 RID: 1984
		public bool wuseItem;
	}
}
