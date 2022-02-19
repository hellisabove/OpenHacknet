using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000011 RID: 17
	internal class WhitelistConnectionDaemon : Daemon
	{
		// Token: 0x0600008E RID: 142 RVA: 0x0000A8ED File Offset: 0x00008AED
		public WhitelistConnectionDaemon(Computer c, OS os) : base(c, "Whitelist Authenticator", os)
		{
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000A984 File Offset: 0x00008B84
		public override void initFiles()
		{
			this.folder = this.comp.files.root.searchForFolder("Whitelist");
			if (this.folder == null)
			{
				this.folder = new Folder("Whitelist");
				this.comp.files.root.folders.Add(this.folder);
			}
			if (!this.folder.containsFile("authenticator.dll"))
			{
				this.folder.files.Add(new FileEntry(Computer.generateBinaryString(500), "authenticator.dll"));
			}
			if (this.RemoteSourceIP == null)
			{
				if (!this.folder.containsFile("list.txt"))
				{
					this.folder.files.Add(new FileEntry(this.comp.adminIP, "list.txt"));
				}
			}
			else
			{
				ComputerLoader.postAllLoadedActions = (Action)Delegate.Combine(ComputerLoader.postAllLoadedActions, new Action(delegate()
				{
					Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
					if (computer != null)
					{
						this.RemoteSourceIP = computer.ip;
					}
					if (!this.folder.containsFile("source.txt"))
					{
						this.folder.files.Add(new FileEntry(this.RemoteSourceIP, "source.txt"));
					}
				}));
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000AAAC File Offset: 0x00008CAC
		public override void loadInit()
		{
			base.loadInit();
			this.folder = this.comp.files.root.searchForFolder("Whitelist");
			if (this.folder.containsFile("source.txt"))
			{
				this.RemoteSourceIP = this.folder.searchForFile("source.txt").data.Trim();
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000AB1C File Offset: 0x00008D1C
		public override void navigatedTo()
		{
			base.navigatedTo();
			if (!this.IPCanPassWhitelist(this.os.thisComputer.ip, false))
			{
				this.DisconnectTarget();
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000AB70 File Offset: 0x00008D70
		public void DisconnectTarget()
		{
			this.os.execute("disconnect");
			this.os.display.command = "connectiondenied";
			this.os.delayer.Post(ActionDelayer.NextTick(), delegate
			{
				this.os.display.command = "connectiondenied";
			});
			this.os.write(" ");
			this.os.write(" ");
			this.os.write("------------------------------");
			this.os.write("------------------------------");
			this.os.write(" ");
			this.os.write("---  " + LocaleTerms.Loc("CONNECTION ERROR") + "  ---");
			this.os.write(" ");
			this.os.write(LocaleTerms.Loc("Message from Server:"));
			this.os.write(string.Format(LocaleTerms.Loc("Whitelist Authenticator denied connection from IP {0}"), this.os.thisComputer.ip));
			this.os.write(" ");
			this.os.write("------------------------------");
			this.os.write("------------------------------");
			this.os.write(" ");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000ACD8 File Offset: 0x00008ED8
		private bool RemoteCompCanBeAccessed()
		{
			bool result;
			if (this.RemoteSourceIP == null)
			{
				result = false;
			}
			else
			{
				Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
				if (computer != null && computer.bootTimer <= 0f)
				{
					WhitelistConnectionDaemon whitelistConnectionDaemon = (WhitelistConnectionDaemon)computer.getDaemon(typeof(WhitelistConnectionDaemon));
					result = (whitelistConnectionDaemon == null || whitelistConnectionDaemon.comp.files.root.searchForFolder("Whitelist").searchForFile("authenticator.dll") != null);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000AD8C File Offset: 0x00008F8C
		public bool IPCanPassWhitelist(string ip, bool isFromRemote)
		{
			bool result;
			if (!this.AuthenticatesItself && !isFromRemote)
			{
				result = true;
			}
			else if (this.RemoteSourceIP != null)
			{
				Computer computer = Programs.getComputer(this.os, this.RemoteSourceIP);
				if (!this.RemoteCompCanBeAccessed())
				{
					result = true;
				}
				else if (this.folder.searchForFile("authenticator.dll") == null)
				{
					result = true;
				}
				else
				{
					WhitelistConnectionDaemon whitelistConnectionDaemon = (WhitelistConnectionDaemon)computer.getDaemon(typeof(WhitelistConnectionDaemon));
					result = (whitelistConnectionDaemon == null || whitelistConnectionDaemon.IPCanPassWhitelist(ip, true));
				}
			}
			else if (this.folder.searchForFile("authenticator.dll") == null)
			{
				result = true;
			}
			else
			{
				FileEntry fileEntry = this.folder.searchForFile("list.txt");
				if (fileEntry == null)
				{
					result = true;
				}
				else
				{
					string[] array = fileEntry.data.Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						if (this.os.thisComputer.ip == array[i].Trim())
						{
							return true;
						}
					}
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000AEFC File Offset: 0x000090FC
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			if (this.comp.adminIP == this.os.thisComputer.ip && !this.HasAllowedSingleTimeAdminPassthroughThisSession)
			{
				this.os.display.command = "connect";
				this.HasAllowedSingleTimeAdminPassthroughThisSession = true;
			}
			Rectangle dest = Utils.InsetRectangle(bounds, 2);
			bool flag = this.RemoteCompCanBeAccessed();
			bool flag2 = this.IPCanPassWhitelist(this.os.thisComputer.ip, false) && flag;
			bool flag3 = this.folder.searchForFile("authenticator.dll") != null;
			bool flag4 = this.RemoteSourceIP != null || this.folder.searchForFile("list.txt") != null;
			bool flag5 = flag2 && flag4 && flag3;
			Color color = (!this.AuthenticatesItself) ? this.os.highlightColor : (flag5 ? this.os.unlockedColor : this.os.brightLockedColor);
			PatternDrawer.draw(dest, 1f, Color.Black * 0.1f, color * 0.4f, sb, flag5 ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
			Rectangle dest2 = new Rectangle(dest.X + 10, dest.Y + dest.Height / 4, dest.Width - 20, 40);
			if (Button.doButton(8711133, dest2.X, dest2.Y - 30, dest.Width / 4, 24, LocaleTerms.Loc("Proceed"), new Color?(this.os.highlightColor)))
			{
				this.os.display.command = "connect";
			}
			string text = string.Format(LocaleTerms.Loc("Whitelist Authentication Successful : {0}"), this.os.thisComputer.ip);
			if (!this.AuthenticatesItself)
			{
				text = LocaleTerms.Loc("Whitelist Server Active for remote nodes...");
			}
			else if (!flag)
			{
				text = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("Source Whitelist server at {0} cannot be accessed"), this.RemoteSourceIP);
			}
			else if (!flag3)
			{
				text = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("System File {0} not found"), "authenticator.dll");
			}
			else if (!flag4)
			{
				text = string.Format(LocaleTerms.Loc("Whitelist Authenticator Critical Error:") + "\n" + LocaleTerms.Loc("Whitelist File {0} not found"), "list.txt");
			}
			sb.Draw(Utils.white, new Rectangle(bounds.X + 1, dest2.Y - 3, bounds.Width - 2, dest2.Height + 6), Color.Black * 0.7f);
			TextItem.doFontLabelToSize(dest2, text, GuiData.font, color, true, true);
			Rectangle dest3 = new Rectangle(dest2.X, dest2.Y + dest2.Height + 6, dest2.Width, dest.Height / 2);
			string text2 = LocaleTerms.Loc("Connection established");
			if (!this.AuthenticatesItself)
			{
				text2 = LocaleTerms.Loc("Processing connections as authentication server.");
			}
			else if (!flag)
			{
				text2 = string.Format(LocaleTerms.Loc("Could not establish connection to whitelist server {0}.") + "\n" + LocaleTerms.Loc("Aborting Execution."), this.RemoteSourceIP);
			}
			else if (!flag3)
			{
				text2 = string.Concat(new string[]
				{
					LocaleTerms.Loc("Unhanded FileNotFoundException"),
					"\n",
					string.Format(LocaleTerms.Loc("File /Whitelist/{0} Could not be read."), "authenticator.dll"),
					"\n",
					LocaleTerms.Loc("Aborting Execution")
				});
			}
			else if (!flag4)
			{
				text2 = string.Concat(new string[]
				{
					LocaleTerms.Loc("Unhanded FileNotFoundException"),
					"\n",
					string.Format(LocaleTerms.Loc("File /Whitelist/{0} Could not be read."), "list.txt"),
					"\n",
					LocaleTerms.Loc("Aborting Execution")
				});
			}
			TextItem.doFontLabelToSize(dest3, text2, GuiData.smallfont, Color.White * 0.8f, true, true);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000B374 File Offset: 0x00009574
		public override string getSaveString()
		{
			return "<WhitelistAuthenticatorDaemon SelfAuthenticating=\"" + this.AuthenticatesItself + "\"/>";
		}

		// Token: 0x04000099 RID: 153
		private const string SystemFilename = "authenticator.dll";

		// Token: 0x0400009A RID: 154
		private const string ListFilename = "list.txt";

		// Token: 0x0400009B RID: 155
		private const string SourceFilename = "source.txt";

		// Token: 0x0400009C RID: 156
		private Folder folder;

		// Token: 0x0400009D RID: 157
		public string RemoteSourceIP = null;

		// Token: 0x0400009E RID: 158
		public bool AuthenticatesItself = true;

		// Token: 0x0400009F RID: 159
		private bool HasAllowedSingleTimeAdminPassthroughThisSession = false;
	}
}
