using System;

namespace Hacknet.DLC.MarkovTextGenerator
{
	// Token: 0x02000024 RID: 36
	public class PotentialEntry
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x0000EDAC File Offset: 0x0000CFAC
		public override string ToString()
		{
			return this.word + " %" + this.weighting.ToString("0.00");
		}

		// Token: 0x040000FE RID: 254
		public string word;

		// Token: 0x040000FF RID: 255
		public double weighting;
	}
}
