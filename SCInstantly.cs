using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200004F RID: 79
	public class SCInstantly : SerializableCondition
	{
		// Token: 0x0600018B RID: 395 RVA: 0x000153B8 File Offset: 0x000135B8
		public override bool Check(object os_obj)
		{
			return !this.needsMissionComplete || ((OS)os_obj).currentMission.isComplete(null);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000153F0 File Offset: 0x000135F0
		public static SerializableCondition DeserializeFromReader(XmlReader rdr)
		{
			SCInstantly scinstantly = new SCInstantly();
			if (rdr.MoveToAttribute("needsMissionComplete"))
			{
				scinstantly.needsMissionComplete = (rdr.ReadContentAsString().ToLower() == "true");
			}
			return scinstantly;
		}

		// Token: 0x04000184 RID: 388
		public bool needsMissionComplete;
	}
}
