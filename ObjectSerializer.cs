using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200002A RID: 42
	public static class ObjectSerializer
	{
		// Token: 0x0600010D RID: 269 RVA: 0x00010734 File Offset: 0x0000E934
		public static string SerializeObject(object o)
		{
			return ObjectSerializer.SerializeObject(o, false);
		}

		// Token: 0x0600010E RID: 270 RVA: 0x00010750 File Offset: 0x0000E950
		private static string SerializeObject(object o, bool preventOuterTag = false)
		{
			string result;
			if (o == null)
			{
				result = "";
			}
			else
			{
				Type type = o.GetType();
				string tagNameForType = ObjectSerializer.GetTagNameForType(type);
				StringBuilder stringBuilder = new StringBuilder();
				if (!preventOuterTag)
				{
					stringBuilder.Append("<" + tagNameForType + ">");
				}
				if (ObjectSerializer.TypeInstanceOfInterface(type, typeof(ICollection)))
				{
					stringBuilder.Append(ObjectSerializer.SerializeCollection((ICollection)o));
				}
				else
				{
					FieldInfo[] fields = type.GetFields();
					for (int i = 0; i < fields.Length; i++)
					{
						string name = fields[i].Name;
						object value = fields[i].GetValue(o);
						if (value != null)
						{
							Type fieldType = fields[i].FieldType;
							string tagValue;
							if (ObjectSerializer.TypeInstanceOfInterface(fieldType, typeof(IFormattable)))
							{
								IFormattable formattable = value as IFormattable;
								tagValue = formattable.ToString("", CultureInfo.InvariantCulture);
							}
							else
							{
								tagValue = value.ToString();
							}
							if (fieldType == typeof(Color))
							{
								tagValue = Utils.convertColorToParseableString((Color)value);
							}
							if (!ObjectSerializer.IsSimple(value.GetType()))
							{
								tagValue = "\n" + ObjectSerializer.SerializeObject(value, true);
							}
							stringBuilder.Append(ObjectSerializer.GetSerializedStringForPrimative(name, tagValue));
						}
					}
				}
				if (!preventOuterTag)
				{
					stringBuilder.Append("\n</" + tagNameForType + ">");
				}
				else
				{
					stringBuilder.Append("\n");
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00010914 File Offset: 0x0000EB14
		public static string GetTagNameForType(Type t)
		{
			string text = t.Name;
			if (text.StartsWith("Hacknet."))
			{
				text = text.Substring("Hacknet.".Length);
			}
			return text.Replace("`", "_");
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00010964 File Offset: 0x0000EB64
		public static bool TypeInstanceOfInterface(Type objType, Type interfaceType)
		{
			return interfaceType.IsAssignableFrom(objType);
		}

		// Token: 0x06000111 RID: 273 RVA: 0x00010980 File Offset: 0x0000EB80
		private static string GetSerializedStringForPrimative(string tagName, string tagValue)
		{
			return string.Format(CultureInfo.InvariantCulture, "\n\t<{0}>{1}</{0}>", new object[]
			{
				tagName,
				tagValue
			});
		}

		// Token: 0x06000112 RID: 274 RVA: 0x000109B4 File Offset: 0x0000EBB4
		private static string SerializeCollection(ICollection o)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("\n");
			foreach (object obj in o)
			{
				string text;
				if (ObjectSerializer.IsSimple(obj.GetType()))
				{
					text = ObjectSerializer.GetSerializedStringForPrimative(ObjectSerializer.GetTagNameForType(obj.GetType()), obj.ToString());
				}
				else
				{
					text = ObjectSerializer.SerializeObject(obj);
				}
				stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "\n\t{0}", new object[]
				{
					text
				}));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00010A90 File Offset: 0x0000EC90
		public static bool IsSimple(Type type)
		{
			bool result;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				result = ObjectSerializer.IsSimple(type.GetGenericArguments()[0]);
			}
			else
			{
				result = (type.IsPrimitive || type.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(decimal)) || type.Equals(typeof(string)) || type.Equals(typeof(DateTime)) || type.Equals(typeof(TimeSpan)));
			}
			return result;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00010B48 File Offset: 0x0000ED48
		public static object DeserializeObject(Stream s, Type t)
		{
			object result;
			using (XmlReader xmlReader = XmlReader.Create(s))
			{
				result = ObjectSerializer.DeserializeObject(xmlReader, t);
			}
			return result;
		}

		// Token: 0x06000115 RID: 277 RVA: 0x00010B8C File Offset: 0x0000ED8C
		public static object DeserializeObject(XmlReader rdr, Type t)
		{
			object result;
			if (ObjectSerializer.IsSimple(t))
			{
				XmlNamespaceManager namespaceResolver = new XmlNamespaceManager(new NameTable());
				result = rdr.ReadElementContentAs(t, namespaceResolver);
			}
			else if (ObjectSerializer.TypeInstanceOfInterface(t, typeof(ICollection)))
			{
				result = ObjectSerializer.DeserializeCollection(rdr, t, "list");
			}
			else
			{
				result = ObjectSerializer.DeserializeXMLObject(rdr, t, null);
			}
			return result;
		}

		// Token: 0x06000116 RID: 278 RVA: 0x00010BF4 File Offset: 0x0000EDF4
		private static object DeserializeXMLObject(XmlReader rdr, Type t, string overrideExpectedEndTag = null)
		{
			object obj = ObjectSerializer.CreateObjectOfType(t);
			FieldInfo[] fields = t.GetFields();
			XmlNamespaceManager resolver = new XmlNamespaceManager(new NameTable());
			string b = ObjectSerializer.GetTagNameForType(t);
			if (overrideExpectedEndTag != null)
			{
				b = overrideExpectedEndTag;
			}
			while (!rdr.EOF)
			{
				if (!string.IsNullOrWhiteSpace(rdr.Name) && rdr.IsStartElement())
				{
					for (int i = 0; i < fields.Length; i++)
					{
						if (fields[i].Name == rdr.Name)
						{
							rdr.MoveToContent();
							object value = null;
							if (fields[i].FieldType == typeof(Color))
							{
								value = Utils.convertStringToColor(rdr.ReadElementContentAsString());
							}
							else if (ObjectSerializer.TypeInstanceOfInterface(fields[i].FieldType, typeof(ICollection)))
							{
								value = ObjectSerializer.DeserializeCollection(rdr, fields[i].FieldType, fields[i].Name);
							}
							else if (!ObjectSerializer.IsSimple(fields[i].FieldType))
							{
								value = ObjectSerializer.DeserializeXMLObject(rdr, fields[i].FieldType, fields[i].Name);
							}
							else
							{
								try
								{
									value = ObjectSerializer.ReadElementContentWithType(rdr, fields[i].FieldType, resolver);
								}
								catch (FormatException)
								{
								}
							}
							fields[i].SetValue(obj, value);
							break;
						}
					}
				}
				rdr.Read();
				if (rdr.Name == b && !rdr.IsStartElement())
				{
					break;
				}
				if (rdr.EOF)
				{
					throw new FormatException();
				}
			}
			return obj;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00010DE4 File Offset: 0x0000EFE4
		private static object ReadElementContentWithType(XmlReader reader, Type type, IXmlNamespaceResolver resolver)
		{
			object result;
			if (type == typeof(DateTime))
			{
				result = DateTime.Parse(reader.ReadElementContentAsString(), CultureInfo.InvariantCulture);
			}
			else if (type == typeof(bool))
			{
				result = (reader.ReadElementContentAsString().ToLower() == "true");
			}
			else if (type == typeof(TimeSpan))
			{
				result = TimeSpan.Parse(reader.ReadElementContentAsString(), CultureInfo.InvariantCulture);
			}
			else if (type.IsEnum)
			{
				string value = reader.ReadElementContentAsString();
				result = Enum.Parse(type, value);
			}
			else
			{
				result = reader.ReadElementContentAs(type, resolver);
			}
			return result;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00010EB8 File Offset: 0x0000F0B8
		private static object CreateObjectOfType(Type targetType)
		{
			object result;
			if (Type.GetTypeCode(targetType) == TypeCode.String)
			{
				result = string.Empty;
			}
			else
			{
				Type[] types = new Type[0];
				ConstructorInfo constructor = targetType.GetConstructor(types);
				object obj;
				if (constructor == null)
				{
					if (!targetType.BaseType.UnderlyingSystemType.FullName.Contains("Enum"))
					{
						throw new ArgumentException("Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Constructor not found");
					}
					obj = Activator.CreateInstance(targetType);
				}
				else
				{
					obj = constructor.Invoke(null);
				}
				if (obj == null)
				{
					throw new ArgumentException("Unable to instantiate type: " + targetType.AssemblyQualifiedName + " - Unknown Error");
				}
				result = obj;
			}
			return result;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00010F84 File Offset: 0x0000F184
		private static object DeserializeCollection(XmlReader rdr, Type t, string tagName = "list")
		{
			object obj = ObjectSerializer.CreateObjectOfType(t);
			IList list = obj as IList;
			if (list == null)
			{
				throw new NotSupportedException();
			}
			bool flag = rdr.Name.ToLower().StartsWith(tagName.ToLower()) && rdr.IsStartElement();
			while (!flag)
			{
				rdr.Read();
				flag = (rdr.Name.ToLower().StartsWith(tagName.ToLower()) && rdr.IsStartElement());
				if (rdr.EOF)
				{
					throw new FormatException();
				}
			}
			bool flag2 = false;
			for (;;)
			{
				rdr.Read();
				if (rdr.IsStartElement())
				{
					string name = rdr.Name;
					Type typeForName = ObjectSerializer.GetTypeForName(name);
					object value = ObjectSerializer.DeserializeObject(rdr, typeForName);
					list.Add(value);
				}
				if (rdr.Name.ToLower().StartsWith(tagName.ToLower()))
				{
					if (!rdr.IsStartElement())
					{
						flag2 = true;
					}
				}
				if (rdr.EOF)
				{
					break;
				}
				if (flag2)
				{
					return list;
				}
			}
			throw new FormatException();
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000110BC File Offset: 0x0000F2BC
		public static Type GetTypeForName(string name)
		{
			Type result;
			if (string.IsNullOrWhiteSpace(name) || name.ToLower() == "none")
			{
				result = null;
			}
			else
			{
				Type type = Type.GetType(name, false, true);
				if (type == null)
				{
					type = Type.GetType("Hacknet." + name, false, true);
				}
				if (type == null)
				{
					type = Type.GetType("System." + name, false, true);
				}
				result = type;
			}
			return result;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00011144 File Offset: 0x0000F344
		public static object DeepCopy(object input)
		{
			string s = ObjectSerializer.SerializeObject(input);
			return ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(s), input.GetType());
		}

		// Token: 0x0600011C RID: 284 RVA: 0x00011170 File Offset: 0x0000F370
		public static object GetValueFromObject(object o, string FieldName)
		{
			Type type = o.GetType();
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i].Name.ToLower() == FieldName.ToLower())
				{
					return properties[i].GetValue(o, null);
				}
			}
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].Name.ToLower() == FieldName.ToLower())
				{
					return fields[i].GetValue(o);
				}
			}
			return null;
		}
	}
}
