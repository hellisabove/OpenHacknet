using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000032 RID: 50
	public class SAAddAsset : SerializableAction
	{
		// Token: 0x06000132 RID: 306 RVA: 0x00012200 File Offset: 0x00010400
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (computer == null)
			{
				throw new NullReferenceException("Computer " + this.TargetComp + " could not be found for AddAssetFunction, adding file: " + this.FileName);
			}
			Folder folderAtPath = Programs.getFolderAtPath(this.TargetFolderpath, os, computer.files.root, true);
			if (folderAtPath == null)
			{
				throw new NullReferenceException("Folder " + this.TargetFolderpath + " could not be found for AddAssetFunction, adding file: " + this.FileName);
			}
			FileEntry item = new FileEntry(ComputerLoader.filter(this.FileContents), this.FileName);
			folderAtPath.files.Add(item);
		}

		// Token: 0x06000133 RID: 307 RVA: 0x000122BC File Offset: 0x000104BC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAddAsset saaddAsset = new SAAddAsset();
			if (rdr.MoveToAttribute("FileName"))
			{
				saaddAsset.FileName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("FileContents"))
			{
				saaddAsset.FileContents = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saaddAsset.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetFolderpath"))
			{
				saaddAsset.TargetFolderpath = rdr.ReadContentAsString();
			}
			return saaddAsset;
		}

		// Token: 0x0400011E RID: 286
		public string FileName;

		// Token: 0x0400011F RID: 287
		public string FileContents;

		// Token: 0x04000120 RID: 288
		public string TargetComp;

		// Token: 0x04000121 RID: 289
		public string TargetFolderpath;

		// Token: 0x04000122 RID: 290
		public int FunctionValue = 0;
	}
}
