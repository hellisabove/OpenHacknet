using System;
using System.Globalization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SDL2;

namespace Hacknet
{
	// Token: 0x0200016C RID: 364
	public static class SettingsLoader
	{
		// Token: 0x0600091C RID: 2332 RVA: 0x0009676C File Offset: 0x0009496C
		private static string GetSettingsPath()
		{
			string text = SDL.SDL_GetPlatform();
			string result;
			if (text.Equals("Linux"))
			{
				string text2 = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
				if (string.IsNullOrEmpty(text2))
				{
					text2 = Environment.GetEnvironmentVariable("HOME");
					if (string.IsNullOrEmpty(text2))
					{
						return "Settings.txt";
					}
					text2 += "/.config";
				}
				text2 += "/Hacknet";
				if (!Directory.Exists(text2))
				{
					Directory.CreateDirectory(text2);
				}
				result = text2 + "/Settings.txt";
			}
			else if (text.Equals("Mac OS X"))
			{
				string text2 = Environment.GetEnvironmentVariable("HOME");
				if (string.IsNullOrEmpty(text2))
				{
					result = "Settings.txt";
				}
				else
				{
					text2 += "/Library/Application Support/Hacknet";
					if (!Directory.Exists(text2))
					{
						Directory.CreateDirectory(text2);
					}
					result = text2 + "/Settings.txt";
				}
			}
			else
			{
				if (!text.Equals("Windows"))
				{
					throw new NotSupportedException("Unhandled SDL2 platform!");
				}
				result = "Settings.txt";
			}
			return result;
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00096894 File Offset: 0x00094A94
		public static void checkStatus()
		{
			if (File.Exists(SettingsLoader.settingsPath))
			{
				StreamReader streamReader = new StreamReader(TitleContainer.OpenStream(SettingsLoader.settingsPath));
				string text = streamReader.ReadToEnd();
				string[] separator = new string[]
				{
					"\r\n",
					"\n"
				};
				string[] array = text.Split(separator, StringSplitOptions.None);
				SettingsLoader.resWidth = Convert.ToInt32(array[0]);
				SettingsLoader.resHeight = Convert.ToInt32(array[1]);
				SettingsLoader.isFullscreen = array[2].ToLower().Equals("true");
				if (array.Length > 3)
				{
					string a = array[3].Substring(array[3].IndexOf(' ') + 1);
					PostProcessor.bloomEnabled = (a == "true");
					a = array[4].Substring(array[4].IndexOf(' ') + 1);
					PostProcessor.scanlinesEnabled = (a == "true");
				}
				if (array.Length > 6)
				{
					string a = array[5].Substring(array[5].IndexOf(' ') + 1);
					MusicManager.setIsMuted(a == "true");
					string value = array[6].Substring(array[6].IndexOf(' ') + 1).Trim();
					float volume = MusicManager.getVolume();
					try
					{
						volume = (float)Convert.ToDouble(value, CultureInfo.InvariantCulture);
						MusicManager.setVolume(volume);
					}
					catch (FormatException)
					{
					}
					MusicManager.dataLoadedFromOutsideFile = true;
				}
				if (array.Length > 7)
				{
					string name = array[7].Substring(array[7].IndexOf(' ') + 1);
					GuiData.ActiveFontConfig.name = name;
				}
				if (array.Length > 8)
				{
					string a2 = array[8].Substring(array[8].IndexOf(' ') + 1);
					SettingsLoader.hasEverSaved = (a2 == "True");
				}
				if (array.Length > 9)
				{
					string text2 = array[9].Substring(array[9].IndexOf(' ') + 1);
					SettingsLoader.ShouldMultisample = (text2.ToLower() == "true");
				}
				if (array.Length > 10)
				{
					if (!string.IsNullOrWhiteSpace(array[10]))
					{
						string text3 = array[10].Substring(array[10].IndexOf(' ') + 1);
						if (string.IsNullOrWhiteSpace(text3))
						{
							text3 = "en-us";
						}
						Settings.ActiveLocale = text3;
					}
				}
				if (array.Length > 11)
				{
					if (!string.IsNullOrWhiteSpace(array[11]))
					{
						string text4 = array[11].Substring(array[11].IndexOf(' ') + 1);
						SettingsLoader.ShouldDrawMusicVis = (text4.ToLower().Trim() == "true");
					}
				}
				SettingsLoader.didLoad = true;
			}
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00096B7C File Offset: 0x00094D7C
		public static void writeStatusFile()
		{
			GraphicsDevice graphicsDevice = Game1.getSingleton().GraphicsDevice;
			if (graphicsDevice != null)
			{
				string text = graphicsDevice.PresentationParameters.BackBufferWidth + "\r\n";
				text = text + graphicsDevice.PresentationParameters.BackBufferHeight + "\r\n";
				text += (Game1.getSingleton().graphics.IsFullScreen ? "true" : "false");
				text += "\r\n";
				text = text + "bloom: " + (PostProcessor.bloomEnabled ? "true" : "false") + "\r\n";
				text = text + "scanlines: " + (PostProcessor.scanlinesEnabled ? "true" : "false") + "\r\n";
				text = text + "muted: " + (MusicManager.isMuted ? "true" : "false") + "\r\n";
				text = text + "volume: " + MusicManager.getVolume().ToString(CultureInfo.InvariantCulture) + "\r\n";
				text = text + "fontConfig: " + GuiData.ActiveFontConfig.name + "\r\n";
				object obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"hasSaved: ",
					SettingsLoader.hasEverSaved,
					"\r\n"
				});
				obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"shouldMultisample: ",
					SettingsLoader.ShouldMultisample,
					"\r\n"
				});
				text = text + "defaultLocale: " + Settings.ActiveLocale + "\r\n";
				obj = text;
				text = string.Concat(new object[]
				{
					obj,
					"drawMusicVis: ",
					SettingsLoader.ShouldDrawMusicVis,
					"\r\n"
				});
				Utils.writeToFile(text, SettingsLoader.settingsPath);
			}
		}

		// Token: 0x04000A9B RID: 2715
		public static int resWidth;

		// Token: 0x04000A9C RID: 2716
		public static int resHeight;

		// Token: 0x04000A9D RID: 2717
		public static bool isFullscreen = false;

		// Token: 0x04000A9E RID: 2718
		public static bool didLoad = false;

		// Token: 0x04000A9F RID: 2719
		public static bool hasEverSaved = false;

		// Token: 0x04000AA0 RID: 2720
		public static bool ShouldMultisample = true;

		// Token: 0x04000AA1 RID: 2721
		public static bool ShouldDrawMusicVis = true;

		// Token: 0x04000AA2 RID: 2722
		private static readonly string settingsPath = SettingsLoader.GetSettingsPath();
	}
}
