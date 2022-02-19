using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000104 RID: 260
	internal class CrashModule : Module
	{
		// Token: 0x060005F6 RID: 1526 RVA: 0x0006191C File Offset: 0x0005FB1C
		public CrashModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.bounds = location;
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x000619E8 File Offset: 0x0005FBE8
		public override void LoadContent()
		{
			base.LoadContent();
			this.bsodFont = this.os.ScreenManager.Game.Content.Load<SpriteFont>("BSODFont");
			CrashModule.beep = this.os.content.Load<SoundEffect>("SFX/beep");
			char[] array = new char[]
			{
				'\n'
			};
			StreamReader streamReader = new StreamReader(TitleContainer.OpenStream("Content/BSOD.txt"));
			this.bsodText = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader = new StreamReader(TitleContainer.OpenStream("Content/OSXBoot.txt"));
			this.originalBootText = streamReader.ReadToEnd();
			streamReader.Close();
			this.loadBootText();
			this.bootTextDelay = CrashModule.BOOT_TIME / ((float)(this.bootText.Length - 1) * 2f);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00061AB8 File Offset: 0x0005FCB8
		private void loadBootText()
		{
			char[] separator = new char[]
			{
				'\n'
			};
			this.bootText = this.checkOSBootFiles(this.originalBootText).Split(separator);
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x00061B0C File Offset: 0x0005FD0C
		public override void Update(float t)
		{
			base.Update(t);
			this.elapsedTime += t;
			if (this.IsInHostileFileCrash)
			{
				if (this.elapsedTime % 0.5f < 0.033333335f)
				{
					this.extraErrors++;
				}
				if (this.elapsedTime >= 15f)
				{
					if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
					{
						this.os.BootAssitanceModule.IsActive = true;
					}
					this.reset();
					this.os.canRunContent = false;
					this.os.bootingUp = false;
				}
			}
			else if (this.elapsedTime < CrashModule.BLUESCREEN_TIME / 4f)
			{
				this.state = 0;
			}
			else if (this.elapsedTime < CrashModule.BLUESCREEN_TIME)
			{
				this.state = 1;
			}
			else if (this.elapsedTime < CrashModule.BLUESCREEN_TIME + CrashModule.BLACK_TIME)
			{
				this.state = 2;
			}
			else if (this.elapsedTime < CrashModule.BLUESCREEN_TIME + CrashModule.BLACK_TIME + CrashModule.BOOT_TIME + this.bootTextErrorDelay)
			{
				this.state = 3;
				this.bootTextTimer -= t;
				if (this.bootTextTimer <= 0f)
				{
					float num = this.bootTextDelay;
					float num2 = (float)Utils.random.NextDouble() * this.bootTextDelay;
					num -= num2;
					num2 = (float)Utils.random.NextDouble() * this.bootTextDelay;
					num += num2;
					this.bootTextTimer = num;
					this.bootTextCount++;
					if (this.bootTextCount >= this.bootText.Length - 1)
					{
						this.bootTextCount = this.bootText.Length - 1;
					}
					if (this.bootText[this.bootTextCount].Equals(" "))
					{
						this.bootTextTimer = this.bootTextDelay * 12f;
					}
					if (this.bootText[this.bootTextCount].StartsWith("ERROR:"))
					{
						this.bootTextTimer = this.bootTextDelay * 29f;
						this.os.thisComputer.bootupTick(-(this.bootTextDelay * 42f));
						this.bootTextErrorDelay += this.bootTextDelay * 42f;
					}
					if (this.bootTextCount == 50)
					{
						if (HostileHackerBreakinSequence.IsInBlockingHostileFileState(this.os))
						{
							this.bootTextTimer = 999999f;
							this.os.thisComputer.bootTimer = 9999f;
							this.IsInHostileFileCrash = true;
							this.elapsedTime = 0.2f;
						}
					}
				}
				if (!this.hasPlayedBeep)
				{
					if (!Settings.soundDisabled)
					{
						CrashModule.beep.Play(0.5f, 0.5f, 0f);
						this.os.delayer.Post(ActionDelayer.Wait(0.1), delegate
						{
							CrashModule.beep.Play(0.5f, 0.5f, 0f);
						});
					}
					this.hasPlayedBeep = true;
				}
			}
			else
			{
				this.state = 2;
			}
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x00061E80 File Offset: 0x00060080
		public override void Draw(float t)
		{
			switch (this.state)
			{
			case 0:
				this.spriteBatch.Draw(Utils.white, this.bounds, this.bluescreenBlue);
				this.drawString((this.elapsedTime % 0.8f > 0.5f) ? "" : "_", new Vector2((float)this.bounds.X, (float)(this.bounds.Y + 10)), this.bsodFont);
				return;
			case 1:
				this.spriteBatch.Draw(Utils.white, this.bounds, this.bluescreenBlue);
				this.drawString(this.bsodText, new Vector2((float)this.bounds.X, (float)(this.bounds.Y + 10)), this.bsodFont);
				return;
			case 3:
			{
				float num = GuiData.ActiveFontConfig.tinyFontCharHeight + 1f;
				this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
				int num2 = Math.Min((int)(((float)(this.bounds.Height - 10) - num) / num), this.bootTextCount);
				num2 = this.bootTextCount;
				Vector2 dpos = new Vector2((float)(this.bounds.X + 10), (float)num2 * num + 10f);
				float y = dpos.Y;
				if (dpos.Y > (float)(this.bounds.Y + this.bounds.Height - 14))
				{
					dpos.Y = (float)(this.bounds.Y + this.bounds.Height - 14);
				}
				while (num2 >= 0 && dpos.Y > num)
				{
					this.drawString(this.bootText[num2], dpos, GuiData.tinyfont);
					dpos.Y -= num;
					num2--;
				}
				if (this.IsInHostileFileCrash)
				{
					dpos.Y = y + num;
					this.drawString("ERROR: Critical boot error loading \"VMBootloaderTrap.dll\"", dpos, GuiData.tinyfont);
					for (int i = 0; i < this.extraErrors; i++)
					{
						dpos.Y += num;
						this.drawString("ERROR:", dpos, GuiData.tinyfont);
					}
				}
				return;
			}
			}
			this.spriteBatch.Draw(Utils.white, this.bounds, this.os.darkBackgroundColor);
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0006212C File Offset: 0x0006032C
		private Vector2 drawString(string text, Vector2 dpos, SpriteFont font)
		{
			if (this.IsInHostileFileCrash)
			{
				float num = this.elapsedTime / 15f;
				num = Utils.QuadraticOutCurve(num);
				text = Utils.FlipRandomChars(text, (double)num * 0.2 * (double)num * (double)num);
				string text2 = "";
				for (int i = 0; i < text.Length; i++)
				{
					text2 += ((Utils.randm(1f) < num * num * num) ? ' ' : text[i]);
				}
				text = text2;
			}
			Vector2 result = font.MeasureString(text);
			bool flag = text.StartsWith("ERROR:");
			this.spriteBatch.DrawString(font, text, dpos, flag ? Color.Red : Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			return result;
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x00062218 File Offset: 0x00060418
		public string checkOSBootFiles(string bootString)
		{
			this.BootLoadErrors = "";
			Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
			bool flag = true;
			string text = "ERROR: " + LocaleTerms.Loc("Unable to Load System file os-config.sys") + "\n";
			if (folder.containsFile("os-config.sys"))
			{
				text = "Loaded os-config.sys : System Config Initialized";
			}
			else
			{
				this.os.failBoot();
				flag = false;
				this.BootLoadErrors = this.BootLoadErrors + text + " \n";
			}
			bootString = bootString.Replace("[OSBoot1]", text);
			text = "ERROR: " + LocaleTerms.Loc("Unable to Load System file bootcfg.dll") + "\n";
			if (folder.containsFile("bootcfg.dll"))
			{
				text = "Loaded bootcfg.dll : Boot Config Module Loaded";
			}
			else
			{
				this.os.failBoot();
				flag = false;
				this.BootLoadErrors = this.BootLoadErrors + text + " \n";
			}
			bootString = bootString.Replace("[OSBoot2]", text);
			text = "ERROR: " + LocaleTerms.Loc("Unable to Load System file netcfgx.dll") + "\n";
			if (folder.containsFile("netcfgx.dll"))
			{
				text = "Loaded netcfgx.dll : Network Config Module Loaded";
			}
			else
			{
				this.os.failBoot();
				flag = false;
				this.BootLoadErrors = this.BootLoadErrors + text + " \n";
			}
			bootString = bootString.Replace("[OSBoot3]", text);
			text = string.Concat(new string[]
			{
				"ERROR: ",
				LocaleTerms.Loc("Unable to Load System file x-server.sys"),
				"\nERROR: ",
				LocaleTerms.Loc("Locate and restore a valid x-server file in ~/sys/ folder to restore UX functionality"),
				"\nERROR: ",
				LocaleTerms.Loc("Consider examining reports in ~/log/ for problem cause and source"),
				"\nERROR: ",
				LocaleTerms.Loc("System UX resources unavailable -- defaulting to terminal mode"),
				"\n .\n .\n .\n"
			});
			if (folder.containsFile("x-server.sys"))
			{
				text = "Loaded x-server.sys : UX Graphics Module Loaded";
				ThemeManager.switchTheme(this.os, ThemeManager.getThemeForDataString(folder.searchForFile("x-server.sys").data));
				this.graphicsErrorsDetected = false;
			}
			else
			{
				this.os.graphicsFailBoot();
				flag = false;
				this.graphicsErrorsDetected = true;
				this.BootLoadErrors = this.BootLoadErrors + text + " \n";
			}
			bootString = bootString.Replace("[OSBootTheme]", text);
			if (flag)
			{
				if (this.os.Flags.HasFlag("BootFailure") && !this.os.Flags.HasFlag("BootFailureThemeSongChange"))
				{
					if (ThemeManager.currentTheme != OSTheme.HacknetBlue)
					{
						this.os.Flags.AddFlag("BootFailureThemeSongChange");
						if (MusicManager.isPlaying)
						{
							MusicManager.stop();
						}
						MusicManager.loadAsCurrentSong("Music\\The_Quickening");
					}
				}
				this.os.sucsesfulBoot();
			}
			else
			{
				this.os.Flags.AddFlag("BootFailure");
			}
			return bootString;
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x00062544 File Offset: 0x00060744
		public void reset()
		{
			this.elapsedTime = 0f;
			this.state = 0;
			this.bootTextCount = 0;
			this.bootTextTimer = 0f;
			this.bootTextErrorDelay = 0f;
			this.hasPlayedBeep = false;
			this.IsInHostileFileCrash = false;
			this.extraErrors = 0;
			this.loadBootText();
			MusicManager.stop();
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x000625A4 File Offset: 0x000607A4
		public void completeReboot()
		{
			this.os.terminal.reset();
			this.os.execute("FirstTimeInitdswhupwnemfdsiuoewnmdsmffdjsklanfeebfjkalnbmsdakj Init");
			this.os.inputEnabled = false;
			if (!this.graphicsErrorsDetected)
			{
				this.os.setMouseVisiblity(true);
				MusicManager.playSong();
			}
			this.os.connectedComp = null;
			Folder folder = this.os.thisComputer.files.root.searchForFolder("sys");
			if (folder.searchForFile("Notes_Reopener.bat") != null)
			{
				this.os.runCommand("notes");
			}
			try
			{
				this.os.threadedSaveExecute(false);
				this.os.gameSavedTextAlpha = -1f;
			}
			catch (Exception)
			{
				int num = -1;
				num++;
			}
		}

		// Token: 0x040006A1 RID: 1697
		private const float BOOT_FAIL_CRASH_TIME = 15f;

		// Token: 0x040006A2 RID: 1698
		public static float BLUESCREEN_TIME = 8f;

		// Token: 0x040006A3 RID: 1699
		public static float BOOT_TIME = Settings.isConventionDemo ? 5f : (Settings.FastBootText ? 1.2f : 14.5f);

		// Token: 0x040006A4 RID: 1700
		public static float BLACK_TIME = 2f;

		// Token: 0x040006A5 RID: 1701
		public static float POST_BLACK_TIME = 1f;

		// Token: 0x040006A6 RID: 1702
		public string BootLoadErrors = "";

		// Token: 0x040006A7 RID: 1703
		public float elapsedTime = 0f;

		// Token: 0x040006A8 RID: 1704
		public int state = 0;

		// Token: 0x040006A9 RID: 1705
		public Color bluescreenBlue = new Color(0, 0, 170);

		// Token: 0x040006AA RID: 1706
		public Color bluescreenGrey = new Color(167, 167, 167);

		// Token: 0x040006AB RID: 1707
		public Color textColor = new Color(0, 0, 255);

		// Token: 0x040006AC RID: 1708
		private SpriteFont bsodFont;

		// Token: 0x040006AD RID: 1709
		private static SoundEffect beep;

		// Token: 0x040006AE RID: 1710
		private string bsodText = "";

		// Token: 0x040006AF RID: 1711
		private string originalBootText;

		// Token: 0x040006B0 RID: 1712
		private string[] bootText;

		// Token: 0x040006B1 RID: 1713
		private int bootTextCount = 0;

		// Token: 0x040006B2 RID: 1714
		private float bootTextDelay = 1f;

		// Token: 0x040006B3 RID: 1715
		private float bootTextTimer = 0f;

		// Token: 0x040006B4 RID: 1716
		private float bootTextErrorDelay = 0f;

		// Token: 0x040006B5 RID: 1717
		private bool graphicsErrorsDetected = false;

		// Token: 0x040006B6 RID: 1718
		private bool hasPlayedBeep = false;

		// Token: 0x040006B7 RID: 1719
		private bool IsInHostileFileCrash = false;

		// Token: 0x040006B8 RID: 1720
		private int extraErrors = 0;
	}
}
