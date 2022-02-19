using System;
using System.IO;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Misc
{
	// Token: 0x02000083 RID: 131
	public static class MultiLanguageScreenshotTool
	{
		// Token: 0x060002A4 RID: 676 RVA: 0x00026AF0 File Offset: 0x00024CF0
		public static void CaptureMultiLanguageScreen(object os_obj)
		{
			OS os = (OS)os_obj;
			GameTime gameTime = new GameTime(os.lastGameTime.TotalGameTime, TimeSpan.Zero, false);
			string str = "Screenshots/";
			string activeLocale = Settings.ActiveLocale;
			for (int i = 0; i < LocaleActivator.SupportedLanguages.Count; i++)
			{
				string code = LocaleActivator.SupportedLanguages[i].Code;
				LocaleActivator.ActivateLocale(code, os.content);
				Texture2D texture2D = null;
				if (!Directory.Exists(str + code))
				{
					Directory.CreateDirectory(str + code);
				}
				string str2 = Guid.NewGuid().ToString() + ".png";
				using (FileStream fileStream = File.Create(str + code + "/" + str2))
				{
					texture2D.SaveAsPng(fileStream, texture2D.Width, texture2D.Height);
				}
			}
			LocaleActivator.ActivateLocale(activeLocale, os.content);
		}
	}
}
