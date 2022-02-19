using System;

namespace Hacknet.UIUtils
{
	// Token: 0x02000179 RID: 377
	public class CLinkBuffer<T>
	{
		// Token: 0x06000969 RID: 2409 RVA: 0x0009C135 File Offset: 0x0009A335
		public CLinkBuffer(int BufferSize = 128)
		{
			this.data = new T[BufferSize];
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x0009C154 File Offset: 0x0009A354
		public T Get(int offset)
		{
			int i;
			for (i = this.currentIndex + offset; i < 0; i += this.data.Length)
			{
			}
			while (i >= this.data.Length)
			{
				i -= this.data.Length;
			}
			return this.data[i];
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x0009C1AF File Offset: 0x0009A3AF
		public void Add(T added)
		{
			this.currentIndex = this.NextIndex();
			this.data[this.currentIndex] = added;
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x0009C1D0 File Offset: 0x0009A3D0
		public void AddOneAhead(T added)
		{
			this.data[this.NextIndex()] = added;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x0009C1E8 File Offset: 0x0009A3E8
		private int NextIndex()
		{
			int num = this.currentIndex + 1;
			if (num >= this.data.Length)
			{
				num = 0;
			}
			return num;
		}

		// Token: 0x04000B0C RID: 2828
		private T[] data;

		// Token: 0x04000B0D RID: 2829
		private int currentIndex = 0;
	}
}
