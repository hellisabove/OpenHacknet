using System;
using System.IO;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000030 RID: 48
	public class SALoadMission : SerializableAction
	{
		// Token: 0x0600012C RID: 300 RVA: 0x00012008 File Offset: 0x00010208
		public override void Trigger(object os_obj)
		{
			ComputerLoader.loadMission(Utils.GetFileLoadPrefix() + this.MissionName, false);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00012024 File Offset: 0x00010224
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SALoadMission saloadMission = new SALoadMission();
			if (rdr.MoveToAttribute("MissionName"))
			{
				saloadMission.MissionName = rdr.ReadContentAsString();
			}
			if (saloadMission.MissionName == null || !File.Exists(Utils.GetFileLoadPrefix() + saloadMission.MissionName))
			{
				throw new FormatException("Invalid Mission file Path :" + saloadMission.MissionName);
			}
			return saloadMission;
		}

		// Token: 0x04000119 RID: 281
		public string MissionName;
	}
}
