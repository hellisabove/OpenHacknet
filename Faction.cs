using System;
using System.Xml;
using Hacknet.Factions;

namespace Hacknet
{
	// Token: 0x02000074 RID: 116
	public class Faction
	{
		// Token: 0x06000244 RID: 580 RVA: 0x00021338 File Offset: 0x0001F538
		public Faction(string _name, int _neededValue)
		{
			this.playerValue = 0;
			this.neededValue = _neededValue;
			this.name = _name;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00021388 File Offset: 0x0001F588
		public virtual void addValue(int value, object os)
		{
			int num = this.playerValue;
			this.playerValue += value;
			if (this.playerValue >= this.neededValue && !this.playerHasPassedValue)
			{
				this.playerPassedValue(os);
			}
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000213D4 File Offset: 0x0001F5D4
		public void contractAbbandoned(object osIn)
		{
			OS os = (OS)osIn;
			if (this.PlayerLosesValueOnAbandon)
			{
				this.playerValue -= 10;
				if (this.playerValue < 0)
				{
					this.playerValue = 0;
				}
			}
			string subject = LocaleTerms.Loc("Contract Abandoned");
			string body = string.Format(Utils.readEntireFile("Content/LocPost/ContractAbandonedEmail.txt"), this.getRank(), this.getMaxRank(), this.name);
			string sender = this.name + " ReplyBot";
			string mail = MailServer.generateEmail(subject, body, sender);
			MailServer mailServer = (MailServer)os.netMap.mailServer.getDaemon(typeof(MailServer));
			mailServer.addMail(mail, os.defaultUser.name);
		}

		// Token: 0x06000247 RID: 583 RVA: 0x000214AC File Offset: 0x0001F6AC
		public int getRank()
		{
			float num = Math.Min((float)this.playerValue, (float)this.neededValue);
			return Math.Max(1, Math.Abs((int)((1f - num / (float)this.neededValue) * (float)this.getMaxRank())));
		}

		// Token: 0x06000248 RID: 584 RVA: 0x000214F8 File Offset: 0x0001F6F8
		public virtual string getSaveString()
		{
			string text = "Faction";
			if (this is EntropyFaction)
			{
				text = "EntropyFaction";
			}
			if (this is HubFaction)
			{
				text = "HubFaction";
			}
			if (this is CustomFaction)
			{
				text = "CustomFaction";
			}
			return string.Concat(new object[]
			{
				"<",
				text,
				" name=\"",
				this.name,
				"\" id=\"",
				this.idName,
				"\" neededVal=\"",
				this.neededValue,
				"\" playerVal=\"",
				this.playerValue,
				"\" playerHasPassed=\"",
				this.playerHasPassedValue,
				"\" />"
			});
		}

		// Token: 0x06000249 RID: 585 RVA: 0x000215DC File Offset: 0x0001F7DC
		public int getMaxRank()
		{
			return 100;
		}

		// Token: 0x0600024A RID: 586 RVA: 0x000215F0 File Offset: 0x0001F7F0
		public virtual void playerPassedValue(object os)
		{
			this.playerHasPassedValue = true;
		}

		// Token: 0x0600024B RID: 587 RVA: 0x000215FC File Offset: 0x0001F7FC
		public static Faction loadFromSave(XmlReader xmlRdr)
		{
			string text = "UNKNOWN";
			string id = "";
			bool playerHasPassed = false;
			int num = 100;
			int playerVal = 0;
			while (xmlRdr.Name != "Faction" && xmlRdr.Name != "CustomFaction" && xmlRdr.Name != "EntropyFaction" && xmlRdr.Name != "HubFaction")
			{
				xmlRdr.Read();
			}
			string text2 = xmlRdr.Name;
			if (xmlRdr.MoveToAttribute("name"))
			{
				text = xmlRdr.ReadContentAsString();
			}
			if (xmlRdr.MoveToAttribute("id"))
			{
				id = xmlRdr.ReadContentAsString();
			}
			if (xmlRdr.MoveToAttribute("neededVal"))
			{
				num = xmlRdr.ReadContentAsInt();
			}
			if (xmlRdr.MoveToAttribute("playerVal"))
			{
				playerVal = xmlRdr.ReadContentAsInt();
			}
			if (xmlRdr.MoveToAttribute("playerHasPassed"))
			{
				playerHasPassed = (xmlRdr.ReadContentAsString().ToLower() == "true");
			}
			string text3 = text2;
			Faction faction;
			if (text3 != null && !(text3 == "Faction"))
			{
				if (text3 == "HubFaction")
				{
					faction = new HubFaction(text, num);
					goto IL_182;
				}
				if (text3 == "EntropyFaction")
				{
					faction = new EntropyFaction(text, num);
					goto IL_182;
				}
				if (text3 == "CustomFaction")
				{
					faction = CustomFaction.DeserializeFromXmlReader(xmlRdr, text, id, playerVal, playerHasPassed);
					goto IL_182;
				}
			}
			faction = new Faction(text, num);
			IL_182:
			faction.playerValue = playerVal;
			faction.idName = id;
			faction.playerHasPassedValue = playerHasPassed;
			return faction;
		}

		// Token: 0x0600024C RID: 588 RVA: 0x000217AC File Offset: 0x0001F9AC
		public bool valuePassedPoint(int oldValue, int neededValue)
		{
			return this.playerValue >= neededValue && oldValue < neededValue;
		}

		// Token: 0x040002BF RID: 703
		public int playerValue;

		// Token: 0x040002C0 RID: 704
		public int neededValue;

		// Token: 0x040002C1 RID: 705
		public string name = "unknown";

		// Token: 0x040002C2 RID: 706
		public string idName = "";

		// Token: 0x040002C3 RID: 707
		public bool playerHasPassedValue = false;

		// Token: 0x040002C4 RID: 708
		public bool PlayerLosesValueOnAbandon = false;
	}
}
