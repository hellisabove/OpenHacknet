using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E2 RID: 226
	public interface IStorageMethod
	{
		// Token: 0x06000494 RID: 1172
		void Load();

		// Token: 0x06000495 RID: 1173
		SaveFileManifest GetSaveManifest();

		// Token: 0x06000496 RID: 1174
		Stream GetFileReadStream(string filename);

		// Token: 0x06000497 RID: 1175
		bool FileExists(string filename);

		// Token: 0x06000498 RID: 1176
		void WriteFileData(string filename, string data);

		// Token: 0x06000499 RID: 1177
		void WriteFileData(string filename, byte[] data);

		// Token: 0x0600049A RID: 1178
		void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime);

		// Token: 0x0600049B RID: 1179
		void UpdateDataFromOtherManager(IStorageMethod otherMethod);

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600049C RID: 1180
		bool ShouldWrite { get; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600049D RID: 1181
		bool DidDeserialize { get; }
	}
}
