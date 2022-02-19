using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000039 RID: 57
	public class SADeleteFile : SerializableAction
	{
		// Token: 0x06000147 RID: 327 RVA: 0x00012EDC File Offset: 0x000110DC
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			Computer computer = Programs.getComputer(os, this.TargetComp);
			if (this.Delay <= 0f)
			{
				Folder folderAtPath = Programs.getFolderAtPath(this.FilePath, os, computer.files.root, true);
				if (folderAtPath != null)
				{
					FileEntry fileEntry = folderAtPath.searchForFile(this.FileName);
					if (fileEntry != null)
					{
						folderAtPath.files.Remove(fileEntry);
					}
				}
			}
			else
			{
				Computer computer2 = Programs.getComputer(os, this.DelayHost);
				if (computer2 == null)
				{
					throw new NullReferenceException("Computer " + computer2 + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer2).AddAction(this, delay);
			}
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00012FB8 File Offset: 0x000111B8
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SADeleteFile sadeleteFile = new SADeleteFile();
			if (rdr.MoveToAttribute("Delay"))
			{
				sadeleteFile.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				sadeleteFile.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("FilePath"))
			{
				sadeleteFile.FilePath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("FileName"))
			{
				sadeleteFile.FileName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sadeleteFile.DelayHost = rdr.ReadContentAsString();
			}
			return sadeleteFile;
		}

		// Token: 0x0400013A RID: 314
		public string TargetComp;

		// Token: 0x0400013B RID: 315
		public string FilePath;

		// Token: 0x0400013C RID: 316
		public string FileName;

		// Token: 0x0400013D RID: 317
		public string DelayHost;

		// Token: 0x0400013E RID: 318
		public float Delay;
	}
}
