using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace Hacknet.Magic
{
	// Token: 0x02000128 RID: 296
	public class NAT
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060006F7 RID: 1783 RVA: 0x00072BF4 File Offset: 0x00070DF4
		// (set) Token: 0x060006F8 RID: 1784 RVA: 0x00072C0B File Offset: 0x00070E0B
		public static TimeSpan TimeOut
		{
			get
			{
				return NAT._timeout;
			}
			set
			{
				NAT._timeout = value;
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x00072C14 File Offset: 0x00070E14
		public static bool Discover()
		{
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			socket.ReceiveTimeout = 5000;
			string s = "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1900\r\nST:upnp:rootdevice\r\nMAN:\"ssdp:discover\"\r\nMX:3\r\n\r\n";
			DebugLog.add(s);
			byte[] bytes = Encoding.ASCII.GetBytes(s);
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Broadcast, 1900);
			byte[] array = new byte[4096];
			DateTime now = DateTime.Now;
			string text;
			for (;;)
			{
				socket.SendTo(bytes, remoteEP);
				socket.SendTo(bytes, remoteEP);
				socket.SendTo(bytes, remoteEP);
				int num = 0;
				do
				{
					try
					{
						num = socket.Receive(array);
					}
					catch (Exception value)
					{
						Console.WriteLine(value);
						num = 0;
					}
					text = Encoding.ASCII.GetString(array, 0, num).ToLower();
					DebugLog.add(text);
					if (text.Contains("upnp:rootdevice"))
					{
						text = text.Substring(text.ToLower().IndexOf("location:") + 9);
						text = text.Substring(0, text.IndexOf("\r")).Trim();
						if (!string.IsNullOrEmpty(NAT._serviceUrl = NAT.GetServiceUrl(text)))
						{
							goto Block_3;
						}
					}
				}
				while (num > 0);
				if (!(now.Subtract(DateTime.Now) < NAT._timeout))
				{
					goto Block_5;
				}
			}
			Block_3:
			NAT._descUrl = text;
			return true;
			Block_5:
			return false;
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00072D9C File Offset: 0x00070F9C
		private static string GetServiceUrl(string resp)
		{
			string result;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(WebRequest.Create(resp).GetResponse().GetResponseStream());
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
				xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
				XmlNode xmlNode = xmlDocument.SelectSingleNode("//tns:device/tns:deviceType/text()", xmlNamespaceManager);
				if (!xmlNode.Value.Contains("InternetGatewayDevice"))
				{
					result = null;
				}
				else
				{
					XmlNode xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", xmlNamespaceManager);
					if (xmlNode2 == null)
					{
						xmlNode2 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANPPPConnection:1\"]/tns:controlURL/text()", xmlNamespaceManager);
						if (xmlNode2 == null)
						{
							result = null;
						}
						else
						{
							XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANPPPConnection:1\"]/tns:eventSubURL/text()", xmlNamespaceManager);
							NAT._eventUrl = NAT.CombineUrls(resp, xmlNode3.Value);
							result = NAT.CombineUrls(resp, xmlNode2.Value);
						}
					}
					else
					{
						XmlNode xmlNode3 = xmlDocument.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:eventSubURL/text()", xmlNamespaceManager);
						NAT._eventUrl = NAT.CombineUrls(resp, xmlNode3.Value);
						result = NAT.CombineUrls(resp, xmlNode2.Value);
					}
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060006FB RID: 1787 RVA: 0x00072ED4 File Offset: 0x000710D4
		private static string CombineUrls(string resp, string p)
		{
			int num = resp.IndexOf("://");
			num = resp.IndexOf('/', num + 3);
			return resp.Substring(0, num) + p;
		}

		// Token: 0x060006FC RID: 1788 RVA: 0x00072F0C File Offset: 0x0007110C
		public static void ForwardPort(int port, ProtocolType protocol, string description)
		{
			if (string.IsNullOrEmpty(NAT._serviceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			XmlDocument xmlDocument = NAT.SOAPRequest(NAT._serviceUrl, string.Concat(new string[]
			{
				"<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
				port.ToString(),
				"</NewExternalPort><NewProtocol>",
				protocol.ToString().ToUpper(),
				"</NewProtocol><NewInternalPort>",
				port.ToString(),
				"</NewInternalPort><NewInternalClient>",
				Dns.GetHostAddresses(Dns.GetHostName())[0].ToString(),
				"</NewInternalClient><NewEnabled>1</NewEnabled><NewPortMappingDescription>",
				description,
				"</NewPortMappingDescription><NewLeaseDuration>0</NewLeaseDuration></u:AddPortMapping>"
			}), "AddPortMapping");
		}

		// Token: 0x060006FD RID: 1789 RVA: 0x00072FC8 File Offset: 0x000711C8
		public static void DeleteForwardingRule(int port, ProtocolType protocol)
		{
			if (string.IsNullOrEmpty(NAT._serviceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			XmlDocument xmlDocument = NAT.SOAPRequest(NAT._serviceUrl, string.Concat(new object[]
			{
				"<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"><NewRemoteHost></NewRemoteHost><NewExternalPort>",
				port,
				"</NewExternalPort><NewProtocol>",
				protocol.ToString().ToUpper(),
				"</NewProtocol></u:DeletePortMapping>"
			}), "DeletePortMapping");
		}

		// Token: 0x060006FE RID: 1790 RVA: 0x00073044 File Offset: 0x00071244
		public static IPAddress GetExternalIP()
		{
			if (string.IsNullOrEmpty(NAT._serviceUrl))
			{
				throw new Exception("No UPnP service available or Discover() has not been called");
			}
			XmlDocument xmlDocument = NAT.SOAPRequest(NAT._serviceUrl, "<u:GetExternalIPAddress xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\"></u:GetExternalIPAddress>", "GetExternalIPAddress");
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
			xmlNamespaceManager.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
			string value = xmlDocument.SelectSingleNode("//NewExternalIPAddress/text()", xmlNamespaceManager).Value;
			return IPAddress.Parse(value);
		}

		// Token: 0x060006FF RID: 1791 RVA: 0x000730C0 File Offset: 0x000712C0
		private static XmlDocument SOAPRequest(string url, string soap, string function)
		{
			string s = "<?xml version=\"1.0\"?><s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"><s:Body>" + soap + "</s:Body></s:Envelope>";
			WebRequest webRequest = WebRequest.Create(url);
			webRequest.Method = "POST";
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			webRequest.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
			webRequest.ContentType = "text/xml; charset=\"utf-8\"";
			webRequest.ContentLength = (long)bytes.Length;
			webRequest.GetRequestStream().Write(bytes, 0, bytes.Length);
			XmlDocument xmlDocument = new XmlDocument();
			WebResponse response = webRequest.GetResponse();
			Stream responseStream = response.GetResponseStream();
			xmlDocument.Load(responseStream);
			return xmlDocument;
		}

		// Token: 0x040007D8 RID: 2008
		private static TimeSpan _timeout = new TimeSpan(0, 0, 0, 3);

		// Token: 0x040007D9 RID: 2009
		private static string _descUrl;

		// Token: 0x040007DA RID: 2010
		private static string _serviceUrl;

		// Token: 0x040007DB RID: 2011
		private static string _eventUrl;
	}
}
