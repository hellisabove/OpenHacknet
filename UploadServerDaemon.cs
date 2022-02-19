using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000175 RID: 373
	internal class UploadServerDaemon : AuthenticatingDaemon
	{
		// Token: 0x0600095A RID: 2394 RVA: 0x0009B408 File Offset: 0x00099608
		public UploadServerDaemon(Computer computer, string serviceName, Color themeColor, OS opSystem, string foldername = null, bool needsAuthentication = false) : base(computer, serviceName, opSystem)
		{
			this.arrow = this.os.content.Load<Texture2D>("Arrow");
			this.themeColor = themeColor;
			this.lightThemeColor = Color.Lerp(themeColor, Color.White, 0.4f);
			this.darkThemeColor = Color.Lerp(themeColor, Color.Black, 0.8f);
			this.Foldername = foldername;
			if (this.Foldername == null)
			{
				this.Foldername = "Drop";
			}
			this.needsAuthentication = needsAuthentication;
			UploadServerDaemon.MESSAGE_FILE_DATA = Utils.readEntireFile("Content/LocPost/UploadServerText.txt");
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x0009B4C4 File Offset: 0x000996C4
		public override string getSaveString()
		{
			return string.Concat(new object[]
			{
				"<UploadServerDaemon name=\"",
				this.name,
				"\" foldername=\"",
				this.Foldername,
				"\" color=\"",
				Utils.convertColorToParseableString(this.themeColor),
				"\" needsAuth=\"",
				this.needsAuthentication,
				"\" hasReturnViewButton=\"",
				this.hasReturnViewButton,
				"\" />"
			});
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x0009B554 File Offset: 0x00099754
		public override void initFiles()
		{
			base.initFiles();
			this.root = this.comp.files.root.searchForFolder(this.Foldername);
			if (this.root == null)
			{
				this.root = new Folder(this.Foldername);
				this.comp.files.root.folders.Add(this.root);
			}
			this.storageFolder = this.root.searchForFolder("Uploads");
			if (this.storageFolder == null)
			{
				this.storageFolder = new Folder("Uploads");
				this.root.folders.Add(this.storageFolder);
			}
			this.root.files.Add(new FileEntry(UploadServerDaemon.MESSAGE_FILE_DATA, "Server_Message.txt"));
			this.uploadFileCountLastFrame = this.storageFolder.files.Count;
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x0009B654 File Offset: 0x00099854
		public override void loadInit()
		{
			base.loadInit();
			this.root = this.comp.files.root.searchForFolder(this.Foldername);
			this.storageFolder = this.root.searchForFolder("Uploads");
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x0009B6A0 File Offset: 0x000998A0
		public override void navigatedTo()
		{
			base.navigatedTo();
			if (!this.needsAuthentication)
			{
				this.moveToActiveState();
			}
			else
			{
				this.state = UploadServerDaemon.UploadServerState.Menu;
			}
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x0009B72C File Offset: 0x0009992C
		private void moveToActiveState()
		{
			this.state = UploadServerDaemon.UploadServerState.Active;
			Programs.sudo(this.os, delegate
			{
				string[] args = new string[]
				{
					"cd",
					this.Foldername + "/Uploads"
				};
				Programs.cd(args, this.os);
				this.os.display.command = this.name;
			});
			if (!this.comp.userLoggedIn)
			{
				this.comp.userLoggedIn = true;
			}
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x0009B774 File Offset: 0x00099974
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.drawBackground(bounds, sb);
			int num = bounds.X + 10;
			int num2 = bounds.Y + 10;
			TextItem.doFontLabel(new Vector2((float)num, (float)num2), this.name, GuiData.font, null, (float)(bounds.Width - 20), (float)bounds.Height, false);
			num2 += (int)GuiData.font.MeasureString(this.name).Y + 2;
			string text = "Error: System.IO.FileNotFoundException went unhandled\n";
			text += "File Server_Message.txt not found\n";
			text += "at UploadDaemonCore.RenderModule.HelptextDisplay.cs\nline 107 position 95";
			FileEntry fileEntry = this.root.searchForFile("Server_Message.txt");
			if (fileEntry != null)
			{
				text = fileEntry.data;
			}
			Vector2 vector = TextItem.doMeasuredFontLabel(new Vector2((float)num, (float)num2), text, GuiData.tinyfont, null, float.MaxValue, float.MaxValue);
			if (this.hasReturnViewButton)
			{
				if (Button.doButton(50821549, num - 2, (int)((float)num2 + vector.Y + 8f), bounds.Width / 3, 26, "Exit Upload View", new Color?(this.themeColor)))
				{
					this.os.display.command = "connect";
				}
			}
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x0009B8D4 File Offset: 0x00099AD4
		public void drawBackground(Rectangle bounds, SpriteBatch sb)
		{
			bounds = new Rectangle(bounds.X + 1, bounds.Y + 1, bounds.Width - 2, bounds.Height - 2);
			sb.Draw(Utils.gradient, bounds, this.themeColor);
			sb.Draw(this.arrow, new Rectangle(bounds.X, bounds.Y + bounds.Height / 3, bounds.Width / 2, (int)((float)bounds.Height * 0.66666f) + 1), new Rectangle?(new Rectangle(this.arrow.Width / 2, 0, this.arrow.Width / 2, this.arrow.Height)), this.darkThemeColor);
			if (this.arrowPositions == null)
			{
				this.arrowPositions = new List<Vector2>();
				this.arrowFades = new List<float>();
				this.arrowDepths = new List<float>();
				for (int i = 0; i < 90; i++)
				{
					float num = 70f;
					Vector2 vector = new Vector2((float)((double)(-(double)num) + Utils.random.NextDouble() * (double)(num + (float)bounds.Width)), (float)((double)bounds.Height / (Utils.random.NextDouble() * 2.0)));
					this.arrowPositions.Add(vector);
					this.arrowFades.Add((float)(0.10000000149011612 + 0.8999999985098839 * Utils.random.NextDouble()));
					this.arrowDepths.Add((float)(0.20000000298023224 + (Utils.random.NextDouble() - 0.2)));
				}
			}
			float num2 = (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			for (int i = 0; i < this.arrowPositions.Count; i++)
			{
				List<Vector2> list;
				int index;
				(list = this.arrowPositions)[index = i] = list[index] - num2 * UploadServerDaemon.ARROW_VELOCITY * this.arrowDepths[i];
				List<float> list2;
				(list2 = this.arrowFades)[index = i] = list2[index] - num2 / 2f;
				if (this.arrowFades[i] < 0f)
				{
					float num = 70f;
					Vector2 vector = new Vector2((float)((double)(-(double)num) + Utils.random.NextDouble() * (double)(num + (float)bounds.Width)), (float)((double)bounds.Height / (Utils.random.NextDouble() * 2.0)));
					this.arrowPositions[i] = vector;
					this.arrowFades[i] = (float)(0.5 + 1.5 * Utils.random.NextDouble());
					this.arrowDepths[i] = (float)(0.20000000298023224 + Utils.random.NextDouble() * 0.4);
				}
				float num3 = 0.3f * this.arrowDepths[i];
				Vector2 vector2 = new Vector2((float)bounds.X + this.arrowPositions[i].X, (float)(bounds.Y + bounds.Height) + this.arrowPositions[i].Y);
				Rectangle clipRectForSpritePos = Utils.getClipRectForSpritePos(bounds, this.arrow, vector2, num3);
				Vector2 value = new Vector2((float)clipRectForSpritePos.X, (float)clipRectForSpritePos.Y);
				vector2 += value;
				Rectangle destinationRectangle = new Rectangle((int)vector2.X, (int)vector2.Y, (int)(num3 * (float)(clipRectForSpritePos.Width - clipRectForSpritePos.X)), (int)(num3 * (float)(clipRectForSpritePos.Height - clipRectForSpritePos.Y)));
				sb.Draw(this.arrow, destinationRectangle, new Rectangle?(clipRectForSpritePos), this.lightThemeColor * (this.arrowDepths[i] * 1.5f) * this.arrowFades[i], 0f, Vector2.Zero, SpriteEffects.None, this.arrowDepths[i]);
			}
			this.drawUploadDetectedEffect(bounds, sb);
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0009BD38 File Offset: 0x00099F38
		public void drawUploadDetectedEffect(Rectangle bounds, SpriteBatch sb)
		{
			this.uploadDetectedEffectTimer -= (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
			if (this.uploadFileCountLastFrame < this.storageFolder.files.Count)
			{
				this.uploadDetectedEffectTimer = 4f;
				this.uploadFileCountLastFrame = this.storageFolder.files.Count;
			}
			if (this.uploadDetectedEffectTimer > 0f)
			{
				Color value = Color.White;
				value.A = 0;
				value *= this.uploadDetectedEffectTimer / 4f;
				bool drawShadow = TextItem.DrawShadow;
				TextItem.DrawShadow = false;
				TextItem.doFontLabel(new Vector2((float)(bounds.X + bounds.Width / 3), (float)(bounds.Y + bounds.Height - 60)), "UPLOAD COMPLETE", GuiData.titlefont, new Color?(value), (float)(bounds.Width / 3 * 2), 58f, false);
				TextItem.DrawShadow = drawShadow;
			}
		}

		// Token: 0x04000AF2 RID: 2802
		private const string DEFAULT_FOLDERNAME = "Drop";

		// Token: 0x04000AF3 RID: 2803
		private const string STORAGE_FOLDER_FOLDERNAME = "Uploads";

		// Token: 0x04000AF4 RID: 2804
		private const string MESSAGE_FILENAME = "Server_Message.txt";

		// Token: 0x04000AF5 RID: 2805
		private const int NUMBER_OF_ARROWS = 90;

		// Token: 0x04000AF6 RID: 2806
		private const float MAX_FADE_TIME = 2f;

		// Token: 0x04000AF7 RID: 2807
		private static string MESSAGE_FILE_DATA = null;

		// Token: 0x04000AF8 RID: 2808
		private static Vector2 ARROW_VELOCITY = new Vector2(0f, 400f);

		// Token: 0x04000AF9 RID: 2809
		private Texture2D arrow;

		// Token: 0x04000AFA RID: 2810
		private Color themeColor;

		// Token: 0x04000AFB RID: 2811
		private Color darkThemeColor;

		// Token: 0x04000AFC RID: 2812
		private Color lightThemeColor;

		// Token: 0x04000AFD RID: 2813
		private Folder root;

		// Token: 0x04000AFE RID: 2814
		private Folder storageFolder;

		// Token: 0x04000AFF RID: 2815
		private UploadServerDaemon.UploadServerState state = UploadServerDaemon.UploadServerState.Menu;

		// Token: 0x04000B00 RID: 2816
		public string Foldername;

		// Token: 0x04000B01 RID: 2817
		public bool needsAuthentication;

		// Token: 0x04000B02 RID: 2818
		public bool hasReturnViewButton = false;

		// Token: 0x04000B03 RID: 2819
		private int uploadFileCountLastFrame;

		// Token: 0x04000B04 RID: 2820
		private float uploadDetectedEffectTimer = 0f;

		// Token: 0x04000B05 RID: 2821
		private List<Vector2> arrowPositions;

		// Token: 0x04000B06 RID: 2822
		private List<float> arrowFades;

		// Token: 0x04000B07 RID: 2823
		private List<float> arrowDepths;

		// Token: 0x02000176 RID: 374
		private enum UploadServerState
		{
			// Token: 0x04000B09 RID: 2825
			Menu,
			// Token: 0x04000B0A RID: 2826
			Login,
			// Token: 0x04000B0B RID: 2827
			Active
		}
	}
}
