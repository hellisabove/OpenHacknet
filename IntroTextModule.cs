using System;
using System.IO;
using System.Linq;
using Hacknet.Extensions;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x02000127 RID: 295
	internal class IntroTextModule : Module
	{
		// Token: 0x060006F2 RID: 1778 RVA: 0x0007250C File Offset: 0x0007070C
		public IntroTextModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.bounds = location;
			this.timer = 0f;
			this.complete = false;
			this.textIndex = 0;
			this.finishedText = false;
			this.fullscreen = new Rectangle(0, 0, this.spriteBatch.GraphicsDevice.Viewport.Width, this.spriteBatch.GraphicsDevice.Viewport.Height);
			string text = this.os.multiplayer ? "Content/MultiplayerIntroText.txt" : (this.os.IsDLCConventionDemo ? "Content/DLC/Docs/DemoIntroText.txt" : "Content/BitSpeech.txt");
			if (Settings.IsInExtensionMode)
			{
				text = Path.Combine(ExtensionLoader.ActiveExtensionInfo.FolderPath, "Intro.txt");
				if (!File.Exists(text))
				{
					text = Path.Combine(ExtensionLoader.ActiveExtensionInfo.FolderPath, "intro.txt");
				}
			}
			if (File.Exists(text))
			{
				string text2 = LocalizedFileLoader.Read(text).Replace("\t", "    ");
				text2 = Utils.CleanFilterStringToRenderable(text2);
				this.text = text2.Split(IntroTextModule.delims, StringSplitOptions.RemoveEmptyEntries);
			}
			else
			{
				this.text = new string[]
				{
					"   "
				};
			}
		}

		// Token: 0x060006F3 RID: 1779 RVA: 0x00072660 File Offset: 0x00070860
		public override void Update(float t)
		{
			base.Update(t);
			double num = (double)this.timer;
			this.timer += t;
			float num2 = Settings.isDemoMode ? IntroTextModule.DEMO_DELAY_FROM_START_MUSIC_TIMER : IntroTextModule.DELAY_FROM_START_MUSIC_TIMER;
			if (Settings.IsInExtensionMode)
			{
				num2 = ExtensionLoader.ActiveExtensionInfo.IntroStartupSongDelay;
			}
			if (num < (double)num2 && this.timer >= num2)
			{
				MusicManager.playSong();
			}
			float num3 = 1f;
			if (LocaleActivator.ActiveLocaleIsCJK())
			{
				num3 = 0.6f;
			}
			if (this.finishedText)
			{
				if (this.timer > IntroTextModule.STAY_ONSCREEN_TIME)
				{
					if (this.timer > IntroTextModule.STAY_ONSCREEN_TIME + IntroTextModule.MODULE_FLASH_TIME)
					{
						this.complete = true;
					}
				}
			}
			else if (this.timer > IntroTextModule.FLASH_TIME)
			{
				this.charTimer += t * num3;
				if (this.charTimer >= IntroTextModule.CHAR_TIME)
				{
					this.charTimer = (Settings.isConventionDemo ? (IntroTextModule.CHAR_TIME * (GuiData.getKeyboadState().IsKeyDown(Keys.LeftShift) ? 0.99f : 0.5f)) : 0f);
					this.charIndex++;
					if (this.charIndex >= this.text[this.textIndex].Length)
					{
						this.charTimer = IntroTextModule.CHAR_TIME;
						this.charIndex = this.text[this.textIndex].Length - 1;
						this.lineTimer += t;
						if (this.lineTimer >= IntroTextModule.LINE_TIME)
						{
							this.lineTimer = (Settings.isConventionDemo ? (IntroTextModule.LINE_TIME * (GuiData.getKeyboadState().IsKeyDown(Keys.LeftShift) ? 0.99f : 0.2f)) : 0f);
							this.textIndex++;
							this.charIndex = 0;
							if (this.textIndex >= this.text.Length)
							{
								if (!MusicManager.isPlaying)
								{
									MusicManager.playSong();
								}
								this.finishedText = true;
								this.timer = 0f;
							}
						}
					}
				}
			}
			else if (Settings.isConventionDemo && GuiData.getKeyboadState().IsKeyDown(Keys.LeftShift))
			{
				this.timer += t + t;
			}
		}

		// Token: 0x060006F4 RID: 1780 RVA: 0x00072900 File Offset: 0x00070B00
		public override void Draw(float t)
		{
			base.Draw(t);
			this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
			if (Utils.random.NextDouble() < (double)(this.timer / IntroTextModule.FLASH_TIME) || this.timer > IntroTextModule.FLASH_TIME || this.finishedText)
			{
				this.os.drawBackground();
			}
			Color color = this.os.terminalTextColor * (this.finishedText ? (1f - this.timer / IntroTextModule.STAY_ONSCREEN_TIME) : 1f);
			Vector2 vector = new Vector2(120f, 100f);
			if (this.timer > IntroTextModule.FLASH_TIME || this.finishedText)
			{
				for (int i = 0; i < this.textIndex; i++)
				{
					string screensizeSplitVersionOfString = this.GetScreensizeSplitVersionOfString(this.text[i], vector);
					this.spriteBatch.DrawString(GuiData.smallfont, screensizeSplitVersionOfString, vector, color);
					vector.Y += 16f;
					if (screensizeSplitVersionOfString.Contains('\n'))
					{
						vector.Y += 16f;
					}
				}
				if (!this.finishedText)
				{
					string input = this.text[this.textIndex].Substring(0, this.charIndex + 1);
					string screensizeSplitVersionOfString = this.GetScreensizeSplitVersionOfString(input, vector);
					this.spriteBatch.DrawString(GuiData.smallfont, screensizeSplitVersionOfString, vector, color);
				}
			}
			if (this.finishedText && this.timer > IntroTextModule.STAY_ONSCREEN_TIME)
			{
				if (Utils.random.NextDouble() < (double)((this.timer - IntroTextModule.STAY_ONSCREEN_TIME) / IntroTextModule.MODULE_FLASH_TIME))
				{
					this.os.drawModules(this.os.lastGameTime);
				}
			}
			this.os.drawScanlines();
		}

		// Token: 0x060006F5 RID: 1781 RVA: 0x00072B18 File Offset: 0x00070D18
		private string GetScreensizeSplitVersionOfString(string input, Vector2 dpos)
		{
			int num = this.spriteBatch.GraphicsDevice.Viewport.Width - (int)dpos.X - 10;
			string result;
			if (GuiData.smallfont.MeasureString(input).X > (float)num)
			{
				result = Utils.SuperSmartTwimForWidth(input, num, GuiData.smallfont);
			}
			else
			{
				result = input;
			}
			return result;
		}

		// Token: 0x040007C7 RID: 1991
		private static float FLASH_TIME = 3.5f;

		// Token: 0x040007C8 RID: 1992
		private static float STAY_ONSCREEN_TIME = 3f;

		// Token: 0x040007C9 RID: 1993
		private static float MODULE_FLASH_TIME = 2f;

		// Token: 0x040007CA RID: 1994
		private static float CHAR_TIME = 0.048f;

		// Token: 0x040007CB RID: 1995
		private static float LINE_TIME = 0.455f;

		// Token: 0x040007CC RID: 1996
		private static float DELAY_FROM_START_MUSIC_TIMER = Settings.isConventionDemo ? 6.2f : 15.06f;

		// Token: 0x040007CD RID: 1997
		private static float DEMO_DELAY_FROM_START_MUSIC_TIMER = 5.18f;

		// Token: 0x040007CE RID: 1998
		private float timer;

		// Token: 0x040007CF RID: 1999
		private float charTimer;

		// Token: 0x040007D0 RID: 2000
		private float lineTimer;

		// Token: 0x040007D1 RID: 2001
		public bool complete;

		// Token: 0x040007D2 RID: 2002
		private bool finishedText;

		// Token: 0x040007D3 RID: 2003
		private string[] text;

		// Token: 0x040007D4 RID: 2004
		private int textIndex;

		// Token: 0x040007D5 RID: 2005
		private int charIndex = 0;

		// Token: 0x040007D6 RID: 2006
		private static string[] delims = new string[]
		{
			"\r\n"
		};

		// Token: 0x040007D7 RID: 2007
		private Rectangle fullscreen;
	}
}
