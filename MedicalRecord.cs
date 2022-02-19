using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Hacknet
{
	// Token: 0x020000E1 RID: 225
	public class MedicalRecord
	{
		// Token: 0x0600048B RID: 1163 RVA: 0x0004914C File Offset: 0x0004734C
		public MedicalRecord()
		{
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x000491A4 File Offset: 0x000473A4
		public MedicalRecord(WorldLocation location, DateTime dob)
		{
			this.DateofBirth = dob;
			int days = (DateTime.Now - dob).Days;
			int num = 155;
			int num2 = 220;
			this.Height = num + (int)(Utils.random.NextDouble() * Utils.random.NextDouble() * (double)(num2 - num));
			this.AddRandomVists(days, location);
			char c = Utils.flipCoin() ? 'A' : (Utils.flipCoin() ? 'B' : 'O');
			char c2 = Utils.flipCoin() ? 'A' : (Utils.flipCoin() ? 'B' : 'O');
			this.BloodType = c + c2;
			if (MedicalRecord.Allergants == null)
			{
				this.LoadStatics();
			}
			this.AddAllergies();
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x000492BE File Offset: 0x000474BE
		private void LoadStatics()
		{
			MedicalRecord.Allergants = new List<string>();
			MedicalRecord.Allergants.AddRange(Utils.readEntireFile("Content/PersonData/Allergies.txt").Split(Utils.robustNewlineDelim, StringSplitOptions.RemoveEmptyEntries));
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x000492EC File Offset: 0x000474EC
		private void AddAllergies()
		{
			int num = 4;
			int num2 = Math.Max(0, Utils.random.Next(0, num + 2) - 2);
			for (int i = 0; i < num2; i++)
			{
				string item;
				do
				{
					item = MedicalRecord.Allergants[Utils.random.Next(0, MedicalRecord.Allergants.Count - 1)];
				}
				while (this.Allergies.Contains(item));
				this.Allergies.Add(item);
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x0004936C File Offset: 0x0004756C
		private void AddRandomVists(int daysOld, WorldLocation loc)
		{
			int num = (int)(Utils.random.NextDouble() * Utils.random.NextDouble() * 10.0);
			int maxValue = daysOld / 2;
			int num2 = daysOld;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				int num4 = Utils.random.Next(1, maxValue);
				string item = string.Concat(new string[]
				{
					(this.DateofBirth + TimeSpan.FromDays((double)(num3 + num4) - Utils.random.NextDouble())).ToString(),
					" - ",
					(Utils.random.NextDouble() < 0.7) ? loc.name : loc.country,
					Utils.flipCoin() ? " Public" : " Private",
					" Hospital"
				});
				this.Visits.Add(item);
				num2 -= num4;
				num3 += num4;
				maxValue = (int)((double)num2 * 0.7);
			}
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x0004948C File Offset: 0x0004768C
		public override string ToString()
		{
			CultureInfo cultureInfo = new CultureInfo("en-au");
			string text = "Medical Record\n";
			text = text + "Date of Birth :: " + Utils.SafeWriteDateTime(this.DateofBirth);
			TimeSpan timeSpan = DateTime.Now - this.DateofBirth;
			text = text + "\nBlood Type :: " + this.BloodType;
			double num = 0.032808399 * (double)this.Height;
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				"\nHeight :: ",
				this.Height,
				"cm (",
				(int)num,
				"\"",
				(int)(num % 1.0 * 12.0),
				"')"
			});
			text = text + "\nAllergies :: " + this.GetCSVFromList(this.Allergies);
			text += "\nActive Prescriptions :: ";
			if (this.Perscriptions.Count == 0)
			{
				text += "NONE";
			}
			else
			{
				obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"x",
					this.Perscriptions.Count,
					" Active"
				});
				for (int i = 0; i < this.Perscriptions.Count; i++)
				{
					text = text + "\n" + this.Perscriptions[i];
				}
			}
			text += "\nRecorded Visits ::";
			if (this.Visits.Count == 0)
			{
				text += "NONE RECORDED\n";
			}
			else
			{
				for (int i = 0; i < this.Visits.Count; i++)
				{
					text = text + this.Visits[i] + "\n";
				}
			}
			return text + "Notes :: " + this.Notes + "\n";
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x000496BC File Offset: 0x000478BC
		public static MedicalRecord Load(XmlReader rdr, WorldLocation location, DateTime dob)
		{
			MedicalRecord medicalRecord = new MedicalRecord(location, dob);
			while (rdr.Name != "Medical")
			{
				rdr.Read();
			}
			rdr.Read();
			while (!(rdr.Name == "Medical") || rdr.IsStartElement())
			{
				if (rdr.IsStartElement())
				{
					string name = rdr.Name;
					string text = name;
					if (text != null)
					{
						if (!(text == "Blood"))
						{
							if (!(text == "Height"))
							{
								if (!(text == "Allergies"))
								{
									if (!(text == "Perscription"))
									{
										if (text == "Notes")
										{
											medicalRecord.Notes = rdr.ReadElementContentAsString();
										}
									}
									else
									{
										medicalRecord.Perscriptions.Add(rdr.ReadElementContentAsString());
									}
								}
								else
								{
									medicalRecord.Allergies.Clear();
									medicalRecord.Allergies.AddRange(rdr.ReadElementContentAsString().Split(new char[]
									{
										','
									}, StringSplitOptions.RemoveEmptyEntries));
								}
							}
							else
							{
								medicalRecord.Height = rdr.ReadElementContentAsInt();
							}
						}
						else
						{
							medicalRecord.BloodType = rdr.ReadElementContentAsString();
						}
					}
				}
				rdr.Read();
			}
			return medicalRecord;
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x0004980C File Offset: 0x00047A0C
		private string GetCSVFromList(List<string> list)
		{
			string result;
			if (list.Count <= 0)
			{
				result = "NONE";
			}
			else
			{
				string text = "";
				for (int i = 0; i < list.Count; i++)
				{
					text = text + list[i] + ",";
				}
				result = text.Substring(0, text.Length - 1);
			}
			return result;
		}

		// Token: 0x0400057B RID: 1403
		public List<string> Visits = new List<string>();

		// Token: 0x0400057C RID: 1404
		public List<string> Perscriptions = new List<string>();

		// Token: 0x0400057D RID: 1405
		public List<string> Allergies = new List<string>();

		// Token: 0x0400057E RID: 1406
		public int Height = 172;

		// Token: 0x0400057F RID: 1407
		public string Notes = "N/A";

		// Token: 0x04000580 RID: 1408
		public string BloodType = "AB";

		// Token: 0x04000581 RID: 1409
		public DateTime DateofBirth;

		// Token: 0x04000582 RID: 1410
		private static List<string> Allergants = null;
	}
}
