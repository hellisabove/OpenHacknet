using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Hacknet.Daemons.Helpers
{
	// Token: 0x02000014 RID: 20
	public static class AttachmentRenderer
	{
		// Token: 0x060000A5 RID: 165 RVA: 0x0000BB3C File Offset: 0x00009D3C
		public static bool RenderAttachment(string data, object osObj, Vector2 dpos, int startingButtonIndex, SoundEffect buttonSound)
		{
			OS os = (OS)osObj;
			string[] array = data.Split(AttachmentRenderer.spaceDelim, StringSplitOptions.RemoveEmptyEntries);
			bool result;
			if (array.Length < 1)
			{
				result = false;
			}
			else
			{
				if (array[0].Equals("link"))
				{
					Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, string.Concat(new string[]
					{
						LocaleTerms.Loc("LINK"),
						" : ",
						array[1],
						"@",
						array[2]
					}), null);
					Computer computer = Programs.getComputer(os, array[2]);
					if (!os.netMap.visibleNodes.Contains(os.netMap.nodes.IndexOf(computer)))
					{
						AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
					}
					if (Button.doButton(800009 + startingButtonIndex, (int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17, "+", null))
					{
						if (computer == null)
						{
							os.write("ERROR: Linked target not found");
						}
						else
						{
							computer.highlightFlashTime = 1f;
							os.netMap.discoverNode(computer);
							SFX.addCircle(Programs.getComputer(os, array[2]).getScreenSpacePosition(), Color.White, 32f);
							if (buttonSound != null && !Settings.soundDisabled)
							{
								buttonSound.Play();
							}
						}
					}
				}
				else if (array[0].Equals("account"))
				{
					Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, string.Concat(new string[]
					{
						LocaleTerms.Loc("ACCOUNT"),
						" : ",
						array[1],
						" : User=",
						array[3],
						" Pass=",
						array[4]
					}), null);
					AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
					if (Button.doButton(801009 + startingButtonIndex, (int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17, "+", null))
					{
						Computer computer2 = Programs.getComputer(os, array[2]);
						computer2.highlightFlashTime = 1f;
						os.netMap.discoverNode(computer2);
						computer2.highlightFlashTime = 1f;
						SFX.addCircle(computer2.getScreenSpacePosition(), Color.White, 32f);
						for (int i = 0; i < computer2.users.Count; i++)
						{
							UserDetail value = computer2.users[i];
							if (value.name.Equals(array[3]))
							{
								value.known = true;
								computer2.users[i] = value;
								break;
							}
						}
						if (buttonSound != null && !Settings.soundDisabled)
						{
							buttonSound.Play();
						}
					}
				}
				else
				{
					if (!array[0].Equals("note"))
					{
						return false;
					}
					Vector2 labelSize = TextItem.doMeasuredTinyLabel(dpos, LocaleTerms.Loc("NOTE") + " : " + array[1], null);
					string text = array[2];
					text = LocalizedFileLoader.SafeFilterString(text);
					if (!NotesExe.NoteExists(text, os))
					{
						AttachmentRenderer.DrawButtonGlow(dpos, labelSize, os);
					}
					if (Button.doButton(800009 + startingButtonIndex, (int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17, "+", null))
					{
						NotesExe.AddNoteToOS(text, os, false);
					}
				}
				result = true;
			}
			return result;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000BF40 File Offset: 0x0000A140
		private static void DrawButtonGlow(Vector2 dpos, Vector2 labelSize, OS os)
		{
			Rectangle rectangle = new Rectangle((int)(dpos.X + labelSize.X + 5f), (int)dpos.Y, 20, 17);
			float num = Utils.QuadraticOutCurve(1f - os.timer % 1f);
			float num2 = 8.5f;
			Rectangle destinationRectangle = Utils.InsetRectangle(rectangle, (int)(-1f * (num2 * (1f - num))));
			GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Utils.AddativeWhite * num * 0.32f);
			GuiData.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.7f);
		}

		// Token: 0x040000A5 RID: 165
		private static string[] spaceDelim = new string[]
		{
			"#%#"
		};
	}
}
