using System;

namespace Hacknet.Effects
{
	// Token: 0x0200005A RID: 90
	public static class TextWriterTimed
	{
		// Token: 0x060001B2 RID: 434 RVA: 0x00017830 File Offset: 0x00015A30
		public static int WriteTextToTerminal(string wholeText, object osObj, float timePerChar, float normalLettersDelayForNewline, float normalLetterDelayForWildcard, float elapsedTimeSoFar, int charsRenderedSoFar)
		{
			char c = '%';
			OS os = (OS)osObj;
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < wholeText.Length; i++)
			{
				num2 += ((wholeText[i] == '\n') ? (normalLettersDelayForNewline * timePerChar) : ((wholeText[i] == c) ? (normalLetterDelayForWildcard * timePerChar) : timePerChar));
				if (num2 >= elapsedTimeSoFar)
				{
					break;
				}
				num++;
			}
			int result;
			if (charsRenderedSoFar > num)
			{
				result = num;
			}
			else
			{
				for (int i = charsRenderedSoFar; i < num; i++)
				{
					if (wholeText[i] != c)
					{
						if (wholeText[i] == '\n')
						{
							os.write(" ");
						}
						else
						{
							os.writeSingle(string.Concat(wholeText[i]));
						}
					}
				}
				result = num;
			}
			return result;
		}
	}
}
