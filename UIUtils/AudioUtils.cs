using System;
using System.IO;

namespace Hacknet.UIUtils
{
	// Token: 0x02000178 RID: 376
	internal class AudioUtils
	{
		// Token: 0x06000966 RID: 2406 RVA: 0x0009BFEC File Offset: 0x0009A1EC
		public static void openWav(string filename, out double[] left, out double[] right)
		{
			byte[] array = File.ReadAllBytes(filename);
			int num = (int)array[22];
			int i = 12;
			while (array[i] != 100 || array[i + 1] != 97 || array[i + 2] != 116 || array[i + 3] != 97)
			{
				i += 4;
				int num2 = (int)array[i] + (int)array[i + 1] * 256 + (int)array[i + 2] * 65536 + (int)array[i + 3] * 16777216;
				i += 4 + num2;
			}
			i += 8;
			int num3 = (array.Length - i) / 2;
			if (num == 2)
			{
				num3 /= 2;
			}
			left = new double[num3];
			if (num == 2)
			{
				right = new double[num3];
			}
			else
			{
				right = null;
			}
			int num4 = 0;
			while (i < array.Length)
			{
				left[num4] = AudioUtils.bytesToDouble(array[i], array[i + 1]);
				i += 2;
				if (num == 2)
				{
					right[num4] = AudioUtils.bytesToDouble(array[i], array[i + 1]);
					i += 2;
				}
				num4++;
			}
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x0009C108 File Offset: 0x0009A308
		private static double bytesToDouble(byte firstByte, byte secondByte)
		{
			short num = (short)((int)secondByte << 8 | (int)firstByte);
			return (double)num / 32768.0;
		}
	}
}
