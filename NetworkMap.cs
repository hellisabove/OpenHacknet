using System;
using System.Collections.Generic;
using System.Xml;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000142 RID: 322
	internal class NetworkMap : CoreModule
	{
		// Token: 0x060007B8 RID: 1976 RVA: 0x0007E6DD File Offset: 0x0007C8DD
		public NetworkMap(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0007E71C File Offset: 0x0007C91C
		public override void LoadContent()
		{
			this.label = "Network Map";
			this.visibleNodes = new List<int>();
			if (OS.WillLoadSave || Settings.IsInExtensionMode)
			{
				this.nodes = new List<Computer>();
				this.corporations = new List<Corporation>();
			}
			else if (this.os.multiplayer)
			{
				this.nodes = this.generateNetwork(this.os);
				this.corporations = new List<Corporation>();
			}
			else
			{
				this.nodes = new List<Computer>();
				List<Computer> collection = this.generateGameNodes();
				this.nodes.Clear();
				this.nodes.AddRange(collection);
				if (Settings.isDemoMode)
				{
					this.nodes.AddRange(this.generateDemoNodes());
				}
				this.nodes.Insert(0, this.generateSPNetwork(this.os)[0]);
				this.corporations = this.generateCorporations();
			}
			this.nodeEffect = new ConnectedNodeEffect(this.os);
			this.adminNodeEffect = new ConnectedNodeEffect(this.os);
			this.adminNodeEffect.color = new Color(60, 65, 75, 19);
			this.circle = TextureBank.load("Circle", this.os.content);
			this.nodeCircle = TextureBank.load("NodeCircle", this.os.content);
			this.adminNodeCircle = TextureBank.load("AdminNodeCircle", this.os.content);
			this.homeNodeCircle = TextureBank.load("HomeNodeCircle", this.os.content);
			this.targetNodeCircle = TextureBank.load("TargetNodeCircle", this.os.content);
			this.assetServerNodeOverlay = TextureBank.load("AssetServerNodeOverlay", this.os.content);
			this.circleOutline = TextureBank.load("CircleOutline", this.os.content);
			this.adminCircle = TextureBank.load("AdminCircle", this.os.content);
			this.nodeGlow = TextureBank.load("RadialGradient", this.os.content);
			this.circleOrigin = new Vector2((float)(this.circleOutline.Width / 2), (float)(this.circleOutline.Height / 2));
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0007E974 File Offset: 0x0007CB74
		public override void Update(float t)
		{
			this.rotation += t / 2f;
			if (this.pulseFade > 0f)
			{
				this.pulseFade -= t * NetworkMap.PULSE_DECAY;
			}
			else
			{
				this.pulseTimer -= t;
				if (this.pulseTimer <= 0f)
				{
					this.pulseFade = 1f;
					this.pulseTimer = NetworkMap.PULSE_FREQUENCY;
				}
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (this.nodes[i].disabled)
				{
					this.nodes[i].bootupTick(t);
				}
			}
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0007EA44 File Offset: 0x0007CC44
		public override void Draw(float t)
		{
			base.Draw(t);
			this.doGui(t);
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0007EA58 File Offset: 0x0007CC58
		public void CleanVisibleListofDuplicates()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < this.visibleNodes.Count; i++)
			{
				if (!list.Contains(this.visibleNodes[i]))
				{
					list.Add(this.visibleNodes[i]);
				}
			}
			this.visibleNodes = list;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0007EAB8 File Offset: 0x0007CCB8
		public string getSaveString()
		{
			string str = string.Format("<NetworkMap sort=\"{0}\" >\n", this.SortingAlgorithm);
			str = str + this.getVisibleNodesString() + "\n";
			str += "<network>\n";
			for (int i = 0; i < this.nodes.Count; i++)
			{
				str += this.nodes[i].getSaveString();
			}
			str += "</network>\n";
			return str + "</NetworkMap>";
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0007EB4C File Offset: 0x0007CD4C
		public string getVisibleNodesString()
		{
			string text = "<visible>";
			for (int i = 0; i < this.visibleNodes.Count; i++)
			{
				text = text + this.visibleNodes[i] + ((i != this.visibleNodes.Count - 1) ? " " : "");
			}
			return text + "</visible>";
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0007EBC4 File Offset: 0x0007CDC4
		public void load(XmlReader reader)
		{
			this.nodes.Clear();
			while (reader.Name != "NetworkMap")
			{
				reader.Read();
			}
			if (reader.MoveToAttribute("sort"))
			{
				string text = reader.ReadContentAsString();
				if (!Enum.TryParse<NetmapSortingAlgorithm>(text, out this.SortingAlgorithm))
				{
					Console.WriteLine("Error parsing netmap sorting algorithm: " + text);
					Utils.AppendToErrorFile("Error parsing netmap sorting algorithm: " + text);
				}
			}
			while (reader.Name != "visible")
			{
				reader.Read();
			}
			string text2 = reader.ReadElementContentAsString();
			string[] array = text2.Split(new char[0]);
			for (int i = 0; i < array.Length; i++)
			{
				this.visibleNodes.Add(Convert.ToInt32(array[i]));
			}
			while (reader.Name != "network")
			{
				reader.Read();
			}
			reader.Read();
			while (reader.Name != "network")
			{
				while (reader.Name == "computer" && reader.NodeType != XmlNodeType.EndElement)
				{
					this.nodes.Add(Computer.load(reader, this.os));
				}
				while ((!(reader.Name == "computer") || reader.NodeType == XmlNodeType.EndElement) && reader.Name != "network")
				{
					reader.Read();
				}
			}
			for (int i = 0; i < this.nodes.Count; i++)
			{
				Computer computer = this.nodes[i];
				for (int j = 0; j < computer.daemons.Count; j++)
				{
					computer.daemons[j].loadInit();
				}
			}
			this.loadAssignGameNodes();
			Console.WriteLine("Done loading");
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0007EDEA File Offset: 0x0007CFEA
		private void loadAssignGameNodes()
		{
			this.mailServer = Programs.getComputer(this.os, "jmail");
			this.academicDatabase = Programs.getComputer(this.os, "academic");
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0007EE1C File Offset: 0x0007D01C
		public List<Corporation> generateCorporations()
		{
			List<Corporation> list = new List<Corporation>();
			int num = 0;
			for (int i = 0; i < num; i++)
			{
				list.Add(new Corporation(this.os));
			}
			return list;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0007EE60 File Offset: 0x0007D060
		public List<Computer> generateNetwork(OS os)
		{
			List<Computer> list = new List<Computer>();
			List<Computer> list2 = new List<Computer>();
			List<Computer> list3 = new List<Computer>();
			int num = 2;
			float num2 = 0.5f;
			int num3 = 4;
			int num4 = 0;
			float num5 = (float)num;
			bool flag = false;
			float num6 = (float)((this.bounds.Width - 40) / (num3 * 2 + 3));
			float num7 = (float)(this.bounds.Height - 30);
			float y = 10f;
			Vector2 compLocation = new Vector2(num6, y);
			while (num4 >= 0 || !flag)
			{
				float num8 = num7 / (num5 + 1f);
				compLocation.Y = num8;
				for (int i = 0; i < (int)num5; i++)
				{
					Computer computer = new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), compLocation, num4, Utils.flipCoin() ? 1 : 2, os);
					Utils.random.NextDouble();
					bool flag2 = 1 == 0;
					int index = Math.Min(Math.Max(num4, 0), PortExploits.services.Count - 1);
					computer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[PortExploits.portNums[index]], PortExploits.cracks[PortExploits.portNums[index]]));
					list3.Add(computer);
					list.Add(computer);
					compLocation.Y += num8;
				}
				for (int i = 0; i < list2.Count; i++)
				{
					bool flag3 = i - 1 >= 0 && i < list2.Count - 1;
					bool flag4 = i < list3.Count && i < list2.Count;
					bool flag5 = i + 1 < list3.Count && i + 1 < list2.Count - 1;
					if (flag3)
					{
						list3[i - 1].links.Add(list.IndexOf(list2[i]));
						list2[i - 1].links.Add(list.IndexOf(list3[i]));
					}
					if (flag4)
					{
						list3[i].links.Add(list.IndexOf(list2[i]));
						list2[i].links.Add(list.IndexOf(list3[i]));
					}
					if (flag5)
					{
						list3[i + 1].links.Add(list.IndexOf(list2[i]));
						list2[i + 1].links.Add(list.IndexOf(list3[i]));
					}
				}
				list2.Clear();
				for (int i = 0; i < list3.Count; i++)
				{
					list2.Add(list3[i]);
				}
				list3.Clear();
				compLocation.X += num6;
				if (flag)
				{
					num4--;
					num5 -= num2;
				}
				else
				{
					num4++;
					num5 += num2;
				}
				if (num4 > num3)
				{
					num4--;
					flag = true;
					num5 += num2;
				}
			}
			if (!os.multiplayer)
			{
				list.AddRange(this.generateGameNodes());
			}
			if (Settings.isDemoMode)
			{
				list.AddRange(this.generateDemoNodes());
			}
			return list;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0007F240 File Offset: 0x0007D440
		public List<Computer> generateSPNetwork(OS os)
		{
			List<Computer> list = new List<Computer>();
			Computer computer = new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), this.getRandomPosition(), 0, 2, os);
			computer.idName = "firstGeneratedNode";
			Folder folder = computer.files.root.searchForFolder("bin");
			FileEntry item = new FileEntry(Utils.readEntireFile("Content/files/config.txt"), "config.txt");
			folder.files.Add(item);
			ThemeManager.setThemeOnComputer(computer, OSTheme.HackerGreen);
			list.Add(computer);
			if (!os.IsDLCConventionDemo)
			{
				os.thisComputer.files.root.folders[2].files.Add(new FileEntry(PortExploits.crackExeData[4], "SecurityTracer.exe"));
			}
			return list;
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x0007F368 File Offset: 0x0007D568
		public List<Computer> generateGameNodes()
		{
			List<Computer> list = new List<Computer>();
			Computer computer = (Computer)ComputerLoader.loadComputer("Content/Missions/CoreServers/JMailServer.xml", false, false);
			computer.location = new Vector2(0.7f, 0.2f);
			this.mailServer = computer;
			list.Add(computer);
			Computer computer2 = new Computer("boatmail.com", "65.55.72.183", new Vector2(0.6f, 0.9f), 4, 3, this.os);
			computer2.idName = "boatmail";
			computer2.daemons.Add(new BoatMail(computer2, "Boatmail", this.os));
			computer2.initDaemons();
			list.Add(computer2);
			Computer computer3 = (Computer)ComputerLoader.loadComputer("Content/Missions/CoreServers/InternationalAcademicDatabase.xml", false, false);
			AcademicDatabaseDaemon item = new AcademicDatabaseDaemon(computer3, "Academic Database", this.os);
			computer3.daemons.Add(item);
			computer3.initDaemons();
			this.academicDatabase = computer3;
			list.Add(computer3);
			Computer computer4 = (Computer)ComputerLoader.loadComputer("Content/Missions/CoreServers/ContractHubAssetsComp.xml", false, false);
			list.Add(computer4);
			Computer ch = (Computer)ComputerLoader.loadComputer("Content/Missions/CoreServers/ContractHubComp.xml", false, false);
			this.os.delayer.Post(ActionDelayer.NextTick(), delegate
			{
				MissionHubServer item2 = new MissionHubServer(ch, "CSEC Contract Database", "CSEC", this.os);
				ch.daemons.Add(item2);
				ch.initDaemons();
			});
			list.Add(ch);
			computer4.location = ch.location + Corporation.getNearbyNodeOffset(ch.location, 1, 1, this, 0f, false);
			list.Add(new Computer("Cheater's Stash", "1337.1337.1337.1337", this.getRandomPosition(), 0, 2, this.os)
			{
				idName = "haxServer",
				files = 
				{
					root = 
					{
						files = 
						{
							new FileEntry(PortExploits.crackExeData[PortExploits.portNums[0]], PortExploits.cracks[PortExploits.portNums[0]]),
							new FileEntry(PortExploits.crackExeData[PortExploits.portNums[1]], PortExploits.cracks[PortExploits.portNums[1]]),
							new FileEntry(PortExploits.crackExeData[PortExploits.portNums[2]], PortExploits.cracks[PortExploits.portNums[2]]),
							new FileEntry(PortExploits.crackExeData[PortExploits.portNums[3]], PortExploits.cracks[PortExploits.portNums[3]]),
							this.GetProgramForNum(1433),
							this.GetProgramForNum(104),
							this.GetProgramForNum(9),
							this.GetProgramForNum(13),
							this.GetProgramForNum(10)
						}
					}
				}
			});
			return list;
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0007F6F0 File Offset: 0x0007D8F0
		private FileEntry GetProgramForNum(int num)
		{
			return new FileEntry(PortExploits.crackExeData[num], PortExploits.cracks[num]);
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0007F720 File Offset: 0x0007D920
		public List<Computer> generateDemoNodes()
		{
			return new List<Computer>
			{
				new Computer("AvCon Hatland Demo PC", "192.168.1.3", this.getRandomPosition(), 1, 2, this.os)
				{
					idName = "avcon1",
					externalCounterpart = new ExternalCounterpart("avcon1", ExternalCounterpart.getIPForServerName("avconServer"))
				}
			};
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x0007F784 File Offset: 0x0007D984
		public void discoverNode(Computer c)
		{
			if (!this.visibleNodes.Contains(this.nodes.IndexOf(c)))
			{
				this.visibleNodes.Add(this.nodes.IndexOf(c));
			}
			c.highlightFlashTime = 1f;
			this.lastAddedNode = c;
		}

		// Token: 0x060007C8 RID: 1992 RVA: 0x0007F7DC File Offset: 0x0007D9DC
		public void discoverNode(string cName)
		{
			for (int i = 0; i < this.nodes.Count; i++)
			{
				if (this.nodes[i].idName.Equals(cName))
				{
					this.discoverNode(this.nodes[i]);
					break;
				}
			}
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x0007F83C File Offset: 0x0007DA3C
		public Vector2 getRandomPosition()
		{
			for (int i = 0; i < 50; i++)
			{
				Vector2 vector = this.generatePos();
				if (!this.collides(vector, -1f))
				{
					return vector;
				}
			}
			return this.generatePos();
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x0007F884 File Offset: 0x0007DA84
		private Vector2 generatePos()
		{
			float num = (float)NetworkMap.NODE_SIZE;
			return new Vector2((float)Utils.random.NextDouble(), (float)Utils.random.NextDouble());
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0007F8B8 File Offset: 0x0007DAB8
		public bool collides(Vector2 location, float minSeperation = -1f)
		{
			bool result;
			if (this.nodes == null)
			{
				result = false;
			}
			else
			{
				float num = 0.075f;
				if (minSeperation > 0f)
				{
					num = minSeperation;
				}
				for (int i = 0; i < this.nodes.Count; i++)
				{
					if (Vector2.Distance(location, this.nodes[i].location) <= num)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x060007CC RID: 1996 RVA: 0x0007F938 File Offset: 0x0007DB38
		public void randomizeNetwork()
		{
			for (int i = 0; i < 10; i++)
			{
				Vector2 compLocation = new Vector2((float)(Utils.random.NextDouble() * (double)this.bounds.Width), (float)(Utils.random.NextDouble() * (double)this.bounds.Height));
				this.nodes.Add(new Computer(NameGenerator.generateName(), NetworkMap.generateRandomIP(), compLocation, Utils.random.Next(0, 4), Utils.randomCompType(), this.os));
			}
			int num = 2;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				int num2 = num + Utils.random.Next(0, 2);
				for (int j = 0; j < num2; j++)
				{
					this.nodes[i].links.Add(Utils.random.Next(0, this.nodes.Count - 1));
				}
			}
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0007FB24 File Offset: 0x0007DD24
		public void doGui(float t)
		{
			int num = -1;
			Color color = this.os.highlightColor;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				Vector2 nodeDrawPos = this.GetNodeDrawPos(this.nodes[i], i);
				if (this.visibleNodes.Contains(i) && !this.nodes[i].disabled)
				{
					for (int j = 0; j < this.nodes[i].links.Count; j++)
					{
						if (this.visibleNodes.Contains(this.nodes[i].links[j]) && !this.nodes[this.nodes[i].links[j]].disabled)
						{
							this.drawLine(this.GetNodeDrawPos(this.nodes[i], i), this.GetNodeDrawPos(this.nodes[this.nodes[i].links[j]], this.nodes[i].links[j]), new Vector2((float)this.bounds.X, (float)this.bounds.Y));
						}
					}
					if (this.pulseFade > 0f)
					{
						Color color2 = this.os.highlightColor * (this.pulseFade * this.pulseFade);
						if (this.DimNonConnectedNodes)
						{
							color2 = Utils.AddativeRed * 0.5f * (this.pulseFade * this.pulseFade);
						}
						this.spriteBatch.Draw(this.circleOutline, new Vector2((float)(this.bounds.X + (int)nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float)(this.bounds.Y + (int)nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), null, color2, this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE * (2f - 2f * this.pulseFade)), SpriteEffects.None, 0.5f);
					}
					if (this.nodes[i].idName == this.os.homeNodeID && !this.nodes[i].disabled)
					{
						this.spriteBatch.Draw(this.homeNodeCircle, new Vector2((float)(this.bounds.X + (int)nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float)(this.bounds.Y + (int)nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), null, (this.os.connectedComp != null && this.os.connectedComp == this.nodes[i]) ? ((this.nodes[i].adminIP == this.os.thisComputer.ip) ? Utils.AddativeWhite : Utils.AddativeWhite) : (this.nodes[i].Equals(this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor), -1f * this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE), SpriteEffects.None, 0.5f);
					}
					if (this.nodes[i] == this.lastAddedNode && !this.nodes[i].disabled)
					{
						float num2 = (this.nodes[i].adminIP == this.os.thisComputer.ip) ? 1f : 4f;
						this.spriteBatch.Draw(this.targetNodeCircle, new Vector2((float)(this.bounds.X + (int)nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float)(this.bounds.Y + (int)nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), null, (this.os.connectedComp != null && this.os.connectedComp == this.nodes[i]) ? ((this.nodes[i].adminIP == this.os.thisComputer.ip) ? this.os.highlightColor : Utils.AddativeWhite) : (this.nodes[i].Equals(this.os.thisComputer) ? this.os.thisComputerNode : ((this.nodes[i].adminIP == this.os.thisComputer.ip) ? this.os.highlightColor : Utils.AddativeWhite)), -1f * this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE) + Vector2.One * (float)(Math.Sin((double)this.os.timer * 3.0) * (double)num2) / (float)this.targetNodeCircle.Height, SpriteEffects.None, 0.5f);
					}
					if (this.nodes[i].adminIP == this.os.thisComputer.ip && !this.nodes[i].disabled)
					{
						this.spriteBatch.Draw(this.nodes[i].ip.Equals(this.os.thisComputer.ip) ? this.adminCircle : this.nodeGlow, new Vector2((float)(this.bounds.X + (int)nodeDrawPos.X + NetworkMap.NODE_SIZE / 2), (float)(this.bounds.Y + (int)nodeDrawPos.Y + NetworkMap.NODE_SIZE / 2)), null, this.nodes[i].Equals(this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor, this.rotation, this.circleOrigin, new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE), SpriteEffects.None, 0.5f);
					}
				}
			}
			lock (this.nodes)
			{
				for (int i = 0; i < this.nodes.Count; i++)
				{
					if (this.visibleNodes.Contains(i) && !this.nodes[i].disabled)
					{
						if (this.os.thisComputer.ip == this.nodes[i].ip)
						{
							color = this.os.thisComputerNode;
						}
						else
						{
							if (this.os.connectedComp != null)
							{
								if (this.os.connectedComp.ip == this.nodes[i].ip)
								{
									color = Color.White;
								}
								else if (this.nodes[i].adminIP == this.os.thisComputer.ip && this.nodes[i].ip == this.os.opponentLocation)
								{
									color = Color.DarkRed;
								}
								else if (this.os.shellIPs.Contains(this.nodes[i].ip))
								{
									color = this.os.shellColor;
								}
								else
								{
									color = this.os.highlightColor;
								}
							}
							else if (this.nodes[i].adminIP == this.os.thisComputer.ip && this.nodes[i].ip == this.os.opponentLocation)
							{
								color = Color.DarkRed;
							}
							else if (this.os.shellIPs.Contains(this.nodes[i].ip))
							{
								color = this.os.shellColor;
							}
							else
							{
								color = this.os.highlightColor;
							}
							if (this.nodes[i].highlightFlashTime > 0f)
							{
								this.nodes[i].highlightFlashTime -= t;
								color = Color.Lerp(color, Utils.AddativeWhite, Utils.QuadraticOutCurve(this.nodes[i].highlightFlashTime));
							}
						}
						Vector2 nodeDrawPos2 = this.GetNodeDrawPos(this.nodes[i], i);
						if (this.DimNonConnectedNodes && this.os.connectedComp != null && this.os.connectedComp.ip != this.nodes[i].ip)
						{
							color *= 0.3f;
						}
						if (Button.doButton(2000 + i, this.bounds.X + (int)nodeDrawPos2.X, this.bounds.Y + (int)nodeDrawPos2.Y, NetworkMap.NODE_SIZE, NetworkMap.NODE_SIZE, "", new Color?(color), (this.nodes[i].adminIP == this.os.thisComputer.ip) ? this.adminNodeCircle : this.nodeCircle))
						{
							if (this.os.inputEnabled)
							{
								bool flag2 = false;
								if (this.os.terminal.preventingExecution && this.os.terminal.executionPreventionIsInteruptable)
								{
									this.os.terminal.executeLine();
									flag2 = true;
								}
								int nodeindex = i;
								Action action = delegate()
								{
									this.os.runCommand("connect " + this.nodes[nodeindex].ip);
								};
								if (flag2)
								{
									this.os.delayer.Post(ActionDelayer.NextTick(), action);
								}
								else
								{
									action();
								}
							}
						}
						if (GuiData.hot == 2000 + i)
						{
							num = i;
						}
						if (this.nodes[i].idName == this.os.homeAssetServerID && !this.nodes[i].disabled)
						{
							this.spriteBatch.Draw(this.assetServerNodeOverlay, new Vector2((float)(this.bounds.X + (int)nodeDrawPos2.X + NetworkMap.NODE_SIZE / 2), (float)(this.bounds.Y + (int)nodeDrawPos2.Y + NetworkMap.NODE_SIZE / 2)), null, (num == i) ? GuiData.Default_Lit_Backing_Color : ((this.os.connectedComp != null && this.os.connectedComp == this.nodes[i]) ? Color.Black : (this.nodes[i].Equals(this.os.thisComputer) ? this.os.thisComputerNode : this.os.highlightColor)), -0.5f * this.rotation, new Vector2((float)(this.assetServerNodeOverlay.Width / 2)), new Vector2(NetworkMap.ADMIN_CIRCLE_SCALE - 0.22f), SpriteEffects.None, 0.5f);
						}
					}
				}
			}
			int nodeIndex = (this.os.connectedComp == null) ? 0 : this.nodes.IndexOf(this.os.connectedComp);
			if (this.os.connectedComp != null && !this.os.connectedComp.Equals(this.os.thisComputer) && !this.os.connectedComp.adminIP.Equals(this.os.thisComputer.ip))
			{
				GuiData.spriteBatch.Draw(this.nodeGlow, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex) - new Vector2((float)(this.nodeGlow.Width / 2), (float)(this.nodeGlow.Height / 2)) + new Vector2((float)this.bounds.X, (float)this.bounds.Y) + new Vector2((float)(NetworkMap.NODE_SIZE / 2)), this.os.connectedNodeHighlight);
				if (this.nodeEffect != null && this.os.connectedComp != null)
				{
					this.nodeEffect.draw(this.spriteBatch, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex) + new Vector2((float)(NetworkMap.NODE_SIZE / 2)) + new Vector2((float)this.bounds.X, (float)this.bounds.Y));
				}
			}
			else if (this.os.connectedComp != null && !this.os.connectedComp.Equals(this.os.thisComputer))
			{
				if (this.adminNodeEffect != null)
				{
					this.adminNodeEffect.draw(this.spriteBatch, this.GetNodeDrawPos(this.os.connectedComp, nodeIndex) + new Vector2((float)(NetworkMap.NODE_SIZE / 2)) + new Vector2((float)this.bounds.X, (float)this.bounds.Y));
				}
			}
			if (num != -1)
			{
				try
				{
					int i = num;
					int num3 = i;
					Vector2 vector = this.GetNodeDrawPos(this.nodes[num3], num3);
					Vector2 ttpos = new Vector2((float)(this.bounds.X + (int)vector.X + NetworkMap.NODE_SIZE), (float)(this.bounds.Y + (int)vector.Y));
					string text = this.nodes[num3].getTooltipString();
					Vector2 textSize = GuiData.tinyfont.MeasureString(text);
					OS os = this.os;
					os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
					{
						GuiData.spriteBatch.Draw(Utils.white, new Rectangle((int)ttpos.X, (int)ttpos.Y, (int)textSize.X, (int)textSize.Y), this.os.netmapToolTipBackground);
						TextItem.doFontLabel(ttpos, text, GuiData.tinyfont, new Color?(this.os.netmapToolTipColor), float.MaxValue, float.MaxValue, false);
					}));
				}
				catch (Exception ex)
				{
					DebugLog.add(ex.ToString());
				}
			}
			if (Settings.debugDrawEnabled)
			{
				for (int i = 0; i < Corporation.TestedPositions.Count; i++)
				{
					Vector2 vector = this.GetNodeDrawPosDebug(Corporation.TestedPositions[i]);
					Vector2 position = new Vector2((float)(this.bounds.X + (int)vector.X + NetworkMap.NODE_SIZE), (float)(this.bounds.Y + (int)vector.Y));
					GuiData.spriteBatch.Draw(Utils.white, position, null, Utils.AddativeRed, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.8f);
				}
			}
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x00080BF4 File Offset: 0x0007EDF4
		public Vector2 GetNodeDrawPosDebug(Vector2 nodeLocation)
		{
			int num = 3;
			nodeLocation = Utils.Clamp(nodeLocation, 0f, 1f);
			float num2 = (float)this.bounds.Width - (float)NetworkMap.NODE_SIZE * 1f;
			float num3 = (float)this.bounds.Height - (float)NetworkMap.NODE_SIZE * 1f;
			num2 -= (float)(2 * num);
			num3 -= (float)(2 * num);
			Vector2 result = new Vector2(nodeLocation.X * num2 + (float)NetworkMap.NODE_SIZE / 4f, nodeLocation.Y * num3 + (float)NetworkMap.NODE_SIZE / 4f);
			return result;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x00080C94 File Offset: 0x0007EE94
		public Vector2 GetNodeDrawPos(Computer node)
		{
			return this.GetNodeDrawPos(node, this.nodes.IndexOf(node));
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x00080CBC File Offset: 0x0007EEBC
		public Vector2 GetNodeDrawPos(Computer node, int nodeIndex)
		{
			int num = 3;
			Vector2 vector = Utils.Clamp(node.location, 0f, 1f);
			float num2 = (float)this.bounds.Width - (float)NetworkMap.NODE_SIZE * 1f;
			float num3 = (float)this.bounds.Height - (float)NetworkMap.NODE_SIZE * 1f;
			num2 -= (float)(2 * num);
			num3 -= (float)(2 * num);
			Vector2 nodePosition = NetmapSortingAlgorithms.GetNodePosition(this.SortingAlgorithm, num2, num3, node, nodeIndex, this.nodes.Count, this.visibleNodes.Count, this.os);
			return nodePosition + new Vector2((float)NetworkMap.NODE_SIZE / 4f);
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x00080D74 File Offset: 0x0007EF74
		public static string generateRandomIP()
		{
			return string.Concat(new object[]
			{
				Utils.random.Next(254) + 1,
				".",
				Utils.random.Next(254) + 1,
				".",
				Utils.random.Next(254) + 1,
				".",
				Utils.random.Next(254) + 1
			});
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x00080E10 File Offset: 0x0007F010
		public void drawLine(Vector2 origin, Vector2 dest, Vector2 offset)
		{
			Vector2 value = new Vector2((float)(NetworkMap.NODE_SIZE / 2));
			origin += value;
			dest += value;
			float y = Vector2.Distance(origin, dest);
			float num = (float)Math.Atan2((double)(dest.Y - origin.Y), (double)(dest.X - origin.X));
			num += 4.712389f;
			this.spriteBatch.Draw(Utils.white, origin + offset, null, this.os.outlineColor, num, Vector2.Zero, new Vector2(1f, y), SpriteEffects.None, 0.5f);
		}

		// Token: 0x040008C8 RID: 2248
		public static int NODE_SIZE = 26;

		// Token: 0x040008C9 RID: 2249
		public static float ADMIN_CIRCLE_SCALE = 0.62f;

		// Token: 0x040008CA RID: 2250
		public static float PULSE_DECAY = 0.5f;

		// Token: 0x040008CB RID: 2251
		public static float PULSE_FREQUENCY = 0.8f;

		// Token: 0x040008CC RID: 2252
		public List<Corporation> corporations;

		// Token: 0x040008CD RID: 2253
		public List<Computer> nodes;

		// Token: 0x040008CE RID: 2254
		public List<int> visibleNodes;

		// Token: 0x040008CF RID: 2255
		private Texture2D circle;

		// Token: 0x040008D0 RID: 2256
		private Texture2D circleOutline;

		// Token: 0x040008D1 RID: 2257
		private Texture2D adminCircle;

		// Token: 0x040008D2 RID: 2258
		private Texture2D nodeCircle;

		// Token: 0x040008D3 RID: 2259
		private Texture2D adminNodeCircle;

		// Token: 0x040008D4 RID: 2260
		private Texture2D nodeGlow;

		// Token: 0x040008D5 RID: 2261
		private Texture2D homeNodeCircle;

		// Token: 0x040008D6 RID: 2262
		private Texture2D targetNodeCircle;

		// Token: 0x040008D7 RID: 2263
		private Texture2D assetServerNodeOverlay;

		// Token: 0x040008D8 RID: 2264
		private string label;

		// Token: 0x040008D9 RID: 2265
		private Vector2 circleOrigin;

		// Token: 0x040008DA RID: 2266
		private float rotation = 0f;

		// Token: 0x040008DB RID: 2267
		private float pulseFade = 1f;

		// Token: 0x040008DC RID: 2268
		private float pulseTimer = NetworkMap.PULSE_FREQUENCY;

		// Token: 0x040008DD RID: 2269
		public ConnectedNodeEffect nodeEffect;

		// Token: 0x040008DE RID: 2270
		public ConnectedNodeEffect adminNodeEffect;

		// Token: 0x040008DF RID: 2271
		public bool DimNonConnectedNodes = false;

		// Token: 0x040008E0 RID: 2272
		public NetmapSortingAlgorithm SortingAlgorithm = NetmapSortingAlgorithm.Scatter;

		// Token: 0x040008E1 RID: 2273
		public Computer mailServer;

		// Token: 0x040008E2 RID: 2274
		public Computer academicDatabase;

		// Token: 0x040008E3 RID: 2275
		public Computer lastAddedNode;
	}
}
