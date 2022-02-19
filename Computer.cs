using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Hacknet.Security;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000101 RID: 257
	[Serializable]
	internal class Computer
	{
		// Token: 0x060005A0 RID: 1440 RVA: 0x000580B8 File Offset: 0x000562B8
		public Computer(string compName, string compIP, Vector2 compLocation, int seclevel, byte compType, OS opSystem)
		{
			this.name = compName;
			this.ip = compIP;
			this.location = compLocation;
			this.type = compType;
			this.files = this.generateRandomFileSystem();
			this.idName = compName.Replace(" ", "_");
			this.os = opSystem;
			this.traceTime = -1f;
			this.securityLevel = seclevel;
			this.adminIP = NetworkMap.generateRandomIP();
			this.users = new List<UserDetail>();
			this.adminPass = PortExploits.getRandomPassword();
			this.users.Add(new UserDetail("admin", this.adminPass, 1));
			this.ports = new List<int>(seclevel);
			this.portsOpen = new List<byte>(seclevel);
			this.openPortsForSecurityLevel(seclevel);
			this.links = new List<int>();
			this.daemons = new List<Daemon>();
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0005823C File Offset: 0x0005643C
		public void initDaemons()
		{
			for (int i = 0; i < this.daemons.Count; i++)
			{
				this.daemons[i].initFiles();
				if (this.daemons[i].isListed)
				{
					this.daemons[i].registerAsDefaultBootDaemon();
				}
			}
		}

		// Token: 0x060005A2 RID: 1442 RVA: 0x000582A4 File Offset: 0x000564A4
		public FileSystem generateRandomFileSystem()
		{
			FileSystem fileSystem = new FileSystem();
			if (this.type != 5 && this.type != 4)
			{
				int num = Utils.random.Next(6);
				for (int i = 0; i < num; i++)
				{
					int num2 = 0;
					string text;
					do
					{
						text = ((num2 > 10) ? "AA" : "") + this.generateFolderName(Utils.random.Next(100));
						num2++;
					}
					while (fileSystem.root.folders[0].searchForFolder(text) != null);
					Folder folder = new Folder(text);
					int num3 = Utils.random.Next(3);
					for (int j = 0; j < num3; j++)
					{
						if (Utils.random.NextDouble() > 0.8)
						{
							num2 = 0;
							string text2;
							do
							{
								text2 = this.generateFileName(Utils.random.Next(300));
								num2++;
								if (num2 > 3)
								{
									text2 = (int)(Utils.getRandomChar() + Utils.getRandomChar()) + text2;
								}
							}
							while (folder.searchForFile(text2) != null);
							folder.files.Add(new FileEntry(Utils.flipCoin() ? this.generateFileData(Utils.random.Next(500)) : Computer.generateBinaryString(500), text2));
						}
						else
						{
							FileEntry fileEntry = new FileEntry();
							string arg = fileEntry.name;
							while (folder.searchForFile(fileEntry.name) != null)
							{
								fileEntry.name = (int)(Utils.getRandomChar() + Utils.getRandomChar()) + arg;
							}
							folder.files.Add(fileEntry);
						}
					}
					fileSystem.root.folders[0].folders.Add(folder);
				}
			}
			else if (this.type == 5)
			{
				fileSystem.root.folders.Insert(0, EOSComp.GenerateEOSFolder());
			}
			return fileSystem;
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x00058500 File Offset: 0x00056700
		public void openPortsForSecurityLevel(int security)
		{
			this.portsNeededForCrack = security - 1;
			if (security >= 5)
			{
				this.portsNeededForCrack--;
				float num = 0f;
				for (int i = 4; i < security; i++)
				{
					num += Computer.BASE_PROXY_TICKS / (float)(i - 3);
				}
				this.addProxy(num);
			}
			switch (security)
			{
			default:
				this.openPorts(PortExploits.portNums.Count);
				break;
			case 1:
				this.openPorts(PortExploits.portNums.Count - 1);
				break;
			}
			if (security >= 4)
			{
				this.traceTime = (float)Math.Max(10 - security, 3) * Computer.BASE_TRACE_TIME;
			}
			if (security >= 5)
			{
				this.firewall = new Firewall(security - 5);
				this.admin = new BasicAdministrator();
			}
		}

		// Token: 0x060005A4 RID: 1444 RVA: 0x000585DC File Offset: 0x000567DC
		private void openPorts(int n)
		{
			int num = 4;
			for (int i = num - 1; i >= 0; i--)
			{
				this.ports.Add(PortExploits.portNums[i]);
				this.portsOpen.Add(0);
			}
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x00058628 File Offset: 0x00056828
		public void addProxy(float time)
		{
			if (time > 0f)
			{
				this.hasProxy = true;
				this.proxyActive = true;
				this.proxyOverloadTicks = time;
				this.startingOverloadTicks = this.proxyOverloadTicks;
			}
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x00058668 File Offset: 0x00056868
		public void addFirewall(int level)
		{
			this.firewall = new Firewall(level);
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00058677 File Offset: 0x00056877
		public void addFirewall(int level, string solution)
		{
			this.firewall = new Firewall(level, solution);
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00058687 File Offset: 0x00056887
		public void addFirewall(int level, string solution, float additionalTime)
		{
			this.firewall = new Firewall(level, solution, additionalTime);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00058698 File Offset: 0x00056898
		public void addMultiplayerTargetFile()
		{
			this.files.root.folders[0].files.Add(new FileEntry("#CRITICAL SYSTEM FILE - DO NOT MODIFY#\n\n" + Computer.generateBinaryString(2000), "system32.sys"));
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x000586E8 File Offset: 0x000568E8
		private void sendNetworkMessage(string s)
		{
			if (this.os.multiplayer && !this.silent)
			{
				this.os.sendMessage(s);
			}
			if (this.externalCounterpart != null)
			{
				this.externalCounterpart.writeMessage(s);
			}
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x0005873C File Offset: 0x0005693C
		private void tryExternalCounterpartDisconnect()
		{
			if (this.externalCounterpart != null)
			{
				this.externalCounterpart.disconnect();
			}
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x00058764 File Offset: 0x00056964
		public void hostileActionTaken()
		{
			if (this.os.connectedComp != null)
			{
				if (this.os.connectedComp.ip.Equals(this.ip))
				{
					if (this.traceTime > 0f)
					{
						this.os.traceTracker.start(this.traceTime);
					}
					if (this.os.timer - this.timeLastPinged > 0.35f)
					{
						SFX.addCircle(this.getScreenSpacePosition(), this.os.brightLockedColor, 25f);
						this.timeLastPinged = this.os.timer;
					}
				}
			}
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x0005882C File Offset: 0x00056A2C
		public void bootupTick(float t)
		{
			this.bootTimer -= t;
			if (this.bootTimer <= 0f)
			{
				this.disabled = false;
			}
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x00058864 File Offset: 0x00056A64
		public void log(string message)
		{
			if (!this.disabled)
			{
				if (this.reportingShell != null)
				{
					this.reportingShell.reportedTo(message);
				}
				message = string.Concat(new object[]
				{
					"@",
					(int)OS.currentElapsedTime,
					" ",
					message
				});
				string text = message;
				if (text.Length > 256)
				{
					text = text.Substring(0, 256);
				}
				string text2 = text.Replace(" ", "_");
				int num = 0;
				Folder folder = this.files.root.searchForFolder("log");
				bool flag;
				do
				{
					flag = false;
					for (int i = 0; i < folder.files.Count; i++)
					{
						if (folder.files[i] != null && folder.files[i].name == text2)
						{
							flag = true;
							num++;
							text2 = (text + "_" + num).Replace(" ", "_");
						}
					}
				}
				while (flag);
				text = text2;
				this.files.root.searchForFolder("log").files.Insert(0, new FileEntry(message, text));
			}
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000589E8 File Offset: 0x00056BE8
		public string generateFolderName(int seed)
		{
			return "NewFolder" + seed;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00058A0C File Offset: 0x00056C0C
		public string generateFileName(int seed)
		{
			return "Data" + seed;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00058A30 File Offset: 0x00056C30
		public string generateFileData(int seed)
		{
			string text = "";
			for (int i = 0; i < seed; i++)
			{
				text = text + " " + i;
			}
			return text;
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00058A70 File Offset: 0x00056C70
		public bool connect(string ipFrom)
		{
			bool result;
			if (this.disabled)
			{
				result = false;
			}
			else
			{
				WhitelistConnectionDaemon whitelistConnectionDaemon = (WhitelistConnectionDaemon)this.getDaemon(typeof(WhitelistConnectionDaemon));
				if (whitelistConnectionDaemon != null && ipFrom == this.os.thisComputer.ip)
				{
					if (!whitelistConnectionDaemon.IPCanPassWhitelist(ipFrom, false))
					{
						whitelistConnectionDaemon.DisconnectTarget();
						return false;
					}
				}
				this.log("Connection: from " + ipFrom);
				this.sendNetworkMessage("cConnection " + this.ip + " " + ipFrom);
				if (this.externalCounterpart != null)
				{
					this.externalCounterpart.establishConnection();
				}
				this.userLoggedIn = false;
				result = true;
			}
			return result;
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00058B3C File Offset: 0x00056D3C
		public void addNewUser(string ipFrom, string name, string pass, byte type)
		{
			this.addNewUser(ipFrom, new UserDetail(name, pass, type));
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x00058B50 File Offset: 0x00056D50
		public void addNewUser(string ipFrom, UserDetail usr)
		{
			this.users.Add(usr);
			if (!this.silent)
			{
				this.log("User Account Added: from " + ipFrom + " -Name: " + this.name);
			}
			this.sendNetworkMessage(string.Concat(new object[]
			{
				"cAddUser #",
				this.ip,
				"#",
				ipFrom,
				"#",
				this.name,
				"#",
				usr.pass,
				"#",
				usr.type
			}));
			for (int i = 0; i < this.daemons.Count; i++)
			{
				this.daemons[i].userAdded(usr.name, usr.pass, usr.type);
			}
		}

		// Token: 0x060005B5 RID: 1461 RVA: 0x00058C44 File Offset: 0x00056E44
		public void crash(string ipFrom)
		{
			if (this.os.connectedComp != null && this.os.connectedComp.Equals(this) && !this.os.connectedComp.Equals(this.os.thisComputer))
			{
				bool flag = this.os.connectedComp.silent;
				Computer connectedComp = this.os.connectedComp;
				connectedComp.silent = true;
				this.os.connectedComputerCrashed(this);
				connectedComp.silent = flag;
			}
			else if (this.os.thisComputer.Equals(this))
			{
				this.os.thisComputerCrashed();
			}
			if (!this.silent)
			{
				this.log("CRASH REPORT: Kernel Panic -- Fatal Trap");
			}
			this.disabled = true;
			this.bootTimer = Computer.BASE_BOOT_TIME;
			this.tryExternalCounterpartDisconnect();
			PostProcessor.dangerModeEnabled = false;
			this.sendNetworkMessage("cCrash " + this.ip + " " + ipFrom);
		}

		// Token: 0x060005B6 RID: 1462 RVA: 0x00058D4C File Offset: 0x00056F4C
		public void reboot(string ipFrom)
		{
			if (this.os.connectedComp != null && this.os.connectedComp.Equals(this) && !this.os.connectedComp.Equals(this.os.thisComputer))
			{
				this.os.connectedComputerCrashed(this);
			}
			else if (this.os.thisComputer.Equals(this) || this.os.thisComputer.Equals(this.os.connectedComp))
			{
				this.os.rebootThisComputer();
			}
			if (!this.silent)
			{
				this.log("Rebooting system : " + ipFrom);
			}
			this.disabled = true;
			this.bootTimer = Computer.BASE_REBOOT_TIME;
			this.tryExternalCounterpartDisconnect();
			this.sendNetworkMessage("cReboot " + this.ip + " " + ipFrom);
		}

		// Token: 0x060005B7 RID: 1463 RVA: 0x00058E4C File Offset: 0x0005704C
		public bool canReadFile(string ipFrom, FileEntry f, int index)
		{
			bool flag = false;
			if (ipFrom.Equals(this.adminIP))
			{
				flag = true;
			}
			if (ipFrom == this.os.thisComputer.ip && this.currentUser.name != null && this.currentUser.type == 0)
			{
				flag = true;
			}
			else
			{
				for (int i = 0; i < this.users.Count; i++)
				{
					if (ipFrom.Equals(this.users[i]))
					{
						flag = true;
					}
				}
			}
			bool result;
			if (!flag)
			{
				result = false;
			}
			else
			{
				if (f.name[0] != '@')
				{
					this.log("FileRead: by " + ipFrom + " - file:" + f.name);
					this.sendNetworkMessage(string.Concat(new object[]
					{
						"cFile ",
						this.ip,
						" ",
						ipFrom,
						" ",
						f.name,
						" ",
						index
					}));
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x00058F98 File Offset: 0x00057198
		public bool canCopyFile(string ipFrom, string name)
		{
			if (this.currentUser.type != 0)
			{
				if (!this.silent && !ipFrom.Equals(this.adminIP))
				{
					return false;
				}
			}
			this.log("FileCopied: by " + ipFrom + " - file:" + name);
			string s = string.Concat(new string[]
			{
				"cCopy ",
				this.ip,
				" ",
				ipFrom,
				" ",
				name
			});
			this.sendNetworkMessage(s);
			return true;
		}

		// Token: 0x060005B9 RID: 1465 RVA: 0x00059038 File Offset: 0x00057238
		public bool deleteFile(string ipFrom, string name, List<int> folderPath)
		{
			bool flag = false;
			if (this.currentUser.type == 1 || this.currentUser.type == 0)
			{
				flag = true;
			}
			bool result;
			if (!flag && !this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
			{
				result = false;
			}
			else if (name == "*")
			{
				Folder folder = this.files.root;
				if (folderPath.Count > 0)
				{
					folder = Programs.getFolderFromNavigationPath(folderPath, folder, this.os);
				}
				bool flag2 = true;
				List<string> list = new List<string>();
				for (int i = 0; i < folder.files.Count; i++)
				{
					if (folder.files[i] != null && !string.IsNullOrWhiteSpace(folder.files[i].name))
					{
						list.Add(folder.files[i].name);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					flag2 &= this.deleteFile(ipFrom, list[i], folderPath);
				}
				result = flag2;
			}
			else
			{
				if (name[0] != '@')
				{
					this.log("FileDeleted: by " + ipFrom + " - file:" + name);
				}
				Folder folder2 = this.files.root;
				if (folderPath.Count > 0)
				{
					folder2 = Programs.getFolderFromNavigationPath(folderPath, folder2, this.os);
				}
				string value = name;
				if (this.os.multiplayer && name[0] == '@')
				{
					value = name.Substring(name.IndexOf('_'));
				}
				for (int i = 0; i < folder2.files.Count; i++)
				{
					string text = folder2.files[i].name;
					bool flag3;
					if (this.os.multiplayer && name[0] == '@')
					{
						text = text.Substring(text.IndexOf('_'));
						flag3 = text.Equals(value);
					}
					else
					{
						flag3 = text.Equals(name);
					}
					if (flag3)
					{
						folder2.files.RemoveAt(i);
						i--;
					}
				}
				string text2 = string.Concat(new string[]
				{
					"cDelete #",
					this.ip,
					"#",
					ipFrom,
					"#",
					name
				});
				for (int i = 0; i < folderPath.Count; i++)
				{
					text2 = text2 + "#" + folderPath[i];
				}
				this.sendNetworkMessage(text2);
				result = true;
			}
			return result;
		}

		// Token: 0x060005BA RID: 1466 RVA: 0x00059370 File Offset: 0x00057570
		public bool moveFile(string ipFrom, string name, string newName, List<int> folderPath, List<int> destFolderPath)
		{
			if (this.currentUser.type != 0)
			{
				if (!this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
				{
					return false;
				}
			}
			Folder folder = this.files.root;
			folder = Programs.getFolderFromNavigationPath(folderPath, this.files.root, this.os);
			Folder folder2 = Programs.getFolderFromNavigationPath(destFolderPath, this.files.root, this.os);
			if (newName.StartsWith("/"))
			{
				if (destFolderPath.Count == 0 || (folderPath.Count > 0 && destFolderPath.Count == folderPath.Count && destFolderPath[0] == folderPath[0]))
				{
					folder2 = this.files.root;
					newName = newName.Substring(1);
					Folder folder3 = folder2.searchForFolder(newName);
					if (folder3 != null)
					{
						folder2 = folder3;
						newName = name;
					}
				}
				else
				{
					newName = newName.Substring(1);
				}
			}
			FileEntry fileEntry = null;
			for (int i = 0; i < folder.files.Count; i++)
			{
				if (folder.files[i].name == name)
				{
					fileEntry = folder.files[i];
					folder.files.RemoveAt(i);
					break;
				}
			}
			bool result;
			if (fileEntry == null)
			{
				this.os.write("File not Found");
				result = false;
			}
			else
			{
				if (newName == "" || newName == " ")
				{
					newName = name;
				}
				fileEntry.name = newName;
				string text = fileEntry.name;
				int num = 1;
				while (folder2.searchForFile(fileEntry.name) != null)
				{
					fileEntry.name = string.Concat(new object[]
					{
						text,
						"(",
						num,
						")"
					});
					num++;
				}
				folder2.files.Add(fileEntry);
				string text2 = string.Concat(new string[]
				{
					"cMove #",
					this.ip,
					"#",
					ipFrom,
					"#",
					name,
					"#",
					newName,
					"#"
				});
				for (int i = 0; i < folderPath.Count; i++)
				{
					text2 = text2 + "%" + folderPath[i];
				}
				text2 += "#";
				for (int i = 0; i < destFolderPath.Count; i++)
				{
					text2 = text2 + "%" + destFolderPath[i];
				}
				this.sendNetworkMessage(text2);
				this.log(string.Concat(new string[]
				{
					"FileMoved: by ",
					ipFrom,
					" - file:",
					name,
					" To: ",
					newName
				}));
				result = true;
			}
			return result;
		}

		// Token: 0x060005BB RID: 1467 RVA: 0x000596F8 File Offset: 0x000578F8
		public bool makeFile(string ipFrom, string name, string data, List<int> folderPath, bool isUpload = false)
		{
			bool result;
			if (!isUpload && !this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
			{
				result = false;
			}
			else
			{
				if (name[0] != '@')
				{
					this.log("FileCreated: by " + ipFrom + " - file:" + name);
				}
				Folder folder = this.files.root;
				if (folderPath.Count > 0)
				{
					for (int i = 0; i < folderPath.Count; i++)
					{
						if (folder.folders.Count > folderPath[i])
						{
							folder = folder.folders[folderPath[i]];
						}
					}
				}
				if (isUpload)
				{
					folder.files.Insert(0, new FileEntry(data, name));
				}
				else
				{
					folder.files.Add(new FileEntry(data, name));
				}
				string text = string.Concat(new string[]
				{
					"cMake #",
					this.ip,
					"#",
					ipFrom,
					"#",
					name,
					"#",
					data
				});
				for (int i = 0; i < folderPath.Count; i++)
				{
					text = text + "#" + folderPath[i];
				}
				this.sendNetworkMessage(text);
				result = true;
			}
			return result;
		}

		// Token: 0x060005BC RID: 1468 RVA: 0x0005989C File Offset: 0x00057A9C
		public bool makeFolder(string ipFrom, string name, List<int> folderPath)
		{
			bool result;
			if (!this.silent && !ipFrom.Equals(this.adminIP) && !ipFrom.Equals(this.ip))
			{
				result = false;
			}
			else
			{
				if (name[0] != '@')
				{
					this.log("FolderCreated: by " + ipFrom + " - folder:" + name);
				}
				Folder folder = this.files.root;
				if (folderPath.Count > 0)
				{
					for (int i = 0; i < folderPath.Count; i++)
					{
						if (folder.folders.Count > folderPath[i])
						{
							folder = folder.folders[folderPath[i]];
						}
					}
				}
				folder.folders.Add(new Folder(name));
				string text = string.Concat(new string[]
				{
					"cMkDir #",
					this.ip,
					"#",
					ipFrom,
					"#",
					name
				});
				for (int i = 0; i < folderPath.Count; i++)
				{
					text = text + "#" + folderPath[i];
				}
				this.sendNetworkMessage(text);
				result = true;
			}
			return result;
		}

		// Token: 0x060005BD RID: 1469 RVA: 0x00059A04 File Offset: 0x00057C04
		public void disconnecting(string ipFrom, bool externalDisconnectToo = true)
		{
			if (!this.silent)
			{
				this.log(ipFrom + " Disconnected");
			}
			if (this.os.multiplayer && !this.silent)
			{
				this.sendNetworkMessage("cDisconnect " + this.ip + " " + ipFrom);
			}
			if (externalDisconnectToo)
			{
				this.tryExternalCounterpartDisconnect();
			}
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00059A7C File Offset: 0x00057C7C
		public void giveAdmin(string ipFrom)
		{
			this.adminIP = ipFrom;
			this.log(ipFrom + " Became Admin");
			if (this.os.multiplayer && !this.silent)
			{
				this.sendNetworkMessage("cAdmin " + this.ip + " " + ipFrom);
			}
			UserDetail value = this.users[0];
			value.known = true;
			this.users[0] = value;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00059B00 File Offset: 0x00057D00
		public void openPort(int portNum, string ipFrom)
		{
			portNum = this.GetCodePortNumberFromDisplayPort(portNum);
			int num = -1;
			for (int i = 0; i < this.ports.Count; i++)
			{
				if (this.ports[i] == portNum)
				{
					num = i;
					break;
				}
			}
			if (num != -1)
			{
				this.portsOpen[num] = 1;
			}
			this.log(ipFrom + " Opened Port#" + portNum);
			if (!this.silent)
			{
				this.sendNetworkMessage(string.Concat(new object[]
				{
					"cPortOpen ",
					this.ip,
					" ",
					ipFrom,
					" ",
					portNum
				}));
			}
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x00059BCC File Offset: 0x00057DCC
		public void closePort(int portNum, string ipFrom)
		{
			portNum = this.GetCodePortNumberFromDisplayPort(portNum);
			int num = -1;
			for (int i = 0; i < this.ports.Count; i++)
			{
				if (this.ports[i] == portNum)
				{
					num = i;
				}
			}
			bool flag = false;
			if (num != -1)
			{
				flag = (this.portsOpen[num] != 0);
				this.portsOpen[num] = 0;
			}
			if (flag)
			{
				this.log(ipFrom + " Closed Port#" + portNum);
			}
			if (!this.silent)
			{
				this.sendNetworkMessage(string.Concat(new object[]
				{
					"cPortClose ",
					this.ip,
					" ",
					ipFrom,
					" ",
					portNum
				}));
			}
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00059CBC File Offset: 0x00057EBC
		public bool isPortOpen(int portNum)
		{
			for (int i = 0; i < this.ports.Count; i++)
			{
				if (this.ports[i] == portNum)
				{
					return this.portsOpen[i] > 0;
				}
			}
			return false;
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00059D14 File Offset: 0x00057F14
		public void openCDTray(string ipFrom)
		{
			if (this.os.thisComputer.ip.Equals(this.ip))
			{
				Programs.cdDrive(true);
			}
			this.sendNetworkMessage("cCDDrive " + this.ip + " open");
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x00059D6C File Offset: 0x00057F6C
		public void closeCDTray(string ipFrom)
		{
			if (this.os.thisComputer.ip.Equals(this.ip))
			{
				Programs.cdDrive(false);
			}
			this.sendNetworkMessage("cCDDrive " + this.ip + " close");
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00059DC4 File Offset: 0x00057FC4
		public void forkBombClients(string ipFrom)
		{
			this.sendNetworkMessage("cFBClients " + this.ip + " " + ipFrom);
			if (!this.os.multiplayer)
			{
				for (int i = 0; i < this.os.ActiveHackers.Count; i++)
				{
					if (this.os.ActiveHackers[i].Value == this.ip)
					{
						string key = this.os.ActiveHackers[i].Key;
						Computer computer = Programs.getComputer(this.os, key);
						computer.crash(this.ip);
					}
				}
			}
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00059E88 File Offset: 0x00058088
		public virtual int login(string username, string password, byte type = 1)
		{
			int result;
			if (username.ToLower().Equals("admin") && password.Equals(this.adminPass))
			{
				this.giveAdmin(this.os.thisComputer.ip);
				result = 1;
			}
			else
			{
				for (int i = 0; i < this.users.Count; i++)
				{
					if (this.users[i].name.Equals(username) && this.users[i].pass.Equals(password) && (this.users[i].type == type || type == 1))
					{
						this.currentUser = this.users[i];
						return 2;
					}
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00059F74 File Offset: 0x00058174
		public int GetDisplayPortNumberFromCodePort(int codePort)
		{
			int result;
			if (this.PortRemapping == null || !this.PortRemapping.ContainsKey(codePort))
			{
				result = codePort;
			}
			else
			{
				result = this.PortRemapping[codePort];
			}
			return result;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00059FB4 File Offset: 0x000581B4
		public int GetCodePortNumberFromDisplayPort(int displayPort)
		{
			int result;
			if (this.PortRemapping == null)
			{
				result = displayPort;
			}
			else
			{
				foreach (KeyValuePair<int, int> keyValuePair in this.PortRemapping)
				{
					if (keyValuePair.Value == displayPort)
					{
						return keyValuePair.Key;
					}
				}
				result = displayPort;
			}
			return result;
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x0005A040 File Offset: 0x00058240
		public void setAdminPassword(string newPass)
		{
			this.adminPass = newPass;
			for (int i = 0; i < this.users.Count; i++)
			{
				if (this.users[i].name.ToLower().Equals("admin"))
				{
					this.users[i] = new UserDetail("admin", newPass, 0);
				}
			}
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x0005A0B4 File Offset: 0x000582B4
		public string getSaveString()
		{
			string text = "none";
			if (this.os.netMap.mailServer.Equals(this))
			{
				text = "mail";
			}
			if (this.os.thisComputer.Equals(this))
			{
				text = "player";
			}
			string text2 = (this.attatchedDeviceIDs == null) ? "" : (" devices=\"" + this.attatchedDeviceIDs + "\"");
			string text3 = this.HasTracker ? " tracker=\"true\" " : "";
			string text4 = string.Concat(new object[]
			{
				"<computer name=\"",
				this.name,
				"\" ip=\"",
				this.ip,
				"\" type=\"",
				this.type,
				"\" spec=\"",
				text,
				"\" id=\"",
				this.idName,
				"\" ",
				(this.icon == null) ? "" : ("icon=\"" + this.icon + "\""),
				text2,
				text3,
				" >\n"
			});
			string text5 = text4;
			text4 = string.Concat(new string[]
			{
				text5,
				"<location x=\"",
				this.location.X.ToString(CultureInfo.InvariantCulture),
				"\" y=\"",
				this.location.Y.ToString(CultureInfo.InvariantCulture),
				"\" />"
			});
			object obj = text4;
			text4 = string.Concat(new object[]
			{
				obj,
				"<security level=\"",
				this.securityLevel,
				"\" traceTime=\"",
				this.traceTime.ToString(CultureInfo.InvariantCulture),
				(this.startingOverloadTicks > 0f) ? ("\" proxyTime=\"" + (this.hasProxy ? this.startingOverloadTicks.ToString(CultureInfo.InvariantCulture) : "-1")) : "",
				"\" portsToCrack=\"",
				this.portsNeededForCrack,
				"\" adminIP=\"",
				this.adminIP,
				"\" />"
			});
			text4 = text4 + "<admin type=\"" + ((this.admin == null) ? "none" : ((this.admin is FastBasicAdministrator) ? "fast" : ((this.admin is FastProgressOnlyAdministrator) ? "progress" : "basic")));
			text4 = text4 + "\" resetPass=\"" + ((this.admin != null && this.admin.ResetsPassword) ? "true" : "false") + "\"";
			text4 = text4 + " isSuper=\"" + ((this.admin != null && this.admin.IsSuper) ? "true" : "false") + "\" />";
			string text6 = "";
			for (int i = 0; i < this.links.Count; i++)
			{
				text6 = text6 + " " + this.links[i];
			}
			text4 = text4 + "<links>" + text6 + "</links>\n";
			if (this.firewall != null)
			{
				text4 = text4 + this.firewall.getSaveString() + "\n";
			}
			string text7 = "";
			for (int i = 0; i < this.portsOpen.Count; i++)
			{
				text7 = text7 + " " + this.ports[i];
			}
			text4 = text4 + "<portsOpen>" + text7 + "</portsOpen>\n";
			if (this.PortRemapping != null)
			{
				text4 += PortRemappingSerializer.GetSaveString(this.PortRemapping);
			}
			text4 += "<users>\n";
			for (int i = 0; i < this.users.Count; i++)
			{
				text4 = text4 + this.users[i].getSaveString() + "\n";
			}
			text4 += "</users>\n";
			if (this.Memory != null)
			{
				text4 += this.Memory.GetSaveString();
			}
			text4 += "<daemons>\n";
			for (int i = 0; i < this.daemons.Count; i++)
			{
				text4 = text4 + this.daemons[i].getSaveString() + "\n";
			}
			text4 += "</daemons>\n";
			text4 += this.files.getSaveString();
			return text4 + "</computer>\n";
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x0005A5E4 File Offset: 0x000587E4
		public static Computer load(XmlReader reader, OS os)
		{
			while (reader.Name != "computer")
			{
				reader.Read();
			}
			reader.MoveToAttribute("name");
			string compName = reader.ReadContentAsString();
			reader.MoveToAttribute("ip");
			string compIP = reader.ReadContentAsString();
			reader.MoveToAttribute("type");
			byte compType = (byte)reader.ReadContentAsInt();
			reader.MoveToAttribute("spec");
			string a = reader.ReadContentAsString();
			reader.MoveToAttribute("id");
			string text = reader.ReadContentAsString();
			string text2 = null;
			if (reader.MoveToAttribute("devices"))
			{
				text2 = reader.ReadContentAsString();
			}
			string text3 = null;
			if (reader.MoveToAttribute("icon"))
			{
				text3 = reader.ReadContentAsString();
			}
			bool hasTracker = false;
			if (reader.MoveToAttribute("tracker"))
			{
				hasTracker = (reader.ReadContentAsString().ToLower() == "true");
			}
			while (reader.Name != "location")
			{
				reader.Read();
			}
			reader.MoveToAttribute("x");
			float x = reader.ReadContentAsFloat();
			reader.MoveToAttribute("y");
			float y = reader.ReadContentAsFloat();
			while (reader.Name != "security")
			{
				reader.Read();
			}
			reader.MoveToAttribute("level");
			int seclevel = reader.ReadContentAsInt();
			reader.MoveToAttribute("traceTime");
			float num = reader.ReadContentAsFloat();
			reader.MoveToAttribute("portsToCrack");
			int num2 = reader.ReadContentAsInt();
			reader.MoveToAttribute("adminIP");
			string text4 = reader.ReadContentAsString();
			float num3 = -1f;
			if (reader.MoveToAttribute("proxyTime"))
			{
				num3 = reader.ReadContentAsFloat();
			}
			while (reader.Name != "admin")
			{
				reader.Read();
			}
			reader.MoveToAttribute("type");
			string text5 = reader.ReadContentAsString();
			reader.MoveToAttribute("resetPass");
			bool resetsPassword = reader.ReadContentAsBoolean();
			reader.MoveToAttribute("isSuper");
			bool isSuper = reader.ReadContentAsBoolean();
			Administrator administrator = null;
			string text6 = text5;
			if (text6 != null)
			{
				if (!(text6 == "fast"))
				{
					if (!(text6 == "basic"))
					{
						if (text6 == "progress")
						{
							administrator = new FastProgressOnlyAdministrator();
						}
					}
					else
					{
						administrator = new BasicAdministrator();
					}
				}
				else
				{
					administrator = new FastBasicAdministrator();
				}
			}
			if (administrator != null)
			{
				administrator.ResetsPassword = resetsPassword;
			}
			if (administrator != null)
			{
				administrator.IsSuper = isSuper;
			}
			while (reader.Name != "links")
			{
				reader.Read();
			}
			string text7 = reader.ReadElementContentAsString();
			string[] array = text7.Split(new char[0]);
			Firewall firewall = null;
			while (reader.Name != "portsOpen" && reader.Name != "firewall")
			{
				reader.Read();
			}
			if (reader.Name == "firewall")
			{
				firewall = Firewall.load(reader);
			}
			while (reader.Name != "portsOpen")
			{
				reader.Read();
			}
			string text8 = reader.ReadElementContentAsString();
			Computer computer = new Computer(compName, compIP, new Vector2(x, y), seclevel, compType, os);
			computer.firewall = firewall;
			computer.admin = administrator;
			computer.HasTracker = hasTracker;
			if (num3 > 0f)
			{
				computer.addProxy(num3);
			}
			else
			{
				computer.hasProxy = false;
				computer.proxyActive = false;
			}
			while (reader.Name != "users" && reader.Name != "portRemap")
			{
				reader.Read();
			}
			if (reader.Name == "portRemap")
			{
				reader.MoveToContent();
				string input = reader.ReadElementContentAsString();
				computer.PortRemapping = PortRemappingSerializer.Deserialize(input);
			}
			while (reader.Name != "users")
			{
				reader.Read();
			}
			computer.users.Clear();
			while (!(reader.Name == "users") || reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.Name == "user")
				{
					UserDetail item = UserDetail.loadUserDetail(reader);
					if (item.name.ToLower() == "admin")
					{
						computer.adminPass = item.pass;
					}
					computer.users.Add(item);
				}
				reader.Read();
			}
			while (reader.Name != "Memory" && reader.Name != "daemons")
			{
				reader.Read();
			}
			if (reader.Name == "Memory")
			{
				MemoryContents memory = MemoryContents.Deserialize(reader);
				computer.Memory = memory;
				reader.Read();
			}
			while (reader.Name != "daemons")
			{
				reader.Read();
			}
			reader.Read();
			while (!(reader.Name == "daemons") || reader.NodeType != XmlNodeType.EndElement)
			{
				if (reader.Name == "MailServer")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					MailServer mailServer = new MailServer(computer, serviceName, os);
					computer.daemons.Add(mailServer);
					if (reader.MoveToAttribute("color"))
					{
						Color themeColor = Utils.convertStringToColor(reader.ReadContentAsString());
						mailServer.setThemeColor(themeColor);
					}
				}
				else if (reader.Name == "MissionListingServer")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					reader.MoveToAttribute("group");
					string group = reader.ReadContentAsString();
					reader.MoveToAttribute("public");
					string text9 = reader.ReadContentAsString();
					bool isPublic = text9.ToLower().Equals("true");
					reader.MoveToAttribute("assign");
					string text10 = reader.ReadContentAsString();
					bool isAssigner = text10.ToLower().Equals("true");
					string text11 = null;
					if (reader.MoveToAttribute("title"))
					{
						text11 = reader.ReadContentAsString();
					}
					string text12 = null;
					string text13 = null;
					string articleFolderPath = null;
					if (reader.MoveToAttribute("icon"))
					{
						text12 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("articles"))
					{
						articleFolderPath = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("color"))
					{
						text13 = reader.ReadContentAsString();
					}
					MissionListingServer missionListingServer;
					if (text12 != null && text13 != null)
					{
						missionListingServer = new MissionListingServer(computer, serviceName, text12, articleFolderPath, Utils.convertStringToColor(text13), os, isPublic, isAssigner);
					}
					else
					{
						missionListingServer = new MissionListingServer(computer, serviceName, group, os, isPublic, isAssigner);
					}
					if (text11 != null)
					{
						missionListingServer.listingTitle = text11;
					}
					computer.daemons.Add(missionListingServer);
				}
				else if (reader.Name == "AddEmailServer")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					AddEmailDaemon item2 = new AddEmailDaemon(computer, serviceName, os);
					computer.daemons.Add(item2);
				}
				else if (reader.Name == "MessageBoard")
				{
					reader.MoveToAttribute("name");
					string text14 = reader.ReadContentAsString();
					MessageBoardDaemon messageBoardDaemon = new MessageBoardDaemon(computer, os);
					if (reader.MoveToAttribute("boardName"))
					{
						messageBoardDaemon.BoardName = reader.ReadContentAsString();
					}
					messageBoardDaemon.name = text14;
					computer.daemons.Add(messageBoardDaemon);
				}
				else if (reader.Name == "WebServer")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					reader.MoveToAttribute("url");
					string text15 = reader.ReadContentAsString();
					WebServerDaemon item3 = new WebServerDaemon(computer, serviceName, os, text15);
					computer.daemons.Add(item3);
				}
				else if (reader.Name == "OnlineWebServer")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					reader.MoveToAttribute("url");
					string text15 = reader.ReadContentAsString();
					OnlineWebServerDaemon onlineWebServerDaemon = new OnlineWebServerDaemon(computer, serviceName, os);
					onlineWebServerDaemon.setURL(text15);
					computer.daemons.Add(onlineWebServerDaemon);
				}
				else if (reader.Name == "AcademicDatabse")
				{
					reader.MoveToAttribute("name");
					string serviceName = reader.ReadContentAsString();
					AcademicDatabaseDaemon item4 = new AcademicDatabaseDaemon(computer, serviceName, os);
					computer.daemons.Add(item4);
				}
				else if (reader.Name == "MissionHubServer")
				{
					MissionHubServer item5 = new MissionHubServer(computer, "unknown", "unknown", os);
					computer.daemons.Add(item5);
				}
				else if (reader.Name == "DeathRowDatabase")
				{
					DeathRowDatabaseDaemon item6 = new DeathRowDatabaseDaemon(computer, "Death Row Database", os);
					computer.daemons.Add(item6);
				}
				else if (reader.Name == "MedicalDatabase")
				{
					MedicalDatabaseDaemon item7 = new MedicalDatabaseDaemon(computer, os);
					computer.daemons.Add(item7);
				}
				else if (reader.Name == "HeartMonitor")
				{
					string patientID = "UNKNOWN";
					if (reader.MoveToAttribute("patient"))
					{
						patientID = reader.ReadContentAsString();
					}
					HeartMonitorDaemon heartMonitorDaemon = new HeartMonitorDaemon(computer, os);
					heartMonitorDaemon.PatientID = patientID;
					computer.daemons.Add(heartMonitorDaemon);
				}
				else if (reader.Name.Equals("PointClicker"))
				{
					PointClickerDaemon item8 = new PointClickerDaemon(computer, "Point Clicker!", os);
					computer.daemons.Add(item8);
				}
				else if (reader.Name.Equals("ispSystem"))
				{
					ISPDaemon item9 = new ISPDaemon(computer, os);
					computer.daemons.Add(item9);
				}
				else if (reader.Name.Equals("porthackheart"))
				{
					PorthackHeartDaemon item10 = new PorthackHeartDaemon(computer, os);
					computer.daemons.Add(item10);
				}
				else if (reader.Name.Equals("SongChangerDaemon"))
				{
					SongChangerDaemon item11 = new SongChangerDaemon(computer, os);
					computer.daemons.Add(item11);
				}
				else if (reader.Name == "UploadServerDaemon")
				{
					string text16;
					string serviceName2;
					string foldername = serviceName2 = (text16 = "");
					if (reader.MoveToAttribute("name"))
					{
						serviceName2 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("foldername"))
					{
						foldername = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("color"))
					{
						text16 = reader.ReadContentAsString();
					}
					bool needsAuthentication = false;
					bool hasReturnViewButton = false;
					if (reader.MoveToAttribute("needsAuh"))
					{
						needsAuthentication = reader.ReadContentAsBoolean();
					}
					if (reader.MoveToAttribute("hasReturnViewButton"))
					{
						hasReturnViewButton = (reader.ReadContentAsString().ToLower() == "true");
					}
					Color themeColor2 = Color.White;
					if (text16 != "")
					{
						themeColor2 = Utils.convertStringToColor(text16);
					}
					UploadServerDaemon uploadServerDaemon = new UploadServerDaemon(computer, serviceName2, themeColor2, os, foldername, needsAuthentication);
					uploadServerDaemon.hasReturnViewButton = hasReturnViewButton;
					computer.daemons.Add(uploadServerDaemon);
				}
				else if (reader.Name == "DHSDaemon")
				{
					DLCHubServer item12 = new DLCHubServer(computer, "unknown", "unknown", os);
					computer.daemons.Add(item12);
				}
				else if (reader.Name == "CustomConnectDisplayDaemon")
				{
					CustomConnectDisplayDaemon item13 = new CustomConnectDisplayDaemon(computer, os);
					computer.daemons.Add(item13);
				}
				else if (reader.Name == "DatabaseDaemon")
				{
					string serviceName;
					string foldername2;
					string permissions;
					string dataTypeIdentifier = serviceName = (permissions = (foldername2 = null));
					Color? themeColor3 = null;
					string text17 = null;
					string adminResetEmailHostID = null;
					if (reader.MoveToAttribute("Name"))
					{
						serviceName = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("Permissions"))
					{
						permissions = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("DataType"))
					{
						dataTypeIdentifier = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("Foldername"))
					{
						foldername2 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("Color"))
					{
						string input2 = reader.ReadContentAsString();
						themeColor3 = new Color?(Utils.convertStringToColor(input2));
					}
					if (reader.MoveToAttribute("AdminEmailAccount"))
					{
						text17 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("AdminEmailHostID"))
					{
						adminResetEmailHostID = reader.ReadContentAsString();
					}
					DatabaseDaemon databaseDaemon = new DatabaseDaemon(computer, os, serviceName, permissions, dataTypeIdentifier, foldername2, themeColor3);
					if (!string.IsNullOrWhiteSpace(text17))
					{
						databaseDaemon.adminResetEmailHostID = adminResetEmailHostID;
						databaseDaemon.adminResetPassEmailAccount = text17;
					}
					computer.daemons.Add(databaseDaemon);
				}
				else if (reader.Name == "WhitelistAuthenticatorDaemon")
				{
					bool authenticatesItself = true;
					if (reader.MoveToAttribute("SelfAuthenticating"))
					{
						authenticatesItself = (reader.ReadContentAsString().ToLower() == "true");
					}
					WhitelistConnectionDaemon item14 = new WhitelistConnectionDaemon(computer, os)
					{
						AuthenticatesItself = authenticatesItself
					};
					computer.daemons.Add(item14);
				}
				else if (reader.Name == "IRCDaemon")
				{
					IRCDaemon item15 = new IRCDaemon(computer, os, "LOAD ERROR");
					computer.daemons.Add(item15);
				}
				else if (reader.Name == "MarkovTextDaemon")
				{
					string text18;
					string corpusLoadPath = text18 = null;
					if (reader.MoveToAttribute("Name"))
					{
						text18 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("SourceFilesContentFolder"))
					{
						corpusLoadPath = reader.ReadContentAsString();
					}
					MarkovTextDaemon item16 = new MarkovTextDaemon(computer, os, text18, corpusLoadPath);
					computer.daemons.Add(item16);
				}
				else if (reader.Name.Equals("AircraftDaemon"))
				{
					Vector2 zero = Vector2.Zero;
					Vector2 mapDest = Vector2.One * 0.5f;
					float progress = 0.5f;
					string text18 = "Pacific Charter Flight";
					if (reader.MoveToAttribute("Name"))
					{
						text18 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("OriginX"))
					{
						zero.X = Utils.RobustReadAsFloat(reader);
					}
					if (reader.MoveToAttribute("OriginY"))
					{
						zero.Y = Utils.RobustReadAsFloat(reader);
					}
					if (reader.MoveToAttribute("DestX"))
					{
						mapDest.X = Utils.RobustReadAsFloat(reader);
					}
					if (reader.MoveToAttribute("DestY"))
					{
						mapDest.Y = Utils.RobustReadAsFloat(reader);
					}
					if (reader.MoveToAttribute("Progress"))
					{
						progress = Utils.RobustReadAsFloat(reader);
					}
					AircraftDaemon item17 = new AircraftDaemon(computer, os, text18, zero, mapDest, progress);
					computer.daemons.Add(item17);
				}
				else if (reader.Name.Equals("LogoCustomConnectDisplayDaemon"))
				{
					string logoImageName = null;
					string text11 = null;
					string buttonAlignment = null;
					bool logoShouldClipoverdraw = false;
					if (reader.MoveToAttribute("logo"))
					{
						logoImageName = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("title"))
					{
						text11 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("overdrawLogo"))
					{
						logoShouldClipoverdraw = (reader.ReadContentAsString().ToLower() == "true");
					}
					if (reader.MoveToAttribute("buttonAlignment"))
					{
						buttonAlignment = reader.ReadContentAsString();
					}
					LogoCustomConnectDisplayDaemon item18 = new LogoCustomConnectDisplayDaemon(computer, os, logoImageName, text11, logoShouldClipoverdraw, buttonAlignment);
					computer.daemons.Add(item18);
				}
				else if (reader.Name.Equals("LogoDaemon"))
				{
					string logoImagePath = null;
					bool showsTitle = true;
					Color textColor = Color.White;
					if (reader.MoveToAttribute("LogoImagePath"))
					{
						logoImagePath = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("TextColor"))
					{
						textColor = Utils.convertStringToColor(reader.ReadContentAsString());
					}
					if (reader.MoveToAttribute("Name"))
					{
						string text19 = reader.ReadContentAsString();
					}
					if (reader.MoveToAttribute("ShowsTitle"))
					{
						showsTitle = (reader.ReadContentAsString().ToLower() == "true");
					}
					LogoDaemon logoDaemon = new LogoDaemon(computer, os, compName, showsTitle, logoImagePath);
					logoDaemon.TextColor = textColor;
					computer.daemons.Add(logoDaemon);
				}
				else if (reader.Name.Equals("DLCCredits"))
				{
					string text20 = null;
					if (reader.MoveToAttribute("Button"))
					{
						text20 = reader.ReadContentAsString();
					}
					string text11 = null;
					if (reader.MoveToAttribute("Title"))
					{
						text11 = reader.ReadContentAsString();
					}
					DLCCreditsDaemon dlccreditsDaemon;
					if (text20 != null || text11 != null)
					{
						dlccreditsDaemon = new DLCCreditsDaemon(computer, os, text11, text20);
					}
					else
					{
						dlccreditsDaemon = new DLCCreditsDaemon(computer, os);
					}
					if (reader.MoveToAttribute("Action"))
					{
						dlccreditsDaemon.ConditionalActionsToLoadOnButtonPress = reader.ReadContentAsString();
					}
					computer.daemons.Add(dlccreditsDaemon);
				}
				else if (reader.Name.Equals("FastActionHost"))
				{
					FastActionHost item19 = new FastActionHost(computer, os, compName);
					computer.daemons.Add(item19);
				}
				reader.Read();
			}
			computer.files = FileSystem.load(reader);
			computer.traceTime = num;
			computer.portsNeededForCrack = num2;
			computer.adminIP = text4;
			computer.idName = text;
			computer.icon = text3;
			computer.attatchedDeviceIDs = text2;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != "")
				{
					computer.links.Add(Convert.ToInt32(array[i]));
				}
			}
			if (text8.Length > 0)
			{
				ComputerLoader.loadPortsIntoComputer(text8, computer);
			}
			if (a == "mail")
			{
				os.netMap.mailServer = computer;
			}
			else if (a == "player")
			{
				os.thisComputer = computer;
			}
			return computer;
		}

		// Token: 0x060005CB RID: 1483 RVA: 0x0005BAB0 File Offset: 0x00059CB0
		public string getTooltipString()
		{
			string str = this.name;
			return str + "\n" + this.ip;
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x0005BADC File Offset: 0x00059CDC
		public Vector2 getScreenSpacePosition()
		{
			Vector2 nodeDrawPos = this.os.netMap.GetNodeDrawPos(this);
			return new Vector2((float)this.os.netMap.bounds.X + nodeDrawPos.X + (float)(NetworkMap.NODE_SIZE / 2), (float)this.os.netMap.bounds.Y + nodeDrawPos.Y + (float)(NetworkMap.NODE_SIZE / 2));
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x0005BB54 File Offset: 0x00059D54
		public Daemon getDaemon(Type t)
		{
			for (int i = 0; i < this.daemons.Count; i++)
			{
				if (this.daemons[i].GetType().Equals(t))
				{
					return this.daemons[i];
				}
			}
			return null;
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0005BBB4 File Offset: 0x00059DB4
		public static string generateBinaryString(int length)
		{
			byte[] array = new byte[length / 8];
			Utils.random.NextBytes(array);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 2);
			}
			return text;
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x0005BC08 File Offset: 0x00059E08
		public static string generateBinaryString(int length, MSRandom rng)
		{
			byte[] array = new byte[length / 8];
			rng.NextBytes(array);
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += Convert.ToString(array[i], 2);
			}
			return text;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0005BC58 File Offset: 0x00059E58
		public static Folder getFolderAtDepth(Computer c, int depth, List<int> path)
		{
			Folder folder = c.files.root;
			if (path.Count > 0)
			{
				for (int i = 0; i < depth; i++)
				{
					if (folder.folders.Count > path[i])
					{
						folder = folder.folders[path[i]];
					}
				}
			}
			return folder;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x0005BCCC File Offset: 0x00059ECC
		public override string ToString()
		{
			return "Comp : " + this.idName;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x0005BCF0 File Offset: 0x00059EF0
		public static Computer loadFromFile(string filename)
		{
			return (Computer)ComputerLoader.loadComputer(filename, false, false);
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x0005BD14 File Offset: 0x00059F14
		public Folder getFolderFromPath(string path, bool createFoldersThatDontExist = false)
		{
			Folder result;
			if (string.IsNullOrWhiteSpace(path))
			{
				result = this.files.root;
			}
			else
			{
				List<int> folderPath = this.getFolderPath(path, createFoldersThatDontExist);
				result = Computer.getFolderAtDepth(this, folderPath.Count, folderPath);
			}
			return result;
		}

		// Token: 0x060005D4 RID: 1492 RVA: 0x0005BD58 File Offset: 0x00059F58
		public List<int> getFolderPath(string path, bool createFoldersThatDontExist = false)
		{
			List<int> list = new List<int>();
			char[] separator = new char[]
			{
				'/',
				'\\'
			};
			string[] array = path.Split(separator);
			Folder folder = this.files.root;
			for (int i = 0; i < array.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < folder.folders.Count; j++)
				{
					if (folder.folders[j].name.Equals(array[i]))
					{
						list.Add(j);
						folder = folder.folders[j];
						flag = true;
						break;
					}
				}
				if (!flag && createFoldersThatDontExist)
				{
					folder.folders.Add(new Folder(array[i]));
					int num = folder.folders.Count - 1;
					folder = folder.folders[num];
					list.Add(num);
				}
			}
			return list;
		}

		// Token: 0x060005D5 RID: 1493 RVA: 0x0005BE74 File Offset: 0x0005A074
		public bool PlayerHasAdminPermissions()
		{
			return this.adminIP == this.os.thisComputer.ip || (this.currentUser.type == 0 && this.currentUser.name != null);
		}

		// Token: 0x0400065A RID: 1626
		public const byte CORPORATE = 1;

		// Token: 0x0400065B RID: 1627
		public const byte HOME = 2;

		// Token: 0x0400065C RID: 1628
		public const byte SERVER = 3;

		// Token: 0x0400065D RID: 1629
		public const byte EMPTY = 4;

		// Token: 0x0400065E RID: 1630
		public const byte EOS = 5;

		// Token: 0x0400065F RID: 1631
		public static float BASE_BOOT_TIME = Settings.isConventionDemo ? 15f : 25.5f;

		// Token: 0x04000660 RID: 1632
		public static float BASE_REBOOT_TIME = 10.5f;

		// Token: 0x04000661 RID: 1633
		public static float BASE_PROXY_TICKS = 30f;

		// Token: 0x04000662 RID: 1634
		public static float BASE_TRACE_TIME = 15f;

		// Token: 0x04000663 RID: 1635
		public string name;

		// Token: 0x04000664 RID: 1636
		public string idName;

		// Token: 0x04000665 RID: 1637
		public string ip;

		// Token: 0x04000666 RID: 1638
		public Vector2 location;

		// Token: 0x04000667 RID: 1639
		public FileSystem files;

		// Token: 0x04000668 RID: 1640
		public int securityLevel;

		// Token: 0x04000669 RID: 1641
		public float traceTime;

		// Token: 0x0400066A RID: 1642
		public int portsNeededForCrack = 0;

		// Token: 0x0400066B RID: 1643
		public string adminIP;

		// Token: 0x0400066C RID: 1644
		public List<UserDetail> users;

		// Token: 0x0400066D RID: 1645
		public List<int> links;

		// Token: 0x0400066E RID: 1646
		public List<int> ports;

		// Token: 0x0400066F RID: 1647
		public List<byte> portsOpen;

		// Token: 0x04000670 RID: 1648
		public bool silent = false;

		// Token: 0x04000671 RID: 1649
		public bool disabled = false;

		// Token: 0x04000672 RID: 1650
		internal float bootTimer = 0f;

		// Token: 0x04000673 RID: 1651
		public bool userLoggedIn = false;

		// Token: 0x04000674 RID: 1652
		public UserDetail currentUser;

		// Token: 0x04000675 RID: 1653
		public Dictionary<int, int> PortRemapping = null;

		// Token: 0x04000676 RID: 1654
		public string icon = null;

		// Token: 0x04000677 RID: 1655
		private float timeLastPinged;

		// Token: 0x04000678 RID: 1656
		public float highlightFlashTime = 0f;

		// Token: 0x04000679 RID: 1657
		public byte type;

		// Token: 0x0400067A RID: 1658
		public string adminPass;

		// Token: 0x0400067B RID: 1659
		public List<Daemon> daemons;

		// Token: 0x0400067C RID: 1660
		public bool AllowsDefaultBootModule = true;

		// Token: 0x0400067D RID: 1661
		public bool hasProxy = false;

		// Token: 0x0400067E RID: 1662
		public float proxyOverloadTicks = 0f;

		// Token: 0x0400067F RID: 1663
		public float startingOverloadTicks = -1f;

		// Token: 0x04000680 RID: 1664
		public bool proxyActive = false;

		// Token: 0x04000681 RID: 1665
		public ShellExe reportingShell = null;

		// Token: 0x04000682 RID: 1666
		public ExternalCounterpart externalCounterpart = null;

		// Token: 0x04000683 RID: 1667
		public string attatchedDeviceIDs = null;

		// Token: 0x04000684 RID: 1668
		public Firewall firewall = null;

		// Token: 0x04000685 RID: 1669
		public bool firewallAnalysisInProgress = false;

		// Token: 0x04000686 RID: 1670
		public bool HasTracker = false;

		// Token: 0x04000687 RID: 1671
		public Administrator admin = null;

		// Token: 0x04000688 RID: 1672
		private OS os;

		// Token: 0x04000689 RID: 1673
		public MemoryContents Memory;
	}
}
