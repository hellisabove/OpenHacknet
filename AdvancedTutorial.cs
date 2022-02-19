using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet
{
	// Token: 0x020000B7 RID: 183
	internal class AdvancedTutorial : ExeModule
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x00038E4C File Offset: 0x0003704C
		public AdvancedTutorial(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.state = 0;
			this.lastCommand = "";
			this.ramCost = 500;
			this.IdentifierName = "Tutorial v16.2";
			this.flashTimer = 1f;
			this.startSound = this.os.content.Load<SoundEffect>("SFX/DoomShock");
			this.advanceFlash = this.os.content.Load<SoundEffect>("SFX/BrightFlash");
			AdvancedTutorial.commandSequence = null;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00039010 File Offset: 0x00037210
		public override void LoadContent()
		{
			base.LoadContent();
			this.bounceEffect = new NodeBounceEffect
			{
				NodeHitDelay = 0.02f,
				TimeBetweenBounces = 0.2f
			};
			string text = "\n\n%&%&%&%\n";
			string text2 = LocalizedFileLoader.Read("Content/Post/AdvancedTutorialData.txt").Replace("\r\n", "\n");
			string[] array = text2.Split(new string[]
			{
				text
			}, StringSplitOptions.None);
			if (AdvancedTutorial.feedbackSequence != null && AdvancedTutorial.loadedLocale != Settings.ActiveLocale)
			{
				AdvancedTutorial.feedbackSequence = null;
			}
			AdvancedTutorial.loadedLocale = (Settings.ActiveLocale ?? "");
			if (AdvancedTutorial.commandSequence == null)
			{
				AdvancedTutorial.altCommandequence = new List<string[]>();
				for (int i = 0; i < 15; i++)
				{
					AdvancedTutorial.altCommandequence.Add(new string[0]);
				}
				AdvancedTutorial.commandSequence = new List<string>();
				AdvancedTutorial.commandSequence.Add("&#*@(&#@(&#@&)@&#)(@&)@#");
				AdvancedTutorial.commandSequence.Add("connect " + this.os.thisComputer.ip);
				AdvancedTutorial.commandSequence.Add("scan");
				AdvancedTutorial.commandSequence.Add("dc");
				AdvancedTutorial.altCommandequence[3] = new string[]
				{
					"disconnect"
				};
				AdvancedTutorial.commandSequence.Add("connect");
				AdvancedTutorial.commandSequence.Add("probe");
				AdvancedTutorial.altCommandequence[5] = new string[]
				{
					"nmap"
				};
				AdvancedTutorial.commandSequence.Add("porthack");
				AdvancedTutorial.commandSequence.Add("scan");
				AdvancedTutorial.commandSequence.Add("ls");
				AdvancedTutorial.altCommandequence[8] = new string[]
				{
					"dir"
				};
				AdvancedTutorial.commandSequence.Add("cd bin");
				AdvancedTutorial.altCommandequence[9] = new string[]
				{
					"cd ../bin",
					"cd ../../bin",
					"cd /bin"
				};
				AdvancedTutorial.commandSequence.Add("cat config.txt");
				AdvancedTutorial.altCommandequence[10] = new string[]
				{
					"less config.txt"
				};
				AdvancedTutorial.commandSequence.Add("cd ..");
				AdvancedTutorial.altCommandequence[11] = new string[]
				{
					"cd..",
					"cd /"
				};
				AdvancedTutorial.commandSequence.Add("cd log");
				AdvancedTutorial.altCommandequence[12] = new string[]
				{
					"cd ../log",
					"cd ../../log",
					"cd /log"
				};
				AdvancedTutorial.commandSequence.Add("rm *");
				AdvancedTutorial.commandSequence.Add("dc");
				AdvancedTutorial.altCommandequence[14] = new string[]
				{
					"disconnect"
				};
			}
			if (AdvancedTutorial.feedbackSequence == null)
			{
				AdvancedTutorial.feedbackSequence = new List<string>();
				AdvancedTutorial.feedbackSequence.Add(array[0]);
				AdvancedTutorial.feedbackSequence.Add(array[1]);
				AdvancedTutorial.feedbackSequence.Add(array[2]);
				AdvancedTutorial.feedbackSequence.Add(array[3]);
				AdvancedTutorial.feedbackSequence.Add(array[4]);
				AdvancedTutorial.feedbackSequence.Add(array[5]);
				AdvancedTutorial.feedbackSequence.Add(array[6]);
				AdvancedTutorial.feedbackSequence.Add(array[7]);
				AdvancedTutorial.feedbackSequence.Add(array[8]);
				AdvancedTutorial.feedbackSequence.Add(array[9]);
				AdvancedTutorial.feedbackSequence.Add(array[10]);
				AdvancedTutorial.feedbackSequence.Add(array[11]);
				AdvancedTutorial.feedbackSequence.Add(array[12]);
				AdvancedTutorial.feedbackSequence.Add(array[13]);
				AdvancedTutorial.feedbackSequence.Add(array[14]);
				AdvancedTutorial.feedbackSequence.Add(array[15]);
			}
			this.stepCompletionSequence = new List<Action>();
			for (int i = 0; i < AdvancedTutorial.feedbackSequence.Count; i++)
			{
				this.stepCompletionSequence.Add(null);
			}
			this.stepCompletionSequence[1] = delegate()
			{
				this.os.netMap.visible = true;
				this.os.netMap.inputLocked = false;
			};
			this.stepCompletionSequence[2] = delegate()
			{
				this.os.display.visible = true;
				this.os.display.inputLocked = false;
			};
			this.stepCompletionSequence[3] = delegate()
			{
				this.os.netMap.inputLocked = true;
			};
			this.stepCompletionSequence[4] = delegate()
			{
				this.os.netMap.inputLocked = false;
			};
			this.stepCompletionSequence[5] = delegate()
			{
				this.os.display.inputLocked = true;
				this.os.ram.inputLocked = true;
				this.os.netMap.inputLocked = true;
				this.os.terminal.visible = true;
				this.os.terminal.inputLocked = false;
				this.os.terminal.clearCurrentLine();
			};
			this.stepCompletionSequence[8] = delegate()
			{
				this.os.display.inputLocked = false;
				this.os.ram.inputLocked = false;
			};
			this.state = 0;
			this.getRenderText();
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x0003956C File Offset: 0x0003776C
		public override void Update(float t)
		{
			base.Update(t);
			string lastRunCommand = this.os.terminal.getLastRunCommand();
			if (this.lastCommand != lastRunCommand)
			{
				this.lastCommand = lastRunCommand;
				this.parseCommand();
			}
			else if (GuiData.getKeyboadState().IsKeyDown(Keys.F8))
			{
				this.lastCommand = "";
			}
			this.flashTimer -= t;
			this.flashTimer = Math.Max(this.flashTimer, 0f);
			this.bounceEffect.Update(t, delegate(Vector2 nodePos)
			{
				this.bounceEffect.NodeHitDelay = Math.Max(0f, -0.2f + Utils.randm(0.5f));
				this.bounceEffect.TimeBetweenBounces = 0.15f + Utils.randm(0.7f);
			});
			this.hintTextFadeTimer = Math.Max(0f, this.hintTextFadeTimer - t);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00039630 File Offset: 0x00037830
		public void parseCommand()
		{
			try
			{
				if (this.state < AdvancedTutorial.commandSequence.Count)
				{
					bool flag = this.lastCommand.ToLower().StartsWith(AdvancedTutorial.commandSequence[this.state]);
					if (!flag)
					{
						for (int i = 0; i < AdvancedTutorial.altCommandequence[this.state].Length; i++)
						{
							flag |= this.lastCommand.ToLower().StartsWith(AdvancedTutorial.altCommandequence[this.state][i]);
						}
					}
					if (flag)
					{
						this.advanceState();
					}
				}
				else
				{
					this.lastCommand = null;
				}
			}
			catch (Exception)
			{
				this.lastCommand = "";
			}
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x0003970C File Offset: 0x0003790C
		private void advanceState()
		{
			this.state++;
			this.getRenderText();
			if (this.state > 0)
			{
				this.printCurrentCommandToTerminal();
			}
			this.flashTimer = 1f;
			if (this.stepCompletionSequence[this.state] != null)
			{
				this.stepCompletionSequence[this.state]();
			}
			if (!Settings.soundDisabled && !MusicManager.isMuted)
			{
				if (this.state > 1)
				{
					this.advanceFlash.Play(0.6f, 1f, 1f);
				}
			}
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x000397C4 File Offset: 0x000379C4
		public void printCurrentCommandToTerminal()
		{
			this.os.terminal.writeLine("\n--------------------------------------------------");
			this.os.terminal.writeLine(" ");
			for (int i = 0; i < this.renderText.Length; i++)
			{
				if (!this.renderText[i].Contains("<#") && !this.renderText[i].Contains("#>"))
				{
					this.os.terminal.writeLine(this.renderText[i]);
				}
			}
			this.os.terminal.writeLine(" ");
			this.os.terminal.writeLine("--------------------------------------------------\n");
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x0003988C File Offset: 0x00037A8C
		public void getRenderText()
		{
			string text = AdvancedTutorial.feedbackSequence[this.state];
			char[] separator = new char[]
			{
				'\n'
			};
			string[] array = text.Split(this.hintButtonDelimiter, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 1)
			{
				this.hintButtonText = DisplayModule.cleanSplitForWidth(array[1], 178).Split(separator);
				text = array[0];
				this.hintTextFadeTimer = 0f;
			}
			else
			{
				this.hintButtonText = null;
			}
			string data = LocalizedFileLoader.SafeFilterString(text);
			this.renderText = Utils.SuperSmartTwimForWidth(data, this.bounds.Width - 10, GuiData.tinyfont).Split(separator);
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00039938 File Offset: 0x00037B38
		public override void Killed()
		{
			base.Killed();
			if (!this.os.multiplayer && this.os.initShowsTutorial)
			{
				this.os.currentMission.sendEmail(this.os);
				this.os.Flags.AddFlag("TutorialComplete");
				this.os.initShowsTutorial = false;
			}
			if (this.os.hubServerAlertsIcon != null)
			{
				this.os.hubServerAlertsIcon.IsEnabled = true;
			}
			this.os.mailicon.isEnabled = true;
			this.os.terminal.visible = true;
			this.os.terminal.inputLocked = false;
			this.os.netMap.visible = true;
			this.os.netMap.inputLocked = false;
			this.os.ram.visible = true;
			this.os.ram.inputLocked = false;
			this.os.display.visible = true;
			this.os.display.inputLocked = false;
			if (this.state < AdvancedTutorial.feedbackSequence.Count - 1)
			{
				AchievementsManager.Unlock("kill_tutorial", false);
			}
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00039A90 File Offset: 0x00037C90
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			Rectangle bounds = this.bounds;
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			bounds.Height -= Module.PANEL_HEIGHT;
			bounds.Y += Module.PANEL_HEIGHT;
			PatternDrawer.draw(bounds, 1f, this.os.highlightColor * 0.2f, this.os.highlightColor, this.spriteBatch);
			bounds.X += 2;
			bounds.Y += 2;
			bounds.Width -= 4;
			bounds.Height -= 4;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor * 0.99f);
			this.drawTarget("app:");
			int num = 260;
			Rectangle bounds2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height - num, this.bounds.Width - 2, num);
			this.bounceEffect.Draw(this.spriteBatch, bounds2, Utils.AdditivizeColor(this.os.highlightColor) * 0.35f, Utils.AddativeWhite * 0.5f);
			int num2 = 210;
			Rectangle rectangle = new Rectangle(bounds2.X, bounds2.Y + (num - num2), bounds2.Width, num2);
			this.spriteBatch.Draw(Utils.gradient, rectangle, null, this.os.darkBackgroundColor, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.7f);
			Rectangle destinationRectangle = rectangle;
			destinationRectangle.Y = bounds2.Y;
			destinationRectangle.Height = num - num2;
			this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
			this.spriteBatch.Draw(Utils.white, bounds, this.os.highlightColor * this.flashTimer);
			this.spriteBatch.DrawString(GuiData.font, Utils.FlipRandomChars("Tutorial", 0.024), new Vector2((float)(this.bounds.X + 5), (float)(this.bounds.Y + 22)), this.os.subtleTextColor * 0.6f);
			this.spriteBatch.DrawString(GuiData.font, "Tutorial", new Vector2((float)(this.bounds.X + 5), (float)(this.bounds.Y + 22)), this.os.subtleTextColor);
			float num3 = GuiData.ActiveFontConfig.tinyFontCharHeight + 1f;
			Vector2 dpos = new Vector2((float)(this.bounds.X + 5), (float)(this.bounds.Y + 67));
			dpos = this.RenderText(this.renderText, dpos, num3, 1f);
			dpos.Y += num3;
			if (this.state == 0 && this.CanActivateFirstStep)
			{
				if (Button.doButton(2933201, this.bounds.X + 10, (int)(dpos.Y + 20f), this.bounds.Width - 20, 30, LocaleTerms.Loc("Continue"), null) || (!this.os.terminal.visible && Utils.keyPressed(GuiData.lastInput, Keys.Enter, null)))
				{
					this.advanceState();
					if (!Settings.soundDisabled)
					{
						this.startSound.Play(1f, 0f, 0f);
						this.startSound.Play(1f, 0f, 0f);
					}
				}
			}
			else if (this.hintButtonText != null)
			{
				if (Button.doButton(2933202, this.bounds.X + 10, (int)(dpos.Y + 6f), this.bounds.Width - 20, 20, LocaleTerms.Loc("Hint"), null))
				{
					this.hintTextFadeTimer = 9f;
				}
				dpos.Y += 30f;
				dpos.X += 10f;
				float num4 = Math.Min(1f, this.hintTextFadeTimer / 3f);
				string text = "";
				for (int i = 0; i < this.hintButtonText.Length; i++)
				{
					text = text + this.hintButtonText[i] + "\n";
				}
				text = Utils.SuperSmartTwimForWidth(text.Trim(), this.bounds.Width, GuiData.tinyfont);
				string[] array = text.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				Rectangle destinationRectangle2 = new Rectangle((int)dpos.X, (int)dpos.Y, this.bounds.Width, (int)(num3 * (float)array.Length));
				this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.5f * num4);
				this.RenderText(array, dpos, num3, num4);
			}
		}

		// Token: 0x060003BB RID: 955 RVA: 0x0003A06C File Offset: 0x0003826C
		private Vector2 RenderText(string[] stringData, Vector2 dpos, float charHeight, float opacityMod = 1f)
		{
			bool flag = false;
			int i = 0;
			while (i < stringData.Length)
			{
				string text = stringData[i];
				text = text.Replace("\r", "");
				if (text.Length > 0)
				{
					if (text.Trim() == "<#")
					{
						flag = true;
					}
					else if (text.Trim() == "#>")
					{
						flag = false;
					}
					else
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, text, dpos, (flag ? this.os.highlightColor : Color.White) * opacityMod);
						dpos.Y += charHeight;
					}
				}
				IL_B6:
				i++;
				continue;
				goto IL_B6;
			}
			return dpos;
		}

		// Token: 0x060003BC RID: 956 RVA: 0x0003A148 File Offset: 0x00038348
		private Vector2 RenderTextOld(string[] stringData, Vector2 dpos, float charHeight, float opacityMod = 1f)
		{
			foreach (string text in stringData)
			{
				if (text.Length > 0)
				{
					bool flag = false;
					if (text[0] == '#')
					{
						text = text.Substring(1, text.Length - 2);
						flag = true;
					}
					this.spriteBatch.DrawString(GuiData.tinyfont, text, dpos, (flag ? this.os.highlightColor : Color.White) * opacityMod);
					dpos.Y += charHeight;
				}
			}
			return dpos;
		}

		// Token: 0x0400044A RID: 1098
		private static List<string> commandSequence;

		// Token: 0x0400044B RID: 1099
		private static List<string[]> altCommandequence;

		// Token: 0x0400044C RID: 1100
		private static List<string> feedbackSequence;

		// Token: 0x0400044D RID: 1101
		private static string loadedLocale = "en-us";

		// Token: 0x0400044E RID: 1102
		private int state;

		// Token: 0x0400044F RID: 1103
		private string lastCommand;

		// Token: 0x04000450 RID: 1104
		private string[] renderText;

		// Token: 0x04000451 RID: 1105
		private string[] hintButtonText = null;

		// Token: 0x04000452 RID: 1106
		private float hintTextFadeTimer = 0f;

		// Token: 0x04000453 RID: 1107
		private string[] hintButtonDelimiter = new string[]
		{
			"|"
		};

		// Token: 0x04000454 RID: 1108
		private float flashTimer;

		// Token: 0x04000455 RID: 1109
		private SoundEffect startSound;

		// Token: 0x04000456 RID: 1110
		private SoundEffect advanceFlash;

		// Token: 0x04000457 RID: 1111
		private List<Action> stepCompletionSequence;

		// Token: 0x04000458 RID: 1112
		private NodeBounceEffect bounceEffect;

		// Token: 0x04000459 RID: 1113
		public bool CanActivateFirstStep = true;
	}
}
