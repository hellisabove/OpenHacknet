using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200003F RID: 63
	public class SAAppendToFile : SerializableAction
	{
		// Token: 0x06000159 RID: 345 RVA: 0x00013A0C File Offset: 0x00011C0C
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				Computer computer = Programs.getComputer(os, this.TargetComp);
				Folder folderAtPath = Programs.getFolderAtPath(this.TargetFolderpath, os, computer.files.root, true);
				if (folderAtPath != null)
				{
					FileEntry fileEntry = folderAtPath.searchForFile(this.TargetFilename);
					fileEntry.data = fileEntry.data + "\n" + this.DataToAdd;
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

		// Token: 0x0600015A RID: 346 RVA: 0x00013AEC File Offset: 0x00011CEC
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAAppendToFile saappendToFile = new SAAppendToFile();
			if (rdr.MoveToAttribute("Delay"))
			{
				saappendToFile.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				saappendToFile.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				saappendToFile.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetFolderpath"))
			{
				saappendToFile.TargetFolderpath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetFilename"))
			{
				saappendToFile.TargetFilename = rdr.ReadContentAsString();
			}
			rdr.MoveToContent();
			saappendToFile.DataToAdd = ComputerLoader.filter(rdr.ReadElementContentAsString());
			return saappendToFile;
		}

		// Token: 0x04000155 RID: 341
		[XMLContent]
		public string DataToAdd;

		// Token: 0x04000156 RID: 342
		public string TargetComp;

		// Token: 0x04000157 RID: 343
		public string TargetFolderpath;

		// Token: 0x04000158 RID: 344
		public string TargetFilename;

		// Token: 0x04000159 RID: 345
		public string DelayHost;

		// Token: 0x0400015A RID: 346
		public float Delay;
	}
}
