using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E3 RID: 227
	public class BasicStorageMethod : IStorageMethod
	{
		// Token: 0x0600049E RID: 1182 RVA: 0x00049878 File Offset: 0x00047A78
		public virtual void Load()
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x00049880 File Offset: 0x00047A80
		public virtual SaveFileManifest GetSaveManifest()
		{
			return this.manifest;
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x00049898 File Offset: 0x00047A98
		public virtual Stream GetFileReadStream(string filename)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004A1 RID: 1185 RVA: 0x000498A0 File Offset: 0x00047AA0
		public virtual bool FileExists(string filename)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004A2 RID: 1186 RVA: 0x000498A8 File Offset: 0x00047AA8
		public virtual void WriteFileData(string filename, string data)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004A3 RID: 1187 RVA: 0x000498B0 File Offset: 0x00047AB0
		public virtual void WriteFileData(string filename, byte[] data)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060004A4 RID: 1188 RVA: 0x000498B8 File Offset: 0x00047AB8
		public virtual void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime)
		{
			this.WriteFileData(filename, data);
			for (int i = 0; i < this.manifest.Accounts.Count; i++)
			{
				SaveAccountData lastLoggedInUser = this.manifest.Accounts[i];
				if (lastLoggedInUser.Username == username)
				{
					lastLoggedInUser.LastWriteTime = utcSaveFileTime;
					this.manifest.LastLoggedInUser = lastLoggedInUser;
					break;
				}
			}
			this.manifest.Save(this, false);
		}

		// Token: 0x060004A5 RID: 1189 RVA: 0x0004993C File Offset: 0x00047B3C
		public virtual void UpdateDataFromOtherManager(IStorageMethod otherMethod)
		{
			string username = this.manifest.LastLoggedInUser.Username;
			SaveFileManifest saveManifest = otherMethod.GetSaveManifest();
			for (int i = 0; i < saveManifest.Accounts.Count; i++)
			{
				SaveAccountData saveAccountData = saveManifest.Accounts[i];
				bool flag = false;
				for (int j = 0; j < this.manifest.Accounts.Count; j++)
				{
					SaveAccountData saveAccountData2 = this.manifest.Accounts[j];
					if (saveAccountData2.Username == saveAccountData.Username)
					{
						flag = true;
						TimeSpan timeSpan = saveAccountData.LastWriteTime - saveAccountData2.LastWriteTime;
						if (saveAccountData.LastWriteTime > saveAccountData2.LastWriteTime && timeSpan.TotalSeconds > 5.0)
						{
							Stream fileReadStream = otherMethod.GetFileReadStream(saveAccountData.FileUsername);
							if (fileReadStream != null)
							{
								string text = Utils.ReadEntireContentsOfStream(fileReadStream);
								if (text.Length > 100)
								{
									this.WriteFileData(saveAccountData2.FileUsername, text);
								}
							}
						}
						break;
					}
				}
				if (!flag)
				{
					Stream fileReadStream2 = otherMethod.GetFileReadStream(saveAccountData.FileUsername);
					if (fileReadStream2 != null)
					{
						string saveFileNameForUsername = SaveFileManager.GetSaveFileNameForUsername(saveAccountData.Username);
						this.manifest.AddUser(saveAccountData.Username, saveAccountData.Password, DateTime.UtcNow, saveFileNameForUsername);
						string text = Utils.ReadEntireContentsOfStream(fileReadStream2);
						this.WriteFileData(saveFileNameForUsername, text);
					}
				}
			}
			for (int j = 0; j < this.manifest.Accounts.Count; j++)
			{
				if (this.manifest.Accounts[j].Username == username)
				{
					this.manifest.LastLoggedInUser = this.manifest.Accounts[j];
				}
			}
			if (this.manifest.LastLoggedInUser.Username == null && this.manifest.Accounts.Count > 0)
			{
				this.manifest.LastLoggedInUser = this.manifest.Accounts[0];
			}
			this.manifest.Save(this, false);
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00049BC3 File Offset: 0x00047DC3
		public virtual bool ShouldWrite
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060004A7 RID: 1191 RVA: 0x00049BCB File Offset: 0x00047DCB
		public virtual bool DidDeserialize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x04000583 RID: 1411
		protected SaveFileManifest manifest;
	}
}
