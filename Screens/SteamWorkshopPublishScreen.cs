using System;
using System.IO;
using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;

namespace Hacknet.Screens
{
	// Token: 0x020000F3 RID: 243
	public class SteamWorkshopPublishScreen
	{
		// Token: 0x0600052F RID: 1327 RVA: 0x00051464 File Offset: 0x0004F664
		public SteamWorkshopPublishScreen(ContentManager content)
		{
			this.spinnerTex = content.Load<Texture2D>("Sprites/Spinner");
		}

		// Token: 0x06000530 RID: 1328 RVA: 0x000514DC File Offset: 0x0004F6DC
		private void SubmitUpdateResult(SubmitItemUpdateResult_t callback, bool bIOFailure)
		{
			Console.WriteLine("Upload complete!");
			if (callback.m_eResult == EResult.k_EResultOK)
			{
				this.currentTitleMessage = "Update Complete!";
				this.currentBodyMessage = "Upload completed successfully";
			}
			else
			{
				this.currentTitleMessage = "Update Failed";
				this.currentBodyMessage = "Error code: " + callback.m_eResult;
			}
			this.isInUpload = false;
			this.showLoadingSpinner = false;
		}

		// Token: 0x06000531 RID: 1329 RVA: 0x00051558 File Offset: 0x0004F758
		private void CreateItemResult(CreateItemResult_t callback, bool bIOFailure)
		{
			EResult eResult = callback.m_eResult;
			if (eResult != EResult.k_EResultTimeout)
			{
				if (eResult != EResult.k_EResultNotLoggedOn)
				{
					if (eResult != EResult.k_EResultInsufficientPrivilege)
					{
						ulong publishedFileId = callback.m_nPublishedFileId.m_PublishedFileId;
						this.showLoadingSpinner = false;
						this.currentTitleMessage = "Extension successfully created!";
						this.currentBodyMessage = "Extension Publish ID: " + callback.m_nPublishedFileId;
						this.ActiveInfo.WorkshopPublishID = string.Concat(callback.m_nPublishedFileId.m_PublishedFileId);
						if (callback.m_bUserNeedsToAcceptWorkshopLegalAgreement)
						{
							SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/" + callback.m_nPublishedFileId);
						}
						string text = this.ActiveInfo.FolderPath + "/ExtensionInfo.xml";
						string text2 = Utils.readEntireFile(text);
						text2 = text2.Replace("<WorkshopPublishID>NONE</WorkshopPublishID>", "<WorkshopPublishID>" + publishedFileId + "</WorkshopPublishID>");
						File.WriteAllText(text, text2);
					}
					else
					{
						this.currentTitleMessage = "Error: Insufficient Privilege";
						this.currentBodyMessage = "The user creating the item is currently banned in the community";
					}
				}
				else
				{
					this.currentTitleMessage = "Error: Not logged on";
					this.currentBodyMessage = "The user creating the item is currently banned in the community";
				}
			}
			else
			{
				this.currentTitleMessage = "Error: Timeout";
				this.currentBodyMessage = "Current user is not currently logged into steam";
			}
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x000516A8 File Offset: 0x0004F8A8
		private void PerformUpdate(ExtensionInfo info)
		{
			this.updateHandle = SteamUGC.StartItemUpdate(this.hacknetAppID, new PublishedFileId_t((ulong)Convert.ToInt64(info.WorkshopPublishID)));
			this.isInUpload = true;
			bool flag = true;
			flag &= SteamUGC.SetItemTitle(this.updateHandle, info.Name);
			flag &= SteamUGC.SetItemDescription(this.updateHandle, info.WorkshopDescription);
			flag &= SteamUGC.SetItemTags(this.updateHandle, info.WorkshopTags.Split(new string[]
			{
				" ,",
				", ",
				","
			}, StringSplitOptions.RemoveEmptyEntries));
			string text = Path.Combine(Directory.GetCurrentDirectory(), info.FolderPath) + "/" + info.WorkshopPreviewImagePath;
			text = text.Replace("\\", "/");
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (fileInfo.Length < 1000000L)
				{
					flag &= SteamUGC.SetItemPreview(this.updateHandle, text);
				}
				else
				{
					this.currentStatusMessage = "Workshop Preview Image too large! Max size 1mb. Ignoring...";
				}
			}
			ERemoteStoragePublishedFileVisibility eVisibility = (info.WorkshopVisibility <= 0) ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic : ((info.WorkshopVisibility == 1) ? ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly : ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate);
			flag &= SteamUGC.SetItemVisibility(this.updateHandle, eVisibility);
			string text2 = Path.Combine(Directory.GetCurrentDirectory(), info.FolderPath);
			text2 = text2.Replace("\\", "/");
			Console.WriteLine("Content Path: " + text2);
			if (!(flag & SteamUGC.SetItemContent(this.updateHandle, text2)))
			{
				Console.WriteLine("Error!");
			}
			string path = info.FolderPath + "/Changelog.txt";
			string pchChangeNote = "";
			if (File.Exists(path))
			{
				pchChangeNote = File.ReadAllText(path);
			}
			SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(this.updateHandle, pchChangeNote);
			this.SubmitItemUpdateResult_t.Set(hAPICall, null);
			this.transferStarted = DateTime.Now;
			this.showLoadingSpinner = true;
			this.isInUpload = true;
			this.currentBodyMessage = "Uploading to Steam Workshop...";
			this.currentTitleMessage = "Upload in progress";
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x000518CF File Offset: 0x0004FACF
		private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
		{
			this.currentBodyMessage = this.currentBodyMessage + "\nPlayer callback: " + pCallback.m_cPlayers;
		}

		// Token: 0x06000534 RID: 1332 RVA: 0x000518F4 File Offset: 0x0004FAF4
		private void InitSteamCallbacks()
		{
			this.m_CreateItemResult = CallResult<CreateItemResult_t>.Create(new CallResult<CreateItemResult_t>.APIDispatchDelegate(this.CreateItemResult));
			this.m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(new CallResult<NumberOfCurrentPlayers_t>.APIDispatchDelegate(this.OnNumberOfCurrentPlayers));
			this.SubmitItemUpdateResult_t = CallResult<Steamworks.SubmitItemUpdateResult_t>.Create(new CallResult<SubmitItemUpdateResult_t>.APIDispatchDelegate(this.SubmitUpdateResult));
			this.HasInitializedSteamCallbacks = true;
		}

		// Token: 0x06000535 RID: 1333 RVA: 0x0005194E File Offset: 0x0004FB4E
		public void Update()
		{
			SteamAPI.RunCallbacks();
		}

		// Token: 0x06000536 RID: 1334 RVA: 0x00051958 File Offset: 0x0004FB58
		public Vector2 Draw(SpriteBatch sb, Rectangle dest, ExtensionInfo info)
		{
			this.ActiveInfo = info;
			if (!this.HasInitializedSteamCallbacks)
			{
				this.InitSteamCallbacks();
			}
			this.Update();
			Vector2 result = new Vector2((float)dest.X, (float)dest.Y);
			bool flag = info.WorkshopPublishID != "NONE";
			this.currentStatusMessage = (flag ? "Ready to push Updates" : "Ready to create in steam");
			if (!flag && string.IsNullOrWhiteSpace(this.currentBodyMessage))
			{
				this.currentBodyMessage = "By submitting this item, you agree to the workshop terms of service\nhttp://steamcommunity.com/sharedfiles/workshoplegalagreement";
			}
			Vector2 vector = new Vector2(result.X + (float)(dest.Width / 2), result.Y);
			TextItem.doFontLabel(vector, this.currentStatusMessage, GuiData.font, new Color?(Color.Gray), (float)dest.Width / 2f, 30f, false);
			vector.Y += 30f;
			TextItem.doFontLabel(vector, this.currentTitleMessage, GuiData.font, new Color?(Color.White), (float)dest.Width / 2f, 30f, false);
			vector.Y += 30f;
			Vector2 pos = vector;
			if (this.showLoadingSpinner)
			{
				vector.X += 16f;
				this.spinnerRot += 0.1f;
				Rectangle destinationRectangle = new Rectangle((int)vector.X, (int)vector.Y + 20, 40, 40);
				sb.Draw(this.spinnerTex, destinationRectangle, null, Color.White, this.spinnerRot, this.spinnerTex.GetCentreOrigin(), SpriteEffects.None, 0.5f);
				pos.X += 45f;
			}
			if (this.isInUpload)
			{
				Rectangle rectangle = new Rectangle((int)pos.X, (int)pos.Y + 6, dest.Width / 2, 20);
				ulong num = 0UL;
				ulong num2 = 1UL;
				SteamUGC.GetItemUpdateProgress(this.updateHandle, out num, out num2);
				double num3 = num / num2;
				if (num2 == 0UL)
				{
					num3 = 0.0;
				}
				sb.Draw(Utils.white, Utils.InsetRectangle(rectangle, -1), Utils.AddativeWhite * 0.7f);
				sb.Draw(Utils.white, rectangle, Utils.VeryDarkGray);
				rectangle.Width = (int)((double)rectangle.Width * num3);
				sb.Draw(Utils.white, rectangle, Color.LightBlue);
				pos.Y += 31f;
				if (num2 > 0UL)
				{
					string text = string.Format("{0}% - {1}mb of {2}mb Transfered", (num3 * 100.0).ToString("00.00"), (num / 1000000UL).ToString("0.00"), (num2 / 1000000UL).ToString("0.00"));
					Utils.DrawStringMonospace(sb, text, GuiData.smallfont, pos, Color.White, 9f);
					pos.Y += 20f;
					TimeSpan time = DateTime.Now - this.transferStarted;
					TimeSpan time2 = TimeSpan.FromSeconds(time.TotalSeconds / Math.Max(num3, 0.01));
					text = string.Format("ETA: {0} // Elapsed : {1}", this.getTimespanDisplayString(time2), this.getTimespanDisplayString(time));
					Utils.DrawStringMonospace(sb, text, GuiData.smallfont, pos, Color.White, 9f);
					pos.Y += 25f;
				}
			}
			TextItem.doFontLabel(pos, this.currentBodyMessage, GuiData.smallfont, new Color?(Color.White * 0.8f), (float)dest.Width / 2f, 30f, false);
			int num4 = 40;
			int width = 450;
			bool flag2 = !this.showLoadingSpinner;
			if (!this.isInUpload)
			{
				if (Button.doButton(371711001, (int)result.X, (int)result.Y, width, num4, flag ? " - Item Created -" : "Create Entry in Steam Workshop", new Color?(flag ? Color.LightBlue : Color.Gray)) && !flag && flag2)
				{
					this.CreateExtensionInSteam(info);
				}
				result.Y += (float)(num4 + 4);
				if (Button.doButton(371711003, (int)result.X, (int)result.Y, width, num4, "Upload to Steam Workshop", new Color?(flag ? Color.LightBlue : Color.Gray)) && flag2)
				{
					this.PerformUpdate(info);
				}
				result.Y += (float)(num4 + 4);
				if (Button.doButton(371711005, (int)result.X, (int)result.Y + 10, width, num4 - 10, "Back to Extension Menu", new Color?(Color.Black)) && flag2)
				{
					if (this.GoBack != null)
					{
						this.GoBack();
					}
				}
				result.Y += (float)(num4 + 4);
			}
			return result;
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00051EE4 File Offset: 0x000500E4
		private string getTimespanDisplayString(TimeSpan time)
		{
			double num = time.TotalSeconds / 60.0;
			num -= num % 1.0;
			double num2 = time.TotalSeconds % 60.0;
			string str = (num >= 1.0) ? (num.ToString("0") + "m ") : "";
			return str + num2.ToString("00") + "s";
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00051F6C File Offset: 0x0005016C
		private void CreateExtensionInSteam(ExtensionInfo info)
		{
			SteamAPICall_t hAPICall = SteamUGC.CreateItem(this.hacknetAppID, EWorkshopFileType.k_EWorkshopFileTypeFirst);
			this.m_CreateItemResult.Set(hAPICall, null);
			this.showLoadingSpinner = true;
			this.currentTitleMessage = "Creating Extension with Steam...";
		}

		// Token: 0x040005D4 RID: 1492
		public Action GoBack;

		// Token: 0x040005D5 RID: 1493
		private bool isInUpload = false;

		// Token: 0x040005D6 RID: 1494
		private bool showLoadingSpinner = false;

		// Token: 0x040005D7 RID: 1495
		public bool HasInitializedSteamCallbacks = false;

		// Token: 0x040005D8 RID: 1496
		private string currentStatusMessage = "";

		// Token: 0x040005D9 RID: 1497
		private string currentTitleMessage = "";

		// Token: 0x040005DA RID: 1498
		private string currentBodyMessage = "";

		// Token: 0x040005DB RID: 1499
		private Texture2D spinnerTex;

		// Token: 0x040005DC RID: 1500
		private float spinnerRot = 0f;

		// Token: 0x040005DD RID: 1501
		private ExtensionInfo ActiveInfo;

		// Token: 0x040005DE RID: 1502
		protected CallResult<CreateItemResult_t> m_CreateItemResult;

		// Token: 0x040005DF RID: 1503
		protected CallResult<SubmitItemUpdateResult_t> SubmitItemUpdateResult_t;

		// Token: 0x040005E0 RID: 1504
		protected CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

		// Token: 0x040005E1 RID: 1505
		private AppId_t hacknetAppID = new AppId_t(365450U);

		// Token: 0x040005E2 RID: 1506
		private UGCUpdateHandle_t updateHandle;

		// Token: 0x040005E3 RID: 1507
		private DateTime transferStarted;
	}
}
