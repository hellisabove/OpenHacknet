using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x0200008A RID: 138
	public class NeopalsAccount
	{
		// Token: 0x060002BB RID: 699 RVA: 0x000282E0 File Offset: 0x000264E0
		public static NeopalsAccount GenerateAccount(string handle, bool isActiveUser = false)
		{
			NeopalsAccount neopalsAccount = new NeopalsAccount
			{
				AccountName = handle,
				NeoPoints = (long)(isActiveUser ? Utils.random.Next(50000) : Utils.random.Next(5000)),
				BankedPoints = (long)(isActiveUser ? Utils.random.Next(10000) : Utils.random.Next(2000)),
				InventoryID = Guid.NewGuid().ToString(),
				Pets = new List<Neopal>()
			};
			int num = 1 + Utils.random.Next(2);
			if (num == 2 && Utils.flipCoin() && Utils.flipCoin())
			{
				num++;
			}
			for (int i = 0; i < num; i++)
			{
				neopalsAccount.Pets.Add(Neopal.GeneratePet(isActiveUser));
			}
			return neopalsAccount;
		}

		// Token: 0x060002BC RID: 700 RVA: 0x000283D4 File Offset: 0x000265D4
		public override string ToString()
		{
			return this.AccountName + "_x" + this.Pets.Count;
		}

		// Token: 0x040002EA RID: 746
		public string AccountName;

		// Token: 0x040002EB RID: 747
		public long NeoPoints;

		// Token: 0x040002EC RID: 748
		public long BankedPoints;

		// Token: 0x040002ED RID: 749
		public string InventoryID;

		// Token: 0x040002EE RID: 750
		public List<Neopal> Pets;
	}
}
