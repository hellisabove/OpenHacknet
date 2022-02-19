using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x020000EB RID: 235
	public class ProgressionFlags
	{
		// Token: 0x060004E8 RID: 1256 RVA: 0x0004E098 File Offset: 0x0004C298
		public bool HasFlag(string flag)
		{
			return this.Flags.Contains(flag);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0004E0B8 File Offset: 0x0004C2B8
		public void AddFlag(string flag)
		{
			if (!this.HasFlag(flag))
			{
				this.Flags.Add(flag);
			}
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0004E0DE File Offset: 0x0004C2DE
		public void RemoveFlag(string flag)
		{
			this.Flags.Remove(flag);
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0004E0F0 File Offset: 0x0004C2F0
		public string GetFlagStartingWith(string start)
		{
			for (int i = 0; i < this.Flags.Count; i++)
			{
				if (this.Flags[i].StartsWith(start))
				{
					return this.Flags[i];
				}
			}
			return null;
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0004E148 File Offset: 0x0004C348
		public void Load(XmlReader rdr)
		{
			this.Flags.Clear();
			while (!(rdr.Name == "Flags") || !rdr.IsStartElement())
			{
				rdr.Read();
				if (rdr.EOF)
				{
					throw new InvalidOperationException("XML reached End of file too fast!");
				}
			}
			rdr.MoveToContent();
			string text = rdr.ReadElementContentAsString();
			string[] array = text.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Replace("[%%COMMAREPLACED%%]", ",");
				if (text2 == "décrypté")
				{
					text2 = "decypher";
				}
				this.Flags.Add(text2);
			}
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0004E224 File Offset: 0x0004C424
		public string GetSaveString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < this.Flags.Count; i++)
			{
				stringBuilder.Append(this.Flags[i].Replace(",", "[%%COMMAREPLACED%%]"));
				stringBuilder.Append(",");
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}
			return "<Flags>" + stringBuilder.ToString() + "</Flags>\r\n";
		}

		// Token: 0x04000595 RID: 1429
		private List<string> Flags = new List<string>();
	}
}
