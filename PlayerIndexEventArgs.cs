using System;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000159 RID: 345
	internal class PlayerIndexEventArgs : EventArgs
	{
		// Token: 0x060008AA RID: 2218 RVA: 0x00091ACB File Offset: 0x0008FCCB
		public PlayerIndexEventArgs(PlayerIndex playerIndex)
		{
			this.playerIndex = playerIndex;
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060008AB RID: 2219 RVA: 0x00091AE0 File Offset: 0x0008FCE0
		public PlayerIndex PlayerIndex
		{
			get
			{
				return this.playerIndex;
			}
		}

		// Token: 0x04000A12 RID: 2578
		private PlayerIndex playerIndex;
	}
}
