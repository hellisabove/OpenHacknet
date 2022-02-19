using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200005D RID: 93
	internal class Module
	{
		// Token: 0x17000003 RID: 3
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00017E24 File Offset: 0x00016024
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x00017E3C File Offset: 0x0001603C
		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
				this.bounds.Y = this.bounds.Y + Module.PANEL_HEIGHT;
				this.bounds.Height = this.bounds.Height - Module.PANEL_HEIGHT;
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00017E74 File Offset: 0x00016074
		public Module(Rectangle location, OS operatingSystem)
		{
			location.Y += Module.PANEL_HEIGHT;
			location.Height -= Module.PANEL_HEIGHT;
			this.bounds = location;
			this.os = operatingSystem;
			this.spriteBatch = this.os.ScreenManager.SpriteBatch;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00017EE6 File Offset: 0x000160E6
		public virtual void LoadContent()
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00017EE9 File Offset: 0x000160E9
		public virtual void Update(float t)
		{
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00017EEC File Offset: 0x000160EC
		public virtual void PreDrawStep()
		{
		}

		// Token: 0x060001BD RID: 445 RVA: 0x00017EEF File Offset: 0x000160EF
		public virtual void Draw(float t)
		{
			this.drawFrame();
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00017EF9 File Offset: 0x000160F9
		public virtual void PostDrawStep()
		{
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00017EFC File Offset: 0x000160FC
		public void drawFrame()
		{
			Module.tmpRect = this.bounds;
			Module.tmpRect.Y = Module.tmpRect.Y - Module.PANEL_HEIGHT;
			Module.tmpRect.Height = Module.tmpRect.Height + Module.PANEL_HEIGHT;
			this.spriteBatch.Draw(Utils.white, Module.tmpRect, this.os.moduleColorBacking);
			RenderedRectangle.doRectangleOutline(Module.tmpRect.X, Module.tmpRect.Y, Module.tmpRect.Width, Module.tmpRect.Height, 1, new Color?(this.os.moduleColorSolid));
			Module.tmpRect.Height = Module.PANEL_HEIGHT;
			this.spriteBatch.Draw(Utils.white, Module.tmpRect, this.os.moduleColorStrong);
			this.spriteBatch.DrawString(GuiData.detailfont, this.name, new Vector2((float)(Module.tmpRect.X + 2), (float)(Module.tmpRect.Y + 2)), this.os.semiTransText);
			Module.tmpRect = this.bounds;
			Module.tmpRect.Y = Module.tmpRect.Y - Module.PANEL_HEIGHT;
			Module.tmpRect.Height = Module.tmpRect.Height + Module.PANEL_HEIGHT;
			RenderedRectangle.doRectangleOutline(Module.tmpRect.X, Module.tmpRect.Y, Module.tmpRect.Width, Module.tmpRect.Height, 1, new Color?(this.os.moduleColorSolid));
		}

		// Token: 0x040001CC RID: 460
		public static int PANEL_HEIGHT = 15;

		// Token: 0x040001CD RID: 461
		public Rectangle bounds;

		// Token: 0x040001CE RID: 462
		public SpriteBatch spriteBatch;

		// Token: 0x040001CF RID: 463
		public OS os;

		// Token: 0x040001D0 RID: 464
		public string name = "Unknown";

		// Token: 0x040001D1 RID: 465
		public bool visible = true;

		// Token: 0x040001D2 RID: 466
		private static Rectangle tmpRect;
	}
}
