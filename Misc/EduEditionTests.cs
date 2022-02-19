using System;

namespace Hacknet.Misc
{
	// Token: 0x0200007F RID: 127
	public static class EduEditionTests
	{
		// Token: 0x0600027F RID: 639 RVA: 0x000246D0 File Offset: 0x000228D0
		public static string TestEDUFunctionality(ScreenManager screenMan, out int errorsAdded)
		{
			string text = "";
			int num = 0;
			int num2 = 0;
			bool educationSafeBuild = Settings.EducationSafeBuild;
			text += EduEditionTests.TestEduSafeFileFlags(screenMan, out num);
			num2 += num;
			object obj = text;
			text = string.Concat(new object[]
			{
				obj,
				(num > 0) ? "\r\n" : " ",
				"Complete - ",
				num,
				" errors found"
			});
			errorsAdded = num2;
			Settings.EducationSafeBuild = educationSafeBuild;
			return text;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x0002475C File Offset: 0x0002295C
		public static string TestEduSafeFileFlags(ScreenManager screenMan, out int errorsAdded)
		{
			int num = 0;
			string text = "";
			Settings.EducationSafeBuild = true;
			Computer computer = (Computer)ComputerLoader.loadComputer("Content/Tests/TestComputer.xml", false, false);
			Settings.EducationSafeBuild = false;
			Computer computer2 = (Computer)ComputerLoader.loadComputer("Content/Tests/TestComputer.xml", false, false);
			Folder folder = computer.files.root.searchForFolder("testfolder");
			Folder folder2 = computer2.files.root.searchForFolder("testfolder");
			if (folder.containsFile("eduUnsafeFile.txt") || !folder.containsFile("eduSafeFile.txt") || !folder.containsFile("eduSafeExplicit.txt") || !folder.containsFile("eduSafeOnlyFile.txt"))
			{
				num++;
				text += "\nError in Education File Flags - EDU Safe version has invalid file set";
			}
			if (!folder2.containsFile("eduUnsafeFile.txt") || !folder2.containsFile("eduSafeFile.txt") || !folder2.containsFile("eduSafeExplicit.txt") || folder2.containsFile("eduSafeOnlyFile.txt"))
			{
				num++;
				text += "\nError in Education File Flags - EDU Unsafe version has invalid file set";
			}
			errorsAdded = num;
			return text;
		}
	}
}
