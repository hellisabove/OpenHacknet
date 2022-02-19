using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000155 RID: 341
	internal class RamModule : CoreModule
	{
		// Token: 0x06000894 RID: 2196 RVA: 0x0009110F File Offset: 0x0008F30F
		public RamModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
		}

		// Token: 0x06000895 RID: 2197 RVA: 0x00091134 File Offset: 0x0008F334
		public override void LoadContent()
		{
			base.LoadContent();
			this.infoBar = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
			this.infoBarUsedRam = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
			this.infoStringPos = new Vector2((float)this.infoBar.X, (float)this.infoBar.Y);
		}

		// Token: 0x06000896 RID: 2198 RVA: 0x000911DC File Offset: 0x0008F3DC
		public override void Update(float t)
		{
			base.Update(t);
			this.infoBar = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
			this.infoString = string.Concat(new object[]
			{
				"USED RAM: ",
				this.os.totalRam - this.os.ramAvaliable,
				"mb / ",
				this.os.totalRam,
				"mb"
			});
			this.infoBarUsedRam = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, this.bounds.Width - 2, RamModule.contentStartOffset);
			if (this.OutOfMemoryFlashTime > 0f)
			{
				this.OutOfMemoryFlashTime -= t;
			}
		}

		// Token: 0x06000897 RID: 2199 RVA: 0x000912E0 File Offset: 0x0008F4E0
		public void FlashMemoryWarning()
		{
			this.OutOfMemoryFlashTime = RamModule.FLASH_TIME;
			for (int i = 0; i < this.os.exes.Count; i++)
			{
				NotesExe notesExe = this.os.exes[i] as NotesExe;
				if (notesExe != null)
				{
					notesExe.DisplayOutOfMemoryWarning();
				}
			}
		}

		// Token: 0x06000898 RID: 2200 RVA: 0x00091340 File Offset: 0x0008F540
		public override void Draw(float t)
		{
			base.Draw(t);
			this.spriteBatch.Draw(Utils.white, this.infoBar, this.os.indentBackgroundColor);
			this.infoBarUsedRam.Width = (int)((float)this.infoBar.Width * (1f - (float)this.os.ramAvaliable / (float)this.os.totalRam));
			this.spriteBatch.Draw(Utils.white, this.infoBarUsedRam, RamModule.USED_RAM_COLOR);
			this.spriteBatch.DrawString(GuiData.detailfont, this.infoString, new Vector2((float)this.infoBar.X, (float)this.infoBar.Y), Color.White);
			this.spriteBatch.DrawString(GuiData.detailfont, string.Concat(this.os.exes.Count), new Vector2((float)(this.bounds.X + this.bounds.Width - ((this.os.exes.Count >= 10) ? 24 : 12)), (float)this.infoBar.Y), Color.White);
			if (this.OutOfMemoryFlashTime > 0f)
			{
				float scale = Math.Min(1f, this.OutOfMemoryFlashTime);
				float amount = Math.Max(0f, this.OutOfMemoryFlashTime - (RamModule.FLASH_TIME - 1f));
				Color patternColor = Color.Lerp(this.os.lockedColor, Utils.AddativeRed, amount) * scale;
				PatternDrawer.draw(this.bounds, 0f, Color.Transparent, patternColor, this.spriteBatch, PatternDrawer.errorTile);
				int num = 40;
				Rectangle rectangle = new Rectangle(this.bounds.X, this.bounds.Y + this.bounds.Height - num - 1, this.bounds.Width, num);
				this.spriteBatch.Draw(Utils.white, Utils.InsetRectangle(rectangle, 4), Color.Black * 0.75f);
				rectangle.X--;
				string text = " ^ " + LocaleTerms.Loc("INSUFFICIENT MEMORY") + " ^ ";
				TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.Black * scale, false, false);
				rectangle.X += 2;
				TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.Black * scale, false, false);
				rectangle.X--;
				TextItem.doFontLabelToSize(rectangle, text, GuiData.font, Color.White * scale, false, false);
			}
		}

		// Token: 0x06000899 RID: 2201 RVA: 0x00091604 File Offset: 0x0008F804
		public virtual void drawOutline()
		{
			Rectangle bounds = this.bounds;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.outlineColor);
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor);
		}

		// Token: 0x040009FB RID: 2555
		public static int contentStartOffset = 16;

		// Token: 0x040009FC RID: 2556
		public static int MODULE_WIDTH = 252;

		// Token: 0x040009FD RID: 2557
		public static Color USED_RAM_COLOR = new Color(60, 60, 67);

		// Token: 0x040009FE RID: 2558
		public static float FLASH_TIME = 3f;

		// Token: 0x040009FF RID: 2559
		private string infoString = "";

		// Token: 0x04000A00 RID: 2560
		private Vector2 infoStringPos;

		// Token: 0x04000A01 RID: 2561
		private Rectangle infoBar;

		// Token: 0x04000A02 RID: 2562
		private Rectangle infoBarUsedRam;

		// Token: 0x04000A03 RID: 2563
		private float OutOfMemoryFlashTime = 0f;
	}
}
