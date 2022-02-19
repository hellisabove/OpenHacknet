using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000033 RID: 51
	public class SACopyAsset : SerializableAction
	{
		// Token: 0x06000135 RID: 309 RVA: 0x00012360 File Offset: 0x00010560
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.DestComp);
			Computer computer2 = Programs.getComputer(os, this.SourceComp);
			if (computer == null)
			{
				throw new NullReferenceException(string.Concat(new object[]
				{
					"Destination Computer ",
					computer,
					" could not be found for SACopyAsset, copying file: ",
					this.SourceFileName
				}));
			}
			if (computer2 == null)
			{
				throw new NullReferenceException(string.Concat(new object[]
				{
					"Source Computer ",
					computer2,
					" could not be found for SACopyAsset, copying file: ",
					this.SourceFileName
				}));
			}
			Folder folderAtPath = Programs.getFolderAtPath(this.SourceFilePath, os, computer2.files.root, true);
			if (folderAtPath == null)
			{
				throw new NullReferenceException("Source Folder " + this.SourceFilePath + " could not be found for SACopyAsset, adding file: " + this.SourceFileName);
			}
			Folder folderFromPath = computer.getFolderFromPath(this.DestFilePath, true);
			if (folderFromPath != null)
			{
				FileEntry fileEntry = folderAtPath.searchForFile(this.SourceFileName);
				if (fileEntry != null)
				{
					FileEntry item = new FileEntry(fileEntry.data, this.DestFileName);
					folderFromPath.files.Add(item);
				}
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x000124BC File Offset: 0x000106BC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SACopyAsset sacopyAsset = new SACopyAsset();
			if (rdr.MoveToAttribute("DestFileName"))
			{
				sacopyAsset.DestFileName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("DestFilePath"))
			{
				sacopyAsset.DestFilePath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("DestComp"))
			{
				sacopyAsset.DestComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("SourceComp"))
			{
				sacopyAsset.SourceComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("SourceFileName"))
			{
				sacopyAsset.SourceFileName = rdr.ReadContentAsString();
				if (string.IsNullOrWhiteSpace(sacopyAsset.DestFileName))
				{
					sacopyAsset.DestFileName = sacopyAsset.SourceFileName;
				}
			}
			if (rdr.MoveToAttribute("SourceFilePath"))
			{
				sacopyAsset.SourceFilePath = rdr.ReadContentAsString();
			}
			return sacopyAsset;
		}

		// Token: 0x04000123 RID: 291
		public string DestFileName;

		// Token: 0x04000124 RID: 292
		public string DestFilePath;

		// Token: 0x04000125 RID: 293
		public string DestComp;

		// Token: 0x04000126 RID: 294
		public string SourceComp;

		// Token: 0x04000127 RID: 295
		public string SourceFileName;

		// Token: 0x04000128 RID: 296
		public string SourceFilePath;

		// Token: 0x04000129 RID: 297
		public int FunctionValue = 0;
	}
}
