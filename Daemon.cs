using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000003 RID: 3
	internal class Daemon
	{
		// Token: 0x06000007 RID: 7 RVA: 0x0000214B File Offset: 0x0000034B
		public Daemon(Computer computer, string serviceName, OS opSystem)
		{
			this.name = serviceName;
			this.isListed = true;
			this.comp = computer;
			this.os = opSystem;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002172 File Offset: 0x00000372
		public virtual void initFiles()
		{
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002175 File Offset: 0x00000375
		public virtual void draw(Rectangle bounds, SpriteBatch sb)
		{
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002178 File Offset: 0x00000378
		public virtual void navigatedTo()
		{
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000217B File Offset: 0x0000037B
		public virtual void userAdded(string name, string pass, byte type)
		{
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002180 File Offset: 0x00000380
		public virtual string getSaveString()
		{
			return "";
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002197 File Offset: 0x00000397
		public virtual void loadInit()
		{
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000219C File Offset: 0x0000039C
		public static bool validUser(byte type)
		{
			return type == 1 || type == 0;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000021BC File Offset: 0x000003BC
		public void registerAsDefaultBootDaemon()
		{
			if (this.comp.AllowsDefaultBootModule)
			{
				FileEntry fileEntry = this.comp.files.root.searchForFolder("sys").searchForFile(ComputerTypeInfo.getDefaultBootDaemonFilename(this));
				if (fileEntry != null)
				{
					if (fileEntry.data != "[Locked]")
					{
						fileEntry.data = LocaleTerms.Loc(this.name);
					}
				}
				else
				{
					fileEntry = new FileEntry(LocaleTerms.Loc(this.name), ComputerTypeInfo.getDefaultBootDaemonFilename(this));
					this.comp.files.root.searchForFolder("sys").files.Add(fileEntry);
				}
			}
		}

		// Token: 0x04000003 RID: 3
		public string name;

		// Token: 0x04000004 RID: 4
		public bool isListed;

		// Token: 0x04000005 RID: 5
		public Computer comp;

		// Token: 0x04000006 RID: 6
		public OS os;
	}
}
