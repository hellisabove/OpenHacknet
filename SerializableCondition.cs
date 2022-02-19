using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004A RID: 74
	public abstract class SerializableCondition
	{
		// Token: 0x0600017B RID: 379
		public abstract bool Check(object os_obj);

		// Token: 0x0600017C RID: 380 RVA: 0x00014DC4 File Offset: 0x00012FC4
		public string GetSaveString(string bodyContent)
		{
			Type type = base.GetType();
			string text = type.Name;
			if (text.StartsWith("Hacknet."))
			{
				text = text.Substring("Hacknet.".Length);
			}
			if (text.StartsWith("SC"))
			{
				text = text.Substring("SC".Length);
			}
			StringBuilder stringBuilder = new StringBuilder("<" + text + " ");
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				stringBuilder.Append(fields[i].Name + "=\"");
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", new object[]
				{
					fields[i].GetValue(this)
				});
				stringBuilder.Append("\" ");
			}
			stringBuilder.Append(">\r\n");
			stringBuilder.Append(bodyContent);
			stringBuilder.Append("\r\n</" + text + ">");
			return stringBuilder.ToString();
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00014EF0 File Offset: 0x000130F0
		public static SerializableCondition Deserialize(XmlReader rdr, Action<XmlReader, string> bodyContentReadAction)
		{
			Dictionary<string, Func<XmlReader, SerializableCondition>> dictionary = new Dictionary<string, Func<XmlReader, SerializableCondition>>();
			dictionary.Add("OnAdminGained", new Func<XmlReader, SerializableCondition>(SCOnAdminGained.DeserializeFromReader));
			dictionary.Add("OnConnect", new Func<XmlReader, SerializableCondition>(SCOnConnect.DeserializeFromReader));
			dictionary.Add("HasFlags", new Func<XmlReader, SerializableCondition>(SCHasFlags.DeserializeFromReader));
			dictionary.Add("Instantly", new Func<XmlReader, SerializableCondition>(SCInstantly.DeserializeFromReader));
			dictionary.Add("OnDisconnect", new Func<XmlReader, SerializableCondition>(SCOnDisconnect.DeserializeFromReader));
			while (!rdr.EOF && (!rdr.IsStartElement() || !dictionary.ContainsKey(rdr.Name)))
			{
				rdr.Read();
			}
			if (rdr.EOF)
			{
				throw new FormatException("Unexpected end of file!");
			}
			string name = rdr.Name;
			SerializableCondition result = dictionary[rdr.Name](rdr);
			rdr.Read();
			if (bodyContentReadAction != null)
			{
				bodyContentReadAction(rdr, name);
			}
			rdr.Read();
			return result;
		}
	}
}
