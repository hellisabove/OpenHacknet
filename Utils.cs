using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Misc;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using SDL2;

namespace Hacknet
{
	// Token: 0x02000189 RID: 393
	internal static class Utils
	{
		// Token: 0x060009A2 RID: 2466 RVA: 0x0009D638 File Offset: 0x0009B838
		public static Vector2 getParallax(Vector2 objectPosition, Vector2 CameraPosition, float objectDepth, float focusDepth)
		{
			Vector2 result;
			if (objectDepth >= focusDepth)
			{
				float num = (objectDepth - focusDepth > 0.1f) ? ((objectDepth - focusDepth) * Utils.PARRALAX_MULTIPLIER) : (0f * Utils.PARRALAX_MULTIPLIER);
				result = new Vector2((CameraPosition.X - objectPosition.X) * num, 0f);
			}
			else
			{
				float num = (objectDepth - focusDepth < -0.05f) ? ((objectDepth - focusDepth) * Utils.PARRALAX_MULTIPLIER) : 0f;
				result = new Vector2((CameraPosition.X - objectPosition.X) * ((num == 0f) ? 0f : (num - 1f)), 0f);
			}
			return result;
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x0009D6E4 File Offset: 0x0009B8E4
		public static void drawLine(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth)
		{
			Texture2D texture = Utils.white;
			float x = Vector2.Distance(vector1, vector2);
			float rotation = (float)Math.Atan2((double)(vector2.Y - vector1.Y), (double)(vector2.X - vector1.X));
			spriteBatch.Draw(texture, OffsetPosition + vector1, null, Colour, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, Depth);
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x0009D758 File Offset: 0x0009B958
		public static void drawLine(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth, Texture2D altTex = null)
		{
			Texture2D texture = (altTex == null) ? Utils.white : altTex;
			float x = Vector2.Distance(vector1, vector2);
			float rotation = (float)Math.Atan2((double)(vector2.Y - vector1.Y), (double)(vector2.X - vector1.X));
			spriteBatch.Draw(texture, OffsetPosition + vector1, null, Colour, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, Depth);
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x0009D7D4 File Offset: 0x0009B9D4
		public static void drawLineAlt(SpriteBatch spriteBatch, Vector2 vector1, Vector2 vector2, Vector2 OffsetPosition, Color Colour, float Depth, float width, Texture2D altTex = null)
		{
			Texture2D texture2D = (altTex == null) ? Utils.white : altTex;
			float num = Vector2.Distance(vector1, vector2);
			float rotation = (float)Math.Atan2((double)(vector2.Y - vector1.Y), (double)(vector2.X - vector1.X));
			float y = 1f;
			if (texture2D.Height > 1)
			{
				y = 1f / (float)texture2D.Height;
			}
			spriteBatch.Draw(texture2D, OffsetPosition + vector1, null, Colour, rotation, Vector2.Zero, new Vector2(num / (float)texture2D.Width * width, y), SpriteEffects.FlipHorizontally, Depth);
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x0009D87C File Offset: 0x0009BA7C
		public static bool keyPressed(InputState input, Keys key, PlayerIndex? player)
		{
			bool result;
			if (GuiData.blockingTextInput)
			{
				result = false;
			}
			else
			{
				int num = 0;
				if (player != null)
				{
					num = (int)player.Value;
				}
				KeyboardState keyboardState = input.CurrentKeyboardStates[num];
				KeyboardState keyboardState2 = input.LastKeyboardStates[num];
				result = (keyboardState.IsKeyDown(key) && keyboardState2.IsKeyUp(key));
			}
			return result;
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x0009D904 File Offset: 0x0009BB04
		public static bool buttonPressed(InputState input, Buttons button, PlayerIndex? player)
		{
			GamePadState gamePadState = input.CurrentGamePadStates[(int)player.Value];
			GamePadState gamePadState2 = input.LastGamePadStates[(int)player.Value];
			return gamePadState.IsButtonDown(button) && gamePadState2.IsButtonUp(button);
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x0009D968 File Offset: 0x0009BB68
		public static bool arraysAreTheSame(Keys[] a, Keys[] b)
		{
			bool result;
			if (a.Length != b.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < a.Length; i++)
				{
					if (a[i].CompareTo(b[i]) == 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x0009D9C4 File Offset: 0x0009BBC4
		public static bool arraysAreTheSame(Buttons[] a, Buttons[] b)
		{
			bool result;
			if (a.Length != b.Length)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < a.Length; i++)
				{
					if (a[i].CompareTo(b[i]) == 0)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x0009DA20 File Offset: 0x0009BC20
		public static float rand(float range)
		{
			return (float)(Utils.random.NextDouble() * (double)range - Utils.random.NextDouble() * (double)range);
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x0009DA50 File Offset: 0x0009BC50
		public static float randm(float range)
		{
			return (float)(Utils.random.NextDouble() * (double)range);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x0009DA70 File Offset: 0x0009BC70
		public static float rand()
		{
			return (float)Utils.random.NextDouble();
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x0009DA90 File Offset: 0x0009BC90
		public static byte getRandomByte()
		{
			Utils.random.NextBytes(Utils.byteBuffer);
			return Utils.byteBuffer[0];
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x0009DABC File Offset: 0x0009BCBC
		public static Rectangle GetFullscreen()
		{
			Viewport viewport = GuiData.spriteBatch.GraphicsDevice.Viewport;
			return new Rectangle(0, 0, viewport.Width, viewport.Height);
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x0009DAF4 File Offset: 0x0009BCF4
		public static AudioEmitter emitterAtPosition(float x, float y)
		{
			if (Utils.emitter == null)
			{
				Utils.emitter = new AudioEmitter();
			}
			Utils.vec3.X = x;
			Utils.vec3.Y = y;
			Utils.vec3.Z = 0f;
			Utils.emitter.Position = Utils.vec3;
			return Utils.emitter;
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x0009DB5C File Offset: 0x0009BD5C
		public static AudioEmitter emitterAtPosition(float x, float y, float z)
		{
			if (Utils.emitter == null)
			{
				Utils.emitter = new AudioEmitter();
			}
			Utils.vec3.X = x;
			Utils.vec3.Y = y;
			Utils.vec3.Z = z;
			Utils.emitter.Position = Utils.vec3;
			return Utils.emitter;
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x0009DBC0 File Offset: 0x0009BDC0
		public static Texture2D White(ContentManager content)
		{
			if (Utils.white == null || Utils.white.IsDisposed)
			{
				Utils.white = TextureBank.load("Other\\white", content);
			}
			return Utils.white;
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0009DC04 File Offset: 0x0009BE04
		public static Color makeColor(byte r, byte g, byte b, byte a)
		{
			Utils.col.R = r;
			Utils.col.G = g;
			Utils.col.B = b;
			Utils.col.A = a;
			return Utils.col;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x0009DC4C File Offset: 0x0009BE4C
		public static Color makeColorAddative(Color c)
		{
			Utils.col.R = c.R;
			Utils.col.G = c.G;
			Utils.col.B = c.B;
			Utils.col.A = 0;
			return Utils.col;
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x0009DCA8 File Offset: 0x0009BEA8
		public static bool flipCoin()
		{
			return Utils.random.NextDouble() > 0.5;
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x0009DCD0 File Offset: 0x0009BED0
		public static byte randomCompType()
		{
			byte result;
			if (Utils.flipCoin())
			{
				result = 1;
			}
			else
			{
				result = 2;
			}
			return result;
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x0009DCF4 File Offset: 0x0009BEF4
		public static void writeToFile(string data, string filename)
		{
			using (StreamWriter streamWriter = new StreamWriter(filename))
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
			}
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x0009DD44 File Offset: 0x0009BF44
		public static void SafeWriteToFile(string data, string filename)
		{
			string text = filename + ".tmp";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text));
			}
			using (StreamWriter streamWriter = new StreamWriter(text, false))
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
			}
			if (File.Exists(filename))
			{
				File.Delete(filename);
			}
			File.Move(text, filename);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x0009DDD8 File Offset: 0x0009BFD8
		public static void SafeWriteToFile(byte[] data, string filename)
		{
			string text = filename + ".tmp";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(text));
			}
			using (StreamWriter streamWriter = new StreamWriter(text, false))
			{
				streamWriter.Write(data);
				streamWriter.Flush();
				streamWriter.Close();
			}
			File.Delete(filename);
			File.Move(text, filename);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x0009DE5C File Offset: 0x0009C05C
		public static void appendToFile(string data, string filename)
		{
			StreamWriter streamWriter = new StreamWriter(filename, true);
			streamWriter.Write(data);
			streamWriter.Close();
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x0009DE84 File Offset: 0x0009C084
		public static string readEntireFile(string filename)
		{
			if (Settings.ActiveLocale != "en-us")
			{
				filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
			}
			StreamReader streamReader = new StreamReader(File.OpenRead(filename));
			string data = streamReader.ReadToEnd();
			streamReader.Close();
			return LocalizedFileLoader.FilterStringForLocalization(data);
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x0009DED8 File Offset: 0x0009C0D8
		public static char getRandomLetter()
		{
			return Convert.ToChar(Convert.ToInt32(Math.Floor(26.0 * Utils.random.NextDouble() + 65.0)));
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x0009DF18 File Offset: 0x0009C118
		public static char getRandomChar()
		{
			char result;
			if (Utils.random.NextDouble() > 0.7)
			{
				result = string.Concat(Math.Min((int)Math.Floor((double)Utils.random.Next(0, 10)), 9))[0];
			}
			else
			{
				result = Utils.getRandomLetter();
			}
			return result;
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x0009DF7C File Offset: 0x0009C17C
		public static char getRandomNumberChar()
		{
			return string.Concat(Math.Min((int)Math.Floor((double)Utils.random.Next(0, 10)), 9))[0];
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x0009DFC8 File Offset: 0x0009C1C8
		public static Color convertStringToColor(string input)
		{
			Color color = Color.White;
			char[] separator = new char[]
			{
				',',
				' ',
				'/'
			};
			string[] array = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			Color result;
			if (array.Length < 3)
			{
				result = color;
			}
			else
			{
				byte r = byte.MaxValue;
				byte g = byte.MaxValue;
				byte b = byte.MaxValue;
				byte alpha = byte.MaxValue;
				for (int i = 0; i < 4; i++)
				{
					if (array.Length > i)
					{
						try
						{
							byte b2 = Convert.ToByte(array[i]);
							switch (i)
							{
							case 0:
								r = b2;
								break;
							case 1:
								g = b2;
								break;
							case 2:
								b = b2;
								break;
							case 3:
								alpha = b2;
								break;
							}
						}
						catch (FormatException)
						{
						}
						catch (OverflowException)
						{
						}
					}
				}
				color = new Color((int)r, (int)g, (int)b, (int)alpha);
				result = color;
			}
			return result;
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x0009E0D4 File Offset: 0x0009C2D4
		public static string convertColorToParseableString(Color c)
		{
			return string.Concat(new object[]
			{
				c.R,
				",",
				c.G,
				",",
				c.B,
				(c.A != byte.MaxValue) ? ("," + c.A) : ""
			});
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x0009E160 File Offset: 0x0009C360
		public static void ClipLineSegmentsForRect(Rectangle dest, Vector2 left, Vector2 right, out Vector2 leftOut, out Vector2 rightOut)
		{
			leftOut = left;
			rightOut = right;
			if (left.X < (float)dest.X)
			{
				leftOut.X = (float)dest.X;
			}
			if (right.X < (float)dest.X)
			{
				rightOut.X = (float)dest.X;
			}
			if (left.X > (float)(dest.X + dest.Width))
			{
				leftOut.X = (float)(dest.X + dest.Width);
			}
			if (right.X > (float)(dest.X + dest.Width))
			{
				rightOut.X = (float)(dest.X + dest.Width);
			}
			if (left.Y < (float)dest.Y)
			{
				leftOut.Y = (float)dest.Y;
			}
			if (right.Y < (float)dest.Y)
			{
				rightOut.Y = (float)dest.Y;
			}
			if (left.Y > (float)(dest.Y + dest.Height))
			{
				leftOut.Y = (float)(dest.Y + dest.Height);
			}
			if (right.Y > (float)(dest.Y + dest.Height))
			{
				rightOut.Y = (float)(dest.Y + dest.Height);
			}
		}

		// Token: 0x060009C1 RID: 2497 RVA: 0x0009E2F4 File Offset: 0x0009C4F4
		public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, float scale)
		{
			int num = (int)((float)tex.Width * scale);
			int num2 = (int)((float)tex.Height * scale);
			int num4;
			int num3 = num4 = 0;
			int num5 = tex.Width;
			int num6 = tex.Height;
			if (pos.X < (float)bounds.X)
			{
				num4 += (int)(((float)bounds.X - pos.X) / scale);
			}
			if (pos.Y < (float)bounds.Y)
			{
				num3 += (int)(((float)bounds.Y - pos.Y) / scale);
			}
			if (pos.X + (float)num > (float)(bounds.X + bounds.Width))
			{
				num5 -= (int)((pos.X + (float)num - (float)(bounds.X + bounds.Width)) * (1f / scale));
			}
			if (pos.Y + (float)num2 > (float)(bounds.Y + bounds.Height))
			{
				num6 -= (int)((pos.Y + (float)num2 - (float)(bounds.Y + bounds.Height)) * (1f / scale));
			}
			if (num4 > tex.Width)
			{
				num4 = tex.Width;
				num5 = 0;
			}
			if (num3 > tex.Height)
			{
				num3 = tex.Height;
				num6 = 0;
			}
			return new Rectangle(num4, num3, num5, num6);
		}

		// Token: 0x060009C2 RID: 2498 RVA: 0x0009E478 File Offset: 0x0009C678
		public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, Vector2 scale)
		{
			int num = (int)((float)tex.Width * scale.X);
			int num2 = (int)((float)tex.Height * scale.Y);
			int num4;
			int num3 = num4 = 0;
			int num5 = tex.Width;
			int num6 = tex.Height;
			if (pos.X < (float)bounds.X)
			{
				int num7 = (int)(((float)bounds.X - pos.X) / scale.X);
				num4 += num7;
				num5 -= num7;
			}
			if (pos.Y < (float)bounds.Y)
			{
				int num7 = (int)(((float)bounds.Y - pos.Y) / scale.Y);
				num3 += num7;
				num6 -= num7;
			}
			if (pos.X + (float)num > (float)(bounds.X + bounds.Width))
			{
				num5 -= (int)((pos.X + (float)num - (float)(bounds.X + bounds.Width)) * (1f / scale.X));
			}
			if (pos.Y + (float)num2 > (float)(bounds.Y + bounds.Height))
			{
				num6 -= (int)((pos.Y + (float)num2 - (float)(bounds.Y + bounds.Height)) * (1f / scale.Y));
			}
			if (num4 > tex.Width)
			{
				num4 = tex.Width;
				num5 = 0;
			}
			if (num3 > tex.Height)
			{
				num3 = tex.Height;
				num6 = 0;
			}
			return new Rectangle(num4, num3, num5, num6);
		}

		// Token: 0x060009C3 RID: 2499 RVA: 0x0009E638 File Offset: 0x0009C838
		public static Rectangle getClipRectForSpritePos(Rectangle bounds, Texture2D tex, Vector2 pos, Vector2 scale, Vector2 origin)
		{
			return Utils.getClipRectForSpritePos(bounds, tex, pos - origin * scale, scale);
		}

		// Token: 0x060009C4 RID: 2500 RVA: 0x0009E660 File Offset: 0x0009C860
		public static void RenderSpriteIntoClippedRectDest(Rectangle fullBounds, Rectangle targetBounds, Texture2D tex, Color c, SpriteBatch sb)
		{
			Vector2 scale = new Vector2((float)targetBounds.Width / (float)tex.Width, (float)targetBounds.Height / (float)tex.Height);
			Vector2 vector = new Vector2((float)targetBounds.X, (float)targetBounds.Y);
			Rectangle clipRectForSpritePos = Utils.getClipRectForSpritePos(fullBounds, tex, vector, scale, Vector2.Zero);
			Vector2 position = vector + new Vector2((float)clipRectForSpritePos.X * scale.X, (float)clipRectForSpritePos.Y * scale.Y);
			sb.Draw(tex, position, new Rectangle?(clipRectForSpritePos), c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
		}

		// Token: 0x060009C5 RID: 2501 RVA: 0x0009E70C File Offset: 0x0009C90C
		public static Vector2 GetCentreOrigin(this Texture2D tex)
		{
			return new Vector2((float)(tex.Width / 2), (float)(tex.Height / 2));
		}

		// Token: 0x060009C6 RID: 2502 RVA: 0x0009E738 File Offset: 0x0009C938
		public static string SmartTwimForWidth(string data, int width, SpriteFont font)
		{
			string[] array = data.Split(new string[]
			{
				"\r\n",
				"\n"
			}, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (font.MeasureString(text).X > (float)width)
				{
					string text2 = "";
					for (int j = 0; j < text.Length; j++)
					{
						bool flag = false;
						char c = text[j];
						if (c != '\t' && c != ' ')
						{
							flag = true;
						}
						else
						{
							text2 += text[j];
						}
						if (flag)
						{
							break;
						}
					}
					string[] collection = text.Substring(text2.Length).Split(new string[]
					{
						" "
					}, StringSplitOptions.None);
					List<string> list = new List<string>(collection);
					string str = "";
					string text3 = "";
					while (list.Count > 0)
					{
						if (font.MeasureString(text3 + " " + list[0]).X > (float)width)
						{
							str = str + text2 + text3.Trim() + "\r\n";
							text3 = "";
						}
						text3 = text3 + list[0] + " ";
						list.RemoveAt(0);
					}
					array[i] = str + text2 + text3.Trim() + "\r\n";
				}
			}
			string text4 = "";
			for (int k = 0; k < array.Length; k++)
			{
				text4 = text4 + array[k] + "\r\n";
			}
			return text4.Trim();
		}

		// Token: 0x060009C7 RID: 2503 RVA: 0x0009E92C File Offset: 0x0009CB2C
		public static Stream GenerateStreamFromString(string s)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(s);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return memoryStream;
		}

		// Token: 0x060009C8 RID: 2504 RVA: 0x0009E964 File Offset: 0x0009CB64
		public static string SuperSmartTwimForWidth(string data, int width, SpriteFont font)
		{
			string result;
			if (width <= 0)
			{
				result = data;
			}
			else
			{
				string[] array = data.Split(new string[]
				{
					"\r\n",
					"\n"
				}, StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i];
					if (font.MeasureString(text).X > (float)width)
					{
						string text2 = "";
						for (int j = 0; j < text.Length; j++)
						{
							bool flag = false;
							char c = text[j];
							if (c != '\t' && c != ' ')
							{
								flag = true;
							}
							else
							{
								text2 += text[j];
							}
							if (flag)
							{
								break;
							}
						}
						string[] collection = text.Substring(text2.Length).Split(new string[]
						{
							" "
						}, StringSplitOptions.None);
						List<string> list = new List<string>(collection);
						string str = "";
						StringBuilder stringBuilder = new StringBuilder();
						while (list.Count > 0)
						{
							if (font.MeasureString(stringBuilder.ToString() + " " + list[0]).X > (float)width)
							{
								str = str + text2 + stringBuilder.ToString().Trim() + "\r\n";
								stringBuilder.Clear();
							}
							int num = 1;
							if (font.MeasureString(list[0]).X > (float)width)
							{
								int num2 = 0;
								int num3 = 1;
								if (font.MeasureString(list[0].Substring(0, num)).X >= (float)width && font.MeasureString(list[0].Substring(0, num - 1)).X < (float)width)
								{
									num3 = num;
								}
								int k;
								for (k = num3; k < list[0].Length; k++)
								{
									if (font.MeasureString(list[0].Substring(0, k)).X >= (float)width)
									{
										break;
									}
									int num4 = 40;
									if (k + num4 < list[0].Length && font.MeasureString(list[0].Substring(0, k + num4)).X < (float)width)
									{
										k += num4;
										num2 += num4;
									}
								}
								k--;
								if (k == 0)
								{
									stringBuilder.Append(list[0]);
									stringBuilder.Append(" ");
									list.RemoveAt(0);
								}
								else
								{
									string text3 = list[0];
									stringBuilder.Append(list[0].Substring(0, k));
									stringBuilder.Append(" ");
									list[0] = text3.Substring(k);
								}
							}
							else
							{
								stringBuilder.Append(list[0]);
								stringBuilder.Append(" ");
								list.RemoveAt(0);
							}
						}
						bool flag2 = array.Length > i + 1 && string.IsNullOrWhiteSpace(array[i + 1]);
						array[i] = str + text2 + stringBuilder.ToString().Trim() + (flag2 ? "" : "\r\n");
					}
				}
				string text4 = "";
				for (int l = 0; l < array.Length; l++)
				{
					text4 = text4 + array[l] + "\r\n";
				}
				result = text4.Trim();
			}
			return result;
		}

		// Token: 0x060009C9 RID: 2505 RVA: 0x0009ED60 File Offset: 0x0009CF60
		public static float QuadraticOutCurve(float point)
		{
			return (-100000000f * point * (point - 2f) - 1f) / 100000000f;
		}

		// Token: 0x060009CA RID: 2506 RVA: 0x0009ED90 File Offset: 0x0009CF90
		public static float CubicInCurve(float point)
		{
			return 100000000f * (point /= 1f) * point * point / 100000000f;
		}

		// Token: 0x060009CB RID: 2507 RVA: 0x0009EDBC File Offset: 0x0009CFBC
		public static float CubicOutCurve(float point)
		{
			return 100000000f * ((point = point / 1f - 1f) * point * point + 1f) / 100000000f;
		}

		// Token: 0x060009CC RID: 2508 RVA: 0x0009EDF4 File Offset: 0x0009CFF4
		public static RenderTarget2D GetCurrentRenderTarget()
		{
			RenderTargetBinding[] renderTargets = GuiData.spriteBatch.GraphicsDevice.GetRenderTargets();
			RenderTarget2D result;
			if (renderTargets.Length == 0)
			{
				result = null;
			}
			else
			{
				result = (RenderTarget2D)renderTargets[0].RenderTarget;
			}
			return result;
		}

		// Token: 0x060009CD RID: 2509 RVA: 0x0009EE38 File Offset: 0x0009D038
		public static float SmoothStep(float edge0, float edge1, float x)
		{
			x = (float)Math.Min(Math.Max((double)((x - edge0) / (edge1 - edge0)), 0.0), 1.0);
			return x * x * x * (x * (x * 6f - 15f) + 10f);
		}

		// Token: 0x060009CE RID: 2510 RVA: 0x0009EE8C File Offset: 0x0009D08C
		public static Rectangle InsetRectangle(Rectangle rect, int inset)
		{
			return new Rectangle(rect.X + inset, rect.Y + inset, rect.Width - inset * 2, rect.Height - inset * 2);
		}

		// Token: 0x060009CF RID: 2511 RVA: 0x0009EECC File Offset: 0x0009D0CC
		public static Vector2 GetNearestPointOnCircle(Vector2 point, Vector2 CircleCentre, float circleRadius)
		{
			float num = point.X - CircleCentre.X;
			float num2 = point.Y - CircleCentre.Y;
			float num3 = (float)Math.Sqrt((double)(num * num + num2 * num2));
			float x = CircleCentre.X + num / num3 * circleRadius;
			float y = CircleCentre.Y + num2 / num3 * circleRadius;
			return new Vector2(x, y);
		}

		// Token: 0x060009D0 RID: 2512 RVA: 0x0009EF34 File Offset: 0x0009D134
		public static float Clamp(float val, float min, float max)
		{
			if (val < min)
			{
				val = min;
			}
			if (val > max)
			{
				val = max;
			}
			return val;
		}

		// Token: 0x060009D1 RID: 2513 RVA: 0x0009EF64 File Offset: 0x0009D164
		public static Vector2 Clamp(Vector2 val, float min, float max)
		{
			return new Vector2(Utils.Clamp(val.X, min, max), Utils.Clamp(val.Y, min, max));
		}

		// Token: 0x060009D2 RID: 2514 RVA: 0x0009EF98 File Offset: 0x0009D198
		public static string RandomFromArray(string[] array)
		{
			return array[Utils.random.Next(array.Length)];
		}

		// Token: 0x060009D3 RID: 2515 RVA: 0x0009EFBC File Offset: 0x0009D1BC
		public static string GetNonRepeatingFilename(string filename, string extension, Folder f)
		{
			string str = filename;
			int num = 0;
			bool flag;
			do
			{
				flag = true;
				for (int i = 0; i < f.files.Count; i++)
				{
					if (f.files[i].name == str + extension)
					{
						num++;
						str = string.Concat(new object[]
						{
							filename,
							"(",
							num,
							")"
						});
						flag = false;
						break;
					}
				}
			}
			while (!flag);
			return str + extension;
		}

		// Token: 0x060009D4 RID: 2516 RVA: 0x0009F06C File Offset: 0x0009D26C
		public static string FlipRandomChars(string original, double chancePerChar)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < original.Length; i++)
			{
				if (Utils.random.NextDouble() < chancePerChar)
				{
					stringBuilder.Append(Utils.getRandomChar());
				}
				else
				{
					stringBuilder.Append(original[i]);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x0009F0D4 File Offset: 0x0009D2D4
		public static Vector3 ColorToVec3(Color c)
		{
			float num = (float)c.A / 255f;
			return new Vector3((float)c.R / 255f * num, (float)c.G / 255f * num, (float)c.B / 255f * num);
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x0009F12C File Offset: 0x0009D32C
		public static Color AdditivizeColor(Color c)
		{
			Utils.col.R = c.R;
			Utils.col.G = c.G;
			Utils.col.B = c.B;
			Utils.col.A = 0;
			return Utils.col;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x0009F188 File Offset: 0x0009D388
		public static Vector2 RotatePoint(Vector2 point, float angle)
		{
			return Utils.PolarToCartesian(angle + Utils.GetPolarAngle(point), point.Length());
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x0009F1B0 File Offset: 0x0009D3B0
		public static bool DebugGoFast()
		{
			return Settings.debugCommandsEnabled && GuiData.getKeyboadState().IsKeyDown(Keys.LeftAlt);
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x0009F1E0 File Offset: 0x0009D3E0
		public static bool FieldContainsAttributeOfType(FieldInfo field, Type attributeType)
		{
			foreach (object obj in field.GetCustomAttributes(true))
			{
				if (obj.GetType() == attributeType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x0009F230 File Offset: 0x0009D430
		public static void SendRealWorldEmail(string subject, string to, string body)
		{
			try
			{
				MailAddress mailAddress = new MailAddress("fractalalligatordev@gmail.com");
				MailAddress to2 = new MailAddress(to);
				SmtpClient smtpClient = new SmtpClient
				{
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(mailAddress.Address, "rgaekwivookrsfqg")
				};
				using (MailMessage mailMessage = new MailMessage(mailAddress, to2)
				{
					Subject = subject,
					Body = body
				})
				{
					smtpClient.Send(mailMessage);
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x0009F30C File Offset: 0x0009D50C
		public static string GenerateReportFromException(Exception ex)
		{
			string text = string.Concat(new object[]
			{
				ex.GetType().ToString(),
				"\r\n\r\n",
				ex.Message,
				"\r\n\r\nSource : ",
				ex.Source,
				"\r\n\r\n",
				ex,
				"\r\n\r\n"
			});
			if (ex.InnerException != null)
			{
				text += "Inner : ---------------\r\n\r\n";
				string text2 = Utils.GenerateReportFromException(ex.InnerException);
				text2 = text2.Replace("\t", "\0");
				text2 = text2.Replace("\r\n", "\r\n\t");
				text2 = text2.Replace("\0", "\t");
				text = text + text2 + "\r\n\r\n";
			}
			text = text + "\r\n\r\n" + ex.StackTrace;
			text = FileSanitiser.purifyStringForDisplay(text);
			try
			{
				text = Utils.SuperSmartTwimForWidth(text, 800, GuiData.smallfont);
			}
			catch (Exception)
			{
			}
			return text;
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x0009F420 File Offset: 0x0009D620
		public static string GenerateReportFromExceptionCompact(Exception ex)
		{
			string text = string.Concat(new string[]
			{
				ex.Message,
				"\r\n\r\nSource : ",
				ex.Source,
				"  ::  ",
				ex.GetType().ToString()
			});
			if (ex.InnerException != null)
			{
				text += "\r\nInner : ---------------\r\n";
				string text2 = Utils.GenerateReportFromExceptionCompact(ex.InnerException);
				text2 = text2.Replace("\t", "\0");
				text2 = text2.Replace("\r\n", "\r\n\t");
				text2 = text2.Replace("\0", "\t");
				text = text + text2 + "\r\n---------------\r\n\r\n";
			}
			text = FileSanitiser.purifyStringForDisplay(text);
			try
			{
				text = Utils.SuperSmartTwimForWidth(text, 800, GuiData.smallfont);
			}
			catch (Exception)
			{
			}
			return text;
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x0009F50C File Offset: 0x0009D70C
		public static bool FloatEquals(float a, float b)
		{
			return Math.Abs(a - b) < 0.0001f;
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x0009F530 File Offset: 0x0009D730
		public static Vector2 PolarToCartesian(float angle, float magnitude)
		{
			return new Vector2(magnitude * (float)Math.Cos((double)angle), magnitude * (float)Math.Sin((double)angle));
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x0009F55C File Offset: 0x0009D75C
		public static float GetPolarAngle(Vector2 point)
		{
			return (float)Math.Atan2((double)point.Y, (double)point.X);
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x0009F584 File Offset: 0x0009D784
		public static Vector3 NormalizeRotationVector(Vector3 rot)
		{
			return new Vector3((float)((double)rot.X % 6.283185307179586), (float)((double)rot.Y % 6.283185307179586), (float)((double)rot.Z % 6.283185307179586));
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x0009F5D4 File Offset: 0x0009D7D4
		public static void FillEverywhereExcept(Rectangle bounds, Rectangle fullscreen, SpriteBatch sb, Color col)
		{
			Rectangle destinationRectangle = new Rectangle(fullscreen.X, fullscreen.Y, bounds.X - fullscreen.X, fullscreen.Height);
			Rectangle destinationRectangle2 = new Rectangle(bounds.X, fullscreen.Y, bounds.Width, bounds.Y - fullscreen.Y);
			Rectangle destinationRectangle3 = new Rectangle(bounds.X, bounds.Y + bounds.Height, bounds.Width, fullscreen.Height - (bounds.Y + bounds.Height));
			Rectangle destinationRectangle4 = new Rectangle(bounds.X + bounds.Width, fullscreen.Y, fullscreen.Width - (bounds.X + bounds.Width), fullscreen.Height);
			sb.Draw(Utils.white, destinationRectangle, col);
			sb.Draw(Utils.white, destinationRectangle4, col);
			sb.Draw(Utils.white, destinationRectangle2, col);
			sb.Draw(Utils.white, destinationRectangle3, col);
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0009F6EC File Offset: 0x0009D8EC
		public static bool CheckStringIsRenderable(string input)
		{
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] != '\n' && !GuiData.font.Characters.Contains(input[i]))
				{
					char c = input[i];
					Console.WriteLine("\r\n------------------\r\nInvalid Char : {" + input[i] + "}\r\n----------------------\r\n");
					return false;
				}
			}
			return true;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x0009F770 File Offset: 0x0009D970
		public static bool CheckStringIsTitleRenderable(string input)
		{
			string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,./!@#$%^&*()<>\\\":;{}_-+= \r\n?[]`~'|";
			for (int i = 0; i < input.Length; i++)
			{
				if (!source.Contains(input[i]))
				{
					char c = input[i];
					Console.WriteLine("\r\n------------------\r\nInvalid Char : {" + input[i] + "}\r\n----------------------\r\n");
					return false;
				}
			}
			return true;
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x0009F7F4 File Offset: 0x0009D9F4
		public static bool StringContainsInvalidFilenameChars(string input)
		{
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			char[] array = new char[]
			{
				'&',
				'<',
				'>',
				'\'',
				'"'
			};
			for (int i = 0; i < invalidFileNameChars.Length; i++)
			{
				if (input.Contains(invalidFileNameChars[i]))
				{
					return true;
				}
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (input.Contains(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0009F878 File Offset: 0x0009DA78
		public static string CleanStringToRenderable(string input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string source = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890,./!@#$%^&*()<>\\\":;{}_-+= \r\n?[]`~'|";
			for (int i = 0; i < input.Length; i++)
			{
				if (!source.Contains(input[i]))
				{
					stringBuilder.Append('?');
				}
				else
				{
					stringBuilder.Append(input[i]);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0009F8E4 File Offset: 0x0009DAE4
		public static string CleanFilterStringToRenderable(string input)
		{
			return Utils.CleanStringToLanguageRenderable(input.Replace("\t", "    ").Replace("…", "..."));
		}

		// Token: 0x060009E7 RID: 2535 RVA: 0x0009F91C File Offset: 0x0009DB1C
		public static string CleanStringToLanguageRenderable(string input)
		{
			input = input.Replace("\t", "    ");
			StringBuilder stringBuilder = new StringBuilder();
			string source = "\r\n";
			for (int i = 0; i < input.Length; i++)
			{
				if (source.Contains(input[i]))
				{
					stringBuilder.Append(input[i]);
				}
				else if (!GuiData.font.Characters.Contains(input[i]))
				{
					stringBuilder.Append('?');
				}
				else
				{
					stringBuilder.Append(input[i]);
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009E8 RID: 2536 RVA: 0x0009F9C8 File Offset: 0x0009DBC8
		public static void AppendToErrorFile(string text)
		{
			try
			{
				if (SDL.SDL_GetPlatform().Equals("Windows"))
				{
					string text2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					string str = "RuntimeErrors.txt";
					if (!File.Exists(text2 + str))
					{
						File.WriteAllText(text2 + str, "Hacknet v" + MainMenu.OSVersion + " Runtime ErrorLog\r\n\r\n");
					}
					string value = "-----\r\n" + MainMenu.OSVersion + " : " + DateTime.Now.ToString();
					using (StreamWriter streamWriter = File.AppendText(text2 + str))
					{
						streamWriter.WriteLine(value);
						streamWriter.WriteLine(text);
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060009E9 RID: 2537 RVA: 0x0009FAD8 File Offset: 0x0009DCD8
		public static void AppendToWarningsFile(string text)
		{
			try
			{
				string path = Utils.GetFileLoadPrefix() + "warnings.txt";
				string text2 = "";
				if (File.Exists(path))
				{
					text2 = File.ReadAllText(path) + "--------------------------------\r\n\r\n";
				}
				text2 += text;
				File.WriteAllText(path, text2);
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060009EA RID: 2538 RVA: 0x0009FB44 File Offset: 0x0009DD44
		public static string SerializeListToCSV(List<string> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				if (i != 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(list[i]);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060009EB RID: 2539 RVA: 0x0009FBB0 File Offset: 0x0009DDB0
		public static string[] SplitToTokens(string input)
		{
			return (from Match m in Regex.Matches(input, "[\\\"].+?[\\\"]|[^ ]+")
			select m.Value).ToList<string>().ToArray();
		}

		// Token: 0x060009EC RID: 2540 RVA: 0x0009FC00 File Offset: 0x0009DE00
		public static string[] SplitToTokens(string[] input)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < input.Length; i++)
			{
				stringBuilder.Append(input[i]);
				stringBuilder.Append(" ");
			}
			return Utils.SplitToTokens(stringBuilder.ToString());
		}

		// Token: 0x060009ED RID: 2541 RVA: 0x0009FC50 File Offset: 0x0009DE50
		public static string ExtractBracketedSection(string input, out string bracketedBit)
		{
			bracketedBit = "";
			int num = input.IndexOf('(');
			string result;
			if (num == -1)
			{
				result = input;
			}
			else
			{
				int num2 = input.Substring(num).IndexOf(')');
				if (num2 == -1)
				{
					result = input;
				}
				else
				{
					num2 += num;
					string text = input.Substring(0, num) + input.Substring(num2 + 1, input.Length - (num2 + 1));
					bracketedBit = input.Substring(num, num2 - num + 1);
					result = text;
				}
			}
			return result;
		}

		// Token: 0x060009EE RID: 2542 RVA: 0x0009FCDC File Offset: 0x0009DEDC
		public static Color ColorFromHexString(string hexString)
		{
			if (hexString.StartsWith("#"))
			{
				hexString = hexString.Substring(1);
			}
			uint num = uint.Parse(hexString, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			Color result = Color.White;
			if (hexString.Length == 8)
			{
				result.A = (byte)(num >> 24);
				result.R = (byte)(num >> 16);
				result.G = (byte)(num >> 8);
				result.B = (byte)num;
			}
			else
			{
				if (hexString.Length != 6)
				{
					throw new InvalidOperationException("Invald hex representation of an ARGB or RGB color value.");
				}
				result.R = (byte)(num >> 16);
				result.G = (byte)(num >> 8);
				result.B = (byte)num;
			}
			return result;
		}

		// Token: 0x060009EF RID: 2543 RVA: 0x0009FDA8 File Offset: 0x0009DFA8
		public static string ReadEntireContentsOfStream(Stream input)
		{
			TextReader textReader = new StreamReader(input);
			string result = textReader.ReadToEnd();
			input.Flush();
			input.Close();
			input.Dispose();
			return result;
		}

		// Token: 0x060009F0 RID: 2544 RVA: 0x0009FDE0 File Offset: 0x0009DFE0
		public static void SendErrorEmail(Exception ex, string postfix = "", string extraData = "")
		{
			string text = Utils.GenerateReportFromException(ex);
			text = text + "\r\n White:" + Utils.white;
			text = text + "\r\n WhiteDisposed:" + Utils.white.IsDisposed;
			text = text + "\r\n SmallFont:" + GuiData.smallfont;
			text = text + "\r\n TinyFont:" + GuiData.tinyfont;
			text = text + "\r\n LineEffectTarget:" + FlickeringTextEffect.GetReportString();
			text = text + "\r\n PostProcessort stuff:" + PostProcessor.GetStatusReportString();
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"\r\nRESOLUTION:\r\n ",
				Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferWidth,
				"x"
			});
			text = text + Game1.getSingleton().GraphicsDevice.PresentationParameters.BackBufferHeight + "\r\nFullscreen: ";
			text += (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false");
			text = text + "\r\n Adapter: " + Game1.getSingleton().GraphicsDevice.Adapter.Description;
			text = text + "\r\n Device Name: " + Game1.getSingleton().GraphicsDevice.Adapter.DeviceName;
			text = text + "\r\n Status: " + Game1.getSingleton().GraphicsDevice.GraphicsDeviceStatus;
			text = text + "\r\n Extra:\r\n" + extraData + "\r\n";
			Utils.SendRealWorldEmail(string.Concat(new string[]
			{
				"Hacknet ",
				postfix,
				MainMenu.OSVersion,
				" Crash ",
				DateTime.Now.ToShortDateString(),
				" ",
				DateTime.Now.ToShortTimeString()
			}), "hacknetbugs+Hacknet@gmail.com", text);
		}

		// Token: 0x060009F1 RID: 2545 RVA: 0x0009FFE4 File Offset: 0x0009E1E4
		public static void SendThreadedErrorReport(Exception ex, string postfix = "", string extraData = "")
		{
			new Thread(delegate()
			{
				Utils.SendErrorEmail(ex, postfix, extraData);
			})
			{
				IsBackground = true,
				Name = postfix + "_Errorthread"
			}.Start();
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x000A0048 File Offset: 0x0009E248
		public static DateTime SafeParseDateTime(string input)
		{
			DateTime dateTime = default(DateTime);
			DateTime result;
			if (DateTime.TryParseExact(input, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				result = dateTime;
			}
			else if (DateTime.TryParse(input, new CultureInfo("en-au"), DateTimeStyles.None, out dateTime))
			{
				result = dateTime;
			}
			else if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				result = dateTime;
			}
			else
			{
				Console.WriteLine("Error Parsing DateTime : " + input);
				result = DateTime.Now;
			}
			return result;
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x000A00D0 File Offset: 0x0009E2D0
		public static string SafeWriteDateTime(DateTime input)
		{
			return input.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x000A00F4 File Offset: 0x0009E2F4
		public static Color GetComplimentaryColor(Color c)
		{
			Utils.hslColor = HSLColor.FromRGB(c);
			Utils.hslColor.Luminosity = Math.Max(0.4f, Utils.hslColor.Luminosity);
			Utils.hslColor.Saturation = Math.Max(0.4f, Utils.hslColor.Saturation);
			Utils.hslColor.Saturation = Math.Min(0.55f, Utils.hslColor.Saturation);
			Utils.hslColor.Hue -= 3.1415927f;
			if (Utils.hslColor.Hue < 0f)
			{
				Utils.hslColor.Hue += 6.2831855f;
			}
			return Utils.hslColor.ToRGB();
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x000A01BC File Offset: 0x0009E3BC
		public static void MeasureTimedSpeechSection()
		{
			string text = Utils.readEntireFile("Content/Post/BitSpeech.txt");
			string[] array = text.Split(Utils.newlineDelim);
			float num = 0f;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				float num2 = 0f;
				for (int j = 0; j < text2.Length; j++)
				{
					if (text2[j] == '#')
					{
						num2 += 1f;
					}
					else if (text2[j] == '%')
					{
						num2 += 0.5f;
					}
					else
					{
						num2 += 0.05f;
					}
				}
				Console.WriteLine(string.Concat(new object[]
				{
					"LINE ",
					i,
					": ",
					num,
					"  --  ",
					num2,
					"   : ",
					array[i]
				}));
				num += num2;
			}
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x000A02E0 File Offset: 0x0009E4E0
		public static Color HSL2RGB(double h, double sl, double l)
		{
			double num = l;
			double num2 = l;
			double num3 = l;
			double num4 = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
			if (num4 > 0.0)
			{
				double num5 = l + l - num4;
				double num6 = (num4 - num5) / num4;
				h *= 6.0;
				int num7 = (int)h;
				double num8 = h - (double)num7;
				double num9 = num4 * num6 * num8;
				double num10 = num5 + num9;
				double num11 = num4 - num9;
				switch (num7)
				{
				case 0:
					num = num4;
					num2 = num10;
					num3 = num5;
					break;
				case 1:
					num = num11;
					num2 = num4;
					num3 = num5;
					break;
				case 2:
					num = num5;
					num2 = num4;
					num3 = num10;
					break;
				case 3:
					num = num5;
					num2 = num11;
					num3 = num4;
					break;
				case 4:
					num = num10;
					num2 = num5;
					num3 = num4;
					break;
				case 5:
					num = num4;
					num2 = num5;
					num3 = num11;
					break;
				}
			}
			return new Color
			{
				R = Convert.ToByte(num * 255.0),
				G = Convert.ToByte(num2 * 255.0),
				B = Convert.ToByte(num3 * 255.0),
				A = byte.MaxValue
			};
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x000A0438 File Offset: 0x0009E638
		public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
		{
			double num = (double)rgb.R / 255.0;
			double num2 = (double)rgb.G / 255.0;
			double num3 = (double)rgb.B / 255.0;
			h = 0.0;
			s = 0.0;
			l = 0.0;
			double num4 = Math.Max(num, num2);
			num4 = Math.Max(num4, num3);
			double num5 = Math.Min(num, num2);
			num5 = Math.Min(num5, num3);
			l = (num5 + num4) / 2.0;
			if (l > 0.0)
			{
				double num6 = num4 - num5;
				s = num6;
				if (s > 0.0)
				{
					s /= ((l <= 0.5) ? (num4 + num5) : (2.0 - num4 - num5));
					double num7 = (num4 - num) / num6;
					double num8 = (num4 - num2) / num6;
					double num9 = (num4 - num3) / num6;
					if (num == num4)
					{
						h = ((num2 == num5) ? (5.0 + num9) : (1.0 - num8));
					}
					else if (num2 == num4)
					{
						h = ((num3 == num5) ? (1.0 + num7) : (3.0 - num9));
					}
					else
					{
						h = ((num == num5) ? (3.0 + num8) : (5.0 - num7));
					}
					h /= 6.0;
				}
			}
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x000A05F0 File Offset: 0x0009E7F0
		public static void ActOnAllFilesRevursivley(string foldername, Action<string> FileAction)
		{
			string[] files = Directory.GetFiles(foldername);
			for (int i = 0; i < files.Length; i++)
			{
				FileAction(files[i]);
			}
			string[] directories = Directory.GetDirectories(foldername);
			for (int i = 0; i < directories.Length; i++)
			{
				Utils.ActOnAllFilesRevursivley(directories[i], FileAction);
			}
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000A0648 File Offset: 0x0009E848
		public static string GetFileLoadPrefix()
		{
			string result;
			if (Settings.IsInExtensionMode)
			{
				result = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
			}
			else
			{
				result = "Content/";
			}
			return result;
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x000A0684 File Offset: 0x0009E884
		public static Vector2 ClipVec2ForTextRendering(Vector2 input)
		{
			input.X = (float)((int)((double)input.X + 0.5));
			input.Y = (float)((int)((double)input.Y + 0.5));
			return input;
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000A06D0 File Offset: 0x0009E8D0
		public static string SerializeObject(object o)
		{
			Type type = o.GetType();
			string text = type.Name;
			if (text.StartsWith("Hacknet."))
			{
				text = text.Substring("Hacknet.".Length);
			}
			StringBuilder stringBuilder = new StringBuilder("<" + text + ">");
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				string name = fields[i].Name;
				object value = fields[i].GetValue(o);
				if (value != null)
				{
					string text2 = value.ToString();
					Type fieldType = fields[i].FieldType;
					if (fieldType == typeof(Color))
					{
						text2 = Utils.convertColorToParseableString((Color)fields[i].GetValue(o));
					}
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\n\t<{0}>{1}</{0}>", new object[]
					{
						name,
						text2
					}));
				}
			}
			stringBuilder.Append("\n</" + text + ">");
			return stringBuilder.ToString();
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x000A0808 File Offset: 0x0009EA08
		public static object DeserializeObject(Stream s, Type t)
		{
			object result;
			using (XmlReader xmlReader = XmlReader.Create(s))
			{
				object obj = Activator.CreateInstance(t);
				FieldInfo[] fields = t.GetFields();
				XmlNamespaceManager namespaceResolver = new XmlNamespaceManager(new NameTable());
				while (!xmlReader.EOF)
				{
					if (!string.IsNullOrWhiteSpace(xmlReader.Name))
					{
						for (int i = 0; i < fields.Length; i++)
						{
							if (fields[i].Name == xmlReader.Name)
							{
								xmlReader.MoveToContent();
								object obj2 = null;
								if (fields[i].FieldType == typeof(Color))
								{
									obj2 = Utils.convertStringToColor(xmlReader.ReadElementContentAsString());
								}
								if (obj2 == null)
								{
									obj2 = xmlReader.ReadElementContentAs(fields[i].FieldType, namespaceResolver);
								}
								fields[i].SetValue(obj, obj2);
							}
						}
					}
					xmlReader.Read();
				}
				result = obj;
			}
			return result;
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x000A0950 File Offset: 0x0009EB50
		public static void ProcessXmlElementInParent(XmlReader rdr, string ParentTag, string ElementTag, Action ProcessElement)
		{
			int num = 0;
			if (rdr.Name == ParentTag)
			{
				for (;;)
				{
					if (rdr.Name == ElementTag && rdr.IsStartElement())
					{
						if (ProcessElement != null)
						{
							ProcessElement();
						}
					}
					do
					{
						rdr.Read();
						num++;
					}
					while (rdr.IsEmptyElement && !rdr.EOF);
					if (rdr.Name == ParentTag && num > 10)
					{
						num++;
					}
					if (rdr.EOF)
					{
						break;
					}
					if (rdr.Name == ParentTag && !rdr.IsStartElement())
					{
						return;
					}
				}
				throw new FormatException(string.Concat(new object[]
				{
					"Unexpected End of File looking for ",
					ElementTag,
					" in ",
					ParentTag,
					" tag. Made ",
					num,
					" reads."
				}));
			}
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x000A0A64 File Offset: 0x0009EC64
		public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
		{
			bool result;
			if (self != null && to != null)
			{
				Type type = self.GetType();
				List<string> list = new List<string>(ignore);
				foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					if (!list.Contains(propertyInfo.Name))
					{
						object value = type.GetProperty(propertyInfo.Name).GetValue(self, null);
						object value2 = type.GetProperty(propertyInfo.Name).GetValue(to, null);
						if (propertyInfo.PropertyType.IsClass && !propertyInfo.PropertyType.Module.ScopeName.Equals("CommonLanguageRuntimeLibrary"))
						{
							if (!Utils.PublicInstancePropertiesEqual<object>(value, value2, ignore))
							{
								return false;
							}
						}
						else if (value != value2 && (value == null || !value.Equals(value2)))
						{
							return false;
						}
					}
				}
				result = true;
			}
			else
			{
				result = (self == to);
			}
			return result;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x000A0BB0 File Offset: 0x0009EDB0
		public static Rectangle DrawSpriteAspectCorrect(Rectangle dest, SpriteBatch sb, Texture2D sprite, Color color, bool ForceToBottom = false)
		{
			Rectangle rectangle = dest;
			float num = (float)dest.Width / (float)dest.Height;
			float num2 = (float)sprite.Width / (float)sprite.Height;
			if (!Utils.FloatEquals(num, num2))
			{
				if (num > num2)
				{
					int num3 = dest.Height;
					int num4 = (int)((float)dest.Height * num2);
					int num5 = (int)((float)(dest.Width - num4) / 2f);
					rectangle = new Rectangle(dest.X + num5, dest.Y, num4, num3);
				}
				else
				{
					int num4 = dest.Width;
					int num3 = (int)((float)dest.Width / num2);
					int num6 = (int)((float)(dest.Height - num3) / (ForceToBottom ? 1f : 2f));
					rectangle = new Rectangle(dest.X, dest.Y + num6, num4, num3);
				}
			}
			sb.Draw(sprite, rectangle, color);
			return rectangle;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x000A0CB0 File Offset: 0x0009EEB0
		public static int GetXForAlignment(AlignmentX align, int width, int margin, int objectWidth)
		{
			switch (align)
			{
			case AlignmentX.Left:
				return margin;
			case AlignmentX.Right:
				return width - (objectWidth + margin);
			}
			return width / 2 - objectWidth / 2;
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x000A0CF4 File Offset: 0x0009EEF4
		public static void LoadImageFromContentOrExtension(string imagePath, ContentManager content, Action<Texture2D> LoadComplete)
		{
			Texture2D obj = null;
			if (imagePath != null)
			{
				string text = Utils.GetFileLoadPrefix() + imagePath;
				if (text.EndsWith(".jpg") || text.EndsWith(".png"))
				{
					if (File.Exists(text))
					{
						using (FileStream fileStream = File.OpenRead(text))
						{
							obj = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, fileStream);
						}
					}
				}
				else if (File.Exists(text + ".xnb"))
				{
					obj = content.Load<Texture2D>("../" + text);
				}
				else
				{
					obj = content.Load<Texture2D>(imagePath);
				}
				if (LoadComplete != null)
				{
					LoadComplete(obj);
				}
			}
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x000A0DDC File Offset: 0x0009EFDC
		public static float RobustReadAsFloat(XmlReader rdr)
		{
			string text = rdr.ReadContentAsString();
			text = text.Replace(",", ".");
			return (float)Convert.ToDouble(text, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x000A0E14 File Offset: 0x0009F014
		public static string[] GetQuoteSeperatedArgs(string[] args)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < args.Length; i++)
			{
				stringBuilder.Append(args[i]);
				stringBuilder.Append(" ");
			}
			string text = stringBuilder.ToString().Trim();
			bool flag = false;
			StringBuilder stringBuilder2 = null;
			int j = -1;
			while (j < text.Length - 1)
			{
				j++;
				char c = text[j];
				if (stringBuilder2 == null)
				{
					if (flag && c == '"')
					{
						flag = false;
					}
					else
					{
						if (!flag)
						{
							if (c == ' ')
							{
								continue;
							}
							if (c == '"')
							{
								flag = true;
								continue;
							}
						}
						stringBuilder2 = new StringBuilder();
						stringBuilder2.Append(c);
					}
				}
				else if (flag && c == '"')
				{
					list.Add(stringBuilder2.ToString());
					flag = false;
					stringBuilder2 = null;
				}
				else if (!flag && c == ' ')
				{
					list.Add(stringBuilder2.ToString());
					flag = false;
					stringBuilder2 = null;
				}
				else
				{
					stringBuilder2.Append(c);
				}
			}
			if (stringBuilder2 != null)
			{
				list.Add(stringBuilder2.ToString());
			}
			return list.ToArray();
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x000A0FA0 File Offset: 0x0009F1A0
		public static SpriteFont GetTitleFontForLocalizedString(string data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				if (!GuiData.titlefont.Characters.Contains(data[i]))
				{
					return GuiData.font;
				}
			}
			return GuiData.titlefont;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x000A0FF0 File Offset: 0x0009F1F0
		public static void DrawStringMonospace(SpriteBatch spriteBatch, string text, SpriteFont font, Vector2 pos, Color c, float charWidth)
		{
			for (int i = 0; i < text.Length; i++)
			{
				spriteBatch.DrawString(font, string.Concat(text[i]), Utils.ClipVec2ForTextRendering(pos), c);
				pos.X += charWidth;
			}
		}

		// Token: 0x04000B42 RID: 2882
		public static float PARRALAX_MULTIPLIER = 1f;

		// Token: 0x04000B43 RID: 2883
		public static float MIN_DIFF_FOR_PARRALAX = 0.1f;

		// Token: 0x04000B44 RID: 2884
		public static Random random = new Random();

		// Token: 0x04000B45 RID: 2885
		public static byte[] byteBuffer = new byte[1];

		// Token: 0x04000B46 RID: 2886
		public static readonly string LevelStateFilename = "LevelState.lst";

		// Token: 0x04000B47 RID: 2887
		public static Texture2D white;

		// Token: 0x04000B48 RID: 2888
		public static Texture2D gradient;

		// Token: 0x04000B49 RID: 2889
		public static Texture2D gradientLeftRight;

		// Token: 0x04000B4A RID: 2890
		public static AudioEmitter emitter;

		// Token: 0x04000B4B RID: 2891
		public static Vector3 vec3;

		// Token: 0x04000B4C RID: 2892
		public static StorageDevice device;

		// Token: 0x04000B4D RID: 2893
		public static Color col;

		// Token: 0x04000B4E RID: 2894
		public static Color VeryDarkGray = new Color(22, 22, 22);

		// Token: 0x04000B4F RID: 2895
		public static Color SlightlyDarkGray = new Color(100, 100, 100);

		// Token: 0x04000B50 RID: 2896
		public static Color AddativeWhite = new Color(255, 255, 255, 0);

		// Token: 0x04000B51 RID: 2897
		public static Color AddativeRed = new Color(255, 15, 15, 0);

		// Token: 0x04000B52 RID: 2898
		private static HSLColor hslColor = new HSLColor(1f, 1f, 1f);

		// Token: 0x04000B53 RID: 2899
		public static char[] newlineDelim = new char[]
		{
			'\n'
		};

		// Token: 0x04000B54 RID: 2900
		public static string[] robustNewlineDelim = new string[]
		{
			"\r\n",
			"\n"
		};

		// Token: 0x04000B55 RID: 2901
		public static char[] spaceDelim = new char[]
		{
			' '
		};

		// Token: 0x04000B56 RID: 2902
		public static string[] commaDelim = new string[]
		{
			" ,",
			", ",
			","
		};

		// Token: 0x04000B57 RID: 2903
		public static string[] directorySplitterDelim = new string[]
		{
			"/",
			"\\"
		};

		// Token: 0x04000B58 RID: 2904
		public static string[] WhitespaceDelim = new string[]
		{
			"\r\n",
			"\n",
			" "
		};

		// Token: 0x04000B59 RID: 2905
		public static LCG LCG = new LCG(true);
	}
}
