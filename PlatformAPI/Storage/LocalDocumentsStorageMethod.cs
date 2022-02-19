using System;
using System.IO;
using Hacknet.Extensions;
using SDL2;

namespace Hacknet.PlatformAPI.Storage
{
	// Token: 0x020000E4 RID: 228
	public class LocalDocumentsStorageMethod : BasicStorageMethod
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x060004A9 RID: 1193 RVA: 0x00049BDC File Offset: 0x00047DDC
		private string FullFolderPath
		{
			get
			{
				string result;
				if (Settings.IsInExtensionMode)
				{
					result = this.FolderPath + ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName() + "/";
				}
				else
				{
					result = this.FolderPath;
				}
				return result;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x060004AA RID: 1194 RVA: 0x00049C20 File Offset: 0x00047E20
		public override bool ShouldWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x060004AB RID: 1195 RVA: 0x00049C34 File Offset: 0x00047E34
		public override bool DidDeserialize
		{
			get
			{
				return this.deserialized;
			}
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x00049C4C File Offset: 0x00047E4C
		public override void Load()
		{
			string text = SDL.SDL_GetPlatform();
			if (text.Equals("Linux"))
			{
				this.FolderPath = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
				if (string.IsNullOrEmpty(this.FolderPath))
				{
					this.FolderPath = Environment.GetEnvironmentVariable("HOME");
					if (string.IsNullOrEmpty(this.FolderPath))
					{
						this.FolderPath = "./";
					}
					else
					{
						this.FolderPath += "/.local/share/Hacknet/Accounts/";
					}
				}
				else
				{
					this.FolderPath += "/Hacknet/Accounts/";
				}
			}
			else if (text.Equals("Mac OS X"))
			{
				this.FolderPath = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(this.FolderPath))
				{
					this.FolderPath = "./";
				}
				else
				{
					this.FolderPath += "/Library/Application Support/Hacknet/Accounts/";
				}
			}
			else
			{
				if (!text.Equals("Windows"))
				{
					throw new NotSupportedException("Unhandled SDL2 platform!");
				}
				this.FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Accounts/";
			}
			try
			{
				if (!Directory.Exists(this.FullFolderPath))
				{
					Directory.CreateDirectory(this.FullFolderPath);
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Utils.AppendToErrorFile("Local Documents Storage load error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException(ex));
				this.FolderPath = "Accounts/";
				MainMenu.AccumErrors = MainMenu.AccumErrors + "\r\nERROR: Local Documents Storage Folder is refusing access. Saving accounts to:\r\n" + Path.GetFullPath(this.FolderPath);
			}
			try
			{
				this.manifest = SaveFileManifest.Deserialize(this);
			}
			catch (FormatException ex2)
			{
				this.manifest = null;
				Console.WriteLine("Local Save Manifest Corruption: " + Utils.GenerateReportFromException(ex2));
			}
			catch (NullReferenceException ex3)
			{
				this.manifest = null;
				Console.WriteLine("Local Save Manifest Corruption: " + Utils.GenerateReportFromException(ex3));
			}
			if (this.manifest == null)
			{
				this.manifest = new SaveFileManifest();
				this.manifest.Save(this, true);
			}
			else
			{
				this.deserialized = true;
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x00049EBC File Offset: 0x000480BC
		public override bool FileExists(string filename)
		{
			return File.Exists(this.FullFolderPath + filename);
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x00049EE0 File Offset: 0x000480E0
		public override Stream GetFileReadStream(string filename)
		{
			return File.OpenRead(this.FullFolderPath + filename);
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x00049F04 File Offset: 0x00048104
		public override void WriteFileData(string filename, byte[] data)
		{
			if (!Directory.Exists(this.FullFolderPath))
			{
				Directory.CreateDirectory(this.FullFolderPath);
			}
			Utils.SafeWriteToFile(data, this.FullFolderPath + filename);
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x00049F40 File Offset: 0x00048140
		public override void WriteFileData(string filename, string data)
		{
			if (!Directory.Exists(this.FullFolderPath))
			{
				Directory.CreateDirectory(this.FullFolderPath);
			}
			Utils.SafeWriteToFile(data, this.FullFolderPath + filename);
		}

		// Token: 0x04000584 RID: 1412
		private string FolderPath;

		// Token: 0x04000585 RID: 1413
		private bool deserialized = false;
	}
}
