using System;

namespace Hacknet
{
	// Token: 0x02000169 RID: 361
	public static class TrackerCompleteSequence
	{
		// Token: 0x06000916 RID: 2326 RVA: 0x00096460 File Offset: 0x00094660
		internal static void TrackComplete(object osobj, Computer source)
		{
			OS os = (OS)osobj;
			os.TrackersInProgress.Clear();
			Folder folder = source.files.root.searchForFolder("log");
			int i = 0;
			while (i < folder.files.Count)
			{
				string data = folder.files[i].data;
				if (data.Contains(os.thisComputer.ip))
				{
					if (data.Contains("FileCopied") || data.Contains("FileDeleted") || data.Contains("FileMoved"))
					{
						folder.files.RemoveAt(i);
						i--;
					}
				}
				IL_A4:
				i++;
				continue;
				goto IL_A4;
			}
			HackerScriptExecuter.runScript("HackerScripts/TrackSequence.txt", os, source.ip, null);
		}

		// Token: 0x06000917 RID: 2327 RVA: 0x00096540 File Offset: 0x00094740
		internal static bool CompShouldStartTrackerFromLogs(object osobj, Computer c, string targetIP = null)
		{
			OS os = (OS)osobj;
			Folder folder = c.files.root.searchForFolder("log");
			if (targetIP == null)
			{
				targetIP = os.thisComputer.ip;
			}
			for (int i = 0; i < folder.files.Count; i++)
			{
				string data = folder.files[i].data;
				if (data.Contains(targetIP))
				{
					if (data.Contains("FileCopied") || data.Contains("FileDeleted") || data.Contains("FileMoved"))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000918 RID: 2328 RVA: 0x00096608 File Offset: 0x00094808
		internal static void TriggerETAS(object osobj)
		{
			OS os = (OS)osobj;
			os.timerExpired();
		}

		// Token: 0x06000919 RID: 2329 RVA: 0x00096624 File Offset: 0x00094824
		internal static void FlagNextForkbombCompletionToTrace(string source)
		{
			TrackerCompleteSequence.NextCompleteForkbombShouldTrace = true;
			TrackerCompleteSequence.ForkbombCompleteTraceIP = source;
		}

		// Token: 0x04000A6A RID: 2666
		public static float MinTrackTime = 10f;

		// Token: 0x04000A6B RID: 2667
		public static float MaxTrackTime = 20f;

		// Token: 0x04000A6C RID: 2668
		public static bool NextCompleteForkbombShouldTrace;

		// Token: 0x04000A6D RID: 2669
		internal static string ForkbombCompleteTraceIP = null;
	}
}
