using System;
using System.Collections.Generic;
using System.Text;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000DF RID: 223
	internal class NotesExe : ExeModule
	{
		// Token: 0x06000475 RID: 1141 RVA: 0x00047E4C File Offset: 0x0004604C
		public NotesExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.IdentifierName = "Notes";
			this.ramCost = Module.PANEL_HEIGHT + 22;
			this.baseRamCost = this.ramCost;
			this.targetRamUse = this.ramCost;
			this.targetIP = this.os.thisComputer.ip;
			if (NotesExe.crossTexture == null)
			{
				NotesExe.crossTexture = this.os.content.Load<Texture2D>("cross");
			}
			if (NotesExe.circleTexture == null)
			{
				NotesExe.circleTexture = this.os.content.Load<Texture2D>("CircleOutline");
			}
		}

		// Token: 0x06000476 RID: 1142 RVA: 0x00047F30 File Offset: 0x00046130
		public override void LoadContent()
		{
			base.LoadContent();
			this.LoadNotesFromDrive();
			Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
			if (folder.searchForFile("Notes_Reopener.bat") == null)
			{
				folder.files.Add(new FileEntry("true", "Notes_Reopener.bat"));
			}
		}

		// Token: 0x06000477 RID: 1143 RVA: 0x00047F9E File Offset: 0x0004619E
		public override void Killed()
		{
			base.Killed();
			this.RemoveReopnener();
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x00047FAF File Offset: 0x000461AF
		public void DisplayOutOfMemoryWarning()
		{
			this.MemoryWarningFlashTime = 4f;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x00047FC0 File Offset: 0x000461C0
		private void RemoveReopnener()
		{
			Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
			for (int i = 0; i < folder.files.Count; i++)
			{
				if (folder.files[i].name == "Notes_Reopener.bat")
				{
					folder.files.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x0004803C File Offset: 0x0004623C
		public static bool NoteExists(string note, OS os)
		{
			for (int i = 0; i < os.exes.Count; i++)
			{
				NotesExe notesExe = os.exes[i] as NotesExe;
				if (notesExe != null)
				{
					return notesExe.HasNote(note);
				}
			}
			return false;
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x000480B0 File Offset: 0x000462B0
		public static void AddNoteToOS(string note, OS os, bool isRecursiveSelfAdd = false)
		{
			for (int i = 0; i < os.exes.Count; i++)
			{
				NotesExe notesExe = os.exes[i] as NotesExe;
				if (notesExe != null)
				{
					notesExe.AddNote(note);
					return;
				}
			}
			if (!isRecursiveSelfAdd)
			{
				os.runCommand("notes");
			}
			Action action = delegate()
			{
				NotesExe.AddNoteToOS(note, os, true);
			};
			os.delayer.Post(ActionDelayer.NextTick(), action);
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00048168 File Offset: 0x00046368
		public override void Update(float t)
		{
			base.Update(t);
			if (this.targetRamUse != this.ramCost)
			{
				if (this.targetRamUse < this.ramCost)
				{
					this.ramCost -= (int)(t * 350f);
					if (this.ramCost < this.targetRamUse)
					{
						this.ramCost = this.targetRamUse;
					}
				}
				else
				{
					int num = (int)(t * 350f);
					if (this.os.ramAvaliable >= num)
					{
						this.ramCost += num;
						if (this.ramCost > this.targetRamUse)
						{
							this.ramCost = this.targetRamUse;
						}
					}
				}
			}
			if (this.gettingNewNote)
			{
				string note = null;
				if (Programs.parseStringFromGetStringCommand(this.os, out note))
				{
					this.gettingNewNote = false;
					this.AddNote(note);
				}
			}
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00048270 File Offset: 0x00046470
		public void AddNote(string note)
		{
			string text = Utils.SuperSmartTwimForWidth(note, this.os.ram.bounds.Width - 8, GuiData.UITinyfont);
			for (int i = 0; i < this.notes.Count; i++)
			{
				if (this.notes[i] == text)
				{
					return;
				}
			}
			this.notes.Add(text);
			this.recalcualteRamCost();
			this.SaveNotesToDrive();
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x000482F4 File Offset: 0x000464F4
		public bool HasNote(string note)
		{
			string b = Utils.SuperSmartTwimForWidth(note, this.os.ram.bounds.Width - 8, GuiData.UITinyfont);
			for (int i = 0; i < this.notes.Count; i++)
			{
				if (this.notes[i] == b)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600047F RID: 1151 RVA: 0x00048364 File Offset: 0x00046564
		private void LoadNotesFromDrive()
		{
			Folder folder = this.os.thisComputer.files.root.searchForFolder("home");
			FileEntry fileEntry = folder.searchForFile("Notes.txt");
			if (fileEntry != null)
			{
				string[] array = fileEntry.data.Split(new string[]
				{
					"\n\n----------\n\n"
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array.Length >= 0)
				{
					this.notes.AddRange(array);
					this.recalcualteRamCost();
				}
			}
		}

		// Token: 0x06000480 RID: 1152 RVA: 0x000483E8 File Offset: 0x000465E8
		private void SaveNotesToDrive()
		{
			Folder folder = this.os.thisComputer.files.root.searchForFolder("home");
			FileEntry fileEntry = folder.searchForFile("Notes.txt");
			if (fileEntry == null)
			{
				fileEntry = new FileEntry("", "Notes.txt");
				folder.files.Add(fileEntry);
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.notes.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append("\n\n----------\n\n");
				}
				stringBuilder.Append(this.notes[i]);
			}
			fileEntry.data = stringBuilder.ToString();
		}

		// Token: 0x06000481 RID: 1153 RVA: 0x000484A8 File Offset: 0x000466A8
		private void recalcualteRamCost()
		{
			this.targetRamUse = this.baseRamCost;
			for (int i = 0; i < this.notes.Count; i++)
			{
				this.targetRamUse += NotesExe.noteBaseCost + this.notes[i].Split(Utils.newlineDelim).Length * NotesExe.noteCostPerLine;
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x00048510 File Offset: 0x00046710
		public override void Draw(float t)
		{
			base.Draw(t);
			if (this.MemoryWarningFlashTime > 0f)
			{
				this.MemoryWarningFlashTime -= (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			}
			if (this.ramCost >= Module.PANEL_HEIGHT)
			{
				this.drawOutline();
				this.drawTarget("");
			}
			else
			{
				base.drawFrame();
			}
			Rectangle dest = new Rectangle(this.bounds.X + 2, this.bounds.Y + 2 + Module.PANEL_HEIGHT, this.bounds.Width - 4, this.ramCost - Module.PANEL_HEIGHT - 22);
			this.DrawNotes(dest);
			if (this.ramCost >= Module.PANEL_HEIGHT + 22)
			{
				Rectangle dest2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + this.bounds.Height - 22, this.bounds.Width - 4, 22);
				this.DrawBasePanel(dest2);
			}
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00048638 File Offset: 0x00046838
		private void DrawBasePanel(Rectangle dest)
		{
			int num = dest.Width - 8;
			int num2 = (int)((double)num * 0.4);
			int width = (int)((double)num * 0.6);
			if (!this.gettingNewNote && Button.doButton(631012 + this.os.exes.IndexOf(this), dest.X + dest.Width - num2, dest.Y + 4, num2, dest.Height - 6, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor * this.fade)))
			{
				if (this.gettingNewNote)
				{
					this.os.terminal.executeLine();
					this.gettingNewNote = false;
				}
				this.Completed();
				this.RemoveReopnener();
				this.isExiting = true;
			}
			if (!this.gettingNewNote && Button.doButton(611014 + this.os.exes.IndexOf(this), dest.X, dest.Y + 4, width, dest.Height - 6, LocaleTerms.Loc("Add Note"), new Color?(this.os.highlightColor * this.fade)))
			{
				this.os.runCommand("getString Note");
				this.os.getStringCache = "";
				this.gettingNewNote = true;
			}
			else if (this.gettingNewNote)
			{
				Rectangle dest2 = new Rectangle(dest.X, dest.Y + 4, num, dest.Height - 6);
				PatternDrawer.draw(dest2, 1f, Color.Transparent, this.os.highlightColor * 0.5f, this.spriteBatch, PatternDrawer.thinStripe);
				TextItem.doFontLabelToSize(dest2, LocaleTerms.Loc("Type In Terminal..."), GuiData.smallfont, Color.White, false, false);
			}
		}

		// Token: 0x06000484 RID: 1156 RVA: 0x00048848 File Offset: 0x00046A48
		private void DrawNotes(Rectangle dest)
		{
			int num = dest.Y;
			for (int i = 0; i < this.notes.Count; i++)
			{
				string[] array = this.notes[i].Split(Utils.newlineDelim);
				int num2 = NotesExe.noteBaseCost + array.Length * NotesExe.noteCostPerLine;
				if (num - dest.Y + num2 > dest.Height + 1)
				{
					break;
				}
				Rectangle destinationRectangle = new Rectangle(dest.X, num + NotesExe.noteBaseCost / 2 - 2, dest.Width, num2 - NotesExe.noteBaseCost / 2);
				this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.highlightColor * 0.2f);
				int num3 = num + NotesExe.noteBaseCost / 2;
				for (int j = 0; j < array.Length; j++)
				{
					this.spriteBatch.DrawString(GuiData.UITinyfont, array[j], new Vector2((float)(destinationRectangle.X + 2), (float)num3), Color.White);
					num3 += NotesExe.noteCostPerLine;
				}
				int num4 = 13;
				if (Button.doButton(539261 + i * 100 + this.os.exes.IndexOf(this) * 2000, destinationRectangle.X + destinationRectangle.Width - num4 - 1, num + NotesExe.noteBaseCost / 2 + 1, num4, num4, "", new Color?(Color.White * 0.5f), NotesExe.crossTexture))
				{
					this.notes.RemoveAt(i);
					this.recalcualteRamCost();
					this.SaveNotesToDrive();
					i--;
				}
				if (this.MemoryWarningFlashTime > 0f)
				{
					float scale = Math.Min(1f, this.MemoryWarningFlashTime);
					Color color = Color.Lerp(this.os.lockedColor, this.os.brightLockedColor, Utils.rand(0.4f)) * scale;
					if (4f - this.MemoryWarningFlashTime < 0.3f)
					{
						float num5 = 1f - (4f - this.MemoryWarningFlashTime) / 0.3f;
						color = Color.Lerp(color, Utils.AddativeWhite, num5 * 0.7f);
					}
					this.spriteBatch.Draw(Utils.white, destinationRectangle, color);
					int num6 = 60;
					int num7 = (destinationRectangle.Height - num6) / 2;
					Rectangle rectangle = new Rectangle(destinationRectangle.X, destinationRectangle.Y + num7, destinationRectangle.Width, num6);
					this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * scale * 0.8f);
					num6 -= 10;
					rectangle.Y += 5;
					rectangle.X += 10;
					rectangle.Width -= 20;
					rectangle.Height = num6 / 2;
					TextItem.doFontLabelToSize(rectangle, Utils.FlipRandomChars(LocaleTerms.Loc("Insufficient Memory"), 0.007000000216066837), GuiData.font, Color.White * scale, false, false);
					rectangle.Y += num6 / 2;
					TextItem.doFontLabelToSize(rectangle, Utils.FlipRandomChars(LocaleTerms.Loc("Close Notes to Free Space"), 0.007000000216066837), GuiData.font, Color.White * scale, false, false);
					Rectangle rectangle2 = new Rectangle(destinationRectangle.X + destinationRectangle.Width - num4 - 1, num + NotesExe.noteBaseCost / 2 + 1, num4, num4);
					this.spriteBatch.Draw(NotesExe.crossTexture, rectangle2, Color.Red * scale);
					double num8 = (1.0 + Math.Sin((double)this.os.timer)) / 2.0;
					rectangle2 = Utils.InsetRectangle(rectangle2, (int)(num8 * -20.0));
					this.spriteBatch.Draw(NotesExe.crossTexture, rectangle2, Utils.AddativeWhite * scale * (float)(1.0 - num8));
				}
				num += num2;
			}
		}

		// Token: 0x04000564 RID: 1380
		private const float RAM_CHANGE_PS = 350f;

		// Token: 0x04000565 RID: 1381
		private const int BASE_PANEL_HEIGHT = 22;

		// Token: 0x04000566 RID: 1382
		private const float NO_MEMORY_WARNING_TIME = 4f;

		// Token: 0x04000567 RID: 1383
		internal const string NotesSaveFilename = "Notes.txt";

		// Token: 0x04000568 RID: 1384
		public const string NotesReopenOnLoadFile = "Notes_Reopener.bat";

		// Token: 0x04000569 RID: 1385
		internal const string NotesSaveFileDelimiter = "\n\n----------\n\n";

		// Token: 0x0400056A RID: 1386
		private static Texture2D crossTexture;

		// Token: 0x0400056B RID: 1387
		private static Texture2D circleTexture;

		// Token: 0x0400056C RID: 1388
		private static int noteBaseCost = 8;

		// Token: 0x0400056D RID: 1389
		private static int noteCostPerLine = 14;

		// Token: 0x0400056E RID: 1390
		public List<string> notes = new List<string>();

		// Token: 0x0400056F RID: 1391
		private int targetRamUse = 100;

		// Token: 0x04000570 RID: 1392
		private int baseRamCost = 100;

		// Token: 0x04000571 RID: 1393
		private bool gettingNewNote = false;

		// Token: 0x04000572 RID: 1394
		public float MemoryWarningFlashTime = 0f;
	}
}
