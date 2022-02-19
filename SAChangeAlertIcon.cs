using System;
using System.Xml;

namespace Hacknet
{
	// Token: 0x02000041 RID: 65
	public class SAChangeAlertIcon : SerializableAction
	{
		// Token: 0x0600015F RID: 351 RVA: 0x00013D40 File Offset: 0x00011F40
		public override void Trigger(object os_obj)
		{
			OS os = (OS)os_obj;
			if (this.Delay <= 0f)
			{
				string flagStartingWith = os.Flags.GetFlagStartingWith("_changeAlertIconType:");
				string flagStartingWith2 = os.Flags.GetFlagStartingWith("_changeAlertIconTarget:");
				if (flagStartingWith != null)
				{
					os.Flags.RemoveFlag(flagStartingWith);
				}
				if (flagStartingWith2 != null)
				{
					os.Flags.RemoveFlag(flagStartingWith2);
				}
				os.Flags.AddFlag("_changeAlertIconType:" + this.Type);
				os.Flags.AddFlag("_changeAlertIconTarget:" + this.Target);
				if (!os.Flags.HasFlag("_alertIconChanged"))
				{
					os.Flags.AddFlag("_alertIconChanged");
				}
				SAChangeAlertIcon.UpdateAlertIcon(os);
			}
			else
			{
				Computer computer = Programs.getComputer(os, this.DelayHost);
				if (computer == null)
				{
					throw new NullReferenceException("Computer " + computer + " could not be found as DelayHost for Function");
				}
				float delay = this.Delay;
				this.Delay = -1f;
				DelayableActionSystem.FindDelayableActionSystemOnComputer(computer).AddAction(this, delay);
			}
		}

		// Token: 0x06000160 RID: 352 RVA: 0x00013E7C File Offset: 0x0001207C
		public static SerializableAction DeserializeFromReader(XmlReader rdr)
		{
			SAChangeAlertIcon sachangeAlertIcon = new SAChangeAlertIcon();
			if (rdr.MoveToAttribute("Delay"))
			{
				sachangeAlertIcon.Delay = rdr.ReadContentAsFloat();
			}
			if (rdr.MoveToAttribute("DelayHost"))
			{
				sachangeAlertIcon.DelayHost = rdr.ReadContentAsString();
			}
			if (rdr.MoveToAttribute("Type"))
			{
				sachangeAlertIcon.Type = rdr.ReadContentAsString().ToLower();
			}
			if (rdr.MoveToAttribute("Target"))
			{
				sachangeAlertIcon.Target = rdr.ReadContentAsString();
			}
			if (sachangeAlertIcon.Type == null)
			{
				throw new FormatException("Type tag for ChangeAlertIconAction not found! Make sure you have written it correctly (capital T)!");
			}
			if (sachangeAlertIcon.Type != "mail" && sachangeAlertIcon.Type != "irc" && sachangeAlertIcon.Type != "board" && sachangeAlertIcon.Type != "irchub")
			{
				throw new FormatException("Provided type " + sachangeAlertIcon.Type + " for ChangeAlertIconAction is invalid! Accepted types: mail, irc, board");
			}
			if (sachangeAlertIcon.Type != "mail" && !DLC1SessionUpgrader.HasDLC1Installed)
			{
				throw new NotSupportedException("Changing alert icon to something other than mail requires the Hacknet Labyrinths DLC to be installed.");
			}
			return sachangeAlertIcon;
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00013FC4 File Offset: 0x000121C4
		public static void UpdateAlertIcon(object osobj)
		{
			OS os = (OS)osobj;
			string flagStartingWith = os.Flags.GetFlagStartingWith("_changeAlertIconType:");
			string flagStartingWith2 = os.Flags.GetFlagStartingWith("_changeAlertIconTarget:");
			if (flagStartingWith != null && flagStartingWith2 != null)
			{
				string text = flagStartingWith.Substring("_changeAlertIconType:".Length);
				string text2 = flagStartingWith2.Substring("_changeAlertIconTarget:".Length);
				Computer computer = Programs.getComputer(os, text2);
				string text3 = text.ToLower();
				if (text3 != null)
				{
					if (!(text3 == "mail"))
					{
						if (!(text3 == "irc"))
						{
							if (!(text3 == "irchub"))
							{
								if (text3 == "board")
								{
									MessageBoardDaemon messageBoardDaemon = (MessageBoardDaemon)computer.getDaemon(typeof(MessageBoardDaemon));
									os.ShowDLCAlertsIcon = true;
									os.hubServerAlertsIcon.UpdateTarget(messageBoardDaemon, messageBoardDaemon.comp);
								}
							}
							else
							{
								DLCHubServer dlchubServer = (DLCHubServer)computer.getDaemon(typeof(DLCHubServer));
								os.ShowDLCAlertsIcon = true;
								os.hubServerAlertsIcon.UpdateTarget(dlchubServer, dlchubServer.comp);
							}
						}
						else
						{
							IRCDaemon ircdaemon = (IRCDaemon)computer.getDaemon(typeof(IRCDaemon));
							os.ShowDLCAlertsIcon = true;
							os.hubServerAlertsIcon.UpdateTarget(ircdaemon, ircdaemon.comp);
						}
					}
					else
					{
						MailServer mailServer = (MailServer)computer.getDaemon(typeof(MailServer));
						bool flag = false;
						for (int i = 0; i < mailServer.comp.users.Count; i++)
						{
							if (mailServer.comp.users[i].name == os.defaultUser.name)
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							throw new FormatException("Mail server " + text2 + " does not have a user account for the player!\nA mail server must have a player account to be used as the alert icon");
						}
						os.mailicon.UpdateTargetServer(mailServer);
						os.ShowDLCAlertsIcon = false;
					}
				}
			}
		}

		// Token: 0x0400015E RID: 350
		private const string TypeFlag = "_changeAlertIconType:";

		// Token: 0x0400015F RID: 351
		private const string TargetFlag = "_changeAlertIconTarget:";

		// Token: 0x04000160 RID: 352
		public string Type;

		// Token: 0x04000161 RID: 353
		public string Target;

		// Token: 0x04000162 RID: 354
		public string DelayHost;

		// Token: 0x04000163 RID: 355
		public float Delay;
	}
}
