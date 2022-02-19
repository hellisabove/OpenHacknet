using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000C2 RID: 194
	internal class ThemeChangerExe : ExeModule
	{
		// Token: 0x060003F3 RID: 1011 RVA: 0x0003D1C4 File Offset: 0x0003B3C4
		public ThemeChangerExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "ThemeSwitch";
			this.ramCost = 320;
			this.IdentifierName = "Theme Switch";
			this.targetIP = this.os.thisComputer.ip;
			this.circle = TextureBank.load("Circle", this.os.content);
			AchievementsManager.Unlock("themeswitch_run", false);
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x0003D268 File Offset: 0x0003B468
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			this.themeColor = this.os.highlightColor;
			Rectangle dest = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width - 4, this.bounds.Height - 2 - Module.PANEL_HEIGHT);
			switch (this.state)
			{
			case ThemeChangerExe.ThemeChangerState.Startup:
				this.loadingTimeRemaining -= t;
				this.DrawLoading(this.loadingTimeRemaining, 25.5f, dest, this.spriteBatch);
				return;
			case ThemeChangerExe.ThemeChangerState.LoadItem:
				return;
			case ThemeChangerExe.ThemeChangerState.Activating:
				return;
			}
			this.DrawListing(dest, this.spriteBatch);
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x0003D344 File Offset: 0x0003B544
		public void DrawWithSeperateRT(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			this.themeColor = this.os.highlightColor;
			Rectangle destinationRectangle = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 2, this.bounds.Width - 4, this.bounds.Height - 3 - Module.PANEL_HEIGHT);
			bool flag = false;
			if (this.target == null)
			{
				this.target = new RenderTarget2D(this.spriteBatch.GraphicsDevice, destinationRectangle.Width, destinationRectangle.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				this.internalSB = new SpriteBatch(this.spriteBatch.GraphicsDevice);
				flag = true;
			}
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			this.spriteBatch.GraphicsDevice.SetRenderTarget(this.target);
			Rectangle dest = new Rectangle(0, 0, destinationRectangle.Width, destinationRectangle.Height);
			SpriteBatch spriteBatch = GuiData.spriteBatch;
			GuiData.spriteBatch = this.internalSB;
			this.internalSB.Begin();
			if (flag)
			{
				this.internalSB.GraphicsDevice.Clear(Color.Transparent);
			}
			switch (this.state)
			{
			case ThemeChangerExe.ThemeChangerState.Startup:
				this.loadingTimeRemaining -= t;
				this.DrawLoading(this.loadingTimeRemaining, 25.5f, dest, this.internalSB);
				goto IL_1B3;
			case ThemeChangerExe.ThemeChangerState.LoadItem:
				goto IL_1B3;
			case ThemeChangerExe.ThemeChangerState.Activating:
				goto IL_1B3;
			}
			this.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
			this.DrawListing(dest, this.internalSB);
			IL_1B3:
			this.internalSB.End();
			GuiData.spriteBatch = spriteBatch;
			this.spriteBatch.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			this.spriteBatch.Draw(this.target, destinationRectangle, Color.White);
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0003D544 File Offset: 0x0003B744
		private void DrawListing(Rectangle dest, SpriteBatch sb)
		{
			this.DrawHeaders(dest, sb);
			if (!this.isExiting)
			{
				Vector2 pos = new Vector2((float)(dest.X + 2), (float)dest.Y + 60f);
				TextItem.doFontLabel(pos, LocaleTerms.Loc("Remote"), GuiData.smallfont, new Color?(this.themeColor), (float)(this.bounds.Width - 20), 20f, false);
				pos.Y += 18f;
				sb.Draw(Utils.white, new Rectangle(this.bounds.X + 2, (int)pos.Y, this.bounds.Width - 6, 1), Utils.AddativeWhite);
				List<string> list = new List<string>();
				Folder folder = Programs.getCurrentFolder(this.os);
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (ThemeManager.getThemeForDataString(folder.files[i].data) != OSTheme.TerminalOnlyBlack)
					{
						list.Add(folder.files[i].name);
					}
				}
				string text = null;
				string selectedFileData = null;
				Color value = Color.Lerp(this.os.topBarColor, Utils.AddativeWhite, 0.2f);
				int scrollOffset = SelectableTextList.scrollOffset;
				Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y, this.bounds.Width - 6, 54);
				if (list.Count > 0)
				{
					SelectableTextList.scrollOffset = this.remoteScroll;
					this.remotesSelected = SelectableTextList.doFancyList(8139191 + this.PID, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, list.ToArray(), this.remotesSelected, new Color?(value), true);
					if (SelectableTextList.selectionWasChanged)
					{
						this.localsSelected = -1;
					}
					this.remoteScroll = SelectableTextList.scrollOffset;
					if (this.remotesSelected >= 0)
					{
						if (this.remotesSelected >= list.Count)
						{
							this.remotesSelected = -1;
						}
						else
						{
							text = list[this.remotesSelected];
							selectedFileData = folder.searchForFile(text).data;
						}
					}
				}
				else
				{
					sb.Draw(Utils.white, rectangle, Utils.VeryDarkGray);
					TextItem.doFontLabelToSize(rectangle, "    -- " + LocaleTerms.Loc("No Valid Files") + " --    ", GuiData.smallfont, Utils.AddativeWhite, false, false);
				}
				pos.Y += (float)(rectangle.Height + 6);
				TextItem.doFontLabel(pos, LocaleTerms.Loc("Local Theme Files"), GuiData.smallfont, new Color?(this.themeColor), (float)(this.bounds.Width - 20), 20f, false);
				pos.Y += 18f;
				sb.Draw(Utils.white, new Rectangle(this.bounds.X + 2, (int)pos.Y, this.bounds.Width - 6, 1), Utils.AddativeWhite);
				list.Clear();
				List<string> list2 = new List<string>();
				folder = this.os.thisComputer.files.root.searchForFolder("sys");
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (ThemeManager.getThemeForDataString(folder.files[i].data) != OSTheme.TerminalOnlyBlack)
					{
						list.Add(folder.files[i].name);
						list2.Add(folder.files[i].data);
					}
				}
				folder = this.os.thisComputer.files.root.searchForFolder("home");
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (ThemeManager.getThemeForDataString(folder.files[i].data) != OSTheme.TerminalOnlyBlack)
					{
						list.Add(folder.files[i].name);
						list2.Add(folder.files[i].data);
					}
				}
				Rectangle rectangle2 = new Rectangle((int)pos.X, (int)pos.Y, this.bounds.Width - 6, 72);
				if (list.Count > 0)
				{
					SelectableTextList.scrollOffset = this.localScroll;
					this.localsSelected = SelectableTextList.doFancyList(839192 + this.PID, rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height, list.ToArray(), this.localsSelected, new Color?(value), true);
					if (SelectableTextList.selectionWasChanged)
					{
						this.remotesSelected = -1;
					}
					this.localScroll = SelectableTextList.scrollOffset;
					if (this.localsSelected >= 0)
					{
						text = list[this.localsSelected];
						selectedFileData = list2[this.localsSelected];
					}
				}
				else
				{
					sb.Draw(Utils.white, rectangle2, Utils.VeryDarkGray);
					TextItem.doFontLabelToSize(rectangle2, "    -- " + LocaleTerms.Loc("No Valid Files") + " --    ", GuiData.smallfont, Utils.AddativeWhite, false, false);
				}
				SelectableTextList.scrollOffset = scrollOffset;
				pos.Y += (float)(rectangle2.Height + 2);
				Rectangle bounds = new Rectangle(this.bounds.X + 4, (int)pos.Y + 2, this.bounds.Width - 8, (int)((float)dest.Height - (pos.Y - (float)dest.Y)) - 4);
				this.DrawApplyField(text, selectedFileData, bounds, sb);
			}
		}

		// Token: 0x060003F7 RID: 1015 RVA: 0x0003DB44 File Offset: 0x0003BD44
		private void DrawApplyField(string selectedFilename, string selectedFileData, Rectangle bounds, SpriteBatch sb)
		{
			sb.Draw(Utils.white, bounds, Utils.VeryDarkGray);
			sb.Draw(Utils.white, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), Utils.AddativeWhite);
			if (selectedFileData != null && selectedFilename != null)
			{
				OSTheme themeForDataString = ThemeManager.getThemeForDataString(selectedFileData);
				Color representativeColorForTheme = ThemeManager.GetRepresentativeColorForTheme(themeForDataString);
				Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 4 * 3, bounds.Y + 2, bounds.Width / 4, bounds.Height - 2);
				sb.Draw(Utils.white, destinationRectangle, representativeColorForTheme);
				destinationRectangle.X += destinationRectangle.Width - 15;
				destinationRectangle.Width = 15;
				sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.6f);
				TextItem.doFontLabel(new Vector2((float)bounds.X, (float)(bounds.Y + 2)), selectedFilename, GuiData.smallfont, new Color?(Utils.AddativeWhite), (float)(bounds.Width / 4 * 3), 25f, false);
				if (Button.doButton(3837791 + this.PID, bounds.X, bounds.Y + 25, bounds.Width / 6 * 5, 30, LocaleTerms.Loc("Activate Theme"), new Color?(representativeColorForTheme)))
				{
					this.ApplyTheme(selectedFilename, selectedFileData);
				}
			}
		}

		// Token: 0x060003F8 RID: 1016 RVA: 0x0003DCC8 File Offset: 0x0003BEC8
		private void ApplyTheme(string themeFilename, string fileData)
		{
			string text = "x-serverBACKUP";
			Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
			FileEntry fileEntry = folder.searchForFile("x-server.sys");
			if (fileEntry != null)
			{
				bool flag = true;
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (folder.files[i].name.StartsWith(text))
					{
						if (folder.files[i].data == fileEntry.data)
						{
							flag = false;
							break;
						}
					}
				}
				if (flag)
				{
					string nonRepeatingFilename = Utils.GetNonRepeatingFilename(text, ".sys", folder);
					folder.files.Add(new FileEntry(fileEntry.data, nonRepeatingFilename));
				}
			}
			fileEntry.data = fileData;
			ThemeManager.switchTheme(this.os, ThemeManager.getThemeForDataString(fileData));
		}

		// Token: 0x060003F9 RID: 1017 RVA: 0x0003DDD8 File Offset: 0x0003BFD8
		private void DrawHeaders(Rectangle dest, SpriteBatch sb)
		{
			if (this.barcodeEffect == null)
			{
				this.barcodeEffect = new BarcodeEffect(this.bounds.Width - 4, false, false);
			}
			this.barcodeEffect.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			this.barcodeEffect.Draw(dest.X, dest.Y, dest.Width - 4, Math.Min(30, this.bounds.Height - Module.PANEL_HEIGHT), sb, new Color?(this.themeColor));
			if (this.bounds.Height > Module.PANEL_HEIGHT + 60)
			{
				TextItem.doFontLabel(new Vector2((float)(dest.X + 2), (float)(dest.Y + 32)), "Themechanger", GuiData.font, new Color?(Utils.AddativeWhite * 0.8f), (float)(this.bounds.Width - 80), 30f, false);
				int num = 60;
				if (Button.doButton(this.PID + 228173, this.bounds.X + this.bounds.Width - num - 2, dest.Y + 38, num, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.isExiting = true;
				}
			}
		}

		// Token: 0x060003FA RID: 1018 RVA: 0x0003DF58 File Offset: 0x0003C158
		private void DrawLoading(float timeRemaining, float totalTime, Rectangle dest, SpriteBatch sb)
		{
			float num = 20f;
			Vector2 loaderCentre = new Vector2((float)dest.X + (float)dest.Width / 2f, (float)dest.Y + (float)dest.Height / 2f);
			float num2 = totalTime - timeRemaining;
			float num3 = num2 / totalTime;
			sb.Draw(Utils.white, dest, Color.Black * 0.05f);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num, num2 * 0.2f, 1f, sb);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num + 20f * num3, num2 * -0.4f, 0.7f, sb);
			this.DrawLoadingCircle(timeRemaining, totalTime, dest, loaderCentre, num + 35f * num3, num2 * 0.5f, 0.52f, sb);
		}

		// Token: 0x060003FB RID: 1019 RVA: 0x0003E028 File Offset: 0x0003C228
		private void DrawLoadingCircle(float timeRemaining, float totalTime, Rectangle dest, Vector2 loaderCentre, float loaderRadius, float baseRotationAdd, float rotationRateRPS, SpriteBatch sb)
		{
			float num = totalTime - timeRemaining;
			int num2 = 10;
			for (int i = 0; i < num2; i++)
			{
				float num3 = (float)i / (float)num2;
				float num4 = 2f;
				float num5 = 1f;
				float num6 = 6.2831855f;
				float num7 = num6 + num5;
				float num8 = num3 * num4;
				if (num > num8)
				{
					float num9 = num / num8 * rotationRateRPS % num7;
					if (num9 >= num6)
					{
						num9 = 0f;
					}
					num9 = num6 * Utils.QuadraticOutCurve(num9 / num6);
					num9 += baseRotationAdd;
					Vector2 vector = loaderCentre + Utils.PolarToCartesian(num9, loaderRadius);
					sb.Draw(this.circle, vector, null, Utils.AddativeWhite, 0f, Vector2.Zero, 0.1f * (loaderRadius / 120f), SpriteEffects.None, 0.3f);
					if (Utils.random.NextDouble() < 0.001)
					{
						Vector2 vector2 = loaderCentre + Utils.PolarToCartesian(num9, 20f + Utils.randm(45f));
						sb.Draw(Utils.white, vector, Utils.AddativeWhite);
						Utils.drawLine(sb, vector, vector2, Vector2.Zero, Utils.AddativeWhite * 0.4f, 0.1f);
					}
				}
			}
		}

		// Token: 0x04000497 RID: 1175
		private const float START_LOADING_TIME = 25.5f;

		// Token: 0x04000498 RID: 1176
		private RenderTarget2D target;

		// Token: 0x04000499 RID: 1177
		private float loadingTimeRemaining = 25.5f;

		// Token: 0x0400049A RID: 1178
		private ThemeChangerExe.ThemeChangerState state = ThemeChangerExe.ThemeChangerState.List;

		// Token: 0x0400049B RID: 1179
		private SpriteBatch internalSB;

		// Token: 0x0400049C RID: 1180
		private Texture2D circle;

		// Token: 0x0400049D RID: 1181
		private BarcodeEffect barcodeEffect;

		// Token: 0x0400049E RID: 1182
		private Color themeColor;

		// Token: 0x0400049F RID: 1183
		private int remotesSelected = -1;

		// Token: 0x040004A0 RID: 1184
		private int localsSelected = -1;

		// Token: 0x040004A1 RID: 1185
		private int remoteScroll = 0;

		// Token: 0x040004A2 RID: 1186
		private int localScroll = 0;

		// Token: 0x020000C3 RID: 195
		private enum ThemeChangerState
		{
			// Token: 0x040004A4 RID: 1188
			Startup,
			// Token: 0x040004A5 RID: 1189
			List,
			// Token: 0x040004A6 RID: 1190
			LoadItem,
			// Token: 0x040004A7 RID: 1191
			Activating
		}
	}
}
