using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000164 RID: 356
	internal class EOSComp
	{
		// Token: 0x060008FB RID: 2299 RVA: 0x00094F60 File Offset: 0x00093160
		public static void AddEOSComp(XmlReader rdr, Computer compAttatchedTo, object osObj)
		{
			OS os = (OS)osObj;
			string compName = "Unregistered eOS Device";
			string idName = compAttatchedTo.idName + "_eos";
			bool flag = false;
			if (rdr.MoveToAttribute("name"))
			{
				compName = ComputerLoader.filter(rdr.ReadContentAsString());
			}
			if (rdr.MoveToAttribute("id"))
			{
				idName = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("empty"))
			{
				flag = (rdr.ReadContentAsString().ToLower() == "true");
			}
			string adminPassword = "alpine";
			if (rdr.MoveToAttribute("passOverride"))
			{
				adminPassword = rdr.ReadContentAsString();
			}
			Computer device = new Computer(compName, NetworkMap.generateRandomIP(), os.netMap.getRandomPosition(), 0, 5, os);
			device.idName = idName;
			string icon = "ePhone";
			if (rdr.MoveToAttribute("icon"))
			{
				icon = rdr.ReadContentAsString();
			}
			device.icon = icon;
			device.location = compAttatchedTo.location + Corporation.getNearbyNodeOffset(compAttatchedTo.location, Utils.random.Next(12), 12, os.netMap, 0f, false);
			device.setAdminPassword(adminPassword);
			ComputerLoader.loadPortsIntoComputer("22,3659", device);
			device.portsNeededForCrack = 2;
			EOSComp.GenerateEOSFilesystem(device);
			rdr.Read();
			Folder folder = device.files.root.searchForFolder("eos");
			Folder folder2 = folder.searchForFolder("notes");
			Folder folder3 = folder.searchForFolder("mail");
			while (!(rdr.Name == "eosDevice") || rdr.IsStartElement())
			{
				if (rdr.Name.ToLower() == "note" && rdr.IsStartElement())
				{
					string text = null;
					if (rdr.MoveToAttribute("filename"))
					{
						text = ComputerLoader.filter(rdr.ReadContentAsString());
					}
					rdr.MoveToContent();
					string text2 = rdr.ReadElementContentAsString();
					text2 = ComputerLoader.filter(text2.TrimStart(new char[0]));
					if (text == null)
					{
						int num = text2.IndexOf("\n");
						if (num == -1)
						{
							num = text2.IndexOf("\n");
						}
						if (num == -1)
						{
							num = text2.Length;
						}
						text = text2.Substring(0, num);
						if (text.Length > 50)
						{
							text = text.Substring(0, 47) + "...";
						}
						text = text.Replace(" ", "_").Replace(":", "").ToLower().Trim() + ".txt";
					}
					FileEntry item = new FileEntry(text2, text);
					folder2.files.Add(item);
				}
				if (rdr.Name.ToLower() == "mail" && rdr.IsStartElement())
				{
					string text3 = null;
					string text4 = null;
					if (rdr.MoveToAttribute("username"))
					{
						text3 = ComputerLoader.filter(rdr.ReadContentAsString());
					}
					if (rdr.MoveToAttribute("pass"))
					{
						text4 = ComputerLoader.filter(rdr.ReadContentAsString());
					}
					string dataEntry = string.Concat(new string[]
					{
						"MAIL ACCOUNT : ",
						text3,
						"\nAccount   :",
						text3,
						"\nPassword :",
						text4,
						"\nLast Sync :",
						DateTime.Now.ToString(),
						"\n\n",
						Computer.generateBinaryString(512)
					});
					string nameEntry = text3 + ".act";
					folder3.files.Add(new FileEntry(dataEntry, nameEntry));
				}
				if (rdr.Name.ToLower() == "file" && rdr.IsStartElement())
				{
					string text = null;
					if (rdr.MoveToAttribute("name"))
					{
						text = rdr.ReadContentAsString();
					}
					string path = "home";
					if (rdr.MoveToAttribute("path"))
					{
						path = rdr.ReadContentAsString();
					}
					rdr.MoveToContent();
					string text2 = ComputerLoader.filter(rdr.ReadElementContentAsString());
					text2 = text2.TrimStart(new char[0]);
					Folder folderFromPath = device.getFolderFromPath(path, true);
					FileEntry item2 = new FileEntry(text2, text);
					folderFromPath.files.Add(item2);
				}
				rdr.Read();
				if (rdr.EOF)
				{
					IL_559:
					if (flag)
					{
						Folder folder4 = folder.searchForFolder("apps");
						if (folder4 != null)
						{
							folder4.files.Clear();
							folder4.folders.Clear();
						}
					}
					os.netMap.nodes.Add(device);
					ComputerLoader.postAllLoadedActions = (Action)Delegate.Combine(ComputerLoader.postAllLoadedActions, new Action(delegate()
					{
						device.links.Add(os.netMap.nodes.IndexOf(compAttatchedTo));
					}));
					if (compAttatchedTo.attatchedDeviceIDs != null)
					{
						Computer compAttatchedTo2 = compAttatchedTo;
						compAttatchedTo2.attatchedDeviceIDs += ",";
					}
					Computer compAttatchedTo3 = compAttatchedTo;
					compAttatchedTo3.attatchedDeviceIDs += device.idName;
					return;
				}
			}
			goto IL_559;
		}

		// Token: 0x060008FC RID: 2300 RVA: 0x0009559C File Offset: 0x0009379C
		public static Folder GenerateEOSFolder()
		{
			Folder folder = new Folder("eos");
			Folder folder2 = new Folder("apps");
			Folder folder3 = new Folder("system");
			Folder item = new Folder("notes");
			Folder item2 = new Folder("mail");
			folder.folders.Add(folder2);
			folder.folders.Add(item);
			folder.folders.Add(item2);
			folder.folders.Add(folder3);
			folder3.files.Add(new FileEntry(Computer.generateBinaryString(1024), "core.sys"));
			folder3.files.Add(new FileEntry(Computer.generateBinaryString(1024), "runtime.bin"));
			int num = 4 + Utils.random.Next(8);
			for (int i = 0; i < num; i++)
			{
				folder2.folders.Add(EOSAppGenerator.GetAppFolder());
			}
			return folder;
		}

		// Token: 0x060008FD RID: 2301 RVA: 0x00095698 File Offset: 0x00093898
		public static void GenerateEOSFilesystem(Computer device)
		{
			if (device.files.root.searchForFolder("eos") == null)
			{
				device.files.root.folders.Insert(0, EOSComp.GenerateEOSFolder());
			}
		}
	}
}
