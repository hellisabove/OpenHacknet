using System;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200012A RID: 298
	internal class MailIcon : MailResponder
	{
		// Token: 0x06000704 RID: 1796 RVA: 0x00073188 File Offset: 0x00071388
		public MailIcon(OS operatingSystem, Vector2 position)
		{
			this.os = operatingSystem;
			this.pos = position;
			this.alertTimer = 0f;
			this.tex = this.os.content.Load<Texture2D>("UnopenedMail");
			this.texBig = this.os.content.Load<Texture2D>("MailIconBig");
			this.bigTexScaleMod = (float)this.tex.Width / (float)this.texBig.Width;
			this.newMailSound = this.os.content.Load<SoundEffect>("SFX/EmailSound");
			if (!this.os.multiplayer)
			{
				MailServer mailServer = (MailServer)this.os.netMap.mailServer.daemons[0];
				mailServer.addResponder(this);
			}
			this.mailUnchecked = false;
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x0007328C File Offset: 0x0007148C
		public void UpdateTargetServer(MailServer server)
		{
			if (this.targetServer != null)
			{
				this.targetServer.removeResponder(this);
			}
			else
			{
				(this.os.netMap.mailServer.daemons[0] as MailServer).removeResponder(this);
			}
			this.targetServer = server;
			this.targetServer.addResponder(this);
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x000732F4 File Offset: 0x000714F4
		public void Update(float t)
		{
			float num = this.alertTimer;
			if (!this.os.Flags.HasFlag("FirstAlertComplete"))
			{
				this.alertTimer -= t * 0.65f;
			}
			else
			{
				this.alertTimer -= t;
			}
			if (this.alertTimer <= 0f)
			{
				this.alertTimer = 0f;
			}
			if (num > 0f && this.alertTimer <= 0f)
			{
				float num2 = 280f;
				if (!this.os.Flags.HasFlag("FirstAlertComplete"))
				{
					this.os.Flags.AddFlag("FirstAlertComplete");
					num2 *= this.firstEverAlertMod;
				}
				SFX.addCircle(this.pos + new Vector2((float)this.tex.Width * MailIcon.SCALE / 2f, (float)this.tex.Height * MailIcon.SCALE / 2f), MailIcon.uncheckedMailPulseColor, num2);
			}
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x00073510 File Offset: 0x00071710
		public void Draw()
		{
			if (Button.doButton(45687, (int)this.pos.X, (int)this.pos.Y, (int)((float)this.tex.Width * MailIcon.SCALE), (int)((float)this.tex.Height * MailIcon.SCALE), "", new Color?(this.os.topBarIconsColor), this.tex))
			{
				if (this.isEnabled)
				{
					this.connectToMail();
				}
			}
			float percent = this.alertTimer / MailIcon.ALERT_TIME;
			percent *= percent;
			percent *= percent;
			percent = (float)Math.Pow((double)percent, (double)(1f - percent));
			Vector2 iconPositionOffset = new Vector2(-100f, 20f);
			float scaleMod = this.bigTexScaleMod;
			if (!this.os.Flags.HasFlag("FirstAlertComplete"))
			{
				scaleMod += this.firstEverAlertMod * percent;
				iconPositionOffset *= this.firstEverAlertMod * percent;
			}
			if (this.alertTimer > 0f)
			{
				OS os = this.os;
				os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
				{
					Vector2 vector = new Vector2((float)this.texBig.Width * MailIcon.SCALE / 2f, (float)this.texBig.Height * MailIcon.SCALE / 2f);
					GuiData.spriteBatch.Draw(this.texBig, this.pos + vector * scaleMod + iconPositionOffset * percent, null, Color.White * (1f - percent), 0f, vector, (Vector2.One + Vector2.One * percent * 25f) * scaleMod, SpriteEffects.None, 0.5f);
				}));
			}
			else if (this.mailUnchecked)
			{
				if (this.os.timer % 2f < 0.032258064f)
				{
					SFX.addCircle(this.pos + new Vector2((float)this.tex.Width * MailIcon.SCALE / 2f, (float)this.tex.Height * MailIcon.SCALE / 2f), MailIcon.uncheckedMailPulseColor, 40f);
				}
			}
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x0007374C File Offset: 0x0007194C
		public int getWidth()
		{
			return (int)((float)this.tex.Width * MailIcon.SCALE);
		}

		// Token: 0x06000709 RID: 1801 RVA: 0x00073774 File Offset: 0x00071974
		public void connectToMail()
		{
			if (!this.os.terminal.preventingExecution)
			{
				if (!this.os.netMap.visibleNodes.Contains(this.os.netMap.nodes.IndexOf(this.os.netMap.mailServer)))
				{
					this.os.netMap.discoverNode(this.os.netMap.mailServer);
				}
				MailServer mailServer = (MailServer)this.os.netMap.mailServer.daemons[0];
				if (this.targetServer != null)
				{
					mailServer = this.targetServer;
				}
				this.os.connectedComp = mailServer.comp;
				this.os.terminal.prompt = this.os.connectedComp.ip + "@> ";
				mailServer.comp.userLoggedIn = true;
				this.os.display.command = mailServer.name;
				mailServer.viewInbox(this.os.defaultUser);
				this.mailUnchecked = false;
			}
		}

		// Token: 0x0600070A RID: 1802 RVA: 0x000738A7 File Offset: 0x00071AA7
		public void mailSent(string mail, string userTo)
		{
		}

		// Token: 0x0600070B RID: 1803 RVA: 0x000738AC File Offset: 0x00071AAC
		public void mailReceived(string mail, string userTo)
		{
			if (userTo.Equals(this.os.defaultUser.name))
			{
				this.alertTimer = MailIcon.ALERT_TIME;
				if (!Settings.soundDisabled)
				{
					this.newMailSound.Play();
				}
				if (this.os.connectedComp == null || !this.os.connectedComp.Equals(this.os.netMap.mailServer))
				{
					this.mailUnchecked = true;
				}
			}
		}

		// Token: 0x040007DC RID: 2012
		private static float ALERT_TIME = 2.4f;

		// Token: 0x040007DD RID: 2013
		private static float SCALE = 1f;

		// Token: 0x040007DE RID: 2014
		public bool isEnabled = true;

		// Token: 0x040007DF RID: 2015
		private Texture2D tex;

		// Token: 0x040007E0 RID: 2016
		private Texture2D texBig;

		// Token: 0x040007E1 RID: 2017
		private float bigTexScaleMod = 1f;

		// Token: 0x040007E2 RID: 2018
		private float firstEverAlertMod = 2.7f;

		// Token: 0x040007E3 RID: 2019
		public Vector2 pos;

		// Token: 0x040007E4 RID: 2020
		private SoundEffect newMailSound;

		// Token: 0x040007E5 RID: 2021
		private float alertTimer;

		// Token: 0x040007E6 RID: 2022
		private bool mailUnchecked;

		// Token: 0x040007E7 RID: 2023
		private static Color uncheckedMailPulseColor = new Color(110, 110, 110, 0);

		// Token: 0x040007E8 RID: 2024
		private OS os;

		// Token: 0x040007E9 RID: 2025
		private MailServer targetServer = null;
	}
}
