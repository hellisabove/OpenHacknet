using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000167 RID: 359
	public class Firewall
	{
		// Token: 0x06000904 RID: 2308 RVA: 0x000959FF File Offset: 0x00093BFF
		public Firewall()
		{
			this.generateRandomSolution();
		}

		// Token: 0x06000905 RID: 2309 RVA: 0x00095A38 File Offset: 0x00093C38
		public Firewall(int complexity)
		{
			this.complexity = complexity;
			this.solutionLength = 6 + complexity;
			this.generateRandomSolution();
		}

		// Token: 0x06000906 RID: 2310 RVA: 0x00095A8C File Offset: 0x00093C8C
		public Firewall(int complexity, string solution)
		{
			this.complexity = complexity;
			this.solution = solution;
			this.solutionLength = solution.Length;
		}

		// Token: 0x06000907 RID: 2311 RVA: 0x00095AE4 File Offset: 0x00093CE4
		public Firewall(int complexity, string solution, float additionalTime)
		{
			this.complexity = complexity;
			this.solution = solution;
			this.additionalDelay = additionalTime;
			this.solutionLength = solution.Length;
		}

		// Token: 0x06000908 RID: 2312 RVA: 0x00095B44 File Offset: 0x00093D44
		private void generateRandomSolution()
		{
			StringBuilder stringBuilder = new StringBuilder(this.solutionLength);
			for (int i = 0; i < this.solutionLength; i++)
			{
				stringBuilder.Append(Utils.getRandomChar());
			}
			this.solution = stringBuilder.ToString().ToUpperInvariant();
		}

		// Token: 0x06000909 RID: 2313 RVA: 0x00095B94 File Offset: 0x00093D94
		public static Firewall load(XmlReader reader)
		{
			while (reader.Name != "firewall")
			{
				reader.Read();
			}
			int num = 0;
			string text = null;
			float additionalTime = 0f;
			if (reader.MoveToAttribute("complexity"))
			{
				num = reader.ReadContentAsInt();
			}
			if (reader.MoveToAttribute("solution"))
			{
				text = reader.ReadContentAsString();
			}
			if (reader.MoveToAttribute("additionalDelay"))
			{
				additionalTime = reader.ReadContentAsFloat();
			}
			return new Firewall(num, text, additionalTime);
		}

		// Token: 0x0600090A RID: 2314 RVA: 0x00095C2C File Offset: 0x00093E2C
		public string getSaveString()
		{
			return string.Concat(new object[]
			{
				"<firewall complexity=\"",
				this.complexity,
				"\" solution=\"",
				this.solution,
				"\" additionalDelay=\"",
				this.additionalDelay.ToString(CultureInfo.InvariantCulture),
				"\" />"
			});
		}

		// Token: 0x0600090B RID: 2315 RVA: 0x00095C95 File Offset: 0x00093E95
		public void resetSolutionProgress()
		{
			this.analysisPasses = 0;
		}

		// Token: 0x0600090C RID: 2316 RVA: 0x00095CA0 File Offset: 0x00093EA0
		public bool attemptSolve(string attempt, object os)
		{
			if (attempt.Length != this.solution.Length)
			{
				string str = (attempt.Length < this.solution.Length) ? LocaleTerms.Loc("Too few characters") : LocaleTerms.Loc("Too many characters");
				((OS)os).write(LocaleTerms.Loc("Solution Incorrect Length") + " - " + str);
			}
			else if (attempt.ToLower().Equals(this.solution.ToLower()))
			{
				this.solved = true;
				return true;
			}
			return false;
		}

		// Token: 0x0600090D RID: 2317 RVA: 0x00095D48 File Offset: 0x00093F48
		public void writeAnalyzePass(object os_object, object target_object)
		{
			Computer computer = (Computer)target_object;
			OS os = (OS)os_object;
			if (computer.firewallAnalysisInProgress)
			{
				os.write("-" + LocaleTerms.Loc("Analysis already in Progress") + "-");
			}
			else
			{
				os.delayer.PostAnimation(this.generateOutputPass(this.analysisPasses, os, computer));
				this.analysisPasses++;
			}
		}

		// Token: 0x0600090E RID: 2318 RVA: 0x00095FD0 File Offset: 0x000941D0
		private IEnumerator<ActionDelayer.Condition> generateOutputPass(int pass, OS os, Computer target)
		{
			target.firewallAnalysisInProgress = true;
			os.write(string.Format(LocaleTerms.Loc("Firewall Analysis Pass {0}"), this.analysisPasses) + "\n");
			yield return ActionDelayer.Wait(0.03);
			os.write("--------------------");
			yield return ActionDelayer.Wait(0.03);
			string preceedeString = "     ";
			double secondsDelayPerLine = 0.08 + 0.06 * (double)pass + (double)this.additionalDelay;
			for (int i = 0; i < this.solutionLength; i++)
			{
				os.write(preceedeString + this.generateOutputLine(i));
				yield return ActionDelayer.Wait(secondsDelayPerLine);
			}
			os.write("--------------------\n");
			target.firewallAnalysisInProgress = false;
			yield break;
		}

		// Token: 0x0600090F RID: 2319 RVA: 0x00096008 File Offset: 0x00094208
		private string generateOutputLine(int location)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 20; i++)
			{
				stringBuilder.Append("0");
			}
			int num = 20 - 3 * this.analysisPasses;
			for (int i = 0; i < num; i++)
			{
				stringBuilder[i] = string.Concat(Utils.getRandomChar()).ToLower()[0];
			}
			int index = Utils.random.Next(stringBuilder.Length);
			if (location < this.solution.Length)
			{
				stringBuilder[index] = this.solution[location];
			}
			for (int i = 0; i < stringBuilder.Length; i += 2)
			{
				stringBuilder.Insert(i, " ");
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000910 RID: 2320 RVA: 0x000960F0 File Offset: 0x000942F0
		public override bool Equals(object obj)
		{
			Firewall firewall = obj as Firewall;
			return firewall != null && (firewall.additionalDelay == this.additionalDelay && firewall.complexity == this.complexity) && firewall.solution == this.solution;
		}

		// Token: 0x06000911 RID: 2321 RVA: 0x00096148 File Offset: 0x00094348
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000912 RID: 2322 RVA: 0x00096160 File Offset: 0x00094360
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Firewall: solution\"",
				this.solution,
				"\" - time:",
				this.additionalDelay,
				" - complexity:",
				this.complexity
			});
		}

		// Token: 0x04000A5F RID: 2655
		private const int MIN_SOLUTION_LENGTH = 6;

		// Token: 0x04000A60 RID: 2656
		private const int OUTPUT_LINE_WIDTH = 20;

		// Token: 0x04000A61 RID: 2657
		private const int CHARS_SOLVED_PER_PASS = 3;

		// Token: 0x04000A62 RID: 2658
		private const string SOLVED_CHAR = "0";

		// Token: 0x04000A63 RID: 2659
		private int solutionLength = 6;

		// Token: 0x04000A64 RID: 2660
		private string solution;

		// Token: 0x04000A65 RID: 2661
		public bool solved = false;

		// Token: 0x04000A66 RID: 2662
		private int complexity = 1;

		// Token: 0x04000A67 RID: 2663
		private int analysisPasses = 0;

		// Token: 0x04000A68 RID: 2664
		private float additionalDelay = 0f;
	}
}
