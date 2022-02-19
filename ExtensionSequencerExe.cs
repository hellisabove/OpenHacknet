using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Extensions;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x0200006D RID: 109
	internal class ExtensionSequencerExe : ExeModule
	{
		// Token: 0x06000222 RID: 546 RVA: 0x0001E424 File Offset: 0x0001C624
		public ExtensionSequencerExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "ESequencer";
			this.ramCost = ExtensionSequencerExe.ACTIVATING_RAM_COST;
			this.IdentifierName = "ESequencer";
			this.targetIP = this.os.thisComputer.ip;
			this.bars.MinLineChangeTime = 1f;
			this.bars.MaxLineChangeTime = 3f;
			this.beatDropTime = (double)ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime;
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0001E500 File Offset: 0x0001C700
		public override void LoadContent()
		{
			base.LoadContent();
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.targetID = ExtensionLoader.ActiveExtensionInfo.SequencerTargetID;
			this.flagForProgressionName = ExtensionLoader.ActiveExtensionInfo.SequencerFlagRequiredForStart;
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0001E5B0 File Offset: 0x0001C7B0
		public override void Update(float t)
		{
			base.Update(t);
			if (!this.HasBeenKilled)
			{
				this.bars.Update(t);
				this.UpdateRamCost(t);
				this.stateTimer += t;
				if (this.isExiting)
				{
					this.os.netMap.DimNonConnectedNodes = false;
				}
				bool flag = true;
				if (!string.IsNullOrWhiteSpace(this.flagForProgressionName))
				{
					string[] array = this.flagForProgressionName.Split(Utils.newlineDelim, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length; i++)
					{
						flag &= (this.os.Flags.HasFlag(this.flagForProgressionName) || Settings.debugCommandsEnabled);
					}
				}
				switch (this.state)
				{
				case ExtensionSequencerExe.SequencerExeState.Unavaliable:
					if (flag)
					{
						this.state = ExtensionSequencerExe.SequencerExeState.AwaitingActivation;
					}
					else
					{
						this.bars.MinLineChangeTime = 1f;
						this.bars.MaxLineChangeTime = 3f;
					}
					break;
				case ExtensionSequencerExe.SequencerExeState.AwaitingActivation:
					if (!flag)
					{
						this.state = ExtensionSequencerExe.SequencerExeState.Unavaliable;
					}
					break;
				case ExtensionSequencerExe.SequencerExeState.SpinningUp:
					if (MediaPlayer.State == MediaState.Playing)
					{
						if (MediaPlayer.PlayPosition.TotalSeconds >= this.beatDropTime && (double)this.stateTimer > this.beatDropTime - 0.5)
						{
							this.MoveToActiveState();
						}
					}
					else if (this.stateTimer > ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime)
					{
						this.MoveToActiveState();
					}
					break;
				case ExtensionSequencerExe.SequencerExeState.Active:
				{
					float num = 2.5f;
					if (this.stateTimer < num)
					{
						double num2 = MediaPlayer.PlayPosition.TotalSeconds - this.beatDropTime;
						if (num2 % (double)this.beatHits < 0.009999999776482582)
						{
							this.os.warningFlash();
						}
					}
					this.ActiveStateUpdate(t);
					break;
				}
				}
			}
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0001E7DC File Offset: 0x0001C9DC
		private void ActiveStateUpdate(float t)
		{
			double num = (double)this.stateTimer;
			double num2 = num - this.beatDropTime;
			if (num2 % (double)ExtensionSequencerExe.TimeBetweenBeats < (double)t)
			{
				this.os.warningFlash();
			}
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0001E81C File Offset: 0x0001CA1C
		private void UpdateRamCost(float t)
		{
			if (this.targetRamUse != this.ramCost)
			{
				if (this.targetRamUse < this.ramCost)
				{
					this.ramCost -= (int)(t * ExtensionSequencerExe.RAM_CHANGE_PS);
					if (this.ramCost < this.targetRamUse)
					{
						this.ramCost = this.targetRamUse;
					}
				}
				else
				{
					int num = (int)(t * ExtensionSequencerExe.RAM_CHANGE_PS);
					if (this.os.ramAvaliable >= num)
					{
						this.ramCost += num;
						if (this.ramCost > this.targetRamUse)
						{
							this.ramCost = this.targetRamUse;
						}
					}
				}
			}
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0001E968 File Offset: 0x0001CB68
		public override void Killed()
		{
			base.Killed();
			if (this.state == ExtensionSequencerExe.SequencerExeState.Active)
			{
				this.os.delayer.Post(ActionDelayer.NextTick(), delegate
				{
					ExtensionSequencerExe exe = new ExtensionSequencerExe(this.bounds, this.os, new string[0])
					{
						state = this.state,
						stateTimer = this.stateTimer,
						bounds = this.bounds,
						oldSongName = this.oldSongName,
						nodeeffects = this.nodeeffects,
						ramCost = this.ramCost
					};
					this.os.addExe(exe);
				});
			}
			else
			{
				this.os.netMap.DimNonConnectedNodes = false;
				this.os.runCommand("disconnect");
				if (this.targetComp != null)
				{
					this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
				}
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0001EA20 File Offset: 0x0001CC20
		private void MoveToActiveState()
		{
			this.state = ExtensionSequencerExe.SequencerExeState.Active;
			this.stateTimer = 0f;
			this.targetRamUse = ExtensionSequencerExe.BASE_RAM_COST;
			this.os.warningFlashTimer = OS.WARNING_FLASH_TIME;
			this.os.netMap.DimNonConnectedNodes = true;
			this.os.netMap.discoverNode(this.targetComp);
			this.os.runCommand("connect " + this.targetComp.ip);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0001EAA4 File Offset: 0x0001CCA4
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			Rectangle rectangle = base.GetContentAreaDest();
			rectangle = Utils.InsetRectangle(rectangle, 1);
			float num = this.os.warningFlashTimer / OS.WARNING_FLASH_TIME;
			float num2 = 2f;
			if (num > 0f)
			{
				num2 += num * ((float)rectangle.Height - num2);
			}
			Color drawColor = Color.Lerp(Utils.AddativeWhite * 0.5f, Utils.AddativeRed, num);
			this.bars.Draw(this.spriteBatch, base.GetContentAreaDest(), num2, 4f, 1f, drawColor);
			switch (this.state)
			{
			case ExtensionSequencerExe.SequencerExeState.Unavaliable:
			{
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.5f);
				Rectangle rectangle2 = Utils.InsetRectangle(rectangle, 6);
				if (!this.isExiting)
				{
					TextItem.doFontLabelToSize(rectangle2, "LINK UNAVAILABLE", GuiData.titlefont, Utils.AddativeWhite, false, false);
				}
				Rectangle destinationRectangle = rectangle2;
				destinationRectangle.Y += destinationRectangle.Height - 20;
				destinationRectangle.Height = 20;
				if (!this.isExiting)
				{
					GuiData.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Black * 0.5f);
					if (Button.doButton(32711803, destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Width, destinationRectangle.Height, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
					{
						this.isExiting = true;
					}
				}
				break;
			}
			case ExtensionSequencerExe.SequencerExeState.AwaitingActivation:
			{
				int num3 = 30;
				Rectangle destinationRectangle2 = new Rectangle(this.bounds.X + 1, this.bounds.Y + this.bounds.Height / 2 - num3, this.bounds.Width - 2, num3 * 2);
				this.spriteBatch.Draw(Utils.white, destinationRectangle2, Color.Black * 0.92f);
				if (Button.doButton(8310101, this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - num3 / 2, this.bounds.Width - 20, num3, LocaleTerms.Loc("ACTIVATE"), new Color?(this.os.highlightColor)))
				{
					if (this.os.TraceDangerSequence.IsActive)
					{
						this.os.write("SEQUENCER ERROR: OS reports critical action already in progress.");
					}
					else
					{
						this.stateTimer = 0f;
						this.state = ExtensionSequencerExe.SequencerExeState.SpinningUp;
						this.bars.MinLineChangeTime = 0.1f;
						this.bars.MaxLineChangeTime = 1f;
						MusicManager.FADE_TIME = 0.6f;
						this.oldSongName = MusicManager.currentSongName;
						this.targetComp = Programs.getComputer(this.os, this.targetID);
						WebServerDaemon webServerDaemon = (WebServerDaemon)this.targetComp.getDaemon(typeof(WebServerDaemon));
						if (webServerDaemon != null)
						{
							webServerDaemon.LoadWebPage("index.html");
						}
						RunnableConditionalActions.LoadIntoOS(ExtensionLoader.ActiveExtensionInfo.ActionsToRunOnSequencerStart, this.os);
					}
				}
				break;
			}
			case ExtensionSequencerExe.SequencerExeState.SpinningUp:
			{
				Rectangle bounds = rectangle;
				bounds.Height = (int)((float)bounds.Height * (this.stateTimer / ExtensionLoader.ActiveExtensionInfo.SequencerSpinUpTime));
				bounds.Y = rectangle.Y + rectangle.Height - bounds.Height + 1;
				bounds.Width += 4;
				this.bars.Draw(this.spriteBatch, bounds, num2, 4f, 1f, this.os.brightLockedColor);
				break;
			}
			case ExtensionSequencerExe.SequencerExeState.Active:
				this.spriteBatch.Draw(Utils.white, base.GetContentAreaDest(), Color.Black * 0.5f);
				TextItem.doFontLabelToSize(base.GetContentAreaDest(), " G O   G O   G O ", GuiData.titlefont, Color.Lerp(Utils.AddativeRed, this.os.brightLockedColor, Math.Min(1f, this.stateTimer / 2f)), false, false);
				this.DrawActiveState();
				break;
			}
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0001EF20 File Offset: 0x0001D120
		private void DrawActiveState()
		{
			float num = 5.2f;
			float point = Math.Min(num, this.stateTimer) / num;
			float num2 = 30f;
			Vector2 value = new Vector2((float)this.os.netMap.bounds.X, (float)this.os.netMap.bounds.Y) + new Vector2((float)NetworkMap.NODE_SIZE / 2f);
			for (int i = 0; i < this.nodeeffects.Count; i++)
			{
				float num3 = (float)(i + 1) / (float)(this.nodeeffects.Count + 1);
				float num4 = 3f * num3;
				float num5 = 1f - Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(point)));
				this.nodeeffects[i].ScaleFactor = num3 * (num2 * num5) + num4;
				this.nodeeffects[i].draw(this.spriteBatch, value + this.os.netMap.GetNodeDrawPos(this.targetComp));
			}
		}

		// Token: 0x0400025C RID: 604
		public static int ACTIVATING_RAM_COST = 170;

		// Token: 0x0400025D RID: 605
		public static int BASE_RAM_COST = 60;

		// Token: 0x0400025E RID: 606
		public static float RAM_CHANGE_PS = 100f;

		// Token: 0x0400025F RID: 607
		public static double Song_Length = 186.0;

		// Token: 0x04000260 RID: 608
		private static float TimeBetweenBeats = 1.832061f;

		// Token: 0x04000261 RID: 609
		private MovingBarsEffect bars = new MovingBarsEffect();

		// Token: 0x04000262 RID: 610
		private string targetID;

		// Token: 0x04000263 RID: 611
		private string flagForProgressionName;

		// Token: 0x04000264 RID: 612
		private string oldSongName = null;

		// Token: 0x04000265 RID: 613
		private int targetRamUse = ExtensionSequencerExe.ACTIVATING_RAM_COST;

		// Token: 0x04000266 RID: 614
		private float stateTimer = 0f;

		// Token: 0x04000267 RID: 615
		private float beatHits = 0.15f;

		// Token: 0x04000268 RID: 616
		private double beatDropTime = 16.64;

		// Token: 0x04000269 RID: 617
		private List<ConnectedNodeEffect> nodeeffects = new List<ConnectedNodeEffect>();

		// Token: 0x0400026A RID: 618
		private ExtensionSequencerExe.SequencerExeState state = ExtensionSequencerExe.SequencerExeState.Unavaliable;

		// Token: 0x0400026B RID: 619
		private Computer targetComp;

		// Token: 0x0400026C RID: 620
		private bool HasBeenKilled = false;

		// Token: 0x0200006E RID: 110
		private enum SequencerExeState
		{
			// Token: 0x0400026E RID: 622
			Unavaliable,
			// Token: 0x0400026F RID: 623
			AwaitingActivation,
			// Token: 0x04000270 RID: 624
			SpinningUp,
			// Token: 0x04000271 RID: 625
			Active
		}
	}
}
