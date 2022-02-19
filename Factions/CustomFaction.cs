using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Hacknet.Factions
{
	// Token: 0x02000075 RID: 117
	public class CustomFaction : Faction
	{
		// Token: 0x0600024D RID: 589 RVA: 0x000217CF File Offset: 0x0001F9CF
		public CustomFaction(string _name, int _neededValue) : base(_name, _neededValue)
		{
			this.PlayerLosesValueOnAbandon = false;
		}

		// Token: 0x0600024E RID: 590 RVA: 0x000217F0 File Offset: 0x0001F9F0
		public static CustomFaction ParseFromFile(string filepath)
		{
			string localizedFilepath = LocalizedFileLoader.GetLocalizedFilepath(filepath);
			CustomFaction result;
			using (FileStream fileStream = File.OpenRead(localizedFilepath))
			{
				XmlReader xmlRdr = XmlReader.Create(fileStream);
				Faction faction = Faction.loadFromSave(xmlRdr);
				result = (faction as CustomFaction);
			}
			return result;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x00021854 File Offset: 0x0001FA54
		public void CheckForAllCustomActionsToRun(object os_obj)
		{
			OS os = (OS)os_obj;
			for (int i = 0; i < this.CustomActions.Count; i++)
			{
				if (this.playerValue >= this.CustomActions[i].ValueRequiredForTrigger)
				{
					bool flag = true;
					if (!string.IsNullOrWhiteSpace(this.CustomActions[i].FlagsRequiredForTrigger))
					{
						string[] array = this.CustomActions[i].FlagsRequiredForTrigger.Split(Utils.commaDelim, StringSplitOptions.RemoveEmptyEntries);
						for (int j = 0; j < array.Length; j++)
						{
							if (!os.Flags.HasFlag(array[j]))
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						CustomFactionAction customFactionAction = this.CustomActions[i];
						this.CustomActions.RemoveAt(i);
						i--;
						customFactionAction.Trigger(os);
					}
				}
			}
		}

		// Token: 0x06000250 RID: 592 RVA: 0x00021958 File Offset: 0x0001FB58
		public override void addValue(int value, object os_obj)
		{
			int playerValue = this.playerValue;
			base.addValue(value, os_obj);
			this.CheckForAllCustomActionsToRun(os_obj);
		}

		// Token: 0x06000251 RID: 593 RVA: 0x0002197E File Offset: 0x0001FB7E
		private void SendNotification(object osIn, string body, string subject)
		{
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00021984 File Offset: 0x0001FB84
		public override string getSaveString()
		{
			string text = base.getSaveString();
			string text2 = ">\n";
			for (int i = 0; i < this.CustomActions.Count; i++)
			{
				text2 += this.CustomActions[i].GetSaveString();
				text2 += "\n";
			}
			text2 += "</CustomFaction>";
			text = text.Replace("/>", text2);
			return "\r\n" + text;
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00021A0C File Offset: 0x0001FC0C
		public static CustomFaction DeserializeFromXmlReader(XmlReader rdr, string name, string id, int playerVal, bool playerHasPassed)
		{
			CustomFaction customFaction = new CustomFaction(name, 100);
			rdr.MoveToElement();
			while (!rdr.EOF)
			{
				if (rdr.Name != "CustomFaction")
				{
					customFaction.CustomActions.Add(CustomFactionAction.Deserialize(rdr));
					rdr.Read();
				}
				else
				{
					if (rdr.Name == "CustomFaction" && !rdr.IsStartElement())
					{
						break;
					}
					rdr.Read();
				}
				while (!rdr.EOF && string.IsNullOrWhiteSpace(rdr.Name))
				{
					rdr.Read();
				}
			}
			return customFaction;
		}

		// Token: 0x040002C5 RID: 709
		internal List<CustomFactionAction> CustomActions = new List<CustomFactionAction>();
	}
}
