using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x0200013D RID: 317
	internal class Multiplayer
	{
		// Token: 0x0600078F RID: 1935 RVA: 0x0007C114 File Offset: 0x0007A314
		public static void parseInputMessage(string message, OS os)
		{
			if (!message.Equals(""))
			{
				string[] array = message.Split(Multiplayer.delims);
				if (array.Length != 0)
				{
					if (os.thisComputer != null && os.thisComputer.ip.Equals(array[1]))
					{
						os.warningFlash();
					}
					if (array[0].Equals("init"))
					{
						int num = Convert.ToInt32(array[1]);
						Utils.random = new Random(num);
						os.canRunContent = true;
						os.LoadContent();
						os.write("Seed Established :" + num);
					}
					else if (array[0].Equals("chat"))
					{
						string text = "";
						for (int i = 2; i < array.Length; i++)
						{
							text = text + array[i] + " ";
						}
						os.write(array[1] + ": " + DisplayModule.splitForWidth(text, 350));
					}
					else if (array[0].Equals("clientConnect"))
					{
						os.write("Connection Established");
					}
					else if (array[0].Equals("cConnection"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						if (computer == null)
						{
							os.write("Error in Message : " + message);
						}
						else
						{
							computer.silent = true;
							computer.connect(array[2]);
							computer.silent = false;
							os.opponentLocation = array[1];
						}
					}
					else if (array[0].Equals("cDisconnect"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.disconnecting(array[2], true);
						computer.silent = false;
						os.opponentLocation = "";
					}
					else if (array[0].Equals("cAdmin"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.giveAdmin(array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("cPortOpen"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.openPort(Convert.ToInt32(array[3]), array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("cPortClose"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.closePort(Convert.ToInt32(array[3]), array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("cFile"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						FileEntry fileEntry = new FileEntry("", array[3]);
						computer.canReadFile(array[2], fileEntry, Convert.ToInt32(array[4]));
						computer.silent = false;
					}
					else if (array[0].Equals("newComp"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Vector2 compLocation = new Vector2((float)Convert.ToInt32(array[2]), (float)Convert.ToInt32(array[3]));
						Computer computer = new Computer(array[5], array[1], compLocation, Convert.ToInt32(array[4]), 1, os);
						computer.idName = "opponent#" + Multiplayer.generatedComputerCount;
						Multiplayer.generatedComputerCount++;
						computer.addMultiplayerTargetFile();
						os.netMap.nodes.Add(computer);
						os.opponentComputer = computer;
					}
					else if (array[0].Equals("cDelete"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Computer computer = Multiplayer.getComp(array[1], os);
						List<int> list = new List<int>();
						for (int i = 4; i < array.Length; i++)
						{
							if (array[i] != "")
							{
								list.Add(Convert.ToInt32(array[i]));
							}
						}
						computer.silent = true;
						computer.deleteFile(array[2], array[3], list);
						computer.silent = false;
					}
					else if (array[0].Equals("cMake"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Computer computer = Multiplayer.getComp(array[1], os);
						List<int> list = new List<int>();
						for (int i = 4; i < array.Length; i++)
						{
							if (array[i] != "")
							{
								list.Add(Convert.ToInt32(array[i]));
							}
						}
						computer.silent = true;
						computer.makeFile(array[2], array[3], array[4], list, false);
						computer.silent = false;
					}
					else if (array[0].Equals("cMove"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Computer computer = Multiplayer.getComp(array[1], os);
						char[] separator = new char[]
						{
							'%'
						};
						List<int> list = new List<int>();
						string[] array2 = array[5].Split(separator, 500, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < array2.Length; i++)
						{
							if (array[i] != "")
							{
								list.Add(Convert.ToInt32(array[i]));
							}
						}
						List<int> list2 = new List<int>();
						array2 = array[6].Split(separator, 500, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < array2.Length; i++)
						{
							if (array[i] != "")
							{
								list2.Add(Convert.ToInt32(array[i]));
							}
						}
						computer.silent = true;
						computer.moveFile(array[2], array[3], array[4], list, list2);
						computer.silent = false;
					}
					else if (array[0].Equals("cMkDir"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Computer computer = Multiplayer.getComp(array[1], os);
						List<int> list = new List<int>();
						for (int i = 4; i < array.Length; i++)
						{
							if (array[i] != "")
							{
								list.Add(Convert.ToInt32(array[i]));
							}
						}
						computer.silent = true;
						computer.makeFolder(array[2], array[3], list);
						computer.silent = false;
					}
					else if (array[0].Equals("cAddUser"))
					{
						array = message.Split(Multiplayer.specSplitDelims);
						Computer computer = Multiplayer.getComp(array[1], os);
						string name = array[3];
						string pass = array[4];
						byte type = Convert.ToByte(array[5]);
						computer.silent = true;
						computer.addNewUser(array[2], name, pass, type);
						computer.silent = false;
					}
					else if (array[0].Equals("cCopy"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.canCopyFile(array[2], array[3]);
						computer.silent = false;
						FileEntry fileEntry = null;
						for (int i = 0; i < computer.files.root.folders[2].files.Count; i++)
						{
							if (computer.files.root.folders[2].files[i].name.Equals(array[3]))
							{
								fileEntry = computer.files.root.folders[2].files[i];
							}
						}
						FileEntry item = new FileEntry(fileEntry.data, fileEntry.name);
						Computer comp = Multiplayer.getComp(array[2], os);
						comp.files.root.folders[2].files.Add(item);
					}
					else if (array[0].Equals("cCDDrive"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						if (array[2].Equals("open"))
						{
							computer.openCDTray(array[1]);
						}
						else
						{
							computer.closeCDTray(array[1]);
						}
						computer.silent = false;
					}
					else if (array[0].Equals("cCrash"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.crash(array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("cReboot"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						computer.silent = true;
						computer.reboot(array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("cFBClients"))
					{
						Computer computer = Multiplayer.getComp(array[1], os);
						if (os.connectedComp != null && os.connectedComp.ip.Equals(array[1]))
						{
							os.exes.Add(new ForkBombExe(os.getExeBounds(), os));
						}
						computer.silent = true;
						computer.forkBombClients(array[2]);
						computer.silent = false;
					}
					else if (array[0].Equals("eForkBomb"))
					{
						if (os.thisComputer.ip.Equals(array[1]))
						{
							ForkBombExe forkBombExe = new ForkBombExe(os.getExeBounds(), os);
							forkBombExe.LoadContent();
							os.exes.Add(forkBombExe);
						}
					}
					else if (array[0].Equals("mpOpponentWin"))
					{
						os.endMultiplayerMatch(false);
					}
					else if (!array[0].Equals("stayAlive"))
					{
						os.write("MSG: " + message);
					}
				}
			}
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0007CBB8 File Offset: 0x0007ADB8
		private static Computer getComp(string ip, OS os)
		{
			for (int i = 0; i < os.netMap.nodes.Count; i++)
			{
				if (os.netMap.nodes[i].ip.Equals(ip))
				{
					return os.netMap.nodes[i];
				}
			}
			return null;
		}

		// Token: 0x0400088F RID: 2191
		private static int generatedComputerCount = 0;

		// Token: 0x04000890 RID: 2192
		public static int PORT = 3020;

		// Token: 0x04000891 RID: 2193
		public static char[] delims = new char[]
		{
			' ',
			'\n'
		};

		// Token: 0x04000892 RID: 2194
		public static char[] specSplitDelims = new char[]
		{
			'#'
		};
	}
}
