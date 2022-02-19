using System;
using System.Collections.Generic;
using Hacknet.DLC.MarkovTextGenerator;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000010 RID: 16
	internal class MarkovTextDaemon : Daemon
	{
		// Token: 0x06000084 RID: 132 RVA: 0x0000A4FC File Offset: 0x000086FC
		public MarkovTextDaemon(Computer c, OS os, string name, string corpusLoadPath) : base(c, name, os)
		{
			this.corpusFolderPath = corpusLoadPath;
			this.TextDisplay = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000A568 File Offset: 0x00008768
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.GeneratedSentences.Clear();
			this.TextDisplay.UpdateScroll(0f);
			if (string.IsNullOrWhiteSpace(this.corpusFolderPath))
			{
				throw new NullReferenceException("No corpus folder path found");
			}
			this.CorpusBeingLoaded = true;
			this.loadProgress = 0f;
			string foldername = Utils.GetFileLoadPrefix() + this.corpusFolderPath;
			CorpusGenerator.GenerateCorpusFromFolderThreaded(foldername, new Action<Corpus>(this.CorpusLoaded), new Action<float, string>(this.LoadProgressUpdated));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000A5FC File Offset: 0x000087FC
		private void CorpusLoaded(Corpus c)
		{
			this.CorpusBeingLoaded = false;
			if (c != null)
			{
				this.loadProgress = 1f;
				this.corpus = c;
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000A62D File Offset: 0x0000882D
		private void LoadProgressUpdated(float progress, string progressMessage)
		{
			this.loadProgress = progress;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000A638 File Offset: 0x00008838
		private void AddNewSentence(string sentence)
		{
			if (!string.IsNullOrWhiteSpace(sentence))
			{
				string text = Utils.SuperSmartTwimForWidth(sentence, this.DisplayWidth, GuiData.smallfont);
				string[] collection = text.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
				this.GeneratedSentences.AddRange(collection);
				this.SentenceBeingGenerated = false;
				this.LastSentenceWasError = false;
			}
			else
			{
				this.LastSentenceWasError = true;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000A6D4 File Offset: 0x000088D4
		private void PrepateToClearCorpusOnNavigateAway()
		{
			this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), delegate
			{
				if (this.os.display.command != this.name)
				{
					this.corpus = null;
				}
			});
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000A728 File Offset: 0x00008928
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.PrepateToClearCorpusOnNavigateAway();
			Rectangle dest = Utils.InsetRectangle(bounds, 2);
			this.DisplayWidth = dest.Width;
			if (this.corpus == null && this.CorpusBeingLoaded)
			{
				this.DrawLoading(dest, sb);
			}
			else
			{
				if (Button.doButton(832192223, bounds.X + 20, bounds.Y + 20, bounds.Width / 3, 20, "Generate", null))
				{
					if (!this.SentenceBeingGenerated)
					{
						this.SentenceBeingGenerated = true;
						this.corpus.GenerateSentenceThreaded(new Action<string>(this.AddNewSentence));
					}
				}
				dest.Y += 40;
				dest.Height -= 40;
				this.TextDisplay.Draw(dest, this.GeneratedSentences, sb, Color.White);
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000A82C File Offset: 0x00008A2C
		private void DrawLoading(Rectangle dest, SpriteBatch sb)
		{
			TextItem.doCenteredFontLabel(dest, "Loading...", GuiData.font, Color.White, false);
			Rectangle rectangle = new Rectangle(dest.X, dest.Y + 30, dest.Width, dest.Height);
			TextItem.doCenteredFontLabel(dest, this.loadProgress.ToString("00.00") + "%", GuiData.font, Color.White, false);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000A8A4 File Offset: 0x00008AA4
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<MarkovTextDaemon Name=\"",
				this.name,
				"\" SourceFilesContentFolder=\"",
				this.corpusFolderPath,
				"\" />"
			});
		}

		// Token: 0x04000090 RID: 144
		private Corpus corpus;

		// Token: 0x04000091 RID: 145
		private bool CorpusBeingLoaded = false;

		// Token: 0x04000092 RID: 146
		private bool SentenceBeingGenerated = false;

		// Token: 0x04000093 RID: 147
		private bool LastSentenceWasError = false;

		// Token: 0x04000094 RID: 148
		public string corpusFolderPath;

		// Token: 0x04000095 RID: 149
		private float loadProgress = 0f;

		// Token: 0x04000096 RID: 150
		private int DisplayWidth = 300;

		// Token: 0x04000097 RID: 151
		private List<string> GeneratedSentences = new List<string>();

		// Token: 0x04000098 RID: 152
		private ScrollableTextRegion TextDisplay;
	}
}
