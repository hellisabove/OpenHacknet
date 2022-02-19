using System;
using System.Collections.Generic;
using System.Text;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Localization;
using Hacknet.Modules.Helpers;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200010A RID: 266
	internal class DisplayModule : CoreModule
	{
		// Token: 0x0600063D RID: 1597 RVA: 0x0006627C File Offset: 0x0006447C
		public DisplayModule(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.catTextRegion = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x000662FC File Offset: 0x000644FC
		public override void LoadContent()
		{
			this.computers = new List<Texture2D>();
			this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec0Computer", this.os.content));
			this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec1Computer", this.os.content));
			this.computers.Add(TextureBank.load("Sprites/CompLogos/Computer", this.os.content));
			this.computers.Add(TextureBank.load("Sprites/CompLogos/OldServer", this.os.content));
			this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec2Computer", this.os.content));
			this.computers.Add(TextureBank.load("Sprites/CompLogos/Sec2Computer", this.os.content));
			this.compAltIcons = new Dictionary<string, Texture2D>();
			this.compAltIcons.Add("laptop", TextureBank.load("Sprites/CompLogos/Laptop", this.os.content));
			this.compAltIcons.Add("chip", TextureBank.load("Sprites/CompLogos/Chip", this.os.content));
			this.compAltIcons.Add("kellis", TextureBank.load("Sprites/CompLogos/KellisCompIcon", this.os.content));
			this.compAltIcons.Add("tablet", TextureBank.load("Sprites/CompLogos/Tablet", this.os.content));
			this.compAltIcons.Add("ePhone", TextureBank.load("Sprites/CompLogos/Phone1", this.os.content));
			this.compAltIcons.Add("ePhone2", TextureBank.load("Sprites/CompLogos/Phone2", this.os.content));
			if (DLC1SessionUpgrader.HasDLC1Installed)
			{
				this.compAltIcons.Add("Psylance", TextureBank.load("DLC/Sprites/Psylance", this.os.content));
				this.compAltIcons.Add("PacificAir", TextureBank.load("DLC/Sprites/PacificAir", this.os.content));
				this.compAltIcons.Add("Alchemist", TextureBank.load("DLC/Sprites/AlchemistsIcon", this.os.content));
				this.compAltIcons.Add("DLCLaptop", TextureBank.load("DLC/Icons/Laptop", this.os.content));
				this.compAltIcons.Add("DLCPC1", TextureBank.load("DLC/Icons/PC1", this.os.content));
				this.compAltIcons.Add("DLCPC2", TextureBank.load("DLC/Icons/PC2", this.os.content));
				this.compAltIcons.Add("DLCServer", TextureBank.load("DLC/Icons/Server", this.os.content));
			}
			this.defaultComputer = TextureBank.load("Sprites/CompLogos/Computer", this.os.content);
			this.lockSprite = TextureBank.load("Lock", this.os.content);
			this.openLockSprite = TextureBank.load("OpenLock", this.os.content);
			this.fancyCornerSprite = TextureBank.load("Corner", this.os.content);
			this.fancyPanelSprite = TextureBank.load("Panel", this.os.content);
			this.tmpRect = default(Rectangle);
			this.scroll = Vector2.Zero;
			this.catScroll = Vector2.Zero;
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0006668E File Offset: 0x0006488E
		public override void Update(float t)
		{
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x00066694 File Offset: 0x00064894
		public override void Draw(float t)
		{
			base.Draw(t);
			try
			{
				this.doCommandModule();
				this.errorCount = 0;
			}
			catch (Exception ex)
			{
				string text = Utils.GenerateReportFromException(ex);
				Console.WriteLine(text);
				if (OS.DEBUG_COMMANDS)
				{
					this.os.write("ERROR RENDERING DAEMON: " + this.command);
					this.os.write(Utils.GenerateReportFromExceptionCompact(ex));
				}
				this.errorCount++;
				if (this.errorCount >= 3)
				{
					if (!this.hasSentErrorEmail)
					{
						Utils.SendThreadedErrorReport(ex, LocaleTerms.Loc("Display module Crash: ") + this.command, DebugLog.GetDump());
						this.hasSentErrorEmail = true;
					}
					this.command = "connect";
					this.commandArgs = new string[1];
					this.commandArgs[0] = "connect";
					this.errorCount = 0;
				}
				else
				{
					Utils.AppendToErrorFile(text);
				}
			}
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x000667A8 File Offset: 0x000649A8
		public void typeChanged()
		{
			this.scroll = Vector2.Zero;
			this.catScroll = Vector2.Zero;
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x000667F4 File Offset: 0x000649F4
		private void doCommandModule()
		{
			this.x = this.bounds.X + 5;
			this.y = this.bounds.Y + 5;
			this.spriteBatch.Draw(Utils.white, this.bounds, this.os.displayModuleExtraLayerBackingColor);
			if (!this.command.Equals("login"))
			{
				this.lockLoginDisplayCache = false;
				this.loginDetailsCache = null;
			}
			for (int i = 0; i < this.os.exes.Count; i++)
			{
				MainDisplayOverrideEXE mainDisplayOverrideEXE = this.os.exes[i] as MainDisplayOverrideEXE;
				if (mainDisplayOverrideEXE != null && mainDisplayOverrideEXE.DisplayOverrideIsActive && !this.os.exes[i].isExiting)
				{
					mainDisplayOverrideEXE.RenderMainDisplay(this.bounds, this.spriteBatch);
					return;
				}
			}
			if (this.os.connectedComp != null && this.os.connectedComp.getDaemon(typeof(PorthackHeartDaemon)) is PorthackHeartDaemon)
			{
				this.doDaemonDisplay();
				return;
			}
			if (this.command.Equals("ls") || this.command.Equals("dir") || this.command.Equals("cd"))
			{
				this.doLsDisplay();
				return;
			}
			if (this.command.Equals("connect"))
			{
				this.doConnectDisplay();
				this.invioableSecurityCacheString = null;
				return;
			}
			if (this.command.Equals("cat") || this.command.Equals("less"))
			{
				this.doCatDisplay();
				return;
			}
			if (this.command.ToLower().Equals("probe") || this.command.Equals("nmap"))
			{
				this.doProbeDisplay();
				return;
			}
			if (this.command.Equals("dc") || this.command.Equals("disconnect") || this.command.Equals("crash"))
			{
				this.doDisconnectDisplay();
				return;
			}
			if (this.command.Equals("login"))
			{
				this.doLoginDisplay();
				return;
			}
			if (this.command.Equals("connectiondenied"))
			{
				this.doDisconnectForcedDisplay();
				return;
			}
			this.doDaemonDisplay();
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00066AB8 File Offset: 0x00064CB8
		private void doDaemonDisplay()
		{
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			for (int i = 0; i < computer.daemons.Count; i++)
			{
				if (computer.daemons[i].name.Equals(this.command) || computer.daemons[i] is PorthackHeartDaemon)
				{
					computer.daemons[i].draw(this.bounds, this.spriteBatch);
					break;
				}
			}
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00066B68 File Offset: 0x00064D68
		private void doLoginDisplay()
		{
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			if (!this.lockLoginDisplayCache)
			{
				this.loginDetailsCache = this.os.displayCache;
			}
			string[] array = this.loginDetailsCache.Split(separator, StringSplitOptions.None);
			string text = "";
			string text2 = "";
			int num = -1;
			int num2 = 0;
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
					num2 = 1;
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
					num2 = 2;
					num = Convert.ToInt32(array[3]);
				}
			}
			this.doConnectHeader();
			Rectangle destinationRectangle = GuiData.tmpRect;
			destinationRectangle.X = this.bounds.X + 2;
			destinationRectangle.Y = this.y;
			destinationRectangle.Height = 200;
			destinationRectangle.Width = this.bounds.Width - 4;
			this.spriteBatch.Draw(Utils.white, destinationRectangle, (num == 0) ? this.os.lockedColor : this.os.indentBackgroundColor);
			if (num == 0)
			{
			}
			destinationRectangle.Height = 22;
			this.y += 30;
			Vector2 vector = TextItem.doMeasuredLabel(new Vector2((float)this.x, (float)this.y), LocaleTerms.Loc("Login "), new Color?(Color.White));
			if (num == 0)
			{
				this.x += (int)vector.X;
				TextItem.doLabel(new Vector2((float)this.x, (float)this.y), LocaleTerms.Loc("Failed"), new Color?(this.os.brightLockedColor));
				this.x -= (int)vector.X;
				this.lockLoginDisplayCache = true;
			}
			else if (num != -1)
			{
				this.x += (int)vector.X;
				TextItem.doLabel(new Vector2((float)this.x, (float)this.y), LocaleTerms.Loc("Successful"), new Color?(this.os.brightUnlockedColor));
				this.x -= (int)vector.X;
				this.lockLoginDisplayCache = true;
			}
			this.y += 60;
			if (num2 == 0)
			{
				destinationRectangle.Y = this.y;
				this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.subtleTextColor);
			}
			int num3 = 100;
			string text4 = LocaleTerms.Loc("username :");
			string text5 = LocaleTerms.Loc("password :");
			num3 = (int)Math.Max(Math.Max(GuiData.smallfont.MeasureString(text4).X + 4f, GuiData.smallfont.MeasureString(text5).X + 4f), (float)num3);
			this.spriteBatch.DrawString(GuiData.smallfont, text4, new Vector2((float)this.x, (float)this.y), Color.White);
			this.x += num3;
			this.spriteBatch.DrawString(GuiData.smallfont, text, new Vector2((float)this.x, (float)this.y), Color.White);
			this.x -= num3;
			this.y += 30;
			if (num2 == 1)
			{
				destinationRectangle.Y = this.y;
				this.spriteBatch.Draw(Utils.white, destinationRectangle, this.os.subtleTextColor);
			}
			this.spriteBatch.DrawString(GuiData.smallfont, text5, new Vector2((float)this.x, (float)this.y), Color.White);
			this.x += num3;
			this.spriteBatch.DrawString(GuiData.smallfont, text2, new Vector2((float)this.x, (float)this.y), Color.White);
			this.y += 30;
			this.x -= num3;
			if (num != -1)
			{
				int num4 = Math.Min(this.bounds.Width / 3, 180);
				if (Button.doButton(12345, this.x, this.y, (num > 0) ? 140 : num4, 30, (num > 0) ? LocaleTerms.Loc("Complete") : LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
				{
					this.command = "connect";
				}
				if (num <= 0)
				{
					if (Button.doButton(123456, this.x + num4 + 5, this.y, num4, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
					{
						this.lockLoginDisplayCache = false;
						this.loginDetailsCache = null;
						this.os.runCommand("login");
					}
				}
			}
			else
			{
				this.y += 65;
				int num5 = this.x;
				int num6 = this.y;
				Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
				for (int i = 0; i < computer.users.Count; i++)
				{
					if (computer.users[i].known && Daemon.validUser(computer.users[i].type))
					{
						num5 = this.x + 320;
						if (Button.doButton(123457 + i, this.x, this.y, 300, 25, "Login - User: " + computer.users[i].name + " Pass: " + computer.users[i].pass, new Color?(this.os.darkBackgroundColor)))
						{
							this.forceLogin(computer.users[i].name, computer.users[i].pass);
						}
						this.y += 27;
					}
				}
				if (Button.doButton(2111844, num5, num6, 100, 25, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
				{
					this.forceLogin("", "");
					this.command = "connect";
				}
			}
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x0006733C File Offset: 0x0006553C
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

		// Token: 0x06000646 RID: 1606 RVA: 0x000673BC File Offset: 0x000655BC
		public Texture2D GetComputerImage(Computer comp)
		{
			Texture2D result;
			if (comp.icon == null || !this.compAltIcons.ContainsKey(comp.icon))
			{
				result = ((comp.securityLevel >= this.computers.Count) ? this.defaultComputer : this.computers[comp.securityLevel]);
			}
			else
			{
				result = this.compAltIcons[comp.icon];
			}
			return result;
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x00067464 File Offset: 0x00065664
		private void doConnectHeader()
		{
			Computer computer = (this.os.connectedComp != null) ? this.os.connectedComp : this.os.thisComputer;
			this.x += 20;
			this.y += 5;
			this.spriteBatch.Draw(this.GetComputerImage(computer), new Vector2((float)this.x, (float)this.y), Color.White);
			this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Connected to") + " ", new Vector2((float)(this.x + 160), (float)this.y), Color.White);
			this.y += 40;
			string text = (this.os.connectedComp == null) ? this.os.thisComputer.name : this.os.connectedComp.name;
			TextItem.doFontLabel(new Vector2((float)(this.x + 160), (float)this.y), text, GuiData.font, new Color?(Color.White), (float)this.bounds.Width - 190f, 60f, false);
			this.y += 33;
			text = ((this.os.connectedComp == null) ? this.os.thisComputer.ip : this.os.connectedComp.ip);
			float num = LocaleActivator.ActiveLocaleIsCJK() ? 4f : 0f;
			this.spriteBatch.DrawString(GuiData.smallfont, "@  " + text, new Vector2((float)(this.x + 160), (float)this.y + num), Color.White);
			this.y += 60;
			if (this.os.hasConnectionPermission(true))
			{
				DisplayModule.<>c__DisplayClass1 CS$<>8__locals1 = new DisplayModule.<>c__DisplayClass1();
				CS$<>8__locals1.<>4__this = this;
				this.y -= 20;
				Rectangle empty = Rectangle.Empty;
				empty.X = this.bounds.X + 1;
				empty.Y = this.y;
				empty.Width = this.bounds.Width - 2;
				empty.Height = 20;
				this.spriteBatch.Draw(Utils.white, empty, this.os.highlightColor);
				CS$<>8__locals1.text = LocaleTerms.Loc("You are the Administrator of this System");
				Vector2 vector = GuiData.UISmallfont.MeasureString(CS$<>8__locals1.text);
				CS$<>8__locals1.pos = new Vector2((float)(empty.X + empty.Width / 2) - vector.X / 2f, (float)empty.Y);
				if (LocaleActivator.ActiveLocaleIsCJK())
				{
					DisplayModule.<>c__DisplayClass1 CS$<>8__locals2 = CS$<>8__locals1;
					CS$<>8__locals2.pos.Y = CS$<>8__locals2.pos.Y - 2f;
				}
				OS os = this.os;
				os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
				{
					CS$<>8__locals1.<>4__this.spriteBatch.DrawString(GuiData.UISmallfont, CS$<>8__locals1.text, CS$<>8__locals1.pos, Color.Black);
				}));
				if (this.bounds.Height > 500)
				{
					this.y += 40;
				}
				else
				{
					this.y += 12;
				}
			}
			if (computer.portsNeededForCrack > 100)
			{
				this.y += 10;
				Rectangle rectangle = new Rectangle(this.bounds.X + 2, this.y, this.bounds.Width - 4, 20);
				string text2 = "INVIOLABILITY DETECTED";
				Vector2 vector2 = GuiData.titlefont.MeasureString(text2);
				float num2 = vector2.X / vector2.Y;
				int num3 = 10;
				int num4 = (int)((float)rectangle.Height * num2);
				vector2.Y /= vector2.Y / 20f;
				int num5 = (int)((float)(rectangle.Width - num4) / 2f);
				Rectangle destinationRectangle = new Rectangle(rectangle.X, rectangle.Y, num5 - num3, rectangle.Height);
				Rectangle dest = new Rectangle(destinationRectangle.X + destinationRectangle.Width + num3, destinationRectangle.Y, num4, rectangle.Height);
				destinationRectangle.Y += 4;
				destinationRectangle.Height -= 8;
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Red);
				FlickeringTextEffect.DrawLinedFlickeringText(dest, text2, 3f, 0.1f, GuiData.titlefont, this.os, Utils.AddativeWhite, 2);
				destinationRectangle.X = rectangle.X + rectangle.Width - (destinationRectangle.Width + num3) + num3;
				this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Red);
				this.y += 40;
			}
			else if (this.os.hasConnectionPermission(true))
			{
				this.y += 40;
			}
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x0006799C File Offset: 0x00065B9C
		private void doConnectDisplay()
		{
			this.doConnectHeader();
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			for (int i = 0; i < computer.daemons.Count; i++)
			{
				if (computer.daemons[i] is CustomConnectDisplayOverride)
				{
					Rectangle bounds = new Rectangle(this.bounds.X + 1, this.y, this.bounds.Width - 2, this.bounds.Height - (this.y - this.bounds.Y) - 1);
					computer.daemons[i].draw(bounds, this.spriteBatch);
					return;
				}
			}
			int num = computer.daemons.Count + 6;
			int num2 = 40;
			int num3 = this.bounds.Height - (this.y - this.bounds.Y) - 20;
			num3 -= num * 5;
			if ((double)num3 / (double)num < (double)num2)
			{
				num2 = (int)((double)num3 / (double)num);
			}
			for (int i = 0; i < computer.daemons.Count; i++)
			{
				if (Button.doButton(29000 + i, this.x, this.y, 300, num2, computer.daemons[i].name, new Color?(this.os.highlightColor)))
				{
					this.command = computer.daemons[i].name;
					computer.daemons[i].navigatedTo();
				}
				this.y += num2 + 5;
			}
			if (Button.doButton(300000, this.x, this.y, 300, num2, LocaleTerms.Loc("Login"), new Color?(this.os.hasConnectionPermission(true) ? this.os.subtleTextColor : this.os.highlightColor)))
			{
				this.os.runCommand("login");
				this.os.terminal.clearCurrentLine();
			}
			this.y += num2 + 5;
			if (Button.doButton(300002, this.x, this.y, 300, num2, LocaleTerms.Loc("Probe System"), new Color?(this.os.highlightColor)))
			{
				this.os.runCommand("probe");
			}
			this.y += num2 + 5;
			if (Button.doButton(300003, this.x, this.y, 300, num2, LocaleTerms.Loc("View Filesystem"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("ls");
			}
			this.y += num2 + 5;
			if (Button.doButton(300006, this.x, this.y, 300, num2, LocaleTerms.Loc("View Logs"), new Color?(this.os.hasConnectionPermission(false) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("cd log");
			}
			this.y += num2 + 5;
			if (Button.doButton(300009, this.x, this.y, 300, num2, LocaleTerms.Loc("Scan Network"), new Color?(this.os.hasConnectionPermission(true) ? this.os.highlightColor : this.os.subtleTextColor)))
			{
				this.os.runCommand("scan");
			}
			this.y = this.bounds.Y + this.bounds.Height - 30;
			if (Button.doButton(300012, this.x, this.y, 300, 20, LocaleTerms.Loc("Disconnect"), new Color?(this.os.lockedColor)))
			{
				this.os.runCommand("dc");
				return;
			}
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x00067E5C File Offset: 0x0006605C
		private void doDisconnectDisplay()
		{
			this.tmpRect.X = this.bounds.X + 2;
			this.tmpRect.Width = this.bounds.Width - 4;
			this.tmpRect.Y = this.bounds.Y + this.bounds.Height / 6 * 2;
			this.tmpRect.Height = this.bounds.Height / 3;
			this.spriteBatch.Draw(Utils.white, this.tmpRect, this.os.indentBackgroundColor);
			Vector2 vector = GuiData.font.MeasureString(LocaleTerms.Loc("Disconnected"));
			Vector2 position = new Vector2((float)(this.tmpRect.X + this.bounds.Width / 2) - vector.X / 2f, (float)(this.bounds.Y + this.bounds.Height / 2 - 10));
			this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Disconnected"), position, this.os.subtleTextColor);
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00067F84 File Offset: 0x00066184
		private void doDisconnectForcedDisplay()
		{
			this.tmpRect.X = this.bounds.X + 2;
			this.tmpRect.Width = this.bounds.Width - 4;
			this.tmpRect.Y = this.bounds.Y + this.bounds.Height / 6 * 2;
			this.tmpRect.Height = this.bounds.Height / 3;
			Rectangle rectangle = this.tmpRect;
			double num = Math.Abs(Math.Sin((double)this.os.timer));
			int num2 = (int)(num * 40.0);
			rectangle = Utils.InsetRectangle(rectangle, -1 * num2);
			rectangle.X = this.tmpRect.X;
			rectangle.Width = this.tmpRect.Width;
			this.spriteBatch.Draw(Utils.white, rectangle, Utils.AddativeRed * (float)(1.0 - num));
			this.spriteBatch.Draw(Utils.white, this.tmpRect, this.os.indentBackgroundColor);
			Vector2 vector = GuiData.font.MeasureString(LocaleTerms.Loc("Connection Denied by Remote Server"));
			Vector2 position = new Vector2((float)(this.tmpRect.X + this.bounds.Width / 2) - vector.X / 2f, (float)(this.bounds.Y + this.bounds.Height / 2 - 10));
			this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Connection Denied by Remote Server"), position, this.os.brightLockedColor);
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x000681B8 File Offset: 0x000663B8
		private void doCatDisplay()
		{
			if (this.os.hasConnectionPermission(false))
			{
				if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 41), this.bounds.Y + 12, 27, 29, "<-", null))
				{
					this.os.runCommand("ls");
				}
				Rectangle rectangle = GuiData.tmpRect;
				rectangle.Width = this.bounds.Width;
				rectangle.X = this.bounds.X;
				rectangle.Y = this.bounds.Y + 1;
				rectangle.Height = this.bounds.Height - 2;
				if (this.os.connectedComp != null && this.os.connectedComp.ip != this.LastDisplayedFileSourceIP && this.LastDisplayedFileSourceIP != this.os.thisComputer.ip)
				{
					this.command = "dc";
				}
				else
				{
					string text = "";
					for (int i = 1; i < this.commandArgs.Length; i++)
					{
						text = text + this.commandArgs[i] + " ";
					}
					text = LocalizedFileLoader.SafeFilterString(text);
					if (this.LastDisplayedFileFolder.searchForFile(text.Trim()) == null)
					{
						OS os = this.os;
						os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
						{
							Rectangle rectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - 70, this.bounds.Width - 2, 140);
							this.spriteBatch.Draw(Utils.white, rectangle2, this.os.lockedColor);
							TextItem.doCenteredFontLabel(rectangle2, "File Not Found", GuiData.font, Color.White, false);
						}));
						this.catScroll = Vector2.Zero;
					}
					else
					{
						TextItem.doFontLabel(new Vector2((float)this.x, (float)(this.y + 3)), text, GuiData.font, new Color?(Color.White), (float)(this.bounds.Width - 70), float.MaxValue, false);
						int num = 55;
						Rectangle dest = new Rectangle(rectangle.X + 4, rectangle.Y + num, rectangle.Width - 6, rectangle.Height - num - 2);
						string data = this.os.displayCache;
						this.y += 70;
						data = LocalizedFileLoader.SafeFilterString(data);
						string text2 = Utils.SuperSmartTwimForWidth(data, this.bounds.Width - 40, GuiData.tinyfont);
						this.catTextRegion.Draw(dest, text2, this.spriteBatch);
					}
				}
			}
			else
			{
				this.command = "connect";
			}
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x00068470 File Offset: 0x00066670
		private void doProbeDisplay()
		{
			Rectangle rectangle = Rectangle.Empty;
			Computer computer = (this.os.connectedComp == null) ? this.os.thisComputer : this.os.connectedComp;
			if (computer.proxyActive)
			{
				rectangle = this.bounds;
				rectangle.X++;
				rectangle.Y++;
				rectangle.Width -= 2;
				rectangle.Height -= 2;
				PatternDrawer.draw(rectangle, 0.8f, Color.Transparent, this.os.superLightWhite, this.os.ScreenManager.SpriteBatch);
			}
			if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 50), this.bounds.Y + this.y, 27, 27, "<-", null))
			{
				this.command = "connect";
			}
			this.spriteBatch.DrawString(GuiData.font, LocaleTerms.Loc("Open Ports"), new Vector2((float)this.x, (float)this.y), Color.White);
			this.y += 40;
			this.spriteBatch.DrawString(GuiData.smallfont, computer.name + " @" + computer.ip, new Vector2((float)this.x, (float)this.y), Color.White);
			this.y += 30;
			int num = Math.Max(computer.portsNeededForCrack + 1, 0);
			string text = string.Concat(num);
			bool flag = num > 100;
			if (flag)
			{
				if (this.invioableSecurityCacheString == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < text.Length; i++)
					{
						stringBuilder.Append(Utils.getRandomChar());
					}
					this.invioableSecurityCacheString = stringBuilder.ToString();
				}
				else
				{
					this.invioabilityCharChangeTimer -= (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
					if (this.invioabilityCharChangeTimer <= 0f)
					{
						StringBuilder stringBuilder2 = new StringBuilder(this.invioableSecurityCacheString);
						stringBuilder2[Utils.random.Next(stringBuilder2.Length)] = ((Utils.random.NextDouble() > 0.3) ? Utils.getRandomNumberChar() : Utils.getRandomChar());
						this.invioableSecurityCacheString = stringBuilder2.ToString();
						this.invioabilityCharChangeTimer = 0.025f;
					}
				}
				text = this.invioableSecurityCacheString;
			}
			this.spriteBatch.DrawString(GuiData.smallfont, LocalizedFileLoader.SafeFilterString(LocaleTerms.Loc("Open Ports Required for Crack:")) + " " + text, new Vector2((float)this.x, (float)this.y), flag ? Color.Lerp(Color.Red, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f) : this.os.highlightColor);
			this.y += 40;
			if (flag)
			{
				rectangle.X = this.bounds.X + 2;
				rectangle.Y = this.y;
				rectangle.Width = this.bounds.Width - 4;
				rectangle.Height = 110;
				this.DrawInvioabilityEffect(rectangle);
				this.y += rectangle.Height + 10;
			}
			if (computer.hasProxy)
			{
				rectangle.X = this.x;
				rectangle.Y = this.y;
				rectangle.Width = this.bounds.Width - 10;
				rectangle.Height = 40;
				PatternDrawer.draw(rectangle, 1f, computer.proxyActive ? this.os.topBarColor : Color.Lerp(this.os.unlockedColor, Color.Black, 0.2f), computer.proxyActive ? (this.os.shellColor * 0.3f) : this.os.unlockedColor, this.os.ScreenManager.SpriteBatch);
				if (computer.proxyActive)
				{
					rectangle.Width = (int)((float)rectangle.Width * (1f - computer.proxyOverloadTicks / computer.startingOverloadTicks));
					this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.5f);
				}
				this.spriteBatch.DrawString(GuiData.smallfont, computer.proxyActive ? LocaleTerms.Loc("Proxy Detected") : LocaleTerms.Loc("Proxy Bypassed"), new Vector2((float)(this.x + 4), (float)(this.y + 2)), Color.Black);
				this.spriteBatch.DrawString(GuiData.smallfont, computer.proxyActive ? LocaleTerms.Loc("Proxy Detected") : LocaleTerms.Loc("Proxy Bypassed"), new Vector2((float)(this.x + 3), (float)(this.y + 1)), computer.proxyActive ? Color.White : this.os.highlightColor);
				this.y += 60;
			}
			if (computer.firewall != null)
			{
				rectangle.X = this.x;
				rectangle.Y = this.y;
				rectangle.Width = this.bounds.Width - 10;
				rectangle.Height = 40;
				bool flag2 = !computer.firewall.solved;
				PatternDrawer.draw(rectangle, 1f, flag2 ? this.os.topBarColor : Color.Lerp(this.os.unlockedColor, Color.Black, 0.2f), flag2 ? (this.os.shellColor * 0.3f) : this.os.unlockedColor, this.os.ScreenManager.SpriteBatch);
				this.spriteBatch.DrawString(GuiData.smallfont, flag2 ? LocaleTerms.Loc("Firewall Detected") : LocaleTerms.Loc("Firewall Solved"), new Vector2((float)(this.x + 4), (float)(this.y + 2)), Color.Black);
				this.spriteBatch.DrawString(GuiData.smallfont, flag2 ? LocaleTerms.Loc("Firewall Detected") : LocaleTerms.Loc("Firewall Solved"), new Vector2((float)(this.x + 3), (float)(this.y + 1)), flag2 ? Color.White : this.os.highlightColor);
				this.y += 60;
			}
			Vector2 vector = Vector2.Zero;
			rectangle.X = this.x + 1;
			rectangle.Width = 420;
			rectangle.Height = 41;
			Vector2 position = new Vector2((float)(rectangle.X + rectangle.Width - 36), (float)(rectangle.Y + 7));
			this.x += 10;
			for (int i = 0; i < computer.ports.Count; i++)
			{
				rectangle.Y = this.y + 4;
				position.Y = (float)(rectangle.Y + 4);
				this.spriteBatch.Draw(Utils.white, rectangle, (computer.portsOpen[i] > 0) ? this.os.unlockedColor : this.os.lockedColor);
				this.spriteBatch.Draw((computer.portsOpen[i] > 0) ? this.openLockSprite : this.lockSprite, position, Color.White);
				string text2 = "Port#: " + computer.GetDisplayPortNumberFromCodePort(computer.ports[i]);
				vector = GuiData.font.MeasureString(text2);
				this.spriteBatch.DrawString(GuiData.font, text2, new Vector2((float)this.x, (float)(this.y + 3)), Color.White);
				string text3 = " - " + PortExploits.services[computer.ports[i]];
				Vector2 vector2 = GuiData.smallfont.MeasureString(text3);
				float num2 = (float)rectangle.Width - vector.X - 50f;
				float scale = Math.Min(1f, num2 / vector2.X);
				this.spriteBatch.DrawString(GuiData.smallfont, text3, new Vector2((float)this.x + vector.X, (float)(this.y + 4)), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.8f);
				this.y += 45;
			}
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x00068DB0 File Offset: 0x00066FB0
		private void DrawInvioabilityEffect(Rectangle dest)
		{
			Color color = Color.Lerp(this.os.lockedColor, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f);
			color.A = 0;
			Rectangle rectangle = new Rectangle(dest.X + this.fancyCornerSprite.Width, dest.Y, dest.Width - this.fancyCornerSprite.Width * 2, this.fancyPanelSprite.Height);
			this.spriteBatch.Draw(this.fancyPanelSprite, rectangle, color);
			this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float)dest.X, (float)dest.Y), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 0.4f);
			this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float)(rectangle.X + rectangle.Width), (float)dest.Y), color);
			int num = this.fancyCornerSprite.Width - 38;
			Rectangle dest2 = new Rectangle(dest.X + num, dest.Y + this.fancyCornerSprite.Height / 2, dest.Width - num * 2, dest.Height - this.fancyCornerSprite.Height);
			Color color2 = Color.Lerp(Utils.AddativeWhite, this.os.brightLockedColor, Utils.randm(0.5f) + 0.5f);
			FlickeringTextEffect.DrawLinedFlickeringText(dest2, "INVIOLABILITY ERROR", 4f, 0.26f, GuiData.titlefont, this.os, Utils.AddativeWhite, 2);
			Rectangle destinationRectangle = rectangle;
			destinationRectangle.Y = dest.Y + dest.Height - this.fancyPanelSprite.Height;
			this.spriteBatch.Draw(this.fancyPanelSprite, destinationRectangle, null, color, 0f, Vector2.Zero, SpriteEffects.FlipVertically, 0.5f);
			float num2 = (float)(this.fancyCornerSprite.Height - this.fancyPanelSprite.Height);
			this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float)dest.X, (float)destinationRectangle.Y - num2), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically, 0.4f);
			this.spriteBatch.Draw(this.fancyCornerSprite, new Vector2((float)(rectangle.X + rectangle.Width), (float)destinationRectangle.Y - num2), null, color, 0f, Vector2.Zero, 1f, SpriteEffects.FlipVertically, 0.5f);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x0006906C File Offset: 0x0006726C
		private void doLsDisplay()
		{
			if (this.os.hasConnectionPermission(false))
			{
				this.x = 5;
				this.y = 5;
				int num = this.bounds.Width - 25;
				string text = (this.os.connectedComp == null) ? this.os.thisComputer.name : (this.os.connectedComp.name + " " + LocaleTerms.Loc("File System"));
				TextItem.doFontLabel(new Vector2((float)(this.bounds.X + this.x), (float)(this.bounds.Y + this.y)), text, GuiData.font, new Color?(Color.White), (float)this.bounds.Width - 46f, 60f, false);
				if (Button.doButton(299999, this.bounds.X + (this.bounds.Width - 41), this.bounds.Y + 12, 27, 29, "<-", null))
				{
					if (this.os.navigationPath.Count > 0)
					{
						this.os.runCommand("cd ..");
					}
					else
					{
						this.os.display.command = "connect";
					}
				}
				this.y += 50;
				Rectangle dest = GuiData.tmpRect;
				dest.Width = this.bounds.Width;
				dest.X = this.bounds.X;
				dest.Y = this.bounds.Y + 55;
				dest.Height = this.bounds.Height - 57;
				this.lsModuleHelper.DrawUI(dest, this.os);
			}
			else
			{
				this.command = "connect";
			}
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x000692E4 File Offset: 0x000674E4
		private void doFolderGui(int width, int height, int indexOffset, Folder f, int recItteration)
		{
			DisplayModule.<>c__DisplayClass8 CS$<>8__locals1 = new DisplayModule.<>c__DisplayClass8();
			CS$<>8__locals1.f = f;
			CS$<>8__locals1.<>4__this = this;
			int i;
			for (i = 0; i < CS$<>8__locals1.f.folders.Count; i++)
			{
				if (Button.doButton(300000 + i + indexOffset, this.x, this.y, width, height, "/" + CS$<>8__locals1.f.folders[i].name, null))
				{
					int num = 0;
					for (int k = 0; k < this.os.navigationPath.Count - recItteration; k++)
					{
						Action action = delegate()
						{
							this.os.runCommand("cd ..");
						};
						if (num > 0)
						{
							this.os.delayer.Post(ActionDelayer.Wait((double)num * 1.0), action);
						}
						else
						{
							action();
						}
						num++;
					}
					Action action2 = delegate()
					{
						CS$<>8__locals1.<>4__this.os.runCommand("cd " + CS$<>8__locals1.f.folders[i].name);
					};
					if (num > 0)
					{
						this.os.delayer.Post(ActionDelayer.Wait((double)num * 1.0), action2);
					}
					else
					{
						action2();
					}
				}
				this.y += height + 2;
				this.x += 30;
				if (this.os.navigationPath.Count - 1 >= recItteration && this.os.navigationPath[recItteration] == i)
				{
					this.doFolderGui(width - 30, height, indexOffset + 10000 * (i + 1), CS$<>8__locals1.f.folders[i], recItteration + 1);
				}
				this.x -= 30;
			}
			for (int j = 0; j < CS$<>8__locals1.f.files.Count; j++)
			{
				if (Button.doButton(400000 + j + indexOffset / 2 + (j + 1) * indexOffset, this.x, this.y, width, height, CS$<>8__locals1.f.files[j].name, null))
				{
					for (int k = 0; k < this.os.navigationPath.Count - recItteration; k++)
					{
						this.os.runCommand("cd ..");
					}
					this.os.runCommand("cat " + CS$<>8__locals1.f.files[j].name);
				}
				this.y += height + 2;
			}
			if (CS$<>8__locals1.f.folders.Count == 0 && CS$<>8__locals1.f.files.Count == 0)
			{
				TextItem.doFontLabel(new Vector2((float)this.x, (float)this.y), "-" + LocaleTerms.Loc("Empty") + "-", GuiData.tinyfont, null, (float)width, (float)height, false);
				this.y += height + 2;
			}
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x000696BC File Offset: 0x000678BC
		public static string splitForWidth(string s, int width)
		{
			string text = "";
			int num = 0;
			foreach (char c in s)
			{
				text += c;
				if (c != '\n')
				{
					num++;
				}
				else
				{
					num = 0;
				}
				if (num > width / 6 && (c == ' ' || (float)num > (float)width / 5.2f))
				{
					text += '\n';
					num = 0;
				}
			}
			return text;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00069760 File Offset: 0x00067960
		public static string splitForWidth(string s, int width, bool correct)
		{
			string text = "";
			int num = 0;
			width /= 8;
			foreach (char c in s)
			{
				text += c;
				if (c != '\n')
				{
					num++;
				}
				else
				{
					num = 0;
				}
				if (num >= width && (c == ' ' || (float)num > (float)width * 0.9f))
				{
					text += '\n';
					num = 0;
				}
			}
			return text;
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00069804 File Offset: 0x00067A04
		public static string cleanSplitForWidth(string s, int width)
		{
			int num = 10;
			width /= num;
			string text = "";
			char[] separator = new char[]
			{
				' '
			};
			string[] array = s.Split(separator);
			int i = 0;
			string result;
			if (array.Length == 1)
			{
				result = DisplayModule.splitForWidth(array[0], width * 8, true);
			}
			else
			{
				while (i < array.Length)
				{
					int num2 = 0;
					while (num2 < width && i < array.Length)
					{
						text = text + array[i] + " ";
						num2 += array[i].Length;
						int num3 = array[i].IndexOf('\n');
						if (num3 >= 0)
						{
							num2 = array[i].Length - (num3 + 1);
						}
						i++;
					}
					text += '\n';
				}
				result = text;
			}
			return result;
		}

		// Token: 0x040006F7 RID: 1783
		private const int MAX_DISPLAY_STRING_LENGTH = 6000;

		// Token: 0x040006F8 RID: 1784
		public string command = "";

		// Token: 0x040006F9 RID: 1785
		public string[] commandArgs;

		// Token: 0x040006FA RID: 1786
		public Folder LastDisplayedFileFolder = null;

		// Token: 0x040006FB RID: 1787
		public string LastDisplayedFileSourceIP = null;

		// Token: 0x040006FC RID: 1788
		private int x;

		// Token: 0x040006FD RID: 1789
		private int y;

		// Token: 0x040006FE RID: 1790
		private Rectangle tmpRect;

		// Token: 0x040006FF RID: 1791
		private List<Texture2D> computers;

		// Token: 0x04000700 RID: 1792
		private Dictionary<string, Texture2D> compAltIcons;

		// Token: 0x04000701 RID: 1793
		private Texture2D defaultComputer;

		// Token: 0x04000702 RID: 1794
		private Texture2D lockSprite;

		// Token: 0x04000703 RID: 1795
		private Texture2D openLockSprite;

		// Token: 0x04000704 RID: 1796
		private Texture2D fancyCornerSprite;

		// Token: 0x04000705 RID: 1797
		private Texture2D fancyPanelSprite;

		// Token: 0x04000706 RID: 1798
		private DisplayModuleLSHelper lsModuleHelper = new DisplayModuleLSHelper();

		// Token: 0x04000707 RID: 1799
		private ScrollableTextRegion catTextRegion;

		// Token: 0x04000708 RID: 1800
		private Vector2 scroll;

		// Token: 0x04000709 RID: 1801
		private Vector2 catScroll;

		// Token: 0x0400070A RID: 1802
		private int errorCount = 0;

		// Token: 0x0400070B RID: 1803
		private bool hasSentErrorEmail = false;

		// Token: 0x0400070C RID: 1804
		private string invioableSecurityCacheString = null;

		// Token: 0x0400070D RID: 1805
		private float invioabilityCharChangeTimer = 0f;

		// Token: 0x0400070E RID: 1806
		private string loginDetailsCache = null;

		// Token: 0x0400070F RID: 1807
		private bool lockLoginDisplayCache = false;
	}
}
