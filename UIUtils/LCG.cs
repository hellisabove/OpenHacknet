using System;
using System.Collections.Generic;

namespace Hacknet.UIUtils
{
	// Token: 0x0200017C RID: 380
	public class LCG
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000972 RID: 2418 RVA: 0x0009C7B8 File Offset: 0x0009A9B8
		// (set) Token: 0x06000973 RID: 2419 RVA: 0x0009C7CF File Offset: 0x0009A9CF
		public bool Microsoft { get; set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000974 RID: 2420 RVA: 0x0009C7D8 File Offset: 0x0009A9D8
		// (set) Token: 0x06000975 RID: 2421 RVA: 0x0009C7F3 File Offset: 0x0009A9F3
		public bool BSD
		{
			get
			{
				return !this.Microsoft;
			}
			set
			{
				this.Microsoft = !value;
			}
		}

		// Token: 0x06000976 RID: 2422 RVA: 0x0009C804 File Offset: 0x0009AA04
		public LCG(bool microsoft = true)
		{
			this._state = (int)DateTime.Now.Ticks;
			this.Microsoft = microsoft;
		}

		// Token: 0x06000977 RID: 2423 RVA: 0x0009C836 File Offset: 0x0009AA36
		public LCG(int n, bool microsoft = true)
		{
			this._state = n;
			this.Microsoft = microsoft;
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x0009C850 File Offset: 0x0009AA50
		public void reSeed(int seed)
		{
			this._state = seed;
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x0009C85C File Offset: 0x0009AA5C
		public int Next()
		{
			int result;
			if (this.BSD)
			{
				result = (this._state = (1103515245 * this._state + 12345 & int.MaxValue));
			}
			else
			{
				result = ((this._state = 214013 * this._state + 2531011) & int.MaxValue) >> 16;
			}
			return result;
		}

		// Token: 0x0600097A RID: 2426 RVA: 0x0009C8C4 File Offset: 0x0009AAC4
		public float NextFloat()
		{
			return (float)((double)this.Next() / 2147483647.0);
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x0009C8E8 File Offset: 0x0009AAE8
		public float NextFloatScaled()
		{
			return (float)((double)this.Next() / 32767.0);
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x0009C90C File Offset: 0x0009AB0C
		public bool Flip()
		{
			bool result;
			if (this.BSD)
			{
				result = ((this._state = (1103515245 * this._state + 12345 & int.MaxValue)) > 1073741823);
			}
			else
			{
				result = (((this._state = 214013 * this._state + 2531011) & int.MaxValue) >> 16 > 1073741823);
			}
			return result;
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x0009CAB8 File Offset: 0x0009ACB8
		public IEnumerable<int> Seq()
		{
			for (;;)
			{
				yield return this.Next();
			}
			yield break;
		}

		// Token: 0x04000B0E RID: 2830
		private int _state;
	}
}
