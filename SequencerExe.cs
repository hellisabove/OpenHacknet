using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace Hacknet
{
	// Token: 0x020000BF RID: 191
	internal class SequencerExe : ExeModule
	{
		// Token: 0x060003E3 RID: 995 RVA: 0x0003BE64 File Offset: 0x0003A064
		public SequencerExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "Sequencer";
			this.ramCost = SequencerExe.ACTIVATING_RAM_COST;
			this.IdentifierName = "Sequencer";
			this.targetIP = this.os.thisComputer.ip;
			this.bars.MinLineChangeTime = 1f;
			this.bars.MaxLineChangeTime = 3f;
		}

		// Token: 0x060003E4 RID: 996 RVA: 0x0003BF38 File Offset: 0x0003A138
		public override void LoadContent()
		{
			base.LoadContent();
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			this.nodeeffects.Add(new ConnectedNodeEffect(this.os, true));
			if (Settings.isPressBuildDemo)
			{
				this.targetID = "finalNodeDemo";
				this.flagForProgressionName = "DemoSequencerEnabled";
			}
			else
			{
				this.targetID = "EnTechOfflineBackup";
				this.flagForProgressionName = "VaporSequencerEnabled";
			}
			if (ThemeManager.currentTheme == OSTheme.HacknetWhite)
			{
				this.targetTheme = OSTheme.HacknetBlue;
			}
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x0003C01C File Offset: 0x0003A21C
		public override void Update(float t)
		{
			base.Update(t);
			if (!this.HasBeenKilled)
			{
				this.bars.Update(t);
				this.UpdateRamCost(t);
				this.stateTimer += t;
				switch (this.state)
				{
				case SequencerExe.SequencerExeState.Unavaliable:
					if (this.os.Flags.HasFlag(this.flagForProgressionName) || Settings.debugCommandsEnabled)
					{
						this.state = SequencerExe.SequencerExeState.AwaitingActivation;
					}
					else
					{
						this.bars.MinLineChangeTime = 1f;
						this.bars.MaxLineChangeTime = 3f;
					}
					break;
				case SequencerExe.SequencerExeState.SpinningUp:
					if (MediaPlayer.State == MediaState.Playing)
					{
						if (MediaPlayer.PlayPosition.TotalSeconds >= this.beatDropTime && this.stateTimer > 10f)
						{
							this.MoveToActiveState();
						}
					}
					else if (this.stateTimer > SequencerExe.SPIN_UP_TIME)
					{
						this.MoveToActiveState();
					}
					break;
				case SequencerExe.SequencerExeState.Active:
				{
					float num = 2.5f;
					if (this.stateTimer < num)
					{
						if (Utils.randm(1f) < 0.3f + this.stateTimer / num * 0.7f)
						{
							ThemeManager.switchThemeColors(this.os, this.targetTheme);
							ThemeManager.loadThemeBackground(this.os, this.targetTheme);
							ThemeManager.currentTheme = this.targetTheme;
						}
						else
						{
							ThemeManager.switchThemeColors(this.os, this.originalTheme);
							ThemeManager.loadThemeBackground(this.os, this.originalTheme);
							ThemeManager.currentTheme = this.originalTheme;
						}
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

		// Token: 0x060003E6 RID: 998 RVA: 0x0003C248 File Offset: 0x0003A448
		private void ActiveStateUpdate(float t)
		{
			PostProcessor.dangerModeEnabled = true;
			double num = (double)this.stateTimer;
			if (MediaPlayer.State == MediaState.Playing && !Settings.soundDisabled)
			{
				num = MediaPlayer.PlayPosition.TotalSeconds;
			}
			PostProcessor.dangerModePercentComplete = (float)((num - (double)SequencerExe.SPIN_UP_TIME) / (SequencerExe.Song_Length - (double)SequencerExe.SPIN_UP_TIME));
			if (PostProcessor.dangerModePercentComplete >= 1f)
			{
				if (Settings.isDemoMode)
				{
					MissionFunctions.runCommand(0, "demoFinalMissionEnd");
				}
				else
				{
					MusicManager.playSongImmediatley("Music/Ambient/AmbientDrone_Clipped");
					this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
					PostProcessor.dangerModeEnabled = false;
					PostProcessor.dangerModePercentComplete = 0f;
					this.os.thisComputer.crash(this.os.thisComputer.ip);
				}
			}
			if (this.os.connectedComp == null && this.stateTimer > 1f)
			{
				if (!Settings.isDemoMode)
				{
					this.isExiting = true;
					if (this.oldSongName != null)
					{
						MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
						MediaPlayer.IsRepeating = true;
					}
					this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
					PostProcessor.dangerModeEnabled = false;
					PostProcessor.dangerModePercentComplete = 0f;
				}
			}
			double num2 = num - this.beatDropTime;
			if (num2 % (double)SequencerExe.TimeBetweenBeats < (double)t)
			{
				this.os.warningFlash();
			}
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x0003C408 File Offset: 0x0003A608
		private void UpdateRamCost(float t)
		{
			if (this.targetRamUse != this.ramCost)
			{
				if (this.targetRamUse < this.ramCost)
				{
					this.ramCost -= (int)(t * SequencerExe.RAM_CHANGE_PS);
					if (this.ramCost < this.targetRamUse)
					{
						this.ramCost = this.targetRamUse;
					}
				}
				else
				{
					int num = (int)(t * SequencerExe.RAM_CHANGE_PS);
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

		// Token: 0x060003E8 RID: 1000 RVA: 0x0003C4D4 File Offset: 0x0003A6D4
		public override void Killed()
		{
			base.Killed();
			this.HasBeenKilled = true;
			PostProcessor.dangerModeEnabled = false;
			PostProcessor.dangerModePercentComplete = 0f;
			if (this.oldSongName != null)
			{
				MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
				MediaPlayer.IsRepeating = true;
			}
			this.os.netMap.DimNonConnectedNodes = false;
			this.os.runCommand("disconnect");
			if (this.targetComp != null)
			{
				this.os.netMap.visibleNodes.Remove(this.os.netMap.nodes.IndexOf(this.targetComp));
			}
		}

		// Token: 0x060003E9 RID: 1001 RVA: 0x0003C598 File Offset: 0x0003A798
		private void MoveToActiveState()
		{
			this.state = SequencerExe.SequencerExeState.Active;
			this.stateTimer = 0f;
			this.targetRamUse = SequencerExe.BASE_RAM_COST;
			this.os.warningFlashTimer = OS.WARNING_FLASH_TIME;
			this.os.netMap.DimNonConnectedNodes = true;
			this.os.netMap.discoverNode(this.targetComp);
			this.os.runCommand("connect " + this.targetComp.ip);
			this.os.delayer.Post(ActionDelayer.Wait(0.05), delegate
			{
				this.os.runCommand("probe");
			});
		}

		// Token: 0x060003EA RID: 1002 RVA: 0x0003C648 File Offset: 0x0003A848
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
			case SequencerExe.SequencerExeState.Unavaliable:
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
			case SequencerExe.SequencerExeState.AwaitingActivation:
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
						this.state = SequencerExe.SequencerExeState.SpinningUp;
						this.bars.MinLineChangeTime = 0.1f;
						this.bars.MaxLineChangeTime = 1f;
						this.originalTheme = ThemeManager.currentTheme;
						MusicManager.FADE_TIME = 0.6f;
						this.oldSongName = MusicManager.currentSongName;
						MusicManager.transitionToSong("Music\\Roller_Mobster_Clipped");
						MediaPlayer.IsRepeating = false;
						this.targetComp = Programs.getComputer(this.os, this.targetID);
						WebServerDaemon webServerDaemon = (WebServerDaemon)this.targetComp.getDaemon(typeof(WebServerDaemon));
						if (webServerDaemon != null)
						{
							webServerDaemon.LoadWebPage("index.html");
						}
					}
				}
				break;
			}
			case SequencerExe.SequencerExeState.SpinningUp:
			{
				Rectangle bounds = rectangle;
				bounds.Height = (int)((float)bounds.Height * (this.stateTimer / SequencerExe.SPIN_UP_TIME));
				bounds.Y = rectangle.Y + rectangle.Height - bounds.Height + 1;
				bounds.Width += 4;
				this.bars.Draw(this.spriteBatch, bounds, num2, 4f, 1f, this.os.brightLockedColor);
				break;
			}
			case SequencerExe.SequencerExeState.Active:
				this.spriteBatch.Draw(Utils.white, base.GetContentAreaDest(), Color.Black * 0.5f);
				TextItem.doFontLabelToSize(base.GetContentAreaDest(), " G O   G O   G O ", GuiData.titlefont, Color.Lerp(Utils.AddativeRed, this.os.brightLockedColor, Math.Min(1f, this.stateTimer / 2f)), false, false);
				this.DrawActiveState();
				break;
			}
		}

		// Token: 0x060003EB RID: 1003 RVA: 0x0003CAC4 File Offset: 0x0003ACC4
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
			this.DrawCountdownOverlay();
		}

		// Token: 0x060003EC RID: 1004 RVA: 0x0003CBF0 File Offset: 0x0003ADF0
		private void DrawCountdownOverlay()
		{
			int num = 110;
			Rectangle destinationRectangle = new Rectangle(0, this.os.fullscreen.Height - num - 20, 400, num);
			Color value = new Color(100, 0, 0, 200);
			this.spriteBatch.Draw(Utils.white, destinationRectangle, Color.Lerp(value, Color.Transparent, Utils.randm(0.2f)));
			Vector2 pos = new Vector2((float)(destinationRectangle.X + 6), (float)(destinationRectangle.Y + 4));
			TextItem.doFontLabel(pos, "ENTECH SEQUENCER ATTACK", GuiData.titlefont, new Color?(Color.White), (float)(destinationRectangle.Width - 12), 35f, false);
			pos.Y += 32f;
			TextItem.doFontLabel(pos, Settings.isDemoMode ? "Analyse security countermeasures with \"Probe\"" : LocaleTerms.Loc("Break active security on target"), GuiData.smallfont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
			pos.Y += 16f;
			TextItem.doFontLabel(pos, Settings.isDemoMode ? "Break active security and gain access (Programs + Porthack)" : LocaleTerms.Loc("Delete all Hacknet related files"), GuiData.smallfont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
			pos.Y += 16f;
			TextItem.doFontLabel(pos, Settings.isDemoMode ? "Delete all files in directories /sys/ and /log/" : LocaleTerms.Loc("Disconnect"), GuiData.smallfont, new Color?(Color.White), (float)(destinationRectangle.Width - 10), 20f, false);
		}

		// Token: 0x0400047C RID: 1148
		public static int ACTIVATING_RAM_COST = 170;

		// Token: 0x0400047D RID: 1149
		public static int BASE_RAM_COST = 60;

		// Token: 0x0400047E RID: 1150
		public static float RAM_CHANGE_PS = 100f;

		// Token: 0x0400047F RID: 1151
		public static double Song_Length = 186.0;

		// Token: 0x04000480 RID: 1152
		public static float SPIN_UP_TIME = 17f;

		// Token: 0x04000481 RID: 1153
		private static float TimeBetweenBeats = 1.832061f;

		// Token: 0x04000482 RID: 1154
		private MovingBarsEffect bars = new MovingBarsEffect();

		// Token: 0x04000483 RID: 1155
		private string targetID;

		// Token: 0x04000484 RID: 1156
		private string flagForProgressionName;

		// Token: 0x04000485 RID: 1157
		private string oldSongName = null;

		// Token: 0x04000486 RID: 1158
		private int targetRamUse = SequencerExe.ACTIVATING_RAM_COST;

		// Token: 0x04000487 RID: 1159
		private float stateTimer = 0f;

		// Token: 0x04000488 RID: 1160
		private float beatHits = 0.15f;

		// Token: 0x04000489 RID: 1161
		private double beatDropTime = 16.64;

		// Token: 0x0400048A RID: 1162
		private List<ConnectedNodeEffect> nodeeffects = new List<ConnectedNodeEffect>();

		// Token: 0x0400048B RID: 1163
		private SequencerExe.SequencerExeState state = SequencerExe.SequencerExeState.Unavaliable;

		// Token: 0x0400048C RID: 1164
		private Computer targetComp;

		// Token: 0x0400048D RID: 1165
		private OSTheme originalTheme;

		// Token: 0x0400048E RID: 1166
		private OSTheme targetTheme = OSTheme.HacknetWhite;

		// Token: 0x0400048F RID: 1167
		private bool HasBeenKilled = false;

		// Token: 0x020000C0 RID: 192
		private enum SequencerExeState
		{
			// Token: 0x04000491 RID: 1169
			Unavaliable,
			// Token: 0x04000492 RID: 1170
			AwaitingActivation,
			// Token: 0x04000493 RID: 1171
			SpinningUp,
			// Token: 0x04000494 RID: 1172
			Active
		}
	}
}
