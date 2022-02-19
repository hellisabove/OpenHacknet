using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000A5 RID: 165
	internal class PointClickerDaemon : Daemon
	{
		// Token: 0x06000361 RID: 865 RVA: 0x000326EC File Offset: 0x000308EC
		public PointClickerDaemon(Computer computer, string serviceName, OS opSystem) : base(computer, serviceName, opSystem)
		{
			this.InitGameSettings();
			this.InitLogoSettings();
			this.InitRest();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x00032803 File Offset: 0x00030A03
		private void InitRest()
		{
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00032808 File Offset: 0x00030A08
		private void InitLogoSettings()
		{
			this.background1 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Background1");
			this.background2 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Background2");
			this.logoBase = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/BaseLogo");
			this.logoOverlay1 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/LogoOverlay1");
			this.logoOverlay2 = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/LogoOverlay2");
			this.logoStar = this.os.content.Load<Texture2D>("EffectFiles/PointClicker/Star");
			this.scanlinesTextBackground = this.os.content.Load<Texture2D>("EffectFiles/ScanlinesTextBackground");
			this.logoRenderBase = new RenderTarget2D(GuiData.spriteBatch.GraphicsDevice, 768, 384);
			this.logoBatch = new SpriteBatch(GuiData.spriteBatch.GraphicsDevice);
			for (int i = 0; i < 40; i++)
			{
				this.AddRandomLogoStar(true);
			}
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00032924 File Offset: 0x00030B24
		private void UpdateRate()
		{
			this.currentRate = 0f;
			for (int i = 0; i < this.upgradeNames.Count; i++)
			{
				this.currentRate += this.upgradeValues[i] * (float)this.activeState.upgradeCounts[i];
			}
		}

		// Token: 0x06000365 RID: 869 RVA: 0x00032988 File Offset: 0x00030B88
		private void UpdatePoints()
		{
			if (this.activeState != null)
			{
				if ((double)this.currentRate > 0.0 || (double)this.currentRate < -1.0)
				{
					double num = (double)this.currentRate * this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
					this.activeState.points += (long)((int)num);
					this.pointOverflow += (float)(num - (double)((int)num));
					if (this.pointOverflow > 1f)
					{
						int num2 = (int)this.pointOverflow;
						this.activeState.points += (long)num2;
						this.pointOverflow -= (float)num2;
					}
				}
				this.UpdateStory();
				if (this.ActiveStory == null)
				{
					this.ActiveStory = "";
				}
				if (this.activeState.points <= -1L)
				{
					AchievementsManager.Unlock("pointclicker_expert", true);
				}
				this.timeSinceLastSave += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				if (this.timeSinceLastSave > 10f)
				{
					this.SaveProgress();
				}
			}
		}

		// Token: 0x06000366 RID: 870 RVA: 0x00032AE8 File Offset: 0x00030CE8
		private void UpdateStory()
		{
			if (this.activeState.currentStoryElement < 0)
			{
				this.activeState.currentStoryElement = 0;
				this.ActiveStory = (this.storyBeats[this.activeState.currentStoryElement] ?? "");
			}
			else if (this.storyBeatChangers.Count > this.activeState.currentStoryElement + 1 && this.activeState.points >= this.storyBeatChangers[this.activeState.currentStoryElement + 1])
			{
				this.activeState.currentStoryElement++;
				this.ActiveStory = (this.storyBeats[this.activeState.currentStoryElement] ?? "");
			}
		}

		// Token: 0x06000367 RID: 871 RVA: 0x00032BC8 File Offset: 0x00030DC8
		private void PurchaseUpgrade(int index)
		{
			if (this.activeState != null && (float)this.activeState.points >= this.upgradeCosts[index])
			{
				List<int> upgradeCounts;
				(upgradeCounts = this.activeState.upgradeCounts)[index] = upgradeCounts[index] + 1;
				this.activeState.points -= (long)((int)this.upgradeCosts[index]);
				this.pointOverflow -= this.upgradeCosts[index] - (float)((int)this.upgradeCosts[index]);
				this.UpdateRate();
				this.UpgradeNotifiers.Add(new PointClickerDaemon.UpgradeNotifier
				{
					text = "+" + this.upgradeValues[index].ToString(),
					timer = 1f
				});
				this.SaveProgress();
				if (index >= this.upgradeNames.Count - 1)
				{
					AchievementsManager.Unlock("pointclicker_basic", true);
				}
			}
		}

		// Token: 0x06000368 RID: 872 RVA: 0x00032CE4 File Offset: 0x00030EE4
		private void SaveProgress()
		{
			for (int i = 0; i < this.savesFolder.files.Count; i++)
			{
				if (this.savesFolder.files[i].name == this.userFilePath)
				{
					this.savesFolder.files[i].data = this.activeState.ToSaveString();
					this.timeSinceLastSave = 0f;
					break;
				}
			}
		}

		// Token: 0x06000369 RID: 873 RVA: 0x00032D6C File Offset: 0x00030F6C
		private void AddRandomLogoStar(bool randomStartLife = false)
		{
			PointClickerDaemon.PointClickerStar item = new PointClickerDaemon.PointClickerStar
			{
				Pos = new Vector2(Utils.randm(1f), Utils.randm(1f)),
				life = (randomStartLife ? Utils.randm(1f) : 1f),
				rot = Utils.randm(6.48f),
				scale = 0.2f + Utils.rand(1.3f),
				timescale = 0.3f + Utils.randm(1.35f)
			};
			item.color = Utils.AddativeWhite;
			float num = Utils.randm(1f);
			int maxValue = 80;
			if (num < 0.3f)
			{
				item.color.R = (byte)(255 - Utils.random.Next(maxValue));
			}
			else if (num < 0.6f)
			{
				item.color.G = (byte)(255 - Utils.random.Next(maxValue));
			}
			else if (num < 0.9f)
			{
				item.color.B = (byte)(255 - Utils.random.Next(maxValue));
			}
			this.Stars.Add(item);
		}

		// Token: 0x0600036A RID: 874 RVA: 0x00032EBC File Offset: 0x000310BC
		private void InitGameSettings()
		{
			this.upgradeNames.Add(LocaleTerms.Loc("Click Me!"));
			this.upgradeNames.Add(LocaleTerms.Loc("Autoclicker v1"));
			this.upgradeNames.Add(LocaleTerms.Loc("Autoclicker v2"));
			this.upgradeNames.Add(LocaleTerms.Loc("Pointereiellion"));
			this.upgradeValues.Add(0.04f);
			this.upgradeValues.Add(1f);
			this.upgradeValues.Add(10f);
			this.upgradeValues.Add(200f);
			this.storyBeats.Add("Your glorious ClickPoints empire begins");
			this.storyBeats.Add("The hard days of manual button clicking labour seem endless, but a better future is in sight.");
			this.storyBeats.Add("The investment is returned - you finally turn a profit.");
			this.storyBeats.Add("Your long days of labour to gather the initial 12 points are a fast-fading memory.");
			this.storyBeats.Add("You reach international acclaim as a prominent and incredibly wealthy point collector.");
			this.storyBeats.Add("Your enormous pile of points is now larger than Everest");
			this.storyBeats.Add("The ClickPoints continent is declared : a landmass made entirely of your insane wealth.");
			this.storyBeatChangers.Add(0L);
			this.storyBeatChangers.Add(5L);
			this.storyBeatChangers.Add(15L);
			this.storyBeatChangers.Add(200L);
			this.storyBeatChangers.Add(100000L);
			this.storyBeatChangers.Add(1000000000000L);
			this.storyBeatChangers.Add(11111000000000000L);
			for (int i = 3; i < 50; i++)
			{
				this.upgradeNames.Add(LocaleTerms.Loc("Upgrade") + " " + (i + 1));
				this.upgradeValues.Add((float)Math.Max((double)(i * i * i * i * i), 0.01));
			}
			for (int j = 0; j < this.upgradeValues.Count; j++)
			{
				this.upgradeCosts.Add(this.upgradeValues[j] * (float)(1 + j / 50 * 5) * this.UpgradeCostMultiplier * (1f + this.UpgradeCostMultiplier * ((float)((j + 1) / this.upgradeValues.Count) * 5f)));
			}
			this.upgradeCosts[0] = 0f;
			this.upgradeCosts[1] = 12f;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x00033144 File Offset: 0x00031344
		public override void initFiles()
		{
			base.initFiles();
			this.rootFolder = new Folder("PointClicker");
			this.savesFolder = new Folder("Saves");
			int num = 50;
			int num2 = 0;
			int i = 0;
			while (i < num)
			{
				string text;
				do
				{
					text = People.all[i + num2].handle;
					if (text == null)
					{
						num2++;
					}
					if (i + num2 >= People.all.Count)
					{
						break;
					}
				}
				while (text == null);
				IL_80:
				if (i == 22)
				{
					text = "Mengsk";
				}
				if (i == 28)
				{
					text = "Bit";
				}
				if (text != null)
				{
					this.AddSaveForName(text, i == 22 || i == 28);
				}
				i++;
				continue;
				goto IL_80;
			}
			this.rootFolder.folders.Add(this.savesFolder);
			this.comp.files.root.folders.Add(this.rootFolder);
			FileEntry item = new FileEntry(Computer.generateBinaryString(1000), "config.ini");
			this.rootFolder.files.Add(item);
			FileEntry item2 = new FileEntry(string.Concat(new string[]
			{
				LocaleTerms.Loc("IMPORTANT : NEVER DELETE OR RE-NAME"),
				" \"config.ini\"\n ",
				LocaleTerms.Loc("IT IS SYSTEM CRITICAL!"),
				" ",
				LocaleTerms.Loc("Removing it causes instant crash. DO NOT TEST THIS")
			}), "IMPORTANT_README_DONT_CRASH.txt");
			this.rootFolder.files.Add(item2);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x000332F6 File Offset: 0x000314F6
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.state = PointClickerDaemon.PointClickerScreenState.Welcome;
			this.pointOverflow = 0f;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x00033312 File Offset: 0x00031512
		public override void loadInit()
		{
			base.loadInit();
			this.rootFolder = this.comp.files.root.searchForFolder("PointClicker");
			this.savesFolder = this.rootFolder.searchForFolder("Saves");
		}

		// Token: 0x0600036E RID: 878 RVA: 0x00033354 File Offset: 0x00031554
		private void AddSaveForName(string name, bool isSuperHighScore = false)
		{
			PointClickerDaemon.PointClickerGameState pointClickerGameState = new PointClickerDaemon.PointClickerGameState(this.upgradeValues.Count);
			for (int i = 0; i < pointClickerGameState.upgradeCounts.Count; i++)
			{
				pointClickerGameState.upgradeCounts[i] = (int)(10f * (Utils.randm(1f) * ((float)i / (float)pointClickerGameState.upgradeCounts.Count)));
				if (isSuperHighScore)
				{
					pointClickerGameState.upgradeCounts[i] = 900 + (int)(Utils.randm(1f) * 99.9f);
				}
			}
			pointClickerGameState.points = (long)Utils.random.Next();
			pointClickerGameState.currentStoryElement = Utils.random.Next(this.storyBeats.Count);
			FileEntry item = new FileEntry(pointClickerGameState.ToSaveString(), name + ".pcsav");
			this.savesFolder.files.Add(item);
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00033440 File Offset: 0x00031640
		public override string getSaveString()
		{
			return "<PointClicker />";
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00033458 File Offset: 0x00031658
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			switch (this.state)
			{
			default:
				this.DrawWelcome(bounds, sb);
				break;
			case PointClickerDaemon.PointClickerScreenState.Error:
				break;
			case PointClickerDaemon.PointClickerScreenState.Main:
				this.UpdatePoints();
				this.DrawMainScreen(bounds, sb);
				break;
			}
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x000334B4 File Offset: 0x000316B4
		private void DrawLogo(Rectangle dest, SpriteBatch sb)
		{
			float num = (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			for (int i = 0; i < this.Stars.Count; i++)
			{
				PointClickerDaemon.PointClickerStar value = this.Stars[i];
				value.life -= num * 2f * value.timescale;
				if (this.Stars[i].life <= 0f)
				{
					this.Stars.RemoveAt(i);
					i--;
					this.AddRandomLogoStar(false);
				}
				else
				{
					this.Stars[i] = value;
				}
			}
			RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
			sb.GraphicsDevice.SetRenderTarget(this.logoRenderBase);
			sb.GraphicsDevice.Clear(Color.Transparent);
			this.logoBatch.Begin();
			float num2 = (float)(Math.Sin((double)(this.os.timer / 2.2f)) + 1.0) / 2f;
			this.logoBatch.Draw(this.background1, Vector2.Zero, Utils.AddativeWhite * num2);
			this.logoBatch.Draw(this.background2, Vector2.Zero, Utils.AddativeWhite * (1f - num2));
			Rectangle dest2 = new Rectangle(0, 0, this.logoBase.Width, this.logoBase.Height);
			this.logoBatch.Draw(this.logoBase, Vector2.Zero, Color.White);
			FlickeringTextEffect.DrawFlickeringSprite(this.logoBatch, dest2, this.logoBase, 4f, 0.25f, this.os, Color.White);
			float num3 = 0.44f + (float)(Math.Sin((double)(this.os.timer * 0.823f)) + 1.0) / 2f;
			this.logoBatch.Draw(this.logoOverlay1, Vector2.Zero, Utils.AddativeWhite * num3);
			this.logoBatch.Draw(this.logoOverlay2, Vector2.Zero, Utils.AddativeWhite * (1f - num3));
			this.logoBatch.End();
			sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
			for (int i = 0; i < this.Stars.Count; i++)
			{
				this.DrawStar(dest, sb, this.Stars[i]);
			}
			FlickeringTextEffect.DrawFlickeringSpriteAltWeightings(sb, dest, this.logoRenderBase, 4f, 0.01f, this.os, Utils.AddativeWhite);
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0003376C File Offset: 0x0003196C
		private void DrawStar(Rectangle logoDest, SpriteBatch sb, PointClickerDaemon.PointClickerStar star)
		{
			Vector2 position = new Vector2(star.Pos.X * (float)logoDest.Width * 0.5f + (float)logoDest.X, star.Pos.Y * (float)logoDest.Height * 0.5f + (float)logoDest.Y);
			position.X += (float)logoDest.Width * 0.25f;
			position.Y += (float)logoDest.Height * 0.25f;
			float num;
			if (star.life < 0.9f)
			{
				num = (star.life - 0.1f) / 0.9f;
			}
			else
			{
				num = 1f - (star.life - 0.9f) / 0.1f;
			}
			float num2 = Vector2.Distance(star.Pos, new Vector2(0.5f));
			float num3 = 0.9f;
			if (num2 > num3)
			{
				num = (1f - (num2 - num3) / 1f) * num;
			}
			sb.Draw(this.logoStar, position, null, star.color * num, star.rot * (star.life * 0.5f), new Vector2((float)(this.logoStar.Width / 2), (float)(this.logoStar.Height / 2)), star.scale * num, SpriteEffects.None, 0.4f);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x000338F8 File Offset: 0x00031AF8
		private void DrawMainScreen(Rectangle bounds, SpriteBatch sb)
		{
			string points = this.activeState.points.ToString();
			Rectangle rectangle = new Rectangle(bounds.X + 1, bounds.Y + 10, bounds.Width - 2, 100);
			this.DrawMonospaceString(rectangle, sb, points);
			rectangle.Y -= 4;
			sb.Draw(this.scanlinesTextBackground, rectangle, this.ThemeColor * 0.2f);
			for (int i = 0; i < this.Stars.Count; i++)
			{
				this.DrawStar(rectangle, sb, this.Stars[i]);
			}
			Rectangle bounds2 = new Rectangle(bounds.X + 2, rectangle.Y + rectangle.Height + 12, bounds.Width / 2 - 4, bounds.Height - 12 - (rectangle.Y + rectangle.Height - bounds.Y));
			this.DrawUpgrades(bounds2, sb);
			float num = (float)this.logoRenderBase.Height / (float)this.logoRenderBase.Width;
			int num2 = 45;
			int num3 = bounds.Width / 2 + num2;
			int num4 = (int)((float)num3 * num);
			Rectangle bounds3 = new Rectangle(bounds.X + bounds2.Width + 4, bounds2.Y, bounds.Width - bounds2.Width - 8, bounds2.Height - num4 + 30);
			this.DrawHoverTooltip(bounds3, sb);
			Rectangle rectangle2 = new Rectangle(bounds.X + bounds.Width - num3 + 42, bounds.Y + bounds.Height - num4 + 10, num3 - 50, 50);
			string text = Utils.SuperSmartTwimForWidth(this.ActiveStory, rectangle2.Width, GuiData.smallfont);
			TextItem.doFontLabel(new Vector2((float)rectangle2.X, (float)rectangle2.Y), text, GuiData.smallfont, new Color?(this.ThemeColor), (float)rectangle2.Width, (float)rectangle2.Height, false);
			Rectangle dest = new Rectangle(bounds.X + bounds.Width - num3 + 35, bounds.Y + bounds.Height - num4 + 20, num3, num4);
			this.DrawLogo(dest, sb);
			if (Button.doButton(3032113, bounds.X + bounds.Width - 22, bounds.Y + bounds.Height - 22, 20, 20, "X", new Color?(this.os.lockedColor)))
			{
				this.state = PointClickerDaemon.PointClickerScreenState.Welcome;
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00033BA8 File Offset: 0x00031DA8
		private void DrawHoverTooltip(Rectangle bounds, SpriteBatch sb)
		{
			float num = 80f;
			int num2 = bounds.Height - 80;
			Rectangle rectangle;
			if (this.hoverIndex > -1 && this.hoverIndex < this.upgradeNames.Count && this.activeState != null)
			{
				Rectangle bounds2 = bounds;
				bounds2.Height = num2;
				bool flag = this.upgradeCosts[this.hoverIndex] <= (float)this.activeState.points;
				float num3 = 20f;
				FancyOutlines.DrawCornerCutOutline(bounds2, sb, num3, this.ThemeColor);
				Rectangle dest = new Rectangle((int)((float)bounds.X + num3 + 4f), bounds.Y + 3, (int)((float)bounds.Width - (num3 + 4f) * 2f), 40);
				TextItem.doFontLabelToSize(dest, this.upgradeNames[this.hoverIndex], GuiData.font, this.ThemeColorHighlight, false, false);
				rectangle = new Rectangle(bounds.X + 2, bounds.Y + dest.Height + 4, bounds.Width - 4, 20);
				string text = flag ? ((Settings.ActiveLocale != "en-us") ? LocaleTerms.Loc("UPGRADE AVALIABLE") : "UPGRADE AVAILABLE") : LocaleTerms.Loc("INSUFFICIENT POINTS");
				TextItem.doFontLabelToSize(rectangle, text, GuiData.font, flag ? (this.ThemeColorHighlight * 0.8f) : Color.Gray, false, false);
				rectangle.Y += rectangle.Height;
				rectangle.Height = 50;
				rectangle.X += 4;
				rectangle.Width -= 4;
				float num4 = (this.activeState.points == 0L) ? 1f : (this.upgradeCosts[this.hoverIndex] / (float)this.activeState.points);
				if (float.IsNaN(num4))
				{
					num4 = 100f;
				}
				else
				{
					num4 *= 100f;
				}
				this.DrawStatsTextBlock(LocaleTerms.Loc("COST"), string.Concat(this.upgradeCosts[this.hoverIndex]), num4.ToString("00.0") + LocaleTerms.Loc("% of current Points"), rectangle, sb, num);
				rectangle.Y += rectangle.Height;
				float num5 = (this.currentRate <= 0f) ? 100f : (this.upgradeValues[this.hoverIndex] / this.currentRate * 100f);
				this.DrawStatsTextBlock("+PPS", string.Concat(this.upgradeValues[this.hoverIndex]), num5.ToString("00.0") + LocaleTerms.Loc("% of current Points Per Second"), rectangle, sb, num);
				Rectangle rectangle2 = new Rectangle((int)((float)bounds.X + num3 + 4f), rectangle.Y + rectangle.Height + 4, (int)((float)bounds.Width - (num3 + 4f) * 2f), 50);
				if (flag)
				{
					sb.Draw(this.scanlinesTextBackground, rectangle2, Utils.makeColorAddative(this.ThemeColorHighlight) * 0.6f);
					FlickeringTextEffect.DrawFlickeringText(rectangle2, "CLICK TO UPGRADE", 3f, 0.1f, GuiData.titlefont, this.os, Utils.AddativeWhite);
				}
				else
				{
					sb.Draw(this.scanlinesTextBackground, rectangle2, Color.Lerp(this.os.brightLockedColor, Utils.makeColorAddative(Color.Red), 0.2f + Utils.randm(0.8f)) * 0.4f);
					FlickeringTextEffect.DrawFlickeringText(rectangle2, "INSUFFICIENT POINTS", 3f, 0.1f, GuiData.titlefont, this.os, Utils.AddativeWhite);
				}
			}
			rectangle = new Rectangle(bounds.X + 2, bounds.Y + num2 + 4, bounds.Width - 4, 50);
			float f = (this.currentRate <= 0f) ? 0f : ((float)this.activeState.points / this.currentRate);
			if (float.IsNaN(f))
			{
				f = float.PositiveInfinity;
			}
			this.DrawStatsTextBlock("PPS", this.currentRate.ToString("000.0") ?? "", f.ToString("00.0") + " " + LocaleTerms.Loc("seconds to double current points"), rectangle, sb, num);
			float num6 = (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			for (int i = 0; i < this.UpgradeNotifiers.Count; i++)
			{
				PointClickerDaemon.UpgradeNotifier value = this.UpgradeNotifiers[i];
				value.timer -= num6 * 4f;
				if (value.timer <= 0f)
				{
					this.UpgradeNotifiers.RemoveAt(i);
					i--;
				}
				else
				{
					Vector2 value2 = GuiData.font.MeasureString(value.text);
					sb.DrawString(GuiData.font, value.text, new Vector2((float)rectangle.X + ((float)rectangle.Width - num) / 2f + num, (float)(rectangle.Y + 10)), this.ThemeColorHighlight * value.timer, 0f, value2 / 2f, 0.5f + (1f - value.timer) * 2.2f, SpriteEffects.None, 0.9f);
					this.UpgradeNotifiers[i] = value;
				}
			}
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00034198 File Offset: 0x00032398
		private void DrawStatsTextBlock(string anouncer, string main, string secondary, Rectangle bounds, SpriteBatch sb, float announcerWidth)
		{
			Vector2 pos = new Vector2((float)bounds.X, (float)bounds.Y);
			TextItem.doFontLabel(pos, anouncer, GuiData.font, new Color?(Utils.AddativeWhite), announcerWidth - 10f, (float)bounds.Height - 8f, true);
			pos.X += announcerWidth + 2f;
			TextItem.doFontLabel(new Vector2((float)bounds.X + announcerWidth - 12f, (float)bounds.Y), ":", GuiData.font, new Color?(Utils.AddativeWhite), 22f, (float)bounds.Height, false);
			TextItem.doFontLabel(pos, main, GuiData.font, new Color?(this.ThemeColorHighlight), (float)bounds.Width - announcerWidth, (float)bounds.Height - 8f, true);
			pos.Y += 29f;
			pos.X = (float)bounds.X;
			TextItem.doFontLabel(pos, "[" + secondary + "]", GuiData.smallfont, new Color?(Color.Gray), (float)bounds.Width, (float)bounds.Height, false);
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00034828 File Offset: 0x00032A28
		private void DrawUpgrades(Rectangle bounds, SpriteBatch sb)
		{
			int panelHeight = 28;
			if (this.scrollerPanel == null)
			{
				this.scrollerPanel = new ScrollableSectionedPanel(panelHeight, sb.GraphicsDevice);
			}
			this.scrollerPanel.NumberOfPanels = this.upgradeNames.Count;
			Button.outlineOnly = true;
			Button.drawingOutline = false;
			int drawnThisCycle = 0;
			bool needsButtonChecks = bounds.Contains(GuiData.getMousePoint());
			this.scrollerPanel.Draw(delegate(int index, Rectangle drawAreaFull, SpriteBatch sprBatch)
			{
				Rectangle destinationRectangle = new Rectangle(drawAreaFull.X, drawAreaFull.Y, drawAreaFull.Width - 12, drawAreaFull.Height);
				int num = 115700 + index * 111;
				if (needsButtonChecks && Button.doButton(num, destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height, "", new Color?(Color.Transparent)))
				{
					this.PurchaseUpgrade(index);
				}
				else if (!needsButtonChecks && GuiData.hot == num)
				{
					GuiData.hot = -1;
				}
				bool flag = this.upgradeCosts[index] <= (float)this.activeState.points;
				bool flag2 = flag && GuiData.hot == num;
				if (GuiData.hot == num)
				{
					this.hoverIndex = index;
				}
				if (flag2)
				{
					int height = destinationRectangle.Height;
					int num2 = 0;
					int num3 = 0;
					if (drawAreaFull.X == 0 && drawAreaFull.Y == 0)
					{
						if (drawnThisCycle == 0)
						{
							num2 = bounds.X;
							num3 = bounds.Y;
						}
						else
						{
							num2 = bounds.X;
							num3 = bounds.Y + bounds.Height - panelHeight / 2;
						}
					}
					Rectangle rectangle = new Rectangle(num2 + destinationRectangle.X - height, num3 + destinationRectangle.Y - height, destinationRectangle.Width + 2 * height, destinationRectangle.Height + 2 * height);
					for (int i = 0; i < this.Stars.Count; i++)
					{
						this.DrawStar(rectangle, sb, this.Stars[i]);
					}
					sb.Draw(this.scanlinesTextBackground, rectangle, this.ThemeColor * ((GuiData.active == num) ? 0.6f : 0.3f));
				}
				sprBatch.Draw(this.scanlinesTextBackground, destinationRectangle, new Rectangle?(new Rectangle(this.scanlinesTextBackground.Width / 2, this.scanlinesTextBackground.Height / 9 * 4, this.scanlinesTextBackground.Width / 2, this.scanlinesTextBackground.Height / 4)), flag ? (this.ThemeColor * 0.2f) : (Utils.AddativeWhite * 0.08f));
				if (GuiData.hot == num)
				{
					RenderedRectangle.doRectangle(destinationRectangle.X + 1, destinationRectangle.Y + 1, destinationRectangle.Width - 2, destinationRectangle.Height - 2, new Color?(flag2 ? ((GuiData.active == num) ? Color.Black : this.ThemeColorBacking) : Color.Black));
				}
				if (index == 0)
				{
					Utils.drawLine(sprBatch, new Vector2((float)(destinationRectangle.X + 1), (float)(destinationRectangle.Y + 1)), new Vector2((float)(destinationRectangle.X + destinationRectangle.Width - 2), (float)(destinationRectangle.Y + 1)), Vector2.Zero, this.ThemeColor, 0.8f);
				}
				Utils.drawLine(sprBatch, new Vector2((float)(destinationRectangle.X + 1), (float)(destinationRectangle.Y + destinationRectangle.Height - 2)), new Vector2((float)(destinationRectangle.X + destinationRectangle.Width - 2), (float)(destinationRectangle.Y + destinationRectangle.Height - 2)), Vector2.Zero, this.ThemeColor, 0.8f);
				if (flag)
				{
					sprBatch.Draw(Utils.white, new Rectangle(destinationRectangle.X, destinationRectangle.Y + 1, 8, destinationRectangle.Height - 2), this.ThemeColor);
				}
				string text = "[" + this.activeState.upgradeCounts[index].ToString("000") + "] " + this.upgradeNames[index];
				TextItem.doFontLabel(new Vector2((float)(destinationRectangle.X + 4 + (flag ? 10 : 0)), (float)(destinationRectangle.Y + 4)), text, GuiData.UISmallfont, new Color?(flag2 ? ((GuiData.active == num) ? this.ThemeColor : (flag ? Color.White : Color.Gray)) : (flag ? Utils.AddativeWhite : Color.Gray)), (float)(destinationRectangle.Width - 6), float.MaxValue, false);
				drawnThisCycle++;
			}, sb, bounds);
			Button.outlineOnly = false;
			Button.drawingOutline = true;
		}

		// Token: 0x06000377 RID: 887 RVA: 0x000348F8 File Offset: 0x00032AF8
		private void DrawMonospaceString(Rectangle bounds, SpriteBatch sb, string points)
		{
			points = points.Trim();
			float num = 65f;
			float num2 = ((float)points.Length + 1f) * num;
			float num3 = 1f;
			if (num2 > (float)bounds.Width)
			{
				num3 = (float)bounds.Width / num2;
			}
			float num4 = num2 * num3;
			float num5 = ((float)bounds.Width - num4) / 2f;
			Vector2 value = new Vector2((float)bounds.X + num5, (float)bounds.Y);
			for (int i = 0; i < points.Length; i++)
			{
				string text = string.Concat(points[i]);
				Vector2 vector = GuiData.titlefont.MeasureString(text);
				float x = num * num3 - vector.X * num3 / 2f;
				sb.DrawString(GuiData.titlefont, text, value + new Vector2(x, 0f), Color.White, 0f, Vector2.Zero, num3, SpriteEffects.None, 0.67f);
				value.X += num * num3;
			}
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00034A20 File Offset: 0x00032C20
		private void DrawWelcome(Rectangle bounds, SpriteBatch sb)
		{
			float num = (float)this.logoRenderBase.Height / (float)this.logoRenderBase.Width;
			int num2 = 45;
			Rectangle dest = new Rectangle(bounds.X - num2 + 20, bounds.Y, bounds.Width + num2, (int)((float)(bounds.Width + 2 * num2) * num));
			this.DrawLogo(dest, sb);
			Rectangle rectangle = new Rectangle(bounds.X, dest.Y + dest.Height, bounds.Width, 60);
			sb.Draw(this.scanlinesTextBackground, rectangle, Utils.AddativeWhite * 0.2f);
			for (int i = 0; i < this.Stars.Count; i++)
			{
				this.DrawStar(rectangle, sb, this.Stars[i]);
			}
			rectangle.X += 100;
			rectangle.Width = bounds.Width - 200;
			rectangle.Y += 13;
			rectangle.Height = 35;
			if (Button.doButton(98373721, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, "GO!", new Color?(Utils.AddativeWhite)))
			{
				this.activeState = null;
				string text = this.os.defaultUser.name;
				text = text.Replace(" ", "_");
				for (int i = 0; i < this.savesFolder.files.Count; i++)
				{
					if (this.savesFolder.files[i].name.StartsWith(text))
					{
						this.userFilePath = this.savesFolder.files[i].name;
						this.activeState = PointClickerDaemon.PointClickerGameState.LoadFromString(this.savesFolder.files[i].data);
						break;
					}
				}
				if (this.activeState == null)
				{
					this.activeState = new PointClickerDaemon.PointClickerGameState(this.upgradeNames.Count);
					FileEntry fileEntry = new FileEntry(this.activeState.ToSaveString(), text + ".pcsav");
					this.savesFolder.files.Add(fileEntry);
					this.userFilePath = fileEntry.name;
				}
				this.state = PointClickerDaemon.PointClickerScreenState.Main;
				this.currentRate = 0f;
				this.ActiveStory = "";
				this.UpdateRate();
				this.UpdateStory();
				this.UpdatePoints();
			}
			if (Button.doButton(98373732, bounds.X + 2, bounds.Y + bounds.Height - 19, 180, 18, LocaleTerms.Loc("Exit") + "  :<", new Color?(this.os.lockedColor)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x040003C9 RID: 969
		private float UpgradeCostMultiplier = 13f;

		// Token: 0x040003CA RID: 970
		private List<string> upgradeNames = new List<string>();

		// Token: 0x040003CB RID: 971
		private List<float> upgradeValues = new List<float>();

		// Token: 0x040003CC RID: 972
		private List<float> upgradeCosts = new List<float>();

		// Token: 0x040003CD RID: 973
		private List<string> storyBeats = new List<string>();

		// Token: 0x040003CE RID: 974
		private List<long> storyBeatChangers = new List<long>();

		// Token: 0x040003CF RID: 975
		private PointClickerDaemon.PointClickerScreenState state = PointClickerDaemon.PointClickerScreenState.Welcome;

		// Token: 0x040003D0 RID: 976
		private PointClickerDaemon.PointClickerGameState activeState = null;

		// Token: 0x040003D1 RID: 977
		private Folder savesFolder;

		// Token: 0x040003D2 RID: 978
		private Folder rootFolder;

		// Token: 0x040003D3 RID: 979
		private float currentRate = 0f;

		// Token: 0x040003D4 RID: 980
		private float pointOverflow = 0f;

		// Token: 0x040003D5 RID: 981
		private Texture2D background1;

		// Token: 0x040003D6 RID: 982
		private Texture2D background2;

		// Token: 0x040003D7 RID: 983
		private Texture2D logoBase;

		// Token: 0x040003D8 RID: 984
		private Texture2D logoOverlay1;

		// Token: 0x040003D9 RID: 985
		private Texture2D logoOverlay2;

		// Token: 0x040003DA RID: 986
		private Texture2D logoStar;

		// Token: 0x040003DB RID: 987
		private Texture2D scanlinesTextBackground;

		// Token: 0x040003DC RID: 988
		private RenderTarget2D logoRenderBase;

		// Token: 0x040003DD RID: 989
		private SpriteBatch logoBatch;

		// Token: 0x040003DE RID: 990
		private List<PointClickerDaemon.PointClickerStar> Stars = new List<PointClickerDaemon.PointClickerStar>();

		// Token: 0x040003DF RID: 991
		private Color ThemeColor = new Color(133, 239, 255, 0);

		// Token: 0x040003E0 RID: 992
		private Color ThemeColorBacking = new Color(13, 59, 74, 250);

		// Token: 0x040003E1 RID: 993
		private Color ThemeColorHighlight = new Color(227, 0, 121, 200);

		// Token: 0x040003E2 RID: 994
		private ScrollableSectionedPanel scrollerPanel;

		// Token: 0x040003E3 RID: 995
		private int hoverIndex = 0;

		// Token: 0x040003E4 RID: 996
		private string ActiveStory = "";

		// Token: 0x040003E5 RID: 997
		private string userFilePath = null;

		// Token: 0x040003E6 RID: 998
		private float timeSinceLastSave = 0f;

		// Token: 0x040003E7 RID: 999
		private List<PointClickerDaemon.UpgradeNotifier> UpgradeNotifiers = new List<PointClickerDaemon.UpgradeNotifier>();

		// Token: 0x020000A6 RID: 166
		private struct PointClickerStar
		{
			// Token: 0x040003E8 RID: 1000
			public Vector2 Pos;

			// Token: 0x040003E9 RID: 1001
			public float scale;

			// Token: 0x040003EA RID: 1002
			public float life;

			// Token: 0x040003EB RID: 1003
			public float rot;

			// Token: 0x040003EC RID: 1004
			public float timescale;

			// Token: 0x040003ED RID: 1005
			public Color color;
		}

		// Token: 0x020000A7 RID: 167
		private struct UpgradeNotifier
		{
			// Token: 0x040003EE RID: 1006
			public string text;

			// Token: 0x040003EF RID: 1007
			public float timer;
		}

		// Token: 0x020000A8 RID: 168
		private class PointClickerGameState
		{
			// Token: 0x06000379 RID: 889 RVA: 0x00034D4C File Offset: 0x00032F4C
			public PointClickerGameState(int upgradesTotal)
			{
				this.currentStoryElement = -1;
				this.points = 0L;
				this.upgradeCounts = new List<int>();
				for (int i = 0; i < upgradesTotal; i++)
				{
					this.upgradeCounts.Add(0);
				}
			}

			// Token: 0x0600037A RID: 890 RVA: 0x00034D9C File Offset: 0x00032F9C
			public string ToSaveString()
			{
				string text = string.Concat(new object[]
				{
					this.points,
					"\n",
					this.currentStoryElement,
					"\n"
				});
				for (int i = 0; i < this.upgradeCounts.Count; i++)
				{
					text = text + this.upgradeCounts[i] + ",";
				}
				return text;
			}

			// Token: 0x0600037B RID: 891 RVA: 0x00034E24 File Offset: 0x00033024
			public static PointClickerDaemon.PointClickerGameState LoadFromString(string save)
			{
				PointClickerDaemon.PointClickerGameState pointClickerGameState = new PointClickerDaemon.PointClickerGameState(0);
				string[] array = save.Split(Utils.newlineDelim);
				pointClickerGameState.points = Convert.ToInt64(array[0]);
				pointClickerGameState.currentStoryElement = Convert.ToInt32(array[1]);
				string[] array2 = array[2].Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array2.Length; i++)
				{
					pointClickerGameState.upgradeCounts.Add(Convert.ToInt32(array2[i]));
				}
				return pointClickerGameState;
			}

			// Token: 0x040003F0 RID: 1008
			public int currentStoryElement;

			// Token: 0x040003F1 RID: 1009
			public long points;

			// Token: 0x040003F2 RID: 1010
			public List<int> upgradeCounts;
		}

		// Token: 0x020000A9 RID: 169
		private enum PointClickerScreenState
		{
			// Token: 0x040003F4 RID: 1012
			Welcome,
			// Token: 0x040003F5 RID: 1013
			Error,
			// Token: 0x040003F6 RID: 1014
			Main
		}
	}
}
