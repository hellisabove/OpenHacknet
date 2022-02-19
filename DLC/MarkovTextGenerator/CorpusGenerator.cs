using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Hacknet.DLC.MarkovTextGenerator
{
	// Token: 0x02000026 RID: 38
	public static class CorpusGenerator
	{
		// Token: 0x06000101 RID: 257 RVA: 0x0000F4E8 File Offset: 0x0000D6E8
		public static Corpus GenerateCorpusFromFolder(string name, int maxFilesToRead = -1, Action<float, string> percentCompleteUpdated = null, Action<Corpus> Complete = null)
		{
			Corpus result;
			try
			{
				Corpus corpus = new Corpus();
				DirectoryInfo directoryInfo = new DirectoryInfo(name + "/");
				string[] extensions = new string[]
				{
					".txt",
					".cs"
				};
				FileInfo[] array = (from f in directoryInfo.GetFiles()
				where extensions.Contains(f.Extension.ToLower())
				select f).ToArray<FileInfo>();
				int num = 0;
				while (num < array.Length && (maxFilesToRead == -1 || num < maxFilesToRead))
				{
					float num2 = (float)num / (float)array.Length;
					num2 *= 100f;
					if (maxFilesToRead != -1)
					{
						num2 = (float)num / (float)maxFilesToRead;
					}
					string input = File.ReadAllText(name + "/" + array[num].Name);
					if (percentCompleteUpdated != null)
					{
						percentCompleteUpdated(num2, num2.ToString("00.00") + "% | Reading " + array[num].Name + "...");
					}
					corpus.LearnText(input);
					num++;
				}
				if (Complete != null)
				{
					Complete(corpus);
				}
				result = corpus;
			}
			catch (Exception)
			{
				if (Complete != null)
				{
					Complete(null);
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000F684 File Offset: 0x0000D884
		public static void GenerateCorpusFromFolderThreaded(string foldername, Action<Corpus> Complete, Action<float, string> percentCompleteUpdated)
		{
			Task.Factory.StartNew<Corpus>(() => CorpusGenerator.GenerateCorpusFromFolder(foldername, -1, percentCompleteUpdated, Complete));
		}
	}
}
