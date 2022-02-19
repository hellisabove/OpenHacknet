using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000065 RID: 101
	internal class MemoryForensicsExe : ExeModule, MainDisplayOverrideEXE
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0001B4B4 File Offset: 0x000196B4
		// (set) Token: 0x060001F7 RID: 503 RVA: 0x0001B4CB File Offset: 0x000196CB
		public bool DisplayOverrideIsActive { get; set; }

		// Token: 0x060001F8 RID: 504 RVA: 0x0001B4D4 File Offset: 0x000196D4
		public MemoryForensicsExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.needsProxyAccess = false;
			this.name = "MemForensics";
			this.ramCost = 300;
			this.IdentifierName = "MemForensics";
			if (p.Length <= 1)
			{
				this.ErrorMessage = "No file specified.";
				this.State = MemoryForensicsExe.MemForensicsState.Error;
			}
			else
			{
				this.LoadFile(p[1], Programs.getCurrentFolder(this.os));
			}
			this.DisplayOverrideIsActive = true;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0001B608 File Offset: 0x00019808
		public static MemoryForensicsExe GenerateInstanceOrNullFromArguments(Rectangle location, OS os, string[] p)
		{
			for (int i = 0; i < os.exes.Count; i++)
			{
				MemoryForensicsExe memoryForensicsExe = os.exes[i] as MemoryForensicsExe;
				if (memoryForensicsExe != null)
				{
					if (p.Length > 1)
					{
						memoryForensicsExe.LoadFile(p[1], Programs.getCurrentFolder(os));
					}
					else
					{
						memoryForensicsExe.State = MemoryForensicsExe.MemForensicsState.SelectingFile;
						memoryForensicsExe.timeInCurrentState = 0f;
					}
					return null;
				}
			}
			return new MemoryForensicsExe(location, os, p);
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0001B694 File Offset: 0x00019894
		internal void LoadFile(string filename, Folder f)
		{
			FileEntry fileEntry = f.searchForFile(filename);
			if (fileEntry == null)
			{
				fileEntry = f.searchForFile(filename + ".md");
				if (fileEntry == null)
				{
					this.State = MemoryForensicsExe.MemForensicsState.Error;
					this.ErrorMessage = string.Format(LocaleTerms.Loc("File {0} not found in current folder!"), filename);
					return;
				}
			}
			try
			{
				this.ActiveMem = MemoryContents.GetMemoryFromEncodedFileString(fileEntry.data);
				this.filenameLoaded = filename;
				this.State = MemoryForensicsExe.MemForensicsState.ReadingFile;
				this.timeInCurrentState = 0f;
			}
			catch (Exception ex)
			{
				this.State = MemoryForensicsExe.MemForensicsState.Error;
				this.ErrorMessage = LocaleTerms.Loc("Error deserializing memory dump.") + "\r\n\r\n" + Utils.GenerateReportFromException(ex);
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0001B760 File Offset: 0x00019960
		public override void Update(float t)
		{
			base.Update(t);
			this.timeInCurrentState += t;
			if (this.State == MemoryForensicsExe.MemForensicsState.ReadingFile)
			{
				if (this.timeInCurrentState >= 2f)
				{
					this.timeInCurrentState = 0f;
					this.State = MemoryForensicsExe.MemForensicsState.Main;
				}
			}
			if (this.State == MemoryForensicsExe.MemForensicsState.Processing)
			{
				if (this.timeInCurrentState >= this.processingTimeThisBatch)
				{
					this.timeInCurrentState = 0f;
					this.State = MemoryForensicsExe.MemForensicsState.DisplayingSolution;
				}
			}
			if (this.State == MemoryForensicsExe.MemForensicsState.Main)
			{
				this.OutputData.Clear();
			}
			this.GridEffect.Update(t);
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0001B820 File Offset: 0x00019A20
		private void StartLoadingInTexturesForMemory()
		{
			this.OutputTextures.Clear();
			for (int i = 0; i < this.OutputData.Count; i++)
			{
				string text = this.OutputData[i];
				Texture2D item = null;
				string text2 = Utils.GetFileLoadPrefix() + text;
				if (text2.EndsWith(".jpg") || text2.EndsWith(".png"))
				{
					using (FileStream fileStream = File.OpenRead(text2))
					{
						item = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, fileStream);
					}
				}
				else
				{
					item = this.os.content.Load<Texture2D>(text);
				}
				this.OutputTextures.Add(item);
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0001B908 File Offset: 0x00019B08
		public override void Completed()
		{
			base.Completed();
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0001B930 File Offset: 0x00019B30
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle contentAreaDest = base.GetContentAreaDest();
			if (this.flyoutEffect == null)
			{
				this.flyoutEffect = new FlyoutEffect(GuiData.spriteBatch.GraphicsDevice, this.os.content, contentAreaDest.Width, contentAreaDest.Height);
			}
			this.flyoutEffect.Draw((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds, contentAreaDest, this.spriteBatch, delegate(SpriteBatch sb, Rectangle innerDest)
			{
				ZoomingDotGridEffect.Render(innerDest, sb, this.os.timer, this.ThemeColorDark);
			});
			this.spriteBatch.Draw(Utils.white, contentAreaDest, Color.Black * 0.6f);
			if (!this.isExiting)
			{
				Rectangle rectangle = new Rectangle(contentAreaDest.X + 8, contentAreaDest.Y + contentAreaDest.Height - 30 - 8, contentAreaDest.Width - 16, 30);
				if (Button.doButton(383023811 + this.PID, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
				rectangle.Y -= rectangle.Height + 8;
				if (Button.doButton(383023822 + this.PID, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, this.DisplayOverrideIsActive ? LocaleTerms.Loc("Close Main Display") : LocaleTerms.Loc("Re-Open Main Display"), new Color?(this.ThemeColorMain)))
				{
					this.DisplayOverrideIsActive = !this.DisplayOverrideIsActive;
				}
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0001BB18 File Offset: 0x00019D18
		private void MoveToProcessing()
		{
			this.State = MemoryForensicsExe.MemForensicsState.Processing;
			this.timeInCurrentState = 0f;
			this.processingTimeThisBatch = 2f + (Utils.randm(2f) - 1f);
			this.PanelScroll = Vector2.Zero;
			this.IsDisplayingImages = false;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0001BB68 File Offset: 0x00019D68
		public void RenderMainDisplay(Rectangle dest, SpriteBatch sb)
		{
			dest = this.RenderMainDisplayHeaders(dest, sb);
			switch (this.State)
			{
			case MemoryForensicsExe.MemForensicsState.Error:
			{
				PatternDrawer.draw(dest, 1f, Color.Transparent, Utils.AddativeRed, this.spriteBatch, PatternDrawer.errorTile);
				Rectangle rectangle = new Rectangle(dest.X, dest.Y + dest.Height / 3, dest.Width, 60);
				sb.Draw(Utils.white, rectangle, Color.Black * 0.4f);
				TextItem.doFontLabelToSize(rectangle, LocaleTerms.Loc("ERROR"), GuiData.font, Color.White, true, true);
				Rectangle rectangle2 = new Rectangle(dest.X, rectangle.Y + rectangle.Height + 2, dest.Width, dest.Height / 2);
				sb.Draw(Utils.white, rectangle2, Color.Black * 0.8f);
				string text = (this.ErrorMessage.Length > 500) ? this.ErrorMessage.Substring(0, 500) : this.ErrorMessage;
				text = Utils.SuperSmartTwimForWidth(text, rectangle2.Width, GuiData.smallfont);
				TextItem.doFontLabelToSize(rectangle2, text, GuiData.smallfont, Utils.AddativeRed, true, true);
				if (Button.doButton(381023801, dest.X + 2, rectangle2.Y + rectangle2.Height + 2, this.bounds.Width / 3, 30, LocaleTerms.Loc("Close"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
				break;
			}
			case MemoryForensicsExe.MemForensicsState.ReadingFile:
			{
				Rectangle bounds = new Rectangle(dest.X, dest.Y, (int)((float)dest.Width * (this.timeInCurrentState / 2f)), dest.Height);
				bounds = new Rectangle(dest.X + (int)((float)dest.Width * (1f - this.timeInCurrentState / 2f)), dest.Y, (int)((float)dest.Width * (this.timeInCurrentState / 2f)), dest.Height);
				this.GridEffect.RenderGrid(bounds, sb, this.ThemeColorDark * 0.5f, this.ThemeColorMain, this.ThemeColorDark, false);
				break;
			}
			case MemoryForensicsExe.MemForensicsState.Main:
				this.DrawMainStateBackground(dest, sb);
				this.RenderMenuMainState(dest, sb);
				break;
			case MemoryForensicsExe.MemForensicsState.Processing:
				this.DrawMainStateBackground(dest, sb);
				this.RenderResultsDisplayMainState(dest, sb, true);
				break;
			case MemoryForensicsExe.MemForensicsState.DisplayingSolution:
				this.DrawMainStateBackground(dest, sb);
				this.RenderResultsDisplayMainState(dest, sb, false);
				break;
			}
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0001BE28 File Offset: 0x0001A028
		private Rectangle RenderMainDisplayHeaders(Rectangle dest, SpriteBatch sb)
		{
			if (Button.doButton(381023001, dest.X + 3, dest.Y + 2, dest.Width / 2, 25, LocaleTerms.Loc("Close Display"), new Color?(Color.Gray)))
			{
				this.DisplayOverrideIsActive = false;
			}
			dest.Y += 30;
			dest.Height -= 30;
			return dest;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0001BEA7 File Offset: 0x0001A0A7
		private void DrawMainStateBackground(Rectangle dest, SpriteBatch sb)
		{
			this.GridEffect.RenderGrid(dest, sb, this.ThemeColorLight, this.ThemeColorMain, this.ThemeColorDark, true);
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0001BED4 File Offset: 0x0001A0D4
		private void RenderMenuMainState(Rectangle dest, SpriteBatch sb)
		{
			int num = 5;
			int num2 = Math.Min(dest.Height / (num + 1), 32);
			int num3 = (int)((float)(dest.Height - num2 * (num + 1)) / 2f);
			bool flag = this.State == MemoryForensicsExe.MemForensicsState.Processing;
			Color value = flag ? Color.Gray : Color.White;
			Rectangle dest2 = new Rectangle(dest.X + 20, dest.Y + num3, (int)((float)dest.Width * 0.9f), num2);
			Rectangle destinationRectangle = new Rectangle(dest.X + 2, dest2.Y - 4, dest.Width - 4, dest2.Height * (num + 1));
			sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.8f);
			if (num2 > 6)
			{
				TextItem.doFontLabelToSize(dest2, LocaleTerms.Loc("Memory Dump") + " : " + this.filenameLoaded, GuiData.smallfont, Color.LightGray, true, true);
				dest2.Y += dest2.Height + 2;
				if (Button.doButton(381023801, dest2.X, dest2.Y, dest2.Width, dest2.Height, LocaleTerms.Loc("Process Recent Commands Run..."), new Color?(value)) && !flag)
				{
					this.MoveToProcessing();
					this.OutputData.Clear();
					this.OutputData.AddRange(this.ActiveMem.CommandsRun);
					this.AnnouncementData = LocaleTerms.Loc("Results for recently run commands remaining in cached memory") + "::";
				}
				dest2.Y += dest2.Height + 2;
				if (Button.doButton(381023803, dest2.X, dest2.Y, dest2.Width, dest2.Height, LocaleTerms.Loc("Process Files in Memory..."), new Color?(value)) && !flag)
				{
					this.MoveToProcessing();
					this.OutputData.Clear();
					this.OutputData.AddRange(this.ActiveMem.DataBlocks);
					this.AnnouncementData = LocaleTerms.Loc("Results for accessed non-binary file fragments in cached memory") + "::";
				}
				dest2.Y += dest2.Height + 2;
				if (Button.doButton(381023807, dest2.X, dest2.Y, dest2.Width, dest2.Height, LocaleTerms.Loc("Process Images in Memory..."), new Color?(value)) && !flag)
				{
					this.MoveToProcessing();
					this.IsDisplayingImages = true;
					this.OutputData.Clear();
					this.OutputData.AddRange(this.ActiveMem.Images);
					Task.Factory.StartNew(delegate()
					{
						this.StartLoadingInTexturesForMemory();
					});
					this.AnnouncementData = LocaleTerms.Loc("Results for accessed image-type tagged binary fragments in cached memory") + "::";
				}
				dest2.Y += dest2.Height + 2;
				if (Button.doButton(381023809, dest2.X, dest2.Y, dest2.Width, dest2.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
				dest2.Y += dest2.Height + 2;
			}
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0001C270 File Offset: 0x0001A470
		private void RenderResultsDisplayMainState(Rectangle dest, SpriteBatch sb, bool isProcessing)
		{
			Color color = new Color(5, 5, 5, 230);
			Rectangle rectangle = new Rectangle(dest.X + 22, dest.Y + 30, dest.Width - 44, 40);
			sb.Draw(Utils.white, rectangle, color);
			rectangle.X += 10;
			TextItem.doFontLabelToSize(rectangle, isProcessing ? LocaleTerms.Loc("PROCESSING") : LocaleTerms.Loc("OUTPUT"), GuiData.font, Color.White, true, true);
			rectangle.X -= 10;
			int num = 200;
			int num2 = 30;
			if (Button.doButton(381023909, rectangle.X + rectangle.Width - (num + 6), rectangle.Y + (rectangle.Height / 2 - num2 / 2), num, num2, isProcessing ? LocaleTerms.Loc("Cancel") : LocaleTerms.Loc("Return to Menu"), new Color?(Color.White)))
			{
				this.State = MemoryForensicsExe.MemForensicsState.Main;
				this.timeInCurrentState = 0f;
			}
			rectangle.Y += rectangle.Height;
			rectangle.Height = 1;
			sb.Draw(Utils.white, rectangle, Color.White);
			if (isProcessing)
			{
				Rectangle destinationRectangle = new Rectangle(dest.X + 2, rectangle.Y + rectangle.Height + 2, dest.Width - 4, 20);
				sb.Draw(Utils.white, destinationRectangle, Color.Black);
				float num3 = this.timeInCurrentState / this.processingTimeThisBatch;
				destinationRectangle.Width = (int)((float)destinationRectangle.Width * num3);
				sb.Draw(Utils.white, destinationRectangle, Utils.makeColorAddative(Color.LightBlue));
			}
			else
			{
				Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 2, rectangle.Width, 1);
				rectangle2.Height = dest.Y + dest.Height - (rectangle.Y + rectangle.Height + 4);
				sb.Draw(Utils.white, rectangle2, color);
				if (this.OutputData.Count <= 0)
				{
					TextItem.doFontLabelToSize(rectangle2, " - " + LocaleTerms.Loc("No Valid Matches Found") + " - ", GuiData.smallfont, Color.White, true, false);
				}
				else
				{
					Rectangle drawbounds = new Rectangle(0, 0, rectangle2.Width, rectangle2.Height);
					float num4 = 10f;
					for (int i = 0; i < this.OutputData.Count; i++)
					{
						if (this.IsDisplayingImages)
						{
							if (this.OutputTextures.Count < i)
							{
								break;
							}
							num4 += (float)(Math.Min(drawbounds.Width - 16, this.OutputTextures[i].Height) + 30);
						}
						else
						{
							num4 += 26f + GuiData.smallfont.MeasureString(Utils.SuperSmartTwimForWidth(this.OutputData[i], drawbounds.Width - 16, GuiData.smallfont)).Y;
						}
					}
					drawbounds.Height = (int)num4;
					ScrollablePanel.beginPanel(3371001, drawbounds, this.PanelScroll);
					Vector2 pos = new Vector2((float)(drawbounds.X + 14), (float)drawbounds.Y);
					TextItem.doMeasuredFontLabel(pos, this.AnnouncementData, GuiData.tinyfont, new Color?(Color.White), float.MaxValue, float.MaxValue);
					pos.Y += 20f;
					Rectangle destinationRectangle2 = new Rectangle(drawbounds.X, (int)pos.Y, drawbounds.Width, 1);
					GuiData.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.White);
					pos.Y += 20f;
					for (int i = 0; i < this.OutputData.Count; i++)
					{
						if (this.IsDisplayingImages)
						{
							if (this.OutputTextures.Count < i)
							{
								break;
							}
							Rectangle dest2 = new Rectangle((int)pos.X + 8, (int)pos.Y, drawbounds.Width - 16, drawbounds.Width - 16);
							dest2.Height = Math.Min(dest2.Height, this.OutputTextures[i].Height);
							Rectangle rectangle3 = Utils.DrawSpriteAspectCorrect(dest2, GuiData.spriteBatch, this.OutputTextures[i], Color.White, false);
							float num5 = (float)(rectangle3.Y - dest2.Y + rectangle3.Height);
							pos.Y += num5 + 12f;
						}
						else
						{
							string text = Utils.SuperSmartTwimForWidth(this.OutputData[i], drawbounds.Width - 16, GuiData.smallfont);
							Vector2 vector = TextItem.doMeasuredFontLabel(pos, text, GuiData.smallfont, new Color?(Color.White * 0.9f), float.MaxValue, float.MaxValue);
							Rectangle destinationRectangle3 = new Rectangle(drawbounds.X + 6, (int)pos.Y + (int)((vector.Y - 3f) / 2f), 3, 3);
							GuiData.spriteBatch.Draw(Utils.white, destinationRectangle3, Color.Gray);
							pos.Y += vector.Y + 6f;
						}
						destinationRectangle2 = new Rectangle(drawbounds.X, (int)pos.Y, rectangle2.Width, 1);
						GuiData.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Gray);
						pos.Y += 10f;
					}
					this.PanelScroll = ScrollablePanel.endPanel(3371001, this.PanelScroll, rectangle2, 2000f, false);
					float num6 = num4 - (float)rectangle2.Height;
					if (this.PanelScroll.Y > num6)
					{
						this.PanelScroll.Y = num6;
					}
				}
			}
		}

		// Token: 0x04000221 RID: 545
		private const float FileReadTime = 2f;

		// Token: 0x04000222 RID: 546
		private const float BaseProcessingTime = 2f;

		// Token: 0x04000223 RID: 547
		public MemoryForensicsExe.MemForensicsState State = MemoryForensicsExe.MemForensicsState.ReadingFile;

		// Token: 0x04000224 RID: 548
		private string ErrorMessage = "Unknown Error";

		// Token: 0x04000225 RID: 549
		private float timeInCurrentState = 0f;

		// Token: 0x04000226 RID: 550
		private float processingTimeThisBatch = 4f;

		// Token: 0x04000227 RID: 551
		private string filenameLoaded = "UNKNOWN";

		// Token: 0x04000228 RID: 552
		private MemoryContents ActiveMem = null;

		// Token: 0x04000229 RID: 553
		private ShiftingGridEffect GridEffect = new ShiftingGridEffect();

		// Token: 0x0400022A RID: 554
		private Color ThemeColorMain = new Color(30, 59, 44, 0);

		// Token: 0x0400022B RID: 555
		private Color ThemeColorLight = new Color(89, 181, 183, 0);

		// Token: 0x0400022C RID: 556
		private Color ThemeColorDark = new Color(19, 51, 35, 0);

		// Token: 0x0400022D RID: 557
		private List<string> OutputData = new List<string>();

		// Token: 0x0400022E RID: 558
		private bool IsDisplayingImages = false;

		// Token: 0x0400022F RID: 559
		private List<Texture2D> OutputTextures = new List<Texture2D>();

		// Token: 0x04000230 RID: 560
		private string AnnouncementData = "Unknown";

		// Token: 0x04000231 RID: 561
		private Vector2 PanelScroll = Vector2.Zero;

		// Token: 0x04000232 RID: 562
		private FlyoutEffect flyoutEffect;

		// Token: 0x02000066 RID: 102
		public enum MemForensicsState
		{
			// Token: 0x04000235 RID: 565
			Error,
			// Token: 0x04000236 RID: 566
			SelectingFile,
			// Token: 0x04000237 RID: 567
			ReadingFile,
			// Token: 0x04000238 RID: 568
			Main,
			// Token: 0x04000239 RID: 569
			Processing,
			// Token: 0x0400023A RID: 570
			DisplayingSolution
		}
	}
}
