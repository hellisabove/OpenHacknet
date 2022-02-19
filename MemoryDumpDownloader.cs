using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000064 RID: 100
	internal class MemoryDumpDownloader : ExeModule
	{
		// Token: 0x060001EF RID: 495 RVA: 0x0001AF38 File Offset: 0x00019138
		private MemoryDumpDownloader(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.name = "MemDumpGenerator";
			this.ramCost = 80;
			this.IdentifierName = "MemoryDumpGenerator";
			this.target = ((this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp);
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0001AFC0 File Offset: 0x000191C0
		public static MemoryDumpDownloader GenerateInstanceOrNullFromArguments(string[] args, Rectangle location, object osObj, Computer target)
		{
			OS os = (OS)osObj;
			MemoryDumpDownloader result;
			if (os.hasConnectionPermission(true))
			{
				result = new MemoryDumpDownloader(location, os);
			}
			else
			{
				os.write(LocaleTerms.Loc("Admin access required to generate memory dump"));
				result = null;
			}
			return result;
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0001B004 File Offset: 0x00019204
		public override void Update(float t)
		{
			base.Update(t);
			this.elapsedTime += t;
			if (this.DownloadComplete)
			{
				if (this.elapsedTime > 5f)
				{
					this.isExiting = true;
				}
			}
			else if (this.elapsedTime > 2f && this.target.Memory == null)
			{
				this.DownloadComplete = true;
				this.DidFail = true;
				this.elapsedTime = 0f;
			}
			else if (this.elapsedTime > 6f && this.target.Memory != null)
			{
				this.DownloadComplete = true;
				this.DidFail = false;
				this.elapsedTime = 0f;
				this.DownloadMemoryDump();
			}
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0001B0E4 File Offset: 0x000192E4
		private void DownloadMemoryDump()
		{
			string encodedFileString = this.target.Memory.GetEncodedFileString();
			if (this.os.thisComputer == this.target)
			{
				List<string> list = new List<string>();
				for (int i = 0; i < this.os.exes.Count; i++)
				{
					NotesExe notesExe = this.os.exes[i] as NotesExe;
					if (notesExe != null)
					{
						list.AddRange(notesExe.notes);
					}
				}
				MemoryContents memoryContents = new MemoryContents
				{
					DataBlocks = new List<string>(list.ToArray()),
					CommandsRun = this.os.terminal.GetRecentTerminalHistoryList()
				};
				encodedFileString = memoryContents.GetEncodedFileString();
			}
			string text = this.getReportFilename(this.target.name);
			Folder folder = this.os.thisComputer.files.root.searchForFolder("home");
			Folder folder2 = folder.searchForFolder("MemDumps");
			if (folder2 == null)
			{
				folder.folders.Add(new Folder("MemDumps"));
				folder2 = folder.searchForFolder("MemDumps");
			}
			text = Utils.GetNonRepeatingFilename(text, ".mem", folder2);
			folder2.files.Add(new FileEntry(encodedFileString, text));
			this.savedFileName = "home/MemDumps/" + text;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0001B268 File Offset: 0x00019468
		private string getReportFilename(string s)
		{
			return Utils.CleanStringToRenderable(s).Replace(" ", "_").ToLower() + "_dump";
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0001B29E File Offset: 0x0001949E
		public override void Completed()
		{
			base.Completed();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0001B2A8 File Offset: 0x000194A8
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle contentAreaDest = base.GetContentAreaDest();
			if (contentAreaDest.Height >= 5)
			{
				PatternDrawer.draw(contentAreaDest, 0.1f, Color.Transparent, this.os.highlightColor * 0.1f, this.spriteBatch, PatternDrawer.wipTile);
				int num = Math.Min(contentAreaDest.Height / 2, 30);
				Rectangle rectangle = new Rectangle(contentAreaDest.X, contentAreaDest.Y + contentAreaDest.Height / 2 - num, contentAreaDest.Width, num);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
				if (this.DownloadComplete)
				{
					this.spriteBatch.Draw(Utils.white, rectangle, this.DidFail ? (Color.Red * 0.5f) : (this.os.highlightColor * 0.5f));
					TextItem.doFontLabelToSize(Utils.InsetRectangle(rectangle, 4), this.DidFail ? LocaleTerms.Loc("Empty Scan Detected") : LocaleTerms.Loc("Download Complete"), GuiData.smallfont, Utils.AddativeWhite, true, false);
					rectangle.Y += rectangle.Height;
					rectangle.Height -= 4;
					rectangle.X += 4;
					rectangle.Width -= 8;
					if (!this.DidFail)
					{
						TextItem.doFontLabelToSize(rectangle, this.savedFileName, GuiData.font, Utils.AddativeWhite, true, false);
					}
				}
				else
				{
					float num2 = this.elapsedTime / 6f;
					rectangle.Width = (int)((float)rectangle.Width * num2);
					this.spriteBatch.Draw(Utils.white, rectangle, this.os.highlightColor * 0.5f);
				}
			}
		}

		// Token: 0x04000219 RID: 537
		private const float DownloadTime = 6f;

		// Token: 0x0400021A RID: 538
		private const float FailTime = 2f;

		// Token: 0x0400021B RID: 539
		private const float ExitTime = 5f;

		// Token: 0x0400021C RID: 540
		private bool DownloadComplete = false;

		// Token: 0x0400021D RID: 541
		private bool DidFail = false;

		// Token: 0x0400021E RID: 542
		private string savedFileName = "Unknown";

		// Token: 0x0400021F RID: 543
		private float elapsedTime = 0f;

		// Token: 0x04000220 RID: 544
		private Computer target;
	}
}
