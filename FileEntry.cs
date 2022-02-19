using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Hacknet
{
	// Token: 0x0200010F RID: 271
	public class FileEntry : FileType
	{
		// Token: 0x06000664 RID: 1636 RVA: 0x0006A2F8 File Offset: 0x000684F8
		public FileEntry()
		{
			int index = Utils.random.Next(0, FileEntry.filenames.Count - 1);
			this.name = FileEntry.filenames[index];
			this.data = FileEntry.fileData[index];
			this.size = this.data.Length * 8;
			this.secondCreatedAt = (int)OS.currentElapsedTime;
		}

		// Token: 0x06000665 RID: 1637 RVA: 0x0006A368 File Offset: 0x00068568
		public FileEntry(string dataEntry, string nameEntry)
		{
			nameEntry = nameEntry.Replace(" ", "_");
			this.name = nameEntry;
			this.data = dataEntry;
			this.size = this.data.Length * 8;
			this.secondCreatedAt = (int)OS.currentElapsedTime;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0006A3C0 File Offset: 0x000685C0
		public string head()
		{
			int num = 0;
			string text = "";
			while (num < this.data.Length && this.data[num] != '\n' && num < 50)
			{
				text += this.data[num];
				num++;
			}
			return text;
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0006A428 File Offset: 0x00068628
		public string getName()
		{
			return this.name;
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0006A440 File Offset: 0x00068640
		public static void init(ContentManager content)
		{
			FileEntry.filenames = new List<string>(128);
			FileEntry.fileData = new List<string>(128);
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(content.RootDirectory, "files"));
			FileInfo[] files = directoryInfo.GetFiles("*.*");
			for (int i = 0; i < files.Length; i++)
			{
				FileEntry.filenames.Add(Path.GetFileNameWithoutExtension(files[i].Name));
				string filename = "Content/files/" + Path.GetFileName(files[i].Name);
				FileEntry.fileData.Add(Utils.readEntireFile(filename));
			}
			string filename2 = Settings.EducationSafeBuild ? "Content/BashLogs_StudentSafe.txt" : "Content/BashLogs.txt";
			string text = Utils.readEntireFile(filename2);
			string[] separator = new string[]
			{
				"\n#"
			};
			string[] array = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Trim();
				int num = array[i].Length - array[i].IndexOf("\r\n");
				FileEntry.filenames.Add("IRC_Log:" + array[i].Substring(0, array[i].IndexOf("\r\n")).Replace("- [X]", "").Replace(" ", ""));
				string str = array[i].Substring(array[i].IndexOf("\r\n")).Replace("\n ", "\n");
				if (Settings.ActiveLocale == "en-us")
				{
					str = FileSanitiser.purifyStringForDisplay(str);
				}
				FileEntry.fileData.Add(str + "\n\nArchived Via : http://Bash.org");
			}
		}

		// Token: 0x04000726 RID: 1830
		public static List<string> filenames;

		// Token: 0x04000727 RID: 1831
		public static List<string> fileData;

		// Token: 0x04000728 RID: 1832
		public string name;

		// Token: 0x04000729 RID: 1833
		public string data;

		// Token: 0x0400072A RID: 1834
		public int size;

		// Token: 0x0400072B RID: 1835
		public int secondCreatedAt;
	}
}
