using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000111 RID: 273
	internal class FileSystem
	{
		// Token: 0x0600066E RID: 1646 RVA: 0x0006A913 File Offset: 0x00068B13
		public FileSystem(bool empty)
		{
		}

		// Token: 0x0600066F RID: 1647 RVA: 0x0006A920 File Offset: 0x00068B20
		public FileSystem()
		{
			this.root = new Folder("/");
			this.root.folders.Add(new Folder("home"));
			this.root.folders.Add(new Folder("log"));
			this.root.folders.Add(new Folder("bin"));
			this.root.folders.Add(new Folder("sys"));
			this.generateSystemFiles();
		}

		// Token: 0x06000670 RID: 1648 RVA: 0x0006A9BC File Offset: 0x00068BBC
		public void generateSystemFiles()
		{
			Folder folder = this.root.searchForFolder("sys");
			folder.files.Add(new FileEntry(ThemeManager.getThemeDataString(OSTheme.HacknetTeal), "x-server.sys"));
			folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "os-config.sys"));
			folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "bootcfg.dll"));
			folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "netcfgx.dll"));
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0006AA58 File Offset: 0x00068C58
		public string getSaveString()
		{
			string str = "<filesystem>\n";
			str += this.root.getSaveString();
			return str + "</filesystem>\n";
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0006AA90 File Offset: 0x00068C90
		public static FileSystem load(XmlReader reader)
		{
			FileSystem fileSystem = new FileSystem(true);
			while (reader.Name != "filesystem")
			{
				reader.Read();
			}
			fileSystem.root = Folder.load(reader);
			return fileSystem;
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0006AAD4 File Offset: 0x00068CD4
		public string TestEquals(object obj)
		{
			FileSystem fileSystem = obj as FileSystem;
			if (fileSystem == null)
			{
				throw new ArgumentNullException();
			}
			return this.root.TestEqualsFolder(fileSystem.root);
		}

		// Token: 0x06000674 RID: 1652 RVA: 0x0006AB10 File Offset: 0x00068D10
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x0400072C RID: 1836
		public Folder root;
	}
}
