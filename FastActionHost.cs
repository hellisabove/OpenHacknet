using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000012 RID: 18
	internal class FastActionHost : Daemon
	{
		// Token: 0x06000099 RID: 153 RVA: 0x0000B3A0 File Offset: 0x000095A0
		public FastActionHost(Computer c, OS os, string name) : base(c, name, os)
		{
			this.isListed = true;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000B3BC File Offset: 0x000095BC
		public override void initFiles()
		{
			base.initFiles();
			this.folder = this.comp.files.root.searchForFolder("runtime");
			if (this.folder == null)
			{
				this.folder = new Folder("runtime");
				this.comp.files.root.folders.Add(this.folder);
			}
			this.DelayedActions = new FastDelayableActionSystem(this.folder, this.os);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000B44C File Offset: 0x0000964C
		public override void loadInit()
		{
			base.loadInit();
			this.folder = this.comp.files.root.searchForFolder("runtime");
			this.DelayedActions = new FastDelayableActionSystem(this.folder, this.os);
			this.DelayedActions.DeserializeActions(this.folder.files);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000B4B0 File Offset: 0x000096B0
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000B4C8 File Offset: 0x000096C8
		public override void navigatedTo()
		{
			base.navigatedTo();
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000B4D4 File Offset: 0x000096D4
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			Vector2 pos = new Vector2((float)bounds.X + 20f, (float)bounds.Y + 20f);
			TextItem.doFontLabel(pos, "Active Actions : " + this.DelayedActions.Actions.Count, GuiData.smallfont, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
			pos.Y += 30f;
			if (Button.doButton(38391101, (int)pos.X, (int)pos.Y, 300, 25, "Back", null))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000B5B0 File Offset: 0x000097B0
		public override string getSaveString()
		{
			this.folder.files = this.DelayedActions.GetAllFilesForActions();
			return "<FastActionHost />";
		}

		// Token: 0x040000A0 RID: 160
		public FastDelayableActionSystem DelayedActions;

		// Token: 0x040000A1 RID: 161
		public bool RequiresLogin = false;

		// Token: 0x040000A2 RID: 162
		private Folder folder;
	}
}
