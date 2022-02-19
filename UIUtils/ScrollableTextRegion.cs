using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x0200017E RID: 382
	public class ScrollableTextRegion
	{
		// Token: 0x06000985 RID: 2437 RVA: 0x0009D044 File Offset: 0x0009B244
		public ScrollableTextRegion(GraphicsDevice gd)
		{
			this.Panel = new ScrollableSectionedPanel((int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 2f), gd);
			this.activeFontConfigName = GuiData.ActiveFontConfig.name;
		}

		// Token: 0x06000986 RID: 2438 RVA: 0x0009D083 File Offset: 0x0009B283
		public void Draw(Rectangle dest, string text, SpriteBatch sb)
		{
			this.Draw(dest, text, sb, Color.White);
		}

		// Token: 0x06000987 RID: 2439 RVA: 0x0009D0E4 File Offset: 0x0009B2E4
		public void Draw(Rectangle dest, string text, SpriteBatch sb, Color TextDrawColor)
		{
			try
			{
				if (GuiData.ActiveFontConfig.name != this.activeFontConfigName)
				{
					this.Panel.ScrollDown = 0f;
					this.Panel.PanelHeight = (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 2f + (float)GuiData.ActiveFontConfig.tinyFont.LineSpacing);
					this.activeFontConfigName = GuiData.ActiveFontConfig.name;
				}
				string[] data = text.Split(Utils.robustNewlineDelim, StringSplitOptions.None);
				this.Panel.NumberOfPanels = data.Length;
				this.Panel.Draw(delegate(int index, Rectangle panelDest, SpriteBatch spriteBatch)
				{
					spriteBatch.DrawString(GuiData.tinyfont, data[index], Utils.ClipVec2ForTextRendering(new Vector2((float)panelDest.X, (float)panelDest.Y)), TextDrawColor);
				}, sb, dest);
			}
			catch (Exception)
			{
				TextItem.doFontLabelToSize(dest, text, GuiData.tinyfont, TextDrawColor, false, false);
			}
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x0009D22C File Offset: 0x0009B42C
		public void Draw(Rectangle dest, List<string> textLines, SpriteBatch sb, Color TextDrawColor)
		{
			if (GuiData.ActiveFontConfig.name != this.activeFontConfigName)
			{
				this.Panel.ScrollDown = 0f;
				this.Panel.PanelHeight = (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 2f);
				this.activeFontConfigName = GuiData.ActiveFontConfig.name;
			}
			this.Panel.NumberOfPanels = textLines.Count;
			this.Panel.Draw(delegate(int index, Rectangle panelDest, SpriteBatch spriteBatch)
			{
				spriteBatch.DrawString(GuiData.tinyfont, textLines[index], Utils.ClipVec2ForTextRendering(new Vector2((float)panelDest.X, (float)panelDest.Y)), TextDrawColor);
			}, sb, dest);
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x0009D2DA File Offset: 0x0009B4DA
		public void UpdateScroll(float newScroll)
		{
			this.Panel.ScrollDown = newScroll;
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x0009D2EC File Offset: 0x0009B4EC
		public float GetScrollDown()
		{
			return this.Panel.ScrollDown;
		}

		// Token: 0x0600098B RID: 2443 RVA: 0x0009D309 File Offset: 0x0009B509
		public void SetScrollbarUIIndexOffset(int index)
		{
			this.Panel.ScrollbarUIIndexOffset = index;
		}

		// Token: 0x04000B19 RID: 2841
		private ScrollableSectionedPanel Panel;

		// Token: 0x04000B1A RID: 2842
		private string activeFontConfigName = null;
	}
}
