using System;
using System.Collections.Generic;
using System.Text;

namespace Hacknet
{
	// Token: 0x0200018C RID: 396
	public static class VehicleInfo
	{
		// Token: 0x06000A0D RID: 2573 RVA: 0x000A125C File Offset: 0x0009F45C
		public static void init()
		{
			string text = Utils.readEntireFile("Content/files/VehicleTypes.txt");
			string[] array = text.Split(Utils.newlineDelim);
			char[] separator = new char[]
			{
				'#'
			};
			VehicleInfo.vehicleTypes = new List<VehicleType>(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator);
				VehicleInfo.vehicleTypes.Add(new VehicleType(array2[0], array2[1]));
			}
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x000A12DC File Offset: 0x0009F4DC
		public static VehicleRegistration getRandomRegistration()
		{
			int index = Utils.random.Next(VehicleInfo.vehicleTypes.Count);
			VehicleType vehicleType = VehicleInfo.vehicleTypes[index];
			string plate = string.Concat(new object[]
			{
				(int)(Utils.getRandomLetter() + Utils.getRandomLetter() + Utils.getRandomLetter()),
				"-",
				Utils.getRandomLetter(),
				Utils.getRandomLetter(),
				Utils.getRandomLetter()
			});
			StringBuilder stringBuilder = new StringBuilder();
			int num = 12;
			int num2 = 4;
			for (int i = 0; i < num; i++)
			{
				if (i % num2 == 0 && i > 0)
				{
					stringBuilder.Append('-');
				}
				else
				{
					stringBuilder.Append(Utils.getRandomChar());
				}
			}
			string regNumber = stringBuilder.ToString();
			return new VehicleRegistration(vehicleType, plate, regNumber);
		}

		// Token: 0x04000B60 RID: 2912
		public static List<VehicleType> vehicleTypes;
	}
}
