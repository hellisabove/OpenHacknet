using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Daemons.Helpers
{
	// Token: 0x02000013 RID: 19
	public static class AircraftAltitudeIndicator
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x0000B5DD File Offset: 0x000097DD
		public static void Init(ContentManager content)
		{
			AircraftAltitudeIndicator.WarningIcon = content.Load<Texture2D>("Sprites/Icons/CautionIcon");
			AircraftAltitudeIndicator.PlaneIcon = content.Load<Texture2D>("DLC/Sprites/Airplane");
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000B600 File Offset: 0x00009800
		public static void RenderAltitudeIndicator(Rectangle dest, SpriteBatch sb, int currentAltitude, bool IsInCriticalDescenet, bool IconFlashIsVisible, int maxAltitude = 50000, int upperReccomended = 40000, int lowerReccomended = 30000, int warningArea = 14000, int criticalFailureArea = 3000)
		{
			if (AircraftAltitudeIndicator.WarningIcon == null)
			{
				AircraftAltitudeIndicator.Init(OS.currentInstance.content);
			}
			bool flag = currentAltitude <= 0;
			if (flag)
			{
				currentAltitude = maxAltitude;
			}
			int num = Math.Min(dest.Width, 100);
			Rectangle rectangle = new Rectangle(dest.X + dest.Width - num, dest.Y, num, dest.Height);
			int num2 = 200;
			Rectangle dest2 = new Rectangle(dest.X + dest.Width - num2, rectangle.Y, num2, 21);
			Color color = IsInCriticalDescenet ? Utils.AddativeRed : OS.currentInstance.highlightColor;
			Rectangle rectangle2 = rectangle;
			rectangle2.Width = num / 2;
			rectangle2.X = dest.X + dest.Width - rectangle2.Width;
			sb.Draw(Utils.gradientLeftRight, rectangle2, color * 0.2f);
			int heightForAltitude = AircraftAltitudeIndicator.GetHeightForAltitude(currentAltitude, maxAltitude, rectangle2);
			rectangle.Y += heightForAltitude;
			rectangle.Height -= heightForAltitude;
			sb.Draw(Utils.gradientLeftRight, rectangle, color);
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, maxAltitude, LocaleTerms.Loc("Maximum Altitude"), maxAltitude, rectangle2, sb, color, true, true);
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, upperReccomended, LocaleTerms.Loc("Maximum Cruising Altitude"), maxAltitude, rectangle2, sb, color, false, false);
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, lowerReccomended, LocaleTerms.Loc("Minimum Cruising Altitude"), maxAltitude, rectangle2, sb, color, false, false);
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, warningArea, LocaleTerms.Loc("Unsafe Altitude Margin"), maxAltitude, rectangle2, sb, color, false, false);
			dest2.Height *= 2;
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, criticalFailureArea, LocaleTerms.Loc("Critical Failure Region") + "\n- " + LocaleTerms.Loc("POINT OF NO RETURN") + " -", maxAltitude, rectangle2, sb, Utils.makeColorAddative(color), true, false);
			dest2 = new Rectangle(dest2.X - 20, dest2.Y, dest2.Width + 20, dest2.Height + 10);
			AircraftAltitudeIndicator.DrawIndicatorForAltitude(dest2, currentAltitude, flag ? (LocaleTerms.Loc("CRITICAL ERROR") + "\n" + LocaleTerms.Loc("SIGNAL LOST")) : (LocaleTerms.Loc("Current Altitude") + "\n" + string.Format("{0}ft", currentAltitude)), maxAltitude, rectangle2, sb, color, true, false);
			int num3 = dest2.Height - 4;
			Rectangle rectangle3 = new Rectangle(dest2.X - num3 - 4, dest2.Y + AircraftAltitudeIndicator.GetHeightForAltitude(currentAltitude, maxAltitude, rectangle2), num3, num3);
			Rectangle destinationRectangle = new Rectangle(dest2.X - num3 - 4, rectangle3.Y, num3 + 4, dest2.Height);
			if (currentAltitude < lowerReccomended)
			{
				Rectangle dest3 = new Rectangle(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width + dest2.Width, destinationRectangle.Height);
				PatternDrawer.draw(dest3, 0.2f, Color.Transparent, Color.Red * 0.2f, sb);
			}
			sb.Draw(Utils.white, destinationRectangle, Color.Black * 0.4f);
			destinationRectangle.Height = 1;
			sb.Draw(Utils.white, destinationRectangle, color);
			rectangle3.Y += 2;
			rectangle3.X += 2;
			rectangle3 = Utils.InsetRectangle(rectangle3, 4);
			if (IsInCriticalDescenet)
			{
				sb.Draw(AircraftAltitudeIndicator.WarningIcon, rectangle3, Color.Red * (IconFlashIsVisible ? 1f : 0.3f));
			}
			else
			{
				sb.Draw(AircraftAltitudeIndicator.PlaneIcon, rectangle3, color);
			}
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000B9E0 File Offset: 0x00009BE0
		private static int GetHeightForAltitude(int altitude, int maxAltitude, Rectangle glowBar)
		{
			float num = (float)altitude / (float)maxAltitude;
			return (int)((float)glowBar.Height * (1f - num));
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000BA0C File Offset: 0x00009C0C
		private static void DrawIndicatorForAltitude(Rectangle dest, int altitude, string ElementTitle, int totalAltitude, Rectangle totalBar, SpriteBatch sb, Color c, bool LineAtTop = false, bool useGradientBacking = false)
		{
			dest.Y = totalBar.Y + AircraftAltitudeIndicator.GetHeightForAltitude(altitude, totalAltitude, totalBar);
			if (LineAtTop)
			{
				dest.Y++;
				dest.Height--;
			}
			sb.Draw(useGradientBacking ? Utils.gradientLeftRight : Utils.white, dest, null, Color.Black * (useGradientBacking ? 1f : 0.5f), 0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0.4f);
			TextItem.doFontLabelToSize(dest, ElementTitle, GuiData.font, c, true, true);
			if (LineAtTop)
			{
				dest.Y--;
				dest.Height = 1;
			}
			else
			{
				dest.Y += dest.Height - 2;
				dest.Height = 1;
			}
			sb.Draw(Utils.white, dest, c);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000BB10 File Offset: 0x00009D10
		public static bool GetFlashRateFromTimer(float timer)
		{
			return (double)timer % 0.3 < 0.15000000596046448;
		}

		// Token: 0x040000A3 RID: 163
		private static Texture2D WarningIcon;

		// Token: 0x040000A4 RID: 164
		private static Texture2D PlaneIcon;
	}
}
