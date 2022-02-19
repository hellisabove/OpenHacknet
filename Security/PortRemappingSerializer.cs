using System;
using System.Collections.Generic;

namespace Hacknet.Security
{
	// Token: 0x02000168 RID: 360
	public static class PortRemappingSerializer
	{
		// Token: 0x06000913 RID: 2323 RVA: 0x000961BC File Offset: 0x000943BC
		public static string GetSaveString(Dictionary<int, int> input)
		{
			string result;
			if (input == null || input.Count == 0)
			{
				result = "";
			}
			else
			{
				string text = "<portRemap>";
				foreach (KeyValuePair<int, int> keyValuePair in input)
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						keyValuePair.Key,
						"=",
						keyValuePair.Value,
						","
					});
				}
				text = text.Substring(0, text.Length - 1);
				text += "</portRemap>\n";
				result = text;
			}
			return result;
		}

		// Token: 0x06000914 RID: 2324 RVA: 0x000962A0 File Offset: 0x000944A0
		public static Dictionary<int, int> Deserialize(string input)
		{
			Dictionary<int, int> result;
			if (string.IsNullOrWhiteSpace(input))
			{
				result = null;
			}
			else
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				string[] array = input.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Trim().Split(PortRemappingSerializer.EqualsDelimiter, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length >= 2)
					{
						string text = array2[0].ToLower();
						string text2 = text;
						if (text2 == null)
						{
							goto IL_159;
						}
						if (<PrivateImplementationDetails>{44D58447-4185-43DF-BEF1-8BBDED416CAA}.$$method0x6000866-1 == null)
						{
							<PrivateImplementationDetails>{44D58447-4185-43DF-BEF1-8BBDED416CAA}.$$method0x6000866-1 = new Dictionary<string, int>(8)
							{
								{
									"ftp",
									0
								},
								{
									"web",
									1
								},
								{
									"ssh",
									2
								},
								{
									"smtp",
									3
								},
								{
									"sql",
									4
								},
								{
									"medical",
									5
								},
								{
									"torrent",
									6
								},
								{
									"ssl",
									7
								}
							};
						}
						int num;
						if (!<PrivateImplementationDetails>{44D58447-4185-43DF-BEF1-8BBDED416CAA}.$$method0x6000866-1.TryGetValue(text2, out num))
						{
							goto IL_159;
						}
						int key;
						switch (num)
						{
						case 0:
							key = 21;
							break;
						case 1:
							key = 80;
							break;
						case 2:
							key = 22;
							break;
						case 3:
							key = 25;
							break;
						case 4:
							key = 1433;
							break;
						case 5:
							key = 104;
							break;
						case 6:
							key = 6881;
							break;
						case 7:
							key = 443;
							break;
						default:
							goto IL_159;
						}
						IL_164:
						dictionary.Add(key, Convert.ToInt32(array2[1]));
						goto IL_176;
						IL_159:
						key = Convert.ToInt32(text);
						goto IL_164;
					}
					IL_176:;
				}
				result = dictionary;
			}
			return result;
		}

		// Token: 0x04000A69 RID: 2665
		private static char[] EqualsDelimiter = new char[]
		{
			'='
		};
	}
}
