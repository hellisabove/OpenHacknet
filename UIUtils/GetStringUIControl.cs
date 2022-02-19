using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.UIUtils
{
	// Token: 0x0200017B RID: 379
	public static class GetStringUIControl
	{
		// Token: 0x0600096F RID: 2415 RVA: 0x0009C3F0 File Offset: 0x0009A5F0
		public static void StartGetString(string prompt, object os_obj)
		{
			((OS)os_obj).execute("getString " + prompt);
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x0009C40C File Offset: 0x0009A60C
		public static string DrawGetStringControl(string prompt, Rectangle bounds, Action errorOccurs, Action cancelled, SpriteBatch sb, object os_obj, Color SearchButtonColor, Color CancelButtonColor, string upperPrompt = null, Color? BackingPanelColor = null)
		{
			OS os = (OS)os_obj;
			upperPrompt = ((upperPrompt == null) ? prompt : upperPrompt);
			string text = "";
			int num = bounds.X + 6;
			int num2 = bounds.Y + 2;
			Vector2 vector = Vector2.Zero;
			if (upperPrompt.Length > 0)
			{
				num2 += (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), upperPrompt, null).Y + 10;
			}
			string[] separator = new string[]
			{
				"#$#$#$$#$&$#$#$#$#"
			};
			string[] array = os.getStringCache.Split(separator, StringSplitOptions.None);
			if (array.Length > 1)
			{
				text = array[1];
				if (text.Equals(""))
				{
					text = os.terminal.currentLine;
				}
			}
			Rectangle destinationRectangle = new Rectangle(num, num2, bounds.Width - 12, bounds.Height - 46);
			sb.Draw(Utils.white, destinationRectangle, (BackingPanelColor != null) ? BackingPanelColor.Value : os.darkBackgroundColor);
			num2 += 18;
			string text2 = "";
			string str = Utils.ExtractBracketedSection(prompt, out text2);
			vector = TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), str + text, null);
			TextItem.doSmallLabel(new Vector2((float)num, (float)num2 + vector.Y + 2f), text2, null);
			destinationRectangle.X = num + (int)vector.X + 2;
			destinationRectangle.Y = num2;
			destinationRectangle.Width = 7;
			destinationRectangle.Height = 20;
			if (os.timer % 1f < 0.3f)
			{
				sb.Draw(Utils.white, destinationRectangle, os.outlineColor);
			}
			num2 += bounds.Height - 44;
			string result;
			if (array.Length > 2 || Button.doButton(30, num, num2, 300, 22, LocaleTerms.Loc("Search"), new Color?(SearchButtonColor)))
			{
				if (array.Length <= 2)
				{
					os.terminal.executeLine();
				}
				if (text.Length > 0)
				{
					result = text;
				}
				else
				{
					errorOccurs();
					result = null;
				}
			}
			else
			{
				if (Button.doButton(38, num, num2 + 24, 300, 22, LocaleTerms.Loc("Cancel"), new Color?(CancelButtonColor)))
				{
					cancelled();
					os.terminal.clearCurrentLine();
					os.terminal.executeLine();
				}
				result = null;
			}
			return result;
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x0009C6C4 File Offset: 0x0009A8C4
		public static void DrawGetStringControlInactive(string prompt, string valueText, Rectangle bounds, SpriteBatch sb, object os_obj, string upperPrompt = null)
		{
			OS os = (OS)os_obj;
			upperPrompt = ((upperPrompt == null) ? prompt : upperPrompt);
			int num = bounds.X + 6;
			int num2 = bounds.Y + 2;
			num2 += (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), upperPrompt, null).Y + 10;
			Rectangle destinationRectangle = new Rectangle(num, num2, bounds.Width - 12, bounds.Height - 46);
			sb.Draw(Utils.white, destinationRectangle, os.darkBackgroundColor);
			num2 += 28;
			destinationRectangle.X = num + (int)TextItem.doMeasuredSmallLabel(new Vector2((float)num, (float)num2), prompt + valueText, null).X + 2;
			destinationRectangle.Y = num2;
			destinationRectangle.Width = 7;
			destinationRectangle.Height = 20;
			num2 += bounds.Height - 44;
		}
	}
}
