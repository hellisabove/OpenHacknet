using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Modules.Helpers
{
	// Token: 0x020000DD RID: 221
	internal class DisplayModuleLSHelper
	{
		// Token: 0x06000472 RID: 1138 RVA: 0x00047741 File Offset: 0x00045941
		public DisplayModuleLSHelper()
		{
			this.panel = new ScrollableSectionedPanel(24, GuiData.spriteBatch.GraphicsDevice);
		}

		// Token: 0x06000473 RID: 1139 RVA: 0x00047858 File Offset: 0x00045A58
		public void DrawUI(Rectangle dest, OS os)
		{
			int ButtonHeight = (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 10f);
			if (this.panel.PanelHeight != ButtonHeight + 4)
			{
				this.panel = new ScrollableSectionedPanel(ButtonHeight + 4, GuiData.spriteBatch.GraphicsDevice);
			}
			Folder f = (os.connectedComp == null) ? os.thisComputer.files.root : os.connectedComp.files.root;
			List<DisplayModuleLSHelper.LSItem> items = this.BuildDirectoryDrawList(f, 0, 0, os);
			this.panel.NumberOfPanels = items.Count;
			int width = dest.Width - 25;
			Action<int, Rectangle, SpriteBatch> drawSection = delegate(int index, Rectangle bounds, SpriteBatch sb)
			{
				DisplayModuleLSHelper.LSItem lsitem = items[index];
				if (lsitem.IsEmtyDisplay)
				{
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 5 + lsitem.indent), (float)(bounds.Y + 2)), "-" + LocaleTerms.Loc("Empty") + "-", GuiData.tinyfont, null, (float)width, (float)ButtonHeight, false);
				}
				else if (Button.doButton(300000 + index, bounds.X + 5 + lsitem.indent, bounds.Y + 2, width - lsitem.indent, ButtonHeight, lsitem.DisplayName, null))
				{
					lsitem.Clicked();
				}
			};
			Button.DisableIfAnotherIsActive = true;
			this.panel.Draw(drawSection, GuiData.spriteBatch, dest);
			Button.DisableIfAnotherIsActive = false;
		}

		// Token: 0x06000474 RID: 1140 RVA: 0x00047BFC File Offset: 0x00045DFC
		private List<DisplayModuleLSHelper.LSItem> BuildDirectoryDrawList(Folder f, int recItteration, int indentOffset, OS os)
		{
			List<DisplayModuleLSHelper.LSItem> list = new List<DisplayModuleLSHelper.LSItem>();
			double commandSeperationDelay = 0.019;
			for (int i = 0; i < f.folders.Count; i++)
			{
				int myIndex = i;
				DisplayModuleLSHelper.LSItem item = new DisplayModuleLSHelper.LSItem
				{
					DisplayName = "/" + f.folders[i].name,
					Clicked = delegate()
					{
						int num = 0;
						for (int j = 0; j < os.navigationPath.Count - recItteration; j++)
						{
							Action action = delegate()
							{
								os.runCommand("cd ..");
							};
							if (num > 0)
							{
								os.delayer.Post(ActionDelayer.Wait((double)num * commandSeperationDelay), action);
							}
							else
							{
								action();
							}
							num++;
						}
						Action action2 = delegate()
						{
							os.runCommand("cd " + f.folders[myIndex].name);
						};
						if (num > 0)
						{
							os.delayer.Post(ActionDelayer.Wait((double)num * commandSeperationDelay), action2);
						}
						else
						{
							action2();
						}
					},
					indent = indentOffset
				};
				list.Add(item);
				indentOffset += 30;
				if (os.navigationPath.Count - 1 >= recItteration && os.navigationPath[recItteration] == i)
				{
					list.AddRange(this.BuildDirectoryDrawList(f.folders[i], recItteration + 1, indentOffset, os));
				}
				indentOffset -= 30;
			}
			for (int i = 0; i < f.files.Count; i++)
			{
				int myIndex = i;
				DisplayModuleLSHelper.LSItem item2 = new DisplayModuleLSHelper.LSItem
				{
					DisplayName = f.files[i].name,
					Clicked = delegate()
					{
						int num = 0;
						for (int j = 0; j < os.navigationPath.Count - recItteration; j++)
						{
							Action action = delegate()
							{
								os.runCommand("cd ..");
							};
							if (num > 0)
							{
								os.delayer.Post(ActionDelayer.Wait((double)num * commandSeperationDelay), action);
							}
							else
							{
								action();
							}
							num++;
						}
						Action action2 = delegate()
						{
							os.runCommand("cat " + f.files[myIndex].name);
						};
						if (num > 0)
						{
							os.delayer.Post(ActionDelayer.Wait((double)num * commandSeperationDelay), action2);
						}
						else
						{
							action2();
						}
					},
					indent = indentOffset
				};
				list.Add(item2);
			}
			if (f.folders.Count == 0 && f.files.Count == 0)
			{
				DisplayModuleLSHelper.LSItem item3 = new DisplayModuleLSHelper.LSItem
				{
					IsEmtyDisplay = true,
					indent = indentOffset
				};
				list.Add(item3);
			}
			return list;
		}

		// Token: 0x0400055C RID: 1372
		private const int BUTTON_HEIGHT = 20;

		// Token: 0x0400055D RID: 1373
		private const int BUTTON_MARGIN = 2;

		// Token: 0x0400055E RID: 1374
		private const int INDENT = 30;

		// Token: 0x0400055F RID: 1375
		private ScrollableSectionedPanel panel;

		// Token: 0x020000DE RID: 222
		private struct LSItem
		{
			// Token: 0x04000560 RID: 1376
			public Action Clicked;

			// Token: 0x04000561 RID: 1377
			public int indent;

			// Token: 0x04000562 RID: 1378
			public string DisplayName;

			// Token: 0x04000563 RID: 1379
			public bool IsEmtyDisplay;
		}
	}
}
