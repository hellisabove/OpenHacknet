using System;
using System.IO;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E5 RID: 229
	public class OldSystemStorageMethod : IStorageMethod
	{
		// Token: 0x060004B2 RID: 1202 RVA: 0x00049F8C File Offset: 0x0004818C
		public OldSystemStorageMethod(SaveFileManifest manifest)
		{
			this.manifest = manifest;
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x00049F9E File Offset: 0x0004819E
		public void Load()
		{
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x00049FA4 File Offset: 0x000481A4
		public SaveFileManifest GetSaveManifest()
		{
			return this.manifest;
		}

		// Token: 0x060004B5 RID: 1205 RVA: 0x00049FBC File Offset: 0x000481BC
		public Stream GetFileReadStream(string filename)
		{
			return File.OpenRead(filename);
		}

		// Token: 0x060004B6 RID: 1206 RVA: 0x00049FD4 File Offset: 0x000481D4
		public bool FileExists(string filename)
		{
			return File.Exists(filename);
		}

		// Token: 0x060004B7 RID: 1207 RVA: 0x00049FEC File Offset: 0x000481EC
		public void WriteFileData(string filename, byte[] data)
		{
			Utils.SafeWriteToFile(data, filename);
		}

		// Token: 0x060004B8 RID: 1208 RVA: 0x00049FF7 File Offset: 0x000481F7
		public void WriteFileData(string filename, string data)
		{
			Utils.SafeWriteToFile(data, filename);
		}

		// Token: 0x060004B9 RID: 1209 RVA: 0x0004A002 File Offset: 0x00048202
		public void UpdateDataFromOtherManager(IStorageMethod otherMethod)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x060004BA RID: 1210 RVA: 0x0004A00C File Offset: 0x0004820C
		public bool ShouldWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x060004BB RID: 1211 RVA: 0x0004A020 File Offset: 0x00048220
		public bool DidDeserialize
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060004BC RID: 1212 RVA: 0x0004A033 File Offset: 0x00048233
		public void WriteSaveFileData(string filename, string username, string data, DateTime utcSaveFileTime)
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000586 RID: 1414
		private SaveFileManifest manifest;
	}
}
