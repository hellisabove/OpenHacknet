using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000089 RID: 137
	internal class BootCrashAssistanceModule : Module
	{
		// Token: 0x060002B6 RID: 694 RVA: 0x00027A74 File Offset: 0x00025C74
		public BootCrashAssistanceModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.bounds = location;
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence0.txt"));
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence1.txt"));
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence2.txt"));
			this.SequenceBlocks.Add("");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/Untranslated/BootFailSequence3.txt"));
			this.SequenceBlocks.Add(">");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel1.txt"));
			this.SequenceBlocks.Add(">");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel2.txt"));
			this.SequenceBlocks.Add(">");
			this.SequenceBlocks.Add(Utils.readEntireFile("Content/DLC/Docs/BootFailSequenceCoel3.txt"));
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00027C1C File Offset: 0x00025E1C
		public override void Update(float t)
		{
			base.Update(t);
			if (this.IsActive)
			{
				this.elapsedTime += t;
				float num = (this.blocksComplete < 11) ? -8.3f : 0f;
				if (!this.AwaitingInput)
				{
					if (this.elapsedTime > num + 9.2f)
					{
						this.elapsedTime = 0f;
						this.blocksComplete++;
						if (this.blocksComplete >= this.SequenceBlocks.Count || (this.ShouldSkipDialogueTypeout && this.blocksComplete == 12))
						{
							this.AwaitingInput = true;
							Game1.getSingleton().IsMouseVisible = true;
							this.blocksComplete = this.SequenceBlocks.Count - 1;
						}
					}
				}
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x00027D08 File Offset: 0x00025F08
		public override void Draw(float t)
		{
			base.Draw(t);
			this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
			Vector2 pos = new Vector2((float)this.bounds.X + 10f, (float)(this.bounds.Y + this.bounds.Height) - (GuiData.ActiveFontConfig.tinyFontCharHeight + 16f));
			Rectangle destinationRectangle = new Rectangle((int)pos.X, (int)pos.Y, 10, 14);
			this.spriteBatch.Draw(Utils.white, destinationRectangle, (this.os.timer % 0.3f < 0.15f) ? Color.White : (Color.White * 0.05f));
			List<string> list = new List<string>();
			Color c = Color.Lerp(Utils.AddativeWhite, Utils.makeColor(204, byte.MaxValue, 249, 0), 0.5f);
			float num = (this.blocksComplete < 11) ? -8.3f : 0f;
			pos.Y -= 6f;
			if (this.AwaitingInput)
			{
				pos.Y -= 46f;
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					if (Button.doButton(464191001, (int)pos.X, (int)pos.Y - 4, 220, 30, LocaleTerms.Loc("Proceed"), new Color?(Color.White)))
					{
						HostileHackerBreakinSequence.CopyHelpFile();
						HostileHackerBreakinSequence.OpenWindowsHelpDocument();
						HostileHackerBreakinSequence.OpenTerminal();
						HostileHackerBreakinSequence.CrashProgram();
					}
				}
				else
				{
					if (Button.doButton(464191002, (int)pos.X, (int)pos.Y - 4, 220, 30, LocaleTerms.Loc("README"), new Color?(Color.White)))
					{
						this.SequenceBlocks.Add("");
						this.SequenceBlocks.Add("");
						this.SequenceBlocks.Add("");
						this.SequenceBlocks.Add(HostileHackerBreakinSequence.GetHelpText());
						this.SequenceBlocks.Add("");
						this.blocksComplete = this.SequenceBlocks.Count - 1;
					}
					if (Button.doButton(464191003, (int)pos.X + 230, (int)pos.Y - 4, 220, 30, LocaleTerms.Loc("Terminal"), new Color?(Color.White)))
					{
						this.SequenceBlocks.Add("---------------------------------------");
						this.SequenceBlocks.Add("TERMINAL SHOULD BE OPEN ON YOUR SYSTEM.");
						this.SequenceBlocks.Add("");
						this.SequenceBlocks.Add("IF NOT, OPEN A TERMINAL TO");
						this.SequenceBlocks.Add(HostileHackerBreakinSequence.OpenTerminal());
						this.SequenceBlocks.Add("---------------------------------------");
						this.SequenceBlocks.Add("");
						this.blocksComplete = this.SequenceBlocks.Count - 1;
					}
					if (Button.doButton(464191001, (int)pos.X + 460, (int)pos.Y - 4, 220, 30, LocaleTerms.Loc("Crash VM"), new Color?(Color.White)))
					{
						HostileHackerBreakinSequence.CopyHelpFile();
						HostileHackerBreakinSequence.CrashProgram();
					}
				}
			}
			if (this.blocksComplete + 1 < this.SequenceBlocks.Count)
			{
				string[] array = this.SequenceBlocks[this.blocksComplete + 1].Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				float num2 = Math.Min(1f, this.elapsedTime / (num + 9.2f)) * (float)(array.Length - 1);
				int num3 = (int)num2;
				for (int i = num3; i >= 0; i--)
				{
					string text = array[i];
					if (i == num3 && this.blocksComplete + 1 >= 11)
					{
						float num4 = (num + 9.2f) / (float)Math.Max(1, array.Length - 1);
						float num5 = Math.Min(num4, this.elapsedTime - (float)num3 * num4);
						int length = (int)((float)text.Length * (num5 / num4));
						text = text.Substring(0, length);
					}
					list.Add(text);
				}
			}
			for (int j = this.blocksComplete; j >= 0; j--)
			{
				string[] array2 = this.SequenceBlocks[j].Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				for (int k = array2.Length - 1; k >= 0; k--)
				{
					list.Add(array2[k]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				pos.Y -= GuiData.ActiveFontConfig.tinyFontCharHeight + 6f;
				this.DrawMonospace(list[j], GuiData.smallfont, pos, c, (float)(LocaleActivator.ActiveLocaleIsCJK() ? 13 : 10));
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00028278 File Offset: 0x00026478
		private void DrawMonospace(string text, SpriteFont font, Vector2 pos, Color c, float charWidth)
		{
			for (int i = 0; i < text.Length; i++)
			{
				this.spriteBatch.DrawString(font, string.Concat(text[i]), Utils.ClipVec2ForTextRendering(pos), c);
				pos.X += charWidth;
			}
		}

		// Token: 0x040002E1 RID: 737
		private const float TimePerBlock = 9.2f;

		// Token: 0x040002E2 RID: 738
		private const float TimeSubForEarlyLines = -8.3f;

		// Token: 0x040002E3 RID: 739
		private const int NumberOfFastBlocks = 11;

		// Token: 0x040002E4 RID: 740
		public bool IsActive = false;

		// Token: 0x040002E5 RID: 741
		private float elapsedTime = 0f;

		// Token: 0x040002E6 RID: 742
		private List<string> SequenceBlocks = new List<string>();

		// Token: 0x040002E7 RID: 743
		private int blocksComplete = 0;

		// Token: 0x040002E8 RID: 744
		private bool AwaitingInput = false;

		// Token: 0x040002E9 RID: 745
		internal bool ShouldSkipDialogueTypeout = false;
	}
}
