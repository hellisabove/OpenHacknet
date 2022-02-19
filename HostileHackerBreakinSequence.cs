using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Hacknet.Extensions;
using SDL2;

namespace Hacknet
{
	// Token: 0x02000022 RID: 34
	public static class HostileHackerBreakinSequence
	{
		// Token: 0x060000DE RID: 222 RVA: 0x0000DE10 File Offset: 0x0000C010
		private static string GetBaseDirectory()
		{
			string text = SDL.SDL_GetPlatform();
			string text2;
			if (text.Equals("Linux"))
			{
				text2 = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = Environment.GetEnvironmentVariable("HOME");
					if (string.IsNullOrEmpty(text2))
					{
						text2 = ".";
					}
					else
					{
						text2 = Path.Combine(text2, ".local/share");
					}
				}
			}
			else if (text.Equals("Mac OS X"))
			{
				text2 = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = ".";
				}
				else
				{
					text2 = Path.Combine(text2, "Library/Application Support");
				}
			}
			else
			{
				if (!text.Equals("Windows"))
				{
					throw new NotSupportedException("Unhandled SDL2 platform!");
				}
				text2 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/";
			}
			if (Settings.IsInExtensionMode)
			{
				text2 = Path.Combine(text2, ExtensionLoader.ActiveExtensionInfo.GetFoldersafeName());
			}
			text2 = Path.Combine(text2, "Hacknet");
			Console.WriteLine("HHBS-Folderpath: " + text2);
			if (!Directory.Exists(text2))
			{
				Directory.CreateDirectory(text2);
			}
			return text2;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000DF84 File Offset: 0x0000C184
		internal static void Execute(object osobj, Computer source, Computer target)
		{
			Console.WriteLine("Breakin Started!");
			PostProcessor.EndingSequenceFlashOutActive = true;
			PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f;
			OS os = (OS)osobj;
			DateTime now = DateTime.Now;
			os.Flags.AddFlag("startupBreakinTrapActivated");
			os.threadedSaveExecute(true);
			HostileHackerBreakinSequence.CopyHostileFileToLocalSystem();
			Console.WriteLine("Copied files to local system...");
			double num = (DateTime.Now - now).TotalSeconds;
			if (num > 3.0)
			{
				num = 1.5;
			}
			else
			{
				num = 4.0 - num;
			}
			os.delayer.Post(ActionDelayer.Wait(num), delegate
			{
				PostProcessor.EndingSequenceFlashOutActive = false;
				PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
				os.thisComputer.crash(source.ip);
			});
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000E068 File Offset: 0x0000C268
		public static bool IsFirstSuccessfulBootAfterBlockingState(object osobj)
		{
			OS os = (OS)osobj;
			return os.Flags.HasFlag("startupBreakinTrapActivated") && !os.Flags.HasFlag("startupBreakinTrapPassed") && !File.Exists(HostileHackerBreakinSequence.HostileFilePath);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000E0C8 File Offset: 0x0000C2C8
		public static void ReactToFirstSuccesfulBoot(object osobj)
		{
			OS os = (OS)osobj;
			os.Flags.AddFlag("startupBreakinTrapPassed");
			MusicManager.loadAsCurrentSong("DLC\\Music\\World_Chase");
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000E0FC File Offset: 0x0000C2FC
		public static bool IsInBlockingHostileFileState(object osobj)
		{
			Console.WriteLine("Booting up -- checking libs");
			OS os = (OS)osobj;
			if (os.Flags.HasFlag("startupBreakinTrapActivated") && !os.Flags.HasFlag("startupBreakinTrapPassed"))
			{
				if (Environment.OSVersion.Platform == PlatformID.Win32NT)
				{
					return File.Exists(HostileHackerBreakinSequence.HostileFilePath);
				}
				if (Directory.Exists(HostileHackerBreakinSequence.HostileDirectory))
				{
					FileAttributes attributes = new DirectoryInfo(HostileHackerBreakinSequence.HostileDirectory).Attributes;
					return attributes.HasFlag(FileAttributes.ReadOnly) || File.Exists(HostileHackerBreakinSequence.HostileFilePath);
				}
			}
			Console.WriteLine("Libs loaded successfully");
			return false;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000E1C8 File Offset: 0x0000C3C8
		private static void CopyHostileFileToLocalSystem()
		{
			if (!Directory.Exists(HostileHackerBreakinSequence.HostileDirectory))
			{
				Directory.CreateDirectory(HostileHackerBreakinSequence.HostileDirectory);
			}
			try
			{
				if (!File.Exists(HostileHackerBreakinSequence.HostileFilePath))
				{
					File.Copy("Content/DLC/Misc/VMBootloaderTrap.dll", HostileHackerBreakinSequence.HostileFilePath);
					if (Environment.OSVersion.Platform == PlatformID.Win32NT)
					{
						HostileHackerBreakinSequence.LockFileToPreventDeletionWin32(HostileHackerBreakinSequence.HostileFilePath);
					}
					else
					{
						HostileHackerBreakinSequence.LockFileToPreventDeletionUnix(HostileHackerBreakinSequence.HostileDirectory, HostileHackerBreakinSequence.HostileFilePath);
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Utils.AppendToErrorFile("HHBreakinSequence error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException(ex));
				Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException(ex));
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000E28C File Offset: 0x0000C48C
		public static string GetHelpText()
		{
			return File.ReadAllText(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Misc/" + ((Environment.OSVersion.Platform == PlatformID.Win32NT) ? "Win_AllyHelpFile.txt" : "Unix_AllyHelpFile.txt")));
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000E2CC File Offset: 0x0000C4CC
		public static void CopyHelpFile()
		{
			try
			{
				if (!File.Exists(HostileHackerBreakinSequence.HelpFilePath))
				{
					File.Copy(LocalizedFileLoader.GetLocalizedFilepath("Content/DLC/Misc/" + ((Environment.OSVersion.Platform == PlatformID.Win32NT) ? "Win_AllyHelpFile.txt" : "Unix_AllyHelpFile.txt")), HostileHackerBreakinSequence.HelpFilePath);
					if (Environment.OSVersion.Platform != PlatformID.Win32NT)
					{
						Process.Start("chmod", " -x " + HostileHackerBreakinSequence.HelpFilePath);
					}
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				Utils.AppendToErrorFile("HHBreakinSequence error : Insufficient permissions for folder access.\r\n" + Utils.GenerateReportFromException(ex));
				Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException(ex));
			}
			catch (Exception ex2)
			{
				Console.WriteLine("HHSequence Error " + Utils.GenerateReportFromException(ex2));
			}
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000E3B8 File Offset: 0x0000C5B8
		public static void OpenWindowsHelpDocument()
		{
			HostileHackerBreakinSequence.MinimizeAllOpenWindows();
			Process.Start("notepad.exe", HostileHackerBreakinSequence.HelpFilePath);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000E3D4 File Offset: 0x0000C5D4
		public static string OpenTerminal()
		{
			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				try
				{
					if (!File.Exists(HostileHackerBreakinSequence.Win32_BatchFilePath))
					{
						File.Copy("Content/DLC/Misc/Win_OpenCMD.bat", HostileHackerBreakinSequence.Win32_BatchFilePath);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				Process.Start(HostileHackerBreakinSequence.Win32_BatchFilePath);
			}
			else
			{
				string text = SDL.SDL_GetPlatform();
				if (text.Equals("Linux"))
				{
					try
					{
						if (File.Exists("/usr/bin/gsettings"))
						{
							Process process = Process.Start(new ProcessStartInfo("/usr/bin/gsettings", " get org.gnome.desktop.default-applications.terminal exec")
							{
								RedirectStandardOutput = true,
								UseShellExecute = false
							});
							process.WaitForExit();
							string text2 = process.StandardOutput.ReadLine();
							if (!string.IsNullOrEmpty(text2))
							{
								text2 = text2.Replace("'", "");
							}
							if (!string.IsNullOrEmpty(text2))
							{
								Process.Start(new ProcessStartInfo(text2)
								{
									WorkingDirectory = HostileHackerBreakinSequence.BaseDirectory
								});
							}
						}
					}
					catch (Exception)
					{
					}
				}
				else if (text.Equals("Mac OS X"))
				{
					Process.Start("/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal", HostileHackerBreakinSequence.BaseDirectory);
				}
				else
				{
					Console.WriteLine(text);
				}
			}
			return HostileHackerBreakinSequence.BaseDirectory.Replace("\\", "/");
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000E578 File Offset: 0x0000C778
		public static void CrashProgram()
		{
			MusicManager.stop();
			Game1.threadsExiting = true;
			Game1.getSingleton().Exit();
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000E594 File Offset: 0x0000C794
		private static void LockFileToPreventDeletionWin32(string filepath)
		{
			FileSecurity accessControl = File.GetAccessControl(filepath);
			AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, true, typeof(NTAccount));
			accessControl.SetAccessRuleProtection(true, false);
			foreach (object obj in accessRules)
			{
				FileSystemAccessRule rule = (FileSystemAccessRule)obj;
				accessControl.RemoveAccessRule(rule);
			}
			accessControl.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.Delete, AccessControlType.Deny));
			File.SetAccessControl(filepath, accessControl);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000E640 File Offset: 0x0000C840
		private static void LockFileToPreventDeletionUnix(string dir, string file)
		{
			Process.Start("chmod", " 000 " + file).WaitForExit();
			Process.Start("chmod", " 000 " + dir).WaitForExit();
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000E679 File Offset: 0x0000C879
		private static void MinimizeWindow(IntPtr handle)
		{
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000E67C File Offset: 0x0000C87C
		private static void MinimizeAllOpenWindows()
		{
			Process currentProcess = Process.GetCurrentProcess();
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process != currentProcess)
				{
					IntPtr mainWindowHandle = process.MainWindowHandle;
					if (!(mainWindowHandle == IntPtr.Zero))
					{
						HostileHackerBreakinSequence.MinimizeWindow(mainWindowHandle);
					}
				}
			}
		}

		// Token: 0x040000EE RID: 238
		private static readonly string BaseDirectory = HostileHackerBreakinSequence.GetBaseDirectory();

		// Token: 0x040000EF RID: 239
		private static readonly string HelpFilePath = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "VM_Recovery_Guide.txt");

		// Token: 0x040000F0 RID: 240
		private static readonly string Win32_BatchFilePath = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "OpenCMD.bat");

		// Token: 0x040000F1 RID: 241
		private static readonly string HostileDirectory = Path.Combine(HostileHackerBreakinSequence.BaseDirectory, "Libs", "Injected");

		// Token: 0x040000F2 RID: 242
		private static readonly string HostileFilePath = Path.Combine(HostileHackerBreakinSequence.HostileDirectory, "VMBootloaderTrap.dll");
	}
}
