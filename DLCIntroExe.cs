using System;
using System.Collections.Generic;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x0200005F RID: 95
	internal class DLCIntroExe : ExeModule
	{
		// Token: 0x060001CB RID: 459 RVA: 0x00018574 File Offset: 0x00016774
		public DLCIntroExe(Rectangle location, OS operatingSystem, string[] p) : base(location, operatingSystem)
		{
			this.name = "KaguyaTrial";
			this.ramCost = 190;
			this.IdentifierName = "KaguyaTrial";
			this.targetIP = this.os.thisComputer.ip;
			this.circle = this.os.content.Load<Texture2D>("Circle");
			this.circleOutline = this.os.content.Load<Texture2D>("CircleOutlineLarge");
			this.Assignment1Text = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial1.txt");
			this.Assignment2Text = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrial2.txt");
			this.AssignmentsCompleteText = Utils.readEntireFile("Content/DLC/Docs/KaguyaTrialComplete.txt");
			this.BackgroundEffect = new HexGridBackground(this.os.content);
			this.explosion.Init(this.os.content);
			this.GlowSound = this.os.content.Load<SoundEffect>("SFX/Ending/PorthackSpindown");
			this.BreakSound = this.os.content.Load<SoundEffect>("SFX/DoomShock");
			if (MusicManager.currentSongName != "DLC\\Music\\snidelyWhiplash")
			{
				if (!this.os.Flags.HasFlag("KaguyaTrialComplete"))
				{
					MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
				}
			}
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00018880 File Offset: 0x00016A80
		public override void Killed()
		{
			base.Killed();
			if (this.State != DLCIntroExe.IntroState.NotStarted && this.State != DLCIntroExe.IntroState.Exiting)
			{
				this.os.delayer.Post(ActionDelayer.NextTick(), delegate
				{
					DLCIntroExe dlcintroExe = new DLCIntroExe(this.bounds, this.os, new string[0])
					{
						State = this.State,
						TimeInThisState = this.TimeInThisState,
						LoadedMission = this.LoadedMission,
						IsOnAssignment1 = this.IsOnAssignment1,
						AllAssignmentsComplete = this.AllAssignmentsComplete,
						MissionIsComplete = this.MissionIsComplete,
						PhaseTitle = this.PhaseTitle,
						PhaseSubtitle = this.PhaseSubtitle
					};
					if (this.OSTraceTimerOverrideActive)
					{
						this.os.traceCompleteOverrideAction = null;
						OS os = this.os;
						os.traceCompleteOverrideAction = (Action)Delegate.Combine(os.traceCompleteOverrideAction, new Action(dlcintroExe.PlayerLostToTraceTimer));
					}
					this.os.addExe(dlcintroExe);
				});
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x000188E0 File Offset: 0x00016AE0
		private void UpdateState(float t)
		{
			this.TimeInThisState += t;
			switch (this.State)
			{
			case DLCIntroExe.IntroState.SpinningUp:
				if (this.TimeInThisState >= DLCIntroExe.SpinUpTime)
				{
					this.PrepareForUIBreakdown();
					this.State = DLCIntroExe.IntroState.Flickering;
					this.PhaseTitle = LocaleTerms.Loc("INITIALIZING");
					this.PhaseSubtitle = "---";
					this.TimeInThisState = 0f;
					this.os.execute("dc");
					this.os.execute("clear");
					this.os.warningFlash();
				}
				this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.SpinUpTime;
				break;
			case DLCIntroExe.IntroState.Flickering:
				if (this.TimeInThisState >= DLCIntroExe.FlickeringTime)
				{
					this.State = DLCIntroExe.IntroState.MailIconPhasingOut;
					this.TimeInThisState = 0f;
					SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed, 100f);
					this.os.execute("clear");
					this.os.warningFlash();
				}
				this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.FlickeringTime;
				break;
			case DLCIntroExe.IntroState.MailIconPhasingOut:
				if (this.TimeInThisState >= DLCIntroExe.MailIconFlickerOutTime)
				{
					this.CompleteMailPhaseOut();
					this.State = DLCIntroExe.IntroState.AssignMission1;
					this.TimeInThisState = 0f;
					this.os.warningFlash();
				}
				this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.MailIconFlickerOutTime;
				break;
			case DLCIntroExe.IntroState.AssignMission1:
				this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.AssignMission1Time;
				break;
			case DLCIntroExe.IntroState.OnMission1:
			case DLCIntroExe.IntroState.OnMission2:
				this.CheckProgressOfCurrentAssignment();
				break;
			case DLCIntroExe.IntroState.AssignMission2:
				this.percentageThroughThisState = this.TimeInThisState / DLCIntroExe.AssignMission2Time;
				break;
			}
			this.explosion.Update(t);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00018B10 File Offset: 0x00016D10
		private void PlayerLostToTraceTimer()
		{
			this.os.runCommand("dc");
			this.os.runCommand("clear");
			this.os.delayer.Post(ActionDelayer.NextTick(), delegate
			{
				this.os.write("-----------------\n\nKT_OVERRIDE_RECOVERY: " + LocaleTerms.Loc("Critical Error - resetting.") + "\n\n----------------");
				this.State = DLCIntroExe.IntroState.AssignMission1;
				this.TimeInThisState = 0f;
			});
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00018B64 File Offset: 0x00016D64
		private void CompleteExecution()
		{
			this.PhaseTitle = LocaleTerms.Loc("COMPLETE");
			this.PhaseSubtitle = "---";
			this.State = DLCIntroExe.IntroState.Exiting;
			this.isExiting = true;
			this.os.traceCompleteOverrideAction = null;
			if (!this.os.Flags.HasFlag("KaguyaTrialComplete"))
			{
				this.os.Flags.AddFlag("KaguyaTrialComplete");
				this.os.allFactions.setCurrentFaction("Bibliotheque", this.os);
				this.os.homeNodeID = "dhs";
				this.os.homeAssetServerID = "dhsDrop";
				MissionFunctions.runCommand(1, "addRankSilent");
				this.os.currentMission = null;
				Computer computer = Programs.getComputer(this.os, "dhs");
				DLCHubServer dlchubServer = computer.getDaemon(typeof(DLCHubServer)) as DLCHubServer;
				dlchubServer.AddAgent(this.os.defaultUser.name, "dnkA19ds", new Color(222, 153, 24));
				for (int i = 0; i < computer.users.Count; i++)
				{
					UserDetail value = computer.users[i];
					if (value.name == this.os.defaultUser.name)
					{
						value.known = true;
						computer.users[i] = value;
					}
				}
			}
			this.os.runCommand("connect 69.58.186.114");
			this.os.IsInDLCMode = true;
			this.os.DisableEmailIcon = false;
			this.isExiting = true;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00018D20 File Offset: 0x00016F20
		private void UpdateMailePhaseOut(float t)
		{
			this.AddRadialMailLine();
			this.AddRadialMailLine();
			if (this.percentageThroughThisState > 0.2f)
			{
				this.AddRadialMailLine();
				this.AddRadialMailLine();
			}
			if (this.TimeInThisState % 0.6f <= t)
			{
				SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed, 100f + 200f * this.percentageThroughThisState);
			}
			this.os.topBarIconsColor = ((Utils.randm(1f) < this.percentageThroughThisState) ? Color.Red : this.originalTopBarIconsColor);
			Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * (0.8f * this.percentageThroughThisState));
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00018ED0 File Offset: 0x000170D0
		private void CompleteMailPhaseOut()
		{
			this.os.DisableEmailIcon = true;
			SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed * 0.8f, 100f);
			for (int i = 0; i < 12; i++)
			{
				double time = (double)i / 7.0;
				this.os.delayer.Post(ActionDelayer.Wait(time), delegate
				{
					SFX.addCircle(this.os.mailicon.pos + new Vector2(20f, 6f), Utils.AddativeRed * 0.8f, 400f);
				});
			}
			Vector2 mailIconPos = this.os.mailicon.pos;
			this.explosion.Explode(1500, new Vector2(-0.1f, 3.2415926f), mailIconPos, 1f, 8f, 100f, 1600f, 1000f, 1200f, 3f, 7f);
			this.os.delayer.Post(ActionDelayer.Wait(0.1), delegate
			{
				this.explosion.Explode(100, new Vector2(-0.1f, 3.2415926f), mailIconPos, 1f, 6f, 100f, 1300f, 1000f, 1300f, 3f, 7f);
			});
			this.BreakSound.Play();
			this.os.topBarIconsColor = this.originalTopBarIconsColor;
			PostProcessor.EndingSequenceFlashOutActive = false;
			PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
			this.os.terminal.reset();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00019054 File Offset: 0x00017254
		private void PrepareForUIBreakdown()
		{
			this.originalTheme = ThemeManager.currentTheme;
			ThemeManager.setThemeOnComputer(this.os.thisComputer, "DLC/Themes/MiamiTheme.xml");
			ThemeManager.switchTheme(this.os, "DLC/Themes/MiamiTheme.xml");
			this.timeBetweenNodeRemovals = DLCIntroExe.FlickeringTime / 2f / (float)this.os.netMap.visibleNodes.Count;
			this.os.netMap.CleanVisibleListofDuplicates();
			this.originalTopBarIconsColor = this.os.topBarIconsColor;
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x000190E0 File Offset: 0x000172E0
		private void AddRadialMailLine()
		{
			SFX.AddRadialLine(this.os.mailicon.pos + new Vector2(20f, 10f), (float)(3.141592653589793 + (double)Utils.rand(3.1415927f)), 600f + Utils.randm(300f), 800f, 500f, 200f + Utils.randm(400f), 0.35f, Color.Lerp(Utils.makeColor(100, 0, 0, byte.MaxValue), Utils.AddativeRed, Utils.randm(1f)), 3f, false);
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00019188 File Offset: 0x00017388
		private void UpdateUIFlickerIn()
		{
			float num = 0.4f;
			float num2 = Math.Min(1f, this.percentageThroughThisState * (1f / num));
			OSTheme theme = OSTheme.Custom;
			if (this.percentageThroughThisState < 0.99f)
			{
				if (Utils.randm(1f) < num2)
				{
					ThemeManager.switchTheme(this.os, theme);
				}
				else
				{
					ThemeManager.switchThemeColors(this.os, this.originalTheme);
					ThemeManager.loadThemeBackground(this.os, this.originalTheme);
					ThemeManager.currentTheme = this.originalTheme;
				}
			}
			PostProcessor.EndingSequenceFlashOutActive = true;
			PostProcessor.EndingSequenceFlashOutPercentageComplete = 1f - num2;
			if (this.percentageThroughThisState > 0.7f)
			{
				this.AddRadialMailLine();
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00019254 File Offset: 0x00017454
		private void UpdateUIBreaking(float t)
		{
			float num = 0.6f;
			float num2 = Math.Max(0f, Math.Min(1f, (this.percentageThroughThisState - (1f - num)) * (1f / num)));
			num2 = this.percentageThroughThisState;
			if (num2 > 0f)
			{
				if (this.os.netMap.visibleNodes.Count > 1)
				{
					this.timeSinceNodeRemoved += t;
					if (this.timeSinceNodeRemoved > this.timeBetweenNodeRemovals)
					{
						this.timeSinceNodeRemoved -= this.timeBetweenNodeRemovals;
						int index;
						Computer computer;
						do
						{
							index = Utils.random.Next(this.os.netMap.visibleNodes.Count);
							computer = this.os.netMap.nodes[this.os.netMap.visibleNodes[index]];
						}
						while (computer == this.os.thisComputer);
						Vector2 screenSpacePosition = computer.getScreenSpacePosition();
						OS os = this.os;
						os.PreDLCVisibleNodesCache = os.PreDLCVisibleNodesCache + ((this.os.PreDLCVisibleNodesCache.Length > 0) ? "," : "") + this.os.netMap.visibleNodes[index];
						this.os.netMap.nodes[this.os.netMap.visibleNodes[index]].adminIP = computer.ip;
						this.os.netMap.visibleNodes.RemoveAt(index);
						this.ImpactEffects.Add(new TraceKillExe.PointImpactEffect
						{
							location = screenSpacePosition,
							scaleModifier = 3f + ((computer.securityLevel > 2) ? 1f : 0f),
							cne = new ConnectedNodeEffect(this.os, true),
							timeEnabled = 0f,
							HasHighlightCircle = true
						});
						if (computer.securityLevel > 3)
						{
							int num3 = 0;
							while (num3 < computer.securityLevel && num3 < 6)
							{
								this.ImpactEffects.Add(new TraceKillExe.PointImpactEffect
								{
									location = screenSpacePosition,
									scaleModifier = (float)Math.Min(8, num3),
									cne = new ConnectedNodeEffect(this.os, true),
									timeEnabled = 0f,
									HasHighlightCircle = false
								});
								num3++;
							}
						}
					}
				}
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00019518 File Offset: 0x00017718
		private void DrawAssignmentPhase(float t)
		{
			Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * 0.8f);
			float num = 1f - Math.Min(2f, this.TimeInThisState / 2f) / 2f;
			num = Utils.CubicInCurve(num);
			float num2 = 200f;
			Rectangle rectangle = Utils.InsetRectangle(this.os.terminal.bounds, (int)(-1f * num2 * num));
			float num3 = 1f - num;
			if (num3 >= 0.8f)
			{
				num3 = (1f - (num3 - 0.8f) * 5f) * 0.8f;
			}
			RenderedRectangle.doRectangleOutline(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, (int)(60f * (0.06f + num)), new Color?(this.os.highlightColor * num3));
			string text = this.AssignmentsCompleteText;
			if (!this.AllAssignmentsComplete)
			{
				text = (this.IsOnAssignment1 ? this.Assignment1Text : this.Assignment2Text);
			}
			this.charsRenderedSoFar = TextWriterTimed.WriteTextToTerminal(text, this.os, 0.04f, 1f, 20f, this.TimeInThisState, this.charsRenderedSoFar);
			if (this.charsRenderedSoFar >= text.Length)
			{
				this.StartAssignment();
			}
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001969C File Offset: 0x0001789C
		private void StartAssignment()
		{
			if (!this.AllAssignmentsComplete)
			{
				if (this.IsOnAssignment1)
				{
					this.PhaseTitle = "74.125.23.121";
					this.PhaseSubtitle = LocaleTerms.Loc("Download Tools") + "\n" + LocaleTerms.Loc("Delete System Files");
				}
				else
				{
					this.PhaseTitle = "216.239.32.181";
					this.PhaseSubtitle = LocaleTerms.Loc("Adapt") + "\n" + LocaleTerms.Loc("Advance");
				}
				this.State = (this.IsOnAssignment1 ? DLCIntroExe.IntroState.OnMission1 : DLCIntroExe.IntroState.OnMission2);
				this.TimeInThisState = 0f;
				this.LoadedMission = (ActiveMission)ComputerLoader.readMission(this.IsOnAssignment1 ? this.assignment1MissionPath : this.assignment2MissionPath);
			}
			else
			{
				this.CompleteExecution();
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0001977C File Offset: 0x0001797C
		private void CheckProgressOfCurrentAssignment()
		{
			if (this.MissionIsComplete)
			{
				if (this.TimeInThisState >= DLCIntroExe.WindDownTimeAfterCompletingMission)
				{
					this.MissionWasCompleted();
				}
			}
			else if (this.os.connectedComp == null && this.LoadedMission.isComplete(null))
			{
				this.TimeInThisState = 0f;
				this.MissionIsComplete = true;
			}
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x000197F0 File Offset: 0x000179F0
		private void MissionWasCompleted()
		{
			if (this.IsOnAssignment1)
			{
				this.State = DLCIntroExe.IntroState.AssignMission2;
			}
			else
			{
				this.State = DLCIntroExe.IntroState.Outro;
				this.AllAssignmentsComplete = true;
				this.GlowSound.Play();
				MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
				for (int i = 0; i < this.os.exes.Count; i++)
				{
					if (this.os.exes[i] is ShellExe)
					{
						(this.os.exes[i] as ShellExe).Killed();
						(this.os.exes[i] as ShellExe).isExiting = true;
					}
				}
			}
			this.charsRenderedSoFar = 0;
			this.TimeInThisState = 0f;
			this.MissionIsComplete = false;
			if (this.IsOnAssignment1)
			{
				this.IsOnAssignment1 = false;
			}
			for (int i = 0; i < this.os.exes.Count; i++)
			{
				if (this.os.exes[i] != this)
				{
					this.os.exes[i].isExiting = true;
				}
			}
			this.os.execute("clear");
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0001994C File Offset: 0x00017B4C
		public override void Draw(float t)
		{
			base.Draw(t);
			this.drawOutline();
			this.drawTarget("app:");
			this.UpdateState(t);
			Rectangle dest = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 2, this.bounds.Width - 4, this.bounds.Height - (Module.PANEL_HEIGHT + 4));
			Rectangle dest2 = new Rectangle(this.bounds.X + 2, this.bounds.Y + Module.PANEL_HEIGHT + 10, this.bounds.Width - 4, this.bounds.Height - (Module.PANEL_HEIGHT + 6));
			switch (this.State)
			{
			case DLCIntroExe.IntroState.NotStarted:
			{
				this.BackgroundEffect.Update(t);
				this.BackgroundEffect.Draw(dest2, this.spriteBatch, Color.Black, this.themeColor * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
				int num = 30;
				this.spriteBatch.Draw(Utils.white, new Rectangle(this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - num / 2, this.bounds.Width - 20, num), Color.Black);
				if (!this.os.Flags.HasFlag("KaguyaTrialComplete"))
				{
					if (Button.doButton(8310101 + this.PID, this.bounds.X + 10, this.bounds.Y + this.bounds.Height / 2 - num / 2, this.bounds.Width - 20, num, LocaleTerms.Loc("BEGIN TRIAL"), new Color?(this.os.highlightColor)))
					{
						this.State = DLCIntroExe.IntroState.SpinningUp;
						this.TimeInThisState = 0f;
						MusicManager.stop();
						MusicManager.playSongImmediatley("DLC\\Music\\snidelyWhiplash");
						this.os.mailicon.isEnabled = false;
						this.os.thisComputer.links.Clear();
						OS os = this.os;
						os.traceCompleteOverrideAction = (Action)Delegate.Combine(os.traceCompleteOverrideAction, new Action(this.PlayerLostToTraceTimer));
						this.OSTraceTimerOverrideActive = true;
					}
				}
				else
				{
					TextItem.doCenteredFontLabel(dest, LocaleTerms.Loc("Trials Locked"), GuiData.font, Color.White, false);
					if (Button.doButton(8310101 + this.PID, dest.X + 10, dest.Y + dest.Height - 22, dest.Width - 20, 18, "Exit", new Color?(this.os.lockedColor)))
					{
						this.isExiting = true;
					}
				}
				break;
			}
			case DLCIntroExe.IntroState.SpinningUp:
			{
				Utils.LCG.reSeed(this.PID);
				Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y, dest.Width, 1);
				for (int i = 0; i < dest.Height; i++)
				{
					float num2 = Utils.LCG.NextFloatScaled() * DLCIntroExe.SpinUpTime;
					float num3 = Math.Min(1f, this.TimeInThisState / num2);
					if (Utils.LCG.NextFloatScaled() > 0.5f)
					{
						float num4 = 0.8f;
						float num5 = num3 * (1f - num4);
						if (num3 > num4)
						{
							float num6 = 1f - num5;
							float num7 = (num3 - num4) / (1f - num4);
							num7 = Utils.QuadraticOutCurve(num7);
							num3 = num5 + num6 * num7;
						}
						else
						{
							num3 = num5;
						}
					}
					else
					{
						num3 = Utils.QuadraticOutCurve(num3);
					}
					destinationRectangle.Y = dest.Y + i;
					destinationRectangle.Width = (int)(num3 * (float)dest.Width);
					Color color = Color.Lerp(Utils.AddativeWhite * 0.1f, this.themeColor, Utils.LCG.NextFloatScaled());
					this.spriteBatch.Draw(Utils.white, destinationRectangle, color);
				}
				break;
			}
			case DLCIntroExe.IntroState.Flickering:
				this.UpdateUIFlickerIn();
				this.UpdateUIBreaking(t);
				this.DrawPhaseTitle(t, dest2);
				break;
			case DLCIntroExe.IntroState.MailIconPhasingOut:
				this.DrawPhaseTitle(t, dest2);
				this.UpdateMailePhaseOut(t);
				break;
			case DLCIntroExe.IntroState.AssignMission1:
			case DLCIntroExe.IntroState.AssignMission2:
			case DLCIntroExe.IntroState.Outro:
				this.DrawPhaseTitle(t, dest2);
				this.DrawAssignmentPhase(t);
				break;
			case DLCIntroExe.IntroState.OnMission1:
			case DLCIntroExe.IntroState.OnMission2:
				this.DrawPhaseTitle(t, dest2);
				if (Settings.forceCompleteEnabled)
				{
					int num8 = 19;
					if (Button.doButton(8310102, this.bounds.X + 10, this.bounds.Y + num8 + 4, this.bounds.Width - 20, num8, LocaleTerms.Loc("DEBUG: Skip"), new Color?(this.os.highlightColor)))
					{
						FileEntry item = new FileEntry(PortExploits.crackExeData[6881], PortExploits.cracks[6881]);
						this.os.thisComputer.files.root.searchForFolder("bin").files.Add(item);
						this.CompleteExecution();
					}
				}
				break;
			case DLCIntroExe.IntroState.Exiting:
				this.DrawPhaseTitle(t, dest2);
				Utils.FillEverywhereExcept(Utils.InsetRectangle(this.os.terminal.Bounds, 1), Utils.GetFullscreen(), this.spriteBatch, Color.Black * 0.8f * (1f - Math.Min(1f, this.TimeInThisState)));
				break;
			}
			this.UpdateImpactEffects(t);
			this.DrawImpactEffects(this.ImpactEffects);
			this.explosion.Render(this.spriteBatch);
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00019F70 File Offset: 0x00018170
		private void DrawPhaseTitle(float t, Rectangle dest)
		{
			this.BackgroundEffect.Update(t);
			this.BackgroundEffect.Draw(dest, this.spriteBatch, Color.Black, this.themeColor * 0.2f, HexGridBackground.ColoringAlgorithm.CorrectedSinWash, 0f);
			int num = 40;
			if (dest.Height > num)
			{
				Rectangle rectangle = new Rectangle(dest.X, dest.Y + (int)((double)dest.Height / 2.8) - num / 2, dest.Width, num);
				this.spriteBatch.Draw(Utils.white, rectangle, Color.Black * 0.6f);
				TextItem.doFontLabelToSize(rectangle, this.PhaseTitle, Utils.GetTitleFontForLocalizedString(this.PhaseTitle), Color.White, true, false);
				if (!this.isExiting)
				{
					string[] array = this.PhaseSubtitle.Split(Utils.newlineDelim);
					Rectangle rectangle2 = new Rectangle(rectangle.X, rectangle.Y + rectangle.Height + 2, rectangle.Width, 22);
					for (int i = 0; i < array.Length; i++)
					{
						this.spriteBatch.Draw(Utils.white, rectangle2, Color.Black * 0.4f);
						TextItem.doFontLabelToSize(rectangle2, array[i], GuiData.UISmallfont, Utils.AddativeWhite * 0.9f, true, false);
						rectangle2.Y += rectangle2.Height + 2;
					}
				}
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0001A108 File Offset: 0x00018308
		private void UpdateImpactEffects(float t)
		{
			for (int i = 0; i < this.ImpactEffects.Count; i++)
			{
				TraceKillExe.PointImpactEffect value = this.ImpactEffects[i];
				value.timeEnabled += t;
				if (value.timeEnabled > DLCIntroExe.NodeImpactEffectTransInTime + DLCIntroExe.NodeImpactEffectTransOutTime)
				{
					this.ImpactEffects.RemoveAt(i);
					i--;
				}
				else
				{
					this.ImpactEffects[i] = value;
				}
			}
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0001A190 File Offset: 0x00018390
		private void DrawImpactEffects(List<TraceKillExe.PointImpactEffect> Effects)
		{
			Utils.LCG.reSeed(this.PID);
			for (int i = 0; i < Effects.Count; i++)
			{
				Color value = Color.Lerp(Utils.AddativeWhite, Utils.AddativeRed, 0.6f + 0.4f * Utils.LCG.NextFloatScaled()) * (0.6f + 0.4f * Utils.LCG.NextFloatScaled());
				TraceKillExe.PointImpactEffect pointImpactEffect = Effects[i];
				Vector2 location = pointImpactEffect.location;
				float num = Utils.QuadraticOutCurve(pointImpactEffect.timeEnabled / DLCIntroExe.NodeImpactEffectTransInTime);
				float num2 = Utils.QuadraticOutCurve(Utils.QuadraticOutCurve(pointImpactEffect.timeEnabled / (DLCIntroExe.NodeImpactEffectTransInTime + DLCIntroExe.NodeImpactEffectTransOutTime)));
				float num3 = Utils.QuadraticOutCurve((pointImpactEffect.timeEnabled - DLCIntroExe.NodeImpactEffectTransInTime) / DLCIntroExe.NodeImpactEffectTransOutTime);
				pointImpactEffect.cne.color = value * num;
				pointImpactEffect.cne.ScaleFactor = num2 * pointImpactEffect.scaleModifier;
				if (pointImpactEffect.timeEnabled > DLCIntroExe.NodeImpactEffectTransInTime)
				{
					pointImpactEffect.cne.color = value * (1f - num3);
				}
				if (num >= 0f && pointImpactEffect.HasHighlightCircle)
				{
					this.spriteBatch.Draw(this.circle, location, null, value * (1f - num - ((num3 >= 0f) ? (1f - num3) : 0f)), 0f, new Vector2((float)(this.circle.Width / 2), (float)(this.circle.Height / 2)), num / (float)this.circle.Width * 60f, SpriteEffects.None, 0.7f);
				}
				pointImpactEffect.cne.draw(this.spriteBatch, location);
			}
		}

		// Token: 0x040001E0 RID: 480
		private const bool combineNodeExplodeAndMailBurst = false;

		// Token: 0x040001E1 RID: 481
		private static float SpinUpTime = 13.8f;

		// Token: 0x040001E2 RID: 482
		private static float FlickeringTime = 10f;

		// Token: 0x040001E3 RID: 483
		private static float MailIconFlickerOutTime = 3.82f;

		// Token: 0x040001E4 RID: 484
		private static float AssignMission1Time = 16f;

		// Token: 0x040001E5 RID: 485
		private static float AssignMission2Time = 16f;

		// Token: 0x040001E6 RID: 486
		private static float WindDownTimeAfterCompletingMission = 0.8f;

		// Token: 0x040001E7 RID: 487
		private static float NodeImpactEffectTransOutTime = 3f;

		// Token: 0x040001E8 RID: 488
		private static float NodeImpactEffectTransInTime = 2f;

		// Token: 0x040001E9 RID: 489
		private Color themeColor = new Color(38, 201, 155, 220);

		// Token: 0x040001EA RID: 490
		private DLCIntroExe.IntroState State = DLCIntroExe.IntroState.NotStarted;

		// Token: 0x040001EB RID: 491
		private float TimeInThisState = 0f;

		// Token: 0x040001EC RID: 492
		private float percentageThroughThisState = 0f;

		// Token: 0x040001ED RID: 493
		private Texture2D circle;

		// Token: 0x040001EE RID: 494
		private Texture2D circleOutline;

		// Token: 0x040001EF RID: 495
		private OSTheme originalTheme = OSTheme.HacknetBlue;

		// Token: 0x040001F0 RID: 496
		private List<TraceKillExe.PointImpactEffect> ImpactEffects = new List<TraceKillExe.PointImpactEffect>();

		// Token: 0x040001F1 RID: 497
		private float timeBetweenNodeRemovals = 1f;

		// Token: 0x040001F2 RID: 498
		private float timeSinceNodeRemoved = 0f;

		// Token: 0x040001F3 RID: 499
		private Color originalTopBarIconsColor;

		// Token: 0x040001F4 RID: 500
		private string assignment1MissionPath = "Content/DLC/Missions/Intro/KaguyaTrialMission1.xml";

		// Token: 0x040001F5 RID: 501
		private string assignment2MissionPath = "Content/DLC/Missions/Intro/KaguyaTrialMission2.xml";

		// Token: 0x040001F6 RID: 502
		private string Assignment1Text = "";

		// Token: 0x040001F7 RID: 503
		private string Assignment2Text = "";

		// Token: 0x040001F8 RID: 504
		private string AssignmentsCompleteText = "";

		// Token: 0x040001F9 RID: 505
		private int charsRenderedSoFar = 0;

		// Token: 0x040001FA RID: 506
		private ActiveMission LoadedMission;

		// Token: 0x040001FB RID: 507
		private bool IsOnAssignment1 = true;

		// Token: 0x040001FC RID: 508
		private bool AllAssignmentsComplete = false;

		// Token: 0x040001FD RID: 509
		private bool MissionIsComplete = false;

		// Token: 0x040001FE RID: 510
		private string PhaseTitle = "";

		// Token: 0x040001FF RID: 511
		private string PhaseSubtitle = "";

		// Token: 0x04000200 RID: 512
		private HexGridBackground BackgroundEffect;

		// Token: 0x04000201 RID: 513
		private bool OSTraceTimerOverrideActive = false;

		// Token: 0x04000202 RID: 514
		private SoundEffect GlowSound;

		// Token: 0x04000203 RID: 515
		private SoundEffect BreakSound;

		// Token: 0x04000204 RID: 516
		private ExplodingUIElementEffect explosion = new ExplodingUIElementEffect();

		// Token: 0x02000060 RID: 96
		private enum IntroState
		{
			// Token: 0x04000206 RID: 518
			NotStarted,
			// Token: 0x04000207 RID: 519
			SpinningUp,
			// Token: 0x04000208 RID: 520
			Flickering,
			// Token: 0x04000209 RID: 521
			MailIconPhasingOut,
			// Token: 0x0400020A RID: 522
			AssignMission1,
			// Token: 0x0400020B RID: 523
			OnMission1,
			// Token: 0x0400020C RID: 524
			AssignMission2,
			// Token: 0x0400020D RID: 525
			OnMission2,
			// Token: 0x0400020E RID: 526
			Outro,
			// Token: 0x0400020F RID: 527
			Exiting
		}
	}
}
