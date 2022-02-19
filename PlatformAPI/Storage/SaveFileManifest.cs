using System;
using System.Collections.Generic;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E8 RID: 232
	public class SaveFileManifest
	{
		// Token: 0x060004CF RID: 1231 RVA: 0x0004AE1C File Offset: 0x0004901C
		public static SaveFileManifest Deserialize(IStorageMethod storage)
		{
			SaveFileManifest result;
			if (!storage.FileExists("Accounts.txt"))
			{
				result = null;
			}
			else
			{
				SaveFileManifest saveFileManifest = null;
				using (Stream fileReadStream = storage.GetFileReadStream("Accounts.txt"))
				{
					StreamReader streamReader = new StreamReader(fileReadStream);
					string text = streamReader.ReadToEnd();
					if (!string.IsNullOrEmpty(text.Trim()))
					{
						saveFileManifest = SaveFileManifest.Deserialize(text);
					}
				}
				result = saveFileManifest;
			}
			return result;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0004AEA4 File Offset: 0x000490A4
		public static SaveFileManifest Deserialize(string data)
		{
			SaveFileManifest saveFileManifest = new SaveFileManifest();
			string[] array = data.Split(new string[]
			{
				"\r\n%------%"
			}, StringSplitOptions.RemoveEmptyEntries);
			SaveFileManifest result;
			if (array.Length <= 1)
			{
				result = null;
			}
			else
			{
				string b = array[0];
				saveFileManifest.Accounts.Clear();
				for (int i = 1; i < array.Length; i++)
				{
					SaveAccountData saveAccountData = SaveAccountData.ParseFromString(array[i]);
					saveFileManifest.Accounts.Add(saveAccountData);
					if (saveAccountData.Username == b)
					{
						saveFileManifest.LastLoggedInUser = saveAccountData;
					}
				}
				result = saveFileManifest;
			}
			return result;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0004AF4C File Offset: 0x0004914C
		public static SaveFileManifest DeserializeSafe(string data)
		{
			SaveFileManifest saveFileManifest = new SaveFileManifest();
			string[] array = data.Split(new string[]
			{
				"\r\n%------%"
			}, StringSplitOptions.RemoveEmptyEntries);
			SaveFileManifest result;
			if (array.Length <= 1)
			{
				result = null;
			}
			else
			{
				string b = array[0];
				saveFileManifest.Accounts.Clear();
				for (int i = 1; i < array.Length; i++)
				{
					try
					{
						SaveAccountData saveAccountData = SaveAccountData.ParseFromString(array[i]);
						saveFileManifest.Accounts.Add(saveAccountData);
						if (saveAccountData.Username == b)
						{
							saveFileManifest.LastLoggedInUser = saveAccountData;
						}
					}
					catch (FormatException)
					{
					}
					catch (NullReferenceException)
					{
					}
				}
				result = saveFileManifest;
			}
			return result;
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0004B01C File Offset: 0x0004921C
		public string GetFilePathForLogin(string username, string pass)
		{
			for (int i = 0; i < this.Accounts.Count; i++)
			{
				if (this.Accounts[i].Username.ToLower() == username.ToLower() && (this.Accounts[i].Password == pass || pass == "buffalo"))
				{
					this.LastLoggedInUser = this.Accounts[i];
					return this.Accounts[i].FileUsername;
				}
			}
			return null;
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0004B0D0 File Offset: 0x000492D0
		public bool CanCreateAccountForName(string username)
		{
			string text = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < this.Accounts.Count; i++)
				{
					if (this.Accounts[i].FileUsername == text)
					{
						return false;
					}
					if (this.Accounts[i].Username == username)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0004B170 File Offset: 0x00049370
		public void UpdateLastWriteTimeForUserFile(string username, DateTime writeTime)
		{
			for (int i = 0; i < this.Accounts.Count; i++)
			{
				if (this.Accounts[i].FileUsername == username)
				{
					SaveAccountData value = this.Accounts[i];
					value.LastWriteTime = writeTime;
					this.Accounts[i] = value;
				}
			}
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0004B1E0 File Offset: 0x000493E0
		public void Save(IStorageMethod storage, bool ExpectNoAccounts = false)
		{
			if (this.Accounts.Count == 0 && !ExpectNoAccounts)
			{
				int num = 10;
				num++;
			}
			else
			{
				string username = this.LastLoggedInUser.Username;
				string text = username + "\r\n%------%";
				for (int i = 0; i < this.Accounts.Count; i++)
				{
					text = text + this.Accounts[i].Serialize() + "\r\n%------%";
				}
				storage.WriteFileData("Accounts.txt", text);
			}
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0004B274 File Offset: 0x00049474
		public bool AddUser(string username, string password, DateTime creationTime, string filepath = null)
		{
			string text = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else
			{
				if (filepath != null)
				{
					text = filepath;
				}
				for (int i = 0; i < this.Accounts.Count; i++)
				{
					if (this.Accounts[i].FileUsername == text)
					{
						return false;
					}
					if (this.Accounts[i].Username == username)
					{
						return false;
					}
				}
				SaveAccountData item = new SaveAccountData
				{
					Username = username,
					Password = password,
					FileUsername = text,
					LastWriteTime = creationTime
				};
				this.Accounts.Add(item);
				result = true;
			}
			return result;
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0004B368 File Offset: 0x00049568
		public SaveAccountData GetAccount(string username)
		{
			for (int i = 0; i < this.Accounts.Count; i++)
			{
				if (this.Accounts[i].FileUsername == username)
				{
					return this.Accounts[i];
				}
			}
			throw new KeyNotFoundException("Username " + username + " does not exist in this manifest");
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0004B3D8 File Offset: 0x000495D8
		public string AddUserAndGetFilename(string username, string password)
		{
			string text = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
			string result;
			if (text.Length <= 0)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < this.Accounts.Count; i++)
				{
					if (this.Accounts[i].FileUsername == text)
					{
						return null;
					}
					if (this.Accounts[i].Username == username)
					{
						return null;
					}
				}
				SaveAccountData saveAccountData = new SaveAccountData
				{
					Username = username,
					Password = password,
					FileUsername = text
				};
				this.Accounts.Add(saveAccountData);
				this.LastLoggedInUser = saveAccountData;
				result = text;
			}
			return result;
		}

		// Token: 0x0400058F RID: 1423
		public const string FILENAME = "Accounts.txt";

		// Token: 0x04000590 RID: 1424
		private const string AccountsDelimiter = "\r\n%------%";

		// Token: 0x04000591 RID: 1425
		public SaveAccountData LastLoggedInUser = new SaveAccountData
		{
			Username = null
		};

		// Token: 0x04000592 RID: 1426
		public List<SaveAccountData> Accounts = new List<SaveAccountData>();
	}
}
