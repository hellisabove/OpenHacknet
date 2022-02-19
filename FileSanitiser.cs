using System;

namespace Hacknet
{
	// Token: 0x02000110 RID: 272
	public static class FileSanitiser
	{
		// Token: 0x06000669 RID: 1641 RVA: 0x0006A618 File Offset: 0x00068818
		public static string purifyStringForDisplay(string data)
		{
			string result;
			if (data == null)
			{
				result = null;
			}
			else
			{
				data = data.Replace("\t", "    ").Replace("“", "\"").Replace("”", "\"");
				for (int i = 0; i < data.Length; i++)
				{
					if ((data[i] < ' ' || data[i] > '\u007f') && data[i] != '\n' && data[i] != '\v' && data[i] != '\f' && data[i] != '\n')
					{
						data = data.Replace(data[i], ' ');
					}
					if (GuiData.font != null && !GuiData.font.Characters.Contains(data[i]) && data[i] != '\n')
					{
						data = data.Replace(data[i], '_');
					}
				}
				result = data;
			}
			return result;
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0006A734 File Offset: 0x00068934
		public static void purifyVehicleFile(string path)
		{
			string text = Utils.readEntireFile(path);
			text = text.Replace('\t', '#');
			text = text.Replace("\r", "");
			for (int i = 0; i < text.Length; i++)
			{
				if (!GuiData.font.Characters.Contains(text[i]) && text[i] != '\n')
				{
					text = FileSanitiser.replaceChar(text, i, '_');
				}
			}
			Utils.writeToFile(text, "SanitisedFile.txt");
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0006A7C0 File Offset: 0x000689C0
		public static string replaceChar(string data, int index, char replacer)
		{
			return data.Substring(0, index - 1) + replacer + data.Substring(index + 1, data.Length - index - 2);
		}

		// Token: 0x0600066C RID: 1644 RVA: 0x0006A7FC File Offset: 0x000689FC
		public static void purifyNameFile(string path)
		{
			string text = Utils.readEntireFile(path);
			string[] array = text.Split(Utils.newlineDelim);
			string text2 = "";
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(Utils.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
				text2 = text2 + array2[0] + "\n";
			}
			Utils.writeToFile(text2, "SanitisedNameFile.txt");
		}

		// Token: 0x0600066D RID: 1645 RVA: 0x0006A868 File Offset: 0x00068A68
		public static void purifyLocationFile(string path)
		{
			string text = Utils.readEntireFile(path);
			char[] separator = new char[]
			{
				'\t'
			};
			string[] array = text.Split(Utils.newlineDelim);
			string text2 = "";
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);
				for (int j = 1; j < array2.Length; j++)
				{
					text2 = text2 + array2[j].Trim() + "#";
				}
				text2 += "\n";
			}
			Utils.writeToFile(text2, "SanitisedLocFile.txt");
		}
	}
}
