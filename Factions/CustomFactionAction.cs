using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Hacknet.Factions
{
	// Token: 0x02000076 RID: 118
	internal class CustomFactionAction
	{
		// Token: 0x06000254 RID: 596 RVA: 0x00021AC8 File Offset: 0x0001FCC8
		public static CustomFactionAction Deserialize(XmlReader rdr)
		{
			CustomFactionAction customFactionAction = new CustomFactionAction();
			while (!rdr.EOF && (!(rdr.Name == "Action") || !rdr.IsStartElement()))
			{
				rdr.Read();
			}
			if (rdr.EOF)
			{
				throw new FormatException("Expected Start element <Action> but did not find it in file!");
			}
			if (rdr.MoveToAttribute("ValueRequired"))
			{
				customFactionAction.ValueRequiredForTrigger = rdr.ReadContentAsInt();
			}
			if (rdr.MoveToAttribute("Flags"))
			{
				customFactionAction.FlagsRequiredForTrigger = rdr.ReadContentAsString();
			}
			rdr.Read();
			while (!rdr.EOF && (!(rdr.Name == "Action") || rdr.IsStartElement()))
			{
				if (string.IsNullOrWhiteSpace(rdr.Name))
				{
					rdr.Read();
				}
				else
				{
					SerializableAction item = SerializableAction.Deserialize(rdr);
					customFactionAction.TriggerActions.Add(item);
					rdr.Read();
					while ((rdr.NodeType == XmlNodeType.Comment || rdr.NodeType == XmlNodeType.Whitespace || rdr.NodeType == XmlNodeType.SignificantWhitespace) && !rdr.EOF)
					{
						rdr.Read();
					}
				}
			}
			if (rdr.EOF)
			{
				throw new FormatException("Unexpected end of file: No closing tag for </Action> Found!");
			}
			return customFactionAction;
		}

		// Token: 0x06000255 RID: 597 RVA: 0x00021C30 File Offset: 0x0001FE30
		public string GetSaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<Action ValueRequired=\"" + this.ValueRequiredForTrigger.ToString(CultureInfo.InvariantCulture) + "\" ");
			if (this.FlagsRequiredForTrigger != null)
			{
				stringBuilder.Append("Flags=\"" + this.FlagsRequiredForTrigger + "\" ");
			}
			stringBuilder.Append(">\n");
			for (int i = 0; i < this.TriggerActions.Count; i++)
			{
				stringBuilder.Append("\t" + this.TriggerActions[i].GetSaveString());
				stringBuilder.Append("\n");
			}
			stringBuilder.Append("</Action>");
			return stringBuilder.ToString();
		}

		// Token: 0x06000256 RID: 598 RVA: 0x00021D00 File Offset: 0x0001FF00
		public void Trigger(object os_obj)
		{
			for (int i = 0; i < this.TriggerActions.Count; i++)
			{
				this.TriggerActions[i].Trigger(os_obj);
			}
		}

		// Token: 0x040002C6 RID: 710
		public const string XML_ELEMENT_NAME = "Action";

		// Token: 0x040002C7 RID: 711
		public int ValueRequiredForTrigger = 10;

		// Token: 0x040002C8 RID: 712
		public string FlagsRequiredForTrigger = null;

		// Token: 0x040002C9 RID: 713
		public List<SerializableAction> TriggerActions = new List<SerializableAction>();
	}
}
