using System;

namespace Hacknet
{
	// Token: 0x0200016B RID: 363
	public static class Settings
	{
		// Token: 0x04000A72 RID: 2674
		public static bool MenuStartup = true;

		// Token: 0x04000A73 RID: 2675
		public static bool slowOSStartup = true;

		// Token: 0x04000A74 RID: 2676
		public static bool osStartsWithTutorial = Settings.slowOSStartup;

		// Token: 0x04000A75 RID: 2677
		public static bool isAlphaDemoMode = false;

		// Token: 0x04000A76 RID: 2678
		public static bool soundDisabled = false;

		// Token: 0x04000A77 RID: 2679
		public static bool debugCommandsEnabled = false;

		// Token: 0x04000A78 RID: 2680
		public static bool testingMenuItemsEnabled = false;

		// Token: 0x04000A79 RID: 2681
		public static bool debugDrawEnabled = false;

		// Token: 0x04000A7A RID: 2682
		public static bool forceCompleteEnabled = false;

		// Token: 0x04000A7B RID: 2683
		public static bool emergencyForceCompleteEnabled = true;

		// Token: 0x04000A7C RID: 2684
		public static bool emergencyDebugCommandsEnabled = true;

		// Token: 0x04000A7D RID: 2685
		public static bool AllTraceTimeSlowed = false;

		// Token: 0x04000A7E RID: 2686
		public static bool FastBootText = false;

		// Token: 0x04000A7F RID: 2687
		public static bool AllowExtensionMode = true;

		// Token: 0x04000A80 RID: 2688
		public static bool AllowExtensionPublish = false;

		// Token: 0x04000A81 RID: 2689
		public static bool EducationSafeBuild = false;

		// Token: 0x04000A82 RID: 2690
		public static string ActiveLocale = "en-us";

		// Token: 0x04000A83 RID: 2691
		public static bool EnableDLC = true;

		// Token: 0x04000A84 RID: 2692
		public static bool isPirateBuild = false;

		// Token: 0x04000A85 RID: 2693
		public static bool sendsDLC1PromoEmailAtEnd = true;

		// Token: 0x04000A86 RID: 2694
		public static bool initShowsTutorial = Settings.osStartsWithTutorial;

		// Token: 0x04000A87 RID: 2695
		public static bool windowed = false;

		// Token: 0x04000A88 RID: 2696
		public static bool IsInExtensionMode = false;

		// Token: 0x04000A89 RID: 2697
		public static bool DrawHexBackground = true;

		// Token: 0x04000A8A RID: 2698
		public static bool StartOnAltMonitor = false;

		// Token: 0x04000A8B RID: 2699
		public static bool isDemoMode = false;

		// Token: 0x04000A8C RID: 2700
		public static bool isPressBuildDemo = false;

		// Token: 0x04000A8D RID: 2701
		public static bool isConventionDemo = false;

		// Token: 0x04000A8E RID: 2702
		public static bool isLockedDemoMode = false;

		// Token: 0x04000A8F RID: 2703
		public static bool isSpecialTestBuild = false;

		// Token: 0x04000A90 RID: 2704
		public static bool lighterColorHexBackground = false;

		// Token: 0x04000A91 RID: 2705
		public static string ConventionLoginName = "Agent";

		// Token: 0x04000A92 RID: 2706
		public static bool MultiLingualDemo = false;

		// Token: 0x04000A93 RID: 2707
		public static bool DLCEnabledDemo = true;

		// Token: 0x04000A94 RID: 2708
		public static bool ShuffleThemeOnDemoStart = true;

		// Token: 0x04000A95 RID: 2709
		public static bool HasLabyrinthsDemoStartMainMenuButton = false;

		// Token: 0x04000A96 RID: 2710
		public static bool ForceEnglish = false;

		// Token: 0x04000A97 RID: 2711
		public static bool IsExpireLocked = false;

		// Token: 0x04000A98 RID: 2712
		public static DateTime ExpireTime = Utils.SafeParseDateTime("10/06/2017 23:59:01");

		// Token: 0x04000A99 RID: 2713
		public static bool isServerMode = false;

		// Token: 0x04000A9A RID: 2714
		public static bool recoverFromErrorsSilently = true;
	}
}
