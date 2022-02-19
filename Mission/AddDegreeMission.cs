using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x02000132 RID: 306
	internal class AddDegreeMission : MisisonGoal
	{
		// Token: 0x06000767 RID: 1895 RVA: 0x0007A5F8 File Offset: 0x000787F8
		public AddDegreeMission(string targetName, string degreeName, string uniName, float desiredGPA, OS _os)
		{
			AddDegreeMission.<>c__DisplayClass1 CS$<>8__locals1 = new AddDegreeMission.<>c__DisplayClass1();
			CS$<>8__locals1._os = _os;
			base..ctor();
			CS$<>8__locals1.<>4__this = this;
			this.ownerName = targetName;
			this.degreeName = degreeName;
			this.uniName = uniName;
			this.desiredGPA = desiredGPA;
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

		// Token: 0x06000768 RID: 1896 RVA: 0x0007A674 File Offset: 0x00078874
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return this.database.doesDegreeExist(this.ownerName, this.degreeName, this.uniName, this.desiredGPA);
		}

		// Token: 0x04000847 RID: 2119
		public AcademicDatabaseDaemon database;

		// Token: 0x04000848 RID: 2120
		private string ownerName;

		// Token: 0x04000849 RID: 2121
		private string degreeName;

		// Token: 0x0400084A RID: 2122
		private string uniName;

		// Token: 0x0400084B RID: 2123
		private float desiredGPA;
	}
}
