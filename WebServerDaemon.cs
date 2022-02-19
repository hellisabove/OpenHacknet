using System;
using System.IO;
using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000107 RID: 263
	internal class WebServerDaemon : Daemon
	{
		// Token: 0x06000628 RID: 1576 RVA: 0x000658E0 File Offset: 0x00063AE0
		public WebServerDaemon(Computer computer, string serviceName, OS opSystem, string pageFileLocation = "Content/Web/BaseImageWebPage.html") : base(computer, serviceName, opSystem)
		{
			if (pageFileLocation != null)
			{
				this.webPageFileLocation = pageFileLocation;
				if (this.webPageFileLocation != "Content/Web/BaseImageWebPage.html")
				{
					WebServerDaemon.BaseWebpageData = null;
				}
			}
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x00065940 File Offset: 0x00063B40
		public override void initFiles()
		{
			this.root = this.comp.files.root.searchForFolder("web");
			if (this.root == null)
			{
				this.root = new Folder("web");
				this.comp.files.root.folders.Add(this.root);
			}
			if (WebServerDaemon.BaseWebpageData == null)
			{
				if (Settings.IsInExtensionMode)
				{
					this.webPageFileLocation = Utils.GetFileLoadPrefix() + this.webPageFileLocation;
				}
				string text = Utils.readEntireFile(this.webPageFileLocation);
				WebServerDaemon.BaseWebpageData = ((Settings.ActiveLocale == "en-us") ? FileSanitiser.purifyStringForDisplay(text) : text);
			}
			FileEntry item = new FileEntry(WebServerDaemon.BaseWebpageData, "index.html");
			this.root.files.Add(item);
			this.lastLoadedFile = item;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x00065A3D File Offset: 0x00063C3D
		public override void loadInit()
		{
			this.root = this.comp.files.root.searchForFolder("web");
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x00065A60 File Offset: 0x00063C60
		public void generateBaseCorporateSite(string companyName, string targetBaseFile = "Content/Web/BaseCorporatePage.html")
		{
			if (WebServerDaemon.BaseComnayPageData == null || targetBaseFile != "Content/Web/BaseCorporatePage.html")
			{
				WebServerDaemon.BaseComnayPageData = FileSanitiser.purifyStringForDisplay(Utils.readEntireFile(targetBaseFile));
			}
			string text = WebServerDaemon.BaseComnayPageData.Replace("#$#COMPANYNAME#$#", companyName).Replace("#$#LC_COMPANYNAME#$#", companyName.Replace(' ', '_'));
			FileEntry fileEntry = this.root.searchForFile("index.html");
			if (fileEntry == null)
			{
				fileEntry = new FileEntry(text, "index.html");
			}
			else
			{
				fileEntry.data = text;
			}
			FileEntry item = new FileEntry(fileEntry.data, "index_BACKUP.html");
			Folder folder = this.comp.files.root.searchForFolder("home");
			folder.files.Add(item);
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00065B30 File Offset: 0x00063D30
		public virtual void LoadWebPage(string url = "index.html")
		{
			this.saveURL = url;
			FileEntry fileEntry = this.root.searchForFile(url);
			if (fileEntry != null)
			{
				this.shouldShow404 = false;
				this.ShowPage(fileEntry.data);
			}
			else
			{
				this.shouldShow404 = true;
			}
			this.lastLoadedFile = fileEntry;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00065B80 File Offset: 0x00063D80
		public virtual void ShowPage(string pageData)
		{
			string text = Directory.GetCurrentDirectory() + "/Content/Web/Cache/HN_OS_WebCache.html";
			if (Settings.IsInExtensionMode)
			{
				string text2 = Path.Combine(Directory.GetCurrentDirectory(), ExtensionLoader.ActiveExtensionInfo.FolderPath, "Web", "Cache");
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				text = text2 + "/HN_OS_WebCache.html";
			}
			Utils.writeToFile(pageData, text);
			WebRenderer.navigateTo(text);
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00065C14 File Offset: 0x00063E14
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			WebServerDaemon.<>c__DisplayClass2 CS$<>8__locals1 = new WebServerDaemon.<>c__DisplayClass2();
			CS$<>8__locals1.sb = sb;
			CS$<>8__locals1.webRect = bounds;
			WebServerDaemon.<>c__DisplayClass2 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.webRect.Height = CS$<>8__locals2.webRect.Height - 16;
			WebServerDaemon.<>c__DisplayClass2 CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.webRect.X = CS$<>8__locals3.webRect.X + 1;
			WebServerDaemon.<>c__DisplayClass2 CS$<>8__locals4 = CS$<>8__locals1;
			CS$<>8__locals4.webRect.Width = CS$<>8__locals4.webRect.Width - 2;
			if (this.shouldShow404)
			{
				Vector2 value = new Vector2((float)(CS$<>8__locals1.webRect.X + 10), (float)(CS$<>8__locals1.webRect.Y + CS$<>8__locals1.webRect.Height / 4));
				CS$<>8__locals1.sb.Draw(Utils.white, new Rectangle(CS$<>8__locals1.webRect.X, (int)value.Y, (int)((double)CS$<>8__locals1.webRect.Width * 0.7), 80), this.os.highlightColor * 0.3f);
				TextItem.doFontLabel(value + new Vector2(0f, 10f), LocaleTerms.Loc("Error 404"), GuiData.font, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
				TextItem.doFontLabel(value + new Vector2(0f, 42f), LocaleTerms.Loc("Page not found"), GuiData.smallfont, new Color?(Color.White), float.MaxValue, float.MaxValue, false);
			}
			else
			{
				OS os = this.os;
				os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
				{
					WebRenderer.drawTo(CS$<>8__locals1.webRect, CS$<>8__locals1.sb);
				}));
			}
			Rectangle rectangle = bounds;
			rectangle.Y += CS$<>8__locals1.webRect.Height;
			rectangle.Height = 16;
			bool smallButtonDraw = Button.smallButtonDraw;
			Button.smallButtonDraw = true;
			int num = 200;
			if (Button.doButton(83801, rectangle.X + 1, rectangle.Y + 1, num, rectangle.Height - 2, "HN: " + LocaleTerms.Loc("Exit Web View"), null))
			{
				this.os.display.command = "connect";
			}
			if (this.os.hasConnectionPermission(false) && Button.doButton(83805, rectangle.X + rectangle.Width - (num + 1), rectangle.Y + 1, num, rectangle.Height - 2, LocaleTerms.Loc("View Source"), null))
			{
				this.showSourcePressed();
			}
			Button.smallButtonDraw = smallButtonDraw;
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00065F00 File Offset: 0x00064100
		public virtual void showSourcePressed()
		{
			string text = "";
			int count = Programs.getNavigationPathAtPath("", this.os, this.root).Count;
			for (int i = 0; i < count; i++)
			{
				text += "../";
			}
			text += "web";
			this.os.runCommand("cd " + text);
			if (this.lastLoadedFile != null)
			{
				this.os.delayer.Post(ActionDelayer.Wait(0.1), delegate
				{
					this.os.runCommand("cat " + this.lastLoadedFile.name);
				});
			}
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00065FB7 File Offset: 0x000641B7
		public override void navigatedTo()
		{
			this.LoadWebPage("index.html");
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x00065FC8 File Offset: 0x000641C8
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<WebServer name=\"",
				this.name,
				"\" url=\"",
				this.saveURL,
				"\" />"
			});
		}

		// Token: 0x040006E4 RID: 1764
		public const string ROOT_FOLDERNAME = "web";

		// Token: 0x040006E5 RID: 1765
		public const string DEFAULT_PAGE_FILE = "index.html";

		// Token: 0x040006E6 RID: 1766
		public const string TEMP_WEBPAGE_CACHE_FILENAME = "/Content/Web/Cache/HN_OS_WebCache.html";

		// Token: 0x040006E7 RID: 1767
		private const string DEFAULT_PAGE_DATA_LOCATION = "Content/Web/BaseImageWebPage.html";

		// Token: 0x040006E8 RID: 1768
		private const string COMPANY_NAME_SENTINAL = "#$#COMPANYNAME#$#";

		// Token: 0x040006E9 RID: 1769
		private const string COMPANY_NAME_COMPACT_SENTINAL = "#$#LC_COMPANYNAME#$#";

		// Token: 0x040006EA RID: 1770
		public const int BASE_BAR_HEIGHT = 16;

		// Token: 0x040006EB RID: 1771
		private static string BaseWebpageData;

		// Token: 0x040006EC RID: 1772
		private static string BaseComnayPageData;

		// Token: 0x040006ED RID: 1773
		private string webPageFileLocation;

		// Token: 0x040006EE RID: 1774
		public Folder root;

		// Token: 0x040006EF RID: 1775
		public FileEntry lastLoadedFile;

		// Token: 0x040006F0 RID: 1776
		private bool shouldShow404 = false;

		// Token: 0x040006F1 RID: 1777
		private string saveURL = "";
	}
}
