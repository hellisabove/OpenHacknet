using System;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Effects
{
	// Token: 0x020000B4 RID: 180
	internal class TraceDangerSequence
	{
		// Token: 0x060003A0 RID: 928 RVA: 0x00037A50 File Offset: 0x00035C50
		public TraceDangerSequence(ContentManager content, SpriteBatch sb, Rectangle fullscreenRect, OS os)
		{
			this.titleFont = GuiData.titlefont;
			this.bodyFont = GuiData.font;
			this.fullscreen = fullscreenRect;
			this.spriteBatch = sb;
			this.scaleupSpriteBatch = new SpriteBatch(sb.GraphicsDevice);
			this.os = os;
			this.spinDownSound = os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_down");
			this.spinUpSound = os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_up");
			this.impactSound = os.content.Load<SoundEffect>("SFX/MeltImpact");
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x00037B2C File Offset: 0x00035D2C
		public void BeginTraceDangerSequence()
		{
			this.timeThisState = 0f;
			this.state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;
			this.IsActive = true;
			this.PreventOSRendering = true;
			this.oldSong = MusicManager.currentSongName;
			this.os.execute("dc");
			this.os.display.command = "";
			this.os.terminal.inputLocked = true;
			MusicManager.playSongImmediatley("Music/Ambient/dark_drone_008");
			this.spinDownSound.Play();
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00037BB4 File Offset: 0x00035DB4
		public void CompleteIPResetSucsesfully()
		{
			this.timeThisState = 0f;
			this.state = TraceDangerSequence.TraceDangerState.DisconnectedReboot;
			this.IsActive = true;
			this.PreventOSRendering = true;
			PostProcessor.dangerModeEnabled = false;
			PostProcessor.dangerModePercentComplete = 0f;
			MusicManager.stop();
			this.spinDownSound.Play(1f, -0.6f, 0f);
			this.impactSound.Play();
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00037C1F File Offset: 0x00035E1F
		public void CancelTraceDangerSequence()
		{
			this.timeThisState = 0f;
			this.state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;
			this.IsActive = false;
			this.PreventOSRendering = false;
			PostProcessor.dangerModeEnabled = false;
			MusicManager.stop();
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x00037C50 File Offset: 0x00035E50
		public void Update(float t)
		{
			this.timeThisState += t;
			float num = float.MaxValue;
			PostProcessor.dangerModeEnabled = false;
			switch (this.state)
			{
			case TraceDangerSequence.TraceDangerState.WarningScrenIntro:
				num = 1f;
				if (this.timeThisState > num)
				{
					this.timeThisState = 0f;
					this.state = TraceDangerSequence.TraceDangerState.WarningScreen;
					this.warningScreenIsActivating = false;
				}
				break;
			case TraceDangerSequence.TraceDangerState.WarningScreenExiting:
				num = 13.9f;
				if (this.timeThisState > num)
				{
					this.timeThisState = 0f;
					this.state = TraceDangerSequence.TraceDangerState.Countdown;
					this.os.display.visible = true;
					this.os.netMap.visible = true;
					this.os.terminal.visible = true;
					this.os.ram.visible = true;
				}
				break;
			case TraceDangerSequence.TraceDangerState.Countdown:
				num = 130f;
				if (this.timeThisState > num)
				{
					this.timeThisState = 0f;
					this.state = TraceDangerSequence.TraceDangerState.Gameover;
					this.CancelTraceDangerSequence();
					Game1.getSingleton().Exit();
				}
				if ((this.os.timer - this.onBeatFlashTimer) % 1.9376667f < 0.05f)
				{
					this.os.warningFlash();
				}
				PostProcessor.dangerModePercentComplete = Math.Min(this.timeThisState / (num * 0.85f), 1f);
				PostProcessor.dangerModeEnabled = true;
				this.PreventOSRendering = false;
				break;
			case TraceDangerSequence.TraceDangerState.DisconnectedReboot:
				num = 10f;
				if (this.timeThisState > num)
				{
					this.CancelTraceDangerSequence();
					MusicManager.loadAsCurrentSong(this.oldSong);
					this.os.rebootThisComputer();
				}
				break;
			}
			this.percentComplete = this.timeThisState / num;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00037E34 File Offset: 0x00036034
		public void Draw()
		{
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			switch (this.state)
			{
			case TraceDangerSequence.TraceDangerState.WarningScrenIntro:
			{
				this.DrawFlashingRedBackground();
				Rectangle destinationRectangle = new Rectangle(10, this.fullscreen.Height / 2 - 2, this.fullscreen.Width - 20, 4);
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black);
				destinationRectangle.Width = (int)((float)destinationRectangle.Width * (1f - this.percentComplete));
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Red);
				break;
			}
			case TraceDangerSequence.TraceDangerState.WarningScreen:
				this.DrawWarningScreen();
				break;
			case TraceDangerSequence.TraceDangerState.WarningScreenExiting:
			{
				this.DrawFlashingRedBackground();
				Rectangle destinationRectangle = new Rectangle(10, this.fullscreen.Height / 2 - 2, this.fullscreen.Width - 20, 4);
				if (this.percentComplete > 0.5f)
				{
					float num = (this.percentComplete - 0.5f) * 2f;
					num = Utils.QuadraticOutCurve(num);
					int num2 = (int)((float)this.os.fullscreen.Height * 0.7f * num);
					destinationRectangle.Y = this.fullscreen.Height / 2 - 2 - num2 / 2;
					destinationRectangle.Height = num2;
				}
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black);
				destinationRectangle.Width = (int)((float)destinationRectangle.Width * Math.Min(1f, Utils.QuadraticOutCurve(this.percentComplete * 2f)));
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.DarkRed);
				float num3 = Utils.QuadraticOutCurve((this.percentComplete - 0.5f) * 2f);
				if (this.percentComplete > 0.5f)
				{
					ThinBarcode thinBarcode = new ThinBarcode(destinationRectangle.Width, destinationRectangle.Height);
					thinBarcode.Draw(this.spriteBatch, destinationRectangle.X, destinationRectangle.Y, (Utils.randm(1f) > num3) ? Color.Black : ((Utils.randm(1f) > num3 && Utils.randm(1f) > 0.8f) ? Utils.AddativeWhite : Utils.VeryDarkGray));
				}
				TextItem.doFontLabel(new Vector2((float)(this.fullscreen.Width / 2 - 250), (float)(destinationRectangle.Y - 70)), "INITIALIZING FAILSAFE", GuiData.titlefont, new Color?(Color.White), 500f, 70f, false);
				break;
			}
			case TraceDangerSequence.TraceDangerState.Countdown:
			{
				this.PreventOSRendering = false;
				float num4 = this.timeThisState * 0.5f;
				if (num4 < 1f)
				{
					this.os.display.visible = (num4 > Utils.randm(1f));
					this.os.netMap.visible = (num4 > Utils.randm(1f));
					this.os.terminal.visible = (num4 > Utils.randm(1f));
					this.os.ram.visible = (num4 > Utils.randm(1f));
				}
				else
				{
					this.os.display.visible = true;
					this.os.netMap.visible = true;
					this.os.terminal.visible = true;
					this.os.ram.visible = true;
				}
				TraceDangerSequence.DrawCountdownOverlay(this.titleFont, this.bodyFont, this.os, null, null, null, null);
				break;
			}
			case TraceDangerSequence.TraceDangerState.DisconnectedReboot:
				this.DrawDisconnectedScreen();
				break;
			}
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0003820C File Offset: 0x0003640C
		private void DrawDisconnectedScreen()
		{
			this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.Black);
			Rectangle destinationRectangle = default(Rectangle);
			destinationRectangle.X = this.fullscreen.X + 2;
			destinationRectangle.Width = this.fullscreen.Width - 4;
			destinationRectangle.Y = this.fullscreen.Y + this.fullscreen.Height / 6 * 2;
			destinationRectangle.Height = this.fullscreen.Height / 3;
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.indentBackgroundColor);
			Vector2 vector = GuiData.titlefont.MeasureString("DISCONNECTED");
			Vector2 position = new Vector2((float)(destinationRectangle.X + this.fullscreen.Width / 2) - vector.X / 2f, (float)(this.fullscreen.Y + this.fullscreen.Height / 2 - 50));
			this.spriteBatch.DrawString(GuiData.titlefont, "DISCONNECTED", position, this.os.subtleTextColor);
			Vector2 pos = new Vector2(200f, (float)(destinationRectangle.Y + destinationRectangle.Height + 20));
			pos = this.DrawFlashInString(LocaleTerms.Loc("IP Address successfully reset"), pos, 4f, 0.2f, true, 0.2f);
			pos = this.DrawFlashInString(LocaleTerms.Loc("Foreign trace averted"), pos, 5f, 0.2f, true, 0.2f);
			pos = this.DrawFlashInString(LocaleTerms.Loc("Preparing for system reboot"), pos, 6f, 0.2f, true, 0.8f);
			pos = this.DrawFlashInString(LocaleTerms.Loc("Rebooting"), pos, 9f, 0.2f, true, 0.2f);
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x00038464 File Offset: 0x00036664
		private void DrawWarningScreen()
		{
			if (this.warningScreenIsActivating)
			{
				this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.White);
			}
			else
			{
				this.DrawFlashingRedBackground();
			}
			string text = "WARNING";
			Vector2 vector = this.titleFont.MeasureString(text);
			float num = (float)this.fullscreen.Width * 0.65f;
			float num2 = num / vector.X;
			Vector2 vector2 = new Vector2(20f, -10f);
			this.spriteBatch.DrawString(this.titleFont, text, vector2, Color.Black, 0f, Vector2.Zero, num2, SpriteEffects.None, 0.5f);
			vector2.Y += vector.Y * num2 - 55f;
			TextItem.doFontLabel(vector2, LocaleTerms.Loc("COMPLETED TRACE DETECTED : EMERGENCY RECOVERY MODE ACTIVE"), Settings.ActiveLocale.StartsWith("en") ? this.titleFont : GuiData.font, new Color?(Color.Black), num, float.MaxValue, false);
			vector2.Y += 40f;
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("Unsyndicated foreign connection detected during active trace"), vector2, 0f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(" :: " + LocaleTerms.Loc("Emergency recovery mode activated"), vector2, 0.1f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString("-----------------------------------------------------------------------", vector2, 0.2f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(" ", vector2, 0.5f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("Automated screening procedures will divert incoming connections temporarily"), vector2, 0.5f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("This window is a final opportunity to regain anonymity."), vector2, 0.6f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("As your current IP Address is known, it must be changed") + " -", vector2, 0.7f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("This can only be done on your currently active ISP's routing server"), vector2, 0.8f, 0.2f, false, 0.2f);
			Computer computer = Programs.getComputer(this.os, "ispComp");
			vector2 = this.DrawFlashInString(string.Format(LocaleTerms.Loc("Reverse tracerouting has located this ISP server's IP address as {0}"), (computer != null) ? computer.ip : "68.144.93.18"), vector2, 0.9f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(string.Format(LocaleTerms.Loc("Your local ip : {0}  must be tracked here and changed."), this.os.thisComputer.ip), vector2, 1f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(" ", vector2, 1.1f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("Failure to complete this while active diversion holds will result in complete"), vector2, 1.1f, 0.2f, false, 0.2f);
			vector2 = this.DrawFlashInString(LocaleTerms.Loc("and permanent loss of all account data - THIS IS NOT REPEATABLE AND CANNOT BE DELAYED"), vector2, 1.2f, 0.2f, false, 0.2f);
			if (!this.warningScreenIsActivating)
			{
				if (this.timeThisState >= 1.2f && Button.doButton(789798001, 20, (int)(vector2.Y + 10f), 400, 40, LocaleTerms.Loc("BEGIN"), new Color?(Color.Black)))
				{
					this.timeThisState = 0f;
					this.state = TraceDangerSequence.TraceDangerState.WarningScreenExiting;
					this.PreventOSRendering = true;
					this.onBeatFlashTimer = this.os.timer;
					this.warningScreenIsActivating = true;
					this.spinUpSound.Play(1f, 0f, 0f);
					this.os.terminal.inputLocked = false;
					this.os.delayer.Post(ActionDelayer.Wait(0.1), delegate
					{
						this.spinUpSound.Play(1f, 0f, 0f);
					});
					this.os.delayer.Post(ActionDelayer.Wait(0.4), delegate
					{
						this.spinUpSound.Play(0.4f, 0f, 0f);
					});
					this.os.delayer.Post(ActionDelayer.Wait(0.8), delegate
					{
						this.spinUpSound.Play(0.2f, 0.1f, 0f);
					});
					this.os.delayer.Post(ActionDelayer.Wait(1.3), delegate
					{
						this.spinUpSound.Play(0.1f, 0.2f, 0f);
					});
					this.os.delayer.Post(ActionDelayer.Wait(0.01), delegate
					{
						MusicManager.playSongImmediatley("Music/Traced");
					});
				}
			}
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x00038980 File Offset: 0x00036B80
		private Vector2 DrawFlashInString(string text, Vector2 pos, float offset, float transitionInTime = 0.2f, bool hasDots = false, float dotsDelayer = 0.2f)
		{
			Vector2 value = new Vector2(40f, 0f);
			if (this.timeThisState >= offset)
			{
				float num = Math.Min((this.timeThisState - offset) / transitionInTime, 1f);
				Vector2 position = pos + value * (1f - Utils.QuadraticOutCurve(num));
				string text2 = "";
				if (hasDots)
				{
					float num2 = this.timeThisState - offset;
					while (num2 > 0f && text2.Length < 5)
					{
						num2 -= dotsDelayer;
						text2 += ".";
					}
				}
				float scale = 0.5f;
				float num3 = 17f;
				if (LocaleActivator.ActiveLocaleIsCJK())
				{
					scale = 0.7f;
					num3 = 22f;
				}
				this.spriteBatch.DrawString(this.bodyFont, text + text2, position, Color.White * num, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.4f);
				pos.Y += num3;
			}
			return pos;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x00038AB0 File Offset: 0x00036CB0
		public static void DrawCountdownOverlay(SpriteFont titleFont, SpriteFont bodyFont, object osobj, string title = null, string l1 = null, string l2 = null, string l3 = null)
		{
			OS os = (OS)osobj;
			if (title == null)
			{
				title = "EMERGENCY TRACE AVERSION SEQUENCE";
			}
			if (l1 == null)
			{
				l1 = LocaleTerms.Loc("Reset Assigned Ip Address on ISP Mainframe");
			}
			Computer computer = Programs.getComputer(os, "ispComp");
			if (l2 == null)
			{
				l2 = string.Format(LocaleTerms.Loc("ISP Mainframe IP: {0}"), (computer != null) ? computer.ip : "68.144.93.18");
			}
			if (l3 == null)
			{
				l3 = string.Format(LocaleTerms.Loc("YOUR Assigned IP: {0}"), os.thisComputer.ip);
			}
			Rectangle rectangle = Utils.GetFullscreen();
			int num = 110;
			Rectangle destinationRectangle = new Rectangle(0, rectangle.Height - num - 20, 520, num);
			GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(TraceDangerSequence.DarkRed, Color.Transparent, Utils.randm(0.2f)));
			Vector2 pos = new Vector2((float)(destinationRectangle.X + 6), (float)(destinationRectangle.Y + 4));
			TextItem.doFontLabel(pos, title, titleFont, new Color?(Color.White), (float)(destinationRectangle.Width - 12), 35f, false);
			pos.Y += 32f;
			TextItem.doFontLabel(pos, l1, bodyFont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
			pos.Y += 16f;
			TextItem.doFontLabel(pos, l2, bodyFont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
			pos.Y += 16f;
			TextItem.doFontLabel(pos, l3, bodyFont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00038C94 File Offset: 0x00036E94
		private void DrawFlashingRedBackground()
		{
			this.spriteBatch.Draw(Utils.white, this.fullscreen, Color.Lerp(TraceDangerSequence.BackgroundRed, Color.Black, Utils.randm(0.22f)));
		}

		// Token: 0x0400042A RID: 1066
		private const float WARNING_INTRO_TIME = 1f;

		// Token: 0x0400042B RID: 1067
		private const float WARNING_EXIT_TIME = 13.9f;

		// Token: 0x0400042C RID: 1068
		private const float DISCONNECT_REBOOT_TIME = 10f;

		// Token: 0x0400042D RID: 1069
		private const float COUNTDOWN_TIME = 130f;

		// Token: 0x0400042E RID: 1070
		private const float FLASH_FREQUENCY = 1.9376667f;

		// Token: 0x0400042F RID: 1071
		public bool IsActive = false;

		// Token: 0x04000430 RID: 1072
		public bool PreventOSRendering = false;

		// Token: 0x04000431 RID: 1073
		private SpriteFont titleFont;

		// Token: 0x04000432 RID: 1074
		private SpriteFont bodyFont;

		// Token: 0x04000433 RID: 1075
		private Rectangle fullscreen;

		// Token: 0x04000434 RID: 1076
		private SpriteBatch spriteBatch;

		// Token: 0x04000435 RID: 1077
		private SpriteBatch scaleupSpriteBatch;

		// Token: 0x04000436 RID: 1078
		private OS os;

		// Token: 0x04000437 RID: 1079
		private SoundEffect spinDownSound;

		// Token: 0x04000438 RID: 1080
		private SoundEffect spinUpSound;

		// Token: 0x04000439 RID: 1081
		private SoundEffect impactSound;

		// Token: 0x0400043A RID: 1082
		private float onBeatFlashTimer = 0f;

		// Token: 0x0400043B RID: 1083
		private float timeThisState = 0f;

		// Token: 0x0400043C RID: 1084
		private float percentComplete = 0f;

		// Token: 0x0400043D RID: 1085
		private TraceDangerSequence.TraceDangerState state = TraceDangerSequence.TraceDangerState.WarningScrenIntro;

		// Token: 0x0400043E RID: 1086
		private string oldSong = null;

		// Token: 0x0400043F RID: 1087
		private bool warningScreenIsActivating = false;

		// Token: 0x04000440 RID: 1088
		private static Color DarkRed = new Color(105, 0, 0, 200);

		// Token: 0x04000441 RID: 1089
		private static Color BackgroundRed = new Color(120, 0, 0);

		// Token: 0x020000B5 RID: 181
		private enum TraceDangerState
		{
			// Token: 0x04000444 RID: 1092
			WarningScrenIntro,
			// Token: 0x04000445 RID: 1093
			WarningScreen,
			// Token: 0x04000446 RID: 1094
			WarningScreenExiting,
			// Token: 0x04000447 RID: 1095
			Countdown,
			// Token: 0x04000448 RID: 1096
			Gameover,
			// Token: 0x04000449 RID: 1097
			DisconnectedReboot
		}
	}
}
