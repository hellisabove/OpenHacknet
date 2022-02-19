using System;
using System.IO;
using System.Text;
using Steamworks;

namespace Hacknet
{
	// Token: 0x02000090 RID: 144
	public static class RemoteSaveStorage
	{
		// Token: 0x060002CC RID: 716 RVA: 0x00028DC4 File Offset: 0x00026FC4
		public static bool FileExists(string playerID, bool forceLocal = false)
		{
			string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
			bool result;
			if (!forceLocal && PlatformAPISettings.Running)
			{
				result = SteamRemoteStorage.FileExists(text);
			}
			else
			{
				result = File.Exists(RemoteSaveStorage.Standalone_FolderPath + text);
			}
			return result;
		}

		// Token: 0x060002CD RID: 717 RVA: 0x00028E18 File Offset: 0x00027018
		public static Stream GetSaveReadStream(string playerID, bool forceLocal = false)
		{
			string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
			Stream result;
			if (!forceLocal && PlatformAPISettings.Running)
			{
				int num = 2000000;
				byte[] array = new byte[num];
				if (!SteamRemoteStorage.FileExists(text))
				{
					result = null;
				}
				else
				{
					int num2 = SteamRemoteStorage.FileRead(text, array, num);
					if (num2 == 0)
					{
						result = null;
					}
					else
					{
						string @string = Encoding.UTF8.GetString(array);
						MemoryStream memoryStream = new MemoryStream(array, 0, num2);
						result = memoryStream;
					}
				}
			}
			else if (File.Exists(RemoteSaveStorage.Standalone_FolderPath + text))
			{
				result = File.OpenRead(RemoteSaveStorage.Standalone_FolderPath + text);
			}
			else
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060002CE RID: 718 RVA: 0x00028EE0 File Offset: 0x000270E0
		public static void WriteSaveData(string saveData, string playerID, bool forcelocal = false)
		{
			string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
			if (!forcelocal || !PlatformAPISettings.Running)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(saveData);
				if (!SteamRemoteStorage.FileWrite(text, bytes, bytes.Length))
				{
					Console.WriteLine("Failed to write to steam");
				}
				try
				{
					Utils.SafeWriteToFile(saveData, RemoteSaveStorage.Standalone_FolderPath + text);
				}
				catch (Exception ex)
				{
					Utils.AppendToErrorFile(Utils.GenerateReportFromException(ex));
				}
			}
			else
			{
				Utils.SafeWriteToFile(saveData, RemoteSaveStorage.Standalone_FolderPath + text);
			}
		}

		// Token: 0x060002CF RID: 719 RVA: 0x00028F8C File Offset: 0x0002718C
		public static void Delete(string playerID)
		{
			string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
			if (PlatformAPISettings.Running)
			{
				SteamRemoteStorage.FileDelete(text);
			}
			else if (Directory.Exists(RemoteSaveStorage.Standalone_FolderPath))
			{
				if (File.Exists(RemoteSaveStorage.Standalone_FolderPath + text))
				{
					File.Delete(RemoteSaveStorage.Standalone_FolderPath + text);
				}
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x00029004 File Offset: 0x00027204
		public static bool CanLoad(string playerID)
		{
			string text = RemoteSaveStorage.BASE_SAVE_FILE_NAME + playerID + RemoteSaveStorage.SAVE_FILE_EXT;
			bool result;
			if (PlatformAPISettings.Running)
			{
				result = SteamRemoteStorage.FileExists(text);
			}
			else
			{
				result = File.Exists(RemoteSaveStorage.Standalone_FolderPath + text);
			}
			return result;
		}

		// Token: 0x04000311 RID: 785
		public static string BASE_SAVE_FILE_NAME = "save";

		// Token: 0x04000312 RID: 786
		public static string SAVE_FILE_EXT = ".xml";

		// Token: 0x04000313 RID: 787
		public static string Standalone_FolderPath = "Accounts/";
	}
}
