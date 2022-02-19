using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Hacknet.Extensions;
using Hacknet.Mission;
using Hacknet.PlatformAPI.Storage;
using Hacknet.Security;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000102 RID: 258
	public static class ComputerLoader
	{
		// Token: 0x060005D7 RID: 1495 RVA: 0x0005BF0F File Offset: 0x0005A10F
		public static void init(object opsys)
		{
			ComputerLoader.os = (OS)opsys;
		}

		// Token: 0x060005D8 RID: 1496 RVA: 0x0005C02C File Offset: 0x0005A22C
		public static object loadComputer(string filename, bool preventAddingToNetmap = false, bool preventInitDaemons = false)
		{
			filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
			Computer c = null;
			XmlReader xmlReader = null;
			Stream input;
			if (filename.EndsWith("ExampleComputer.xml"))
			{
				string text = File.ReadAllText(filename);
				int num = text.IndexOf("<!--START_LABYRINTHS_ONLY_CONTENT-->");
				string text2 = "<!--END_LABYRINTHS_ONLY_CONTENT-->";
				int num2 = text.IndexOf(text2);
				if (num >= 0 && num2 >= 0)
				{
					text = text.Substring(0, num) + text.Substring(num2 + text2.Length);
				}
				input = Utils.GenerateStreamFromString(text);
			}
			else
			{
				input = File.OpenRead(filename);
			}
			xmlReader = XmlReader.Create(input);
			string idName = "UNKNOWN";
			string text3 = "UNKNOWN";
			string icon = null;
			int seclevel = 0;
			byte compType = 1;
			bool allowsDefaultBootModule = true;
			string compIP = NetworkMap.generateRandomIP();
			while (xmlReader.Name != "Computer")
			{
				xmlReader.Read();
				if (xmlReader.EOF)
				{
					return null;
				}
			}
			if (xmlReader.MoveToAttribute("id"))
			{
				idName = xmlReader.ReadContentAsString();
			}
			if (xmlReader.MoveToAttribute("name"))
			{
				text3 = ComputerLoader.filter(xmlReader.ReadContentAsString());
			}
			if (xmlReader.MoveToAttribute("security"))
			{
				seclevel = xmlReader.ReadContentAsInt();
			}
			if (xmlReader.MoveToAttribute("type"))
			{
				string text4 = xmlReader.ReadContentAsString();
				if (text4.ToLowerInvariant() == "empty")
				{
					text4 = string.Concat(4);
				}
				compType = Convert.ToByte(text4);
			}
			if (xmlReader.MoveToAttribute("ip"))
			{
				compIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
			}
			if (xmlReader.MoveToAttribute("icon"))
			{
				icon = xmlReader.ReadContentAsString();
			}
			if (xmlReader.MoveToAttribute("allowsDefaultBootModule"))
			{
				allowsDefaultBootModule = xmlReader.ReadContentAsBoolean();
			}
			c = new Computer(text3, compIP, ComputerLoader.os.netMap.getRandomPosition(), seclevel, compType, ComputerLoader.os);
			c.idName = idName;
			c.AllowsDefaultBootModule = allowsDefaultBootModule;
			c.icon = icon;
			if (c.type == 4)
			{
				Folder folder = c.files.root.searchForFolder("home");
				if (folder != null)
				{
					folder.files.Clear();
					folder.folders.Clear();
				}
			}
			while (xmlReader.Name != "Computer")
			{
				if (xmlReader.Name.ToLower().Equals("file"))
				{
					bool flag = true;
					bool flag2 = false;
					string path;
					if (xmlReader.MoveToAttribute("path"))
					{
						path = xmlReader.ReadContentAsString();
					}
					else
					{
						path = "home";
					}
					string text5;
					if (xmlReader.MoveToAttribute("name"))
					{
						text5 = xmlReader.ReadContentAsString();
					}
					else
					{
						text5 = "Data";
					}
					if (xmlReader.MoveToAttribute("EduSafe"))
					{
						flag = (xmlReader.ReadContentAsString().ToLower() == "true");
					}
					if (xmlReader.MoveToAttribute("EduSafeOnly"))
					{
						flag2 = (xmlReader.ReadContentAsString().ToLower() == "true");
					}
					text5 = ComputerLoader.filter(text5);
					xmlReader.MoveToContent();
					string text = xmlReader.ReadElementContentAsString();
					if (text.Equals(""))
					{
						text = Computer.generateBinaryString(500);
					}
					text = ComputerLoader.filter(text);
					Folder folderFromPath = c.getFolderFromPath(path, true);
					if (flag || !Settings.EducationSafeBuild)
					{
						if (Settings.EducationSafeBuild || !flag2)
						{
							if (folderFromPath.searchForFile(text5) != null)
							{
								folderFromPath.searchForFile(text5).data = text;
							}
							else
							{
								folderFromPath.files.Add(new FileEntry(text, text5));
							}
						}
					}
				}
				if (xmlReader.Name.Equals("encryptedFile"))
				{
					bool flag3 = false;
					string path;
					if (xmlReader.MoveToAttribute("path"))
					{
						path = xmlReader.ReadContentAsString();
					}
					else
					{
						path = "home";
					}
					string text5;
					if (xmlReader.MoveToAttribute("name"))
					{
						text5 = xmlReader.ReadContentAsString();
					}
					else
					{
						text5 = "Data";
					}
					string header;
					if (xmlReader.MoveToAttribute("header"))
					{
						header = xmlReader.ReadContentAsString();
					}
					else
					{
						header = "ERROR";
					}
					string ipLink;
					if (xmlReader.MoveToAttribute("ip"))
					{
						ipLink = xmlReader.ReadContentAsString();
					}
					else
					{
						ipLink = "ERROR";
					}
					string pass;
					if (xmlReader.MoveToAttribute("pass"))
					{
						pass = xmlReader.ReadContentAsString();
					}
					else
					{
						pass = "";
					}
					string text6;
					if (xmlReader.MoveToAttribute("extension"))
					{
						text6 = xmlReader.ReadContentAsString();
					}
					else
					{
						text6 = null;
					}
					if (xmlReader.MoveToAttribute("double"))
					{
						flag3 = xmlReader.ReadContentAsBoolean();
					}
					text5 = ComputerLoader.filter(text5);
					xmlReader.MoveToContent();
					string text = xmlReader.ReadElementContentAsString();
					if (text.Equals(""))
					{
						text = Computer.generateBinaryString(500);
					}
					text = ComputerLoader.filter(text);
					if (flag3)
					{
						text = FileEncrypter.EncryptString(text, header, ipLink, pass, text6);
					}
					text = FileEncrypter.EncryptString(text, header, ipLink, pass, flag3 ? "_LAYER2.dec" : text6);
					Folder folderFromPath = c.getFolderFromPath(path, true);
					if (folderFromPath.searchForFile(text5) != null)
					{
						folderFromPath.searchForFile(text5).data = text;
					}
					else
					{
						folderFromPath.files.Add(new FileEntry(text, text5));
					}
				}
				if (xmlReader.Name.Equals("memoryDumpFile"))
				{
					string path;
					if (xmlReader.MoveToAttribute("path"))
					{
						path = xmlReader.ReadContentAsString();
					}
					else
					{
						path = "home";
					}
					string text5;
					if (xmlReader.MoveToAttribute("name"))
					{
						text5 = xmlReader.ReadContentAsString();
					}
					else
					{
						text5 = "Data";
					}
					xmlReader.MoveToContent();
					MemoryContents memoryContents = MemoryContents.Deserialize(xmlReader);
					Folder folderFromPath = c.getFolderFromPath(path, true);
					if (folderFromPath.searchForFile(text5) != null)
					{
						folderFromPath.searchForFile(text5).data = memoryContents.GetEncodedFileString();
					}
					else
					{
						folderFromPath.files.Add(new FileEntry(memoryContents.GetEncodedFileString(), text5));
					}
					while (!(xmlReader.Name == "memoryDumpFile") || xmlReader.IsStartElement() || xmlReader.EOF)
					{
						xmlReader.Read();
					}
					if (xmlReader.EOF)
					{
						throw new FormatException("Unexpected end of file looking for memoryDumpFile close tag");
					}
					xmlReader.Read();
				}
				if (xmlReader.Name.ToLower().Equals("customthemefile"))
				{
					string path;
					if (xmlReader.MoveToAttribute("path"))
					{
						path = xmlReader.ReadContentAsString();
					}
					else
					{
						path = "home";
					}
					string text5;
					if (xmlReader.MoveToAttribute("name"))
					{
						text5 = xmlReader.ReadContentAsString();
					}
					else
					{
						text5 = "Data.txt";
					}
					text5 = ComputerLoader.filter(text5);
					string text;
					if (xmlReader.MoveToAttribute("themePath"))
					{
						text = xmlReader.ReadContentAsString();
						text = ThemeManager.getThemeDataStringForCustomTheme(text);
					}
					else
					{
						text = null;
					}
					if (string.IsNullOrWhiteSpace(text))
					{
						text = "DEFINITION ERROR - Theme generated incorrectly. No Custom theme found at definition path";
					}
					text = ComputerLoader.filter(text);
					Folder folderFromPath = c.getFolderFromPath(path, true);
					if (folderFromPath.searchForFile(text5) != null)
					{
						folderFromPath.searchForFile(text5).data = text;
					}
					else
					{
						folderFromPath.files.Add(new FileEntry(text, text5));
					}
				}
				else if (xmlReader.Name.Equals("ports"))
				{
					xmlReader.MoveToContent();
					string portsList = xmlReader.ReadElementContentAsString();
					ComputerLoader.loadPortsIntoComputer(portsList, c);
				}
				else if (xmlReader.Name.Equals("positionNear"))
				{
					string targetNearNodeName = "";
					if (xmlReader.MoveToAttribute("target"))
					{
						targetNearNodeName = xmlReader.ReadContentAsString();
					}
					int position = 0;
					int total = 3;
					if (xmlReader.MoveToAttribute("position"))
					{
						position = xmlReader.ReadContentAsInt();
					}
					position++;
					if (xmlReader.MoveToAttribute("total"))
					{
						total = xmlReader.ReadContentAsInt();
					}
					bool forceUse = false;
					if (xmlReader.MoveToAttribute("force"))
					{
						forceUse = (xmlReader.ReadContentAsString().ToLower() == "true");
					}
					float extraDistance = 0f;
					if (xmlReader.MoveToAttribute("extraDistance"))
					{
						extraDistance = xmlReader.ReadContentAsFloat();
						extraDistance = Math.Max(-1f, Math.Min(1f, extraDistance));
					}
					ComputerLoader.postAllLoadedActions = (Action)Delegate.Combine(ComputerLoader.postAllLoadedActions, new Action(delegate()
					{
						Computer computer2 = Programs.getComputer(ComputerLoader.os, targetNearNodeName);
						if (computer2 != null)
						{
							c.location = computer2.location + Corporation.getNearbyNodeOffset(computer2.location, position, total, ComputerLoader.os.netMap, extraDistance, forceUse);
						}
					}));
				}
				else if (xmlReader.Name.Equals("proxy"))
				{
					float num3 = 1f;
					if (xmlReader.MoveToAttribute("time"))
					{
						num3 = xmlReader.ReadContentAsFloat();
					}
					if (num3 > 0f)
					{
						c.addProxy(Computer.BASE_PROXY_TICKS * num3);
					}
					else
					{
						c.hasProxy = false;
						c.proxyActive = false;
					}
				}
				else if (xmlReader.Name.Equals("portsForCrack"))
				{
					int num4 = -1;
					if (xmlReader.MoveToAttribute("val"))
					{
						num4 = xmlReader.ReadContentAsInt();
					}
					if (num4 != -1)
					{
						c.portsNeededForCrack = num4 - 1;
					}
				}
				else if (xmlReader.Name.Equals("firewall"))
				{
					int num5 = 1;
					if (xmlReader.MoveToAttribute("level"))
					{
						num5 = xmlReader.ReadContentAsInt();
					}
					if (num5 > 0)
					{
						string text7 = null;
						float additionalTime = 0f;
						if (xmlReader.MoveToAttribute("solution"))
						{
							text7 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("additionalTime"))
						{
							additionalTime = xmlReader.ReadContentAsFloat();
						}
						if (text7 != null)
						{
							c.addFirewall(num5, text7, additionalTime);
						}
						else
						{
							c.addFirewall(num5);
						}
					}
					else
					{
						c.firewall = null;
					}
				}
				else if (xmlReader.Name.Equals("link"))
				{
					string ip_Or_ID_or_Name = "";
					if (xmlReader.MoveToAttribute("target"))
					{
						ip_Or_ID_or_Name = xmlReader.ReadContentAsString();
					}
					Computer computer = Programs.getComputer(ComputerLoader.os, ip_Or_ID_or_Name);
					if (computer != null)
					{
						c.links.Add(ComputerLoader.os.netMap.nodes.IndexOf(computer));
					}
				}
				else if (xmlReader.Name.Equals("dlink"))
				{
					string comp = "";
					if (xmlReader.MoveToAttribute("target"))
					{
						comp = xmlReader.ReadContentAsString();
					}
					Computer local = c;
					ComputerLoader.postAllLoadedActions = (Action)Delegate.Combine(ComputerLoader.postAllLoadedActions, new Action(delegate()
					{
						Computer computer2 = Programs.getComputer(ComputerLoader.os, comp);
						if (computer2 != null)
						{
							local.links.Add(ComputerLoader.os.netMap.nodes.IndexOf(computer2));
						}
					}));
				}
				else if (xmlReader.Name.Equals("trace"))
				{
					float num3 = 1f;
					if (xmlReader.MoveToAttribute("time"))
					{
						num3 = xmlReader.ReadContentAsFloat();
					}
					c.traceTime = num3;
				}
				else if (xmlReader.Name.Equals("adminPass"))
				{
					string text8 = null;
					if (xmlReader.MoveToAttribute("pass"))
					{
						text8 = ComputerLoader.filter(xmlReader.ReadContentAsString());
					}
					if (text8 == null)
					{
						text8 = PortExploits.getRandomPassword();
					}
					c.setAdminPassword(text8);
				}
				else
				{
					if (xmlReader.Name.Equals("admin"))
					{
						string text9 = "basic";
						bool resetsPassword = true;
						bool isSuper = false;
						if (xmlReader.MoveToAttribute("type"))
						{
							text9 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("resetPassword"))
						{
							resetsPassword = xmlReader.ReadContentAsBoolean();
						}
						if (xmlReader.MoveToAttribute("isSuper"))
						{
							isSuper = xmlReader.ReadContentAsBoolean();
						}
						string text10 = text9;
						if (text10 == null || text10 == "basic")
						{
							goto IL_E16;
						}
						if (!(text10 == "fast"))
						{
							if (!(text10 == "progress"))
							{
								if (!(text10 == "none"))
								{
									goto IL_E16;
								}
								c.admin = null;
							}
							else
							{
								c.admin = new FastProgressOnlyAdministrator();
							}
						}
						else
						{
							c.admin = new FastBasicAdministrator();
						}
						IL_E5E:
						if (c.admin != null)
						{
							c.admin.ResetsPassword = resetsPassword;
							c.admin.IsSuper = isSuper;
						}
						goto IL_2C1A;
						IL_E16:
						c.admin = new BasicAdministrator();
						goto IL_E5E;
					}
					if (xmlReader.Name.Equals("portRemap"))
					{
						xmlReader.MoveToContent();
						string text11 = xmlReader.ReadElementContentAsString();
						try
						{
							c.PortRemapping = PortRemappingSerializer.Deserialize(text11);
						}
						catch (FormatException innerException)
						{
							throw new FormatException("Error in portRemap tag. Check that your list is properly comma separated!\nBroken Data: " + text11, innerException);
						}
					}
					else if (xmlReader.Name.Equals("ExternalCounterpart"))
					{
						string serverName = "";
						string idName2 = "";
						if (xmlReader.MoveToAttribute("id"))
						{
							serverName = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("name"))
						{
							idName2 = xmlReader.ReadContentAsString();
						}
						ExternalCounterpart externalCounterpart = new ExternalCounterpart(idName2, ExternalCounterpart.getIPForServerName(serverName));
						c.externalCounterpart = externalCounterpart;
					}
					else if (xmlReader.Name.Equals("account"))
					{
						byte b = 0;
						string text12;
						string text8 = text12 = "ERROR";
						if (xmlReader.MoveToAttribute("username"))
						{
							text12 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("password"))
						{
							text8 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("type"))
						{
							string text = xmlReader.ReadContentAsString();
							if (text.ToLower() == "admin")
							{
								b = 0;
							}
							else if (text.ToLower() == "all")
							{
								b = 1;
							}
							else if (text.ToLower() == "mail")
							{
								b = 2;
							}
							else if (text.ToLower() == "missionlist")
							{
								b = 3;
							}
							else
							{
								b = (byte)Convert.ToInt32(text);
							}
						}
						text12 = ComputerLoader.filter(text12);
						text8 = ComputerLoader.filter(text8);
						bool flag4 = false;
						for (int i = 0; i < c.users.Count; i++)
						{
							UserDetail userDetail = c.users[i];
							if (userDetail.name.Equals(text12))
							{
								userDetail.pass = text8;
								userDetail.type = b;
								c.users[i] = userDetail;
								if (text12.Equals("admin"))
								{
									c.adminPass = text8;
								}
								flag4 = true;
							}
						}
						if (!flag4)
						{
							UserDetail userDetail = new UserDetail(text12, text8, b);
							c.users.Add(userDetail);
						}
					}
					else if (xmlReader.Name.ToLower().Equals("tracker"))
					{
						c.HasTracker = true;
					}
					else if (xmlReader.Name.Equals("missionListingServer"))
					{
						bool flag5 = false;
						bool isPublic = false;
						string group;
						string text13 = group = "ERROR";
						if (xmlReader.MoveToAttribute("name"))
						{
							text13 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("group"))
						{
							group = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("assigner"))
						{
							flag5 = xmlReader.ReadContentAsBoolean();
						}
						if (xmlReader.MoveToAttribute("public"))
						{
							isPublic = xmlReader.ReadContentAsBoolean();
						}
						MissionListingServer missionListingServer = new MissionListingServer(c, text13, group, ComputerLoader.os, isPublic, false);
						missionListingServer.missionAssigner = flag5;
						c.daemons.Add(missionListingServer);
					}
					else if (xmlReader.Name.Equals("variableMissionListingServer"))
					{
						bool flag5 = false;
						bool isPublic = false;
						string text14;
						string articleFolderPath;
						string iconPath;
						string text13 = iconPath = (articleFolderPath = (text14 = null));
						Color themeColor = Color.IndianRed;
						if (xmlReader.MoveToAttribute("name"))
						{
							text13 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("iconPath"))
						{
							iconPath = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("articleFolderPath"))
						{
							articleFolderPath = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("public"))
						{
							isPublic = xmlReader.ReadContentAsBoolean();
						}
						if (xmlReader.MoveToAttribute("assigner"))
						{
							flag5 = xmlReader.ReadContentAsBoolean();
						}
						if (xmlReader.MoveToAttribute("title"))
						{
							text14 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("color"))
						{
							string input2 = xmlReader.ReadContentAsString();
							themeColor = Utils.convertStringToColor(input2);
						}
						MissionListingServer missionListingServer = new MissionListingServer(c, text13, iconPath, articleFolderPath, themeColor, ComputerLoader.os, isPublic, flag5);
						missionListingServer.missionAssigner = flag5;
						if (text14 != null)
						{
							missionListingServer.listingTitle = text14;
						}
						c.daemons.Add(missionListingServer);
					}
					else if (xmlReader.Name.Equals("missionHubServer"))
					{
						Color themeColorBackground;
						Color themeColor2;
						Color themeColorLine = themeColor2 = (themeColorBackground = Color.PaleTurquoise);
						string serviceName;
						string group2;
						string text15 = group2 = (serviceName = null);
						bool allowAbandon = true;
						if (xmlReader.MoveToAttribute("groupName"))
						{
							ComputerLoader.filter(group2 = xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("serviceName"))
						{
							serviceName = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("missionFolderPath"))
						{
							text15 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("themeColor"))
						{
							themeColor2 = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("lineColor"))
						{
							themeColorLine = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("backgroundColor"))
						{
							themeColorBackground = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("allowAbandon"))
						{
							allowAbandon = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						string str = "Content/Missions/";
						if (Settings.IsInExtensionMode)
						{
							str = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
						}
						text15 = text15.Replace('\\', '/');
						if (!text15.EndsWith("/"))
						{
							text15 += "/";
						}
						MissionHubServer missionHubServer = new MissionHubServer(c, serviceName, group2, ComputerLoader.os);
						missionHubServer.MissionSourceFolderPath = str + text15;
						missionHubServer.themeColor = themeColor2;
						missionHubServer.themeColorBackground = themeColorBackground;
						missionHubServer.themeColorLine = themeColorLine;
						missionHubServer.allowAbandon = allowAbandon;
						c.daemons.Add(missionHubServer);
					}
					else if (xmlReader.Name.Equals("mailServer"))
					{
						string text13 = "Mail Server";
						if (xmlReader.MoveToAttribute("name"))
						{
							text13 = xmlReader.ReadContentAsString();
						}
						bool shouldGenerateJunkEmails = true;
						if (xmlReader.MoveToAttribute("generateJunk"))
						{
							shouldGenerateJunkEmails = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						MailServer ms = new MailServer(c, text13, ComputerLoader.os);
						ms.shouldGenerateJunkEmails = shouldGenerateJunkEmails;
						if (xmlReader.MoveToAttribute("color"))
						{
							ms.setThemeColor(Utils.convertStringToColor(xmlReader.ReadContentAsString()));
						}
						while (!(xmlReader.Name == "mailServer") || xmlReader.IsStartElement())
						{
							if (xmlReader.Name == "email")
							{
								string sender = "UNKNOWN";
								string text16 = null;
								string subject = "UNKNOWN";
								if (xmlReader.MoveToAttribute("sender"))
								{
									sender = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								if (xmlReader.MoveToAttribute("recipient"))
								{
									text16 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								if (xmlReader.MoveToAttribute("subject"))
								{
									subject = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								xmlReader.MoveToContent();
								string text17 = ComputerLoader.filter(xmlReader.ReadElementContentAsString());
								if (text16 != null)
								{
									string email = MailServer.generateEmail(subject, text17, sender);
									string recp = text16;
									MailServer ms2 = ms;
									ms2.setupComplete = (Action)Delegate.Combine(ms2.setupComplete, new Action(delegate()
									{
										ms.addMail(email, recp);
									}));
								}
							}
							xmlReader.Read();
						}
						c.daemons.Add(ms);
					}
					else if (xmlReader.Name.Equals("addEmailDaemon"))
					{
						AddEmailDaemon item = new AddEmailDaemon(c, "Final Task", ComputerLoader.os);
						c.daemons.Add(item);
					}
					else if (xmlReader.Name.Equals("deathRowDatabase"))
					{
						DeathRowDatabaseDaemon item2 = new DeathRowDatabaseDaemon(c, "Death Row Database", ComputerLoader.os);
						c.daemons.Add(item2);
					}
					else if (xmlReader.Name.Equals("academicDatabase"))
					{
						AcademicDatabaseDaemon item3 = new AcademicDatabaseDaemon(c, "International Academic Database", ComputerLoader.os);
						c.daemons.Add(item3);
					}
					else if (xmlReader.Name.Equals("ispSystem"))
					{
						ISPDaemon item4 = new ISPDaemon(c, ComputerLoader.os);
						c.daemons.Add(item4);
					}
					else if (xmlReader.Name.Equals("messageBoard"))
					{
						MessageBoardDaemon messageBoardDaemon = new MessageBoardDaemon(c, ComputerLoader.os);
						string text18 = "Anonymous";
						if (xmlReader.MoveToAttribute("name"))
						{
							text18 = xmlReader.ReadContentAsString();
						}
						messageBoardDaemon.name = text18;
						messageBoardDaemon.BoardName = text18;
						while (!(xmlReader.Name == "messageBoard") || xmlReader.IsStartElement())
						{
							if (xmlReader.Name == "thread")
							{
								xmlReader.MoveToContent();
								string text19 = xmlReader.ReadElementContentAsString();
								string text20 = "Content/Missions/";
								if (text19.StartsWith(text20))
								{
									text19 = text19.Substring(text20.Length);
								}
								if (Settings.IsInExtensionMode)
								{
									text20 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
								}
								if (text19 != null)
								{
									messageBoardDaemon.AddThread(Utils.readEntireFile(text20 + text19));
								}
							}
							xmlReader.Read();
						}
						c.daemons.Add(messageBoardDaemon);
					}
					else if (xmlReader.Name.Equals("addAvconDemoEndDaemon"))
					{
						AvconDemoEndDaemon item5 = new AvconDemoEndDaemon(c, "Demo End", ComputerLoader.os);
						c.daemons.Add(item5);
					}
					else if (xmlReader.Name.Equals("addWebServer"))
					{
						string text21 = "Web Server";
						string text22 = null;
						if (xmlReader.MoveToAttribute("name"))
						{
							text21 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("url"))
						{
							text22 = xmlReader.ReadContentAsString();
						}
						WebServerDaemon webServerDaemon = new WebServerDaemon(c, text21, ComputerLoader.os, text22);
						webServerDaemon.registerAsDefaultBootDaemon();
						c.daemons.Add(webServerDaemon);
					}
					else if (xmlReader.Name.Equals("addOnlineWebServer"))
					{
						string text21 = "Web Server";
						string text22 = null;
						if (xmlReader.MoveToAttribute("name"))
						{
							text21 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("url"))
						{
							text22 = xmlReader.ReadContentAsString();
						}
						OnlineWebServerDaemon onlineWebServerDaemon = new OnlineWebServerDaemon(c, text21, ComputerLoader.os);
						if (text22 != null)
						{
							onlineWebServerDaemon.setURL(text22);
						}
						onlineWebServerDaemon.registerAsDefaultBootDaemon();
						c.daemons.Add(onlineWebServerDaemon);
					}
					else if (xmlReader.Name.Equals("uploadServerDaemon"))
					{
						string serviceName2 = "File Upload Server";
						string foldername = null;
						string input3 = "0,94,38";
						bool needsAuthentication = false;
						bool hasReturnViewButton = false;
						if (xmlReader.MoveToAttribute("name"))
						{
							serviceName2 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("folder"))
						{
							foldername = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("color"))
						{
							input3 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("needsAuth"))
						{
							needsAuthentication = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						if (xmlReader.MoveToAttribute("hasReturnViewButton"))
						{
							hasReturnViewButton = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						Color themeColor3 = Utils.convertStringToColor(input3);
						UploadServerDaemon uploadServerDaemon = new UploadServerDaemon(c, serviceName2, themeColor3, ComputerLoader.os, foldername, needsAuthentication);
						uploadServerDaemon.hasReturnViewButton = hasReturnViewButton;
						uploadServerDaemon.registerAsDefaultBootDaemon();
						c.daemons.Add(uploadServerDaemon);
					}
					else if (xmlReader.Name.Equals("MedicalDatabase"))
					{
						MedicalDatabaseDaemon item6 = new MedicalDatabaseDaemon(c, ComputerLoader.os);
						c.daemons.Add(item6);
					}
					else if (xmlReader.Name.Equals("HeartMonitor"))
					{
						string patientID = "UNKNOWN";
						if (xmlReader.MoveToAttribute("patient"))
						{
							patientID = xmlReader.ReadContentAsString();
						}
						HeartMonitorDaemon heartMonitorDaemon = new HeartMonitorDaemon(c, ComputerLoader.os);
						heartMonitorDaemon.PatientID = patientID;
						c.daemons.Add(heartMonitorDaemon);
					}
					else if (xmlReader.Name.Equals("PointClicker"))
					{
						PointClickerDaemon item7 = new PointClickerDaemon(c, "Point Clicker!", ComputerLoader.os);
						c.daemons.Add(item7);
					}
					else if (xmlReader.Name.Equals("PorthackHeart"))
					{
						PorthackHeartDaemon item8 = new PorthackHeartDaemon(c, ComputerLoader.os);
						c.daemons.Add(item8);
					}
					else if (xmlReader.Name.Equals("SongChangerDaemon"))
					{
						SongChangerDaemon item9 = new SongChangerDaemon(c, ComputerLoader.os);
						c.daemons.Add(item9);
					}
					else if (xmlReader.Name.Equals("DHSDaemon"))
					{
						ComputerLoader.DLCCheck(xmlReader.Name);
						string group3 = "UNKNOWN";
						bool addsFactionPointForMissionCompleteion = true;
						bool autoClearMissionsOnSingleComplete = true;
						bool allowContractAbbandon = false;
						Color themeColor2 = new Color(38, 201, 155);
						if (xmlReader.MoveToAttribute("groupName"))
						{
							group3 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("addsFactionPointOnMissionComplete"))
						{
							addsFactionPointForMissionCompleteion = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						if (xmlReader.MoveToAttribute("autoClearMissionsOnPlayerComplete"))
						{
							autoClearMissionsOnSingleComplete = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						if (xmlReader.MoveToAttribute("themeColor"))
						{
							themeColor2 = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("allowContractAbbandon"))
						{
							allowContractAbbandon = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						DLCHubServer dlchubServer = new DLCHubServer(c, "DHS", group3, ComputerLoader.os);
						dlchubServer.AddsFactionPointForMissionCompleteion = addsFactionPointForMissionCompleteion;
						dlchubServer.AutoClearMissionsOnSingleComplete = autoClearMissionsOnSingleComplete;
						dlchubServer.AllowContractAbbandon = allowContractAbbandon;
						dlchubServer.themeColor = themeColor2;
						while (!(xmlReader.Name == "DHSDaemon") || xmlReader.IsStartElement())
						{
							if (xmlReader.Name.ToLower() == "user" || xmlReader.Name.ToLower() == "agent")
							{
								string text23 = null;
								if (xmlReader.MoveToAttribute("name"))
								{
									text23 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								string agentPassword = "password";
								if (xmlReader.MoveToAttribute("pass"))
								{
									agentPassword = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								Color color = Color.LightGreen;
								if (xmlReader.MoveToAttribute("color"))
								{
									color = Utils.convertStringToColor(xmlReader.ReadContentAsString());
								}
								if (!string.IsNullOrWhiteSpace(text23))
								{
									dlchubServer.AddAgent(ComputerLoader.filter(text23), agentPassword, color);
								}
							}
							xmlReader.Read();
						}
						c.daemons.Add(dlchubServer);
					}
					else if (xmlReader.Name.Equals("CustomConnectDisplayDaemon"))
					{
						CustomConnectDisplayDaemon item10 = new CustomConnectDisplayDaemon(c, ComputerLoader.os);
						c.daemons.Add(item10);
					}
					else if (xmlReader.Name.Equals("DatabaseDaemon"))
					{
						ComputerLoader.DLCCheck(xmlReader.Name);
						string text24 = null;
						string foldername2 = null;
						Color? themeColor4 = null;
						if (xmlReader.MoveToAttribute("DataType"))
						{
							text24 = xmlReader.ReadContentAsString();
						}
						DatabaseDaemon.DatabasePermissions permissions = DatabaseDaemon.DatabasePermissions.Public;
						if (xmlReader.MoveToAttribute("Permissions"))
						{
							permissions = DatabaseDaemon.GetDatabasePermissionsFromString(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("Foldername"))
						{
							foldername2 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("Color"))
						{
							string input4 = xmlReader.ReadContentAsString();
							themeColor4 = new Color?(Utils.convertStringToColor(input4));
						}
						string text25 = null;
						string adminResetEmailHostID = null;
						if (xmlReader.MoveToAttribute("AdminEmailAccount"))
						{
							text25 = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("AdminEmailHostID"))
						{
							adminResetEmailHostID = xmlReader.ReadContentAsString();
						}
						string text21 = "Database";
						if (xmlReader.MoveToAttribute("Name"))
						{
							text21 = xmlReader.ReadContentAsString();
						}
						List<object> list = null;
						xmlReader.MoveToElement();
						if (!xmlReader.IsEmptyElement)
						{
							Type typeForName = ObjectSerializer.GetTypeForName(text24);
							if (typeForName != null)
							{
								list = new List<object>();
								while (!(xmlReader.Name == "DatabaseDaemon") || xmlReader.IsStartElement())
								{
									if (xmlReader.Name != null)
									{
										if (xmlReader.Name == ObjectSerializer.GetTagNameForType(typeForName))
										{
											object item11 = ObjectSerializer.DeserializeObject(xmlReader, typeForName);
											list.Add(item11);
										}
									}
									xmlReader.Read();
								}
							}
						}
						DatabaseDaemon databaseDaemon = new DatabaseDaemon(c, ComputerLoader.os, text21, permissions, text24, foldername2, themeColor4);
						if (!string.IsNullOrWhiteSpace(text25))
						{
							databaseDaemon.adminResetEmailHostID = adminResetEmailHostID;
							databaseDaemon.adminResetPassEmailAccount = text25;
						}
						databaseDaemon.Dataset = list;
						c.daemons.Add(databaseDaemon);
					}
					else if (xmlReader.Name.Equals("WhitelistAuthenticatorDaemon"))
					{
						string remoteSourceIP = null;
						if (xmlReader.MoveToAttribute("Remote"))
						{
							remoteSourceIP = xmlReader.ReadContentAsString();
						}
						bool authenticatesItself = true;
						if (xmlReader.MoveToAttribute("SelfAuthenticating"))
						{
							authenticatesItself = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						WhitelistConnectionDaemon item12 = new WhitelistConnectionDaemon(c, ComputerLoader.os)
						{
							RemoteSourceIP = remoteSourceIP,
							AuthenticatesItself = authenticatesItself
						};
						c.daemons.Add(item12);
					}
					else if (xmlReader.Name.Equals("MarkovTextDaemon"))
					{
						string name;
						string corpusLoadPath = name = null;
						if (xmlReader.MoveToAttribute("Name"))
						{
							name = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("SourceFilesContentFolder"))
						{
							corpusLoadPath = xmlReader.ReadContentAsString();
						}
						MarkovTextDaemon item13 = new MarkovTextDaemon(c, ComputerLoader.os, name, corpusLoadPath);
						c.daemons.Add(item13);
					}
					else if (xmlReader.Name.Equals("IRCDaemon"))
					{
						Color themeColor2 = new Color(184, 2, 141);
						if (xmlReader.MoveToAttribute("themeColor"))
						{
							themeColor2 = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						string name2 = "IRC Server";
						if (xmlReader.MoveToAttribute("name"))
						{
							name2 = xmlReader.ReadContentAsString();
						}
						bool requiresLogin = false;
						if (xmlReader.MoveToAttribute("needsLogin"))
						{
							requiresLogin = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						IRCDaemon ircdaemon = new IRCDaemon(c, ComputerLoader.os, name2);
						ircdaemon.ThemeColor = themeColor2;
						ircdaemon.RequiresLogin = requiresLogin;
						while (!(xmlReader.Name == "IRCDaemon") || xmlReader.IsStartElement())
						{
							if (xmlReader.Name.ToLower() == "user" || xmlReader.Name.ToLower() == "agent")
							{
								string text26 = null;
								if (xmlReader.MoveToAttribute("name"))
								{
									text26 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								Color value = Color.LightGreen;
								if (xmlReader.MoveToAttribute("color"))
								{
									value = Utils.convertStringToColor(xmlReader.ReadContentAsString());
								}
								if (!string.IsNullOrWhiteSpace(text26))
								{
									ircdaemon.UserColors.Add(ComputerLoader.filter(text26), value);
								}
							}
							if (xmlReader.Name == "post" && xmlReader.IsStartElement())
							{
								string text26 = null;
								if (xmlReader.MoveToAttribute("user"))
								{
									text26 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								}
								xmlReader.MoveToElement();
								string text27 = xmlReader.ReadElementContentAsString();
								if (!string.IsNullOrWhiteSpace(text27) && !string.IsNullOrWhiteSpace(text26))
								{
									ircdaemon.StartingMessages.Add(new KeyValuePair<string, string>(text26, ComputerLoader.filter(text27)));
								}
							}
							xmlReader.Read();
						}
						c.daemons.Add(ircdaemon);
					}
					else if (xmlReader.Name.Equals("AircraftDaemon"))
					{
						ComputerLoader.DLCCheck(xmlReader.Name);
						Vector2 zero = Vector2.Zero;
						Vector2 mapDest = Vector2.One * 0.5f;
						float progress = 0.5f;
						string name = null;
						if (xmlReader.MoveToAttribute("Name"))
						{
							name = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("OriginX"))
						{
							zero.X = xmlReader.ReadContentAsFloat();
						}
						if (xmlReader.MoveToAttribute("OriginY"))
						{
							zero.Y = xmlReader.ReadContentAsFloat();
						}
						if (xmlReader.MoveToAttribute("DestX"))
						{
							mapDest.X = xmlReader.ReadContentAsFloat();
						}
						if (xmlReader.MoveToAttribute("DestY"))
						{
							mapDest.Y = xmlReader.ReadContentAsFloat();
						}
						if (xmlReader.MoveToAttribute("Progress"))
						{
							progress = xmlReader.ReadContentAsFloat();
						}
						AircraftDaemon item14 = new AircraftDaemon(c, ComputerLoader.os, name, zero, mapDest, progress);
						c.daemons.Add(item14);
					}
					else if (xmlReader.Name.Equals("LogoCustomConnectDisplayDaemon"))
					{
						string logoImageName = null;
						string text28 = null;
						string buttonAlignment = null;
						bool logoShouldClipoverdraw = false;
						if (xmlReader.MoveToAttribute("logo"))
						{
							logoImageName = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("title"))
						{
							text28 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("overdrawLogo"))
						{
							logoShouldClipoverdraw = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						if (xmlReader.MoveToAttribute("buttonAlignment"))
						{
							buttonAlignment = xmlReader.ReadContentAsString();
						}
						LogoCustomConnectDisplayDaemon item15 = new LogoCustomConnectDisplayDaemon(c, ComputerLoader.os, logoImageName, text28, logoShouldClipoverdraw, buttonAlignment);
						c.daemons.Add(item15);
					}
					else if (xmlReader.Name.Equals("LogoDaemon"))
					{
						string logoImagePath = null;
						bool showsTitle = true;
						Color textColor = Color.White;
						if (xmlReader.MoveToAttribute("LogoImagePath"))
						{
							logoImagePath = xmlReader.ReadContentAsString();
						}
						if (xmlReader.MoveToAttribute("TextColor"))
						{
							textColor = Utils.convertStringToColor(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("Name"))
						{
							string text29 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						if (xmlReader.MoveToAttribute("ShowsTitle"))
						{
							showsTitle = (xmlReader.ReadContentAsString().ToLower() == "true");
						}
						string text17 = null;
						if (xmlReader.IsStartElement())
						{
							text17 = xmlReader.ReadElementContentAsString();
						}
						LogoDaemon logoDaemon = new LogoDaemon(c, ComputerLoader.os, text3, showsTitle, logoImagePath);
						logoDaemon.TextColor = textColor;
						logoDaemon.BodyText = text17;
						c.daemons.Add(logoDaemon);
					}
					else if (xmlReader.Name.Equals("DLCCredits") || xmlReader.Name.Equals("CreditsDaemon"))
					{
						string text30 = null;
						if (xmlReader.MoveToAttribute("ButtonText"))
						{
							text30 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						string text28 = null;
						if (xmlReader.MoveToAttribute("Title"))
						{
							text28 = ComputerLoader.filter(xmlReader.ReadContentAsString());
						}
						DLCCreditsDaemon dlccreditsDaemon;
						if (text30 != null || text28 != null)
						{
							dlccreditsDaemon = new DLCCreditsDaemon(c, ComputerLoader.os, text28, text30);
						}
						else
						{
							dlccreditsDaemon = new DLCCreditsDaemon(c, ComputerLoader.os);
						}
						if (xmlReader.MoveToAttribute("ConditionalActionSetToRunOnButtonPressPath"))
						{
							dlccreditsDaemon.ConditionalActionsToLoadOnButtonPress = xmlReader.ReadContentAsString();
						}
						c.daemons.Add(dlccreditsDaemon);
					}
					else if (xmlReader.Name.Equals("FastActionHost"))
					{
						FastActionHost item16 = new FastActionHost(c, ComputerLoader.os, text3);
						c.daemons.Add(item16);
					}
					else if (xmlReader.Name.Equals("eosDevice"))
					{
						EOSComp.AddEOSComp(xmlReader, c, ComputerLoader.os);
					}
					else if (xmlReader.Name.Equals("Memory"))
					{
						MemoryContents memoryContents = MemoryContents.Deserialize(xmlReader);
						c.Memory = memoryContents;
					}
				}
				IL_2C1A:
				xmlReader.Read();
			}
			xmlReader.Close();
			if (!preventInitDaemons)
			{
				c.initDaemons();
			}
			if (!preventAddingToNetmap)
			{
				ComputerLoader.os.netMap.nodes.Add(c);
			}
			return c;
		}

		// Token: 0x060005D9 RID: 1497 RVA: 0x0005ECD4 File Offset: 0x0005CED4
		private static void DLCCheck(string name)
		{
			if (!DLC1SessionUpgrader.HasDLC1Installed)
			{
				throw new NotSupportedException("LABYRINTHS DLC REQUIRED.\nThe tag " + name + " requires Hacknet Labyrinths to be installed.");
			}
		}

		// Token: 0x060005DA RID: 1498 RVA: 0x0005ED04 File Offset: 0x0005CF04
		public static void loadPortsIntoComputer(string portsList, object computer_obj)
		{
			Computer computer = (Computer)computer_obj;
			char[] separator = new char[]
			{
				' ',
				','
			};
			string[] array = portsList.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			computer.ports.Clear();
			computer.portsOpen.Clear();
			int i = 0;
			while (i < array.Length)
			{
				try
				{
					int item = Convert.ToInt32(array[i]);
					if (PortExploits.portNums.Contains(item))
					{
						computer.ports.Add(item);
						computer.portsOpen.Add(0);
						goto IL_12C;
					}
				}
				catch (OverflowException)
				{
				}
				catch (FormatException)
				{
				}
				goto IL_99;
				IL_12C:
				i++;
				continue;
				IL_99:
				int num = -1;
				foreach (KeyValuePair<int, string> keyValuePair in PortExploits.cracks)
				{
					if (keyValuePair.Value.ToLower().Equals(array[i].ToLower()))
					{
						num = keyValuePair.Key;
					}
				}
				if (num != -1)
				{
					computer.ports.Add(num);
					computer.portsOpen.Add(0);
				}
				goto IL_12C;
			}
		}

		// Token: 0x060005DB RID: 1499 RVA: 0x0005EF50 File Offset: 0x0005D150
		public static object readMission(string filename)
		{
			filename = LocalizedFileLoader.GetLocalizedFilepath(filename);
			Stream input = File.OpenRead(filename);
			XmlReader xmlReader = XmlReader.Create(input);
			List<MisisonGoal> list = new List<MisisonGoal>();
			int val = 0;
			string text = "";
			string text2 = "";
			int val2 = 0;
			bool activeCheck = false;
			bool shouldIgnoreSenderVerification = false;
			while (xmlReader.Name != "mission")
			{
				xmlReader.Read();
				if (xmlReader.EOF)
				{
					return null;
				}
			}
			if (xmlReader.MoveToAttribute("activeCheck"))
			{
				activeCheck = xmlReader.ReadContentAsBoolean();
			}
			if (xmlReader.MoveToAttribute("shouldIgnoreSenderVerification"))
			{
				shouldIgnoreSenderVerification = xmlReader.ReadContentAsBoolean();
			}
			while (xmlReader.Name != "goals" && xmlReader.Name != "generationKeys")
			{
				xmlReader.Read();
				if (xmlReader.EOF)
				{
					throw new FormatException("<goals> tag was not found where it was expected. It's either missing or out of order.");
				}
			}
			if (xmlReader.Name == "generationKeys")
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				while (xmlReader.MoveToNextAttribute())
				{
					string name = xmlReader.Name;
					string value = xmlReader.Value;
					dictionary.Add(name, value);
				}
				xmlReader.MoveToContent();
				string text3 = xmlReader.ReadElementContentAsString();
				if (text3 != null & text3.Length >= 1)
				{
					dictionary.Add("Data", text3);
				}
				MissionGenerator.setMissionGenerationKeys(dictionary);
				while (xmlReader.Name != "goals")
				{
					xmlReader.Read();
				}
			}
			xmlReader.Read();
			if (ComputerLoader.MissionPreLoadComplete != null)
			{
				ComputerLoader.MissionPreLoadComplete();
			}
			while (xmlReader.Name != "goals")
			{
				if (xmlReader.Name.Equals("goal"))
				{
					string text4 = "UNKNOWN";
					if (xmlReader.MoveToAttribute("type"))
					{
						text4 = xmlReader.ReadContentAsString();
					}
					try
					{
						if (text4.ToLower().Equals("filedeletion"))
						{
							string path;
							string text5;
							string filename2 = text5 = (path = "");
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("file"))
							{
								filename2 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("path"))
							{
								path = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							Computer computer = ComputerLoader.findComp(text5);
							FileDeletionMission item = new FileDeletionMission(path, filename2, (computer != null) ? computer.ip : text5, ComputerLoader.os);
							list.Add(item);
						}
						if (text4.ToLower().Equals("clearfolder"))
						{
							string text5;
							string path = text5 = "";
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("path"))
							{
								path = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							Computer computer = ComputerLoader.findComp(text5);
							FileDeleteAllMission item2 = new FileDeleteAllMission(path, computer.ip, ComputerLoader.os);
							list.Add(item2);
						}
						else if (text4.ToLower().Equals("filedownload"))
						{
							string path;
							string text5;
							string filename2 = text5 = (path = "");
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("file"))
							{
								filename2 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("path"))
							{
								path = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							FileDownloadMission item3 = new FileDownloadMission(path, filename2, text5, ComputerLoader.os);
							list.Add(item3);
						}
						else if (text4.ToLower().Equals("filechange"))
						{
							string path;
							string text5;
							string targetKeyword;
							string filename2 = text5 = (path = (targetKeyword = ""));
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("file"))
							{
								filename2 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("path"))
							{
								path = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("keyword"))
							{
								targetKeyword = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							bool isRemoval = false;
							if (xmlReader.MoveToAttribute("removal"))
							{
								isRemoval = xmlReader.ReadContentAsBoolean();
							}
							bool caseSensitive = false;
							if (xmlReader.MoveToAttribute("caseSensitive"))
							{
								caseSensitive = (xmlReader.ReadContentAsString().ToLower() == "true");
							}
							Computer computer = ComputerLoader.findComp(text5);
							if (computer != null)
							{
								list.Add(new FileChangeMission(path, filename2, computer.ip, targetKeyword, ComputerLoader.os, isRemoval)
								{
									caseSensitive = caseSensitive
								});
							}
						}
						else if (text4.ToLower().Equals("getadmin"))
						{
							string text5 = "";
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							GetAdminMission item4 = new GetAdminMission(ComputerLoader.findComp(text5).ip, ComputerLoader.os);
							list.Add(item4);
						}
						else if (text4.ToLower().Equals("getstring"))
						{
							string text5 = "";
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							GetStringMission item5 = new GetStringMission(text5);
							list.Add(item5);
						}
						else if (text4.ToLower().Equals("delay"))
						{
							float time = 1f;
							if (xmlReader.MoveToAttribute("time"))
							{
								time = xmlReader.ReadContentAsFloat();
							}
							DelayMission item6 = new DelayMission(time);
							list.Add(item6);
						}
						else if (text4.ToLower().Equals("hasflag"))
						{
							string text5 = "";
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							CheckFlagSetMission item7 = new CheckFlagSetMission(text5, ComputerLoader.os);
							list.Add(item7);
						}
						if (text4.ToLower().Equals("fileupload"))
						{
							bool needsDecrypt = false;
							string path;
							string text5;
							string decryptPass;
							string destToUploadToPath;
							string computerToUploadToIP;
							string filename2 = text5 = (path = (computerToUploadToIP = (destToUploadToPath = (decryptPass = ""))));
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("file"))
							{
								filename2 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("path"))
							{
								path = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("destTarget"))
							{
								computerToUploadToIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("destPath"))
							{
								destToUploadToPath = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("decryptPass"))
							{
								decryptPass = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("decrypt"))
							{
								needsDecrypt = xmlReader.ReadContentAsBoolean();
							}
							FileUploadMission item8 = new FileUploadMission(path, filename2, text5, computerToUploadToIP, destToUploadToPath, ComputerLoader.os, needsDecrypt, decryptPass);
							list.Add(item8);
						}
						else if (text4.ToLower().Equals("adddegree"))
						{
							string degreeName;
							string targetName;
							string uniName = targetName = (degreeName = "");
							float desiredGPA = -1f;
							if (xmlReader.MoveToAttribute("owner"))
							{
								targetName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("degree"))
							{
								degreeName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("uni"))
							{
								uniName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("gpa"))
							{
								desiredGPA = xmlReader.ReadContentAsFloat();
							}
							AddDegreeMission item9 = new AddDegreeMission(targetName, degreeName, uniName, desiredGPA, ComputerLoader.os);
							list.Add(item9);
						}
						else if (text4.ToLower().Equals("wipedegrees"))
						{
							string targetName = "";
							if (xmlReader.MoveToAttribute("owner"))
							{
								targetName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							WipeDegreesMission item10 = new WipeDegreesMission(targetName, ComputerLoader.os);
							list.Add(item10);
						}
						else if (text4.ToLower().Equals("removeDeathRowRecord".ToLower()))
						{
							string firstName = "UNKNOWN";
							string lastName = "UNKNOWN";
							if (xmlReader.MoveToAttribute("name"))
							{
								string text6 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								string[] array = text6.Split(new char[]
								{
									' '
								});
								firstName = array[0];
								lastName = array[1];
							}
							if (xmlReader.MoveToAttribute("fname"))
							{
								firstName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("lname"))
							{
								lastName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							DeathRowRecordRemovalMission item11 = new DeathRowRecordRemovalMission(firstName, lastName, ComputerLoader.os);
							list.Add(item11);
						}
						else if (text4.ToLower().Equals("modifyDeathRowRecord".ToLower()))
						{
							string firstName = "UNKNOWN";
							string lastName = "UNKNOWN";
							if (xmlReader.MoveToAttribute("name"))
							{
								string text6 = ComputerLoader.filter(xmlReader.ReadContentAsString());
								string[] array = text6.Split(new char[]
								{
									' '
								});
								firstName = array[0];
								lastName = array[1];
							}
							if (xmlReader.MoveToAttribute("fname"))
							{
								firstName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("lname"))
							{
								lastName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							xmlReader.MoveToContent();
							string lastWords = xmlReader.ReadElementContentAsString();
							DeathRowRecordModifyMission item12 = new DeathRowRecordModifyMission(firstName, lastName, lastWords, ComputerLoader.os);
							list.Add(item12);
						}
						else if (text4.ToLower().Equals("sendemail"))
						{
							string mailRecipient;
							string proposedEmailSubject = mailRecipient = "";
							string mailServerID = "jmail";
							if (xmlReader.MoveToAttribute("mailServer"))
							{
								mailServerID = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("recipient"))
							{
								mailRecipient = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("subject"))
							{
								proposedEmailSubject = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							SendEmailMission item13 = new SendEmailMission(mailServerID, mailRecipient, proposedEmailSubject, ComputerLoader.os);
							list.Add(item13);
						}
						else if (text4.ToLower().Equals("databaseentrychange"))
						{
							string recordName;
							string targetValue;
							string fieldName;
							string computerIP;
							string operation = computerIP = (fieldName = (targetValue = (recordName = null)));
							if (xmlReader.MoveToAttribute("comp"))
							{
								computerIP = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("operation"))
							{
								operation = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("fieldName"))
							{
								fieldName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("targetValue"))
							{
								targetValue = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							if (xmlReader.MoveToAttribute("recordName"))
							{
								recordName = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							DatabaseEntryChangeMission item14 = new DatabaseEntryChangeMission(computerIP, ComputerLoader.os, operation, fieldName, targetValue, recordName);
							list.Add(item14);
						}
						else if (text4.ToLower().Equals("getadminpasswordstring"))
						{
							string text5 = "";
							if (xmlReader.MoveToAttribute("target"))
							{
								text5 = ComputerLoader.filter(xmlReader.ReadContentAsString());
							}
							GetAdminPasswordStringMission item15 = new GetAdminPasswordStringMission(ComputerLoader.findComp(text5).ip, ComputerLoader.os);
							list.Add(item15);
						}
					}
					catch (Exception ex)
					{
						if (ex is NullReferenceException)
						{
							throw new FormatException("Error loading mission Goal \"" + text4 + "\"\r\nNullReferenceException - this means something referenced by an ID (probably a computer) or a filename (missions/scripts etc) was not found.");
						}
						throw new FormatException("Error loading mission Goal \"" + text4 + "\"", ex);
					}
				}
				xmlReader.Read();
			}
			while (xmlReader.Name != "nextMission" && xmlReader.Name != "missionEnd" && xmlReader.Name != "missionStart" && !xmlReader.EOF)
			{
				xmlReader.Read();
			}
			if (xmlReader.EOF)
			{
				throw new FormatException("Unexpected end of file looking for nextMission tag in computer.\nYour tags might be out of order!\n");
			}
			if (xmlReader.Name.Equals("missionStart"))
			{
				int num = 1;
				bool flag = Settings.IsInExtensionMode;
				if (xmlReader.MoveToAttribute("val"))
				{
					num = xmlReader.ReadContentAsInt();
				}
				if (xmlReader.MoveToAttribute("suppress"))
				{
					flag = xmlReader.ReadContentAsBoolean();
				}
				xmlReader.MoveToContent();
				string text7 = xmlReader.ReadElementContentAsString();
				if (flag)
				{
					text2 = text7;
					val2 = num;
				}
				else
				{
					try
					{
						MissionFunctions.runCommand(num, text7);
					}
					catch (Exception ex)
					{
						Utils.AppendToErrorFile("Mission Start function exception!\r\n" + Utils.GenerateReportFromException(ex));
					}
				}
				xmlReader.Read();
			}
			while (xmlReader.NodeType != XmlNodeType.Element)
			{
				xmlReader.Read();
			}
			if (xmlReader.Name.Equals("missionEnd"))
			{
				int num = 1;
				if (xmlReader.MoveToAttribute("val"))
				{
					num = xmlReader.ReadContentAsInt();
				}
				xmlReader.MoveToContent();
				text = xmlReader.ReadElementContentAsString();
				val = num;
			}
			bool willSendEmail = true;
			while (!xmlReader.Name.Equals("nextMission"))
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("Could not find \"nextMission\" tag in mission file! This tag needs to exist!");
				}
				xmlReader.Read();
			}
			if (xmlReader.MoveToAttribute("IsSilent"))
			{
				string text7 = xmlReader.ReadContentAsString().ToLower();
				willSendEmail = text7.Equals("false");
			}
			xmlReader.MoveToContent();
			string next = xmlReader.ReadElementContentAsString();
			if (ComputerLoader.os.branchMissions != null)
			{
				ComputerLoader.os.branchMissions.Clear();
			}
			while (xmlReader.Name != "posting" && xmlReader.Name != "email")
			{
				if (xmlReader.Name.Equals("branchMissions"))
				{
					xmlReader.Read();
					List<ActiveMission> list2 = new List<ActiveMission>();
					while (!xmlReader.Name.Equals("branchMissions") || xmlReader.IsStartElement())
					{
						if (xmlReader.Name == "branch")
						{
							xmlReader.MoveToContent();
							string str = xmlReader.ReadElementContentAsString();
							string str2 = "Content/Missions/";
							if (Settings.IsInExtensionMode)
							{
								str2 = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/";
							}
							list2.Add((ActiveMission)ComputerLoader.readMission(str2 + str));
						}
						xmlReader.Read();
					}
					ComputerLoader.os.branchMissions = list2;
				}
				xmlReader.Read();
				if (xmlReader.EOF)
				{
					throw new FormatException("email tag not found where it was expected! You may have tags out of order.");
				}
			}
			int requiredRank;
			int difficulty = requiredRank = 0;
			string target;
			string client;
			string text9;
			string text8 = text9 = (client = (target = "UNKNOWN"));
			string text10 = null;
			while (xmlReader.Name != "posting" && xmlReader.Name != "email")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("email tag not found where it was expected! You may have tags out of order.");
				}
				xmlReader.Read();
			}
			if (xmlReader.Name.Equals("posting"))
			{
				if (xmlReader.MoveToAttribute("title"))
				{
					text8 = xmlReader.ReadContentAsString();
				}
				if (xmlReader.MoveToAttribute("reqs"))
				{
					text10 = xmlReader.ReadContentAsString();
				}
				if (xmlReader.MoveToAttribute("requiredRank"))
				{
					requiredRank = xmlReader.ReadContentAsInt();
				}
				if (xmlReader.MoveToAttribute("difficulty"))
				{
					difficulty = xmlReader.ReadContentAsInt();
				}
				if (xmlReader.MoveToAttribute("client"))
				{
					client = xmlReader.ReadContentAsString();
				}
				if (xmlReader.MoveToAttribute("target"))
				{
					target = xmlReader.ReadContentAsString();
				}
				xmlReader.MoveToContent();
				text9 = xmlReader.ReadElementContentAsString();
				text8 = ComputerLoader.filter(text8);
				text9 = ComputerLoader.filter(text9);
			}
			while (xmlReader.Name != "email")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("email tag was not found!");
				}
				xmlReader.Read();
			}
			while (xmlReader.Name != "sender")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("sender tag was not found!");
				}
				xmlReader.Read();
			}
			string text11 = xmlReader.ReadElementContentAsString();
			while (xmlReader.Name != "subject")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("subject tag was not found!");
				}
				xmlReader.Read();
			}
			string text12 = xmlReader.ReadElementContentAsString();
			while (xmlReader.Name != "body")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("body tag was not found!");
				}
				xmlReader.Read();
			}
			string text13 = xmlReader.ReadElementContentAsString();
			text13.Trim();
			text13 = ComputerLoader.filter(text13);
			text12 = ComputerLoader.filter(text12);
			text11 = ComputerLoader.filter(text11);
			while (xmlReader.Name != "attachments")
			{
				if (xmlReader.EOF)
				{
					throw new FormatException("attachments tag was not found! A mission must have an attachments tag even if it contains nothing.");
				}
				xmlReader.Read();
			}
			xmlReader.Read();
			List<string> attachments = new List<string>();
			while (xmlReader.Name != "attachments")
			{
				if (xmlReader.Name.Equals("link"))
				{
					string compname = "";
					if (xmlReader.MoveToAttribute("comp"))
					{
						compname = ComputerLoader.filter(xmlReader.ReadContentAsString());
					}
					Computer c = null;
					for (int i = 0; i < ComputerLoader.os.netMap.nodes.Count; i++)
					{
						if (ComputerLoader.os.netMap.nodes[i].idName.Equals(compname))
						{
							c = ComputerLoader.os.netMap.nodes[i];
						}
					}
					if (c != null)
					{
						attachments.Add("link#%#" + c.name + "#%#" + c.ip);
					}
					else
					{
						ComputerLoader.postAllLoadedActions = (Action)Delegate.Combine(ComputerLoader.postAllLoadedActions, new Action(delegate()
						{
							for (int j = 0; j < ComputerLoader.os.netMap.nodes.Count; j++)
							{
								if (ComputerLoader.os.netMap.nodes[j].idName.Equals(compname))
								{
									c = ComputerLoader.os.netMap.nodes[j];
								}
							}
							if (c != null)
							{
								attachments.Add("link#%#" + c.name + "#%#" + c.ip);
							}
						}));
					}
				}
				if (xmlReader.Name.Equals("account"))
				{
					string value2 = "";
					if (xmlReader.MoveToAttribute("comp"))
					{
						value2 = ComputerLoader.filter(xmlReader.ReadContentAsString());
					}
					Computer computer2 = null;
					for (int i = 0; i < ComputerLoader.os.netMap.nodes.Count; i++)
					{
						if (ComputerLoader.os.netMap.nodes[i].idName.Equals(value2))
						{
							computer2 = ComputerLoader.os.netMap.nodes[i];
						}
					}
					string text15;
					string text14 = text15 = "UNKNOWN";
					if (xmlReader.MoveToAttribute("user"))
					{
						text15 = xmlReader.ReadContentAsString();
					}
					if (xmlReader.MoveToAttribute("pass"))
					{
						text14 = xmlReader.ReadContentAsString();
					}
					text15 = ComputerLoader.filter(text15);
					text14 = ComputerLoader.filter(text14);
					if (computer2 != null)
					{
						attachments.Add(string.Concat(new string[]
						{
							"account#%#",
							computer2.name,
							"#%#",
							computer2.ip,
							"#%#",
							text15,
							"#%#",
							text14
						}));
					}
				}
				if (xmlReader.Name.Equals("note"))
				{
					string str3 = "Data";
					if (xmlReader.MoveToAttribute("title"))
					{
						str3 = ComputerLoader.filter(xmlReader.ReadContentAsString());
					}
					xmlReader.MoveToContent();
					string text16 = xmlReader.ReadElementContentAsString();
					text16 = ComputerLoader.filter(text16);
					attachments.Add("note#%#" + str3 + "#%#" + text16);
				}
				xmlReader.Read();
				if (xmlReader.EOF)
				{
					throw new FormatException("attachments tag not found where it was expected! You may have tags out of order.");
				}
			}
			MailServer.EMailData email = new MailServer.EMailData(text11, text13, text12, attachments);
			ActiveMission activeMission = new ActiveMission(list, next, email);
			activeMission.activeCheck = activeCheck;
			activeMission.ShouldIgnoreSenderVerification = shouldIgnoreSenderVerification;
			activeMission.postingBody = text9;
			activeMission.postingTitle = text8;
			activeMission.requiredRank = requiredRank;
			activeMission.difficulty = difficulty;
			activeMission.client = client;
			activeMission.target = target;
			activeMission.reloadGoalsSourceFile = filename;
			if (text10 != null)
			{
				activeMission.postingAcceptFlagRequirements = text10.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
			}
			activeMission.willSendEmail = willSendEmail;
			if (!text.Equals(""))
			{
				activeMission.addEndFunction(val, text);
			}
			if (!text2.Equals(""))
			{
				activeMission.addStartFunction(val2, text2);
			}
			xmlReader.Close();
			return activeMission;
		}

		// Token: 0x060005DC RID: 1500 RVA: 0x000607F8 File Offset: 0x0005E9F8
		public static void loadMission(string filename, bool PreventEmail = false)
		{
			ActiveMission activeMission = (ActiveMission)ComputerLoader.readMission(filename);
			ComputerLoader.os.currentMission = activeMission;
			activeMission.sendEmail(ComputerLoader.os);
		}

		// Token: 0x060005DD RID: 1501 RVA: 0x0006082C File Offset: 0x0005EA2C
		public static string filter(string s)
		{
			string text = LocalizedFileLoader.FilterStringForLocalization(MissionGenerationParser.parse(s.Replace("#BINARY#", Computer.generateBinaryString(2000)).Replace("#BINARYSMALL#", Computer.generateBinaryString(800)).Replace("#PLAYERNAME#", ComputerLoader.os.defaultUser.name).Replace("#PLAYER_IP#", ComputerLoader.os.thisComputer.ip).Replace("#PLAYER_ACCOUNT_PASSWORD#", SaveFileManager.LastLoggedInUser.Password).Replace("#RANDOM_IP#", NetworkMap.generateRandomIP()).Replace("#SSH_CRACK#", PortExploits.crackExeData[22]).Replace("#FTP_CRACK#", PortExploits.crackExeData[21]).Replace("#WEB_CRACK#", PortExploits.crackExeData[80]).Replace("#DECYPHER_PROGRAM#", PortExploits.crackExeData[9]).Replace("#DECHEAD_PROGRAM#", PortExploits.crackExeData[10]).Replace("#CLOCK_PROGRAM#", PortExploits.crackExeData[11]).Replace("#MEDICAL_PROGRAM#", PortExploits.crackExeData[104]).Replace("#SMTP_CRACK#", PortExploits.crackExeData[25]).Replace("#SQL_CRACK#", PortExploits.crackExeData[1433]).Replace("#SECURITYTRACER_PROGRAM#", PortExploits.crackExeData[4]).Replace("#HACKNET_EXE#", PortExploits.crackExeData[15]).Replace("#HEXCLOCK_EXE#", PortExploits.crackExeData[16]).Replace("#SEQUENCER_EXE#", PortExploits.crackExeData[17]).Replace("#THEMECHANGER_EXE#", PortExploits.crackExeData[14]).Replace("#EOS_SCANNER_EXE#", PortExploits.crackExeData[13]).Replace("#TRACEKILL_EXE#", PortExploits.crackExeData[12]).Replace("#GREEN_THEME#", ThemeManager.getThemeDataString(OSTheme.HackerGreen)).Replace("#WHITE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetWhite)).Replace("#YELLOW_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetYellow)).Replace("#TEAL_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetTeal)).Replace("#BASE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetBlue)).Replace("#PURPLE_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetPurple)).Replace("#MINT_THEME#", ThemeManager.getThemeDataString(OSTheme.HacknetMint)).Replace("#PACEMAKER_FW_WORKING#", PortExploits.ValidPacemakerFirmware).Replace("#PACEMAKER_FW_DANGER#", PortExploits.DangerousPacemakerFirmware).Replace("#RTSP_EXE#", PortExploits.crackExeData[554]).Replace("#EXT_SEQUENCER_EXE#", PortExploits.crackExeData[40]).Replace("#SHELL_OPENER_EXE#", PortExploits.crackExeData[41]).Replace("#FTP_FAST_EXE#", PortExploits.crackExeData[211]).Replace("#EXTENSION_FOLDER_PATH#", (ExtensionLoader.ActiveExtensionInfo != null) ? ExtensionLoader.ActiveExtensionInfo.GetFullFolderPath().Replace("/Content", "/ Content") : "ERROR GETTING PATH").Replace("#PLAYERLOCATION#", "UNKNOWN").Replace("\t", "    ")));
			if (DLC1SessionUpgrader.HasDLC1Installed)
			{
				text = text.Replace("#TORRENT_EXE#", PortExploits.crackExeData[6881]).Replace("#SSL_EXE#", PortExploits.crackExeData[443]).Replace("#KAGUYA_EXE#", PortExploits.crackExeData[31]).Replace("#SIGNAL_SCRAMBLER_EXE#", PortExploits.crackExeData[32]).Replace("#MEM_FORENSICS_EXE#", PortExploits.crackExeData[33]).Replace("#MEM_DUMP_GENERATOR#", PortExploits.crackExeData[34]).Replace("#PACIFIC_EXE#", PortExploits.crackExeData[192]).Replace("#NETMAP_ORGANIZER_EXE#", PortExploits.crackExeData[35]).Replace("#SHELL_CONTROLLER_EXE#", PortExploits.crackExeData[36]).Replace("#NOTES_DUMPER_EXE#", PortExploits.crackExeData[37]).Replace("#CLOCK_V2_EXE#", PortExploits.crackExeData[38]).Replace("#DLC_MUSIC_EXE#", PortExploits.crackExeData[39]).Replace("#GIBSON_IP#", ComputerLoader.os.GibsonIP);
			}
			return text;
		}

		// Token: 0x060005DE RID: 1502 RVA: 0x00060CA0 File Offset: 0x0005EEA0
		private static Computer findComp(string target)
		{
			for (int i = 0; i < ComputerLoader.os.netMap.nodes.Count; i++)
			{
				if (ComputerLoader.os.netMap.nodes[i].idName.Equals(target))
				{
					return ComputerLoader.os.netMap.nodes[i];
				}
			}
			return null;
		}

		// Token: 0x060005DF RID: 1503 RVA: 0x00060D18 File Offset: 0x0005EF18
		public static object findComputer(string target)
		{
			return ComputerLoader.findComp(target);
		}

		// Token: 0x0400068A RID: 1674
		public static Action MissionPreLoadComplete;

		// Token: 0x0400068B RID: 1675
		public static Action postAllLoadedActions;

		// Token: 0x0400068C RID: 1676
		private static OS os;
	}
}
