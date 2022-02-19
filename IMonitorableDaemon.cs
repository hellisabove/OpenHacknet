using System;

namespace Hacknet
{
	// Token: 0x0200000A RID: 10
	internal interface IMonitorableDaemon
	{
		// Token: 0x06000043 RID: 67
		void SubscribeToAlertActionFroNewMessage(Action<string, string> act);

		// Token: 0x06000044 RID: 68
		void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act);

		// Token: 0x06000045 RID: 69
		bool ShouldDisplayNotifications();

		// Token: 0x06000046 RID: 70
		string GetName();

		// Token: 0x06000047 RID: 71
		void navigatedTo();
	}
}
