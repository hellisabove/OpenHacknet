using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000063 RID: 99
	public interface MainDisplayOverrideEXE
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x060001EC RID: 492
		// (set) Token: 0x060001ED RID: 493
		bool DisplayOverrideIsActive { get; set; }

		// Token: 0x060001EE RID: 494
		void RenderMainDisplay(Rectangle dest, SpriteBatch sb);
	}
}
