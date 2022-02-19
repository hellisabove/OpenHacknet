using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200002B RID: 43
	public static class ReflectiveRenderer
	{
		// Token: 0x0600011D RID: 285 RVA: 0x00011224 File Offset: 0x0000F424
		public static int GetEntryLineHeight()
		{
			return (int)(GuiData.ActiveFontConfig.tinyFontCharHeight * 2f + 2f);
		}

		// Token: 0x0600011E RID: 286 RVA: 0x000114A0 File Offset: 0x0000F6A0
		public static void RenderObject(object o, Rectangle bounds, SpriteBatch spriteBatch, ScrollableSectionedPanel panel, Color TitleColor)
		{
			Type type = o.GetType();
			List<ReflectiveRenderer.RenderableField> fields = ReflectiveRenderer.GetRenderablesFromType(type, o, 0);
			int entryLineHeight = ReflectiveRenderer.GetEntryLineHeight();
			panel.PanelHeight = entryLineHeight;
			panel.NumberOfPanels = fields.Count;
			int pixelsPerIndentLevel = 20;
			panel.Draw(delegate(int i, Rectangle dest, SpriteBatch sb)
			{
				int num = fields[i].IndentLevel * pixelsPerIndentLevel;
				dest.X += num;
				dest.Width -= num;
				if (fields[i].IsTitle)
				{
					TextItem.doFontLabelToSize(dest, fields[i].RenderedValue, GuiData.font, TitleColor, true, true);
					Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height - 1, dest.Width, 1);
					sb.Draw(Utils.white, destinationRectangle, TitleColor);
				}
				else
				{
					string text = fields[i].VariableName + " :";
					Vector2 vector = GuiData.smallfont.MeasureString(text);
					sb.DrawString(GuiData.smallfont, text, new Vector2((float)dest.X, (float)dest.Y), Color.Gray);
					string text2 = fields[i].RenderedValue;
					Vector2 vector2 = GuiData.smallfont.MeasureString(text2);
					if (vector2.X > (float)(dest.Width - 20) || vector2.Y > (float)dest.Height)
					{
						text2.Replace("\n", " ");
						text2 = text2.Substring(0, Math.Min(text2.Length, (int)((float)dest.Width / (GuiData.ActiveFontConfig.tinyFontCharHeight + 2f)))) + "...";
					}
					Vector2 vector3 = new Vector2((float)dest.X + vector.X + 6f, (float)dest.Y);
					if (ReflectiveRenderer.PreRenderForObject != null && sb.Name != "AltRTBatch")
					{
						ReflectiveRenderer.PreRenderForObject(vector3, fields[i].t, fields[i].RenderedValue);
					}
					sb.DrawString(GuiData.smallfont, text2, vector3, Color.White);
				}
			}, spriteBatch, bounds);
			ReflectiveRenderer.PreRenderForObject = null;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00011514 File Offset: 0x0000F714
		private static List<ReflectiveRenderer.RenderableField> GetRenderablesFromType(Type type, object o, int indentLevel = 0)
		{
			List<ReflectiveRenderer.RenderableField> list = new List<ReflectiveRenderer.RenderableField>();
			if (ObjectSerializer.IsSimple(type))
			{
				list.Add(new ReflectiveRenderer.RenderableField
				{
					VariableName = "Data",
					RenderedValue = o.ToString(),
					IsTitle = false,
					IndentLevel = indentLevel,
					t = type
				});
			}
			else if (ObjectSerializer.TypeInstanceOfInterface(type, typeof(ICollection)))
			{
				ICollection collection = o as ICollection;
				foreach (object obj in collection)
				{
					list.AddRange(ReflectiveRenderer.GetRenderablesFromType(obj.GetType(), obj, indentLevel + 1));
				}
			}
			else
			{
				list.Add(new ReflectiveRenderer.RenderableField
				{
					IsTitle = true,
					RenderedValue = ReflectiveRenderer.FilterTypeName(type.Name),
					IndentLevel = indentLevel
				});
				FieldInfo[] fields = type.GetFields();
				for (int i = 0; i < fields.Length; i++)
				{
					if (ObjectSerializer.IsSimple(fields[i].FieldType))
					{
						list.Add(new ReflectiveRenderer.RenderableField
						{
							VariableName = fields[i].Name,
							RenderedValue = fields[i].GetValue(o).ToString(),
							IsTitle = false,
							IndentLevel = indentLevel,
							t = fields[i].FieldType
						});
					}
					else
					{
						list.AddRange(ReflectiveRenderer.GetRenderablesFromType(fields[i].FieldType, fields[i].GetValue(o), indentLevel + 1));
					}
				}
				PropertyInfo[] properties = type.GetProperties();
				for (int i = 0; i < properties.Length; i++)
				{
					if (ObjectSerializer.IsSimple(properties[i].PropertyType))
					{
						list.Add(new ReflectiveRenderer.RenderableField
						{
							VariableName = properties[i].Name,
							RenderedValue = properties[i].GetValue(o, null).ToString(),
							IsTitle = false,
							IndentLevel = indentLevel,
							t = properties[i].PropertyType
						});
					}
					else
					{
						list.AddRange(ReflectiveRenderer.GetRenderablesFromType(properties[i].PropertyType, properties[i].GetValue(o, null), indentLevel + 1));
					}
				}
			}
			return list;
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000117D8 File Offset: 0x0000F9D8
		private static string FilterTypeName(string name)
		{
			return name.Replace("Hacknet.", "");
		}

		// Token: 0x04000110 RID: 272
		public static Action<Vector2, Type, string> PreRenderForObject;

		// Token: 0x0200002C RID: 44
		private struct RenderableField
		{
			// Token: 0x06000121 RID: 289 RVA: 0x000117FC File Offset: 0x0000F9FC
			public override string ToString()
			{
				return (this.IsTitle ? "TITLE" : this.VariableName) + ": " + this.RenderedValue;
			}

			// Token: 0x04000111 RID: 273
			public string VariableName;

			// Token: 0x04000112 RID: 274
			public string RenderedValue;

			// Token: 0x04000113 RID: 275
			public bool IsTitle;

			// Token: 0x04000114 RID: 276
			public Type t;

			// Token: 0x04000115 RID: 277
			public int IndentLevel;
		}
	}
}
