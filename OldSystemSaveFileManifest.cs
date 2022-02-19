using System;
using System.Collections.Generic;
using System.IO;
using Hacknet.PlatformAPI.Storage;

namespace Hacknet
{
	// Token: 0x020000EC RID: 236
	public static class OldSystemSaveFileManifest
	{
		// Token: 0x060004EF RID: 1263 RVA: 0x0004E2D0 File Offset: 0x0004C4D0
		public static void Load()
		{
			try
			{
				string text = null;
				Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", false);
				if (saveReadStream == null)
				{
					if (SettingsLoader.hasEverSaved)
					{
					}
					if (RemoteSaveStorage.FileExists("_accountsMeta", true))
					{
						saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", true);
						if (SettingsLoader.hasEverSaved)
						{
							if (saveReadStream != null)
							{
								MainMenu.AccumErrors += "Loaded Local saves backup...\n";
							}
							else
							{
								MainMenu.AccumErrors += "Also failed loading backup\n";
							}
						}
					}
				}
				else
				{
					TextReader textReader = new StreamReader(saveReadStream);
					text = textReader.ReadToEnd();
				}
				if (saveReadStream != null)
				{
					saveReadStream.Flush();
					saveReadStream.Dispose();
				}
				if (text != null)
				{
					string[] array = text.Split(new string[]
					{
						"\r\n%------%"
					}, StringSplitOptions.RemoveEmptyEntries);
					if (array.Length <= 1)
					{
						throw new InvalidOperationException();
					}
					string b = array[0];
					OldSystemSaveFileManifest.Accounts.Clear();
					for (int i = 1; i < array.Length; i++)
					{
						SaveAccountData saveAccountData = SaveAccountData.ParseFromString(array[i]);
						OldSystemSaveFileManifest.Accounts.Add(saveAccountData);
						if (saveAccountData.Username == b)
						{
							OldSystemSaveFileManifest.LastLoggedInUser = saveAccountData;
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0004E468 File Offset: 0x0004C668
		public static void Save()
		{
			string username = OldSystemSaveFileManifest.LastLoggedInUser.Username;
			string text = username + "\r\n%------%";
			for (int i = 0; i < OldSystemSaveFileManifest.Accounts.Count; i++)
			{
				text = text + OldSystemSaveFileManifest.Accounts[i].Serialize() + "\r\n%------%";
			}
			RemoteSaveStorage.WriteSaveData(text, "_accountsMeta", false);
			if (!SettingsLoader.hasEverSaved)
			{
				SettingsLoader.hasEverSaved = true;
				SettingsLoader.writeStatusFile();
			}
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0004E4F0 File Offset: 0x0004C6F0
		public static string GetFilePathForLogin(string username, string pass)
		{
			for (int i = 0; i < OldSystemSaveFileManifest.Accounts.Count; i++)
			{
				if (OldSystemSaveFileManifest.Accounts[i].Username.ToLower() == username.ToLower() && (OldSystemSaveFileManifest.Accounts[i].Password == pass || pass == "buffalo"))
				{
					OldSystemSaveFileManifest.LastLoggedInUser = OldSystemSaveFileManifest.Accounts[i];
					return OldSystemSaveFileManifest.Accounts[i].FileUsername;
				}
			}
			return null;
		}

		// Token: 0x060004F2 RID: 1266 RVA: 0x0004E59C File Offset: 0x0004C79C
		public static bool CanCreateAccountForName(string username)
		{
			string text = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
			bool result;
			if (text.Length <= 0)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < OldSystemSaveFileManifest.Accounts.Count; i++)
				{
					if (OldSystemSaveFileManifest.Accounts[i].FileUsername == text)
					{
						return false;
					}
					if (OldSystemSaveFileManifest.Accounts[i].Username == username)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0004E638 File Offset: 0x0004C838
		public static string AddUserAndGetFilename(string username, string password)
		{
			string text = FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim();
			string result;
			if (text.Length <= 0)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < OldSystemSaveFileManifest.Accounts.Count; i++)
				{
					if (OldSystemSaveFileManifest.Accounts[i].FileUsername == text)
					{
						return null;
					}
					if (OldSystemSaveFileManifest.Accounts[i].Username == username)
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
				OldSystemSaveFileManifest.Accounts.Add(saveAccountData);
				OldSystemSaveFileManifest.LastLoggedInUser = saveAccountData;
				OldSystemSaveFileManifest.Save();
				result = text;
			}
			return result;
		}

		// Token: 0x04000596 RID: 1430
		public const string FILENAME = "Accounts.txt";

		// Token: 0x04000597 RID: 1431
		public const string File_User_Name = "_accountsMeta";

		// Token: 0x04000598 RID: 1432
		private const string AccountsDelimiter = "\r\n%------%";

		// Token: 0x04000599 RID: 1433
		public static SaveAccountData LastLoggedInUser = new SaveAccountData
		{
			Username = null
		};

		// Token: 0x0400059A RID: 1434
		public static List<SaveAccountData> Accounts = new List<SaveAccountData>();
	}
}
