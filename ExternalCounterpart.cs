using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hacknet
{
	// Token: 0x02000160 RID: 352
	public class ExternalCounterpart
	{
		// Token: 0x060008DE RID: 2270 RVA: 0x000940BC File Offset: 0x000922BC
		public static string getIPForServerName(string serverName)
		{
			if (ExternalCounterpart.networkIPList == null)
			{
				ExternalCounterpart.loadNetIPList();
			}
			if (ExternalCounterpart.networkIPList.ContainsKey(serverName))
			{
				return ExternalCounterpart.networkIPList[serverName];
			}
			throw new InvalidOperationException("Server Name Not Found");
		}

		// Token: 0x060008DF RID: 2271 RVA: 0x00094110 File Offset: 0x00092310
		private static void loadNetIPList()
		{
			ExternalCounterpart.networkIPList = new Dictionary<string, string>();
			string[] array = Utils.readEntireFile("Content/Network/NetworkIPList.txt").Split(new string[]
			{
				"\n\r",
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(Utils.spaceDelim);
				ExternalCounterpart.networkIPList.Add(array2[0], array2[1]);
			}
		}

		// Token: 0x060008E0 RID: 2272 RVA: 0x00094188 File Offset: 0x00092388
		public ExternalCounterpart(string idName, string ipEndpoint)
		{
			this.idName = idName;
			this.connectionIP = ipEndpoint;
			this.buffer = new byte[4096];
			if (ExternalCounterpart.encoder == null)
			{
				ExternalCounterpart.encoder = new ASCIIEncoding();
			}
		}

		// Token: 0x060008E1 RID: 2273 RVA: 0x000941E0 File Offset: 0x000923E0
		public void sendMessage(string message)
		{
			if (this.isConnected)
			{
				this.writeMessage(message);
			}
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00094208 File Offset: 0x00092408
		public void disconnect()
		{
			if (this.isConnected)
			{
				this.writeMessage("Disconnecting");
				this.connection.Close();
				this.isConnected = false;
			}
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x00094244 File Offset: 0x00092444
		public void testConnection()
		{
			this.establishConnection();
			while (!this.isConnected)
			{
				Thread.Sleep(5);
			}
			this.writeMessage("Test Message From " + this.idName);
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00094288 File Offset: 0x00092488
		public void establishConnection()
		{
			TcpClient tcpClient = new TcpClient();
			tcpClient.BeginConnect(IPAddress.Parse(this.connectionIP), Multiplayer.PORT, new AsyncCallback(this.DoTcpConnectionCallback), tcpClient);
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x000942C0 File Offset: 0x000924C0
		private void DoTcpConnectionCallback(IAsyncResult ar)
		{
			TcpClient tcpClient = ar.AsyncState as TcpClient;
			if (tcpClient != null && tcpClient.Connected)
			{
				this.isConnected = true;
				this.connection = tcpClient;
				tcpClient.EndConnect(ar);
			}
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x00094308 File Offset: 0x00092508
		public void writeMessage(string message)
		{
			if (this.isConnected)
			{
				NetworkStream stream = this.connection.GetStream();
				this.buffer = ExternalCounterpart.encoder.GetBytes(message);
				stream.BeginWrite(this.buffer, 0, this.buffer.Length, new AsyncCallback(this.TCPWriteMessageCallback), stream);
			}
			else
			{
				this.establishConnection();
			}
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00094370 File Offset: 0x00092570
		private void TCPWriteMessageCallback(IAsyncResult ar)
		{
			NetworkStream networkStream = ar.AsyncState as NetworkStream;
			if (networkStream != null)
			{
				try
				{
					networkStream.EndWrite(ar);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x04000A51 RID: 2641
		private static Dictionary<string, string> networkIPList;

		// Token: 0x04000A52 RID: 2642
		private static ASCIIEncoding encoder;

		// Token: 0x04000A53 RID: 2643
		public string idName;

		// Token: 0x04000A54 RID: 2644
		public string connectionIP;

		// Token: 0x04000A55 RID: 2645
		public bool isConnected = false;

		// Token: 0x04000A56 RID: 2646
		private TcpClient connection;

		// Token: 0x04000A57 RID: 2647
		private byte[] buffer;
	}
}
