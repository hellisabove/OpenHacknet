using System;

namespace Hacknet
{
	// Token: 0x0200018B RID: 395
	public class VehicleType
	{
		// Token: 0x06000A0B RID: 2571 RVA: 0x000A1235 File Offset: 0x0009F435
		public VehicleType()
		{
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x000A1240 File Offset: 0x0009F440
		public VehicleType(string vModel, string vMaker)
		{
			this.model = vModel;
			this.maker = vMaker;
		}

		// Token: 0x04000B5E RID: 2910
		public string model;

		// Token: 0x04000B5F RID: 2911
		public string maker;
	}
}
