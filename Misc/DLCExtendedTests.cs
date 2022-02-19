using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Hacknet.PlatformAPI.Storage;
using Microsoft.Xna.Framework;

namespace Hacknet.Misc
{
	// Token: 0x0200007C RID: 124
	public static class DLCExtendedTests
	{
		// Token: 0x06000268 RID: 616 RVA: 0x00022534 File Offset: 0x00020734
		public static string TesExtendedFunctionality(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "";
			int num = 0;
			int num2 = 0;
			text += DLCExtendedTests.TestConditionalActionSets(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestAdvancedConditionalActionSets(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestConditionalActionSetCollections(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestConditionalActionSetCollections2(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestConditionalActionSetCollectionsOnOS(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestObjectSerializer(screenMan, out num);
			num2 += num;
			text += DLCExtendedTests.TestDLCSessionUpgrader(screenMan, out num);
			num2 += num;
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				(num > 0) ? "\r\n" : " ",
				"Complete - ",
				num,
				" errors found"
			});
			errorsAdded = num2;
			return text;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x00022624 File Offset: 0x00020824
		public static string TestConditionalActionSets(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSet.xml"))
			{
				XmlReader xmlReader = XmlReader.Create(fileStream);
				SerializableConditionalActionSet serializableConditionalActionSet = SerializableConditionalActionSet.Deserialize(xmlReader);
				if (!Directory.Exists("Content/Tests/Output/"))
				{
					Directory.CreateDirectory("Content/Tests/Output");
				}
				File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput.txt", serializableConditionalActionSet.GetSaveString());
				using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput.txt"))
				{
					XmlReader rdr = XmlReader.Create(fileStream2);
					SerializableConditionalActionSet serializableConditionalActionSet2 = SerializableConditionalActionSet.Deserialize(rdr);
					if (serializableConditionalActionSet.Actions.Count != serializableConditionalActionSet2.Actions.Count)
					{
						num++;
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"\r\n\r\nConditional Action Sets are Broken! Expected 2 actions, got ",
							serializableConditionalActionSet.Actions.Count,
							" and ",
							serializableConditionalActionSet2.Actions.Count
						});
					}
				}
				xmlReader.Close();
			}
			GitCommitEntry gitCommitEntry = new GitCommitEntry();
			gitCommitEntry.EntryNumber = 1;
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026A RID: 618 RVA: 0x00022788 File Offset: 0x00020988
		public static string TestAdvancedConditionalActionSets(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestAdvancedConditionalActionSet.xml"))
			{
				XmlReader xmlReader = XmlReader.Create(fileStream);
				SerializableConditionalActionSet serializableConditionalActionSet = SerializableConditionalActionSet.Deserialize(xmlReader);
				if (serializableConditionalActionSet.Actions.Count != 3)
				{
					num++;
					text = text + "\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 3 actions, got " + serializableConditionalActionSet.Actions.Count;
				}
				if (!Directory.Exists("Content/Tests/Output/"))
				{
					Directory.CreateDirectory("Content/Tests/Output");
				}
				File.WriteAllText("Content/Tests/Output/TestAdvConditionalActionSetsOutput.txt", serializableConditionalActionSet.GetSaveString());
				using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestAdvConditionalActionSetsOutput.txt"))
				{
					XmlReader rdr = XmlReader.Create(fileStream2);
					SerializableConditionalActionSet serializableConditionalActionSet2 = SerializableConditionalActionSet.Deserialize(rdr);
					if (serializableConditionalActionSet.Actions.Count != serializableConditionalActionSet2.Actions.Count)
					{
						num++;
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 2 actions, got ",
							serializableConditionalActionSet.Actions.Count,
							" and ",
							serializableConditionalActionSet2.Actions.Count
						});
					}
				}
				xmlReader.Close();
			}
			using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestAdvancedConditionalActionSet2.xml"))
			{
				XmlReader xmlReader = XmlReader.Create(fileStream);
				SerializableConditionalActionSet serializableConditionalActionSet = SerializableConditionalActionSet.Deserialize(xmlReader);
				if (serializableConditionalActionSet.Actions.Count != 21)
				{
					num++;
					text = text + "\r\n\r\nAdvanced Conditional Action Sets are Broken! Expected 21 actions, got " + serializableConditionalActionSet.Actions.Count;
				}
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026B RID: 619 RVA: 0x000229B0 File Offset: 0x00020BB0
		public static string TestConditionalActionSetCollections(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSetCollection.xml"))
			{
				XmlReader xmlReader = XmlReader.Create(fileStream);
				RunnableConditionalActions runnableConditionalActions = RunnableConditionalActions.Deserialize(xmlReader);
				if (!Directory.Exists("Content/Tests/Output/"))
				{
					Directory.CreateDirectory("Content/Tests/Output");
				}
				File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput2.xml", runnableConditionalActions.GetSaveString());
				using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput2.xml"))
				{
					XmlReader rdr = XmlReader.Create(fileStream2);
					RunnableConditionalActions runnableConditionalActions2 = RunnableConditionalActions.Deserialize(rdr);
					if (runnableConditionalActions.Actions.Count != runnableConditionalActions2.Actions.Count || runnableConditionalActions.Actions.Count != 3)
					{
						num++;
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"\r\n\r\nConditional Action Set Collections are Broken! Expected 3 actions, got ",
							runnableConditionalActions.Actions.Count,
							" and ",
							runnableConditionalActions2.Actions.Count
						});
					}
					if (runnableConditionalActions.Actions[0].Actions.Count != 2 || runnableConditionalActions.Actions[1].Actions.Count != 0)
					{
						num++;
						text += "\nSave on OS COnditional actions failed! Incorrect action contents on original deserialization. ";
					}
					if (runnableConditionalActions2.Actions[0].Actions.Count != 2 || runnableConditionalActions2.Actions[1].Actions.Count != 0)
					{
						num++;
						text += "\nSave on OS COnditional actions failed! Incorrect action contents on realo.";
					}
				}
				xmlReader.Close();
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026C RID: 620 RVA: 0x00022BD0 File Offset: 0x00020DD0
		public static string TestConditionalActionSetCollections2(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			using (FileStream fileStream = File.OpenRead("Content/Tests/DLCTests/TestConditionalActionSetCollection2.xml"))
			{
				XmlReader xmlReader = XmlReader.Create(fileStream);
				RunnableConditionalActions runnableConditionalActions = RunnableConditionalActions.Deserialize(xmlReader);
				if (!Directory.Exists("Content/Tests/Output/"))
				{
					Directory.CreateDirectory("Content/Tests/Output");
				}
				File.WriteAllText("Content/Tests/Output/TestConditionalActionSetsOutput2.xml", runnableConditionalActions.GetSaveString());
				using (FileStream fileStream2 = File.OpenRead("Content/Tests/Output/TestConditionalActionSetsOutput2.xml"))
				{
					XmlReader rdr = XmlReader.Create(fileStream2);
					RunnableConditionalActions runnableConditionalActions2 = RunnableConditionalActions.Deserialize(rdr);
					if (runnableConditionalActions.Actions.Count != runnableConditionalActions2.Actions.Count || runnableConditionalActions.Actions.Count != 1)
					{
						num++;
						object obj = text;
						text = string.Concat(new object[]
						{
							obj,
							"\r\n\r\nConditional Action Set Collections are Broken! Expected 1 actions, got ",
							runnableConditionalActions.Actions.Count,
							" and ",
							runnableConditionalActions2.Actions.Count
						});
					}
					if (runnableConditionalActions.Actions[0].Actions.Count != 1)
					{
						num++;
						text += "\n\nSave on OS Conditional actions v2 failed! Incorrect action contents on original deserialization. \n\n";
					}
				}
				xmlReader.Close();
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026D RID: 621 RVA: 0x00022D80 File Offset: 0x00020F80
		public static string TestConditionalActionSetCollectionsOnOS(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			string text2 = "__hacknettestaccount";
			string pass = "__testingpassword";
			SaveFileManager.AddUser(text2, pass);
			string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(text2);
			OS.TestingPassOnly = true;
			OS os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text2;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			os.delayer.RunAllDelayedActions();
			RunnableConditionalActions conditionalActions = os.ConditionalActions;
			os.ConditionalActions.Actions.Add(new SerializableConditionalActionSet
			{
				Condition = new SCOnAdminGained(),
				Actions = new List<SerializableAction>()
			});
			os.ConditionalActions.Actions.Add(new SerializableConditionalActionSet
			{
				Condition = new SCOnAdminGained(),
				Actions = new List<SerializableAction>()
			});
			os.threadedSaveExecute(false);
			List<Computer> nodes = os.netMap.nodes;
			screenMan.RemoveScreen(os);
			OS.WillLoadSave = true;
			os = new OS();
			os.SaveGameUserName = saveFileNameForUsername;
			os.SaveUserAccountName = text2;
			screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
			os.delayer.RunAllDelayedActions();
			Game1.getSingleton().IsMouseVisible = true;
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			if (os.ConditionalActions.Actions.Count != conditionalActions.Actions.Count)
			{
				num++;
				text = text + "Save on OS COnditional actions failed! Expected 2, got " + os.ConditionalActions.Actions.Count;
			}
			screenMan.RemoveScreen(os);
			OS.TestingPassOnly = false;
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026E RID: 622 RVA: 0x00022F40 File Offset: 0x00021140
		public static string TestObjectSerializer(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			List<string> list = new List<string>();
			string s = ObjectSerializer.SerializeObject(list);
			List<string> list2 = (List<string>)ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(s), list.GetType());
			if (list2 == null || list2.Count != 0)
			{
				num++;
				text += "\nError deserializing empty list";
			}
			list.Add("test 1");
			list.Add("12345");
			list2 = (List<string>)ObjectSerializer.DeepCopy(list);
			if (list2.Count != 2 || list2[0] != "test 1" || list2[1] != "12345")
			{
				num++;
				text += "\nError deserializing empty list";
			}
			VehicleRegistration vehicleRegistration = new VehicleRegistration
			{
				licenceNumber = "1123-123",
				licencePlate = "11-11",
				vehicle = new VehicleType
				{
					maker = "asdf",
					model = "another asdf"
				}
			};
			VehicleRegistration to = (VehicleRegistration)ObjectSerializer.DeepCopy(vehicleRegistration);
			if (!Utils.PublicInstancePropertiesEqual<VehicleRegistration>(vehicleRegistration, to, new string[0]))
			{
				num++;
				text += "\nError auto deserializing vehicle info\n";
			}
			errorsAdded = num;
			return text;
		}

		// Token: 0x0600026F RID: 623 RVA: 0x000230A4 File Offset: 0x000212A4
		public static string TestDLCSessionUpgrader(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			string activeLocale = Settings.ActiveLocale;
			string text2 = "Content/Tests/DLCTests/save_preDLC.xml";
			OS.TestingPassOnly = true;
			OS os = new OS();
			os.SaveGameUserName = text2;
			os.SaveUserAccountName = "preDLCAccountTest";
			OS.WillLoadSave = true;
			try
			{
				using (FileStream fileStream = File.OpenRead(text2))
				{
					os.ForceLoadOverrideStream = fileStream;
					screenMan.AddScreen(os, new PlayerIndex?(screenMan.controllingPlayer));
				}
			}
			catch (Exception ex)
			{
				num++;
				text = text + "\r\nUnexpected error loading pre DLC save file for upgrade test!\r\n" + Utils.GenerateReportFromException(ex) + "\r\n";
			}
			if (os.isLoaded)
			{
				Computer computer = Programs.getComputer(os, "polarSnakeDest");
				if (os.thisComputer.Memory == null)
				{
					num++;
					text += "\r\nPlayer computer memory dump still null after upgrade!\r\n";
				}
				computer = Programs.getComputer(os, "polarSnakeDest");
				if (computer.Memory == null || computer.Memory.DataBlocks.Count != 1)
				{
					num++;
					text += "\r\nGibson Link memory dump did not get applied correctly!\r\n";
				}
				Computer computer2 = Programs.getComputer(os, "dPets_MF");
				Folder folder = computer2.files.root.searchForFolder("Database");
				bool flag = false;
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (folder.files[i].name.Contains("minx"))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					num++;
					text += "\r\nSession upgrade does not add Minx file to DPets Database!\r\n";
					text = text + "PeopleWereGeneratedWithDLCAdditions=" + People.PeopleWereGeneratedWithDLCAdditions;
				}
			}
			screenMan.RemoveScreen(os);
			OS.TestingPassOnly = false;
			errorsAdded = num;
			return text;
		}
	}
}
