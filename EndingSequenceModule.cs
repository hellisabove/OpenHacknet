using System;
using Hacknet.Effects;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x020000DC RID: 220
	internal class EndingSequenceModule : Module
	{
		// Token: 0x06000469 RID: 1129 RVA: 0x00046BCC File Offset: 0x00044DCC
		public EndingSequenceModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.bounds = location;
			this.creditsScroll = (float)(this.os.fullscreen.Height / 2);
			this.spinUpEffect = this.os.content.Load<SoundEffect>("Music/Ambient/spiral_gauge_up");
			this.traceDownEffect = this.os.content.Load<SoundEffect>("SFX/TraceKill");
			this.BitSpeechText = Utils.readEntireFile("Content/Post/BitSpeech.txt");
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00046C98 File Offset: 0x00044E98
		public override void Update(float t)
		{
			base.Update(t);
			if (this.IsActive)
			{
				if (!this.IsInCredits)
				{
					if (this.waveRender == null)
					{
						this.traceDownEffect.Play();
						this.speech = this.os.content.Load<SoundEffect>("SFX/Ending/EndingSpeech");
						this.waveRender = new WaveformRenderer("Content/SFX/Ending/EndingSpeech.wav");
						this.speechinstance = this.speech.CreateInstance();
						this.speechinstance.IsLooped = false;
						this.CreditsData = Utils.readEntireFile("Content/Post/CreditsData.txt").Split(Utils.newlineDelim, StringSplitOptions.None);
						MusicManager.stop();
					}
					if (this.speechinstance.State == SoundState.Playing)
					{
						this.elapsedTime += t;
						if (this.elapsedTime > (float)this.speech.Duration.TotalSeconds)
						{
							this.RollCredits();
						}
						else
						{
							this.SpeechTextTimer += t;
							if (this.SpeechTextIndex < this.BitSpeechText.Length)
							{
								if (this.BitSpeechText[this.SpeechTextIndex] == '#')
								{
									if (this.SpeechTextTimer >= 1f)
									{
										this.SpeechTextTimer -= 1f;
										this.SpeechTextIndex++;
									}
								}
								else if (this.BitSpeechText[this.SpeechTextIndex] == '%')
								{
									if (this.SpeechTextTimer >= 0.5f)
									{
										this.SpeechTextTimer -= 0.5f;
										this.SpeechTextIndex++;
									}
								}
								else if (this.SpeechTextTimer >= 0.05f)
								{
									this.SpeechTextTimer -= 0.05f;
									this.SpeechTextIndex++;
								}
							}
						}
					}
					else
					{
						this.speechinstance.Play();
					}
				}
				else
				{
					this.elapsedTime += t;
					if (this.elapsedTime > this.HacknetTitleFreezeTime)
					{
						float num = Math.Min(1f, (this.elapsedTime - 10f) / 8f);
						this.creditsScroll -= t * this.creditsPixelsScrollPerSecond * num;
					}
				}
			}
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00046F4C File Offset: 0x0004514C
		private void RollCredits()
		{
			if (this.os.TraceDangerSequence.IsActive)
			{
				this.os.TraceDangerSequence.CancelTraceDangerSequence();
			}
			this.IsInCredits = true;
			if (this.speechinstance != null)
			{
				this.speechinstance.Stop();
			}
			Settings.soundDisabled = false;
			this.elapsedTime = 0f;
			this.os.delayer.Post(ActionDelayer.Wait(1.0), delegate
			{
				MusicManager.playSongImmediatley("Music\\Bit(Ending)");
				MediaPlayer.IsRepeating = false;
				AchievementsManager.Unlock("progress_complete", false);
			});
			PostProcessor.dangerModeEnabled = false;
			PostProcessor.dangerModePercentComplete = 0f;
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x00047004 File Offset: 0x00045204
		public override void Draw(float t)
		{
			base.Draw(t);
			Rectangle fullscreen = this.os.fullscreen;
			this.spriteBatch.Draw(Utils.white, fullscreen, Color.Black);
			if (this.waveRender != null)
			{
				if (!this.IsInCredits)
				{
					int width = this.os.fullscreen.Width;
					int height = this.os.fullscreen.Height;
					Rectangle bounds = new Rectangle(0, this.os.fullscreen.Height / 2 - height / 2, width, height);
					this.waveRender.RenderWaveform((double)this.elapsedTime, this.speech.Duration.TotalSeconds, this.spriteBatch, bounds);
					string[] array = this.BitSpeechText.Substring(0, this.SpeechTextIndex).Replace("#", "").Replace("%", "").Split(Utils.newlineDelim, StringSplitOptions.None);
					Vector2 position = new Vector2((float)this.bounds.X + 150f, (float)(this.bounds.Y + this.bounds.Height) - 100f);
					float num = 1f;
					int num2 = array.Length - 1;
					while (num2 >= 0 && num2 > array.Length - 5)
					{
						this.spriteBatch.DrawString(GuiData.smallfont, array[num2], position, Utils.AddativeWhite * num);
						num *= 0.6f;
						position.Y -= GuiData.ActiveFontConfig.tinyFontCharHeight + 8f;
						num2--;
					}
				}
				else
				{
					this.DrawCredits();
				}
			}
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x000471D4 File Offset: 0x000453D4
		private void DrawCredits()
		{
			float y = this.creditsScroll;
			Vector2 value = new Vector2(0f, y);
			int num = (int)((float)this.os.fullscreen.Width * 0.5f);
			int num2 = (int)((float)this.os.fullscreen.Height * 0.4f);
			Rectangle dest = new Rectangle(this.os.fullscreen.Width / 2 - num / 2, (int)(value.Y - (float)(num2 / 2)), num, num2);
			Rectangle destinationRectangle = new Rectangle(this.os.fullscreen.X, dest.Y + 65, this.os.fullscreen.Width, dest.Height - 135);
			bool flag = this.elapsedTime >= 1.71f;
			if (flag)
			{
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(Utils.AddativeRed, Color.Red, 0.2f + Utils.randm(0.05f)) * 0.5f);
				FlickeringTextEffect.DrawLinedFlickeringText(dest, "HACKNET", 16f, 0.4f, GuiData.titlefont, this.os, Color.White, 5);
			}
			value.Y += (float)this.os.fullscreen.Height / 2f;
			for (int i = 0; i < this.CreditsData.Length; i++)
			{
				float num3 = 20f;
				string text = this.CreditsData[i];
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = text;
					SpriteFont spriteFont = GuiData.font;
					Color color = Color.White * 0.7f;
					if (text.StartsWith("^"))
					{
						text2 = text.Substring(1);
						color = Color.Gray * 0.6f;
					}
					else if (text.StartsWith("%"))
					{
						text2 = text.Substring(1);
						spriteFont = GuiData.titlefont;
						num3 = 90f;
					}
					if (text.StartsWith("$"))
					{
						text2 = text.Substring(1);
						color = Color.Gray * 0.6f;
						spriteFont = GuiData.smallfont;
					}
					Vector2 vector = spriteFont.MeasureString(text2);
					Vector2 position = value + new Vector2((float)(this.os.fullscreen.Width / 2) - vector.X / 2f, 0f);
					text2 = Utils.CleanStringToRenderable(text2);
					this.spriteBatch.DrawString(spriteFont, text2, position, color);
					value.Y += num3;
				}
				value.Y += num3;
			}
			if (value.Y < -500f)
			{
				this.CompleteAndReturnToMenu();
			}
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x000475A4 File Offset: 0x000457A4
		private void CompleteAndReturnToMenu()
		{
			this.os.Flags.AddFlag("Victory");
			Programs.disconnect(new string[0], this.os);
			Computer computer = Programs.getComputer(this.os, "porthackHeart");
			this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(computer));
			computer.disabled = true;
			computer.daemons.Clear();
			computer.ip = NetworkMap.generateRandomIP();
			this.os.terminal.inputLocked = false;
			this.os.ram.inputLocked = false;
			this.os.netMap.inputLocked = false;
			this.os.DisableTopBarButtons = false;
			this.os.canRunContent = true;
			this.IsActive = false;
			ComputerLoader.loadMission("Content/Missions/CreditsMission.xml", false);
			this.os.threadedSaveExecute(false);
			MediaPlayer.IsRepeating = true;
			MusicManager.playSongImmediatley("Music\\Bit(Ending)");
			if (Settings.isPirateBuild)
			{
				this.os.delayer.Post(ActionDelayer.Wait(15.0), delegate
				{
					try
					{
						ComputerLoader.loadMission("Content/Missions/CreditsMission_p.xml", false);
					}
					catch (Exception)
					{
					}
				});
			}
			if (Settings.sendsDLC1PromoEmailAtEnd)
			{
				this.os.delayer.Post(ActionDelayer.Wait(30.0), delegate
				{
					try
					{
						string body = Utils.readEntireFile("Content/LocPost/DLCMessage.txt");
						string subject = "Labyrinths";
						string sender = "Matt Trobbiani";
						string mail = MailServer.generateEmail(subject, body, sender);
						MailServer mailServer = this.os.netMap.mailServer.getDaemon(typeof(MailServer)) as MailServer;
						if (mailServer != null)
						{
							mailServer.addMail(mail, this.os.defaultUser.name);
						}
					}
					catch (Exception)
					{
					}
				});
			}
		}

		// Token: 0x04000548 RID: 1352
		private const float SpeechTextHashDelay = 1f;

		// Token: 0x04000549 RID: 1353
		private const float SpeechTextPercDelay = 0.5f;

		// Token: 0x0400054A RID: 1354
		private const float SpeechTextCharDelay = 0.05f;

		// Token: 0x0400054B RID: 1355
		public bool IsActive = false;

		// Token: 0x0400054C RID: 1356
		private float elapsedTime = 0f;

		// Token: 0x0400054D RID: 1357
		private float HacknetTitleFreezeTime = 10f;

		// Token: 0x0400054E RID: 1358
		private float creditsPixelsScrollPerSecond = 65f;

		// Token: 0x0400054F RID: 1359
		private SoundEffect speech;

		// Token: 0x04000550 RID: 1360
		private SoundEffectInstance speechinstance;

		// Token: 0x04000551 RID: 1361
		private WaveformRenderer waveRender;

		// Token: 0x04000552 RID: 1362
		private bool IsInCredits = false;

		// Token: 0x04000553 RID: 1363
		private float creditsScroll = 0f;

		// Token: 0x04000554 RID: 1364
		private string[] CreditsData;

		// Token: 0x04000555 RID: 1365
		private SoundEffect spinUpEffect;

		// Token: 0x04000556 RID: 1366
		private SoundEffect traceDownEffect;

		// Token: 0x04000557 RID: 1367
		private string BitSpeechText;

		// Token: 0x04000558 RID: 1368
		private int SpeechTextIndex = 0;

		// Token: 0x04000559 RID: 1369
		private float SpeechTextTimer = 0f;
	}
}
