using System;
using System.Collections.Generic;
using Hacknet.ExternalCounterparts;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000161 RID: 353
	internal class ServerScreen : GameScreen
	{
		// Token: 0x060008E8 RID: 2280 RVA: 0x000943B8 File Offset: 0x000925B8
		public ServerScreen()
		{
			this.mainIP = MultiplayerLobby.getLocalIP();
			this.messages = new List<string>();
			this.server = new ExternalNetworkedServer();
			this.server.initializeListener();
			ExternalNetworkedServer externalNetworkedServer = this.server;
			externalNetworkedServer.messageReceived = (Action<string>)Delegate.Combine(externalNetworkedServer.messageReceived, new Action<string>(this.parseMessage));
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00094437 File Offset: 0x00092637
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			this.canCloseServer = (!otherScreenHasFocus && !coveredByOtherScreen);
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00094455 File Offset: 0x00092655
		public override void HandleInput(InputState input)
		{
			base.HandleInput(input);
			GuiData.doInput(input);
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x00094468 File Offset: 0x00092668
		public void parseMessage(string msg)
		{
			this.addDisplayMessage(msg);
			char[] separator = new char[]
			{
				' ',
				'\n'
			};
			string[] array = msg.Split(separator);
			if (array[0].Equals("cCDDrive"))
			{
				if (array[2].Equals("open"))
				{
					Programs.cdDrive(true);
				}
				else
				{
					Programs.cdDrive(false);
				}
			}
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x000944D7 File Offset: 0x000926D7
		public void addDisplayMessage(string msg)
		{
			this.messages.Add(msg);
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x000944E8 File Offset: 0x000926E8
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			if (this.drawingWithEffects)
			{
				PostProcessor.begin();
			}
			GuiData.startDraw();
			Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
			GuiData.spriteBatch.Draw(Utils.white, new Rectangle(0, 0, viewport.Width, viewport.Height), this.backgroundColor);
			int num = 80;
			int num2 = 80;
			TextItem.doFontLabel(new Vector2((float)num, (float)num2), "HACKNET RELAY SERVER", GuiData.titlefont, null, 500f, 50f, false);
			num2 += 55;
			if (this.canCloseServer && Button.doButton(800, num, num2, 160, 30, "Shut Down Server", null))
			{
				this.server.closeServer();
				base.ExitScreen();
			}
			num2 += 35;
			for (int i = 0; i < MultiplayerLobby.allLocalIPs.Count; i++)
			{
				TextItem.doFontLabel(new Vector2((float)num, (float)num2), "IP: " + MultiplayerLobby.allLocalIPs[i], GuiData.smallfont, null, float.MaxValue, float.MaxValue, false);
				num2 += 20;
			}
			num2 += 30;
			this.drawMessageLog(num, num2);
			GuiData.endDraw();
			if (this.drawingWithEffects)
			{
				PostProcessor.end();
			}
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x00094668 File Offset: 0x00092868
		public void drawMessageLog(int x, int y)
		{
			float scale = 1f;
			Vector2 pos = new Vector2((float)x, (float)y);
			int num = 0;
			while (num < 20 && num < this.messages.Count)
			{
				if (num > 10)
				{
					scale = 1f - (float)(num - 10) / 10f;
				}
				TextItem.doTinyLabel(pos, this.messages[this.messages.Count - 1 - num], new Color?(Color.White * scale));
				pos.Y += 13f;
				num++;
			}
		}

		// Token: 0x04000A58 RID: 2648
		private string mainIP;

		// Token: 0x04000A59 RID: 2649
		private List<string> messages;

		// Token: 0x04000A5A RID: 2650
		private Color backgroundColor = new Color(6, 6, 6);

		// Token: 0x04000A5B RID: 2651
		private ExternalNetworkedServer server;

		// Token: 0x04000A5C RID: 2652
		private bool canCloseServer;

		// Token: 0x04000A5D RID: 2653
		public bool drawingWithEffects = false;
	}
}
