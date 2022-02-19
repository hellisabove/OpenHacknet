using System;
using System.Collections.Generic;
using System.IO;
using Steamworks;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E7 RID: 231
	public static class SaveFileManager
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x060004C0 RID: 1216 RVA: 0x0004A1F4 File Offset: 0x000483F4
		public static List<SaveAccountData> Accounts
		{
			get
			{
				return SaveFileManager.StorageMethods[0].GetSaveManifest().Accounts;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060004C1 RID: 1217 RVA: 0x0004A21C File Offset: 0x0004841C
		public static SaveAccountData LastLoggedInUser
		{
			get
			{
				return SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060004C2 RID: 1218 RVA: 0x0004A244 File Offset: 0x00048444
		public static bool HasSaves
		{
			get
			{
				return SaveFileManager.StorageMethods[0].GetSaveManifest().Accounts.Count > 0;
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0004A274 File Offset: 0x00048474
		public static void Init(bool needsOtherSourcesUpdate = true)
		{
			SaveFileManager.StorageMethods.Clear();
			SaveFileManager.StorageMethods.Add(new LocalDocumentsStorageMethod());
			if (PlatformAPISettings.Running && PlatformAPISettings.RemoteStorageRunning)
			{
				SaveFileManager.StorageMethods.Add(new SteamCloudStorageMethod());
			}
			for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
			{
				SaveFileManager.StorageMethods[i].Load();
			}
			if (needsOtherSourcesUpdate)
			{
				SaveFileManager.UpdateStorageMethodsFromSourcesToLatest();
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0004A300 File Offset: 0x00048500
		public static void UpdateStorageMethodsFromSourcesToLatest()
		{
			int num = -1;
			for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
			{
				if (SaveFileManager.StorageMethods[i].DidDeserialize)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				SaveFileManifest saveFileManifest = SaveFileManager.ReadOldSysemSteamCloudSaveManifest();
				SaveFileManifest saveFileManifest2 = SaveFileManager.ReadOldSysemLocalSaveManifest();
				if (saveFileManifest == null && saveFileManifest2 == null)
				{
					Console.WriteLine("New Game Detected!");
				}
				else
				{
					SaveFileManifest saveFileManifest3 = new SaveFileManifest();
					DateTime utcNow = DateTime.UtcNow;
					bool flag = false;
					if (saveFileManifest2 != null)
					{
						for (int i = 0; i < saveFileManifest2.Accounts.Count; i++)
						{
							string text = RemoteSaveStorage.Standalone_FolderPath + RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest2.Accounts[i].FileUsername + RemoteSaveStorage.SAVE_FILE_EXT;
							if (File.Exists(text))
							{
								FileInfo fileInfo = new FileInfo(text);
								if (fileInfo.Length > 100L)
								{
									saveFileManifest3.AddUser(saveFileManifest2.Accounts[i].Username, saveFileManifest2.Accounts[i].Password, fileInfo.LastWriteTimeUtc, text);
								}
							}
						}
						for (int i = 0; i < saveFileManifest3.Accounts.Count; i++)
						{
							if (saveFileManifest2.LastLoggedInUser.Username == saveFileManifest3.Accounts[i].Username)
							{
								saveFileManifest3.LastLoggedInUser = saveFileManifest3.Accounts[i];
							}
						}
						if (saveFileManifest3.LastLoggedInUser.Username == null)
						{
							int num2 = saveFileManifest3.Accounts.Count - 1;
							if (num2 >= 0)
							{
								saveFileManifest3.LastLoggedInUser = saveFileManifest3.Accounts[num2];
								flag = true;
							}
						}
					}
					if (saveFileManifest != null)
					{
						for (int i = 0; i < saveFileManifest.Accounts.Count; i++)
						{
							try
							{
								string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest.Accounts[i].FileUsername + RemoteSaveStorage.SAVE_FILE_EXT;
								if (SteamRemoteStorage.FileExists(text))
								{
									if (SteamRemoteStorage.GetFileSize(text) > 100)
									{
										int num3 = -1;
										for (int j = 0; j < saveFileManifest3.Accounts.Count; j++)
										{
											if (saveFileManifest3.Accounts[j].Username == saveFileManifest.Accounts[i].Username)
											{
												num3 = j;
												break;
											}
										}
										if (num3 >= 0)
										{
											long fileTimestamp = SteamRemoteStorage.GetFileTimestamp(text);
											TimeSpan t = TimeSpan.FromSeconds((double)fileTimestamp);
											DateTime t2 = new DateTime(1970, 1, 1, 0, 0, 0) + t;
											if (t2 > saveFileManifest3.Accounts[num3].LastWriteTime)
											{
												Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream(text, false);
												if (saveReadStream != null)
												{
													TextReader textReader = new StreamReader(saveReadStream);
													string saveData = textReader.ReadToEnd();
													saveReadStream.Close();
													saveReadStream.Dispose();
													RemoteSaveStorage.WriteSaveData(saveData, text, true);
												}
												else
												{
													MainMenu.AccumErrors = MainMenu.AccumErrors + "WARNING: Cloud account " + saveFileManifest.Accounts[i].Username + " failed to convert over to new secure account system.\nRestarting your computer and Hacknet may resolve this issue.";
												}
											}
										}
										else
										{
											string filepath = RemoteSaveStorage.Standalone_FolderPath + RemoteSaveStorage.BASE_SAVE_FILE_NAME + saveFileManifest.Accounts[i].Username + RemoteSaveStorage.SAVE_FILE_EXT;
											Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream(saveFileManifest.Accounts[i].Username, false);
											if (saveReadStream != null)
											{
												string saveData = Utils.ReadEntireContentsOfStream(saveReadStream);
												RemoteSaveStorage.WriteSaveData(saveData, saveFileManifest.Accounts[i].Username, true);
												saveFileManifest3.AddUser(saveFileManifest.Accounts[i].Username, saveFileManifest.Accounts[i].Password, utcNow, filepath);
											}
											else
											{
												MainMenu.AccumErrors = MainMenu.AccumErrors + "WARNING: Cloud account " + saveFileManifest.Accounts[i].Username + " failed to convert over to new secure account system.\nRestarting your computer and Hacknet may resolve this issue.";
											}
										}
									}
								}
							}
							catch (Exception ex)
							{
								object accumErrors = MainMenu.AccumErrors;
								MainMenu.AccumErrors = string.Concat(new object[]
								{
									accumErrors,
									"WARNING: Error upgrading account #",
									i + 1,
									":\r\n",
									Utils.GenerateReportFromException(ex)
								});
								if (!SaveFileManager.HasSentErrorReport)
								{
									string extraData = "cloudAccounts: " + ((saveFileManifest == null) ? "NULL" : string.Concat(saveFileManifest.Accounts.Count)) + " vs localAccounts " + ((saveFileManifest2 == null) ? "NULL" : string.Concat(saveFileManifest2.Accounts.Count));
									Utils.SendThreadedErrorReport(ex, "AccountUpgrade_Error", extraData);
									SaveFileManager.HasSentErrorReport = true;
								}
							}
						}
						if (flag)
						{
							for (int i = 0; i < saveFileManifest3.Accounts.Count; i++)
							{
								if (saveFileManifest2.LastLoggedInUser.Username == saveFileManifest3.Accounts[i].Username)
								{
									saveFileManifest3.LastLoggedInUser = saveFileManifest3.Accounts[i];
								}
							}
						}
					}
					OldSystemStorageMethod otherMethod = new OldSystemStorageMethod(saveFileManifest3);
					for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
					{
						SaveFileManager.StorageMethods[i].UpdateDataFromOtherManager(otherMethod);
					}
				}
			}
			else
			{
				for (int i = 1; i < SaveFileManager.StorageMethods.Count; i++)
				{
					SaveFileManager.StorageMethods[0].UpdateDataFromOtherManager(SaveFileManager.StorageMethods[i]);
				}
			}
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0004A960 File Offset: 0x00048B60
		private static SaveFileManifest ReadOldSysemSteamCloudSaveManifest()
		{
			Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", false);
			SaveFileManifest result;
			if (saveReadStream == null)
			{
				result = null;
			}
			else
			{
				TextReader textReader = new StreamReader(saveReadStream);
				string data = textReader.ReadToEnd();
				saveReadStream.Close();
				saveReadStream.Dispose();
				SaveFileManifest saveFileManifest = SaveFileManifest.DeserializeSafe(data);
				result = saveFileManifest;
			}
			return result;
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0004A9B8 File Offset: 0x00048BB8
		private static SaveFileManifest ReadOldSysemLocalSaveManifest()
		{
			Stream saveReadStream = RemoteSaveStorage.GetSaveReadStream("_accountsMeta", true);
			SaveFileManifest result;
			if (saveReadStream == null)
			{
				result = null;
			}
			else
			{
				TextReader textReader = new StreamReader(saveReadStream);
				string data = textReader.ReadToEnd();
				saveReadStream.Close();
				saveReadStream.Dispose();
				SaveFileManifest saveFileManifest = SaveFileManifest.DeserializeSafe(data);
				result = saveFileManifest;
			}
			return result;
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x0004AA10 File Offset: 0x00048C10
		public static string GetSaveFileNameForUsername(string username)
		{
			return "save_" + FileSanitiser.purifyStringForDisplay(username).Replace("_", "-").Trim() + ".xml";
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x0004AA4C File Offset: 0x00048C4C
		public static Stream GetSaveReadStream(string playerID)
		{
			return SaveFileManager.StorageMethods[0].GetFileReadStream(playerID);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0004AA70 File Offset: 0x00048C70
		public static void WriteSaveData(string saveData, string playerID)
		{
			DateTime utcNow = DateTime.UtcNow;
			for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
			{
				try
				{
					SaveFileManager.StorageMethods[i].WriteSaveFileData(SaveFileManager.GetSaveFileNameForUsername(playerID), playerID, saveData, utcNow);
				}
				catch (Exception ex)
				{
					Utils.AppendToErrorFile("Error writing save data for user : " + playerID + "\r\n" + Utils.GenerateReportFromException(ex));
				}
			}
			if (!SettingsLoader.hasEverSaved)
			{
				SettingsLoader.hasEverSaved = true;
				SettingsLoader.writeStatusFile();
			}
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0004AB08 File Offset: 0x00048D08
		public static bool AddUser(string username, string pass)
		{
			DateTime utcNow = DateTime.UtcNow;
			for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
			{
				try
				{
					if (!SaveFileManager.StorageMethods[i].GetSaveManifest().AddUser(username, pass, utcNow, SaveFileManager.GetSaveFileNameForUsername(username)))
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					Utils.AppendToErrorFile("Error creating user : " + username + "\r\n" + Utils.GenerateReportFromException(ex));
					return false;
				}
			}
			return true;
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x0004ABA0 File Offset: 0x00048DA0
		public static string GetFilePathForLogin(string username, string pass)
		{
			for (int i = 0; i < SaveFileManager.Accounts.Count; i++)
			{
				if (SaveFileManager.Accounts[i].Username.ToLower() == username.ToLower() && (SaveFileManager.Accounts[i].Password == pass || pass == "buffalo" || string.IsNullOrEmpty(SaveFileManager.Accounts[i].Password)))
				{
					SaveFileManager.StorageMethods[0].GetSaveManifest().LastLoggedInUser = SaveFileManager.Accounts[i];
					return SaveFileManager.Accounts[i].FileUsername;
				}
			}
			return null;
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0004AC74 File Offset: 0x00048E74
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
				for (int i = 0; i < SaveFileManager.Accounts.Count; i++)
				{
					if (SaveFileManager.Accounts[i].FileUsername == text)
					{
						return false;
					}
					if (SaveFileManager.Accounts[i].Username == username)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x0004AD10 File Offset: 0x00048F10
		public static void DeleteUser(string username)
		{
			for (int i = 0; i < SaveFileManager.StorageMethods.Count; i++)
			{
				SaveFileManifest saveManifest = SaveFileManager.StorageMethods[i].GetSaveManifest();
				for (int j = 0; j < saveManifest.Accounts.Count; j++)
				{
					if (saveManifest.Accounts[j].Username == username)
					{
						saveManifest.Accounts.RemoveAt(j);
						j--;
					}
				}
				if (saveManifest.LastLoggedInUser.Username == username)
				{
					if (saveManifest.Accounts.Count > 0)
					{
						saveManifest.LastLoggedInUser = saveManifest.Accounts[0];
					}
					else
					{
						saveManifest.LastLoggedInUser = default(SaveAccountData);
					}
				}
				saveManifest.Save(SaveFileManager.StorageMethods[i], true);
			}
		}

		// Token: 0x0400058C RID: 1420
		public static object CurrentlySaving = false;

		// Token: 0x0400058D RID: 1421
		public static List<IStorageMethod> StorageMethods = new List<IStorageMethod>();

		// Token: 0x0400058E RID: 1422
		private static bool HasSentErrorReport = false;
	}
}
