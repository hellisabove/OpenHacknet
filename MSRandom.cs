using System;

namespace Hacknet
{
	// Token: 0x02000082 RID: 130
	public class MSRandom
	{
		// Token: 0x0600029A RID: 666 RVA: 0x00026765 File Offset: 0x00024965
		public MSRandom() : this(Environment.TickCount)
		{
		}

		// Token: 0x0600029B RID: 667 RVA: 0x00026778 File Offset: 0x00024978
		public MSRandom(int Seed)
		{
			int num = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
			int num2 = 161803398 - num;
			this.SeedArray[55] = num2;
			int num3 = 1;
			for (int i = 1; i < 55; i++)
			{
				int num4 = 21 * i % 55;
				this.SeedArray[num4] = num3;
				num3 = num2 - num3;
				if (num3 < 0)
				{
					num3 += int.MaxValue;
				}
				num2 = this.SeedArray[num4];
			}
			for (int j = 1; j < 5; j++)
			{
				for (int i = 1; i < 56; i++)
				{
					this.SeedArray[i] -= this.SeedArray[1 + (i + 30) % 55];
					if (this.SeedArray[i] < 0)
					{
						this.SeedArray[i] += int.MaxValue;
					}
				}
			}
			this.inext = 0;
			this.inextp = 21;
			Seed = 1;
		}

		// Token: 0x0600029C RID: 668 RVA: 0x000268B8 File Offset: 0x00024AB8
		protected virtual double Sample()
		{
			return (double)this.InternalSample() * 4.656612875245797E-10;
		}

		// Token: 0x0600029D RID: 669 RVA: 0x000268DC File Offset: 0x00024ADC
		private int InternalSample()
		{
			int num = this.inext;
			int num2 = this.inextp;
			if (++num >= 56)
			{
				num = 1;
			}
			if (++num2 >= 56)
			{
				num2 = 1;
			}
			int num3 = this.SeedArray[num] - this.SeedArray[num2];
			if (num3 == 2147483647)
			{
				num3--;
			}
			if (num3 < 0)
			{
				num3 += int.MaxValue;
			}
			this.SeedArray[num] = num3;
			this.inext = num;
			this.inextp = num2;
			return num3;
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00026974 File Offset: 0x00024B74
		public virtual int Next()
		{
			return this.InternalSample();
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0002698C File Offset: 0x00024B8C
		private double GetSampleForLargeRange()
		{
			int num = this.InternalSample();
			bool flag = this.InternalSample() % 2 == 0;
			if (flag)
			{
				num = -num;
			}
			double num2 = (double)num;
			num2 += 2147483646.0;
			return num2 / 4294967293.0;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x000269E0 File Offset: 0x00024BE0
		public virtual int Next(int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException("minValue", "minValue out of range: " + minValue);
			}
			long num = (long)maxValue - (long)minValue;
			int result;
			if (num <= 2147483647L)
			{
				result = (int)(this.Sample() * (double)num) + minValue;
			}
			else
			{
				result = (int)((long)(this.GetSampleForLargeRange() * (double)num) + (long)minValue);
			}
			return result;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00026A4C File Offset: 0x00024C4C
		public virtual int Next(int maxValue)
		{
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue", "maxValue out of range: " + maxValue);
			}
			return (int)(this.Sample() * (double)maxValue);
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00026A90 File Offset: 0x00024C90
		public virtual double NextDouble()
		{
			return this.Sample();
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00026AA8 File Offset: 0x00024CA8
		public virtual void NextBytes(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = (byte)(this.InternalSample() % 256);
			}
		}

		// Token: 0x040002D2 RID: 722
		private const int MBIG = 2147483647;

		// Token: 0x040002D3 RID: 723
		private const int MSEED = 161803398;

		// Token: 0x040002D4 RID: 724
		private const int MZ = 0;

		// Token: 0x040002D5 RID: 725
		private int inext;

		// Token: 0x040002D6 RID: 726
		private int inextp;

		// Token: 0x040002D7 RID: 727
		private int[] SeedArray = new int[56];
	}
}
