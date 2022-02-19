using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x020000D2 RID: 210
	internal class DeathRowRecordModifyMission : MisisonGoal
	{
		// Token: 0x0600043F RID: 1087 RVA: 0x00044AD0 File Offset: 0x00042CD0
		public DeathRowRecordModifyMission(string firstName, string lastName, string lastWords, OS _os)
		{
			this.os = _os;
			this.fname = firstName;
			this.lname = lastName;
			this.lastWords = lastWords;
			Computer computer = Programs.getComputer(this.os, "deathRow");
			this.deathRowDatabase = computer;
			this.container = computer.getFolderFromPath("dr_database/Records", false);
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00044B30 File Offset: 0x00042D30
		public override bool isComplete(List<string> additionalDetails = null)
		{
			DeathRowDatabaseDaemon deathRowDatabaseDaemon = (DeathRowDatabaseDaemon)this.deathRowDatabase.getDaemon(typeof(DeathRowDatabaseDaemon));
			DeathRowDatabaseDaemon.DeathRowEntry recordForName = deathRowDatabaseDaemon.GetRecordForName(this.fname, this.lname);
			bool result;
			if (recordForName.RecordNumber != null)
			{
				if (this.lastWords != null)
				{
					string text = recordForName.Statement.ToLower().Replace("\r", "").Replace(",", "").Replace(".", "");
					string text2 = this.lastWords.ToLower().Replace("\r", "").Replace(",", "").Replace(".", "");
					if (text.Contains(text2) || text == text2 || text.StartsWith(text2))
					{
						return true;
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0400051C RID: 1308
		public Folder container;

		// Token: 0x0400051D RID: 1309
		public string fname;

		// Token: 0x0400051E RID: 1310
		public string lname;

		// Token: 0x0400051F RID: 1311
		public Computer deathRowDatabase;

		// Token: 0x04000520 RID: 1312
		public OS os;

		// Token: 0x04000521 RID: 1313
		public string lastWords;
	}
}
