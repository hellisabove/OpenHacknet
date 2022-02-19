using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hacknet.DLC.MarkovTextGenerator
{
	// Token: 0x02000025 RID: 37
	public class Corpus
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000EDE8 File Offset: 0x0000CFE8
		public List<PotentialEntry> GetPotentialEntries(string preceeding)
		{
			List<PotentialEntry> list = new List<PotentialEntry>();
			List<PotentialEntry> result;
			if (!this.data.ContainsKey(preceeding))
			{
				result = list;
			}
			else
			{
				Dictionary<string, int> dictionary = this.data[preceeding];
				int num = 0;
				int num2 = 0;
				foreach (KeyValuePair<string, int> keyValuePair in dictionary)
				{
					list.Add(new PotentialEntry
					{
						word = keyValuePair.Key,
						weighting = (double)keyValuePair.Value
					});
					if (keyValuePair.Value > num)
					{
						num = keyValuePair.Value;
					}
					num2 += keyValuePair.Value;
				}
				for (int i = 0; i < list.Count; i++)
				{
					list[i].weighting = list[i].weighting / (double)num2;
				}
				result = list;
			}
			return result;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000EF00 File Offset: 0x0000D100
		public void Serialize(string filename)
		{
			using (FileStream fileStream = File.OpenWrite(filename))
			{
				StreamWriter streamWriter = new StreamWriter(fileStream);
				foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in this.data)
				{
					streamWriter.Write(keyValuePair.Key + "##^@%##\r\n");
					try
					{
						foreach (KeyValuePair<string, int> keyValuePair2 in keyValuePair.Value)
						{
							streamWriter.Write(string.Concat(new object[]
							{
								"\t",
								keyValuePair2.Key,
								"\t|\t",
								keyValuePair2.Value,
								"\r\n"
							}));
						}
					}
					catch (Exception)
					{
					}
					streamWriter.Write("#*@*@*@*@*@#\r\n\r\n");
				}
				streamWriter.Flush();
				streamWriter.Close();
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000F098 File Offset: 0x0000D298
		public void LearnText(string input)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.LearningLength; i++)
			{
				list.Add(null);
			}
			string[] array = input.Split(Utils.WhitespaceDelim, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string key = Corpus.ConvertMemoryToString(list);
				Dictionary<string, int> dictionary;
				if (this.data.ContainsKey(key))
				{
					dictionary = this.data[key];
				}
				else
				{
					dictionary = new Dictionary<string, int>();
					try
					{
						this.data.Add(key, dictionary);
					}
					catch (OutOfMemoryException)
					{
						break;
					}
				}
				string text = array[i];
				if (dictionary.ContainsKey(text))
				{
					Dictionary<string, int> dictionary2;
					string key2;
					(dictionary2 = dictionary)[key2 = text] = dictionary2[key2] + 1;
				}
				else
				{
					dictionary.Add(text, 1);
				}
				try
				{
					this.data[key] = dictionary;
				}
				catch (OutOfMemoryException)
				{
					break;
				}
				if (Corpus.WordEndsWithSentenceEnder(text))
				{
					for (int j = 0; j < this.LearningLength; j++)
					{
						list[j] = null;
					}
				}
				else
				{
					list.Add(text);
					list.RemoveAt(0);
				}
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000F20C File Offset: 0x0000D40C
		private static bool WordEndsWithSentenceEnder(string word)
		{
			string source = ".!?;{}";
			return source.Contains(word[word.Length - 1]);
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000F238 File Offset: 0x0000D438
		private static string ConvertMemoryToString(List<string> memory)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < memory.Count; i++)
			{
				stringBuilder.Append(memory[i]);
				stringBuilder.Append(" ");
			}
			return stringBuilder.ToString().Trim();
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000F28C File Offset: 0x0000D48C
		public string GetAnalysisStringFromWordList(List<string> words)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.LearningLength; i++)
			{
				list.Add(words[words.Count - this.LearningLength + i]);
			}
			return Corpus.ConvertMemoryToString(list);
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000F2E0 File Offset: 0x0000D4E0
		public string GenerateSentence(Action<string> CompleteAction = null)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.LearningLength; i++)
			{
				list.Add(null);
			}
			int j = 0;
			while (j < 1000)
			{
				List<PotentialEntry> potentialEntries = this.GetPotentialEntries(this.GetAnalysisStringFromWordList(list));
				double num = Utils.random.NextDouble();
				double num2 = 0.0;
				for (int i = 0; i < potentialEntries.Count; i++)
				{
					num2 += potentialEntries[i].weighting;
					if (num < num2)
					{
						list.Add(potentialEntries[i].word);
						break;
					}
				}
				j++;
				if (Corpus.WordEndsWithSentenceEnder(list[list.Count - 1]))
				{
					string text = "";
					for (int i = 0; i < list.Count; i++)
					{
						text = text + list[i] + " ";
					}
					text = text.Trim();
					if (CompleteAction != null)
					{
						CompleteAction(text);
					}
					return text;
				}
			}
			if (CompleteAction != null)
			{
				CompleteAction(null);
			}
			return null;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000F460 File Offset: 0x0000D660
		public void GenerateSentenceThreaded(Action<string> complete)
		{
			Task.Factory.StartNew<string>(() => this.GenerateSentence(complete));
		}

		// Token: 0x04000100 RID: 256
		private const string DICT_FILE_TYPE = ".csd";

		// Token: 0x04000101 RID: 257
		private const string BODY_FILE_TYPE = ".csb";

		// Token: 0x04000102 RID: 258
		private const string EMPTY_STRING_KEY_WILDCARD = "\tEMPTY\t";

		// Token: 0x04000103 RID: 259
		public int LearningLength = 3;

		// Token: 0x04000104 RID: 260
		private Dictionary<string, Dictionary<string, int>> data = new Dictionary<string, Dictionary<string, int>>();
	}
}
