using System;
using System.IO;
using System.Text;
using Hacknet.Extensions;
using Steamworks;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E9 RID: 233
	public class SteamCloudStorageMethod : BasicStorageMethod
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060004DA RID: 1242 RVA: 0x0004B4F8 File Offset: 0x000496F8
		private string PathPrefix
		{
			get
			{
				string result;
				if (Settings.IsInExtensionMode)
				{
					result = ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName() + "/";
				}
				else
				{
					result = "";
				}
				return result;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060004DB RID: 1243 RVA: 0x0004B534 File Offset: 0x00049734
		public override bool ShouldWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060004DC RID: 1244 RVA: 0x0004B548 File Offset: 0x00049748
		public override bool DidDeserialize
		{
			get
			{
				return this.deserialized;
			}
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x0004B560 File Offset: 0x00049760
		public override void Load()
		{
			if (PlatformAPISettings.Running)
			{
				try
				{
					this.manifest = SaveFileManifest.Deserialize(this);
				}
				catch (NullReferenceException)
				{
				}
				catch (FormatException)
				{
				}
				if (this.manifest == null)
				{
					this.manifest = new SaveFileManifest();
					this.manifest.Save(this, false);
				}
				else
				{
					this.deserialized = true;
				}
			}
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x0004B5EC File Offset: 0x000497EC
		public override bool FileExists(string filename)
		{
			return PlatformAPISettings.Running && SteamRemoteStorage.FileExists(this.PathPrefix + filename);
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x0004B61C File Offset: 0x0004981C
		public override Stream GetFileReadStream(string filename)
		{
			Stream result;
			if (!PlatformAPISettings.Running)
			{
				result = null;
			}
			else
			{
				int num = 2000000;
				byte[] array = new byte[num];
				if (!SteamRemoteStorage.FileExists(this.PathPrefix + filename))
				{
					result = null;
				}
				else
				{
					int count = SteamRemoteStorage.FileRead(this.PathPrefix + filename, array, num);
					string @string = Encoding.UTF8.GetString(array);
					MemoryStream memoryStream = new MemoryStream(array, 0, count);
					result = memoryStream;
				}
			}
			return result;
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x0004B698 File Offset: 0x00049898
		public override void WriteFileData(string filename, byte[] data)
		{
			if (PlatformAPISettings.Running)
			{
				if (!SteamRemoteStorage.FileWrite(this.PathPrefix + filename, data, data.Length))
				{
					Console.WriteLine("Failed to write to steam");
				}
			}
		}

		// Token: 0x060004E1 RID: 1249 RVA: 0x0004B6DC File Offset: 0x000498DC
		public override void WriteFileData(string filename, string data)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			this.WriteFileData(filename, bytes);
		}

		// Token: 0x04000593 RID: 1427
		private bool deserialized = false;
	}
}
