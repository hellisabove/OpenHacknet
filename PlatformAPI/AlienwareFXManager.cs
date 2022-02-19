using System;
using System.Collections.Generic;
using System.Text;
using AlienFXManagedWrapper;
using Microsoft.Xna.Framework;

namespace Hacknet.PlatformAPI
{
	// Token: 0x0200008E RID: 142
	public static class AlienwareFXManager
	{
		// Token: 0x060002C1 RID: 705 RVA: 0x00028618 File Offset: 0x00026818
		public static void Init()
		{
			AlienwareFXManager.LightFX = new LightFXController();
			LFX_Result lfx_Result = AlienwareFXManager.LightFX.LFX_Initialize();
			if (lfx_Result == LFX_Result.LFX_Success)
			{
				AlienwareFXManager.LightFX.LFX_GetNumDevices(out AlienwareFXManager.numDevices);
				for (uint num = 0U; num < AlienwareFXManager.numDevices; num += 1U)
				{
					uint num2;
					AlienwareFXManager.LightFX.LFX_GetNumLights(num, out num2);
					AlienwareFXManager.deviceLightCounts.Add(num2);
					List<string> list = new List<string>();
					StringBuilder stringBuilder = new StringBuilder(255);
					for (uint num3 = 0U; num3 < num2; num3 += 1U)
					{
						AlienwareFXManager.LightFX.LFX_GetLightDescription(num, num3, stringBuilder, 255);
						list.Add(stringBuilder.ToString());
					}
					AlienwareFXManager.deviceLightDescriptions.Add(list);
				}
				AlienwareFXManager.TimeStarted = DateTime.Now;
				AlienwareFXManager.IsRunning = true;
			}
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x000286FC File Offset: 0x000268FC
		public static void UpdateForOS(object OS_Obj)
		{
			if (AlienwareFXManager.IsRunning)
			{
				if (OS_Obj != null)
				{
					OS os = (OS)OS_Obj;
					float warningFlashTimer = os.warningFlashTimer;
					bool flag = 1 == 0;
					AlienwareFXManager.UpdateLightsForThemeAndFlash(os);
					AlienwareFXManager.HasUpdatedPostFlash = false;
				}
				else
				{
					double totalSeconds = (DateTime.Now - AlienwareFXManager.TimeStarted).TotalSeconds;
					AlienwareFXManager.CycleAllLights(Color.Red, Color.White, (float)totalSeconds);
				}
			}
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00028774 File Offset: 0x00026974
		private static void UpdateLightsForThemeAndFlash(OS os)
		{
			if (os.thisComputer.disabled)
			{
				AlienwareFXManager.UpdateLightColors(Color.Black, Color.Black, Color.Black, Color.Black);
			}
			else
			{
				float amount = Math.Max(0f, Math.Min(1f, os.warningFlashTimer));
				Color color = Color.Lerp(os.AFX_KeyboardOuter, Color.Red, amount);
				color = Color.Lerp(color, Color.White, os.PorthackCompleteFlashTime / PortHackExe.COMPLETE_LIGHT_FLASH_TIME);
				Color color2 = Color.Lerp(os.AFX_WordLogo, Color.Red, amount);
				color2 = Color.Lerp(color2, Color.White, os.MissionCompleteFlashTime / 3f);
				AlienwareFXManager.UpdateLightColors(color2, Color.Lerp(os.AFX_KeyboardMiddle, Color.Red, amount), color, Color.Lerp(os.AFX_Other, Color.Red, amount));
			}
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00028850 File Offset: 0x00026A50
		private static void CycleAllLights(Color from, Color to, float time)
		{
			double totalSeconds = (DateTime.Now - AlienwareFXManager.LastUpdateTime).TotalSeconds;
			if (totalSeconds >= 0.1)
			{
				for (uint num = 0U; num < AlienwareFXManager.numDevices; num += 1U)
				{
					for (uint num2 = 0U; num2 < AlienwareFXManager.deviceLightCounts[(int)num]; num2 += 1U)
					{
						float num3 = (float)Math.Sin((double)time / (num2 / 2.0));
						num3 = (num3 + 1f) / 2f;
						Color c = Color.Lerp(from, to, num3);
						LFX_ColorStruct lfx_ColorStruct = AlienwareFXManager.Col2LFXC(c);
						AlienwareFXManager.LightFX.LFX_SetLightColor(num, num2, ref lfx_ColorStruct);
					}
				}
				AlienwareFXManager.LightFX.LFX_Update();
				AlienwareFXManager.LastUpdateTime = DateTime.Now;
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00028920 File Offset: 0x00026B20
		private static void UpdateLightColors(Color LogoColor, Color KeyboardMiddleColor, Color KeyboardOuterColor, Color StatusColor)
		{
			if (!(LogoColor == AlienwareFXManager.LastLogoColor) || !(KeyboardMiddleColor == AlienwareFXManager.LastMidKeyColor) || !(KeyboardOuterColor == AlienwareFXManager.LastOutKeyColor) || !(StatusColor == AlienwareFXManager.LastOtherColor))
			{
				double totalSeconds = (DateTime.Now - AlienwareFXManager.LastUpdateTime).TotalSeconds;
				if (totalSeconds >= 0.1)
				{
					for (uint num = 0U; num < AlienwareFXManager.numDevices; num += 1U)
					{
						for (uint num2 = 0U; num2 < AlienwareFXManager.deviceLightCounts[(int)num]; num2 += 1U)
						{
							LFX_ColorStruct lfx_ColorStruct = AlienwareFXManager.Col2LFXC(LogoColor);
							string text = AlienwareFXManager.deviceLightDescriptions[(int)num][(int)num2].ToLower();
							if (text.Contains("keyboard"))
							{
								if (text.Contains("middle"))
								{
									lfx_ColorStruct = AlienwareFXManager.Col2LFXC(KeyboardMiddleColor);
								}
								else
								{
									lfx_ColorStruct = AlienwareFXManager.Col2LFXC(KeyboardOuterColor);
								}
							}
							else if (text.Contains("status"))
							{
								lfx_ColorStruct = AlienwareFXManager.Col2LFXC(StatusColor);
							}
							AlienwareFXManager.LightFX.LFX_SetLightColor(num, num2, ref lfx_ColorStruct);
						}
					}
					AlienwareFXManager.LightFX.LFX_Update();
					AlienwareFXManager.LastLogoColor = LogoColor;
					AlienwareFXManager.LastMidKeyColor = KeyboardMiddleColor;
					AlienwareFXManager.LastOutKeyColor = KeyboardOuterColor;
					AlienwareFXManager.LastOtherColor = StatusColor;
					AlienwareFXManager.LastUpdateTime = DateTime.Now;
				}
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00028A9C File Offset: 0x00026C9C
		private static LFX_ColorStruct Col2LFXC(Color c)
		{
			float num = (float)(c.R + c.G + c.B) / 765f;
			return new LFX_ColorStruct
			{
				red = c.R,
				green = c.G,
				blue = c.B,
				brightness = byte.MaxValue
			};
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00028B0C File Offset: 0x00026D0C
		public static void ReleaseHandle()
		{
			if (AlienwareFXManager.IsRunning)
			{
				AlienwareFXManager.LightFX.LFX_Release();
			}
		}

		// Token: 0x04000301 RID: 769
		private const double MIN_SECONDS_BETWEEN_UPDATES = 0.1;

		// Token: 0x04000302 RID: 770
		public static bool IsRunning = false;

		// Token: 0x04000303 RID: 771
		private static ILightFXController LightFX;

		// Token: 0x04000304 RID: 772
		private static bool HasUpdatedPostFlash = false;

		// Token: 0x04000305 RID: 773
		private static uint numDevices;

		// Token: 0x04000306 RID: 774
		private static List<uint> deviceLightCounts = new List<uint>();

		// Token: 0x04000307 RID: 775
		private static List<List<string>> deviceLightDescriptions = new List<List<string>>();

		// Token: 0x04000308 RID: 776
		private static DateTime TimeStarted;

		// Token: 0x04000309 RID: 777
		private static DateTime LastUpdateTime;

		// Token: 0x0400030A RID: 778
		private static Color LastLogoColor;

		// Token: 0x0400030B RID: 779
		private static Color LastMidKeyColor;

		// Token: 0x0400030C RID: 780
		private static Color LastOutKeyColor;

		// Token: 0x0400030D RID: 781
		private static Color LastOtherColor;
	}
}
