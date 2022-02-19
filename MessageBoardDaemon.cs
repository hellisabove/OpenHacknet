using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x020000BC RID: 188
	internal class MessageBoardDaemon : AuthenticatingDaemon, IMonitorableDaemon
	{
		// Token: 0x060003CC RID: 972 RVA: 0x0003A9AC File Offset: 0x00038BAC
		public MessageBoardDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Message Board"), os)
		{
			if (MessageBoardDaemon.Images == null)
			{
				MessageBoardDaemon.Images = new Dictionary<MessageBoardPostImage, Texture2D>();
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Academic, os.content.Load<Texture2D>("Sprites/Academic_Logo"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Sun, os.content.Load<Texture2D>("Sprites/Sun"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Snake, os.content.Load<Texture2D>("Sprites/Snake"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Circle, os.content.Load<Texture2D>("CircleOutline"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Duck, os.content.Load<Texture2D>("Sprites/Duck"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Page, os.content.Load<Texture2D>("Sprites/Page"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Speech, os.content.Load<Texture2D>("Sprites/SpeechBubble"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Mod, os.content.Load<Texture2D>("Sprites/Hammer"));
				MessageBoardDaemon.Images.Add(MessageBoardPostImage.Chip, os.content.Load<Texture2D>("Sprites/Chip"));
			}
			this.threadsPanel = new ScrollableSectionedPanel(415, GuiData.spriteBatch.GraphicsDevice);
		}

		// Token: 0x060003CD RID: 973 RVA: 0x0003AB50 File Offset: 0x00038D50
		public override void initFiles()
		{
			base.initFiles();
			this.rootFolder = new Folder("ImageBoard");
			this.threadsFolder = new Folder("Threads");
			this.rootFolder.folders.Add(this.threadsFolder);
			this.comp.files.root.folders.Add(this.rootFolder);
			for (int i = 0; i < this.ThreadsToAdd.Count; i++)
			{
				this.AddThread(this.ThreadsToAdd[i]);
			}
			this.ThreadsToAdd.Clear();
		}

		// Token: 0x060003CE RID: 974 RVA: 0x0003ABF8 File Offset: 0x00038DF8
		public override void loadInit()
		{
			base.loadInit();
			this.rootFolder = this.comp.files.root.searchForFolder("ImageBoard");
			this.threadsFolder = this.rootFolder.searchForFolder("Threads");
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0003AC38 File Offset: 0x00038E38
		public void SubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			this.MessageAdded = (Action<string, string>)Delegate.Combine(this.MessageAdded, act);
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x0003AC52 File Offset: 0x00038E52
		public void UnSubscribeToAlertActionFroNewMessage(Action<string, string> act)
		{
			this.MessageAdded = (Action<string, string>)Delegate.Remove(this.MessageAdded, act);
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x0003AC6C File Offset: 0x00038E6C
		public bool ShouldDisplayNotifications()
		{
			return true;
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x0003AC80 File Offset: 0x00038E80
		public string GetName()
		{
			return this.name;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x0003AC98 File Offset: 0x00038E98
		public override string getSaveString()
		{
			return string.Concat(new string[]
			{
				"<MessageBoard name=\"",
				this.name,
				"\" boardName=\"",
				this.BoardName,
				"\"/>"
			});
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x0003ACE4 File Offset: 0x00038EE4
		public void AddThread(string threadData)
		{
			if (this.threadsFolder == null)
			{
				this.ThreadsToAdd.Add(threadData);
			}
			else
			{
				threadData = ComputerLoader.filter(threadData);
				string text;
				do
				{
					text = Utils.getRandomByte().ToString("000") + Utils.getRandomByte().ToString("000") + Utils.getRandomByte().ToString("000") + ".tm";
				}
				while (this.threadsFolder.searchForFile(text) != null);
				if (this.MessageAdded != null)
				{
					this.MessageAdded("anon", threadData);
				}
				this.threadsFolder.files.Add(new FileEntry(threadData, text));
			}
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x0003ADB4 File Offset: 0x00038FB4
		public MessageBoardThread ParseThread(string threadData)
		{
			MessageBoardThread result;
			try
			{
				string[] array = threadData.Split(new string[]
				{
					"------------------------------------------\r\n",
					"------------------------------------------\n",
					"------------------------------------------"
				}, StringSplitOptions.None);
				string text = array[0];
				text = text.Replace("\n", "");
				text = text.Replace("\r", "");
				MessageBoardThread messageBoardThread = new MessageBoardThread
				{
					id = text,
					posts = new List<MessageBoardPost>()
				};
				for (int i = 1; i < array.Length; i++)
				{
					if (array[i].Length > 1)
					{
						MessageBoardPost item = default(MessageBoardPost);
						string text2 = array[i];
						if (array[i].StartsWith("#"))
						{
							string value = array[i].Substring(1, array[i].IndexOf('\n'));
							try
							{
								MessageBoardPostImage img = (MessageBoardPostImage)Enum.Parse(typeof(MessageBoardPostImage), value);
								item.img = img;
								text2 = array[i].Substring(array[i].IndexOf('\n') + 1);
							}
							catch (ArgumentException)
							{
								item.img = MessageBoardPostImage.None;
							}
						}
						else
						{
							item.img = MessageBoardPostImage.None;
						}
						item.text = text2;
						messageBoardThread.posts.Add(item);
					}
				}
				result = messageBoardThread;
			}
			catch (Exception)
			{
				result = new MessageBoardThread
				{
					id = "error",
					posts = new List<MessageBoardPost>(new MessageBoardPost[]
					{
						new MessageBoardPost
						{
							img = MessageBoardPostImage.None,
							text = "-- Error Parsing Thread --"
						}
					})
				};
			}
			return result;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x0003AFAC File Offset: 0x000391AC
		public void ViewThread(MessageBoardThread thread, int width, int margin, int ImageSize, int headerOffset)
		{
			this.CurrentThreadHeight = 20;
			for (int i = 0; i < thread.posts.Count; i++)
			{
				this.CurrentThreadHeight += this.MeasurePost(thread.posts[i], width, margin, ImageSize, headerOffset) + margin * 2;
			}
			this.state = MessageBoardDaemon.MessageBoardState.Thread;
			this.viewingThread = thread;
			this.ThreadScrollPosition = Vector2.Zero;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x0003B022 File Offset: 0x00039222
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.state = MessageBoardDaemon.MessageBoardState.Board;
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0003B06C File Offset: 0x0003926C
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			int num = 60;
			Rectangle dest = new Rectangle(bounds.X, bounds.Y, bounds.Width, num);
			this.DrawHeader(sb, dest);
			Rectangle rectangle = bounds;
			rectangle.Y += num;
			rectangle.Height -= num + 1;
			switch (this.state)
			{
			case MessageBoardDaemon.MessageBoardState.Thread:
				this.DrawFullThreadView(sb, this.viewingThread, rectangle);
				return;
			}
			this.threadsPanel.NumberOfPanels = this.threadsFolder.files.Count;
			Action<int, Rectangle, SpriteBatch> drawSection = delegate(int index, Rectangle drawArea, SpriteBatch sBatch)
			{
				MessageBoardThread thread = this.ParseThread(this.threadsFolder.files[index].data);
				this.DrawThread(thread, sBatch, drawArea, true);
			};
			this.threadsPanel.Draw(drawSection, sb, rectangle);
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x0003B140 File Offset: 0x00039340
		private void DrawHeader(SpriteBatch sb, Rectangle dest)
		{
			int num = 4;
			int num2 = 200;
			sb.Draw(Utils.white, new Rectangle(dest.X + num, dest.Y + 4, dest.Width - num * 2, 1), Color.White * 0.5f);
			Vector2 position = new Vector2((float)(dest.X + num), (float)(dest.Y + 5));
			int num3 = 0;
			int num4 = 7;
			while (position.X + (float)(num4 * 2) < (float)(dest.X + dest.Width) && num3 < this.boardsListingString.Length)
			{
				sb.DrawString(GuiData.detailfont, string.Concat(this.boardsListingString[num3]), position, Color.White * 0.6f);
				num3++;
				position.X += (float)num4;
			}
			sb.DrawString(GuiData.detailfont, "]", position, Color.White * 0.8f);
			try
			{
				if (this.BoardName == null)
				{
					this.BoardName = this.name;
				}
				TextItem.doFontLabel(new Vector2((float)(dest.X + num), (float)(dest.Y + 22)), this.BoardName, GuiData.font, new Color?(this.os.highlightColor), (float)(dest.Width - (num2 + 6 + 22)), (float)dest.Height, false);
			}
			catch (Exception)
			{
			}
			if (this.state != MessageBoardDaemon.MessageBoardState.Board)
			{
				if (Button.doButton(1931655802, dest.X + dest.Width - num2 - 6, dest.Y + dest.Height / 2 - 4, num2, 24, LocaleTerms.Loc("Back to Board"), new Color?(this.os.highlightColor)))
				{
					this.state = MessageBoardDaemon.MessageBoardState.Board;
				}
			}
			sb.Draw(Utils.white, new Rectangle(dest.X + num, dest.Y + dest.Height - 2, dest.Width - num * 2, 1), Color.White * 0.5f);
		}

		// Token: 0x060003DA RID: 986 RVA: 0x0003B39C File Offset: 0x0003959C
		private void DrawFullThreadView(SpriteBatch sb, MessageBoardThread thread, Rectangle dest)
		{
			Rectangle rectangle = dest;
			rectangle.Height = this.CurrentThreadHeight + 50;
			bool flag = this.CurrentThreadHeight > dest.Height;
			if (flag)
			{
				ScrollablePanel.beginPanel(1931655001, rectangle, this.ThreadScrollPosition);
				rectangle.X = (rectangle.Y = 0);
			}
			this.DrawThread(thread, GuiData.spriteBatch, rectangle, false);
			if (flag)
			{
				float maxScroll = (float)Math.Max(dest.Height, this.CurrentThreadHeight - dest.Height);
				this.ThreadScrollPosition = ScrollablePanel.endPanel(1931655001, this.ThreadScrollPosition, dest, maxScroll, false);
			}
		}

		// Token: 0x060003DB RID: 987 RVA: 0x0003B44C File Offset: 0x0003964C
		private void DrawThread(MessageBoardThread thread, SpriteBatch sb, Rectangle bounds, bool isPreview = false)
		{
			int num = 4;
			int num2 = 8;
			int num3 = 450;
			int height = 36;
			int num4 = 20;
			int num5 = 80;
			int num6 = 30;
			SpriteFont tinyfont = GuiData.tinyfont;
			Rectangle dest = new Rectangle(bounds.X + num2, bounds.Y + num2, num3, height);
			int num7 = bounds.Y;
			List<MessageBoardPost> list = thread.posts;
			if (isPreview)
			{
				list = this.GetLastPostsToFitHeight(thread, 415 - num6, bounds.Width, num2, num5, num4, num6, int.MaxValue);
			}
			for (int i = 0; i < list.Count; i++)
			{
				MessageBoardPost messageBoardPost = list[i];
				int num8 = bounds.Width - 4 * num2;
				if (messageBoardPost.img != MessageBoardPostImage.None)
				{
					num8 -= num5;
				}
				string text = Utils.SmartTwimForWidth(messageBoardPost.text, num8, tinyfont);
				Vector2 vector = tinyfont.MeasureString(text);
				vector.Y *= 1.3f;
				dest.Y = num7;
				int num9 = (int)vector.X + num2 * 4;
				if (messageBoardPost.img != MessageBoardPostImage.None)
				{
					num9 += num5 + num2;
				}
				dest.Width = Math.Max(num3, num9);
				dest.Height = (int)vector.Y + 2 * num2;
				dest.Width = Math.Max(dest.Width, num5 + 2 * num2);
				if (messageBoardPost.img != MessageBoardPostImage.None)
				{
					dest.Height = Math.Max(dest.Height, num5 + 2 * num2);
				}
				dest.Height += num4 + num2 * 2;
				this.DrawPost(text, messageBoardPost.img, dest, num2, num5, num4, sb, tinyfont);
				num7 += dest.Height + num;
				if (i == 0 && isPreview)
				{
					MessageBoardThread thread2 = thread;
					int num10 = thread.posts.Count - list.Count;
					string text2 = "[+] " + string.Format(LocaleTerms.Loc("{0} posts and image replies omitted"), num10);
					string text3 = LocaleTerms.Loc("Click here to view.");
					TextItem.doFontLabel(new Vector2((float)dest.X, (float)num7), text2, GuiData.tinyfont, new Color?(this.os.lightGray), 288f, 18f, true);
					if (Button.doButton(17839000 + thread.id.GetHashCode(), dest.X + 290, num7 - 2, bounds.Width - 316, 17, text3, null))
					{
						Console.WriteLine("clicked " + i);
						this.ViewThread(thread2, bounds.Width, num2, num5, num4);
					}
					num7 += 16 + num;
				}
			}
			sb.Draw(Utils.white, new Rectangle(bounds.X + num2, bounds.Y + bounds.Height - 6, bounds.Width - num2 * 2, 1), Color.White * 0.5f);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x0003B790 File Offset: 0x00039990
		private int MeasurePost(MessageBoardPost post, int width, int margin, int ImageSize, int postHeaderOffset)
		{
			SpriteFont tinyfont = GuiData.tinyfont;
			int num = width - 4 * margin;
			if (post.img != MessageBoardPostImage.None)
			{
				num -= ImageSize;
			}
			string text = Utils.SmartTwimForWidth(post.text, num, tinyfont);
			Vector2 vector = tinyfont.MeasureString(text);
			vector.Y *= 1.3f;
			int num2 = postHeaderOffset + (int)vector.Y;
			num2 += 2 * margin;
			return Math.Max(num2, ImageSize + 2 * margin);
		}

		// Token: 0x060003DD RID: 989 RVA: 0x0003B814 File Offset: 0x00039A14
		private void DrawPost(string text, MessageBoardPostImage img, Rectangle dest, int margin, int ImageSize, int postheaderOffset, SpriteBatch sb, SpriteFont font)
		{
			sb.Draw(Utils.white, dest, this.os.highlightColor * 0.2f);
			Vector2 value = new Vector2((float)(dest.X + margin), (float)(dest.Y + margin));
			sb.Draw(Utils.white, new Rectangle((int)value.X, (int)value.Y, postheaderOffset - 5, postheaderOffset - 5), this.os.indentBackgroundColor);
			sb.DrawString(GuiData.smallfont, LocaleTerms.Loc("Anonymous"), value + new Vector2((float)postheaderOffset, -2f), MessageBoardDaemon.UsernameColor);
			Vector2 vector = GuiData.smallfont.MeasureString(LocaleTerms.Loc("Anonymous"));
			sb.DrawString(GuiData.detailfont, "01/01/1970(Thu)00:00 UTC+0:0", value + new Vector2(Math.Max(112f - (float)postheaderOffset, vector.X + 4f) + (float)postheaderOffset, 3f), Utils.SlightlyDarkGray);
			value.Y += (float)postheaderOffset;
			if (img != MessageBoardPostImage.None && MessageBoardDaemon.Images.ContainsKey(img))
			{
				Rectangle destinationRectangle = new Rectangle(dest.X + margin, dest.Y + margin + postheaderOffset, ImageSize, ImageSize);
				sb.Draw(MessageBoardDaemon.Images[img], destinationRectangle, Color.White);
				value.X += (float)(ImageSize + margin + margin);
			}
			string[] array = text.Split(Utils.newlineDelim);
			float y = font.MeasureString(array[0]).Y;
			for (int i = 0; i < array.Length; i++)
			{
				sb.DrawString(font, array[i], value + new Vector2(0f, (float)i * (y + 2f)), array[i].StartsWith(">") ? MessageBoardDaemon.ImplicationColor : Color.White);
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x0003BA24 File Offset: 0x00039C24
		private List<MessageBoardPost> GetLastPostsToFitHeight(MessageBoardThread thread, int height, int width, int margin, int ImageSize, int PostHeaderOffset, int ThreadFooterSize, int maxOPSize = 2147483647)
		{
			List<MessageBoardPost> list = new List<MessageBoardPost>();
			int num = ThreadFooterSize + margin * 4;
			int num2 = this.MeasurePost(thread.posts[0], width, margin, ImageSize, PostHeaderOffset);
			num2 = Math.Min(maxOPSize, num2);
			list.Add(thread.posts[0]);
			num += num2;
			for (int i = thread.posts.Count - 1; i >= 0; i--)
			{
				num2 = this.MeasurePost(thread.posts[i], width, margin, ImageSize, PostHeaderOffset);
				if (num + num2 >= height)
				{
					break;
				}
				list.Insert(1, thread.posts[i]);
				num += num2;
			}
			return list;
		}

		// Token: 0x0400046A RID: 1130
		private const int THREAD_PREVIEW_HEIGHT = 415;

		// Token: 0x0400046B RID: 1131
		private static Dictionary<MessageBoardPostImage, Texture2D> Images;

		// Token: 0x0400046C RID: 1132
		private static Color UsernameColor = new Color(17, 119, 67);

		// Token: 0x0400046D RID: 1133
		private static Color ImplicationColor = new Color(56, 184, 131);

		// Token: 0x0400046E RID: 1134
		private string boardsListingString = "[a/b/c/d/e/f/g/gif/h/hr/k/m/o/p/r/s/t/u/v/vg/vr/w/wg][i/ic][r9k][s4s][cm/hm/lgbt/y][3/adv/an/asp/cgl/ck/co/diy/fa/fit/gd/hc/int/jp/lit/mlp/mu/n/out/po/pol/sci/soc/sp/tg/toy/trv/tv/vp/wsg/x][rs]";

		// Token: 0x0400046F RID: 1135
		public string BoardName = "/el/ - " + LocaleTerms.Loc("Digital Security");

		// Token: 0x04000470 RID: 1136
		public List<string> ThreadsToAdd = new List<string>();

		// Token: 0x04000471 RID: 1137
		private MessageBoardDaemon.MessageBoardState state = MessageBoardDaemon.MessageBoardState.Board;

		// Token: 0x04000472 RID: 1138
		private ScrollableSectionedPanel threadsPanel;

		// Token: 0x04000473 RID: 1139
		private MessageBoardThread viewingThread;

		// Token: 0x04000474 RID: 1140
		private Vector2 ThreadScrollPosition = Vector2.Zero;

		// Token: 0x04000475 RID: 1141
		private int CurrentThreadHeight = 100;

		// Token: 0x04000476 RID: 1142
		private Folder rootFolder;

		// Token: 0x04000477 RID: 1143
		private Folder threadsFolder;

		// Token: 0x04000478 RID: 1144
		public Action<string, string> MessageAdded;

		// Token: 0x020000BD RID: 189
		private enum MessageBoardState
		{
			// Token: 0x0400047A RID: 1146
			Thread,
			// Token: 0x0400047B RID: 1147
			Board
		}
	}
}
