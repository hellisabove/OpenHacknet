using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000112 RID: 274
	public class Folder : FileType
	{
		// Token: 0x06000675 RID: 1653 RVA: 0x0006AB28 File Offset: 0x00068D28
		public Folder(string foldername)
		{
			this.name = foldername;
		}

		// Token: 0x06000676 RID: 1654 RVA: 0x0006AB50 File Offset: 0x00068D50
		public string getName()
		{
			return this.name;
		}

		// Token: 0x06000677 RID: 1655 RVA: 0x0006AB68 File Offset: 0x00068D68
		public bool containsFile(string name, string data)
		{
			for (int i = 0; i < this.files.Count; i++)
			{
				if (this.files[i].name.Equals(name) && this.files[i].data.Equals(data))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000678 RID: 1656 RVA: 0x0006ABD8 File Offset: 0x00068DD8
		public bool containsFileWithData(string data)
		{
			for (int i = 0; i < this.files.Count; i++)
			{
				if (this.files[i].data.Equals(data))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000679 RID: 1657 RVA: 0x0006AC2C File Offset: 0x00068E2C
		public bool containsFile(string name)
		{
			for (int i = 0; i < this.files.Count; i++)
			{
				if (this.files[i].name.Equals(name))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600067A RID: 1658 RVA: 0x0006AC80 File Offset: 0x00068E80
		public Folder searchForFolder(string folderName)
		{
			for (int i = 0; i < this.folders.Count; i++)
			{
				if (this.folders[i].name == folderName)
				{
					return this.folders[i];
				}
			}
			return null;
		}

		// Token: 0x0600067B RID: 1659 RVA: 0x0006ACDC File Offset: 0x00068EDC
		public FileEntry searchForFile(string fileName)
		{
			for (int i = 0; i < this.files.Count; i++)
			{
				if (this.files[i].name == fileName)
				{
					return this.files[i];
				}
			}
			return null;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x0006AD38 File Offset: 0x00068F38
		public string getSaveString()
		{
			string str = "<folder name=\"" + Folder.Filter(this.name) + "\">\n";
			for (int i = 0; i < this.folders.Count; i++)
			{
				str += this.folders[i].getSaveString();
			}
			for (int i = 0; i < this.files.Count; i++)
			{
				str = str + "<file name=\"" + Folder.Filter(this.files[i].name) + "\">";
				str += Folder.Filter(this.files[i].data);
				str += "</file>\n";
			}
			return str + "</folder>\n";
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0006AE14 File Offset: 0x00069014
		public static string Filter(string s)
		{
			return s.Replace("&", Folder.ampersandReplacer).Replace("\\", Folder.backslashReplacer).Replace("[", Folder.leftSBReplacer).Replace("]", Folder.rightSBReplacer).Replace(">", Folder.rightABReplacer).Replace("<", Folder.leftABReplacer).Replace("\"", Folder.quoteReplacer).Replace("'", Folder.singlequoteReplacer);
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0006AEA0 File Offset: 0x000690A0
		public static string deFilter(string s)
		{
			return s.Replace(Folder.ampersandReplacer, "&").Replace(Folder.backslashReplacer, "\\").Replace(Folder.rightSBReplacer, "]").Replace(Folder.leftSBReplacer, "[").Replace(Folder.rightABReplacer, ">").Replace(Folder.leftABReplacer, "<").Replace(Folder.quoteReplacer, "\"").Replace(Folder.singlequoteReplacer, "'");
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0006AF2C File Offset: 0x0006912C
		public static Folder load(XmlReader reader)
		{
			while (reader.Name != "folder" || reader.NodeType == XmlNodeType.EndElement)
			{
				reader.Read();
				if (reader.EOF)
				{
					return null;
				}
			}
			reader.MoveToAttribute("name");
			string text = reader.ReadContentAsString();
			text = Folder.deFilter(text);
			Folder folder = new Folder(text);
			reader.Read();
			while (reader.Name != "folder" && reader.Name != "file")
			{
				reader.Read();
				if (reader.EOF || (reader.Name == "folder" && reader.NodeType == XmlNodeType.EndElement))
				{
					return folder;
				}
			}
			while (reader.Name == "folder")
			{
				if (reader.NodeType == XmlNodeType.EndElement)
				{
					return folder;
				}
				folder.folders.Add(Folder.load(reader));
				reader.Read();
				while (reader.Name != "folder" && reader.Name != "file")
				{
					reader.Read();
					if (reader.EOF || reader.Name == "computer")
					{
						return folder;
					}
				}
			}
			while (reader.Name != "folder" && reader.Name != "file")
			{
				reader.Read();
			}
			while (reader.Name == "file" && reader.NodeType != XmlNodeType.EndElement)
			{
				reader.MoveToAttribute("name");
				string text2 = reader.ReadContentAsString();
				bool flag = true;
				if (reader.MoveToAttribute("EduSafe"))
				{
					flag = reader.ReadContentAsBoolean();
				}
				text2 = Folder.deFilter(text2);
				reader.MoveToElement();
				string text3 = reader.ReadElementContentAsString();
				text3 = Folder.deFilter(text3);
				if (flag || !Settings.EducationSafeBuild)
				{
					folder.files.Add(new FileEntry(text3, text2));
				}
				reader.Read();
				while (reader.Name != "folder" && reader.Name != "file")
				{
					reader.Read();
				}
			}
			reader.Read();
			return folder;
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0006B201 File Offset: 0x00069401
		public void load(string data)
		{
		}

		// Token: 0x06000681 RID: 1665 RVA: 0x0006B204 File Offset: 0x00069404
		public string TestEqualsFolder(Folder f)
		{
			string text = null;
			if (!Utils.CheckStringIsRenderable(this.name) || !Utils.CheckStringIsRenderable(f.name))
			{
				text = text + "Folder name includes Invalid Chars! " + f.name;
			}
			if (this.name != f.name)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"Name Mismatch : Expected \"",
					this.name,
					"\" But got \"",
					f.name,
					"\"\r\n"
				});
			}
			if (f.folders.Count != this.folders.Count)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"Folder Count Mismatch : Expected \"",
					this.folders.Count,
					"\" But got \"",
					f.folders.Count,
					"\"\r\n"
				});
			}
			if (f.files.Count != this.files.Count)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"File Count Mismatch In folder \"",
					f.name,
					"\" : Expected (loaded) \"",
					this.files.Count,
					"\" But got (original creation) \"",
					f.files.Count,
					"\"\r\nFound Files:\r\n"
				});
				for (int i = 0; i < f.files.Count; i++)
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						f.files[i].name,
						"\r\n--------\r\n",
						f.files[i].data,
						"\r\n#######END FILE#############\r\n\r\n"
					});
				}
			}
			string result;
			if (text != null)
			{
				text += "Previous errors are blocking. Abandoning examination.\r\n";
				result = text;
			}
			else
			{
				for (int i = 0; i < this.folders.Count; i++)
				{
					string text3 = this.folders[i].TestEqualsFolder(f.folders[i]);
					if (text3 != null)
					{
						text = text + "\r\n" + text3;
					}
				}
				for (int i = 0; i < this.files.Count; i++)
				{
					if (!Utils.CheckStringIsRenderable(this.files[i].name) || !Utils.CheckStringIsRenderable(f.files[i].name))
					{
						if (Settings.ActiveLocale == "en-us")
						{
							text = text + "File name includes Invalid Chars! " + f.files[i].name;
						}
					}
					if (this.files[i].name != f.files[i].name)
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"Filename Mismatch (",
							i,
							") expected \"",
							this.files[i].name,
							"\" but got \"",
							f.files[i].name,
							"\"\r\n"
						});
					}
					if (this.files[i].data != f.files[i].data && this.files[i].data.Replace("\r\n", "\n") != f.files[i].data.Replace("\r\n", "\n"))
					{
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"Data Mismatch (",
							i,
							") expected ------\r\n",
							this.files[i].data,
							"\r\n----- but got ------\r\n",
							f.files[i].data,
							"\r\n-----\r\n"
						});
					}
					else if (Settings.ActiveLocale == "en-us")
					{
						if (!Utils.CheckStringIsRenderable(this.files[i].data))
						{
						}
					}
				}
				result = text;
			}
			return result;
		}

		// Token: 0x0400072D RID: 1837
		private static string ampersandReplacer = "|##AMP##|";

		// Token: 0x0400072E RID: 1838
		private static string backslashReplacer = "|##BS##|";

		// Token: 0x0400072F RID: 1839
		private static string rightSBReplacer = "|##RSB##|";

		// Token: 0x04000730 RID: 1840
		private static string leftSBReplacer = "|##LSB##|";

		// Token: 0x04000731 RID: 1841
		private static string rightABReplacer = "|##RAB##|";

		// Token: 0x04000732 RID: 1842
		private static string leftABReplacer = "|##LAB##|";

		// Token: 0x04000733 RID: 1843
		private static string quoteReplacer = "|##QOT##|";

		// Token: 0x04000734 RID: 1844
		private static string singlequoteReplacer = "|##SIQ##|";

		// Token: 0x04000735 RID: 1845
		public List<FileEntry> files = new List<FileEntry>();

		// Token: 0x04000736 RID: 1846
		public List<Folder> folders = new List<Folder>();

		// Token: 0x04000737 RID: 1847
		public string name;
	}
}
