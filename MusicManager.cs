using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x02000140 RID: 320
	public static class MusicManager
	{
		// Token: 0x060007A6 RID: 1958 RVA: 0x0007DCC0 File Offset: 0x0007BEC0
		public static void init(ContentManager content)
		{
			try
			{
				MusicManager.contentManager = content;
				MusicManager.currentSongName = "Music\\Revolve";
				MusicManager.curentSong = content.Load<Song>(MusicManager.currentSongName);
				MusicManager.isPlaying = false;
				if (!MusicManager.dataLoadedFromOutsideFile)
				{
					MusicManager.isMuted = false;
					MediaPlayer.Volume = 0.3f;
				}
				MusicManager.destinationVolume = MediaPlayer.Volume;
				MusicManager.state = 3;
				MusicManager.initialized = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Music Error: Could not initialize first song load\r\n" + Utils.GenerateReportFromException(ex));
			}
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x0007DD58 File Offset: 0x0007BF58
		public static void playSong()
		{
			try
			{
				if (!MusicManager.isPlaying && MusicManager.initialized)
				{
					if (!Settings.soundDisabled)
					{
						MediaPlayer.Play(MusicManager.curentSong);
						MediaPlayer.IsRepeating = true;
					}
					MusicManager.isPlaying = true;
					MusicManager.state = 0;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error playing " + Utils.GenerateReportFromException(ex));
			}
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x0007DDD8 File Offset: 0x0007BFD8
		public static void toggleMute()
		{
			MusicManager.isMuted = !MusicManager.isMuted;
			MediaPlayer.IsMuted = MusicManager.isMuted;
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x0007DDF3 File Offset: 0x0007BFF3
		public static void setIsMuted(bool muted)
		{
			MusicManager.isMuted = muted;
			MediaPlayer.IsMuted = MusicManager.isMuted;
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x0007DE07 File Offset: 0x0007C007
		public static void stop()
		{
			MediaPlayer.Stop();
			MusicManager.isPlaying = false;
			MusicManager.state = 3;
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x0007DE1C File Offset: 0x0007C01C
		public static float getVolume()
		{
			return MusicManager.destinationVolume;
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x0007DE33 File Offset: 0x0007C033
		public static void setVolume(float volume)
		{
			MediaPlayer.Volume = volume;
			MusicManager.destinationVolume = volume;
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x0007DE44 File Offset: 0x0007C044
		public static void playSongImmediatley(string songname)
		{
			if (MusicManager.currentSongName != songname)
			{
				try
				{
					MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error switching to song: " + songname + "\r\n" + Utils.GenerateReportFromException(ex));
				}
			}
			if (MusicManager.curentSong != null)
			{
				MusicManager.isPlaying = false;
				MusicManager.currentSongName = songname;
				MusicManager.playSong();
				MusicManager.setVolume(MusicManager.destinationVolume);
			}
		}

		// Token: 0x060007AE RID: 1966 RVA: 0x0007DEE0 File Offset: 0x0007C0E0
		public static void loadAsCurrentSong(string songname)
		{
			try
			{
				MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
				MusicManager.nextSongName = songname;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error switching to song as current: " + songname + "\r\n" + Utils.GenerateReportFromException(ex));
			}
			if (MusicManager.curentSong != null)
			{
				MusicManager.isPlaying = false;
				MusicManager.currentSongName = songname;
			}
		}

		// Token: 0x060007AF RID: 1967 RVA: 0x0007DF5C File Offset: 0x0007C15C
		public static void loadAsCurrentSongUnsafe(string songname)
		{
			MusicManager.curentSong = MusicManager.contentManager.Load<Song>(songname);
			if (MusicManager.curentSong != null)
			{
				MusicManager.isPlaying = false;
				MusicManager.currentSongName = songname;
			}
		}

		// Token: 0x060007B0 RID: 1968 RVA: 0x0007DF9C File Offset: 0x0007C19C
		public static void transitionToSong(string songName)
		{
			try
			{
				if (MusicManager.currentSongName != songName)
				{
					Thread thread = new Thread(new ThreadStart(MusicManager.loadSong));
					thread.IsBackground = true;
					MusicManager.nextSongName = songName;
					thread.Start();
					Console.WriteLine("Started song loader thread");
					MusicManager.state = 1;
					MusicManager.fadeTimer = MusicManager.FADE_TIME;
					MusicManager.currentSongName = songName;
				}
			}
			catch
			{
				Console.WriteLine("Error transitioning to Song");
			}
		}

		// Token: 0x060007B1 RID: 1969 RVA: 0x0007E02C File Offset: 0x0007C22C
		private static void loadSong()
		{
			try
			{
				MusicManager.nextSong = MusicManager.contentManager.Load<Song>(MusicManager.nextSongName);
				if (MusicManager.loadedSongs.ContainsKey(MusicManager.nextSongName))
				{
					MusicManager.nextSong = MusicManager.loadedSongs[MusicManager.nextSongName];
				}
				else
				{
					MusicManager.loadedSongs.Add(MusicManager.nextSongName, MusicManager.nextSong);
				}
			}
			catch (ArgumentException)
			{
			}
			catch (ContentLoadException ex)
			{
				if (OS.TestingPassOnly)
				{
					throw ex;
				}
				if (OS.currentInstance != null)
				{
					OS.currentInstance.write(ex.ToString());
					OS.currentInstance.write(ex.Message);
				}
			}
		}

		// Token: 0x060007B2 RID: 1970 RVA: 0x0007E0FC File Offset: 0x0007C2FC
		public static void Update(float t)
		{
			switch (MusicManager.state)
			{
			case 0:
				MusicManager.fadeVolume = MusicManager.destinationVolume;
				MusicManager.FADE_TIME = MusicManager.DEFAULT_FADE_TIME;
				break;
			case 1:
				MusicManager.fadeTimer -= t;
				MusicManager.fadeVolume = MusicManager.destinationVolume * (MusicManager.fadeTimer / MusicManager.FADE_TIME);
				if (MusicManager.fadeVolume <= 0f)
				{
					if (MusicManager.nextSong != null)
					{
						MusicManager.state = 2;
						MediaPlayer.Volume = 0f;
						MusicManager.curentSong = MusicManager.nextSong;
						MusicManager.nextSong = null;
						MusicManager.fadeTimer = MusicManager.FADE_TIME;
						if (!Settings.soundDisabled && !MusicManager.IsMediaPlayerCrashDisabled)
						{
							try
							{
								MediaPlayer.Play(MusicManager.curentSong);
							}
							catch (InvalidOperationException)
							{
								MusicManager.IsMediaPlayerCrashDisabled = true;
								if (OS.currentInstance != null && OS.currentInstance.terminal != null)
								{
									OS.currentInstance.write("-------------------------------");
									OS.currentInstance.write("-------------WARNING-----------");
									OS.currentInstance.write("-------------------------------");
									OS.currentInstance.write("HacknetOS VM Audio hook could not be established.");
									OS.currentInstance.write("Music Playback Disabled - Media Player (VM Hook:WindowsMediaPlayer)");
									OS.currentInstance.write("Has been uninstalled or disabled.");
									OS.currentInstance.write("-------------------------------");
									OS.currentInstance.write("-------------WARNING-----------");
									OS.currentInstance.write("-------------------------------");
								}
							}
						}
					}
					else
					{
						MusicManager.fadeVolume = 0f;
					}
				}
				else
				{
					MediaPlayer.Volume = MusicManager.fadeVolume;
				}
				break;
			case 2:
				MusicManager.fadeTimer -= t;
				MusicManager.fadeVolume = MusicManager.destinationVolume * (1f - MusicManager.fadeTimer / MusicManager.FADE_TIME);
				if (MusicManager.fadeVolume >= MusicManager.destinationVolume)
				{
					MediaPlayer.Volume = MusicManager.destinationVolume;
					MusicManager.state = 0;
				}
				else
				{
					MediaPlayer.Volume = MusicManager.fadeVolume;
				}
				break;
			}
		}

		// Token: 0x040008B1 RID: 2225
		private const int PLAYING = 0;

		// Token: 0x040008B2 RID: 2226
		private const int FADING_OUT = 1;

		// Token: 0x040008B3 RID: 2227
		private const int FADING_IN = 2;

		// Token: 0x040008B4 RID: 2228
		private const int STOPPED = 3;

		// Token: 0x040008B5 RID: 2229
		private static float DEFAULT_FADE_TIME = Settings.isConventionDemo ? 0.5f : 2f;

		// Token: 0x040008B6 RID: 2230
		public static float FADE_TIME = MusicManager.DEFAULT_FADE_TIME;

		// Token: 0x040008B7 RID: 2231
		public static Song curentSong;

		// Token: 0x040008B8 RID: 2232
		private static Song nextSong;

		// Token: 0x040008B9 RID: 2233
		private static string nextSongName;

		// Token: 0x040008BA RID: 2234
		public static bool isPlaying;

		// Token: 0x040008BB RID: 2235
		public static bool isMuted;

		// Token: 0x040008BC RID: 2236
		public static string currentSongName;

		// Token: 0x040008BD RID: 2237
		private static float destinationVolume;

		// Token: 0x040008BE RID: 2238
		private static float fadeVolume;

		// Token: 0x040008BF RID: 2239
		private static float fadeTimer = 0f;

		// Token: 0x040008C0 RID: 2240
		private static int state;

		// Token: 0x040008C1 RID: 2241
		private static ContentManager contentManager;

		// Token: 0x040008C2 RID: 2242
		private static bool initialized = false;

		// Token: 0x040008C3 RID: 2243
		public static bool dataLoadedFromOutsideFile = false;

		// Token: 0x040008C4 RID: 2244
		private static Dictionary<string, Song> loadedSongs = new Dictionary<string, Song>();

		// Token: 0x040008C5 RID: 2245
		private static bool IsMediaPlayerCrashDisabled = false;
	}
}
