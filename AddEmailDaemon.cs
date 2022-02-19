using System;
using System.Linq;
using System.Net;
using System.Threading;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000F9 RID: 249
	internal class AddEmailDaemon : Daemon
	{
		// Token: 0x06000559 RID: 1369 RVA: 0x000543FB File Offset: 0x000525FB
		public AddEmailDaemon(Computer computer, string serviceName, OS opSystem) : base(computer, serviceName, opSystem)
		{
			this.state = 0;
			this.email = "";
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x0005441C File Offset: 0x0005261C
		private void saveEmail()
		{
			try
			{
				Utils.appendToFile(this.email + "\r\n", "Emails.txt");
				AddEmailDaemon.lastSentEmail = this.email;
				SFX.addCircle(this.os.netMap.GetNodeDrawPos(this.comp) + new Vector2((float)this.os.netMap.bounds.X, (float)this.os.netMap.bounds.Y) + new Vector2((float)(NetworkMap.NODE_SIZE / 2)), this.os.thisComputerNode, 100f);
				this.state = 0;
				this.os.display.command = "connect";
			}
			catch (Exception)
			{
				this.state = 3;
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00054504 File Offset: 0x00052704
		public void sendEmail()
		{
			if (AddEmailDaemon.lastSentEmail == null || !AddEmailDaemon.lastSentEmail.Equals(this.email))
			{
				if (this.email != null && this.email.Contains('@'))
				{
					new Thread(new ThreadStart(this.makeWebRequest))
					{
						IsBackground = true
					}.Start();
					AddEmailDaemon.lastSentEmail = this.email;
					SFX.addCircle(this.comp.location + new Vector2((float)this.os.netMap.bounds.X, (float)this.os.netMap.bounds.Y) + new Vector2((float)(NetworkMap.NODE_SIZE / 2)), this.os.thisComputerNode, 100f);
					this.state = 0;
					this.os.display.command = "connect";
				}
				else
				{
					this.state = 3;
				}
			}
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00054618 File Offset: 0x00052818
		public override string getSaveString()
		{
			return "<AddEmailServer name=\"" + this.name + "\"/>";
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00054640 File Offset: 0x00052840
		public void makeWebRequest()
		{
			try
			{
				string address = "http://www.tijital-games.com/hacknet/SendVictoryEmail.php?mail=[EMAIL]".Replace("[EMAIL]", this.email);
				WebClient webClient = new WebClient();
				string value = webClient.DownloadString(address);
				Console.WriteLine(value);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00054694 File Offset: 0x00052894
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			int num = bounds.X + 10;
			int num2 = bounds.Y + 10;
			TextItem.doFontLabel(new Vector2((float)num, (float)num2), "Email Verification", GuiData.font, null, (float)(bounds.Width - 20), (float)bounds.Height, false);
			num2 += 50;
			switch (this.state)
			{
			default:
				num2 += bounds.Height / 2 - 60;
				if (this.state == 3)
				{
					TextItem.doSmallLabel(new Vector2((float)num, (float)(num2 - 60)), "Error - Invalid Email Address", null);
				}
				if (Button.doButton(10, num, num2, 300, 50, "Add Email", new Color?(this.os.highlightColor)))
				{
					this.state = 1;
					this.os.execute("getString Email");
				}
				if (Button.doButton(12, num, num2 + 55, 300, 20, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.os.display.command = "connect";
				}
				break;
			case 1:
			{
				num2 += (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), "Enter a secure email address :\nEnsure that you are the only one with access to it", null).Y + 10;
				string[] separator = new string[]
				{
					"#$#$#$$#$&$#$#$#$#"
				};
				string[] array = this.os.getStringCache.Split(separator, StringSplitOptions.None);
				if (array.Length > 1)
				{
					this.email = array[1];
					if (this.email.Equals(""))
					{
						this.email = this.os.terminal.currentLine;
					}
				}
				Rectangle destinationRectangle = new Rectangle(num, num2, bounds.Width - 20, 200);
				sb.Draw(Utils.white, destinationRectangle, this.os.darkBackgroundColor);
				num2 += 80;
				destinationRectangle.X = num + (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), "Email: " + this.email, null).X + 2;
				destinationRectangle.Y = num2;
				destinationRectangle.Width = 7;
				destinationRectangle.Height = 20;
				if (this.os.timer % 1f < 0.3f)
				{
					sb.Draw(Utils.white, destinationRectangle, this.os.outlineColor);
				}
				num2 += 122;
				if (array.Length > 2 || Button.doButton(30, num, num2, 300, 22, "Confirm", new Color?(this.os.highlightColor)))
				{
					if (array.Length <= 2)
					{
						this.os.terminal.executeLine();
					}
					this.state = 2;
				}
				break;
			}
			case 2:
				num2 += 20;
				TextItem.doSmallLabel(new Vector2((float)num, (float)num2), "Confirm this Email Address :\n" + this.email, null);
				num2 += 60;
				if (Button.doButton(21, num, num2, 200, 50, "Confirm Email", new Color?(this.os.highlightColor)))
				{
					if (!Settings.isDemoMode)
					{
						this.sendEmail();
					}
					else
					{
						this.saveEmail();
					}
				}
				num += 220;
				if (Button.doButton(20, num, num2, 200, 50, "Re-Enter Email", null))
				{
					this.state = 1;
					this.email = "";
					this.os.execute("getString Email");
				}
				break;
			}
		}

		// Token: 0x0400060F RID: 1551
		private const string SEND_SCRIPT_URL = "http://www.tijital-games.com/hacknet/SendVictoryEmail.php?mail=[EMAIL]";

		// Token: 0x04000610 RID: 1552
		private const int WAITING = 0;

		// Token: 0x04000611 RID: 1553
		private const int ENTERING = 1;

		// Token: 0x04000612 RID: 1554
		private const int CONFIRM = 2;

		// Token: 0x04000613 RID: 1555
		private const int ERROR = 3;

		// Token: 0x04000614 RID: 1556
		private static string lastSentEmail;

		// Token: 0x04000615 RID: 1557
		private int state;

		// Token: 0x04000616 RID: 1558
		private string email;
	}
}
