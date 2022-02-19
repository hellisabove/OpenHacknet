using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Daemons.Helpers
{
	// Token: 0x0200009B RID: 155
	public interface IMedicalMonitor
	{
		// Token: 0x06000326 RID: 806
		void HeartBeat(float beatTime);

		// Token: 0x06000327 RID: 807
		void Update(float dt);

		// Token: 0x06000328 RID: 808
		void Draw(Rectangle bounds, SpriteBatch sb, Color c, float timeRollback);
	}
}
