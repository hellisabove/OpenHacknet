using System;

namespace Hacknet
{
	// Token: 0x02000181 RID: 385
	public struct DelayedInput
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000991 RID: 2449 RVA: 0x0009D500 File Offset: 0x0009B700
		// (set) Token: 0x06000992 RID: 2450 RVA: 0x0009D517 File Offset: 0x0009B717
		public double Delay { get; set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000993 RID: 2451 RVA: 0x0009D520 File Offset: 0x0009B720
		// (set) Token: 0x06000994 RID: 2452 RVA: 0x0009D537 File Offset: 0x0009B737
		public InputMap inputs { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000995 RID: 2453 RVA: 0x0009D540 File Offset: 0x0009B740
		// (set) Token: 0x06000996 RID: 2454 RVA: 0x0009D557 File Offset: 0x0009B757
		public float xPos { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x0009D560 File Offset: 0x0009B760
		// (set) Token: 0x06000998 RID: 2456 RVA: 0x0009D577 File Offset: 0x0009B777
		public float yPos { get; set; }
	}
}
