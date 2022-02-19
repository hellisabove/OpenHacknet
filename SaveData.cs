using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x02000156 RID: 342
	[Serializable]
	public class SaveData
	{
		// Token: 0x0600089C RID: 2204 RVA: 0x000916C7 File Offset: 0x0008F8C7
		public void addNodes(object nodesToAdd)
		{
			this.nodes = (List<Computer>)nodesToAdd;
		}

		// Token: 0x0600089D RID: 2205 RVA: 0x000916D6 File Offset: 0x0008F8D6
		public void setMission(object missionToAdd)
		{
			this.mission = (ActiveMission)missionToAdd;
		}

		// Token: 0x0600089E RID: 2206 RVA: 0x000916E8 File Offset: 0x0008F8E8
		public object getNodes()
		{
			return this.nodes;
		}

		// Token: 0x0600089F RID: 2207 RVA: 0x00091700 File Offset: 0x0008F900
		public object getMission()
		{
			return this.mission;
		}

		// Token: 0x04000A04 RID: 2564
		private List<Computer> nodes;

		// Token: 0x04000A05 RID: 2565
		private ActiveMission mission;
	}
}
