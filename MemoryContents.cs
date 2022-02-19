using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000027 RID: 39
	public class MemoryContents
	{
		// Token: 0x06000103 RID: 259 RVA: 0x0000F6C8 File Offset: 0x0000D8C8
		public string GetSaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<Memory>\r\n");
			if (this.DataBlocks != null && this.DataBlocks.Count > 0)
			{
				stringBuilder.Append("<Data>");
				for (int i = 0; i < this.DataBlocks.Count; i++)
				{
					stringBuilder.Append("<Block>" + Folder.Filter(this.DataBlocks[i]) + "</Block>\r\n");
				}
				stringBuilder.Append("</Data>\r\n");
			}
			if (this.CommandsRun != null && this.CommandsRun.Count > 0)
			{
				stringBuilder.Append("<Commands>");
				for (int i = 0; i < this.CommandsRun.Count; i++)
				{
					stringBuilder.Append("<Command>" + Folder.Filter(this.CommandsRun[i]) + "</Command>\r\n");
				}
				stringBuilder.Append("</Commands>\r\n");
			}
			if (this.FileFragments != null && this.FileFragments.Count > 0)
			{
				stringBuilder.Append("<FileFragments>");
				for (int i = 0; i < this.CommandsRun.Count; i++)
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						"<File name=\"",
						Folder.Filter(this.FileFragments[i].Key),
						"\">",
						Folder.Filter(this.FileFragments[i].Value),
						"</Command>\r\n"
					}));
				}
				stringBuilder.Append("</FileFragments>\r\n");
			}
			if (this.Images != null && this.Images.Count > 0)
			{
				stringBuilder.Append("<Images>");
				for (int i = 0; i < this.Images.Count; i++)
				{
					stringBuilder.Append("<Image>" + Folder.Filter(this.Images[i]) + "</Image>\r\n");
				}
				stringBuilder.Append("</Images>\r\n");
			}
			stringBuilder.Append("</Memory>");
			return stringBuilder.ToString();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000FAF4 File Offset: 0x0000DCF4
		public static MemoryContents Deserialize(XmlReader rdr)
		{
			MemoryContents ret = new MemoryContents();
			while (rdr.Name != "Memory")
			{
				rdr.Read();
				if (rdr.EOF)
				{
					throw new FormatException("Unexpected end of file looking for start of Memory tag");
				}
			}
			for (;;)
			{
				rdr.Read();
				if (rdr.Name == "Memory" && !rdr.IsStartElement())
				{
					break;
				}
				Utils.ProcessXmlElementInParent(rdr, "Commands", "Command", delegate
				{
					rdr.MoveToContent();
					string text = rdr.ReadElementContentAsString();
					if (text.Contains("\n"))
					{
						string[] array = text.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
						for (int i = 0; i < array.Length; i++)
						{
							if (string.IsNullOrEmpty(array[i]))
							{
								array[i] = " ";
							}
							ret.CommandsRun.Add(ComputerLoader.filter(Folder.deFilter(array[i])));
						}
					}
					else
					{
						ret.CommandsRun.Add(ComputerLoader.filter(Folder.deFilter(text)));
					}
				});
				Utils.ProcessXmlElementInParent(rdr, "Data", "Block", delegate
				{
					rdr.MoveToContent();
					string s = rdr.ReadElementContentAsString();
					ret.DataBlocks.Add(ComputerLoader.filter(Folder.deFilter(s)));
				});
				Utils.ProcessXmlElementInParent(rdr, "FileFragments", "File", delegate
				{
					string s = "UNKNOWN";
					if (rdr.MoveToAttribute("name"))
					{
						s = rdr.ReadContentAsString();
					}
					rdr.MoveToContent();
					string s2 = rdr.ReadElementContentAsString();
					ret.FileFragments.Add(new KeyValuePair<string, string>(Folder.deFilter(s), Folder.deFilter(s2)));
				});
				Utils.ProcessXmlElementInParent(rdr, "Images", "Image", delegate
				{
					rdr.MoveToContent();
					string s = rdr.ReadElementContentAsString();
					ret.Images.Add(Folder.deFilter(s));
				});
				if (rdr.EOF)
				{
					goto Block_8;
				}
			}
			return ret;
			Block_8:
			throw new FormatException("Unexpected end of file trying to deserialize memory contents!");
		}

		// Token: 0x06000105 RID: 261 RVA: 0x0000FC90 File Offset: 0x0000DE90
		private string GetCompactSaveString()
		{
			string saveString = this.GetSaveString();
			return saveString.Replace("Commands>", "CM>").Replace("Command>", "c>").Replace("Data>", "D>").Replace("Block>", "b>").Replace("FileFragments>", "FF>").Replace("File>", "f>").Replace("Memory>", "M>").Replace("Images>", "Is>").Replace("Image>", "i>");
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000FD34 File Offset: 0x0000DF34
		private static string ReExpandSaveString(string save)
		{
			save = save.Replace("CM>", "Commands>\r\n").Replace("</c>", "</Command>\r\n").Replace("c>", "Command>").Replace("D>", "Data>\r\n").Replace("</b>", "</Block>\r\n").Replace("b>", "Block>").Replace("FF>", "FileFragments>\r\n").Replace("f>", "File>\r\n").Replace("M>", "Memory>\r\n").Replace("Is>", "Images>").Replace("i>", "Image>");
			return save;
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000FDF0 File Offset: 0x0000DFF0
		public string GetEncodedFileString()
		{
			string text = this.GetCompactSaveString();
			text = FileEncrypter.EncryptString(text, "MEMORY DUMP", "------", "19474-217316293", null);
			return "MEMORY_DUMP : FORMAT v1.22 ----------\n\n" + Computer.generateBinaryString(512).Substring(0, 400) + "\n\n" + text;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000FE48 File Offset: 0x0000E048
		public static MemoryContents GetMemoryFromEncodedFileString(string data)
		{
			string data2 = data.Substring("MEMORY_DUMP : FORMAT v1.22 ----------\n\n".Length + 400 + 2);
			string save = FileEncrypter.DecryptString(data2, "19474-217316293")[2];
			string s = MemoryContents.ReExpandSaveString(save);
			MemoryContents result;
			using (Stream stream = Utils.GenerateStreamFromString(s))
			{
				XmlReader rdr = XmlReader.Create(stream);
				MemoryContents memoryContents = MemoryContents.Deserialize(rdr);
				result = memoryContents;
			}
			return result;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000FEC8 File Offset: 0x0000E0C8
		public string TestEqualsWithErrorReport(MemoryContents other)
		{
			string text = "";
			string result;
			if (other == null)
			{
				result = "Other memory object is null!";
			}
			else
			{
				if (other.DataBlocks.Count == this.DataBlocks.Count)
				{
					for (int i = 0; i < this.DataBlocks.Count; i++)
					{
						if (other.DataBlocks[i] != this.DataBlocks[i])
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"Data block difference for item ",
								i,
								" - mismatch"
							});
						}
					}
				}
				else
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"Datablock count difference - found ",
						other.DataBlocks.Count,
						" - expected ",
						this.DataBlocks.Count
					});
				}
				if (other.CommandsRun.Count == this.CommandsRun.Count)
				{
					for (int i = 0; i < this.CommandsRun.Count; i++)
					{
						if (other.CommandsRun[i] != this.CommandsRun[i])
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"\n\nCommandsRun difference for item ",
								i,
								" - mismatch.\nFound ",
								other.CommandsRun[i],
								"  :vs:  ",
								this.CommandsRun[i],
								"\n"
							});
						}
					}
				}
				else
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"CommandsRun count difference - found ",
						other.CommandsRun.Count,
						" - expected ",
						this.CommandsRun.Count
					});
				}
				if (other.FileFragments.Count == this.FileFragments.Count)
				{
					for (int i = 0; i < this.FileFragments.Count; i++)
					{
						if (other.FileFragments[i].Key != this.FileFragments[i].Key)
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"FileFragments difference for item ",
								i,
								" - key mismatch"
							});
						}
						if (other.FileFragments[i].Value != this.FileFragments[i].Value)
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"FileFragments difference for item ",
								i,
								" - Value mismatch"
							});
						}
					}
				}
				else
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"FileFragments count difference - found ",
						other.FileFragments.Count,
						" - expected ",
						this.FileFragments.Count
					});
				}
				if (other.Images.Count == this.Images.Count)
				{
					for (int i = 0; i < this.Images.Count; i++)
					{
						if (other.Images[i] != this.Images[i])
						{
							object obj = text;
							text = string.Concat(new object[]
							{
								obj,
								"\n\nImages difference for item ",
								i,
								" - mismatch.\nFound ",
								other.Images[i],
								"  :vs:  ",
								this.Images[i],
								"\n"
							});
						}
					}
				}
				else
				{
					object obj = text;
					text = string.Concat(new object[]
					{
						obj,
						"Images count difference - found ",
						other.Images.Count,
						" - expected ",
						this.Images.Count
					});
				}
				result = text;
			}
			return result;
		}

		// Token: 0x04000105 RID: 261
		private const string EncryptionPass = "19474-217316293";

		// Token: 0x04000106 RID: 262
		private const string FileHeader = "MEMORY_DUMP : FORMAT v1.22 ----------\n\n";

		// Token: 0x04000107 RID: 263
		public List<string> DataBlocks = new List<string>();

		// Token: 0x04000108 RID: 264
		public List<string> CommandsRun = new List<string>();

		// Token: 0x04000109 RID: 265
		public List<KeyValuePair<string, string>> FileFragments = new List<KeyValuePair<string, string>>();

		// Token: 0x0400010A RID: 266
		public List<string> Images = new List<string>();
	}
}
