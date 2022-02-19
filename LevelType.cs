using System;

namespace Hacknet
{
	// Token: 0x02000185 RID: 389
	public struct LevelType
	{
		// Token: 0x060009A1 RID: 2465 RVA: 0x0009D61F File Offset: 0x0009B81F
		public LevelType(int puzzles, int bgs, string lvlname)
		{
			this.NumOfPuzzles = puzzles;
			this.NumOfBackgrounds = bgs;
			this.name = lvlname;
		}

		// Token: 0x04000B2E RID: 2862
		public int NumOfPuzzles;

		// Token: 0x04000B2F RID: 2863
		public int NumOfBackgrounds;

		// Token: 0x04000B30 RID: 2864
		public string name;
	}
}
