using System;
using System.Net;

namespace Hacknet
{
	// Token: 0x02000108 RID: 264
	internal class OnlineWebServerDaemon : WebServerDaemon
	{
		// Token: 0x06000633 RID: 1587 RVA: 0x00066011 File Offset: 0x00064211
		public OnlineWebServerDaemon(Computer computer, string serviceName, OS opSystem) : base(computer, serviceName, opSystem, "Content/Web/BaseImageWebPage.html")
		{
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00066036 File Offset: 0x00064236
		public void setURL(string url)
		{
			this.webURL = url;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x00066040 File Offset: 0x00064240
		public override void LoadWebPage(string url = null)
		{
			if (url == null || url == "index.html")
			{
				url = this.webURL;
			}
			WebClient webClient = new WebClient();
			webClient.DownloadStringAsync(new Uri(url));
			webClient.DownloadStringCompleted += this.web_DownloadStringCompleted;
			this.lastRequestedURL = url;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0006609C File Offset: 0x0006429C
		private void web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				this.instantiateWebPage(e.Result);
			}
			else
			{
				string body = FileSanitiser.purifyStringForDisplay(Utils.readEntireFile("Content/Web/404Page.html"));
				this.instantiateWebPage(body);
			}
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x000660F4 File Offset: 0x000642F4
		private void instantiateWebPage(string body)
		{
			string data = FileSanitiser.purifyStringForDisplay(body);
			string text = this.lastRequestedURL;
			FileEntry fileEntry = this.root.searchForFile(text);
			if (fileEntry == null)
			{
				fileEntry = this.root.searchForFile("index.html");
				text = "index.html";
			}
			if (fileEntry != null)
			{
				fileEntry.data = data;
			}
			base.LoadWebPage(text);
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00066158 File Offset: 0x00064358
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<OnlineWebServer name=\"",
				this.name,
				"\" url=\"",
				this.webURL,
				"\" />"
			});
		}

		// Token: 0x040006F2 RID: 1778
		private static string DEFAULT_PAGE_URL = "http://www.google.com";

		// Token: 0x040006F3 RID: 1779
		public string webURL = OnlineWebServerDaemon.DEFAULT_PAGE_URL;

		// Token: 0x040006F4 RID: 1780
		private string lastRequestedURL = null;
	}
}
