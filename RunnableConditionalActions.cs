using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200002D RID: 45
	public class RunnableConditionalActions
	{
		// Token: 0x06000122 RID: 290 RVA: 0x00011834 File Offset: 0x0000FA34
		public virtual void Update(float dt, object os)
		{
			this.IsUpdating = true;
			for (int i = 0; i < this.Actions.Count; i++)
			{
				if (this.Actions[i].Condition.Check(os))
				{
					SerializableConditionalActionSet serializableConditionalActionSet = this.Actions[i];
					this.Actions.RemoveAt(i);
					i--;
					for (int j = 0; j < serializableConditionalActionSet.Actions.Count; j++)
					{
						serializableConditionalActionSet.Actions[j].Trigger(os);
					}
				}
			}
			this.IsUpdating = false;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000118DC File Offset: 0x0000FADC
		public string GetSaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<ConditionalActions>\r\n");
			for (int i = 0; i < this.Actions.Count; i++)
			{
				stringBuilder.Append(this.Actions[i].GetSaveString() + "\r\n");
			}
			stringBuilder.Append("</ConditionalActions>");
			return stringBuilder.ToString();
		}

		// Token: 0x06000124 RID: 292 RVA: 0x00011950 File Offset: 0x0000FB50
		public static RunnableConditionalActions Deserialize(XmlReader rdr)
		{
			while (!rdr.EOF && rdr.Name != "ConditionalActions" && !rdr.IsStartElement())
			{
				rdr.Read();
			}
			if (rdr.EOF)
			{
				throw new FormatException("Unexpected end of file trying to deserialize Runnable Conditional Actions!");
			}
			RunnableConditionalActions runnableConditionalActions = new RunnableConditionalActions();
			rdr.Read();
			for (;;)
			{
				while (!rdr.EOF && (rdr.IsEmptyElement || string.IsNullOrWhiteSpace(rdr.Name)))
				{
					rdr.Read();
				}
				if (rdr.EOF)
				{
					break;
				}
				bool flag = !(rdr.Name == "ConditionalActions") || rdr.IsStartElement();
				if (flag)
				{
					SerializableConditionalActionSet item = SerializableConditionalActionSet.Deserialize(rdr);
					runnableConditionalActions.Actions.Add(item);
					rdr.Read();
				}
				if (!flag)
				{
					return runnableConditionalActions;
				}
			}
			throw new FormatException("Unexpected end of file trying to deserialize Runnable Conditional Actions!");
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00011A6C File Offset: 0x0000FC6C
		public static void LoadIntoOS(string filepath, object OSobj)
		{
			OS os = (OS)OSobj;
			using (FileStream fileStream = File.OpenRead(LocalizedFileLoader.GetLocalizedFilepath(Utils.GetFileLoadPrefix() + filepath)))
			{
				XmlReader rdr = XmlReader.Create(fileStream);
				RunnableConditionalActions runnableConditionalActions = RunnableConditionalActions.Deserialize(rdr);
				for (int i = 0; i < runnableConditionalActions.Actions.Count; i++)
				{
					os.ConditionalActions.Actions.Add(runnableConditionalActions.Actions[i]);
				}
			}
			if (!os.ConditionalActions.IsUpdating)
			{
				os.ConditionalActions.Update(0f, os);
			}
		}

		// Token: 0x04000116 RID: 278
		public const string SerializationKey = "ConditionalActions";

		// Token: 0x04000117 RID: 279
		public List<SerializableConditionalActionSet> Actions = new List<SerializableConditionalActionSet>();

		// Token: 0x04000118 RID: 280
		private bool IsUpdating = false;
	}
}
