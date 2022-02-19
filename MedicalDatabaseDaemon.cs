using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000A1 RID: 161
	internal class MedicalDatabaseDaemon : Daemon
	{
		// Token: 0x0600033E RID: 830 RVA: 0x0002FB3C File Offset: 0x0002DD3C
		public MedicalDatabaseDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Universal Medical Database"), os)
		{
			this.logo = os.content.Load<Texture2D>("Sprites/MedicalLogo");
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0002FC28 File Offset: 0x0002DE28
		public override void initFiles()
		{
			base.initFiles();
			this.recordsFolder = new Folder("Medical");
			this.comp.files.root.folders.Add(this.recordsFolder);
			for (int i = 0; i < People.all.Count; i++)
			{
				MedicalDatabaseDaemon.FileMedicalRecord fileMedicalRecord = new MedicalDatabaseDaemon.FileMedicalRecord(People.all[i]);
				FileEntry item = new FileEntry(fileMedicalRecord.ToString(), fileMedicalRecord.GetFileName());
				this.recordsFolder.files.Add(item);
			}
			string dataEntry = Utils.readEntireFile("Content/Post/MedicalDatabaseInfo.txt");
			Folder folder = this.comp.files.root.searchForFolder("home");
			if (folder != null)
			{
				folder.files.Add(new FileEntry(dataEntry, "MedicalDatabaseInfo.txt"));
			}
		}

		// Token: 0x06000340 RID: 832 RVA: 0x0002FD0C File Offset: 0x0002DF0C
		public override void loadInit()
		{
			base.loadInit();
			this.recordsFolder = this.comp.files.root.searchForFolder("Medical");
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0002FD38 File Offset: 0x0002DF38
		public override string getSaveString()
		{
			return "<MedicalDatabase />";
		}

		// Token: 0x06000342 RID: 834 RVA: 0x0002FD50 File Offset: 0x0002DF50
		public void ResetThemeGrid()
		{
			for (int i = 0; i < this.themeGrid.GetLength(0); i++)
			{
				for (int j = 0; j < this.themeGrid.GetLength(1); j++)
				{
					this.ResetGridPoint(j, i);
				}
			}
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0002FDA1 File Offset: 0x0002DFA1
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.elapsedTimeThisState = 0f;
			this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0002FDC0 File Offset: 0x0002DFC0
		public void ResetGridPoint(int x, int y)
		{
			this.themeGrid[y, x] = new MedicalDatabaseDaemon.GridSpot
			{
				from = this.themeGrid[y, x].to,
				to = Utils.randm(2f),
				time = 0f,
				totalTime = Utils.randm(3f) + 1.2f
			};
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0002FE38 File Offset: 0x0002E038
		private void LookupEntry()
		{
			List<string> list = new List<string>();
			string item = this.searchName.Trim().ToLower().Replace(" ", "_");
			list.Add(item);
			if (this.searchName.Contains(" "))
			{
				string text = this.searchName.Substring(this.searchName.IndexOf(" ")) + this.searchName.Substring(0, this.searchName.IndexOf(" "));
				text = text.Trim().ToLower().Replace(" ", "_");
				list.Add(text);
				text = this.searchName.Substring(this.searchName.IndexOf(" ")) + "_" + this.searchName.Substring(0, this.searchName.IndexOf(" "));
				text = text.Trim().ToLower().Replace(" ", "_");
				list.Add(text);
			}
			FileEntry fileEntry = null;
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < this.recordsFolder.files.Count; j++)
				{
					if (this.recordsFolder.files[j].name.ToLower().StartsWith(list[i]))
					{
						fileEntry = this.recordsFolder.files[j];
						break;
					}
				}
				if (fileEntry != null)
				{
					break;
				}
			}
			if (fileEntry == null)
			{
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
				this.errorMessage = string.Concat(new string[]
				{
					LocaleTerms.Loc("No entry found for name"),
					" ",
					this.searchName,
					"\n",
					LocaleTerms.Loc("Permutations tested"),
					":\n"
				});
				for (int j = 0; j < list.Count; j++)
				{
					this.errorMessage = this.errorMessage + list[j] + "\n";
				}
				this.elapsedTimeThisState = 0f;
			}
			else
			{
				this.currentFile = fileEntry;
				MedicalDatabaseDaemon.FileMedicalRecord fileMedicalRecord = new MedicalDatabaseDaemon.FileMedicalRecord();
				if (MedicalDatabaseDaemon.FileMedicalRecord.RecordFromString(this.currentFile.data, out fileMedicalRecord))
				{
					this.currentRecord = fileMedicalRecord;
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Entry;
					this.elapsedTimeThisState = 0f;
				}
				else
				{
					this.elapsedTimeThisState = 0f;
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
					this.errorMessage = string.Concat(new string[]
					{
						LocaleTerms.Loc("Corrupt record"),
						" --\n",
						LocaleTerms.Loc("Unable to parse record"),
						" ",
						this.currentFile.name
					});
				}
			}
		}

		// Token: 0x06000346 RID: 838 RVA: 0x00030160 File Offset: 0x0002E360
		private void UpdateStates(float t)
		{
			this.elapsedTimeThisState += t;
			if (this.elapsedTimeThisState >= this.totalTimeThisState)
			{
				if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.Searching)
				{
					this.LookupEntry();
				}
			}
		}

		// Token: 0x06000347 RID: 839 RVA: 0x000301A8 File Offset: 0x0002E3A8
		private void SendReportEmail(MedicalDatabaseDaemon.FileMedicalRecord record, string emailAddress)
		{
			try
			{
				string mail = MailServer.generateEmail(string.Concat(new string[]
				{
					LocaleTerms.Loc("MedicalRecord"),
					" - ",
					record.Lastname,
					"_",
					record.Firstname
				}), record.ToEmailString(), "records@meddb.org");
				string userTo = emailAddress;
				if (emailAddress.Contains('@'))
				{
					userTo = emailAddress.Substring(0, emailAddress.IndexOf('@'));
				}
				Computer computer = Programs.getComputer(this.os, "jmail");
				if (computer != null)
				{
					MailServer mailServer = (MailServer)computer.getDaemon(typeof(MailServer));
					if (mailServer != null)
					{
						mailServer.addMail(mail, userTo);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000348 RID: 840 RVA: 0x00030290 File Offset: 0x0002E490
		private void updateGrid()
		{
			float num = (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			for (int i = 0; i < this.themeGrid.GetLength(0); i++)
			{
				for (int j = 0; j < this.themeGrid.GetLength(1); j++)
				{
					MedicalDatabaseDaemon.GridSpot gridSpot = this.themeGrid[i, j];
					gridSpot.time += num;
					if (gridSpot.time >= gridSpot.totalTime)
					{
						this.ResetGridPoint(j, i);
					}
					else
					{
						this.themeGrid[i, j] = gridSpot;
					}
				}
			}
		}

		// Token: 0x06000349 RID: 841 RVA: 0x00030350 File Offset: 0x0002E550
		private void drawGrid(Rectangle bounds, SpriteBatch sb, int width)
		{
			int num = 12;
			int x = bounds.X + bounds.Width - num - 1;
			Rectangle destinationRectangle = new Rectangle(x, bounds.Y + 1, num, num);
			int num3;
			int num2 = num3 = 0;
			while (destinationRectangle.Y + 1 < bounds.Y + bounds.Height)
			{
				if (destinationRectangle.Y + num + 1 >= bounds.Y + bounds.Height)
				{
					destinationRectangle.Height = bounds.Y + bounds.Height - (destinationRectangle.Y + 2);
				}
				while (destinationRectangle.X - num > bounds.X + bounds.Width - width - 1)
				{
					MedicalDatabaseDaemon.GridSpot gridSpot = this.themeGrid[num3 % this.themeGrid.GetLength(0), num2 % this.themeGrid.GetLength(1)];
					float num4 = gridSpot.time / gridSpot.totalTime;
					float num5 = gridSpot.from + num4 * (gridSpot.to - gridSpot.from);
					Color value = this.theme_deep;
					Color value2 = this.theme_strong;
					if (num5 >= 1f)
					{
						num5 -= 1f;
						value = this.theme_strong;
						value2 = this.theme_light;
					}
					Color color = Color.Lerp(value, value2, num5);
					sb.Draw(Utils.white, destinationRectangle, color);
					num3++;
					destinationRectangle.X -= num + 1;
				}
				num2++;
				destinationRectangle.X = x;
				destinationRectangle.Y += num + 1;
			}
		}

		// Token: 0x0600034A RID: 842 RVA: 0x00030514 File Offset: 0x0002E714
		public void DrawError(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int)((double)bounds.Width / 1.6) - 4, bounds.Height / 2);
			rectangle.Height = (int)((float)this.logo.Height / (float)this.logo.Width * (float)rectangle.Height);
			sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
			int width = rectangle.Width;
			rectangle.Y += 100;
			rectangle.Height = 35;
			rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
			this.DrawMessage(LocaleTerms.Loc("Error"), true, sb, rectangle, this.os.lockedColor, Color.White);
			rectangle.Y += rectangle.Height + 2;
			rectangle.Height = 100;
			rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
			this.DrawMessageBot(this.errorMessage, false, sb, rectangle, this.theme_back, this.theme_light);
			rectangle.Width = width;
			rectangle.Y += rectangle.Height + 2;
			if (Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
			{
				this.elapsedTimeThisState = 0f;
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
			}
		}

		// Token: 0x0600034B RID: 843 RVA: 0x00030714 File Offset: 0x0002E914
		public void DrawSendReport(Rectangle bounds, SpriteBatch sb)
		{
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int)((double)bounds.Width / 1.6) - 4, bounds.Height / 2);
			rectangle.Height = (int)((float)this.logo.Height / (float)this.logo.Width * (float)rectangle.Height);
			sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
			int width = rectangle.Width;
			rectangle.Y += 100;
			rectangle.Height = 35;
			rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
			this.DrawMessage(LocaleTerms.Loc("Send Record Copy"), true, sb, rectangle, this.theme_deep, Color.White);
			rectangle.Y += rectangle.Height + 2;
			rectangle.Height = 22;
			rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
			this.DrawMessageBot(string.Concat(new string[]
			{
				LocaleTerms.Loc("Record for"),
				" ",
				this.currentRecord.Firstname,
				" ",
				this.currentRecord.Lastname
			}), false, sb, rectangle, this.theme_back, this.theme_light);
			rectangle.Width = width;
			rectangle.Y += rectangle.Height + 12;
			rectangle.Height = 35;
			rectangle.Width = (int)((float)width);
			this.DrawMessage(LocaleTerms.Loc("Recipient Address"), true, sb, rectangle, this.theme_deep, Color.White);
			rectangle.Y += rectangle.Height + 2;
			string upperPrompt = " ---------";
			rectangle.Height = 130;
			Rectangle bounds2 = rectangle;
			if (bounds2.Width < 400)
			{
				bounds2.Width = bounds.Width - bounds.Width / 4 - 2;
			}
			if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch)
			{
				this.emailRecipientAddress = GetStringUIControl.DrawGetStringControl(LocaleTerms.Loc("Recipient Address (Case Sensitive):") + " ", bounds2, delegate
				{
					this.elapsedTimeThisState = 0f;
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
					this.errorMessage = LocaleTerms.Loc("Error getting recipient email");
				}, delegate
				{
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport;
				}, sb, this.os, this.theme_strong, this.os.lockedColor, upperPrompt, null);
				rectangle.Y += 26;
				if (this.emailRecipientAddress != null)
				{
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending;
					this.elapsedTimeThisState = 1f;
				}
			}
			else
			{
				string valueText = (this.emailRecipientAddress == null) ? LocaleTerms.Loc("Undefined") : this.emailRecipientAddress;
				GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Recipient Address") + ": ", valueText, bounds2, sb, this.os, upperPrompt);
			}
			rectangle.Y += rectangle.Height + 2;
			rectangle.Height = 24;
			if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReport)
			{
				if (Button.doButton(444402023, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Specify Address"), new Color?(this.theme_strong)))
				{
					GetStringUIControl.StartGetString("Recipient_Address", this.os);
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch;
				}
			}
			else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending || this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
			{
				float num = (this.elapsedTimeThisState - 1f) / 3f;
				if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
				{
					num = 1f;
				}
				if (num >= 1f && this.state != MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
				{
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete;
					this.SendReportEmail(this.currentRecord, this.emailRecipientAddress);
				}
				sb.Draw(Utils.white, rectangle, this.theme_back);
				Rectangle destinationRectangle = rectangle;
				destinationRectangle.Width = (int)((float)destinationRectangle.Width * Utils.QuadraticOutCurve(num));
				sb.Draw(Utils.white, destinationRectangle, this.theme_light);
				sb.DrawString(GuiData.smallfont, (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete) ? LocaleTerms.Loc("COMPLETE") : (LocaleTerms.Loc("SENDING") + " ..."), new Vector2((float)rectangle.X, (float)(rectangle.Y + 2)), Color.Black);
			}
			rectangle.Y += rectangle.Height + 2;
			if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete)
			{
				if (Button.doButton(444402001, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Send to different address"), new Color?(this.theme_light)))
				{
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport;
				}
			}
			rectangle.Y += rectangle.Height + 2;
			if (Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
			{
				this.elapsedTimeThisState = 0f;
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
			}
		}

		// Token: 0x0600034C RID: 844 RVA: 0x00030D00 File Offset: 0x0002EF00
		public void DrawAbout(Rectangle bounds, SpriteBatch sb)
		{
			string text = null;
			Folder folder = this.comp.files.root.searchForFolder("home");
			if (folder != null)
			{
				FileEntry fileEntry = folder.searchForFile("MedicalDatabaseInfo.txt");
				if (fileEntry != null)
				{
					text = fileEntry.data;
				}
			}
			if (text == null)
			{
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
				this.errorMessage = "DatabaseInfo file not found\n~/home/MedicalDatabaseInfo.txt\nCould not be found or opened";
				this.elapsedTimeThisState = 0f;
			}
			else
			{
				Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int)((double)bounds.Width / 1.6) - 4, bounds.Height / 2);
				rectangle.Height = (int)((float)this.logo.Height / (float)this.logo.Width * (float)rectangle.Height);
				sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
				int width = rectangle.Width;
				rectangle.Y += 100;
				rectangle.Height = 35;
				rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 2f, 1f)));
				this.DrawMessage(LocaleTerms.Loc("Info"), true, sb, rectangle, this.theme_deep, Color.White);
				text = Utils.SuperSmartTwimForWidth(text, rectangle.Width - 12, GuiData.tinyfont);
				rectangle.Y += rectangle.Height + 2;
				rectangle.Height = Math.Min(bounds.Height - 200, 420);
				rectangle.Width = (int)((float)width * Utils.QuadraticOutCurve(Math.Min(this.elapsedTimeThisState * 1.5f, 1f)));
				this.DrawMessageBot(text, false, sb, rectangle, this.theme_back, this.theme_light);
				rectangle.Width = width;
				rectangle.Y += rectangle.Height + 2;
				if (Button.doButton(444402002, rectangle.X, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
				{
					this.elapsedTimeThisState = 0f;
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
				}
			}
		}

		// Token: 0x0600034D RID: 845 RVA: 0x00031164 File Offset: 0x0002F364
		public void DrawEntry(Rectangle bounds, SpriteBatch sb)
		{
			MedicalDatabaseDaemon.<>c__DisplayClassa CS$<>8__locals1 = new MedicalDatabaseDaemon.<>c__DisplayClassa();
			CS$<>8__locals1.<>4__this = this;
			int num = 34;
			if (this.displayPanel == null)
			{
				this.displayPanel = new ScrollableSectionedPanel(26, sb.GraphicsDevice);
			}
			CS$<>8__locals1.drawCalls = new List<Action<int, Rectangle, SpriteBatch>>();
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, (int)((double)bounds.Width / 1.6) - 4, bounds.Height / 2);
			CS$<>8__locals1.allTextBounds = rectangle;
			MedicalDatabaseDaemon.<>c__DisplayClassa CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.allTextBounds.Width = CS$<>8__locals2.allTextBounds.Width + 2;
			MedicalDatabaseDaemon.<>c__DisplayClassa CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.allTextBounds.Y = CS$<>8__locals3.allTextBounds.Y + num;
			CS$<>8__locals1.allTextBounds.Height = bounds.Height - num - 2 - 40 - 28;
			rectangle.Height = this.logo.Height / this.logo.Width * rectangle.Width;
			sb.Draw(this.logo, rectangle, this.theme_deep * 0.4f);
			rectangle.Height = num;
			this.DrawMessage(this.currentRecord.Lastname + ", " + this.currentRecord.Firstname, true, sb, rectangle, this.theme_light, this.theme_back);
			rectangle.Y += num;
			num = 22;
			rectangle.Height = num;
			CS$<>8__locals1.lines = this.currentRecord.record.Split(Utils.newlineDelim);
			string[] separator = new string[]
			{
				" :: ",
				":: ",
				" ::",
				"::",
				"\n"
			};
			bool flag = false;
			for (int i = 0; i < CS$<>8__locals1.lines.Length; i++)
			{
				string[] sections = Utils.SuperSmartTwimForWidth(CS$<>8__locals1.lines[i], rectangle.Width - 12, GuiData.tinyfont).Split(separator, StringSplitOptions.RemoveEmptyEntries);
				if (sections.Length > 1)
				{
					for (int j = 0; j < sections.Length; j++)
					{
						if (j == 0 && !flag)
						{
							if (sections[j] == "Notes")
							{
								flag = true;
							}
							int secID = j;
							CS$<>8__locals1.drawCalls.Add(delegate(int index, Rectangle drawPos, SpriteBatch sprBatch)
							{
								Rectangle dest2 = drawPos;
								dest2.Y++;
								dest2.Height -= 2;
								CS$<>8__locals1.<>4__this.DrawMessage(sections[secID] + " :", false, sprBatch, dest2, CS$<>8__locals1.<>4__this.theme_deep, CS$<>8__locals1.<>4__this.theme_light);
							});
							rectangle.Y += num + 2;
						}
						else if (sections[j].Trim().Length > 0)
						{
							int subSecID = j;
							CS$<>8__locals1.drawCalls.Add(delegate(int index, Rectangle drawPos, SpriteBatch sprBatch)
							{
								Rectangle dest2 = drawPos;
								dest2.Y++;
								dest2.Height -= 2;
								CS$<>8__locals1.<>4__this.DrawMessage(sections[subSecID], false, sprBatch, dest2);
							});
							rectangle.Y += num + 2;
						}
					}
				}
				else if (CS$<>8__locals1.lines[i].Trim().Length > 0)
				{
					int idx = i;
					CS$<>8__locals1.drawCalls.Add(delegate(int index, Rectangle drawPos, SpriteBatch sprBatch)
					{
						Rectangle rectangle2 = drawPos;
						rectangle2.Y++;
						rectangle2.Height -= 2;
						CS$<>8__locals1.<>4__this.DrawMessage(CS$<>8__locals1.lines[idx], false, sprBatch, drawPos);
					});
					rectangle.Y += num + 2;
				}
			}
			CS$<>8__locals1.drawCalls.Add(delegate(int index, Rectangle drawPos, SpriteBatch sprBatch)
			{
				Rectangle dest2 = drawPos;
				dest2.Y += 2;
				dest2.Height -= 4;
				this.DrawMessage(" ", false, sprBatch, dest2);
			});
			rectangle.Y += num + 2;
			this.displayPanel.NumberOfPanels = CS$<>8__locals1.drawCalls.Count;
			this.displayPanel.Draw(delegate(int idx, Rectangle rect, SpriteBatch sprBatch)
			{
				if ((CS$<>8__locals1.drawCalls.Count + 1) * CS$<>8__locals1.<>4__this.displayPanel.PanelHeight >= CS$<>8__locals1.allTextBounds.Height)
				{
					rect.Width -= 10;
				}
				CS$<>8__locals1.drawCalls[idx](idx, rect, sprBatch);
			}, sb, CS$<>8__locals1.allTextBounds);
			rectangle.Y += 2;
			if (Button.doButton(444402033, rectangle.X, bounds.Y + bounds.Height - 26, rectangle.Width, 24, LocaleTerms.Loc("Back to menu"), new Color?(this.theme_strong)))
			{
				this.elapsedTimeThisState = 0f;
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
			}
			if (Button.doButton(444402035, rectangle.X, bounds.Y + bounds.Height - 26 - 2 - 26, rectangle.Width, 24, LocaleTerms.Loc("e-mail this record"), new Color?(this.theme_light)))
			{
				this.elapsedTimeThisState = 0f;
				this.state = MedicalDatabaseDaemon.MedicalDatabaseState.SendReport;
			}
			Rectangle dest = new Rectangle(rectangle.X + rectangle.Width + 2, bounds.Y + 34 + 12, (int)((double)bounds.Width / 6.5) - 2, bounds.Height - 4);
			int height = 33;
			int height2 = 22;
			dest.Height = height;
			this.DrawMessage(LocaleTerms.Loc("Age"), true, sb, dest);
			dest.Y += dest.Height + 2;
			TimeSpan timeSpan = DateTime.Now - this.currentRecord.DOB;
			int num2 = (int)((double)timeSpan.Days / 365.0);
			this.DrawMessage(string.Concat(timeSpan.Days / 365), true, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height;
			dest.Height = height2;
			this.DrawMessage(LocaleTerms.Loc("Years"), false, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height + 2;
			dest.Height = height;
			this.DrawMessage(string.Concat(timeSpan.Days - num2 * 365), true, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height;
			dest.Height = height2;
			this.DrawMessage(LocaleTerms.Loc("Days"), false, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height + 2;
			dest.Height = height;
			this.DrawMessage(string.Concat(timeSpan.Hours), true, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height;
			dest.Height = height2;
			this.DrawMessage(LocaleTerms.Loc("Hours"), false, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height + 2;
			dest.Height = height;
			this.DrawMessage(string.Concat(timeSpan.Minutes), true, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height;
			dest.Height = height2;
			this.DrawMessage(LocaleTerms.Loc("Minutes"), false, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height + 2;
			dest.Height = height;
			this.DrawMessage(string.Concat(timeSpan.Seconds), true, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height;
			dest.Height = height2;
			this.DrawMessage(LocaleTerms.Loc("Seconds"), false, sb, dest, Color.Transparent, this.theme_light);
			dest.Y += dest.Height + 2;
			dest.Height = height;
		}

		// Token: 0x0600034E RID: 846 RVA: 0x000319FC File Offset: 0x0002FBFC
		public void DrawMenu(Rectangle bounds, SpriteBatch sb)
		{
			int num = 34;
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 12, bounds.Width / 2 - 4, num);
			this.DrawMessage(LocaleTerms.Loc("Universal Medical"), true, sb, rectangle, this.theme_light, Color.Black);
			rectangle.Y += num + 2;
			rectangle.Height = 20;
			this.DrawMessage(LocaleTerms.Loc("Records & Monitoring Services"), false, sb, rectangle);
			Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 2 + 10, bounds.Y + 12, bounds.Width / 4 - 12, (int)((float)this.logo.Height / (float)this.logo.Width * ((float)bounds.Width / 4f)));
			sb.Draw(this.logo, destinationRectangle, this.theme_light);
			Rectangle destinationRectangle2 = new Rectangle(rectangle.X + 10, rectangle.Y + 40, rectangle.Width - 20, 1);
			sb.Draw(Utils.white, destinationRectangle2, Utils.SlightlyDarkGray * 0.5f);
			destinationRectangle2.Y += 4;
			sb.Draw(Utils.white, destinationRectangle2, Utils.SlightlyDarkGray * 0.5f);
			rectangle.Y += 90;
			if (!(this.comp.adminIP == this.os.thisComputer.ip))
			{
				rectangle.Height = bounds.Y + bounds.Height - rectangle.Y;
				this.DrawNoAdminMenuSection(rectangle, sb);
			}
			else
			{
				rectangle.Height = 80;
				this.DrawMessageBot(LocaleTerms.Loc("Information"), true, sb, rectangle, this.theme_light, Color.Black);
				rectangle.Y += rectangle.Height + 2;
				rectangle.Height = 20;
				this.DrawMessage(LocaleTerms.Loc("Details and Administration"), false, sb, rectangle);
				rectangle.Y += rectangle.Height + 2;
				if (Button.doButton(444402000, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Info"), new Color?(this.theme_strong)))
				{
					this.state = MedicalDatabaseDaemon.MedicalDatabaseState.AboutScreen;
					this.elapsedTimeThisState = 0f;
				}
				rectangle.Y += 60;
				rectangle.Height = 80;
				this.DrawMessageBot(LocaleTerms.Loc("Database"), true, sb, rectangle, this.theme_light, Color.Black);
				rectangle.Y += rectangle.Height + 2;
				rectangle.Height = 20;
				this.DrawMessage(LocaleTerms.Loc("Records Lookup"), false, sb, rectangle);
				rectangle.Y += rectangle.Height + 2;
				if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu)
				{
					if (Button.doButton(444402005, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Search"), new Color?(this.theme_strong)))
					{
						this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Search;
						this.elapsedTimeThisState = 0f;
						GetStringUIControl.StartGetString("Patient_Name", this.os);
					}
					rectangle.Y += 26;
					if (Button.doButton(444402007, rectangle.X + 1, rectangle.Y, rectangle.Width, 24, LocaleTerms.Loc("Random Entry"), new Color?(this.theme_strong)))
					{
						this.searchName = this.recordsFolder.files[Utils.random.Next(this.recordsFolder.files.Count)].name;
						this.elapsedTimeThisState = 0f;
						this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Searching;
						this.totalTimeThisState = 1.6f;
					}
				}
				else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.Search)
				{
					float num2 = Utils.QuadraticOutCurve(Math.Min(1f, this.elapsedTimeThisState * 2f));
					Rectangle bounds2 = new Rectangle(rectangle.X, rectangle.Y - 10, rectangle.Width, (int)(num2 * 72f));
					Rectangle destinationRectangle3 = new Rectangle(bounds2.X, rectangle.Y + 2, rectangle.Width, (int)(num2 * 32f));
					sb.Draw(Utils.white, destinationRectangle3, this.os.darkBackgroundColor);
					string text = GetStringUIControl.DrawGetStringControl(LocaleTerms.Loc("Enter patient name") + " :", bounds2, delegate
					{
						this.elapsedTimeThisState = 0f;
						this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Error;
						this.errorMessage = LocaleTerms.Loc("Error in name input");
					}, delegate
					{
						this.elapsedTimeThisState = 0f;
						this.state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;
						this.os.terminal.executeLine();
					}, sb, this.os, this.theme_strong, this.theme_back, "", new Color?(Color.Transparent));
					if (text != null)
					{
						this.searchName = text;
						this.elapsedTimeThisState = 0f;
						this.state = MedicalDatabaseDaemon.MedicalDatabaseState.Searching;
						this.totalTimeThisState = 1.6f;
					}
				}
				else if (this.state == MedicalDatabaseDaemon.MedicalDatabaseState.Searching)
				{
					Rectangle destinationRectangle4 = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, 24);
					sb.Draw(Utils.white, destinationRectangle4, this.theme_deep);
					destinationRectangle4.Width = (int)((float)destinationRectangle4.Width * Utils.QuadraticOutCurve(this.elapsedTimeThisState / this.totalTimeThisState));
					sb.Draw(Utils.white, destinationRectangle4, this.theme_light);
					destinationRectangle4.Y += destinationRectangle4.Height / 2 - 2;
					destinationRectangle4.Height = 4;
					sb.Draw(Utils.white, destinationRectangle4, this.theme_deep);
				}
				if (Button.doButton(444402800, rectangle.X + 1, bounds.Y + bounds.Height - 28, rectangle.Width, 24, LocaleTerms.Loc("Exit Database View"), new Color?(this.os.lockedColor)))
				{
					this.os.display.command = "connect";
				}
			}
		}

		// Token: 0x0600034F RID: 847 RVA: 0x000320A0 File Offset: 0x000302A0
		private void DrawNoAdminMenuSection(Rectangle bounds, SpriteBatch sb)
		{
			bounds.Height -= 2;
			bounds.X++;
			bounds.Width -= 2;
			bounds.Height -= 30;
			PatternDrawer.draw(bounds, 0.2f, Color.Transparent, this.os.brightLockedColor, sb, PatternDrawer.errorTile);
			bounds.Height += 30;
			Rectangle dest = bounds;
			dest.Height = 36;
			dest.Y = bounds.Y + bounds.Height / 2 - dest.Height / 2;
			this.DrawMessage(LocaleTerms.Loc("Admin Access"), true, sb, dest, this.os.brightLockedColor * 0.8f, Color.Black);
			dest.Y += dest.Height + 2;
			dest.Height = 22;
			this.DrawMessage(LocaleTerms.Loc("Required for use"), false, sb, dest, this.theme_back, this.os.brightLockedColor);
			if (Button.doButton(444402800, bounds.X + 1, bounds.Y + bounds.Height - 28, bounds.Width, 24, LocaleTerms.Loc("Exit Database View"), new Color?(this.os.lockedColor)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x06000350 RID: 848 RVA: 0x00032224 File Offset: 0x00030424
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.UpdateStates((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			this.updateGrid();
			this.drawGrid(bounds, sb, bounds.Width / 4);
			switch (this.state)
			{
			case MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu:
			case MedicalDatabaseDaemon.MedicalDatabaseState.Search:
			case MedicalDatabaseDaemon.MedicalDatabaseState.Searching:
				this.DrawMenu(bounds, sb);
				break;
			case MedicalDatabaseDaemon.MedicalDatabaseState.Entry:
				this.DrawEntry(bounds, sb);
				break;
			case MedicalDatabaseDaemon.MedicalDatabaseState.Error:
				this.DrawError(bounds, sb);
				break;
			case MedicalDatabaseDaemon.MedicalDatabaseState.AboutScreen:
				this.DrawAbout(bounds, sb);
				break;
			case MedicalDatabaseDaemon.MedicalDatabaseState.SendReport:
			case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSearch:
			case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportSending:
			case MedicalDatabaseDaemon.MedicalDatabaseState.SendReportComplete:
				this.DrawSendReport(bounds, sb);
				break;
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x000322E2 File Offset: 0x000304E2
		private void DrawMessage(string msg, bool big, SpriteBatch sb, Rectangle dest)
		{
			this.DrawMessage(msg, big, sb, dest, this.theme_back, this.theme_light);
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00032300 File Offset: 0x00030500
		private void DrawMessage(string msg, bool big, SpriteBatch sb, Rectangle dest, Color back, Color front)
		{
			sb.Draw(Utils.white, dest, back);
			SpriteFont spriteFont = big ? GuiData.font : GuiData.tinyfont;
			Vector2 vector = spriteFont.MeasureString(msg);
			Vector2 position = new Vector2((float)(dest.X + dest.Width - 4) - vector.X, (float)dest.Y + (float)dest.Height / 2f - vector.Y / 2f);
			bool flag = vector.X >= (float)(dest.Width - 4) || vector.Y >= (float)dest.Height;
			if (flag)
			{
				TextItem.doFontLabelToSize(dest, msg, spriteFont, front, false, false);
			}
			else
			{
				sb.DrawString(spriteFont, msg, position, front);
			}
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000323D4 File Offset: 0x000305D4
		private void DrawMessageBot(string msg, bool big, SpriteBatch sb, Rectangle dest, Color back, Color front)
		{
			sb.Draw(Utils.white, dest, back);
			SpriteFont spriteFont = big ? GuiData.font : GuiData.tinyfont;
			Vector2 vector = spriteFont.MeasureString(msg);
			Vector2 position = new Vector2((float)(dest.X + dest.Width - 4) - vector.X, (float)(dest.Y + dest.Height) - vector.Y - 4f);
			bool flag = vector.X >= (float)(dest.Width - 4) || vector.Y >= (float)(dest.Height - 4);
			if (flag)
			{
				TextItem.doFontLabelToSize(dest, msg, spriteFont, front, false, false);
			}
			else
			{
				sb.DrawString(spriteFont, msg, position, front);
			}
		}

		// Token: 0x040003A1 RID: 929
		private const string FOLDER_NAME = "Medical";

		// Token: 0x040003A2 RID: 930
		private const float SEARCH_TIME = 1.6f;

		// Token: 0x040003A3 RID: 931
		private Folder recordsFolder;

		// Token: 0x040003A4 RID: 932
		private MedicalDatabaseDaemon.MedicalDatabaseState state = MedicalDatabaseDaemon.MedicalDatabaseState.MainMenu;

		// Token: 0x040003A5 RID: 933
		private float totalTimeThisState = 1f;

		// Token: 0x040003A6 RID: 934
		private float elapsedTimeThisState = 0f;

		// Token: 0x040003A7 RID: 935
		private string searchName = "";

		// Token: 0x040003A8 RID: 936
		private string emailRecipientAddress = "";

		// Token: 0x040003A9 RID: 937
		private FileEntry currentFile = null;

		// Token: 0x040003AA RID: 938
		private MedicalDatabaseDaemon.FileMedicalRecord currentRecord = null;

		// Token: 0x040003AB RID: 939
		private ScrollableSectionedPanel displayPanel;

		// Token: 0x040003AC RID: 940
		private string errorMessage = LocaleTerms.Loc("UNKNOWN ERROR");

		// Token: 0x040003AD RID: 941
		private Color theme_deep = new Color(8, 78, 90);

		// Token: 0x040003AE RID: 942
		private Color theme_strong = new Color(64, 157, 174);

		// Token: 0x040003AF RID: 943
		private Color theme_light = new Color(165, 237, 249);

		// Token: 0x040003B0 RID: 944
		private Color theme_back = new Color(20, 20, 20);

		// Token: 0x040003B1 RID: 945
		private Texture2D logo;

		// Token: 0x040003B2 RID: 946
		private MedicalDatabaseDaemon.GridSpot[,] themeGrid = new MedicalDatabaseDaemon.GridSpot[20, 100];

		// Token: 0x020000A2 RID: 162
		private class FileMedicalRecord
		{
			// Token: 0x06000359 RID: 857 RVA: 0x000324A3 File Offset: 0x000306A3
			public FileMedicalRecord()
			{
			}

			// Token: 0x0600035A RID: 858 RVA: 0x000324B8 File Offset: 0x000306B8
			public FileMedicalRecord(Person p)
			{
				this.Firstname = p.firstName;
				this.Lastname = p.lastName;
				this.IsMale = p.isMale;
				this.record = this.MedicalRecordToReport(p.medicalRecord);
				this.DOB = p.medicalRecord.DateofBirth;
			}

			// Token: 0x0600035B RID: 859 RVA: 0x0003251C File Offset: 0x0003071C
			public string MedicalRecordToReport(MedicalRecord rec)
			{
				return rec.ToString();
			}

			// Token: 0x0600035C RID: 860 RVA: 0x00032534 File Offset: 0x00030734
			public override string ToString()
			{
				return string.Concat(new string[]
				{
					this.Firstname,
					"\n-----------------\n",
					this.Lastname,
					"\n-----------------\n",
					this.IsMale ? "male" : "female",
					"\n-----------------\n",
					Utils.SafeWriteDateTime(this.DOB),
					"\n-----------------\n",
					this.record
				});
			}

			// Token: 0x0600035D RID: 861 RVA: 0x000325B8 File Offset: 0x000307B8
			public string ToEmailString()
			{
				string text = this.ToString();
				text = text.Replace("tions ::", "tions::\n");
				text = text.Replace("Visits ::", "Visits::\n");
				return text.Replace("Notes ::", "\nNotes ::\n");
			}

			// Token: 0x0600035E RID: 862 RVA: 0x00032608 File Offset: 0x00030808
			public string GetFileName()
			{
				return this.Lastname.ToLower() + "_" + this.Firstname.ToLower() + ".rec";
			}

			// Token: 0x0600035F RID: 863 RVA: 0x00032640 File Offset: 0x00030840
			public static bool RecordFromString(string rec, out MedicalDatabaseDaemon.FileMedicalRecord record)
			{
				CultureInfo cultureInfo = new CultureInfo("en-au");
				string[] array = rec.Split(MedicalDatabaseDaemon.FileMedicalRecord.SPLIT_DELIM, StringSplitOptions.RemoveEmptyEntries);
				bool result;
				if (array.Length >= 5)
				{
					record = new MedicalDatabaseDaemon.FileMedicalRecord();
					record.Firstname = array[0];
					record.Lastname = array[1];
					record.IsMale = (array[2] == "male");
					record.DOB = Utils.SafeParseDateTime(array[3]);
					record.record = array[4];
					result = true;
				}
				else
				{
					record = null;
					result = false;
				}
				return result;
			}

			// Token: 0x040003B3 RID: 947
			private const string DELIMITER = "\n-----------------\n";

			// Token: 0x040003B4 RID: 948
			private static string[] SPLIT_DELIM = new string[]
			{
				"\n-----------------\n"
			};

			// Token: 0x040003B5 RID: 949
			public string Firstname;

			// Token: 0x040003B6 RID: 950
			public string Lastname;

			// Token: 0x040003B7 RID: 951
			public string record;

			// Token: 0x040003B8 RID: 952
			public bool IsMale = true;

			// Token: 0x040003B9 RID: 953
			public DateTime DOB;
		}

		// Token: 0x020000A3 RID: 163
		private enum MedicalDatabaseState
		{
			// Token: 0x040003BB RID: 955
			MainMenu,
			// Token: 0x040003BC RID: 956
			Search,
			// Token: 0x040003BD RID: 957
			Searching,
			// Token: 0x040003BE RID: 958
			Entry,
			// Token: 0x040003BF RID: 959
			Error,
			// Token: 0x040003C0 RID: 960
			AboutScreen,
			// Token: 0x040003C1 RID: 961
			SendReport,
			// Token: 0x040003C2 RID: 962
			SendReportSearch,
			// Token: 0x040003C3 RID: 963
			SendReportSending,
			// Token: 0x040003C4 RID: 964
			SendReportComplete
		}

		// Token: 0x020000A4 RID: 164
		private struct GridSpot
		{
			// Token: 0x040003C5 RID: 965
			public float from;

			// Token: 0x040003C6 RID: 966
			public float to;

			// Token: 0x040003C7 RID: 967
			public float time;

			// Token: 0x040003C8 RID: 968
			public float totalTime;
		}
	}
}
