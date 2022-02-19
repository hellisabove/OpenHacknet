using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004C RID: 76
	public class SCHasFlags : SerializableCondition
	{
		// Token: 0x06000182 RID: 386 RVA: 0x000151B0 File Offset: 0x000133B0
		public override bool Check(object os_obj)
		{
			OS os = (OS)os_obj;
			if (!string.IsNullOrWhiteSpace(this.requiredFlags))
			{
				string[] array = this.requiredFlags.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < array.Length; i++)
				{
					if (!os.Flags.HasFlag(array[i]))
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00015220 File Offset: 0x00013420
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCHasFlags schasFlags = new SCHasFlags();
			if (rdr.MoveToAttribute("requiredFlags"))
			{
				schasFlags.requiredFlags = rdr.ReadContentAsString();
			}
			return schasFlags;
		}

		// Token: 0x04000181 RID: 385
		public string requiredFlags;
	}
}
