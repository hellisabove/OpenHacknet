using System;
using System.Collections.Generic;
using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000FC RID: 252
	internal class MailServer : Daemon
	{
		// Token: 0x06000569 RID: 1385 RVA: 0x00054FB4 File Offset: 0x000531B4
		public MailServer(Computer c, string name, OS os) : base(c, name, os)
		{
			this.state = 0;
			this.panel = os.content.Load<Texture2D>("Panel");
			this.corner = os.content.Load<Texture2D>("Corner");
			this.unopenedIcon = os.content.Load<Texture2D>("UnopenedMail");
			MailServer.buttonSound = os.content.Load<SoundEffect>("SFX/Bip");
			this.panelRect = default(Rectangle);
			this.evenLine = new Color(80, 81, 83);
			this.oddLine = new Color(58, 58, 58);
			this.senderDarkeningColor = new Color(0, 0, 0, 100);
			this.seperatorLineColor = Color.Transparent;
			this.textColor = Color.White;
			this.responders = new List<MailResponder>();
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x000550C0 File Offset: 0x000532C0
		public override void initFiles()
		{
			base.initFiles();
			this.initFilesystem();
			for (int i = 0; i < 10; i++)
			{
				this.comp.users.Add(new UserDetail(UsernameGenerator.getName()));
			}
			for (int i = 0; i < this.comp.users.Count; i++)
			{
				UserDetail userDetail = this.comp.users[i];
				if (userDetail.type == 1 || userDetail.type == 0 || userDetail.type == 2)
				{
					Folder folder = new Folder(userDetail.name);
					folder.files.Add(new FileEntry("Username: " + userDetail.name + "\nPassword: " + userDetail.pass, "AccountInfo"));
					Folder folder2 = new Folder("inbox");
					if (this.shouldGenerateJunkEmails && userDetail.name != this.os.defaultUser.name)
					{
						this.addJunkEmails(folder2);
					}
					folder.folders.Add(folder2);
					folder.folders.Add(new Folder("sent"));
					this.accounts.folders.Add(folder);
				}
			}
			if (this.setupComplete != null)
			{
				this.setupComplete();
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x0005524C File Offset: 0x0005344C
		public override void loadInit()
		{
			base.loadInit();
			this.root = this.comp.files.root.searchForFolder("mail");
			this.accounts = this.root.searchForFolder("accounts");
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x0005528C File Offset: 0x0005348C
		public void addJunkEmails(Folder f)
		{
			if (MailServer.shouldGenerateJunk)
			{
				int num = Utils.random.Next(10);
				for (int i = 0; i < num; i++)
				{
					f.files.Add(new FileEntry(MailServer.generateEmail("Re: Junk", BoatMail.JunkEmail, "admin@" + this.comp.name), "Re:_Junk#" + OS.currentElapsedTime));
				}
			}
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00055310 File Offset: 0x00053510
		public void initFilesystem()
		{
			this.rootPath = new List<int>();
			this.accountsPath = new List<int>();
			this.root = this.comp.files.root.searchForFolder("mail");
			if (this.root == null)
			{
				this.root = new Folder("mail");
				this.comp.files.root.folders.Add(this.root);
			}
			this.rootPath.Add(this.comp.files.root.folders.IndexOf(this.root));
			this.accountsPath.Add(this.comp.files.root.folders.IndexOf(this.root));
			this.accounts = new Folder("accounts");
			this.accountsPath.Add(0);
			this.root.folders.Add(this.accounts);
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00055423 File Offset: 0x00053623
		public void setThemeColor(Color newThemeColor)
		{
			this.themeColor = newThemeColor;
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x0005542D File Offset: 0x0005362D
		public void addResponder(MailResponder resp)
		{
			this.responders.Add(resp);
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x0005543D File Offset: 0x0005363D
		public void removeResponder(MailResponder resp)
		{
			this.responders.Remove(resp);
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x0005544D File Offset: 0x0005364D
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.state = 0;
			this.inboxPage = 0;
			this.totalPagesDetected = -1;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x0005546C File Offset: 0x0005366C
		public void viewInbox(UserDetail newUser)
		{
			this.userFolder = null;
			this.state = 3;
			for (int i = 0; i < this.comp.users.Count; i++)
			{
				if (this.comp.users[i].name.Equals(newUser.name))
				{
					this.user = this.comp.users[i];
					for (int j = 0; j < this.accounts.folders.Count; j++)
					{
						if (this.accounts.folders[j].name.Equals(this.user.name))
						{
							this.userFolder = this.accounts.folders[j];
							break;
						}
					}
					break;
				}
			}
			this.comp.currentUser = this.user;
			if (this.userFolder == null)
			{
			}
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00055580 File Offset: 0x00053780
		public void addMail(string mail, string userTo)
		{
			for (int i = 0; i < this.responders.Count; i++)
			{
				this.responders[i].mailReceived(mail, userTo);
			}
			Folder folder = null;
			for (int j = 0; j < this.accounts.folders.Count; j++)
			{
				if (this.accounts.folders[j].name.Equals(userTo))
				{
					folder = this.accounts.folders[j];
					folder = folder.folders[0];
					break;
				}
			}
			if (folder != null)
			{
				folder.files.Insert(0, new FileEntry(mail, MailServer.getSubject(mail)));
				return;
			}
			throw new NullReferenceException(string.Concat(new string[]
			{
				"User ",
				userTo,
				" has no valid mail account on this mail server :",
				this.comp.idName,
				". Check account type and name matching!"
			}));
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x0005569C File Offset: 0x0005389C
		public bool MailWithSubjectExists(string userName, string mailSubject)
		{
			for (int i = 0; i < this.accounts.folders.Count; i++)
			{
				if (this.accounts.folders[i].name.Equals(userName))
				{
					Folder folder = this.accounts.folders[i];
					for (int j = 0; j < folder.files.Count; j++)
					{
						string[] array = folder.files[j].data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
						if (array.Length >= 4)
						{
							if (array[2].ToLower() == mailSubject.ToLower())
							{
								return true;
							}
						}
					}
					break;
				}
			}
			return false;
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x00055780 File Offset: 0x00053980
		public override void userAdded(string name, string pass, byte type)
		{
			base.userAdded(name, pass, type);
			if (type == 0 || type == 1 || type == 2)
			{
				Folder folder = new Folder(name);
				folder.files.Add(new FileEntry("Username: " + name + "\nPassword: " + pass, "AccountInfo"));
				Folder folder2 = new Folder("inbox");
				this.addJunkEmails(folder2);
				folder.folders.Add(folder2);
				folder.folders.Add(new Folder("sent"));
				this.accounts.folders.Add(folder);
			}
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x00055828 File Offset: 0x00053A28
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<MailServer name=\"",
				this.name,
				"\" color=\"",
				Utils.convertColorToParseableString(this.themeColor),
				"\"/>"
			});
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x00055878 File Offset: 0x00053A78
		public virtual void drawTopBar(Rectangle bounds, SpriteBatch sb)
		{
			this.panelRect = new Rectangle(bounds.X + 1, bounds.Y, bounds.Width - (this.corner.Width + 2), this.panel.Height);
			sb.Draw(this.panel, this.panelRect, this.themeColor);
			sb.Draw(this.corner, new Vector2((float)(bounds.X + bounds.Width - (this.corner.Width + 1)), (float)bounds.Y), this.themeColor);
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x00055918 File Offset: 0x00053B18
		public virtual void drawBackingGradient(Rectangle boundsTo, SpriteBatch sb)
		{
			Rectangle destinationRectangle = boundsTo;
			destinationRectangle.X++;
			destinationRectangle.Width -= 2;
			destinationRectangle.Height -= 2;
			sb.Draw(Utils.gradient, destinationRectangle, Color.Black);
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x00055968 File Offset: 0x00053B68
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			Vector2 position = new Vector2((float)(bounds.X + 8), (float)(bounds.Y + 120));
			this.drawTopBar(bounds, sb);
			switch (this.state)
			{
			case 0:
				sb.DrawString(GuiData.font, this.name + " " + LocaleTerms.Loc("Mail Server"), position, this.textColor);
				position.Y += 80f;
				position.Y += 35f;
				if (Button.doButton(800002, (int)position.X, (int)position.Y, 300, 40, LocaleTerms.Loc("Login"), new Color?(this.themeColor)))
				{
					this.state = 2;
					this.os.displayCache = "";
					this.os.execute("login");
					while (this.os.displayCache.Equals(""))
					{
					}
					this.os.display.command = this.name;
				}
				position.Y += 45f;
				if (Button.doButton(800003, (int)position.X, (int)position.Y, 300, 30, LocaleTerms.Loc("Exit"), new Color?(this.themeColor)))
				{
					this.os.display.command = "connect";
				}
				break;
			case 1:
				sb.DrawString(GuiData.font, LocaleTerms.Loc("Test"), position, this.textColor);
				position.Y += 80f;
				if (Button.doButton(800001, (int)position.X, (int)position.Y, 300, 60, "testing", null))
				{
					this.state = 0;
				}
				break;
			case 2:
				this.drawBackingGradient(bounds, sb);
				sb.DrawString(GuiData.font, LocaleTerms.Loc("login"), position, this.textColor);
				position.Y += 80f;
				this.doLoginDisplay(bounds, sb);
				break;
			case 3:
				this.doInboxDisplay(bounds, sb);
				break;
			case 4:
				this.doEmailViewerDisplay(bounds, sb);
				break;
			case 5:
				this.doRespondDisplay(bounds, sb);
				break;
			}
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x00055C0C File Offset: 0x00053E0C
		private int GetRenderTextHeight()
		{
			return (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 2f);
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x00055C9C File Offset: 0x00053E9C
		private int DrawMailMessageText(Rectangle textBounds, SpriteBatch sb, string[] text)
		{
			if (this.sectionedPanel == null || this.sectionedPanel.PanelHeight != this.GetRenderTextHeight())
			{
				this.sectionedPanel = new ScrollableSectionedPanel(this.GetRenderTextHeight(), sb.GraphicsDevice);
			}
			this.sectionedPanel.NumberOfPanels = text.Length;
			int itemsDrawn = 0;
			this.sectionedPanel.Draw(delegate(int index, Rectangle dest, SpriteBatch spBatch)
			{
				spBatch.DrawString(GuiData.tinyfont, LocalizedFileLoader.SafeFilterString(text[index]), new Vector2((float)dest.X, (float)dest.Y), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.8f);
				itemsDrawn++;
			}, sb, textBounds);
			int result;
			if (this.sectionedPanel.NumberOfPanels * this.sectionedPanel.PanelHeight < textBounds.Height)
			{
				result = this.sectionedPanel.NumberOfPanels * this.sectionedPanel.PanelHeight;
			}
			else
			{
				result = textBounds.Height;
			}
			return result;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x00055D74 File Offset: 0x00053F74
		public void doEmailViewerDisplay(Rectangle bounds, SpriteBatch sb)
		{
			Vector2 vector = new Vector2((float)(bounds.X + 2), (float)(bounds.Y + 20));
			if (Button.doButton(800007, (int)vector.X, (int)vector.Y, bounds.Width - 20 - this.corner.Width, 30, LocaleTerms.Loc("Return to Inbox"), new Color?(this.os.darkBackgroundColor)))
			{
				this.state = 3;
			}
			vector.Y += 35f;
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = bounds.X + 1;
			tmpRect.Y = (int)vector.Y;
			tmpRect.Width = bounds.Width - 2;
			tmpRect.Height = 38;
			sb.Draw(Utils.white, tmpRect, this.textColor);
			vector.Y += 3f;
			sb.DrawString(GuiData.UITinyfont, "<" + this.emailData[1] + ">", vector, Color.Black);
			vector.Y += 18f;
			sb.DrawString(GuiData.UITinyfont, LocaleTerms.Loc("Subject") + ": " + LocalizedFileLoader.SafeFilterString(this.emailData[2]), vector, Color.Black);
			vector.Y += 25f;
			vector.X += 20f;
			int num = 25;
			int num2 = (this.emailData.Length - 4 + 1) * num;
			Rectangle textBounds = new Rectangle((int)vector.X, (int)vector.Y, bounds.Width - 22, (int)((float)bounds.Height - (vector.Y - (float)bounds.Y) - (float)num2));
			string text3;
			if (LocaleActivator.ActiveLocaleIsCJK())
			{
				string text = "\t";
				string text2 = this.emailData[3];
				text2 = text2.Replace("。\n", text);
				text2 = text2.Replace("\n", "");
				text2 = text2.Replace(text, "。\n").Replace(text, "！\n").Replace(text, "!\n");
				text3 = Utils.SuperSmartTwimForWidth(LocalizedFileLoader.SafeFilterString(text2), bounds.Width - 50, GuiData.tinyfont);
				text3 = text3.Replace("\r\n\r\n\r\n", "\r\n\r\n");
			}
			else
			{
				text3 = Utils.SmartTwimForWidth(LocalizedFileLoader.SafeFilterString(this.emailData[3]), bounds.Width - 50, GuiData.tinyfont);
			}
			vector.Y += (float)this.DrawMailMessageText(textBounds, sb, text3.Split(Utils.newlineDelim));
			vector.Y += (float)(num - 5);
			int num3 = 0;
			for (int i = 4; i < this.emailData.Length; i++)
			{
				if (AttachmentRenderer.RenderAttachment(this.emailData[i], this.os, vector, num3, MailServer.buttonSound))
				{
					num3++;
					vector.Y += (float)num;
				}
			}
			vector.Y = (float)(bounds.Y + bounds.Height - 35);
			if (Button.doButton(90200, (int)vector.X, (int)vector.Y, 300, 30, LocaleTerms.Loc("Reply"), null))
			{
				this.emailReplyStrings.Clear();
				this.addingNewReplyString = false;
				this.missionIncompleteReply = false;
				this.state = 5;
			}
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00056138 File Offset: 0x00054338
		private void DrawButtonGlow(Vector2 dpos, Vector2 labelSize)
		{
			Rectangle rectangle = new Rectangle((int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17);
			float num = Utils.QuadraticOutCurve(1f - this.os.timer % 1f);
			float num2 = 8.5f;
			Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, (int)(-1f * (num2 * (1f - num))));
			GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * num * 0.32f);
			GuiData.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x000561F0 File Offset: 0x000543F0
		public virtual void doInboxHeader(Rectangle bounds, SpriteBatch sb)
		{
			Vector2 vector = new Vector2((float)(bounds.X + 2), (float)(bounds.Y + 20));
			string text = string.Format(LocaleTerms.Loc("Logged in as {0}"), this.user.name);
			sb.DrawString(GuiData.font, text, vector, this.textColor);
			vector.Y += 26f;
			if (Button.doButton(800007, bounds.X + bounds.Width - 95, (int)vector.Y, 90, 30, LocaleTerms.Loc("Logout"), new Color?(this.themeColor)))
			{
				this.state = 0;
			}
			if (this.totalPagesDetected > 0)
			{
				int num = 100;
				int num2 = 20;
				vector.Y = (float)(bounds.Y + 91 - (num2 + 4));
				if (this.inboxPage < this.totalPagesDetected && Button.doButton(801008, (int)vector.X, (int)vector.Y, num, num2, "<", null))
				{
					this.inboxPage++;
				}
				vector.X += (float)(num + 2);
				Vector2 vector2 = TextItem.doMeasuredFontLabel(vector, this.inboxPage + 1 + " / " + (this.totalPagesDetected + 1), GuiData.tinyfont, new Color?(Utils.AddativeWhite), float.MaxValue, float.MaxValue);
				vector.X += vector2.X + 4f;
				if (this.inboxPage > 0 && Button.doButton(801009, (int)vector.X, (int)vector.Y, num, num2, ">", null))
				{
					this.inboxPage--;
				}
			}
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x000563F8 File Offset: 0x000545F8
		public void doInboxDisplay(Rectangle bounds, SpriteBatch sb)
		{
			int num = 24;
			this.doInboxHeader(bounds, sb);
			Vector2 value = new Vector2((float)(bounds.X + 2), (float)(bounds.Y + 91));
			Folder folder = this.userFolder.folders[0];
			Rectangle destinationRectangle = new Rectangle(bounds.X + 2, (int)value.Y, bounds.Width - 4, num);
			Button.outlineOnly = true;
			int num2 = 0;
			while (destinationRectangle.Y + destinationRectangle.Height < bounds.Y + bounds.Height - 2)
			{
				sb.Draw(Utils.white, destinationRectangle, (num2 % 2 == 0) ? this.evenLine : this.oddLine);
				num2++;
				int height = destinationRectangle.Height;
				destinationRectangle.Height = 1;
				if (num2 > 1)
				{
					sb.Draw(Utils.white, destinationRectangle, this.seperatorLineColor);
				}
				destinationRectangle.Height = height;
				destinationRectangle.Y += num;
			}
			Rectangle tmpRect = GuiData.tmpRect;
			this.drawBackingGradient(bounds, sb);
			tmpRect.X = bounds.X + 1;
			tmpRect.Y = (int)value.Y - 3;
			tmpRect.Width = bounds.Width - 4;
			tmpRect.Height = 3;
			sb.Draw(Utils.white, tmpRect, this.themeColor);
			tmpRect.X += tmpRect.Width;
			tmpRect.Width = 3;
			sb.Draw(Utils.white, tmpRect, this.themeColor);
			tmpRect.X = destinationRectangle.X;
			tmpRect.Y = (int)value.Y;
			tmpRect.Width = 160;
			tmpRect.Height = bounds.Height - (tmpRect.Y - bounds.Y) - 1;
			sb.Draw(Utils.white, tmpRect, this.senderDarkeningColor);
			tmpRect.X += tmpRect.Width;
			tmpRect.Width = 3;
			sb.Draw(Utils.white, tmpRect, this.themeColor);
			tmpRect.X = destinationRectangle.X;
			tmpRect.Y = (int)value.Y;
			tmpRect.Width = 160;
			tmpRect.Height = bounds.Height - (tmpRect.Y - bounds.Y) - 1;
			destinationRectangle.Y = (int)value.Y;
			TextItem.DrawShadow = false;
			float num3 = (float)(bounds.Height - 2);
			int num4 = num2;
			int num5 = Math.Max(0, num4 * this.inboxPage - 1);
			this.totalPagesDetected = (int)((float)folder.files.Count / (float)num4);
			int num6 = Math.Min(num4, folder.files.Count - num5);
			for (int i = num5; i < num5 + num6; i++)
			{
				try
				{
					string[] array = folder.files[i].data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
					byte b = Convert.ToByte(array[0]);
					int num7 = 8100 + i;
					if (GuiData.hot == num7)
					{
						Rectangle destinationRectangle2 = new Rectangle(bounds.X + 2, destinationRectangle.Y + 1, bounds.Width - 4, num - 2);
						sb.Draw(Utils.white, destinationRectangle2, (i % 2 == 0) ? (Color.White * 0.07f) : (Color.Black * 0.2f));
					}
					if (Button.doButton(num7, bounds.X + 2, destinationRectangle.Y + 1, bounds.Width - 4, num - 2, "", new Color?(Color.Transparent)))
					{
						this.state = 4;
						this.selectedEmail = folder.files[i];
						this.emailData = this.selectedEmail.data.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
						this.selectedEmail.data = "1" + this.selectedEmail.data.Substring(1);
						if (this.sectionedPanel != null)
						{
							this.sectionedPanel.ScrollDown = 0f;
						}
					}
					if (b == 0)
					{
						sb.Draw(this.unopenedIcon, value + Vector2.One, this.themeColor);
					}
					TextItem.doFontLabel(value + new Vector2((float)(this.unopenedIcon.Width + 1), 2f), array[1], GuiData.tinyfont, new Color?(this.textColor), (float)(tmpRect.Width - this.unopenedIcon.Width - 3), (float)num, false);
					TextItem.doFontLabel(value + new Vector2((float)(tmpRect.Width + 10), 2f), array[2], GuiData.tinyfont, new Color?(this.textColor), (float)(bounds.Width - tmpRect.Width - 20), (float)num, false);
					value.Y += (float)num;
					destinationRectangle.Y = (int)value.Y;
				}
				catch (FormatException)
				{
				}
			}
			Button.outlineOnly = false;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x00056970 File Offset: 0x00054B70
		public void doRespondDisplay(Rectangle bounds, SpriteBatch sb)
		{
			Vector2 vector = new Vector2((float)(bounds.X + 2), (float)(bounds.Y + 20));
			string text = null;
			int width = bounds.Width - 20 - this.corner.Width;
			if (Button.doButton(800007, (int)vector.X, (int)vector.Y, width, 30, LocaleTerms.Loc("Return to Inbox"), new Color?(this.os.darkBackgroundColor)))
			{
				this.state = 3;
			}
			vector.Y += 50f;
			int num = 24;
			TextItem.doFontLabel(vector, LocaleTerms.Loc("Additional Details") + " :", GuiData.smallfont, null, (float)bounds.Width - (vector.X - (float)bounds.Width) * 1.2f, float.MaxValue, false);
			vector.Y += (float)num;
			for (int i = 0; i < this.emailReplyStrings.Count; i++)
			{
				TextItem.doFontLabel(vector + new Vector2(25f, 0f), this.emailReplyStrings[i], GuiData.tinyfont, null, (float)bounds.Width - (vector.X - (float)bounds.X) * 2f - 20f, float.MaxValue, false);
				float num2 = Math.Min(GuiData.tinyfont.MeasureString(this.emailReplyStrings[i]).X, (float)bounds.Width - (vector.X - (float)bounds.X) * 2f - 20f);
				if (Button.doButton(80000 + i * 100, (int)(vector.X + num2 + 30f), (int)vector.Y, 20, 20, "-", null))
				{
					this.emailReplyStrings.RemoveAt(i);
				}
				vector.Y += (float)num;
			}
			if (this.addingNewReplyString)
			{
				string text2 = null;
				bool flag = Programs.parseStringFromGetStringCommand(this.os, out text2);
				if (text2 == null)
				{
					text2 = "";
				}
				vector.Y += 5f;
				GuiData.spriteBatch.Draw(Utils.white, new Rectangle(bounds.X + 1, (int)vector.Y, bounds.Width - 2 - bounds.Width / 9, 40), this.os.indentBackgroundColor);
				vector.Y += 10f;
				TextItem.doFontLabel(vector + new Vector2(25f, 0f), text2, GuiData.tinyfont, null, float.MaxValue, float.MaxValue, false);
				Vector2 vector2 = GuiData.tinyfont.MeasureString(text2);
				vector2.Y = 0f;
				if (this.os.timer % 1f <= 0.5f)
				{
					GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int)(vector.X + vector2.X + 2f) + 25, (int)vector.Y, 4, 20), Color.White);
				}
				int num3 = bounds.Width - 1 - bounds.Width / 10;
				if (flag || Button.doButton(8000094, bounds.X + num3 - 4, (int)vector.Y - 10, bounds.Width / 9 - 3, 40, LocaleTerms.Loc("Add"), new Color?(this.os.highlightColor)))
				{
					if (!flag)
					{
						this.os.terminal.executeLine();
					}
					this.addingNewReplyString = false;
					this.emailReplyStrings.Add(text2);
					text = null;
				}
				else
				{
					text = text2;
				}
			}
			else if (Button.doButton(8000098, (int)(vector.X + 25f), (int)vector.Y, 20, 20, "+", null))
			{
				this.addingNewReplyString = true;
				this.os.execute("getString Detail");
				this.os.terminal.executionPreventionIsInteruptable = true;
			}
			vector.Y += 50f;
			if (Button.doButton(800008, (int)vector.X, (int)vector.Y, width, 30, LocaleTerms.Loc("Send"), null))
			{
				if (this.os.currentMission != null)
				{
					if (text != null)
					{
						this.os.terminal.executeLine();
						this.addingNewReplyString = false;
						if (!string.IsNullOrEmpty(text))
						{
							this.emailReplyStrings.Add(text);
						}
					}
					ActiveMission currentMission = this.os.currentMission;
					bool flag2 = this.attemptCompleteMission(this.os.currentMission);
					if (!flag2)
					{
						int i = 0;
						while (i < this.os.branchMissions.Count && !flag2)
						{
							flag2 = this.attemptCompleteMission(this.os.branchMissions[i]);
							if (flag2)
							{
								this.os.branchMissions.Clear();
							}
							i++;
						}
					}
					if (!flag2)
					{
						this.missionIncompleteReply = true;
					}
					else
					{
						this.AddSentEmailRecordFileForMissionCompletion(currentMission, this.emailReplyStrings);
					}
				}
			}
			vector.Y += 45f;
			if (Settings.forceCompleteEnabled && Button.doButton(800009, (int)vector.X, (int)vector.Y, width, 30, LocaleTerms.Loc("Force Complete"), null))
			{
				if (this.os.currentMission != null)
				{
					this.os.currentMission.finish();
					this.os.MissionCompleteFlashTime = 3f;
				}
				this.state = 3;
			}
			vector.Y += 70f;
			if (this.missionIncompleteReply)
			{
				if (this.comp.idName == "jmail")
				{
					Rectangle dest = new Rectangle(bounds.X + 2, (int)vector.Y, bounds.Width - 4, 128);
					PatternDrawer.draw(dest, 1f, this.os.lockedColor * 0.1f, this.os.brightLockedColor, sb, PatternDrawer.errorTile);
					string text3 = LocaleTerms.Loc("Mission Incomplete");
					Vector2 vector3 = GuiData.font.MeasureString(text3);
					TextItem.doLabel(new Vector2((float)(bounds.X + bounds.Width / 2) - vector3.X / 2f, vector.Y + 40f), text3, null);
				}
			}
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00057114 File Offset: 0x00055314
		private void AddSentEmailRecordFileForMissionCompletion(ActiveMission mission, List<string> additionalDetails)
		{
			try
			{
				Folder folder = this.userFolder.searchForFolder("sent");
				string text = mission.email.subject + " completed.";
				if (additionalDetails != null && additionalDetails.Count > 0)
				{
					text += "\nRequested Details:\n";
					for (int i = 0; i < additionalDetails.Count; i++)
					{
						text = text + additionalDetails[i] + "\n";
					}
				}
				string text2 = MailServer.generateEmail(mission.email.subject + " : Complete", text, this.user.name + "@" + this.comp.name);
				string subject = MailServer.getSubject(text2);
				FileEntry item = new FileEntry(text2, subject);
				folder.files.Add(item);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x00057214 File Offset: 0x00055414
		private bool attemptCompleteMission(ActiveMission mission)
		{
			if (mission.isComplete(this.emailReplyStrings))
			{
				if (mission.ShouldIgnoreSenderVerification || mission.email.sender == this.emailData[1])
				{
					mission.finish();
					this.state = 3;
					this.os.MissionCompleteFlashTime = 3f;
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x0005728C File Offset: 0x0005548C
		public void doLoginDisplay(Rectangle bounds, SpriteBatch sb)
		{
			int num = bounds.X + 20;
			int num2 = bounds.Y + 100;
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			string[] array = this.os.displayCache.Split(separator, StringSplitOptions.None);
			string text = "";
			string text2 = "";
			int num3 = -1;
			int num4 = 0;
			if (array[0].Equals("loginData"))
			{
				if (array[1] != "")
				{
					text = array[1];
				}
				else
				{
					text = this.os.terminal.currentLine;
				}
				if (array.Length > 2)
				{
					num4 = 1;
					text2 = array[2];
					if (text2.Equals(""))
					{
						for (int i = 0; i < this.os.terminal.currentLine.Length; i++)
						{
							text2 += "*";
						}
					}
					else
					{
						string text3 = "";
						for (int i = 0; i < text2.Length; i++)
						{
							text3 += "*";
						}
						text2 = text3;
					}
				}
				if (array.Length > 3)
				{
					num4 = 2;
					num3 = Convert.ToInt32(array[3]);
				}
			}
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = bounds.X + 2;
			tmpRect.Y = num2;
			tmpRect.Height = 200;
			tmpRect.Width = bounds.Width - 4;
			sb.Draw(Utils.white, tmpRect, (num3 == 0) ? this.os.lockedColor : this.os.indentBackgroundColor);
			if (num3 != 0)
			{
				if (num3 != -1)
				{
					for (int i = 0; i < this.comp.users.Count; i++)
					{
						if (this.comp.users[i].name.Equals(text))
						{
							this.user = this.comp.users[i];
							for (int j = 0; j < this.accounts.folders.Count; j++)
							{
								if (this.accounts.folders[j].name.Equals(this.user.name))
								{
									this.userFolder = this.accounts.folders[j];
									break;
								}
							}
							break;
						}
					}
					this.state = 3;
				}
			}
			tmpRect.Height = 22;
			num2 += 30;
			Vector2 vector = TextItem.doMeasuredLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Login") + " ", new Color?(this.textColor));
			if (num3 == 0)
			{
				num += (int)vector.X;
				TextItem.doLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Failed"), new Color?(this.os.brightLockedColor));
				num -= (int)vector.X;
			}
			num2 += 60;
			if (num4 == 0)
			{
				tmpRect.Y = num2;
				sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
			}
			sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("username") + " :", new Vector2((float)num, (float)num2), this.textColor);
			num += 100;
			sb.DrawString(GuiData.smallfont, text, new Vector2((float)num, (float)num2), this.textColor);
			num -= 100;
			num2 += 30;
			if (num4 == 1)
			{
				tmpRect.Y = num2;
				sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
			}
			sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("password") + " :", new Vector2((float)num, (float)num2), this.textColor);
			num += 100;
			sb.DrawString(GuiData.smallfont, text2, new Vector2((float)num, (float)num2), this.textColor);
			num2 += 30;
			num -= 100;
			if (num3 != -1)
			{
				if (Button.doButton(12345, num, num2, 70, 30, LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
				{
					this.state = 0;
				}
				if (Button.doButton(123456, num + 75, num2, 70, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
				{
					this.os.displayCache = "";
					this.os.execute("login");
					while (this.os.displayCache.Equals(""))
					{
					}
					this.os.display.command = this.name;
				}
			}
			else
			{
				num2 += 65;
				for (int i = 0; i < this.comp.users.Count; i++)
				{
					if (this.comp.users[i].known && MailServer.validUser(this.comp.users[i].type))
					{
						if (Button.doButton(123457 + i, num, num2, 300, 25, "User: " + this.comp.users[i].name + " Pass: " + this.comp.users[i].pass, new Color?(this.os.darkBackgroundColor)))
						{
							this.forceLogin(this.comp.users[i].name, this.comp.users[i].pass);
						}
						num2 += 27;
					}
				}
			}
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00057920 File Offset: 0x00055B20
		public void forceLogin(string username, string pass)
		{
			string prompt = this.os.terminal.prompt;
			this.os.terminal.currentLine = username;
			this.os.terminal.executeLine();
			while (this.os.terminal.prompt.Equals(prompt))
			{
			}
			this.os.terminal.currentLine = pass;
			this.os.terminal.executeLine();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x000579A0 File Offset: 0x00055BA0
		public new static bool validUser(byte type)
		{
			return Daemon.validUser(type) || type == 2;
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x000579C4 File Offset: 0x00055BC4
		public static string generateEmail(string subject, string body, string sender)
		{
			string str = "0" + MailServer.emailSplitDelimiter;
			str = str + MailServer.cleanString(sender) + MailServer.emailSplitDelimiter;
			str = str + MailServer.cleanString(subject) + MailServer.emailSplitDelimiter;
			return str + MailServer.minimalCleanString(body);
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00057A20 File Offset: 0x00055C20
		public static string generateEmail(string subject, string body, string sender, List<string> attachments)
		{
			string text = MailServer.generateEmail(subject, body, sender);
			text += MailServer.emailSplitDelimiter;
			for (int i = 0; i < attachments.Count; i++)
			{
				text = text + attachments[i] + MailServer.emailSplitDelimiter;
			}
			return text;
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x00057A74 File Offset: 0x00055C74
		public static string getSubject(string mail)
		{
			string[] array = mail.Split(MailServer.emailSplitDelims, StringSplitOptions.None);
			return array[2].Replace(' ', '_');
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x00057AA0 File Offset: 0x00055CA0
		public static string cleanString(string s)
		{
			return s.Replace('\n', '_').Replace('\r', '_').Replace(MailServer.emailSplitDelimiter, "_");
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00057AD4 File Offset: 0x00055CD4
		public static string minimalCleanString(string s)
		{
			return s.Replace('\t', ' ');
		}

		// Token: 0x04000625 RID: 1573
		public const int TYPE = 0;

		// Token: 0x04000626 RID: 1574
		public const int SENDER = 1;

		// Token: 0x04000627 RID: 1575
		public const int SUBJECT = 2;

		// Token: 0x04000628 RID: 1576
		public const int BODY = 3;

		// Token: 0x04000629 RID: 1577
		public const int ATTACHMENT = 4;

		// Token: 0x0400062A RID: 1578
		public const int UNREAD = 0;

		// Token: 0x0400062B RID: 1579
		public const int READ = 1;

		// Token: 0x0400062C RID: 1580
		public const float MISSION_COMPLETE_FLASH_TIME = 3f;

		// Token: 0x0400062D RID: 1581
		public static bool shouldGenerateJunk = true;

		// Token: 0x0400062E RID: 1582
		private Folder root;

		// Token: 0x0400062F RID: 1583
		private Folder accounts;

		// Token: 0x04000630 RID: 1584
		private Folder userFolder;

		// Token: 0x04000631 RID: 1585
		private List<int> rootPath;

		// Token: 0x04000632 RID: 1586
		private List<int> accountsPath;

		// Token: 0x04000633 RID: 1587
		private UserDetail user;

		// Token: 0x04000634 RID: 1588
		private FileEntry selectedEmail;

		// Token: 0x04000635 RID: 1589
		private int state;

		// Token: 0x04000636 RID: 1590
		private int inboxPage = 0;

		// Token: 0x04000637 RID: 1591
		private int totalPagesDetected = -1;

		// Token: 0x04000638 RID: 1592
		private static string emailSplitDelimiter = "@*&^#%@)_!_)*#^@!&*)(#^&\n";

		// Token: 0x04000639 RID: 1593
		private static string[] emailSplitDelims = new string[]
		{
			MailServer.emailSplitDelimiter
		};

		// Token: 0x0400063A RID: 1594
		private static string[] spaceDelim = new string[]
		{
			"#%#"
		};

		// Token: 0x0400063B RID: 1595
		private Color themeColor = new Color(125, 5, 6);

		// Token: 0x0400063C RID: 1596
		public Color textColor;

		// Token: 0x0400063D RID: 1597
		private string[] emailData;

		// Token: 0x0400063E RID: 1598
		public bool shouldGenerateJunkEmails = true;

		// Token: 0x0400063F RID: 1599
		public Color evenLine;

		// Token: 0x04000640 RID: 1600
		public Color oddLine;

		// Token: 0x04000641 RID: 1601
		public Color senderDarkeningColor;

		// Token: 0x04000642 RID: 1602
		public Color seperatorLineColor;

		// Token: 0x04000643 RID: 1603
		private List<MailResponder> responders;

		// Token: 0x04000644 RID: 1604
		public Texture2D panel;

		// Token: 0x04000645 RID: 1605
		public Texture2D corner;

		// Token: 0x04000646 RID: 1606
		public Texture2D unopenedIcon;

		// Token: 0x04000647 RID: 1607
		private Rectangle panelRect;

		// Token: 0x04000648 RID: 1608
		private bool missionIncompleteReply;

		// Token: 0x04000649 RID: 1609
		private static SoundEffect buttonSound;

		// Token: 0x0400064A RID: 1610
		private ScrollableSectionedPanel sectionedPanel;

		// Token: 0x0400064B RID: 1611
		public Action setupComplete;

		// Token: 0x0400064C RID: 1612
		private List<string> emailReplyStrings = new List<string>();

		// Token: 0x0400064D RID: 1613
		private bool addingNewReplyString = false;

		// Token: 0x020000FD RID: 253
		public struct EMailData
		{
			// Token: 0x0600058C RID: 1420 RVA: 0x00057B37 File Offset: 0x00055D37
			public EMailData(string sendr, string bod, string subj, List<string> _attachments)
			{
				this.sender = sendr;
				this.body = bod;
				this.subject = subj;
				this.attachments = _attachments;
			}

			// Token: 0x0400064E RID: 1614
			public string sender;

			// Token: 0x0400064F RID: 1615
			public string body;

			// Token: 0x04000650 RID: 1616
			public string subject;

			// Token: 0x04000651 RID: 1617
			public List<string> attachments;
		}
	}
}
