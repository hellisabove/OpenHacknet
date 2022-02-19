using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x020000D3 RID: 211
	internal class DeathRowRecordRemovalMission : MisisonGoal
	{
		// Token: 0x06000441 RID: 1089 RVA: 0x00044C40 File Offset: 0x00042E40
		public DeathRowRecordRemovalMission(string firstName, string lastName, OS _os)
		{
			this.os = _os;
			this.fname = firstName;
			this.lname = lastName;
			Computer computer = Programs.getComputer(this.os, "deathRow");
			this.deathRowDatabase = computer;
			this.container = computer.getFolderFromPath("dr_database/Records", false);
		}

		// Token: 0x06000442 RID: 1090 RVA: 0x00044C98 File Offset: 0x00042E98
		public override bool isComplete(List<string> additionalDetails = null)
		{
			DeathRowDatabaseDaemon deathRowDatabaseDaemon = (DeathRowDatabaseDaemon)this.deathRowDatabase.getDaemon(typeof(DeathRowDatabaseDaemon));
			return !deathRowDatabaseDaemon.ContainsRecordForName(this.fname, this.lname);
		}

		// Token: 0x04000522 RID: 1314
		public Folder container;

		// Token: 0x04000523 RID: 1315
		public string fname;

		// Token: 0x04000524 RID: 1316
		public string lname;

		// Token: 0x04000525 RID: 1317
		public Computer deathRowDatabase;

		// Token: 0x04000526 RID: 1318
		public OS os;
	}
}
