using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Hacknet
{
	// Token: 0x02000174 RID: 372
	internal class TutorialExe : ExeModule
	{
		// Token: 0x06000951 RID: 2385 RVA: 0x0009AB16 File Offset: 0x00098D16
		public TutorialExe(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
			this.state = 0;
			this.lastCommand = "";
			this.ramCost = 500;
			this.IdentifierName = "Tutorial";
			this.flashTimer = 1f;
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x0009AB58 File Offset: 0x00098D58
		public override void LoadContent()
		{
			base.LoadContent();
			if (TutorialExe.advanced)
			{
				if (TutorialExe.commandSequence == null)
				{
					TutorialExe.commandSequence = new List<string>();
					TutorialExe.commandSequence.Add("connect " + this.os.thisComputer.ip);
					TutorialExe.commandSequence.Add("scan");
					TutorialExe.commandSequence.Add("connect");
					TutorialExe.commandSequence.Add("probe");
					TutorialExe.commandSequence.Add("exe porthack");
					TutorialExe.commandSequence.Add("scan");
					TutorialExe.commandSequence.Add("ls");
					TutorialExe.commandSequence.Add("cd bin");
					TutorialExe.commandSequence.Add("ls");
					TutorialExe.commandSequence.Add("scp sshcrack.exe");
					TutorialExe.commandSequence.Add("cd ..");
					TutorialExe.commandSequence.Add("cd log");
					TutorialExe.commandSequence.Add("rm *");
					TutorialExe.commandSequence.Add("dc");
				}
				if (TutorialExe.feedbackSequence == null)
				{
					TutorialExe.feedbackSequence = new List<string>();
					TutorialExe.feedbackSequence.Add("#As of right now you are#\n#at risk!#\nLearn as quickly as possible. Connect to a computer by typing \"connect [IP]\" - or by clicking on a node on the network map. Connect to your own server on the map now by clicking the green circle.\n \n \nIf at any point you get stuck, a command list can be found by typing \"help\".");
					TutorialExe.feedbackSequence.Add("Good work.\nThe first thing to do on any system is scan it for adjacent nodes. This will reveal more computers on your map that you can use. Scan this computer now by typing \n#scan#\n and run it by pressing enter.");
					TutorialExe.feedbackSequence.Add("It's time for you to connect to an outside computer. Be aware that attempting to compromise the security of another's computer is Illegal under the U.S.C. Act 1030-18. Proceed at your own risk by using the command \n#connect [TARGET IP]#\n or by clicking a blue node on the network map.");
					TutorialExe.feedbackSequence.Add("A computer's security system and open ports can be analysed using the \n#probe#\n (nmap) command. analyse the computer you are currently connected to.");
					TutorialExe.feedbackSequence.Add("Here you can see the active ports, active security, and the number of open ports required to sucsesfully crack this machine using PortHack.\nThis Machine has no active security and requires no open ports to crack. If you are prepared to, it is possible to crack this comput er using the program \n#PortHack#\n. To run a program you own on the machine you are connected to, use the command \n#exe [PROGRAM NAME]#\n. A program name will never require \".exe on the end\"");
					TutorialExe.feedbackSequence.Add("Congratulations. You have taken control of an external system and are now it's administrator. You can do whatever you like with it, however you should start by using \n#scan#\n to locate additional computers");
					TutorialExe.feedbackSequence.Add("Next, you should investigate the filesystem. Use the \n#ls#\n command to view the files and folders in your current directory.");
					TutorialExe.feedbackSequence.Add("Navigate to the \n#bin#\n (Binaries) folder to search for useful executables usind the command \n#cd [FOLDER NAME]#\n");
					TutorialExe.feedbackSequence.Add("List the files in this folder using \n#ls#\n");
					TutorialExe.feedbackSequence.Add("The file \"\n#SSHcrack.exe#\n\" is a program used to open a locked port running SSH on an external server. If you had it on your local computer, you could run it by using \"exe SSHCrack [SSH PORT NUMBER]\" - but you need it first. Download files from an external machine by using \n#scp [FILE NAME]#\n\n NOTE: Downloading a file you do not own is highly illegal and punishable by prison sentence.");
					TutorialExe.feedbackSequence.Add("That's sure to come in handy.\n Now to clear your tracks before you leave. Move up a folder in the directory using \n#cd ..#\n");
					TutorialExe.feedbackSequence.Add("Move to the log folder.\n#cd [FOLDER NAME]#\n");
					TutorialExe.feedbackSequence.Add("You can delete a file using the command \n#rm [FILENAME]#\n , however you can delete ALL files in the current directory with the command \n#rm *#\n");
					TutorialExe.feedbackSequence.Add("Excellent work.\n\nTo Disconnect from a computer, use the \n#dc#\n command");
					TutorialExe.feedbackSequence.Add("Congratulations, You have completed the guided section of this tutorial.\n\nTo finish it, you must locate the Process ID of this tutorial program and kill it. the \n#help#\n command will give you a complete command list at any time. Use \n#exe Tutorial#\n to run this program again.");
				}
			}
			else
			{
				if (TutorialExe.commandSequence == null)
				{
					TutorialExe.commandSequence = new List<string>();
					TutorialExe.commandSequence.Add("connect " + this.os.thisComputer.ip);
					TutorialExe.commandSequence.Add("scan");
					TutorialExe.commandSequence.Add("connect");
					TutorialExe.commandSequence.Add("probe");
					TutorialExe.commandSequence.Add("exe porthack");
					TutorialExe.commandSequence.Add("scan");
					TutorialExe.commandSequence.Add("ls");
					TutorialExe.commandSequence.Add("cd log");
					TutorialExe.commandSequence.Add("rm *");
					TutorialExe.commandSequence.Add("dc");
				}
				if (TutorialExe.feedbackSequence == null)
				{
					TutorialExe.feedbackSequence = new List<string>();
					TutorialExe.feedbackSequence.Add("#As of right now you are#\n#at risk!#\nLearn as quickly as possible. Connect to a computer by typing \"connect [IP]\" - or by clicking on a node on the network map. Connect to your own server on the map now by clicking the green circle.\n \n \nIf at any point you get stuck, a command list can be found by typing \"help\".");
					TutorialExe.feedbackSequence.Add("Good work.\nThe first thing to do on any system is scan it for adjacent nodes. This will reveal more computers on your map that you can use. Scan this computer now by typing \n#scan#\n and run it by pressing enter.");
					TutorialExe.feedbackSequence.Add("It's time for you to connect to an outside computer. Be aware that attempting to compromise the security of another's computer is Illegal under the U.S.C. Act 1030-18. Proceed at your own risk by using the command \n#connect [TARGET IP]#\n or by clicking a blue node on the network map.");
					TutorialExe.feedbackSequence.Add("A computer's security system and open ports can be analysed using the \n#probe#\n command. Analyse the computer you are currently connected to.");
					TutorialExe.feedbackSequence.Add("Here you can see the active ports, active security, and the number of open ports required to successfully crack this machine using PortHack.\nThis Machine has no active security and requires no open ports to crack. If you are prepared to, it is possible to crack this computer using the program \n#PortHack#\n. To run a program you own on the machine you are connected to, use the command \n#exe \"PROGRAM NAME\"#\n. A program name will never require \".exe on the end\"");
					TutorialExe.feedbackSequence.Add("Congratulations. You have taken control of an external system and are now it's \nadministrator. You can do whatever you like with it, however you should start by using \n#scan#\n to locate additional computers");
					TutorialExe.feedbackSequence.Add("Next, you should investigate the filesystem. Use the \n#ls#\n command to view the files and folders in your current directory.\nFeel free to play around and investigate anything you like. This is your node now.");
					TutorialExe.feedbackSequence.Add("Move to the log folder when you are ready.\n#cd [FOLDER NAME]#\nThe Up and Down arrow keys let you quickly access previous commands, and the tab key will autocomplete file and program names in the terminal.");
					TutorialExe.feedbackSequence.Add("You can delete a file using the command \n#rm [FILENAME]#\n , however you can delete ALL files in the current directory with the command \n#rm *#\n");
					TutorialExe.feedbackSequence.Add("Excellent work.\n\nTo Disconnect from a computer, use the \n#dc#\n command");
					TutorialExe.feedbackSequence.Add("Congratulations, You have completed the guided section of this tutorial.\n\nTo finish it, you must locate the Process ID of this tutorial program and kill it. the \n#help#\n command will give you a complete command list at any time. Use \n#exe Tutorial#\n to run this program again.\nOnce you complete this, check your email with the icon in the top right...");
				}
			}
			this.state = 0;
			this.getRenderText();
			this.printCurrentCommandToTerminal();
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x0009AF5C File Offset: 0x0009915C
		public override void Update(float t)
		{
			base.Update(t);
			string lastRunCommand = this.os.terminal.getLastRunCommand();
			if (!this.lastCommand.Equals(lastRunCommand))
			{
				this.lastCommand = lastRunCommand;
				this.parseCommand();
			}
			this.flashTimer -= t;
			this.flashTimer = Math.Max(this.flashTimer, 0f);
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x0009AFC8 File Offset: 0x000991C8
		public void parseCommand()
		{
			if (this.state < TutorialExe.commandSequence.Count)
			{
				if (this.lastCommand.ToLower().StartsWith(TutorialExe.commandSequence[this.state]))
				{
					this.state++;
					this.getRenderText();
					this.printCurrentCommandToTerminal();
					this.flashTimer = 1f;
				}
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x0009B044 File Offset: 0x00099244
		public void printCurrentCommandToTerminal()
		{
			this.os.terminal.writeLine("--------------------------------------------------");
			this.os.terminal.writeLine(" ");
			for (int i = 0; i < this.renderText.Length; i++)
			{
				this.os.terminal.writeLine(this.renderText[i]);
			}
			this.os.terminal.writeLine(" ");
			this.os.terminal.writeLine("--------------------------------------------------");
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x0009B0DC File Offset: 0x000992DC
		public void getRenderText()
		{
			char[] separator = new char[]
			{
				'\n'
			};
			this.renderText = DisplayModule.cleanSplitForWidth(TutorialExe.feedbackSequence[this.state], 200).Split(separator);
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x0009B120 File Offset: 0x00099320
		public override void Killed()
		{
			base.Killed();
			if (!this.os.multiplayer && this.os.initShowsTutorial)
			{
				this.os.currentMission.sendEmail(this.os);
				this.os.initShowsTutorial = false;
			}
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x0009B180 File Offset: 0x00099380
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			Rectangle bounds = this.bounds;
			bounds.X++;
			bounds.Y++;
			bounds.Width -= 2;
			bounds.Height -= 2;
			bounds.Height -= Module.PANEL_HEIGHT;
			bounds.Y += Module.PANEL_HEIGHT;
			PatternDrawer.draw(bounds, 1f, this.os.highlightColor * 0.2f, this.os.highlightColor, this.spriteBatch);
			bounds.X += 2;
			bounds.Y += 2;
			bounds.Width -= 4;
			bounds.Height -= 4;
			this.spriteBatch.Draw(Utils.white, bounds, this.os.darkBackgroundColor * 0.99f);
			this.spriteBatch.Draw(Utils.white, bounds, this.os.highlightColor * this.flashTimer);
			this.drawTarget("");
			this.spriteBatch.DrawString(GuiData.font, "Tutorial", new Vector2((float)(this.bounds.X + 3), (float)(this.bounds.Y + 22)), this.os.subtleTextColor);
			float num = 12f;
			Vector2 position = new Vector2((float)(this.bounds.X + 5), (float)(this.bounds.Y + 57));
			for (int i = 0; i < this.renderText.Length; i++)
			{
				string text = this.renderText[i];
				if (text.Length > 0)
				{
					bool flag = false;
					if (text[0] == '#')
					{
						text = text.Substring(1, text.Length - 2);
						flag = true;
					}
					this.spriteBatch.DrawString(GuiData.tinyfont, text, position, flag ? this.os.highlightColor : Color.White);
					position.Y += num;
				}
			}
			position.Y += num;
		}

		// Token: 0x04000AEB RID: 2795
		public static bool advanced = false;

		// Token: 0x04000AEC RID: 2796
		private static List<string> commandSequence;

		// Token: 0x04000AED RID: 2797
		private static List<string> feedbackSequence;

		// Token: 0x04000AEE RID: 2798
		private int state;

		// Token: 0x04000AEF RID: 2799
		private string lastCommand;

		// Token: 0x04000AF0 RID: 2800
		private string[] renderText;

		// Token: 0x04000AF1 RID: 2801
		private float flashTimer;
	}
}
