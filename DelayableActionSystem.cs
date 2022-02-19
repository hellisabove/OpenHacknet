using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000020 RID: 32
	public class DelayableActionSystem
	{
		// Token: 0x060000CE RID: 206 RVA: 0x0000D450 File Offset: 0x0000B650
		public DelayableActionSystem(Folder sourceFolder, object osObj)
		{
			DelayableActionSystem <>4__this = this;
			this.folder = sourceFolder;
			OS os = (OS)osObj;
			os.UpdateSubscriptions = (Action<float>)Delegate.Combine(os.UpdateSubscriptions, new Action<float>(delegate(float t)
			{
				<>4__this.Update(t, (OS)osObj);
			}));
		}

		// Token: 0x060000CF RID: 207 RVA: 0x0000D4B6 File Offset: 0x0000B6B6
		internal DelayableActionSystem()
		{
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000D4C4 File Offset: 0x0000B6C4
		private void Update(float t, object osObj)
		{
			for (int i = 0; i < this.folder.files.Count; i++)
			{
				try
				{
					string data = this.folder.files[i].data;
					int num = data.IndexOf('\n');
					string value = data.Substring(0, num);
					string text = data.Substring(num + 1);
					float num2 = (float)Convert.ToDouble(value, CultureInfo.InvariantCulture);
					num2 -= t;
					if (num2 <= 0f)
					{
						string s = FileEncrypter.DecryptString(text, DelayableActionSystem.EncryptionPass)[2];
						using (Stream stream = Utils.GenerateStreamFromString(s))
						{
							try
							{
								XmlReader xmlReader = XmlReader.Create(stream);
								SerializableAction serializableAction = SerializableAction.Deserialize(xmlReader);
								serializableAction.Trigger((OS)osObj);
								xmlReader.Close();
							}
							catch (Exception ex)
							{
								((OS)osObj).write(Utils.GenerateReportFromException(ex));
								Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
							}
						}
						this.folder.files.RemoveAt(i);
						i--;
					}
					else
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(num2.ToString("0.0000000000", CultureInfo.InvariantCulture));
						stringBuilder.Append('\n');
						stringBuilder.Append(text);
						this.folder.files[i].data = stringBuilder.ToString();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000D6A0 File Offset: 0x0000B8A0
		public virtual void InstantlyResolveAllActions(object osObj)
		{
			for (int i = 0; i < this.folder.files.Count; i++)
			{
				string data = this.folder.files[i].data;
				int num = data.IndexOf('\n');
				string data2 = data.Substring(num + 1);
				string s = FileEncrypter.DecryptString(data2, DelayableActionSystem.EncryptionPass)[2];
				using (Stream stream = Utils.GenerateStreamFromString(s))
				{
					XmlReader xmlReader = XmlReader.Create(stream);
					SerializableAction serializableAction = SerializableAction.Deserialize(xmlReader);
					serializableAction.Trigger((OS)osObj);
					xmlReader.Close();
				}
				this.folder.files.RemoveAt(i);
				i--;
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000D780 File Offset: 0x0000B980
		public virtual void AddAction(SerializableAction action, float delay)
		{
			string str = FileEncrypter.EncryptString(action.GetSaveString(), "das", "UNKNOWN", DelayableActionSystem.EncryptionPass, null);
			string dataEntry = delay.ToString("0.0000000000", CultureInfo.InvariantCulture) + "\n" + str;
			FileEntry item = new FileEntry(dataEntry, this.GetNextSeqNumber() + ".act");
			this.folder.files.Add(item);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x0000D7F0 File Offset: 0x0000B9F0
		internal string GetNextSeqNumber()
		{
			int num = 0;
			if (this.folder.files.Count > 0)
			{
				string name = this.folder.files[this.folder.files.Count - 1].name;
				string value = name.Substring(0, name.IndexOf('.'));
				try
				{
					int num2 = Convert.ToInt32(value, CultureInfo.InvariantCulture);
					num2++;
					num = num2;
				}
				catch (Exception)
				{
				}
			}
			return num.ToString("000", CultureInfo.InvariantCulture);
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x0000D89C File Offset: 0x0000BA9C
		internal static DelayableActionSystem FindDelayableActionSystemOnComputer(Computer c)
		{
			IRCDaemon ircdaemon = c.getDaemon(typeof(IRCDaemon)) as IRCDaemon;
			DelayableActionSystem delayedActions;
			if (ircdaemon != null)
			{
				delayedActions = ircdaemon.DelayedActions;
			}
			else
			{
				DLCHubServer dlchubServer = c.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				if (dlchubServer != null)
				{
					delayedActions = dlchubServer.DelayedActions;
				}
				else
				{
					FastActionHost fastActionHost = c.getDaemon(typeof(FastActionHost)) as FastActionHost;
					if (fastActionHost == null)
					{
						throw new InvalidOperationException("Target computer " + c.name + " does not contain a Daemon that supports delayable actions");
					}
					delayedActions = fastActionHost.DelayedActions;
				}
			}
			return delayedActions;
		}

		// Token: 0x040000E8 RID: 232
		private static string EncryptionPass = "dasencrypt";

		// Token: 0x040000E9 RID: 233
		private Folder folder;
	}
}
