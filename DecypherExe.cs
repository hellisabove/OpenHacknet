using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000C6 RID: 198
	internal class DecypherExe : ExeModule
	{
		// Token: 0x06000404 RID: 1028 RVA: 0x0003ECB0 File Offset: 0x0003CEB0
		public DecypherExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.IdentifierName = "Decypher Module";
			this.ramCost = 370;
			if (p.Length < 2)
			{
				this.status = DecypherExe.DecypherStatus.Error;
				this.errorMessage = "No File Provided";
			}
			else
			{
				this.InitializeFiles(p[1]);
				if (p.Length > 2)
				{
					this.password = p[2];
				}
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x0003EDB4 File Offset: 0x0003CFB4
		private void InitializeFiles(string filename)
		{
			this.targetComputer = this.os.thisComputer;
			if (this.os.connectedComp != null)
			{
				this.targetComputer = this.os.connectedComp;
			}
			this.destFolder = Programs.getCurrentFolder(this.os);
			this.targetFilename = filename;
			this.destFilename = filename.Replace(".dec", "[NUMBER][EXT]");
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0003EE24 File Offset: 0x0003D024
		public override void Update(float t)
		{
			base.Update(t);
			this.timeOnThisPhase += t;
			float num = 3.5f;
			switch (this.status)
			{
			case DecypherExe.DecypherStatus.Error:
				num = 6f;
				if (this.timeOnThisPhase >= 6f)
				{
					this.isExiting = true;
				}
				goto IL_20E;
			case DecypherExe.DecypherStatus.Working:
				num = 10f;
				if (this.timeOnThisPhase >= 10f)
				{
					try
					{
						this.CompleteWorking();
					}
					catch (Exception)
					{
						this.status = DecypherExe.DecypherStatus.Error;
						this.timeOnThisPhase = 0f;
						this.errorMessage = LocaleTerms.Loc("Fatal error in decryption\nfile may be corrupt");
					}
					this.status = DecypherExe.DecypherStatus.Complete;
					this.timeOnThisPhase = 0f;
				}
				else if (this.percentComplete % 0.1f < 0.01f)
				{
					this.lastLockedPercentage = this.percentComplete;
					this.lcgSeed = Utils.random.Next();
					this.rowsActive.Clear();
					this.columnsActive.Clear();
					for (int i = 0; i < this.columnsDrawn; i++)
					{
						if (Utils.random.NextDouble() < 0.2)
						{
							this.rowsActive.Add(i);
						}
					}
					for (int j = 0; j < this.rowsDrawn; j++)
					{
						if (Utils.random.NextDouble() < 0.2)
						{
							this.columnsActive.Add(j);
						}
					}
				}
				goto IL_20E;
			case DecypherExe.DecypherStatus.Complete:
				num = 3f;
				if (this.timeOnThisPhase >= 3f)
				{
					this.isExiting = true;
				}
				goto IL_20E;
			}
			num = 3.5f;
			if (this.timeOnThisPhase >= 3.5f)
			{
				if (this.CompleteLoading())
				{
					this.status = DecypherExe.DecypherStatus.Working;
				}
				this.timeOnThisPhase = 0f;
			}
			IL_20E:
			this.percentComplete = this.timeOnThisPhase / num;
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x0003F060 File Offset: 0x0003D260
		private bool CompleteLoading()
		{
			bool result;
			try
			{
				this.targetFile = this.destFolder.searchForFile(this.targetFilename);
				if (this.targetFile == null)
				{
					this.status = DecypherExe.DecypherStatus.Error;
					this.errorMessage = LocaleTerms.Loc("File not found");
					result = false;
				}
				else
				{
					int num = FileEncrypter.FileIsEncrypted(this.targetFile.data, this.password);
					if (num == 0)
					{
						this.status = DecypherExe.DecypherStatus.Error;
						this.errorMessage = LocaleTerms.Loc("File is not\nDEC encrypted");
						result = false;
					}
					else if (num == 2)
					{
						this.status = DecypherExe.DecypherStatus.Error;
						if (this.password == "")
						{
							this.errorMessage = LocaleTerms.Loc("This File Requires\n a Password");
						}
						else
						{
							this.errorMessage = LocaleTerms.Loc("Provided Password\nIs Incorrect");
						}
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			catch (Exception)
			{
				this.status = DecypherExe.DecypherStatus.Error;
				this.errorMessage = LocaleTerms.Loc("Fatal error in loading");
				result = false;
			}
			return result;
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x0003F178 File Offset: 0x0003D378
		private void CompleteWorking()
		{
			string[] array = FileEncrypter.DecryptString(this.targetFile.data, this.password);
			if (!this.destFilename.Contains("[NUMBER]"))
			{
				this.destFilename += "[NUMBER]";
			}
			string text = this.destFilename.Replace("[NUMBER]", "");
			if (array[3] != null)
			{
				text = text.Replace("[EXT]", array[3]);
			}
			else
			{
				text = text.Replace("[EXT]", ".txt");
			}
			if (this.destFolder.containsFile(text))
			{
				int num = 1;
				do
				{
					text = this.destFilename.Replace("[NUMBER]", "(" + num + ")");
					num++;
				}
				while (this.destFolder.containsFile(text));
			}
			FileEntry item = new FileEntry(array[2], text);
			this.writtenFilename = text;
			this.destFolder.files.Add(item);
			this.os.write("Decryption complete - file " + this.targetFilename + " decrypted to target file " + this.targetFilename);
			this.os.write("Encryption Header    : \"" + array[0] + "\"");
			this.os.write("Encryption Source IP: \"" + array[1] + "\"");
			this.displayHeader = array[0];
			this.displayIP = array[1];
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x0003F2FC File Offset: 0x0003D4FC
		private Rectangle DrawLoadingMessage(string message, float startPoint, Rectangle dest, bool showLoading = true)
		{
			float num = 0.18f;
			float num2 = (this.percentComplete - startPoint) / num;
			if (this.percentComplete > startPoint)
			{
				dest.Y += 22;
				this.spriteBatch.Draw(Utils.white, dest, Color.Black);
				this.spriteBatch.DrawString(GuiData.tinyfont, message, new Vector2((float)(this.bounds.X + 6), (float)(dest.Y + 2)), Color.White);
				if (showLoading)
				{
					if (this.percentComplete > startPoint + num)
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, LocaleTerms.Loc("COMPLETE"), new Vector2((float)(this.bounds.X + 172), (float)(dest.Y + 2)), Color.DarkGreen);
					}
					else
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, (num2 * 100f).ToString("00") + "%", new Vector2((float)(this.bounds.X + 195), (float)(dest.Y + 2)), Color.White);
					}
				}
			}
			return dest;
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0003F44C File Offset: 0x0003D64C
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			string text = this.status.ToString();
			switch (this.status)
			{
			case DecypherExe.DecypherStatus.Error:
			{
				PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.errorTile);
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
				this.spriteBatch.Draw(Utils.white, rectangle, this.os.lockedColor);
				this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("ERROR"), new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
				rectangle.Y += 50;
				rectangle.Height = 80;
				rectangle.Width = 250;
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
				this.spriteBatch.DrawString(GuiData.smallfont, this.errorMessage, new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 74)), Color.White);
				break;
			}
			case DecypherExe.DecypherStatus.Loading:
			{
				PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.2f, this.spriteBatch, PatternDrawer.thinStripe);
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
				this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Loading..."), new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
				rectangle.Height = 20;
				rectangle.Width = 240;
				rectangle.Y += 28;
				rectangle = this.DrawLoadingMessage("Reading File...", 0f, rectangle, true);
				rectangle = this.DrawLoadingMessage("Parsing Headers...", 0.18f, rectangle, true);
				rectangle = this.DrawLoadingMessage("Verifying Content...", 0.36f, rectangle, true);
				rectangle = this.DrawLoadingMessage("Checking DEC...", 0.54f, rectangle, true);
				rectangle = this.DrawLoadingMessage("Reading Codes...", 0.72f, rectangle, true);
				break;
			}
			case DecypherExe.DecypherStatus.Working:
			{
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 1, 200, 40);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
				this.spriteBatch.DrawString(GuiData.font, "Decode:: " + (this.percentComplete * 100f).ToString("00.0") + " %", new Vector2((float)(this.bounds.X + 2), (float)(this.bounds.Y + 2)), Color.White);
				Rectangle rectangle2 = new Rectangle(this.bounds.X, this.bounds.Y + 50, this.bounds.Width, this.bounds.Height - 50);
				int num = rectangle2.X;
				int num2 = 12;
				int num3 = rectangle2.Y;
				int num4 = 13;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				Utils.LCG.reSeed(this.lcgSeed);
				while (num + num2 < rectangle2.X + rectangle2.Width)
				{
					num3 = rectangle2.Y;
					num7 = 0;
					while (num3 + num4 < rectangle2.Y + rectangle2.Height)
					{
						bool flag = this.rowsActive.Contains(num6) || this.columnsActive.Contains(num7);
						float num8 = flag ? this.percentComplete : this.lastLockedPercentage;
						int index = (int)((num8 % 10f * (float)this.targetFile.data.Length * 3f + (float)(num5 * 222)) % (float)this.targetFile.data.Length);
						char c = this.targetFile.data[index];
						this.spriteBatch.DrawString(GuiData.UITinyfont, string.Concat(c), new Vector2((float)num, (float)num3), Color.Lerp(Color.White, this.os.highlightColor, flag ? Utils.randm(1f) : Utils.LCG.NextFloat()));
						num3 += num4;
						num5++;
						num7++;
					}
					num += num2 + 2;
					num6++;
				}
				this.columnsDrawn = num6;
				this.rowsDrawn = num7;
				break;
			}
			case DecypherExe.DecypherStatus.Complete:
				PatternDrawer.draw(this.bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.2f, this.spriteBatch, PatternDrawer.thinStripe);
				if (this.bounds.Height > 60)
				{
					Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 85);
					this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
					this.spriteBatch.DrawString(GuiData.font, "Operation\nComplete", new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
					rectangle.Height = 20;
					rectangle.Width = 240;
					rectangle.Y += 63;
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage(LocaleTerms.Loc("Headers:"), 0f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage("\"" + this.displayHeader + "\"", 0.1f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage(LocaleTerms.Loc("Source IP:"), 0.3f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage("\"" + this.displayIP + "\"", 0.4f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage(LocaleTerms.Loc("Output File:"), 0.6f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage("\"" + this.writtenFilename + "\"", 0.7f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage(LocaleTerms.Loc("Operation Complete"), 0.9f, rectangle, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height)
					{
						rectangle = this.DrawLoadingMessage(LocaleTerms.Loc("Shutting Down"), 0.95f, rectangle, false);
					}
				}
				break;
			}
		}

		// Token: 0x040004BC RID: 1212
		private const float LOADING_TIME = 3.5f;

		// Token: 0x040004BD RID: 1213
		private const float WORKING_TIME = 10f;

		// Token: 0x040004BE RID: 1214
		private const float COMPLETE_TIME = 3f;

		// Token: 0x040004BF RID: 1215
		private const float ERROR_TIME = 6f;

		// Token: 0x040004C0 RID: 1216
		private Computer targetComputer;

		// Token: 0x040004C1 RID: 1217
		private Folder destFolder;

		// Token: 0x040004C2 RID: 1218
		private FileEntry targetFile;

		// Token: 0x040004C3 RID: 1219
		private string targetFilename;

		// Token: 0x040004C4 RID: 1220
		private string destFilename;

		// Token: 0x040004C5 RID: 1221
		private string password = "";

		// Token: 0x040004C6 RID: 1222
		private DecypherExe.DecypherStatus status = DecypherExe.DecypherStatus.Loading;

		// Token: 0x040004C7 RID: 1223
		private float timeOnThisPhase = 0f;

		// Token: 0x040004C8 RID: 1224
		private float percentComplete = 0f;

		// Token: 0x040004C9 RID: 1225
		private string errorMessage = "Unknown Error";

		// Token: 0x040004CA RID: 1226
		private string displayHeader = "Unknown";

		// Token: 0x040004CB RID: 1227
		private string displayIP = "Unknown";

		// Token: 0x040004CC RID: 1228
		private string writtenFilename = "Unknown";

		// Token: 0x040004CD RID: 1229
		private List<int> rowsActive = new List<int>();

		// Token: 0x040004CE RID: 1230
		private List<int> columnsActive = new List<int>();

		// Token: 0x040004CF RID: 1231
		private float lastLockedPercentage = 0f;

		// Token: 0x040004D0 RID: 1232
		private int rowsDrawn = 10;

		// Token: 0x040004D1 RID: 1233
		private int columnsDrawn = 10;

		// Token: 0x040004D2 RID: 1234
		private int lcgSeed = 1;

		// Token: 0x020000C7 RID: 199
		private enum DecypherStatus
		{
			// Token: 0x040004D4 RID: 1236
			Error,
			// Token: 0x040004D5 RID: 1237
			Loading,
			// Token: 0x040004D6 RID: 1238
			Working,
			// Token: 0x040004D7 RID: 1239
			Complete
		}
	}
}
