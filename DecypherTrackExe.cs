using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x020000C4 RID: 196
	internal class DecypherTrackExe : ExeModule
	{
		// Token: 0x060003FC RID: 1020 RVA: 0x0003E198 File Offset: 0x0003C398
		public DecypherTrackExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.IdentifierName = "DEC File Tracer";
			this.ramCost = 240;
			if (p.Length < 2)
			{
				this.status = DecypherTrackExe.DecHeadStatus.Error;
				this.errorMessage = "No File Provided";
			}
			else
			{
				this.InitializeFiles(p[1]);
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x0003E234 File Offset: 0x0003C434
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

		// Token: 0x060003FE RID: 1022 RVA: 0x0003E2A4 File Offset: 0x0003C4A4
		public override void Update(float t)
		{
			base.Update(t);
			this.timeOnThisPhase += t;
			float num;
			switch (this.status)
			{
			case DecypherTrackExe.DecHeadStatus.Error:
				num = 6f;
				if (this.timeOnThisPhase >= 6f)
				{
					this.isExiting = true;
				}
				goto IL_BD;
			case DecypherTrackExe.DecHeadStatus.Complete:
				num = 10f;
				if (this.timeOnThisPhase >= 10f)
				{
					this.isExiting = true;
				}
				goto IL_BD;
			}
			num = 3.5f;
			if (this.timeOnThisPhase >= 3.5f)
			{
				if (this.CompleteLoading())
				{
					this.status = DecypherTrackExe.DecHeadStatus.Complete;
					this.GetHeaders();
				}
				this.timeOnThisPhase = 0f;
			}
			IL_BD:
			this.percentComplete = this.timeOnThisPhase / num;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x0003E37C File Offset: 0x0003C57C
		private bool CompleteLoading()
		{
			bool result;
			try
			{
				this.targetFile = this.destFolder.searchForFile(this.targetFilename);
				if (this.targetFile == null)
				{
					this.status = DecypherTrackExe.DecHeadStatus.Error;
					this.errorMessage = "File not found";
					result = false;
				}
				else
				{
					int num = FileEncrypter.FileIsEncrypted(this.targetFile.data, "");
					if (num == 0)
					{
						this.status = DecypherTrackExe.DecHeadStatus.Error;
						this.errorMessage = "File is not\nDEC encrypted";
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
				this.status = DecypherTrackExe.DecHeadStatus.Error;
				this.errorMessage = "Fatal error in loading";
				result = false;
			}
			return result;
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x0003E434 File Offset: 0x0003C634
		private void GetHeaders()
		{
			try
			{
				string[] array = FileEncrypter.DecryptHeaders(this.targetFile.data, "");
				this.displayHeader = array[0];
				this.displayIP = array[1];
				this.os.write(" \n \n---------------Header Analysis complete---------------\n \nDEC Encrypted File " + this.targetFilename + " headers:");
				this.os.write("Encryption Header    : \"" + this.displayHeader + "\"");
				this.os.write("Encryption Source IP: \"" + this.displayIP + "\"");
				this.os.write(" \n---------------------------------------------------------\n ");
			}
			catch (Exception ex)
			{
				this.os.write(" \n \n--------------- ERROR ---------------\n \n");
				this.os.write("Fatal Error: " + ex.GetType().ToString());
				this.os.write(ex.Message + "\n\n");
			}
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0003E548 File Offset: 0x0003C748
		private Rectangle DrawLoadingMessage(string message, float startPoint, Rectangle dest, bool showLoading = true, bool highlight = false)
		{
			float num = 0.18f;
			float num2 = (this.percentComplete - startPoint) / num;
			if (this.percentComplete > startPoint)
			{
				dest.Y += 22;
				this.spriteBatch.Draw(Utils.white, dest, Color.Black);
				float point;
				if (this.percentComplete > startPoint + num)
				{
					point = 1f;
				}
				else
				{
					point = num2;
				}
				dest.Width = (int)((float)dest.Width * Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(point)));
				this.spriteBatch.Draw(Utils.white, dest, (this.status == DecypherTrackExe.DecHeadStatus.Complete) ? DecypherTrackExe.LoadingBarColorBlue : DecypherTrackExe.LoadingBarColorRed);
				this.spriteBatch.DrawString(GuiData.tinyfont, message, new Vector2((float)(this.bounds.X + 6), (float)(dest.Y + 2)), highlight ? Color.Black : Color.White);
				if (showLoading)
				{
					if (this.percentComplete > startPoint + num)
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, LocaleTerms.Loc("COMPLETE"), new Vector2((float)(this.bounds.X + 172), (float)(dest.Y + 2)), Color.Black);
					}
					else
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, (num2 * 100f).ToString("00") + "%", new Vector2((float)(this.bounds.X + 195), (float)(dest.Y + 2)), Color.White);
					}
				}
			}
			return dest;
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x0003E714 File Offset: 0x0003C914
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			Rectangle bounds = this.bounds;
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			string text = this.status.ToString();
			switch (this.status)
			{
			case DecypherTrackExe.DecHeadStatus.Error:
			{
				PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.binaryTile);
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
				if (this.bounds.Height > 120)
				{
					this.spriteBatch.Draw(Utils.white, rectangle, this.os.lockedColor);
					this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("ERROR"), new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
				}
				rectangle.Y += 50;
				rectangle.Height = 80;
				rectangle.Width = 250;
				if (this.bounds.Height > 160)
				{
					this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
					this.spriteBatch.DrawString(GuiData.smallfont, this.errorMessage, new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 74)), Color.White);
				}
				break;
			}
			case DecypherTrackExe.DecHeadStatus.Loading:
			{
				PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.lockedColor, this.spriteBatch, PatternDrawer.binaryTile);
				Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 50);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
				this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Loading") + "...", new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
				rectangle.Height = 20;
				rectangle.Width = 240;
				rectangle.Y += 28;
				rectangle = this.DrawLoadingMessage("Reading File...", 0f, rectangle, true, false);
				rectangle = this.DrawLoadingMessage("Parsing Headers...", 0.18f, rectangle, true, false);
				rectangle = this.DrawLoadingMessage("Verifying Content...", 0.36f, rectangle, true, false);
				rectangle = this.DrawLoadingMessage("Checking DEC...", 0.54f, rectangle, true, false);
				rectangle = this.DrawLoadingMessage("Reading Codes...", 0.72f, rectangle, true, false);
				break;
			}
			case DecypherTrackExe.DecHeadStatus.Complete:
				PatternDrawer.draw(bounds, 1.2f, Color.Transparent, this.os.highlightColor * 0.5f, this.spriteBatch, PatternDrawer.binaryTile);
				if (this.bounds.Height > 70)
				{
					Rectangle rectangle = new Rectangle(this.bounds.X + 1, this.bounds.Y + 20, 200, 85);
					this.spriteBatch.Draw(Utils.white, rectangle, Color.Black);
					this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Operation\nComplete"), new Vector2((float)(this.bounds.X + 6), (float)(this.bounds.Y + 24)), Color.White);
					rectangle.Height = 20;
					rectangle.Width = 240;
					rectangle.Y += 63;
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height - 10)
					{
						rectangle = this.DrawLoadingMessage("Headers:", 0f, rectangle, false, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height - 10)
					{
						rectangle = this.DrawLoadingMessage("\"" + this.displayHeader + "\"", 0.1f, rectangle, false, true);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height - 10)
					{
						rectangle = this.DrawLoadingMessage("Source IP:", 0.2f, rectangle, false, false);
					}
					if (rectangle.Y + rectangle.Height < this.bounds.Y + this.bounds.Height - 10)
					{
						rectangle = this.DrawLoadingMessage("\"" + this.displayIP + "\"", 0.3f, rectangle, false, true);
					}
				}
				break;
			}
		}

		// Token: 0x040004A8 RID: 1192
		private const float LOADING_TIME = 3.5f;

		// Token: 0x040004A9 RID: 1193
		private const float COMPLETE_TIME = 10f;

		// Token: 0x040004AA RID: 1194
		private const float ERROR_TIME = 6f;

		// Token: 0x040004AB RID: 1195
		private Computer targetComputer;

		// Token: 0x040004AC RID: 1196
		private Folder destFolder;

		// Token: 0x040004AD RID: 1197
		private FileEntry targetFile;

		// Token: 0x040004AE RID: 1198
		private string targetFilename;

		// Token: 0x040004AF RID: 1199
		private string destFilename;

		// Token: 0x040004B0 RID: 1200
		private DecypherTrackExe.DecHeadStatus status = DecypherTrackExe.DecHeadStatus.Loading;

		// Token: 0x040004B1 RID: 1201
		private float timeOnThisPhase = 0f;

		// Token: 0x040004B2 RID: 1202
		private float percentComplete = 0f;

		// Token: 0x040004B3 RID: 1203
		private string errorMessage = "Unknown Error";

		// Token: 0x040004B4 RID: 1204
		private string displayHeader = "Unknown";

		// Token: 0x040004B5 RID: 1205
		private string displayIP = "Unknown";

		// Token: 0x040004B6 RID: 1206
		private static Color LoadingBarColorRed = new Color(196, 29, 60, 80);

		// Token: 0x040004B7 RID: 1207
		private static Color LoadingBarColorBlue = new Color(29, 113, 196, 80);

		// Token: 0x020000C5 RID: 197
		private enum DecHeadStatus
		{
			// Token: 0x040004B9 RID: 1209
			Error,
			// Token: 0x040004BA RID: 1210
			Loading,
			// Token: 0x040004BB RID: 1211
			Complete
		}
	}
}
