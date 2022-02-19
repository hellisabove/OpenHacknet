using System;
using System.Collections.Generic;
using System.Globalization;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000F6 RID: 246
	internal class AcademicDatabaseDaemon : Daemon
	{
		// Token: 0x0600053E RID: 1342 RVA: 0x00051FE8 File Offset: 0x000501E8
		public AcademicDatabaseDaemon(Computer c, string serviceName, OS os) : base(c, serviceName, os)
		{
			this.themeColor = new Color(53, 96, 156);
			this.backThemeColor = new Color(27, 58, 102);
			this.darkThemeColor = new Color(12, 20, 40, 100);
			this.init();
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00052060 File Offset: 0x00050260
		private void init()
		{
			this.backBars = new List<Vector2>();
			this.topBars = new List<Vector2>();
			this.searchedDegrees = new List<Degree>();
			this.searchResultsNames = new List<string>();
			for (int i = 0; i < 5; i++)
			{
				this.backBars.Add(new Vector2(0.5f, Utils.randm(1f)));
				this.topBars.Add(new Vector2(0.5f, Utils.randm(1f)));
			}
			this.loadingCircle = this.os.content.Load<Texture2D>("Sprites/Spinner");
		}

		// Token: 0x06000540 RID: 1344 RVA: 0x00052108 File Offset: 0x00050308
		public override void initFiles()
		{
			this.root = this.comp.files.root.searchForFolder("academic_data");
			if (this.root == null)
			{
				this.root = new Folder("academic_data");
				this.comp.files.root.folders.Add(this.root);
			}
			this.entries = new Folder("entry_cache");
			this.root.folders.Add(this.entries);
			string dataEntry = Utils.readEntireFile("Content/LocPost/AcademicDatabaseInfo.txt");
			FileEntry item = new FileEntry(dataEntry, "info.txt");
			this.root.files.Add(item);
			this.infoText = dataEntry;
			this.initFilesFromPeople(People.all);
		}

		// Token: 0x06000541 RID: 1345 RVA: 0x000521DC File Offset: 0x000503DC
		public override void loadInit()
		{
			this.root = this.comp.files.root.searchForFolder("academic_data");
			this.entries = this.root.searchForFolder("entry_cache");
			FileEntry fileEntry = this.root.searchForFile("info.txt");
			this.infoText = ((fileEntry != null) ? fileEntry.data : LocaleTerms.Loc("DESCRIPTION FILE NOT FOUND"));
		}

		// Token: 0x06000542 RID: 1346 RVA: 0x00052250 File Offset: 0x00050450
		public void initFilesFromPeople(List<Person> people = null)
		{
			if (people == null)
			{
				people = People.all;
			}
			for (int i = 0; i < people.Count; i++)
			{
				if (people[i].degrees.Count > 0)
				{
					this.addFileForPerson(people[i]);
				}
			}
		}

		// Token: 0x06000543 RID: 1347 RVA: 0x000522B2 File Offset: 0x000504B2
		private void addFileForPerson(Person p)
		{
			this.entries.files.Add(this.getFileForPerson(p));
		}

		// Token: 0x06000544 RID: 1348 RVA: 0x000522D0 File Offset: 0x000504D0
		private FileEntry getFileForPerson(Person p)
		{
			FileEntry fileEntry = new FileEntry();
			fileEntry.name = this.convertNameToFileNameStart(p.FullName);
			int num = 0;
			while (this.entries.searchForFile(fileEntry.name) != null)
			{
				if (num == 0)
				{
					fileEntry.name = fileEntry.name.Substring(0, fileEntry.name.Length - 1) + num;
				}
				else
				{
					FileEntry fileEntry2 = fileEntry;
					fileEntry2.name += "1";
				}
				num++;
			}
			string text = p.FullName + "\n--------------------\n";
			for (int i = 0; i < p.degrees.Count; i++)
			{
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					p.degrees[i].name,
					"\n",
					p.degrees[i].uni,
					"\n",
					p.degrees[i].GPA
				});
				text += "\n--------------------";
			}
			fileEntry.data = text;
			return fileEntry;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00052430 File Offset: 0x00050630
		public string convertNameToFileNameStart(string name)
		{
			return name.ToLower().Replace(" ", "_");
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x00052458 File Offset: 0x00050658
		public FileEntry findFileForName(string name)
		{
			FileEntry fileEntry = null;
			this.searchResultsNames.Clear();
			string value = this.convertNameToFileNameStart(name);
			for (int i = 0; i < this.entries.files.Count; i++)
			{
				if (this.entries.files[i].name.StartsWith(value))
				{
					string data = this.entries.files[i].data;
					if (fileEntry == null)
					{
						fileEntry = this.entries.files[i];
					}
					this.searchResultsNames.Add(data.Substring(0, data.IndexOf('\n')));
				}
			}
			FileEntry result;
			if (this.searchResultsNames.Count > 1)
			{
				result = null;
			}
			else if (fileEntry == null)
			{
				result = null;
			}
			else
			{
				this.foundFileName = fileEntry.name;
				result = fileEntry;
			}
			return result;
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x00052564 File Offset: 0x00050764
		private void setDegreesFromFileEntryData(string file)
		{
			string[] separator = new string[]
			{
				"--------------------"
			};
			string[] array = file.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			CultureInfo provider = new CultureInfo("en-au");
			this.searchedName = array[0];
			this.searchedDegrees.Clear();
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
				if (array2.Length >= 3)
				{
					string degreeName = array2[0];
					string uniName = array2[1];
					float degreeGPA = (float)Convert.ToDouble(array2[2], provider);
					Degree item = new Degree(degreeName, uniName, degreeGPA);
					this.searchedDegrees.Add(item);
				}
			}
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00052618 File Offset: 0x00050818
		public bool doesDegreeExist(string owner_name, string degree_name, string uni_name, float gpaMin)
		{
			List<Degree> list = this.searchedDegrees;
			FileEntry fileEntry = this.findFileForName(owner_name);
			bool result;
			if (fileEntry != null)
			{
				this.setDegreesFromFileEntryData(fileEntry.data);
				bool flag = false;
				for (int i = 0; i < this.searchedDegrees.Count; i++)
				{
					if (this.searchedDegrees[i].GPA >= gpaMin && (degree_name == null || this.searchedDegrees[i].name.ToLower().Contains(degree_name.ToLower())) && (uni_name == null || this.searchedDegrees[i].uni.ToLower().Equals(uni_name.ToLower())))
					{
						flag = true;
						break;
					}
				}
				this.searchedDegrees = list;
				result = flag;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x000526FC File Offset: 0x000508FC
		public bool hasDegrees(string owner_name)
		{
			FileEntry fileEntry = this.findFileForName(owner_name);
			bool result;
			if (fileEntry == null)
			{
				result = false;
			}
			else
			{
				List<Degree> list = this.searchedDegrees;
				this.setDegreesFromFileEntryData(fileEntry.data);
				bool flag = true;
				if (this.searchedDegrees.Count <= 0)
				{
					flag = false;
				}
				this.searchedDegrees = list;
				result = flag;
			}
			return result;
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x0005275C File Offset: 0x0005095C
		private void doPreEntryViewSearch()
		{
			FileEntry fileEntry = this.findFileForName(this.searchedName);
			if (fileEntry != null)
			{
				this.setDegreesFromFileEntryData(fileEntry.data);
			}
			else if (this.searchResultsNames.Count > 1)
			{
				this.state = AcademicDatabaseDaemon.ADDState.MultipleEntriesFound;
			}
			else
			{
				this.state = AcademicDatabaseDaemon.ADDState.EntryNotFound;
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x000527BC File Offset: 0x000509BC
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			this.drawSideBar(bounds, sb);
			int num = (int)((double)bounds.Width / 5.0);
			bounds.Width -= num;
			bounds.X += num;
			this.drawTitle(bounds, sb);
			bounds.Y += 30;
			bounds.Height -= 30;
			switch (this.state)
			{
			case AcademicDatabaseDaemon.ADDState.Welcome:
			{
				bool flag = this.comp.adminIP == this.os.thisComputer.ip;
				Rectangle destinationRectangle = bounds;
				destinationRectangle.Y = bounds.Y + 60 - 20;
				destinationRectangle.Height = 22;
				sb.Draw(Utils.white, destinationRectangle, flag ? this.themeColor : this.darkThemeColor);
				string text = LocaleTerms.Loc("Valid Administrator Account Detected");
				if (!flag)
				{
					text = LocaleTerms.Loc("Non-Admin Account Active");
				}
				Vector2 vector = GuiData.smallfont.MeasureString(text);
				Vector2 position = new Vector2((float)(destinationRectangle.X + destinationRectangle.Width / 2) - vector.X / 2f, (float)destinationRectangle.Y);
				sb.DrawString(GuiData.smallfont, text, position, Color.Black);
				if (Button.doButton(456011, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15, bounds.Width / 2, 40, LocaleTerms.Loc("About This Server"), new Color?(this.themeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.InfoPanel;
				}
				if (Button.doButton(456001, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15 + 50, bounds.Width / 2, 40, LocaleTerms.Loc("Search Entries"), new Color?(this.themeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Seach;
					this.os.execute("getString Name");
				}
				if (Button.doButton(456005, bounds.X + 30, bounds.Y + bounds.Height / 2 - 15 + 100, bounds.Width / 2, 40, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
				{
					this.os.display.command = "connect";
				}
				break;
			}
			case AcademicDatabaseDaemon.ADDState.Seach:
				this.drawSearchState(bounds, sb);
				break;
			case AcademicDatabaseDaemon.ADDState.MultiMatchSearch:
			case AcademicDatabaseDaemon.ADDState.PendingResult:
			{
				if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Welcome;
				}
				Vector2 position = new Vector2((float)(bounds.X + bounds.Width / 2), (float)(bounds.Y + bounds.Height / 2));
				Vector2 origin = new Vector2((float)(this.loadingCircle.Width / 2), (float)(this.loadingCircle.Height / 2));
				sb.Draw(this.loadingCircle, position, null, Color.White, (float)((double)this.os.timer % 3.141592653589793 * 3.0), origin, Vector2.One, SpriteEffects.None, 0.5f);
				float num2 = this.os.timer - this.searchStartTime;
				if ((this.state == AcademicDatabaseDaemon.ADDState.PendingResult && num2 > 3.6f) || (this.state == AcademicDatabaseDaemon.ADDState.MultiMatchSearch && num2 > 0.7f))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Entry;
					this.needsDeletionConfirmation = true;
					this.doPreEntryViewSearch();
				}
				break;
			}
			case AcademicDatabaseDaemon.ADDState.Entry:
			case AcademicDatabaseDaemon.ADDState.EditPerson:
				this.drawEntryState(bounds, sb);
				break;
			case AcademicDatabaseDaemon.ADDState.EntryNotFound:
				if (Button.doButton(456010, bounds.X + 2, bounds.Y + 20, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Welcome;
				}
				if (Button.doButton(456015, bounds.X + 2, bounds.Y + 55, 160, 30, LocaleTerms.Loc("Search Again"), new Color?(this.darkThemeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Seach;
					this.os.execute("getString Name");
				}
				TextItem.doFontLabel(new Vector2((float)(bounds.X + 2), (float)(bounds.Y + 90)), LocaleTerms.Loc("No Entries Found"), GuiData.font, null, float.MaxValue, float.MaxValue, false);
				break;
			case AcademicDatabaseDaemon.ADDState.MultipleEntriesFound:
				this.drawMultipleEntriesState(bounds, sb);
				break;
			case AcademicDatabaseDaemon.ADDState.InfoPanel:
			{
				if (Button.doButton(456010, bounds.X + 2, bounds.Y + 30, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
				{
					this.state = AcademicDatabaseDaemon.ADDState.Welcome;
				}
				Vector2 pos = new Vector2((float)(bounds.X + 20), (float)(bounds.Y + 70));
				TextItem.doFontLabel(pos, LocaleTerms.Loc("Information"), GuiData.font, null, float.MaxValue, float.MaxValue, false);
				pos.Y += 40f;
				FileEntry fileEntry = this.root.searchForFile("info.txt");
				string text = "ERROR: Unhandled System.IO.FileNotFoundException\nFile \"info.txt\" was not found";
				if (fileEntry != null)
				{
					text = DisplayModule.cleanSplitForWidth(fileEntry.data, bounds.Width - 80);
				}
				TextItem.DrawShadow = false;
				TextItem.doFontLabel(pos, text, GuiData.smallfont, null, (float)(bounds.Width - 40), float.MaxValue, false);
				break;
			}
			case AcademicDatabaseDaemon.ADDState.EditEntry:
				this.drawEditDegreeState(bounds, sb);
				break;
			}
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00052E00 File Offset: 0x00051000
		private void drawSearchState(Rectangle bounds, SpriteBatch sb)
		{
			if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Welcome;
			}
			string text = "";
			int num = bounds.X + 6;
			int num2 = bounds.Y + 100;
			num2 += (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Please enter the name you with to search for:"), null).Y + 10;
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			string[] array = this.os.getStringCache.Split(separator, StringSplitOptions.None);
			if (array.Length > 1)
			{
				text = array[1];
				if (text.Equals(""))
				{
					text = this.os.terminal.currentLine;
				}
			}
			Rectangle destinationRectangle = new Rectangle(num, num2, bounds.Width - 12, 80);
			sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
			num2 += 28;
			destinationRectangle.X = num + (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Name") + ": " + text, null).X + 2;
			destinationRectangle.Y = num2;
			destinationRectangle.Width = 7;
			destinationRectangle.Height = 20;
			if (this.os.timer % 1f < 0.3f)
			{
				sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
			}
			num2 += 122;
			if (array.Length > 2 || Button.doButton(30, num, num2, 300, 22, LocaleTerms.Loc("Search"), new Color?(this.os.highlightColor)))
			{
				if (array.Length <= 2)
				{
					this.os.terminal.executeLine();
				}
				if (text.Length > 0)
				{
					this.state = AcademicDatabaseDaemon.ADDState.PendingResult;
					this.searchedName = text;
					this.searchStartTime = this.os.timer;
					this.comp.log("ACADEMIC_DATABASE::RecordSearch_:_" + text);
				}
				else
				{
					this.state = AcademicDatabaseDaemon.ADDState.EntryNotFound;
				}
			}
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x0005308C File Offset: 0x0005128C
		private void drawMultipleEntriesState(Rectangle bounds, SpriteBatch sb)
		{
			float num = 22f;
			float num2 = 2f;
			Vector2 pos = new Vector2((float)bounds.X + 20f, (float)bounds.Y + 10f);
			int num3 = (int)Math.Min((float)this.searchResultsNames.Count, ((float)bounds.Height - 40f - 40f - 80f) / (num + num2));
			TextItem.doFontLabel(pos, LocaleTerms.Loc("Multiple Matches"), GuiData.font, null, float.MaxValue, float.MaxValue, false);
			pos.Y += 30f;
			if (num3 > this.searchResultsNames.Count)
			{
				TextItem.doFontLabel(new Vector2(pos.X, pos.Y - 18f), LocaleTerms.Loc("Some Results Omitted"), GuiData.tinyfont, null, float.MaxValue, float.MaxValue, false);
			}
			sb.Draw(Utils.white, new Rectangle((int)pos.X, (int)pos.Y, (int)((float)bounds.Width - pos.X - 5f), 2), Color.White);
			pos.Y += 12f;
			for (int i = 0; i < num3; i++)
			{
				if (Button.doButton(1237000 + i, (int)pos.X, (int)pos.Y, (int)((double)bounds.Width * 0.666), (int)num, this.searchResultsNames[i], new Color?(this.darkThemeColor)))
				{
					this.searchedName = this.searchResultsNames[i];
					this.state = AcademicDatabaseDaemon.ADDState.MultiMatchSearch;
					this.searchStartTime = this.os.timer;
				}
				pos.Y += num + num2;
			}
			pos.Y += 5f;
			sb.Draw(Utils.white, new Rectangle((int)pos.X, (int)pos.Y, (int)((float)bounds.Width - pos.X - 5f), 2), Color.White);
			pos.Y += 10f;
			if (Button.doButton(12346080, (int)pos.X, (int)pos.Y, 160, 25, LocaleTerms.Loc("Refine Search"), new Color?(this.themeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Seach;
				this.os.execute("getString Name");
			}
			if (Button.doButton(12346085, (int)pos.X + 170, (int)pos.Y, 160, 25, LocaleTerms.Loc("Go Back"), new Color?(this.darkThemeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Welcome;
			}
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x000533A0 File Offset: 0x000515A0
		private void drawEntryState(Rectangle bounds, SpriteBatch sb)
		{
			if (this.state == AcademicDatabaseDaemon.ADDState.Entry && this.os.hasConnectionPermission(true))
			{
				this.state = AcademicDatabaseDaemon.ADDState.EditPerson;
			}
			if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Welcome;
			}
			float num = (float)bounds.X + 20f;
			float num2 = (float)bounds.Y + 50f;
			TextItem.doFontLabel(new Vector2(num, num2), this.searchedName, GuiData.font, null, (float)bounds.Width - (num - (float)bounds.X), 60f, false);
			num2 += 30f;
			if (this.searchedDegrees.Count == 0)
			{
				TextItem.doFontLabel(new Vector2(num, num2), " -" + LocaleTerms.Loc("No Degrees Found"), GuiData.smallfont, null, float.MaxValue, float.MaxValue, false);
			}
			for (int i = 0; i < this.searchedDegrees.Count; i++)
			{
				string text = string.Concat(new string[]
				{
					LocaleTerms.Loc("Degree"),
					" :",
					this.searchedDegrees[i].name,
					"\n",
					LocaleTerms.Loc("Uni"),
					"      :",
					this.searchedDegrees[i].uni
				});
				text = text + "\nGPA      :" + this.searchedDegrees[i].GPA;
				TextItem.doFontLabel(new Vector2(num, num2), text, GuiData.smallfont, null, (float)bounds.Width - ((float)bounds.X - num), 50f, false);
				num2 += 60f;
				if (this.state == AcademicDatabaseDaemon.ADDState.EditPerson)
				{
					num2 -= 10f;
					if (Button.doButton(457900 + i, (int)num, (int)num2, 100, 20, LocaleTerms.Loc("Edit"), null))
					{
						this.state = AcademicDatabaseDaemon.ADDState.EditEntry;
						this.editedField = AcademicDatabaseDaemon.ADDEditField.None;
						this.editedIndex = i;
					}
					if (Button.doButton(456900 + i, (int)num + 105, (int)num2, 100, 20, this.needsDeletionConfirmation ? LocaleTerms.Loc("Delete") : LocaleTerms.Loc("Confirm?"), new Color?(this.needsDeletionConfirmation ? Color.Gray : Color.Red)))
					{
						if (this.needsDeletionConfirmation)
						{
							this.needsDeletionConfirmation = false;
						}
						else
						{
							this.comp.log(string.Concat(new object[]
							{
								"ACADEMIC_DATABASE::RecordDeletion_:_#",
								i,
								"_: ",
								this.searchedName.Replace(" ", "_")
							}));
							this.searchedDegrees.RemoveAt(i);
							this.saveChangesToEntry();
							i--;
							this.needsDeletionConfirmation = true;
						}
					}
					num2 += 35f;
				}
			}
			num2 += 10f;
			if (this.state == AcademicDatabaseDaemon.ADDState.EditPerson && Button.doButton(458009, (int)num, (int)num2, 200, 30, LocaleTerms.Loc("Add Degree"), new Color?(this.themeColor)))
			{
				Degree item = new Degree("UNKNOWN", "UNKNOWN", 0f);
				this.searchedDegrees.Add(item);
				this.editedIndex = this.searchedDegrees.IndexOf(item);
				this.state = AcademicDatabaseDaemon.ADDState.EditEntry;
				this.comp.log(string.Concat(new object[]
				{
					"ACADEMIC_DATABASE::RecordAdd_:_#",
					this.editedIndex,
					"_: ",
					this.searchedName.Replace(" ", "_")
				}));
			}
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x00053808 File Offset: 0x00051A08
		private void drawEditDegreeState(Rectangle bounds, SpriteBatch sb)
		{
			if (Button.doButton(456010, bounds.X + 2, bounds.Y + 2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.darkThemeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Entry;
			}
			float x = (float)bounds.X + 10f;
			float y = (float)bounds.Y + 60f;
			Vector2 pos = new Vector2(x, y);
			bool flag = this.editedField == AcademicDatabaseDaemon.ADDEditField.None;
			TextItem.doSmallLabel(pos, LocaleTerms.Loc("University") + ":", null);
			pos.X += 110f;
			TextItem.doSmallLabel(pos, this.searchedDegrees[this.editedIndex].uni, new Color?((this.editedField == AcademicDatabaseDaemon.ADDEditField.Uni) ? this.themeColor : Color.White));
			if (this.editedField == AcademicDatabaseDaemon.ADDEditField.Uni)
			{
				Vector2 vector = GuiData.smallfont.MeasureString(this.searchedDegrees[this.editedIndex].uni);
				Rectangle destinationRectangle = new Rectangle((int)(pos.X + vector.X + 4f), (int)pos.Y, 10, 16);
				if (this.os.timer % 0.6f < 0.3f)
				{
					sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
				}
			}
			pos.X -= 110f;
			pos.Y += 20f;
			if (flag && Button.doButton(46700, (int)pos.X, (int)pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
			{
				this.editedField = AcademicDatabaseDaemon.ADDEditField.Uni;
				this.os.execute("getString University");
			}
			pos.Y += 30f;
			TextItem.doSmallLabel(pos, LocaleTerms.Loc("Degree") + ":", null);
			pos.X += 110f;
			TextItem.doSmallLabel(pos, this.searchedDegrees[this.editedIndex].name, new Color?((this.editedField == AcademicDatabaseDaemon.ADDEditField.Degree) ? this.themeColor : Color.White));
			if (this.editedField == AcademicDatabaseDaemon.ADDEditField.Degree)
			{
				Vector2 vector = GuiData.smallfont.MeasureString(this.searchedDegrees[this.editedIndex].name);
				Rectangle destinationRectangle = new Rectangle((int)(pos.X + vector.X + 4f), (int)pos.Y, 10, 16);
				if (this.os.timer % 0.6f < 0.3f)
				{
					sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
				}
			}
			pos.X -= 110f;
			pos.Y += 20f;
			if (flag && Button.doButton(46705, (int)pos.X, (int)pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
			{
				this.editedField = AcademicDatabaseDaemon.ADDEditField.Degree;
				this.os.execute("getString Degree");
			}
			pos.Y += 30f;
			TextItem.doSmallLabel(pos, "GPA:", null);
			pos.X += 110f;
			TextItem.doSmallLabel(pos, string.Concat(this.searchedDegrees[this.editedIndex].GPA), new Color?((this.editedField == AcademicDatabaseDaemon.ADDEditField.GPA) ? this.themeColor : Color.White));
			if (this.editedField == AcademicDatabaseDaemon.ADDEditField.GPA)
			{
				Vector2 vector = GuiData.smallfont.MeasureString(string.Concat(this.searchedDegrees[this.editedIndex].GPA));
				Rectangle destinationRectangle = new Rectangle((int)(pos.X + vector.X + 4f), (int)pos.Y, 10, 16);
				if (this.os.timer % 0.6f < 0.3f)
				{
					sb.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * 0.8f);
				}
			}
			pos.X -= 110f;
			pos.Y += 20f;
			if (flag && Button.doButton(46710, (int)pos.X, (int)pos.Y, 110, 20, LocaleTerms.Loc("Edit"), new Color?(this.darkThemeColor)))
			{
				this.editedField = AcademicDatabaseDaemon.ADDEditField.GPA;
				this.os.execute("getString GPA");
			}
			pos.Y += 30f;
			if (this.editedField != AcademicDatabaseDaemon.ADDEditField.None)
			{
				if (this.doEditField())
				{
					this.editedField = AcademicDatabaseDaemon.ADDEditField.None;
					this.os.getStringCache = "";
					this.saveChangesToEntry();
				}
			}
			else if (Button.doButton(486012, bounds.X + 2, bounds.Y + bounds.Height - 40, 230, 30, LocaleTerms.Loc("Save And Return"), new Color?(this.backThemeColor)))
			{
				this.state = AcademicDatabaseDaemon.ADDState.Entry;
				this.comp.log(string.Concat(new object[]
				{
					"ACADEMIC_DATABASE::RecordEdit_:_#",
					this.editedIndex,
					"_: ",
					this.searchedName.Replace(" ", "_")
				}));
			}
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x00053E80 File Offset: 0x00052080
		private bool doEditField()
		{
			string text = "";
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			string[] array = this.os.getStringCache.Split(separator, StringSplitOptions.None);
			if (array.Length > 1)
			{
				text = array[1];
				if (text.Equals(""))
				{
					text = this.os.terminal.currentLine;
				}
				this.setEditedFieldValue(text);
			}
			if (array.Length > 2)
			{
				if (array.Length <= 2)
				{
					this.os.terminal.executeLine();
				}
				if (text.Length > 0)
				{
					this.setEditedFieldValue(text);
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00053F50 File Offset: 0x00052150
		private void setEditedFieldValue(string value)
		{
			CultureInfo provider = new CultureInfo("en-au");
			switch (this.editedField)
			{
			case AcademicDatabaseDaemon.ADDEditField.Uni:
				this.searchedDegrees[this.editedIndex] = new Degree(this.searchedDegrees[this.editedIndex].name, value, this.searchedDegrees[this.editedIndex].GPA);
				break;
			case AcademicDatabaseDaemon.ADDEditField.Degree:
				this.searchedDegrees[this.editedIndex] = new Degree(value, this.searchedDegrees[this.editedIndex].uni, this.searchedDegrees[this.editedIndex].GPA);
				break;
			case AcademicDatabaseDaemon.ADDEditField.GPA:
			{
				float degreeGPA = 2f;
				try
				{
					if (value.Length > 0)
					{
						degreeGPA = (float)Convert.ToDouble(value, provider);
					}
				}
				catch
				{
				}
				this.searchedDegrees[this.editedIndex] = new Degree(this.searchedDegrees[this.editedIndex].name, this.searchedDegrees[this.editedIndex].uni, degreeGPA);
				break;
			}
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x0005409C File Offset: 0x0005229C
		private void drawTitle(Rectangle bounds, SpriteBatch sb)
		{
			TextItem.doFontLabel(new Vector2((float)bounds.X, (float)bounds.Y), LocaleTerms.Loc("International Academic Database"), GuiData.font, null, (float)(bounds.Width - 6), float.MaxValue, false);
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x000540F0 File Offset: 0x000522F0
		private void drawSideBar(Rectangle bounds, SpriteBatch sb)
		{
			this.updateSideBar();
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 1, bounds.Width / 8, bounds.Height - 2);
			int num = bounds.Width / 15;
			Rectangle destinationRectangle = rectangle;
			destinationRectangle.Width = (int)((double)((float)rectangle.Width) * 1.5);
			sb.Draw(Utils.white, destinationRectangle, this.darkThemeColor);
			for (int i = 0; i < this.backBars.Count; i++)
			{
				destinationRectangle.X = (int)((float)rectangle.X + this.backBars[i].X * (float)rectangle.Width);
				destinationRectangle.Width = (int)((float)num * this.backBars[i].Y);
				sb.Draw(Utils.white, destinationRectangle, this.backThemeColor);
				destinationRectangle.X = (int)((float)rectangle.X + this.topBars[i].X * (float)rectangle.Width);
				destinationRectangle.Width = (int)((float)num * this.topBars[i].Y * 0.5f);
				sb.Draw(Utils.white, destinationRectangle, this.themeColor);
			}
			destinationRectangle.X = rectangle.X;
			destinationRectangle.Width = (int)((double)((float)rectangle.Width) * 1.5);
			sb.Draw(Utils.gradient, destinationRectangle, Color.Black);
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x00054288 File Offset: 0x00052488
		private void updateSideBar()
		{
			float num = this.os.timer * 0.4f;
			for (int i = 0; i < this.backBars.Count; i++)
			{
				this.backBars[i] = new Vector2((float)(0.5 + Math.Sin((double)(num * (float)i)) * 0.5), (float)Math.Abs(Math.Sin((double)(num * (float)i))));
				this.topBars[i] = new Vector2((float)(0.5 + Math.Sin((double)(-(double)num * (float)(this.backBars.Count - i))) * 0.5), (float)Math.Abs(Math.Sin((double)(-(double)num) / 2.0 * (double)i)));
			}
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00054368 File Offset: 0x00052568
		private void saveChangesToEntry()
		{
			string[] array = this.searchedName.Split(Utils.spaceDelim);
			FileEntry fileForPerson = this.getFileForPerson(new Person(array[0], array[1], true, false, null)
			{
				degrees = this.searchedDegrees
			});
			this.entries.searchForFile(this.foundFileName).data = fileForPerson.data;
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x000543C6 File Offset: 0x000525C6
		public override void navigatedTo()
		{
			this.state = AcademicDatabaseDaemon.ADDState.Welcome;
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x000543D0 File Offset: 0x000525D0
		public override void userAdded(string name, string pass, byte type)
		{
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000543D4 File Offset: 0x000525D4
		public override string getSaveString()
		{
			return "<AcademicDatabse name=\"" + this.name + "\"/>";
		}

		// Token: 0x040005E6 RID: 1510
		public const string ROOT_FOLDERNAME = "academic_data";

		// Token: 0x040005E7 RID: 1511
		public const string ENTRIES_FOLDERNAME = "entry_cache";

		// Token: 0x040005E8 RID: 1512
		public const string CONFIG_FILENAME = "config.sys";

		// Token: 0x040005E9 RID: 1513
		public const string INFO_FILENAME = "info.txt";

		// Token: 0x040005EA RID: 1514
		private const float SEARCH_TIME = 3.6f;

		// Token: 0x040005EB RID: 1515
		private const float MULTI_MATCH_SEARCH_TIME = 0.7f;

		// Token: 0x040005EC RID: 1516
		public const string DEGREE_SPLIT_DELIM = "--------------------";

		// Token: 0x040005ED RID: 1517
		private AcademicDatabaseDaemon.ADDState state = AcademicDatabaseDaemon.ADDState.Welcome;

		// Token: 0x040005EE RID: 1518
		private Folder root;

		// Token: 0x040005EF RID: 1519
		private Folder entries;

		// Token: 0x040005F0 RID: 1520
		private Color themeColor;

		// Token: 0x040005F1 RID: 1521
		private Color backThemeColor;

		// Token: 0x040005F2 RID: 1522
		private Color darkThemeColor;

		// Token: 0x040005F3 RID: 1523
		private Texture2D loadingCircle;

		// Token: 0x040005F4 RID: 1524
		private string searchedName;

		// Token: 0x040005F5 RID: 1525
		private string foundFileName;

		// Token: 0x040005F6 RID: 1526
		private float searchStartTime = 0f;

		// Token: 0x040005F7 RID: 1527
		private List<Degree> searchedDegrees;

		// Token: 0x040005F8 RID: 1528
		private List<string> searchResultsNames;

		// Token: 0x040005F9 RID: 1529
		private string infoText;

		// Token: 0x040005FA RID: 1530
		private bool needsDeletionConfirmation = false;

		// Token: 0x040005FB RID: 1531
		private int editedIndex = 0;

		// Token: 0x040005FC RID: 1532
		private AcademicDatabaseDaemon.ADDEditField editedField;

		// Token: 0x040005FD RID: 1533
		private List<Vector2> backBars;

		// Token: 0x040005FE RID: 1534
		private List<Vector2> topBars;

		// Token: 0x020000F7 RID: 247
		private enum ADDEditField
		{
			// Token: 0x04000600 RID: 1536
			None,
			// Token: 0x04000601 RID: 1537
			Uni,
			// Token: 0x04000602 RID: 1538
			Degree,
			// Token: 0x04000603 RID: 1539
			GPA
		}

		// Token: 0x020000F8 RID: 248
		private enum ADDState
		{
			// Token: 0x04000605 RID: 1541
			Welcome,
			// Token: 0x04000606 RID: 1542
			Seach,
			// Token: 0x04000607 RID: 1543
			MultiMatchSearch,
			// Token: 0x04000608 RID: 1544
			Entry,
			// Token: 0x04000609 RID: 1545
			PendingResult,
			// Token: 0x0400060A RID: 1546
			EntryNotFound,
			// Token: 0x0400060B RID: 1547
			MultipleEntriesFound,
			// Token: 0x0400060C RID: 1548
			InfoPanel,
			// Token: 0x0400060D RID: 1549
			EditPerson,
			// Token: 0x0400060E RID: 1550
			EditEntry
		}
	}
}
