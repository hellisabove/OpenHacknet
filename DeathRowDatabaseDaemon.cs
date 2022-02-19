using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000097 RID: 151
	internal class DeathRowDatabaseDaemon : Daemon
	{
		// Token: 0x060002EE RID: 750 RVA: 0x00029C4C File Offset: 0x00027E4C
		public DeathRowDatabaseDaemon(Computer c, string serviceName, OS os) : base(c, serviceName, os)
		{
			if (DeathRowDatabaseDaemon.Logo == null)
			{
				DeathRowDatabaseDaemon.Logo = os.content.Load<Texture2D>("Sprites/DeathRowLogo");
			}
			if (DeathRowDatabaseDaemon.Circle == null)
			{
				DeathRowDatabaseDaemon.Circle = os.content.Load<Texture2D>("Sprites/ThinCircleOutline");
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x00029CD4 File Offset: 0x00027ED4
		public override void initFiles()
		{
			base.initFiles();
			this.root = new Folder("dr_database");
			this.records = new Folder("records");
			this.root.folders.Add(this.records);
			this.comp.files.root.folders.Add(this.root);
			FileEntry item = new FileEntry(Utils.readEntireFile("Content/Post/DeathRowServerInfo.txt").Replace('\t', ' '), "ServerDetails.txt");
			this.root.files.Add(item);
			this.LoadRecords(null);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x00029DA0 File Offset: 0x00027FA0
		private void LoadRecords(string data = null)
		{
			string text = Utils.readEntireFile("Content/Post/DeathRow.txt");
			if (data != null)
			{
				text = data;
			}
			string str = Utils.readEntireFile("Content/Post/DeathRowSpecials.txt");
			text += str;
			string[] array = text.Split(new string[]
			{
				"\r\n###%%##%%##%%##\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				DeathRowDatabaseDaemon.DeathRowEntry deathRowEntry = this.ConvertStringToRecord(array[i]);
				if (deathRowEntry.FName != null)
				{
					string nameEntry = string.Concat(new string[]
					{
						deathRowEntry.LName,
						"_",
						deathRowEntry.FName,
						"[",
						deathRowEntry.RecordNumber,
						"]"
					});
					string dataEntry = array[i].Replace("#", "#\n");
					FileEntry item = new FileEntry(dataEntry, nameEntry);
					this.records.files.Add(item);
				}
			}
			this.records.files.Sort((FileEntry f1, FileEntry f2) => string.Compare(f1.name, f2.name));
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x00029EE0 File Offset: 0x000280E0
		private DeathRowDatabaseDaemon.DeathRowEntry ConvertStringToRecord(string data)
		{
			string[] array = data.Split(new string[]
			{
				"#\n",
				"#"
			}, StringSplitOptions.None);
			DeathRowDatabaseDaemon.DeathRowEntry result = default(DeathRowDatabaseDaemon.DeathRowEntry);
			if (array.Length >= 9)
			{
				result.FName = array[0];
				result.LName = array[1];
				result.RecordNumber = array[2];
				result.Age = array[3];
				result.Date = array[4];
				result.Country = array[5];
				result.PriorRecord = array[6];
				result.IncidentReport = array[7];
				result.Statement = array[8];
			}
			return result;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x00029F84 File Offset: 0x00028184
		public override void loadInit()
		{
			base.loadInit();
			this.root = this.comp.files.root.searchForFolder("dr_database");
			this.records = this.root.searchForFolder("records");
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x00029FC4 File Offset: 0x000281C4
		public override string getSaveString()
		{
			return "<DeathRowDatabase />";
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0002A000 File Offset: 0x00028200
		private string ConvertFilesToOutput()
		{
			string str = "\r\n###%%##%%##%%##\r\n";
			string text = "#";
			string text2 = "";
			this.records.files.Sort((FileEntry f1, FileEntry f2) => string.Compare(f1.name, f2.name));
			for (int i = 0; i < this.records.files.Count; i++)
			{
				DeathRowDatabaseDaemon.DeathRowEntry deathRowEntry = this.ConvertStringToRecord(this.records.files[i].data);
				if (deathRowEntry.RecordNumber != null)
				{
					string text3 = string.Concat(new string[]
					{
						deathRowEntry.FName,
						text,
						deathRowEntry.LName,
						text,
						deathRowEntry.RecordNumber,
						text,
						deathRowEntry.Age,
						text
					});
					string text4 = text3;
					text3 = string.Concat(new string[]
					{
						text4,
						deathRowEntry.Date,
						text,
						deathRowEntry.Country,
						text,
						deathRowEntry.PriorRecord,
						text,
						deathRowEntry.IncidentReport,
						text,
						deathRowEntry.Statement
					});
					text3 += str;
					text2 += text3;
				}
			}
			return text2;
		}

		// Token: 0x060002F5 RID: 757 RVA: 0x0002A180 File Offset: 0x00028380
		private void TestStringConversion()
		{
			string data = this.ConvertFilesToOutput();
			this.records.files.Clear();
			this.LoadRecords(data);
		}

		// Token: 0x060002F6 RID: 758 RVA: 0x0002A1B0 File Offset: 0x000283B0
		public bool ContainsRecordForName(string fName, string lName)
		{
			string value = lName + "_" + fName;
			for (int i = 0; i < this.records.files.Count; i++)
			{
				if (this.records.files[i].name.StartsWith(value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0002A218 File Offset: 0x00028418
		public DeathRowDatabaseDaemon.DeathRowEntry GetRecordForName(string fName, string lName)
		{
			string value = lName + "_" + fName;
			int i = 0;
			while (i < this.records.files.Count)
			{
				if (!this.records.files[i].name.StartsWith(value))
				{
					try
					{
						DeathRowDatabaseDaemon.DeathRowEntry result = this.ConvertStringToRecord(this.records.files[i].data);
						if (result.FName.ToLower() == fName.ToLower() && result.LName.ToLower() == lName.ToLower())
						{
							return result;
						}
					}
					catch (Exception)
					{
					}
					i++;
					continue;
				}
				return this.ConvertStringToRecord(this.records.files[i].data);
			}
			return default(DeathRowDatabaseDaemon.DeathRowEntry);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0002A32C File Offset: 0x0002852C
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			string[] array = new string[this.records.files.Count];
			for (int i = 0; i < this.records.files.Count; i++)
			{
				try
				{
					array[i] = this.records.files[i].name.Substring(0, this.records.files[i].name.IndexOf('[')).Replace("_", ", ");
				}
				catch (Exception)
				{
					array[i] = "UNKNOWN" + i;
				}
			}
			int num = bounds.Width / 3;
			int selectedIndex = this.SelectedIndex;
			this.SelectedIndex = SelectableTextList.doFancyList(832190831, bounds.X + 1, bounds.Y + 4, num, bounds.Height - 8, array, this.SelectedIndex, new Color?(this.themeColor), true);
			if (this.SelectedIndex != selectedIndex)
			{
				this.recordScrollPosition = Vector2.Zero;
			}
			sb.Draw(Utils.white, new Rectangle(bounds.X + num - 1, bounds.Y + 1, 2, bounds.Height - 2), this.themeColor);
			DeathRowDatabaseDaemon.DeathRowEntry entry = default(DeathRowDatabaseDaemon.DeathRowEntry);
			Rectangle bounds2 = bounds;
			bounds2.X += num;
			bounds2.Width -= num + 1;
			if (this.SelectedIndex >= 0 && this.SelectedIndex < this.records.files.Count)
			{
				entry = this.ConvertStringToRecord(this.records.files[this.SelectedIndex].data);
			}
			if (entry.RecordNumber != null)
			{
				this.DrawRecord(bounds2, sb, entry);
			}
			else
			{
				this.DrawTitleScreen(bounds2, sb);
			}
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0002A540 File Offset: 0x00028740
		private void DrawTitleScreen(Rectangle bounds, SpriteBatch sb)
		{
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			Rectangle destinationRectangle = new Rectangle(bounds.X + 12, bounds.Y + 12, DeathRowDatabaseDaemon.Logo.Width, DeathRowDatabaseDaemon.Logo.Height);
			sb.Draw(DeathRowDatabaseDaemon.Logo, destinationRectangle, Color.White);
			float num = 1f;
			float num2 = 1.4f;
			float num3 = this.os.timer % 3f;
			if (num3 > 1f)
			{
				num3 = 0f;
			}
			float num4 = 1f - num3;
			num4 *= 0.4f;
			if (num3 == 0f)
			{
				num4 = 0f;
			}
			num3 = Utils.QuadraticOutCurve(num3) * (num2 - num);
			if (num3 > 0f)
			{
				num3 += num;
			}
			destinationRectangle = new Rectangle((int)((float)destinationRectangle.X - (float)(destinationRectangle.Width / 2) * num3), (int)((float)destinationRectangle.Y - (float)(destinationRectangle.Height / 2) * num3), (int)((float)destinationRectangle.Width * num3), (int)((float)destinationRectangle.Height * num3));
			destinationRectangle.X += DeathRowDatabaseDaemon.Logo.Width / 2;
			destinationRectangle.Y += DeathRowDatabaseDaemon.Logo.Height / 2;
			sb.Draw(DeathRowDatabaseDaemon.Circle, destinationRectangle, Color.White * num4);
			Vector2 pos = new Vector2((float)(bounds.X + DeathRowDatabaseDaemon.Logo.Width + 22), (float)(bounds.Y + 14));
			TextItem.doFontLabel(pos, "DEATH ROW", GuiData.titlefont, new Color?(this.themeColor), (float)(bounds.Width - DeathRowDatabaseDaemon.Logo.Width) - 26f, 50f, false);
			pos.Y += 45f;
			TextItem.doFontLabel(pos, "EXECUTED OFFENDERS LISTING", GuiData.titlefont, new Color?(Color.White), (float)(bounds.Width - DeathRowDatabaseDaemon.Logo.Width) - 26f, 40f, false);
			pos.Y += (float)(DeathRowDatabaseDaemon.Logo.Height - 40);
			pos.X = (float)(bounds.X + 12);
			FileEntry fileEntry = this.root.searchForFile("ServerDetails.txt");
			if (fileEntry != null)
			{
				string text = Utils.SmartTwimForWidth(fileEntry.data, bounds.Width - 30, GuiData.tinyfont);
				TextItem.doFontLabel(pos, text, GuiData.tinyfont, new Color?(Color.White), (float)(bounds.Width - 20), (float)bounds.Height - 200f, false);
			}
			if (Button.doButton(166261601, bounds.X + 6, bounds.Y + bounds.Height - 26, bounds.Width - 12, 22, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
			{
				this.os.display.command = "connect";
			}
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0002A87C File Offset: 0x00028A7C
		private void DrawRecord(Rectangle bounds, SpriteBatch sb, DeathRowDatabaseDaemon.DeathRowEntry entry)
		{
			bounds.X += 2;
			bounds.Width -= 2;
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			int num = 850;
			ScrollablePanel.beginPanel(98302836, new Rectangle(bounds.X, bounds.Y, bounds.Width, num), this.recordScrollPosition);
			int num2 = bounds.Width - 16;
			Vector2 vector = new Vector2(5f, 5f);
			GuiData.spriteBatch.Draw(DeathRowDatabaseDaemon.Logo, new Rectangle((int)vector.X, (int)vector.Y, 60, 60), Color.White);
			vector.X += 70f;
			TextItem.doFontLabel(vector, "DEATH ROW : EXECUTED OFFENDERS LISTING", GuiData.titlefont, new Color?(this.themeColor), (float)(num2 - 80), 45f, false);
			vector.Y += 22f;
			if (Button.doButton(98102855, (int)vector.X, (int)vector.Y, bounds.Width / 2, 25, "Return", new Color?(Color.Black)))
			{
				this.SelectedIndex = -1;
			}
			vector.X = 5f;
			vector.Y += 55f;
			TextItem.doFontLabel(vector, "RECORD " + entry.RecordNumber, GuiData.titlefont, new Color?(Color.White), (float)(num2 - 4), 60f, false);
			vector.Y += 70f;
			int num3 = 18;
			int num4 = 12;
			vector = this.DrawCompactLabel(LocaleTerms.Loc("Name") + ":", entry.LName + ", " + entry.FName, vector, num4, num3, num2);
			vector = this.DrawCompactLabel(LocaleTerms.Loc("Age") + ":", entry.Age, vector, num4, num3, num2);
			num3 = 20;
			num4 = 20;
			Vector2 vector2 = Vector2.Zero;
			TextItem.doFontLabel(vector, LocaleTerms.Loc("Incident Report") + ":", GuiData.smallfont, new Color?(this.themeColor), (float)num2, float.MaxValue, false);
			vector.Y += (float)num3;
			TextItem.DrawShadow = false;
			vector2 = TextItem.doMeasuredSmallLabel(vector, Utils.SmartTwimForWidth(entry.IncidentReport, num2, GuiData.smallfont), new Color?(Color.White));
			vector.Y += Math.Max(vector2.Y, (float)num3);
			vector.Y += (float)num4;
			TextItem.doFontLabel(vector, LocaleTerms.Loc("Final Statement") + ":", GuiData.smallfont, new Color?(this.themeColor), (float)num2, float.MaxValue, false);
			vector.Y += (float)num3;
			vector2 = TextItem.doMeasuredSmallLabel(vector, Utils.SmartTwimForWidth(entry.Statement, num2, GuiData.smallfont), new Color?(Color.White));
			vector.Y += (float)num4;
			this.recordScrollPosition = ScrollablePanel.endPanel(98302836, this.recordScrollPosition, bounds, (float)(num - bounds.Height), true);
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0002ABDC File Offset: 0x00028DDC
		private Vector2 DrawCompactLabel(string label, string value, Vector2 drawPos, int margin, int seperatorHeight, int textWidth)
		{
			TextItem.doFontLabel(drawPos, label, GuiData.smallfont, new Color?(this.themeColor), (float)textWidth, float.MaxValue, false);
			drawPos.Y += (float)seperatorHeight;
			TextItem.doFontLabel(drawPos, value, GuiData.smallfont, new Color?(Color.White), (float)textWidth, float.MaxValue, false);
			drawPos.Y += (float)seperatorHeight;
			drawPos.Y += (float)margin;
			return drawPos;
		}

		// Token: 0x0400032B RID: 811
		public const string ROOT_FOLDERNAME = "dr_database";

		// Token: 0x0400032C RID: 812
		public const string RECORDS_FOLDERNAME = "records";

		// Token: 0x0400032D RID: 813
		public const string SERVER_INFO_FILENAME = "ServerDetails.txt";

		// Token: 0x0400032E RID: 814
		private static Texture2D Logo;

		// Token: 0x0400032F RID: 815
		private static Texture2D Circle;

		// Token: 0x04000330 RID: 816
		private Folder root;

		// Token: 0x04000331 RID: 817
		private Folder records;

		// Token: 0x04000332 RID: 818
		private int SelectedIndex = -1;

		// Token: 0x04000333 RID: 819
		private Color themeColor = new Color(207, 44, 19);

		// Token: 0x04000334 RID: 820
		private Vector2 recordScrollPosition = Vector2.Zero;

		// Token: 0x02000098 RID: 152
		public struct DeathRowEntry
		{
			// Token: 0x04000337 RID: 823
			public string FName;

			// Token: 0x04000338 RID: 824
			public string LName;

			// Token: 0x04000339 RID: 825
			public string RecordNumber;

			// Token: 0x0400033A RID: 826
			public string Age;

			// Token: 0x0400033B RID: 827
			public string Date;

			// Token: 0x0400033C RID: 828
			public string Country;

			// Token: 0x0400033D RID: 829
			public string PriorRecord;

			// Token: 0x0400033E RID: 830
			public string IncidentReport;

			// Token: 0x0400033F RID: 831
			public string Statement;
		}
	}
}
