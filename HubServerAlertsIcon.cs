using System;
using System.Collections.Generic;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000023 RID: 35
	public class HubServerAlertsIcon
	{
		// Token: 0x060000EE RID: 238 RVA: 0x0000E75C File Offset: 0x0000C95C
		public HubServerAlertsIcon(ContentManager content, string serverToSyncTo, string[] tagsToAlertFor)
		{
			if (DLC1SessionUpgrader.HasDLC1Installed)
			{
				this.icon = content.Load<Texture2D>("DLC/Icons/ChatWideIcon");
				this.alertSound = content.Load<SoundEffect>("DLC/SFX/NotificationSound");
			}
			else
			{
				this.icon = content.Load<Texture2D>("Sprites/Misc/ChatWideIcon");
				this.alertSound = content.Load<SoundEffect>("SFX/EmailSound");
			}
			this.circle = content.Load<Texture2D>("CircleOutlineLarge");
			this.syncServerName = serverToSyncTo;
			this.TagsToAlertFor = tagsToAlertFor;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x0000E800 File Offset: 0x0000CA00
		public void Init(object OSobj)
		{
			this.os = (OS)OSobj;
			this.HubServerComp = Programs.getComputer(this.os, this.syncServerName);
			if (this.HubServerComp != null || !Settings.IsInExtensionMode)
			{
				this.HubServer = (this.HubServerComp.getDaemon(typeof(DLCHubServer)) as DLCHubServer);
				if (this.HubServer == null)
				{
					throw new NullReferenceException("DLCHubServer not found on AlertsIcon sync destination computer");
				}
				this.HubServer.SubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
			}
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x0000E8A0 File Offset: 0x0000CAA0
		public void UpdateTarget(object monitor, object comp)
		{
			if (this.HubServer != null)
			{
				this.HubServer.UnSubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
			}
			this.HubServer = (IMonitorableDaemon)monitor;
			this.HubServerComp = (Computer)comp;
			this.HubServer.SubscribeToAlertActionFroNewMessage(new Action<string, string>(this.ProcessNewLog));
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000E908 File Offset: 0x0000CB08
		private void ProcessNewLog(string Author, string Message)
		{
			for (int i = 0; i < this.TagsToAlertFor.Length; i++)
			{
				if (Message.Contains(this.TagsToAlertFor[i]))
				{
					if (this.HubServer.ShouldDisplayNotifications())
					{
						this.SendAlert();
					}
					break;
				}
			}
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000E964 File Offset: 0x0000CB64
		private void SendAlert()
		{
			if (!Settings.soundDisabled)
			{
				this.alertSound.Play();
			}
			this.RadiusCircles.Add(0f);
			if (this.os.connectedComp != this.HubServerComp)
			{
				this.PendingAlerts = true;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000E9BC File Offset: 0x0000CBBC
		public void Update(float dt)
		{
			for (int i = 0; i < this.RadiusCircles.Count; i++)
			{
				List<float> radiusCircles;
				int index;
				(radiusCircles = this.RadiusCircles)[index = i] = radiusCircles[index] + dt * 0.2f;
				if (this.RadiusCircles[i] > 1f)
				{
					this.RadiusCircles.RemoveAt(i);
					i--;
				}
			}
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000EA34 File Offset: 0x0000CC34
		private void ConnectToServer()
		{
			if (!this.os.terminal.preventingExecution)
			{
				if (!this.os.netMap.visibleNodes.Contains(this.os.netMap.nodes.IndexOf(this.HubServerComp)))
				{
					this.os.netMap.discoverNode(this.HubServerComp);
				}
				this.os.connectedComp = this.HubServerComp;
				this.HubServerComp.userLoggedIn = true;
				this.HubServer.navigatedTo();
				this.os.display.command = this.HubServer.GetName();
				this.PendingAlerts = false;
			}
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000EBC8 File Offset: 0x0000CDC8
		public void Draw(Rectangle dest, SpriteBatch sb)
		{
			if (Button.doButton(4568702, dest.X, dest.Y, dest.Width, dest.Height, "", new Color?(this.os.topBarIconsColor), this.icon))
			{
				if (this.IsEnabled)
				{
					this.ConnectToServer();
				}
			}
			if (this.RadiusCircles.Count > 0)
			{
				for (int i = 0; i < this.RadiusCircles.Count; i++)
				{
					float num = 320f - this.RadiusCircles[i] * 240f;
					float renderSize = num * Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(this.RadiusCircles[i]));
					int activeIndex = i;
					OS os = this.os;
					os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
					{
						sb.Draw(this.circle, Utils.InsetRectangle(new Rectangle(dest.X + dest.Width / 2, dest.Y + dest.Height / 2, 1, 1), (int)renderSize), this.os.highlightColor * (1f - this.RadiusCircles[activeIndex]));
					}));
				}
			}
			else if (this.PendingAlerts)
			{
				if (this.os.timer % 2f < 0.032258064f)
				{
					SFX.addCircle(new Vector2((float)dest.X, (float)dest.Y) + new Vector2((float)(dest.Width / 2), (float)(dest.Height / 2)), this.os.highlightColor, 40f);
				}
			}
		}

		// Token: 0x040000F3 RID: 243
		public bool IsEnabled = true;

		// Token: 0x040000F4 RID: 244
		private Texture2D icon;

		// Token: 0x040000F5 RID: 245
		private Texture2D circle;

		// Token: 0x040000F6 RID: 246
		private SoundEffect alertSound;

		// Token: 0x040000F7 RID: 247
		private string syncServerName;

		// Token: 0x040000F8 RID: 248
		private IMonitorableDaemon HubServer;

		// Token: 0x040000F9 RID: 249
		private Computer HubServerComp;

		// Token: 0x040000FA RID: 250
		private string[] TagsToAlertFor;

		// Token: 0x040000FB RID: 251
		private OS os;

		// Token: 0x040000FC RID: 252
		private bool PendingAlerts = false;

		// Token: 0x040000FD RID: 253
		private List<float> RadiusCircles = new List<float>();
	}
}
