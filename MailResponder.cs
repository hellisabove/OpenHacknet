using System;

namespace Hacknet
{
	// Token: 0x02000129 RID: 297
	internal interface MailResponder
	{
		// Token: 0x06000702 RID: 1794
		void mailSent(string mail, string userTo);

		// Token: 0x06000703 RID: 1795
		void mailReceived(string mail, string userTo);
	}
}
