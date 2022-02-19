using System;
using System.Collections.Generic;

namespace Hacknet.Mission
{
	// Token: 0x020000D5 RID: 213
	internal class SendEmailMission : MisisonGoal
	{
		// Token: 0x06000449 RID: 1097 RVA: 0x000453F0 File Offset: 0x000435F0
		public SendEmailMission(string mailServerID, string mailRecipient, string proposedEmailSubject, OS _os)
		{
			Computer computer = Programs.getComputer(_os, mailServerID);
			this.server = (MailServer)computer.getDaemon(typeof(MailServer));
			this.mailSubject = proposedEmailSubject;
			this.mailRecipient = mailRecipient;
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x00045438 File Offset: 0x00043638
		public override bool isComplete(List<string> additionalDetails = null)
		{
			return this.server == null || this.server.MailWithSubjectExists(this.mailRecipient, this.mailSubject);
		}

		// Token: 0x0600044B RID: 1099 RVA: 0x00045476 File Offset: 0x00043676
		public override void reset()
		{
		}

		// Token: 0x04000528 RID: 1320
		private string mailSubject;

		// Token: 0x04000529 RID: 1321
		private string mailRecipient;

		// Token: 0x0400052A RID: 1322
		private MailServer server;
	}
}
