using System;

namespace Hacknet.Input
{
	// Token: 0x020000FF RID: 255
	public class Stack<T>
	{
		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000592 RID: 1426 RVA: 0x00057D40 File Offset: 0x00055F40
		public int Capacity
		{
			get
			{
				return this.stack.Length;
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000593 RID: 1427 RVA: 0x00057D5C File Offset: 0x00055F5C
		// (set) Token: 0x06000594 RID: 1428 RVA: 0x00057D73 File Offset: 0x00055F73
		public int Count { get; private set; }

		// Token: 0x06000595 RID: 1429 RVA: 0x00057D7C File Offset: 0x00055F7C
		public Stack() : this(32)
		{
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00057D8C File Offset: 0x00055F8C
		public Stack(int capacity)
		{
			if (capacity < 0)
			{
				capacity = 0;
			}
			this.stack = new T[capacity];
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00057DBC File Offset: 0x00055FBC
		public void Push(ref T item)
		{
			if (this.Count == this.stack.Length)
			{
				T[] destinationArray = new T[this.stack.Length << 1];
				Array.Copy(this.stack, 0, destinationArray, 0, this.stack.Length);
				this.stack = destinationArray;
			}
			this.stack[this.Count] = item;
			this.Count++;
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00057E38 File Offset: 0x00056038
		public void Pop(out T item)
		{
			if (this.Count <= 0)
			{
				throw new InvalidOperationException();
			}
			item = this.stack[this.Count];
			this.stack[this.Count] = default(T);
			this.Count--;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x00057E99 File Offset: 0x00056099
		public void PopSegment(out ArraySegment<T> segment)
		{
			segment = new ArraySegment<T>(this.stack, 0, this.Count);
			this.Count = 0;
		}

		// Token: 0x04000654 RID: 1620
		private T[] stack;
	}
}
