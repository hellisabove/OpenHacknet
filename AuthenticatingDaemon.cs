using System;
using System.Threading;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000BB RID: 187
	internal class AuthenticatingDaemon : Daemon
	{
		// Token: 0x060003C5 RID: 965 RVA: 0x0003A20C File Offset: 0x0003840C
		public AuthenticatingDaemon(Computer computer, string serviceName, OS opSystem) : base(computer, serviceName, opSystem)
		{
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x0003A21A File Offset: 0x0003841A
		public virtual void loginGoBack()
		{
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0003A21D File Offset: 0x0003841D
		public virtual void userLoggedIn()
		{
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x0003A220 File Offset: 0x00038420
		public void startLogin()
		{
			this.os.displayCache = "";
			this.os.execute("login");
			while (this.os.displayCache.Equals(""))
			{
			}
			this.os.display.command = this.name;
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x0003A280 File Offset: 0x00038480
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
						}
					}
					this.userLoggedIn();
				}
			}
			tmpRect.Height = 22;
			num2 += 30;
			Vector2 vector = TextItem.doMeasuredLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Login") + " ", new Color?(Color.White));
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
			int num5 = (int)(6f + Math.Max(GuiData.smallfont.MeasureString(LocaleTerms.Loc("username :")).X, GuiData.smallfont.MeasureString(LocaleTerms.Loc("password :")).X));
			sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("username :"), new Vector2((float)num, (float)num2), Color.White);
			num += num5;
			sb.DrawString(GuiData.smallfont, text, new Vector2((float)num, (float)num2), Color.White);
			num -= num5;
			num2 += 30;
			if (num4 == 1)
			{
				tmpRect.Y = num2;
				sb.Draw(Utils.white, tmpRect, this.os.subtleTextColor);
			}
			sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("password :"), new Vector2((float)num, (float)num2), Color.White);
			num += num5;
			sb.DrawString(GuiData.smallfont, text2, new Vector2((float)num, (float)num2), Color.White);
			num2 += 30;
			num -= num5;
			if (num3 != -1)
			{
				if (Button.doButton(12345, num, num2, 160, 30, LocaleTerms.Loc("Back"), new Color?(this.os.indentBackgroundColor)))
				{
					this.loginGoBack();
				}
				if (Button.doButton(123456, num + 165, num2, 160, 30, LocaleTerms.Loc("Retry"), new Color?(this.os.indentBackgroundColor)))
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
					if (this.comp.users[i].known && AuthenticatingDaemon.validUser(this.comp.users[i].type))
					{
						if (Button.doButton(123457 + i, num, num2, 300, 25, string.Concat(new string[]
						{
							LocaleTerms.Loc("User"),
							": ",
							this.comp.users[i].name,
							" ",
							LocaleTerms.Loc("Pass"),
							": ",
							this.comp.users[i].pass
						}), new Color?(this.os.darkBackgroundColor)))
						{
							this.forceLogin(this.comp.users[i].name, this.comp.users[i].pass);
						}
						num2 += 27;
					}
				}
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x0003A900 File Offset: 0x00038B00
		public new static bool validUser(byte type)
		{
			return Daemon.validUser(type) || type == 3;
		}

		// Token: 0x060003CB RID: 971 RVA: 0x0003A924 File Offset: 0x00038B24
		public void forceLogin(string username, string pass)
		{
			string prompt = this.os.terminal.prompt;
			this.os.terminal.currentLine = username;
			this.os.terminal.NonThreadedInstantExecuteLine();
			while (this.os.terminal.prompt.Equals(prompt))
			{
				Thread.Sleep(0);
			}
			this.os.terminal.currentLine = pass;
			this.os.terminal.NonThreadedInstantExecuteLine();
		}

		// Token: 0x04000469 RID: 1129
		public UserDetail user;
	}
}
