using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Hacknet.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200016F RID: 367
	internal class Terminal : CoreModule
	{
		// Token: 0x06000926 RID: 2342 RVA: 0x00097238 File Offset: 0x00095438
		public Terminal(Rectangle location, OS operatingSystem) : base(location, operatingSystem)
		{
		}

		// Token: 0x06000927 RID: 2343 RVA: 0x000972B4 File Offset: 0x000954B4
		public override void LoadContent()
		{
			this.history = new List<string>(512);
			this.runCommands = new List<string>(512);
			this.commandHistoryOffset = 0;
			this.currentLine = "";
			this.lastRunCommand = "";
			this.prompt = "> ";
		}

		// Token: 0x06000928 RID: 2344 RVA: 0x0009730A File Offset: 0x0009550A
		public override void Update(float t)
		{
		}

		// Token: 0x06000929 RID: 2345 RVA: 0x00097310 File Offset: 0x00095510
		public override void Draw(float t)
		{
			base.Draw(t);
			float tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
			this.spriteBatch.Draw(Utils.white, this.bounds, this.os.displayModuleExtraLayerBackingColor);
			int num = (int)((float)(this.bounds.Height - 12) / (tinyFontCharHeight + 1f));
			num -= 3;
			num = Math.Min(num, this.history.Count);
			Vector2 input = new Vector2((float)(this.bounds.X + 4), (float)(this.bounds.Y + this.bounds.Height) - tinyFontCharHeight * 5f);
			if (num > 0)
			{
				for (int i = this.history.Count; i > this.history.Count - num; i--)
				{
					try
					{
						this.spriteBatch.DrawString(GuiData.tinyfont, this.history[i - 1], Utils.ClipVec2ForTextRendering(input), this.os.terminalTextColor);
						input.Y -= tinyFontCharHeight + 1f;
					}
					catch (Exception)
					{
					}
				}
			}
			this.doGui();
		}

		// Token: 0x0600092A RID: 2346 RVA: 0x00097458 File Offset: 0x00095658
		public void executeLine()
		{
			string text = this.currentLine;
			if (TextBox.MaskingText)
			{
				text = "";
				for (int i = 0; i < this.currentLine.Length; i++)
				{
					text += "*";
				}
			}
			this.history.Add(this.prompt + text);
			this.lastRunCommand = this.currentLine;
			this.runCommands.Add(this.currentLine);
			if (!this.preventingExecution)
			{
				this.commandHistoryOffset = 0;
				this.os.execute(this.currentLine);
				if (this.currentLine.Length > 0)
				{
					StatsManager.IncrementStat("commands_run", 1);
				}
			}
			this.currentLine = "";
			TextBox.cursorPosition = 0;
			TextBox.textDrawOffsetPosition = 0;
			this.executionPreventionIsInteruptable = false;
		}

		// Token: 0x0600092B RID: 2347 RVA: 0x00097548 File Offset: 0x00095748
		public string GetRecentTerminalHistoryString()
		{
			string text = "";
			int num = this.history.Count - 1;
			while (num > this.history.Count - 30 && this.history.Count > num)
			{
				text = text + this.history[num] + "\r\n";
				num--;
			}
			return text;
		}

		// Token: 0x0600092C RID: 2348 RVA: 0x000975B8 File Offset: 0x000957B8
		public List<string> GetRecentTerminalHistoryList()
		{
			List<string> list = new List<string>();
			int num = 0;
			while (num < 30 && num < this.history.Count)
			{
				list.Add(this.history[num]);
				num++;
			}
			return list;
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x0009760C File Offset: 0x0009580C
		public void NonThreadedInstantExecuteLine()
		{
			string text = this.currentLine;
			if (TextBox.MaskingText)
			{
				text = "";
				for (int i = 0; i < this.currentLine.Length; i++)
				{
					text += "*";
				}
			}
			this.history.Add(this.prompt + text);
			this.lastRunCommand = this.currentLine;
			this.runCommands.Add(this.currentLine);
			if (!this.preventingExecution)
			{
				this.commandHistoryOffset = 0;
				ProgramRunner.ExecuteProgram(this.os, this.currentLine.Split(new char[]
				{
					' '
				}));
			}
			this.currentLine = "";
			TextBox.cursorPosition = 0;
			TextBox.textDrawOffsetPosition = 0;
			this.executionPreventionIsInteruptable = false;
		}

		// Token: 0x0600092E RID: 2350 RVA: 0x000976E8 File Offset: 0x000958E8
		public void doGui()
		{
			SpriteFont tinyfont = GuiData.tinyfont;
			float tinyFontCharHeight = GuiData.ActiveFontConfig.tinyFontCharHeight;
			int num = -4;
			int num2 = (int)((float)(this.bounds.Y + this.bounds.Height - 16) - tinyFontCharHeight - (float)num);
			int i = (int)tinyfont.MeasureString(this.prompt).X;
			if (this.bounds.Width > 0)
			{
				while (i >= (int)((double)this.bounds.Width * 0.7))
				{
					this.prompt = this.prompt.Substring(1);
					i = (int)tinyfont.MeasureString(this.prompt).X;
				}
			}
			this.spriteBatch.DrawString(tinyfont, this.prompt, new Vector2((float)(this.bounds.X + 3), (float)num2), this.currentTextColor);
			if (LocaleActivator.ActiveLocaleIsCJK())
			{
				num -= 4;
			}
			num2 += num;
			if (this.os.inputEnabled)
			{
				if (!this.inputLocked)
				{
					TextBox.LINE_HEIGHT = (int)(tinyFontCharHeight + 15f);
					this.currentLine = TextBox.doTerminalTextField(7001, this.bounds.X + 3 + (int)Terminal.PROMPT_OFFSET + (int)tinyfont.MeasureString(this.prompt).X, num2, this.bounds.Width - i - 4, this.bounds.Height, 1, this.currentLine, tinyfont);
					if (TextBox.BoxWasActivated)
					{
						this.executeLine();
					}
					if (TextBox.UpWasPresed)
					{
						if (this.runCommands.Count > 0)
						{
							this.commandHistoryOffset++;
							if (this.commandHistoryOffset > this.runCommands.Count)
							{
								this.commandHistoryOffset = this.runCommands.Count;
							}
							this.currentLine = this.runCommands[this.runCommands.Count - this.commandHistoryOffset];
							TextBox.cursorPosition = this.currentLine.Length;
						}
					}
					if (TextBox.DownWasPresed)
					{
						if (this.commandHistoryOffset > 0)
						{
							this.commandHistoryOffset--;
							if (this.commandHistoryOffset < 0)
							{
								this.commandHistoryOffset = 0;
							}
							if (this.commandHistoryOffset <= 0)
							{
								this.currentLine = "";
							}
							else
							{
								this.currentLine = this.runCommands[this.runCommands.Count - this.commandHistoryOffset];
							}
							TextBox.cursorPosition = this.currentLine.Length;
						}
					}
					if (TextBox.TabWasPresed)
					{
						if (this.usingTabExecution)
						{
							this.executeLine();
						}
						else
						{
							this.doTabComplete();
						}
					}
				}
			}
		}

		// Token: 0x0600092F RID: 2351 RVA: 0x00097A04 File Offset: 0x00095C04
		public void doTabComplete()
		{
			List<string> list = new List<string>();
			if (this.currentLine.Length != 0)
			{
				int num = this.currentLine.IndexOf(' ');
				if (num >= 1)
				{
					string text = this.currentLine.Substring(num + 1);
					string text2 = this.currentLine.Substring(0, num);
					if (text2.Equals("upload") || text2.Equals("up"))
					{
						int num2 = text.LastIndexOf('/');
						if (num2 < 0)
						{
							num2 = 0;
						}
						string text3 = text.Substring(0, num2) + "/";
						if (text3.StartsWith("/"))
						{
							text3 = text3.Substring(1);
						}
						string text4 = text.Substring(num2 + ((num2 == 0) ? 0 : 1));
						Folder folder = Programs.getFolderAtPathAsFarAsPossible(text, this.os, this.os.thisComputer.files.root);
						bool flag = false;
						if (folder == this.os.thisComputer.files.root && text3.Length > 1)
						{
							flag = true;
						}
						if (folder == null)
						{
							folder = this.os.thisComputer.files.root;
						}
						if (!flag)
						{
							for (int i = 0; i < folder.folders.Count; i++)
							{
								if (folder.folders[i].name.ToLower().StartsWith(text4.ToLower(), StringComparison.InvariantCultureIgnoreCase))
								{
									list.Add(string.Concat(new string[]
									{
										text2,
										" ",
										text3,
										folder.folders[i].name,
										"/"
									}));
								}
							}
							for (int i = 0; i < folder.files.Count; i++)
							{
								if (folder.files[i].name.ToLower().StartsWith(text4.ToLower()))
								{
									list.Add(text2 + " " + text3 + folder.files[i].name);
								}
							}
						}
					}
					else
					{
						if (text == null || ((text.Equals("") || text.Length < 1) && !text2.Equals("exe")))
						{
							return;
						}
						Folder folder = Programs.getCurrentFolder(this.os);
						for (int i = 0; i < folder.folders.Count; i++)
						{
							if (folder.folders[i].name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
							{
								list.Add(text2 + " " + folder.folders[i].name + "/");
							}
						}
						for (int i = 0; i < folder.files.Count; i++)
						{
							if (folder.files[i].name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
							{
								list.Add(text2 + " " + folder.files[i].name);
							}
						}
						if (list.Count == 0)
						{
							for (int i = 0; i < folder.files.Count; i++)
							{
								if (folder.files[i].name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
								{
									list.Add(text2 + " " + folder.files[i].name);
								}
							}
						}
					}
				}
				else
				{
					List<string> list2 = new List<string>();
					list2.AddRange(ProgramList.programs);
					list2.AddRange(ProgramList.getExeList(this.os));
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i].ToLower().StartsWith(this.currentLine.ToLower()))
						{
							list.Add(list2[i]);
						}
					}
				}
				if (list.Count == 1)
				{
					this.currentLine = list[0];
					TextBox.moveCursorToEnd(this.currentLine);
				}
				else if (list.Count > 1)
				{
					this.os.write(this.prompt + this.currentLine);
					string text5 = list[0];
					for (int i = 0; i < list.Count; i++)
					{
						this.os.write(list[i]);
						for (int j = 0; j < text5.Length; j++)
						{
							if (list[i].Length <= j || string.Concat(text5[j]).ToLowerInvariant()[0] != string.Concat(list[i][j]).ToLowerInvariant()[0])
							{
								text5 = text5.Substring(0, j);
								break;
							}
						}
						this.currentLine = text5;
						TextBox.moveCursorToEnd(this.currentLine);
					}
				}
			}
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x00098020 File Offset: 0x00096220
		public void writeLine(string text)
		{
			text = Utils.SuperSmartTwimForWidth(text, this.bounds.Width - 6, GuiData.tinyfont);
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			for (int i = 0; i < array.Length; i++)
			{
				this.history.Add(array[i]);
			}
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00098080 File Offset: 0x00096280
		public void write(string text)
		{
			if (this.history.Count <= 0 || GuiData.tinyfont.MeasureString(this.history[this.history.Count - 1] + text).X > (float)(this.bounds.Width - 6))
			{
				this.writeLine(text);
			}
			else
			{
				List<string> list;
				int index;
				(list = this.history)[index = this.history.Count - 1] = list[index] + text;
			}
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x0009811C File Offset: 0x0009631C
		public void clearCurrentLine()
		{
			this.currentLine = "";
			TextBox.cursorPosition = 0;
			TextBox.textDrawOffsetPosition = 0;
		}

		// Token: 0x06000933 RID: 2355 RVA: 0x00098136 File Offset: 0x00096336
		public void reset()
		{
			this.history.Clear();
			this.clearCurrentLine();
		}

		// Token: 0x06000934 RID: 2356 RVA: 0x0009814C File Offset: 0x0009634C
		public int commandsRun()
		{
			return this.runCommands.Count;
		}

		// Token: 0x06000935 RID: 2357 RVA: 0x0009816C File Offset: 0x0009636C
		public string getLastRunCommand()
		{
			return this.lastRunCommand;
		}

		// Token: 0x04000AB4 RID: 2740
		public static float PROMPT_OFFSET = 0f;

		// Token: 0x04000AB5 RID: 2741
		private List<string> history;

		// Token: 0x04000AB6 RID: 2742
		private List<string> runCommands = new List<string>();

		// Token: 0x04000AB7 RID: 2743
		private int commandHistoryOffset;

		// Token: 0x04000AB8 RID: 2744
		public string currentLine;

		// Token: 0x04000AB9 RID: 2745
		public string lastRunCommand;

		// Token: 0x04000ABA RID: 2746
		public string prompt;

		// Token: 0x04000ABB RID: 2747
		public bool usingTabExecution = false;

		// Token: 0x04000ABC RID: 2748
		public bool preventingExecution = false;

		// Token: 0x04000ABD RID: 2749
		public bool executionPreventionIsInteruptable = false;

		// Token: 0x04000ABE RID: 2750
		private Color outlineColor = new Color(68, 68, 68);

		// Token: 0x04000ABF RID: 2751
		private Color backColor = new Color(8, 8, 8);

		// Token: 0x04000AC0 RID: 2752
		private Color historyTextColor = new Color(220, 220, 220);

		// Token: 0x04000AC1 RID: 2753
		private Color currentTextColor = Color.White;
	}
}
