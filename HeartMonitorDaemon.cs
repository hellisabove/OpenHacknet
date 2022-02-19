using System;
using System.Collections.Generic;
using Hacknet.Daemons.Helpers;
using Hacknet.Gui;
using Hacknet.UIUtils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000099 RID: 153
	internal class HeartMonitorDaemon : Daemon
	{
		// Token: 0x060002FE RID: 766 RVA: 0x0002AC64 File Offset: 0x00028E64
		public HeartMonitorDaemon(Computer c, OS os) : base(c, LocaleTerms.Loc("Remote Monitor"), os)
		{
			this.blufEffect = os.content.Load<Effect>("Shaders/DOFBlur");
			this.blufEffect.CurrentTechnique = this.blufEffect.Techniques["SmoothGaussBlur"];
			this.Heart = os.content.Load<Texture2D>("Sprites/Icons/Heart");
			this.OxyIcon = os.content.Load<Texture2D>("Sprites/Icons/O2Img");
			this.WarnIcon = os.content.Load<Texture2D>("Sprites/Icons/CautionIcon");
			this.beepSound = os.content.Load<SoundEffect>("SFX/HeartMonitorBeep");
			this.beepSustainSound = os.content.Load<SoundEffect>("SFX/HeartMonitorSustain").CreateInstance();
			this.beepSustainSound.IsLooped = true;
			this.beepSustainSound.Volume = this.volume;
			this.SetUpMonitors();
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0002AEAC File Offset: 0x000290AC
		public override string getSaveString()
		{
			return "<HeartMonitor patient=\"" + this.PatientID + "\" />";
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0002AED4 File Offset: 0x000290D4
		public override void initFiles()
		{
			base.initFiles();
			Folder folder = this.comp.files.root.searchForFolder("KBT_Pacemaker");
			if (folder == null)
			{
				folder = new Folder("KBT_Pacemaker");
				this.comp.files.root.folders.Add(folder);
			}
			Folder folder2 = folder.searchForFolder("Active");
			if (folder2 == null)
			{
				folder2 = new Folder("Active");
			}
			folder.folders.Add(folder2);
			FileEntry item = new FileEntry(PortExploits.ValidPacemakerFirmware, "KBT_Firmware_v1.2.dll");
			folder.files.Add(item);
			FileEntry fileEntry = new FileEntry(PortExploits.ValidPacemakerFirmware, "LiveFirmware.dll");
			folder2.files.Add(item);
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0002B478 File Offset: 0x00029678
		private void SetUpMonitors()
		{
			this.HeartMonitor = new BasicMedicalMonitor(delegate(float lastVal, float dt)
			{
				List<BasicMedicalMonitor.MonitorRecordKeypoint> list = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
				BasicMedicalMonitor.MonitorRecordKeypoint item = default(BasicMedicalMonitor.MonitorRecordKeypoint);
				item.timeOffset = dt;
				item.value = lastVal;
				float num = this.PatientDead ? 0.05f : 0.25f;
				if (lastVal > num)
				{
					item.value -= dt * 0.5f;
				}
				else if (lastVal < -1f * num)
				{
					item.value += dt * 0.5f * (this.PatientDead ? (0.5f * Math.Max(1f - this.timeDead / 16f, 0f)) : 1f);
				}
				else
				{
					item.value += (Utils.randm(0.2f) - 0.1f) * 0.3f;
				}
				list.Add(item);
				return list;
			}, delegate(float lastVal, float dt)
			{
				List<BasicMedicalMonitor.MonitorRecordKeypoint> list = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
				if (!this.PatientDead)
				{
					list.Add(new BasicMedicalMonitor.MonitorRecordKeypoint
					{
						timeOffset = dt / 3f,
						value = -0.8f + (Utils.randm(0.2f) - 0.1f)
					});
					list.Add(new BasicMedicalMonitor.MonitorRecordKeypoint
					{
						timeOffset = dt / 3f,
						value = 0.9f + (Utils.randm(0.1f) - 0.05f)
					});
					list.Add(new BasicMedicalMonitor.MonitorRecordKeypoint
					{
						timeOffset = dt / 3f,
						value = Utils.randm(0.1f * dt) - 0.05f
					});
				}
				return list;
			});
			this.Monitors.Add(this.HeartMonitor);
			this.BPMonitor = new BasicMedicalMonitor(delegate(float lastVal, float dt)
			{
				List<BasicMedicalMonitor.MonitorRecordKeypoint> list = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
				BasicMedicalMonitor.MonitorRecordKeypoint item = default(BasicMedicalMonitor.MonitorRecordKeypoint);
				item.timeOffset = dt;
				item.value = lastVal;
				if (lastVal > 0.25f)
				{
					item.value -= dt * 0.5f;
				}
				else if (lastVal < -0.25f)
				{
					item.value += dt * 0.5f;
				}
				else
				{
					item.value += (Utils.randm(0.2f) - 0.1f) * 0.3f * (this.PatientDead ? (0.5f * Math.Max(1f - this.timeDead / 16f, 0f)) : 1f);
				}
				list.Add(item);
				return list;
			}, (float lastVal, float dt) => new List<BasicMedicalMonitor.MonitorRecordKeypoint>
			{
				new BasicMedicalMonitor.MonitorRecordKeypoint
				{
					timeOffset = dt / 3f,
					value = -0.8f + (Utils.randm(0.2f) - 0.1f)
				},
				new BasicMedicalMonitor.MonitorRecordKeypoint
				{
					timeOffset = dt / 3f,
					value = 0.9f + (Utils.randm(0.1f) - 0.05f)
				},
				new BasicMedicalMonitor.MonitorRecordKeypoint
				{
					timeOffset = dt / 3f,
					value = Utils.randm(0.1f * dt) - 0.05f
				}
			});
			this.Monitors.Add(this.BPMonitor);
			this.SPMonitor = new BasicMedicalMonitor(delegate(float lastVal, float dt)
			{
				List<BasicMedicalMonitor.MonitorRecordKeypoint> list = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
				BasicMedicalMonitor.MonitorRecordKeypoint item = default(BasicMedicalMonitor.MonitorRecordKeypoint);
				item.timeOffset = dt;
				item.value = lastVal;
				if (lastVal > 0.8f)
				{
					item.value += (Utils.randm(0.2f) - 0.1f) * 0.3f;
				}
				else
				{
					item.value += 0.15f * (float)(Utils.random.NextDouble() * Utils.random.NextDouble()) * (this.PatientDead ? (0.5f * Math.Max(1f - this.timeDead / 16f, 0f)) : 1f);
				}
				list.Add(item);
				return list;
			}, delegate(float lastVal, float dt)
			{
				List<BasicMedicalMonitor.MonitorRecordKeypoint> list = new List<BasicMedicalMonitor.MonitorRecordKeypoint>();
				BasicMedicalMonitor.MonitorRecordKeypoint item = new BasicMedicalMonitor.MonitorRecordKeypoint
				{
					timeOffset = dt * 0.7f,
					value = -0.6f - Utils.randm(0.4f)
				};
				list.Add(item);
				list.Add(new BasicMedicalMonitor.MonitorRecordKeypoint
				{
					timeOffset = dt * 0.3f,
					value = item.value - Utils.randm(0.15f) - 0.05f
				});
				return list;
			});
			this.Monitors.Add(this.SPMonitor);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0002B54C File Offset: 0x0002974C
		private void UpdateReports(float dt)
		{
			this.timeSinceLastHeartBeat += dt;
			this.currentSPO2 = 1f - this.SPMonitor.GetCurrentValue(this.projectionFowardsTime);
			this.averageSPO2 = this.averageSPO2 * 0.96f + this.currentSPO2 * 0.04f;
			this.reportedSP02 = Math.Min(100, 90 + (int)(5f * this.averageSPO2 + 0.5f));
			if (this.HeartRate > 120 || this.HeartRate < 50)
			{
				this.timeSinceNormalHeartRate += dt;
			}
			else
			{
				this.alarmHeartOKTimer += dt;
				if (this.alarmHeartOKTimer > 10f)
				{
					this.timeSinceNormalHeartRate = 0f;
					this.alarmHeartOKTimer = 0f;
				}
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0002B632 File Offset: 0x00029832
		private void UpdateReportsForHeartbeat()
		{
			this.HeartRate = (int)(60f / this.timeSinceLastHeartBeat + 0.5f);
			this.timeSinceLastHeartBeat = 0f;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0002B65C File Offset: 0x0002985C
		private void ChangeState(HeartMonitorDaemon.HeartMonitorState newState)
		{
			if (this.State != HeartMonitorDaemon.HeartMonitorState.MainDisplay || newState != HeartMonitorDaemon.HeartMonitorState.MainDisplay)
			{
				if (newState != HeartMonitorDaemon.HeartMonitorState.MainDisplay)
				{
					if (this.State == HeartMonitorDaemon.HeartMonitorState.MainDisplay)
					{
						this.opOpening = true;
						this.opTransition = 0f;
					}
				}
				else if (this.State != HeartMonitorDaemon.HeartMonitorState.Welcome)
				{
					this.opOpening = false;
					this.opTransition = 0f;
				}
				else
				{
					this.opOpening = false;
					this.opTransition = 2f;
				}
				this.State = newState;
				this.timeThisState = 0f;
			}
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0002B700 File Offset: 0x00029900
		public override void navigatedTo()
		{
			base.navigatedTo();
			this.ChangeState(HeartMonitorDaemon.HeartMonitorState.Welcome);
			if (this.os.Flags.HasFlag(this.PatientID + ":DEAD"))
			{
				this.PatientDead = true;
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0002B74C File Offset: 0x0002994C
		private void UpdateStates(float dt)
		{
			this.timeThisState += dt;
			switch (this.State)
			{
			case HeartMonitorDaemon.HeartMonitorState.MainDisplay:
				this.opTransition += (this.opOpening ? 1f : 2f) * dt;
				break;
			case HeartMonitorDaemon.HeartMonitorState.SecondaryLogin:
			case HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning:
			case HeartMonitorDaemon.HeartMonitorState.Error:
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreen:
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
				this.opTransition += dt;
				break;
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
				this.opTransition += dt;
				this.firmwareLoadTime += (float)((double)dt * Utils.random.NextDouble());
				if (this.firmwareLoadTime > 10f)
				{
					this.EnactFirmwareChange();
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete);
					this.firmwareLoadTime = 0f;
				}
				break;
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
				this.opTransition += dt;
				this.firmwareLoadTime += dt;
				if (this.firmwareLoadTime > 3.3333333f)
				{
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
				}
				break;
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x0002B870 File Offset: 0x00029A70
		public void ForceStopBeepSustainSound()
		{
			if (this.beepSustainSound != null)
			{
				this.beepSustainSound.Stop();
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x0002B908 File Offset: 0x00029B08
		private void Update(float dt)
		{
			this.os.delayer.Post(ActionDelayer.Wait(this.os.lastGameTime.ElapsedGameTime.TotalSeconds * 1.999), delegate
			{
				if (this.os.display.command != this.name)
				{
					if (this.PatientDead)
					{
						this.beepSustainSound.Stop(true);
					}
				}
			});
			if (this.PatientInCardiacArrest)
			{
				this.PatientTimeInDanger += dt;
				if (this.PatientTimeInDanger > 21f)
				{
					this.PatientDead = true;
					this.HeartRate = 0;
					this.os.Flags.AddFlag(this.PatientID + ":DEAD");
					this.beepSustainSound.Play();
				}
				else
				{
					float num = 3.5f;
					if (this.PatientTimeInDanger - dt < num && this.PatientTimeInDanger >= num)
					{
						if (this.os.currentMission.postingTitle == "Project Junebug")
						{
							MusicManager.FADE_TIME = 10f;
							MusicManager.transitionToSong("Music/Ambient/dark_drone_008");
						}
					}
				}
			}
			else
			{
				this.PatientTimeInDanger = 0f;
			}
			this.timeTillNextHeartbeat -= dt;
			if (this.timeTillNextHeartbeat <= 0f && !this.PatientDead)
			{
				this.timeTillNextHeartbeat = this.timeBetweenHeartbeats + (Utils.randm(0.1f) - 0.05f);
				if (this.PatientInCardiacArrest)
				{
					if (this.PatientTimeInDanger > 15f)
					{
						this.timeTillNextHeartbeat = 0.36f - 0.25f * (this.PatientTimeInDanger / 21f);
					}
					else
					{
						this.timeTillNextHeartbeat = Utils.randm(this.timeBetweenHeartbeats * 2f);
					}
				}
				this.projectionFowardsTime += this.beatTime + dt;
				for (int i = 0; i < this.Monitors.Count; i++)
				{
					this.Monitors[i].HeartBeat(this.beatTime);
				}
				this.UpdateReportsForHeartbeat();
				if (this.State != HeartMonitorDaemon.HeartMonitorState.Welcome)
				{
					this.os.delayer.Post(ActionDelayer.Wait((double)(this.projectionFowardsTime + this.beatTime / 4f)), delegate
					{
						this.beepSound.Play(this.volume, 0f, 0f);
					});
				}
			}
			else if (this.projectionFowardsTime > 0.3f)
			{
				this.projectionFowardsTime -= dt;
			}
			else
			{
				for (int i = 0; i < this.Monitors.Count; i++)
				{
					this.Monitors[i].Update(dt);
				}
			}
			if (this.PatientDead)
			{
				this.timeDead += dt;
				if (this.timeDead > 5f)
				{
					float num2 = 1f - (this.timeDead - 5f) / 16f;
					num2 *= this.volume;
					num2 = Math.Max(0f, num2);
					this.beepSustainSound.Volume = num2;
				}
			}
			this.UpdateStates(dt);
			this.UpdateReports(dt);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x0002BC74 File Offset: 0x00029E74
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			this.Update((float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds);
			if (this.bloomTarget == null || this.bloomTarget.Width != bounds.Width || this.bloomTarget.Height != bounds.Height)
			{
				this.bloomTarget = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height);
				this.secondaryBloomTarget = new RenderTarget2D(sb.GraphicsDevice, bounds.Width, bounds.Height);
			}
			if (this.BlurContentSpritebatch == null)
			{
				this.BlurContentSpritebatch = new SpriteBatch(sb.GraphicsDevice);
			}
			bool drawShadow = TextItem.DrawShadow;
			TextItem.DrawShadow = false;
			this.PostBloomDrawCalls.Clear();
			this.StartBloomDraw(this.BlurContentSpritebatch);
			Rectangle rectangle = bounds;
			rectangle.X = (rectangle.Y = 0);
			SpriteBatch spriteBatch = GuiData.spriteBatch;
			GuiData.spriteBatch = this.BlurContentSpritebatch;
			this.DrawStates(rectangle, this.BlurContentSpritebatch);
			GuiData.spriteBatch = spriteBatch;
			this.EndBloomDraw(bounds, rectangle, sb, this.BlurContentSpritebatch);
			for (int i = 0; i < this.PostBloomDrawCalls.Count; i++)
			{
				this.PostBloomDrawCalls[i](bounds.X, bounds.Y, sb);
			}
			TextItem.DrawShadow = drawShadow;
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0002BE00 File Offset: 0x0002A000
		private void DrawStates(Rectangle bounds, SpriteBatch sb)
		{
			switch (this.State)
			{
			case HeartMonitorDaemon.HeartMonitorState.Welcome:
				this.DrawWelcomeScreen(bounds, sb);
				goto IL_2E;
			}
			this.DrawSegments(bounds, sb);
			IL_2E:
			if (this.State != HeartMonitorDaemon.HeartMonitorState.Welcome)
			{
				this.DrawOptionsPanel(bounds, sb);
			}
		}

		// Token: 0x0600030B RID: 779 RVA: 0x0002BE54 File Offset: 0x0002A054
		private void StartBloomDraw(SpriteBatch sb)
		{
			this.priorTarget = (RenderTarget2D)sb.GraphicsDevice.GetRenderTargets()[0].RenderTarget;
			sb.GraphicsDevice.SetRenderTarget(this.bloomTarget);
			sb.GraphicsDevice.Clear(Color.Transparent);
			sb.Begin();
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0002BEB0 File Offset: 0x0002A0B0
		private void EndBloomDraw(Rectangle bounds, Rectangle zeroedBounds, SpriteBatch mainSB, SpriteBatch bloomContentSpritebatch)
		{
			bloomContentSpritebatch.End();
			mainSB.GraphicsDevice.SetRenderTarget(this.secondaryBloomTarget);
			mainSB.GraphicsDevice.Clear(Color.Transparent);
			bloomContentSpritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, this.blufEffect);
			zeroedBounds.X -= 2;
			bloomContentSpritebatch.Draw(this.bloomTarget, zeroedBounds, this.bloomColor);
			zeroedBounds.X += 4;
			bloomContentSpritebatch.Draw(this.bloomTarget, zeroedBounds, this.bloomColor);
			zeroedBounds.X -= 2;
			zeroedBounds.Y -= 2;
			bloomContentSpritebatch.Draw(this.bloomTarget, zeroedBounds, this.bloomColor);
			zeroedBounds.Y += 4;
			bloomContentSpritebatch.Draw(this.bloomTarget, zeroedBounds, this.bloomColor);
			bloomContentSpritebatch.End();
			mainSB.GraphicsDevice.SetRenderTarget(this.priorTarget);
			mainSB.Draw(this.bloomTarget, bounds, Color.White);
			mainSB.Draw(this.secondaryBloomTarget, bounds, Color.White);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0002C25C File Offset: 0x0002A45C
		public void DrawSegments(Rectangle bounds, SpriteBatch sb)
		{
			HeartMonitorDaemon.<>c__DisplayClassf CS$<>8__locals1 = new HeartMonitorDaemon.<>c__DisplayClassf();
			CS$<>8__locals1.<>4__this = this;
			int num = 26;
			Rectangle rectangle = bounds;
			rectangle.Height = bounds.Height / 4 - 4;
			this.DrawGraph(this.HeartMonitor, rectangle, this.heartColor, sb, true);
			Rectangle rectangle2 = rectangle;
			rectangle2.Y += rectangle2.Height + 2;
			CS$<>8__locals1.HRM_DisplayBounds = rectangle2;
			CS$<>8__locals1.HRM_DisplayBounds.Width = rectangle2.Width / 3;
			CS$<>8__locals1.showingHeartIcon = (this.timeSinceLastHeartBeat > this.beatTime * 1.6f && this.timeSinceLastHeartBeat < 4f * this.beatTime);
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle hrm_DisplayBounds = CS$<>8__locals1.HRM_DisplayBounds;
				hrm_DisplayBounds.X += x;
				hrm_DisplayBounds.Y += y;
				CS$<>8__locals1.<>4__this.DrawMonitorNumericalDisplay(hrm_DisplayBounds, "HR", string.Concat(CS$<>8__locals1.<>4__this.HeartRate), sprBatch, CS$<>8__locals1.<>4__this.heartColor, CS$<>8__locals1.showingHeartIcon ? CS$<>8__locals1.<>4__this.Heart : null);
			});
			CS$<>8__locals1.statusMonitorBounds = rectangle2;
			HeartMonitorDaemon.<>c__DisplayClassf CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.statusMonitorBounds.Width = CS$<>8__locals2.statusMonitorBounds.Width - (CS$<>8__locals1.HRM_DisplayBounds.Width + 8 + num);
			HeartMonitorDaemon.<>c__DisplayClassf CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.statusMonitorBounds.X = CS$<>8__locals3.statusMonitorBounds.X + (CS$<>8__locals1.HRM_DisplayBounds.Width + 4 + num);
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle statusMonitorBounds2 = CS$<>8__locals1.statusMonitorBounds;
				statusMonitorBounds2.X += x;
				statusMonitorBounds2.Y += y;
				string value = "OK";
				Color col = Color.Gray;
				Texture2D icon = null;
				if (CS$<>8__locals1.<>4__this.timeSinceNormalHeartRate > 0.5f)
				{
					if (CS$<>8__locals1.<>4__this.timeSinceNormalHeartRate > 4f)
					{
						value = "DANGER";
						col = Color.Red;
						if (CS$<>8__locals1.<>4__this.os.timer % 0.3f < 0.1f)
						{
							icon = CS$<>8__locals1.<>4__this.WarnIcon;
						}
					}
					else
					{
						value = "WARN";
						col = Color.Yellow;
						if (CS$<>8__locals1.<>4__this.os.timer % 0.5f < 0.2f)
						{
							icon = CS$<>8__locals1.<>4__this.WarnIcon;
						}
					}
				}
				CS$<>8__locals1.<>4__this.DrawMonitorStatusPanelDisplay(statusMonitorBounds2, "ALARM", value, sprBatch, col, icon);
			});
			CS$<>8__locals1.BPColor = new Color(148, 231, 243);
			CS$<>8__locals1.BP_DisplayBounds = CS$<>8__locals1.HRM_DisplayBounds;
			HeartMonitorDaemon.<>c__DisplayClassf CS$<>8__locals4 = CS$<>8__locals1;
			CS$<>8__locals4.BP_DisplayBounds.Y = CS$<>8__locals4.BP_DisplayBounds.Y + (CS$<>8__locals1.BP_DisplayBounds.Height + 4);
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle bp_DisplayBounds = CS$<>8__locals1.BP_DisplayBounds;
				bp_DisplayBounds.X += x;
				bp_DisplayBounds.Y += y;
				CS$<>8__locals1.<>4__this.DrawMonitorNumericalDisplay(bp_DisplayBounds, "BP", "133 : 97\n\n  (109)", sprBatch, CS$<>8__locals1.BPColor, null);
			});
			Rectangle statusMonitorBounds = CS$<>8__locals1.statusMonitorBounds;
			statusMonitorBounds.Y += statusMonitorBounds.Height + 4;
			this.DrawGraph(this.BPMonitor, statusMonitorBounds, CS$<>8__locals1.BPColor, sb, true);
			CS$<>8__locals1.SPColor = new Color(165, 241, 138);
			CS$<>8__locals1.SP_DisplayBounds = CS$<>8__locals1.BP_DisplayBounds;
			HeartMonitorDaemon.<>c__DisplayClassf CS$<>8__locals5 = CS$<>8__locals1;
			CS$<>8__locals5.SP_DisplayBounds.Y = CS$<>8__locals5.SP_DisplayBounds.Y + (CS$<>8__locals1.SP_DisplayBounds.Height + 4);
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle sp_DisplayBounds = CS$<>8__locals1.SP_DisplayBounds;
				sp_DisplayBounds.X += x;
				sp_DisplayBounds.Y += y;
				CS$<>8__locals1.<>4__this.DrawMonitorNumericalDisplay(sp_DisplayBounds, "Sp02", string.Concat(CS$<>8__locals1.<>4__this.reportedSP02), sprBatch, CS$<>8__locals1.SPColor, (CS$<>8__locals1.<>4__this.reportedSP02 >= 91) ? CS$<>8__locals1.<>4__this.OxyIcon : null);
			});
			Rectangle dest = statusMonitorBounds;
			dest.Y += dest.Height + 4;
			this.DrawGraph(this.SPMonitor, dest, CS$<>8__locals1.SPColor, sb, true);
		}

		// Token: 0x0600030E RID: 782 RVA: 0x0002C4C8 File Offset: 0x0002A6C8
		private void DrawMonitorNumericalDisplay(Rectangle bounds, string display, string value, SpriteBatch sb, Color col, Texture2D icon = null)
		{
			float num = 45f;
			Vector2 vector = TextItem.doMeasuredFontLabel(new Vector2((float)bounds.X + 2f, (float)bounds.Y + 2f), display, GuiData.font, new Color?(col), (float)bounds.Width - 12f, num);
			int num2 = 8;
			Rectangle destinationRectangle = new Rectangle((int)((float)bounds.X + vector.X + 9f), bounds.Y + num2, 8, (int)num - 2 * num2);
			sb.Draw(Utils.white, destinationRectangle, Color.DarkRed);
			if (icon != null)
			{
				sb.Draw(icon, new Rectangle(bounds.X + 2, (int)((float)bounds.Y + vector.Y + 2f), (int)num, (int)num), col);
			}
			TextItem.doFontLabelToSize(new Rectangle(bounds.X + bounds.Width / 3, bounds.Y + bounds.Height / 3, (int)((double)bounds.Width * 0.6), (int)((double)bounds.Height * 0.6)), value, GuiData.titlefont, col, false, false);
			Rectangle destinationRectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height + 2, bounds.Width - 8, 1);
			sb.Draw(Utils.white, destinationRectangle2, Color.Gray);
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0002C644 File Offset: 0x0002A844
		private void DrawMonitorStatusPanelDisplay(Rectangle bounds, string display, string value, SpriteBatch sb, Color col, Texture2D icon = null)
		{
			float num = 45f;
			Vector2 vector = TextItem.doMeasuredFontLabel(new Vector2((float)bounds.X + 2f, (float)bounds.Y + 2f), display, GuiData.font, new Color?(col), (float)bounds.Width - 12f, num);
			int num2 = 8;
			Rectangle destinationRectangle = new Rectangle((int)((float)bounds.X + vector.X + 9f), bounds.Y + num2, 8, (int)num - 2 * num2);
			sb.Draw(Utils.white, destinationRectangle, Color.DarkRed);
			if (icon != null)
			{
				sb.Draw(icon, new Rectangle(bounds.X + 2, (int)((float)bounds.Y + vector.Y + 2f), (int)num, (int)num), col);
			}
			TextItem.doFontLabelToSize(new Rectangle(bounds.X + (int)((double)num * 1.5), bounds.Y + bounds.Height / 3, (int)((double)bounds.Width * 0.3), (int)((double)bounds.Height * 0.3)), value, GuiData.titlefont, col, false, false);
			Rectangle destinationRectangle2 = new Rectangle(bounds.X + 4, bounds.Y + bounds.Height + 2, bounds.Width - 8, 1);
			sb.Draw(Utils.white, destinationRectangle2, Color.Gray);
		}

		// Token: 0x06000310 RID: 784 RVA: 0x0002C82C File Offset: 0x0002AA2C
		private void DrawGraph(IMedicalMonitor monitor, Rectangle dest, Color col, SpriteBatch sb, bool drawUnderline = true)
		{
			monitor.Draw(dest, sb, col, this.projectionFowardsTime);
			if (drawUnderline)
			{
				this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
				{
					Rectangle destinationRectangle = new Rectangle(dest.X + 4 + x, dest.Y + dest.Height + 2 + y, dest.Width - 8, 1);
					sprBatch.Draw(Utils.white, destinationRectangle, Color.Gray);
				});
			}
		}

		// Token: 0x06000311 RID: 785 RVA: 0x0002C88A File Offset: 0x0002AA8A
		private void EnactFirmwareChange()
		{
			this.PatientInCardiacArrest = (this.selectedFirmwareData == PortExploits.DangerousPacemakerFirmware || this.selectedFirmwareData == PortExploits.DangerousPacemakerFirmwareLRNG);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x0002CBE0 File Offset: 0x0002ADE0
		public void DrawWelcomeScreen(Rectangle bounds, SpriteBatch sb)
		{
			HeartMonitorDaemon.<>c__DisplayClass16 CS$<>8__locals1 = new HeartMonitorDaemon.<>c__DisplayClass16();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.hasAdmin = (this.comp.adminIP == this.os.thisComputer.ip);
			CS$<>8__locals1.graphColor = Color.Red;
			CS$<>8__locals1.graphColor.A = 100;
			if (CS$<>8__locals1.hasAdmin)
			{
				CS$<>8__locals1.graphColor = new Color(20, 200, 20, 100);
			}
			CS$<>8__locals1.heartRateDisplay = new Rectangle(bounds.X + 10, bounds.Y + bounds.Height / 4, bounds.Width - 20, bounds.Height / 4);
			this.DrawGraph(this.HeartMonitor, CS$<>8__locals1.heartRateDisplay, CS$<>8__locals1.graphColor, sb, false);
			HeartMonitorDaemon.<>c__DisplayClass16 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.heartRateDisplay.Y = CS$<>8__locals2.heartRateDisplay.Y + (CS$<>8__locals1.heartRateDisplay.Height + 10);
			CS$<>8__locals1.adminMsg = (CS$<>8__locals1.hasAdmin ? LocaleTerms.Loc("Admin Access Granted") : LocaleTerms.Loc("Admin Access Required"));
			CS$<>8__locals1.adminMsg = CS$<>8__locals1.adminMsg.ToUpper();
			CS$<>8__locals1.heartRateDisplay.Height = 20;
			this.DrawLinedMessage(CS$<>8__locals1.adminMsg, CS$<>8__locals1.graphColor * 0.2f, CS$<>8__locals1.heartRateDisplay, sb);
			CS$<>8__locals1.nextToButonsLineThing = new Rectangle(CS$<>8__locals1.heartRateDisplay.X + CS$<>8__locals1.heartRateDisplay.Width / 4 - 18, CS$<>8__locals1.heartRateDisplay.Y + CS$<>8__locals1.heartRateDisplay.Height + 10, 14, bounds.Height / 3);
			sb.Draw(Utils.white, CS$<>8__locals1.nextToButonsLineThing, CS$<>8__locals1.graphColor * 0.2f);
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle heartRateDisplay = CS$<>8__locals1.heartRateDisplay;
				heartRateDisplay.X += x;
				heartRateDisplay.Y += y;
				CS$<>8__locals1.<>4__this.DrawLinedMessage(CS$<>8__locals1.adminMsg, CS$<>8__locals1.graphColor, heartRateDisplay, sprBatch);
				CS$<>8__locals1.nextToButonsLineThing.X = CS$<>8__locals1.nextToButonsLineThing.X + x;
				CS$<>8__locals1.nextToButonsLineThing.Y = CS$<>8__locals1.nextToButonsLineThing.Y + y;
				sprBatch.Draw(Utils.white, CS$<>8__locals1.nextToButonsLineThing, CS$<>8__locals1.graphColor);
				heartRateDisplay.Y += heartRateDisplay.Height + 10;
				heartRateDisplay.Width /= 2;
				heartRateDisplay.X += heartRateDisplay.Width / 2;
				heartRateDisplay.Height = 28;
				heartRateDisplay.Height *= 2;
				sprBatch.DrawString(GuiData.font, CS$<>8__locals1.<>4__this.comp.name, new Vector2((float)heartRateDisplay.X, (float)heartRateDisplay.Y), Color.White);
				sprBatch.DrawString(GuiData.detailfont, "Kellis Biotech\nB-Type Pacemaker v2.44", new Vector2((float)(heartRateDisplay.X + 2), (float)(heartRateDisplay.Y + 30)), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.5f);
				heartRateDisplay.Y += heartRateDisplay.Height + 6;
				heartRateDisplay.Height /= 2;
				if (Button.doButton(686868001, heartRateDisplay.X, heartRateDisplay.Y, heartRateDisplay.Width, heartRateDisplay.Height + 6, LocaleTerms.Loc("View Monitor"), new Color?(CS$<>8__locals1.hasAdmin ? CS$<>8__locals1.<>4__this.os.highlightColor : Color.Gray)))
				{
					if (CS$<>8__locals1.hasAdmin)
					{
						CS$<>8__locals1.<>4__this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
					}
				}
				heartRateDisplay.Y += heartRateDisplay.Height + 10 + 6;
				if (Button.doButton(686868003, heartRateDisplay.X, heartRateDisplay.Y, heartRateDisplay.Width, heartRateDisplay.Height, LocaleTerms.Loc("Admin Login"), new Color?((!CS$<>8__locals1.hasAdmin) ? CS$<>8__locals1.<>4__this.os.highlightColor : Color.Gray)))
				{
					if (!CS$<>8__locals1.hasAdmin)
					{
						CS$<>8__locals1.<>4__this.os.runCommand("login");
					}
				}
				heartRateDisplay.Y += heartRateDisplay.Height + 10;
				if (Button.doButton(686868005, heartRateDisplay.X, heartRateDisplay.Y, heartRateDisplay.Width, heartRateDisplay.Height, LocaleTerms.Loc("Exit"), new Color?(CS$<>8__locals1.<>4__this.os.brightLockedColor)))
				{
					CS$<>8__locals1.<>4__this.os.display.command = "connect";
				}
			});
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0002CDD0 File Offset: 0x0002AFD0
		private void DrawLinedMessage(string message, Color col, Rectangle dest, SpriteBatch sb)
		{
			int num = 16;
			SpriteFont smallfont = GuiData.smallfont;
			Vector2 vector = smallfont.MeasureString(message);
			vector.X += (float)num;
			dest.Height = (int)(vector.Y + 0.5f);
			Rectangle destinationRectangle = new Rectangle(dest.X, dest.Y + dest.Height / 2 - 1, dest.Width / 2 - (int)(vector.X / 2f), 2);
			sb.Draw(Utils.white, destinationRectangle, col);
			sb.DrawString(smallfont, message, new Vector2((float)(destinationRectangle.X + destinationRectangle.Width + num / 2), (float)(destinationRectangle.Y - dest.Height / 2)), col);
			destinationRectangle.X = dest.X + dest.Width - destinationRectangle.Width;
			sb.Draw(Utils.white, destinationRectangle, col);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0002D2D8 File Offset: 0x0002B4D8
		private void DrawOptionsPanel(Rectangle bounds, SpriteBatch spritebatch)
		{
			int buttonWidth = 120;
			float num = (float)(bounds.Width - buttonWidth) * 0.7f;
			float num2 = Math.Min(1f, this.opTransition);
			if (!this.opOpening)
			{
				num2 = 1f - num2;
			}
			num2 = Utils.QuadraticOutCurve(num2);
			bool ShowingContent = num2 >= 0.98f;
			float contentFade = 0f;
			float num3 = 0.5f;
			if (ShowingContent)
			{
				contentFade = Math.Min(1f + num3, this.opTransition) - num3;
			}
			int num4 = (int)(num2 * num) + buttonWidth;
			Rectangle panelArea = new Rectangle(bounds.X + bounds.Width - num4, bounds.Y + bounds.Height / 10, num4, bounds.Height - bounds.Height / 5);
			Rectangle buttonSourceRect = panelArea;
			this.PostBloomDrawCalls.Add(delegate(int x, int y, SpriteBatch sprBatch)
			{
				Rectangle buttonSourceRect = buttonSourceRect;
				buttonSourceRect.X += x;
				buttonSourceRect.Y += y;
				int num5 = bounds.Height / 4 + 12;
				buttonSourceRect.Y += num5 - bounds.Height / 10;
				int num6 = 30;
				Rectangle destinationRectangle = buttonSourceRect;
				destinationRectangle.Width = buttonWidth;
				destinationRectangle.Height = num6;
				destinationRectangle.X -= 2;
				sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
				if (Button.doButton(83838001, destinationRectangle.X, destinationRectangle.Y, buttonWidth, num6, LocaleTerms.Loc("Login"), new Color?((this.State != HeartMonitorDaemon.HeartMonitorState.SecondaryLogin) ? this.os.highlightColor : Color.Black)))
				{
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLogin);
				}
				destinationRectangle.Y += num6 + 2;
				sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
				if (Button.doButton(83838003, destinationRectangle.X, destinationRectangle.Y, buttonWidth, num6, LocaleTerms.Loc("Firmware"), new Color?((this.State != HeartMonitorDaemon.HeartMonitorState.FirmwareScreen) ? this.os.highlightColor : Color.Black)))
				{
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
				}
				destinationRectangle.Y += num6 + 2;
				sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
				if (Button.doButton(83838006, destinationRectangle.X, destinationRectangle.Y, buttonWidth, num6, LocaleTerms.Loc("Monitor"), new Color?((this.State != HeartMonitorDaemon.HeartMonitorState.MainDisplay) ? this.os.highlightColor : Color.Black)))
				{
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.MainDisplay);
				}
				destinationRectangle.Y += num6 + 2;
				sprBatch.Draw(Utils.white, destinationRectangle, Utils.VeryDarkGray);
				if (Button.doButton(83838009, destinationRectangle.X, destinationRectangle.Y, buttonWidth, num6, LocaleTerms.Loc("Exit"), new Color?(this.os.lockedColor)))
				{
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.Welcome);
				}
				destinationRectangle.Y += num6 + 2;
				panelArea.X += x;
				panelArea.Y += y;
				panelArea.X += buttonWidth - 3;
				panelArea.Width -= buttonWidth;
				if (this.opOpening || this.opTransition < 1f)
				{
					panelArea.Width += 2;
				}
				sprBatch.Draw(Utils.white, panelArea, this.os.outlineColor);
				int num7 = 3;
				panelArea.X += num7;
				panelArea.Width -= 2 * num7;
				panelArea.Y += num7;
				panelArea.Height -= 2 * num7;
				sprBatch.Draw(Utils.white, panelArea, this.os.indentBackgroundColor);
				if (ShowingContent)
				{
					this.DrawOptionsPanelContent(panelArea, sprBatch, contentFade);
				}
			});
		}

		// Token: 0x06000315 RID: 789 RVA: 0x0002D438 File Offset: 0x0002B638
		private Rectangle DrawOptionsPanelHeaders(Rectangle bounds, SpriteBatch sb, float contentFade)
		{
			Rectangle rectangle = bounds;
			rectangle.Height = 30;
			rectangle.X += 2;
			rectangle.Width -= 4;
			rectangle.Y += 2;
			string message = LocaleTerms.Loc("Firmware Config");
			this.DrawLinedMessage(message, this.os.highlightColor * contentFade, rectangle, sb);
			rectangle.X -= 2;
			this.DrawLinedMessage(message, this.os.highlightColor * contentFade * 0.2f, rectangle, sb);
			rectangle.X += 4;
			this.DrawLinedMessage(message, this.os.highlightColor * contentFade * 0.2f, rectangle, sb);
			rectangle.X -= 2;
			rectangle.Y += rectangle.Height - 10;
			rectangle.Height = 54;
			string text = LocaleTerms.Loc("Firmware Administration") + "\n" + LocaleTerms.Loc("Access") + " : ";
			Color color = this.os.brightLockedColor;
			if (this.HasSecondaryLogin)
			{
				text = text + " " + LocaleTerms.Loc("GRANTED");
				color = this.os.brightUnlockedColor;
			}
			else
			{
				text = text + " " + LocaleTerms.Loc("DENIED");
			}
			color *= contentFade;
			sb.DrawString(GuiData.font, text, new Vector2((float)rectangle.X, (float)rectangle.Y), color, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 0.6f);
			rectangle.Y += rectangle.Height;
			sb.DrawString(GuiData.tinyfont, LocaleTerms.Loc("Secondary security layer for firmware read/write access"), new Vector2((float)rectangle.X, (float)rectangle.Y), color * 0.7f, 0f, Vector2.Zero, 0.9f, SpriteEffects.None, 0.6f);
			rectangle.Y += 15;
			rectangle.Height = 1;
			sb.Draw(Utils.white, rectangle, Color.Gray * contentFade * (float)(Utils.random.NextDouble() * 0.15 + 0.2800000011920929));
			Rectangle result = new Rectangle(bounds.X, rectangle.Y, bounds.Width, bounds.Y + bounds.Height - rectangle.Y - rectangle.Height);
			return result;
		}

		// Token: 0x06000316 RID: 790 RVA: 0x0002D6F4 File Offset: 0x0002B8F4
		private void DrawOptionsPanelContent(Rectangle bounds, SpriteBatch sb, float contentFade)
		{
			Rectangle bounds2 = this.DrawOptionsPanelHeaders(bounds, sb, contentFade);
			switch (this.State)
			{
			default:
				this.DrawOptionsPanelLoginContent(bounds2, sb, contentFade);
				break;
			case HeartMonitorDaemon.HeartMonitorState.Error:
				break;
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreen:
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
			case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
				this.DrawOptionsPanelFirmwareContent(bounds2, sb, contentFade);
				break;
			}
		}

		// Token: 0x06000317 RID: 791 RVA: 0x0002D754 File Offset: 0x0002B954
		private void DrawOptionsPanelFirmwareContent(Rectangle bounds, SpriteBatch sb, float contentFade)
		{
			Color color = this.heartColor;
			color.A = 0;
			if (!this.HasSecondaryLogin)
			{
				Rectangle dest = new Rectangle(bounds.X + bounds.Width / 4, bounds.Y, bounds.Width / 2, bounds.Height);
				TextItem.doFontLabelToSize(dest, string.Concat(new string[]
				{
					LocaleTerms.Loc("Firmware Administration"),
					"\n",
					LocaleTerms.Loc("Access Required"),
					"\n",
					LocaleTerms.Loc("Log In First")
				}), GuiData.font, color, false, false);
			}
			else
			{
				switch (this.State)
				{
				default:
				{
					Folder folder = this.comp.files.root.searchForFolder("KBT_Pacemaker");
					int num = folder.files.Count + 1;
					string[] array = new string[num];
					array[0] = LocaleTerms.Loc("Currently Active Firmware");
					if (folder.files.Count > 0)
					{
						for (int i = 0; i < folder.files.Count; i++)
						{
							array[i + 1] = folder.files[i].name;
						}
					}
					this.selectedFirmwareIndex = SelectableTextList.doFancyList(8937001, bounds.X + 2, bounds.Y + 10, (int)((float)bounds.Width - 4f), bounds.Height / 3, array, this.selectedFirmwareIndex, new Color?(this.os.topBarColor), false);
					Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 10 + bounds.Height / 3 + 4, bounds.Width - 4, bounds.Height / 4);
					string data = (this.selectedFirmwareIndex != 0) ? folder.files[this.selectedFirmwareIndex - 1].data : null;
					string filename = (this.selectedFirmwareIndex != 0) ? folder.files[this.selectedFirmwareIndex - 1].name : LocaleTerms.Loc("Currently Active Firmware");
					bool flag = this.DrawSelectedFirmwareFileDetails(rectangle, sb, data, filename);
					rectangle.Y += rectangle.Height + 6;
					if (flag && this.selectedFirmwareIndex != 0)
					{
						rectangle.Height = 30;
						if (!this.isConfirmingSelection)
						{
							if (Button.doButton(8937004, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Activate This Firmware"), new Color?(this.os.highlightColor)))
							{
								this.isConfirmingSelection = true;
							}
						}
						else
						{
							this.DrawLinedMessage(LocaleTerms.Loc("Confirm Firmware Activation"), this.os.brightLockedColor, rectangle, sb);
							rectangle.Y += rectangle.Height;
							if (Button.doButton(8937008, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Confirm Activation"), new Color?(this.os.highlightColor)))
							{
								this.selectedFirmwareName = filename;
								this.selectedFirmwareData = data;
								this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading);
								this.firmwareLoadTime = 0f;
								this.isConfirmingSelection = false;
							}
							rectangle.Y += rectangle.Height + 4;
							rectangle.Height = 20;
							if (Button.doButton(8937009, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, LocaleTerms.Loc("Cancel"), new Color?(this.os.lockedColor)))
							{
								this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
								this.isConfirmingSelection = false;
							}
						}
					}
					break;
				}
				case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenConfirm:
					break;
				case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenLoading:
				{
					Rectangle bounds2 = bounds;
					bounds2.X++;
					bounds2.Width -= 2;
					bounds2.Height = 110;
					this.DrawSelectedFirmwareFileDetails(bounds2, sb, this.selectedFirmwareData, this.selectedFirmwareName);
					bounds2.Y += bounds2.Height + 2;
					Rectangle rectangle2 = bounds;
					int num2 = bounds2.Height + 10;
					rectangle2.Height -= num2;
					rectangle2.Y += num2;
					Rectangle destinationRectangle = rectangle2;
					destinationRectangle.Height = 1;
					float num3 = this.firmwareLoadTime / 10f;
					color.A = 0;
					for (int i = 0; i < rectangle2.Height; i += 3)
					{
						float num4 = (float)i / (float)rectangle2.Height;
						if (num3 > num4)
						{
							destinationRectangle.Y = rectangle2.Y + i;
							sb.Draw(Utils.white, destinationRectangle, color);
						}
					}
					break;
				}
				case HeartMonitorDaemon.HeartMonitorState.FirmwareScreenComplete:
				{
					Rectangle rectangle3 = bounds;
					rectangle3.X += 2;
					rectangle3.Width -= 4;
					rectangle3.Y += 2;
					rectangle3.Height -= 4;
					sb.Draw(Utils.white, rectangle3, this.os.brightLockedColor * 0.3f * contentFade);
					rectangle3.X += 40;
					rectangle3.Width -= 80;
					TextItem.doFontLabelToSize(rectangle3, LocaleTerms.Loc("FIRMWARE UPDATE COMPLETE"), GuiData.font, Color.White, false, false);
					break;
				}
				}
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x0002DD40 File Offset: 0x0002BF40
		private bool DrawSelectedFirmwareFileDetails(Rectangle bounds, SpriteBatch sb, string data, string filename)
		{
			bool flag = data == null || this.IsValidFirmwareData(data);
			Rectangle dest = bounds;
			dest.Height = 28;
			this.DrawLinedMessage(flag ? LocaleTerms.Loc("Valid Firmware File") : LocaleTerms.Loc("Invalid Firmware File"), flag ? this.heartColor : this.os.brightLockedColor, dest, sb);
			dest.Y += dest.Height - 4;
			Color value = Color.White;
			if (!flag)
			{
				value = Color.Gray;
			}
			TextItem.doFontLabel(new Vector2((float)dest.X + 6f, (float)dest.Y), filename, GuiData.font, new Color?(value), (float)dest.Width, (float)dest.Height, false);
			dest.Y += dest.Height + 2;
			dest.Height = 14;
			string text = string.Concat(new string[]
			{
				LocaleTerms.Loc("Invalid binary package"),
				"\n",
				LocaleTerms.Loc("Valid firmware packages must be"),
				"\n",
				LocaleTerms.Loc("digitally signed by an authorized manufacturer")
			});
			if (flag)
			{
				text = string.Concat(new string[]
				{
					LocaleTerms.Loc("Valid binary package"),
					"\n",
					LocaleTerms.Loc("Signed by"),
					" : KELLIS BIOTECH\n",
					LocaleTerms.Loc("Compiled by"),
					" : EIDOLON SOFT"
				});
			}
			TextItem.doFontLabel(new Vector2((float)dest.X + 6f, (float)dest.Y), text, GuiData.detailfont, new Color?(value * 0.7f), (float)(dest.Width - 12), float.MaxValue, false);
			Rectangle destinationRectangle = bounds;
			destinationRectangle.Y += destinationRectangle.Height - 1;
			destinationRectangle.Height = 1;
			sb.Draw(Utils.white, destinationRectangle, Color.Gray);
			return flag;
		}

		// Token: 0x06000319 RID: 793 RVA: 0x0002DF5C File Offset: 0x0002C15C
		private bool IsValidFirmwareData(string data)
		{
			return data == PortExploits.DangerousPacemakerFirmware || data == PortExploits.ValidPacemakerFirmware;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x0002DFD4 File Offset: 0x0002C1D4
		private void DrawOptionsPanelLoginContent(Rectangle bounds, SpriteBatch sb, float contentFade)
		{
			string text = LocaleTerms.Loc("A secondary login is required to review and modify running firmware. Personal login details are provided for each chip. If you have lost your login details, connect your support program to our content server") + " (111.105.22.1)";
			Rectangle rectangle = new Rectangle(bounds.X + 2, bounds.Y + 10, bounds.Width - 4, 24);
			text = Utils.SuperSmartTwimForWidth(text, rectangle.Width, GuiData.detailfont);
			sb.DrawString(GuiData.detailfont, text, new Vector2((float)rectangle.X, (float)rectangle.Y), Color.Gray * contentFade);
			Vector2 vector = GuiData.detailfont.MeasureString(text);
			rectangle.Y += (int)(vector.Y + 10f);
			if (this.State == HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning || this.HasSecondaryLogin)
			{
				Rectangle rectangle2 = rectangle;
				rectangle2.Height = bounds.Height / 4;
				int height = rectangle2.Height;
				if (this.loginUsername != null)
				{
					rectangle2.Height = 60;
					GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Username") + " : ", this.loginUsername, rectangle2, sb, this.os, "");
					rectangle2.Y += rectangle2.Height + 2;
				}
				if (this.loginPass != null)
				{
					rectangle2.Height = 60;
					GetStringUIControl.DrawGetStringControlInactive(LocaleTerms.Loc("Password") + " : ", this.loginPass, rectangle2, sb, this.os, "");
					rectangle2.Y += rectangle2.Height + 2;
				}
				rectangle2.Height = height;
				if (this.loginPass == null || this.loginUsername == null)
				{
					string text2 = GetStringUIControl.DrawGetStringControl((this.loginUsername == null) ? (LocaleTerms.Loc("Username") + " : ") : (LocaleTerms.Loc("Password") + " : "), rectangle2, delegate
					{
						this.loginUsername = (this.loginPass = "");
					}, delegate
					{
						this.loginUsername = (this.loginPass = "");
					}, sb, this.os, this.os.highlightColor, this.os.lockedColor, "", null);
					if (text2 != null)
					{
						if (this.loginUsername == null)
						{
							this.loginUsername = text2;
							GetStringUIControl.StartGetString(LocaleTerms.Loc("Password"), this.os);
						}
						else
						{
							this.loginPass = text2;
						}
					}
					rectangle2.Y += rectangle2.Height + 10;
				}
				else
				{
					rectangle2.Y += 20;
					if (this.loginUsername == "EAdmin" && this.loginPass == "tens86")
					{
						this.HasSecondaryLogin = true;
					}
					else
					{
						this.HasSecondaryLogin = false;
					}
					rectangle2.Height = 20;
					Color col = this.HasSecondaryLogin ? this.os.brightUnlockedColor : this.os.brightLockedColor;
					this.DrawLinedMessage(this.HasSecondaryLogin ? LocaleTerms.Loc("Login Complete") : LocaleTerms.Loc("Login Failed"), col, rectangle2, sb);
					rectangle2.Y += rectangle2.Height + 20;
					if (!this.HasSecondaryLogin)
					{
						if (Button.doButton(92923008, rectangle2.X + 4, rectangle2.Y, rectangle2.Width - 80, 24, LocaleTerms.Loc("Retry Login"), null))
						{
							this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLogin);
						}
					}
					else if (Button.doButton(92923009, rectangle2.X + 4, rectangle2.Y, rectangle2.Width - 80, 24, LocaleTerms.Loc("Administrate Firmware"), new Color?(this.os.highlightColor)))
					{
						this.ChangeState(HeartMonitorDaemon.HeartMonitorState.FirmwareScreen);
					}
				}
			}
			else if (!this.HasSecondaryLogin)
			{
				if (this.ButtonFlashForContentFade(contentFade) && Button.doButton(92923001, rectangle.X, rectangle.Y, (int)((float)rectangle.Width * 0.7f), 30, LocaleTerms.Loc("Login"), new Color?(this.os.highlightColor)))
				{
					this.loginUsername = (this.loginPass = null);
					GetStringUIControl.StartGetString(LocaleTerms.Loc("Username"), this.os);
					this.ChangeState(HeartMonitorDaemon.HeartMonitorState.SecondaryLoginRunning);
				}
			}
		}

		// Token: 0x0600031B RID: 795 RVA: 0x0002E4C4 File Offset: 0x0002C6C4
		private bool ButtonFlashForContentFade(float contentFade)
		{
			return contentFade > 0.95f || Utils.random.NextDouble() > (double)contentFade;
		}

		// Token: 0x04000340 RID: 832
		private const string ActiveFirmwareFilename = "LiveFirmware.dll";

		// Token: 0x04000341 RID: 833
		private const string FolderName = "KBT_Pacemaker";

		// Token: 0x04000342 RID: 834
		private const string LiveFolderName = "Active";

		// Token: 0x04000343 RID: 835
		private const string SubLoginUsername = "EAdmin";

		// Token: 0x04000344 RID: 836
		private const string SubLoginPass = "tens86";

		// Token: 0x04000345 RID: 837
		private const float MinFirmwareLoadTime = 10f;

		// Token: 0x04000346 RID: 838
		private const float TimeToDeathInDanger = 21f;

		// Token: 0x04000347 RID: 839
		private const float DyingDangerTime = 6f;

		// Token: 0x04000348 RID: 840
		private const float DeadBeepSustainFadeOut = 16f;

		// Token: 0x04000349 RID: 841
		private const float DeadBeepSustainFadeoutStartDelay = 5f;

		// Token: 0x0400034A RID: 842
		private Texture2D Heart;

		// Token: 0x0400034B RID: 843
		private Texture2D OxyIcon;

		// Token: 0x0400034C RID: 844
		private Texture2D WarnIcon;

		// Token: 0x0400034D RID: 845
		private Effect blufEffect;

		// Token: 0x0400034E RID: 846
		private RenderTarget2D bloomTarget;

		// Token: 0x0400034F RID: 847
		private RenderTarget2D priorTarget;

		// Token: 0x04000350 RID: 848
		private RenderTarget2D secondaryBloomTarget;

		// Token: 0x04000351 RID: 849
		private List<Action<int, int, SpriteBatch>> PostBloomDrawCalls = new List<Action<int, int, SpriteBatch>>();

		// Token: 0x04000352 RID: 850
		private SpriteBatch BlurContentSpritebatch;

		// Token: 0x04000353 RID: 851
		private Color bloomColor = new Color(10, 10, 10, 0);

		// Token: 0x04000354 RID: 852
		private Color heartColor = new Color(247, 237, 125);

		// Token: 0x04000355 RID: 853
		private bool HasSecondaryLogin = false;

		// Token: 0x04000356 RID: 854
		public string PatientID = "UNKNOWN";

		// Token: 0x04000357 RID: 855
		private bool PatientInCardiacArrest = false;

		// Token: 0x04000358 RID: 856
		private bool PatientDead = false;

		// Token: 0x04000359 RID: 857
		private float PatientTimeInDanger = 0f;

		// Token: 0x0400035A RID: 858
		private float timeDead = 0f;

		// Token: 0x0400035B RID: 859
		private float timeTillNextHeartbeat = 0f;

		// Token: 0x0400035C RID: 860
		private float timeBetweenHeartbeats = 0.88235295f;

		// Token: 0x0400035D RID: 861
		private float beatTime = 0.18f;

		// Token: 0x0400035E RID: 862
		private SoundEffect beepSound;

		// Token: 0x0400035F RID: 863
		private SoundEffectInstance beepSustainSound;

		// Token: 0x04000360 RID: 864
		private float volume = 0.4f;

		// Token: 0x04000361 RID: 865
		private float opTransition = 0f;

		// Token: 0x04000362 RID: 866
		private bool opOpening = false;

		// Token: 0x04000363 RID: 867
		private string loginUsername = null;

		// Token: 0x04000364 RID: 868
		private string loginPass = null;

		// Token: 0x04000365 RID: 869
		private float firmwareLoadTime = 0f;

		// Token: 0x04000366 RID: 870
		private int selectedFirmwareIndex = 0;

		// Token: 0x04000367 RID: 871
		private bool isConfirmingSelection = false;

		// Token: 0x04000368 RID: 872
		private string selectedFirmwareName = "";

		// Token: 0x04000369 RID: 873
		private string selectedFirmwareData = "";

		// Token: 0x0400036A RID: 874
		private BasicMedicalMonitor HeartMonitor;

		// Token: 0x0400036B RID: 875
		private BasicMedicalMonitor BPMonitor;

		// Token: 0x0400036C RID: 876
		private BasicMedicalMonitor SPMonitor;

		// Token: 0x0400036D RID: 877
		private List<IMedicalMonitor> Monitors = new List<IMedicalMonitor>();

		// Token: 0x0400036E RID: 878
		private float projectionFowardsTime = 0.3f;

		// Token: 0x0400036F RID: 879
		private float timeSinceLastHeartBeat = float.MaxValue;

		// Token: 0x04000370 RID: 880
		private int HeartRate = 0;

		// Token: 0x04000371 RID: 881
		private float currentSPO2 = 0f;

		// Token: 0x04000372 RID: 882
		private float averageSPO2 = 0f;

		// Token: 0x04000373 RID: 883
		private int reportedSP02 = 95;

		// Token: 0x04000374 RID: 884
		private float timeSinceNormalHeartRate = 0f;

		// Token: 0x04000375 RID: 885
		private float alarmHeartOKTimer = 0f;

		// Token: 0x04000376 RID: 886
		private HeartMonitorDaemon.HeartMonitorState State = HeartMonitorDaemon.HeartMonitorState.Welcome;

		// Token: 0x04000377 RID: 887
		private float timeThisState = 0f;

		// Token: 0x0200009A RID: 154
		private enum HeartMonitorState
		{
			// Token: 0x0400037B RID: 891
			Welcome,
			// Token: 0x0400037C RID: 892
			MainDisplay,
			// Token: 0x0400037D RID: 893
			SecondaryLogin,
			// Token: 0x0400037E RID: 894
			SecondaryLoginRunning,
			// Token: 0x0400037F RID: 895
			Error,
			// Token: 0x04000380 RID: 896
			FirmwareScreen,
			// Token: 0x04000381 RID: 897
			FirmwareScreenConfirm,
			// Token: 0x04000382 RID: 898
			FirmwareScreenLoading,
			// Token: 0x04000383 RID: 899
			FirmwareScreenComplete
		}
	}
}
