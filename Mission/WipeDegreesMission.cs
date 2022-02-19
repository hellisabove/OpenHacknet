using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000133 RID: 307
	internal class WipeDegreesMission : MisisonGoal
	{
		// Token: 0x06000769 RID: 1897 RVA: 0x0007A74C File Offset: 0x0007894C
		public WipeDegreesMission(string targetName, OS _os)
		{
			WipeDegreesMission.<>c__DisplayClass1 CS$<>8__locals1 = new WipeDegreesMission.<>c__DisplayClass1();
			CS$<>8__locals1._os = _os;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.ownerName = targetName;
			Action init = null;
			init = delegate()
			{
				if (CS$<>8__locals1._os.netMap.academicDatabase == null)
				{
					CS$<>8__locals1._os.delayer.Post(ActionDelayer.NextTick(), init);
				}
				else
				{
					CS$<>8__locals1.<>4__this.database = (AcademicDatabaseDaemon)CS$<>8__locals1._os.netMap.academicDatabase.getDaemon(typeof(AcademicDatabaseDaemon));
				}
			};
			init();
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x0007A7B0 File Offset: 0x000789B0
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return !this.database.hasDegrees(this.ownerName);
		}

		// Token: 0x0400084C RID: 2124
		public AcademicDatabaseDaemon database;

		// Token: 0x0400084D RID: 2125
		private string ownerName;
	}
}
