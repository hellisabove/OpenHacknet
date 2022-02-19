using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Daemons.Helpers
{
	// Token: 0x02000015 RID: 21
	public class IRCSystem
	{
		// Token: 0x060000A8 RID: 168 RVA: 0x0000C018 File Offset: 0x0000A218
		public IRCSystem(Folder storageFolder)
		{
			this.StorageFolder = storageFolder;
			this.ActiveLogFile = this.StorageFolder.searchForFile("active.log");
			if (this.ActiveLogFile == null)
			{
				this.ActiveLogFile = new FileEntry("#", "active.log");
				this.StorageFolder.files.Add(this.ActiveLogFile);
			}
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x0000C0A8 File Offset: 0x0000A2A8
		private List<IRCSystem.IRCLogEntry> GetLogsFromFile()
		{
			string text = this.ActiveLogFile.data.Substring(1);
			string[] array = text.Split(IRCSystem.EntryLineDelimiter, StringSplitOptions.RemoveEmptyEntries);
			List<IRCSystem.IRCLogEntry> list = new List<IRCSystem.IRCLogEntry>();
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(IRCSystem.IRCLogEntry.DeserializeSafe(array[i]));
			}
			return list;
		}

		// Token: 0x060000AA RID: 170 RVA: 0x0000C108 File Offset: 0x0000A308
		public void AddLog(string author, string message, double timestampSecondsOffset)
		{
			DateTime dateTime = DateTime.Now - TimeSpan.FromSeconds(timestampSecondsOffset);
			string timestamp = dateTime.Hour.ToString("00") + ":" + dateTime.Minute.ToString("00");
			this.AddLog(author, message, timestamp);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000C164 File Offset: 0x0000A364
		public void AddLog(string author, string message, string timestamp = null)
		{
			if (timestamp == null)
			{
				DateTime now = DateTime.Now;
				timestamp = now.Hour.ToString("00") + ":" + now.Minute.ToString("00");
			}
			string str = new IRCSystem.IRCLogEntry
			{
				Author = author,
				Message = message,
				Timestamp = timestamp
			}.Serialize();
			if (this.ActiveLogFile.data.Length > 1)
			{
				str = "\n#" + str;
			}
			FileEntry activeLogFile = this.ActiveLogFile;
			activeLogFile.data += str;
			this.messagesAddedSinceLastView++;
			if (this.LogAdded != null)
			{
				this.LogAdded(author, message);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000C251 File Offset: 0x0000A451
		public void LeftView()
		{
			this.messagesAddedSinceLastView = 0;
			this.isCurrentlyBeingViewed = false;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000C264 File Offset: 0x0000A464
		public void Draw(Rectangle dest, SpriteBatch sb, bool CanWrite, string WriteUsername, Dictionary<string, Color> HighlightKeywords)
		{
			this.isCurrentlyBeingViewed = true;
			if (!CanWrite)
			{
				this.DrawLog(dest, sb, HighlightKeywords);
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000C28C File Offset: 0x0000A48C
		private void DrawLog(Rectangle dest, SpriteBatch sb, Dictionary<string, Color> HighlightKeywords)
		{
			List<IRCSystem.IRCLogEntry> logsFromFile = this.GetLogsFromFile();
			int num = (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 4f);
			int num2 = 4;
			int num3 = (int)((float)dest.Height / (float)num);
			int num4 = num3;
			int num5 = logsFromFile.Count - 1;
			int num6 = 0;
			int y = dest.Y;
			this.DrawnButtonIndex = 1892;
			while (num4 > 0 && num5 >= 0 && dest.Height - num6 > num)
			{
				bool needsNewMessagesLineDraw = this.messagesAddedSinceLastView > 0 && this.messagesAddedSinceLastView < logsFromFile.Count && logsFromFile.Count - num5 == this.messagesAddedSinceLastView;
				num4 -= this.DrawLogEntry(logsFromFile[num5], dest, sb, HighlightKeywords, num, num4, y, needsNewMessagesLineDraw, out dest);
				dest.Y -= num2;
				num6 += num2;
				num5--;
			}
			if (num5 <= -1 && num4 > 1)
			{
				int num7 = num + 8;
				Rectangle rectangle = new Rectangle(dest.X, dest.Y + dest.Height - num7, dest.Width, num7);
				SpriteFont tinyfont = GuiData.tinyfont;
				string text = "--- " + LocaleTerms.Loc("Log Cleared by Administrator") + " ---";
				Vector2 vector = tinyfont.MeasureString(text);
				sb.DrawString(tinyfont, text, Utils.ClipVec2ForTextRendering(new Vector2((float)rectangle.X + (float)rectangle.Width / 2f - vector.X / 2f, (float)rectangle.Y + (float)rectangle.Height / 2f - vector.Y / 2f)), Color.Gray);
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000C454 File Offset: 0x0000A654
		private int DrawLogEntry(IRCSystem.IRCLogEntry log, Rectangle startingDest, SpriteBatch sb, Dictionary<string, Color> HighlightKeywords, int lineHeight, int linesRemaining, int yNotToPass, bool needsNewMessagesLineDraw, out Rectangle dest)
		{
			dest = startingDest;
			int num = 55;
			int num2 = 76;
			int num3 = 4;
			if (Settings.ActiveLocale != "en-us")
			{
				num2 = 78;
			}
			if (GuiData.ActiveFontConfig.name.ToLower() == "medium")
			{
				num2 = 92;
			}
			else if (GuiData.ActiveFontConfig.name.ToLower() == "large")
			{
				num2 = 115;
			}
			string text = "<" + log.Author + ">";
			int num4 = (int)(GuiData.tinyfont.MeasureString(text).X + (float)num3);
			num2 = Math.Max(num2, (int)(GuiData.tinyfont.MeasureString(text).X + (float)num3));
			int width = dest.Width - (num + num3 + num2);
			string text2 = log.Message;
			string[] array = new string[]
			{
				text2
			};
			if (!log.Message.StartsWith("!ATTACHMENT:"))
			{
				text2 = Utils.SuperSmartTwimForWidth(text2, width, GuiData.tinyfont);
				array = text2.Split(Utils.newlineDelim, StringSplitOptions.None);
			}
			Rectangle rectangle = new Rectangle(dest.X + num + num3 + num2, dest.Y, dest.Width - (num + num3 + num2), dest.Height);
			Rectangle dest2 = new Rectangle(dest.X, dest.Y, num + num2, dest.Height);
			Color color = Color.LightBlue;
			if (HighlightKeywords.ContainsKey(log.Author))
			{
				color = HighlightKeywords[log.Author];
			}
			Color defaultColor = Color.Lerp(Color.White, color, 0.22f);
			if (needsNewMessagesLineDraw)
			{
				int num5 = array.Length;
				Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height - lineHeight * num5 + 1, dest.Width, 1);
				sb.Draw(Utils.white, destinationRectangle, Color.White * 0.5f);
			}
			int num6 = array.Length - 1;
			while (num6 >= 0 && linesRemaining > 0)
			{
				if (num6 == 0)
				{
					this.DrawLine("[" + log.Timestamp + "] ", dest2, sb, Color.White);
					int x = dest2.X;
					dest2.X = dest2.X + dest2.Width - num4;
					this.DrawLine(text, dest2, sb, color);
					dest2.X = x;
				}
				this.DrawLine(array[num6], rectangle, sb, defaultColor);
				dest.Height -= lineHeight;
				rectangle.Height = dest.Height;
				dest2.Height = dest.Height;
				linesRemaining--;
				if (dest.Y + dest.Height - 6 <= yNotToPass)
				{
					needsNewMessagesLineDraw = false;
					break;
				}
				num6--;
			}
			Rectangle destinationRectangle2 = rectangle;
			destinationRectangle2.Width = 1;
			destinationRectangle2.X -= 5;
			destinationRectangle2.Height = lineHeight * array.Length + 4;
			destinationRectangle2.Y = rectangle.Y + rectangle.Height + 2;
			sb.Draw(Utils.white, destinationRectangle2, Color.White * 0.12f);
			return array.Length;
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000C7E0 File Offset: 0x0000A9E0
		private void DrawLine(string line, Rectangle dest, SpriteBatch sb, Color defaultColor)
		{
			Vector2 vector = Utils.ClipVec2ForTextRendering(new Vector2((float)dest.X, (float)(dest.Y + dest.Height) - (GuiData.ActiveFontConfig.tinyFontCharHeight + 1f)));
			if (line.StartsWith("!ATTACHMENT:"))
			{
				if (AttachmentRenderer.RenderAttachment(line.Substring("!ATTACHMENT:".Length), OS.currentInstance, vector, this.DrawnButtonIndex, this.AttachmentPressedSound))
				{
					this.DrawnButtonIndex++;
				}
			}
			else if (line.StartsWith("!ANNOUNCEMENT!"))
			{
				Rectangle destinationRectangle = new Rectangle((int)vector.X - 2, (int)vector.Y, dest.Width + 2, (int)(GuiData.ActiveFontConfig.tinyFontCharHeight + 6f));
				sb.Draw(Utils.white, destinationRectangle, Color.Red * 0.22f);
				sb.DrawString(GuiData.tinyfont, line, vector, defaultColor);
			}
			else
			{
				sb.DrawString(GuiData.tinyfont, line, vector, defaultColor);
			}
		}

		// Token: 0x040000A6 RID: 166
		private const string ACTIVE_LOG_FILENAME = "active.log";

		// Token: 0x040000A7 RID: 167
		public const string ATTACHMENT_FLAG_PREFIX = "!ATTACHMENT:";

		// Token: 0x040000A8 RID: 168
		public const string ANNOUNCE_FLAG_PREFIX = "!ANNOUNCEMENT!";

		// Token: 0x040000A9 RID: 169
		private const string EntryLineDelimiterMarker = "\n#";

		// Token: 0x040000AA RID: 170
		private static string[] EntryLineDelimiter = new string[]
		{
			"\n#"
		};

		// Token: 0x040000AB RID: 171
		private Folder StorageFolder;

		// Token: 0x040000AC RID: 172
		private FileEntry ActiveLogFile;

		// Token: 0x040000AD RID: 173
		private int DrawnButtonIndex = 0;

		// Token: 0x040000AE RID: 174
		private int messagesAddedSinceLastView = 0;

		// Token: 0x040000AF RID: 175
		private bool isCurrentlyBeingViewed = true;

		// Token: 0x040000B0 RID: 176
		public SoundEffect AttachmentPressedSound = null;

		// Token: 0x040000B1 RID: 177
		public Action<string, string> LogAdded;

		// Token: 0x02000016 RID: 22
		public struct IRCLogEntry
		{
			// Token: 0x060000B2 RID: 178 RVA: 0x0000C928 File Offset: 0x0000AB28
			public string Serialize()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append((this.Timestamp == null) ? "" : this.Timestamp);
				stringBuilder.Append("//");
				stringBuilder.Append(this.Author);
				stringBuilder.Append("//");
				stringBuilder.Append(this.Message.Replace("//", "&dsr"));
				return stringBuilder.ToString();
			}

			// Token: 0x060000B3 RID: 179 RVA: 0x0000C9A4 File Offset: 0x0000ABA4
			public static IRCSystem.IRCLogEntry Deserialize(string entry)
			{
				IRCSystem.IRCLogEntry result = default(IRCSystem.IRCLogEntry);
				string[] array = entry.Split(IRCSystem.IRCLogEntry.SplitDelmiter, StringSplitOptions.None);
				result.Timestamp = array[0];
				result.Author = array[1];
				result.Message = array[2].Replace("&dsr", "//");
				return result;
			}

			// Token: 0x060000B4 RID: 180 RVA: 0x0000C9FC File Offset: 0x0000ABFC
			public static IRCSystem.IRCLogEntry DeserializeSafe(string entry)
			{
				IRCSystem.IRCLogEntry result;
				try
				{
					IRCSystem.IRCLogEntry irclogEntry = IRCSystem.IRCLogEntry.Deserialize(entry);
					result = irclogEntry;
				}
				catch (Exception)
				{
					result = default(IRCSystem.IRCLogEntry);
				}
				return result;
			}

			// Token: 0x040000B2 RID: 178
			private const string Delimiter = "//";

			// Token: 0x040000B3 RID: 179
			private const string SerializationDelimiterReplacement = "&dsr";

			// Token: 0x040000B4 RID: 180
			private static string[] SplitDelmiter = new string[]
			{
				"//"
			};

			// Token: 0x040000B5 RID: 181
			public string Message;

			// Token: 0x040000B6 RID: 182
			public string Timestamp;

			// Token: 0x040000B7 RID: 183
			public string Author;
		}
	}
}
