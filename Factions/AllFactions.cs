using System;
using System.Collections.Generic;
using System.Xml;

namespace Hacknet.Factions
{
	// Token: 0x0200010D RID: 269
	internal class AllFactions
	{
		// Token: 0x0600065E RID: 1630 RVA: 0x0006A0C0 File Offset: 0x000682C0
		public AllFactions()
		{
			this.currentFaction = null;
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0006A0E0 File Offset: 0x000682E0
		public void init()
		{
			if (!Settings.IsInExtensionMode)
			{
				this.currentFaction = "entropy";
				this.factions.Add("entropy", new EntropyFaction("Entropy", 5)
				{
					idName = "entropy"
				});
				this.factions.Add("lelzSec", new Faction("lelzSec", 1000)
				{
					idName = "lelzSec"
				});
				this.factions.Add("hub", new HubFaction("CSEC", 10)
				{
					idName = "hub"
				});
			}
		}

		// Token: 0x06000660 RID: 1632 RVA: 0x0006A188 File Offset: 0x00068388
		public string getSaveString()
		{
			string text = "<AllFactions current=\"" + this.currentFaction + "\">\n";
			foreach (KeyValuePair<string, Faction> keyValuePair in this.factions)
			{
				text = text + "\t" + keyValuePair.Value.getSaveString();
			}
			text += "</AllFactions>";
			return text;
		}

		// Token: 0x06000661 RID: 1633 RVA: 0x0006A220 File Offset: 0x00068420
		public void setCurrentFaction(string newFaction, OS os)
		{
			this.currentFaction = newFaction;
			os.currentFaction = this.factions[this.currentFaction];
		}

		// Token: 0x06000662 RID: 1634 RVA: 0x0006A244 File Offset: 0x00068444
		public static AllFactions loadFromSave(XmlReader xmlRdr)
		{
			AllFactions allFactions = new AllFactions();
			while (xmlRdr.Name != "AllFactions")
			{
				xmlRdr.Read();
			}
			if (xmlRdr.MoveToAttribute("current"))
			{
				allFactions.currentFaction = xmlRdr.ReadContentAsString();
			}
			do
			{
				xmlRdr.Read();
			}
			while (string.IsNullOrWhiteSpace(xmlRdr.Name));
			while (!(xmlRdr.Name == "AllFactions") || xmlRdr.IsStartElement())
			{
				Faction faction = Faction.loadFromSave(xmlRdr);
				allFactions.factions.Add(faction.idName, faction);
				xmlRdr.Read();
			}
			return allFactions;
		}

		// Token: 0x04000724 RID: 1828
		public Dictionary<string, Faction> factions = new Dictionary<string, Faction>();

		// Token: 0x04000725 RID: 1829
		public string currentFaction;
	}
}
