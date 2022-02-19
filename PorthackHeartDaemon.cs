using System;
using Hacknet.Effects;
using Hacknet.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Hacknet
{
	// Token: 0x02000096 RID: 150
	internal class PorthackHeartDaemon : Daemon
	{
		// Token: 0x060002E7 RID: 743 RVA: 0x0002949C File Offset: 0x0002769C
		public PorthackHeartDaemon(Computer c, OS os) : base(c, "Porthack.Heart", os)
		{
			this.name = "Porthack.Heart";
			this.SpinDownEffect = os.content.Load<SoundEffect>("SFX/TraceKill");
			this.glowSoundEffect = os.content.Load<SoundEffect>("SFX/Ending/PorthackSpindown");
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x00029535 File Offset: 0x00027735
		public override void navigatedTo()
		{
			base.navigatedTo();
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x000295A4 File Offset: 0x000277A4
		private void UpdateForTime(Rectangle bounds, SpriteBatch sb)
		{
			if (this.playTimeExpended > this.FadeoutDelay)
			{
				float fade = Math.Min(1f, (this.playTimeExpended - this.FadeoutDelay) / this.FadeoutDuration);
				Rectangle correctedbounds = new Rectangle(bounds.X, bounds.Y - Module.PANEL_HEIGHT, bounds.Width, bounds.Height + Module.PANEL_HEIGHT);
				OS os = this.os;
				os.postFXDrawActions = (Action)Delegate.Combine(os.postFXDrawActions, new Action(delegate()
				{
					Utils.FillEverywhereExcept(correctedbounds, this.os.fullscreen, sb, Color.Black * fade * 0.8f);
				}));
			}
			if (this.pcs.HeartFadeSequenceComplete)
			{
				this.IsFlashingOut = true;
				this.flashOutTime += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				float num = 3.8f;
				if (this.flashOutTime > num)
				{
					this.flashOutTime = num;
					this.os.canRunContent = false;
					this.os.endingSequence.IsActive = true;
					PostProcessor.EndingSequenceFlashOutActive = false;
					PostProcessor.EndingSequenceFlashOutPercentageComplete = 0f;
					return;
				}
				PostProcessor.EndingSequenceFlashOutPercentageComplete = this.flashOutTime / num;
			}
			else
			{
				this.IsFlashingOut = false;
			}
			PostProcessor.EndingSequenceFlashOutActive = this.IsFlashingOut;
		}

		// Token: 0x060002EA RID: 746 RVA: 0x00029734 File Offset: 0x00027934
		public void BreakHeart()
		{
			if (this.os.TraceDangerSequence.IsActive)
			{
				this.os.TraceDangerSequence.CancelTraceDangerSequence();
			}
			this.os.RequestRemovalOfAllPopups();
			this.PlayingHeartbreak = true;
			this.os.terminal.inputLocked = true;
			this.os.netMap.inputLocked = true;
			this.os.ram.inputLocked = true;
			this.os.DisableTopBarButtons = true;
			MusicManager.transitionToSong("Music/Ambient/AmbientDrone_Clipped");
			this.SpinDownEffect.Play();
			this.os.delayer.Post(ActionDelayer.Wait(18.0), delegate
			{
				this.glowSoundEffect.Play();
			});
		}

		// Token: 0x060002EB RID: 747 RVA: 0x00029800 File Offset: 0x00027A00
		public override void draw(Rectangle bounds, SpriteBatch sb)
		{
			base.draw(bounds, sb);
			try
			{
				int width = bounds.Width;
				int height = bounds.Height;
				if (this.rendertarget == null || this.rendertarget.Width != width || this.rendertarget.Height != height)
				{
					if (this.rtSpritebatch == null)
					{
						this.rtSpritebatch = new SpriteBatch(sb.GraphicsDevice);
					}
					if (this.rendertarget != null)
					{
						this.rendertarget.Dispose();
					}
					this.rendertarget = new RenderTarget2D(sb.GraphicsDevice, width, height);
				}
				if (!this.PlayingHeartbreak)
				{
					TextItem.DrawShadow = false;
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 6), (float)(bounds.Y + 2)), Utils.FlipRandomChars("PortHack.Heart", 0.003), GuiData.font, new Color?(Utils.AddativeWhite * 0.6f), (float)(bounds.Width - 10), 100f, false);
					TextItem.doFontLabel(new Vector2((float)(bounds.X + 6), (float)(bounds.Y + 2)), Utils.FlipRandomChars("PortHack.Heart", 0.1), GuiData.font, new Color?(Utils.AddativeWhite * 0.2f), (float)(bounds.Width - 10), 100f, false);
				}
				if (this.PlayingHeartbreak)
				{
					this.playTimeExpended += (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds;
				}
				this.UpdateForTime(bounds, sb);
				RenderTarget2D currentRenderTarget = Utils.GetCurrentRenderTarget();
				sb.GraphicsDevice.SetRenderTarget(this.rendertarget);
				sb.GraphicsDevice.Clear(Color.Transparent);
				this.rtSpritebatch.Begin();
				Rectangle dest = new Rectangle(0, 0, bounds.Width, bounds.Height);
				Vector3 value = new Vector3(MathHelper.ToRadians(35.4f), MathHelper.ToRadians(45f), MathHelper.ToRadians(0f));
				Vector3 vector = new Vector3(1f, 1f, 0f) * this.os.timer * 0.2f + new Vector3(this.os.timer * 0.1f, this.os.timer * -0.4f, 0f);
				float num = 2.5f;
				if (this.PlayingHeartbreak)
				{
					if (this.playTimeExpended < num)
					{
						vector = Vector3.Lerp(Utils.NormalizeRotationVector(vector), value, Utils.QuadraticOutCurve(this.playTimeExpended / num));
						Cube3D.RenderWireframe(Vector3.Zero, 2.6f, vector, Color.White);
					}
					else
					{
						this.pcs.DrawHeartSequence(dest, (float)this.os.lastGameTime.ElapsedGameTime.TotalSeconds, 30f);
					}
				}
				else
				{
					Cube3D.RenderWireframe(new Vector3(0f, 0f, 0f), 2.6f, vector, Color.White);
				}
				this.rtSpritebatch.End();
				sb.GraphicsDevice.SetRenderTarget(currentRenderTarget);
				Rectangle rectangle = new Rectangle(bounds.X + (bounds.Width - width) / 2, bounds.Y + (bounds.Height - height) / 2, width, height);
				float rarity = Math.Min(1f, this.playTimeExpended / num * 0.8f + 0.2f);
				FlickeringTextEffect.DrawFlickeringSprite(sb, rectangle, this.rendertarget, 4f, rarity, this.os, Color.White);
				sb.Draw(this.rendertarget, rectangle, Utils.AddativeWhite * 0.7f);
			}
			catch (Exception ex)
			{
				Console.WriteLine(Utils.GenerateReportFromException(ex));
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x00029C24 File Offset: 0x00027E24
		public override string getSaveString()
		{
			return "<porthackheart name=\"" + this.name + "\"/>";
		}

		// Token: 0x04000320 RID: 800
		private RenderTarget2D rendertarget;

		// Token: 0x04000321 RID: 801
		private SpriteBatch rtSpritebatch;

		// Token: 0x04000322 RID: 802
		private float playTimeExpended = 0f;

		// Token: 0x04000323 RID: 803
		private bool PlayingHeartbreak = false;

		// Token: 0x04000324 RID: 804
		private PortHackCubeSequence pcs = new PortHackCubeSequence();

		// Token: 0x04000325 RID: 805
		private float FadeoutDelay = 1f;

		// Token: 0x04000326 RID: 806
		private float FadeoutDuration = 10f;

		// Token: 0x04000327 RID: 807
		private bool IsFlashingOut = false;

		// Token: 0x04000328 RID: 808
		private float flashOutTime = 0f;

		// Token: 0x04000329 RID: 809
		private SoundEffect SpinDownEffect;

		// Token: 0x0400032A RID: 810
		private SoundEffect glowSoundEffect;
	}
}
