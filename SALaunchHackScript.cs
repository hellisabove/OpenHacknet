using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x0200003B RID: 59
	public class SALaunchHackScript : SerializableAction
	{
		// Token: 0x0600014D RID: 333 RVA: 0x00013190 File Offset: 0x00011390
		public override void Trigger(object os_obj)
		{
			if (this.Delay <= 0f)
			{
				Computer computer = Programs.getComputer((OS)os_obj, this.SourceComp);
				Computer computer2 = Programs.getComputer((OS)os_obj, this.TargetComp);
				if (this.RequireLogsOnSource)
				{
					if (computer == null)
					{
						throw new NullReferenceException("Launch Hacker Script Error: Source comp " + this.SourceComp + " does not exist");
					}
					if (computer2 == null)
					{
						throw new NullReferenceException("Launch Hacker Script Error: Target comp " + this.TargetComp + " does not exist");
					}
					Folder folder = computer.files.root.searchForFolder("log");
					bool flag = false;
					for (int i = 0; i < folder.files.Count; i++)
					{
						if (TrackerCompleteSequence.CompShouldStartTrackerFromLogs(os_obj, computer, computer2.ip))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						return;
					}
					if (this.RequireSourceIntact)
					{
						Folder folder2 = computer.files.root.searchForFolder("sys");
						bool flag2 = false;
						for (int i = 0; i < folder2.files.Count; i++)
						{
							if (folder2.files[i].name == "netcfgx.dll" && folder2.files[i].data.Contains("1") && folder2.files[i].data.Contains("0"))
							{
								flag2 = true;
								break;
							}
						}
						if (!flag2)
						{
							return;
						}
					}
				}
				HackerScriptExecuter.runScript(this.Filepath, os_obj, this.SourceComp, this.TargetComp);
			}
			else
			{
				OS os = (OS)os_obj;
				Computer computer3 = Programs.getComputer(os, this.DelayHost);
				if (computer3 == null)
				{
					throw new NullReferenceException("Computer " + computer3 + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer3).AddAction(this, delay);
			}
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000133E8 File Offset: 0x000115E8
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SALaunchHackScript salaunchHackScript = new SALaunchHackScript();
			if (rdr.MoveToAttribute("Filepath"))
			{
				salaunchHackScript.Filepath = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Delay"))
			{
				salaunchHackScript.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				salaunchHackScript.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("SourceComp"))
			{
				salaunchHackScript.SourceComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("TargetComp"))
			{
				salaunchHackScript.TargetComp = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("RequireLogsOnSource"))
			{
				salaunchHackScript.RequireLogsOnSource = (rdr.ReadContentAsString().ToLower() == "true");
			}
			if (rdr.MoveToAttribute("RequireSourceIntact"))
			{
				salaunchHackScript.RequireSourceIntact = (rdr.ReadContentAsString().ToLower() == "true");
			}
			if (string.IsNullOrWhiteSpace(salaunchHackScript.Filepath))
			{
				throw new FormatException("Invalid Filepath");
			}
			return salaunchHackScript;
		}

		// Token: 0x04000142 RID: 322
		public string Filepath;

		// Token: 0x04000143 RID: 323
		public string DelayHost;

		// Token: 0x04000144 RID: 324
		public float Delay;

		// Token: 0x04000145 RID: 325
		public string SourceComp;

		// Token: 0x04000146 RID: 326
		public string TargetComp;

		// Token: 0x04000147 RID: 327
		public bool RequireLogsOnSource;

		// Token: 0x04000148 RID: 328
		public bool RequireSourceIntact;
	}
}
