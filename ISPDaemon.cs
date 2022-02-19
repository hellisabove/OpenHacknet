using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200009E RID: 158
	internal class ISPDaemon : Daemon
	{
		// Token: 0x0600032E RID: 814 RVA: 0x0002E8E0 File Offset: 0x0002CAE0
		public ISPDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("ISP Management System"), os)
		{
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0002E940 File Offset: 0x0002CB40
		public override void initFiles()
		{
			base.initFiles();
			Folder folder = this.comp.files.root.searchForFolder("home");
			if (folder != null)
			{
				string dataEntry = Utils.readEntireFile("Content/LocPost/ISPAbout.txt");
				FileEntry item = new FileEntry(dataEntry, "ISP_About_Message.txt");
				folder.files.Add(item);
			}
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0002E99E File Offset: 0x0002CB9E
		public override void navigatedTo()
		{
			this.state = ISPDaemon.ISPDaemonState.Welcome;
			base.navigatedTo();
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0002E9B0 File Offset: 0x0002CBB0
		public override string getSaveString()
		{
			return "<ispSystem />";
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0002E9C8 File Offset: 0x0002CBC8
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			if (this.state == ISPDaemon.ISPDaemonState.AdminOnlyError || this.state == ISPDaemon.ISPDaemonState.NotFoundError)
			{
				PatternDrawer.draw(bounds, 0.1f, Color.Transparent, this.os.lockedColor, sb, PatternDrawer.errorTile);
			}
			Rectangle bounds2 = new Rectangle(bounds.X + 40, bounds.Y + 40, bounds.Width - 80, bounds.Height - 80);
			switch (this.state)
			{
			case ISPDaemon.ISPDaemonState.Welcome:
				this.DrawWelcomeScreen(bounds2, sb, bounds);
				break;
			case ISPDaemon.ISPDaemonState.About:
				this.DrawAboutScreen(bounds2, sb);
				break;
			case ISPDaemon.ISPDaemonState.Loading:
				this.DrawLoadingScreen(bounds2, sb);
				break;
			case ISPDaemon.ISPDaemonState.IPEntry:
				this.DrawIPEntryScreen(bounds2, sb);
				break;
			case ISPDaemon.ISPDaemonState.EnterIP:
				this.DrawEnterIPScreen(bounds2, sb);
				break;
			case ISPDaemon.ISPDaemonState.AdminOnlyError:
				this.DrawAdminOnlyError(bounds2, sb);
				break;
			case ISPDaemon.ISPDaemonState.NotFoundError:
				this.DrawNotFoundError(bounds2, sb);
				break;
			}
			this.drawOutlineEffect(bounds, sb);
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0002EAD4 File Offset: 0x0002CCD4
		private void DrawIPEntryScreen(Rectangle bounds, SpriteBatch sb)
		{
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 20)), LocaleTerms.Loc("IP Entry") + " :: " + this.scannedComputer.ip, GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 70)), LocaleTerms.Loc("Identified as") + " :\"" + this.scannedComputer.name + "\"", GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			sb.Draw(this.os.display.GetComputerImage(this.scannedComputer), new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 100)), Color.White);
			int x = bounds.X + 30 + 130;
			int num = bounds.Y + 100;
			if (Button.doButton(3388301, x, num, bounds.Width - 180, 30, LocaleTerms.Loc("Assign New IP"), new Color?(this.os.brightUnlockedColor)))
			{
				bool flag;
				do
				{
					this.scannedComputer.ip = NetworkMap.generateRandomIP();
					flag = false;
					for (int i = 0; i < this.os.netMap.nodes.Count; i++)
					{
						if (this.os.netMap.nodes[i].ip == this.scannedComputer.ip && this.os.netMap.nodes[i].idName != this.scannedComputer.idName)
						{
							flag = true;
						}
					}
				}
				while (flag);
				if (this.os.thisComputer.idName == this.scannedComputer.idName)
				{
					this.os.thisComputerIPReset();
				}
			}
			num += 34;
			if (Button.doButton(3388304, x, num, bounds.Width - 180, 30, LocaleTerms.Loc("Flag for Inspection") + (this.inspectionFlagged ? (" : " + LocaleTerms.Loc("ACTIVE")) : ""), new Color?(this.inspectionFlagged ? Color.Gray : this.os.highlightColor)))
			{
				this.inspectionFlagged = true;
			}
			num += 34;
			if (Button.doButton(3388308, x, num, bounds.Width - 180, 30, LocaleTerms.Loc("Prioritize Routing"), new Color?(this.os.highlightColor)))
			{
			}
			this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0002EDFC File Offset: 0x0002CFFC
		private void DrawLoadingScreen(Rectangle bounds, SpriteBatch sb)
		{
			float num = this.os.timer - this.timeEnteredLoadingScreen;
			if (num >= 2f)
			{
				this.scannedComputer = null;
				for (int i = 0; i < this.os.netMap.nodes.Count; i++)
				{
					if (this.ipSearch == this.os.netMap.nodes[i].ip)
					{
						this.scannedComputer = this.os.netMap.nodes[i];
						break;
					}
				}
				if (this.scannedComputer != null)
				{
					this.state = ISPDaemon.ISPDaemonState.IPEntry;
					this.inspectionFlagged = false;
				}
				else
				{
					this.state = ISPDaemon.ISPDaemonState.NotFoundError;
				}
			}
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + bounds.Height / 2 - 35)), LocaleTerms.Loc("Scanning for") + " " + this.ipSearch + " ...", GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			Rectangle destinationRectangle = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 2, bounds.Width - 20, 16);
			sb.Draw(Utils.white, destinationRectangle, Color.Gray);
			destinationRectangle.X++;
			destinationRectangle.Y++;
			destinationRectangle.Width -= 2;
			destinationRectangle.Height -= 2;
			sb.Draw(Utils.white, destinationRectangle, Color.Black);
			float num2 = Utils.QuadraticOutCurve(num / 2f);
			destinationRectangle.Width = (int)((float)destinationRectangle.Width * num2);
			sb.Draw(Utils.white, destinationRectangle, this.os.highlightColor);
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0002F004 File Offset: 0x0002D204
		private void DrawEnterIPScreen(Rectangle bounds, SpriteBatch sb)
		{
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			string[] array = this.os.getStringCache.Split(separator, StringSplitOptions.None);
			string text = null;
			int num = bounds.X + 10;
			int num2 = bounds.Y + 10;
			num2 += (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("Enter IP Address to Scan for") + " :", null).Y + 5;
			if (array.Length > 1)
			{
				text = array[1];
				if (text.Equals(""))
				{
					text = this.os.terminal.currentLine;
				}
			}
			Rectangle destinationRectangle = new Rectangle(num, num2, bounds.Width - 20, 200);
			sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
			num2 += 80;
			destinationRectangle.X = num + (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), LocaleTerms.Loc("IP Address") + ": " + text, null).X + 2;
			destinationRectangle.Y = num2;
			destinationRectangle.Width = 7;
			destinationRectangle.Height = 20;
			if (this.os.timer % 1f < 0.3f)
			{
				sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
			}
			num2 += 122;
			if (array.Length > 2 || Button.doButton(30, num, num2, 300, 22, LocaleTerms.Loc("Scan"), new Color?(this.os.highlightColor)))
			{
				if (array.Length <= 2)
				{
					this.os.terminal.executeLine();
				}
				this.ipSearch = text;
				this.state = ISPDaemon.ISPDaemonState.Loading;
				this.timeEnteredLoadingScreen = this.os.timer;
				this.os.getStringCache = "";
			}
			num2 += 26;
			if (Button.doButton(35, num, num2, 300, 22, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
			{
				this.os.terminal.executeLine();
				this.state = ISPDaemon.ISPDaemonState.Welcome;
				this.os.getStringCache = "";
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0002F294 File Offset: 0x0002D494
		private void drawBackButton(Rectangle bounds, ISPDaemon.ISPDaemonState stateTo = ISPDaemon.ISPDaemonState.Welcome)
		{
			if (Button.doButton(92271094, bounds.X + 30, bounds.Y + bounds.Height - 50, 200, 25, LocaleTerms.Loc("Back"), new Color?(this.os.lockedColor)))
			{
				this.state = stateTo;
			}
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0002F2FC File Offset: 0x0002D4FC
		private void DrawAdminOnlyError(Rectangle bounds, SpriteBatch sb)
		{
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 20)), LocaleTerms.Loc("Insufficient Permissions"), GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 70)), string.Concat(new string[]
			{
				LocaleTerms.Loc("IP Search is limited to administrators"),
				"\n",
				LocaleTerms.Loc("of this machine only"),
				".\n",
				LocaleTerms.Loc("If you require access, contact customer support"),
				"."
			}), GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0002F3E8 File Offset: 0x0002D5E8
		private void DrawNotFoundError(Rectangle bounds, SpriteBatch sb)
		{
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 20)), LocaleTerms.Loc("IP Not Found"), GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 70)), string.Concat(new string[]
			{
				LocaleTerms.Loc("IP Address is not registered with"),
				"\n",
				LocaleTerms.Loc("our servers"),
				".\n",
				LocaleTerms.Loc("Check address and try again"),
				"."
			}), GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
		}

		// Token: 0x06000339 RID: 825 RVA: 0x0002F4D4 File Offset: 0x0002D6D4
		private void DrawAboutScreen(Rectangle bounds, SpriteBatch sb)
		{
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 20)), LocaleTerms.Loc("About this server"), GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			Folder folder = this.comp.files.root.searchForFolder("home");
			if (folder != null)
			{
				FileEntry fileEntry = folder.searchForFile("ISP_About_Message.txt");
				if (fileEntry != null)
				{
					TextItem.DrawShadow = false;
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 30), (float)(bounds.Y + 70)), fileEntry.data, GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
				}
			}
			this.drawBackButton(bounds, ISPDaemon.ISPDaemonState.Welcome);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x0002F5B8 File Offset: 0x0002D7B8
		private void DrawWelcomeScreen(Rectangle bounds, SpriteBatch sb, Rectangle fullBounds)
		{
			TextItem.doFontLabel(new Vector2((float)(bounds.X + 20), (float)(bounds.Y + 20)), LocaleTerms.Loc("ISP Management System"), GuiData.font, new Color?(Color.White), (float)bounds.Width, float.MaxValue, false);
			int num = 30;
			int width = (int)((float)bounds.Width * 0.8f);
			int num2 = bounds.Y + 90;
			Rectangle destinationRectangle = fullBounds;
			destinationRectangle.Y = num2 - 20;
			destinationRectangle.Height = 22;
			bool flag = this.comp.adminIP == this.os.thisComputer.ip;
			sb.Draw(Utils.white, destinationRectangle, flag ? this.os.unlockedColor : this.os.lockedColor);
			string text = LocaleTerms.Loc("Valid Administrator Account Detected");
			if (!flag)
			{
				text = LocaleTerms.Loc("Non-Admin Account Active");
			}
			Vector2 vector = GuiData.smallfont.MeasureString(text);
			Vector2 position = new Vector2((float)(destinationRectangle.X + destinationRectangle.Width / 2) - vector.X / 2f, (float)destinationRectangle.Y);
			sb.DrawString(GuiData.smallfont, text, position, Color.White);
			num2 += 30;
			if (Button.doButton(95371001, bounds.X + 20, num2, width, num, LocaleTerms.Loc("About"), new Color?(this.os.highlightColor)))
			{
				this.state = ISPDaemon.ISPDaemonState.About;
			}
			num2 += num + 10;
			if (Button.doButton(95371004, bounds.X + 20, num2, width, num, LocaleTerms.Loc("Search for IP"), new Color?(this.os.highlightColor)))
			{
				if (this.comp.adminIP == this.os.thisComputer.ip)
				{
					this.state = ISPDaemon.ISPDaemonState.EnterIP;
					this.os.execute("getString IP_Address");
					this.ipSearch = null;
				}
				else
				{
					this.state = ISPDaemon.ISPDaemonState.AdminOnlyError;
				}
			}
			num2 += num + 10;
			if (Button.doButton(95371008, bounds.X + 20, num2, width, num, LocaleTerms.Loc("Exit"), new Color?(this.os.highlightColor)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0002F838 File Offset: 0x0002DA38
		private void drawOutlineEffect(Rectangle bounds, SpriteBatch sb)
		{
			if (this.lastTimer == 0f)
			{
				this.lastTimer = this.os.timer;
			}
			if (this.os.timer % 0.5f > 0.1f && this.lastTimer % 0.5f < 0.1f)
			{
				this.addNewOutlineEffect();
			}
			for (int i = 0; i < this.outlineEffectEntries.Count; i++)
			{
				ISPDaemon.ExpandingRectangleData value = this.outlineEffectEntries[i];
				value.scaleIn += (this.os.timer - this.lastTimer) * value.rate * 1f;
				if (value.scaleIn > 30f)
				{
					this.outlineEffectEntries.RemoveAt(i);
					i--;
				}
				else
				{
					Rectangle rect = new Rectangle((int)((float)bounds.X + value.scaleIn), (int)((float)bounds.Y + value.scaleIn), (int)((float)bounds.Width - 2f * value.scaleIn), (int)((float)bounds.Height - 2f * value.scaleIn));
					this.drawRect(rect, sb, (int)value.thickness, 1f - value.scaleIn / 30f, value.blackLerp);
					this.outlineEffectEntries[i] = value;
				}
			}
			this.drawRect(bounds, sb, 4, 1f, 0f);
			this.lastTimer = this.os.timer;
		}

		// Token: 0x0600033C RID: 828 RVA: 0x0002F9EC File Offset: 0x0002DBEC
		private void drawRect(Rectangle rect, SpriteBatch sb, int thickness, float opacity, float blackLerp)
		{
			Rectangle destinationRectangle = rect;
			destinationRectangle.Width = thickness;
			Color color = Color.Lerp(this.os.highlightColor, Color.Black, blackLerp) * opacity;
			sb.Draw(Utils.white, destinationRectangle, color);
			destinationRectangle.X += rect.Width - thickness;
			sb.Draw(Utils.white, destinationRectangle, color);
			destinationRectangle.X = rect.X + thickness;
			destinationRectangle.Width = rect.Width - 2 * thickness;
			destinationRectangle.Height = thickness;
			sb.Draw(Utils.white, destinationRectangle, color);
			destinationRectangle.Y += rect.Height - thickness;
			sb.Draw(Utils.white, destinationRectangle, color);
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0002FAB4 File Offset: 0x0002DCB4
		private void addNewOutlineEffect()
		{
			ISPDaemon.ExpandingRectangleData item = new ISPDaemon.ExpandingRectangleData
			{
				rate = Utils.randm(1f) * 15f + 1f,
				scaleIn = 1f,
				thickness = 1f + Utils.randm(1f) * Utils.randm(1f) * 6f,
				blackLerp = Utils.randm(0.8f)
			};
			this.outlineEffectEntries.Add(item);
		}

		// Token: 0x04000389 RID: 905
		private const float MAX_WIDTH = 7f;

		// Token: 0x0400038A RID: 906
		private const float MAX_RATE = 16f;

		// Token: 0x0400038B RID: 907
		private const float EFFECT_TIMER = 30f;

		// Token: 0x0400038C RID: 908
		private const float SEARCH_TIME = 2f;

		// Token: 0x0400038D RID: 909
		private const string ABOUT_MESSAGE_FILE = "ISP_About_Message.txt";

		// Token: 0x0400038E RID: 910
		private List<ISPDaemon.ExpandingRectangleData> outlineEffectEntries = new List<ISPDaemon.ExpandingRectangleData>();

		// Token: 0x0400038F RID: 911
		private float lastTimer = 0f;

		// Token: 0x04000390 RID: 912
		private float timeEnteredLoadingScreen = 0f;

		// Token: 0x04000391 RID: 913
		private ISPDaemon.ISPDaemonState state = ISPDaemon.ISPDaemonState.Welcome;

		// Token: 0x04000392 RID: 914
		private string ipSearch = null;

		// Token: 0x04000393 RID: 915
		private Computer scannedComputer = null;

		// Token: 0x04000394 RID: 916
		private bool inspectionFlagged = false;

		// Token: 0x0200009F RID: 159
		private struct ExpandingRectangleData
		{
			// Token: 0x04000395 RID: 917
			public float scaleIn;

			// Token: 0x04000396 RID: 918
			public float rate;

			// Token: 0x04000397 RID: 919
			public float thickness;

			// Token: 0x04000398 RID: 920
			public float blackLerp;
		}

		// Token: 0x020000A0 RID: 160
		private enum ISPDaemonState
		{
			// Token: 0x0400039A RID: 922
			Welcome,
			// Token: 0x0400039B RID: 923
			About,
			// Token: 0x0400039C RID: 924
			Loading,
			// Token: 0x0400039D RID: 925
			IPEntry,
			// Token: 0x0400039E RID: 926
			EnterIP,
			// Token: 0x0400039F RID: 927
			AdminOnlyError,
			// Token: 0x040003A0 RID: 928
			NotFoundError
		}
	}
}
