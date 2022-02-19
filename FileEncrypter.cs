using System;
using System.Text;

namespace Hacknet
{
	// Token: 0x02000163 RID: 355
	public static class FileEncrypter
	{
		// Token: 0x060008F1 RID: 2289 RVA: 0x0009489C File Offset: 0x00092A9C
		public static string EncryptString(string data, string header, string ipLink, string pass = "", string fileExtension = null)
		{
			if (string.IsNullOrWhiteSpace(data))
			{
				data = "";
			}
			ushort passCodeFromString = FileEncrypter.GetPassCodeFromString(pass);
			ushort passCodeFromString2 = FileEncrypter.GetPassCodeFromString("");
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Concat(new string[]
			{
				"#DEC_ENC::",
				FileEncrypter.Encrypt(header, passCodeFromString2),
				"::",
				FileEncrypter.Encrypt(ipLink, passCodeFromString2),
				"::",
				FileEncrypter.Encrypt("ENCODED", passCodeFromString)
			});
			if (fileExtension != null)
			{
				text = text + "::" + FileEncrypter.Encrypt(fileExtension, passCodeFromString2);
			}
			stringBuilder.Append(text);
			stringBuilder.Append("\r\n");
			stringBuilder.Append(FileEncrypter.Encrypt(data, passCodeFromString));
			return stringBuilder.ToString();
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x00094978 File Offset: 0x00092B78
		private static ushort GetPassCodeFromString(string code)
		{
			return (ushort)code.GetHashCode();
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00094994 File Offset: 0x00092B94
		private static string Encrypt(string data, ushort passcode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				int num = (int)(data[i] * 'ܞ' + '翿' + (char)passcode);
				stringBuilder.Append(num + " ");
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00094A00 File Offset: 0x00092C00
		private static string Decrypt(string data, ushort passcode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = data.Split(Utils.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				int num = Convert.ToInt32(array[i]);
				int num2 = 32767;
				int num3 = num - num2 - (int)passcode;
				num3 /= 1822;
				stringBuilder.Append((char)num3);
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00094A74 File Offset: 0x00092C74
		public static string[] DecryptString(string data, string pass = "")
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new NullReferenceException("String to decrypt cannot be null or empty");
			}
			string[] array = new string[6];
			ushort passCodeFromString = FileEncrypter.GetPassCodeFromString(pass);
			ushort passCodeFromString2 = FileEncrypter.GetPassCodeFromString("");
			string[] array2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < 2)
			{
				throw new FormatException(string.Concat(new object[]
				{
					"Tried to decrypt an invalid valid DEC ENC file \"",
					data,
					"\" - not enough elements. Need 2 lines, had ",
					array2.Length
				}));
			}
			string[] array3 = array2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
			if (array3.Length < 4)
			{
				throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough headers");
			}
			string text = FileEncrypter.Decrypt(array3[1], passCodeFromString2);
			string text2 = FileEncrypter.Decrypt(array3[2], passCodeFromString2);
			string text3 = FileEncrypter.Decrypt(array3[3], passCodeFromString);
			string text4 = null;
			if (array3.Length > 4)
			{
				text4 = FileEncrypter.Decrypt(array3[4], passCodeFromString2);
			}
			string text5 = null;
			string text6 = "1";
			if (text3 == "ENCODED")
			{
				text5 = FileEncrypter.Decrypt(array2[1], passCodeFromString);
			}
			else
			{
				text6 = "0";
			}
			array[0] = text;
			array[1] = text2;
			array[2] = text5;
			array[3] = text4;
			array[4] = text6;
			array[5] = text3;
			return array;
		}

		// Token: 0x060008F6 RID: 2294 RVA: 0x00094BE0 File Offset: 0x00092DE0
		internal static string[] TestingDecryptString(string data, ushort pass)
		{
			if (string.IsNullOrEmpty(data))
			{
				throw new NullReferenceException("String to decrypt cannot be null or empty");
			}
			string[] array = new string[6];
			ushort passCodeFromString = FileEncrypter.GetPassCodeFromString("");
			string[] array2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < 2)
			{
				throw new FormatException(string.Concat(new object[]
				{
					"Tried to decrypt an invalid valid DEC ENC file \"",
					data,
					"\" - not enough elements. Need 2 lines, had ",
					array2.Length
				}));
			}
			string[] array3 = array2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
			if (array3.Length < 4)
			{
				throw new FormatException("Tried to decrypt an invalid valid DEC ENC file \"" + data + "\" - not enough headers");
			}
			string text = FileEncrypter.Decrypt(array3[1], passCodeFromString);
			string text2 = FileEncrypter.Decrypt(array3[2], passCodeFromString);
			string text3 = FileEncrypter.Decrypt(array3[3], pass);
			string text4 = null;
			if (array3.Length > 4)
			{
				text4 = FileEncrypter.Decrypt(array3[4], passCodeFromString);
			}
			string text5 = null;
			string text6 = "1";
			if (text3 == "ENCODED")
			{
				text5 = FileEncrypter.Decrypt(array2[1], pass);
			}
			else
			{
				text6 = "0";
			}
			array[0] = text;
			array[1] = text2;
			array[2] = text5;
			array[3] = text4;
			array[4] = text6;
			array[5] = text3;
			return array;
		}

		// Token: 0x060008F7 RID: 2295 RVA: 0x00094D48 File Offset: 0x00092F48
		public static string[] DecryptHeaders(string data, string pass = "")
		{
			string[] array = new string[3];
			ushort passCodeFromString = FileEncrypter.GetPassCodeFromString(pass);
			string[] array2 = data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
			if (array2.Length < 2)
			{
				throw new FormatException("Tried to decrypt an invalid valid DEC ENC file");
			}
			string[] array3 = array2[0].Split(FileEncrypter.HeaderSplitDelimiters, StringSplitOptions.None);
			if (array3.Length < 4)
			{
				throw new FormatException("Tried to decrypt an invalid valid DEC ENC file");
			}
			string text = FileEncrypter.Decrypt(array3[1], passCodeFromString);
			string text2 = FileEncrypter.Decrypt(array3[2], passCodeFromString);
			string text3 = null;
			if (array3.Length > 4)
			{
				text3 = FileEncrypter.Decrypt(array3[4], passCodeFromString);
			}
			array[0] = text;
			array[1] = text2;
			array[2] = text3;
			return array;
		}

		// Token: 0x060008F8 RID: 2296 RVA: 0x00094E00 File Offset: 0x00093000
		public static int FileIsEncrypted(string data, string pass = "")
		{
			if (data.StartsWith("#DEC_ENC::"))
			{
				string[] array = FileEncrypter.DecryptString(data, pass);
				if (array[5] != "ENCODED")
				{
					if (array[4] == "0")
					{
						return 2;
					}
				}
				else
				{
					if (array[4] == "0")
					{
						return 2;
					}
					if (array[4] == "1")
					{
						return 1;
					}
				}
			}
			return 0;
		}

		// Token: 0x060008F9 RID: 2297 RVA: 0x00094E94 File Offset: 0x00093094
		public static string MakeReplacementsForDisplay(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				if (GuiData.tinyfont.Characters.Contains(input[i]))
				{
					stringBuilder.Append(input[i]);
				}
				else
				{
					stringBuilder.Append("_");
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x04000A5E RID: 2654
		private static string[] HeaderSplitDelimiters = new string[]
		{
			"::"
		};
	}
}
