using System;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E6 RID: 230
	public struct SaveAccountData
	{
		// Token: 0x060004BD RID: 1213 RVA: 0x0004A03C File Offset: 0x0004823C
		public static SaveAccountData ParseFromString(string input)
		{
			string[] array = input.Split(SaveAccountData.Delimiter, StringSplitOptions.None);
			string password = "";
			if (!string.IsNullOrEmpty(array[1]))
			{
				try
				{
					password = FileEncrypter.DecryptString(array[1], "")[2];
				}
				catch (FormatException ex)
				{
					Console.WriteLine("ACCOUNT AUTHENTICATION DETAILS CORRUPT : " + array[0]);
					password = "";
				}
				catch (NullReferenceException)
				{
					Console.WriteLine("ACCOUNT AUTHENTICATION DETAILS REMOVED OR DELETED : " + array[0]);
					password = "";
				}
			}
			SaveAccountData result;
			if (array.Length <= 3)
			{
				result = new SaveAccountData
				{
					Username = array[0],
					Password = password,
					FileUsername = array[2]
				};
			}
			else
			{
				result = new SaveAccountData
				{
					Username = array[0],
					Password = password,
					LastWriteTime = Utils.SafeParseDateTime(array[2]),
					FileUsername = array[3]
				};
			}
			return result;
		}

		// Token: 0x060004BE RID: 1214 RVA: 0x0004A14C File Offset: 0x0004834C
		public string Serialize()
		{
			string str = "";
			str = str + this.Username + SaveAccountData.Delimiter[0];
			str = str + FileEncrypter.EncryptString(this.Password, "", "", "", null) + SaveAccountData.Delimiter[0];
			str = str + Utils.SafeWriteDateTime(this.LastWriteTime) + SaveAccountData.Delimiter[0];
			return str + this.FileUsername;
		}

		// Token: 0x04000587 RID: 1415
		private static string[] Delimiter = new string[]
		{
			"\r\n__",
			"\n__"
		};

		// Token: 0x04000588 RID: 1416
		public string Username;

		// Token: 0x04000589 RID: 1417
		public string Password;

		// Token: 0x0400058A RID: 1418
		public string FileUsername;

		// Token: 0x0400058B RID: 1419
		public DateTime LastWriteTime;
	}
}
