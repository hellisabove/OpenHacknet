using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000007 RID: 7
	internal class DatabaseDaemon : Daemon
	{
		// Token: 0x0600002B RID: 43 RVA: 0x00004104 File Offset: 0x00002304
		public static DatabaseDaemon.DatabasePermissions GetDatabasePermissionsFromString(string data)
		{
			DatabaseDaemon.DatabasePermissions result = DatabaseDaemon.DatabasePermissions.Public;
			if (data.ToLower() == "private" || data.ToLower().StartsWith("admin"))
			{
				result = DatabaseDaemon.DatabasePermissions.AdminOnly;
			}
			return result;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00004148 File Offset: 0x00002348
		public DatabaseDaemon(Computer c, OS os, string name, DatabaseDaemon.DatabasePermissions permissions, string DataTypeIdentifier, string Foldername = null, Color? ThemeColor = null) : base(c, name, os)
		{
			this.Init(c, os, name, permissions, DataTypeIdentifier, (Foldername == null) ? "Database" : Foldername, (ThemeColor == null) ? os.highlightColor : ThemeColor.Value);
			this.HadThemeColorApplied = (ThemeColor != null);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00004214 File Offset: 0x00002414
		public DatabaseDaemon(Computer c, OS os, string name, string permissions, string DataTypeIdentifier, string Foldername = null, Color? ThemeColor = null) : base(c, name, os)
		{
			this.Init(c, os, name, DatabaseDaemon.GetDatabasePermissionsFromString(permissions), DataTypeIdentifier, (Foldername == null) ? "Database" : Foldername, (ThemeColor == null) ? os.highlightColor : ThemeColor.Value);
			this.HadThemeColorApplied = (ThemeColor != null);
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000042E4 File Offset: 0x000024E4
		private void Init(Computer c, OS os, string name, DatabaseDaemon.DatabasePermissions permissions, string DataTypeIdentifier, string Foldername, Color themeColor)
		{
			this.sideEffect = new MovingBarsEffect();
			this.Permissions = permissions;
			this.State = DatabaseDaemon.DatabaseState.Welcome;
			this.DataTypeIdentifier = DataTypeIdentifier;
			this.DataType = ObjectSerializer.GetTypeForName(DataTypeIdentifier);
			this.ScrollPanel = new ScrollableSectionedPanel(26, GuiData.spriteBatch.GraphicsDevice);
			this.TextRegion = new ScrollableTextRegion(GuiData.spriteBatch.GraphicsDevice);
			this.Foldername = ((Foldername == null) ? "Database" : Foldername);
			this.ThemeColor = themeColor;
			if (DataTypeIdentifier.EndsWith("NeopalsAccount"))
			{
				this.WildcardAssets.Add(Neopal.PetType.Blundo, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Blundo"));
				this.WildcardAssets.Add(Neopal.PetType.Chisha, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Chisha"));
				this.WildcardAssets.Add(Neopal.PetType.Jubdub, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Jubdub"));
				this.WildcardAssets.Add(Neopal.PetType.Kachici, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Kachici"));
				this.WildcardAssets.Add(Neopal.PetType.Kyrill, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Kyrill"));
				this.WildcardAssets.Add(Neopal.PetType.Myncl, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Myncl"));
				this.WildcardAssets.Add(Neopal.PetType.Pageri, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Pageri"));
				this.WildcardAssets.Add(Neopal.PetType.Psybunny, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Psybunny"));
				this.WildcardAssets.Add(Neopal.PetType.Scorchum, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Scorchum"));
				this.WildcardAssets.Add(Neopal.PetType.Unisam, os.content.Load<Texture2D>("DLC/Sprites/Neopals/Unisam"));
			}
			this.PlaceholderSprite = os.content.Load<Texture2D>("Sprites/Chip");
			this.Triangle = os.content.Load<Texture2D>("DLC/Sprites/Triangle");
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00004504 File Offset: 0x00002704
		public override void initFiles()
		{
			base.initFiles();
			this.DatasetFolder = this.comp.files.root.searchForFolder(this.Foldername);
			if (this.DatasetFolder == null)
			{
				this.DatasetFolder = new Folder(this.Foldername);
				this.comp.files.root.folders.Add(this.DatasetFolder);
			}
			this.InitDataset();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00004588 File Offset: 0x00002788
		public override void loadInit()
		{
			base.loadInit();
			this.DatasetFolder = this.comp.files.root.searchForFolder(this.Foldername);
			this.DataType = ObjectSerializer.GetTypeForName(this.DataTypeIdentifier);
			string dataTypeIdentifier = this.DataTypeIdentifier;
			if (dataTypeIdentifier != null)
			{
				if (dataTypeIdentifier == "NeopalsAccount")
				{
					this.HasSpecialCaseDraw = true;
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000045F4 File Offset: 0x000027F4
		private void InitDataset()
		{
			if (this.Dataset != null)
			{
				if (this.DataType == typeof(Person))
				{
					this.FilenameIsPersonName = true;
				}
				else
				{
					this.FilenameIsPersonName = false;
				}
				for (int i = 0; i < this.Dataset.Count; i++)
				{
					string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject(this.Dataset[i]));
					string nameEntry = this.GetFilenameForObject(this.Dataset[i]);
					try
					{
					}
					catch (Exception arg)
					{
						Console.WriteLine("Deserialization error for Generic Object: " + arg);
					}
					this.DatasetFolder.files.Add(new FileEntry(dataEntry, nameEntry));
				}
			}
			else if (this.DataTypeIdentifier.EndsWith("VehicleInfo"))
			{
				this.FilenameIsPersonName = true;
				this.DataType = People.all[0].vehicles.GetType();
				for (int i = 0; i < People.all.Count; i++)
				{
					if (People.all[i].vehicles.Count != 0)
					{
						string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject(People.all[i].vehicles));
						List<VehicleRegistration> vehicles = People.all[i].vehicles;
						try
						{
							string nameEntry = this.GetFilenameForPersonName(People.all[i].firstName, People.all[i].lastName);
							this.DatasetFolder.files.Add(new FileEntry(dataEntry, nameEntry));
						}
						catch (Exception arg)
						{
							Console.WriteLine("Deserialization error for Vehicle: " + arg);
						}
					}
				}
			}
			else if (this.DataTypeIdentifier.EndsWith("NeopalsAccount"))
			{
				this.FilenameIsPersonName = false;
				this.HasSpecialCaseDraw = true;
				this.DataType = typeof(NeopalsAccount);
				if (People.all.Count != 0)
				{
					for (int i = 0; i < People.all.Count; i++)
					{
						if (People.all[i].NeopalsAccount != null)
						{
							string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject(People.all[i].NeopalsAccount));
							NeopalsAccount neopalsAccount = People.all[i].NeopalsAccount;
							try
							{
								string nameEntry = this.GetFilenameForObject(neopalsAccount);
								this.DatasetFolder.files.Add(new FileEntry(dataEntry, nameEntry));
							}
							catch (Exception arg)
							{
								Console.WriteLine("Deserialization error for NeopalsAccount: " + arg);
							}
						}
					}
				}
			}
			else if (this.DataTypeIdentifier.EndsWith("Person"))
			{
				this.FilenameIsPersonName = true;
				this.DataType = People.all[0].GetType();
				for (int i = 0; i < People.all.Count; i++)
				{
					if (People.all[i].vehicles.Count != 0)
					{
						string dataEntry = this.CleanXMLForFile(ObjectSerializer.SerializeObject(People.all[i]));
						Person person = People.all[i];
						try
						{
							string nameEntry = this.GetFilenameForPersonName(People.all[i].firstName, People.all[i].lastName);
							this.DatasetFolder.files.Add(new FileEntry(dataEntry, nameEntry));
						}
						catch (Exception arg)
						{
							Console.WriteLine("Deserialization error for People: " + arg);
						}
					}
				}
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00004A10 File Offset: 0x00002C10
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.State = DatabaseDaemon.DatabaseState.Welcome;
			this.AdminEmailResetStarted = false;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00004A28 File Offset: 0x00002C28
		public object GetObjectForRecordName(string recordName)
		{
			for (int i = 0; i < this.DatasetFolder.files.Count; i++)
			{
				if (this.DatasetFolder.files[i].name.ToLower().Contains(recordName.ToLower()))
				{
					return ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(this.DeCleanXMLForFile(this.DatasetFolder.files[i].data)), this.DataType);
				}
			}
			return null;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00004ABC File Offset: 0x00002CBC
		private void ResetAccessPassword()
		{
			this.AdminEmailResetHasHappened = true;
			try
			{
				Computer computer = Programs.getComputer(this.os, this.adminResetEmailHostID);
				MailServer mailServer = (MailServer)computer.getDaemon(typeof(MailServer));
				string userTo = this.adminResetPassEmailAccount;
				if (this.adminResetPassEmailAccount.Contains("@"))
				{
					userTo = this.adminResetPassEmailAccount.Substring(0, this.adminResetPassEmailAccount.IndexOf("@"));
				}
				string text = string.Concat(new object[]
				{
					Utils.getRandomLetter(),
					Utils.getRandomLetter(),
					Utils.getRandomNumberChar(),
					Utils.getRandomLetter(),
					Utils.getRandomNumberChar()
				});
				this.comp.adminPass = text;
				this.comp.adminIP = this.comp.ip;
				for (int i = 0; i < this.comp.users.Count; i++)
				{
					if (this.comp.users[i].name.ToLower() == "admin")
					{
						UserDetail value = this.comp.users[i];
						value.pass = text;
						this.comp.users[i] = value;
					}
				}
				string text2 = Utils.readEntireFile("Content/DLC/Docs/DatabasePasswordResetEmail.txt");
				text2 = string.Format(text2, this.name, text);
				mailServer.addMail(MailServer.generateEmail(string.Format("{0} Password Reset", this.name), text2, "AdminBot"), userTo);
				this.passwordResetMessage = "Password Reset Complete";
			}
			catch (Exception ex)
			{
				this.passwordResetMessage = "CRITICAL RESET ERROR\n" + ex.ToString();
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00004CC8 File Offset: 0x00002EC8
		private string CleanXMLForFile(string data)
		{
			return data.Replace("<", "[").Replace(">", "]").Replace("`", "_").Replace("\t", "    ");
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004D18 File Offset: 0x00002F18
		private string DeCleanXMLForFile(string data)
		{
			return data.Replace("[", "<").Replace("]", ">");
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004D4C File Offset: 0x00002F4C
		private string GetFilenameForPersonName(string firstname, string lastname)
		{
			return Utils.GetNonRepeatingFilename((lastname + "_" + firstname).ToLower(), ".rec", this.DatasetFolder);
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00004D80 File Offset: 0x00002F80
		private string GetFilenameForObject(object obj)
		{
			return Utils.GetNonRepeatingFilename(obj.ToString().Replace(" ", "_").ToLower(), ".rec", this.DatasetFolder);
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00004DBC File Offset: 0x00002FBC
		public override string getSaveString()
		{
			string text = this.HadThemeColorApplied ? (" Color=\"" + Utils.convertColorToParseableString(this.ThemeColor) + "\"") : "";
			string text2 = "";
			if (!string.IsNullOrWhiteSpace(this.adminResetPassEmailAccount))
			{
				text2 = string.Format(" AdminEmailAccount=\"{0}\" AdminEmailHostID=\"{1}\" ", this.adminResetPassEmailAccount, this.adminResetEmailHostID);
			}
			return string.Concat(new string[]
			{
				"<DatabaseDaemon Name=\"",
				this.name,
				"\" Permissions=\"",
				this.Permissions.ToString(),
				"\" DataType=\"",
				this.DataTypeIdentifier,
				"\" Foldername=\"",
				this.Foldername,
				"\"",
				text,
				text2,
				" />"
			});
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00004EAC File Offset: 0x000030AC
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			Rectangle dest = Utils.InsetRectangle(bounds, 2);
			this.DrawBackground(dest, sb, 6);
			switch (this.State)
			{
			case DatabaseDaemon.DatabaseState.Welcome:
				this.DrawWelcome(dest, sb);
				break;
			case DatabaseDaemon.DatabaseState.Browse:
				this.DrawBrowse(dest, sb);
				break;
			case DatabaseDaemon.DatabaseState.EntryDisplay:
				this.DrawEntry(dest, sb);
				break;
			case DatabaseDaemon.DatabaseState.Error:
				this.DrawError(dest, sb);
				break;
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00004F30 File Offset: 0x00003130
		private void DrawWelcome(Rectangle dest, SpriteBatch sb)
		{
			this.TextRegion.UpdateScroll(0f);
			Rectangle drawRectForRow = this.GetDrawRectForRow(0, 12);
			bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
			TextItem.doCenteredFontLabel(drawRectForRow, this.name, GuiData.font, Color.White, true);
			Rectangle dest2 = new Rectangle(dest.X + 1, (int)this.BlockStartTopLeft.Y, dest.Width - 2, (int)(this.blockSize * 2f));
			PatternDrawer.draw(dest2, 0.5f, Color.Transparent, (flag ? this.ThemeColor : Utils.AddativeRed) * 0.2f, sb, flag ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
			Rectangle drawRectForRow2 = this.GetDrawRectForRow(1, 12);
			TextItem.doCenteredFontLabel(drawRectForRow2, flag ? LocaleTerms.Loc("Access Granted") : LocaleTerms.Loc("Administrator Login Required"), GuiData.font, flag ? (this.ThemeColor * 0.8f) : (Utils.AddativeRed * 0.8f), true);
			Rectangle drawRectForRow3 = this.GetDrawRectForRow(3, 14);
			if (flag && Button.doButton(73616101, (int)this.BlockStartTopLeft.X, drawRectForRow3.Y, dest.Width / 2, drawRectForRow3.Height, LocaleTerms.Loc("Browse Records"), new Color?(this.ThemeColor)))
			{
				this.State = DatabaseDaemon.DatabaseState.Browse;
			}
			drawRectForRow3.Y += drawRectForRow3.Height + 10;
			drawRectForRow3.Height /= 2;
			if (Button.doButton(73616102, (int)this.BlockStartTopLeft.X, drawRectForRow3.Y, dest.Width / 2, drawRectForRow3.Height, LocaleTerms.Loc("Login"), new Color?(this.ThemeColor)))
			{
				this.os.execute("login");
			}
			drawRectForRow3.Y += drawRectForRow3.Height + 6;
			Color color = Color.Lerp(Color.Black, this.ThemeColor, 0.5f);
			if (!string.IsNullOrWhiteSpace(this.adminResetPassEmailAccount) && Button.doButton(73616106, (int)this.BlockStartTopLeft.X, drawRectForRow3.Y, dest.Width / 2, drawRectForRow3.Height, this.AdminEmailResetHasHappened ? LocaleTerms.Loc("Password Reset In Cooldown") : (this.AdminEmailResetStarted ? LocaleTerms.Loc("Cancel") : LocaleTerms.Loc("Reset Access Password")), new Color?(this.AdminEmailResetStarted ? (this.AdminEmailResetHasHappened ? Color.Gray : Color.DarkRed) : color)))
			{
				this.AdminEmailResetStarted = !this.AdminEmailResetStarted;
			}
			if (this.AdminEmailResetStarted)
			{
				Rectangle rectangle = this.GetDrawRectForRow(3, 10);
				rectangle.X += dest.Width / 2 + 10;
				rectangle.Width -= dest.Width / 2;
				rectangle.Height = Math.Max(300, (int)(this.blockSize * 3f - 30f));
				rectangle.Width += 30;
				if (rectangle.Height == 300)
				{
					rectangle.Y -= (int)this.blockSize;
				}
				Color color2 = Color.Gray * 0.3f;
				sb.Draw(Utils.white, rectangle, Color.Black * 0.8f);
				rectangle = Utils.InsetRectangle(rectangle, 2);
				PatternDrawer.draw(rectangle, 0.5f, color2, color2 * 0.4f, sb, PatternDrawer.thinStripe);
				int num = 20;
				Rectangle destinationRectangle = new Rectangle(rectangle.X - num - 2, drawRectForRow3.Y + (drawRectForRow3.Height - num) / 2, num, num);
				sb.Draw(this.Triangle, destinationRectangle, Color.Black * 0.8f);
				Rectangle dest3 = new Rectangle(rectangle.X + 6, rectangle.Y, rectangle.Width - 12, rectangle.Height / 6);
				TextItem.doCenteredFontLabel(dest3, LocaleTerms.Loc("Reset Admin Password"), GuiData.font, Color.LightGray, true);
				Rectangle dest4 = new Rectangle(dest3.X, dest3.Y + dest3.Height + 4, dest3.Width + 9, rectangle.Height / 6 * 4);
				if (this.passwordResetHelperString == null)
				{
					this.passwordResetHelperString = Utils.readEntireFile("Content/DLC/Docs/PasswordResetText.txt");
				}
				string text = this.passwordResetHelperString;
				text = text.Replace("[PASSWORD]", this.adminResetPassEmailAccount);
				if (this.AdminEmailResetHasHappened)
				{
					text = this.passwordResetMessage;
				}
				text = Utils.SuperSmartTwimForWidth(text, dest4.Width, GuiData.UITinyfont);
				TextItem.doFontLabelToSize(dest4, text, GuiData.tinyfont, Color.White, true, true);
				if (!this.AdminEmailResetHasHappened && Button.doButton(73606111, rectangle.X + 4, rectangle.Y + rectangle.Height - 28, rectangle.Width - 8, 26, LocaleTerms.Loc("Reset Password"), new Color?(Color.Red)))
				{
					this.ResetAccessPassword();
				}
			}
			drawRectForRow3.Y += drawRectForRow3.Height + 6;
			if (Button.doButton(73616129, (int)this.BlockStartTopLeft.X, drawRectForRow3.Y, dest.Width / 2, drawRectForRow3.Height, LocaleTerms.Loc("Exit"), new Color?(Color.Black)))
			{
				this.os.display.command = "connect";
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00005568 File Offset: 0x00003768
		private void DrawError(Rectangle dest, SpriteBatch sb)
		{
			Rectangle drawRectForRow = this.GetDrawRectForRow(0, 12);
			bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
			TextItem.doFontLabelToSize(drawRectForRow, this.name, GuiData.font, Color.White, false, true);
			Rectangle dest2 = new Rectangle(dest.X + 1, (int)this.BlockStartTopLeft.Y, dest.Width - 2, (int)(this.blockSize * 2f));
			PatternDrawer.draw(dest2, 0.5f, Color.Transparent, Utils.AddativeRed * 0.2f, sb, flag ? PatternDrawer.thinStripe : PatternDrawer.errorTile);
			Rectangle drawRectForRow2 = this.GetDrawRectForRow(1, 12);
			TextItem.doFontLabelToSize(drawRectForRow2, LocaleTerms.Loc("ERROR"), GuiData.font, Utils.AddativeRed * 0.8f, true, true);
			Rectangle drawRectForRow3 = this.GetDrawRectForRow(2, 14);
			if (Button.doButton(73616101, (int)this.BlockStartTopLeft.X, drawRectForRow3.Y, dest.Width / 2, drawRectForRow3.Height, LocaleTerms.Loc("Back"), new Color?(this.ThemeColor)))
			{
				this.State = DatabaseDaemon.DatabaseState.Welcome;
			}
			Rectangle rectangle = this.GetDrawRectForRow(3, 14);
			rectangle.Height = (int)(((float)this.blocksHigh - 4f) * this.blockSize);
			rectangle = Utils.InsetRectangle(rectangle, 2);
			string text = Utils.SuperSmartTwimForWidth(this.errorMessage, rectangle.Width, GuiData.smallfont);
			this.TextRegion.Draw(rectangle, text, sb, Utils.AddativeRed);
		}

		// Token: 0x0600003D RID: 61 RVA: 0x000058E0 File Offset: 0x00003AE0
		private void DrawBrowse(Rectangle dest, SpriteBatch spriteBatch)
		{
			Rectangle drawRectForRow = this.GetDrawRectForRow(0, 12);
			bool flag = this.Permissions == DatabaseDaemon.DatabasePermissions.Public || this.comp.adminIP == this.os.thisComputer.ip;
			TextItem.doFontLabelToSize(drawRectForRow, string.Format(LocaleTerms.Loc("{0} : Records"), this.name), GuiData.font, Color.White, true, true);
			drawRectForRow = this.GetDrawRectForRow(0, 0);
			PatternDrawer.draw(drawRectForRow, 0.5f, Color.Transparent, this.ThemeColor * 0.2f, spriteBatch, PatternDrawer.thinStripe);
			Rectangle rectangle = new Rectangle(drawRectForRow.X + 4, drawRectForRow.Y + drawRectForRow.Height / 2 + 12, drawRectForRow.Width / 3, drawRectForRow.Height / 2 - 15);
			if (Button.doButton(71118000, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
			{
				this.State = DatabaseDaemon.DatabaseState.Welcome;
			}
			if (this.DataType == null || string.IsNullOrWhiteSpace(this.DataTypeIdentifier) || this.DataTypeIdentifier.ToLower() == "none")
			{
				Rectangle drawRectForRow2 = this.GetDrawRectForRow(1, 2);
				drawRectForRow2.Height += (int)(this.blockSize * (float)(this.blocksHigh - 3));
				PatternDrawer.draw(drawRectForRow2, 0.4f, Color.Black * 0.3f, Utils.makeColorAddative(this.ThemeColor) * 0.25f, spriteBatch, PatternDrawer.binaryTile);
				Rectangle drawRectForRow3 = this.GetDrawRectForRow(3, 8);
				TextItem.doCenteredFontLabel(drawRectForRow3, " - " + LocaleTerms.Loc("API Access Enabled") + " - ", GuiData.smallfont, Color.White, false);
			}
			else
			{
				DatabaseDaemon.<>c__DisplayClass1 CS$<>8__locals1 = new DatabaseDaemon.<>c__DisplayClass1();
				CS$<>8__locals1.<>4__this = this;
				CS$<>8__locals1.ListDest = this.GetDrawRectForRow(1, 4);
				DatabaseDaemon.<>c__DisplayClass1 CS$<>8__locals2 = CS$<>8__locals1;
				CS$<>8__locals2.ListDest.Height = CS$<>8__locals2.ListDest.Height + (int)(this.blockSize * ((float)this.blocksHigh - 2.5f));
				this.ScrollPanel.NumberOfPanels = this.DatasetFolder.files.Count;
				this.ScrollPanel.Draw(delegate(int i, Rectangle bounds, SpriteBatch sb)
				{
					FileEntry fileEntry = CS$<>8__locals1.<>4__this.DatasetFolder.files[i];
					string text = "REC#" + i.ToString("000") + " : " + CS$<>8__locals1.<>4__this.GetAnnounceNameForFileName(fileEntry.name);
					int num = Math.Max(160, bounds.Width / 3);
					if (Button.doButton(71118100 + i, bounds.X, bounds.Y + 2, num, bounds.Height - 4, string.Format(LocaleTerms.Loc("View Record #{0}"), i.ToString("000")), new Color?(CS$<>8__locals1.<>4__this.ThemeColor)))
					{
						CS$<>8__locals1.<>4__this.ActiveFile = CS$<>8__locals1.<>4__this.DatasetFolder.files[i];
						CS$<>8__locals1.<>4__this.State = DatabaseDaemon.DatabaseState.EntryDisplay;
						CS$<>8__locals1.<>4__this.DeserializedFile = null;
					}
					Vector2 vector = GuiData.smallfont.MeasureString(text);
					Rectangle destinationRectangle = new Rectangle(bounds.X + bounds.Width / 3 + 6, bounds.Y + bounds.Height / 2 - 1, CS$<>8__locals1.ListDest.Width - (num + (int)vector.X + 26), 1);
					sb.Draw(Utils.white, destinationRectangle, Color.White * 0.1f);
					TextItem.doFontLabel(new Vector2((float)(destinationRectangle.X + destinationRectangle.Width) + 6f, (float)bounds.Y + 2f), text, GuiData.smallfont, new Color?(Color.White), (float)bounds.Width, (float)bounds.Height, false);
				}, spriteBatch, CS$<>8__locals1.ListDest);
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00005B8C File Offset: 0x00003D8C
		private void DrawEntry(Rectangle dest, SpriteBatch spriteBatch)
		{
			if (this.DeserializedFile == null)
			{
				try
				{
					this.DeserializedFile = ObjectSerializer.DeserializeObject(Utils.GenerateStreamFromString(this.DeCleanXMLForFile(this.ActiveFile.data)), this.DataType);
				}
				catch (Exception ex)
				{
					this.State = DatabaseDaemon.DatabaseState.Error;
					this.errorMessage = string.Concat(new string[]
					{
						LocaleTerms.Loc("Error De-serializing Entry"),
						" : ",
						this.ActiveFile.name,
						"\n",
						Utils.GenerateReportFromException(ex).Replace("Hacknet", "Database")
					});
					return;
				}
			}
			Rectangle drawRectForRow = this.GetDrawRectForRow(0, 0);
			drawRectForRow.Height /= 2;
			TextItem.doFontLabelToSize(drawRectForRow, string.Format(LocaleTerms.Loc("Entry for {0}"), this.GetAnnounceNameForFileName(this.ActiveFile.name)), GuiData.font, Color.White, true, true);
			drawRectForRow.Y += drawRectForRow.Height;
			if (Button.doButton(7301991, drawRectForRow.X, drawRectForRow.Y, drawRectForRow.Width / 2, drawRectForRow.Height, LocaleTerms.Loc("Back"), new Color?(Color.Gray)))
			{
				this.State = DatabaseDaemon.DatabaseState.Browse;
				this.ActiveFile = null;
				this.DeserializedFile = null;
			}
			else
			{
				Rectangle bounds = new Rectangle((int)this.BlockStartTopLeft.X, (int)this.BlockStartTopLeft.Y + (int)this.blockSize, (int)(this.blockSize * (float)(this.blocksWide - 1)), (int)(this.blockSize * (float)(this.blocksHigh - 2)));
				if (this.HasSpecialCaseDraw)
				{
					ReflectiveRenderer.PreRenderForObject = (Action<Vector2, Type, string>)Delegate.Combine(ReflectiveRenderer.PreRenderForObject, new Action<Vector2, Type, string>(delegate(Vector2 pos, Type targetType, string targetValue)
					{
						this.DrawSpecialCase(pos, bounds, targetType, targetValue, spriteBatch);
					}));
				}
				ReflectiveRenderer.RenderObject(this.DeserializedFile, bounds, spriteBatch, this.ScrollPanel, this.ThemeColor);
				if (this.HasSpecialCaseDraw)
				{
					ReflectiveRenderer.PreRenderForObject = null;
				}
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00005DF4 File Offset: 0x00003FF4
		private string GetAnnounceNameForFileName(string filename)
		{
			filename = filename.Replace(".rec", "");
			string result;
			if (this.FilenameIsPersonName)
			{
				string[] array = filename.Split(new char[]
				{
					'_'
				}, StringSplitOptions.None);
				result = array[1] + " " + array[0];
			}
			else
			{
				result = filename;
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00005E50 File Offset: 0x00004050
		private Rectangle GetDrawRectForRow(int row, int inset)
		{
			Rectangle rectangle = new Rectangle((int)this.BlockStartTopLeft.X, (int)(this.BlockStartTopLeft.Y + this.blockSize * (float)row), (int)(this.blockSize * (float)(this.blocksWide - 1)), (int)this.blockSize);
			rectangle = Utils.InsetRectangle(rectangle, inset);
			return rectangle;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00005EB0 File Offset: 0x000040B0
		private void DrawBackground(Rectangle dest, SpriteBatch sb, int desiredNumOfBlocks)
		{
			Color color = Color.Gray * 0.4f;
			int num = dest.Width / (desiredNumOfBlocks + 2);
			int num2 = 9;
			float num3 = 1f;
			int num4 = dest.X + num / 2;
			int num5 = dest.Y + num / 2;
			this.BlockStartTopLeft = new Vector2((float)num4, (float)num5);
			this.blocksWide = (this.blocksHigh = 0);
			this.blockSize = (float)num;
			do
			{
				this.blocksWide = 0;
				do
				{
					Rectangle destinationRectangle = new Rectangle(Math.Max(dest.X, num4 - num2 / 2), num5 - (int)(num3 / 2f + 0.5f), num2, (int)num3);
					sb.Draw(Utils.white, destinationRectangle, color);
					Rectangle destinationRectangle2 = new Rectangle(num4 - (int)(num3 / 2f), Math.Max(dest.Y, num5 - (int)((float)num2 / 2f + 0.5f)), (int)num3, num2 / 2 - (int)(num3 / 2f));
					sb.Draw(Utils.white, destinationRectangle2, color);
					Rectangle destinationRectangle3 = new Rectangle(num4 - (int)(num3 / 2f), num5 + (int)(num3 / 2f), (int)num3, num2 / 2 - (int)(num3 / 2f));
					sb.Draw(Utils.white, destinationRectangle3, color);
					num4 += num;
					this.blocksWide++;
				}
				while (num4 <= dest.X + dest.Width - num / 2);
				num4 = dest.X + num / 2;
				num5 += num;
				this.blocksHigh++;
			}
			while (num5 <= dest.Y + dest.Height - num / 2);
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000060B0 File Offset: 0x000042B0
		public void DrawSpecialCase(Vector2 currentPos, Rectangle totalArea, Type objType, string drawnValue, SpriteBatch sb)
		{
			if (sb.Name != "AltRTBatch")
			{
				if (objType == typeof(Neopal.PetType))
				{
					int num = Math.Min(ReflectiveRenderer.GetEntryLineHeight() * 6, totalArea.Width / 4);
					Texture2D sprite = this.PlaceholderSprite;
					Neopal.PetType petType = (Neopal.PetType)Enum.Parse(objType, drawnValue);
					if (this.WildcardAssets.ContainsKey(petType))
					{
						sprite = this.WildcardAssets[petType];
					}
					Rectangle petDrawPos = new Rectangle(totalArea.X + totalArea.Width - num - 10, (int)currentPos.Y + 4, num, num);
					double num2 = (double)sprite.Height / (double)petDrawPos.Height;
					int num3 = totalArea.Y + totalArea.Height - petDrawPos.Y;
					int height = Math.Min(sprite.Height, (int)(num2 * (double)num3));
					Rectangle? clip = new Rectangle?(new Rectangle(0, 0, sprite.Width, height));
					petDrawPos.Height = Math.Min(petDrawPos.Height, num3);
					OS os = this.os;
					os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
					{
						sb.Draw(sprite, petDrawPos, clip, Color.White);
					}));
				}
			}
		}

		// Token: 0x04000027 RID: 39
		private MovingBarsEffect sideEffect;

		// Token: 0x04000028 RID: 40
		private DatabaseDaemon.DatabasePermissions Permissions;

		// Token: 0x04000029 RID: 41
		private DatabaseDaemon.DatabaseState State;

		// Token: 0x0400002A RID: 42
		private Folder DatasetFolder;

		// Token: 0x0400002B RID: 43
		private Type DataType;

		// Token: 0x0400002C RID: 44
		private string DataTypeIdentifier;

		// Token: 0x0400002D RID: 45
		private bool FilenameIsPersonName = false;

		// Token: 0x0400002E RID: 46
		private FileEntry ActiveFile = null;

		// Token: 0x0400002F RID: 47
		private string errorMessage = "None";

		// Token: 0x04000030 RID: 48
		private object DeserializedFile = null;

		// Token: 0x04000031 RID: 49
		private string Foldername;

		// Token: 0x04000032 RID: 50
		public List<object> Dataset = null;

		// Token: 0x04000033 RID: 51
		private Color ThemeColor;

		// Token: 0x04000034 RID: 52
		private bool HadThemeColorApplied = true;

		// Token: 0x04000035 RID: 53
		private Vector2 BlockStartTopLeft;

		// Token: 0x04000036 RID: 54
		private float blockSize;

		// Token: 0x04000037 RID: 55
		private int blocksWide;

		// Token: 0x04000038 RID: 56
		private int blocksHigh;

		// Token: 0x04000039 RID: 57
		private bool HasSpecialCaseDraw = false;

		// Token: 0x0400003A RID: 58
		private Texture2D PlaceholderSprite;

		// Token: 0x0400003B RID: 59
		private Texture2D Triangle;

		// Token: 0x0400003C RID: 60
		private Dictionary<object, Texture2D> WildcardAssets = new Dictionary<object, Texture2D>();

		// Token: 0x0400003D RID: 61
		private ScrollableSectionedPanel ScrollPanel;

		// Token: 0x0400003E RID: 62
		private ScrollableTextRegion TextRegion;

		// Token: 0x0400003F RID: 63
		private string passwordResetHelperString = null;

		// Token: 0x04000040 RID: 64
		public string adminResetEmailHostID = null;

		// Token: 0x04000041 RID: 65
		public string adminResetPassEmailAccount = null;

		// Token: 0x04000042 RID: 66
		private bool AdminEmailResetStarted = false;

		// Token: 0x04000043 RID: 67
		private bool AdminEmailResetHasHappened = false;

		// Token: 0x04000044 RID: 68
		private string passwordResetMessage = "";

		// Token: 0x02000008 RID: 8
		public enum DatabasePermissions
		{
			// Token: 0x04000046 RID: 70
			AdminOnly,
			// Token: 0x04000047 RID: 71
			Public
		}

		// Token: 0x02000009 RID: 9
		private enum DatabaseState
		{
			// Token: 0x04000049 RID: 73
			Welcome,
			// Token: 0x0400004A RID: 74
			Search,
			// Token: 0x0400004B RID: 75
			Browse,
			// Token: 0x0400004C RID: 76
			Loading,
			// Token: 0x0400004D RID: 77
			EntryDisplay,
			// Token: 0x0400004E RID: 78
			Error
		}
	}
}
