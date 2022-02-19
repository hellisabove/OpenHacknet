using System;

namespace Hacknet
{
	// Token: 0x02000199 RID: 409
	internal static class ShellOverloaderExe
	{
		// Token: 0x06000A4A RID: 2634 RVA: 0x000A429C File Offset: 0x000A249C
		public static void RunShellOverloaderExe(string[] args, object osObj, Computer target)
		{
			OS os = (OS)osObj;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (args.Length > 1)
			{
				if (args[1].ToLower() == "-c")
				{
					flag = false;
					flag2 = false;
					flag3 = true;
				}
				else if (args[1].ToLower() == "-o")
				{
					flag = false;
					flag2 = true;
				}
				else if (args[1].ToLower() == "-e")
				{
					flag = true;
					flag2 = false;
					flag3 = false;
				}
			}
			if (!flag2 && !flag && !flag3)
			{
				os.write("--------------------------------------");
				os.write("ConShell " + LocaleTerms.Loc("ERROR: Not enough arguments!"));
				os.write(LocaleTerms.Loc("Usage:") + " ConShell [-" + LocaleTerms.Loc("option") + "]");
				os.write(string.Concat(new string[]
				{
					LocaleTerms.Loc("Valid Options:"),
					" [-e (",
					LocaleTerms.Loc("Exit"),
					")] [-o (",
					LocaleTerms.Loc("Overload"),
					")] [-c (",
					LocaleTerms.Loc("Cancel Overload"),
					")]"
				}));
				os.write("--------------------------------------");
			}
			else if (os.exes.Count <= 0)
			{
				os.write("--------------------------------------");
				os.write("ConShell " + LocaleTerms.Loc("ERROR: No active shells"));
				os.write("--------------------------------------");
			}
			else
			{
				for (int i = 0; i < os.exes.Count; i++)
				{
					ShellExe shellExe = os.exes[i] as ShellExe;
					if (shellExe != null)
					{
						if (flag)
						{
							shellExe.Completed();
						}
						else if (flag3)
						{
							shellExe.cancelTarget();
						}
						else if (flag2)
						{
							shellExe.StartOverload();
						}
					}
				}
			}
		}
	}
}
