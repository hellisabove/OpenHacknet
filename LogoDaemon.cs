using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000196 RID: 406
	internal class LogoDaemon : Daemon
	{
		// Token: 0x06000A40 RID: 2624 RVA: 0x000A3604 File Offset: 0x000A1804
		public LogoDaemon(Computer c, OS os, string name, bool showsTitle, string LogoImagePath) : base(c, name, os)
		{
			this.isListed = true;
			this.showsTitle = showsTitle;
			this.LogoImagePath = LogoImagePath;
			this.name = name;
			this.spinner = new TrailLoadingSpinnerEffect(os);
		}

		// Token: 0x06000A41 RID: 2625 RVA: 0x000A3664 File Offset: 0x000A1864
		public override void initFiles()
		{
			base.initFiles();
			if (!string.IsNullOrWhiteSpace(this.BodyText))
			{
				this.comp.files.root.searchForFolder("sys").files.Add(new FileEntry(this.BodyText, "DisplayText.txt"));
			}
		}

		// Token: 0x06000A42 RID: 2626 RVA: 0x000A36D0 File Offset: 0x000A18D0
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.timeOnPage = 0f;
			if (this.LoadedLogo == null && !this.hasTriedToLoadLogo)
			{
				if (!string.IsNullOrWhiteSpace(this.LogoImagePath))
				{
					Utils.LoadImageFromContentOrExtension(this.LogoImagePath, this.os.content, delegate(Texture2D loadedTex)
					{
						this.hasTriedToLoadLogo = true;
						this.LoadedLogo = loadedTex;
					});
				}
			}
			Folder folder = this.comp.files.root.searchForFolder("sys");
			FileEntry fileEntry = folder.searchForFile("DisplayText.txt");
			if (fileEntry != null)
			{
				this.BodyText = fileEntry.data;
			}
		}

		// Token: 0x06000A43 RID: 2627 RVA: 0x000A3784 File Offset: 0x000A1984
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			bounds = Utils.InsetRectangle(bounds, 1);
			sb.Draw(Utils.white, bounds, Color.Black * 0.4f);
			Rectangle dest = new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height / 8);
			Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + dest.Height, bounds.Width, bounds.Height / 8 * 7);
			if (!this.showsTitle)
			{
				rectangle.Y -= dest.Height;
				rectangle.Height += dest.Height;
				rectangle.Y += 6;
				rectangle.Height -= 6;
			}
			int num = 40;
			rectangle.Height -= num;
			if (!string.IsNullOrWhiteSpace(this.BodyText))
			{
				string[] array = this.BodyText.Trim().Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				int num2 = (int)GuiData.smallfont.MeasureString("W").Y;
				num2 += 2;
				int num3 = 6;
				int num4 = num2 * array.Length + num3 * 2;
				rectangle.Height -= num4;
				int num5 = bounds.Y + bounds.Height - num3 - num2 + 1 - num;
				for (int i = array.Length - 1; i >= 0; i--)
				{
					Rectangle dest2 = new Rectangle(bounds.X, num5 + 1, bounds.Width, num2 - 1);
					TextItem.doCenteredFontLabel(dest2, array[i], GuiData.smallfont, this.TextColor, false);
					num5 -= num2;
				}
			}
			if (this.LoadedLogo == null)
			{
				this.timeOnPage += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				this.spinner.Draw(rectangle, sb, 1f, 1f - this.timeOnPage * 0.4f, 0f, new Color?(this.TextColor));
			}
			else
			{
				Utils.DrawSpriteAspectCorrect(rectangle, sb, this.LoadedLogo, Color.White, false);
			}
			if (this.showsTitle)
			{
				TextItem.doCenteredFontLabel(dest, this.name, GuiData.font, this.TextColor, false);
			}
			int num6 = bounds.Width / 2;
			int num7 = 26;
			if (Button.doButton(22928201, bounds.X + (bounds.Width - num6) / 2, bounds.Y + bounds.Height - (num - num7) / 2 - num7, num6, num7, LocaleTerms.Loc("Proceed"), new Color?(Color.Black)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x06000A44 RID: 2628 RVA: 0x000A3A90 File Offset: 0x000A1C90
		public override string getSaveString()
		{
			return string.Format("<LogoDaemon LogoImagePath=\"{0}\" ShowsTitle=\"{1}\" TextColor=\"{2}\" Name=\"{3}\" />", new object[]
			{
				this.LogoImagePath,
				this.showsTitle ? "true" : "false",
				Utils.convertColorToParseableString(this.TextColor),
				this.name
			});
		}

		// Token: 0x04000B8B RID: 2955
		private const string FileName = "DisplayText.txt";

		// Token: 0x04000B8C RID: 2956
		private string LogoImagePath;

		// Token: 0x04000B8D RID: 2957
		private bool showsTitle;

		// Token: 0x04000B8E RID: 2958
		public string BodyText;

		// Token: 0x04000B8F RID: 2959
		public Color TextColor = Color.White;

		// Token: 0x04000B90 RID: 2960
		private Texture2D LoadedLogo;

		// Token: 0x04000B91 RID: 2961
		private bool hasTriedToLoadLogo = false;

		// Token: 0x04000B92 RID: 2962
		private TrailLoadingSpinnerEffect spinner;

		// Token: 0x04000B93 RID: 2963
		private float timeOnPage = 0f;
	}
}
