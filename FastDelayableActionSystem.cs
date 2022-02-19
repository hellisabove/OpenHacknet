using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000021 RID: 33
	public class FastDelayableActionSystem : DelayableActionSystem
	{
		// Token: 0x060000D6 RID: 214 RVA: 0x0000D978 File Offset: 0x0000BB78
		public FastDelayableActionSystem(Folder sourceFolder, object osObj)
		{
			FastDelayableActionSystem <>4__this = this;
			this.folder = sourceFolder;
			OS os = (OS)osObj;
			os.UpdateSubscriptions = (Action<float>)Delegate.Combine(os.UpdateSubscriptions, new Action<float>(delegate(float t)
			{
				<>4__this.Update(t, (OS)osObj);
			}));
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000D9F0 File Offset: 0x0000BBF0
		private void Update(float t, object osObj)
		{
			for (int i = 0; i < this.Actions.Count; i++)
			{
				try
				{
					float num = this.Actions[i].Key;
					SerializableAction value = this.Actions[i].Value;
					num -= t;
					if (num <= 0f)
					{
						try
						{
							value.Trigger((OS)osObj);
						}
						catch (Exception ex)
						{
							((OS)osObj).write(Utils.GenerateReportFromException(ex));
							Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
						}
						this.Actions.RemoveAt(i);
						i--;
					}
					else
					{
						this.Actions[i] = new KeyValuePair<float, SerializableAction>(num, value);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x0000DAE8 File Offset: 0x0000BCE8
		public override void InstantlyResolveAllActions(object osObj)
		{
			for (int i = 0; i < this.Actions.Count; i++)
			{
				try
				{
					this.Actions[i].Value.Trigger(osObj);
					this.Actions.RemoveAt(i);
					i--;
				}
				catch (Exception ex)
				{
					((OS)osObj).write(Utils.GenerateReportFromException(ex));
					Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				}
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x0000DB78 File Offset: 0x0000BD78
		public override void AddAction(SerializableAction action, float delay)
		{
			this.Actions.Add(new KeyValuePair<float, SerializableAction>(delay, action));
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000DB90 File Offset: 0x0000BD90
		private FileEntry EncryptAction(SerializableAction action, float delay)
		{
			string str = FileEncrypter.EncryptString(action.GetSaveString(), "das", "UNKNOWN", FastDelayableActionSystem.EncryptionPass, null);
			string dataEntry = delay.ToString("0.0000000000", CultureInfo.InvariantCulture) + "\n" + str;
			FileEntry result = new FileEntry(dataEntry, this.SeqNum.ToString("000") + ".act");
			this.SeqNum++;
			return result;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000DC0C File Offset: 0x0000BE0C
		public List<FileEntry> GetAllFilesForActions()
		{
			this.SeqNum = 0;
			List<FileEntry> list = new List<FileEntry>();
			for (int i = 0; i < this.Actions.Count; i++)
			{
				list.Add(this.EncryptAction(this.Actions[i].Value, this.Actions[i].Key));
			}
			return list;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000DC80 File Offset: 0x0000BE80
		public void DeserializeActions(List<FileEntry> files)
		{
			this.Actions.Clear();
			for (int i = 0; i < files.Count; i++)
			{
				try
				{
					string data = this.folder.files[i].data;
					int num = data.IndexOf('\n');
					string value = data.Substring(0, num);
					string data2 = data.Substring(num + 1);
					float key = (float)Convert.ToDouble(value, CultureInfo.InvariantCulture);
					string s = FileEncrypter.DecryptString(data2, FastDelayableActionSystem.EncryptionPass)[2];
					using (Stream stream = Utils.GenerateStreamFromString(s))
					{
						try
						{
							XmlReader rdr = XmlReader.Create(stream);
							SerializableAction value2 = SerializableAction.Deserialize(rdr);
							this.Actions.Add(new KeyValuePair<float, SerializableAction>(key, value2));
						}
						catch (Exception ex)
						{
							if (OS.DEBUG_COMMANDS)
							{
								Utils.AppendToWarningsFile("Error deserializing action in fast action host :\n" + Utils.GenerateReportFromExceptionCompact(ex));
							}
						}
					}
				}
				catch (Exception ex)
				{
					if (OS.DEBUG_COMMANDS)
					{
						Utils.AppendToWarningsFile(string.Concat(new object[]
						{
							"Error deserializing action ",
							i,
							" in fast action host :\n",
							Utils.GenerateReportFromExceptionCompact(ex)
						}));
					}
				}
			}
		}

		// Token: 0x040000EA RID: 234
		private static string EncryptionPass = "dasencrypt";

		// Token: 0x040000EB RID: 235
		private Folder folder;

		// Token: 0x040000EC RID: 236
		internal List<KeyValuePair<float, SerializableAction>> Actions = new List<KeyValuePair<float, SerializableAction>>();

		// Token: 0x040000ED RID: 237
		private int SeqNum = 0;
	}
}
