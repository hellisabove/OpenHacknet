using System;

namespace Hacknet
{
	// Token: 0x02000138 RID: 312
	public static class MissionGenerationParser
	{
		// Token: 0x0600077A RID: 1914 RVA: 0x0007B33E File Offset: 0x0007953E
		public static void init()
		{
			MissionGenerationParser.Path = (MissionGenerationParser.File = (MissionGenerationParser.Client = (MissionGenerationParser.Target = "UNKNOWN")));
			MissionGenerationParser.Comp = "firstGeneratedNode";
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0007B368 File Offset: 0x00079568
		public static string parse(string input)
		{
			return input.Replace("#PATH#", MissionGenerationParser.Path).Replace("#FILE#", MissionGenerationParser.File).Replace("#COMP#", MissionGenerationParser.Comp).Replace("#CLIENT#", MissionGenerationParser.Client).Replace("#TARGET#", MissionGenerationParser.Target).Replace("#OTHER#", MissionGenerationParser.Other).Replace("#LC_CLIENT#", MissionGenerationParser.Client.Replace(' ', '_').ToLower());
		}

		// Token: 0x04000866 RID: 2150
		public static string Path;

		// Token: 0x04000867 RID: 2151
		public static string File;

		// Token: 0x04000868 RID: 2152
		public static string Comp;

		// Token: 0x04000869 RID: 2153
		public static string Client;

		// Token: 0x0400086A RID: 2154
		public static string Target;

		// Token: 0x0400086B RID: 2155
		public static string Other;
	}
}
