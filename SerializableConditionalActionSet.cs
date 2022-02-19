using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000051 RID: 81
	public class SerializableConditionalActionSet
	{
		// Token: 0x06000191 RID: 401 RVA: 0x00015588 File Offset: 0x00013788
		public string GetSaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				stringBuilder.Append(this.Actions[i].GetSaveString());
				stringBuilder.Append("\r\n");
			}
			return this.Condition.GetSaveString(stringBuilder.ToString());
		}

		// Token: 0x06000192 RID: 402 RVA: 0x000156F4 File Offset: 0x000138F4
		public static SerializableConditionalActionSet Deserialize(XmlReader rdr)
		{
			SerializableConditionalActionSet ret = new SerializableConditionalActionSet();
			Action<XmlReader, string> bodyContentReadAction = delegate(XmlReader xmlReader, string EndKeyName)
			{
				while (!rdr.EOF && (string.IsNullOrWhiteSpace(rdr.Name) || rdr.NodeType == XmlNodeType.Comment || rdr.NodeType == XmlNodeType.Whitespace))
				{
					rdr.Read();
				}
				bool flag = !xmlReader.EOF && (!(xmlReader.Name == EndKeyName) || xmlReader.IsStartElement());
				while (flag)
				{
					SerializableAction item = SerializableAction.Deserialize(xmlReader);
					ret.Actions.Add(item);
					do
					{
						xmlReader.Read();
					}
					while (xmlReader.NodeType == XmlNodeType.Whitespace || xmlReader.NodeType == XmlNodeType.Comment);
					flag = (!xmlReader.EOF && (!(xmlReader.Name == EndKeyName) || xmlReader.IsStartElement()));
				}
			};
			ret.Condition = SerializableCondition.Deserialize(rdr, bodyContentReadAction);
			return ret;
		}

		// Token: 0x04000187 RID: 391
		public SerializableCondition Condition;

		// Token: 0x04000188 RID: 392
		public List<SerializableAction> Actions = new List<SerializableAction>();
	}
}
