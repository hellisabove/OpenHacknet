using System;
using System.IO;

namespace Hacknet.Misc
{
	// Token: 0x02000193 RID: 403
	public class WordCounter
	{
		// Token: 0x06000A27 RID: 2599 RVA: 0x000A2070 File Offset: 0x000A0270
		public static void PerformWordCount(string[] folders, string[] fileOnlyFolders)
		{
			int num = 0;
			WordCounter.accum = "";
			WordCounter.charAccum = 0;
			for (int i = 0; i < folders.Length; i++)
			{
				string[] directories = Directory.GetDirectories(folders[i]);
				num += WordCounter.GetWordCountFromFolder(folders[i]);
				for (int j = 0; j < directories.Length; j++)
				{
					num += WordCounter.GetWordCountFromFolder(directories[j]);
				}
			}
			for (int i = 0; i < fileOnlyFolders.Length; i++)
			{
				num += WordCounter.GetWordCountFromFolder(fileOnlyFolders[i]);
			}
			Console.WriteLine("--------------\n\nWORD COUNT COMPLETE::\n\n");
			Console.WriteLine(string.Concat(new object[]
			{
				"Total Words: ",
				num,
				"\nTotal Chars: ",
				WordCounter.charAccum,
				"\n\n"
			}));
			object obj = WordCounter.accum;
			WordCounter.accum = string.Concat(new object[]
			{
				obj,
				"\r\n---------------\r\nTotal Count: ",
				num,
				"\r\nChars: ",
				WordCounter.charAccum,
				"\r\n"
			});
			File.WriteAllText("WordCount.txt", WordCounter.accum);
		}

		// Token: 0x06000A28 RID: 2600 RVA: 0x000A21B4 File Offset: 0x000A03B4
		private static int GetWordCountFromFolder(string folderpath)
		{
			int result;
			if (folderpath.EndsWith("Untranslated"))
			{
				result = 0;
			}
			else
			{
				string[] files = Directory.GetFiles(folderpath);
				int num = 0;
				for (int i = 0; i < files.Length; i++)
				{
					int num2 = 0;
					string text = files[i];
					if (text.EndsWith(".xml"))
					{
						Console.WriteLine("Reading " + text);
						try
						{
							num2 += WordCounter.GetTextCountFromXMLFile(text);
							num += num2;
							WordCounter.accum += string.Format("COMPLETE ({0}) : {1}\r\n", num2, text);
						}
						catch (Exception)
						{
							WordCounter.accum = WordCounter.accum + "ERROR: Could not process " + text + "\r\n";
						}
					}
					else if (text.EndsWith(".txt"))
					{
						Console.WriteLine("Reading " + text);
						string text2 = File.ReadAllText(text);
						WordCounter.charAccum += text2.Length;
						num2 += WordCounter.CountString(text2);
						num += num2;
						Console.Write("...Complete\n");
						WordCounter.accum += string.Format("COMPLETE ({0}) : {1}\r\n", num2, text);
					}
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000A29 RID: 2601 RVA: 0x000A2330 File Offset: 0x000A0530
		private static int CountString(string input)
		{
			char[] separator = new char[]
			{
				' ',
				'.',
				',',
				';',
				':',
				'\n',
				'\t'
			};
			return input.Split(separator, StringSplitOptions.RemoveEmptyEntries).Length;
		}

		// Token: 0x06000A2A RID: 2602 RVA: 0x000A2360 File Offset: 0x000A0560
		private static int GetWordCountFromComputer(Computer c)
		{
			int num = 0;
			num += WordCounter.GetWordCountFromFolder(c.files.root);
			WordCounter.charAccum += c.name.Length;
			return num + WordCounter.CountString(c.name);
		}

		// Token: 0x06000A2B RID: 2603 RVA: 0x000A23AC File Offset: 0x000A05AC
		private static int GetWordCountFromFolder(Folder f)
		{
			int num = 0;
			int result;
			if (f.name == "sys")
			{
				result = num;
			}
			else
			{
				for (int i = 0; i < f.folders.Count; i++)
				{
					num += WordCounter.GetWordCountFromFolder(f.folders[i]);
				}
				for (int i = 0; i < f.files.Count; i++)
				{
					WordCounter.charAccum += f.files[i].name.Length;
					WordCounter.charAccum += f.files[i].data.Length;
					num += WordCounter.CountString(f.files[i].name);
					num += WordCounter.CountString(f.files[i].data);
				}
				result = num;
			}
			return result;
		}

		// Token: 0x06000A2C RID: 2604 RVA: 0x000A24A4 File Offset: 0x000A06A4
		private static int GetWordCountFromMission(ActiveMission m)
		{
			int num = 0;
			WordCounter.charAccum += m.email.body.Length;
			WordCounter.charAccum += m.email.subject.Length;
			WordCounter.charAccum += m.postingBody.Length;
			WordCounter.charAccum += m.postingTitle.Length;
			num += WordCounter.CountString(m.email.body);
			num += WordCounter.CountString(m.email.subject);
			num += WordCounter.CountString(m.postingBody);
			return num + WordCounter.CountString(m.postingTitle);
		}

		// Token: 0x06000A2D RID: 2605 RVA: 0x000A255D File Offset: 0x000A075D
		private static int GetTextCountFromXMLFile(string path)
		{
			throw new NotSupportedException("Only Supported in XNA!");
		}

		// Token: 0x04000B79 RID: 2937
		private static string accum;

		// Token: 0x04000B7A RID: 2938
		private static int charAccum;
	}
}
