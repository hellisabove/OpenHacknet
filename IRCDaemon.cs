using System;
using System.Collections.Generic;
using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200000E RID: 14
	internal class IRCDaemon : Daemon, IMonitorableDaemon
	{
		// Token: 0x06000073 RID: 115 RVA: 0x00009711 File Offset: 0x00007911
		public IRCDaemon(Computer c, OS os, string name) : base(c, name, os)
		{
			this.isListed = true;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00009744 File Offset: 0x00007944
		public override void initFiles()
		{
			base.initFiles();
			Folder folder = this.comp.files.root.searchForFolder("IRC");
			if (folder == null)
			{
				folder = new Folder("IRC");
				this.comp.files.root.folders.Add(folder);
			}
			this.System = new IRCSystem(folder);
			for (int i = 0; i < this.StartingMessages.Count; i++)
			{
				this.System.AddLog(this.StartingMessages[i].Key, this.StartingMessages[i].Value, null);
			}
			string text = string.Concat(new string[]
			{
				this.name,
				"\nRequireAuth: ",
				this.RequiresLogin ? "true" : "false",
				"\n",
				Utils.convertColorToParseableString(this.ThemeColor),
				"\n"
			});
			foreach (KeyValuePair<string, Color> keyValuePair in this.UserColors)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					keyValuePair.Key,
					"#%#",
					Utils.convertColorToParseableString(keyValuePair.Value),
					"\n"
				});
			}
			folder.files.Add(new FileEntry(text.Trim(), "users.cfg"));
			Folder folder2 = folder.searchForFolder("runtime");
			if (folder2 == null)
			{
				folder2 = new Folder("runtime");
				folder.folders.Add(folder2);
			}
			this.DelayedActions = new DelayableActionSystem(folder2, this.os);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00009960 File Offset: 0x00007B60
		public override void loadInit()
		{
			base.loadInit();
			Folder folder = this.comp.files.root.searchForFolder("IRC");
			this.System = new IRCSystem(folder);
			Folder sourceFolder = folder.searchForFolder("runtime");
			this.DelayedActions = new DelayableActionSystem(sourceFolder, this.os);
			this.ReloadUserColors();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000099C1 File Offset: 0x00007BC1
		public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			IRCSystem system = this.System;
			system.LogAdded = (Action<string, string>)Delegate.Combine(system.LogAdded, act);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x000099E0 File Offset: 0x00007BE0
		public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			IRCSystem system = this.System;
			system.LogAdded = (Action<string, string>)Delegate.Remove(system.LogAdded, act);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00009A00 File Offset: 0x00007C00
		public bool ShouldDisplayNotifications()
		{
			return true;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00009A14 File Offset: 0x00007C14
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00009A2C File Offset: 0x00007C2C
		private void ReloadUserColors()
		{
			Folder folder = this.comp.files.root.searchForFolder("IRC");
			this.UserColors.Clear();
			FileEntry fileEntry = folder.searchForFile("users.cfg");
			if (fileEntry != null)
			{
				string[] array = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
				try
				{
					this.name = array[0];
					this.RequiresLogin = (array[1].Substring("RequireAuth: ".Length).Trim().ToLower() == "true");
					this.ThemeColor = Utils.convertStringToColor(array[2]);
				}
				catch (Exception)
				{
				}
				for (int i = 3; i < array.Length; i++)
				{
					try
					{
						string[] array2 = array[i].Split(new string[]
						{
							"#%#"
						}, StringSplitOptions.RemoveEmptyEntries);
						this.UserColors.Add(array2[0], Utils.convertStringToColor(array2[1]));
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00009B50 File Offset: 0x00007D50
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.ReloadUserColors();
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00009B64 File Offset: 0x00007D64
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			bool flag = !this.RequiresLogin || this.comp.adminIP == this.os.thisComputer.ip || this.comp.userLoggedIn;
			Rectangle dest = Utils.InsetRectangle(bounds, 2);
			Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y - 1, 18, dest.Height + 2);
			sb.Draw(Utils.white, destinationRectangle, this.ThemeColor);
			destinationRectangle.X += destinationRectangle.Width / 2;
			destinationRectangle.Width /= 2;
			sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.2f);
			dest.X += 20;
			dest.Width -= 25;
			Rectangle rectangle = new Rectangle(dest.X + 4, dest.Y, dest.Width, 35);
			TextItem.doFontLabelToSize(rectangle, this.name, GuiData.font, Color.White, true, true);
			int num = dest.Width / 4;
			int num2 = 22;
			if (Button.doButton(37849102, rectangle.X + rectangle.Width - 6 - num, rectangle.Y + rectangle.Height - rectangle.Height / 2 - num2 / 2, num, num2, LocaleTerms.Loc("Exit IRC View"), new Color?(this.ThemeColor)))
			{
				this.os.display.command = "connect";
			}
			rectangle.Y += rectangle.Height;
			rectangle.X -= 6;
			dest.Y += rectangle.Height;
			dest.Height -= rectangle.Height;
			rectangle.Height = 2;
			sb.Draw(Utils.white, rectangle, this.ThemeColor);
			dest.Y += rectangle.Height + 2;
			dest.Height -= rectangle.Height + 2;
			dest.Height -= 6;
			PatternDrawer.draw(dest, 0.22f, Color.Black * 0.5f, flag ? (this.ThemeColor * 0.12f) : (Utils.AddativeRed * 0.2f), sb, flag ? PatternDrawer.thinStripe : PatternDrawer.warningStripe);
			dest.X += 2;
			dest.Width -= 4;
			dest.Height -= 4;
			if (flag)
			{
				this.System.Draw(dest, sb, false, LocaleTerms.Loc("UNKNOWN"), this.UserColors);
			}
			else
			{
				int num3 = dest.Height / 4;
				Rectangle rectangle2 = new Rectangle(dest.X - 4, dest.Y + dest.Height / 2 - num3 / 2, dest.Width + 6, num3);
				sb.Draw(Utils.white, rectangle2, this.os.lockedColor);
				rectangle2.Height -= 35;
				TextItem.doCenteredFontLabel(rectangle2, LocaleTerms.Loc("Login To Server"), GuiData.font, Color.White, false);
				if (Button.doButton(84109551, rectangle2.X + rectangle2.Width / 2 - rectangle2.Width / 4, rectangle2.Y + rectangle2.Height - 32, rectangle2.Width / 2, 28, "Login", null))
				{
					this.os.runCommand("login");
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00009F5C File Offset: 0x0000815C
		public override string getSaveString()
		{
			return "<IRCDaemon />";
		}

		// Token: 0x04000082 RID: 130
		public IRCSystem System;

		// Token: 0x04000083 RID: 131
		public List<KeyValuePair<string, string>> StartingMessages = new List<KeyValuePair<string, string>>();

		// Token: 0x04000084 RID: 132
		public Dictionary<string, Color> UserColors = new Dictionary<string, Color>();

		// Token: 0x04000085 RID: 133
		public Color ThemeColor;

		// Token: 0x04000086 RID: 134
		public DelayableActionSystem DelayedActions;

		// Token: 0x04000087 RID: 135
		public bool RequiresLogin = false;
	}
}
