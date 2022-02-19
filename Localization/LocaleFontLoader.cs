using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Localization
{
	// Token: 0x02000079 RID: 121
	public static class LocaleFontLoader
	{
		// Token: 0x0600025D RID: 605 RVA: 0x00021F4C File Offset: 0x0002014C
		public static void LoadFontConfigForLocale(string locale, ContentManager content)
		{
			if (!GuiData.LocaleFontConfigs.ContainsKey(locale))
			{
				GuiData.LocaleFontConfigs.Add(locale, LocaleFontLoader.LoadFontConfigSetForLocale(locale, "", content));
			}
			GuiData.ActivateFontConfig(GuiData.ActiveFontConfig.name);
			GuiData.UITinyfont = GuiData.LocaleFontConfigs[locale][0].tinyFont;
			GuiData.UISmallfont = GuiData.LocaleFontConfigs[locale][0].smallFont;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00021FCC File Offset: 0x000201CC
		private static List<GuiData.FontCongifOption> LoadFontConfigSetForLocale(string locale, string fontPrefix, ContentManager content)
		{
			List<GuiData.FontCongifOption> list = new List<GuiData.FontCongifOption>();
			string str = string.Concat(new string[]
			{
				"Locales/",
				locale,
				"/Fonts/",
				locale,
				"_",
				fontPrefix
			});
			bool flag = LocaleActivator.ActiveLocaleIsCJK();
			list.Add(new GuiData.FontCongifOption
			{
				name = "default",
				detailFont = content.Load<SpriteFont>(str + "Font7"),
				smallFont = content.Load<SpriteFont>(str + "Font12"),
				tinyFont = content.Load<SpriteFont>(str + "Font10"),
				bigFont = content.Load<SpriteFont>(str + "Font23"),
				tinyFontCharHeight = (flag ? 15f : 10f)
			});
			if (flag)
			{
				list[0].smallFont.LineSpacing += 2;
				list[0].detailFont.LineSpacing += 2;
			}
			list.Add(new GuiData.FontCongifOption
			{
				name = "medium",
				detailFont = content.Load<SpriteFont>(str + "Font7"),
				smallFont = content.Load<SpriteFont>(str + "Font14"),
				tinyFont = content.Load<SpriteFont>(str + "Font12"),
				bigFont = content.Load<SpriteFont>(str + "Font23"),
				tinyFontCharHeight = (flag ? 17f : 14f)
			});
			list.Add(new GuiData.FontCongifOption
			{
				name = "large",
				detailFont = content.Load<SpriteFont>(str + "Font7"),
				smallFont = content.Load<SpriteFont>(str + "Font16"),
				tinyFont = content.Load<SpriteFont>(str + "Font14"),
				bigFont = content.Load<SpriteFont>(str + "Font23"),
				tinyFontCharHeight = (flag ? 19f : 16f)
			});
			return list;
		}
	}
}
