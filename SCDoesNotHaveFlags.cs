using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004D RID: 77
	public class SCDoesNotHaveFlags : SerializableCondition
	{
		// Token: 0x06000185 RID: 389 RVA: 0x00015260 File Offset: 0x00013460
		public override bool Check(object os_obj)
		{
			OS os = (OS)os_obj;
			if (!string.IsNullOrWhiteSpace(this.Flags))
			{
				string[] array = this.Flags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					if (os.Flags.HasFlag(array[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x000152D0 File Offset: 0x000134D0
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCDoesNotHaveFlags scdoesNotHaveFlags = new SCDoesNotHaveFlags();
			if (rdr.MoveToAttribute("Flags"))
			{
				scdoesNotHaveFlags.Flags = rdr.ReadContentAsString();
			}
			return scdoesNotHaveFlags;
		}

		// Token: 0x04000182 RID: 386
		public string Flags;
	}
}
