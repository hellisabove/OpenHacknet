using System;

namespace Hacknet
{
	// Token: 0x0200014A RID: 330
	public class Degree
	{
		// Token: 0x0600082C RID: 2092 RVA: 0x00089AC5 File Offset: 0x00087CC5
		public Degree()
		{
		}

		// Token: 0x0600082D RID: 2093 RVA: 0x00089AD0 File Offset: 0x00087CD0
		public Degree(string degreeName, string uniName, float degreeGPA)
		{
			this.name = degreeName;
			this.uni = uniName;
			this.GPA = degreeGPA;
		}

		// Token: 0x0600082E RID: 2094 RVA: 0x00089AF0 File Offset: 0x00087CF0
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.name,
				" from ",
				this.uni,
				". GPA: ",
				this.GPA
			});
		}

		// Token: 0x040009B1 RID: 2481
		public string name;

		// Token: 0x040009B2 RID: 2482
		public string uni;

		// Token: 0x040009B3 RID: 2483
		public float GPA;
	}
}
