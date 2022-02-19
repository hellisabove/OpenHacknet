using System;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hacknet.Gui
{
	// Token: 0x02000120 RID: 288
	public static class TextBox
	{
		// Token: 0x060006BE RID: 1726 RVA: 0x0006ED14 File Offset: 0x0006CF14
		public static string doTextBox(int myID, int x, int y, int width, int lines, string str, SpriteFont font)
		{
			string text = str;
			if (font == null)
			{
				font = GuiData.smallfont;
			}
			TextBox.BoxWasActivated = false;
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = lines * TextBox.LINE_HEIGHT;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
			}
			else if (GuiData.hot == myID)
			{
				GuiData.hot = -1;
			}
			if (GuiData.mouseWasPressed())
			{
				if (GuiData.hot == myID)
				{
					if (GuiData.active == myID)
					{
						int num = GuiData.mouse.X - x;
						bool flag = false;
						for (int i = 1; i <= str.Length; i++)
						{
							if (font.MeasureString(str.Substring(0, i)).X > (float)num)
							{
								TextBox.cursorPosition = i - 1;
								break;
							}
							if (!flag)
							{
								TextBox.cursorPosition = str.Length;
							}
						}
					}
					else
					{
						GuiData.active = myID;
						TextBox.cursorPosition = str.Length;
					}
				}
				else if (GuiData.active == myID)
				{
					GuiData.active = -1;
				}
			}
			if (GuiData.active == myID)
			{
				GuiData.willBlockTextInput = true;
				text = TextBox.getStringInput(text, GuiData.getKeyboadState(), GuiData.getLastKeyboadState());
				if (GuiData.getKeyboadState().IsKeyDown(Keys.Enter) && GuiData.getLastKeyboadState().IsKeyDown(Keys.Enter))
				{
					TextBox.BoxWasActivated = true;
					GuiData.active = -1;
				}
			}
			TextBox.FramesSelected++;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = lines * TextBox.LINE_HEIGHT;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, (GuiData.active == myID) ? GuiData.Default_Lit_Backing_Color : ((GuiData.hot == myID) ? GuiData.Default_Selected_Color : GuiData.Default_Dark_Background_Color));
			tmpRect.X += 2;
			tmpRect.Y += 2;
			tmpRect.Width -= 4;
			tmpRect.Height -= 4;
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, GuiData.Default_Light_Backing_Color);
			float num2 = ((float)TextBox.LINE_HEIGHT - font.MeasureString(text).Y) / 2f;
			GuiData.spriteBatch.DrawString(font, text, new Vector2((float)(x + 2), (float)y + num2), Color.White);
			if (GuiData.active == myID)
			{
				tmpRect.X = (int)((float)x + font.MeasureString(text.Substring(0, TextBox.cursorPosition)).X) + 3;
				tmpRect.Y = y + 2;
				tmpRect.Width = 1;
				tmpRect.Height = TextBox.LINE_HEIGHT - 4;
				GuiData.spriteBatch.Draw(Utils.white, tmpRect, (TextBox.FramesSelected % 60 < 40) ? Color.White : Color.Gray);
			}
			return text;
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0006F074 File Offset: 0x0006D274
		public static string doTerminalTextField(int myID, int x, int y, int width, int selectionHeight, int lines, string str, SpriteFont font)
		{
			if (font == null)
			{
				font = GuiData.smallfont;
			}
			TextBox.BoxWasActivated = false;
			TextBox.UpWasPresed = false;
			TextBox.DownWasPresed = false;
			TextBox.TabWasPresed = false;
			Rectangle tmpRect = GuiData.tmpRect;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = 0;
			if (tmpRect.Contains(GuiData.getMousePoint()))
			{
				GuiData.hot = myID;
			}
			else if (GuiData.hot == myID)
			{
				GuiData.hot = -1;
			}
			if (GuiData.mouseWasPressed())
			{
				if (GuiData.hot == myID)
				{
					if (GuiData.active == myID)
					{
						int num = GuiData.mouse.X - x;
						bool flag = false;
						for (int i = 1; i <= str.Length; i++)
						{
							if (font.MeasureString(str.Substring(0, i)).X > (float)num)
							{
								TextBox.cursorPosition = i - 1;
								break;
							}
							if (!flag)
							{
								TextBox.cursorPosition = str.Length;
							}
						}
					}
					else
					{
						GuiData.active = myID;
						TextBox.cursorPosition = str.Length;
					}
				}
				else if (GuiData.active == myID)
				{
					GuiData.active = -1;
				}
			}
			int active = GuiData.active;
			bool flag2 = 1 == 0;
			string filteredStringInput = TextBox.getFilteredStringInput(str, GuiData.getKeyboadState(), GuiData.getLastKeyboadState());
			if (GuiData.getKeyboadState().IsKeyDown(Keys.Enter) && !GuiData.getLastKeyboadState().IsKeyDown(Keys.Enter))
			{
				TextBox.BoxWasActivated = true;
				TextBox.cursorPosition = 0;
				TextBox.textDrawOffsetPosition = 0;
			}
			tmpRect.Height = lines * TextBox.LINE_HEIGHT;
			TextBox.FramesSelected++;
			tmpRect.X = x;
			tmpRect.Y = y;
			tmpRect.Width = width;
			tmpRect.Height = 10;
			tmpRect.X += 2;
			tmpRect.Y += 2;
			tmpRect.Width -= 4;
			tmpRect.Height -= 4;
			float num2 = ((float)TextBox.LINE_HEIGHT - font.MeasureString(filteredStringInput).Y) / 2f;
			string text = filteredStringInput;
			int num3 = 0;
			int num4 = 0;
			string text2 = text;
			while (font.MeasureString(text2).X > (float)(width - 5))
			{
				num3++;
				int num5 = text.Length - num4 - (num3 - num4);
				if (num5 < 0)
				{
					break;
				}
				text2 = text.Substring(num4, num5);
			}
			if (TextBox.cursorPosition < TextBox.textDrawOffsetPosition)
			{
				TextBox.textDrawOffsetPosition = Math.Max(0, TextBox.textDrawOffsetPosition - 1);
			}
			while (TextBox.cursorPosition > TextBox.textDrawOffsetPosition + (text.Length - num3))
			{
				TextBox.textDrawOffsetPosition++;
			}
			if (text.Length <= num3 || TextBox.textDrawOffsetPosition < 0)
			{
				if (TextBox.textDrawOffsetPosition <= text.Length - num3)
				{
					TextBox.textDrawOffsetPosition = text.Length - num3;
				}
				else
				{
					TextBox.textDrawOffsetPosition = 0;
				}
			}
			else if (TextBox.textDrawOffsetPosition > num3)
			{
				num3 = TextBox.textDrawOffsetPosition;
			}
			if (num3 > text.Length)
			{
				num3 = text.Length - 1;
			}
			if (TextBox.textDrawOffsetPosition >= text.Length)
			{
				TextBox.textDrawOffsetPosition = 0;
			}
			text = text.Substring(TextBox.textDrawOffsetPosition, text.Length - num3);
			if (TextBox.MaskingText)
			{
				string text3 = "";
				for (int i = 0; i < filteredStringInput.Length; i++)
				{
					text3 += "*";
				}
				text = text3;
			}
			GuiData.spriteBatch.DrawString(font, text, Utils.ClipVec2ForTextRendering(new Vector2((float)(x + 2), (float)y + num2)), Color.White);
			int active2 = GuiData.active;
			flag2 = (1 == 0);
			if (filteredStringInput != "")
			{
				int num6 = Math.Min(TextBox.cursorPosition - TextBox.textDrawOffsetPosition, text.Length);
				if (num6 <= 0)
				{
					num6 = 1;
				}
				if (text.Length == 0)
				{
					tmpRect.X = x;
				}
				else
				{
					tmpRect.X = (int)((float)x + font.MeasureString(text.Substring(0, num6)).X) + 3;
				}
			}
			else
			{
				tmpRect.X = x + 3;
			}
			tmpRect.Y = y + 2;
			tmpRect.Width = 1;
			tmpRect.Height = TextBox.LINE_HEIGHT - 4;
			if (LocaleActivator.ActiveLocaleIsCJK())
			{
				tmpRect.Y += 4;
			}
			GuiData.spriteBatch.Draw(Utils.white, tmpRect, (TextBox.FramesSelected % 60 < 40) ? Color.White : Color.Gray);
			return filteredStringInput;
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0006F5D8 File Offset: 0x0006D7D8
		private static string getStringInput(string s, KeyboardState input, KeyboardState lastInput)
		{
			Keys[] pressedKeys = input.GetPressedKeys();
			for (int i = 0; i < pressedKeys.Length; i++)
			{
				if (!lastInput.IsKeyDown(pressedKeys[i]))
				{
					if (!TextBox.IsSpecialKey(pressedKeys[i]))
					{
						string str = TextBox.ConvertKeyToChar(pressedKeys[i], input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.CapsLock));
						string str2 = s.Substring(0, TextBox.cursorPosition) + str;
						s = str2 + s.Substring(TextBox.cursorPosition);
						TextBox.cursorPosition++;
					}
					else
					{
						Keys keys = pressedKeys[i];
						if (keys <= Keys.Right)
						{
							if (keys == Keys.Back)
							{
								goto IL_D0;
							}
							switch (keys)
							{
							case Keys.Left:
								TextBox.cursorPosition--;
								if (TextBox.cursorPosition < 0)
								{
									TextBox.cursorPosition = 0;
								}
								break;
							case Keys.Right:
								TextBox.cursorPosition++;
								if (TextBox.cursorPosition > s.Length)
								{
									TextBox.cursorPosition = s.Length;
								}
								break;
							}
						}
						else if (keys == Keys.Delete || keys == Keys.OemClear)
						{
							goto IL_D0;
						}
						goto IL_17D;
						IL_D0:
						if (s.Length > 0 && TextBox.cursorPosition > 0)
						{
							string str2 = s.Substring(0, TextBox.cursorPosition - 1);
							s = str2 + s.Substring(TextBox.cursorPosition);
							TextBox.cursorPosition--;
						}
					}
					IL_17D:;
				}
			}
			return s;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0006F780 File Offset: 0x0006D980
		private static string getFilteredStringInput(string s, KeyboardState input, KeyboardState lastInput)
		{
			foreach (char c in GuiData.getFilteredKeys())
			{
				string str = s.Substring(0, TextBox.cursorPosition) + c;
				s = str + s.Substring(TextBox.cursorPosition);
				TextBox.cursorPosition++;
			}
			Keys[] pressedKeys = input.GetPressedKeys();
			if (pressedKeys.Length == 1 && lastInput.IsKeyDown(pressedKeys[0]))
			{
				if (pressedKeys[0] == TextBox.lastHeldKey && TextBox.IsSpecialKey(pressedKeys[0]))
				{
					TextBox.keyRepeatDelay -= GuiData.lastTimeStep;
					if (TextBox.keyRepeatDelay <= 0f)
					{
						s = TextBox.forceHandleKeyPress(s, pressedKeys[0], input, lastInput);
						TextBox.keyRepeatDelay = 0.04f;
					}
				}
				else
				{
					TextBox.lastHeldKey = pressedKeys[0];
					TextBox.keyRepeatDelay = 0.44f;
				}
			}
			else
			{
				for (int i = 0; i < pressedKeys.Length; i++)
				{
					if (!lastInput.IsKeyDown(pressedKeys[i]))
					{
						if (TextBox.IsSpecialKey(pressedKeys[i]))
						{
							Keys keys = pressedKeys[i];
							switch (keys)
							{
							case Keys.Back:
								goto IL_1DF;
							case Keys.Tab:
								TextBox.TabWasPresed = true;
								break;
							default:
								switch (keys)
								{
								case Keys.End:
									TextBox.cursorPosition = (TextBox.cursorPosition = s.Length);
									break;
								case Keys.Home:
									TextBox.cursorPosition = 0;
									break;
								case Keys.Left:
									TextBox.cursorPosition--;
									if (TextBox.cursorPosition < 0)
									{
										TextBox.cursorPosition = 0;
									}
									break;
								case Keys.Up:
									TextBox.UpWasPresed = true;
									break;
								case Keys.Right:
									TextBox.cursorPosition++;
									if (TextBox.cursorPosition > s.Length)
									{
										TextBox.cursorPosition = s.Length;
									}
									break;
								case Keys.Down:
									TextBox.DownWasPresed = true;
									break;
								case Keys.Select:
								case Keys.Print:
								case Keys.Execute:
								case Keys.PrintScreen:
								case Keys.Insert:
									break;
								case Keys.Delete:
									if (s.Length > 0 && TextBox.cursorPosition < s.Length)
									{
										string str = s.Substring(0, TextBox.cursorPosition);
										s = str + s.Substring(TextBox.cursorPosition + 1);
									}
									break;
								default:
									if (keys == Keys.OemClear)
									{
										goto IL_1DF;
									}
									break;
								}
								break;
							}
							goto IL_2CC;
							IL_1DF:
							if (s.Length > 0 && TextBox.cursorPosition > 0)
							{
								string str = s.Substring(0, TextBox.cursorPosition - 1);
								s = str + s.Substring(TextBox.cursorPosition);
								TextBox.cursorPosition--;
							}
						}
						IL_2CC:;
					}
				}
			}
			return s;
		}

		// Token: 0x060006C2 RID: 1730 RVA: 0x0006FA78 File Offset: 0x0006DC78
		private static string forceHandleKeyPress(string s, Keys key, KeyboardState input, KeyboardState lastInput)
		{
			if (!TextBox.IsSpecialKey(key))
			{
				string str = TextBox.ConvertKeyToChar(key, input.IsKeyDown(Keys.LeftShift) || input.IsKeyDown(Keys.CapsLock) || input.IsKeyDown(Keys.RightAlt));
				string str2 = s.Substring(0, TextBox.cursorPosition) + str;
				s = str2 + s.Substring(TextBox.cursorPosition);
				TextBox.cursorPosition++;
			}
			else
			{
				if (key <= Keys.Down)
				{
					switch (key)
					{
					case Keys.Back:
						break;
					case Keys.Tab:
						TextBox.TabWasPresed = true;
						goto IL_186;
					default:
						switch (key)
						{
						case Keys.Left:
							TextBox.cursorPosition--;
							if (TextBox.cursorPosition < 0)
							{
								TextBox.cursorPosition = 0;
							}
							goto IL_186;
						case Keys.Up:
							TextBox.UpWasPresed = true;
							goto IL_186;
						case Keys.Right:
							TextBox.cursorPosition++;
							if (TextBox.cursorPosition > s.Length)
							{
								TextBox.cursorPosition = s.Length;
							}
							goto IL_186;
						case Keys.Down:
							TextBox.DownWasPresed = true;
							goto IL_186;
						default:
							goto IL_184;
						}
						break;
					}
				}
				else if (key != Keys.Delete && key != Keys.OemClear)
				{
					goto IL_184;
				}
				if (s.Length > 0 && TextBox.cursorPosition > 0)
				{
					string str2 = s.Substring(0, TextBox.cursorPosition - 1);
					s = str2 + s.Substring(TextBox.cursorPosition);
					TextBox.cursorPosition--;
				}
				IL_184:
				IL_186:;
			}
			return s;
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x0006FC14 File Offset: 0x0006DE14
		public static bool IsSpecialKey(Keys key)
		{
			return (key < Keys.A || key > Keys.Z) && (key < Keys.D0 || key > Keys.D9) && key != Keys.Space && key != Keys.OemPeriod && key != Keys.OemComma && key != Keys.OemTilde && key != Keys.OemMinus && key != Keys.OemPipe && key != Keys.OemOpenBrackets && key != Keys.OemCloseBrackets && key != Keys.OemQuotes && key != Keys.OemQuestion && key != Keys.OemPlus;
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0006FCA4 File Offset: 0x0006DEA4
		public static string ConvertKeyToChar(Keys key, bool shift)
		{
			switch (key)
			{
			case Keys.Tab:
				return "\t";
			case (Keys)10:
			case (Keys)11:
			case (Keys)12:
			case (Keys)14:
			case (Keys)15:
			case (Keys)16:
			case (Keys)17:
			case (Keys)18:
			case Keys.Pause:
			case Keys.CapsLock:
			case Keys.Kana:
			case (Keys)22:
			case (Keys)23:
			case (Keys)24:
			case Keys.Kanji:
			case (Keys)26:
			case Keys.Escape:
			case Keys.ImeConvert:
			case Keys.ImeNoConvert:
			case (Keys)30:
			case (Keys)31:
			case Keys.PageUp:
			case Keys.PageDown:
			case Keys.End:
			case Keys.Home:
			case Keys.Left:
			case Keys.Up:
			case Keys.Right:
			case Keys.Down:
			case Keys.Select:
			case Keys.Print:
			case Keys.Execute:
			case Keys.PrintScreen:
			case Keys.Insert:
			case Keys.Delete:
			case Keys.Help:
			case (Keys)58:
			case (Keys)59:
			case (Keys)60:
			case (Keys)61:
			case (Keys)62:
			case (Keys)63:
			case (Keys)64:
			case Keys.LeftWindows:
			case Keys.RightWindows:
			case Keys.Apps:
			case (Keys)94:
			case Keys.Sleep:
			case Keys.Separator:
				break;
			case Keys.Enter:
				return "\n";
			case Keys.Space:
				return " ";
			case Keys.D0:
				return shift ? ")" : "0";
			case Keys.D1:
				return shift ? "!" : "1";
			case Keys.D2:
				return shift ? "@" : "2";
			case Keys.D3:
				return shift ? "#" : "3";
			case Keys.D4:
				return shift ? "$" : "4";
			case Keys.D5:
				return shift ? "%" : "5";
			case Keys.D6:
				return shift ? "^" : "6";
			case Keys.D7:
				return shift ? "&" : "7";
			case Keys.D8:
				return shift ? "*" : "8";
			case Keys.D9:
				return shift ? "(" : "9";
			case Keys.A:
				return shift ? "A" : "a";
			case Keys.B:
				return shift ? "B" : "b";
			case Keys.C:
				return shift ? "C" : "c";
			case Keys.D:
				return shift ? "D" : "d";
			case Keys.E:
				return shift ? "E" : "e";
			case Keys.F:
				return shift ? "F" : "f";
			case Keys.G:
				return shift ? "G" : "g";
			case Keys.H:
				return shift ? "H" : "h";
			case Keys.I:
				return shift ? "I" : "i";
			case Keys.J:
				return shift ? "J" : "j";
			case Keys.K:
				return shift ? "K" : "k";
			case Keys.L:
				return shift ? "L" : "l";
			case Keys.M:
				return shift ? "M" : "m";
			case Keys.N:
				return shift ? "N" : "n";
			case Keys.O:
				return shift ? "O" : "o";
			case Keys.P:
				return shift ? "P" : "p";
			case Keys.Q:
				return shift ? "Q" : "q";
			case Keys.R:
				return shift ? "R" : "r";
			case Keys.S:
				return shift ? "S" : "s";
			case Keys.T:
				return shift ? "T" : "t";
			case Keys.U:
				return shift ? "U" : "u";
			case Keys.V:
				return shift ? "V" : "v";
			case Keys.W:
				return shift ? "W" : "w";
			case Keys.X:
				return shift ? "X" : "x";
			case Keys.Y:
				return shift ? "Y" : "y";
			case Keys.Z:
				return shift ? "Z" : "z";
			case Keys.NumPad0:
				return "0";
			case Keys.NumPad1:
				return "1";
			case Keys.NumPad2:
				return "2";
			case Keys.NumPad3:
				return "3";
			case Keys.NumPad4:
				return "4";
			case Keys.NumPad5:
				return "5";
			case Keys.NumPad6:
				return "6";
			case Keys.NumPad7:
				return "7";
			case Keys.NumPad8:
				return "8";
			case Keys.NumPad9:
				return "9";
			case Keys.Multiply:
				return "*";
			case Keys.Add:
				return "+";
			case Keys.Subtract:
				return "-";
			case Keys.Decimal:
				return ".";
			case Keys.Divide:
				return "/";
			default:
				switch (key)
				{
				case Keys.OemSemicolon:
					return shift ? ":" : ";";
				case Keys.OemPlus:
					return shift ? "+" : "=";
				case Keys.OemComma:
					return shift ? "<" : ",";
				case Keys.OemMinus:
					return shift ? "_" : "-";
				case Keys.OemPeriod:
					return shift ? ">" : ".";
				case Keys.OemQuestion:
					return shift ? "?" : "/";
				case Keys.OemTilde:
					return shift ? "~" : "`";
				default:
					switch (key)
					{
					case Keys.OemOpenBrackets:
						return shift ? "{" : "[";
					case Keys.OemPipe:
						return shift ? "|" : "\\";
					case Keys.OemCloseBrackets:
						return shift ? "}" : "]";
					case Keys.OemQuotes:
						return shift ? "\"" : "'";
					}
					break;
				}
				break;
			}
			return string.Empty;
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x00070366 File Offset: 0x0006E566
		public static void moveCursorToEnd(string targetString)
		{
			TextBox.cursorPosition = targetString.Length;
		}

		// Token: 0x0400079C RID: 1948
		private const float DELAY_BEFORE_KEY_REPEAT_START = 0.44f;

		// Token: 0x0400079D RID: 1949
		private const float KEY_REPEAT_DELAY = 0.04f;

		// Token: 0x0400079E RID: 1950
		private const int OUTLINE_WIDTH = 2;

		// Token: 0x0400079F RID: 1951
		private static Keys lastHeldKey;

		// Token: 0x040007A0 RID: 1952
		private static float keyRepeatDelay = 0.44f;

		// Token: 0x040007A1 RID: 1953
		public static int LINE_HEIGHT = 25;

		// Token: 0x040007A2 RID: 1954
		public static int cursorPosition = 0;

		// Token: 0x040007A3 RID: 1955
		public static int textDrawOffsetPosition = 0;

		// Token: 0x040007A4 RID: 1956
		private static int FramesSelected = 0;

		// Token: 0x040007A5 RID: 1957
		public static bool MaskingText = false;

		// Token: 0x040007A6 RID: 1958
		public static bool BoxWasActivated = false;

		// Token: 0x040007A7 RID: 1959
		public static bool UpWasPresed = false;

		// Token: 0x040007A8 RID: 1960
		public static bool DownWasPresed = false;

		// Token: 0x040007A9 RID: 1961
		public static bool TabWasPresed = false;
	}
}
