using System;

namespace Hacknet
{
	// Token: 0x0200014D RID: 333
	public class WorldLocation
	{
		// Token: 0x0600083C RID: 2108 RVA: 0x0008A1DB File Offset: 0x000883DB
		public WorldLocation()
		{
		}

		// Token: 0x0600083D RID: 2109 RVA: 0x0008A1E6 File Offset: 0x000883E6
		public WorldLocation(string countryName, string locName, float education, float life, float employer, float affordability)
		{
			this.country = countryName;
			this.name = locName;
			this.educationLevel = education;
			this.lifeLevel = life;
			this.employerLevel = employer;
			this.affordabilityLevel = affordability;
		}

		// Token: 0x0600083E RID: 2110 RVA: 0x0008A220 File Offset: 0x00088420
		public new string ToString()
		{
			return this.name + ", " + this.country;
		}

		// Token: 0x040009C2 RID: 2498
		public string country;

		// Token: 0x040009C3 RID: 2499
		public string name;

		// Token: 0x040009C4 RID: 2500
		public float educationLevel;

		// Token: 0x040009C5 RID: 2501
		public float lifeLevel;

		// Token: 0x040009C6 RID: 2502
		public float employerLevel;

		// Token: 0x040009C7 RID: 2503
		public float affordabilityLevel;
	}
}
