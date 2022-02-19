using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200002E RID: 46
	public abstract class SerializableAction
	{
		// Token: 0x06000127 RID: 295
		public abstract void Trigger(object os_obj);

		// Token: 0x06000128 RID: 296 RVA: 0x00011B50 File Offset: 0x0000FD50
		public string GetSaveString()
		{
			Type type = base.GetType();
			string text = type.Name;
			if (text.StartsWith("Hacknet."))
			{
				text = text.Substring("Hacknet.".Length);
			}
			if (text.StartsWith("SA"))
			{
				text = text.Substring("SA".Length);
			}
			StringBuilder stringBuilder = new StringBuilder("<" + text + " ");
			string text2 = null;
			FieldInfo[] fields = type.GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (Utils.FieldContainsAttributeOfType(fields[i], typeof(XMLContentAttribute)))
				{
					if (text2 != null)
					{
						throw new InvalidOperationException("More than one field in object " + this.ToString() + " is a content serializable type!");
					}
					text2 = string.Format(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						fields[i].GetValue(this)
					});
				}
				else
				{
					stringBuilder.Append(fields[i].Name + "=\"");
					stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", new object[]
					{
						fields[i].GetValue(this)
					});
					stringBuilder.Append("\" ");
				}
			}
			if (text2 == null)
			{
				stringBuilder.Append("/>");
			}
			else
			{
				stringBuilder.Append(">");
				stringBuilder.Append(text2);
				stringBuilder.Append("</" + text + ">");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000129 RID: 297 RVA: 0x00011D10 File Offset: 0x0000FF10
		public static SerializableAction Deserialize(XmlReader rdr)
		{
			Dictionary<string, Func<XmlReader, SerializableAction>> dictionary = new Dictionary<string, Func<XmlReader, SerializableAction>>();
			dictionary.Add("LoadMission", new Func<XmlReader, SerializableAction>(SALoadMission.DeserializeFromReader));
			dictionary.Add("RunFunction", new Func<XmlReader, SerializableAction>(SARunFunction.DeserializeFromReader));
			dictionary.Add("AddAsset", new Func<XmlReader, SerializableAction>(SAAddAsset.DeserializeFromReader));
			dictionary.Add("AddMissionToHubServer", new Func<XmlReader, SerializableAction>(SAAddMissionToHubServer.DeserializeFromReader));
			dictionary.Add("RemoveMissionFromHubServer", new Func<XmlReader, SerializableAction>(SARemoveMissionFromHubServer.DeserializeFromReader));
			dictionary.Add("AddThreadToMissionBoard", new Func<XmlReader, SerializableAction>(SAAddThreadToMissionBoard.DeserializeFromReader));
			dictionary.Add("AddIRCMessage", new Func<XmlReader, SerializableAction>(SAAddIRCMessage.DeserializeFromReader));
			dictionary.Add("AddConditionalActions", new Func<XmlReader, SerializableAction>(SAAddConditionalActions.DeserializeFromReader));
			dictionary.Add("CopyAsset", new Func<XmlReader, SerializableAction>(SACopyAsset.DeserializeFromReader));
			dictionary.Add("CrashComputer", new Func<XmlReader, SerializableAction>(SACrashComputer.DeserializeFromReader));
			dictionary.Add("DeleteFile", new Func<XmlReader, SerializableAction>(SADeleteFile.DeserializeFromReader));
			dictionary.Add("LaunchHackScript", new Func<XmlReader, SerializableAction>(SALaunchHackScript.DeserializeFromReader));
			dictionary.Add("SwitchToTheme", new Func<XmlReader, SerializableAction>(SASwitchToTheme.DeserializeFromReader));
			dictionary.Add("StartScreenBleedEffect", new Func<XmlReader, SerializableAction>(SAStartScreenBleedEffect.DeserializeFromReader));
			dictionary.Add("CancelScreenBleedEffect", new Func<XmlReader, SerializableAction>(SACancelScreenBleedEffect.DeserializeFromReader));
			dictionary.Add("AppendToFile", new Func<XmlReader, SerializableAction>(SAAppendToFile.DeserializeFromReader));
			dictionary.Add("KillExe", new Func<XmlReader, SerializableAction>(SAKillExe.DeserializeFromReader));
			dictionary.Add("ChangeAlertIcon", new Func<XmlReader, SerializableAction>(SAChangeAlertIcon.DeserializeFromReader));
			dictionary.Add("HideNode", new Func<XmlReader, SerializableAction>(SAHideNode.DeserializeFromReader));
			dictionary.Add("GivePlayerUserAccount", new Func<XmlReader, SerializableAction>(SAGivePlayerUserAccount.DeserializeFromReader));
			dictionary.Add("ChangeIP", new Func<XmlReader, SerializableAction>(SAChangeIP.DeserializeFromReader));
			dictionary.Add("ChangeNetmapSortMethod", new Func<XmlReader, SerializableAction>(SAChangeNetmapSortMethod.DeserializeFromReader));
			dictionary.Add("SaveGame", new Func<XmlReader, SerializableAction>(SASaveGame.DeserializeFromReader));
			dictionary.Add("HideAllNodes", new Func<XmlReader, SerializableAction>(SAHideAllNodes.DeserializeFromReader));
			dictionary.Add("ShowNode", new Func<XmlReader, SerializableAction>(SAShowNode.DeserializeFromReader));
			dictionary.Add("SetLock", new Func<XmlReader, SerializableAction>(SASetLock.DeserializeFromReader));
			while (!rdr.EOF && (!rdr.IsStartElement() || !dictionary.ContainsKey(rdr.Name)))
			{
				rdr.Read();
			}
			if (rdr.EOF)
			{
				throw new FormatException("Unexpected end of file!");
			}
			return dictionary[rdr.Name](rdr);
		}
	}
}
