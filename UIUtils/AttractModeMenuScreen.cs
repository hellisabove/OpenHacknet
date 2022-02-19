using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x020000ED RID: 237
	public class AttractModeMenuScreen
	{
		// Token: 0x060004F5 RID: 1269 RVA: 0x0004E750 File Offset: 0x0004C950
		public void Draw(Rectangle dest, SpriteBatch sb)
		{
			int num = (int)((float)dest.Width * 0.5f);
			int num2 = (int)((float)dest.Height * 0.4f);
			int num3 = 400;
			bool flag = num3 > num2;
			num2 = Math.Max(num3, num2);
			Rectangle dest2 = new Rectangle(dest.Width / 2 - num / 2, (int)((double)dest.Y + (double)dest.Height / 2.5 - (double)(num2 / 2)), num, num2);
			Rectangle destinationRectangle = new Rectangle(dest.X, dest2.Y + 115 + (flag ? 7 : 0), dest.Width, dest2.Height - 245);
			sb.Draw(Utils.white, destinationRectangle, Color.Lerp(new Color(115, 0, 0), new Color(122, 0, 0), 0f + Utils.randm(1f - Utils.randm(1f) * Utils.randm(1f))));
			dest2.Y += 12;
			TextItem.doFontLabelToSize(dest2, Utils.FlipRandomChars("HACKNET", 0.028), GuiData.titlefont, Color.White * 0.12f, false, false);
			FlickeringTextEffect.DrawLinedFlickeringText(dest2, Utils.FlipRandomChars("HACKNET", 0.003), 11f, 0.7f, GuiData.titlefont, null, Color.White, 5);
			int y = destinationRectangle.Y + dest2.Height - 80;
			int num4 = dest.Width / 4;
			int num5 = 20;
			int num6 = (num4 - num5) / 2;
			int height = 42;
			Rectangle destinationRectangle2 = new Rectangle(dest.X + dest.Width / 2 - num4 / 2, y, num4, height);
			if (Settings.MultiLingualDemo)
			{
				sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
				if (Button.doButton(18273302, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("New Session"), new Color?(new Color(124, 137, 149))))
				{
					LocaleActivator.ActivateLocale("ko-kr", Game1.getSingleton().Content);
					if (this.Start != null)
					{
						this.Start();
					}
				}
				destinationRectangle2.Y += destinationRectangle2.Height + 12;
				sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
				if (Button.doButton(18273304, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, "Hacknet Veterans", new Color?(new Color(124, 137, 149))))
				{
					LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
					if (this.StartDLC != null)
					{
						this.StartDLC();
					}
				}
				destinationRectangle2.Y += destinationRectangle2.Height + 12;
				sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
				if (Button.doButton(18273306, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, "New Session", new Color?(new Color(124, 137, 149))))
				{
					LocaleActivator.ActivateLocale("en-us", Game1.getSingleton().Content);
					if (this.Start != null)
					{
						this.Start();
					}
				}
			}
			else
			{
				sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
				if (Button.doButton(18273302, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("New Session"), new Color?(new Color(124, 137, 149))))
				{
					if (this.Start != null)
					{
						this.Start();
					}
				}
				destinationRectangle2.Y += destinationRectangle2.Height + 12;
				if (Settings.DLCEnabledDemo)
				{
					sb.Draw(Utils.white, destinationRectangle2, Color.Black * 0.8f);
					if (Button.doButton(18273303, destinationRectangle2.X, destinationRectangle2.Y, destinationRectangle2.Width, destinationRectangle2.Height, LocaleTerms.Loc("Labyrinths (Hacknet Veterans)"), new Color?(new Color(184, 137, 149))))
					{
						if (this.StartDLC != null)
						{
							this.StartDLC();
						}
					}
				}
			}
		}

		// Token: 0x0400059B RID: 1435
		public Action Start;

		// Token: 0x0400059C RID: 1436
		public Action Exit;

		// Token: 0x0400059D RID: 1437
		public Action StartDLC;
	}
}
