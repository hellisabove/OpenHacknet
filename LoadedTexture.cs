using System;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000182 RID: 386
	public struct LoadedTexture
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000999 RID: 2457 RVA: 0x0009D580 File Offset: 0x0009B780
		// (set) Token: 0x0600099A RID: 2458 RVA: 0x0009D597 File Offset: 0x0009B797
		public string path { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600099B RID: 2459 RVA: 0x0009D5A0 File Offset: 0x0009B7A0
		// (set) Token: 0x0600099C RID: 2460 RVA: 0x0009D5B7 File Offset: 0x0009B7B7
		public Texture2D tex { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600099D RID: 2461 RVA: 0x0009D5C0 File Offset: 0x0009B7C0
		// (set) Token: 0x0600099E RID: 2462 RVA: 0x0009D5D7 File Offset: 0x0009B7D7
		public int retainCount { get; set; }
	}
}
