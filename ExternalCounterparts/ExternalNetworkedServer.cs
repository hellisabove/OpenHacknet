using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hacknet.ExternalCounterparts
{
	// Token: 0x0200010C RID: 268
	public class ExternalNetworkedServer
	{
		// Token: 0x06000659 RID: 1625 RVA: 0x00069E10 File Offset: 0x00068010
		public ExternalNetworkedServer()
		{
			this.connections = new List<TcpClient>();
			this.buffers = new Dictionary<NetworkStream, byte[]>();
			if (ExternalNetworkedServer.encoder == null)
			{
				ExternalNetworkedServer.encoder = new ASCIIEncoding();
			}
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00069E58 File Offset: 0x00068058
		public void initializeListener()
		{
			this.listener = new TcpListener(new IPEndPoint(IPAddress.Any, Multiplayer.PORT));
			this.listener.Start();
			this.listener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpConnectionCallback), this.listener);
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x00069EAC File Offset: 0x000680AC
		public void closeServer()
		{
			this.listener.Stop();
			foreach (TcpClient tcpClient in this.connections)
			{
				tcpClient.GetStream().Close();
				tcpClient.Close();
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x00069F20 File Offset: 0x00068120
		private void AcceptTcpConnectionCallback(IAsyncResult ar)
		{
			TcpListener tcpListener = ar.AsyncState as TcpListener;
			if (tcpListener != null)
			{
				try
				{
					TcpClient tcpClient = this.listener.EndAcceptTcpClient(ar);
					if (tcpClient != null)
					{
						this.connections.Add(tcpClient);
						NetworkStream stream = tcpClient.GetStream();
						this.buffers.Add(stream, new byte[4096]);
						if (this.messageReceived != null)
						{
							this.messageReceived("Connection");
						}
						tcpClient.GetStream().BeginRead(this.buffers[stream], 0, 4096, new AsyncCallback(this.TcpReadCallback), stream);
						tcpListener.BeginAcceptTcpClient(new AsyncCallback(this.AcceptTcpConnectionCallback), tcpListener);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0006A004 File Offset: 0x00068204
		private void TcpReadCallback(IAsyncResult ar)
		{
			NetworkStream networkStream = ar.AsyncState as NetworkStream;
			if (networkStream != null)
			{
				try
				{
					int num = networkStream.EndRead(ar);
					if (num != 0)
					{
						byte[] bytes = this.buffers[networkStream];
						string @string = ExternalNetworkedServer.encoder.GetString(bytes, 0, num);
						if (this.messageReceived != null)
						{
							this.messageReceived(@string);
						}
						networkStream.BeginRead(this.buffers[networkStream], 0, 4096, new AsyncCallback(this.TcpReadCallback), networkStream);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x0400071E RID: 1822
		private const int BUFFER_SIZE = 4096;

		// Token: 0x0400071F RID: 1823
		public Action<string> messageReceived;

		// Token: 0x04000720 RID: 1824
		private static ASCIIEncoding encoder;

		// Token: 0x04000721 RID: 1825
		private List<TcpClient> connections;

		// Token: 0x04000722 RID: 1826
		private Dictionary<NetworkStream, byte[]> buffers;

		// Token: 0x04000723 RID: 1827
		private TcpListener listener;
	}
}
