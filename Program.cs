using System;
using System.IO;
using SDL2;

namespace Hacknet
{
	// Token: 0x02000153 RID: 339
	internal static class Program
	{
		// Token: 0x0600087D RID: 2173 RVA: 0x0008FFD8 File Offset: 0x0008E1D8
		[STAThread]
		private static void Main(string[] args)
		{
			for (int i = 0; i < args.Length; i++)
			{
				string text = args[i];
				if (text.Equals("-disableweb"))
				{
					WebRenderer.Enabled = false;
				}
				if (text.Equals("-disablebackground"))
				{
					Settings.DrawHexBackground = false;
				}
				if (text.Equals("-altmonitor"))
				{
					Settings.StartOnAltMonitor = true;
				}
				if (text.Equals("-enablefc"))
				{
					if (Settings.emergencyForceCompleteEnabled)
					{
						Settings.forceCompleteEnabled = true;
					}
				}
				if (text.Equals("-enabledebug"))
				{
					if (Settings.emergencyDebugCommandsEnabled)
					{
						Settings.debugCommandsEnabled = true;
						OS.DEBUG_COMMANDS = true;
					}
				}
				if (text.ToLower().Equals("-extstart") && Settings.AllowExtensionMode)
				{
					if (i < args.Length - 1)
					{
						string text2 = args[i + 1];
						i++;
						text2 = Path.Combine("Extensions", text2);
						Game1.AutoLoadExtensionPath = text2;
					}
				}
				if (text.ToLower().Equals("-allowextpublish"))
				{
					Settings.AllowExtensionPublish = true;
				}
			}
			using (Game1 game = new Game1())
			{
				try
				{
					game.Run();
				}
				catch (Exception ex)
				{
					SDL.SDL_ShowSimpleMessageBox(SDL.SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR, "SAVE THIS MESSAGE!", ex.ToString(), IntPtr.Zero);
					string text3 = Utils.GenerateReportFromException(ex);
					try
					{
						if (OS.currentInstance != null)
						{
							text3 = text3 + "\r\n\r\n" + OS.currentInstance.Flags.GetSaveString() + "\r\n\r\n";
							object obj = text3;
							text3 = string.Concat(new object[]
							{
								obj,
								"Timer : ",
								OS.currentInstance.timer / 60f,
								"mins\r\n"
							});
							text3 = text3 + "Display cache : " + OS.currentInstance.displayCache + "\r\n";
							text3 = text3 + "String Cache : " + OS.currentInstance.getStringCache + "\r\n";
							text3 = text3 + "Terminal--------------\r\n" + OS.currentInstance.terminal.GetRecentTerminalHistoryString() + "-------------\r\n";
						}
						else
						{
							text3 += "\r\n\r\nOS INSTANCE NULL\r\n\r\n";
						}
						text3 = text3 + "\r\nMenuErrorCache: " + MainMenu.AccumErrors + "\r\n";
					}
					catch (Exception)
					{
					}
					text3 = text3 + "\r\n\r\nPlatform API: " + PlatformAPISettings.Report;
					text3 = text3 + "\r\n\r\nGraphics Device Reset Log: " + Program.GraphicsDeviceResetLog;
					text3 = text3 + "\r\n\r\nCurrent Time: " + DateTime.Now.ToShortTimeString() + "\r\n";
					text3 = text3 + "\r\n\r\nVersion: " + MainMenu.OSVersion + "\r\n";
					text3 += "\r\n\r\nMode: FNA\r\n";
					string text4 = "";
					if (SDL.SDL_GetPlatform().Equals("Windows"))
					{
						text4 = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/My Games/Hacknet/Reports/";
						if (!Directory.Exists(text4))
						{
							Directory.CreateDirectory(text4);
						}
					}
					Utils.writeToFile(text3, text4 + "CrashReport_" + Guid.NewGuid().ToString().Replace(" ", "_") + ".txt");
					Utils.SendRealWorldEmail(string.Concat(new string[]
					{
						"Hackent ",
						MainMenu.OSVersion,
						" Crash ",
						DateTime.Now.ToShortDateString(),
						" ",
						DateTime.Now.ToShortTimeString()
					}), "hacknetbugs+Hacknet@gmail.com", text3);
					throw ex;
				}
			}
		}

		// Token: 0x040009EB RID: 2539
		public static string GraphicsDeviceResetLog = "";
	}
}
