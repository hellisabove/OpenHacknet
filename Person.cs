using System;
using System.Collections.Generic;

namespace Hacknet
{
	// Token: 0x0200014C RID: 332
	public class Person
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000834 RID: 2100 RVA: 0x00089DC8 File Offset: 0x00087FC8
		public string FullName
		{
			get
			{
				return this.firstName + " " + this.lastName;
			}
		}

		// Token: 0x06000835 RID: 2101 RVA: 0x00089DF0 File Offset: 0x00087FF0
		public Person()
		{
		}

		// Token: 0x06000836 RID: 2102 RVA: 0x00089E14 File Offset: 0x00088014
		public Person(string fName, string lName, bool male, bool isHacker = false, string handle = null)
		{
			this.firstName = fName;
			this.lastName = lName;
			this.isMale = male;
			if (handle == null)
			{
				handle = UsernameGenerator.getName();
			}
			if (handle == null)
			{
				throw new InvalidOperationException();
			}
			this.handle = handle;
			this.isHacker = isHacker;
			this.birthplace = WorldLocationLoader.getRandomLocation();
			this.vehicles = new List<VehicleRegistration>();
			this.degrees = new List<Degree>();
			this.addRandomDegrees();
			this.addRandomVehicles();
			int num = 18;
			int num2 = 72;
			if (isHacker)
			{
				num2 = 45;
			}
			int num3 = num * 365 + (int)(Utils.random.NextDouble() * (double)(num2 - num) * 365.0);
			this.DateOfBirth = DateTime.Now - TimeSpan.FromDays((double)num3);
			this.medicalRecord = new MedicalRecord(this.birthplace, this.DateOfBirth);
			if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed && (isHacker || Utils.randm(1f) < 0.8f))
			{
				this.NeopalsAccount = NeopalsAccount.GenerateAccount(handle, Utils.flipCoin() && Utils.flipCoin() && isHacker && handle.ToLower() != "bit");
			}
		}

		// Token: 0x06000837 RID: 2103 RVA: 0x00089F8C File Offset: 0x0008818C
		public void addRandomDegrees()
		{
			double num = 0.6;
			if (this.isHacker)
			{
				num = 0.9;
			}
			while (Utils.random.NextDouble() < num)
			{
				if (this.isHacker)
				{
					this.degrees.Add(PeopleAssets.getRandomHackerDegree(this.birthplace));
				}
				else
				{
					this.degrees.Add(PeopleAssets.getRandomDegree(this.birthplace));
				}
				num *= num;
				if (this.isHacker)
				{
					num *= 0.36;
				}
			}
		}

		// Token: 0x06000838 RID: 2104 RVA: 0x0008A02C File Offset: 0x0008822C
		public void addRandomVehicles()
		{
			double num = 0.7;
			while (Utils.random.NextDouble() < num)
			{
				this.vehicles.Add(VehicleInfo.getRandomRegistration());
				num *= num;
				if (this.isHacker)
				{
					num *= num;
				}
			}
		}

		// Token: 0x06000839 RID: 2105 RVA: 0x0008A080 File Offset: 0x00088280
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.firstName,
				" ",
				this.lastName,
				"\n Gender: ",
				this.isMale ? "Male  " : "Female  ",
				"Born: ",
				this.birthplace.ToString(),
				"\n",
				this.getDegreeString(),
				" ",
				this.getVehicleRegString(),
				"\n",
				this.medicalRecord.ToString()
			});
		}

		// Token: 0x0600083A RID: 2106 RVA: 0x0008A12C File Offset: 0x0008832C
		private string getDegreeString()
		{
			string text = "Degrees:\n";
			for (int i = 0; i < this.degrees.Count; i++)
			{
				text = text + " -" + this.degrees[i].ToString() + "\n";
			}
			return text;
		}

		// Token: 0x0600083B RID: 2107 RVA: 0x0008A184 File Offset: 0x00088384
		private string getVehicleRegString()
		{
			string text = "Vehicle Registrations:\n";
			for (int i = 0; i < this.vehicles.Count; i++)
			{
				text = text + " -" + this.vehicles[i].ToString() + "\n";
			}
			return text;
		}

		// Token: 0x040009B7 RID: 2487
		public DateTime DateOfBirth = DateTime.Now;

		// Token: 0x040009B8 RID: 2488
		public bool isMale = true;

		// Token: 0x040009B9 RID: 2489
		public bool isHacker = false;

		// Token: 0x040009BA RID: 2490
		public string firstName;

		// Token: 0x040009BB RID: 2491
		public string lastName;

		// Token: 0x040009BC RID: 2492
		public string handle;

		// Token: 0x040009BD RID: 2493
		public List<Degree> degrees;

		// Token: 0x040009BE RID: 2494
		public List<VehicleRegistration> vehicles;

		// Token: 0x040009BF RID: 2495
		public WorldLocation birthplace;

		// Token: 0x040009C0 RID: 2496
		public MedicalRecord medicalRecord;

		// Token: 0x040009C1 RID: 2497
		public NeopalsAccount NeopalsAccount;
	}
}
