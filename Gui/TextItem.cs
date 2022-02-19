using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Gui
{
	// Token: 0x02000121 RID: 289
	public static class TextItem
	{
		// Token: 0x060006C7 RID: 1735 RVA: 0x000703C4 File Offset: 0x0006E5C4
		public static Vector2 doMeasuredLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 result = GuiData.font.MeasureString(text);
			GuiData.spriteBatch.DrawString(GuiData.font, text, pos + Vector2.One, Color.Gray);
			GuiData.spriteBatch.DrawString(GuiData.font, text, pos, color.Value);
			return result;
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x00070438 File Offset: 0x0006E638
		public static void doLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			GuiData.spriteBatch.DrawString(GuiData.font, text, pos, color.Value);
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x00070478 File Offset: 0x0006E678
		public static void doLabel(Vector2 pos, string text, Color? color, float MaxWidth)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			TextItem.doFontLabel(pos, text, GuiData.font, color, MaxWidth, float.MaxValue, false);
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x000704B4 File Offset: 0x0006E6B4
		public static void doFontLabelToSize(Rectangle dest, string text, SpriteFont font, Color color, bool doNotOversize = false, bool offsetToTopLeft = false)
		{
			Vector2 vector = font.MeasureString(text);
			Vector2 scale = new Vector2((float)dest.Width / vector.X, (float)dest.Height / vector.Y);
			float num = Math.Min(scale.X, scale.Y);
			Vector2 zero = Vector2.Zero;
			if (scale.X > num)
			{
				zero.X = vector.X * (scale.X - num) / 2f;
				scale.X = num;
			}
			if (scale.Y > num)
			{
				zero.Y = vector.Y * (scale.Y - num) / 2f;
				scale.Y = num;
			}
			if (doNotOversize)
			{
				scale.X = Math.Min(scale.X, 1f);
				scale.Y = Math.Min(scale.Y, 1f);
			}
			Vector2 vector2 = new Vector2((float)dest.X, (float)dest.Y);
			if (!offsetToTopLeft)
			{
				vector2 += zero;
			}
			Vector2 position = Utils.ClipVec2ForTextRendering(vector2);
			GuiData.spriteBatch.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.55f);
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x00070614 File Offset: 0x0006E814
		public static void doCenteredFontLabel(Rectangle dest, string text, SpriteFont font, Color color, bool LockToLeft = false)
		{
			Vector2 vector = font.MeasureString(text);
			Vector2 vector2 = new Vector2((float)dest.Width / vector.X, (float)dest.Height / vector.Y);
			if (Math.Min(vector2.X, vector2.Y) < 1f)
			{
				TextItem.doFontLabelToSize(dest, text, font, color, true, false);
			}
			else
			{
				Vector2 position = Utils.ClipVec2ForTextRendering(new Vector2((float)dest.X + (float)dest.Width / 2f - vector.X / 2f, (float)dest.Y + (float)dest.Height / 2f - vector.Y / 2f));
				if (LockToLeft)
				{
					position.X = (float)dest.X;
				}
				GuiData.spriteBatch.DrawString(font, text, position, color);
			}
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x00070704 File Offset: 0x0006E904
		public static void doFontLabel(Vector2 pos, string text, SpriteFont font, Color? color, float widthTo = 3.4028235E+38f, float heightTo = 3.4028235E+38f, bool centreVertically = false)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 scale = font.MeasureString(text);
			float y = scale.Y;
			if (scale.X > widthTo)
			{
				scale.X = widthTo / scale.X;
			}
			else
			{
				scale.X = 1f;
			}
			if (scale.Y > heightTo)
			{
				scale.Y = heightTo / scale.Y;
			}
			else
			{
				scale.Y = 1f;
			}
			scale.X = (scale.Y = Math.Min(scale.X, scale.Y));
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(font, text, pos + Vector2.One, Color.Gray, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
			}
			Vector2 zero = Vector2.Zero;
			if (centreVertically && heightTo < 3.4028235E+38f)
			{
				float y2 = (heightTo - y * scale.Y) / 2f;
				zero = new Vector2(0f, y2);
			}
			Vector2 position = Utils.ClipVec2ForTextRendering(pos + zero);
			GuiData.spriteBatch.DrawString(font, text, position, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0007087C File Offset: 0x0006EA7C
		public static void doRightAlignedBackingLabel(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
		{
			GuiData.spriteBatch.Draw(Utils.white, dest, back);
			Vector2 vector = font.MeasureString(msg);
			Vector2 position = new Vector2((float)(dest.X + dest.Width - 4) - vector.X, (float)(dest.Y + dest.Height - 2) - vector.Y);
			GuiData.spriteBatch.DrawString(font, msg, position, front);
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x000708F0 File Offset: 0x0006EAF0
		public static void doRightAlignedBackingLabelScaled(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
		{
			GuiData.spriteBatch.Draw(Utils.white, dest, back);
			Vector2 value = font.MeasureString(msg);
			Vector2[] stringScaleForSize = TextItem.GetStringScaleForSize(font, msg, dest);
			Vector2 vector = stringScaleForSize[0];
			Vector2 vector2 = stringScaleForSize[1];
			Vector2 vector3 = value * vector;
			Vector2 position = new Vector2((float)(dest.X + dest.Width) - vector3.X, (float)(dest.Y + dest.Height / 2) - vector3.Y / 2f);
			GuiData.spriteBatch.DrawString(font, msg, position, front, 0f, Vector2.Zero, vector, SpriteEffects.None, 0.6f);
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x000709A8 File Offset: 0x0006EBA8
		public static void doRightAlignedBackingLabelFill(Rectangle dest, string msg, SpriteFont font, Color back, Color front)
		{
			GuiData.spriteBatch.Draw(Utils.white, dest, back);
			Vector2 vector = font.MeasureString(msg);
			Rectangle dest2 = dest;
			dest2.Width -= 7;
			dest2.X += 4;
			Vector2[] stringScaleForSize = TextItem.GetStringScaleForSize(font, msg, dest2);
			Vector2 scale = stringScaleForSize[0];
			Vector2 value = stringScaleForSize[1];
			value.Y *= 2f;
			value.Y -= 3f;
			Vector2 value2 = new Vector2((float)dest2.X, (float)dest.Y);
			GuiData.spriteBatch.DrawString(font, msg, value2 + value, front, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.6f);
			Rectangle destinationRectangle = new Rectangle(dest.X, (int)((float)dest.Y + value.Y - 4f + vector.Y * scale.Y), dest.Width, 1);
			GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, front);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x00070AD0 File Offset: 0x0006ECD0
		private static Vector2[] GetStringScaleForSize(SpriteFont font, string text, Rectangle dest)
		{
			Vector2 vector = font.MeasureString(text);
			Vector2 vector2 = new Vector2((float)dest.Width / vector.X, (float)dest.Height / vector.Y);
			float num = Math.Min(vector2.X, vector2.Y);
			Vector2 zero = Vector2.Zero;
			if (vector2.X > num)
			{
				zero.X = vector.X * (vector2.X - num) / 2f;
				vector2.X = num;
			}
			if (vector2.Y > num)
			{
				zero.Y = vector.Y * (vector2.Y - num) / 2f;
				vector2.Y = num;
			}
			return new Vector2[]
			{
				vector2,
				zero
			};
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x00070BCC File Offset: 0x0006EDCC
		public static Vector2 doMeasuredFontLabel(Vector2 pos, string text, SpriteFont font, Color? color, float widthTo = 3.4028235E+38f, float heightTo = 3.4028235E+38f)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 vector = font.MeasureString(text);
			if (vector.X > widthTo)
			{
				vector.X = widthTo / vector.X;
			}
			else
			{
				vector.X = 1f;
			}
			if (vector.Y > heightTo)
			{
				vector.Y = heightTo / vector.Y;
			}
			else
			{
				vector.Y = 1f;
			}
			vector.X = (vector.Y = Math.Min(vector.X, vector.Y));
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(font, text, pos + Vector2.One, Color.Gray, 0f, Vector2.Zero, vector, SpriteEffects.None, 0.5f);
			}
			GuiData.spriteBatch.DrawString(font, text, pos, color.Value, 0f, Vector2.Zero, vector, SpriteEffects.None, 0.5f);
			return font.MeasureString(text) * vector;
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x00070CF4 File Offset: 0x0006EEF4
		public static void doSmallLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray);
			}
			GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value);
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x00070D60 File Offset: 0x0006EF60
		public static void doTinyLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos, color.Value);
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x00070DA0 File Offset: 0x0006EFA0
		public static void doSmallLabel(Vector2 pos, string text, Color? color, float widthTo, float heightTo)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 scale = GuiData.font.MeasureString(text);
			if (scale.X > widthTo)
			{
				scale.X = widthTo / scale.X;
			}
			else
			{
				scale.X = 1f;
			}
			if (scale.Y > heightTo)
			{
				scale.Y = heightTo / scale.Y;
			}
			else
			{
				scale.Y = 1f;
			}
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
			}
			GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value, 0f, Vector2.Zero, scale, SpriteEffects.None, 0.5f);
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x00070E9C File Offset: 0x0006F09C
		public static Vector2 doMeasuredSmallLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 result = GuiData.smallfont.MeasureString(text);
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos + Vector2.One, Color.Gray);
			}
			GuiData.spriteBatch.DrawString(GuiData.smallfont, text, pos, color.Value);
			return result;
		}

		// Token: 0x060006D6 RID: 1750 RVA: 0x00070F1C File Offset: 0x0006F11C
		public static Vector2 doMeasuredTinyLabel(Vector2 pos, string text, Color? color)
		{
			if (color == null)
			{
				color = new Color?(Color.White);
			}
			Vector2 result = GuiData.tinyfont.MeasureString(text);
			if (TextItem.DrawShadow)
			{
				GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos + Vector2.One, Color.Gray);
			}
			GuiData.spriteBatch.DrawString(GuiData.tinyfont, text, pos, color.Value);
			return result;
		}

		// Token: 0x040007AA RID: 1962
		public static bool DrawShadow = false;
	}
}
