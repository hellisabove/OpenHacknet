using System;

namespace Hacknet
{
	// Token: 0x02000125 RID: 293
	public struct InputMap
	{
		// Token: 0x060006E8 RID: 1768 RVA: 0x000720E2 File Offset: 0x000702E2
		public InputMap(InputStates last, InputStates now)
		{
			this.last = last;
			this.now = now;
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060006E9 RID: 1769 RVA: 0x000720F4 File Offset: 0x000702F4
		public static InputMap Empty
		{
			get
			{
				InputMap inputMap = InputMap.e;
				bool flag = 0 == 0;
				return InputMap.e;
			}
		}

		// Token: 0x060006EA RID: 1770 RVA: 0x00072118 File Offset: 0x00070318
		public static bool operator ==(InputMap self, InputMap other)
		{
			bool flag = 0 == 0;
			return self.now == other.now && self.last == other.last;
		}

		// Token: 0x060006EB RID: 1771 RVA: 0x0007216C File Offset: 0x0007036C
		public static bool operator !=(InputMap self, InputMap other)
		{
			bool flag = 0 == 0;
			return !(self.now == other.now) || !(self.last == other.last);
		}

		// Token: 0x060006EC RID: 1772 RVA: 0x000721C0 File Offset: 0x000703C0
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		// Token: 0x060006ED RID: 1773 RVA: 0x000721E4 File Offset: 0x000703E4
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x040007C1 RID: 1985
		public InputStates now;

		// Token: 0x040007C2 RID: 1986
		public InputStates last;

		// Token: 0x040007C3 RID: 1987
		private static InputMap e = default(InputMap);
	}
}
