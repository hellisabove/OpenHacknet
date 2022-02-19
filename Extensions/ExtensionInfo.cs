using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet.Extensions
{
	// Token: 0x02000072 RID: 114
	public class ExtensionInfo
	{
		// Token: 0x06000238 RID: 568 RVA: 0x0001FF34 File Offset: 0x0001E134
		public string GetFullFolderPath()
		{
			return Path.Combine(Directory.GetCurrentDirectory(), this.FolderPath).Replace("\\", "/");
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0001FF74 File Offset: 0x0001E174
		public static ExtensionInfo ReadExtensionInfo(string folderpath)
		{
			if (ExtensionInfo.ExtensionExists(folderpath))
			{
				ExtensionInfo extensionInfo = new ExtensionInfo();
				extensionInfo.FolderPath = folderpath;
				extensionInfo.Language = "en-us";
				using (FileStream fileStream = File.OpenRead(folderpath + "/ExtensionInfo.xml"))
				{
					XmlReader xmlReader = XmlReader.Create(fileStream);
					while (!xmlReader.EOF)
					{
						if (xmlReader.Name == "Name")
						{
							xmlReader.MoveToContent();
							extensionInfo.Name = Utils.CleanStringToLanguageRenderable(xmlReader.ReadElementContentAsString());
						}
						if (xmlReader.Name == "Language")
						{
							xmlReader.MoveToContent();
							extensionInfo.Language = xmlReader.ReadElementContentAsString();
						}
						if (xmlReader.Name == "AllowSaves")
						{
							xmlReader.MoveToContent();
							extensionInfo.AllowSave = xmlReader.ReadElementContentAsBoolean();
						}
						if (xmlReader.Name == "StartingVisibleNodes")
						{
							xmlReader.MoveToContent();
							string text = xmlReader.ReadElementContentAsString();
							extensionInfo.StartingVisibleNodes = text.Split(new char[]
							{
								',',
								' ',
								'\t',
								'\n',
								'\r',
								'/'
							}, StringSplitOptions.RemoveEmptyEntries);
						}
						if (xmlReader.Name == "StartingMission")
						{
							xmlReader.MoveToContent();
							extensionInfo.StartingMissionPath = xmlReader.ReadElementContentAsString();
							if (extensionInfo.StartingMissionPath == "NONE")
							{
								extensionInfo.StartingMissionPath = null;
							}
						}
						if (xmlReader.Name == "StartingActions")
						{
							xmlReader.MoveToContent();
							extensionInfo.StartingActionsPath = xmlReader.ReadElementContentAsString();
							if (extensionInfo.StartingActionsPath == "NONE")
							{
								extensionInfo.StartingActionsPath = null;
							}
						}
						if (xmlReader.Name == "Description")
						{
							xmlReader.MoveToContent();
							extensionInfo.Description = Utils.CleanFilterStringToRenderable(xmlReader.ReadElementContentAsString());
						}
						if (xmlReader.Name == "Faction")
						{
							xmlReader.MoveToContent();
							extensionInfo.FactionDescriptorPaths.Add(xmlReader.ReadElementContentAsString());
						}
						if (xmlReader.Name == "StartsWithTutorial")
						{
							xmlReader.MoveToContent();
							extensionInfo.StartsWithTutorial = (xmlReader.ReadElementContentAsString().ToLower() == "true");
						}
						if (xmlReader.Name == "HasIntroStartup")
						{
							xmlReader.MoveToContent();
							extensionInfo.HasIntroStartup = (xmlReader.ReadElementContentAsString().ToLower() == "true");
						}
						if (xmlReader.Name == "StartingTheme")
						{
							xmlReader.MoveToContent();
							string theme = xmlReader.ReadElementContentAsString().ToLower();
							extensionInfo.Theme = theme;
						}
						if (xmlReader.Name == "IntroStartupSong")
						{
							xmlReader.MoveToContent();
							string introStartupSong = xmlReader.ReadElementContentAsString();
							extensionInfo.IntroStartupSong = introStartupSong;
						}
						if (xmlReader.Name == "IntroStartupSongDelay")
						{
							xmlReader.MoveToContent();
							float introStartupSongDelay = xmlReader.ReadElementContentAsFloat();
							extensionInfo.IntroStartupSongDelay = introStartupSongDelay;
						}
						if (xmlReader.Name == "SequencerSpinUpTime")
						{
							xmlReader.MoveToContent();
							float sequencerSpinUpTime = xmlReader.ReadElementContentAsFloat();
							extensionInfo.SequencerSpinUpTime = sequencerSpinUpTime;
						}
						if (xmlReader.Name == "ActionsToRunOnSequencerStart")
						{
							xmlReader.MoveToContent();
							string actionsToRunOnSequencerStart = xmlReader.ReadElementContentAsString();
							extensionInfo.ActionsToRunOnSequencerStart = actionsToRunOnSequencerStart;
						}
						if (xmlReader.Name == "SequencerFlagRequiredForStart")
						{
							xmlReader.MoveToContent();
							string sequencerFlagRequiredForStart = xmlReader.ReadElementContentAsString();
							extensionInfo.SequencerFlagRequiredForStart = sequencerFlagRequiredForStart;
						}
						if (xmlReader.Name == "SequencerTargetID")
						{
							xmlReader.MoveToContent();
							string sequencerTargetID = xmlReader.ReadElementContentAsString();
							extensionInfo.SequencerTargetID = sequencerTargetID;
						}
						if (xmlReader.Name == "WorkshopDescription")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopDescription = xmlReader.ReadElementContentAsString();
						}
						if (xmlReader.Name == "WorkshopVisibility")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopVisibility = (byte)xmlReader.ReadElementContentAsInt();
						}
						if (xmlReader.Name == "WorkshopTags")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopTags = xmlReader.ReadElementContentAsString();
						}
						if (xmlReader.Name == "WorkshopPreviewImagePath")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopPreviewImagePath = xmlReader.ReadElementContentAsString();
						}
						if (xmlReader.Name == "WorkshopLanguage")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopLanguage = xmlReader.ReadElementContentAsString();
						}
						if (xmlReader.Name == "WorkshopPublishID")
						{
							xmlReader.MoveToContent();
							extensionInfo.WorkshopPublishID = xmlReader.ReadElementContentAsString();
						}
						xmlReader.Read();
					}
				}
				string text2 = folderpath + "/Logo";
				bool flag = false;
				if (File.Exists(text2 + ".png"))
				{
					text2 += ".png";
					flag = true;
				}
				else if (File.Exists(text2 + ".jpg"))
				{
					text2 += ".png";
					flag = true;
				}
				if (flag)
				{
					using (FileStream fileStream = File.OpenRead(text2))
					{
						extensionInfo.LogoImage = Texture2D.FromStream(Game1.getSingleton().GraphicsDevice, fileStream);
					}
				}
				return extensionInfo;
			}
			throw new FileNotFoundException("No extension info exists for folder " + folderpath);
		}

		// Token: 0x0600023A RID: 570 RVA: 0x000205C4 File Offset: 0x0001E7C4
		public static void VerifyExtensionInfo(ExtensionInfo info)
		{
			if (info.Name == null)
			{
				throw new NullReferenceException("Extension must have a Name");
			}
			if (string.IsNullOrWhiteSpace(info.GetFoldersafeName()))
			{
				throw new NullReferenceException("Extension names require at least one letter or number in them");
			}
			if (!Directory.Exists(info.FolderPath))
			{
				throw new DirectoryNotFoundException("Directory folderpath could not be found");
			}
			if (!File.Exists(info.FolderPath + "/" + info.StartingMissionPath))
			{
				throw new FileNotFoundException("Starting Mission File does not exist!");
			}
			if (info.IntroStartupSong != null)
			{
				string introStartupSong = info.IntroStartupSong;
				string text = introStartupSong;
				if (!introStartupSong.EndsWith(".ogg"))
				{
					text = introStartupSong + ".ogg";
				}
				string text2 = info.FolderPath + "/" + text;
				if (File.Exists(text2))
				{
					MusicManager.loadAsCurrentSongUnsafe((info.FolderPath.StartsWith("Extensions") ? "../" : "") + info.FolderPath + "/" + introStartupSong);
				}
				else
				{
					text2 = "Music/" + text;
					if (File.Exists("Content/" + text2))
					{
						MusicManager.loadAsCurrentSong(text2.Replace(".ogg", ""));
					}
				}
			}
		}

		// Token: 0x0600023B RID: 571 RVA: 0x00020718 File Offset: 0x0001E918
		public static bool ExtensionExists(string folderpath)
		{
			return File.Exists(folderpath + "/ExtensionInfo.xml");
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0002073C File Offset: 0x0001E93C
		public string GetFoldersafeName()
		{
			string text = this.Name;
			text = text.Replace(" ", "_");
			foreach (char c in Path.GetInvalidFileNameChars())
			{
				text = text.Replace(string.Concat(c), "");
			}
			return text;
		}

		// Token: 0x040002A1 RID: 673
		public const string INFO_FILENAME = "ExtensionInfo.xml";

		// Token: 0x040002A2 RID: 674
		public const string LOGO_FILENAME = "Logo";

		// Token: 0x040002A3 RID: 675
		public const string NODES_FOLDER = "/Nodes";

		// Token: 0x040002A4 RID: 676
		public const string MISSIONS_FOLDER = "/Missions";

		// Token: 0x040002A5 RID: 677
		public string Name;

		// Token: 0x040002A6 RID: 678
		public string Language;

		// Token: 0x040002A7 RID: 679
		public string FolderPath;

		// Token: 0x040002A8 RID: 680
		public string StartingMissionPath;

		// Token: 0x040002A9 RID: 681
		public string StartingActionsPath;

		// Token: 0x040002AA RID: 682
		public string Description = "";

		// Token: 0x040002AB RID: 683
		public bool AllowSave = true;

		// Token: 0x040002AC RID: 684
		public bool StartsWithTutorial = false;

		// Token: 0x040002AD RID: 685
		public bool HasIntroStartup = true;

		// Token: 0x040002AE RID: 686
		public string Theme = "HacknetBlue";

		// Token: 0x040002AF RID: 687
		public string IntroStartupSong = null;

		// Token: 0x040002B0 RID: 688
		public float IntroStartupSongDelay = 0f;

		// Token: 0x040002B1 RID: 689
		public string[] StartingVisibleNodes = new string[0];

		// Token: 0x040002B2 RID: 690
		public List<string> FactionDescriptorPaths = new List<string>();

		// Token: 0x040002B3 RID: 691
		public Texture2D LogoImage;

		// Token: 0x040002B4 RID: 692
		public string SequencerTargetID;

		// Token: 0x040002B5 RID: 693
		public string SequencerFlagRequiredForStart;

		// Token: 0x040002B6 RID: 694
		public string ActionsToRunOnSequencerStart;

		// Token: 0x040002B7 RID: 695
		public float SequencerSpinUpTime = 17f;

		// Token: 0x040002B8 RID: 696
		public string WorkshopDescription;

		// Token: 0x040002B9 RID: 697
		public string WorkshopLanguage;

		// Token: 0x040002BA RID: 698
		public byte WorkshopVisibility = 2;

		// Token: 0x040002BB RID: 699
		public string WorkshopTags;

		// Token: 0x040002BC RID: 700
		public string WorkshopPreviewImagePath;

		// Token: 0x040002BD RID: 701
		public string WorkshopPublishID;
	}
}
