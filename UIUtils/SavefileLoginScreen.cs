using System;
using System.Collections.Generic;
using System.Text;
using Hacknet.Gui;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet.UIUtils
{
	// Token: 0x020000F2 RID: 242
	public class SavefileLoginScreen
	{
		// Token: 0x06000526 RID: 1318 RVA: 0x00050638 File Offset: 0x0004E838
		public void WriteToHistory(string message)
		{
			this.History.Add(message);
		}

		// Token: 0x06000527 RID: 1319 RVA: 0x00050648 File Offset: 0x0004E848
		public void ClearTextBox()
		{
			GuiData.getFilteredKeys();
			this.terminalString = "";
			TextBox.cursorPosition = 0;
		}

		// Token: 0x06000528 RID: 1320 RVA: 0x00050664 File Offset: 0x0004E864
		public void ResetForNewAccount()
		{
			this.promptIndex = 0;
			this.IsReady = false;
			this.PromptSequence.Clear();
			this.PromptSequence.Add(LocaleTerms.Loc("USERNAME") + " :");
			this.PromptSequence.Add(LocaleTerms.Loc("PASSWORD") + " :");
			this.PromptSequence.Add(LocaleTerms.Loc("CONFIRM PASS") + " :");
			this.History.Clear();
			this.History.Add("-- " + LocaleTerms.Loc("New " + this.ProjectName + " User Registration") + " --");
			this.currentPrompt = this.PromptSequence[this.promptIndex];
			this.IsNewAccountMode = true;
			this.terminalString = "";
			TextBox.cursorPosition = 0;
		}

		// Token: 0x06000529 RID: 1321 RVA: 0x0005075C File Offset: 0x0004E95C
		public void ResetForLogin()
		{
			this.promptIndex = 0;
			this.IsReady = false;
			this.PromptSequence.Clear();
			this.PromptSequence.Add(LocaleTerms.Loc("USERNAME") + " :");
			this.PromptSequence.Add(LocaleTerms.Loc("PASSWORD") + " :");
			this.History.Clear();
			this.History.Add("-- Login --");
			this.currentPrompt = this.PromptSequence[this.promptIndex];
			this.IsNewAccountMode = false;
			this.terminalString = "";
			TextBox.cursorPosition = 0;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x00050810 File Offset: 0x0004EA10
		private void Advance(string answer)
		{
			this.promptIndex++;
			this.Answers.Add(answer);
			if (this.IsNewAccountMode)
			{
				if (this.promptIndex == 1)
				{
					if (string.IsNullOrWhiteSpace(this.Answers[0]))
					{
						this.History.Add(" -- " + LocaleTerms.Loc("Username cannot be blank. Try Again") + " -- ");
						this.promptIndex = 0;
						this.Answers.Clear();
					}
					else if (Utils.StringContainsInvalidFilenameChars(this.Answers[0]))
					{
						this.History.Add(" -- " + LocaleTerms.Loc("Username contains invalid characters. Try Again") + " -- ");
						this.promptIndex = 0;
						this.Answers.Clear();
					}
					else if (!SaveFileManager.CanCreateAccountForName(this.Answers[0]))
					{
						this.History.Add(" -- " + LocaleTerms.Loc("Username already in use. Try Again") + " -- ");
						this.promptIndex = 0;
						this.Answers.Clear();
					}
				}
				if (this.promptIndex == 3)
				{
					if (string.IsNullOrWhiteSpace(answer))
					{
						this.History.Add(" -- " + LocaleTerms.Loc("Password Cannot be Blank! Try Again") + " -- ");
						this.promptIndex = 1;
						string text = this.Answers[0];
						this.Answers.Clear();
						this.Answers.Add(text);
					}
					else if (this.Answers[1] != answer)
					{
						this.History.Add(" -- " + LocaleTerms.Loc("Password Mismatch! Try Again") + " -- ");
						this.promptIndex = 1;
						string text = this.Answers[0];
						this.Answers.Clear();
						this.Answers.Add(text);
					}
				}
				this.InPasswordMode = (this.promptIndex == 1 || this.promptIndex == 2);
			}
			else
			{
				this.InPasswordMode = (this.promptIndex == 1);
			}
			if (this.promptIndex >= this.PromptSequence.Count)
			{
				if (this.IsNewAccountMode)
				{
					this.History.Add(" -- " + LocaleTerms.Loc("Details Confirmed") + " -- ");
					this.History.Add(LocaleTerms.Loc("WARNING") + " : " + LocaleTerms.Loc("Once created, a session's language cannot be changed"));
					this.currentPrompt = LocaleTerms.Loc("READY - PRESS ENTER TO CONFIRM");
					this.IsReady = true;
				}
				else
				{
					string text = this.Answers[0];
					string pass = this.Answers[1];
					string filePathForLogin = SaveFileManager.GetFilePathForLogin(text, pass);
					this.userPathCache = filePathForLogin;
					if (filePathForLogin == null)
					{
						this.promptIndex = 0;
						this.currentPrompt = this.PromptSequence[this.promptIndex];
						this.History.Add(" -- " + LocaleTerms.Loc("Invalid Login Details") + " -- ");
					}
					else
					{
						this.IsReady = true;
						this.currentPrompt = LocaleTerms.Loc("READY - PRESS ENTER TO CONFIRM");
					}
				}
			}
			else
			{
				this.currentPrompt = this.PromptSequence[this.promptIndex];
			}
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x00050BF4 File Offset: 0x0004EDF4
		public void Draw(SpriteBatch sb, Rectangle dest)
		{
			int num = 300;
			Rectangle rectangle = dest;
			Rectangle rectangle2 = new Rectangle(dest.X, dest.Y, dest.Width - num, dest.Height);
			if (!this.IsNewAccountMode)
			{
				dest = rectangle2;
			}
			SpriteFont smallfont = GuiData.smallfont;
			int num2 = (int)(smallfont.MeasureString(this.currentPrompt).X + 4f);
			int num3 = this.DrawFromTop ? dest.Y : (dest.Y + dest.Height - 18);
			GuiData.spriteBatch.DrawString(smallfont, this.currentPrompt, new Vector2((float)dest.X, (float)num3), Color.White);
			if (!this.IsReady)
			{
				TextBox.MaskingText = this.InPasswordMode;
				this.terminalString = TextBox.doTerminalTextField(16392802, dest.X + num2, num3 - 2, dest.Width, 20, 1, this.terminalString, GuiData.UISmallfont);
			}
			if (!this.IsNewAccountMode)
			{
				Vector2 pos = new Vector2((float)(rectangle.X + rectangle2.Width), (float)rectangle.Y);
				if (SaveFileManager.Accounts.Count > 0)
				{
					TextItem.doFontLabel(pos, LocaleTerms.Loc("LOCAL ACCOUNTS") + " ::", GuiData.font, new Color?(Color.Gray), (float)num, 22f, false);
					pos.Y += 22f;
				}
				if (!this.HasOverlayScreen)
				{
					for (int i = 0; i < SaveFileManager.Accounts.Count; i++)
					{
						if (Button.doButton(2870300 + i + i * 12, (int)pos.X, (int)pos.Y, num, 18, SaveFileManager.Accounts[i].Username, new Color?(Color.Black)))
						{
							this.Answers = new List<string>(new string[]
							{
								SaveFileManager.Accounts[i].Username,
								SaveFileManager.Accounts[i].Password
							});
							this.promptIndex = 2;
							TextBox.BoxWasActivated = true;
							this.IsReady = true;
							break;
						}
						int index = i;
						if (Button.doButton(7070300 + i + i * 12, (int)pos.X + num + 4, (int)pos.Y, 21, 18, "X", new Color?(Color.DarkRed)))
						{
							string text = LocaleTerms.Loc("Are you sure you wish to delete account {0}?");
							text = string.Format(text, "\"" + SaveFileManager.Accounts[i].Username + "\"");
							text = Utils.SuperSmartTwimForWidth(text, 400, GuiData.font);
							MessageBoxScreen messageBoxScreen = new MessageBoxScreen(text);
							messageBoxScreen.OverrideAcceptedText = LocaleTerms.Loc("Delete Account");
							messageBoxScreen.OverrideCancelText = LocaleTerms.Loc("Cancel");
							MessageBoxScreen messageBoxScreen2 = messageBoxScreen;
							messageBoxScreen2.AcceptedClicked = (Action)Delegate.Combine(messageBoxScreen2.AcceptedClicked, new Action(delegate()
							{
								this.HasOverlayScreen = false;
								SaveFileManager.DeleteUser(SaveFileManager.Accounts[index].Username);
							}));
							MessageBoxScreen messageBoxScreen3 = messageBoxScreen;
							messageBoxScreen3.CancelClicked = (Action)Delegate.Combine(messageBoxScreen3.CancelClicked, new Action(delegate()
							{
								this.HasOverlayScreen = false;
							}));
							Game1.getSingleton().sman.AddScreen(messageBoxScreen);
							this.HasOverlayScreen = true;
						}
						pos.Y += 22f;
					}
				}
			}
			if (!this.HasOverlayScreen && TextBox.BoxWasActivated)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.currentPrompt);
				stringBuilder.Append(" ");
				string text2 = this.terminalString;
				if (this.InPasswordMode)
				{
					string text3 = "";
					for (int i = 0; i < text2.Length; i++)
					{
						text3 += "*";
					}
					text2 = text3;
				}
				stringBuilder.Append(text2);
				this.History.Add(stringBuilder.ToString());
				this.Advance(this.terminalString);
				this.terminalString = "";
				TextBox.cursorPosition = 0;
				TextBox.BoxWasActivated = false;
			}
			int num4 = this.DrawFromTop ? (dest.Y + 24) : (dest.Y + dest.Height + 12);
			GuiData.spriteBatch.Draw(Utils.white, new Rectangle(dest.X, num4, dest.Width / 2, 1), Utils.SlightlyDarkGray);
			num4 += 10;
			int num5 = num4 - 60;
			if (this.IsReady)
			{
				if (!GuiData.getKeyboadState().IsKeyDown(Keys.Enter))
				{
					this.CanReturnEnter = true;
				}
				if ((!this.HasOverlayScreen && (!this.IsNewAccountMode || Button.doButton(16392804, dest.X, num4, dest.Width / 3, 28, LocaleTerms.Loc("CONFIRM"), new Color?(Color.White)))) || (this.CanReturnEnter && Utils.keyPressed(GuiData.lastInput, Keys.Enter, null)))
				{
					if (!this.PreventAdvancing)
					{
						if (this.IsNewAccountMode)
						{
							if (this.Answers.Count < 3)
							{
								this.ResetForNewAccount();
							}
							else
							{
								string arg = this.Answers[0];
								string arg2 = this.Answers[1];
								TextBox.MaskingText = false;
								if (this.StartNewGameForUsernameAndPass != null)
								{
									this.StartNewGameForUsernameAndPass(arg, arg2);
								}
							}
						}
						else
						{
							TextBox.MaskingText = false;
							if (this.LoadGameForUserFileAndUsername != null)
							{
								this.LoadGameForUserFileAndUsername(this.userPathCache, this.Answers[0]);
							}
							this.History.Clear();
							this.currentPrompt = "";
						}
						this.PreventAdvancing = true;
					}
				}
				num4 += 36;
			}
			if (!this.HasOverlayScreen && Button.doButton(16392806, dest.X, this.DrawFromTop ? num5 : num4, dest.Width / 3, 22, LocaleTerms.Loc("CANCEL"), new Color?(SavefileLoginScreen.CancelColor)))
			{
				if (this.RequestGoBack != null)
				{
					this.InPasswordMode = false;
					TextBox.MaskingText = false;
					this.RequestGoBack();
				}
			}
			float num6 = GuiData.ActiveFontConfig.tinyFontCharHeight + 8f;
			Vector2 position = new Vector2((float)dest.X, this.DrawFromTop ? ((float)num4) : ((float)(dest.Y + dest.Height - 20) - num6));
			for (int i = this.History.Count - 1; i >= 0; i--)
			{
				sb.DrawString(GuiData.UISmallfont, this.History[i], position, Color.White);
				position.Y -= num6 * (this.DrawFromTop ? -1f : 1f);
			}
		}

		// Token: 0x040005C1 RID: 1473
		public Action<string, string> StartNewGameForUsernameAndPass;

		// Token: 0x040005C2 RID: 1474
		public Action<string, string> LoadGameForUserFileAndUsername;

		// Token: 0x040005C3 RID: 1475
		public Action RequestGoBack;

		// Token: 0x040005C4 RID: 1476
		private string terminalString = "";

		// Token: 0x040005C5 RID: 1477
		private List<string> History = new List<string>();

		// Token: 0x040005C6 RID: 1478
		private string currentPrompt = "USERNAME :";

		// Token: 0x040005C7 RID: 1479
		private int promptIndex = 0;

		// Token: 0x040005C8 RID: 1480
		public string ProjectName = "Hacknet";

		// Token: 0x040005C9 RID: 1481
		private List<string> PromptSequence = new List<string>();

		// Token: 0x040005CA RID: 1482
		private List<string> Answers = new List<string>();

		// Token: 0x040005CB RID: 1483
		private bool IsReady = false;

		// Token: 0x040005CC RID: 1484
		private bool IsNewAccountMode = true;

		// Token: 0x040005CD RID: 1485
		private bool InPasswordMode = false;

		// Token: 0x040005CE RID: 1486
		private bool CanReturnEnter = false;

		// Token: 0x040005CF RID: 1487
		private string userPathCache = null;

		// Token: 0x040005D0 RID: 1488
		public bool DrawFromTop = false;

		// Token: 0x040005D1 RID: 1489
		private bool HasOverlayScreen = false;

		// Token: 0x040005D2 RID: 1490
		private static Color CancelColor = new Color(125, 82, 82);

		// Token: 0x040005D3 RID: 1491
		private bool PreventAdvancing = false;
	}
}
