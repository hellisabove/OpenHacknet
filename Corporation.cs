using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000103 RID: 259
	internal class Corporation
	{
		// Token: 0x060005E0 RID: 1504 RVA: 0x00060D30 File Offset: 0x0005EF30
		public Corporation(OS _os)
		{
			this.os = _os;
			this.baseSecurityLevel = 3;
			this.serverCount = 5;
			this.baseID = "corp" + GenerationStatics.CorportationsGenerated + "#";
			GenerationStatics.CorportationsGenerated++;
			this.servers = new List<Computer>();
			this.altRotation = Utils.flipCoin();
			do
			{
				this.basePosition = this.os.netMap.getRandomPosition();
			}
			while (this.locationCollides(this.basePosition));
			this.generate();
		}

		// Token: 0x060005E1 RID: 1505 RVA: 0x00060DCB File Offset: 0x0005EFCB
		private void generate()
		{
			this.generateName();
			this.generateServers();
		}

		// Token: 0x060005E2 RID: 1506 RVA: 0x00060DDC File Offset: 0x0005EFDC
		private void generateName()
		{
			string[] array = NameGenerator.generateCompanyName();
			this.name = array[0];
			this.postfix = array[1];
			this.ipSubstring = NetworkMap.generateRandomIP();
			this.ipSubstring = this.ipSubstring.Substring(this.ipSubstring.Length - this.ipSubstring.LastIndexOf('.'));
			this.ipSubstring += ".";
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x00060E50 File Offset: 0x0005F050
		private void generateServers()
		{
			this.generateMainframe();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.generateMailServer();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.generateWebServer();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.generateInternalServices();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.generateFileServer();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.generateBackupMachine();
			this.os.netMap.nodes.Add(this.servers[this.servers.Count - 1]);
			this.linkServers();
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00060FA4 File Offset: 0x0005F1A4
		private void generateMainframe()
		{
			this.mainframe = new Computer(this.getFullName() + " Central Mainframe", this.getAddress(), this.getLocation(), this.baseSecurityLevel + 2, 3, this.os);
			this.mainframe.idName = this.baseID + "MF";
			this.servers.Add(this.mainframe);
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00061018 File Offset: 0x0005F218
		private void generateMailServer()
		{
			this.mailServer = new Computer(this.name + " Mail Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, 3, this.os);
			MailServer item = new MailServer(this.mailServer, this.name + " Mail", this.os);
			this.mailServer.daemons.Add(item);
			this.mailServer.initDaemons();
			this.mailServer.idName = this.baseID + "MS";
			this.servers.Add(this.mailServer);
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x000610C8 File Offset: 0x0005F2C8
		private void generateWebServer()
		{
			this.webServer = new Computer(this.name + " Web Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, 3, this.os);
			WebServerDaemon webServerDaemon = new WebServerDaemon(this.webServer, this.name + " Web Server", this.os, "Content/Web/BaseImageWebPage.html");
			this.webServer.daemons.Add(webServerDaemon);
			this.webServer.initDaemons();
			webServerDaemon.generateBaseCorporateSite(this.getFullName(), "Content/Web/BaseCorporatePage.html");
			this.webServer.idName = this.baseID + "WS";
			this.servers.Add(this.webServer);
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00061190 File Offset: 0x0005F390
		private void generateInternalServices()
		{
			this.internalServices = new Computer(this.name + " Internal Services Machine", this.getAddress(), this.getLocation(), this.baseSecurityLevel - 1, 1, this.os);
			this.internalServices.idName = this.baseID + "IS";
			this.servers.Add(this.internalServices);
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00061204 File Offset: 0x0005F404
		private void generateFileServer()
		{
			this.fileServer = new Computer(this.name + " File Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel, 3, this.os);
			MissionListingServer item = new MissionListingServer(this.fileServer, "Mission Board", this.name, this.os, false, false);
			this.fileServer.daemons.Add(item);
			this.fileServer.initDaemons();
			this.fileServer.idName = this.baseID + "FS";
			this.servers.Add(this.fileServer);
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x000612B0 File Offset: 0x0005F4B0
		private void generateBackupMachine()
		{
			this.backupServer = new Computer(this.getFullName() + " Backup Server", this.getAddress(), this.getLocation(), this.baseSecurityLevel - 1, 1, this.os);
			this.backupServer.idName = this.baseID + "BU";
			this.servers.Add(this.backupServer);
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00061324 File Offset: 0x0005F524
		private void linkServers()
		{
			for (int i = 0; i < this.servers.Count; i++)
			{
				for (int j = 0; j < this.servers.Count; j++)
				{
					if (j != i)
					{
						this.servers[i].links.Add(this.os.netMap.nodes.IndexOf(this.servers[j]) + 1);
					}
				}
			}
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x000613B0 File Offset: 0x0005F5B0
		private void addServersToInternet()
		{
			for (int i = 0; i < this.servers.Count; i++)
			{
				this.os.netMap.nodes.Add(this.servers[i]);
			}
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x000613FC File Offset: 0x0005F5FC
		private string getAddress()
		{
			string text;
			bool flag;
			do
			{
				text = this.ipSubstring + (Utils.random.Next() % 253 + 1);
				flag = true;
				for (int i = 0; i < this.servers.Count; i++)
				{
					if (this.servers[i].ip.Equals(text))
					{
						flag = false;
					}
				}
			}
			while (!flag);
			return text;
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00061488 File Offset: 0x0005F688
		private Vector2 getLocation()
		{
			Vector2 value = this.basePosition;
			Vector2 nearbyNodeOffset = Corporation.getNearbyNodeOffset(this.basePosition, this.servers.Count, this.serverCount, this.os.netMap, 0f, false);
			return value + nearbyNodeOffset;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x000614D8 File Offset: 0x0005F6D8
		public static Vector2 GetOffsetPositionFromCycle(int pos, int total, float ExtraDistance = 0f)
		{
			int num = pos / total - 1;
			num = Math.Max(0, num);
			int num2 = pos % total;
			float magnitude = ExtraDistance + Corporation.COMPUTER_SEPERATION + (float)num * Corporation.COMPUTER_SEPERATION_ADD_PER_CYCLE;
			float angle = (float)num2 / (float)total * 6.2831855f;
			Vector2 result = Utils.PolarToCartesian(angle, magnitude);
			result.Y *= Corporation.Y_ASPECT_RATIO_BIAS;
			return result;
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x0006153C File Offset: 0x0005F73C
		private static bool GeneratedPositionIsValid(Vector2 position, NetworkMap netMap, bool ignoreNetmap = false)
		{
			return position.X >= 0f && position.X <= 1f && position.Y >= 0f && position.Y <= 1f && (ignoreNetmap || !netMap.collides(position, -1f));
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x000615AC File Offset: 0x0005F7AC
		public static Vector2 getNearbyNodeOffset(Vector2 basePos, int positionNumber, int total, NetworkMap map, float extraDistance = 0f, bool forceUseThisPosition = false)
		{
			int total2 = total;
			int num = positionNumber;
			if (total < 20)
			{
				int num2 = 30;
				float num3 = (float)positionNumber / (float)total;
				total2 = num2;
				num = (int)(num3 * (float)num2);
			}
			int num4 = 300;
			for (int i = 0; i < num4; i++)
			{
				Vector2 offsetPositionFromCycle = Corporation.GetOffsetPositionFromCycle(num + i, total2, extraDistance);
				if (Corporation.GeneratedPositionIsValid(offsetPositionFromCycle + basePos, map, forceUseThisPosition))
				{
					return offsetPositionFromCycle;
				}
				Corporation.TestedPositions.Add(offsetPositionFromCycle + basePos);
			}
			Vector2 randomPosition = map.getRandomPosition();
			return randomPosition - basePos;
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x0006165C File Offset: 0x0005F85C
		public static Vector2 getNearbyNodeOffsetOld(Vector2 basePos, int positionNumber, int total, NetworkMap map, float ExtraSeperationDistance = 0f)
		{
			int num = 60;
			int num2 = 0;
			Vector2 vector = Vector2.Zero;
			Vector2 location;
			do
			{
				int i = positionNumber + num2;
				int num3 = total;
				while (i >= num3)
				{
					i -= num3;
					num3 += total;
				}
				if (i > 0)
				{
					Vector2 vector2 = Utils.PolarToCartesian((float)i / (float)num3 * 6.2831855f, 1f);
					vector = vector2;
				}
				else
				{
					vector.X = 1f;
				}
				vector.Y *= Corporation.Y_ASPECT_RATIO_BIAS;
				float num4 = Corporation.COMPUTER_SEPERATION + ExtraSeperationDistance;
				vector = new Vector2(vector.X * Corporation.COMPUTER_SEPERATION, vector.Y * Corporation.COMPUTER_SEPERATION);
				location = basePos + vector;
				num2++;
				Corporation.TestedPositions.Add(vector);
			}
			while ((location.X < 0f || location.X > 1f || location.Y < 0f || location.Y > 1f || map.collides(location, 0.075f)) && num2 < num);
			if (num2 >= num)
			{
				if (ExtraSeperationDistance <= 0f)
				{
					return Corporation.getNearbyNodeOffsetOld(basePos, positionNumber, total, map, Corporation.COMPUTER_SEPERATION);
				}
				if (ExtraSeperationDistance <= Corporation.COMPUTER_SEPERATION)
				{
					return Corporation.getNearbyNodeOffsetOld(basePos, positionNumber, total, map, Corporation.COMPUTER_SEPERATION + Corporation.COMPUTER_SEPERATION);
				}
				Vector2 randomPosition = map.getRandomPosition();
				vector = randomPosition - basePos;
			}
			return vector;
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00061808 File Offset: 0x0005FA08
		private bool locationCollides(Vector2 loc)
		{
			for (int i = 0; i < this.servers.Count; i++)
			{
				if (Vector2.Distance(loc, this.servers[i].location) <= 0.08f)
				{
					return true;
				}
			}
			return loc.X < 0f || loc.X > 1f || (loc.Y < 0f || loc.Y > 1f);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x000618B4 File Offset: 0x0005FAB4
		public string getName()
		{
			return this.name;
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x000618CC File Offset: 0x0005FACC
		public string getFullName()
		{
			return this.name + this.postfix;
		}

		// Token: 0x0400068D RID: 1677
		private static float COMPUTER_SEPERATION = 0.066f;

		// Token: 0x0400068E RID: 1678
		private static float COMPUTER_SEPERATION_ADD_PER_CYCLE = 0.04f;

		// Token: 0x0400068F RID: 1679
		private static float Y_ASPECT_RATIO_BIAS = 1.9f;

		// Token: 0x04000690 RID: 1680
		public static List<Vector2> TestedPositions = new List<Vector2>();

		// Token: 0x04000691 RID: 1681
		private OS os;

		// Token: 0x04000692 RID: 1682
		public List<Computer> servers;

		// Token: 0x04000693 RID: 1683
		private string name;

		// Token: 0x04000694 RID: 1684
		private string postfix;

		// Token: 0x04000695 RID: 1685
		private string ipSubstring;

		// Token: 0x04000696 RID: 1686
		private string baseID;

		// Token: 0x04000697 RID: 1687
		public Computer mainframe;

		// Token: 0x04000698 RID: 1688
		public Computer mailServer;

		// Token: 0x04000699 RID: 1689
		public Computer webServer;

		// Token: 0x0400069A RID: 1690
		public Computer internalServices;

		// Token: 0x0400069B RID: 1691
		public Computer fileServer;

		// Token: 0x0400069C RID: 1692
		public Computer backupServer;

		// Token: 0x0400069D RID: 1693
		private Vector2 basePosition;

		// Token: 0x0400069E RID: 1694
		private int baseSecurityLevel;

		// Token: 0x0400069F RID: 1695
		private int serverCount;

		// Token: 0x040006A0 RID: 1696
		private bool altRotation;
	}
}
