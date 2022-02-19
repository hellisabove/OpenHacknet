using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000149 RID: 329
	public static class People
	{
		// Token: 0x06000825 RID: 2085 RVA: 0x00089038 File Offset: 0x00087238
		public static void init()
		{
			string text = Utils.readEntireFile("Content/PersonData/MaleNames.txt").Replace("\r", "");
			People.maleNames = text.Split(Utils.newlineDelim);
			text = Utils.readEntireFile("Content/PersonData/FemaleNames.txt").Replace("\r", "");
			People.femaleNames = text.Split(Utils.newlineDelim);
			text = Utils.readEntireFile("Content/PersonData/Surnames.txt").Replace("\r", "");
			People.surnames = text.Split(Utils.newlineDelim);
			People.all = new List<Person>(200);
			int num = 0;
			DirectoryInfo directoryInfo = new DirectoryInfo("Content/People");
			FileInfo[] files = directoryInfo.GetFiles("*.xml");
			List<string> list = new List<string>();
			for (int i = 0; i < files.Length; i++)
			{
				list.Add("Content/People/" + Path.GetFileName(files[i].Name));
			}
			if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
			{
				directoryInfo = new DirectoryInfo("Content/DLC/People");
				files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					list.Add("Content/DLC/People/" + Path.GetFileName(files[i].Name));
				}
				People.PeopleWereGeneratedWithDLCAdditions = true;
			}
			for (int i = 0; i < list.Count; i++)
			{
				string text2 = list[i];
				text2 = LocalizedFileLoader.GetLocalizedFilepath(text2);
				Person person = People.loadPersonFromFile(text2);
				if (person != null)
				{
					People.all.Add(person);
					num++;
				}
				else
				{
					Console.WriteLine("Person Load Error: " + list[i]);
				}
			}
			for (int i = num; i < 200; i++)
			{
				bool flag = Utils.flipCoin();
				string fName = flag ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)];
				string lName = People.surnames[Utils.random.Next(People.surnames.Length)];
				People.all.Add(new Person(fName, lName, flag, false, UsernameGenerator.getName()));
			}
			People.hackers = new List<Person>();
			People.generatePeopleForList(People.hackers, 10, true);
			People.hubAgents = new List<Person>();
			People.generatePeopleForList(People.hubAgents, 22, true);
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x000892D8 File Offset: 0x000874D8
		public static void LoadInDLCPeople()
		{
			if (Settings.EnableDLC && DLC1SessionUpgrader.HasDLC1Installed)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo("Content/DLC/People");
				FileInfo[] files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string text = "Content/DLC/People/" + Path.GetFileName(files[i].Name);
					text = LocalizedFileLoader.GetLocalizedFilepath(text);
					Person person = People.loadPersonFromFile(text);
					if (person != null)
					{
						People.all.Insert(0, person);
					}
				}
			}
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x00089370 File Offset: 0x00087570
		public static void ReInitPeopleForExtension()
		{
			People.all.Clear();
			People.hackers.Clear();
			People.hubAgents.Clear();
			int num = 0;
			string text = Path.Combine(Utils.GetFileLoadPrefix(), "People");
			if (Directory.Exists(text))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(text);
				FileInfo[] files = directoryInfo.GetFiles("*.xml");
				for (int i = 0; i < files.Length; i++)
				{
					string text2 = text + "/" + Path.GetFileName(files[i].Name);
					text2 = LocalizedFileLoader.GetLocalizedFilepath(text2);
					Person person = People.loadPersonFromFile(text2);
					if (person != null)
					{
						People.all.Insert(0, person);
						num++;
					}
				}
			}
			for (int i = num; i < 200; i++)
			{
				bool flag = Utils.flipCoin();
				string fName = flag ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)];
				string lName = People.surnames[Utils.random.Next(People.surnames.Length)];
				People.all.Add(new Person(fName, lName, flag, false, UsernameGenerator.getName()));
			}
			People.hackers = new List<Person>();
			People.generatePeopleForList(People.hackers, 10, true);
			People.hubAgents = new List<Person>();
			People.generatePeopleForList(People.hubAgents, 22, true);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x00089500 File Offset: 0x00087700
		private static void generatePeopleForList(List<Person> list, int numberToGenerate, bool areHackers = false)
		{
			for (int i = 0; i < numberToGenerate; i++)
			{
				bool flag = Utils.flipCoin();
				if (areHackers)
				{
					flag = (!flag || !Utils.flipCoin());
				}
				string fName = flag ? People.maleNames[Utils.random.Next(People.maleNames.Length)] : People.femaleNames[Utils.random.Next(People.femaleNames.Length)];
				string lName = People.surnames[Utils.random.Next(People.surnames.Length)];
				Person item = new Person(fName, lName, flag, areHackers, UsernameGenerator.getName());
				list.Add(item);
				People.all.Add(item);
			}
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x000895BC File Offset: 0x000877BC
		public static Person loadPersonFromFile(string path)
		{
			Person result;
			try
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Open))
				{
					XmlReader xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings());
					while (xmlReader.Name != "Person")
					{
						xmlReader.Read();
					}
					string lName;
					string handle;
					string fName = handle = (lName = "unknown");
					bool male = true;
					bool isHacker = false;
					bool flag = false;
					if (xmlReader.MoveToAttribute("id"))
					{
						string text = xmlReader.ReadContentAsString();
					}
					if (xmlReader.MoveToAttribute("handle"))
					{
						handle = xmlReader.ReadContentAsString();
					}
					if (xmlReader.MoveToAttribute("firstName"))
					{
						fName = xmlReader.ReadContentAsString();
					}
					if (xmlReader.MoveToAttribute("lastName"))
					{
						lName = xmlReader.ReadContentAsString();
					}
					if (xmlReader.MoveToAttribute("isMale"))
					{
						male = xmlReader.ReadContentAsBoolean();
					}
					if (xmlReader.MoveToAttribute("isHacker"))
					{
						isHacker = xmlReader.ReadContentAsBoolean();
					}
					if (xmlReader.MoveToAttribute("forceHasNeopals"))
					{
						flag = xmlReader.ReadContentAsBoolean();
					}
					Person person = new Person(fName, lName, male, isHacker, handle);
					if (person.NeopalsAccount == null && flag && DLC1SessionUpgrader.HasDLC1Installed)
					{
						person.NeopalsAccount = NeopalsAccount.GenerateAccount(person.handle, Utils.flipCoin());
					}
					xmlReader.Read();
					while (!(xmlReader.Name == "Person") || xmlReader.IsStartElement())
					{
						string name = xmlReader.Name;
						if (name != null)
						{
							if (!(name == "Degrees"))
							{
								if (!(name == "Birthplace"))
								{
									if (!(name == "DOB"))
									{
										if (name == "Medical")
										{
											person.medicalRecord = MedicalRecord.Load(xmlReader, person.birthplace, person.DateOfBirth);
										}
									}
									else
									{
										CultureInfo cultureInfo = new CultureInfo("en-au");
										xmlReader.MoveToContent();
										string input = xmlReader.ReadElementContentAsString();
										DateTime dateTime = Utils.SafeParseDateTime(input);
										if (dateTime.Hour == 0 && dateTime.Second == 0)
										{
											TimeSpan t = TimeSpan.FromHours(Utils.random.NextDouble() * 23.99);
											dateTime += t;
										}
										person.DateOfBirth = dateTime;
									}
								}
								else
								{
									string text2 = null;
									if (xmlReader.MoveToAttribute("name"))
									{
										text2 = xmlReader.ReadContentAsString();
									}
									if (text2 == null)
									{
										text2 = WorldLocationLoader.getRandomLocation().name;
									}
									person.birthplace = WorldLocationLoader.getClosestOrCreate(text2);
								}
							}
							else
							{
								List<Degree> list = new List<Degree>();
								xmlReader.Read();
								while (!(xmlReader.Name == "Degrees") || xmlReader.IsStartElement())
								{
									if (xmlReader.Name == "Degree")
									{
										string uniName = "UNKNOWN";
										double num = 3.0;
										if (xmlReader.MoveToAttribute("uni"))
										{
											uniName = xmlReader.ReadContentAsString();
										}
										if (xmlReader.MoveToAttribute("gpa"))
										{
											num = xmlReader.ReadContentAsDouble();
										}
										xmlReader.MoveToContent();
										string degreeName = xmlReader.ReadElementContentAsString();
										Degree item = new Degree(degreeName, uniName, (float)num);
										list.Add(item);
									}
									xmlReader.Read();
								}
								if (list.Count > 0)
								{
									person.degrees = list;
								}
							}
						}
						IL_38D:
						xmlReader.Read();
						continue;
						goto IL_38D;
					}
					if (DLC1SessionUpgrader.HasDLC1Installed)
					{
						if (person.handle == "Minx" && person.NeopalsAccount == null)
						{
							person.NeopalsAccount = NeopalsAccount.GenerateAccount("Minx", false);
						}
						if (person.handle == "Orann" && person.NeopalsAccount == null && DLC1SessionUpgrader.HasDLC1Installed && People.PeopleWereGeneratedWithDLCAdditions)
						{
							person.NeopalsAccount = NeopalsAccount.GenerateAccount("Orann", false);
						}
					}
					result = person;
				}
			}
			catch (FileNotFoundException)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x00089A74 File Offset: 0x00087C74
		public static void printAllPeople()
		{
			for (int i = 0; i < People.all.Count; i++)
			{
				Console.WriteLine("------------------------------------------\n" + People.all[i].ToString());
			}
		}

		// Token: 0x040009A7 RID: 2471
		private const int NUMBER_OF_PEOPLE = 200;

		// Token: 0x040009A8 RID: 2472
		private const int NUMBER_OF_HACKERS = 10;

		// Token: 0x040009A9 RID: 2473
		private const int NUMBER_OF_HUB_AGENTS = 22;

		// Token: 0x040009AA RID: 2474
		public static List<Person> all;

		// Token: 0x040009AB RID: 2475
		public static List<Person> hackers;

		// Token: 0x040009AC RID: 2476
		public static List<Person> hubAgents;

		// Token: 0x040009AD RID: 2477
		public static string[] maleNames;

		// Token: 0x040009AE RID: 2478
		public static string[] femaleNames;

		// Token: 0x040009AF RID: 2479
		public static string[] surnames;

		// Token: 0x040009B0 RID: 2480
		public static bool PeopleWereGeneratedWithDLCAdditions = false;
	}
}
