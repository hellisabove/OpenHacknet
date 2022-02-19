using System;

namespace Hacknet
{
	// Token: 0x0200008B RID: 139
	public class Neopal
	{
		// Token: 0x060002BE RID: 702 RVA: 0x00028414 File Offset: 0x00026614
		private static string GenerateName()
		{
			if (Neopal.PossibleNames == null)
			{
				string text = Utils.readEntireFile("Content/DLC/Docs/Untranslated/NeopalNames.txt");
				Neopal.PossibleNames = text.Split(new string[]
				{
					"\r\n",
					"\n",
					", ",
					" ,",
					","
				}, StringSplitOptions.RemoveEmptyEntries);
			}
			return Neopal.PossibleNames[Utils.random.Next(Neopal.PossibleNames.Length)];
		}

		// Token: 0x060002BF RID: 703 RVA: 0x00028498 File Offset: 0x00026698
		public static Neopal GeneratePet(bool isActiveUser = false)
		{
			isActiveUser = (isActiveUser || Utils.randm(1f) < 0.02f);
			Array values = Enum.GetValues(typeof(Neopal.PetType));
			Neopal.PetType type = (Neopal.PetType)values.GetValue(Utils.random.Next(values.Length));
			return new Neopal
			{
				CombatRating = (byte)Utils.random.Next(255),
				DaysSinceFed = (isActiveUser ? Utils.random.Next(2) : Utils.random.Next(3650)),
				Happiness = (isActiveUser ? (1f - Utils.randm(0.15f)) : Utils.randm(0.05f)),
				Name = Neopal.GenerateName(),
				Type = type,
				Identifier = Guid.NewGuid().ToString().Substring(0, 13)
			};
		}

		// Token: 0x040002EF RID: 751
		public Neopal.PetType Type;

		// Token: 0x040002F0 RID: 752
		public string Name;

		// Token: 0x040002F1 RID: 753
		public int DaysSinceFed;

		// Token: 0x040002F2 RID: 754
		public byte CombatRating;

		// Token: 0x040002F3 RID: 755
		public float Happiness;

		// Token: 0x040002F4 RID: 756
		public string Identifier;

		// Token: 0x040002F5 RID: 757
		private static string[] PossibleNames;

		// Token: 0x0200008C RID: 140
		public enum PetType
		{
			// Token: 0x040002F7 RID: 759
			Blundo,
			// Token: 0x040002F8 RID: 760
			Chisha,
			// Token: 0x040002F9 RID: 761
			Jubdub,
			// Token: 0x040002FA RID: 762
			Kachici,
			// Token: 0x040002FB RID: 763
			Kyrill,
			// Token: 0x040002FC RID: 764
			Myncl,
			// Token: 0x040002FD RID: 765
			Pageri,
			// Token: 0x040002FE RID: 766
			Psybunny,
			// Token: 0x040002FF RID: 767
			Scorchum,
			// Token: 0x04000300 RID: 768
			Unisam
		}
	}
}
