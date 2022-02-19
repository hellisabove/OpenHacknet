using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Hacknet.Gui;
using Hacknet.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200013F RID: 319
	internal class MultiplayerLobby : GameScreen
	{
		// Token: 0x06000797 RID: 1943 RVA: 0x0007CF80 File Offset: 0x0007B180
		public MultiplayerLobby()
		{
			base.TransitionOnTime = TimeSpan.FromSeconds(0.1);
			base.TransitionOffTime = TimeSpan.FromSeconds(0.3);
		}

		// Token: 0x06000798 RID: 1944 RVA: 0x0007D030 File Offset: 0x0007B230
		public override void LoadContent()
		{
			base.LoadContent();
			try
			{
				this.listener = new TcpListener(IPAddress.Any, Multiplayer.PORT);
				this.listener.Start();
				this.listener.BeginAcceptTcpClient(new AsyncCallback(this.DoAcceptTcpClientCallback), this.listener);
				this.buffer = new byte[4096];
				this.encoder = new ASCIIEncoding();
				if (MultiplayerLobby.allLocalIPs.Count == 0)
				{
					this.myIP = MultiplayerLobby.getLocalIP();
					Thread thread = new Thread(new ThreadStart(this.getExternalIP));
					thread.Start();
					Console.WriteLine("Started Multiplayer IP Getter Thread");
				}
				else
				{
					this.myIP = MultiplayerLobby.allLocalIPs[0];
				}
				this.messages = new List<string>();
				Viewport viewport = base.ScreenManager.GraphicsDevice.Viewport;
				this.fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
			}
			catch (Exception ex)
			{
				base.ScreenManager.RemoveScreen(this);
				base.ScreenManager.AddScreen(new MainMenu(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
			}
		}

		// Token: 0x06000799 RID: 1945 RVA: 0x0007D178 File Offset: 0x0007B378
		public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
		{
			base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
			if (this.shouldAddServer && base.ScreenState != ScreenState.TransitionOff)
			{
				base.ExitScreen();
				base.ScreenManager.AddScreen(new OS(this.client, this.clientStream, true, base.ScreenManager), new PlayerIndex?(base.ScreenManager.controllingPlayer));
				if (this.chatclient != null)
				{
					this.chatclient.Close();
				}
			}
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x0007D201 File Offset: 0x0007B401
		public override void HandleInput(InputState input)
		{
			base.HandleInput(input);
			GuiData.doInput(input);
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x0007D214 File Offset: 0x0007B414
		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
			GuiData.startDraw();
			base.ScreenManager.SpriteBatch.Draw(Utils.white, this.fullscreen, this.isConnecting ? Color.Gray : Color.Black);
			this.doGui();
			GuiData.endDraw();
			base.ScreenManager.FadeBackBufferToBlack((int)(byte.MaxValue - base.TransitionAlpha));
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x0007D288 File Offset: 0x0007B488
		public void drawConnectingGui()
		{
			PatternDrawer.draw(new Rectangle(100, 200, 600, 250), 1.4f, Color.DarkGreen * 0.3f, Color.DarkGreen, GuiData.spriteBatch);
			TextItem.doLabel(new Vector2(110f, 210f), "Connecting...", null);
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x0007D2F4 File Offset: 0x0007B4F4
		public void doGui()
		{
			PatternDrawer.draw(new Rectangle(180, 160, 500, 85), 1f, this.darkgrey, this.dark_ish_gray, GuiData.spriteBatch);
			TextItem.doSmallLabel(new Vector2(200f, 170f), "IP To Connect to:", null);
			this.destination = TextBox.doTextBox(100, 200, 200, 300, 1, this.destination, GuiData.smallfont);
			if (Button.doButton(123, 510, 201, 120, 23, "Connect", null) || TextBox.BoxWasActivated)
			{
				this.ConnectToServer(this.destination);
			}
			else
			{
				if (this.isConnecting)
				{
					this.drawConnectingGui();
				}
				TextItem.doLabel(new Vector2(200f, 300f), "Local IPs: " + this.myIP, null);
				float num = 340f;
				for (int i = 0; i < MultiplayerLobby.allLocalIPs.Count - 1; i++)
				{
					TextItem.doLabel(new Vector2(351f, num), MultiplayerLobby.allLocalIPs[i], null);
					num += 40f;
				}
				num += 40f;
				TextItem.doLabel(new Vector2(200f, num), "Extrn IP: " + this.externalIP, null);
				Vector2 pos = new Vector2(610f, 280f);
				TextItem.doLabel(pos, "Info:", null);
				pos.Y += 40f;
				string text = "To Begin a multiplayer session, type in the IP of the computer you want to connect to and press enter or connect. Both players must be on this screen. To connect over the internet, use the extern IP address and ensure port 3030 is open.";
				text = DisplayModule.cleanSplitForWidth(text, 400);
				TextItem.doFontLabel(pos, text, GuiData.tinyfont, new Color?(Color.DarkGray), float.MaxValue, float.MaxValue, false);
				if (Button.doButton(999, 10, 10, 200, 30, "<- Back to Menu", new Color?(Color.Gray)))
				{
					base.ExitScreen();
					base.ScreenManager.AddScreen(new MainMenu(), new PlayerIndex?(base.ScreenManager.controllingPlayer));
				}
			}
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x0007D560 File Offset: 0x0007B760
		public void ConnectToServer(string ip)
		{
			this.connectingToServer = true;
			try
			{
				TcpClient tcpClient = new TcpClient();
				IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), Multiplayer.PORT);
				tcpClient.Connect(remoteEP);
				NetworkStream stream = tcpClient.GetStream();
				this.buffer = this.encoder.GetBytes("connect Client Connecting");
				stream.Write(this.buffer, 0, this.buffer.Length);
				stream.Flush();
				stream.Read(this.buffer, 0, this.buffer.Length);
				base.ExitScreen();
				base.ScreenManager.AddScreen(new OS(tcpClient, stream, false, base.ScreenManager), new PlayerIndex?(base.ScreenManager.controllingPlayer));
			}
			catch (Exception ex)
			{
				DebugLog.add(ex.ToString());
				this.isConnecting = false;
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x0007D644 File Offset: 0x0007B844
		public void chatToServer(string ip, string msg)
		{
			try
			{
				if (this.chatclientStream == null)
				{
					this.chatclient = new TcpClient();
					IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(ip), Multiplayer.PORT);
					this.chatclient.Connect(remoteEP);
					this.chatclientStream = this.chatclient.GetStream();
				}
				this.buffer = this.encoder.GetBytes("Chat " + msg);
				this.messages.Add("Me: " + msg);
				this.chatclientStream.Write(this.buffer, 0, this.buffer.Length);
				this.chatclientStream.Flush();
				this.messageString = "";
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x0007D724 File Offset: 0x0007B924
		public void DoAcceptTcpClientCallback(IAsyncResult ar)
		{
			TcpListener tcpListener = (TcpListener)ar.AsyncState;
			try
			{
				this.client = tcpListener.EndAcceptTcpClient(ar);
				this.clientStream = this.client.GetStream();
				bool flag = true;
				while (flag)
				{
					int num = 0;
					try
					{
						num = this.clientStream.Read(this.buffer, 0, 4096);
					}
					catch (Exception value)
					{
						Console.WriteLine(value);
						if (Game1.threadsExiting)
						{
							break;
						}
					}
					if (num == 0)
					{
						this.client.Close();
						if (Game1.threadsExiting)
						{
							break;
						}
					}
					string @string = this.encoder.GetString(this.buffer);
					char[] separator = new char[]
					{
						' '
					};
					string[] array = @string.Split(separator);
					if (!array[0].Equals("Chat"))
					{
						this.buffer = this.encoder.GetBytes("Replying - Hello World");
						this.clientStream.Write(this.buffer, 0, this.buffer.Length);
						this.clientStream.Flush();
						this.shouldAddServer = true;
						flag = false;
						break;
					}
					char[] array2 = new char[3];
					array2[0] = ' ';
					array2[1] = '\n';
					char[] trimChars = array2;
					this.messages.Add(@string.Substring(4).Trim(trimChars).Replace("\0", ""));
					if (Game1.threadsExiting)
					{
						break;
					}
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x0007D908 File Offset: 0x0007BB08
		private void listenForConnections()
		{
			try
			{
				this.listener = new TcpListener(IPAddress.Any, Multiplayer.PORT);
				this.listener.Start();
				this.client = this.listener.AcceptTcpClient();
				this.clientStream = this.client.GetStream();
				bool flag = true;
				while (flag)
				{
					int num = 0;
					try
					{
						num = this.clientStream.Read(this.buffer, 0, 4096);
					}
					catch (Exception value)
					{
						Console.WriteLine(value);
						if (Game1.threadsExiting)
						{
							break;
						}
					}
					if (num == 0)
					{
						this.client.Close();
						if (Game1.threadsExiting)
						{
							break;
						}
					}
					string @string = this.encoder.GetString(this.buffer);
					char[] separator = new char[]
					{
						' '
					};
					string[] array = @string.Split(separator);
					if (!array[0].Equals("Chat"))
					{
						this.buffer = this.encoder.GetBytes("Replying - Hello World");
						this.clientStream.Write(this.buffer, 0, this.buffer.Length);
						this.clientStream.Flush();
						this.shouldAddServer = true;
						flag = false;
						break;
					}
					char[] array2 = new char[3];
					array2[0] = ' ';
					array2[1] = '\n';
					char[] trimChars = array2;
					this.messages.Add(@string.Substring(4).Trim(trimChars).Replace("\0", ""));
					if (Game1.threadsExiting)
					{
						break;
					}
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x0007DB04 File Offset: 0x0007BD04
		public static string getLocalIP()
		{
			string result = "?";
			IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
			foreach (IPAddress ipaddress in hostEntry.AddressList)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					result = ipaddress.ToString();
					MultiplayerLobby.allLocalIPs.Add(ipaddress.ToString());
				}
			}
			return result;
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x0007DB80 File Offset: 0x0007BD80
		public void getExternalIPwithPF()
		{
			try
			{
				this.externalIP = "Attempting automated port-fowarding...";
				try
				{
					if (NAT.Discover())
					{
						Console.WriteLine("Attempting port foward");
						NAT.ForwardPort(Multiplayer.PORT, ProtocolType.Tcp, "Hacknet (TCP)");
						this.externalIP = NAT.GetExternalIP().ToString();
					}
					else
					{
						base.ScreenManager.ShowPopup("You dont have UPNP enabled - Internet play will not work");
					}
				}
				catch (Exception value)
				{
					Console.WriteLine(value);
				}
				if (this.externalIP.Equals("Attempting automated port-fowarding..."))
				{
					this.externalIP = "Automated port-fowarding Failed - Internet Play Disabled";
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				this.externalIP = "Automated port-fowarding Failed - Internet Play Disabled";
			}
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x0007DC58 File Offset: 0x0007BE58
		public void getExternalIP()
		{
			try
			{
				this.externalIP = "Unknown...";
				WebClient webClient = new WebClient();
				this.externalIP = webClient.DownloadString("http://icanhazip.com/");
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
				this.externalIP = "Could not Find Conection";
			}
		}

		// Token: 0x0400089C RID: 2204
		private Rectangle fullscreen;

		// Token: 0x0400089D RID: 2205
		private string destination = "192.168.1.1";

		// Token: 0x0400089E RID: 2206
		private string myIP = "?";

		// Token: 0x0400089F RID: 2207
		private string externalIP = "Loading...";

		// Token: 0x040008A0 RID: 2208
		public static List<string> allLocalIPs = new List<string>();

		// Token: 0x040008A1 RID: 2209
		private TcpListener listener;

		// Token: 0x040008A2 RID: 2210
		private Thread listenerThread;

		// Token: 0x040008A3 RID: 2211
		private byte[] buffer;

		// Token: 0x040008A4 RID: 2212
		private ASCIIEncoding encoder;

		// Token: 0x040008A5 RID: 2213
		private bool shouldAddServer = false;

		// Token: 0x040008A6 RID: 2214
		private TcpClient client;

		// Token: 0x040008A7 RID: 2215
		private NetworkStream clientStream;

		// Token: 0x040008A8 RID: 2216
		private bool isConnecting = false;

		// Token: 0x040008A9 RID: 2217
		private List<string> messages;

		// Token: 0x040008AA RID: 2218
		private string chatIP = "";

		// Token: 0x040008AB RID: 2219
		private string messageString = "Test Message";

		// Token: 0x040008AC RID: 2220
		private TcpClient chatclient;

		// Token: 0x040008AD RID: 2221
		private NetworkStream chatclientStream;

		// Token: 0x040008AE RID: 2222
		private Color darkgrey = new Color(9, 9, 9);

		// Token: 0x040008AF RID: 2223
		private Color dark_ish_gray = new Color(20, 20, 20);

		// Token: 0x040008B0 RID: 2224
		private bool connectingToServer = false;
	}
}
