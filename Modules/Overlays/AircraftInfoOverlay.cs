using System;
using Hacknet.Daemons.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace Hacknet.Modules.Overlays
{
	// Token: 0x02000018 RID: 24
	public class AircraftInfoOverlay
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x0000CCEC File Offset: 0x0000AEEC
		public AircraftInfoOverlay(object OSobj)
		{
			this.os = (OS)OSobj;
			Computer computer = Programs.getComputer(this.os, "dair_crash");
			this.CrashingAircraft = (AircraftDaemon)computer.getDaemon(typeof(AircraftDaemon));
			AircraftDaemon crashingAircraft = this.CrashingAircraft;
			crashingAircraft.CrashAction = (Action)Delegate.Combine(crashingAircraft.CrashAction, new Action(delegate()
			{
				this.CrashingAircraft = null;
			}));
			Computer computer2 = Programs.getComputer(this.os, "dair_secondary");
			this.SecondaryAircraft = (AircraftDaemon)computer2.getDaemon(typeof(AircraftDaemon));
			AircraftDaemon secondaryAircraft = this.SecondaryAircraft;
			secondaryAircraft.CrashAction = (Action)Delegate.Combine(secondaryAircraft.CrashAction, new Action(delegate()
			{
				this.SecondaryAircraft = null;
			}));
			this.AircraftSaveSound = this.os.content.Load<SoundEffect>("SFX/TraceKill");
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000CE14 File Offset: 0x0000B014
		public void Activate()
		{
			this.IsActive = true;
			this.timeElapsed = 0f;
			this.flashInTimeLeft = 1f;
			this.CrashingAircraft.StartUpdating();
			this.SecondaryAircraft.StartUpdating();
			this.os.Flags.AddFlag("AircraftInfoOverlayActivated");
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000CE7C File Offset: 0x0000B07C
		public void Update(float dt)
		{
			this.flashInTimeLeft = Math.Max(0f, this.flashInTimeLeft - dt);
			this.timeElapsed += dt;
			if (this.IsMonitoringDLCEndingCases)
			{
				if (this.CrashingAircraft != null)
				{
					if (this.CrashingAircraft.IsInCriticalFirmwareFailure)
					{
						this.TargetHasStartedCrashing = true;
						this.IsInPostSaveState = false;
						double totalSeconds = MediaPlayer.PlayPosition.TotalSeconds;
						double num = 131.0;
						double num2 = 1.0 / (num / 60.0);
						double num3 = num2 * 4.0;
						double num4 = num2 * 2.0;
						double num5 = (totalSeconds < 58.0) ? 999.0 : ((totalSeconds < 117.0) ? num3 : num4);
						if ((totalSeconds + num2 / 2.0) % num5 < num2 / 4.0)
						{
							this.os.warningFlash();
						}
					}
					else if (this.TargetHasStartedCrashing)
					{
						if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
						{
							RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleSaveActions.xml", this.os);
							this.os.Flags.AddFlag("DLC_PlaneSaveResponseTriggered");
							this.os.Flags.AddFlag("DLC_PlaneResult");
						}
						if (!this.CrashingAircraft.IsInCriticalDescent() && !MediaPlayer.IsRepeating)
						{
							MusicManager.FADE_TIME = 6f;
							MusicManager.transitionToSong("DLC/Music/RemiDrone");
							MediaPlayer.IsRepeating = true;
							this.os.delayer.Post(ActionDelayer.Wait(2.0), delegate
							{
								this.AircraftSaveSound.Play();
							});
							this.IsInPostSaveState = true;
						}
					}
				}
				else if (this.TargetHasStartedCrashing)
				{
					if (this.SecondaryAircraft == null || this.SecondaryAircraft.IsInCriticalFirmwareFailure)
					{
						if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
						{
							RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleDoubleCrashActions.xml", this.os);
							this.os.Flags.AddFlag("DLC_DoubleCrashResponseTriggered");
							this.os.Flags.AddFlag("DLC_PlaneResult");
						}
					}
					else if (!this.os.Flags.HasFlag("DLC_PlaneResult"))
					{
						RunnableConditionalActions.LoadIntoOS("DLC/ActionScripts/FinaleCrashActions.xml", this.os);
						this.os.Flags.AddFlag("DLC_PlaneCrashedResponseTriggered");
						this.os.Flags.AddFlag("DLC_PlaneResult");
					}
					if (MusicManager.currentSongName != "DLC\\Music\\CrashTrack")
					{
						MusicManager.playSongImmediatley("DLC\\Music\\CrashTrack");
						MediaPlayer.IsRepeating = false;
					}
				}
				if (!MediaPlayer.IsRepeating && MediaPlayer.State != MediaState.Playing)
				{
					if (!this.IsInPostSaveState)
					{
						MusicManager.FADE_TIME = 6f;
						MissionFunctions.runCommand(7, "changeSongDLC");
						MediaPlayer.IsRepeating = true;
					}
				}
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000D1FC File Offset: 0x0000B3FC
		public void Draw(Rectangle dest, SpriteBatch sb)
		{
			if (this.IsActive)
			{
				bool flag = Utils.randm(1f) >= this.flashInTimeLeft;
				if (flag)
				{
					if (this.CrashingAircraft != null)
					{
						AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, (int)this.CrashingAircraft.CurrentAltitude, this.CrashingAircraft.IsInCriticalDescent(), AircraftAltitudeIndicator.GetFlashRateFromTimer(this.os.timer), 50000, 40000, 30000, 14000, 3000);
					}
					else
					{
						AircraftAltitudeIndicator.RenderAltitudeIndicator(dest, sb, 0, true, AircraftAltitudeIndicator.GetFlashRateFromTimer(this.os.timer), 50000, 40000, 30000, 14000, 3000);
					}
				}
			}
		}

		// Token: 0x040000BA RID: 186
		private AircraftDaemon CrashingAircraft;

		// Token: 0x040000BB RID: 187
		private AircraftDaemon SecondaryAircraft;

		// Token: 0x040000BC RID: 188
		public bool IsActive = false;

		// Token: 0x040000BD RID: 189
		private float timeElapsed = 0f;

		// Token: 0x040000BE RID: 190
		private float flashInTimeLeft = 1f;

		// Token: 0x040000BF RID: 191
		private OS os;

		// Token: 0x040000C0 RID: 192
		public bool IsMonitoringDLCEndingCases = false;

		// Token: 0x040000C1 RID: 193
		private bool TargetHasStartedCrashing = false;

		// Token: 0x040000C2 RID: 194
		private bool IsInPostSaveState = false;

		// Token: 0x040000C3 RID: 195
		private SoundEffect AircraftSaveSound;
	}
}
