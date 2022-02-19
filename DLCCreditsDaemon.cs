using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x02000195 RID: 405
	internal class DLCCreditsDaemon : Daemon
	{
		// Token: 0x06000A35 RID: 2613 RVA: 0x000A2AE8 File Offset: 0x000A0CE8
		public DLCCreditsDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Labyrinths project"), os)
		{
			this.LoadSounds();
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x000A2B58 File Offset: 0x000A0D58
		public DLCCreditsDaemon(Computer c, OS os, string overrideTitle, string overrideButtonText) : base(c, overrideTitle, os)
		{
			this.LoadSounds();
			this.OverrideTitle = overrideTitle;
			this.OverrideButtonText = overrideButtonText;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x000A2BD0 File Offset: 0x000A0DD0
		private void LoadSounds()
		{
			if (DLC1SessionUpgrader.HasDLC1Installed)
			{
				this.spindown = this.os.content.Load<SoundEffect>("DLC/SFX/Spindown");
				this.spindownImpact = this.os.content.Load<SoundEffect>("DLC/SFX/RecoverImpact");
				this.buildup = this.os.content.Load<SoundEffect>("DLC/SFX/Kilmer_Woosh");
			}
			else
			{
				this.spindown = null;
				this.spindownImpact = this.os.content.Load<SoundEffect>("SFX/MeltImpact");
				this.buildup = this.os.content.Load<SoundEffect>("SFX/BrightFlash");
			}
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x000A2C80 File Offset: 0x000A0E80
		public override void navigatedTo()
		{
			base.navigatedTo();
			Folder folder = this.comp.files.root.searchForFolder("home");
			FileEntry fileEntry = folder.searchForFile("CreditsData.txt");
			if (fileEntry == null)
			{
				this.CreditsData = new string[]
				{
					"- " + LocaleTerms.Loc("Critical Error") + " -",
					LocaleTerms.Loc("Datafile not found")
				};
			}
			else
			{
				this.CreditsData = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
			}
			if (this.os.Flags.HasFlag("dlc_complete"))
			{
				this.showingCredits = true;
			}
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x000A2D40 File Offset: 0x000A0F40
		private void EndDLC()
		{
			DLC1SessionUpgrader.EndDLCSection(this.os);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x000A2D50 File Offset: 0x000A0F50
		private void AddRadialMailLine()
		{
			SFX.AddRadialLine(this.os.mailicon.pos + new Vector2(20f, 10f), (float)(3.141592653589793 + (double)Utils.rand(3.1415927f)), 600f + Utils.randm(300f), 800f, 500f, 200f + Utils.randm(400f), 0.35f, Utils.AddativeWhite * Utils.randm(1f), 3f, false);
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x000A2E54 File Offset: 0x000A1054
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			bounds = Utils.InsetRectangle(bounds, 1);
			int num = 90;
			int num2 = 30;
			Rectangle dest = new Rectangle(bounds.X, bounds.Y + num2, bounds.Width, num - num2);
			string text = (this.OverrideTitle == null) ? LocaleTerms.Loc("Labyrinths Project") : this.OverrideTitle;
			TextItem.doCenteredFontLabel(dest, text, GuiData.font, Color.White, false);
			Rectangle rectangle = new Rectangle(bounds.X, bounds.Y + num + num2, bounds.Width, bounds.Height - (num + 2 * num2));
			if (this.showingCredits)
			{
				this.timeInCredits += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				float num3 = this.timeInCredits;
				if (num3 <= 5f)
				{
					num3 = Utils.CubicInCurve(num3 / 5f);
				}
				else
				{
					num3 -= 4f;
				}
				num3 %= 165f;
				Utils.FillEverywhereExcept(rectangle, Utils.GetFullscreen(), sb, Color.Black * 0.5f);
				float num4 = 20f;
				float num5 = (float)(rectangle.Y - num2 + rectangle.Height) - num3 * num4;
				float num6 = num5;
				for (int i = 0; i < this.CreditsData.Length; i++)
				{
					int num7 = 22;
					SpriteFont font = GuiData.smallfont;
					Color color = Color.LightGray * 0.9f;
					string text2 = this.CreditsData[i];
					if (text2.StartsWith("%"))
					{
						text2 = text2.Substring(1);
						num7 = 45;
						font = GuiData.font;
						color = Utils.AddativeWhite * 0.9f;
					}
					else if (text2.StartsWith("^"))
					{
						text2 = text2.Substring(1);
						num7 = 30;
						font = GuiData.font;
						color = Color.White;
					}
					if (num6 >= (float)(rectangle.Y - num2))
					{
						Rectangle dest2 = new Rectangle(rectangle.X, (int)num6, rectangle.Width, num7);
						TextItem.doCenteredFontLabel(dest2, text2, font, color, false);
					}
					num6 += (float)(num7 + 2);
					if (num6 > (float)(rectangle.Y + rectangle.Height))
					{
						break;
					}
				}
				if (this.timeInCredits > 40f)
				{
					if (Button.doButton(18394902, rectangle.X + rectangle.Width / 4, rectangle.Y + rectangle.Height - 23, rectangle.Width / 2, 20, LocaleTerms.Loc("Proceed"), new Color?((this.timeInCredits > 65f) ? this.os.highlightColor : Color.Black)))
					{
						this.os.display.command = "connect";
					}
				}
			}
			else if (this.isInResetSequence)
			{
				this.timeInReset += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				if (this.timeInReset >= 5f)
				{
					this.showingCredits = true;
					PostProcessor.EndingSequenceFlashOutActive = false;
					PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
					if (!Settings.IsInExtensionMode)
					{
						this.EndDLC();
						if (!this.hasCuedFinaleSong)
						{
							this.os.delayer.Post(ActionDelayer.Wait(2.0), delegate
							{
								MusicManager.playSongImmediatley("DLC\\Music\\DreamHead");
							});
							this.hasCuedFinaleSong = true;
						}
						MediaPlayer.IsRepeating = true;
						for (int i = 0; i < 9; i++)
						{
							double time = (double)i / 7.0;
							this.os.delayer.Post(ActionDelayer.Wait(time), delegate
							{
								SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeWhite * 0.8f, 400f);
							});
						}
					}
				}
				else
				{
					float num8 = Math.Min(this.timeInReset, 1f);
					Utils.FillEverywhereExcept(rectangle, Utils.GetFullscreen(), sb, Color.Black * 0.5f * num8);
					PatternDrawer.draw(rectangle, 0.3f, Color.Transparent, Color.Black * 0.7f, sb, PatternDrawer.binaryTile);
					this.AddRadialMailLine();
					PostProcessor.EndingSequenceFlashOutActive = true;
					PostProcessor.EndingSequenceFlashOutPercentageComplete = num8;
					if (!this.hasCuedBuildup && (double)this.timeInReset > 2.8)
					{
						this.buildup.Play();
						this.hasCuedBuildup = true;
					}
					TextItem.doCenteredFontLabel(rectangle, LocaleTerms.Loc("Disabling..."), GuiData.font, Color.White, false);
				}
			}
			else
			{
				PatternDrawer.draw(rectangle, 0.3f, Color.Transparent, Color.Black * 0.7f, sb, PatternDrawer.binaryTile);
				string text3 = (this.OverrideButtonText == null) ? LocaleTerms.Loc("Disable Agent Monitoring") : this.OverrideButtonText;
				bool flag = this.OverrideButtonText != null;
				if (Button.doButton(38101920, rectangle.X + 50, rectangle.Y + rectangle.Height / 2 - 13, rectangle.Width - 100, 26, text3, new Color?(this.os.highlightColor)))
				{
					if (!flag)
					{
						this.isInResetSequence = true;
						this.timeInReset = 0f;
						if (MusicManager.currentSongName == "DLC/Music/RemiDrone")
						{
							MusicManager.stop();
						}
						this.spindownImpact.Play();
						if (this.spindown != null)
						{
							this.os.delayer.Post(ActionDelayer.Wait(1.1), delegate
							{
								this.spindown.Play();
							});
						}
						DLC1SessionUpgrader.ReDsicoverAllVisibleNodesInOSCache(this.os);
					}
					else
					{
						this.isInResetSequence = false;
						this.showingCredits = true;
						if (this.ConditionalActionsToLoadOnButtonPress != null)
						{
							RunnableConditionalActions.LoadIntoOS(this.ConditionalActionsToLoadOnButtonPress, this.os);
						}
					}
				}
			}
			Rectangle destinationRectangle = new Rectangle(bounds.X, bounds.Y + num, bounds.Width, num2);
			sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
			destinationRectangle.Y = bounds.Y + num + num2 + rectangle.Height;
			sb.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x000A3570 File Offset: 0x000A1770
		public override string getSaveString()
		{
			string text = "";
			if (this.OverrideTitle != null)
			{
				text += string.Format("Title=\"{0}\" ", this.OverrideTitle);
			}
			if (this.OverrideButtonText != null)
			{
				text += string.Format("Button=\"{0}\" ", this.OverrideButtonText);
			}
			if (this.ConditionalActionsToLoadOnButtonPress != null)
			{
				text += string.Format("Action=\"{0}\" ", this.ConditionalActionsToLoadOnButtonPress);
			}
			return "<DLCCredits " + text + "/>";
		}

		// Token: 0x04000B7C RID: 2940
		private const float ResetSequenceTime = 5f;

		// Token: 0x04000B7D RID: 2941
		private string[] CreditsData;

		// Token: 0x04000B7E RID: 2942
		private bool showingCredits = false;

		// Token: 0x04000B7F RID: 2943
		private bool isInResetSequence = false;

		// Token: 0x04000B80 RID: 2944
		private float timeInReset = 0f;

		// Token: 0x04000B81 RID: 2945
		private float timeInCredits = 0f;

		// Token: 0x04000B82 RID: 2946
		private SoundEffect spindown;

		// Token: 0x04000B83 RID: 2947
		private SoundEffect spindownImpact;

		// Token: 0x04000B84 RID: 2948
		private SoundEffect buildup;

		// Token: 0x04000B85 RID: 2949
		private bool hasCuedBuildup = false;

		// Token: 0x04000B86 RID: 2950
		private bool hasCuedFinaleSong = false;

		// Token: 0x04000B87 RID: 2951
		public string OverrideTitle = null;

		// Token: 0x04000B88 RID: 2952
		public string OverrideButtonText = null;

		// Token: 0x04000B89 RID: 2953
		public string ConditionalActionsToLoadOnButtonPress = null;
	}
}
