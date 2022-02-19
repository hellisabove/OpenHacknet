using System;
using System.IO;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000180 RID: 384
	public static class UsernameGenerator
	{
		// Token: 0x0600098E RID: 2446 RVA: 0x0009D450 File Offset: 0x0009B650
		public static void init()
		{
			StreamReader streamReader = new StreamReader(TitleContainer.OpenStream("Content/Usernames.txt"));
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			UsernameGenerator.names = text.Split(UsernameGenerator.delims, StringSplitOptions.RemoveEmptyEntries);
			UsernameGenerator.nameIndex = (int)(Utils.random.NextDouble() * (double)(UsernameGenerator.names.Length - 1));
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0009D4A8 File Offset: 0x0009B6A8
		public static string getName()
		{
			UsernameGenerator.nameIndex = (UsernameGenerator.nameIndex + 1) % (UsernameGenerator.names.Length - 1);
			return UsernameGenerator.names[UsernameGenerator.nameIndex];
		}

		// Token: 0x04000B1E RID: 2846
		public static string[] names;

		// Token: 0x04000B1F RID: 2847
		private static string[] delims = new string[]
		{
			"\r\n\r\n"
		};

		// Token: 0x04000B20 RID: 2848
		private static int nameIndex;
	}
}
